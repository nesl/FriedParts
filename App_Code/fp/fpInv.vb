Option Explicit On
Imports Microsoft.VisualBasic
Imports System.Data

Public Module fpInv

    Public Class fpInvBinError
        Public PartID_or_ErrCode As Integer
        Public WarehouseID As Integer
        Public BinName As String
    End Class

    'Returns the number of units indicated in all FriedParts "Local" Inventories (summation)
    'Returns sysErrors.ERR_NOTFOUND if the passed in FriedParts Part Number is not found or otherwise invalid.
    'fpPartNum is the Fried Parts part number e.g. 1243, not the manufacturer or distributor part number
    Public Function invLocalStock(ByVal fpPartNum As Integer) As Integer
        Dim dt As New DataTable
        dt = dbAcc.SelectRows(dt, _
               "SELECT SUM([FriedParts].[dbo].[inv-Bins].[QtyHere]) " & _
               "FROM [FriedParts].[dbo].[inv-Bins] " & _
               "WHERE [FriedParts].[dbo].[inv-Bins].[PartID] = " & fpPartNum & ";")
        If dt.Rows.Count > 0 Then
            'Found!
            If Not IsDBNull(dt.Rows(0).Item(0)) Then
                Return dt.Rows(0).Field(Of Integer)(0)
            Else
                'Not Found!
                Return sysErrors.ERR_NOTFOUND
            End If
        Else
            'Not Found!
            Return sysErrors.ERR_NOTFOUND
        End If
    End Function

    'Adds a new bin to the database and logs the event.
    'Me_Page parameter is called as "Me" from the webpage using this function (needed for logging)
    Public Function invAddAvailableBin(ByRef Me_Page As Page, ByVal WarehouseID As Integer, ByVal Cabinet As Char, ByVal Drawer As Byte, Optional ByVal SlotStart As Int16 = -1, Optional ByVal SlotStop As Int16 = -1) As fpInvBinError
        'VAR
        Dim MultipleSlots As Boolean = False
        Dim i As Byte
        Dim BinName As String
        Dim RetVal As New fpInvBinError

        'MANAGE MULTIPLE SLOT ADDING
        If SlotStart <> -1 And SlotStop <> -1 Then
            'MULTIPLE SLOT ADD DETECTED!
            MultipleSlots = True
        End If

        If MultipleSlots Then
            For i = SlotStart To SlotStop
                'FORMAT INTO A BIN NAME
                BinName = invFormatBinName(Cabinet, Drawer, i)

                'CHECK IF BIN IS VACANT
                Dim Result As Integer = invGetPartInBin(WarehouseID, BinName)
                If Result <> sysErrors.ERR_NOTFOUND Then
                    RetVal.PartID_or_ErrCode = Result
                    RetVal.WarehouseID = WarehouseID
                    RetVal.BinName = BinName
                    Return RetVal
                End If


                'CHECK FOR UNIQUENESS
                If invBinExists(WarehouseID, BinName) <> sysErrors.ERR_NOTFOUND Then
                    'Error conditions are negative, Zero is unused (included for safety), Positive values are valid BinID's -- so it already exists
                    MsgBox(Me_Page, sysErrors.BINADD_ALREADYEXISTS, BinName, invGetWarehouseName(WarehouseID))
                    Exit Function
                End If
            Next

            For i = SlotStart To SlotStop
                'FORMAT INTO A BIN NAME
                BinName = invFormatBinName(Cabinet, Drawer, i)

                'WRITE TO DB
                Dim sqlStr As String = _
                    "INSERT INTO [FriedParts].[dbo].[inv-WarehouseBins]" & _
                    "           ([WarehouseID]" & _
                    "           ,[BinName])" & _
                    "     VALUES (" & _
                    "           " & WarehouseID & "," & _
                    "           '" & BinName & "'" & _
                    "           )"
                dbAcc.SQLexe(sqlStr)

                'Log (to save the SQL CMD)
                logUserActivity(Me_Page, suGetUserFirstName() & " added bin location " & BinName & " to warehouse " & invGetWarehouseName(WarehouseID), sqlStr)
            Next
        Else 'SINGLE BIN/SLOT

            'FORMAT INTO A BIN NAME
            BinName = invFormatBinName(Cabinet, Drawer, SlotStart)

            'CHECK IF BIN IS VACANT
            Dim Result As Integer = invGetPartInBin(WarehouseID, BinName)
            If Result <> sysErrors.ERR_NOTFOUND Then
                RetVal.PartID_or_ErrCode = Result
                RetVal.WarehouseID = WarehouseID
                RetVal.BinName = BinName
                Return RetVal
            End If

            'CHECK FOR UNIQUENESS
            If invBinExists(WarehouseID, BinName) <> sysErrors.ERR_NOTFOUND Then
                'Error conditions are negative, Zero is unused (included for safety), Positive values are valid BinID's -- so it already exists
                MsgBox(Me_Page, sysErrors.BINADD_ALREADYEXISTS, BinName, invGetWarehouseName(WarehouseID))
                Exit Function
            End If

            'WRITE TO DB
            Dim sqlStr As String = _
                "INSERT INTO [FriedParts].[dbo].[inv-WarehouseBins]" & _
                "           ([WarehouseID]" & _
                "           ,[BinName])" & _
                "     VALUES (" & _
                "           " & WarehouseID & "," & _
                "           '" & BinName & "'" & _
                "           )"
            dbAcc.SQLexe(sqlStr)

            'LOG
            logUserActivity(Me_Page, suGetUserFirstName() & " added bin location " & BinName & " to warehouse " & invGetWarehouseName(WarehouseID), sqlStr)
        End If

        'REPORT
        If MultipleSlots Then
            MsgBox(Me_Page, sysErrors.BINADD_SUCCESS, invFormatBinName(Cabinet, Drawer, SlotStart) & "-" & SlotStop, invGetWarehouseName(WarehouseID))
        Else
            MsgBox(Me_Page, sysErrors.BINADD_SUCCESS, BinName, invGetWarehouseName(WarehouseID))
        End If
        RetVal.BinName = BinName
        RetVal.WarehouseID = WarehouseID
        RetVal.PartID_or_ErrCode = sysErrors.NO_ERROR
        Return RetVal
    End Function

    'Takes in the parts of a bin box and returns a properly formatted string representing the official FriedParts BinBox Name
    Public Function invFormatBinName(ByVal Cabinet As Char, ByVal Drawer As Byte, Optional ByVal Slot As Integer = -1) As String
        If Slot = -1 Then
            Return Cabinet & Drawer.ToString("D2")
        Else
            Return Cabinet & Drawer.ToString("D2") & "." & Slot
        End If
    End Function

    Public Function invBinExists(ByVal WarehouseID As Integer, ByVal Cabinet As Char, ByVal Drawer As Byte, Optional ByVal Slot As Integer = -1) As Integer
        Return invBinExists(WarehouseID, invFormatBinName(Cabinet, Drawer, Slot))
    End Function
    'Tests to see if a particular Bin/Warehouse combination exists in the table. Returns its BinID if yes, ERRNOTFOUND otherwise.
    Public Function invBinExists(ByVal WarehouseID As Integer, ByVal BinName As String) As Integer
        Dim sqlTxt As String = _
            "SELECT [BinID]" & _
            "      ,[WarehouseID]" & _
            "      ,[BinName]" & _
            "  FROM [FriedParts].[dbo].[inv-WarehouseBins] " & _
            " WHERE [WarehouseID]=" & WarehouseID & " AND [BinName]='" & BinName & "'"
        Dim dt As New DataTable
        SelectRows(dt, sqlTxt)
        Select Case dt.Rows.Count
            Case 0
                Return sysErrors.ERR_NOTFOUND
            Case 1
                Return dt.Rows(0).Field(Of Integer)("BinID")
            Case Else
                Return sysErrors.ERR_NOTUNIQUE
        End Select
    End Function


    'Retrieves the Warehouse Name from WarehouseID
    'Returns syserror.ERRNOTFOUND otherwise
    Public Function invGetWarehouseName(ByVal WarehouseID As Integer) As String
        Dim sqlStr As String = _
            "SELECT " & _
            "      [Abbrev]" & _
            "  FROM [FriedParts].[dbo].[inv-Warehouses] " & _
            " WHERE [WarehouseID] = " & WarehouseID
        Dim dt As New DataTable
        dbAcc.SelectRows(dt, sqlStr)
        Select Case dt.Rows.Count
            Case 0
                Return sysErrors.ERR_NOTFOUND
            Case 1
                'Normal case
                Return dt.Rows(0).Field(Of String)("Abbrev")
            Case Else
                Return sysErrors.ERR_NOTUNIQUE
        End Select
    End Function


    'Returns the FriedParts PartID of the Part currently assigned to the bin
    'Returns errNOTFOUND otherwise
    Public Function invGetPartInBin(ByVal WarehouseID As Integer, ByVal BinName As String) As Integer
        Dim sqlTxt As String = _
            "SELECT [PartID]" & _
            "      ,[WarehouseID]" & _
            "      ,[BinLocation]" & _
            "      ,[QtyHere]" & _
            "  FROM [FriedParts].[dbo].[inv-Bins] " & _
            " WHERE [WarehouseID] = " & WarehouseID & " AND [BinLocation] = '" & BinName & "'"
        Dim dt As New DataTable
        SelectRows(dt, sqlTxt)
        Select Case dt.Rows.Count
            Case 0
                Return sysErrors.ERR_NOTFOUND
            Case 1
                Return dt.Rows(0).Field(Of Integer)("PartID")
            Case Else
                Return sysErrors.ERR_NOTUNIQUE
        End Select
    End Function

    'Adds a specific part to a specific bin
    Public Sub invAddPartToBin(ByVal Me_Page As Page, ByVal WarehouseID As Integer, ByVal BinName As String, ByVal PartID As Integer)
        'DO IT
        Dim sqlTxt As String = _
            "INSERT INTO [FriedParts].[dbo].[inv-Bins]" & _
            "           ([PartID]" & _
            "           ,[WarehouseID]" & _
            "           ,[BinLocation]" & _
            "           ,[QtyHere])" & _
            "     VALUES (" & _
            "           '" & PartID & "'," & _
            "           '" & WarehouseID & "'," & _
            "           '" & BinName & "'," & _
            "           " & 0 & "" & _
            "           )"
        SQLexe(sqlTxt)

        'REMOVE NOW FILLED BIN FROM EMPTY LIST
        invUpdateAvailableBins()

        'LOG IT
        dbLog.logUserActivity(Me_Page, suGetUserFirstName() & " added Part #" & PartID & " (" & partGetShortName(PartID) & ") to bin " & BinName & " of " & fpInv.invGetWarehouseName(WarehouseID), sqlTxt, , "PartID", , PartID, "WarehouseID", , WarehouseID, "BinName", , BinName)
    End Sub


    'Adds a specific part quantity to the inventory
    Public Sub invAddQtyToBin(ByVal Me_Page As Page, ByVal QtyChange As Integer, ByVal WarehouseID As Integer, ByVal BinName As String, ByVal PartID As Integer)
        'DO IT
        Dim sqlTxt As String = _
            "INSERT INTO [FriedParts].[dbo].[inv-WarehouseQty]" & _
            "           ([QtyDelta]" & _
            "           ,[WarehouseID]" & _
            "           ,[BinName]" & _
            "           ,[PartID]" & _
            "           ,[UserID]" & _
            "           ,[Date])" & _
            "     VALUES (" & _
            "           '" & QtyChange & "'," & _
            "           '" & WarehouseID & "'," & _
            "           '" & BinName & "'," & _
            "           '" & PartID & "'," & _
            "           '" & HttpContext.Current.Session("user.UserID") & "'," & _
            "           '" & sysText.txtSQLDate(Now) & "'" & _
            "           )"
        dbAcc.SQLexe(sqlTxt)
        'LOG IT
        dbLog.logUserActivity(Me_Page, suGetUserFirstName() & " added " & QtyChange & " piece(s) of part #" & PartID & " (" & partGetShortName(PartID) & ") to Bin " & BinName & " of " & invGetWarehouseName(WarehouseID), sqlTxt, , "QtyChange", , QtyChange, "PartID", , PartID, "WarehouseID", , WarehouseID)
    End Sub

    'Deletes all bins from the available bin list table (inv-WarehouseBins) that are in use (e.g. not available)
    Public Sub invUpdateAvailableBins()
        Dim sqlTxt As String = _
            "DELETE FROM [FriedParts].[dbo].[inv-WarehouseBins] " & _
            "WHERE EXISTS( " & _
            "Select [BinLocation] " & _
            "FROM [FriedParts].[dbo].[inv-Bins] " & _
            "WHERE [inv-WarehouseBins].[BinName] = [inv-Bins].[BinLocation] " & _
            "AND [inv-WarehouseBins].[WarehouseID] = [inv-Bins].[WarehouseID] " & _
            ") "
        SQLexe(sqlTxt)
    End Sub
End Module
