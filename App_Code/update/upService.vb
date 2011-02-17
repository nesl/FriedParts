Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Threading

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
    ''' This module contains all code that is specific to the logging, error reporting, and 
    ''' status management of the WebService/WinService -- e.g. everything except the actual 
    ''' data-updating worker code (that goes in fpUpdate.vb)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class upService
        ''' <summary>
        ''' The timer interval in seconds. This specifies the sleep delay between 
        ''' executions of the maintainence worker (updater) dispatcher
        ''' </summary>
        ''' <remarks>Original intent was 10 seconds.</remarks>
        Private TimerInterval As Byte = 10

        ''' <summary>
        ''' The timer interval in seconds. This specifies the sleep delay between 
        ''' executions of the maintainence worker (updater) dispatcher
        ''' </summary>
        ''' <value>The interval specified in seconds (positive integers only)</value>
        ''' <returns>The current set value for the interval</returns>
        ''' <remarks>The default value is 10 seconds. Changes are not preserved in non-volatile storage. This is predominantly for debugging.</remarks>
        Public Property DispatchTimerInterval As Byte
            Get
                Return TimerInterval
            End Get
            Set(ByVal value As Byte)
                TimerInterval = value
            End Set
        End Property

        ''' <summary>
        ''' The FriedParts-Update-Service creates a new worker process every ten seconds, but we want to emulate a single worker process behavior.
        ''' Consequently, we use this variable as a semaphore to provide mutual exclusion (MUTEX).
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared fpusStatus As New upMutex

        ''' <summary>
        ''' Converts the scanStatus Type to human-readable text
        ''' </summary>
        ''' <param name="theStatus">The upService.scanStatus enum typed object to display</param>
        ''' <returns>Human readable text representation of the status</returns>
        ''' <remarks></remarks>
        Public Shared Function StatusToString(ByRef theStatus As scanStatus) As String
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

        '======================================
        ' SCHEDULING / DISPATCH
        '======================================
        ''' <summary>
        ''' Control parameter which disables the use of the timing prescaler in 
        ''' the maintainence/update worker dispatcher. Without prescaling, the 
        ''' workers will be dispatched at the rate indicated by TimerInterval
        ''' </summary>
        ''' <remarks>Normal condition is intended for this to be set to False.</remarks>
        Private fpusOptionDisablePrescaler As Boolean = False

        ''' <summary>Set Now to True to disable the prescaler. This causes the dispatcher to dispatch worker threads on this execution
        ''' instead of waiting for the prescaler timer to countdown. This is extremely useful for debugging or when you need to force the 
        ''' workers to run when performing manual update management. The normal condition (and default value) is False. Now may be omitted -- 
        ''' it's optional -- and the normal case will be assumed.</summary>
        ''' <value>The new value. Set to True to disable the prescaler.</value>
        ''' <returns>The current state of the Prescaler. False = enabled.</returns>
        ''' <remarks>False is the default and normal condition.</remarks>
        Public Property DisablePrescaler As Boolean
            Get
                Return fpusOptionDisablePrescaler
            End Get
            Set(ByVal value As Boolean)
                fpusOptionDisablePrescaler = value
            End Set
        End Property

        ''' <summary>
        ''' Control parameter which disables the use of subsequent threading in the
        ''' worker dispatcher. Worker threads will execute sequentially in the same thread as 
        ''' the dispatcher. This facilities line-by-line step-through debugging.
        ''' </summary>
        ''' <remarks>Normal condition is intended for this to be set to False.</remarks>
        Private fpusOptionDisableThreading As Boolean = False

        ''' <summary>Run everything in this thread -- do not spawn worker processes. To facilitate debugging! Do not do this at runtime!</summary>
        ''' <value>Set to False to enable Threading. If True, the dispatches will run in this thread.</value>
        ''' <returns>Current state of threaded dispatch. True means no threads are forked from this one.</returns>
        Public Property DisableThreading As Boolean
            Get
                Return fpusOptionDisableThreading
            End Get
            Set(ByVal value As Boolean)
                fpusOptionDisableThreading = value
            End Set
        End Property

        ''' <summary>
        ''' Do not change fpusStatus variables directly. Use the appropriate Update... function.
        ''' This one is for Dispatcher threads.
        ''' </summary>
        ''' <param name="newStatus">The new status code</param>
        ''' <remarks></remarks>
        Private Sub UpdateStatus(ByRef newStatus As scanStatus)
            'Update state
            If newStatus = scanStatus.scanIDLE Then
                fpusStatus.Reset()
            Else
                fpusStatus.Status = newStatus
            End If
            'Write to Database (log)
            Dim sqltxt As String = _
                "INSERT INTO [FriedParts].[dbo].[update-Status]" & _
                "           ([Date]" & _
                "           ,[Status]" & _
                "           ,[ThreadID])" & _
                "     VALUES (" & _
                "            " & txtSqlDate(Now) & "," & _
                "            " & newStatus & "," & _
                "            '" & Thread.CurrentThread.Name & "'" & _
                "           )"
            dbAcc.SQLexe(sqltxt)
        End Sub


        ''' <summary>
        ''' Performs the principal dispatching operations associated with the FriedParts-Update-Service.
        ''' This function is called every 10 seconds by the WinService via the WebService (API). It
        ''' checks for exclusion (previous call finished), dispatches the worker threads, and reports
        ''' back to the API with a string message -- this, in turn, gets logged into the Windows event
        ''' log/viewer.
        ''' </summary>
        ''' <returns>A message to be logged by the calling WinService into the Windows Event Log on the server.</returns>
        ''' <remarks>This is the main entry point for the FriedParts-Update-Service</remarks>
        Public Function Start() As String
            If Not fpusOptionDisableThreading Then
                'FORK ME!
                Dim blah As New Threading.Thread(AddressOf TheActualThread)
                blah.Name = "FP Dispatcher"
                blah.Start()
                Return "[Dispatcher]: " & upService.StatusToString(fpusStatus.Status)
            Else
                'DON'T FORK ME! (debugging)
                Return TheActualThread()
            End If
        End Function

        ''' <summary>
        ''' The actual dispatcher thread. This is the function that gets forked.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function TheActualThread() As String
            'Threading doesn't allow the user to pass in parameters, so we handle it this way
            Dim Now As Boolean = fpusOptionDisablePrescaler
            Dim NoThreads As Boolean = fpusOptionDisableThreading

            If fpusStatus.Status = scanStatus.scanIDLE Then
                UpdateStatus(scanStatus.scanRUNNING)
                While True
                    'Throttle this during development
                    If Now Then
                        'Debug case -- caller has requested immediate dispatch
                        fpusExecuteDispatch() 'EXECUTE! (decision to thread or not is made inside here)
                    Else
                        'Normal case -- we prescale (slow) the execution rate of the dispatches
                        Const Prescaler As Byte = 6 'xxx
                        Static Dim TimerPrescaler As Byte = 0
                        TimerPrescaler = TimerPrescaler + 1
                        If TimerPrescaler = Prescaler Then
                            fpusExecuteDispatch() 'EXECUTE! (decision to thread or not is made inside here)
                            TimerPrescaler = 0 'Reset Prescaler
                        End If
                    End If
                    'Sleep thread!
                    UpdateStatus(scanStatus.scanSLEEPING)
                    Thread.Sleep(1000 * TimerInterval)
                End While 'Infinite Loop!
                Throw New Exception("[OH CRAP!] How did we ever reach here?!")
                Return "[OH CRAP!] How did we ever reach here?!" 'Exit from infinite loop not possible!
            Else
                Return "Scanner is busy... try again later!"
            End If
        End Function

        ''' <summary>
        ''' Performs the actual dispatching
        ''' </summary>
        ''' <remarks>Called exclusively by fpusDispatch() -- separates out the execution functions because they can be called under
        ''' several different conditions including with a prescaling timer (normal) and without (debug)</remarks>
        Private Sub fpusExecuteDispatch()
            'Update a part
            Dim threadPartWorker As New upWorkerPart
            'threadPartWorker.Start() 'xxx Seems to be a problem in the apiOctopart

            'Update a Dropbox
            Dim threadDropboxWorker As New upWorkerDropbox
            threadDropboxWorker.Start(Not fpusOptionDisableThreading)
        End Sub

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="NoThreads">Set to True to disable thread forking. All work is done in this process.</param>
        ''' <param name="Now">Set to True to disable the prescalar. All dispatches will be issued on every DispatchTimerInterval.</param>
        ''' <remarks>Don't forget to call Start() to actually start the sync work! Just instantiating the class is not enough!</remarks>
        Public Sub New(Optional ByRef Now As Boolean = False, Optional ByRef NoThreads As Boolean = False)
            '[Deal with Parameters]
            fpusOptionDisablePrescaler = Now
            fpusOptionDisableThreading = NoThreads
        End Sub
    End Class
End Namespace