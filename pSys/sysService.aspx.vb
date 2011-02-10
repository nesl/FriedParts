
Partial Class pSys_sysService
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        sysUser.suLoginRequired(Me)
    End Sub
End Class
