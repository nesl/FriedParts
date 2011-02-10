
Partial Class pInv_invShowAll
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not (IsCallback Or IsPostBack)) Then
            'Do any page load work in here!
            Page_Load()
        Else
            'Reload Search Results state if available
            If Not HttpContext.Current.Session("inv.SearchResults") Is Nothing Then
                xGV.DataSource = HttpContext.Current.Session("inv.SearchResults")
                xGV.DataBind()
            Else
                Page_Load()
            End If
        End If
    End Sub

    Private Sub Page_Load()
        Dim blah As System.Collections.Specialized.NameValueCollection = HttpContext.Current.Request.QueryString()
        'Do DB lookup and bind data view
        HttpContext.Current.Session("inv.SearchResults") = fpSearchDataSource(blah("Search"))
        xGV.KeyFieldName = "PartID" 'Set Primary Key for row selection
        xGV.DataSource = HttpContext.Current.Session("inv.SearchResults")
        xGV.DataBind()
    End Sub


    Protected Sub xGV_FocusedRowChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles xGV.FocusedRowChanged
        'Fires when the user clicks on a row in the grid (basically this is handled on the client-side right now)

    End Sub

    Protected Sub xGV_HtmlRowPrepared(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridView.ASPxGridViewTableRowEventArgs) Handles xGV.HtmlRowPrepared
        'Fires for each row of the inventory grid after data is loaded into it, but before formatting. Used to implement data specific display issues (like hilighting local inventory)
        If invLocalStock(Convert.ToInt32(e.GetValue("PartID"))) >= 0 Then
            e.Row.Font.Bold = True
        End If
    End Sub
End Class