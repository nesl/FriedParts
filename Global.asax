<%@ Application Language="VB" %>

<script runat="server">

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application startup
        apiFriedParts.apiInit(System.Web.Routing.RouteTable.Routes)
    End Sub
    
    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub
        
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
    End Sub
    
    Sub Application_PostReleaseRequestState(ByVal sender As Object, ByVal e As EventArgs)
        If Response.ContentType = "text/html" Then
            Response.Filter = New sysReplaceHTML(Response.Filter)
        End If
    End Sub
    
    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a new session is started
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.
        Try
            Response.Redirect("http://friedparts.nesl.ucla.edu/friedparts/")
        Catch ex As HttpException
            'Response object not available in the current context
            'xxx -- implement better handling in the future?
        End Try
    End Sub
    
       
    ''' <summary>
    ''' Fires on each request serviced by the app
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Application_BeginRequest(ByVal sender As Object, ByVal e As System.EventArgs)
        sysLog.logRequest(Me, New sysLog.logMessageTypes(logMessageTypes.MsgTypes.InternetInformationServer), , "BegReq")
    End Sub

    ''' <summary>
    ''' One BeginRequest can spawn multiple EndRequests as the webserver tries to meet the original
    ''' user request. As in the user asks for a page, but we need to go get all the images and resources.    
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Disabled for now to reduce log footprint. We're really only interested in the attackers submissions anyway.</remarks>
    Protected Sub Application_EndRequest(ByVal sender As Object, ByVal e As System.EventArgs)
        'sysLog.logRequest(Me, "EndReq")
    End Sub

    ''' <summary>
    ''' Only fires in IIS7 (requires Integrated Pipeline Mode) -- but then you can 
    ''' probably access the response object earlier in EndRequest
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Does not work in IIS6</remarks>
    Protected Sub Application_PreSendRequestHeaders(ByVal sender As Object, ByVal e As System.EventArgs)
    End Sub
</script>