Imports System.Data

Partial Class pAdmin_dbUtils
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsCallback And Not IsPostBack Then dbUtils.dbOrphCount(True)
        'Orphaned Records
        orphrecsLit.Text = "<table>" & _
                            "<tr><td class=""fpTableLabel"">FOUND ORPHANS:</td><td></td></tr>" & _
                            AddStat("Distributor Part Records", dbOrphDistParts) & _
                            AddStat("Distributor Name Records", dbOrphDistNames) & _
                            AddStat("Manufacturer Name Records", dbOrphMfrNames) & _
                            AddStat("CADD Entries", dbOrphCADD) & _
                            AddStat("Inventory Records", dbOrphBins) & _
                            "<tr><td><hr></td><td><hr></td></tr>" & _
                            AddStat("TOTAL EFFECTED", dbUtils.dbOrphCount()) & _
                            "</table>"
    End Sub

    ''' <summary>
    ''' Returns the HTML for a table row
    ''' </summary>
    ''' <param name="Label"></param>
    ''' <param name="Statistic"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function AddStat(ByRef Label As String, ByRef Statistic As Int32) As String
        Dim htmlOut As String = "<tr><td class=""fpTableLabel"">" & Label & ":</td>" & _
                                "<td>" & Statistic & "</td></tr>"
        dbUtils.dbOrphCount(, Statistic)
        Return htmlOut
    End Function

    Protected Sub orphrecsBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles orphrecsBtn.Click
        Dim count As Int16 = dbOrphCount()
        dbOrphBins(True)
        dbOrphCADD(True)
        dbOrphDistNames(True)
        dbOrphDistParts(True)
        dbOrphMfrNames(True)
        MsgBox(Me, sysErrors.DBUTILS_ORPHANDONE, count)
    End Sub

    Protected Sub btnCreateCadAltiumLibTable_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateCadAltiumLibTable.Click
        'Get the Dropbox Records
        Dim str As String = _
            "SELECT [FileID]" & _
            "      ,[DropboxPath]" & _
            "      ,[Name]" & _
            "      ,[OwnerID]" & _
            "      ,[LocalTimeStamp]" & _
            "  FROM [FriedParts].[dbo].[files-Common]"
        Dim dt As New DataTable
        SelectRows(dt, str)

        'Creates the cad-AltiumLib table
        For Each dr As DataRow In dt.Rows
            If dr.Field(Of String)("Name").CompareTo("FriedParts.DbLib") <> 0 Then
                str = _
                    "INSERT INTO [FriedParts].[dbo].[cad-AltiumLib]" & _
                    "           ([LibID]" & _
                    "           ,[LibName]" & _
                    "           ,[LibType])" & _
                    "     VALUES (" & _
                    "            " & dr.Field(Of Int32)("FileID") & "," & _
                    "           '" & fplibMakeLibName(dr.Field(Of String)("Name")) & "'," & _
                    "           '" & fplibLibTypeNum(dr.Field(Of String)("Name")) & "'" & _
                    "           )"
                Try
                    SQLexe(str)
                Catch ex As SqlClient.SqlException
                    'Means that the record is already in the table, just ignore the duplicate
                End Try
            End If
        Next
    End Sub
End Class
