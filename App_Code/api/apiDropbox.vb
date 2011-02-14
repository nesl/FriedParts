Imports Microsoft.VisualBasic
Imports System.Data
Imports System.IO

Public Module apiDropbox

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

    ''' <summary>
    ''' The class is just a convenient container for storing the four keys of a dropbox access token
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DropboxUserCredentials
        Private uToken As String
        Private uSecret As String
        Private aToken As String
        Private aSecret As String
        Public ReadOnly Property getUserKey As String
            Get
                Return uToken
            End Get
        End Property
        Public ReadOnly Property getUserSecret As String
            Get
                Return uSecret
            End Get
        End Property
        Public ReadOnly Property getAppKey As String
            Get
                Return aToken
            End Get
        End Property
        Public ReadOnly Property getAppSecret As String
            Get
                Return aSecret
            End Get
        End Property
        Public Sub New(ByVal userToken As String, ByVal userSecret As String, Optional ByVal appToken As String = "", Optional ByVal appSecret As String = "")
            uToken = userToken
            uSecret = userSecret
            aToken = appToken
            aSecret = appSecret
        End Sub
    End Class

    Public Class DropboxFile
        'Private Internal Data
        '=====================
        Private dr As DataRow
        Private theFileID As Integer
        ''' <summary>
        ''' The UserID of the person requesting information about this DropboxFile.
        ''' </summary>
        ''' <remarks>UserID is important because we can't check for changes without knowing which Dropbox account we are working with.</remarks>
        Private whosAskingUserID As Int32

        'Public Properties
        '=================
        ''' <summary>
        ''' The UserID of the person requesting information about this DropboxFile. 
        ''' UserID is important because we can't check for changes without knowing which Dropbox account we are working with.
        ''' </summary>
        ''' <value></value>
        ''' <returns>The UserID of the person requestion information about this Dropbox File</returns>
        ''' <remarks>UserID is important because we can't check for changes without knowing which Dropbox account we are working with.</remarks>
        Public ReadOnly Property GetUserID As Int32
            Get
                Return whosAskingUserID
            End Get
        End Property

        Public Property DropboxTimeStamp As Date
            Get
                Dim str As String = _
                   "SELECT [FileID]" & _
                   "      ,[Version]" & _
                   "      ,[UserID]" & _
                   "      ,[Modified]" & _
                   "  FROM [FriedParts].[dbo].[files-MetaByUser]" & _
                   " WHERE [FileID] = " & theFileID & _
                   "   AND [UserID] = " & GetUserID() & _
                   " ORDER BY [Modified] DESC"
                Dim dt As New DataTable
                SelectRows(dt, str)
                If dt.Rows.Count = 0 Then
                    Throw New FileNotFoundException("Undefined version for FileID " & theFileID & " and UserID " & GetUserID())
                End If
                Return dt.Rows(0).Field(Of Date)("Modified")
            End Get
            Set(ByVal value As Date)
                'Var
                Dim theVersion As Int32
                'Deal with Version
                theVersion = GetVersion
                If IamTheOwner Then
                    'Increment version
                    theVersion = theVersion + 1
                End If

                'Write the UserMetaData (record new dropbox state)
                Dim str As String = _
                    "INSERT INTO [FriedParts].[dbo].[files-MetaByUser]" & _
                    "           ([FileID]" & _
                    "           ,[Version]" & _
                    "           ,[UserID]" & _
                    "           ,[Modified])" & _
                    "     VALUES (" & _
                    "            " & theFileID & "," & _
                    "            " & theVersion & "," & _
                    "            " & GetUserID() & "," & _
                    "            " & txtSqlDate(value) & "" & _
                    "           )"
                SQLexe(str)
            End Set
        End Property

        Public ReadOnly Property GetVersion As Int32
            Get
                Dim str As String = _
                    "SELECT [FileID]" & _
                    "      ,[Version]" & _
                    "      ,[UserID]" & _
                    "      ,[Modified]" & _
                    "  FROM [FriedParts].[dbo].[files-MetaByUser]" & _
                    " WHERE [FileID] = " & theFileID & _
                    "   AND [UserID] = " & GetOwnerID & _
                    " ORDER BY [Modified] DESC"
                Dim dt As New DataTable
                SelectRows(dt, str)
                If dt.Rows.Count = 0 Then
                    Throw New FileNotFoundException("Undefined version for FileID " & theFileID)
                End If
                Return dt.Rows(0).Field(Of Int32)("Version")
            End Get
        End Property

        Public ReadOnly Property GetOwnerID As Integer
            Get
                Return dr.Field(Of Integer)("OwnerID")
            End Get
        End Property

        Public ReadOnly Property GetOwnerFullName As String
            Get
                Return sysUser.suGetUsername(GetOwnerID)
            End Get
        End Property

        Public ReadOnly Property GetOwnerFirstName As String
            Get
                Return sysUser.suGetUserFirstName(GetOwnerID)
            End Get
        End Property

        'Returns true if the currently logged in user is the owner of this file...
        Public ReadOnly Property IamTheOwner As Boolean
            Get
                Return (GetUserID() = dr.Field(Of Integer)("OwnerID"))
            End Get
        End Property

        'Construction
        '==================

        'Common Constructor Code
        Private Sub NewByID(ByVal FileID As Integer)
            theFileID = FileID
            Dim str As String = _
                "SELECT " & _
                "       [DropboxPath]" & _
                "      ,[Name]" & _
                "      ,[OwnerID]" & _
                "      ,[LocalTimeStamp]" & _
                "  FROM [FriedParts].[dbo].[files-Common] " & _
                " WHERE [FileID] = " & FileID
            Dim dt As New DataTable
            SelectRows(dt, str)
            If dt.Rows.Count <> 1 Then
                Throw New FileNotFoundException("The specified FileID (" & FileID & ") does not exist.")
            End If
            dr = dt.Rows(0)
        End Sub

        Public Sub NewByPath(ByRef DirectoryPath As String, ByRef Filename As String)
            NewByID(apiDropboxGetFileID(DirectoryPath, Filename))
        End Sub

        'For existing file
        Public Sub New(ByRef UserID As Int32, ByRef FileID As Integer)
            whosAskingUserID = UserID
            NewByID(FileID)
        End Sub
        'For existing file, but retrieve by Path, rather than FileID
        Public Sub New(ByRef UserID As Int32, ByRef DirectoryPath As String, ByRef Filename As String)
            whosAskingUserID = UserID
            NewByPath(DirectoryPath, Filename)
        End Sub
        'For existing file, but retrieve by Path and Filename as one combined string, rather than FileID or separated string fields
        Public Sub New(ByRef UserID As Int32, ByRef thePathAndFilename As String)
            whosAskingUserID = UserID
            Dim thePaF As New txtPathAndFilename(thePathAndFilename)
            NewByPath(thePaF.GetPath, thePaF.GetFilename)
        End Sub
        'For new file (does not perform the actual file upload -- do that FIRST so you can get the DropboxTimeStamp!)
        Public Sub New(ByRef UserID As Int32, ByRef DirectoryPath As String, ByRef Filename As String, ByRef DropboxTimestamp As Date)
            whosAskingUserID = UserID
            'Write the common file data
            Dim str As String = _
                "INSERT INTO [FriedParts].[dbo].[files-Common]" & _
                "           (" & _
                "            [DropboxPath]" & _
                "           ,[Name]" & _
                "           ,[OwnerID])" & _
                "     VALUES (" & _
                "           '" & DirectoryPath & "'," & _
                "           '" & Filename & "'," & _
                "           " & GetUserID() & "" & _
                "           )"
            SQLexe(str)
            'Write the UserMetaData (record new dropbox state) -- this is version = 1!!!
            str = _
                "INSERT INTO [FriedParts].[dbo].[files-MetaByUser]" & _
                "           ([FileID]" & _
                "           ,[Version]" & _
                "           ,[UserID]" & _
                "           ,[Modified])" & _
                "     VALUES (" & _
                "            " & apiDropboxGetFileID(DirectoryPath, Filename) & "," & _
                "            " & 1 & "," & _
                "            " & GetUserID() & "," & _
                "            " & txtSqlDate(DropboxTimestamp) & "" & _
                "           )"
            SQLexe(str)

            NewByPath(DirectoryPath, Filename)
        End Sub
    End Class

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

    'Log Dropbox File Activity
    '---Defaults to current user
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



    '== USE CLASSES
    '=======================================================
    'For any and all activities that you have to be logged in for...
    Public Class dropboxUser
        'Internal Variables
        '==========================================================
        Private Dropbox As DropNet.DropNetClient
        Private dbxUserID As Int32

        'Public Properties
        '==========================================================
        Public ReadOnly Property GetUserID() As Int32
            Get
                Return dbxUserID
            End Get
        End Property

        Private UserTokens As DropboxUserCredentials
        Public ReadOnly Property GetUserTokens() As DropboxUserCredentials
            Get
                Return UserTokens
            End Get
        End Property

        Private AccountInfo As DropNet.Models.AccountInfo
        Public ReadOnly Property GetAccountInfo() As DropNet.Models.AccountInfo
            Get
                Return AccountInfo
            End Get
        End Property

        Public ReadOnly Property GetQuotaInfo() As DropNet.Models.QuotaInfo
            Get
                Return AccountInfo.quota_info
            End Get
        End Property

        Private Sub UpdateUserTimeStamp(ByRef DropboxPath As String, ByRef Filename As String)
            'Var
            Dim theServerFile As New DropboxFile(dbxUserID, DropboxPath & Filename)

            'Retrieve Dropbox's new modified timestamp
            Dim MetaResult As DropNet.Models.MetaData = Dropbox.GetMetaData(DropboxPath)
            Dim foundMatch As Boolean = False
            For Each cont As DropNet.Models.MetaData In MetaResult.Contents
                If cont.Name.CompareTo(Filename) = 0 Then
                    'Match!
                    foundMatch = True
                    'Write the UserMetaData (record new dropbox state)
                    theServerFile.DropboxTimeStamp = cont.ModifiedDate
                End If
            Next
            If Not foundMatch Then
                Throw New FileNotFoundException("Could not find the recent upload's new timestamp! Impossible!")
            End If
        End Sub

        'Uploads the specified local file and returns true on success...
        '   LocalFile = the local file's FileInfo object.
        '       If you don't have a FileInfo object, you can get one from the *fully qualified* 
        '       local path and filename -- e.g. "M:\FriedParts\Dropbox\Blah.Txt"
        '       Using: uploadFile(New FileInfo(LocalFilenameWithPath),"/DropboxPath/")
        '   DropboxPath = just the path starts and ends with the delimitor -- e.g "\FriedParts\SchLibs\"
        '       This will create the DropboxPath if it doesn't exist
        Public Function uploadFile(ByRef LocalFile As FileInfo, Optional ByRef DropboxPath As String = "/") As Boolean
            Dim blah As FileInfo = LocalFile
            Dim content As Byte() = Dropbox.GetFileContentFromFS(blah)
            Dim tStart As Date
            Dim tStop As Date
            logDropbox(GetUserID, apiDropbox.DropboxStates.Uploading, blah.FullName, DropboxPath)
            Try
                tStart = Now
                Dim Response As System.Net.HttpStatusCode
                Response = Dropbox.UploadFile(DropboxPath, blah.Name, content)
                If Response = System.Net.HttpStatusCode.OK Then
                    'Success
                    tStop = Now
                    Dim TransferTime As New TimeSpan(tStop.Ticks - tStart.Ticks)
                    logDropbox(GetUserID, apiDropbox.DropboxStates.UploadComplete, blah.FullName, DropboxPath, "Wrote " & content.Length & " Bytes in " & TransferTime.TotalSeconds & " seconds (" & (content.Length / 1024) / TransferTime.TotalSeconds & " kB/s).")
                    UpdateUserTimeStamp(DropboxPath, LocalFile.Name) 'Update modified date
                    Return True
                Else
                    'Failure
                    logDropbox(GetUserID, apiDropbox.DropboxStates.UploadFailed, blah.FullName, DropboxPath, "Result: " & Response.ToString)
                    'Err.Raise(-342, , "Upload Failed. " & Response.ToString)
                    Return False
                End If
            Catch ex As Exception
                'The Dropbox API periodically returns random "Internal Server Error" -- no big deal for us really, we'll just try again later
                logDropbox(GetUserID, apiDropbox.DropboxStates.UploadFailed, blah.FullName, DropboxPath, "Exception: " & ex.Message)
                'Throw New IOException("Upload of " & blah.FullName & " failed. " & ex.Message) 'In the future this might be removed and true handling added... don't know...
                Return False
            End Try
        End Function

        '
        ''' <summary>
        ''' Downloads the specified dropbox file and returns true on success...
        ''' 
        ''' </summary>
        ''' <param name="DropboxPathAndFilename">The path and filename for the source of the copy operation (from Dropbox). Dropbox paths start with "/"</param>
        ''' <param name="LocalPathAndFilename">The path and filename for the destination of the copy operation (the local FriedParts Server). Paths end in "\".</param>
        ''' <param name="Which_Webpage">The webpage making use of this file. For logging user activity. This is the "Me" value when writing in the code-behind.</param>
        ''' <returns>True on success. False otherwise.</returns>
        ''' <remarks>Handles all of the logging tasks.</remarks>
        Public Function downloadFile(ByRef DropboxPathAndFilename As String, ByRef LocalPathAndFilename As String, Optional ByRef Which_Webpage As System.Web.UI.Page = Nothing) As Boolean
            Dim PaF As New txtPathAndFilename(DropboxPathAndFilename, True)
            Dim tStart As Date
            Dim tStop As Date
            logDropbox(apiDropbox.DropboxStates.Downloading, PaF.GetFilename, PaF.GetPath)
            Try
                tStart = Now
                Dim theFileData As Byte() = Dropbox.GetFile(DropboxPathAndFilename)
                Dim theWriteStream As New FileStream(LocalPathAndFilename, FileMode.Create, FileAccess.Write)
                theWriteStream.Write(theFileData, 0, theFileData.Length)
                theWriteStream.Close()
                tStop = Now
                Dim TransferTime As New TimeSpan(tStop.Ticks - tStart.Ticks)
                logDropbox(GetUserID, apiDropbox.DropboxStates.DownloadComplete, PaF.GetFilename, PaF.GetPath, "Wrote " & theFileData.Length & " Bytes in " & TransferTime.TotalSeconds & " seconds (" & (theFileData.Length / 1024) / TransferTime.TotalSeconds & " kB/s).")
                logUserActivity(Which_Webpage, suGetUserFirstName(GetUserID) & " updated " & PaF.GetPathSubDirsOnly & PaF.GetFilename)
                Return True
            Catch ex As Exception
                logDropbox(GetUserID, apiDropbox.DropboxStates.DownloadFailed, PaF.GetFilename, PaF.GetPath, "Exception: " & ex.Message)
                'Throw New Exception("Download Failed!") 'xxx handle more smoothly in the future?
                Return False
            End Try
        End Function

        Public Sub SyncFile(ByRef UserDbxFile As DropNet.Models.MetaData)
            'Can get Dbx path, filename -- from UserDbxFile
            'Sync The Base Databse Lib
            Dim theFilename As String = UserDbxFile.Name
            Dim theDropboxPath As New txtPathAndFilename(UserDbxFile.Path)
            Dim theServerPath As String = dropboxROOT() & theDropboxPath.GetPathSubDirsOnly.Substring(1).Replace("/", "\")
            Dim thePaF As txtPathAndFilename

            'UserDbxFile.Path includes the filename and a leading forward slash = "/FriedParts/FriedParts.DbLib"
            If UserDbxFile.Is_Dir = False Then
                If UserDbxFile.Is_Deleted = False Then
                    'File exists -- check its version
                    Dim ServerFile As New DropboxFile(dbxUserID, theDropboxPath.GetPath, theFilename)

                    '=============================================================================================================
                    Try
                        If UserDbxFile.ModifiedDate.CompareTo(ServerFile.DropboxTimeStamp) > 0 Then
                            '<<< User's copy is newer than server's copy... >>>
                            If ServerFile.IamTheOwner Then 'Am I the Owner?
                                'I am the Owner -- so update the Server's copy
                                'Download file from user's dropbox
                                If downloadFile(UserDbxFile.Path, theServerPath & theFilename) Then 'also handles all of the logging
                                    'Update the meta-data in the database (if the download succeeded)
                                    ServerFile.DropboxTimeStamp = UserDbxFile.ModifiedDate
                                End If
                            Else
                                'I am not the Owner -- so reset the local copy
                                thePaF = New txtPathAndFilename(UserDbxFile.Path, UserDbxFile.Is_Dir)
                                uploadFile(New FileInfo(theServerPath & theFilename), thePaF.GetPath)
                                'Log the overwrite event!
                                logDropbox(GetUserID, DropboxStates.ErrorUnauthorizedChanges, theFilename, UserDbxFile.Path, "User changed file without permission. Authorized version has been restored.")
                            End If
                            '=============================================================================================================
                        ElseIf UserDbxFile.ModifiedDate.CompareTo(ServerFile.DropboxTimeStamp) < 0 Then
                            '<<< Server's copy is newer than user's >>>
                            thePaF = New txtPathAndFilename(UserDbxFile.Path, UserDbxFile.Is_Dir)
                            uploadFile(New FileInfo(theServerPath & theFilename), thePaF.GetPath)
                            '=============================================================================================================
                        Else
                            'File is sync'd -- no changes necessary
                            '=============================================================================================================
                        End If
                    Catch ex As FileNotFoundException
                        'The file exists in the user's dropbox, but we don't have any metadata for it. This is a weird condition that 
                        'shouldn't occur, but might if the user side-loads a library file
                        '<<< JUST ASSUME that server's copy is newer than user's >>>
                        logDropbox(GetUserID, DropboxStates.ErrorMissingMetaData, theFilename, New txtPathAndFilename(UserDbxFile.Path, UserDbxFile.Is_Dir).GetPath, "No Metadata was found. Perhaps the file was sideloaded or the database entries were deleted?")
                        thePaF = New txtPathAndFilename(UserDbxFile.Path, UserDbxFile.Is_Dir)
                        uploadFile(New FileInfo(theServerPath & theFilename), thePaF.GetPath)
                    End Try
                Else
                    'File doesn't exist -- upload it!
                    thePaF = New txtPathAndFilename(UserDbxFile.Path, UserDbxFile.Is_Dir)
                    uploadFile(New FileInfo(theServerPath & theFilename), thePaF.GetPath)
                End If
            Else
                'Entry is a sub-directory, not a file -- so do nothing (e.g. -- skip!)
            End If
        End Sub

        Public Sub Sync()
            Dim theAppRootMeta As DropNet.Models.MetaData
            Dim theDirMeta As DropNet.Models.MetaData
            Dim entry As DropNet.Models.MetaData

            'Does Approot Exist?
            Dropbox.CreateFolder(apiDropboxRoot) 'Creating it is faster than Metadata retrieval and testing for existance; No effect if Dir already exists

            'Get Metadata for Dropbox AppRoot
            theAppRootMeta = Dropbox.GetMetaData(apiDropboxRoot)

            'Sync The Base Databse Lib
            Const DbLibFilename As String = "FriedParts.DbLib"
            Dim FileExists As Boolean = False
            For Each entry In theAppRootMeta.Contents
                If entry.Name.CompareTo(DbLibFilename) = 0 And Not entry.Is_Dir Then
                    'File exists -- check its version
                    FileExists = True
                    SyncFile(entry)
                    Exit For 'Done searching for FriedParts.DbLib
                End If
            Next
            If Not FileExists Then
                'File doesn't exist -- upload it!
                uploadFile(New FileInfo(dropboxROOT() & DbLibFilename), apiDropboxRoot)
            End If

            'Sync Directory Structure
            Dim di As New IO.DirectoryInfo(dropboxROOT())
            Dim DirExists As Boolean
            For Each Folder As IO.DirectoryInfo In di.EnumerateDirectories
                'If this is a system or hidden directory -- SKIP IT!
                If ((Folder.Attributes And FileAttributes.Hidden) = FileAttributes.Hidden) Or ((Folder.Attributes And FileAttributes.System) = FileAttributes.System) Then
                    Continue For 'Skip to next For Loop cycle -- e.g. skip this directory because it is hidden or system
                End If
                'Check if it exists in the AppRootMeta data -- if not create it
                DirExists = False
                For Each entry In theAppRootMeta.Contents
                    If entry.Is_Dir AndAlso (entry.Name.CompareTo(Folder.Name) = 0) Then
                        'Let's sync this directory's contents!
                        DirExists = True

                        'Sync the files in here (User --> Server)!
                        theDirMeta = Dropbox.GetMetaData(apiDropboxRoot & Folder.Name & "/")
                        For Each userFile As DropNet.Models.MetaData In theDirMeta.Contents
                            Try
                                SyncFile(userFile)
                            Catch ex As FileNotFoundException
                                'Doesn't exist on the server!
                                If fplibValidExtension(userFile.Extension, userFile.Path) Then
                                    'Is a valid library type so lets add it!
                                    Dim uFile As New txtPathAndFilename(userFile.Path)
                                    Dim downloadSuccess As Boolean = downloadFile(userFile.Path, dropboxROOT() & uFile.GetPathSubDirsOnly.Substring(1).Replace("/", "\") & uFile.GetFilename)
                                    If downloadSuccess Then
                                        'Download succeeded
                                        Dim newFile As New DropboxFile(dbxUserID, New txtPathAndFilename(userFile.Path).GetPath, userFile.Name, userFile.ModifiedDate) 'Save metadata
                                        fpLibCreateCadAltiumLibTable() 'Update server's Altium Library knowledge
                                    End If
                                End If
                            End Try
                        Next

                        'Sync the files in here (Server --> User)! (In case there are any new files on the server that the user doesn't have)
                        For Each serverFile As FileInfo In Folder.GetFiles()
                            Dim foundFile As DropNet.Models.MetaData = apiDropboxFindFile(serverFile.Name, theDirMeta.Contents)
                            If foundFile Is Nothing Then
                                'Server file not found in user's dropbox -- upload file to user's dropbox
                                If fplibValidExtension(serverFile.Extension, apiDropboxRoot & Folder.Name & "/") Then
                                    uploadFile(serverFile, apiDropboxRoot & Folder.Name & "/")
                                End If
                            Else
                                'files found on the server and the user's dropbox have already been sync'd
                            End If
                        Next

                        'Move on to next directory (no need to finish searching)
                        Exit For
                    End If
                Next
                If Not DirExists Then
                    'Create missing directory
                    Dropbox.CreateFolder(apiDropboxRoot & Folder.Name)
                End If
            Next 'Looping through local (to server) directories
        End Sub

        Public Sub New(ByRef UserID As Integer)
            'Confirm user exists
            sysUser.suGetUsername(UserID) 'Don't care, but this will throw error if invalid UserID -- just for safety (b/c query will fail silently)
            dbxUserID = UserID 'Save the UserID

            'Is user token already on file?
            Try
                UserTokens = getUserToken(UserID)
            Catch ex As NullReferenceException
                'No token on file! (probably an error at this point, since UI is required to enter password and approve it!)
                Err.Raise(-877, , "No Dropbox credentials on file for user " & sysUser.suGetUsername(UserID) & " (ID = " & UserID & ")!")
            End Try

            'Login
            Dropbox = getDropboxClient(UserTokens)

            'Cache Status Information
            AccountInfo = Dropbox.Account_Info
        End Sub
    End Class
End Module
