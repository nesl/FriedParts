
Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        logUpdateUserStats()

        fpSQLNews.ConnectionString = cnxStr
        fpSQLNews.SelectCommand = "SELECT * FROM [FriedParts].[dbo].[msg-News] ORDER BY [FriedParts].[dbo].[msg-News].[newsDate] DESC"
        fpSQLNews.DataBind()

        Dim str As String = _
            "SELECT [Date], [Message] FROM [FriedParts].[dbo].[log-User] " & _
            "ORDER BY [Date] DESC"
        Dim dt As New System.Data.DataTable
        xGridUserLog.DataSource = dbAcc.SelectRows(dt, str)
        xGridUserLog.DataBind()
    End Sub
End Class
