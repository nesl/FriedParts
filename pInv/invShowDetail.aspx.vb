Imports System.Data

Partial Class pInv_invShowDetail
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim passedIN As System.Collections.Specialized.NameValueCollection = HttpContext.Current.Request.QueryString()

        If Not (Page.IsCallback Or Page.IsPostBack) AndAlso Not passedIN("detail") Is Nothing AndAlso passedIN("detail").Length > 0 Then

            Dim SqlQuery As String = "SELECT * FROM [FriedParts].[dbo].[view-part] WHERE [PartID] = " & passedIN("detail")
            Dim DT As DataTable = New DataTable
            Dim DR As DataRow
            DT = dbAcc.SelectRows(DT, cnxStr, SqlQuery)
            DR = DT.Rows(0)

            'TITLES
            TitleMfr.InnerText = DR.Field(Of String)("mfrName")
            TitlePart.InnerText = DR.Field(Of String)("mfrPartNum")

            'IMAGE
            'Enables dynamic image resizing which is really important, but I can't seem to get working right now
            'Page.RegisterStartupScript("ResizeImage", _
            '    "<script type=""text/javascript"">" & _
            '    "       function ResizeImage(image, maxwidth, maxheight){" & _
            '    "            {w = image.width;h = image.height;" & _
            '    "               if( w == 0 || h == 0 ){image.width = maxwidth;image.height = maxheight;}" & _
            '    "               ElseIf (w > h) Then { if (w > maxwidth) image.width = maxwidth;}" & _
            '    "            {Else {if (h > maxheight) image.height = maxheight;}" & _
            '    "        } </script>")

            If DR.Field(Of String)("URL_Image") Is DBNull.Value Or Len(DR.Field(Of String)("URL_Image")) = 0 Then
                ImagePart.Src = sysEnv.sysNoPartPhoto
            Else
                ImagePart.Src = DR.Field(Of String)("URL_Image")
            End If

            'SPECIFICATION
            If DR.Field(Of String)("URL_Datasheet") Is DBNull.Value Or Len(DR.Field(Of String)("URL_Datasheet")) = 0 Then
                sDatasheet.InnerText = "No Datasheet On File!"
            Else
                sDatasheet.HRef = DR.Field(Of String)("URL_Datasheet")
            End If
            sValue.InnerText = "Value: " & DR.Field(Of String)("Value") & " " & DR.Field(Of String)("Value_Units") & " " & DR.Field(Of String)("Value_Tolerance")
            sTemp.InnerText = "Operating (SOR): " & DR.Field(Of String)("Temp_Min") & Chr(176) & "C Min to " & DR.Field(Of String)("Temp_Max") & Chr(176) & "C Max"

            'DESCRIPTIONS
            TextBoxDesc.Text = DR.Field(Of String)("Description")
            TextBoxXDesc.Text = DR.Field(Of String)("Extra_Description")

            'LOCAL INVENTORY
            fpSQLData.ConnectionString = dbAcc.cnxStr
            fpSQLData.SelectCommand = "SELECT * FROM [FriedParts].[dbo].[view-inv] WHERE [FriedParts].[dbo].[view-inv].[Local] = " & sysEnv.sqlTRUE & " AND [FriedParts].[dbo].[view-inv].[PartID] = " & passedIN("detail")
            fpSQLData.DataBind()
            xGVLocal.DataSource = fpSQLData
            xGVLocal.KeyFieldName = "PartID" 'Set Primary Key for row selection
            xGVLocal.DataBind()
            linkAssignBins.NavigateUrl = "invAssignBins.aspx?FPID=" & Server.UrlEncode(passedIN("detail"))

            'DISTRIBUTOR INVENTORY
            Dim SqlQuery2 As String = "SELECT DISTINCT [distName] AS [Distributor], [distWebsite] AS Website, [DistPartNum] AS [Part Number] FROM [FriedParts].[dbo].[view-dist] INNER JOIN [FriedParts].[dbo].[dist-Parts] ON [view-dist].[DistID] = [dist-Parts].[DistID] WHERE [dist-Parts].[PartID] = " & passedIN("detail")
            Dim DT2 As DataTable = New DataTable
            DT2 = dbAcc.SelectRows(DT2, cnxStr, SqlQuery2)
            xGVDist.DataSource = DT2
            xGVDist.KeyFieldName = "Part Number" 'Key off the Dist Part Number in case a distributor has multiple part numbers for the same Fried Part (can happen if part numbers at the distributor change -- we store both for reference and to not break old designs)
            xGVDist.DataBind()

            'CADD MODELS
            xGVAltium.DataSource = fpAltium.fpaltSummaryTable(passedIN("detail"))
            xGVAltium.DataBind()
        End If
    End Sub
End Class
