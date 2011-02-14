Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Threading

''' <summary>
''' This module contains all code that is specific to the logging, error reporting, and 
''' status management of the WebService/WinService -- e.g. everything except the actual 
''' data-updating worker code (that goes in fpUpdate.vb)
''' </summary>
''' <remarks></remarks>
Public Module apiUpdateService

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
    End Enum

    ''' <summary>
    ''' Converts the scanStatus Type to human-readable text
    ''' </summary>
    ''' <param name="theStatus">The apiWebService.scanStatus enum typed object to display</param>
    ''' <returns>Human readable text representation of the status</returns>
    ''' <remarks></remarks>
    Private Function fpusStatusToString(ByRef theStatus As scanStatus) As String
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
            Case Else
                Return "UNKNOWN!"
        End Select
    End Function

    ''' <summary>
    ''' The FriedParts-Update-Service creates a new worker process every ten seconds, but we want to emulate a single worker process behavior.
    ''' Consequently, we use this variable as a semaphore to provide mutual exclusion (MUTEX).
    ''' </summary>
    ''' <remarks></remarks>
    Private fpusStatus As scanStatus = scanStatus.scanIDLE
    ''' <summary>
    ''' The FriedParts-Update-Service creates a new worker process every ten seconds, but we want to emulate a single worker process behavior.
    ''' Consequently, we use this variable as a semaphore to provide mutual exclusion (MUTEX).
    ''' </summary>
    ''' <remarks></remarks>
    Private fpusStatusDropbox As scanStatus = scanStatus.scanIDLE
    ''' <summary>
    ''' The FriedParts-Update-Service creates a new worker process every ten seconds, but we want to emulate a single worker process behavior.
    ''' Consequently, we use this variable as a semaphore to provide mutual exclusion (MUTEX).
    ''' </summary>
    ''' <remarks></remarks>
    Private fpusStatusPart As scanStatus = scanStatus.scanIDLE



    '======================================
    ' SCHEDULING / DISPATCH
    '======================================

    ''' <summary>
    ''' Performs the principal dispatching operations associated with the FriedParts-Update-Service.
    ''' This function is called every 10 seconds by the WinService via the WebService (API). It
    ''' checks for exclusion (previous call finished), dispatches the worker threads, and reports
    ''' back to the API with a string message -- this, in turn, gets logged into the Windows event
    ''' log/viewer.
    ''' </summary>
    ''' <param name="Now">Set Now to True to disable the prescaler. This causes the dispatcher to dispatch worker threads on this execution
    ''' instead of waiting for the prescaler timer to countdown. This is extremely useful for debugging or when you need to force the 
    ''' workers to run when performing manual update management. The normal condition (and default value) is False. Now may be omitted -- 
    ''' it's optional -- and the normal case will be assumed.</param>
    ''' <param name="NoThreads">Run everything in this thread -- do not spawn worker processes. To facilitate debugging! Do not do this at runtime!</param>
    ''' <returns>A message to be logged by the calling WinService into the Windows Event Log on the server.</returns>
    ''' <remarks>This is the main entry point for the FriedParts-Update-Service</remarks>
    Public Function fpusDispatch(Optional ByRef Now As Boolean = False, Optional ByRef NoThreads As Boolean = False) As String
        'Check for exclusion
        If fpusStatus <> scanStatus.scanIDLE Then
            Return "Scanner is busy... try again later!"
        End If

        'Throttle this during development
        If Now Then
            'Debug case -- caller has requested immediate dispatch
            If NoThreads Then
                'RUN IN THIS THREAD TO FACILITATE DEBUGGING!
                'Executes sequentially in this thread...
                fpusDispatchDropbox()
                'fpusDispatchPart() 'xxx Seems to be a problem in the apiOctopart
            Else
                fpusExecuteDispatch()
            End If
        Else
            'Normal case -- we prescale (slow) the execution rate of the dispatches
            Const Prescaler As Byte = 6 'xxx
            Static Dim TimerPrescaler As Byte = 0
            TimerPrescaler = TimerPrescaler + 1
            If TimerPrescaler = Prescaler Then
                fpusExecuteDispatch()
                'Reset Prescaler
                TimerPrescaler = 0
            End If
        End If
        'Report result
        Return "Status: [Dropbox Worker] " & fpusStatusToString(fpusStatusDropbox) & "; [Parts Worker] " & fpusStatusToString(fpusStatusPart)
    End Function

    ''' <summary>
    ''' Performs the actual dispatching
    ''' </summary>
    ''' <remarks>Called exclusively by fpusDispatch() -- separates out the execution functions because they can be called under
    ''' several different conditions including with a prescaling timer (normal) and without (debug)</remarks>
    Private Sub fpusExecuteDispatch()
        'Update a part
        Dim threadPartWorker As New Thread(AddressOf fpusDispatchPart)
        threadPartWorker.Name = "FPUS Parts"
        'threadPartWorker.Start() 'xxx Seems to be a problem in the apiOctopart
        'Update a Dropbox
        Dim threadDropboxWorker As New Thread(AddressOf fpusDispatchDropbox)
        threadDropboxWorker.Name = "FPUS Dropbox"
        threadDropboxWorker.Start()
    End Sub

    ''' <summary>
    ''' Worker thread for the syncing of Dropbox accounts with the FriedParts server. This 
    ''' dispatcher is separate from other sync/update processes that happen in FriedParts so that
    ''' updates can happen in parallel when sourced from different data providers. For example,
    ''' Dropbox updates and Part updates happen in parallel, but each one is throttled to a certain
    ''' rate to prevent abusing our data providers servers.
    ''' </summary>
    ''' <returns>A human-readable message explaining what happened -- for log/display as needed (safe to ignore)</returns>
    ''' <remarks>Is called by fpusDispatch() and never directly</remarks>
    Private Function fpusDispatchDropbox() As String
        Dim resultMsg As String
        Dim resultID As Int32 = sysErrors.ERR_NOTFOUND
        'Prevent overload (only one thread of this worker type should execute at a time)
        If fpusStatusDropbox = scanStatus.scanIDLE Then
            UpdateDropboxStatus(scanStatus.scanRUNNING, resultID)
            '--Find which user is next in the update queue
            Dim nextUserID As Int32 = GetNextDropboxUserToUpdate()
            resultID = nextUserID
            If nextUserID <> sysErrors.ERR_NOTFOUND Then
                'There's a dropbox to update!
                '--Update that users dropbox!
                UpdateDropboxStatus(scanStatus.scanWAITFORDROPBOX, resultID)
                Dim dbxu As New dropboxUser(nextUserID)
                UpdateDropboxStatus(scanStatus.scanSYNCING, resultID)
                dbxu.Sync()
                resultMsg = "Synced " & sysUser.suGetUsername(nextUserID) & "'s Dropbox!"
            Else
                resultMsg = "There are no registered Dropbox users, so nothing to update!"
            End If
            UpdateDropboxStatus(scanStatus.scanIDLE, resultID)
        Else
            'Another worker is still busy... abort...
            resultMsg = "Another worker is still busy. Aborting."
        End If
        fpusLogDropboxError(resultID, resultMsg)
        Return resultMsg
    End Function

    ''' <summary>
    ''' Worker thread for the updating of parts. This 
    ''' dispatcher is separate from other sync/update processes that happen in FriedParts so that
    ''' updates can happen in parallel when sourced from different data providers. For example,
    ''' Dropbox updates and Part updates happen in parallel, but each one is throttled to a certain
    ''' rate to prevent abusing our data provider's servers.
    ''' </summary>
    ''' <returns>A human-readable message explaining what happened -- for log/display as needed (safe to ignore)</returns>
    ''' <remarks>Is called by fpusDispatch() and never directly</remarks>
    Private Function fpusDispatchPart() As String
        'MUTEX
        If fpusStatusPart = scanStatus.scanIDLE Then
            'Find next part to update
            Dim ThePartID As Int32 = NextPartToUpdate()

            'Update!
            Dim TheWorker As New partUpdateWorker(ThePartID)
            TheWorker.Update()

            'Report
            Return "Scanned/Updated PartID " & ThePartID
        Else
            'Another worker is still busy... abort...
            fpusLogPartError(sysErrors.ERR_NOTFOUND, "Another worker is still busy. Aborting.")
            Return "Another worker is still busy. Aborting."
        End If
    End Function



    '======================================
    ' LOGGING FUNCTIONS
    '======================================



    ''' <summary>
    ''' Log a FriedParts-Update-Service dropbox sync error event to the update-Log database table
    ''' </summary>
    ''' <param name="UserID">The FriedParts UserID of the user whose Dropbox encountered the error</param>
    ''' <param name="ErrorMessage">The text of the error message</param>
    ''' <remarks></remarks>
    Private Sub fpusLogDropboxError(ByRef UserID As Int32, ByRef ErrorMessage As String, Optional ByRef MsgType As logMsgTypes = logMsgTypes.msgERROR)
        Dim sqlTxt As String = _
            "INSERT INTO [FriedParts].[dbo].[update-Log]" & _
            "           ([Date]" & _
            "           ,[UserID]" & _
            "           ,[MsgType]" & _
            "           ,[Msg])" & _
            "     VALUES (" & _
            "            " & sysText.txtSqlDate(Now) & "," & _
            "           '" & UserID & "'," & _
            "           '" & logMsgTypes.msgERROR & "'," & _
            "           '" & txtDefangSQL(ErrorMessage) & "'" & _
            "           )"
        dbAcc.SQLexe(sqlTxt)
    End Sub

    ''' <summary>
    ''' Log a FriedParts-Update-Service part update error event to the update-Log database table
    ''' </summary>
    ''' <param name="PartID">The FriedParts PartID of the part, which encountered the error</param>
    ''' <param name="ErrorMessage">The text of the error message</param>
    ''' <remarks></remarks>
    Private Sub fpusLogPartError(ByRef PartID As Int32, ByRef ErrorMessage As String, Optional ByRef MsgType As logMsgTypes = logMsgTypes.msgERROR)
        Dim sqlTxt As String = _
            "INSERT INTO [FriedParts].[dbo].[update-Log]" & _
            "           ([Date]" & _
            "           ,[PartID]" & _
            "           ,[MsgType]" & _
            "           ,[Msg])" & _
            "     VALUES (" & _
            "            " & sysText.txtSqlDate(Now) & "," & _
            "           '" & PartID & "'," & _
            "           '" & logMsgTypes.msgERROR & "'," & _
            "           '" & txtDefangSQL(ErrorMessage) & "'" & _
            "           )"
        dbAcc.SQLexe(sqlTxt)
    End Sub

    ''' <summary>
    ''' Do not change fpusStatus variables directly. Use the appropriate Update... function.
    ''' This one is for Dropbox worker threads.
    ''' </summary>
    ''' <param name="newStatus">The new status code</param>
    ''' <param name="UserID">The UserID of the current Dropbox's owner.</param>
    ''' <remarks></remarks>
    Private Sub UpdateDropboxStatus(ByRef newStatus As scanStatus, Optional ByRef UserID As Int32 = sysErrors.ERR_NOTFOUND)
        fpusStatusDropbox = newStatus
        Dim sqltxt As String = _
            "INSERT INTO [FriedParts].[dbo].[update-Status]" & _
            "           ([Date]" & _
            "           ,[Status]" & _
            "           ,[UserID])" & _
            "     VALUES (" & _
            "            " & txtSqlDate(Now) & "," & _
            "            " & newStatus & "," & _
            "            " & UserID & "" & _
            "           )"
        dbAcc.SQLexe(sqltxt)
    End Sub

    ''' <summary>
    ''' Do not change fpusStatus variables directly. Use the appropriate Update... function.
    ''' This one is for Parts worker threads.
    ''' </summary>
    ''' <param name="newStatus">The new status code</param>
    ''' <param name="PartID">The PartID of the current Part under investigation.</param>
    ''' <remarks></remarks>
    Private Sub UpdatePartStatus(ByRef newStatus As scanStatus, Optional ByRef PartID As Int32 = sysErrors.ERR_NOTFOUND)
        fpusStatusPart = newStatus
        Dim sqltxt As String = _
            "INSERT INTO [FriedParts].[dbo].[update-Status]" & _
            "           ([Date]" & _
            "           ,[Status]" & _
            "           ,[PartID])" & _
            "     VALUES (" & _
            "            " & txtSqlDate(Now) & "," & _
            "            " & newStatus & "," & _
            "            " & PartID & "" & _
            "           )"
        dbAcc.SQLexe(sqltxt)
    End Sub



    '======================================
    ' DROPBOX WORKER FUNCTIONS
    '======================================



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



    '======================================
    ' PART UPDATE WORKER FUNCTIONS
    '======================================



    ''' <summary>
    ''' Updates the UpdatingPartID class state variable
    ''' Updates the current LastScanned Date/Time value in the database (Does not update the LastModified date/time -- do that only if changes are made)
    ''' [Priority One] Update any parts that have *Never* been updated.
    ''' [Priority Two] Update the part with the oldest "Last Scanned" date
    ''' </summary>
    ''' <returns>The PartID of the part to update next</returns>
    ''' <remarks>Used by the Part Update Worker thread dispatcher</remarks>
    Private Function NextPartToUpdate() As Int32
        '[Priority One] Update any parts that have *Never* been updated.
        Dim dt As New DataTable
        dbAcc.SelectRows(dt, _
            "SELECT [PartID] " & _
            "FROM [FriedParts].[dbo].[part-Common] " & _
            "WHERE [Date_LastScanned] IS NULL")
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Field(Of Int32)("PartID")
        End If

        '[Priority Two] Update the part with the oldest "Last Scanned" date
        dt = dbAcc.SelectRows(dt, _
            "SELECT [PartID] " & _
            "FROM [FriedParts].[dbo].[part-Common] " & _
            "WHERE [Date_LastScanned] IS NOT NULL " & _
            "ORDER BY [FriedParts].[dbo].[Date_LastScanned] DESC")
        If dt.Rows.Count = 0 Then
            'Error No Parts Found! -- this can't happen
            fpusLogPartError(sysErrors.ERR_NOTFOUND, "No Parts Found! [part-Common] table EMPTY?!")
            Return sysErrors.ERR_NOTFOUND
        Else
            'Grab the first record -- which should be the least updated one because we sorted by scan-date
            Return dt.Rows(0).Field(Of Int32)("PartID")
        End If
    End Function

    ''' <summary>
    ''' Used to keep track of the current part under investigation by the FriedParts-Update-Service.
    ''' This functionality is wrapped into a class to allow for future deployment of multiple simultaneous
    ''' worker threads if the need arises.
    ''' </summary>
    ''' <remarks>Does not handle MUTEX. Manage this above this class.</remarks>
    Private Class partUpdateWorker
        ''' <summary>
        ''' The internal variable holding the PartID that his object is working on
        ''' </summary>
        ''' <remarks></remarks>
        Private UpdatingPartID As Int32

        ''' <summary>
        ''' Returns the PartID of the part represented by this object
        ''' </summary>
        ''' <value>Read only!</value>
        ''' <returns>The PartID of the part being updated by this object</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetPartID As Int32
            Get
                Return UpdatingPartID
            End Get
        End Property



        ''' <summary>
        ''' Entry point for updating a part. Checks with the data providers and updates any changed
        ''' information (for example, pricing and availability), corrects any known database data
        ''' integrity issues, and fills in any missing information. 
        ''' </summary>
        ''' <remarks>Do NOT call me faster than once per 10 seconds!</remarks>
        Public Sub Update()
            UpdatePartStatus(scanStatus.scanRUNNING)

            'Digikey Search
            '==============
            UpdatePartStatus(scanStatus.scanWAITFORDK)
            Dim dkPartNum As String = apiDigikey.dkPartNumber(UpdatingPartID)
            If Not (dkPartNum = sysErrors.ERR_NOTFOUND Or dkPartNum = sysErrors.ERR_NOTUNIQUE) Then
                'this part has a Digikey Part Number
                Dim DK As New apiDigikey(dkPartNum)
                If DK.PartReady Then
                    'Check Part Type
                    'Check Pricing
                    'Check Long Description
                    'Check Value, Units
                    'Check Value Numeric
                    'Check Temp Codes

                    'hiddenDigikeyPartNumber.Value = DK.getDigikeyPartNum
                    'hiddenMfrPartNumber.Value = DK.getMfrPartNum
                    'xPanelDkNo.Visible = False
                    'xPanelDkYes.Visible = True
                    'xLblDigikey.Text = "FOUND! " & DK.getMfrName & " " & DK.getMfrPartNum & ": " & DK.getShortDesc
                    'imgDigikey.ImageUrl = DK.getImageURL
                    'imgDigikey.Width = 200
                    'imgDigikey.Height = 200
                    'linkDigikey.NavigateUrl = DK.getDatasheetURL
                Else
                    'Digikey part number is not found
                    fpusLogPartError(UpdatingPartID, "Digikey part number not found -- or Digikey timeout")
                End If
            Else
                If dkPartNum = sysErrors.ERR_NOTFOUND Then
                    'Digikey Part Number not known for this part
                    fpusLogPartError(UpdatingPartID, "This part does not have a Digikey part number!")
                Else
                    'Multiple Digikey Part Numbers found
                    fpusLogPartError(UpdatingPartID, "This part had multiple matching Digikey part numbers!")
                End If
            End If

            'Octopart Search
            '===============
            UpdatePartStatus(scanStatus.scanWAITFOROP)
            Dim OP As New Octopart("The Part Number")
            'Make Changes
            'Log Changes
            'Update Status Entry in Database
            UpdatePartStatus(scanStatus.scanIDLE)

            'Mark this one as SCANNED!
            '=========================
            Dim sqlText As String = _
                "UPDATE [FriedParts].[dbo].[part-Common]" & _
                "   SET " & _
                "      [Date_LastScanned] = '" & txtSqlDate(Now) & "'" & _
                "   WHERE [PartID] = " & UpdatingPartID
            dbAcc.SQLexe(sqlText)
        End Sub


        ''' <summary>
        ''' Constructor. Assumes PartID is valid or will throw an ObjectNotFoundException.
        ''' </summary>
        ''' <param name="PartID">A FriedParts PartID. Must be valid.</param>
        ''' <remarks></remarks>
        Public Sub New(ByRef PartID As Int32)

        End Sub
    End Class
End Module
