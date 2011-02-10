Imports System.Diagnostics

Partial Class pLib_libBrowse
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        scanUpdate()
        xGridViewPcb.DataBind()
        xGridViewSch.DataBind()
    End Sub

    Protected Sub scanUpdate()
        Dim column As DevExpress.Web.ASPxGridView.GridViewDataHyperLinkColumn
        column = xGridViewSch.Columns("LibName")
        column.PropertiesHyperLinkEdit.NavigateUrlFormatString = urlAppRoot() & "Lib_Altium/{0}"
        column = xGridViewPcb.Columns("LibName")
        column.PropertiesHyperLinkEdit.NavigateUrlFormatString = urlAppRoot() & "Lib_Altium/{0}"
    End Sub
End Class
