Imports System.Data

''' <summary>
''' Part of the API. Returns a list of PartTypes (categories).
''' level = 0..7 ; The category level to list.
''' </summary>
''' <remarks></remarks>
Partial Class API_PartTypes
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim passedIN As System.Collections.Specialized.NameValueCollection = HttpContext.Current.Request.QueryString()
        If Not passedIN("level") Is Nothing Then
            'Process the API command
            Output.InnerHtml = ""

        End If
    End Sub
End Class
