Imports Microsoft.VisualBasic
Imports System.Data

Namespace UpdateService
    Public Class upWorkerPart

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
        ''' Worker thread for the updating of parts. This 
        ''' dispatcher is separate from other sync/update processes that happen in FriedParts so that
        ''' updates can happen in parallel when sourced from different data providers. For example,
        ''' Dropbox updates and Part updates happen in parallel, but each one is throttled to a certain
        ''' rate to prevent abusing our data provider's servers.
        ''' </summary>
        ''' <returns>A human-readable message explaining what happened -- for log/display as needed (safe to ignore)</returns>
        ''' <remarks>Is called by fpusDispatch() and never directly</remarks>
        Private Function fpusWorkerPart() As String
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
    End Class
End Namespace