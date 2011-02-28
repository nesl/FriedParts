Imports Microsoft.VisualBasic
Imports System.Data
Imports System 'Covers DBNull

Public Module fpAltium
    Public Const fpaltTypeSymbol As String = "Schematic Symbol"
    Public Const fpaltTypeFootprint As String = "PCB Footprint"
    Public Const fpaltTypeM3D As String = "3D Mechanical Body"
    Public Const fpaltTypeElectrical As String = "Spice Function"
    Public Const fpaltModeNorm As String = "[Use Normal Mode]: "
    Public Const fpaltModeAlt1 As String = "[Use Alternate1 Mode]: "
    Public Const fpaltModeAlt2 As String = "[Use Alternate2 Mode]: "

    Public Function fpaltSummaryTable(ByVal xPartID As Int32) As DataTable
        'INIT
        Dim DTin As New DataTable
        Dim DTout As New DataTable
        Dim DR As DataRow
        Dim str1 As String
        Dim str2 As String
        Dim str3 As String

        'GET INPUT DATA FROM DATABASE
        DTin = dbAcc.SelectRows(DTin, cnxStr, "SELECT * FROM [FriedParts].[dbo].[cad-Altium] WHERE [PartID]=" & xPartID)
        'INIT OUTPUT TABLE
        DTout.Columns.Add("Model Type", Type.GetType("System.String"))
        DTout.Columns.Add("Library Name", Type.GetType("System.String"))
        DTout.Columns.Add("Model Name", Type.GetType("System.String"))
        DTout.Columns.Add("Model Description", Type.GetType("System.String"))

        'FILL OUTPUT TABLE

        'Schematic Symbol 
        str1 = DTin.Rows(0).Field(Of String)("Library Path")
        str2 = DTin.Rows(0).Field(Of String)("Library Ref")
        str3 = DTin.Rows(0).Field(Of String)("Symbol_Desc")
        If (str1 Is DBNull.Value) Or (Len(str1) = 0) Or (str2 Is DBNull.Value) Or (Len(str2) = 0) Then
            'Skip if one of the key fields is null
        Else
            DR = DTout.NewRow()
            DR(0) = fpaltTypeSymbol
            DR(1) = str1
            DR(2) = str2
            If (str3 Is DBNull.Value) Or (Len(str3) = 0) Then
                'No description available
                DR(3) = ""
            Else
                DR(3) = fpaltModeNorm & str3
            End If
            DTout.Rows.Add(DR)
            'Desc2 -- second schematic symbol mode
            str3 = DTin.Rows(0).Field(Of String)("Symbol_Desc2")
            If (str3 Is DBNull.Value) Or (Len(str3) = 0) Then
                'Does not exist -- skip!
            Else
                DR = DTout.NewRow()
                DR(0) = fpaltTypeSymbol
                DR(1) = str1
                DR(2) = str2
                DR(3) = fpaltModeAlt1 & str3
                DTout.Rows.Add(DR)
            End If
            'Desc3 -- third schematic symbol mode
            str3 = DTin.Rows(0).Field(Of String)("Symbol_Desc3")
            If (str3 Is DBNull.Value) Or (Len(str3) = 0) Then
                'Does not exist -- skip!
            Else
                DR = DTout.NewRow()
                DR(0) = fpaltTypeSymbol
                DR(1) = str1
                DR(2) = str2
                DR(3) = fpaltModeAlt2 & str3
                DTout.Rows.Add(DR)
            End If
        End If 'Schematic Symbol

        'Footprint1
        str1 = DTin.Rows(0).Field(Of String)("Footprint Path")
        str2 = DTin.Rows(0).Field(Of String)("Footprint Ref")
        str3 = DTin.Rows(0).Field(Of String)("Footprint_Desc")
        If (str1 Is DBNull.Value) Or (Len(str1) = 0) Or (str2 Is DBNull.Value) Or (Len(str2) = 0) Then
            'Skip if one of the key fields is null
        Else
            DR = DTout.NewRow()
            DR(0) = fpaltTypeFootprint
            DR(1) = str1
            DR(2) = str2
            If (str3 Is DBNull.Value) Or (Len(str3) = 0) Then
                'No description available
                DR(3) = ""
            Else
                DR(3) = str3
            End If
            DTout.Rows.Add(DR)
        End If 'Footprint1

        'Footprint2
        str1 = DTin.Rows(0).Field(Of String)("Footprint Path 2")
        str2 = DTin.Rows(0).Field(Of String)("Footprint Ref 2")
        str3 = DTin.Rows(0).Field(Of String)("Footprint_Desc2")
        If (str1 Is DBNull.Value) Or (Len(str1) = 0) Or (str2 Is DBNull.Value) Or (Len(str2) = 0) Then
            'Skip if one of the key fields is null
        Else
            DR = DTout.NewRow()
            DR(0) = fpaltTypeFootprint
            DR(1) = str1
            DR(2) = str2
            If (str3 Is DBNull.Value) Or (Len(str3) = 0) Then
                'No description available
                DR(3) = ""
            Else
                DR(3) = str3
            End If
            DTout.Rows.Add(DR)
        End If 'Footprint2

        'Footprint3
        str1 = DTin.Rows(0).Field(Of String)("Footprint Path 3")
        str2 = DTin.Rows(0).Field(Of String)("Footprint Ref 3")
        str3 = DTin.Rows(0).Field(Of String)("Footprint_Desc3")
        If (str1 Is DBNull.Value) Or (Len(str1) = 0) Or (str2 Is DBNull.Value) Or (Len(str2) = 0) Then
            'Skip if one of the key fields is null
        Else
            DR = DTout.NewRow()
            DR(0) = fpaltTypeFootprint
            DR(1) = str1
            DR(2) = str2
            If (str3 Is DBNull.Value) Or (Len(str3) = 0) Then
                'No description available
                DR(3) = ""
            Else
                DR(3) = str3
            End If
            DTout.Rows.Add(DR)
        End If 'Footprint3

        'PCB3D1
        str1 = DTin.Rows(0).Field(Of String)("PCB3D Path")
        str2 = DTin.Rows(0).Field(Of String)("PCB3D Ref")
        str3 = DTin.Rows(0).Field(Of String)("PCB3D_Desc")
        If (str1 Is DBNull.Value) Or (Len(str1) = 0) Or (str2 Is DBNull.Value) Or (Len(str2) = 0) Then
            'Skip if one of the key fields is null
        Else
            DR = DTout.NewRow()
            DR(0) = fpaltTypeM3D
            DR(1) = str1
            DR(2) = str2
            If (str3 Is DBNull.Value) Or (Len(str3) = 0) Then
                'No description available
                DR(3) = ""
            Else
                DR(3) = str3
            End If
            DTout.Rows.Add(DR)
        End If 'PCB3D1

        'PCB3D2
        str1 = DTin.Rows(0).Field(Of String)("PCB3D Path 2")
        str2 = DTin.Rows(0).Field(Of String)("PCB3D Ref 2")
        str3 = DTin.Rows(0).Field(Of String)("PCB3D_Desc2")
        If (str1 Is DBNull.Value) Or (Len(str1) = 0) Or (str2 Is DBNull.Value) Or (Len(str2) = 0) Then
            'Skip if one of the key fields is null
        Else
            DR = DTout.NewRow()
            DR(0) = fpaltTypeM3D
            DR(1) = str1
            DR(2) = str2
            If (str3 Is DBNull.Value) Or (Len(str3) = 0) Then
                'No description available
                DR(3) = ""
            Else
                DR(3) = str3
            End If
            DTout.Rows.Add(DR)
        End If 'PCB3D2

        'PCB3D3
        str1 = DTin.Rows(0).Field(Of String)("PCB3D Path 3")
        str2 = DTin.Rows(0).Field(Of String)("PCB3D Ref 3")
        str3 = DTin.Rows(0).Field(Of String)("PCB3D_Desc3")
        If (str1 Is DBNull.Value) Or (Len(str1) = 0) Or (str2 Is DBNull.Value) Or (Len(str2) = 0) Then
            'Skip if one of the key fields is null
        Else
            DR = DTout.NewRow()
            DR(0) = fpaltTypeM3D
            DR(1) = str1
            DR(2) = str2
            If (str3 Is DBNull.Value) Or (Len(str3) = 0) Then
                'No description available
                DR(3) = ""
            Else
                DR(3) = str3
            End If
            DTout.Rows.Add(DR)
        End If 'PCB3D3

        'RETURN OUTPUT TABLE
        Return DTout
    End Function

    'Very primative (needs elaboration!)
    Public Sub fpaltAddPart(ByVal PartID As Int32, ByVal LibRef As String, ByVal LibPath As String, ByVal FootRef As String, ByVal FootPath As String)
        Dim sqlPartAdd As String = _
            "INSERT INTO [FriedParts].[dbo].[cad-Altium]" & _
            "           ([PartID]" & _
            "           ,[Library Ref]" & _
            "           ,[Library Path]" & _
            "           ,[Symbol_Desc]" & _
            "           ,[Symbol_Desc2]" & _
            "           ,[Symbol_Desc3]" & _
            "           ,[PCB3D Ref]" & _
            "           ,[PCB3D Path]" & _
            "           ,[PCB3D_Desc]" & _
            "           ,[PCB3D Ref 2]" & _
            "           ,[PCB3D Path 2]" & _
            "           ,[PCB3D_Desc2]" & _
            "           ,[PCB3D Ref 3]" & _
            "           ,[PCB3D Path 3]" & _
            "           ,[PCB3D_Desc3]" & _
            "           ,[Footprint Ref]" & _
            "           ,[Footprint Path]" & _
            "           ,[Footprint_Desc]" & _
            "           ,[Footprint Ref 2]" & _
            "           ,[Footprint Path 2]" & _
            "           ,[Footprint_Desc2]" & _
            "           ,[Footprint Ref 3]" & _
            "           ,[Footprint Path 3]" & _
            "           ,[Footprint_Desc3])" & _
            "     VALUES (" & _
            "           " & PartID & "," & _
            "           '" & sysText.txtDefangSQL(LibRef) & "'," & _
            "           '" & sysText.txtDefangSQL(LibPath) & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & " '," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & sysText.txtDefangSQL(FootRef) & "'," & _
            "           '" & sysText.txtDefangSQL(FootPath) & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'," & _
            "           '" & "" & "'" & _
            "           )"
        dbAcc.SQLexe(sqlPartAdd)
    End Sub
End Module
