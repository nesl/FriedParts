Imports Microsoft.VisualBasic
Imports System.Data

Namespace UpdateService

    '===========================================
    '== MUTEX/Semaphore
    '===========================================

    ''' <summary>
    ''' Worker-specific Semaphore
    ''' </summary>
    ''' <remarks>
    ''' The FriedParts-Update-Service creates a new worker process every ten seconds, but we 
    ''' want to emulate a single worker process behavior.
    ''' Consequently, we use this variable as a semaphore to provide mutual exclusion (MUTEX).
    ''' </remarks>
    Public Class upMutexPart
        Inherits upMutex
        Public PartID As Int32 = sysErrors.PARTADD_MFRNUMNOTUNIQUE
        Public Overrides Sub Reset()
            MyBase.Reset()
            PartID = sysErrors.PARTADD_MFRNUMNOTUNIQUE
        End Sub
    End Class



    '===========================================
    '== WORKER CLASS
    '===========================================

    ''' <summary>
    ''' The Update-Service Worker class for update a specific FriedPart (FPID) data.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class upWorkerPart

        '===========================================
        '== PROPERTIES / INTERNAL STATE
        '===========================================

        ''' <summary>
        ''' The FriedParts-Update-Service creates a new worker process every ten seconds, but we want to emulate a single worker process behavior.
        ''' Consequently, we use this variable as a semaphore to provide mutual exclusion (MUTEX).
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared fpusStatusPart As New upMutexPart

        ''' <summary>
        ''' Internal storage of the PartID of the part that this worker is trying to update.
        ''' </summary>
        ''' <remarks></remarks>
        Private fpusPartID As Int32 = sysErrors.PARTADD_MFRNUMNOTUNIQUE

        ''' <summary>
        ''' Returns the PartID of the part represented by this object
        ''' </summary>
        ''' <value>Read only!</value>
        ''' <returns>The PartID of the part being updated by this object</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetPartID As Int32
            Get
                Return fpusPartID
            End Get
        End Property



        '===========================================
        '== LOGGING/REPORTING
        '===========================================

        ''' <summary>
        ''' Do not change fpusStatus variables directly. Use the appropriate Update... function.
        ''' This one is for Parts worker threads.
        ''' </summary>
        ''' <param name="newStatus">The new status code</param>
        ''' <remarks></remarks>
        Private Sub UpdatePartStatus(ByRef newStatus As scanStatus)
            If newStatus = scanStatus.scanIDLE Then
                fpusStatusPart.Reset()
            Else
                fpusStatusPart.Status = newStatus
            End If
            Dim sqltxt As String = _
                "INSERT INTO [FriedParts].[dbo].[update-Status]" & _
                "           ([Date]" & _
                "           ,[Status]" & _
                "           ,[PartID])" & _
                "     VALUES (" & _
                "            " & txtSqlDate(Now) & "," & _
                "            " & newStatus & "," & _
                "            " & fpusPartID & "" & _
                "           )"
            dbAcc.SQLexe(sqltxt)
        End Sub

        ''' <summary>
        ''' Log a FriedParts-Update-Service part update error event to the update-Log database table
        ''' </summary>
        ''' <param name="ErrorMessage">The text of the error message</param>
        ''' <remarks></remarks>
        Private Sub LogPartError(ByRef ErrorMessage As String, Optional ByRef MsgType As logMsgTypes = logMsgTypes.msgERROR)
            Dim sqlTxt As String = _
                "INSERT INTO [FriedParts].[dbo].[update-Log]" & _
                "           ([Date]" & _
                "           ,[PartID]" & _
                "           ,[MsgType]" & _
                "           ,[Msg])" & _
                "     VALUES (" & _
                "            " & sysText.txtSqlDate(Now) & "," & _
                "           '" & fpusPartID & "'," & _
                "           '" & logMsgTypes.msgERROR & "'," & _
                "           '" & txtDefangSQL(ErrorMessage) & "'" & _
                "           )"
            dbAcc.SQLexe(sqlTxt)
        End Sub




        '===========================================
        '== SPAWNING, MUTEX, & EXECUTION MECHANICS
        '===========================================

        ''' <summary>
        ''' This function does the work...
        ''' </summary>
        ''' <returns>A human-readable message explaining what happened -- for log/display as needed (safe to ignore)</returns>
        ''' <param name="AsThread">Start as a Thread? Forks a new thread process if True. Runs in-line if False. Optional (defaults to True).</param>
        ''' <remarks></remarks>
        Public Function Start(Optional ByRef AsThread As Boolean = True) As String
            If AsThread Then
                Dim blah As New Threading.Thread(AddressOf TheActualThread)
                blah.Name = "FP Part Worker"
                blah.Start()
                Return "[Part Worker]: " & upService.StatusToString(fpusStatusPart.Status)
            Else
                Return TheActualThread()
            End If
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
        Private Function TheActualThread() As String
            'MUTEX
            If fpusStatusPart.Status = scanStatus.scanIDLE Then
                'Claim Semaphore -- LOCK!
                UpdatePartStatus(scanStatus.scanRUNNING)

                'Find next part to update
                If fpusPartID <= 0 Then
                    'Find it
                    fpusPartID = Me.NextPartToUpdate()
                    'Sanity Check
                    If Not fpParts.partExistsID(fpusPartID) Then
                        Throw New Exception("PartID " & fpusPartID & "for Part Worker was NOT Valid!")
                    End If
                End If

                'Update!
                LogPartError("Scanning/Updating PartID " & fpusPartID, logMsgTypes.msgSTART)
                Update()

                'Report
                UpdatePartStatus(scanStatus.scanIDLE) 'Release LOCK
                LogPartError("Scanned/Updated PartID " & fpusPartID, logMsgTypes.msgSTOP)
                Return "Scanned/Updated PartID " & fpusPartID
            Else
                'Another worker is still busy... abort...
                LogPartError(sysErrors.ERR_NOTFOUND, "Another worker is still busy. Aborting.")
                Return "Another worker is still busy. Aborting."
            End If
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
                LogPartError(sysErrors.ERR_NOTFOUND, "No Parts Found! [part-Common] table EMPTY?!")
                Return sysErrors.ERR_NOTFOUND
            Else
                'Grab the first record -- which should be the least updated one because we sorted by scan-date
                Return dt.Rows(0).Field(Of Int32)("PartID")
            End If
        End Function

        ''' <summary>
        ''' Entry point for updating a part. Checks with the data providers and updates any changed
        ''' information (for example, pricing and availability), corrects any known database data
        ''' integrity issues, and fills in any missing information. 
        ''' </summary>
        ''' <remarks>Do NOT call me faster than once per 10 seconds! (In the future, could 
        ''' optimize this by having it fork out all of the different data providers to 
        ''' different workers to run them in parallel.)</remarks>
        Public Sub Update()

            'Digikey Search
            '==============
            UpdatePartStatus(scanStatus.scanWAITFORDK)
            Dim dkPartNum As String = apiDigikey.dkPartNumber(fpusPartID)
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
                    LogPartError("Digikey part number not found -- or Digikey timeout")
                End If
            Else
                If dkPartNum = sysErrors.ERR_NOTFOUND Then
                    'Digikey Part Number not known for this part
                    LogPartError("This part does not have a Digikey part number!")
                Else
                    'Multiple Digikey Part Numbers found
                    LogPartError("This part had multiple matching Digikey part numbers!")
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
                "   WHERE [PartID] = " & fpusPartID
            dbAcc.SQLexe(sqlText)
        End Sub



        '======================================
        ' CONSTRUCTOR
        '======================================

        ''' <summary>
        ''' Constructor. Assumes PartID is valid or will throw an ObjectNotFoundException.
        ''' </summary>
        ''' <param name="PartID">A FriedParts PartID. Must be valid.</param>
        ''' <remarks></remarks>
        Public Sub New(Optional ByVal PartID As Int32 = sysErrors.PARTADD_MFRNUMNOTUNIQUE)
            If PartID > 0 Then
                'User has asked us to look into a specific PartID -- manual refresh?

                'Sanity check
                If Not fpParts.partExistsID(PartID) Then
                    Throw New Exception("The specified PartID, " & PartID & ", DOES NOT EXIST!")
                End If

                'OK to Proceed!
                fpusPartID = PartID
            End If
        End Sub
    End Class
End Namespace