Imports System.Data

Partial Class pDevel_devClick
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        devTestsysTextModule()
    End Sub

    Private Sub devTestsysTextModule()
        Label0a.Text = "Input String"
        Label0b.Text = "Delimiter"
        Label1b.Text = txtNumOfSubstrings(txtInput0a.Text, txtInput0b.Text)
        Label2b.Text = txtNumWords(txtInput0a.Text, txtInput0b.Text)
        Dim wordnum As Byte = 3
        Label3.Text = "Get Word Number " & wordnum
        Label3b.Text = txtGetWord(txtInput0a.Text, wordnum, txtInput0b.Text)
    End Sub

    Private Sub devTimespan()
        Dim W As Date
        Dim TS As New TimeSpan(1, 0, 0, 0)
        W = Now
        Label1.Text = W.Date.Month & "/" & W.Date.Day & "/" & W.Date.Year
        W = W + TS
        Label1.Text = Label1.Text & " ||| " & W.Date.Month & "/" & W.Date.Day & "/" & W.Date.Year
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dt As DataTable = New fpProj.fpBOM(8).GetDataSource
        xGrid.DataSource = dt
    End Sub
End Class
