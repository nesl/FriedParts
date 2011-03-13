Imports Microsoft.VisualBasic
Imports System.Data

'Implements Searching Algorithms
Namespace fpSearch
    Public Module searchStatic
        Private Const sqlCommonSelect As String = _
            "USE [FriedParts] " & _
            "SELECT * " & _
            "FROM [view-part] "


        ''Returns the SQL statement for the part search in response to the SearchCommand
        ''MAINTAINED FOR LEGACY -- so current sys doesn't break while developing new search algorithm
        'Public Function PartSearch(ByVal SearchCommand As String) As String
        '    If Len(SearchCommand) = 0 Then
        '        'BROWSE MODE (no search terms passed in, so show all)
        '        Return sqlCommonSelect
        '    Else
        '        'SEARCH MODE (a search was passed to us)
        '        fpSearchLog(SearchCommand, HttpContext.Current.Session("user.UserID")) 'Log it -- for analysis and debugging
        '        Return fpSearchSqlGeneric(SearchCommand)
        '    End If 
        'End Function

        'Returns the SQL statement for the part search in response to the SearchCommand
        'THIS IS A GENERIC FUZZY SEARCH ON TEXT-LIKE FIELDS (no business logic)
        Private Function fpSearchSqlGeneric(ByVal SearchCommand As String) As String
            Dim BLAH As String = SearchCommand
            Dim sqlSearchQuery As String
            sqlSearchQuery = sqlCommonSelect & _
                "   WHERE " & _
                "[mfrName] LIKE '%" & BLAH & "%' OR " & _
                "[Description] LIKE '%" & BLAH & "%' OR " & _
                "[PartID] LIKE '%" & BLAH & "%' OR " & _
                "[Extra_Description] LIKE '%" & BLAH & "%' OR " & _
                "[mfrPartNum] LIKE '%" & BLAH & "%' OR " & _
                "[Value] LIKE '%" & BLAH & "%' "
            Return sqlSearchQuery
        End Function

        'Returns the SQL statement for the specific partID search
        'Takes in a datatable with a list of "PartID" values and returns the SQL string
        '   to collect those PartID records from the generic schema
        'Allows searching of sub-tables (ex. [dist-Parts]) without JOINING during search
        'Schema loading required to do table merge -- so this bridges the gap! yay!
        Private Function fpSearchSqlSpecific(ByVal PartID_List As DataTable) As String
            Dim sqlSearchQuery As String = ""
            Dim r As DataRow
            For Each r In PartID_List.Rows
                If sqlSearchQuery.Length = 0 Then
                    sqlSearchQuery = sqlSearchQuery & _
                    "[PartID] = " & r.Field(Of Integer)("PartID")
                Else
                    sqlSearchQuery = sqlSearchQuery & _
                    " OR [PartID] = " & r.Field(Of Integer)("PartID")
                End If
            Next
            Return sqlCommonSelect & " WHERE " & sqlSearchQuery
        End Function

        Private Function fpSearchInvBins(ByVal SearchCommand As String) As DataTable
            Dim dt As New DataTable
            Dim sqlSearchQuery As String = _
                "SELECT [PartID]" & _
                "      ,[BinLocation]" & _
                "  FROM [FriedParts].[dbo].[inv-Bins] " & _
                " WHERE [BinLocation] LIKE '%" & SearchCommand & "%'"
            dt = SelectRows(dt, sqlSearchQuery)
            If dt.Rows.Count > 0 Then
                'Found a match or 2...
                Dim append_table As New DataTable
                append_table = SelectRows(append_table, fpSearchSqlSpecific(dt))
                Return append_table
            End If
            Return Nothing
        End Function

        Private Function fpSearchDistPartNum(ByVal SearchCommand As String) As DataTable
            Dim dt As New DataTable
            Dim sqlSearchQuery As String = _
                "SELECT [PartID]" & _
                "      ,[DistID]" & _
                "      ,[DistPartNum]" & _
                "  FROM [FriedParts].[dbo].[dist-Parts] " & _
                " WHERE [DistPartNum] LIKE '%" & SearchCommand & "%'"
            dt = SelectRows(dt, sqlSearchQuery)
            If dt.Rows.Count > 0 Then
                'Found a match or 2...
                Dim append_table As New DataTable
                append_table = SelectRows(append_table, fpSearchSqlSpecific(dt))
                Return append_table
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' THIS IS THE PRIMARY API INTERFACE TO SEARCH -- returns a usable sorted (by FPID) dataset
        ''' </summary>
        ''' <param name="SearchCommand"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function fpSearchDataSource(ByVal SearchCommand As String) As DataTable
            'Generic Fuzzy Search (over common part data)
            Dim dt As New DataTable
            Dim dtMerge As DataTable
            dt = SelectRows(dt, fpSearchSqlGeneric(SearchCommand))

            'Distributor Part Number Search
            dtMerge = New DataTable
            dtMerge = fpSearchDistPartNum(SearchCommand)
            If Not dtMerge Is Nothing Then
                'Merge!
                dt = mergeTables(dt, dtMerge, "PartID")
            End If

            'Bin Location Search
            dtMerge = New DataTable
            dtMerge = fpSearchInvBins(SearchCommand)
            If Not dtMerge Is Nothing Then
                'Merge!
                dt = mergeTables(dt, dtMerge, "PartID")
            End If

            'Footprint/Symbol Search

            'Sort (by FP Part# newest to oldest)
            dt.DefaultView.Sort = "PartID DESC"

            'Log
            If HttpContext.Current.Session("user.UserID") Is Nothing Or Not IsNumeric(HttpContext.Current.Session("user.UserID")) Then
                fpSearchLog(SearchCommand, -1) 'Log it -- for analysis and debugging
            Else
                fpSearchLog(SearchCommand, HttpContext.Current.Session("user.UserID")) 'Log it -- for analysis and debugging
            End If
            'Return. Yay!
            Return dt
        End Function

        Public Sub fpSearchLog(ByRef theSearchText As String, ByRef UserID As Integer)
            If Not theSearchText Is Nothing AndAlso theSearchText.Length > 0 Then
                Dim theText As String
                'If the search command is huge, then lets only log the first part of it!
                If theSearchText.Length > 255 Then
                    theText = theSearchText.Substring(0, 255)
                Else
                    theText = theSearchText
                End If
                'Now let's do it!
                Dim sqlTxt As String = _
                    "INSERT INTO [FriedParts].[dbo].[log-Search]" & _
                    "           (" & _
                    "           [Date]" & _
                    "           ,[UserID]" & _
                    "           ,[SearchText])" & _
                    "     VALUES (" & _
                    "           " & txtSqlDate(Now) & "," & _
                    "           " & UserID & "," & _
                    "           '" & theText & "'" & _
                    "           )"
                dbAcc.SQLexe(sqlTxt)
            End If
        End Sub
    End Module
End Namespace