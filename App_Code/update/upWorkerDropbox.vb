Imports Microsoft.VisualBasic
Imports System.Data
Imports UpdateService.upService

Namespace UpdateService

    ''' <summary>
    ''' Worker-specific Semaphore
    ''' </summary>
    ''' <remarks>
    ''' The FriedParts-Update-Service creates a new worker process every ten seconds, but we 
    ''' want to emulate a single worker process behavior.
    ''' Consequently, we use this variable as a semaphore to provide mutual exclusion (MUTEX).
    ''' </remarks>
    Public Class upMutexDropbox
        Inherits upMutex
        Public UserID As Int32 = sysErrors.USER_NOTLOGGEDIN
        Public Overrides Sub Reset()
            MyBase.Reset()
            UserID = sysErrors.USER_NOTLOGGEDIN
        End Sub
    End Class

    ''' <summary>
    ''' Worker thread for the syncing of Dropbox accounts with the FriedParts server. This 
    ''' dispatcher is separate from other sync/update processes that happen in FriedParts so that
    ''' updates can happen in parallel when sourced from different data providers. For example,
    ''' Dropbox updates and Part updates happen in parallel, but each one is throttled to a certain
    ''' rate to prevent abusing our data providers servers.
    ''' </summary>    
    ''' <remarks>Is called by fpusDispatch() and never directly</remarks>
    Public Class upWorkerDropbox
        Inherits upProcess

        ''' <summary>
        ''' The retry delay between attempts to gain the semaphore over this UserID's dropbox.
        ''' </summary>
        ''' <remarks>Value is in milliseconds</remarks>
        Private Const GAIN_LOCK_RETRY As Int16 = 100

        ''' <summary>
        ''' Worker-specific Semaphore instantiation!
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared fpusStatusDropbox As New upMutexDropbox

        ''' <summary>
        ''' Implements the MUTEX release operation
        ''' </summary>
        ''' <remarks>Required by the base class</remarks>
        Protected Overrides Sub ResetMutex()
            fpusStatusDropbox.Reset()
        End Sub

        ''' <summary>
        ''' Retrieves the FriedParts UserID of the owner of this Dropbox (the one this worker
        ''' thread is working on). Returns a sysErrors condition otherwise (a negative integer).
        ''' </summary>
        ''' <returns>The UserID or sysErrors.USER_NOTLOGGEDIN or other sysErrors condition.</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetOwnerID As Int32
            Get
                Return procMeta.ThreadDataID
            End Get
        End Property

        ''' <summary>
        ''' Finds the next Dropbox to synchronize with FriedParts. Returns the UserID of the owner of
        ''' this Dropbox. Updates proceed in FIFO order with the oldest (least recently updated) 
        ''' account being selected for update. This is implemented by finding the last UserID updated 
        ''' and then finding the next highest UserID value with a linked Dropbox. 
        ''' </summary>
        ''' <returns>The FriedParts UserID of the owner of the Dropbox we are going to update. Returns 
        ''' sysErrors.ERR_NOTFOUND if no valid Dropbox exists</returns>
        ''' <remarks>Helper function to dpusDispatch()</remarks>
        Private Function GetNextDropboxUserToUpdate() As Int32
            'Find who we last updated...
            Dim sqlTxt As String = _
                "  SELECT [UserID]" & _
                "    FROM [FriedParts].[dbo].[update-Status] " & _
                "   WHERE [UserID] IS NOT NULL AND [UserID] > 0" & _
                "ORDER BY [Date] DESC"
            Dim dt As New DataTable
            SelectRows(dt, sqlTxt)
            Dim LastUserID As Int32
            If dt.Rows.Count > 0 Then
                'We have previously updated someones dropbox... who? LastUserID
                LastUserID = dt.Rows(0).Field(Of Int32)("UserID")
            Else
                'No previous updates on record
                LastUserID = sysErrors.ERR_NOTFOUND
            End If

            '...now go find who to update next! (next highest UserID)
            dt = New DataTable 'Reset
            sqlTxt = _
                "  SELECT [UserID]" & _
                "    FROM [FriedParts].[dbo].[user-Accounts] " & _
                "   WHERE [DropboxUserKey] IS NOT NULL " & _
                "ORDER BY [UserID]"
            SelectRows(dt, sqlTxt)
            If dt.Rows.Count = 0 Then
                'No Dropboxes are registered!
                Return sysErrors.ERR_NOTFOUND
            End If
            If LastUserID = sysErrors.ERR_NOTFOUND Then
                Return dt.Rows(0).Field(Of Int32)("UserID")
            Else
                Dim DoIt As Boolean = False 'Used to mark the next record for return (the one after the one matching LastUserID)
                For Each dr As DataRow In dt.Rows
                    If DoIt Then
                        Return dr.Field(Of Int32)("UserID")
                    ElseIf dr.Field(Of Int32)("UserID") = LastUserID Then
                        DoIt = True
                    End If
                Next
                If DoIt Then
                    'We cycled over -- last record was marked, but there is no record following it in the for loop!
                    Return dt.Rows(0).Field(Of Int32)("UserID")
                End If
            End If
            Throw New ObjectNotFoundException("[GetNextDropboxUserToUpdate()] Could not find the next DropboxUserID to update")
            Return sysErrors.ERR_NOTFOUND 'Should never reach here!
        End Function

        Protected Overrides Function TheActualThread() As String
            Dim resultMsg As String = ""

            If GetOwnerID > 0 Then
                '[Case #1] Sync a specific user NOW!
                'Prevent overload -- ok if another worker thread, just make sure that it isn't hitting the exact same user
                Dim GotLock As Boolean = False
                While fpusStatusDropbox.UserID = GetOwnerID
                    Threading.Thread.Sleep(GAIN_LOCK_RETRY)
                    'WARNING: Could possibly deadlock here if you have a bug in the scan code...
                End While
                Do While Not GotLock
                    If fpusStatusDropbox.UserID <> GetOwnerID Then
                        'Do it!
                        fpusStatusDropbox.Status = scanStatus.scanSYNCING
                        fpusStatusDropbox.UserID = GetOwnerID
                        resultMsg = SyncMeBabyOneMoreTime()
                        GotLock = True 'break from loop
                    End If
                Loop
            Else
                '[Case #2] Automatically select the "next" user and Sync NOW!
                'Prevent overload (only one thread of this worker type should execute at a time)
                If fpusStatusDropbox.Status = scanStatus.scanIDLE Then
                    UpdateThreadStatus(scanStatus.scanRUNNING)
                    '--got lock, so lets pick a user and update!
                    procMeta.ThreadDataID = GetNextDropboxUserToUpdate() 'find the next user to update
                    fpusStatusDropbox.UserID = GetOwnerID
                    '--validate this user is next in the update queue
                    If GetOwnerID <> sysErrors.ERR_NOTFOUND Then
                        resultMsg = SyncMeBabyOneMoreTime()
                    Else
                        resultMsg = "There are no registered Dropbox users, so nothing to update!"
                    End If
                    UpdateThreadStatus(scanStatus.scanIDLE)
                Else
                    'Another worker is still busy... abort...
                    resultMsg = "Another worker is still busy. Aborting."
                End If
                LogEvent(resultMsg)
                UpdateThreadStatus(scanStatus.scanIDLE)
            End If
            Return resultMsg
        End Function

        Private Function SyncMeBabyOneMoreTime() As String
            'There's a dropbox to update!
            UpdateThreadStatus(scanStatus.scanWAITFORDROPBOX)
            '--Do we have valid login credentials?
            Dim theResult As DropboxErrors
            theResult = loginDropboxUser(GetOwnerID)
            If theResult <> DropboxErrors.Successful Then
                LogEvent("Dropbox login for user " & suGetUsername(GetOwnerID) & " failed! DropboxErrors code: " & theResult)
                UpdateThreadStatus(scanStatus.scanIDLE)
                Return "Dropbox credentials are not valid. User has either revoked permission or keys have changed."
            End If
            '--Update that users dropbox!
            Dim dbxu As New dropboxUser(GetOwnerID)
            UpdateThreadStatus(scanStatus.scanSYNCING)
            dbxu.Sync()
            Return "Synced " & sysUser.suGetUsername(GetOwnerID) & "'s Dropbox!"
        End Function

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="UserID">Optional. If provided (must be a valid!) the UserID will be sync'd immediately. Otherwise, we'll just figure who goes next in our round-robin schedule.</param>
        ''' <remarks>Don't forget to call Start() to actually start the sync work! Just instantiating the class is not enough!</remarks>
        Public Sub New(Optional ByRef UserID As Int32 = sysErrors.USER_NOTLOGGEDIN)
            'Configure Base
            MyBase.New() 'Always do this and do it first!
            procMeta.ThreadType = upThreadTypes.ttWorkerDropbox

            'Perform Specifics
            '[DETERMINE WHICH USER'S DROPBOX TO UPDATE]
            If UserID > 0 Then
                '[Case #1] User was specifically requested -- this happens when users first link their
                '           Dropbox accounts to FriedParts and we want to quickly perform an initial 
                '           population...
                procMeta.ThreadDataID = UserID
            Else
                '[Case #2] No specific user was requested -- this happens when the Update Service
                '           dispatcher is performing routing syncronization.
                procMeta.ThreadDataID = sysErrors.USER_NOTLOGGEDIN 'Need to preserve error into later "actual work" function

            End If
        End Sub
    End Class
End Namespace