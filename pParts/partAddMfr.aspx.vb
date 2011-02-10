
Partial Class pParts_partAddMfr
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        sysUser.suLoginRequired(Me) 'Access control
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        'Init
        errRow.Visible = False

        'Manufacturer was entered
        If txtMfrName.Text.Length = 0 Then
            submitError("You must enter a manufacturer name! duh...")
            Exit Sub
        End If

        'Manufacturer does not already exist
        If Not fpMfr.mfrExistsUniqueName(txtMfrName.Text, True) = sysErrors.ERR_NOTFOUND Then
            submitError("This manufacturer already exists in the database.")
            Exit Sub
        End If

        'Website was entered
        If txtMfrWebsite.Text.Length = 0 Then
            submitError("You must enter the manufacturer's website.")
            Exit Sub
        End If

        'URL valid
        If Not sysText.txtValidURL(txtMfrWebsite.Text) Then
            submitError("The URL entered is not in a valid format. Example: http://www.maxim-ic.com")
        End If

        'ALL CHECKS PASSED! LET'S ADD!
        mfrAdd(txtMfrName.Text, txtMfrWebsite.Text.ToLower(), Me) 'Files log entry

        'Return
        MsgBox(Me, sysErrors.MFRADD_SUCCESS, txtMfrName.Text)
    End Sub

    Private Sub submitError(ByVal errMessage As String)
        errRow.Visible = True
        errMsg.Text = errMessage
    End Sub

End Class
