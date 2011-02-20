Imports apiDropbox

Partial Class pAdmin_filesDropbox
    Inherits System.Web.UI.Page
    Protected theDropbox As apiDropbox.dropboxUser

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        suLoginRequired(Me) 'Access Control

        'Login
        If HttpContext.Current.Session("dropbox.Account") Is Nothing Then
            'Save
            theDropbox = New apiDropbox.dropboxUser(HttpContext.Current.Session("user.UserID"))
            HttpContext.Current.Session("dropbox.Account") = theDropbox 'Save!
        Else
            'Restore
            theDropbox = HttpContext.Current.Session("dropbox.Account")
        End If

        If Not (IsCallback Or IsPostBack) Then
            'Initial Page Load

            'Update Status
            lblAccountHolder.Text = theDropbox.GetAccountInfo.display_name & " (" & theDropbox.GetAccountInfo.email & ")"
            If Not HttpContext.Current.Session("dropbox.Status") Is Nothing Then
                lblStatus.Text = HttpContext.Current.Session("dropbox.Status")
            Else
                lblStatus.Text = "Uninitialized (Null)"
            End If
        End If
    End Sub

    Protected Sub btnTest_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTest.Click
        'Force DBx Sync
        Dim TestMe As New dropboxUser(HttpContext.Current.Session("user.UserID"))
        TestMe.Sync()

        'Refresh grid!
        SqlDataSource1.DataBind()
        xGridUserLog.DataBind()
    End Sub
End Class
