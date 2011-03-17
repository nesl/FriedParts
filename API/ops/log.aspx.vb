Imports System.Data

''' <summary>
''' Part of the API. Returns a list of PartTypes (categories).
''' level = 0..7 ; The category level to list.
''' </summary>
''' <remarks></remarks>
Partial Class API_PartTypes
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '[Define Commands]
        Dim xC() As String = {"get", "ack", "set", "reset"}
        Dim Cmds As New apiCommands(xC)

        '[Validate API Parameters]
        Dim TheCmd As String = apiValidateParameters(Me, Cmds)

        '[Capture Query String Parameters]
        Dim Queries As System.Collections.Specialized.NameValueCollection = Me.Request.QueryString()

        '[Initialize Output Stream]
        Dim Out As String

        '[Execute The Command]
        Select Case TheCmd
            Case "get"
                'Header
                Out = "<events>" & "</events>" & vbCrLf
                'Footer
                Out += "<tid>" & "</tid>" & vbCrLf
                Out += Queries("key") & vbCrLf
                Out += Queries("me") & vbCrLf
            Case "ack"
            Case "set"
            Case "reset"
            Case Else
                Throw New Exception("PANIC!!! THIS SHOULD NOT BE POSSIBLE!!!")
        End Select

        '[Generate Output Stream]
        PageContent.Text = Out
    End Sub

End Class
