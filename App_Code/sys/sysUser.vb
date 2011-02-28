Imports DotNetOpenAuth.OpenId 'Identifier
Imports DotNetOpenAuth.OpenId.RelyingParty
Imports DotNetOpenAuth.Messaging 'ProtocolException
Imports DotNetOpenAuth.OpenId.Extensions.SimpleRegistration 'ClaimsRequest
Imports System.Web 'HttpContext.Current.Session, HttpContext.Current.Response (for Redirect)
Imports Microsoft.VisualBasic
Imports System.Diagnostics 'for Debug.Write
Imports System.Data 'For Logging
Imports apiDropbox 'For Dropbox States

'===========================
'SESSION STATE DOCUMENTATION (PARTIAL, ONLY THE PART ABOUT USER STUFF -- see sysEnv for full documentation)
'===========================
'HttpContext.Current.Session("user.OpenID") 
'   user.Status     -- of type LoginStates
'   user.OpenID     -- OpenID authenticated ID
'   user.Name       -- User's registered real full name
'   user.email      -- User's registered e-mail address
'   user.UserID     -- Our DB's UserID number
'   user.RoleID     -- Our DB's Permission level for this user (number)
'   user.RoleDesc   -- Our DB's Permission level for this user (description)

Public Module sysUser
    Public Const SYSTEM_USERNAME As String = "System Context"
    Public Const SYSTEM_USERID As Int32 = 0
    Public Const OpenIDEndPointGoogle As String = "https://www.google.com/accounts/o8/id"

    Public Enum LoginStates As Byte
        NotInterested = 1   'Not logged in, but not trying to log in
        Trying = 2  'Attempting to Login
        Authenticated = 3  'Login authentication with OpenID Provider Successful, but not registered on our system yet
        Online = 4 'Authenticated session and registered in our database
        NoDropboxYet = 5 'Authenticated, Registered, but no working Dropbox credentials -- used to display the modal dialog box
        StillNoDropboxYet = 6 'Authenticated, Registered, but no working Dropbox credentials -- used to display the modal dialog box
        NoDropbox = 7 'Authenticated, Registered, but no working Dropbox credentials
        YesDropbox = 100 'Authenticated, Registered, Dropboxed!
    End Enum

    'Reset and configure the session state back to its initial (logged out) status
    Public Sub suConfigureSessionState()

        HttpContext.Current.Session.Clear()
        HttpContext.Current.Session.Add("user.Status", LoginStates.NotInterested)
        HttpContext.Current.Session.Add("user.OpenId", "uninitialized")
        HttpContext.Current.Session.Add("user.UserId", "uninitialized")
        HttpContext.Current.Session.Add("user.email", "uninitialized")
        HttpContext.Current.Session.Add("user.Name", "uninitialized")
    End Sub

    'Returns error text
    Public Function suSignInGoogle() As String
        Try
            HttpContext.Current.Session("user.Status") = LoginStates.Trying
            Dim openid As New OpenIdRelyingParty
            Using (openid)
                Dim request As IAuthenticationRequest = openid.CreateRequest(OpenIDEndPointGoogle)
                Dim SimpleReg As New ClaimsRequest

                'This is where you would add any OpenID extensions you wanted
                'to include in the authentication request.
                With SimpleReg
                    .FullName = DemandLevel.Require
                    .Email = DemandLevel.Require
                    .Country = DemandLevel.Request
                    .Gender = DemandLevel.Request
                    .PostalCode = DemandLevel.Request
                    .TimeZone = DemandLevel.Request
                End With
                request.AddExtension(SimpleReg)

                'Send your visitor to their Provider for authentication.
                request.RedirectToProvider()
            End Using
            Return "Submitted. Waiting."
        Catch ex As ProtocolException
            Return ex.Message.ToString
            'The user probably entered an Identifier that 
            'was not a valid OpenID endpoint.
            'Me.openidValidator.Text = ex.Message
            'Me.openidValidator.IsValid = False
        End Try
    End Function

    'Checks current login status and returns state
    Public Function suDoIt() As String
        'HttpContext.Current.Session.Abandon()
        'HttpContext.Current.Session.Clear() 'Reset Session (Logout)
        Try
            Dim test As String = HttpContext.Current.Session("user.Status")
        Catch ex As System.Exception
            If TypeOf ex Is System.NullReferenceException Then
                suConfigureSessionState()
            End If
        End Try
        Try
            If Len(HttpContext.Current.Session("user.Status")) > 0 Then
                Select Case HttpContext.Current.Session("user.Status")
                    Case LoginStates.Authenticated
                        'This code is actually never reached -- this state occurs inside of suLoginUser(), but just in case...
                        Return "Logged In: " & HttpContext.Current.Session("jName")
                    Case LoginStates.Trying
                        'Log Me In!
                        Dim OpenID_RP As New OpenIdRelyingParty
                        Dim OPresponse = OpenID_RP.GetResponse()
                        If (Not OPresponse Is Nothing) Then
                            Select Case (OPresponse.Status)
                                Case AuthenticationStatus.Authenticated
                                    'This is where you would look for any OpenID extension OPresponses included
                                    'in the authentication assertion.
                                    Dim gotClaims As ClaimsResponse
                                    gotClaims = OPresponse.GetExtension(Of ClaimsResponse)()
                                    HttpContext.Current.Session("user.OpenId") = OPresponse.ClaimedIdentifier.ToString
                                    HttpContext.Current.Session("user.email") = gotClaims.Email.ToString
                                    HttpContext.Current.Session("user.Status") = LoginStates.Authenticated 'Update state to confirm authentication!

                                    'Check if valid local user
                                    If (suLoginUser(gotClaims.Email.ToString)) Then
                                        'Local Login Complete! Valid Local User!
                                        'Login to Dropbox
                                        If (apiDropbox.loginDropboxUser(HttpContext.Current.Session("user.UserID")) = DropboxErrors.Successful) Then
                                            'Successful Dropbox Login
                                            HttpContext.Current.Session("user.Status") = LoginStates.YesDropbox
                                        Else
                                            'Unsuccessful Dropbox Login
                                            HttpContext.Current.Session("user.Status") = LoginStates.NoDropboxYet
                                        End If
                                        Return "Login Successful! Welcome: " & HttpContext.Current.Session("user.Name")
                                    Else
                                        'Failed Local User Check... New User Account Request?
                                        HttpContext.Current.Response.Redirect("/FriedParts/pUser/userNew.aspx")
                                    End If

                                Case AuthenticationStatus.Canceled
                                    HttpContext.Current.Session("user.Status") = LoginStates.NotInterested 'Stop trying to login until user re-requests
                                    Return "User cancelled Login. Why? This site is awesome!"
                                Case AuthenticationStatus.Failed
                                    HttpContext.Current.Session("user.Status") = LoginStates.NotInterested 'Stop trying to login until user re-requests
                                    Return "Invalid User Login. Bad Username/Password?"
                            End Select
                        End If
                        Return "No OPresponse from OpenID Endpoint!"
                End Select
            Else
                'Do Nothing (User not logged in, but doesn't want to be)
                'HttpContext.Current.Session("user.Status") = LoginStates.NotInterested 'Stop trying to login until user re-requests
                Return "Do Nothing (User not logged in, but doesn't want to be)"
            End If
            Return "[suDoIt] ERROR! SHOULD NOT REACH HERE! " & HttpContext.Current.Session("user.Status")
        Catch ex As Exception
            Debug.WriteLine(ex.GetType.ToString & ": " & ex.Message)
            Return "ERROR! BOO!: " & ex.Message
        End Try
    End Function

    'Log's In a User
    '   1. Delete's any existing user session
    '   2. Create's a new user session
    '   3. Returns FALSE if OpenID does not match anyone in the database's user table
    'We check userId based on OpenId associated e-mail address since Google does not return the same
    '   OpenID for the user each time. Lord only knows why they fucked up OpenId that badly.
    Public Function suLoginUser(ByVal OpenIDEmail As String) As Boolean
        'Is this a recognized user?
        Dim result As New System.Data.DataTable
        result = dbAcc.SelectRows(result, _
            "SELECT [UserID]" & _
            "      ,[OpenID]" & _
            "      ,[RoleID]" & _
            "      ,[RoleDesc]" & _
            "      ,[UserName]" & _
            "      ,[UserEmail]" & _
            "      ,[DateCreated]" & _
            "  FROM [FriedParts].[dbo].[view-user] " & _
            "WHERE [UserEmail] = '" & sysText.txtDefangSQL(OpenIDEmail) & "'") 'Defang entry here so it matches the way we stored it!
        If Not result.Rows.Count = 1 Then
            'Error! Not found or found too many!
            Return False
        End If

        'Delete Old Session (just to be safe!)
        suConfigureSessionState()

        'Copy User from Database
        HttpContext.Current.Session("user.OpenID") = result.Rows(0).Item("OpenID").ToString
        HttpContext.Current.Session("user.Name") = result.Rows(0).Item("UserName").ToString
        HttpContext.Current.Session("user.email") = result.Rows(0).Item("UserEmail").ToString
        HttpContext.Current.Session("user.UserID") = result.Rows(0).Item("UserID").ToString 'Database ID for this user
        HttpContext.Current.Session("user.RoleID") = result.Rows(0).Item("RoleID").ToString 'Permission Group ID for this user
        HttpContext.Current.Session("user.RoleDesc") = result.Rows(0).Item("RoleDesc").ToString 'Permissions group description

        'Indicate Successful Login!
        'Use FormsAuthentication to tell ASP.NET that the user is now logged in,
        'with the OpenID Claimed Identifier as their username.
        HttpContext.Current.Session("user.Status") = LoginStates.Online
        HttpContext.Current.Response.AppendHeader("refresh", "0") 'force page callback! (reload)
        'FormsAuthentication.RedirectFromLoginPage(result.Rows.Item("OpenID").ToString, False)
        Return True
    End Function 'suLoginUser

    Public Function suUserLoggedIn() As Boolean
        If HttpContext.Current.Session("user.Status") >= LoginStates.Online Then
            Return True
        Else
            Return False
        End If
    End Function

    'This function called on page.load for pages that are access controlled.
    'Redirects non-logged-in users to access denied page.
    'Call via:
    '   suLoginRequired(Me)
    Public Sub suLoginRequired(ByRef thePage As System.Web.UI.Page)
        If sysUser.suUserLoggedIn = False Then
            If thePage.IsCallback Then
                'Can not server transfer on a callback
                thePage.Response.Redirect("~/pUser/userLoginRequired.aspx?Next=" & thePage.Server.UrlEncode(thePage.Request.Url.ToString))
            Else
                thePage.Server.Transfer("~/pUser/userLoginRequired.aspx?Next=" & thePage.Server.UrlEncode(thePage.Request.Url.ToString))
            End If

            'Causes infinite loop! --> thePage.Server.Transfer(thePage.Request.ServerVariables("URL") & "?Code=" & sysErrors.USER_LOGINREQUIRED)
        End If
    End Sub













    'Returns the name of the current user
    Public Function suGetUsername() As String
        If HttpContext.Current Is Nothing Then
            Return SYSTEM_USERNAME
        Else
            Return HttpContext.Current.Session("user.Name")
        End If
    End Function

    'Returns the name of the specified user
    Public Function suGetUsername(ByVal UserID As Int32) As String
        Dim dt As New DataTable
        dt = SelectRows(dt, "SELECT [UserName] FROM [FriedParts].[dbo].[user-Accounts] WHERE [UserID] = " & UserID)
        If dt.Rows.Count = 1 Then
            Return dt.Rows(0).Field(Of String)("UserName")
        Else
            Err.Raise(-2343, , "UserID not found!")
            Return "" 'For compiler
        End If
    End Function

    'Returns the first name of the current user
    Public Function suGetUserFirstName() As String
        If HttpContext.Current Is Nothing Then
            Return sysText.txtGetWord(SYSTEM_USERNAME, 1)
        Else
            Return sysText.txtGetWord(HttpContext.Current.Session("user.Name"), 1)
        End If

    End Function

    'Returns the first name of the specified user
    Public Function suGetUserFirstName(ByVal UserID As Int16) As String
        Return sysText.txtGetWord(suGetUsername(UserID), 1)
    End Function

    ''' <summary>
    ''' Returns the UserID of the currently logged in user SAFELY! This is important since the user
    ''' may not be logged in, or the function may be called by a server-side process during an
    ''' FriedParts-Update-Service worker thread.
    ''' </summary>
    ''' <returns>The UserID of the currently logged in user; Returns sysErrors.USER_NOTLOGGEDIN otherwise</returns>
    ''' <remarks></remarks>
    Public Function suGetUserID() As Int32
        Try
            If HttpContext.Current Is Nothing Then
                'Running as a server process -- no user involved.
                Return SYSTEM_USERID
            Else
                If CInt(HttpContext.Current.Session("user.UserID")) Then
                    'Valid user login
                    Return HttpContext.Current.Session("user.UserID")
                Else
                    'Value defined (not server process), but user not logged in
                    Return sysErrors.USER_NOTLOGGEDIN
                End If
            End If
        Catch ex As InvalidCastException
            'Most likely cause is that there is no httpContext.Current because
            'we are running as a server process
            Return sysErrors.USER_NOTLOGGEDIN
        End Try
    End Function
End Module
