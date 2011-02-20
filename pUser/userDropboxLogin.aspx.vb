Imports apiDropbox
Imports apiDropbox.DropboxStatic

Partial Class pUser_userDropboxLogin
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        suLoginRequired(Me)
        'Hide "Development Status" instructions / data input fields
        If DropboxInProductionStatus Then
            dev1.Visible = False
            dev2.Visible = False
            dev3.Visible = False
        End If
    End Sub

    '=======================
    '= Link Account Button =
    '=======================
    Protected Sub btnDropboxLinkAccount_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDropboxLinkAccount.Click

        'Validate Text Fields
        If txtUsername.Text.Length < 3 Then
            errMsgBox("You must enter your dropbox account username")
            Exit Sub
        End If
        If txtPassword.Text.Length < 6 Then
            errMsgBox("You forgot to enter your Dropbox account password. FriedParts will not store your password.")
            Exit Sub
        End If
        If Not DropboxInProductionStatus Then
            If txtAppKey.Text.Length < 10 Then
                errMsgBox("You forgot to enter your App Key. If you do not have one (or know what one is) refer to the instructions on this page.")
                Exit Sub
            End If
            If txtAppSecret.Text.Length < 10 Then
                errMsgBox("You forgot to enter your App Secret. If you do not have one (or know what one is) refer to the instructions on this page.")
                Exit Sub
            End If
        End If

        'Select the Appropriate AppKeys
        Dim theAppKey As String
        Dim theAppSecret As String
        If DropboxInProductionStatus Then
            'Use the production keys
            theAppKey = DropboxAppKey
            theAppSecret = DropboxAppSecret
        Else
            'Use the use provided keys
            theAppKey = txtAppKey.Text
            theAppSecret = txtAppSecret.Text
        End If

        'All OK!
        errBox.Visible = False

        'Test Login / Retrieve User Token (keys)
        Dim DbxClient As New DropNet.DropNetClient(theAppKey, theAppSecret)
        Dim UserToken As DropNet.Models.UserLogin = DbxClient.Login(txtUsername.Text, txtPassword.Text)
        If UserToken.Token Is Nothing Or UserToken.Secret Is Nothing Then
            errMsgBox("Uh oh! Dropbox told me to buzz off! Something's wrong with at least one of your entries! All four entries must be correct or linking will fail.")
            Exit Sub
        End If

        'SUCCESS! -- Save to Database
        Dim Tokens As New DropboxUserCredentials(UserToken.Token, UserToken.Secret, theAppKey, theAppSecret)
        apiDropbox.apiDropboxSaveUserCredentials(HttpContext.Current.Session("user.UserID"), Tokens)

        'Update State
        Dim DbxAccount As DropNet.Models.AccountInfo = DbxClient.Account_Info
        HttpContext.Current.Session("user.Status") = LoginStates.YesDropbox
        dbLog.logUserActivity(Me, suGetUserFirstName() & " linked his Dropbox account to FriedParts. You should too!", , , "Dropbox_Displayname", DbxAccount.display_name, "Dropbox_Email", DbxAccount.email)
        MsgBox_Transfer(Me, "/FriedParts/default.aspx", sysErrors.DROPBOX_LINK_SUCCESS, sysUser.suGetUserFirstName(), DbxClient.Account_Info().display_name)
    End Sub

    Private Sub errMsgBox(ByRef ErrText As String)
        errBox.Visible = True
        lblErrMsg.Text = ErrText
    End Sub
End Class
