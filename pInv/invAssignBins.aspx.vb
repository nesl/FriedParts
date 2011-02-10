
Partial Class pInv_invAssignBins
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        sysUser.suLoginRequired(Me) 'Access control
        If Not (IsCallback Or IsPostBack) Then
            'Initial Page Load

            'If we were not called with a "Part" parameter then just show all the parts for the user to select one.
            Dim passedIN As System.Collections.Specialized.NameValueCollection = HttpContext.Current.Request.QueryString()
            If passedIN("FPID") Is Nothing Then
                Server.Transfer("./invShowAll.aspx")
            Else
                'Load the local page since we are trying to add a new part...
                hidPartID.Value = passedIN("FPID")
            End If
        End If
    End Sub


    Protected Sub lbxWarehouse_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbxWarehouse.SelectedIndexChanged
        lbxBins.DataBind()
        BinRow.Visible = True 'turn on the bins list
    End Sub


    Protected Sub lbxBins_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbxBins.SelectedIndexChanged
        QtyRow.Visible = True
    End Sub


    Protected Sub txtQty_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQty.TextChanged
        txtQty.Text = Trim(txtQty.Text)
        If IsNumeric(txtQty.Text) Then
            SubmitRow.Visible = True
        Else
            txtQty.Text = ""
        End If
        If txtQty.Text.Length = 0 Then SubmitRow.Visible = False
    End Sub


    '=======================
    '== ASSIGN BIN BUTTON ==
    '=======================
    Protected Sub btnAssignBin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAssignBin.Click
        'Add Entry to [inv-Bins]
        Dim sqlTxt As String = _
            "INSERT INTO [FriedParts].[dbo].[inv-Bins]" & _
            "           ([PartID]" & _
            "           ,[WarehouseID]" & _
            "           ,[BinLocation]" & _
            "           ,[QtyHere])" & _
            "     VALUES (" & _
            "           " & hidPartID.Value & "," & _
            "           " & lbxWarehouse.SelectedItem.Value & "," & _
            "           '" & lbxBins.SelectedItem.Value & "'," & _
            "           " & CInt(txtQty.Text) & "" & _
            "           )"
        SQLexe(sqlTxt)

        'Remove from available bins
        fpInv.invUpdateAvailableBins()

        'LOG
        logUserActivity(Me, suGetUserFirstName() & " assigned part #" & hidPartID.Value & " (" & partGetShortName(hidPartID.Value) & ") to bin location " & lbxBins.SelectedItem.Value & " in warehouse " & invGetWarehouseName(lbxWarehouse.SelectedItem.Value), sqlTxt, , "PartID", , hidPartID.Value, "WarehouseID", , lbxWarehouse.SelectedItem.Value, "BinLocation", , lbxBins.SelectedItem.Value)

        'REPORT
        MsgBox_Transfer(Me, "invShowDetail.aspx", sysErrors.BINASSIGN_SUCCESS, hidPartID.Value, lbxBins.SelectedItem.Value, hidPartID.Value)
    End Sub
End Class
