Imports Microsoft.VisualBasic
Imports System.Data

'Database Maintainence, Recovery, and Data Integrity Tools
Public Module dbUtils

    'Detect and Delete orphaned records in the distributor, CADD, and Inventory tables
    'The most common cause is from the addition of "test" parts or during errors while adding a part
    Public Function dbOrphDist(Optional ByVal DELETE As Boolean = False) As Int16
        Dim SqlText As String = _
            "FROM [FriedParts].[dbo].[dist-Parts] " & _
            "WHERE [PartID] NOT IN (SELECT [PartID] FROM [FriedParts].[dbo].[part-Common])"
        If DELETE Then
            SqlText = "DELETE " & SqlText
            dbAcc.SQLexe(SqlText)
            Return sysErrors.NO_ERROR
        Else
            SqlText = "SELECT * " & SqlText
            Dim dt As New DataTable
            dt = dbAcc.SelectRows(dt, SqlText)
            Return dt.Rows.Count
        End If
    End Function
    Public Function dbOrphCADD(Optional ByVal DELETE As Boolean = False) As Int16
        Dim SqlText As String = _
            "FROM [FriedParts].[dbo].[cad-Altium] " & _
            "WHERE [PartID] NOT IN (SELECT [PartID] FROM [FriedParts].[dbo].[part-Common])"
        If DELETE Then
            SqlText = "DELETE " & SqlText
            dbAcc.SQLexe(SqlText)
            Return sysErrors.NO_ERROR
        Else
            SqlText = "SELECT * " & SqlText
            Dim dt As New DataTable
            dt = dbAcc.SelectRows(dt, SqlText)
            Return dt.Rows.Count
        End If
    End Function
    Public Function dbOrphBins(Optional ByVal DELETE As Boolean = False) As Int16
        Dim SqlText As String = _
            "FROM [FriedParts].[dbo].[inv-Bins] " & _
            "WHERE [PartID] NOT IN (SELECT [PartID] FROM [FriedParts].[dbo].[part-Common])"
        If DELETE Then
            SqlText = "DELETE " & SqlText
            dbAcc.SQLexe(SqlText)
            Return sysErrors.NO_ERROR
        Else
            SqlText = "SELECT * " & SqlText
            Dim dt As New DataTable
            dt = dbAcc.SelectRows(dt, SqlText)
            Return dt.Rows.Count
        End If
    End Function
    'Returns a complete count of orphaned records
    Public Function dbOrphCount() As Int16
        Return dbOrphBins() + dbOrphCADD() + dbOrphDist()
    End Function

    'Appends append_table to the end of orig_table. 
    '   * Only records that are unique are added. 
    '   * Uniqueness is determined by the KeyFieldName -- which must be the name of an ID field in the table
    '   * Both tables must have the same schema.
    '
    'FIX THIS IMPLEMENTATION -- for some reason there appears to be some bug in VB.NET that 
    'causes a Collection modified -ish exception to be thrown, when it shouldn't so I had to 
    'rewrite it to this cluster-fucky code
    Public Function mergeTables(ByRef orig_table As DataTable, ByRef append_table As DataTable, ByRef KeyFieldName As String) As DataTable
        'VAR
        Dim r As DataRow
        Dim blah As DataTable
        Dim selectedRows As DataRow()

        'INIT
        blah = orig_table.Copy

        'COPY OLD TABLE
        'For Each r In orig_table.Rows
        ' blah.ImportRow(r)
        ' Next

        'MERGE NEW TABLE
        For Each r In append_table.Rows
            selectedRows = blah.Select(KeyFieldName & " = " & r.Field(Of Int32)(KeyFieldName))
            If selectedRows.Length = 0 Then
                'add this row only if we don't already have it in the array
                blah.ImportRow(r)
            End If
        Next
        Return blah
    End Function
End Module
