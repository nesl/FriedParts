Imports System.Data
Imports System.Web
Imports System.Web.UI.WebControls
Imports DevExpress.Web.ASPxGridView

Partial Class pAdmin_newsAdd
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        suLoginRequired(Me) 'Login Required
    End Sub

End Class

