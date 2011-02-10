
Partial Class pUser_New
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        'User Wishes to Create a New Account!
        Dim xMsgBox As DevExpress.Web.ASPxPopupControl.ASPxPopupControl
        xMsgBox = DirectCast(Master.FindControl("xMsgBox"), DevExpress.Web.ASPxPopupControl.ASPxPopupControl)

        'Validate USER'S NAME
        Dim userName As String = Me.lName.Text
        If (Not Len(userName) > 0) Then
            MsgBox(xMsgBox, sysErrors.USERADD_NOTREALNAME)
            Exit Sub
        Else
            userName = Trim(sysText.txtWhitesToSpaces(userName))
            If (Not sysText.txtNumOfSpaces(userName) >= 1) Then
                MsgBox(xMsgBox, sysErrors.USERADD_NOTREALNAME)
                Exit Sub
            Else
                'Probably the correct real name
            End If
        End If

        'Validate the USER'S EMAIL address
        Dim eml As String = HttpContext.Current.Session("user.Email") 'for security reasons -- DO NOT TRUST the client-side text box!
        eml = Trim(eml)
        If txtNumOfSubstrings(eml, "@") = 1 Then
            If txtNumOfSpaces(eml) = 0 Then
                'Valid E-mail address from OpenID!
            Else
                MsgBox(xMsgBox, sysErrors.USERADD_NOSHARE)
                Exit Sub
            End If
        Else
            MsgBox(xMsgBox, sysErrors.USERADD_NOSHARE)
            Exit Sub
        End If

        'ADD NEW USER! (If we made it this far, then the passed all validations!)
        Me.createUser(HttpContext.Current.Session("user.OpenID"), Me.lName.Text, Me.lEmail.Text)
        If sysUser.suLoginUser(HttpContext.Current.Session("user.email")) Then
            Response.Redirect(urlAppRoot() & "default.aspx?Code=" & sysErrors.USERADD_SUCCESS)
        Else
            MsgBox(xMsgBox, sysErrors.USERADD_CREATEFAILED)
        End If
    End Sub

    Private Function format_OpenID(ByVal OpenID As String) As String
        Const breakup_len As Byte = 50
        Dim i As Int16 = 1
        Do
            If i * breakup_len < Len(OpenID) Then
                OpenID = OpenID.Insert(breakup_len * i + i - 2, " ")
                i += 1
            Else
                Exit Do
            End If
        Loop
        Return OpenID
    End Function

    Private Sub createUser(ByVal OpenID As String, ByVal UserName As String, ByVal UserEmail As String)
        dbAcc.SQLexe( _
            "INSERT INTO [FriedParts].[dbo].[user-Accounts]" & _
            "           ([OpenID]" & _
            "           ,[UserName]" & _
            "           ,[UserEmail]" & _
            "           ,[RoleID]" & _
            "           ,[DateCreated])" & _
            "     VALUES (" & _
            "           '" & sysText.txtDefangSQL(OpenID) & "'," & _
            "           '" & sysText.txtDefangSQL(UserName) & "'," & _
            "           '" & sysText.txtDefangSQL(UserEmail) & "'," & _
            "           50," & _
            "           " & txtSQLDate(Now) & _
            "           )")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not (Page.IsCallback Or Page.IsPostBack) Then
            'Init xMsgBox
            Dim xMsgBox As DevExpress.Web.ASPxPopupControl.ASPxPopupControl
            xMsgBox = DirectCast(Master.FindControl("xMsgBox"), DevExpress.Web.ASPxPopupControl.ASPxPopupControl)

            'Run state machine
            Select Case HttpContext.Current.Session("user.Status")
                Case LoginStates.NotInterested
                    'ERROR: Should not be at this page.
                    Response.Redirect(urlAppRoot() & "default.aspx?Code=" & sysErrors.USERADD_NOINTEREST)
                Case LoginStates.Trying
                    'ERROR: Should first authenticate with OpenID provider!
                    MsgBox(xMsgBox, sysErrors.USERADD_NOAUTHCOMPLETE)
                Case LoginStates.Authenticated
                    'LET'S GET TO WORK! User is authenticated, but maybe not in our system. Let's get them online!
                    lAuth.Text = format_OpenID(HttpContext.Current.Session("user.OpenID"))
                    lName.Text = HttpContext.Current.Session("user.Name")
                    lEmail.Text = HttpContext.Current.Session("user.Email")
                Case LoginStates.Online
                    'ERROR: User is already recognized!
                    Response.Redirect(urlAppRoot() & "default.aspx?Code=" & sysErrors.USERADD_ALREADYIN)
            End Select
        End If
    End Sub
End Class
