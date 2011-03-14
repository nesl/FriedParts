Imports System.IO
Imports System.Data
Imports System
Imports Microsoft.VisualBasic
Imports Ionic.Zip   'DOTNETZIP v1.9 - open source zip dll

Partial Class pBOM_bomAddNew
    Inherits System.Web.UI.Page
    Protected Validator As fpPartValidation
    Protected Project As fpProject

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        suLoginRequired(Me) 'Access Control
        If Not (IsCallback Or IsPostBack) Then
            'Initial Page Load
            xTabPageControl.ActiveTabIndex = 0 'Set the starting tab page (in case development accidentally changes it in the Visual Studio gui
            xtabProjectType.ActiveTabIndex = 0
            'Clear prior temporary project data
            Project = New fpProject
            saveState()
        Else
            'Callback/Postback
            Project = HttpContext.Current.Session("bom.Project") 'Restore!
            If Project Is Nothing Then
                'ERROR! But usually means we were debugging and forced the server to recompile (which drops all current sessions)
                '...so we just correct it and move on
                Project = New fpProject
                saveState()
            End If
            restoreState()
        End If
    End Sub

    'Saves the project's data state between callbacks
    Protected Sub saveState()
        HttpContext.Current.Session("bom.Project") = Project
    End Sub

    'Rebinds all the datatables and datasources
    Protected Sub restoreState()
        If Project.BomAnalysis IsNot Nothing Then
            'Bom Import
            xGridReview.DataSource = Project.GroupedBomDataSource
            xGridReview.DataBind()
            'Bom Exclusions
            xGridExclusions.DataSource = Project.BomDataSource
            xGridExclusions.DataBind()
            'Bom Exclusions Summary
            Dim DNPs As DoNotPopulateOverview = Project.DoNotPopulateSummary
            lblDnpCount.Text = "The following " & DNPs.Count & " Reference Designators (RefDes) have been marked DO NOT POPULATE (DNP):"
            lblDnpDescription.Text = DNPs.ToString
        End If
        'Bom Additional
        xGridBomAdditional.DataSource = Project.AdditionalBomDataSource
        xGridBomAdditional.DataBind()
    End Sub

    'Displays the validation error grid
    Protected Sub displayErrors()
        If Not Validator.PartValid() Then
            xGridSubmit.DataSource = Validator.GetDataSource()
            xGridSubmit.DataBind()
            xGridSubmit.Visible = True
            Exit Sub
        End If
    End Sub

    '=====================
    '== SUBMIT PROJECT ===
    '=====================
    Protected Sub btnSubmitProject_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmitProject.Click
        'Initialize Validation UI
        Validator = New fpPartValidation
        xGridSubmit.Visible = False
        Dim ProjectValid As Boolean 'Holds state if the current project configuration is valid -- so we know at the end we can save successfully

        'Determine if New Project, New Revision, or just Updating Project
        Select Case xtabProjectType.ActiveTabIndex
            Case 0
                'Create new project!

                '[PHASE I] -- Macro stuff (Project & XML Import)
                ProjectValid = NewProject_Valid() 'Check if required data is present and ok (writes to Validator)
                'Verify CADD Import
                If Not Project.HasImportedBOM Then
                    Validator.AddError(1, -7374, "You MUST import the BOM from Altium using a valid XML file.")
                End If
                'Exit if Phase I errors exist
                If Not Validator.PartValid Then
                    displayErrors()
                    Exit Sub
                End If

                '[PHASE II] -- Detail Stuff
                'Files and filenames
                If ProjectHasFiles() Then
                    If Project.ZipFilename Is Nothing Then
                        Validator.AddError(5, -757, "It appears that you are trying to include files with your project, but you did not upload them. Please click the 'Upload!' button! Duh!")
                    Else
                        Dim files As String()
                        files = Directory.GetFiles(sysEnv.uploadBOMIMPORT(), Project.ZipFilename)
                        For Each File As String In files
                            Response.Write(File & "<BR>")
                        Next
                    End If
                End If

                'Exit if Phase II errors exist
                If Not Validator.PartValid Then
                    displayErrors()
                    Exit Sub
                End If

                '[FINALIZE] -- Write to DB
                Project.MyProjectID = NewProject_Save() 'Create the new project
                Project.MyProjectTitle = txtTitle.Text
                Project.MyProjectRevision = txtRevision.Text
                Project.Save() 'Write the BOM Parts to DB

                'Deal with File Uploads

                '[DONE] -- Report Success
                MsgBox(Me, sysErrors.PROJECTADD_SUCCESS, Project.MyProjectID, Project.MyProjectTitle & " " & Project.MyProjectRevision)

            Case 1
                'Create new revision
                Validator.AddError(0, -9143, "Creating a new project REVISION is not supported yet. Coming soon. <-- 'soon', LOL, yeah right.")
                If Not Validator.PartValid Then
                    displayErrors()
                    Exit Sub
                End If
            Case 2
                'Update an Existing Project
                Validator.AddError(0, -9153, "Updating an EXISTING project is not supported yet. Coming soon. <-- 'soon', LOL, yeah right.")
                If Not Validator.PartValid Then
                    displayErrors()
                    Exit Sub
                End If
            Case Else
                'ERROR! Should not be possible
                Err.Raise(-1234324, , "Hell no!")
        End Select
    End Sub



    '=========================
    '== CREATE NEW PROJECT ===
    '=========================
    'Returns FALSE on error, TRUE on success
    Protected Function NewProject_Valid() As Boolean
        'Defang Text Fields
        txtTitle.Text = txtDefangSQL(txtTitle.Text)
        txtDesc.Text = txtDefangSQL(txtDesc.Text)
        txtRevision.Text = txtDefangSQL(txtRevision.Text)

        'Check for empty fields
        If txtTitle.Text = "" Or txtRevision.Text = "" Then
            'Title
            If txtTitle.Text = "" Then
                Validator.AddError(0, -21911, "You must enter the project's title. This is the project's name.")
            End If
            'Revision
            If txtRevision.Text = "" Then
                Validator.AddError(0, -21913, "You must enter the project's revision. Revisions are date codes in four digit format: MMYY.")
            End If
        Else
            'Validate the Revision entered is in a valid format
            If Not fpProj.projValidRevision(txtRevision.Text) Then
                Validator.AddError(0, -21913, "The project's revision is not valid. Revisions are date codes in four digit format: MMYY.")
            Else
                'Check for name uniqueness
                If fpProj.projExists(txtTitle.Text, txtRevision.Text) <> sysErrors.ERR_NOTFOUND Then
                    Validator.AddError(0, -21914, "This project already exists. Projects must have a unique combination of name (title) and revision.")
                End If
            End If
        End If

        'Description
        If txtDesc.Text = "" Then
            Validator.AddError(0, -21912, "You must enter a description. Please make it a good one!")
        End If

        'Validate!
        If Not Validator.PartValid Then
            Return False
        Else
            Return True
        End If
    End Function

    'Write the new project to disk and return its newly created ProjectID
    Protected Function NewProject_Save() As Integer
        'Add to Database!
        Dim sqlTxt As String = _
            "INSERT INTO [FriedParts].[dbo].[proj-Common]" & _
            "           ([Title]" & _
            "           ,[Description]" & _
            "           ,[Revision]" & _
            "           ,[OwnerID]" & _
            "           ,[DateCreated])" & _
            "     VALUES (" & _
            "           '" & txtTitle.Text & "'," & _
            "           '" & txtDesc.Text & "'," & _
            "           '" & txtRevision.Text & "'," & _
            "           " & HttpContext.Current.Session("user.UserID") & "," & _
            "           " & sysText.txtSqlDate(Now) & _
            "           )"
        SQLexe(sqlTxt)
        sqlTxt = _
            "SELECT [ProjectID] " & _
            "FROM [FriedParts].[dbo].[proj-Common] " & _
            "WHERE [Title]='" & txtTitle.Text & "' AND [Revision]='" & txtRevision.Text & "';"
        Dim dt As New DataTable
        dt = SelectRows(dt, sqlTxt)
        If dt.Rows.Count = 1 Then
            'Found it!
            Return dt.Rows(0).Field(Of Integer)("ProjectID")
        Else
            Err.Raise(-342, , "New project created, but something weird happened with the ProjectID! rows.count = " & dt.Rows.Count)
        End If
    End Function





    '=========================
    '== IMPORT BOM BUTTON  ===
    '=========================
    'Import this bom XML Text button
    Protected Sub btnImport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnImport.Click
        Dim filename_server As String
        Dim filename_original As String
        Dim importResult As baBomAnalysis

        If xBomUpload.HasFile Then
            If xBomUpload.UploadedFiles.Length > 1 Then
                'Error multiple files selected!
                reportError("Multiple files selected! You may only import one BOM at a time. BOM files must correspond to projects in a 1 to 1 fashion.")
            Else
                filename_server = Now.Ticks & HttpContext.Current.Session("user.UserID")
                filename_original = xBomUpload.UploadedFiles(0).FileName
                xBomUpload.UploadedFiles(0).SaveAs(uploadBOMIMPORT() & filename_server)
                importResult = baImportBOM(filename_server)
                If importResult.isError Then
                    reportError(importResult.ErrorMessage)
                Else
                    'It worked!
                    If HttpContext.Current.Session("bom.Project") Is Nothing Then
                        'No project data saved yet
                        Project = New fpProject
                    End If
                    Project.BomAnalysis = importResult 'Save the import without disturbing any manually added parts
                    saveState()
                    restoreState()
                    reportSuccess(filename_original)
                End If
            End If
        Else
            'Error: No file selected for upload
            reportError("No file selected for upload. Please select a valid Altium XML BOM file for upload.")
        End If
    End Sub

    Protected Sub reportError(ByRef errMsg As String)
        importOK.Visible = False
        importErrors.Visible = True
        lblErrors.Text = errMsg
    End Sub
    Protected Sub reportSuccess(ByRef Local_Filename As String)
        importOK.Visible = True
        importErrors.Visible = False
        lblOK.Text = Local_Filename & " was imported and analyzed successfully!"
    End Sub

    Protected Function checkExt(ByVal fileString As String, ByVal ext As String) As Boolean

        Dim extExtract As String = fileString.Substring(fileString.Length - ext.Length - 1)
        If extExtract.ToLower() = ("." & ext.ToLower()) Then
            Return True
        Else
            Return False
        End If

        Return False

    End Function


    '===================
    '== FILE UPLOAD  ===
    '===================
    Protected Sub btnUploadProject_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUploadProject.Click

        Dim dirName_server As String
        Dim toZip As Boolean = False


        'create temp directory to save a copy of all project files to it
        dirName_server = Now.Ticks & HttpContext.Current.Session("user.UserID")
        Directory.CreateDirectory(uploadBOMIMPORT() & dirName_server)

        'Check if file path available and check extensions (not case sensitive)
        If PrjPcbUpload.HasFile Then
            If checkExt(PrjPcbUpload.FileName, "PrjPcb") Then
                PrjPcbUpload.SaveAs(uploadBOMIMPORT() & dirName_server & "/" & PrjPcbUpload.FileName())
            Else
                'Validator.AddError(5, -12233, "PrjPcb file type not accepted")
            End If
        Else
            'Validator.AddError(5, -12234, "No PrjPcb file uploaded")
        End If

        If Schdocupload.HasFile Then
            If checkExt(Schdocupload.FileName, "SchDoc") Then
                Schdocupload.SaveAs(uploadBOMIMPORT() & dirName_server & "/" & Schdocupload.FileName())
            Else
                'Validator.AddError(5, -12234, "SchDoc file type not accepted")
            End If
        Else
            'Validator.AddError(5, -12235, "No SchDoc file uploaded")
        End If

        If PcbDocUpload.HasFile Then
            If checkExt(PcbDocUpload.FileName, "PcbDoc") Then
                PcbDocUpload.SaveAs(uploadBOMIMPORT() & dirName_server & "/" & PcbDocUpload.FileName())
            Else
                'Validator.AddError(5, -12236, "PcbDoc file type not accepted")
            End If
        Else
            'Validator.AddError(5,-12237, "No PcbDoc file uploaded")
        End If

        If OutJobUpload.HasFile Then
            If checkExt(OutJobUpload.FileName, "OutJob") Then
                OutJobUpload.SaveAs(uploadBOMIMPORT() & dirName_server & "/" & OutJobUpload.FileName())
            Else
                'Validator.AddError(5, -12238, "OutJob file type not accepted")
            End If
        Else
            'Validator.AddError(5, -12239, "No OutJob file uploaded")
        End If

        If TOFABGerbersUpload.HasFile Then
            If checkExt(TOFABGerbersUpload.FileName, "zip") Then
                TOFABGerbersUpload.SaveAs(uploadBOMIMPORT() & dirName_server & "/" & TOFABGerbersUpload.FileName())
            Else
                'Validator.AddError(5, -12240, "TOFABGerbers file type not accepted")
            End If
        Else
            'Validator.AddError(5, -12241, "No ToFABGerbers file uploaded")
        End If

        If TOASMGerbersUpload.HasFile Then
            If checkExt(TOASMGerbersUpload.FileName, "zip") Then
                TOASMGerbersUpload.SaveAs(uploadBOMIMPORT() & dirName_server & "/" & TOASMGerbersUpload.FileName())
            Else
                'Validator.AddError(5, -12242, "TOASMGerbers file type not accepted")
            End If
        Else
            'Validator.AddError(5, -12243, "No TOASMGerbers file uploaded")
        End If

        If SchPDFUpload.HasFile Then
            If checkExt(SchPDFUpload.FileName, "pdf") Then
                SchPDFUpload.SaveAs(uploadBOMIMPORT() & dirName_server & "/" & SchPDFUpload.FileName())
            Else
                'Validator.AddError(5, -12244, "SchPDFUpload file type not accepted")
            End If
        Else
            'Validator.AddError(5, -12245, "No SchPDFUpload file uploaded")
        End If

        'Package all files uploaded into a zip file and place in BOMUpload folder
        Dim zip As ZipFile = New ZipFile()
        zip.AddDirectory(uploadBOMIMPORT() & dirName_server, "")
        Dim ZipFileName As String = txtTitle.Text.Replace(" ", "") & "-" & txtRevision.Text.Replace(" ", "") & ".zip"
        zip.Save(uploadBOMPROJECTS() & ZipFileName)
        Project.ZipFilename = ZipFileName
    End Sub

    'Returns true if user is trying to upload at least one file.
    Private Function ProjectHasFiles() As Boolean
        Dim blah As Boolean = False
        blah = blah And PrjImage.HasFile
        blah = blah And PrjPcbUpload.HasFile
        blah = blah And Schdocupload.HasFile
        blah = blah And PcbDocUpload.HasFile
        blah = blah And OutJobUpload.HasFile
        blah = blah And TOFABGerbersUpload.HasFile
        blah = blah And TOASMGerbersUpload.HasFile
        blah = blah And SchPDFUpload.HasFile
        Return blah
    End Function

    '==============================
    '== ADD A PART TO BOM BUTTON ==
    '==============================
    Protected Sub btnBomAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBomAdd.Click
        'Confirm Data OK
        If validBomAddID.InnerText.StartsWith("NOT") Then Exit Sub 'PartID not valid
        If validBomAddQty.InnerText.StartsWith("NOT") Then Exit Sub 'Quantity not valid

        'Data is valid add it to table!
        Project.AddAdditionalPart(CInt(txtBomAdd.Text), CInt(txtBomAddQty.Text))
        saveState()

        'Update!
        xGridBomAdditional.DataSource = Project.AdditionalBomDataSource
        xGridBomAdditional.DataBind()
    End Sub

    'Add additional BOM items -- PartID and Qty sanity checks
    Protected Sub txtBomAdd_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBomAdd.TextChanged
        Try
            Dim pid As Integer = CInt(txtBomAdd.Text)
            If partExistsID(pid) Then
                If Project.AdditionalBomDataSource.Rows.Find(pid) IsNot Nothing Then
                    'Invalid ID -- part already in table
                    txtBomAdd_Invalid("It's already there!")
                Else
                    If Project.HasImportedBOM Then
                        If Project.BomAnalysis.Contains(pid) Then
                            'Invalid ID -- part already in Altium BOM
                            txtBomAdd_Invalid("Already in the Imported BOM!")
                        Else
                            'Valid ID
                            txtBomAdd_Valid()
                        End If
                    Else
                        txtBomAdd_Invalid("You MUST import an XML BOM file first!")
                    End If
                End If
            Else
                'Invalid ID
                txtBomAdd_Invalid()
            End If
        Catch ex As Exception
            'Invalid ID
            txtBomAdd_Invalid(ex.Message)
        End Try
    End Sub
    Public Sub txtBomAdd_Valid()
        validBomAddID.Visible = True
        validBomAddID.InnerText = fpParts.partGetShortName(txtBomAdd.Text)
        validBomAddID.Attributes("class") = "fpHighlightBox"
    End Sub
    Public Sub txtBomAdd_Invalid(Optional ByRef Message As String = "")
        validBomAddID.Visible = True
        validBomAddID.InnerText = "NOT Valid! " & Message
        validBomAddID.Attributes("class") = "fpErrorBox"
    End Sub
    Protected Sub txtBomAddQty_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBomAddQty.TextChanged
        Try
            Dim blah As Integer = CInt(txtBomAddQty.Text)
            If blah > 0 Then
                txtBomAddQty_Valid()
            Else
                txtBomAddQty_Invalid()
            End If
        Catch
            'Invalid ID
            txtBomAddQty_Invalid()
        End Try
    End Sub
    Public Sub txtBomAddQty_Valid()
        validBomAddQty.Visible = True
        validBomAddQty.InnerText = "Valid!"
        validBomAddQty.Attributes("class") = "fpHighlightBox"
    End Sub
    Public Sub txtBomAddQty_Invalid()
        validBomAddQty.Visible = True
        validBomAddQty.InnerText = "NOT Valid!"
        validBomAddQty.Attributes("class") = "fpErrorBox"
    End Sub

    '"Delete" button event handler
    'DELETE MANUAL BOM ITEM
    Protected Sub xGridBomAdditional_CustomButtonCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridView.ASPxGridViewCustomButtonCallbackEventArgs) Handles xGridBomAdditional.CustomButtonCallback
        'Retrieve the FPID of the part the user clicked the "Delete" button for
        Dim fpid As Integer
        fpid = CInt(xGridBomAdditional.GetRowValues(e.VisibleIndex, "PartID"))
        'Actually delete from the table
        Project.DeleteAdditionalPart(fpid)
        saveState()
        'Update the display
        restoreState()
    End Sub

    'Used in the Import Review Grid to mark a part as a non-friedpart
    'FRIED/BAKED BOM PART TOGGLE
    Protected Sub xGridReview_CustomButtonCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridView.ASPxGridViewCustomButtonCallbackEventArgs) Handles xGridReview.CustomButtonCallback
        'Retrieve the fpID of the part the user clicked the "Toggle" button for
        Dim fpID As Integer
        Try
            fpID = CInt(xGridReview.GetRowValues(e.VisibleIndex, "PartID"))
        Catch ex As InvalidCastException
            'Should not happen because you should not be able to click on the button (it should be hidden at this point)
            Exit Sub
        End Try
        'Get current value
        Dim isFP As Boolean = CBool(xGridReview.GetRowValues(e.VisibleIndex, "isFP"))
        Project.BomAnalysis.SetIsFriedPart(fpID, Not isFP) 'implement toggle
        saveState()
        'Update the display
        restoreState()
    End Sub

    'Used in the exclusions tab to mark individual RefDes for exclusion
    'REFDES DNP EXCLUSION TOGGLE BUTTON
    Protected Sub xGridExclusions_CustomButtonCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridView.ASPxGridViewCustomButtonCallbackEventArgs) Handles xGridExclusions.CustomButtonCallback
        'Retrieve the designator of the activated RefDes
        Dim RefDes As String = xGridExclusions.GetRowValues(e.VisibleIndex, "Designator")
        'Get current value
        Dim DNP As Boolean = CBool(xGridExclusions.GetRowValues(e.VisibleIndex, "DoNotPopulate"))
        Project.BomAnalysis.SetDoNotPopulate(RefDes, Not DNP) 'implement toggle
        saveState()
        'Update the display
        restoreState()
    End Sub

    Protected Sub xGridReview_HtmlRowPrepared(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridView.ASPxGridViewTableRowEventArgs) Handles xGridReview.HtmlRowPrepared
        'Fires for each row of the review grid after data is loaded into it, but before formatting. 
        'Used to suppress the Fried/Baked toggle for parts not allowed to be fried
        Try
            Dim fpID As Integer = CInt(e.GetValue("PartID"))
        Catch ex As InvalidCastException
            'Invalid PartID Hide the controls because not allowed to be Fried!
            For Each ctrl As Control In e.Row.Cells(0).Controls
                ctrl.Visible = False
            Next
            Exit Sub
        End Try
        'Valid control
        e.Row.Font.Bold = True
    End Sub
End Class
