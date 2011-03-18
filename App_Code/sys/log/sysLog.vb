Imports Microsoft.VisualBasic
Imports System.Data

''' <summary>
''' This module encapsulates the SYSTEM ACCESS LOG.
''' That's the log file that Splunk mines.
''' </summary>
''' <remarks></remarks>
Public Module sysLog

    ''' <summary>
    ''' Encapsulates Message Type values to force the developer to get it right
    ''' ...plus make that person's life easier.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class logMessageTypes
        Private iType As MsgTypes
        Public Enum MsgTypes As Byte
            InternetInformationServer = 0
            FriedPartsPageLoad = 1
        End Enum
        Public Overrides Function ToString() As String
            Select Case iType
                Case MsgTypes.InternetInformationServer
                    Return "IIS"
                Case MsgTypes.FriedPartsPageLoad
                    Return "fpPageLoad"
                Case Else
                    Throw New Exception("Message Type Not Defined!")
            End Select
        End Function
        ''' <summary>
        ''' To use: 
        ''' Dim mType as New logMessageTypes(logMessageTypes.MsgTypes.InternetInformationServer)
        ''' </summary>
        ''' <param name="TheMessageType"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal TheMessageType As MsgTypes)
            iType = TheMessageType
        End Sub
    End Class


    ''' <summary>
    ''' Should be called by all pages to log their loading event.
    ''' </summary>
    ''' <remarks>Called by FP.master (master page), so you don't need to add this if you derive from FP.master</remarks>
    Public Sub logRequest(ByVal TheRequest As System.Web.HttpRequest)
        Dim sqltxt As String = _
            "INSERT INTO [FriedParts].[dbo].[log-Ops]" & _
            "           ([Timestamp]" & _
            "           ,[Page]" & _
            "           ,[UserID])" & _
            "     VALUES (" & _
            "            " & txtSqlDate(Now) & "," & _
            "           '[EndReq] " & TheRequest.Url.OriginalString & "'," & _
            "            " & suGetUserID() & "" & _
            "           )"
        SQLexe(sqltxt)
    End Sub

    Public Sub logRequest(ByRef WebApp As HttpApplication, ByVal MsgType As sysLog.logMessageTypes, Optional ByVal UseResponseData As Boolean = False, Optional ByVal Message As String = "")
        'Deal with response objects
        Dim responseStatus As String = "NULL"
        Dim responseSub As String = "NULL"
        If UseResponseData Then
            responseStatus = WebApp.Response.StatusCode
            responseSub = WebApp.Response.SubStatusCode
        End If

        'Some shorthand
        Dim r As HttpRequest = WebApp.Request
        Dim rURL As System.Uri = r.Url
        Dim requestReferrer As String = "NULL"
        If r.UrlReferrer IsNot Nothing Then
            requestReferrer = "'" & txtDefangSQL(r.UrlReferrer.AbsoluteUri) & "'"
        End If


        'Enter data!
        Dim sqltxt As String = _
            "INSERT INTO [FriedParts].[dbo].[log-Ops]" & _
            "           ([Timestamp]" & _
            "           ,[MsgType]" & _
            "           ,[HttpMethod]" & _
            "           ,[UrlHost]" & _
            "           ,[UrlPort]" & _
            "           ,[UrlStem]" & _
            "           ,[UrlQuery]" & _
            "           ,[UrlReferrer]" & _
            "           ,[UserID]" & _
            "           ,[UserIP]" & _
            "           ,[UserAgent]" & _
            "           ,[Status]" & _
            "           ,[Substatus]" & _
            "           ,[MsgID]" & _
            "           ,[MsgIDName]" & _
            "           ,[Message])" & _
            "     VALUES (" & _
            "            " & txtSqlDate(Now) & "," & _
            "           '" & MsgType.ToString & "'," & _
            "           '" & txtDefangSQL(r.HttpMethod) & "'," & _
            "           '" & txtDefangSQL(rURL.Host) & "'," & _
            "            " & rURL.Port & "," & _
            "           '" & txtDefangSQL(rURL.AbsolutePath) & "'," & _
            "           '" & txtDefangSQL(rURL.Query) & "'," & _
            "           " & requestReferrer & "," & _
            "            " & suGetUserID() & "," & _
            "           '" & txtDefangSQL(r.UserHostAddress) & "'," & _
            "           '" & txtDefangSQL(r.UserAgent) & "'," & _
            "            " & responseStatus & "," & _
            "            " & responseSub & "," & _
            "            " & "NULL" & "," & _
            "            " & "NULL" & "," & _
            "           '" & Message & "'" & _
            "           )"
        SQLexe(sqltxt)
    End Sub

    ''' <summary>
    ''' This enumeration contains the cursor motion directives for the logSerializer readout class
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum logSerializerCursors As Byte
        MoveFirst
        MoveNext
        MoveLast
    End Enum

    ''' <summary>
    ''' Use this class to turn the operations log into a Splunk compatible IIS-style text-log file
    ''' </summary>
    ''' <remarks></remarks>
    Public Class logSerializer
        Private iDT As DataTable
        Private iCursor As Int64

        Public ReadOnly Property Count As UInt64
            Get
                Return iDT.Rows.Count
            End Get
        End Property

        ''' <summary>
        ''' Executes the cursor movement and then returns the row. 
        ''' --MoveFirst will return the first row (it moved to the first record, then returned)
        ''' --MoveNext will return the first row the first time executed (if no other 
        ''' Get commands have been issued) because the first MoveNext moves the cursor from 
        ''' uninitialized (loading default) to the first row
        ''' </summary>
        ''' <param name="CursorPosition"></param>
        ''' <returns>Serialized representation of the row contents in Splunk compatible format</returns>
        ''' <remarks>Throws DataException if called when the cursor is past the last record.</remarks>
        Public Function GetRow(Optional ByVal CursorPosition As logSerializerCursors = logSerializerCursors.MoveNext) As String
            'Init
            Dim Output As String = ""
            Dim Data As String
            'Handle Cursor
            Select Case CursorPosition
                Case logSerializerCursors.MoveFirst
                    iCursor = 0
                Case logSerializerCursors.MoveLast
                    iCursor = iDT.Rows.Count - 1
                Case logSerializerCursors.MoveNext
                    iCursor += 1
                Case Else
                    Throw New Exception("Invalid Cursor Movement Specified")
            End Select
            'Handle EOF
            If iCursor >= iDT.Rows.Count Then Throw New DataException("Cursor has run off the end of the dataset... oops!")
            'Serialize This Row!
            For Each c As DataColumn In iDT.Columns
                'Skip this column/field if its data is NULL
                Dim o As Object = iDT.Rows(iCursor).Item(c.ColumnName)
                If o Is Nothing OrElse o.GetType.IsEquivalentTo(GetType(System.DBNull)) _
                    OrElse (TypeOf o Is System.String AndAlso o = "") Then Continue For
                If c.DataType.IsEquivalentTo(GetType(System.DateTime)) Then
                    'This is a date field (requires special handling of its data payload then)
                    'Use RFC1123 formatting -- make sure the data is a GMT timezone time/date
                    Data = Format(iDT.Rows(iCursor).Field(Of DateTime)(c.ColumnName), "R")
                    If c.ColumnName.CompareTo("Timestamp") = 0 Then
                        'This is a special column that goes first and does not have a field name
                        Output += Data & " "
                    End If
                Else
                    'Make the remaining data types Splunk-safe by removing the equals sign (the delimitor)
                    Data = iDT.Rows(iCursor).Item(c.ColumnName).ToString.Replace("=", "")
                    Output += sysText.txtRemoveWhiteSpace(c.ColumnName) & " = " & Data & " "
                End If
            Next
            Return Output
        End Function

        ''' <summary>
        ''' Place holder for constructor common-code.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub NewWorker()
            iCursor = -1
        End Sub

        ''' <summary>
        ''' Alternate constructor if you want to specify your own (alternate) datatable/datasource
        ''' </summary>
        ''' <param name="LogData"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef LogData As DataTable)
            If LogData Is Nothing Then Throw New ArgumentNullException("LogData must be specified. Pass in a valid datatable")
            iDT = LogData
            NewWorker()
        End Sub

        ''' <summary>
        ''' Assumes the log data source is the [FriedParts].[dbo].[log-Ops]
        ''' </summary>
        ''' <param name="StartLogID">The ID of the last log entry to exclude (we start with the 
        ''' next entry after this one). By default it returns everything.</param>
        ''' <remarks></remarks>
        Public Sub New(Optional ByVal StartLogID As UInt64 = 0)
            iDT = New DataTable
            SelectRows(iDT, _
                       "SELECT " & _
                       "[Timestamp], [LogID], [MsgType], [HttpMethod], " & _
                       "[UrlHost], [UrlPort], [UrlStem], [UrlQuery], [UrlReferrer], " & _
                       "[log-Ops].[UserID],[UserName],[UserIP],[UserAgent], " & _
                       "[Status],[Substatus], " & _
                       "[MsgID],[MsgIDName],[Message]" & _
                       "FROM [FriedParts].[dbo].[log-Ops] " & _
                       "LEFT JOIN [FriedParts].[dbo].[user-Accounts] ON [log-Ops].[UserID] = [user-Accounts].[UserID]" & _
                       "WHERE [LogID] > " & StartLogID & " ORDER BY [LogID] ASC")
            NewWorker()
        End Sub
    End Class
End Module
