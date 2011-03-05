'System
Imports System
Imports System.Net
Imports System.IO
Imports System.Web.UI.WebControls
Imports System.Data
Imports System.Web
Imports System.Collections

'Devexpress
Imports DevExpress.Web.ASPxTreeList
Imports DevExpress.Web.ASPxGridView

'Our FriedParts API's
Imports apiOctopart
Imports fpManufacturer
Imports fpDistributor


Partial Class pParts_partAddNew
    Inherits System.Web.UI.Page

    Private OP As OctoPart
    Private OPR As OctoResults

    '''<summary>The PartTypeHorizontalAccordion Psuedo-Control</summary>
    Protected ptAccordion As System.Web.UI.FriedParts.PartTypeAccordionControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        sysUser.suLoginRequired(Me) 'Access control

        'btnTransferToOctopart.Visible = False 'XXX TEMP DEBUGGING SERVER CRASH ON GREEN BUTTON CLICK

        If (Not (IsCallback Or IsPostBack)) Then
            'Do any page load work in here!
            Page_Reset()
        Else
            'Reload WP.Part_Digikey/WP.Part_Octopart state if available
            If Not HttpContext.Current.Session("padd.OPRs") Is Nothing Then
                OPR = HttpContext.Current.Session("padd.OPRs")
                'Reload the Octopart Data View if available
                If OPR IsNot Nothing Then
                    xGridOctopartMaster.DataSource = OPR.DataSouceResults
                    xGridOctopartMaster.DataBind()
                End If
            End If
        End If

        '======================================================================
        '== PartTypeAccordion (Psuedo-Control)
        '======================================================================
        Const ptAccordianSessionName As String = "padd.PartTypeControl"
        ptAccordion = System.Web.UI.FriedParts.PartTypeAccordionControl.Reload(Me, ptAccordianSessionName)
        ptAccordion.UpdateControls(Me)
        For Each grid As DevExpress.Web.ASPxGridView.ASPxGridView In ptAccordion.GetGrids()
            AddHandler grid.CustomCallback, AddressOf ptAccordion.HandleRowChanged
        Next
        '======================================================================
    End Sub

    'Reset the page to its default condition -- for example, after we just added a part
    Protected Sub Page_Reset()
        'Session State
        HttpContext.Current.Session("pt.ArmPartType") = False
        HttpContext.Current.Session("pt.SetPartType") = Nothing
        HttpContext.Current.Session("pt.Numeric") = False
        HttpContext.Current.Session("padd.OPR") = Nothing
        OPR = Nothing
        OP = Nothing

        'Controls
        xTextBoxSearch.Text = "MAX232"
        easyButton.Visible = False
        Databind_Comboboxes()
        SelectMfr.Text = Nothing
        SelectDist.Text = ""
        SelectSchematic.Text = ""
        SelectFootprint.Text = ""
        'Datasheet defaults to manual
        Dim iDS As DataTable
        'iDS = apiWebPart.createTableDatasheetURL()
        'iDS.Rows.Add(apiWebPart.ADD_YOUR_OWN)
        DatasheetURL.DataSource = iDS
        'DatasheetURL.DataTextField = apiWebPart.DatasheetURL_FieldName
        'DatasheetURL.DataValueField = apiWebPart.DatasheetURL_FieldName
        DatasheetURL.DataBind()
        DatasheetURL_SelectedIndexChanged(Nothing, Nothing)

        'Validation
        xGridSubmit.Enabled = False
        MoreDatasheetURL.Visible = False

        'Tab Pages
        xTabPages.ActiveTabIndex = 0 'Revert tab to open on to the Search Tab even if we accidentally change it in visual studio design view


        'Development / Debug

    End Sub

    Private Sub Databind_Comboboxes()
        'Init
        Dim dt As DataTable
        Dim str As String

        'Manufacturer
        dt = New DataTable
        str = "SELECT [mfrName] FROM [view-mfr] ORDER BY [mfrName]"
        dbAcc.SelectRows(dt, str)
        dt.Rows.Add("Hi!")
        SelectMfr.DataSource = dt
        SelectMfr.DataBind()

        'Distributor
        dt = New DataTable
        str = "SELECT [distName] FROM [view-dist] ORDER BY [distName]"
        dbAcc.SelectRows(dt, str)
        dt.Rows.Add("")
        SelectDist.DataSource = dt
        SelectDist.DataBind()

        'Cadd lib
        dt = New DataTable
        str = "SELECT DISTINCT [LibName] FROM [FriedParts].[dbo].[cad-AltiumLib] WHERE [LibType]=1 ORDER BY [LibName]"
        dbAcc.SelectRows(dt, str)
        dt.Rows.Add("")
        SelectSchematic.DataSource = dt
        SelectSchematic.DataBind()

        'PCB lib
        dt = New DataTable
        str = "SELECT DISTINCT [LibName] FROM [FriedParts].[dbo].[cad-AltiumLib] WHERE [LibType]=2 ORDER BY [LibName]"
        dbAcc.SelectRows(dt, str)
        dt.Rows.Add("")
        SelectFootprint.DataSource = dt
        SelectFootprint.DataBind()
    End Sub

    '========================================
    '========================================
    '||           SUBMIT BUTTON            ||
    '========================================
    '========================================

    'THE SUBMIT BUTTON -- ADD NEW PART! YAY!
    Protected Sub EnterThePart_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles EnterThePart.Click
        'VALIDATION INITIALIZATION
        Dim pg As New fpPartValidation() 'Reset the pg object on each Submit Click
        Dim DsUrl As String 'Used to store the Datasheet URL since it may come from one of two controls

        '[STEP 1: Manufacturer]
        'Manufacturer
        Dim oMfrID As Int32 = fpMfr.Exists(Trim(Me.SelectMfr.Text))
        If (Trim(SelectMfr.Text) = "") OrElse (oMfrID < 0) Then
            Select Case oMfrID
                Case sysErrors.ERR_NOTFOUND
                    pg.AddError(1, sysErrors.PARTADD_NOMFR, "No manufacturer was specified for this part.")
                Case sysErrors.ERR_NOTUNIQUE
                    pg.AddError(1, sysErrors.PARTADD_MFRNUMNOTUNIQUE, "The specified manufacturer has multiple entries in the FP database. This represents a critical error and data table corruption. Inform management!")
                Case Else
            End Select
        End If

        'Part Number
        If (Trim(MfrPartNum.Text) = "") Then
            pg.AddError(1, -666, "You must OF COURSE enter the manufacturer's part number!")
        End If

        'IS THIS PART ALREADY IN THE DATABASE?!
        If (Not partExistsID(Me.SelectMfr.Text, Trim(MfrPartNum.Text), True) = sysErrors.ERR_NOTFOUND) Then
            'Uh Oh! We found this part in the DB already!
            pg.AddError(1, -692, "THIS PART ALREADY EXISTS IN THE DATABASE. A specific combination of manufacturer and part number may only be entered once. Edit the existing part or delete and re-create it here.")
        End If

        '[STEP 2: Part Type]
        'PartType
        Dim oPartTypeID As Int32
        If Not SelectedPartTypeID.Value Is Nothing AndAlso SelectedPartTypeID.Value.Length > 0 Then

            'xxx FINISH!!!
            'Check if the specified PartTypeID is valid...

            'If pt Then
            '    pg.AddError(2, -666, "A part's type must be specified. UNKNOWN is not a valid entry. Part typing is the basis for a lot of secondary automation. Trust me. It is in your best interest to get this right!")
            'Else
            '    PartType is defined!
            '    oPartTypeID = fpPartTypes.ptGetTypeIDFromPath(hPartTypePath.Value)
            'End If
        Else
            'oPartTypeID = sysEnv.sysPARTTYPEUNKNOWN
            pg.AddError(2, -666, "You forgot to categorize this part. Assign a Part Type by exploring the Part Type tree.")
        End If

        'Value
        'We do not enforce Tolerance at this point...
        If txtValue.Text = "" Then
            pg.AddError(2, -666, "You must enter a value for this part.")
        End If
        If HttpContext.Current.Session("pt.Numeric") = True Then
            'We have a numeric value -- enforce!
            If Not fpPartValues.pvValidNumeric(txtValue.Text) Then
                pg.AddError(2, -666, "The value you entered for this part must be number or evaluate to a number. You may use engineering and pSpice notation. For example 34p, 3m, 2e3. Use Meg for mega. M = m = milli!")
            End If
        End If
        If Not xCheckNoValue.Checked Then
            'Only enforce units if value is not the part number.
            If txtValueUnits.Text = "" Then
                pg.AddError(2, -666, "You must enter the units for this part. Enter the unitary unit. Ex. Farads, not picoFarads. Include the magnitude with the value itself -- as in 34p.")
            End If
        End If

        '[STEP 3: Parameters]
        'Datasheet URL
        If DatasheetURL.Text.StartsWith("(") Then
            DsUrl = txtDatasheetURL.Text
        Else
            DsUrl = DatasheetURL.Text
        End If
        If DsUrl = "" Then
            pg.AddError(3, sysErrors.PARTADD_NODATASHEET, "Please provide a URL for the datasheet. If there is no datasheet please provide your facebook URL.")
        Else
            If Not txtValidURL(DsUrl) Then
                pg.AddError(3, -666, "The URL you entered for the datasheet is not valid. Please enter a valid URL such as http://blah.com/datasheet.pdf")
            End If
        End If

        'Image URL
        If ImageURL.Text = "" Then
            pg.AddError(3, sysErrors.PARTADD_NOIMAGE, "Please provide an Image URL for this part. If you do not want to provide an image you can go jump off the E4-E5 bridge.")
        Else
            If Not txtValidURL(ImageURL.Text) Then
                pg.AddError(3, -666, "The URL you entered for the image is not valid. Please enter a valid URL such as http://blah.com/image.jpg")
            End If
        End If

        'Short Description
        If SrtDesBox.Text = "" Then
            pg.AddError(3, sysErrors.PARTADD_NOSHORTDESC, "You must provide a short description for this part.")
        End If

        'Long Description
        If LongDesBox.Text = "" Then
            pg.AddError(3, sysErrors.PARTADD_NOLONGDESC, "You have not provided any extended description.")
        End If

        '[STEP 5: Sourcing]
        'Distributor
        Dim oDistID As Int32 = fpDist.Exists(Trim(Me.SelectDist.Text))
        If (Trim(SelectDist.Text) = "") OrElse (oDistID < 0) Then
            Select Case oDistID
                Case sysErrors.ERR_NOTFOUND
                    pg.AddError(5, sysErrors.PARTADD_NODIST, "No distributor was specified for this part.")
                Case sysErrors.ERR_NOTUNIQUE
                    pg.AddError(5, -666, "The specified distributor has multiple entries in the FP database. This represents a critical error and data table corruption. Inform management!")
                Case Else
            End Select
        End If

        'CHECK IF WE PASSED VALIDATION
        If pg.PartValid Then
            xGridSubmit.Enabled = False
            xGridSubmit.Visible = False
        Else
            'IF NOT DISPLAY VALIDATION ERRORS
            xGridSubmit.DataSource = pg.GetDataSource
            xGridSubmit.DataBind()
            xGridSubmit.KeyFieldName = "UID"
            xGridSubmit.Enabled = True
            xGridSubmit.Visible = True
            Exit Sub
        End If
        'Falls through to here if Part Valid -- Now lets commit it!

        'WRITE TO THE DATABASE!
        Dim newPart As Int32
        newPart = _
        fpParts.partAdd(oMfrID, Me.MfrPartNum.Text, oPartTypeID, 0, Me.SrtDesBox.Text, Me.LongDesBox.Text, _
                        Me.txtValue.Text, Me.txtValueUnits.Text, Me.txtValueTol.Text, _
                        Me.ImageURL.Text, DsUrl, _
                        oDistID, Me.DistPartNum.Text, _
                        Me.LibraryRef.Text, Me.SelectSchematic.Text, _
                        Me.FootprintRef.Text, Me.SelectFootprint.Text)

        'CHECK AND REPORT
        If newPart < 0 Then
            'ERRORS OCCURED IN fpParts.partAdd()
            MsgBox(Me, newPart)
        Else
            'SUCCESS!
            dbLog.logUserActivity(Me, _
                                suGetUserFirstName() & " added part #" & newPart & ", " & fpMfr.GetName(oMfrID) & " " & Me.MfrPartNum.Text, _
                                fpParts.partAddSql(oMfrID, Me.MfrPartNum.Text, oPartTypeID, 0, Me.SrtDesBox.Text, Me.LongDesBox.Text, _
                                    Me.txtValue.Text, Me.txtValueUnits.Text, Me.txtValueTol.Text, _
                                    Me.ImageURL.Text, DsUrl, _
                                    oDistID, Me.DistPartNum.Text, _
                                    Me.LibraryRef.Text, Me.SelectSchematic.Text, _
                                    Me.FootprintRef.Text, Me.SelectFootprint.Text), , "PartID", , newPart)
            MsgBox(Me, sysErrors.PARTADD_SUCCESS, newPart, fpMfr.GetName(oMfrID) & " " & Me.MfrPartNum.Text)
        End If
    End Sub



    '========================================
    '========================================
    '||           SEARCH BUTTON            ||
    '========================================
    '========================================

    'Octopart/Digikey search button
    Protected Sub xbtnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles xbtnSearch.Click
        'Abort if box is empty
        If Trim(xTextBoxSearch.Text).Length = 0 Then Exit Sub

        'Reset Page
        Dim TheSearchText As String = xTextBoxSearch.Text
        Page_Reset()
        xTextBoxSearch.Text = TheSearchText

        'Octopart Search
        OPR = New OctoResults(TheSearchText)
        xGridOctopartMaster.DataSource = OPR.DataSouceResults
        xGridOctopartMaster.DataBind()

        'Save State
        HttpContext.Current.Session("padd.OPRs") = OPR

        'Check if too many results were returned...
        divSearchStats.InnerText = "Returned " & OPR.NumResultsTotal & " results in " & FormatNumber(OPR.RunTime * 1000, 1) & " milliseconds..."
        divSearchStats.Visible = True
        If OPR.NumResultsTotal > 10 Then
            divTooManyResults.InnerText = "Your search has returned too many results. We're showing you the first 10. Please narrow your search."
            divTooManyResults.Visible = True
        End If
        'Turn on the easy button after a search is performed
        easyButton.Visible = True

        'Restart Octopart Selection Control (Datasheet, Image, ...)
        'DatasheetURL Control
        xDatasheetPreviewLink.ClientVisible = "False"
        xDatasheetPreviewLink.NavigateUrl = ""

        'Image Select Control
        ASPxReselectImage.ClientVisible = "False"
        ASPxSelectedImage.ClientVisible = "False"
        ASPxSelectedImage.ImageUrl = ""
        ImageDataView.ClientVisible = "False"

    End Sub

    'This function implements all the necessary changes when a new PartType is selected
    Private Sub partTypeChanged(ByVal newPartTypeID As Int32)
        'PART TYPES TREE LIST
        Dim newPath As String = ptGetPath(newPartTypeID)
        HttpContext.Current.Session("pt.ArmPartType") = True
        HttpContext.Current.Session("pt.SetPartType") = newPath

        'VALUE
        Dim tval As String
        Dim tvaldesc As String
        Dim tvalnumeric As Boolean
        Dim tunits As String
        ptGetTypeValue(newPartTypeID, tval, tvaldesc, tvalnumeric, tunits)
        'Numeric units for this part category?
        HttpContext.Current.Session("pt.Numeric") = tvalnumeric
        '"Value" label -- replace with specific term (ex. "Capacitance:")
        If Not tval Is Nothing AndAlso tval.Length > 0 Then
            lblValue.Text = tval & ":"
        End If
        'Show the "Value" description if available
        If Not tvaldesc Is Nothing AndAlso tvaldesc.Length > 0 Then
            lblValueNotes.Text = tvaldesc
            lblValueNotes.Visible = True
        Else
            lblValueNotes.Visible = False
        End If
        'If numeric, show the numeric hint box (provides feedback so the user knows what value FP thinks it is specifying)
        lblComputedValue.Visible = tvalnumeric
        'Deal with Units -- if a specific unit is specified then prevent user changes!
        If Not tunits = "" Then
            'Lock units
            txtValueUnits.Text = tunits
            txtValueUnits.ReadOnly = True
            txtValueUnits.BorderStyle = BorderStyle.None
            txtValueUnits.BackColor = Drawing.Color.Transparent
        Else
            'Unlock units
            txtValueUnits.ReadOnly = False
            txtValueUnits.BorderStyle = BorderStyle.Inset
            txtValueUnits.BackColor = Drawing.Color.White
        End If

    End Sub

    '========================================
    '========================================
    '||             EASY BUTTON            ||
    '========================================
    '========================================

    Protected Sub easyButton_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles easyButton.Click
        'Select the Specific Octopart
        If OPR IsNot Nothing AndAlso OPR.DataSouceResults.Rows.Count > 0 Then
            'Get specific Octopart data of the first part on the MPN List
            Dim chosenIndex As Integer = xGridOctopartMaster.FocusedRowIndex()
            If chosenIndex <> -1 Then
                divEasyError.Visible = False
                OP = OPR.Parts(chosenIndex + 1) 'Align indexes (Collections are 1-based; DevExpress.GridView is 0-based)
            Else
                divEasyError.InnerText = "You must select a part from the list of search results first!"
                divEasyError.Visible = True
                Exit Sub
            End If
        End If

        'Confirm that we have a set of Inet search selections to work with

        'Step 1. Mfr
        Databind_Comboboxes() 'Remap Comboboxes in case we added a new manufacturer
        SelectMfr.Text = OP.Manufacturer.DefaultName
        MfrPartNum.Text = OP.MfrPartNum
        MfrPartUnique() 'Inform user if this part already exists in the database

        'Step 2. Parameters
        '[[DATASHEETS]]
        'Assign the datasource
        DatasheetURL.DataSource = OP.UrlDatasheetsDatasource
        DatasheetURL.DataTextField = OP.UrlDatasheetsDatasource.Columns(1).ColumnName
        DatasheetURL.DataValueField = OP.UrlDatasheetsDatasource.Columns(1).ColumnName
        DatasheetURL.DataBind()
        DatasheetURL_SelectedIndexChanged(Nothing, Nothing) 'Fire to update display of manual box
        'Prompt user that more URLs available in the drop down
        If OP.UrlDatasheetsDatasource.Rows.Count > 2 Then
            MoreDatasheetURL.Visible = True
        Else
            MoreDatasheetURL.Visible = False
        End If

        '[[IMAGES]]
        'Assign the datasource
        ImageDataView.DataSource = OP.UrlImagesDatasource
        ImageDataView.DataBind()
        ImageDataView.ClientVisible = "True"
        'Default value
        ImageURL.Text = OP.UrlImagesDatasource.Rows(0).Field(Of String)("URL")

        'Descriptions
        SrtDesBox.Text = OP.DescriptionShort
        LongDesBox.Text = OP.DescriptionLong

        '[Part Type] -- xxx --- ULTIMATELY THIS SHOULD BE ABSORPED INTO apiWEBPART CLASS!!!
        Dim parentTypeID As Int32
        Dim partTypeID As Int32
        'If WP.Part_Digikey.getTypeCategory Is Nothing OrElse WP.Part_Digikey.getTypeCategory.Length = 0 Then
        '    'ERROR: Category is unknown -- digikey provided no data or improperly formated data
        '    partTypeID = sysEnv.sysPARTTYPEUNKNOWN
        'Else
        '    'Category defined by Digikey -- use it!
        '    If Not ptExistsPartType(WP.Part_Digikey.getTypeCategory, 0) Then
        '        'Found a new Digikey Category. Let's add this Category!
        '        ptAddNewPartType(WP.Part_Digikey.getTypeCategory, 0, "Added " & Now, Me)
        '    End If
        '    'Parent TypeID (Category)
        '    parentTypeID = ptFindPartType(WP.Part_Digikey.getTypeCategory, 0)
        '    '--------------------------
        '    'Child TypeID (Family)
        '    '--------------------------
        '    If WP.Part_Digikey.getTypeFamily Is Nothing OrElse WP.Part_Digikey.getTypeFamily.Length = 0 Then
        '        'ERROR: Family is unknown -- digikey provided no data or improperly formated data
        '        '       So assign this part to the parent class (generic)
        '        partTypeID = parentTypeID
        '    Else
        '        'Digikey provides Family info -- use it!
        '        If Not ptExistsPartType(WP.Part_Digikey.getTypeFamily, parentTypeID) Then
        '            'This Family Type doesn't exist yet. Let's add it!
        '            ptAddNewPartType(WP.Part_Digikey.getTypeFamily, parentTypeID, "Added " & Now, Me)
        '        End If
        '        'Setup the Part-Type box
        '        partTypeID = ptFindPartType(WP.Part_Digikey.getTypeFamily, parentTypeID)
        '    End If
        'End If

        'Set and focus the new part type
        partTypeChanged(partTypeID)


        'Step 3. CADD

        'Step 4. Sourcing
        

        'Switch to next tab page
        xTabPages.ActiveTabIndex = 1
    End Sub

    Protected Sub ImageBtn_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        ImageURL.Text = e.CommandName
        ASPxSelectedImage.ClientVisible = "True"
        ASPxSelectedImage.ImageUrl = e.CommandName
        ImageDataView.ClientVisible = "False"
        ASPxReselectImage.ClientVisible = "True"
    End Sub

    Protected Sub ReselectBtn_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        ImageURL.Text = "No Image Selected"
        ASPxSelectedImage.ClientVisible = "False"
        ASPxSelectedImage.ImageUrl = "null"
        ImageDataView.ClientVisible = "True"
        ASPxReselectImage.ClientVisible = "False"
    End Sub

    Protected Sub DatasheetURL_Select()
        If DatasheetURL.Text <> "" Then
            xDatasheetPreviewLink.ClientVisible = "True"
            xDatasheetPreviewLink.NavigateUrl = DatasheetURL.Text
        Else
            xDatasheetPreviewLink.ClientVisible = "False"
            xDatasheetPreviewLink.NavigateUrl = ""
        End If
    End Sub

    Protected Sub SelectSchematic_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SelectSchematic.SelectedIndexChanged
        Dim filename As String = fplibLibPath() & SelectSchematic.Text
        LibraryRef.DataSource = fpLibReadLib(fplibTypes.SchLib, filename)
        LibraryRef.DataBind()
    End Sub

    Protected Sub SelectFootprint_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SelectFootprint.SelectedIndexChanged
        Dim filename As String = fplibLibPath() & SelectFootprint.Text
        FootprintRef.DataSource = fpLibReadLib(fplibTypes.PcbLib, filename)
        FootprintRef.DataBind()
    End Sub

    Protected Sub DatasheetURL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DatasheetURL.SelectedIndexChanged
        If DatasheetURL.Text <> "" Then
            If DatasheetURL.Text.StartsWith("(") Then
                'Manual URL requested
                txtDatasheetURL.Visible = True
                txtDatasheetURL.Enabled = True
                xDatasheetPreviewLink.ClientVisible = "False"
                xDatasheetPreviewLink.NavigateUrl = ""
            Else
                'Disable Manual URL
                txtDatasheetURL.Visible = False
                txtDatasheetURL.Enabled = False
                xDatasheetPreviewLink.ClientVisible = "True"
                xDatasheetPreviewLink.NavigateUrl = DatasheetURL.Text
            End If
        Else
            xDatasheetPreviewLink.ClientVisible = "False"
            xDatasheetPreviewLink.NavigateUrl = ""
        End If
    End Sub

    Protected Sub xCheckNoValue_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles xCheckNoValue.CheckedChanged
        If xCheckNoValue.Checked Then
            txtValue.Text = hiddenMfrPartNumber.Value
            txtValueTol.Text = ""
            txtValueUnits.Text = ""
            txtValue.Enabled = False
            txtValueTol.Enabled = False
            txtValueUnits.Enabled = False
        Else
            txtValue.Enabled = True
            txtValueTol.Enabled = True
            txtValueUnits.Enabled = True
        End If
    End Sub

    Protected Sub SelectMfr_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SelectMfr.SelectedIndexChanged
        'Manufacturer Name has been Changed/Updated
        MfrPartUnique()
    End Sub

    Protected Sub MfrPartNum_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MfrPartNum.TextChanged
        'Manufacturer Part number has been changed/updated
        MfrPartUnique()
    End Sub

    'Tests if this part is unique -- used for early detection of redundant parts...
    'Returns True (meaning ok to continue) if one of the two values is missing
    Private Function MfrPartUnique() As Boolean
        If (SelectMfr.Text.Length > 0 And Trim(MfrPartNum.Text).Length > 0) Then
            If (Not partExistsID(Me.SelectMfr.Text, Trim(MfrPartNum.Text), True) = sysErrors.ERR_NOTFOUND) Then
                'Uh Oh! We found this part in the DB already!
                MsgBox(Me, sysErrors.PARTADD_ALREADYEXISTS, SelectMfr.Text & " " & Trim(MfrPartNum.Text))
                Return False
            Else
                Return True
            End If
        Else
            'Not enough info to test for redundancy
            Return True
        End If
    End Function
End Class

