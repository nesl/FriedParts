Option Explicit On
Imports Microsoft.VisualBasic
Imports System.Net
Imports System.Data

Public Module fpDist

    '=========================================================================================
    '== DISTRIBUTOR
    '=========================================================================================

    'If exists returns an array of matching DistID's, Else returns -1
    'If EXACT = TRUE then only an exact match of DistName will match, otherwise *DistName* is matched
    'The returned array consists only of unique Distributor ID's 
    '   (in case it is found under multiple names -- likely -- as in "Digikey Corp.", "Digikey, Inc.", "Digikey").
    Public Function distExistsID(ByVal DistName As String, Optional ByVal Exact As Boolean = False) As Int32()
        Dim dt As New DataTable
        Dim retval() As Int32 = {-1}

        If Exact Then
            dt = dbAcc.SelectRows(dt, _
                "SELECT DISTINCT [FriedParts].[dbo].[dist-Common].[distID] " & _
                "FROM [FriedParts].[dbo].[dist-Common] INNER JOIN [FriedParts].[dbo].[dist-Names] ON [dist-Common].[distID] = [dist-Names].[distID] " & _
                "WHERE [FriedParts].[dbo].[dist-Names].[distName] = '" & Trim(DistName) & "';")
        Else
            dt = dbAcc.SelectRows(dt, _
                "SELECT DISTINCT [FriedParts].[dbo].[dist-Common].[distID] " & _
                "FROM [FriedParts].[dbo].[dist-Common] INNER JOIN [FriedParts].[dbo].[dist-Names] ON [dist-Common].[distID] = [dist-Names].[distID] " & _
                "WHERE [FriedParts].[dbo].[dist-Names].[distName] LIKE '%" & Trim(DistName) & "%';")
        End If
        If dt.Rows.Count > 0 Then
            'Found!
            Return fpUniqueID(dt, "distID")
        Else
            'Not Found!
            Return retval
        End If
    End Function

    'Checks for a UNIQUE match to your name-to-ID query. Returns sysErrors.ERR_NOTUNIQUE or sysErrors.ERR_NOTFOUND depending.
    'Be careful when specifying Exact = False, as this may match multiple records and hence result in sysErrors.ERR_NOTUNIQUE
    Public Function distExistsUID(ByVal DistName As String, Optional ByVal Exact As Boolean = True) As Int32
        Dim blah() As Int32
        blah = distExistsID(DistName, Exact)
        Select Case UBound(blah)
            Case 0
                'Only one return! Yay!
                Return blah(0) 'Covers the sysErrors.ERR_NOTFOUND case since that will be in the result
            Case Else
                Return sysErrors.ERR_NOTUNIQUE
        End Select
    End Function

    'Adds a new Distributor to the Database
    'Returns the DistID if found, or if added
    'Returns -1 if can't find or add (e.g. on a weird error condition)
    Public Function distAdd(ByVal DistName As String, Optional ByVal DistWebsite As String = "") As Int32
        Dim dt As New DataTable
        Dim retval As Int32() = existsDistID(DistName, True) 'use exact matching!
        If retval(0) = -1 Then
            'NOT FOUND! ADD THIS Distributor!
            'This is actually a bit of a tricky process
            '
            '[STEP 1] CREATE A NEW Distributor ID
            '
            dbLog.logDebug("[Add Dist] '" & DistName & "' Not Found! Adding!")
            Dim newDistID As Int32
            'Generate Temporary ID
            Dim tempUID As Int32 = fpCore.procGenTempUID()
            'Create New Distributor Record (COMMON)
            Dim strcmd As String = _
                "INSERT INTO [FriedParts].[dbo].[dist-Common]" & _
                "           ([distNameID]" & _
                "           ,[distWebsite])" & _
                "     VALUES (" & _
                "           " & tempUID & "," & _
                "           '" & DistWebsite & "'" & _
                "           )"
            'We set the mfrNameID to a unique negative number to mark it for error detection. In theory, there should never be any -1 records in the common table since every mfr-common entry corresponds to at least one mfr-name entry.
            dbAcc.SQLexe(strcmd)
            '
            '[STEP 2] FIND THE NEW DIST-ID WE JUST CREATED
            '
            dt = dbAcc.SelectRows(dt, _
                "SELECT [FriedParts].[dbo].[dist-Common].[distID] " & _
                "FROM [FriedParts].[dbo].[dist-Common] " & _
                "WHERE [FriedParts].[dbo].[dist-Common].[distNameID] = " & tempUID & ";")
            Select Case dt.Rows.Count
                Case 0
                    'Not found! ERROR!!! (should never happen)
                    dbLog.logSys(0, "[Add Dist] '" & DistName & "' Not Found! During Add Using Temp ID: " & tempUID)
                    Return -1 'ERROR!
                Case 1
                    'Yay! Perfect (this is exactly what should happen)
                    newDistID = dt.Rows(0).Field(Of Int32)(0) 'query only selected one row
                Case Else
                    'Found more than one!!! Oh No!!! (shouldn't happen)
                    dbLog.logSys(0, "[Add Mfr] '" & DistName & "' Matched multiple using Temp ID: " & tempUID)
                    Return -1 'ERROR!
            End Select
            '
            '[STEP 3] ADD THE NEW DIST-NAME RECORD
            '
            Dim newDistNameID As Int32 = addDistAlias(newDistID, DistName)
            '
            '[STEP 4] GO BACK AND ADD THE CORRECT RELATIONSHIP TO THE NEW NAME
            '
            strcmd = _
                "UPDATE [FriedParts].[dbo].[dist-Common]" & _
                "   SET [distNameID] = " & newDistNameID & _
                " WHERE [distID] = " & newDistID & ";"
            dbAcc.SQLexe(strcmd)
            'WE'RE DONE! EXIT FUNCTION!!!
            Return newDistID
        Else
            'Distributor WAS FOUND
            If retval.Length = 1 Then
                'Found Only One! Perfect Match! Yay!
                Return retval(0)
            Else
                'Found Multiple!... Crazy Panic Time!
                dbLog.logDebug("[procDistributor] Looking for '" & DistName & "' found " & retval.Length & " matches! Matched [mfrID]'s: " & retval.ToString)
                dbLog.logSys("[procDistributor] Looking for '" & DistName & "' found " & retval.Length & " matches!")
                Return -1 'ERROR!!!
            End If
        End If
    End Function

    'Adds a name alias for an existing Distributor
    'Returns a zero or negative number on failure to locate MfrName
    'Returns the MfrNameID of the new name record for Distributor MfrID (positive integer) on success
    Public Function distAddAlias(ByVal existingDistID As String, ByVal DistName As String) As Int32
        'Create New Distributor Record (NAMES ALIAS TABLE)
        dbAcc.SQLexe( _
            "INSERT INTO [FriedParts].[dbo].[dist-Names]" & _
            "           ([distID]" & _
            "           ,[distName])" & _
            "     VALUES (" & _
            "           " & existingDistID & "," & _
            "           '" & Trim(DistName) & "'" & _
            "           )")
        Dim dt As New DataTable
        Dim sqltxt As String = _
            "SELECT [distNameID]" & _
            "  FROM [FriedParts].[dbo].[dist-Names]" & _
            "  WHERE [distID] = " & existingDistID & _
            "      AND [distName] = '" & DistName & "';"
        dbAcc.SelectRows(dt, sqltxt)
        Return dt.Rows(0).Field(Of Int32)(0)
    End Function

    Public Sub distAddPart(ByVal DistID As Int32, ByVal PartNum As Int32, ByVal DistPartNum As String)
        Dim SqlPartAdd As String = _
        "INSERT INTO [FriedParts].[dbo].[dist-Parts]" & _
        "           ([PartID]" & _
        "           ,[DistID]" & _
        "           ,[DistPartNum])" & _
        "     VALUES (" & _
        "           '" & PartNum & "'," & _
        "           '" & DistID & "'," & _
        "           '" & sysText.txtDefangSQL(DistPartNum) & "'" & _
        "           )"
        dbAcc.SQLexe(SqlPartAdd)
    End Sub
    '=========================================================================================
    '== [END] DISTRIBUTOR
    '=========================================================================================

End Module
