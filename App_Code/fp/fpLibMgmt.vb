Imports Microsoft.VisualBasic
Imports System.Data
Imports System.IO 'for working with files and folders
Imports System.Web.SessionState ' for working with session state (User ID)

Public Module fpLibMgmt

    '=======================================================================
    '= Library Types
    '=======================================================================
    Public Enum fplibTypes As Byte
        SchLib = 1
        PcbLib = 2
        Pcb3DLib = 3
    End Enum
    'Convert LibType to LibName
    Public Function fplibLibTypeName(ByVal LibTypeNum As Byte) As String
        Select Case LibTypeNum
            Case 1
                Return "Schematic Symbol Library"
            Case 2
                Return "PCB Layout Footprint Library"
            Case 3
                Return "3D Component Body Library"
            Case Else
                Return "Unknown"
        End Select
    End Function
    'Convert LibName to LibType
    Public Function fplibLibTypeNum(ByVal Filename As String) As fplibTypes
        'Get Extension
        Select Case Filename.Substring(Filename.LastIndexOf("."))
            Case ".SchLib"
                Return fplibTypes.SchLib
            Case ".PcbLib"
                Return fplibTypes.PcbLib
            Case ".Pcb3D"
                Return fplibTypes.Pcb3DLib
            Case Else
                Return "Unknown"
        End Select
    End Function
    'Convert LibType to File Extension
    Public Function fplibLibTypeExt(ByVal LibTypeNum As Byte) As String
        Select Case LibTypeNum
            Case 1
                Return ".SchLib"
            Case 2
                Return ".PcbLib"
            Case 3
                Return ".Pcb3D"
            Case Else
                Return ".txt"
        End Select
    End Function

    'DropboxPath is delimiter bounded -- e.g. "/FriedParts/SchLibs/"
    Public Function fplibValidExtension(ByRef TestExtension As String, Optional ByRef DropboxPath As String = Nothing) As Boolean
        'Test if extension is one of several acceptable
        Select Case TestExtension
            Case ".SchLib"
            Case ".PcbLib"
            Case ".Pcb3D"
            Case Else
                Return False
        End Select

        If Not DropboxPath Is Nothing Then
            'Check if DropboxPath is appropriate for this filetype
            Dim Folder As String = New txtPathAndFilename(DropboxPath).GetPathLastDirOnly
            If TestExtension.Substring(1).CompareTo(Folder.Trim("/").Substring(0, Folder.Trim("/").Length - 1)) = 0 Then 'Folder names and file extensions are CASE SENSITIVE!!! Folder names end in "s" -- e.g. "SchLibs" vs. ".SchLib" extension
                Return True
            Else
                Return False
            End If
        Else
            'Not checking the Dropbox path
            Return True
        End If
    End Function

    'Library Paths
    Public Function fplibLibTypePath(ByVal LibTypeNum As Byte) As String
        Select Case LibTypeNum
            Case 1
                Select Case Environment.MachineName.ToString()
                    Case "FRED" 'For debugging on FRED machine
                        Return "M:\FriedParts\Dropbox\SchLibs\"
                    Case Else
                        Return "C:\Inetpub\wwwroot\FriedParts\Dropbox\SchLibs\"
                End Select
            Case 2
                Select Case Environment.MachineName.ToString()
                    Case "FRED" 'For debugging on FRED machine
                        Return "M:\FriedParts\Dropbox\PcbLibs\"
                    Case Else
                        Return "C:\Inetpub\wwwroot\FriedParts\Dropbox\PcbLibs\"
                End Select
            Case 3
                Return "C:\Inetpub\wwwroot\FriedParts\Dropbox\Pcb3D\"
            Case Else
                Return "\INVALID PATH\"
        End Select
    End Function

    Public Function fplibLibPath() As String
        Select Case Environment.MachineName.ToString()
            Case "FRED" 'For debugging on FRED machine
                Return "M:\FriedParts\Dropbox\"
            Case Else
                Return "C:\Inetpub\wwwroot\FriedParts\Dropbox\"
        End Select
    End Function

    '=======================================================================
    '= [END] Library Types
    '=======================================================================

    'Constructs the cad-AltiumLib Table from the Dropbox files registered on the server. It ignores duplicates so it is safe to call this function repeatedly
    Public Sub fpLibCreateCadAltiumLibTable()
        'Get the Dropbox Records
        Dim str As String = _
            "SELECT [FileID]" & _
            "      ,[DropboxPath]" & _
            "      ,[Name]" & _
            "      ,[OwnerID]" & _
            "      ,[LocalTimeStamp]" & _
            "  FROM [FriedParts].[dbo].[files-Common]"
        Dim dt As New DataTable
        SelectRows(dt, str)

        'Creates the cad-AltiumLib table
        For Each dr As DataRow In dt.Rows
            If dr.Field(Of String)("Name").CompareTo("FriedParts.DbLib") <> 0 Then
                str = _
                    "INSERT INTO [FriedParts].[dbo].[cad-AltiumLib]" & _
                    "           ([LibID]" & _
                    "           ,[LibName]" & _
                    "           ,[LibType])" & _
                    "     VALUES (" & _
                    "            " & dr.Field(Of Int32)("FileID") & "," & _
                    "           '" & fplibMakeLibName(dr.Field(Of String)("Name")) & "'," & _
                    "           '" & fplibLibTypeNum(dr.Field(Of String)("Name")) & "'" & _
                    "           )"
                Try
                    SQLexe(str)
                Catch ex As SqlClient.SqlException
                    'Means that the record is already in the table, just ignore the duplicate
                End Try
            End If
        Next
    End Sub

    'Returns a 'correct' filename for the given user and date
    Public Function fplibMakeLibName(ByVal LibType As fplibTypes) As String
        Return _
            Now.Year.ToString & "_" & _
            txtGetWord(HttpContext.Current.Session("user.name"), 1) & _
            HttpContext.Current.Session("user.UserID") & _
            fplibLibTypeExt(LibType)
    End Function

    Public Function fplibMakeLibName(ByRef Filename As String) As String
        Dim result As String
        Dim extension As String = Filename.Substring(Filename.LastIndexOf("."))
        If fplibValidExtension(extension) Then
            result = extension.Substring(1) & "s\" & Filename
            Return result
        Else
            Throw New FileNotFoundException("The filename entered (" & Filename & ") does not have a valid extension!")
        End If
        Return Nothing 'Should never reach here
    End Function

    'Returns a datatable containing the new library files found in the specified directory
    Public Function fplibReportNew(ByVal DirectoryPath As String) As DataTable
        Return fplibReportNew(fplibScanFolder(DirectoryPath))
    End Function

    'Returns a subset of the passed in file table
    Public Function fplibReportNew(ByRef FileTable As DataTable) As DataTable
        Dim resultsTable As DataTable = fplibCreateFileTable()
        For i As Integer = 0 To FileTable.Rows.Count - 1
            If (Not fplibExists(FileTable.Rows(i))) Then
                resultsTable.ImportRow(FileTable.Rows(i)) 'Copy missing lib to output table
            End If
        Next
        Return resultsTable
    End Function

    'Returns True if the specified file exists in the FriedParts cad-AltiumLib table
    'Returns False otherwise
    Public Function fplibExists(ByVal FileTableRow As DataRow) As Boolean
        Dim LibPrefix As String = FileTableRow("extension").ToString
        LibPrefix = LibPrefix.Remove(0, 1) 'Eliminate the period prefix in the string "Extension"
        Dim LibName As String = LibPrefix & "s\" & FileTableRow("filename")
        Dim sqlSearch As String
        sqlSearch = _
            "SELECT [LibID]" & _
            "      ,[LibName]" & _
            "      ,[LibType]" & _
            "      ,[OwnerID]" & _
            "  FROM [FriedParts].[dbo].[cad-AltiumLib]" & _
            "  WHERE [LibName] = '" & LibName & "'"
        Dim dt As DataTable = New DataTable
        dbAcc.SelectRows(dt, sqlSearch)
        If dt.Rows.Count = 1 Then
            Return True
        Else
            Return False
        End If
    End Function

    'Scans the specified directory and reports all files found in a datatable
    Public Function fplibScanFolder(ByVal DirectoryPath As String) As Data.DataTable
        Dim DirInfo As DirectoryInfo = New DirectoryInfo(DirectoryPath)
        Dim Files As FileInfo() = DirInfo.GetFiles()
        Dim outTable As DataTable = fplibCreateFileTable()
        Dim Row1 As DataRow
        For i As Integer = 0 To Files.Length - 1
            Row1 = outTable.NewRow
            Row1("filename") = Files(i).Name
            Row1("extension") = Files(i).Extension
            Row1("path") = Files(i).DirectoryName
            outTable.Rows.Add(Row1)
        Next
        Return outTable
    End Function

    'Creates the schema of the file table that is common to all file listing operations
    'Namely:  Filename, Extension, Path
    Public Function fplibCreateFileTable() As Data.DataTable
        Dim Table1 As DataTable
        Table1 = New DataTable("Filelist")
        'Column 0: UID
        Dim Col1 As DataColumn = New DataColumn("UID")
        Col1.DataType = System.Type.GetType("System.Int16")
        Col1.AutoIncrement = True
        Col1.AutoIncrementSeed = 1
        Col1.AutoIncrementStep = 1
        Table1.Columns.Add(Col1)
        'Column 1: filename
        Col1 = New DataColumn("filename")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)
        'Column 2: extension
        Col1 = New DataColumn("extension")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)
        'Column 3: path
        Col1 = New DataColumn("path")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)
        Return Table1
    End Function

    'Creates the schema of the file table that is common to all file listing operations
    'Namely:  Filename, Extension, Path
    Public Function fplibCreateLibContentsTable() As Data.DataTable
        Dim Table1 As DataTable
        Table1 = New DataTable("LibraryContents")
        'Column 0: UID
        Dim Col1 As DataColumn = New DataColumn("UID")
        Col1.DataType = System.Type.GetType("System.UInt16")
        Col1.AutoIncrement = True
        Col1.AutoIncrementSeed = 1
        Col1.AutoIncrementStep = 1
        Table1.Columns.Add(Col1)
        'Column 1: filename
        Col1 = New DataColumn("LibRef")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)
        Return Table1
    End Function

    'Determines if a specified file exists in the specified path
    Public Function fplibFileExists(ByVal path As String, ByVal filename As String) As Boolean
        Dim DirInfo As DirectoryInfo = New DirectoryInfo(path)
        Dim Files As FileInfo() = DirInfo.GetFiles()
        For i As Integer = 0 To Files.Length - 1
            If (Files(i).Name = filename) Then
                'File found!
                Return True
            End If
        Next
        Return False
    End Function

    '[OVERLOADS] Checks if a specified file exists in the directory determined by its filetype
    'Returns true if exists, false otherwise
    Public Function fplibFileExists(ByVal LibType As fplibTypes, ByVal Filename As String) As Boolean
        Return fplibFileExists(fplibLibTypePath(CByte(LibType)), Filename)
    End Function

    'Add a new library!
    Public Sub fplibAdd(ByVal libName As String, ByVal libType As fplibTypes, ByVal libFilename As String, ByVal libOwner As Int16)
        'Confirm library adding file exists (implicitly checks libtype by extension)
        If fplibFileExists(fplibLibTypePath(libType), libFilename) Then
            'File exists! yay!
        Else
            'File not found! ERROR!
        End If
        'Confirm library adding owner = current user

        'ADD IT!
        Dim a As String = ""
        Dim SqlQuery As String = _
            "INSERT INTO [FriedParts].[dbo].[cad-AltiumLib]" & _
            "           ([LibName]" & _
            "           ,[LibType]" & _
            "           ,[OwnerID]" & _
            "           ,[Desc])" & _
            "     VALUES (" & _
            "           '" & a & "'," & _
            "           '" & a & "'," & _
            "           '" & a & "'," & _
            "           '" & a & "'," & _
            "           )"
    End Sub

    '=======================================================================
    '= ALTIUM LIBRARY CONTENTS ANALYZER
    '=======================================================================
    'Schematic Libraries:
    'Format is "LIBREFERENCE" then "=" <RECORD ALL THIS> then "|"
    'PCB Libraries:
    'Format is "PATTERN" then "=" <RECORD ALL THIS> then "|"
    Public Function fpLibReadLib(ByVal LibraryType As fplibTypes, ByVal filename As String) As DataTable
        Dim dt As DataTable = fplibCreateLibContentsTable()
        Dim counter As Integer = 1

        'declaring a FileStream to open the file named file.doc with access mode of reading
        Dim fs As New FileStream(filename, FileMode.Open, FileAccess.Read)

        'creating a new StreamReader and passing the filestream object fs as argument
        Dim d As New StreamReader(fs)

        'Seek method is used to move the cursor to different positions in a file, in this code, to 
        'the beginning
        d.BaseStream.Seek(0, SeekOrigin.Begin)

        Dim LibField As New System.Text.StringBuilder()
        Dim i As Integer
        Dim str As String
        Dim idxStart As Integer
        'peek method of StreamReader object tells how much more data is left in the file
        While d.Peek() > -1
            i = d.Read() 'Read next character
            If i = &H7C Then
                'Found the pipe character delimiter -- done reading in this field
                str = LibField.ToString
                'Find the field's contents
                Select Case LibraryType
                    Case fplibTypes.SchLib
                        If str.StartsWith("LIBREFERENCE") Then
                            'Found the library reference -- add it to table
                            idxStart = str.IndexOf("=")
                            Dim o() As Object = {counter, str.Substring(idxStart + 1)}
                            dt.Rows.Add(o)
                        End If
                    Case fplibTypes.PcbLib
                        If str.StartsWith("PATTERN") Then
                            'Found the library reference -- add it to table
                            idxStart = str.IndexOf("=")
                            Dim o() As Object = {counter, str.Substring(idxStart + 1)}
                            dt.Rows.Add(o)
                        End If
                    Case Else
                        Err.Raise(-76456, , "Library Type Error")
                End Select
                counter = counter + 1
                'Reset for next field
                LibField = New System.Text.StringBuilder()
            Else
                'Keep going
                'Save character if it is printable...
                If i >= 32 And i <= 126 Then
                    LibField.Append(Chr(i))
                End If
                '...discard it otherwise
            End If
        End While
        d.Close()
        'Sort & Return
        dt.DefaultView.Sort = "LibRef, UID"
        Return dt
    End Function

    '=======================================================================
    '= [END] ALTIUM LIBRARY CONTENTS ANALYZER
    '=======================================================================

End Module