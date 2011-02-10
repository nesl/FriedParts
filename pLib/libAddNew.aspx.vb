Imports System.Diagnostics

Partial Class CODElibAddNew
    Inherits System.Web.UI.Page

    Protected Sub btnUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpload.Click
        If (FileUpload.HasFile) Then
            'Check if existing file already exists on the server, if so prompt overwrite -- Dangerous -- maybe rename a backup copy?
            'fileupload.filename()
            'check if filename is in proper format, year_username
            'check for correct file extension
            'write to disk!
            FileUpload.SaveAs(sysLIBALTIUMROOT & "filename.txt")
        Else
            'No file selected or file not found
            '...just do nothing -- they'll figure it out!
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Access control
        sysUser.suLoginRequired(Me)

        'Lib Sch
        xGridNewSch.DataSource = fplibReportNew(fpLibMgmt.fplibLibTypePath(fplibTypes.SchLib))
        xGridNewSch.DataBind()
        xGridNewSch.KeyFieldName = "UID"

        'Lib PCB
        xGridNewPCB.DataSource = fplibReportNew(fpLibMgmt.fplibLibTypePath(fplibTypes.PcbLib))
        xGridNewPCB.DataBind()
    End Sub
End Class
