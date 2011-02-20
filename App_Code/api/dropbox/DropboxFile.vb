Imports Microsoft.VisualBasic
Imports System.Data
Imports System.IO


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