Imports System.Web
Imports System.Diagnostics 'for debug functions Debug.Write output goes to: (Debug-->Windows-->Immediate)

Partial Class FP
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'MsgBox Handling (Reports messages and errors to user)
        Dim passedIN As System.Collections.Specialized.NameValueCollection = HttpContext.Current.Request.QueryString()
        If passedIN("Code") <> 0 Then
            MsgBox(xMsgBox, passedIN("Code"), passedIN("P1"), passedIN("P2"))
        End If

        'Reroute after message box?
        '   Strip off the query string and redirect
        '   Server.Transfer(theURL.Scheme & theURL.Host & theURL.AbsolutePath)

        'Reroute after successful login...
        If Not (passedIN("Next") Is Nothing) Then
            If suUserLoggedIn() Then
                Dim theURL As New Uri(passedIN("Next"))
                Response.Redirect(theURL.AbsolutePath)
            End If
        End If

        'Default User Login State
        If HttpContext.Current.Session("user.Status") Is Nothing Then
            suConfigureSessionState()
        End If

        'User Login
        If Not (Page.IsCallback Or Page.IsPostBack) Then
            'FIRST TIME LOADING PAGE
            '[User Login Box]
            userLoginStatus.Text = sysUser.suDoIt()
            LoginUpdateStatus()
        Else
            'PAGE IS RELOADING (Callback or PostBack)
            '[MANAGE USER LOGIN]
            If (HttpContext.Current.Session("user.Status") = LoginStates.Trying) Then
                'PAGE RELOAD TRIGGERED BY LOGIN ATTEMPT AND POSSIBLY SOMETHING ELSE
                Try
                    userLoginStatus.Text = sysUser.suDoIt()
                    'If login has not yet succeeded wait one more second and check again
                    If Not HttpContext.Current.Session("user.Status") > LoginStates.Online Then
                        Response.AppendHeader("refresh", "1") 'Not yet authenticated so try again!
                    End If
                Catch ex As Exception
                    userLoginStatus.Text = "Holy shit! This should not happen!"
                End Try
            End If
            LoginUpdateStatus()
            '[OTHER ACTIONS NEEDED ON CALLBACK]
        End If
    End Sub

    Private Sub LoginUpdateStatus()
        Dim userStatus As Byte = HttpContext.Current.Session("user.Status")
        If Not (userStatus = LoginStates.NotInterested) Then
            'USER LOGGED IN
            xrpUserNotLoggedIn.Visible = False
            xrpUserLoggedIn.Visible = True
            If userStatus >= LoginStates.Online Then
                xrpUserLoggedIn.HeaderText = "Welcome " & sysText.txtGetWord(HttpContext.Current.Session("user.Name"), 1) & "!"
                userLoginStatus.Text = HttpContext.Current.Session("user.email")
                If userStatus = LoginStates.Online Then
                    imgNoDropbox.Visible = True
                    imgYesDropbox.Visible = False
                    userLoginStatus.Text = "Checking with Dropbox..."
                    divLinkDropboxAccount.Visible = False
                ElseIf userStatus = LoginStates.NoDropboxYet Then
                    HttpContext.Current.Session("user.Status") = LoginStates.StillNoDropboxYet
                    imgNoDropbox.Visible = True
                    imgYesDropbox.Visible = False
                    divLinkDropboxAccount.Visible = True
                ElseIf userStatus = LoginStates.StillNoDropboxYet Then
                    HttpContext.Current.Session("user.Status") = LoginStates.NoDropbox
                    DropBoxMessage.Visible = True 'Show the modal dialog box
                    imgNoDropbox.Visible = True
                    imgYesDropbox.Visible = False
                    divLinkDropboxAccount.Visible = True
                ElseIf userStatus = LoginStates.NoDropbox Then
                    imgNoDropbox.Visible = True
                    imgYesDropbox.Visible = False
                    divLinkDropboxAccount.Visible = True
                ElseIf userStatus = LoginStates.YesDropbox Then
                    imgNoDropbox.Visible = False
                    imgYesDropbox.Visible = True
                    divLinkDropboxAccount.Visible = False
                Else
                    Throw New Exception("user.Status (" & userStatus & ") in invalid state!")
                End If
            End If
        Else
            'NO USER LOGGED IN
            xrpUserLoggedIn.Visible = False
            xrpUserNotLoggedIn.Visible = True
        End If
    End Sub

    Protected Sub LoginWithGoogle_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles LoginWithGoogle.Click
        'If Not (Page.IsCallback Or Page.IsPostBack Or HttpContext.Current Is Nothing) Then
        'Only attempt login if prior state was not logged in and not currently trying to login
        Debug.WriteLine("Login clicked!")
        If HttpContext.Current.Session("user.Status") = LoginStates.NotInterested Then
            Console.WriteLine(sysUser.suSignInGoogle()) 'Write error output to the debug console
            Response.AppendHeader("Refresh", "1") 'Reload this page in 1 second (wait for authentication from OpenID provider)
        End If
    End Sub


    Protected Sub userLoginGoodbye_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles userLoginGoodbye.Click
        'HttpContext.Current.Session.Abandon()
        suConfigureSessionState()
        LoginUpdateStatus()
    End Sub

    Protected Sub SearchBox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchBox.TextChanged
        If (Not HttpContext.Current Is Nothing) Then _
            HttpContext.Current.Response.Redirect( _
            "/friedparts/pInv/invShowAll.aspx?Search=" & _
            SearchBox.Text)
    End Sub
End Class


