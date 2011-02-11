Imports Microsoft.VisualBasic
Imports System.Data

'===========================
' This module contains all code that is specific to the logging, error reporting, and 
' status management of the WebService/WinService -- e.g. everything except the actual 
' data-updating worker code (that goes in fpUpdate.vb)
'===========================

Public Module apiWebService

    'THIS STUFF NEEDS A MAJOR REWRITE 'xxx

    Public Const UNINITIALIZED As Int16 = 0

    Public Enum logMsgTypes As Byte
        msgERROR = 1
        msgSTART = 2
        msgSTOP = 3
    End Enum

    Public Enum scanStatus As Byte
        scanREADSTATUS = 0
        scanIDLE = 1
        scanWAITFORDK = 2
        scanWAITFOROP = 3
        scanRUNNING = 4
    End Enum

    'Log an scanner error event to the scan-Log database table
    Public Sub logError(ByVal ErrorMessage As String)
        Dim sqlTxt As String = _
            "INSERT INTO [FriedParts].[dbo].[scan-Log]" & _
            "           ([Date]" & _
            "           ,[PartID]" & _
            "           ,[MsgType]" & _
            "           ,[Msg])" & _
            "     VALUES (" & _
            "           '" & sysText.txtSqlDate(Now) & "'," & _
            "           '" & UpdatingPartID & "'," & _
            "           '" & logMsgTypes.msgERROR & "'," & _
            "           '" & ErrorMessage & "'," & _
            "           )"
        dbAcc.SQLexe(sqlTxt)
    End Sub

    Public Sub logScanStart()

    End Sub

    Public Sub logScanStop()

    End Sub

    Public Sub logStatus(Optional ByVal newStatus As scanStatus = scanStatus.scanREADSTATUS)
        If newStatus = scanStatus.scanREADSTATUS Then
            'Reading the existing status provides MUTEX function and prevents repeated calls to the service from accumulating
            Dim dt As DataTable
            dt = dbAcc.SelectRows(dt, _
                "SELECT [Date]" & _
                "      ,[Status]" & _
                "      ,[PartID]" & _
                "  FROM [FriedParts].[dbo].[scan-Status]" & _
                " ORDER BY [Date] DESC")
            'Grab the newest entry
            UpdatingStatus = dt.Rows(0).Field(Of Byte)("Status")
            UpdatingPartID = dt.Rows(0).Field(Of Int16)("PartID")
        Else
            'Change Status
            Dim sqltxt As String = _
                "INSERT INTO [FriedParts].[dbo].[scan-Status]" & _
                "           ([Date]" & _
                "           ,[Status]" & _
                "           ,[PartID])" & _
                "     VALUES (" & _
                "           '" & txtSqlDate(Now) & "'," & _
                "            " & newStatus & "," & _
                "            " & UpdatingPartID & "" & _
                "           )"
            dbAcc.SQLexe(sqltxt)
            UpdatingStatus = newStatus
        End If
    End Sub
End Module
