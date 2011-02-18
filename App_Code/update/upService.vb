Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Threading

Namespace UpdateService

    ''' <summary>
    ''' Acts like an Operating System table -- tracking references to all the Managed Threads
    ''' being created.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class upThreadList

        Protected Shared procAllThreads As New Collection
        Protected Shared procAllMeta As New Collection

        Public Shared ReadOnly Property GetThreads As Collection
            Get
                Return procAllThreads
            End Get
        End Property

        Public Shared ReadOnly Property GetMetadata As Collection
            Get
                Return procAllMeta
            End Get
        End Property

        Public Shared ReadOnly Property CountThreads As Int16
            Get
                Return procAllThreads.Count
            End Get
        End Property
        ''' <summary>
        ''' Adds a new managed code thread
        ''' </summary>
        ''' <param name="TheThread">The FriedParts upProcess object</param>
        ''' <param name="TheMetaData">The associated upThreadMetaData object</param>
        ''' <remarks></remarks>
        Public Shared Sub StartThread(ByRef TheThread As Thread, ByRef TheMetaData As upThreadMetaData)
            TheThread.Start() 'Actually start the thread
            procAllThreads.Add(TheThread, TheMetaData.GetThreadID)
            procAllMeta.Add(TheMetaData, TheMetaData.GetThreadID)
        End Sub

        ''' <summary>
        ''' Stops the specified managed code thread
        ''' </summary>
        ''' <param name="TheThreadID">The FriedParts issued ThreadID</param>
        ''' <remarks>Fails silently if the thread is not found! Careful!</remarks>
        Public Shared Sub StopThread(ByRef TheThreadID As Int32)
            If procAllThreads.Contains(TheThreadID) Then
                DirectCast(procAllThreads(TheThreadID), Thread).Abort() 'Actually stop the thread
                procAllThreads.Remove(TheThreadID) 'Remove the reference? -- it will be null at this point so I don't know if this will work
            End If
        End Sub

        ''' <summary>
        ''' Shutsdown all Update Service threads. Use to keep the server 
        ''' from running out of resources if things get away from us.
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub StopAllThreads()
            For Each Th As Thread In procAllThreads
                Th.Abort()
            Next
        End Sub
    End Class


    ''' <summary>
    ''' Contains all the metadata about a specific FriedParts-Update-Service thread
    ''' </summary>
    ''' <remarks></remarks>
    Public Class upThreadMetaData
        Public Const EncodingDelimiter As String = ";"
        Public Const EncodingStartSymbol As String = "FP" & EncodingDelimiter
        Private tID As Int32
        Private tType As upThreadTypes
        Private tDataID As Int32
        Private tStatus As scanStatus

        Public ReadOnly Property GetThreadID As Int32
            Get
                Return tID
            End Get
        End Property
        Public Property ThreadType As upThreadTypes
            Get
                Return tType
            End Get
            Set(ByVal value As upThreadTypes)
                tType = value
            End Set
        End Property
        Public Property ThreadDataID As Int32
            Get
                Return tDataID
            End Get
            Set(ByVal value As Int32)
                tDataID = value
            End Set
        End Property
        Public Property ThreadStatus As scanStatus
            Get
                Return tStatus
            End Get
            Set(ByVal value As scanStatus)
                tStatus = value
            End Set
        End Property

        Public Function Encode() As String
            Return _
                EncodingStartSymbol & _
                tID & EncodingDelimiter & _
                tType & EncodingDelimiter & _
                tDataID & EncodingDelimiter & _
                tStatus
        End Function

        ''' <summary>
        ''' Parses an encoded thread.name into the internal variables of this metadata structure.
        ''' </summary>
        ''' <param name="ThreadName">The return value from System.Threading.Thread.Name</param>
        ''' <remarks>Used by New(string) constructor</remarks>
        Private Sub Decode(ByRef ThreadName As String)
            tID = CInt(txtGetWord(ThreadName, 1, EncodingDelimiter))
            tType = CByte(txtGetWord(ThreadName, 2, EncodingDelimiter))
            tDataID = CInt(txtGetWord(ThreadName, 3, EncodingDelimiter))
            tStatus = CByte(txtGetWord(ThreadName, 4, EncodingDelimiter))
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ThreadID">A unique identifier for this thread</param>
        ''' <param name="ThreadType">Describes the principal purpose of this thread</param>
        ''' <param name="ThreadDataID">Describes the data target of this thread. Varies by 
        ''' ThreadType. For DropboxWorkers DataID = the UserID of the Dropbox's owner. For
        ''' PartWorkers DataID = the PartID that the worker will update. etc...
        ''' </param>
        ''' <param name="ThreadStatus">The current runtime status of this thread.</param>
        ''' <remarks>This is the encoder</remarks>
        Public Sub New(ByRef ThreadID As Int32, ByRef ThreadType As upThreadTypes, ByRef ThreadDataID As Int32, ByRef ThreadStatus As scanStatus)
            tID = ThreadID
            tType = ThreadType
            tDataID = ThreadDataID
            tStatus = ThreadStatus
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ThreadName"></param>
        ''' <remarks>This is the decoder</remarks>
        Public Sub New(ByRef ThreadName As String)
            Decode(ThreadName)
        End Sub
    End Class

    Public Enum logMsgTypes As Byte
        msgERROR = 1
        msgSTART = 2
        msgSTOP = 3
    End Enum

    Public Enum scanStatus As Byte
        scanREADSTATUS = 0
        scanIDLE = 1
        scanWAITFORDK = 2
        scanWAITFOROP = 3
        scanRUNNING = 4
        scanWAITFORDROPBOX = 5
        scanSYNCING = 6
        scanSLEEPING = 7
    End Enum

    ''' <summary>
    ''' This enumeration defines the type of thread being forked.
    ''' </summary>
    ''' <remarks>Used to tag each thread so we can keep track of them.</remarks>
    Public Enum upThreadTypes As Byte
        rpAllWorkers = 0 'Indicates All Workers -- used in reporting -- DO NOT USE with labelling an actual thread!
        ttGeneric = 40
        ttWorkerDropbox = 42
        ttWorkerPart = 43
        ttDispatcher = 99
    End Enum

    ''' <summary>
    ''' This module contains all code that is specific to the logging, error reporting, and 
    ''' status management of the WebService/WinService -- e.g. everything except the actual 
    ''' data-updating worker (upWorkerNNN.vb) and dispatcher (upDispatcher.vb) code
    ''' </summary>
    ''' <remarks></remarks>
    Public Module upService
        ''' <summary>
        ''' Converts the scanStatus Type to human-readable text
        ''' </summary>
        ''' <param name="theStatus">The upService.scanStatus enum typed object to display</param>
        ''' <returns>Human readable text representation of the status</returns>
        ''' <remarks></remarks>
        Public Function StatusToString(ByRef theStatus As scanStatus) As String
            Select Case theStatus
                Case scanStatus.scanIDLE
                    Return "Idle."
                Case scanStatus.scanSYNCING
                    Return "Syncing."
                Case scanStatus.scanRUNNING
                    Return "Running."
                Case scanStatus.scanWAITFORDK
                    Return "Waiting on Digikey's Server."
                Case scanStatus.scanWAITFORDROPBOX
                    Return "Waiting on Dropbox's Server."
                Case scanStatus.scanWAITFOROP
                    Return "Waiting on Octopart's Server."
                Case scanStatus.scanSLEEPING
                    Return "Sleeping."
                Case Else
                    Return "UNKNOWN!"
            End Select
        End Function

        ''' <summary>
        ''' Converts the upThreadTypes Type to human-readable text
        ''' </summary>
        ''' <param name="theType">The upService.upThreadTypes enum typed object to display</param>
        ''' <returns>Human readable text representation of the Thread Type</returns>
        ''' <remarks></remarks>
        Public Function ThreadTypeToString(ByRef theType As UpdateService.upThreadTypes) As String
            Select Case theType
                Case upThreadTypes.ttDispatcher
                    Return "Dispatcher"
                Case upThreadTypes.ttWorkerDropbox
                    Return "Dropbox Worker"
                Case upThreadTypes.ttWorkerPart
                    Return "Part Worker"
                Case Else
                    Return "UNKNOWN!"
            End Select
        End Function
    End Module

End Namespace