<%@ WebService Class="sysWebService" %>
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://friedparts.com/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class sysWebService
    Inherits System.Web.Services.WebService
    
    Private Const UNINITIALIZED As Int16 = 0

    Private Enum logMsgTypes As Byte
        msgERROR = 1
        msgSTART = 2
        msgSTOP = 3
    End Enum

    Private Enum scanStatus As Byte
        scanREADSTATUS = 0
        scanIDLE = 1
        scanWAITFORDK = 2
        scanWAITFOROP = 3
        scanRUNNING = 4
    End Enum

    Private UpdatingPartID As Int16 = UNINITIALIZED
    Private UpdatingStatus As scanStatus = scanStatus.scanIDLE

    <WebMethod()> Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    'Do NOT call me faster than once per 10 seconds
    Private Function UpdateOnePart() As String
        'Mutual Exclusion (MUTEX)
        logStatus() 'Check the DB to see if worker thread is already running...
        If UpdatingStatus = scanStatus.scanIDLE Then
            'MUTEX -- this mechanism isn't perfect in that there is a few milliseconds of vulnerability -- but we'll just chance it for now
            logStatus(scanStatus.scanRUNNING)

            'Determine which record we will update next
            FindPartToUpdate() 'Find the oldest record and update it first (rolling scan)

            'Digikey Search
            logStatus(scanStatus.scanWAITFORDK)
            Dim dkPartNum As String = apiDigikey.dkPartNumber(UpdatingPartID)
            If Not (dkPartNum = sysErrors.ERR_NOTFOUND Or dkPartNum = sysErrors.ERR_NOTUNIQUE) Then
                'this part has a Digikey Part Number
                Dim DK As New apiDigikey(dkPartNum)
                If DK.PartReady Then
                    'Check Part Type
                    'Check Pricing
                    'Check Long Description
                    'Check Value, Units
                    'Check Value Numeric
                    'Check Temp Codes

                    'hiddenDigikeyPartNumber.Value = DK.getDigikeyPartNum
                    'hiddenMfrPartNumber.Value = DK.getMfrPartNum
                    'xPanelDkNo.Visible = False
                    'xPanelDkYes.Visible = True
                    'xLblDigikey.Text = "FOUND! " & DK.getMfrName & " " & DK.getMfrPartNum & ": " & DK.getShortDesc
                    'imgDigikey.ImageUrl = DK.getImageURL
                    'imgDigikey.Width = 200
                    'imgDigikey.Height = 200
                    'linkDigikey.NavigateUrl = DK.getDatasheetURL
                Else
                    'Digikey part number is not found
                    logError("Digikey part number not found -- or Digikey timeout")
                End If
            Else
                If dkPartNum = sysErrors.ERR_NOTFOUND Then
                    'Digikey Part Number not known for this part
                    logError("This part does not have a Digikey part number!")
                Else
                    'Multiple Digikey Part Numbers found
                    logError("This part had multiple matching Digikey part numbers!")
                End If

                'Octopart Search
                logStatus(scanStatus.scanWAITFOROP)
                Dim OP As New Octopart("The Part Number")
                'xGridOctopartMaster.DataSource = OP.MPN_List
                'xGridOctopartMaster.DataBind()


                'Make Changes
                'Log Changes
                'Update Status Entry in Database
                logStatus(scanStatus.scanIDLE)
                Return "Updated Part: "
            End If
        Else
            'Scanner started, but another thread is already running!
            logError("Scanner process started, but already running in another thread. Status is " & UpdatingStatus & " via PartID " & UpdatingPartID & ".")
        End If
        Return "Hi!"
    End Function

    'Updates the UpdatingPartID class state variable
    'Updates the current LastScanned Date/Time value in the database (Does not update the LastModified date/time -- do that only if changes are made)
    Private Sub FindPartToUpdate()
        Dim dt As DataTable
        dt = dbAcc.SelectRows(dt, _
                "SELECT * " & _
                "FROM [FriedParts].[dbo].[part-Common] " & _
                "ORDER BY [FriedParts].[dbo].[Date_LastScanned] ASC")
        If dt.Rows.Count = 0 Then
            'Error No Parts Found! -- this can't happen
            logError("No Parts Found! [part-Common] table EMPTY?!")
        Else

            'Grab the first record -- which should be the least updated one because we sorted by scan-date
            UpdatingPartID = dt.Rows(0).Field(Of Int16)("PartID")

            'Mark this one as scanned
            If Not UpdatingPartID = UNINITIALIZED Then
                Dim sqlText As String = _
                    "UPDATE [FriedParts].[dbo].[part-Common]" & _
                    "   SET " & _
                    "      [Date_LastScanned] = '" & txtSqlDate(Now) & "'" & _
                    "   WHERE [PartID] = " & UpdatingPartID
                dbAcc.SQLexe(sqlText)
            End If
        End If
    End Sub

    'Log an scanner error event to the scan-Log database table
    Private Sub logError(ByVal ErrorMessage As String)
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

    Private Sub logScanStart()

    End Sub

    Private Sub logScanStop()

    End Sub

    Private Sub logStatus(Optional ByVal newStatus As scanStatus = scanStatus.scanREADSTATUS)
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
    
End Class