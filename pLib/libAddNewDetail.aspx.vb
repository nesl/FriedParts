Imports System.Data

Partial Class pInv_invAddNewDetail
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim passedIN As System.Collections.Specialized.NameValueCollection = HttpContext.Current.Request.QueryString()

        'FIELDS
        txtLibName.Text = fplibMakeLibName(fplibTypes.SchLib)
        txtLibType.Text = fplibLibTypeName(passedIN("LibType"))
        txtFileName.Text = passedIN("Filename")
        txtOwner.Text = HttpContext.Current.Session("user.Name")
        txtDesc.Text = ""

        'TITLES
        TitleMain.InnerText = fplibMakeLibName(fplibTypes.SchLib)
        TitleSub.InnerText = fplibLibTypeName(passedIN("LibType"))

    End Sub

    Protected Sub Button_NewName_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button_NewName.ServerClick

        'txtLibName.Text
        'txtLibType.Text
        'txtFileName.Text

        'txtOwner.Text

        txtDesc.Text = Environment.MachineName.ToString() & Environment.UserName.ToString() & Environment.OSVersion.ToString()
        'xMsgBox.ShowOnPageLoad = True
    End Sub
End Class
