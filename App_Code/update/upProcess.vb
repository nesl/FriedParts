Imports Microsoft.VisualBasic

Namespace UpdateService


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
        ''' The maximum number of concurrent copies of this process that are allowed to run simultaneously
        ''' </summary>
        ''' <remarks>Redefine this number BY SHADOWING IT in derivative classes if need 
        ''' more than single concurrency.</remarks>
        Protected Shared mutexMaxConcurrent As Byte = 1

        ''' <summary>
        ''' The actual semaphore object used to control access. Derivative classes MUST SHADOW this 
        ''' variable in order to dissociate from the global pool of thread resources.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Shared mutexSemaphore As Threading.Semaphore

        ''' <summary>
        ''' Represents whether this process is currently holding a lock (one of the semaphores) 
        ''' or not.
        ''' </summary>
        ''' <remarks></remarks>
        Protected mutexLocked As Boolean = False

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
                If procMeta.ThreadStatus <> scanStatus.scanIDLE Then
                    'Detected transition back to IDLE state...
                    ReleaseMutex()
                End If
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
                "            " & procMeta.ThreadDataID & "," & _
                "            " & procMeta.ThreadType & "," & _
                "            '" & ThreadTypeToString(procMeta.ThreadType) & "'" & _
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
        '== MUTUAL EXCLUSION (MUTEX)
        '===========================================

        ''' <summary>
        ''' Initializes the mutual exclusion lock used to manage concurrency.
        ''' </summary>
        ''' <remarks>This is only really called from the constructor here. Derivative classes
        ''' should never need to mess with this, but in case I didn't forsee some additional
        ''' process specific initialization it is declared protected to allow override/extension
        ''' </remarks>
        Protected Sub InitMutex()
            mutexSemaphore = New Threading.Semaphore(mutexMaxConcurrent, mutexMaxConcurrent)
        End Sub

        ''' <summary>
        ''' Attempt to gain exclusive rights to run this process.
        ''' </summary>
        ''' <returns>Whether or not we were able to successful acquire the lock.</returns>
        ''' <remarks></remarks>
        Protected Function LockMutex() As Boolean
            If mutexSemaphore.WaitOne(1) Then
                'We successfully locked it
                mutexLocked = True
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Releases the exclusive lock on the the right to run this process.
        ''' </summary>
        ''' <remarks>Fails silently if you call it, but there is nothing to release (because you
        ''' never locked it in the first place).</remarks>
        Protected Sub ReleaseMutex()
            If mutexLocked Then
                mutexSemaphore.Release(1)
            End If
        End Sub




        '===========================================
        '== THREAD EXECUTION
        '===========================================

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
                If LockMutex() Then
                    'Successfully acquire lock! Safe to proceed...
                    Dim blah As New Threading.Thread(AddressOf TheActualThread)
                    blah.Name = procMeta.Encode
                    upThreadList.StartThread(blah, procMeta)
                    Return "[" & procMeta.GetThreadID & "]: " & upService.StatusToString(procMeta.ThreadStatus)
                Else
                    'Failed to lock! Someone else is using the semaphore
                    LogEvent("Failed to start. LOCKED. Only " & mutexMaxConcurrent & " concurrent instances allowed.")
                    Return "[" & procMeta.GetThreadID & "]: " & upService.StatusToString(procMeta.ThreadStatus)
                End If
            Else
                Return TheActualThread()
            End If
        End Function

        ''' <summary>
        ''' Aborts the execution of this thread
        ''' </summary>
        ''' <param name="StopAllThreads">If true, will stop all currently running managed code
        ''' threads. If false, just this one.</param>
        ''' <remarks></remarks>
        Public Sub Abort(Optional ByRef StopAllThreads As Boolean = False)
            If StopAllThreads Then
                upThreadList.StopAllThreads()
            Else
                upThreadList.StopThread(procMeta.GetThreadID)
            End If
        End Sub


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
            'MUTEX
            InitMutex()
        End Sub
    End Class
End Namespace