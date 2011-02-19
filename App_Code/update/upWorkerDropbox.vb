Imports Microsoft.VisualBasic
Imports System.Data
Imports UpdateService.upService

Namespace UpdateService

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
        ''' The actual semaphore object used to control access. Derivative classes MUST SHADOW this 
        ''' variable in order to dissociate from the global pool of thread resources.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Shared Shadows mutexSemaphore As Threading.Semaphore

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
        Protected Overrides Sub MutexInit()
            If mutexSemaphore Is Nothing Then mutexSemaphore = New Threading.Semaphore(mutexMaxConcurrent, mutexMaxConcurrent)
            mutexLocked = False
        End Sub

        ''' <summary>
        ''' Attempt to gain exclusive rights to run this process.
        ''' </summary>
        ''' <returns>Whether or not we were able to successful acquire the lock.</returns>
        ''' <remarks></remarks>
        Protected Overrides Function MutexLock() As Boolean
            If mutexSemaphore.WaitOne(1) Then
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
        Protected Overrides Sub MutexRelease()
            mutexSemaphore.Release(1)
            mutexLocked = False
        End Sub





        ''' <summary>
        ''' Finds the next Dropbox to synchronize with FriedParts. Returns the UserID of the owner of
        ''' this Dropbox. Updates proceed in FIFO order with the oldest (least recently updated) 
        ''' account being selected for update. This is implemented by finding the last UserID updated 
        ''' and then finding the next highest UserID value with a linked Dropbox. 
        ''' </summary>
        ''' <returns>The FriedParts UserID of the owner of the Dropbox we are going to update. Returns 
        ''' sysErrors.ERR_NOTFOUND if no valid Dropbox exists</returns>
        ''' <remarks>Prioritizes new users to FriedParts -- people who just linked their dropboxes, 
        ''' but have never been synced before.</remarks>
        Private Function GetNextDropboxUserToUpdate() As Int32
            'Init
            Dim dt As New DataTable
            Dim sqlTxt As String

            'Are there any VIP's waiting? (VIP's are people who just joined so they have not been sync'd yet)
            sqlTxt = _
                "USE    [FriedParts] " & _
                "Select [UserID] " & _
                "FROM   [user-Accounts] AS a " & _
                "WHERE  ([DropboxUserKey] Is Not NULL) " & _
                "AND    NOT EXISTS (SELECT * FROM [update-Log] AS b WHERE b.[DataID] = a.[UserID])"
            SelectRows(dt, sqlTxt)
            If dt.Rows.Count > 0 Then
                Return dt.Rows(0).Field(Of Int32)("UserID")
            End If

            'Find who we last updated...
            sqlTxt = _
                "  SELECT [DataID]" & _
                "    FROM [FriedParts].[dbo].[update-Status] " & _
                "   WHERE [ThreadTypeID] = " & upThreadTypes.ttWorkerDropbox & _
                "         AND [DataID] IS NOT NULL AND [DataID] > 0" & _
                "ORDER BY [Date] DESC"
            SelectRows(dt, sqlTxt)
            Dim LastUserID As Int32
            If dt.Rows.Count > 0 Then
                'We have previously updated someone's dropbox... who? LastUserID
                LastUserID = dt.Rows(0).Field(Of Int32)("DataID")
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
            'Automatically select the "next" user and Sync NOW!
            UpdateThreadStatus(scanStatus.scanRUNNING)
            '--got lock, so lets pick a user and update!
            procMeta.ThreadDataID = GetNextDropboxUserToUpdate() 'find the next user to update
            '--validate this user is next in the update queue
            If GetOwnerID <> sysErrors.ERR_NOTFOUND Then
                resultMsg = SyncMeBabyOneMoreTime()
            Else
                resultMsg = "There are no registered Dropbox users, so nothing to update!"
            End If
            'Finalize
            LogEvent(resultMsg)
            UpdateThreadStatus(scanStatus.scanIDLE) 'IMPORTANT! Signals we are done (releases semaphore)
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
        ''' <remarks>Don't forget to call Start() to actually start the sync work! Just instantiating the class is not enough!</remarks>
        Public Sub New()
            'Configure Base
            MyBase.New() 'Always do this and do it first!
            procMeta.ThreadType = upThreadTypes.ttWorkerDropbox

            'Perform Specifics
            procMeta.ThreadDataID = sysErrors.USER_NOTLOGGEDIN 'Need to preserve error into later "actual work" function
        End Sub
    End Class
End Namespace