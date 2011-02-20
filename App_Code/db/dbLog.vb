Imports Microsoft.VisualBasic
Imports System.Data

'PROVIDES ACTIVITY LOGGING FUNCTIONALITY
'   WORKS AT DEVELOPER LEVEL (use dbAcc for lower level data access)
'   Provides User Activity Logging FUNCTIONALITY
'   Provides System Error Reporting FUNCTIONALITY

Public Module dbLog
    Public Const ERR_DB_MIGRATION As Int16 = 42

    Public Sub logSys(ByVal UserID As String, ByVal ErrorCode As Integer, ByVal Message As String)
        SQLexe( _
        "INSERT INTO [FriedParts].[dbo].[log-Sys] " & _
           "([Date]" & _
           ",[UserID]" & _
           ",[ErrorNum]" & _
           ",[Desc]) " & _
        "VALUES " & _
           "('" & Now & "'" & _
           ",'" & UserID & "'" & _
           ",'" & ErrorCode & "'" & _
           ",'" & sysText.txtDefangSQL(Message) & "')" _
        )
    End Sub
    Public Sub logSys(ByVal UserID As String, ByVal Message As String)
        logSys(UserID, 0, Message) 'Defaults the Error Code if none specified
    End Sub
    Public Sub logSys(ByVal Message As String)
        logSys(0, Message) 'Defaults the UID if none specified
    End Sub

    'Write a debug message to the "Immediate Window" -- only works in local debugging mode
    Public Sub logDebug(ByVal Message As String)
        System.Diagnostics.Debug.WriteLine("[" & Now & "]: " & Message)
    End Sub

    'Generic Interface into the FriedParts User Activity Logging Sub-system.
    'SQL commands should be passed in the same way they would be passed to the SQL Server. 
    '   We'll URL-encode them in this function for safe storage in the database and decode 
    '   with a retrieve function call
    Public Sub logServiceActivity(ByRef Me_Page As Page, ByVal UserID As Int16, ByVal LogMessage As String, _
        Optional ByVal SQL_Redo As String = "", Optional ByVal SQL_Undo As String = "", _
        Optional ByVal Param1_Desc As String = "", Optional ByVal Param1_Old As String = "", Optional ByVal Param1_New As String = "", _
        Optional ByVal Param2_Desc As String = "", Optional ByVal Param2_Old As String = "", Optional ByVal Param2_New As String = "", _
        Optional ByVal Param3_Desc As String = "", Optional ByVal Param3_Old As String = "", Optional ByVal Param3_New As String = "")
        Dim SQLtxt As String = _
            "INSERT INTO [FriedParts].[dbo].[log-User]" & _
            "           (" & _
            "           [UserID]" & _
            "           ,[Page]" & _
            "           ,[Client-Browser]" & _
            "           ,[Client-Address]" & _
            "           ,[Message]" & _
            "           ,[P1desc]" & _
            "           ,[P1old]" & _
            "           ,[P1new]" & _
            "           ,[P2desc]" & _
            "           ,[P2old]" & _
            "           ,[P2new]" & _
            "           ,[P3desc]" & _
            "           ,[P3old]" & _
            "           ,[P3new]" & _
            "           ,[SQL-Redo]" & _
            "           ,[SQL-Undo])" & _
            "     VALUES (" & _
            "           " & UserID & ","
        If Not Me_Page Is Nothing Then
            'Page Request
            SQLtxt = SQLtxt & _
            "           '" & Me_Page.Request.ServerVariables("URL") & "'," & _
            "           '" & txtDefangSQL(Me_Page.Request.ServerVariables("HTTP_USER_AGENT")) & "'," & _
            "           '" & Me_Page.Request.ServerVariables("REMOTE_ADDR") & "',"
        Else
            'Process Request
            SQLtxt = SQLtxt & _
            "           'Scanner Service'," & _
            "           'SOAP'," & _
            "           'localhost',"
        End If
        SQLtxt = SQLtxt & _
            "           '" & txtDefangSQL(LogMessage) & "'," & _
            "           '" & txtDefangSQL(Param1_Desc) & "'," & _
            "           '" & txtDefangSQL(Param1_Old) & "'," & _
            "           '" & txtDefangSQL(Param1_New) & "'," & _
            "           '" & txtDefangSQL(Param2_Desc) & "'," & _
            "           '" & txtDefangSQL(Param2_Old) & "'," & _
            "           '" & txtDefangSQL(Param2_New) & "'," & _
            "           '" & txtDefangSQL(Param3_Desc) & "'," & _
            "           '" & txtDefangSQL(Param3_Old) & "'," & _
            "           '" & txtDefangSQL(Param3_New) & "'," & _
            "           '" & txtSqlEncode(SQL_Redo) & "'," & _
            "           '" & txtSqlEncode(SQL_Undo) & "'" & _
            "           )"
        dbAcc.SQLexe(SQLtxt)
    End Sub

    'Generic Interface into the FriedParts User Activity Logging Sub-system.
    'SQL commands should be passed in the same way they would be passed to the SQL Server. 
    '   We'll URL-encode them in this function for safe storage in the database and decode 
    '   with a retrieve function call
    Public Sub logUserActivity(ByRef Me_Page As Page, ByVal LogMessage As String, _
        Optional ByVal SQL_Redo As String = "", Optional ByVal SQL_Undo As String = "", _
        Optional ByVal Param1_Desc As String = "", Optional ByVal Param1_Old As String = "", Optional ByVal Param1_New As String = "", _
        Optional ByVal Param2_Desc As String = "", Optional ByVal Param2_Old As String = "", Optional ByVal Param2_New As String = "", _
        Optional ByVal Param3_Desc As String = "", Optional ByVal Param3_Old As String = "", Optional ByVal Param3_New As String = "")
        Dim theActiveUserID As Integer
        If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Session("user.UserID") Is Nothing Then
            theActiveUserID = HttpContext.Current.Session("user.UserID")
        Else
            theActiveUserID = sysErrors.ERR_NOTFOUND
        End If
        logServiceActivity(Me_Page, theActiveUserID, LogMessage, SQL_Redo, SQL_Undo, Param1_Desc, Param1_Old, Param1_New, Param2_Desc, Param2_Old, Param2_New, Param3_Desc, Param3_Old, Param3_New)
    End Sub


    Public Sub logUpdateUserStats()

        '[[Determine last day on file]]
        Dim sqlTxt As String = _
            "SELECT [LogID]" & _
            "      ,[Date]" & _
            "  FROM [FriedParts].[dbo].[log-Stats] " & _
            "ORDER BY [Date] DESC"
        Dim dt As New DataTable
        dt = SelectRows(dt, sqlTxt)
        Dim LatestDate As Date
        LatestDate = dt.Rows(0).Field(Of Date)("Date")

        '[[Update any missing days]]
        Dim TS As TimeSpan = Now - LatestDate
        If TS.Days > 1 Then
            Dim OneDay As New TimeSpan(1, 0, 0, 0)
            Dim sqlWrite As String
            Do While TS.Days > 1
                LatestDate = LatestDate + OneDay
                sqlWrite = _
                    "INSERT INTO [FriedParts].[dbo].[log-Stats]" & _
                    "           ([Date]" & _
                    "           ,[UserActivity]" & _
                    "           ,[NumSearches])" & _
                    "     VALUES (" & _
                    "           " & txtSqlDate(LatestDate) & "," & _
                    "           " & numUserEventsPerDay(LatestDate) & "," & _
                    "           " & numSearchesPerDay(LatestDate) & _
                    "           )"
                SQLexe(sqlWrite)
                TS = Now - LatestDate
            Loop
        End If

    End Sub

    Private Function numSearchesPerDay(ByRef theDay As Date) As Integer
        Dim OneDay As New TimeSpan(1, 0, 0, 0)
        Dim sqlTxt As String = _
            "SELECT COUNT([LogID]) " & _
            "FROM [FriedParts].[dbo].[log-Search] " & _
            "WHERE [Date] BETWEEN " & _
            "convert(datetime, '" & txtDateOnly(theDay) & "') AND " & _
            "convert(datetime, '" & txtDateOnly(theDay + OneDay) & "')"
        'MS SQL Server interprets the search "BETWEEN 9/1/2010 AND 9/2/2010" to include
        'All records from 9/1 and none from 9/2 -- e.g. it counts from beginning 
        '   of day to beginning of day
        Dim dt As New DataTable
        dt = SelectRows(dt, sqlTxt)
        Return dt.Rows(0).Field(Of Integer)(0)
    End Function

    Private Function numUserEventsPerDay(ByRef theDay As Date) As Integer
        Dim OneDay As New TimeSpan(1, 0, 0, 0)
        Dim sqlTxt As String = _
            "SELECT COUNT([LogID]) " & _
            "FROM [FriedParts].[dbo].[log-User] " & _
            "WHERE [Date] BETWEEN " & _
            "convert(datetime, '" & txtDateOnly(theDay) & "') AND " & _
            "convert(datetime, '" & txtDateOnly(theDay + OneDay) & "')"
        'MS SQL Server interprets the search "BETWEEN 9/1/2010 AND 9/2/2010" to include
        'All records from 9/1 and none from 9/2 -- e.g. it counts from beginning 
        '   of day to beginning of day
        Dim dt As New DataTable
        dt = SelectRows(dt, sqlTxt)
        Return dt.Rows(0).Field(Of Integer)(0)
    End Function

End Module
