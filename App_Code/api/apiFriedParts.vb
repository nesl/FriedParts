Imports Microsoft.VisualBasic
Imports System.Web.Routing
Imports System.Web

Public Module apiFriedParts
    ''' <summary>
    ''' Used to specify what commands are valid for different areas of the api
    ''' </summary>
    ''' <remarks></remarks>
    Public Class apiCommands
        Protected iCmds As Collection
        Public ReadOnly Property GetCommands As Collection
            Get
                Return iCmds
            End Get
        End Property
        Public Sub Add(ByVal CommandName As String)
            iCmds.Add(iCmds.Count, CommandName) 'Produces 0-indexed keys
        End Sub
        Public Sub Add(ByVal CommandNames() As String)
            For i As Byte = LBound(CommandNames) To UBound(CommandNames)
                iCmds.Add(iCmds.Count, CommandNames(i))
            Next
        End Sub
        Public Function IsValid(ByVal CommandName As String) As Boolean
            Return iCmds.Contains(CommandName)
        End Function
        Public Sub New()
            iCmds = New Collection
        End Sub
        Public Sub New(ByRef CommandNames() As String)
            iCmds = New Collection
            Add(CommandNames)
        End Sub
    End Class

    ''' <summary>
    ''' Return a 404 Not Found error.
    ''' Call this method when you want to not support a particular value the user sent us.
    ''' </summary>
    ''' <remarks>Could be upgraded in the future to provide better handling</remarks>
    Sub apiNotAllowed(ByVal MePage As Page, Optional ByRef Message As String = "")
        If Len(Message) > 0 Then
            MePage.Response.Redirect("/API/errors/NotAllowed?msg=" & HttpUtility.UrlEncode(Message))
        Else
            MePage.Response.Redirect("/API/errors/NotAllowed")
        End If
    End Sub

    ''' <summary>
    ''' Validates the client's API request
    ''' </summary>
    ''' <param name="MePage">The Me as System.Web.Page object</param>
    ''' <param name="ValidCommands"></param>
    ''' <returns>The name of the command the user requested</returns>
    ''' <remarks></remarks>
    Public Function apiValidateParameters(ByRef MePage As Page, ByRef ValidCommands As apiCommands) As String
        Dim Parameters As System.Web.Routing.RouteValueDictionary = MePage.RouteData.Values
        '[API Version]
        Try
            If CInt(Parameters("version")) > 1 Then
                apiNotAllowed(MePage, "Invalid Version")
            End If
        Catch ex As Exception
            apiNotAllowed(MePage)
        End Try
        '[Security]
        Try
            If Not CStr(MePage.Request.QueryString("key")).CompareTo(sysSecret.FriedPartsAppKey) = 0 Then
                apiNotAllowed(MePage, "Unauthorized")
            End If
        Catch ex As Exception
            apiNotAllowed(MePage, "Unauthorized")
        End Try
        '[The Specified Command]
        Try
            If Not ValidCommands.IsValid(CStr(Parameters("command"))) Then
                apiNotAllowed(MePage, "Command Not Valid")
            End If
        Catch ex As Exception
            apiNotAllowed(MePage, ex.Message)
        End Try
        '[Result]
        Return CStr(Parameters("command"))
    End Function


    Sub apiInit(ByVal routes As RouteCollection)
        routes.MapPageRoute("ops.log", "API/v{version}/ops/log/{command}", "~/API/ops/log.aspx")
    End Sub
End Module
