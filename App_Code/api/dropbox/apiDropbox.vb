Imports Microsoft.VisualBasic
Imports System.Data
Imports System.IO

''' <summary>
''' Contains all of the general configuration and static functions pertaining the Dropbox
''' Web API and our application of it within FriedParts
''' </summary>
''' <remarks></remarks>
Public Module apiDropbox

    ''' <summary>
    ''' Controls whether this deployment of FriedParts is in production status at Dropbox.
    ''' </summary>
    ''' <remarks>Controls whether two (TRUE) or four (FALSE) keys are used to login each user and whether the additional signup fields and instructions are displayed on the webpage.</remarks>
    Public Const DropboxInProductionStatus As Boolean = True

    '== PUBLIC DATA STRUCTURES
    '=======================================================

    Public Enum DropboxErrors As Byte
        Successful
        InvalidPassword
        NoCredentialsOnFile
        InvalidTokens
    End Enum

    Public Enum DropboxStates As Byte
        Idle = 0
        MetaAnalysis = 1
        Uploading = 2
        UploadComplete = 3
        UploadFailed = 4
        Downloading = 5
        DownloadComplete = 6
        DownloadFailed = 7
        ErrorOccured = 8
        ErrorUnauthorizedChanges = 9
        ErrorMissingMetaData = 10
    End Enum

    Public Const apiDropboxRoot As String = "/FriedParts/"

    '== DATA STORAGE CLASSES
    '=======================================================


    '== PUBLIC FUNCTIONS
    '=======================================================

    'Get the File's FileID
    Public Function apiDropboxGetFileID(ByRef DirectoryPath As String, ByRef Filename As String) As Int32
        Dim str As String = _
            "SELECT [FileID]" & _
            "      ,[DropboxPath]" & _
            "      ,[Name]" & _
            "  FROM [FriedParts].[dbo].[files-Common] " & _
            " WHERE [DropboxPath] = '" & DirectoryPath & "' " & _
            "   AND [Name] = '" & Filename & "'"
        Dim dt As New DataTable
        SelectRows(dt, str)
        If dt.Rows.Count <> 1 Then
            Throw New FileNotFoundException("The specified file ( " & DirectoryPath & Filename & " ) does not have a server metadata entry.")
        End If
        Return dt.Rows(0).Field(Of Integer)("FileID")
    End Function

    'Save the User's tokens to the database
    Public Sub apiDropboxSaveUserCredentials(ByRef UserID As Integer, ByRef Tokens As DropboxUserCredentials)
        'Confirm user exists
        sysUser.suGetUsername(UserID) 'Don't care, but this will throw error if invalid UserID -- just for safety (b/c query will fail silently)

        'If successful store token
        Dim sqltxt As String = _
            "UPDATE [FriedParts].[dbo].[user-Accounts]" & _
            "   SET [DropboxUserKey] = '" & txtDefangSQL(Tokens.getUserKey) & "', " & _
            "       [DropboxUserSecret] = '" & txtDefangSQL(Tokens.getUserSecret) & "', " & _
            "       [DropboxAppKey] = '" & txtDefangSQL(Tokens.getAppKey) & "', " & _
            "       [DropboxAppSecret] = '" & txtDefangSQL(Tokens.getAppSecret) & "' " & _
            " WHERE [UserID] = " & UserID
        dbAcc.SQLexe(sqltxt)
    End Sub

    'Logs in the Dropbox user
    Public Function loginDropboxUser(ByRef UserID As Integer) As DropboxErrors
        'Check if values are currently on file
        Dim Tokens As DropboxUserCredentials
        Tokens = getUserToken(UserID)
        If Tokens Is Nothing Then
            'No credentials on file... :'(
            Return DropboxErrors.NoCredentialsOnFile
        End If

        'Test against Dropbox
        Dim blah As DropNet.DropNetClient = getDropboxClient(Tokens)
        If blah.Account_Info().display_name Is Nothing Then
            'Token on file is no longer valid... :'(
            Return DropboxErrors.InvalidTokens
        End If

        'Return
        Return DropboxErrors.Successful
    End Function

    'Search the User's dropbox for a specific filename. 
    'Due to the way the Dropbox API works (for efficiency), this search works on only one directory at a time
    'Returns the file's MetaData structure
    Public Function apiDropboxFindFile(ByRef Filename As String, ByVal Contents As System.Collections.Generic.List(Of DropNet.Models.MetaData)) As DropNet.Models.MetaData
        For Each TheFile As DropNet.Models.MetaData In Contents
            If TheFile.Name.CompareTo(Filename) = 0 Then
                'Found it
                Return TheFile
            End If
        Next
        Return Nothing
    End Function

    ''' <summary>
    ''' Log Dropbox File Activity
    ''' ---Defaults to current user
    ''' </summary>
    ''' <param name="ActivityType"></param>
    ''' <param name="Filename"></param>
    ''' <param name="Path"></param>
    ''' <param name="Message"></param>
    ''' <remarks></remarks>
    Public Sub logDropbox(ByRef ActivityType As DropboxStates, ByRef Filename As String, ByRef Path As String, Optional ByRef Message As String = "")
        logDropbox(suGetUserID(), ActivityType, Filename, Path, Message)
    End Sub

    'Log Dropbox File Activity
    '---Full parameter list (can specify/assign user -- for background/offline use purposes)
    Public Sub logDropbox(ByRef UserID As Integer, ByRef ActivityType As DropboxStates, ByRef Filename As String, ByRef Path As String, Optional ByRef Message As String = "")
        Dim sqltxt As String = _
            "INSERT INTO [FriedParts].[dbo].[log-Dropbox]" & _
            "           ([UserID]" & _
            "           ,[Filename]" & _
            "           ,[Path]" & _
            "           ,[Operation]" & _
            "           ,[TimestampDate]" & _
            "           ,[Message])" & _
            "     VALUES (" & _
            "           " & UserID & "," & _
            "           '" & txtDefangSQL(Filename) & "'," & _
            "           '" & txtDefangSQL(Path) & "'," & _
            "           " & ActivityType & "," & _
            "           " & txtSqlDate(Now) & "," & _
            "           '" & txtDefangSQL(Message) & "'" & _
            "           )"
        Try
            SQLexe(sqltxt)
        Catch ex As System.Data.SqlClient.SqlException
            Throw New EntitySqlException(ex.Message & " --- " & sqltxt, ex)
        End Try
    End Sub



    '== PRIVATE FUNCTIONS
    '=======================================================

    'For use now with our workaround
    Private Function getDropboxClient(ByRef UserTokens As DropboxUserCredentials) As DropNet.DropNetClient
        Return New DropNet.DropNetClient(UserTokens.getAppKey, UserTokens.getAppSecret, UserTokens.getUserKey, UserTokens.getUserSecret)
    End Function

    'Returns the Dropbox Authentication Token for the user "UserID" and Nothing if the token is not on file
    Private Function getUserToken(ByRef UserID As Integer) As DropboxUserCredentials
        'Check database
        Dim sqltxt As String = _
            "SELECT " & _
            "   [UserID], " & _
            "   [DropboxUserKey], " & _
            "   [DropboxUserSecret], " & _
            "   [DropboxAppKey], " & _
            "   [DropboxAppSecret] " & _
            "FROM [FriedParts].[dbo].[user-Accounts] " & _
            "WHERE [UserID] = " & UserID
        Dim dt As New DataTable
        SelectRows(dt, sqltxt)

        'If not present, error!
        If dt.Rows.Count <> 1 Then
            Err.Raise(-2343, , "UserID not found or not unique! This is really bad!")
        End If

        'If no token on file...
        If IsNothing(dt.Rows(0).Field(Of String)("DropboxUserKey")) _
            Or IsNothing(dt.Rows(0).Field(Of String)("DropboxUserSecret")) _
            Or IsNothing(dt.Rows(0).Field(Of String)("DropboxAppKey")) _
            Or IsNothing(dt.Rows(0).Field(Of String)("DropboxAppSecret")) _
        Then
            Return Nothing
        End If

        'All good! Return!
        Dim RetVal As New DropboxUserCredentials( _
                                                    dt.Rows(0).Field(Of String)("DropboxUserKey"), _
                                                    dt.Rows(0).Field(Of String)("DropboxUserSecret"), _
                                                    dt.Rows(0).Field(Of String)("DropboxAppKey"), _
                                                    dt.Rows(0).Field(Of String)("DropboxAppSecret") _
                                                )
        Return RetVal
    End Function



    
End Module
