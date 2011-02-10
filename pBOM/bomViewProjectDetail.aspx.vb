
Partial Class pBOM_bomViewProjectDetail
    Inherits System.Web.UI.Page
    Dim ProjectID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim passedIN As System.Collections.Specialized.NameValueCollection = HttpContext.Current.Request.QueryString()

        If Not (Page.IsCallback Or Page.IsPostBack) AndAlso Not passedIN("detail") Is Nothing AndAlso passedIN("detail").Length > 0 Then
            'Title and Revisions
            ProjectID = CInt(passedIN("detail"))
            Dim theProject As New fpBOM(ProjectID)
            lblTitle.Text = theProject.GetTitle
            litRevisions.Text = theProject.GetRevisions(True)

            'Description
            lblDescription.Text = theProject.GetDescription

            'Project Details
            lblProjectID.Text = ProjectID
            lblOwner.Text = theProject.GetOwnerName
            lblDateCreated.Text = theProject.GetDateCreated
            lblRev.Text = theProject.GetRevision

            'View Reports
            aViewReports.NavigateUrl = "./bomReport.aspx?detail=" & ProjectID
            aViewReports.Target = "_blank"

            'Files
            lnkAllFiles.NavigateUrl = ""
            lnkAllFiles.Target = "_blank"


        End If

    End Sub
End Class
