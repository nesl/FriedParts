Imports Microsoft.VisualBasic
Imports System.Data

'Database Maintainence, Recovery, and Data Integrity Tools
Public Module dbUtils

    ''' <summary>
    ''' Deletes orphaned Distributor institution records. Misalignment between [dist-Names] and [dist-Common]
    ''' </summary>
    ''' <param name="DELETE">Deletes if true; Just counts the records scheduled for deletion if false.</param>
    ''' <returns>The number of records scheduled for deletion if DELETE=false, sysErrors.NO_ERROR otherwise.</returns>
    ''' <remarks>Delete the distributor if it has no name.</remarks>
    Public Function dbOrphDistNames(Optional ByVal DELETE As Boolean = False) As Int16
        Dim SqlText1 As String = _
            "FROM [FriedParts].[dbo].[dist-Common] " & _
            "WHERE [DistID] NOT IN (SELECT [DistID] FROM [FriedParts].[dbo].[dist-Names])"
        Dim SqlText2 As String = _
            "FROM [FriedParts].[dbo].[dist-Names] " & _
            "WHERE [DistID] NOT IN (SELECT [DistID] FROM [FriedParts].[dbo].[dist-Common])"
        If DELETE Then
            SqlText1 = "DELETE " & SqlText1
            dbAcc.SQLexe(SqlText1)
            SqlText2 = "DELETE " & SqlText2
            dbAcc.SQLexe(SqlText2)
            Return sysErrors.NO_ERROR
        Else
            SqlText1 = "SELECT * " & SqlText1
            Dim dt As New DataTable
            dt = dbAcc.SelectRows(dt, SqlText1)
            Dim NumRecordsEffected As UInt16 = dt.Rows.Count
            dt.Clear()
            SqlText2 = "SELECT * " & SqlText2
            dt = dbAcc.SelectRows(dt, SqlText2)
            NumRecordsEffected += dt.Rows.Count
            Return NumRecordsEffected
        End If
    End Function

    ''' <summary>
    ''' Deletes orphaned Manufacturer institution records. Misalignment between [mfr-Names] and [mfr-Common]
    ''' </summary>
    ''' <param name="DELETE">Deletes if true; Just counts the records scheduled for deletion if false.</param>
    ''' <returns>The number of records scheduled for deletion if DELETE=false, sysErrors.NO_ERROR otherwise.</returns>
    ''' <remarks>Delete the name if it has no distributor.</remarks>
    Public Function dbOrphMfrNames(Optional ByVal DELETE As Boolean = False) As Int16
        Dim SqlText1 As String = _
            "FROM [FriedParts].[dbo].[mfr-Common] " & _
            "WHERE [mfrID] NOT IN (SELECT [mfrID] FROM [FriedParts].[dbo].[mfr-Names])"
        Dim SqlText2 As String = _
            "FROM [FriedParts].[dbo].[mfr-Names] " & _
            "WHERE [mfrID] NOT IN (SELECT [mfrID] FROM [FriedParts].[dbo].[mfr-Common])"
        If DELETE Then
            SqlText1 = "DELETE " & SqlText1
            dbAcc.SQLexe(SqlText1)
            SqlText2 = "DELETE " & SqlText2
            dbAcc.SQLexe(SqlText2)
            Return sysErrors.NO_ERROR
        Else
            SqlText1 = "SELECT * " & SqlText1
            Dim dt As New DataTable
            dt = dbAcc.SelectRows(dt, SqlText1)
            Dim NumRecordsEffected As UInt16 = dt.Rows.Count
            dt.Clear()
            SqlText2 = "SELECT * " & SqlText2
            dt = dbAcc.SelectRows(dt, SqlText2)
            NumRecordsEffected += dt.Rows.Count
            Return NumRecordsEffected
        End If
    End Function

    'Detect and Delete orphaned records in the distributor, CADD, and Inventory tables
    'The most common cause is from the addition of "test" parts or during errors while adding a part
    Public Function dbOrphDistParts(Optional ByVal DELETE As Boolean = False) As Int16
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
    Public Function dbOrphCount(Optional ByRef Reset As Boolean = False, Optional ByRef Additional As Int32 = 0) As Int16
        Static iCounter As Int32 = 0
        If Reset Then iCounter = 0
        iCounter += Additional
        Return iCounter
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
