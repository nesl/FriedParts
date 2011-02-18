Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Diagnostics
Imports System.Threading

Namespace UpdateService

    

    Public Class upServiceReport
        ''' <summary>
        ''' The Application Pool Server Process. Conatins the array of WebServer Threads.
        ''' </summary>
        ''' <remarks>Caches the Sys.Diagnostics information about the Application Pool worker process (the OS thread that services this website -- contains the application-level threads)</remarks>
        Private TheAppPool As Process

        ''' <summary>
        ''' Caches the number of Application Pools running on the server. 
        ''' </summary>
        ''' <remarks>This is not implemented since the live site only has one AppPool (ergo only one server process).</remarks>
        Private nServerProcesses As Byte = 1

        Private dt As New DataTable

        ''' <summary>
        ''' Reports the number of Application Pools found running on the server. If more than one, 
        ''' this is the only value that will be populated. We only process if there is exactly one
        ''' App Pool found.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Each AppPool runs in a different server process. We assume there is 
        ''' only one, since that how it works on the live site.</remarks>
        Public ReadOnly Property NumWebserverProcesses As Int16
            Get
                Return nServerProcesses
            End Get
        End Property
        Private Function countWorkers(ByRef WorkerType As upThreadTypes) As Int16
            Dim retval As Int16 = 0
            For Each dr As DataRow In dt.Rows
                If dr.Field(Of upThreadTypes)("TypeID") = WorkerType Then retval += 1
            Next
            Return retval
        End Function
        Public ReadOnly Property NumWorkerThreads(Optional ByVal WorkerType As upThreadTypes = upThreadTypes.rpAllWorkers) As Int16
            Get
                If WorkerType = upThreadTypes.rpAllWorkers Then
                    Return _
                        countWorkers(upThreadTypes.ttWorkerDropbox) + _
                        countWorkers(upThreadTypes.ttWorkerPart)
                Else
                    Return countWorkers(WorkerType)
                End If
            End Get
        End Property
        Public ReadOnly Property NumDispatcherThreads As Int16
            Get
                Return countWorkers(upThreadTypes.ttDispatcher)
            End Get
        End Property
        Public ReadOnly Property DataSource As DataTable
            Get
                Return dt
            End Get
        End Property

        Public Function CreateThreadReportTable() As Data.DataTable
            Dim Table1 As DataTable
            Table1 = New DataTable("TextTable")
            'Init
            Dim col1 As DataColumn
            '[UID]
            col1 = New DataColumn("ThreadID")
            col1.DataType = System.Type.GetType("System.Int32")
            Table1.Columns.Add(col1)
            '[ThreadType]
            col1 = New DataColumn("TypeID")
            col1.DataType = System.Type.GetType("System.Int32")
            col1 = New DataColumn("Type")
            col1.DataType = System.Type.GetType("System.String")
            '[DataID]
            col1 = New DataColumn("DataID")
            col1.DataType = System.Type.GetType("System.Int32")
            '[TheadStatus]
            col1 = New DataColumn("StatusID")
            col1.DataType = System.Type.GetType("System.Int32")
            col1 = New DataColumn("Status")
            col1.DataType = System.Type.GetType("System.String")
            'Finalize
            Table1.Columns.Add(col1)
            Return Table1
        End Function

        Public Sub New()
            'Find the Application Pool Server Process
            Dim theProcesses() As Process
            theProcesses = Process.GetProcessesByName("w3wp")

            'Is this it? More than one? Uh Oh!
            If theProcesses.Length <> 1 Then
                nServerProcesses = theProcesses.Length
                Exit Sub
            End If
            TheAppPool = theProcesses(0)

            'All set! Let's report!
            dt = CreateThreadReportTable() 'Load schema
            Dim dr As DataRow
            Dim Metadata As upThreadMetaData
            For Each Th As Thread In TheAppPool.Threads
                If Th.Name.StartsWith(upThreadMetaData.EncodingStartSymbol) Then
                    Metadata = New upThreadMetaData(Th.Name)
                    dr = dt.NewRow
                    dr.Item("ThreadID") = Metadata.GetThreadID
                    dr.Item("DataID") = Metadata.GetThreadDataID
                    dr.Item("TypeID") = Metadata.GetThreadType
                    dr.Item("Type") = upService.ThreadTypeToString(Metadata.GetThreadType)
                    dr.Item("StatusID") = Metadata.ThreadStatus
                    dr.Item("Status") = upService.StatusToString(Metadata.ThreadStatus)
                    dt.Rows.Add(dr)
                End If
            Next
        End Sub
    End Class
End Namespace
