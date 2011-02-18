Imports Microsoft.VisualBasic

Namespace UpdateService

    ''' <summary>
    ''' Update Service's base MUTEX class -- provides mutual exclusion by acting as the 
    ''' semaphore
    ''' </summary>
    ''' <remarks>Specific worker classes will extend this base class by adding 
    ''' worker-specific parameters to the class definition</remarks>
    Public Class upMutex
        Public Status As scanStatus = scanStatus.scanIDLE
        ''' <summary>
        ''' Resets (e.g. releases) the MUTEX 
        ''' </summary>
        ''' <remarks>Extended by inheritors</remarks>
        Public Overridable Sub Reset()
            Status = scanStatus.scanIDLE
        End Sub
    End Class



    '==============================================================================================
    '==============================================================================================

    '==============================================================================================
    '==============================================================================================


    ''' <summary>
    ''' This is the base class of all Update Service processes because each one runs in 
    ''' a separate thread. There are two general types of processes: Dispatcher and Worker, but
    ''' at this level they are all the same.
    ''' </summary>
    ''' <remarks>Do the general thread management and logging here to avoid redundancy.</remarks>
    Public MustInherit Class upProcess

        '===========================================
        '== STATE / PROPERTIES
        '===========================================

        ''' <summary>
        ''' Holds this threads metadata. Useful for reporting and debugging.
        ''' </summary>
        ''' <remarks></remarks>
        Protected procMeta As upThreadMetaData

        ''' <summary>
        ''' The global FriedParts ThreadID index.
        ''' </summary>
        ''' <remarks>Used to ensure each thread has a unique identifier</remarks>
        Protected Shared procID As UInt16 = 0

        ''' <summary>
        ''' Gets a new ThreadID. Needed when creating a new thread.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Shared Function GetNextProcID() As UInt16
            procID += 1
            If procID = UInt16.MaxValue Then
                procID = 1
            End If
            Return procID
        End Function

        ''' <summary>
        ''' Retrieves the current operational status of this worker thread 
        ''' (IDLE, RUNNING, SYNCING, WAITING, etc). 
        ''' </summary>
        ''' <returns>The current scanStatus value</returns>
        ''' <remarks>The conditions come from upService.scanStatus </remarks>
        Public ReadOnly Property GetStatus As scanStatus
            Get
                Return procMeta.ThreadStatus
            End Get
        End Property



        '===========================================
        '== LOGGING/REPORTING
        '===========================================

        ''' <summary>
        ''' Do not change procMeta.ThreadStatus variables directly. This subroutine!
        ''' </summary>
        ''' <param name="newStatus">The new status code</param>
        ''' <remarks>This status change will be visible outside the thread for monitoring. Yay!</remarks>
        Protected Sub UpdateThreadStatus(ByRef newStatus As scanStatus)
            If newStatus = scanStatus.scanIDLE Then
                ResetMutex()
            Else
                procMeta.ThreadStatus = newStatus
            End If
            Dim sqltxt As String = _
                "INSERT INTO [FriedParts].[dbo].[update-Status]" & _
                "           ([Date]" & _
                "           ,[Status]" & _
                "           ,[ThreadID]" & _
                "           ,[DataID]" & _
                "           ,[ThreadTypeID]" & _
                "           ,[ThreadType])" & _
                "     VALUES (" & _
                "            " & txtSqlDate(Now) & "," & _
                "            " & procMeta.ThreadStatus & "," & _
                "            " & procMeta.GetThreadID & "," & _
                "            " & procMeta.ThreadDataID & "" & _
                "            " & procMeta.ThreadType & "" & _
                "            " & ThreadTypeToString(procMeta.ThreadType) & "" & _
                "           )"
            dbAcc.SQLexe(sqltxt)
        End Sub

        ''' <summary>
        ''' Log a FriedParts-Update-Service update event to the update-Log database table. This
        ''' is used mostly for human-readable tracking of what the heck is going on in this 
        ''' thread. Signifies major events (Errors, Successful completion, etc...)
        ''' </summary>
        ''' <param name="Message">The text of the error message</param>
        ''' <param name="MsgType">The type of message. Optional. Defaults to an Error Message</param>
        ''' <remarks></remarks>
        Protected Sub LogEvent(ByRef Message As String, Optional ByRef MsgType As logMsgTypes = logMsgTypes.msgERROR)
            Dim sqlTxt As String = _
                "INSERT INTO [FriedParts].[dbo].[update-Log]" & _
                "           ([Date]" & _
                "           ,[ThreadID]" & _
                "           ,[ThreadTypeID]" & _
                "           ,[DataID]" & _
                "           ,[MsgType]" & _
                "           ,[Msg])" & _
                "     VALUES (" & _
                "            " & sysText.txtSqlDate(Now) & "," & _
                "           " & procMeta.GetThreadID & "," & _
                "           " & procMeta.ThreadType & "," & _
                "           " & procMeta.ThreadDataID & "," & _
                "           " & MsgType & "," & _
                "           '" & txtDefangSQL(Message) & "'" & _
                "           )"
            dbAcc.SQLexe(sqlTxt)
        End Sub



        '===========================================
        '== THREAD EXECUTION
        '===========================================

        ''' <summary>
        ''' If this process is going to require mutual exclusion control (MUTEX) then implement
        ''' the apppropriate MUTEX release calls in this subroutine. Otherwise implement an 
        ''' empty subroutine.
        ''' </summary>
        ''' <remarks>Required in order to force the developer to be aware of 
        ''' this requirement when working with MUTEX.</remarks>
        Protected MustOverride Sub ResetMutex()

        ''' <summary>
        ''' Entry point for the thread. This is where the work of the thread is done. The thread is
        ''' wrapped in a class because we cannot pass parameters to a function during forking.
        ''' This function is worker/dispatcher specific and must be implemented there.
        ''' </summary>
        ''' <returns>A human-readable message explaining what happened -- for log/display as needed (safe to ignore)</returns>
        ''' <remarks>Is called by Start() and never directly</remarks>
        Protected MustOverride Function TheActualThread() As String

        ''' <summary>
        ''' Spawns a new thread (if requested) or otherwise just proceeds with execution. Use this 
        ''' function to actually begin doing the work of this process (dispatcher/worker).
        ''' </summary>
        ''' <returns>A human-readable message explaining what happened -- for log/display as needed (safe to ignore)</returns>
        ''' <param name="AsThread">Start as a Thread? Forks a new thread process if True. Runs in-line if False. Optional (defaults to True).</param>
        ''' <remarks></remarks>
        Public Function Start(Optional ByRef AsThread As Boolean = True) As String
            If AsThread Then
                Dim blah As New Threading.Thread(AddressOf TheActualThread)
                blah.Name = procMeta.Encode
                blah.Start()
                Return "[" & procMeta.GetThreadID & "]: " & upService.StatusToString(procMeta.ThreadStatus)
            Else
                Return TheActualThread()
            End If
        End Function



        '===========================================
        '== CONSTRUCTION
        '===========================================

        ''' <summary>
        ''' Generic constructor. Make sure all derivative classes call MyBase.New() in all of their
        ''' constructors.
        ''' </summary>
        ''' <remarks>Common setup processing</remarks>
        Public Sub New()
            'Update METADATA
            procMeta = New upThreadMetaData(GetNextProcID(), upThreadTypes.ttGeneric, sysErrors.ERR_NOTFOUND, scanStatus.scanIDLE)
        End Sub
    End Class
End Namespace