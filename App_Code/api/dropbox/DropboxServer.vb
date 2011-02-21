Imports Microsoft.VisualBasic
Imports System.Data
Imports System.IO

Namespace apiDropbox

    ''' <summary>
    ''' The DropboxServer object is like a DropboxUser object except that there is only one
    ''' server and, therefore, no reason to create more than one object of this type ever.
    ''' For efficiency we have one global object (shared class = module). Call Update() to 
    ''' rescan the hard disk as needed.
    ''' </summary>
    ''' <remarks></remarks>
    Public Module DropboxServer

        ''' <summary>
        ''' Maintains a file count / UID for file rows
        ''' </summary>
        ''' <remarks></remarks>
        Private FileCount As Int16 = 0

        ''' <summary>
        ''' Internal cache of the filesystem data.
        ''' </summary>
        ''' <remarks>Dropbox state as seen by the FriedParts server</remarks>
        Private DT As DataTable = Nothing

        ''' <summary>
        ''' Keeps track of the last time the Update() function was called.
        ''' </summary>
        ''' <remarks>Enables use to determine how old the cache of the data is...</remarks>
        Private LastScan As Date

        ''' <summary>
        ''' Returns the amount of time that has passed since the server was last scanned
        ''' </summary>
        ''' <returns>Timespan object</returns>
        ''' <remarks></remarks>
        Public Function HowStale() As TimeSpan
            Return New TimeSpan(Now.Ticks - LastScan.Ticks)
        End Function

        Public Function CountFiles() As Int16
            Return FileCount
        End Function

        ''' <summary>
        ''' Creates the Contents Table schema.
        ''' </summary>
        ''' <returns>Structured (but empty) DataTable</returns>
        ''' <remarks></remarks>
        Public Function CreateContentsTable() As DataTable
            Dim Table1 As DataTable
            Table1 = New DataTable("TextTable")
            'Init
            Dim col1 As DataColumn
            '[UID]
            col1 = New DataColumn("UID")
            col1.DataType = System.Type.GetType("System.Int16")
            col1.AutoIncrement = True
            col1.AutoIncrementSeed = 1
            col1.AutoIncrementStep = 1
            Table1.Columns.Add(col1)
            '[Something]
            col1 = New DataColumn("Path")
            col1.DataType = System.Type.GetType("System.String")
            Table1.Columns.Add(col1)
            '[Something]
            col1 = New DataColumn("Filename")
            col1.DataType = System.Type.GetType("System.String")
            Table1.Columns.Add(col1)
            '[Something]
            col1 = New DataColumn("FileSize")
            col1.DataType = System.Type.GetType("System.Int16")
            Table1.Columns.Add(col1)
            '[Something]
            col1 = New DataColumn("Version")
            col1.DataType = System.Type.GetType("System.Int16")
            Table1.Columns.Add(col1)
            '[Something]
            col1 = New DataColumn("LastUpdate")
            col1.DataType = System.Type.GetType("System.DateTime")
            Table1.Columns.Add(col1)
            '[Something]
            col1 = New DataColumn("LastAccessed")
            col1.DataType = System.Type.GetType("System.DateTime")
            Table1.Columns.Add(col1)
            '[Something]
            col1 = New DataColumn("Owner")
            col1.DataType = System.Type.GetType("System.String")
            Table1.Columns.Add(col1)
            '[Something]
            col1 = New DataColumn("Notes")
            col1.DataType = System.Type.GetType("System.String")
            Table1.Columns.Add(col1)
            Return Table1
        End Function

        ''' <summary>
        ''' Returns the server contents table.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Schema is controlled by DropboxServer.CreateContentsTable()</remarks>
        Public Function GetDataSource() As DataTable
            If DT Is Nothing Then
                Update()
            End If
            Return DT
        End Function

        ''' <summary>
        ''' Updates the internal state. Rescans disk.
        ''' </summary>
        ''' <remarks>We only support one level of sub-directories in FriedParts Dropbox.
        ''' That is, "\FriedParts\SchLibs\" is ok, but "\FP\SchLibs\Blah" is ignored.</remarks>
        Public Sub Update()
            'Reset Table
            If DT Is Nothing Then
                DT = CreateContentsTable()
            Else
                DT.Clear()
            End If
            FileCount = 0

            'Initialize Scan
            Dim strFileSize As String = ""
            Dim di As New IO.DirectoryInfo(sysEnv.dropboxROOT)
            Dim sdi As IO.DirectoryInfo
            Dim fi As IO.FileInfo

            'Scan Files
            For Each fi In di.GetFiles
                AddFileToTable(di, fi)
            Next

            'Scan Subdirectories!
            For Each sdi In di.GetDirectories
                'Process this folder...
                For Each fi In sdi.GetFiles
                    AddFileToTable(sdi, fi)
                Next
            Next

            'Mark this update as done.
            LastScan = Now
        End Sub

        Private Sub AddFileToTable(ByRef sdi As DirectoryInfo, ByRef fi As FileInfo)
            'Init
            Dim DRow As DataRow
            Dim FpFile As DropboxFile
            FileCount += 1

            'File System Properties
            Dim strFileSize As String = (Math.Round(fi.Length / 1024)).ToString()
            DRow = DT.NewRow
            DRow.Item("UID") = FileCount
            DRow.Item("Filename") = fi.Name
            DRow.Item("Path") = fi.DirectoryName
            DRow.Item("LastAccessed") = fi.LastAccessTime
            DRow.Item("FileSize") = strFileSize 'in KB

            'FriedParts Properties
            Try
                If sdi.Name.CompareTo("Dropbox") = 0 Then
                    'This is the root folder...
                    FpFile = New DropboxFile(31, apiDropboxRoot & fi.Name) 'UserID is not important here since we are only seeking Read Only access -- just set it something valid
                Else
                    'This is a subdirectory
                    FpFile = New DropboxFile(31, apiDropboxRoot & sdi.Name & "/" & fi.Name) 'UserID is not important here since we are only seeking Read Only access -- just set it something valid
                End If
                DRow.Item("Version") = FpFile.GetVersion
                DRow.Item("Owner") = FpFile.GetOwnerFullName
                DRow.Item("LastUpdate") = FpFile.GetLastUpdate
            Catch ex As FileNotFoundException
                'This file does not have a FriedParts entry
                DRow.Item("Notes") = "THIS FILE NOT CATALOGED!!!"
            End Try
            DT.Rows.Add(DRow)
        End Sub
    End Module

End Namespace