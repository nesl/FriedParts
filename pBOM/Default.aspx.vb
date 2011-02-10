Imports Microsoft.VisualBasic
Imports System
Imports System.Web.UI
Imports DevExpress.Web.ASPxUploadControl
Imports DevExpress.Web.ASPxClasses.Internal
Imports System.IO

Partial Class pBOM_Default
    Inherits System.Web.UI.Page

    Private Const UploadDirectory As String = "~/UploadControl/UploadImages/"
    Private Const ThumbnailSize As Integer = 100

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        If IsPostBack Then
            UploadControl.ShowProgressPanel = chbShowProgressPanel.Checked
        End If
    End Sub

    Protected Sub UploadControl_FileUploadComplete(ByVal sender As Object, ByVal e As FileUploadCompleteEventArgs)
        Try
            e.CallbackData = SavePostedFiles(e.UploadedFile)
        Catch ex As Exception
            e.IsValid = False
            e.ErrorText = ex.Message
        End Try
    End Sub

    Private Function SavePostedFiles(ByVal uploadedFile As UploadedFile) As String
        If (Not uploadedFile.IsValid) Then
            Return String.Empty
        End If

        Dim fileInfo As New FileInfo(uploadedFile.FileName)
        Dim resFileName As String = MapPath(UploadDirectory) + fileInfo.Name
        uploadedFile.SaveAs(resFileName)

        Dim fileLabel As String = fileInfo.Name
        Dim fileType As String = uploadedFile.PostedFile.ContentType.ToString()
        Dim fileLength As String = uploadedFile.PostedFile.ContentLength / 1024 & "K"
        Return String.Format("{0} <i>({1})</i> {2}|{3}", fileLabel, fileType, fileLength, fileInfo.Name)
    End Function

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
    End Sub
End Class