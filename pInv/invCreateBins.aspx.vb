
Partial Class pInv_invCreateBins
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        sysUser.suLoginRequired(Me) 'Access control
        If Not IsCallback Or IsPostBack Then
            'Initial Page Load!
        End If
    End Sub

    Protected Sub xBtnAddBins_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles xBtnAddBins.Click
        Dim usesSlots As Boolean = False
        'WAREHOUSE
        If lbxWarehouse.SelectedIndex = -1 Then
            disp_error("You must select the warehouse where this bin box is located.")
            Exit Sub
        End If

        'CABINET
        xTxtBin1.Text = Trim(UCase(xTxtBin1.Text))
        If xTxtBin1.Text.Length <> 1 Then
            disp_error("You must enter a valid cabinet designator. You entered too many or too few characters.")
            xTxtBin1.Text = ""
            Exit Sub
        Else
            If Not Char.IsLetter(xTxtBin1.Text, 0) Then
                disp_error("The Cabinet designator must be a single letter.")
                xTxtBin1.Text = ""
                Exit Sub
            End If
        End If

        'DRAWER
        xTxtBin2.Text = Trim(xTxtBin2.Text)
        If Not IsNumeric(xTxtBin2.Text) Then
            disp_error("The Drawer designator must be a number.")
            xTxtBin2.Text = ""
            Exit Sub
        End If

        'SLOT START
        xTxtBin3.Text = Trim(xTxtBin3.Text)
        If xTxtBin3.Text.Length > 0 Then
            If Not IsNumeric(xTxtBin3.Text) Then
                'Entered something, but it wasn't numeric
                disp_error("Slot designators must be a number. If you do not want to designate a slot within the drawer then leave the slot fields blank.")
                xTxtBin3.Text = ""
                Exit Sub
            Else
                'entered something and it is a valid number
                usesSlots = True
            End If
        Else
            'Did not enter a slot
        End If

        'SLOT END
        xTxtBin4.Text = Trim(xTxtBin4.Text)
        If xTxtBin4.Text.Length > 0 Then
            If Not usesSlots Then
                disp_error("If you want to designate only one specific slot, use the first box. Otherwise you are missing the start of range value.")
                xTxtBin4.Text = ""
                Exit Sub
            Else
                'Uses slots! -- test for valid range
                If Not IsNumeric(xTxtBin4.Text) Then
                    'Entered something, but it wasn't numeric
                    disp_error("Slot designators must be a number. If you do not want to designate a slot within the drawer then leave the slot fields blank.")
                    xTxtBin4.Text = ""
                    Exit Sub
                Else
                    Dim slot_start As Byte
                    Dim slot_stop As Byte
                    slot_start = CByte(xTxtBin3.Text)
                    slot_stop = CByte(xTxtBin4.Text)
                    If slot_stop <= slot_start Then
                        disp_error("The provided slot values do not create a valid range. The range must increase in value from start to stop.")
                        Exit Sub
                    Else
                        'VALID SLOT RANGE!
                        Dim result As fpInv.fpInvBinError = invAddAvailableBin(Me, lbxWarehouse.SelectedItem.Value, xTxtBin1.Text, xTxtBin2.Text, xTxtBin3.Text, xTxtBin4.Text)
                        Select Case result.PartID_or_ErrCode
                            Case sysErrors.NO_ERROR
                                'Normal case, proceed!
                            Case sysErrors.ERR_NOTUNIQUE
                                'System Datatable problem -- multiple assigned parts to bin -- is that an error? Maybe we want to allow this
                                Err.Raise(-23432, , "Found multiple part numbers assigned to warehouse " & invGetWarehouseName(result.WarehouseID) & " bin(s) " & result.BinName & ". The first of which was " & result.PartID_or_ErrCode & " (" & fpParts.partGetShortName(result.PartID_or_ErrCode) & ")")
                            Case Else
                                lblPopHeader.Text = "This Bin Location is already in use."
                                lblPopWarehouse.Text = invGetWarehouseName(result.WarehouseID)
                                lblPopBin.Text = result.BinName
                                lblPopCurrentContents.Text = fpParts.partGetShortName(result.PartID_or_ErrCode)
                                HttpContext.Current.Session("inv.ConflictBin") = result
                                xPopupReplace.ShowOnPageLoad = True
                        End Select
                    End If
                End If
            End If
        Else
            If usesSlots Then
                'VALID SINGLE SLOT SPECIFIED
                Dim result As fpInv.fpInvBinError = invAddAvailableBin(Me, lbxWarehouse.SelectedItem.Value, xTxtBin1.Text, xTxtBin2.Text, xTxtBin3.Text)
                Select Case result.PartID_or_ErrCode
                    Case sysErrors.NO_ERROR
                        'Normal case, proceed!
                    Case sysErrors.ERR_NOTUNIQUE
                        'System Datatable problem -- multiple assigned parts to bin -- is that an error? Maybe we want to allow this
                        Err.Raise(-23432, , "Found multiple part numbers assigned to warehouse " & invGetWarehouseName(result.WarehouseID) & " bin(s) " & result.BinName & ". The first of which was " & result.PartID_or_ErrCode & " (" & fpParts.partGetShortName(result.PartID_or_ErrCode) & ")")
                    Case Else
                        lblPopHeader.Text = "This Bin Location is already in use."
                        lblPopWarehouse.Text = invGetWarehouseName(result.WarehouseID)
                        lblPopBin.Text = result.BinName
                        lblPopCurrentContents.Text = fpParts.partGetShortName(result.PartID_or_ErrCode)
                        HttpContext.Current.Session("inv.ConflictBin") = result
                        xPopupReplace.ShowOnPageLoad = True
                End Select
            Else
                'VALID NO SLOT SPECIFIED
                Dim result As fpInv.fpInvBinError = invAddAvailableBin(Me, lbxWarehouse.SelectedItem.Value, xTxtBin1.Text, xTxtBin2.Text)
                Select Case result.PartID_or_ErrCode
                    Case sysErrors.NO_ERROR
                        'Normal case, proceed!
                    Case sysErrors.ERR_NOTUNIQUE
                        'System Datatable problem -- multiple assigned parts to bin -- is that an error? Maybe we want to allow this
                        Err.Raise(-23432, , "Found multiple part numbers assigned to warehouse " & invGetWarehouseName(result.WarehouseID) & " bin(s) " & result.BinName & ". The first of which was " & result.PartID_or_ErrCode & " (" & fpParts.partGetShortName(result.PartID_or_ErrCode) & ")")
                    Case Else
                        lblPopHeader.Text = "This Bin Location is already in use."
                        lblPopWarehouse.Text = invGetWarehouseName(result.WarehouseID)
                        lblPopBin.Text = result.BinName
                        lblPopCurrentContents.Text = fpParts.partGetShortName(result.PartID_or_ErrCode)
                        HttpContext.Current.Session("inv.ConflictBin") = result
                        xPopupReplace.ShowOnPageLoad = True
                End Select
            End If
        End If
    End Sub

    Private Sub disp_error(ByRef Msg As String)
        lblError.Text = Msg
        lblError.Visible = True
        divErrorBox.Visible = True
    End Sub

    Protected Sub btnPopCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPopCancel.Click
        xPopupReplace.ShowOnPageLoad = False
    End Sub

    Protected Sub btnPopYes_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPopYes.Click
        'Restore State
        Dim result As fpInv.fpInvBinError = HttpContext.Current.Session("inv.ConflictBin")

        'Mark this BinBox as available
        Dim sqlTxt As String = _
            "DELETE FROM [FriedParts].[dbo].[inv-Bins] " & _
            "WHERE [WarehouseID] = " & result.WarehouseID & _
            "  AND [BinLocation] = '" & result.BinName & "'" & _
            "  AND [PartID] = '" & result.PartID_or_ErrCode & "'"
        SQLexe(sqlTxt)

        'LOG CHANGE
        logUserActivity(Me, suGetUserFirstName() & " removed part #" & result.PartID_or_ErrCode & " from bin location " & result.BinName & " in warehouse " & invGetWarehouseName(result.WarehouseID), sqlTxt, , "WarehouseID", result.WarehouseID, , "BinLocation", result.BinName, , "PartID", result.PartID_or_ErrCode)

        'Warn User
        lblError.Text = "WARNING: Your last bin creation process was interrupted because a part already existed in it. That part has been removed now, but you must attempt your bin creation again. This can pose challenges when you attempt to create a slot range with occupancies occuring in the middle. Be careful and check the dashboard to confirm that the boxes you want were created."
        divErrorBox.Visible = True
        xPopupReplace.ShowOnPageLoad = False
    End Sub
End Class
