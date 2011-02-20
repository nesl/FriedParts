Imports System.Data

Partial Class pBOM_bomReport
    Inherits System.Web.UI.Page

    'If you click on a different project in the projects table, then we update the session variable which is used as a parameter-passing mechanism to the report itself
    Protected Sub xGridProjects_FocusedRowChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles xGridProjects.FocusedRowChanged
        Dim drow As DataRow = xGridProjects.GetDataRow(xGridProjects.FocusedRowIndex)
        If drow IsNot Nothing Then
            'ReportWindow.Visible = True
            HttpContext.Current.Session("bom.ProjectID") = drow.Field(Of Integer)("ProjectID")
        Else
            'Do nothing... keep current report displayed (this really only happens on initial page load)
            'ReportWindow.Visible = False
        End If
    End Sub

    Protected Sub xGridProjects_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles xGridProjects.Load
        Dim passedIN As System.Collections.Specialized.NameValueCollection = HttpContext.Current.Request.QueryString()
        If Not (Page.IsCallback Or Page.IsPostBack) Then
            If Not passedIN("detail") Is Nothing AndAlso passedIN("detail").Length > 0 Then
                'Specific Project is being requested
                HttpContext.Current.Session("bom.ProjectID") = passedIN("detail") 'Select requested project
                xTabPages.ActiveTabIndex = 1 'Start with BOM tab
            Else
                'Initial page load
                HttpContext.Current.Session("bom.ProjectID") = 2 'Default it to anything valid
                xTabPages.ActiveTabIndex = 0 'Start with project select tab
            End If
        End If
    End Sub
End Class
