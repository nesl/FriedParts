Imports Microsoft.VisualBasic
Imports System.Data
Imports System.IO

Namespace apiDropbox

    Public Class DropboxFile
        'Private Internal Data
        '=====================
        ''' <summary>
        ''' Used to cache the databse entry for this file from [files-Common]
        ''' </summary>
        ''' <remarks></remarks>
        Private dr As DataRow

        Private theFileID As Integer
        ''' <summary>
        ''' The UserID of the person requesting information about this DropboxFile.
        ''' </summary>
        ''' <remarks>UserID is important because we can't check for changes without knowing 
        ''' which Dropbox account we are working with.</remarks>
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

        ''' <summary>
        ''' Accessor/Modifier for the Dropbox server's timestamp associated with this file.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
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

        ''' <summary>
        ''' "Version" is a FriedParts concept of the file's version since the Dropbox notion
        ''' of version is a timestamp which varies across each user's dropbox. If you upload
        ''' the same file to a bunch of dropboxes the timestamp for the identical file will 
        ''' be different in every case. FriedParts synchronizes files against different 
        ''' dropboxes by keeping track of its own notion of version. Versions can only be
        ''' updated by the file's owner and versioning is handled automatically. The users
        ''' are never exposed to it.
        ''' </summary>
        ''' <returns>The current version of this file</returns>
        ''' <remarks></remarks>
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

        Public ReadOnly Property GetLastUpdate As Date
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
                Return dt.Rows(0).Field(Of Date)("Modified")
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

        ''' <summary>
        ''' Common Constructor Code. This is the most basic construction worker function.
        ''' </summary>
        ''' <param name="FileID">The FP FileID</param>
        ''' <remarks>Not called by developer. Throws FileNotFoundException if the file doesn't exist.</remarks>
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

        ''' <summary>
        ''' Common constructor code. This is the 2nd most basic construction worker function.
        ''' Simply looks up the FileID and calls the common constructor 
        ''' with it. This function never called by developer. Use New(...) during
        ''' object construction
        ''' </summary>
        ''' <param name="DirectoryPath">The Dropbox Path including starting and
        ''' ending delimiters (e.g. "/FriedParts/").</param>
        ''' <param name="Filename">Just the filename and extension.</param>
        ''' <remarks>Throws FileNotFoundException if the file is not found.</remarks>
        Public Sub NewByPath(ByRef DirectoryPath As String, ByRef Filename As String)
            NewByID(apiDropboxGetFileID(DirectoryPath, Filename))
        End Sub

        ''' <summary>
        ''' Constructor. For existing file.
        ''' </summary>
        ''' <param name="UserID">We need the UserID in order to determine if the file has
        ''' been modified -- since the modified timestamp is user specific. This is the 
        ''' UserID of the person asking (it doesn't have to be the file's ownerID).</param>
        ''' <param name="FileID">If you know it, great! Otherwise use one of the other
        ''' constructors which can look it up by path.</param>
        ''' <remarks></remarks>
        Public Sub New(ByRef UserID As Int32, ByRef FileID As Integer)
            whosAskingUserID = UserID
            NewByID(FileID)
        End Sub

        ''' <summary>
        ''' Constructor. For existing file, but retrieve by Path, rather than FileID.
        ''' </summary>
        ''' <param name="UserID">We need the UserID in order to determine if the file has
        ''' been modified -- since the modified timestamp is user specific. This is the 
        ''' UserID of the person asking (it doesn't have to be the file's ownerID).</param>
        ''' <param name="DirectoryPath">The Dropbox Path including starting and
        ''' ending delimiters (e.g. "/FriedParts/").</param>
        ''' <param name="Filename">Just the filename and extension.</param>
        ''' <remarks>Actual constructor function. Developers interact with this one.</remarks>
        Public Sub New(ByRef UserID As Int32, ByRef DirectoryPath As String, ByRef Filename As String)
            whosAskingUserID = UserID
            NewByPath(DirectoryPath, Filename)
        End Sub

        ''' <summary>
        ''' For existing file, but retrieve by Path and Filename as one combined string, 
        ''' rather than FileID or separated string fields
        ''' </summary>
        ''' <param name="UserID">We need the UserID in order to determine if the file has
        ''' been modified -- since the modified timestamp is user specific. This is the 
        ''' UserID of the person asking (it doesn't have to be the file's ownerID).</param>
        ''' <param name="thePathAndFilename"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef UserID As Int32, ByRef thePathAndFilename As String)
            whosAskingUserID = UserID
            Dim thePaF As New txtPathAndFilename(thePathAndFilename)
            NewByPath(thePaF.GetPath, thePaF.GetFilename)
        End Sub

        ''' <summary>
        ''' For new file (does not perform the actual file upload -- do that FIRST so you 
        ''' can get the DropboxTimeStamp!)
        ''' </summary>
        ''' <param name="UserID">We need the UserID in order to determine if the file has
        ''' been modified -- since the modified timestamp is user specific. This is the 
        ''' UserID of the person asking (it doesn't have to be the file's ownerID).</param>
        ''' <param name="DirectoryPath">The Dropbox Path including starting and
        ''' ending delimiters (e.g. "/FriedParts/").</param>
        ''' <param name="Filename">Just the filename and extension.</param>
        ''' <param name="DropboxTimestamp">*IMPORTANT* You must upload the file first so that you 
        ''' can retrieve the new Dropbox *SERVER-ISSUED* timestamp and pass it in here so that FriedParts
        ''' can record it. All version control is based on this value!</param>
        ''' <remarks></remarks>
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
End Namespace