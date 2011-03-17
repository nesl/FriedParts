Imports Microsoft.VisualBasic
'Imports DevExpress.Web.ASPxPopupControl
Imports System.Diagnostics
Imports System.Web

'Global Variable Container
Public Module sysError
    Public Enum sysErrors As Integer
        DROPBOX_LINK_SUCCESS = 60
        PROJECTADD_SUCCESS = 50
        PARTADD_SUCCESS = 10
        PARTTYPEADD_SUCCESS = 11
        MFRADD_SUCCESS = 18
        MISC_INFO = 20
        USERADD_SUCCESS = 30
        BINADD_SUCCESS = 35
        BINASSIGN_SUCCESS = 36
        DBUTILS_ORPHANDONE = 40
        NO_ERROR = 0
        ERR_NOTFOUND = -1
        ERR_NOTUNIQUE = -2
        ERR_ALREADYINUSE = -3
        LIBADD_FILENOTFOUND = -30
        BINADD_ALREADYEXISTS = -35
        USERADD_NOTREALNAME = -40
        USERADD_CREATEFAILED = -41
        USERADD_NOINTEREST = -42
        USERADD_NOAUTHCOMPLETE = -43
        USERADD_ALREADYIN = -44
        USERADD_NOSHARE = -45
        PARTADD_NOMFR = -46
        PARTADD_NODIST = -47
        PARTADD_CREATEFAILED = -48
        PARTADD_MFRNUMNOTUNIQUE = -49
        PARTADD_NOSHORTDESC = -50
        PARTADD_NOLONGDESC = -51
        PARTADD_NOIMAGE = -52
        PARTADD_NODATASHEET = -53
        PARTADD_ALREADYEXISTS = -54
        USER_LOGINREQUIRED = -60
        USER_NOTLOGGEDIN = -61
        MISC_ERROR = -99
    End Enum

    Public Function errDecodeMsg(ByVal errCode As sysErrors, Optional ByVal Param1 As String = "", Optional ByVal Param2 As String = "") As String
        Select Case errCode
            Case sysErrors.PARTTYPEADD_SUCCESS
                Return "Part Type " & Param1 & ": " & Param2 & ", added successfully!"
            Case sysErrors.DROPBOX_LINK_SUCCESS
                Return Param1 & ", you have successfully linked your Dropbox account (" & Param2 & ") to FriedParts. All kinds of awesomeness will now ensue!"
            Case sysErrors.PROJECTADD_SUCCESS
                Return "Project " & Param1 & ", " & Param2 & ", added successfully! You can now generate reports for this project."
            Case sysErrors.MFRADD_SUCCESS
                Return "Manufacturer " & Param1 & " added successfully!"
            Case sysErrors.USERADD_SUCCESS
                Return "Account created successfully! Welcome to FriedParts! Please behave. We are watching you with logs."
            Case sysErrors.BINADD_SUCCESS
                Return "Bin box " & Param1 & " in warehouse " & Param2 & " created successfully!"
            Case sysErrors.BINASSIGN_SUCCESS
                Return "Part #" & Param1 & " (" & partGetShortName(Param1) & ") successfully assigned to bin location " & Param2 & "!"
            Case sysErrors.BINADD_ALREADYEXISTS
                Return "Bin box " & Param1 & " in warehouse " & Param2 & " already exists. Bin boxes must be unique of course."
            Case sysErrors.LIBADD_FILENOTFOUND
                Return "The specified file was not found on the server! Are you sure that you uploaded or copied it over?"
            Case sysErrors.USERADD_NOTREALNAME
                Return "You did not enter your real name! You must enter your real name to be allowed access to the system. Among other things, your real name is used to create your login identity. You must enter at least your first and last name (separated by a space)."
            Case sysErrors.USERADD_CREATEFAILED
                Return "User not created successfully! this should NOT happen!"
            Case sysErrors.USERADD_NOINTEREST
                Return "You should not have been directed to this page. It appears that you are not currently attempting to login. Come back when you are."
            Case sysErrors.USERADD_NOAUTHCOMPLETE
                Return "I have no idea what you are doing on this page at this time. You are attempting to login, but you have not completed authentication yet!"
            Case sysErrors.USERADD_ALREADYIN
                Return "You are already logged in! What are you doing here?!"
            Case sysErrors.USERADD_NOSHARE
                Return "We were unable to retrieve a valid e-mail address from your OpenID provider. You can not create an account on FriedParts until you do. The most common cause is that you refused to share this information with us."
            Case sysErrors.PARTADD_SUCCESS
                Return "Part " & Param1 & ", " & Param2 & ", added successfully!"
            Case sysErrors.PARTADD_NODIST
                Return "Distributor ID " & Param1 & " is invalid. This should not happen. Database corruption is likely!"
            Case sysErrors.PARTADD_NOMFR
                Return "Manufacturer ID " & Param1 & " is invalid. This should not happen. Database corruption is likely!"
            Case sysErrors.PARTADD_CREATEFAILED
                Return "Attempted to create the new part, but the database did not assign a new PartID. At least only one table may have been messed up (part-Common) at this point. Sorry!"
            Case sysErrors.PARTADD_MFRNUMNOTUNIQUE
                Return "The manufacturer part number queried returned multiple results. This should not happen and represents a corrput state of the of the [part-Common] table. Please report this to an administrator."
            Case sysErrors.PARTADD_ALREADYEXISTS
                Return "The part: " & Param1 & ", ALREADY EXISTS in the database. Duplicate entries are not allowed."
            Case sysErrors.MISC_ERROR
                Return Param1 & " " & Param2
            Case sysErrors.MISC_INFO
                Return Param1 & " " & Param2
            Case sysErrors.DBUTILS_ORPHANDONE
                Return "Removed " & Param1 & " orphaned records!"
            Case sysErrors.USER_LOGINREQUIRED
                Return "LOGIN REQUIRED. If you were already logged in, your session has expired and you will need to login again. Sorry."
            Case Else
                Return "An UNKNOWN ERROR has occured!"
        End Select

    End Function

    Public Function errDecodeTitle(ByVal errCode As sysErrors) As String
        Select Case errCode
            Case sysErrors.LIBADD_FILENOTFOUND
                Return "File Not Found on Server"
            Case sysErrors.USER_LOGINREQUIRED
                Return "ACCESS DENIED"
            Case Else
                If errCode > 0 Then Return "INFORMATION"
                If errCode < 0 Then Return "ERROR!"
                Return "UNKNOWN ERROR!"
        End Select
    End Function

    'DISPLAY AN ERROR or MESSAGE BOX! Yay!
    Public Sub MsgBox(ByRef xMsgBox As DevExpress.Web.ASPxPopupControl.ASPxPopupControl, ByVal Code As Int16, Optional ByVal Param1 As String = "", Optional ByVal Param2 As String = "")
        'Determine Content
        xMsgBox.ShowOnPageLoad = True
        DirectCast(xMsgBox.FindControl("xMessage"), System.Web.UI.WebControls.Label).Text = sysError.errDecodeMsg(Code, Param1, Param2)
        xMsgBox.HeaderText = sysError.errDecodeTitle(Code)
        'Determine Type
        If Code > 0 Then
            'Information for user
            xMsgBox.BackColor = Drawing.Color.AntiqueWhite
            xMsgBox.FindControl("errImage").Visible = False
            xMsgBox.FindControl("okImage").Visible = True
        ElseIf Code < 0 Then
            'Error notification for user
            xMsgBox.FindControl("errImage").Visible = True
            xMsgBox.FindControl("okImage").Visible = False
            xMsgBox.BackColor = Drawing.Color.Yellow
        Else
            'Should not happen!!!
        End If

    End Sub

    'Show the message box by redirecting the URL request
    Public Sub MsgBox(ByRef Webpage As Page, ByVal Code As sysErrors, Optional ByVal Param1 As String = "", Optional ByVal Param2 As String = "")
        Webpage.Server.Transfer( _
            Webpage.Request.ServerVariables("URL") & _
            "?Code=" & Code & "&P1=" & Webpage.Server.UrlEncode(Param1) & "&P2=" & Webpage.Server.UrlEncode(Param2))
    End Sub

    'Show the message box by redirecting the URL request
    'Use this function when you want to move the use to a new page (specified by URL string)
    'Use this function to specify the "detail" parameter
    Public Sub MsgBox_Transfer(ByRef Me_Page As Page, ByVal URL As String, ByVal Code As sysErrors, Optional ByVal Param1 As String = "", Optional ByVal Param2 As String = "", Optional ByVal Detail As String = "")
        Me_Page.Server.Transfer( _
            URL & "?Code=" & Code & "&P1=" & Me_Page.Server.UrlEncode(Param1) & "&P2=" & Me_Page.Server.UrlEncode(Param2) & "&detail=" & Me_Page.Server.UrlEncode(Detail))
    End Sub
End Module
