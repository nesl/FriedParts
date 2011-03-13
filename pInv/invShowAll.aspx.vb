Imports fpSearch

Partial Class pInv_invShowAll
    Inherits System.Web.UI.Page

    '''<summary>The PartTypeHorizontalAccordion Psuedo-Control</summary>
    Protected ptAccordion As System.Web.UI.FriedParts.PartTypeAccordionControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '======================================================================
        '== PartTypeAccordion (Psuedo-Control) (put this first so we can manipulate its object in other Page_Load functions if desired)
        '======================================================================
        Const ptAccordianSessionName As String = "padd.PartTypeControl0"
        ptAccordion = System.Web.UI.FriedParts.PartTypeAccordionControl.Reload(Me, ptAccordianSessionName)
        ptAccordion.UpdateControls(Me, New System.Web.UI.FriedParts.PartTypeAccordionControl.OnChange(AddressOf PartType_OnChange))
        For Each grid As DevExpress.Web.ASPxGridView.ASPxGridView In ptAccordion.GetGrids()
            AddHandler grid.CustomCallback, AddressOf ptAccordion.HandleRowChanged
        Next
        '======================================================================

        'Is there a search? (UI)
        If HttpContext.Current.Request.QueryString("Search") IsNot Nothing Then
            If HttpContext.Current.Request.QueryString("Search").CompareTo("") = 0 Then
                'No search -- Reset page (e.g. show all parts)
                Page_Load(True)
            Else
                'Do search
                SearchWarning.Visible = True
                SearchWarning.InnerText = "Displaying results for " & HttpUtility.HtmlEncode(HttpContext.Current.Request.QueryString("Search")) & "."
            End If
        End If

        If (Not (IsCallback Or IsPostBack)) Then
            'Do any page load work in here!
            Page_Load()
        Else
            'Reload Search Results state if available
            If HttpContext.Current.Session("inv.SearchResults") IsNot Nothing Then
                If SelectedPartTypeID.Value Is Nothing Then
                    SelectedPartTypeID.Value = 0 'Reset to ALL PARTS
                End If
                xGV.DataSource = ptGetPartsOfThisType( _
                        SelectedPartTypeID.Value,
                        DirectCast(HttpContext.Current.Session("inv.SearchResults"), fpSearch.searchResults).getDataSource() _
                    )
                xGV.DataBind()
            Else
                Page_Load()
            End If
        End If
    End Sub

    Private Sub Page_Load(Optional ByRef Reset As Boolean = False)
        Dim blah As System.Collections.Specialized.NameValueCollection = HttpContext.Current.Request.QueryString()
        Dim objSR As fpSearch.searchResults
        'Do DB lookup and bind data view
        If Reset Then
            objSR = New fpSearch.searchResults(Nothing)
            ptAccordion.ResetToAllParts() 'Reset the part catagory to "All Parts"
        Else
            objSR = New fpSearch.searchResults(blah("Search"))
        End If
        HttpContext.Current.Session("inv.SearchResults") = objSR
        xGV.KeyFieldName = "PartID" 'Set Primary Key for row selection
        xGV.DataSource = DirectCast(HttpContext.Current.Session("inv.SearchResults"), fpSearch.searchResults).getDataSource()
        xGV.DataBind()
    End Sub

    ''' <summary>
    ''' Event Handler for the Part Type Accordion. Fires when the user selects a new Part Type.
    ''' </summary>
    ''' <param name="NewTypeID"></param>
    ''' <remarks></remarks>
    Public Sub PartType_OnChange(ByVal NewTypeID As Int32)
        Dim results As fpSearch.searchResults = DirectCast(HttpContext.Current.Session("inv.SearchResults"), fpSearch.searchResults)
        xGV.DataSource = results.filterByPartType(NewTypeID)
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