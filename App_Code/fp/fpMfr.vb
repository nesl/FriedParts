Option Explicit On
Imports Microsoft.VisualBasic
Imports System.Data

'Contains all code to manipulate the Manufacturer related database tables

Public Module fpMfr

    '=========================================================================================
    '== MANUFACTURER
    '=========================================================================================

    Public Const fpMfr_ADD_YOUR_OWN As String = "(...or if it isn't already here, enter the manufacturer in the box below!)"

    'Returns the name for the ID specified manufacturer
    Public Function mfrGetName(ByVal mfrID As Int16) As String
        Dim dt As New DataTable
        dt = dbAcc.SelectRows(dt, _
            "SELECT [mfrName]" & _
            "  FROM [FriedParts].[dbo].[view-mfr]" & _
            " WHERE [mfrID] = " & mfrID)
        If dt.Rows.Count = 0 Then
            Return sysErrors.ERR_NOTFOUND
        Else
            Return dt.Rows(0).Field(Of String)("mfrName")
        End If
    End Function

    Public Function mfrGetWebsite(ByVal mfrID As Int16) As String
        Dim dt As New DataTable
        dt = dbAcc.SelectRows(dt, _
            "SELECT [mfrWebsite]" & _
            "  FROM [FriedParts].[dbo].[view-mfr]" & _
            " WHERE [mfrID] = " & mfrID)
        Return dt.Rows(0).Field(Of String)("mfrWebsite")
    End Function

    'If exists returns an array of matching MfrID's, Else returns -1
    'The returned array consists only of unique manufacturer ID's 
    '   (in case it is found under multiple names -- likely -- as in "Panasonic ECG", "Panasonic, Inc.", "Panasonic Corporation").
    Public Function mfrExistsName(ByVal MfrName As String, Optional ByVal Exact As Boolean = False) As Int32()
        Dim dt As New DataTable
        Dim retval() As Int32 = {sysErrors.ERR_NOTFOUND}
        If Exact Then
            dt = dbAcc.SelectRows(dt, _
                "SELECT DISTINCT [FriedParts].[dbo].[mfr-Common].[mfrID] " & _
                "FROM [FriedParts].[dbo].[mfr-Common] INNER JOIN [FriedParts].[dbo].[mfr-Names] ON [mfr-Common].[mfrID] = [mfr-Names].[mfrID] " & _
                "WHERE [FriedParts].[dbo].[mfr-Names].[mfrName] = '" & Trim(MfrName) & "';")
        Else
            dt = dbAcc.SelectRows(dt, _
                "SELECT DISTINCT [FriedParts].[dbo].[mfr-Common].[mfrID] " & _
                "FROM [FriedParts].[dbo].[mfr-Common] INNER JOIN [FriedParts].[dbo].[mfr-Names] ON [mfr-Common].[mfrID] = [mfr-Names].[mfrID] " & _
                "WHERE [FriedParts].[dbo].[mfr-Names].[mfrName] LIKE '%" & Trim(MfrName) & "%';")
        End If

        If dt.Rows.Count > 0 Then
            'Found!
            Return fpUniqueID(dt, "mfrID")
        Else
            'Not Found!
            Return retval
        End If
    End Function

    'Checks for a UNIQUE match to your name-to-ID query. Returns sysErrors.ERR_NOTUNIQUE or sysErrors.ERR_NOTFOUND depending.
    'Be careful when specifying Exact = False, as this may match multiple records and hence result in sysErrors.ERR_NOTUNIQUE
    Public Function mfrExistsUniqueName(ByVal MfrName As String, Optional ByVal Exact As Boolean = True) As Int32
        Dim blah() As Int32
        blah = mfrExistsName(MfrName, Exact)
        Select Case UBound(blah)
            Case 0
                'Only one return! Yay!
                Return blah(0) 'Covers the sysErrors.ERR_NOTFOUND case since that will be in the result
            Case Else
                Return sysErrors.ERR_NOTUNIQUE
        End Select
    End Function

    'Adds a new Manufacturer to the Database
    'Returns the MfrID if found, or if added
    'Returns -1 if can't find or add (e.g. on a weird error condition)
    Public Function mfrAdd(ByVal MfrName As String, Optional ByVal MfrWebsite As String = "", Optional ByRef Me_Page As Page = Nothing) As Int32
        Dim dt As New DataTable
        Dim retval As Int32() = mfrExistsName(MfrName, True) 'Use exact matching
        If retval(0) = sysErrors.ERR_NOTFOUND Then
            'NOT FOUND! ADD THIS MANUFACTURER!
            'This is actually a bit of a tricky process
            '
            '[STEP 1] CREATE A NEW MANUFACTURER ID
            '
            dbLog.logDebug("[Add Mfr] '" & MfrName & "' Not Found! Adding!")
            Dim newMfrID As Int32
            'Generate Temporary ID
            Dim tempUID As Int32 = procGenTempUID()
            'Create New Manufacturer Record (COMMON)
            Dim strcmd As String = _
                "INSERT INTO [FriedParts].[dbo].[mfr-Common]" & _
                "           ([mfrNameID]" & _
                "           ,[mfrWebsite])" & _
                "     VALUES (" & _
                "           " & tempUID & "," & _
                "           '" & MfrWebsite & "'" & _
                "           )"
            'We set the mfrNameID to a unique negative number to mark it for error detection. In theory, there should never be any -1 records in the common table since every mfr-common entry corresponds to at least one mfr-name entry.
            dbAcc.SQLexe(strcmd)
            '
            '[STEP 2] FIND THE NEW MFR-ID WE JUST CREATED
            '
            dt = dbAcc.SelectRows(dt, _
                "SELECT [FriedParts].[dbo].[mfr-Common].[mfrID] " & _
                "FROM [FriedParts].[dbo].[mfr-Common] " & _
                "WHERE [FriedParts].[dbo].[mfr-Common].[mfrNameID] = " & tempUID & ";")
            Select Case dt.Rows.Count
                Case 0
                    'Not found! ERROR!!! (should never happen)
                    dbLog.logSys(0, "[Add Mfr] '" & MfrName & "' Not Found! During Add Using Temp ID: " & tempUID)
                    Return sysErrors.ERR_NOTFOUND 'ERROR!
                Case 1
                    'Yay! Perfect (this is exactly what should happen)
                    newMfrID = dt.Rows(0).Field(Of Int32)(0) 'query only selected one row
                Case Else
                    'Found more than one!!! Oh No!!! (shouldn't happen)
                    dbLog.logSys(0, "[Add Mfr] '" & MfrName & "' Matched multiple using Temp ID: " & tempUID)
                    Return sysErrors.ERR_NOTFOUND 'ERROR!
            End Select
            '
            '[STEP 3] ADD THE NEW MFR-NAME RECORD
            '
            Dim newMfrNameID As Int32 = addMfrAlias(newMfrID, MfrName)
            '
            '[STEP 4] GO BACK AND ADD THE CORRECT RELATIONSHIP TO THE NEW NAME
            '
            strcmd = _
                "UPDATE [FriedParts].[dbo].[mfr-Common]" & _
                "   SET [mfrNameID] = " & newMfrNameID & _
                " WHERE [mfrID] = " & newMfrID & ";"
            dbAcc.SQLexe(strcmd)
            '
            '[STEP 5] LOG AND REPORT
            '
            If Not Me_Page Is Nothing Then
                'User Request
                logMfr(Me_Page, HttpContext.Current.Session("user.UserID"), newMfrID)
            Else
                'System Request
                logMfr(Me_Page, sysEnv.SysUserID, newMfrID)
            End If
            'WE'RE DONE! EXIT FUNCTION!!!
            Return newMfrID
        Else
            'MANUFACTURER WAS FOUND
            If retval.Length = 1 Then
                'Found Only One! Perfect Match! Yay!
                Return retval(0)
            Else
                'Found Multiple!... Crazy Panic Time!
                dbLog.logDebug("[procManufacturer] Looking for '" & MfrName & "' found " & retval.Length & " matches! Matched [mfrID]'s: " & retval.ToString)
                dbLog.logSys("[procManufacturer] Looking for '" & MfrName & "' found " & retval.Length & " matches!")
                Return sysErrors.ERR_NOTFOUND 'ERROR!!!
            End If
        End If
    End Function

    'Adds a name alias for an existing manufacturer
    'Returns a zero or negative number on failure to locate MfrName
    'Returns the MfrNameID of the new name record for manufacturer MfrID (positive integer) on success
    Public Function mfrAddAlias(ByVal existingMfrID As String, ByVal MfrName As String) As Int32
        'Create New Manufacturer Record (NAMES ALIAS TABLE)
        dbAcc.SQLexe( _
            "INSERT INTO [FriedParts].[dbo].[mfr-Names]" & _
            "           ([mfrID]" & _
            "           ,[mfrName])" & _
            "     VALUES (" & _
            "           " & existingMfrID & "," & _
            "           '" & Trim(MfrName) & "'" & _
            "           )")
        Dim dt As New DataTable
        Dim sqltxt As String = _
            "SELECT [mfrNameID]" & _
            "  FROM [FriedParts].[dbo].[mfr-Names]" & _
            "  WHERE [mfrID] = " & existingMfrID & _
            "      AND [mfrName] = '" & MfrName & "';"
        dbAcc.SelectRows(dt, sqltxt)
        Return dt.Rows(0).Field(Of Int32)(0)
    End Function

    '=========================================================================================
    '== [END] MANUFACTURER
    '=========================================================================================

    Public Sub logMfr(ByRef Me_Page As Page, ByVal UserID As Int16, ByVal MfrID As Int16)
        Dim sqlUndo As String = _
            "DELETE FROM [FriedParts].[dbo].[mfr-Names]" & _
            " WHERE [MfrID] = " & MfrID & vbCrLf & _
            "GO" & vbCrLf & _
            "DELETE FROM [FriedParts].[dbo].[mfr-Common]" & _
            " WHERE [MfrID] = " & MfrID & vbCrLf & _
            "GO" & vbCrLf
        logServiceActivity(Me_Page, UserID, suGetUserFirstName(UserID) & " added a new manufacturer, " & mfrGetName(MfrID), "", sqlUndo, "MfrID", MfrID, "MfrName", mfrGetName(MfrID), "MfrWebsite", mfrGetWebsite(MfrID))
    End Sub
End Module
