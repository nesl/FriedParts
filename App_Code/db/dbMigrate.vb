Imports Microsoft.VisualBasic
Imports System.Data

'DATA MIGRATION FUNCTIONALITY -- ALL CODE NECESSARY OR NECESSARILY USED DURING THE TRANSITION
'   WORKS AT SQL LEVEL (use dbAcc for lower level data access)

Public Module dbMigrate
    'Processes one OLD_DATA record (pass it in as dr)
    Public Sub procOLD(ByVal dr As DataRow)
        Dim recMfrID As Int32
        Dim recDistID As Int32
        Dim recPartID As Int32
        Dim recPartTypeID As Int32

        'PartID
        recPartID = dr.Field(Of Int32)("PartID")

        'Add Mfr if not exist, otherwise find this manufacturer's Unique ID
        recMfrID = procManufacturer(Trim(dr.Field(Of String)("Mfr_Name")))

        'Add Dist if not exist, otherwise find this distributor's Unique ID
        recDistID = procDistributor(Trim(dr.Field(Of String)("Distributor")))

        'Deal with Part Type!
        Dim token As String
        Dim oldPT As String
        Dim foundID As Int32

        oldPT = dr.Field(Of String)("Part_Type")
        If oldPT = "" Then
            oldPT = "UNKNOWN"
        End If

        'xxx
        If oldPT = "Integrated Circuits (ICs)\FPGAs (Field Programmable Gate Array)" Then
            Dim blah As String
            blah = "Use this processing to allow a break-on-condition"
        End If

        'Major
        token = txtGetToken(oldPT, "\")
        If Not ptExistsPartType(token, 0, recPartTypeID) Then
            recPartTypeID = ptAddNewPartType(token, 0)
        End If
        'Minor
        token = txtGetToken("", "\")
        If token <> "" Then
            If Not ptExistsPartType(token, recPartTypeID, foundID) Then
                recPartTypeID = ptAddNewPartType(token, recPartTypeID)
            Else
                'We can't do this in one step because ptExistsPartType(token, recPartTypeID, recPartTypeID) will assign the byRef before the byVal erasing the intended passed-in pre-existing value of recPartTypeID
                recPartTypeID = foundID
            End If
            'Sub
            token = txtGetToken("", "\")
            If token <> "" Then
                If Not ptExistsPartType(token, recPartTypeID, foundID) Then
                    recPartTypeID = ptAddNewPartType(token, recPartTypeID)
                Else
                    recPartTypeID = foundID
                End If
                'Family
                token = txtGetToken("", "\")
                If token <> "" Then
                    If Not ptExistsPartType(token, recPartTypeID, foundID) Then
                        recPartTypeID = ptAddNewPartType(token, recPartTypeID)
                    Else
                        recPartTypeID = foundID
                    End If
                End If 'Family
            End If 'Sub
        End If 'Minor

        Dim SQLcmd As String = _
            "INSERT INTO [FriedParts].[dbo].[part-Common]" & _
            "           ([PartID]" & _
            "           ,[TypeID]" & _
            "           ,[Description]" & _
            "           ,[Extra_Description]" & _
            "           ,[Value]" & _
            "           ,[mfrID]" & _
            "           ,[mfrPartNum]" & _
            "           ,[URL_Datasheet]" & _
            "           ,[Date_Created]" & _
            "           ,[Date_LastModified]" & _
            "           ,[Verified])" & _
            "     VALUES (" & _
            "           '" & recPartID & "'," & _
            "           '" & recPartTypeID & "'," & _
            "           '" & txtDefangSQL(dr.Field(Of String)("Description")) & "'," & _
            "           '" & txtDefangSQL(dr.Field(Of String)("Extra_Description")) & "'," & _
            "           '" & txtDefangSQL(dr.Field(Of String)("Value")) & "'," & _
            "           '" & recMfrID & "'," & _
            "           '" & txtDefangSQL(dr.Field(Of String)("Mfr_Part_Number")) & "'," & _
            "           '" & txtDefangSQL(dr.Field(Of String)("URL")) & "'," & _
            "           " & procDate(dr) & "," & _
            "           " & txtSQLDate(Now) & "," & _
            "           " & sqlFALSE & _
            "           )"
        dbAcc.SQLexe(SQLcmd)

        'Distributor Part Number
        SQLcmd = _
            "INSERT INTO [FriedParts].[dbo].[dist-Parts]" & _
            "           ([PartID]" & _
            "           ,[DistID]" & _
            "           ,[DistPartNum])" & _
            "     VALUES (" & _
            "           '" & recPartID & "'," & _
            "           '" & recDistID & "'," & _
            "           '" & dr.Field(Of String)("Distributor_Part_Number") & "'" & _
            "           )"
        dbAcc.SQLexe(SQLcmd)

        'Altium CADD!
        migrateALTIUM(dr) 'This should work

    End Sub

    'Takes is a datarow of OLD_DATA and returns the "Last_Auto_Update" in SQL Date (as string) format
    Private Function procDate(ByVal dr As DataRow) As String
        Dim str As String
        Try
            str = txtSQLDate(dr.Field(Of Date)("Last_Auto_Update"))
            If str Is System.DBNull.Value Or str = "" Or Len(str) = 0 Then
                str = txtSQLDate(Now)
            End If
        Catch ex As Exception
            'Err.Raise(-42, , ex.Message)
            str = txtSQLDate(Now)
        End Try
        Return str
    End Function

    Private Sub migrateALTIUM(ByRef dr As DataRow)
        If Not dbMigrate.existsPartID("cad-Altium", dr.Field(Of Int32)("PartID")) Then
            Dim addString As String
            addString = _
            "INSERT INTO [FriedParts].[dbo].[cad-Altium]" & _
            "           ([PartID]" & _
            "           ,[Library Ref]" & _
            "           ,[Library Path]" & _
            "           ,[Library Ref 2]" & _
            "           ,[Library Path 2]" & _
            "           ,[Library Ref 3]" & _
            "           ,[Library Path 3]" & _
            "           ,[Footprint Ref]" & _
            "           ,[Footprint Path]" & _
            "           ,[Footprint Ref 2]" & _
            "           ,[Footprint Path 2]" & _
            "           ,[Footprint Ref 3]" & _
            "           ,[Footprint Path 3])" & _
            "     VALUES (" & _
            "           '" & dr.Field(Of Int32)("PartID") & "'," & _
            "           '" & dr.Field(Of String)("Library Ref") & "'," & _
            "           '" & dr.Field(Of String)("Library Path") & "'," & _
            "           '" & dr.Field(Of String)("Library Ref 2") & "'," & _
            "           '" & dr.Field(Of String)("Library Path 2") & "'," & _
            "           '" & dr.Field(Of String)("Library Ref 3") & "'," & _
            "           '" & dr.Field(Of String)("Library Path 3") & "'," & _
            "           '" & dr.Field(Of String)("Footprint Ref") & "'," & _
            "           '" & dr.Field(Of String)("Footprint Path") & "'," & _
            "           '" & dr.Field(Of String)("Footprint Ref 2") & "'," & _
            "           '" & dr.Field(Of String)("Footprint Path 2") & "'," & _
            "           '" & dr.Field(Of String)("Footprint Ref 3") & "'," & _
            "           '" & dr.Field(Of String)("Footprint Path 3") & "'" & _
            "           )"
            dbAcc.SQLexe(addString)
        Else
            'PartID already exists in table... report error (and move to next record)
            dbLog.logSys(0, ERR_DB_MIGRATION, "PartID (" & dr.Field(Of Int32)("PartID") & ") already exists in table [cad.Altium]")
        End If
    End Sub

    'Checks table "TableName" for the PartID specified by "PID"
    'If exists returns true, else false
    'PID input is marked String for compatibility reasons (most likely it is an unsigned integer)
    Public Function existsPartID(ByVal TableName As String, ByVal PID As String) As Boolean
        Dim dt As New DataTable
        dt = dbAcc.SelectRows(dt, "SELECT [PartID] FROM [" & TableName & "] WHERE [PartID]=" & PID)
        If dt.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function





    '=========================================================================================
    '== DISTRIBUTOR
    '=========================================================================================

    'If exists returns an array of matching DistID's, Else returns -1
    'If EXACT = TRUE then only an exact match of DistName will match, otherwise *DistName* is matched
    'The returned array consists only of unique Distributor ID's 
    '   (in case it is found under multiple names -- likely -- as in "Digikey Corp.", "Digikey, Inc.", "Digikey").
    Public Function existsDistID(ByVal DistName As String, Optional ByVal Exact As Boolean = False) As Int32()
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

    'Adds a new Distributor to the Database
    'Returns the DistID if found, or if added
    'Returns -1 if can't find or add (e.g. on a weird error condition)
    Public Function procDistributor(ByVal DistName As String, Optional ByVal DistWebsite As String = "") As Int32
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
            Dim tempUID As Int32 = procGenTempUID()
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
    Public Function addDistAlias(ByVal existingDistID As String, ByVal DistName As String) As Int32
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

    '=========================================================================================
    '== [END] DISTRIBUTOR
    '=========================================================================================










    '=========================================================================================
    '== MANUFACTURER
    '=========================================================================================

    'If exists returns an array of matching MfrID's, Else returns -1
    'The returned array consists only of unique manufacturer ID's 
    '   (in case it is found under multiple names -- likely -- as in "Panasonic ECG", "Panasonic, Inc.", "Panasonic Corporation").
    Public Function existsMfrID(ByVal MfrName As String, Optional ByVal Exact As Boolean = False) As Int32()
        Dim dt As New DataTable
        Dim retval() As Int32 = {-1}
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

    'Generate a unique negative number within the int32 number space
    'If this function returns the same number for two different users at the same time BAD THINGS WILL HAPPEN!
    Private Function procGenTempUID() As Int32
        Dim r As New Random(Now.Millisecond)
        Return (-1 * (Now.Ticks Mod (2 ^ 32 / 2 - 1)) + r.Next()) Mod (2 ^ 32 / 2 - 1)
    End Function

    'Adds a new Manufacturer to the Database
    'Returns the MfrID if found, or if added
    'Returns -1 if can't find or add (e.g. on a weird error condition)
    Public Function procManufacturer(ByVal MfrName As String, Optional ByVal MfrWebsite As String = "") As Int32
        Dim dt As New DataTable
        Dim retval As Int32() = existsMfrID(MfrName, True) 'Use exact matching
        If retval(0) = -1 Then
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
                    Return -1 'ERROR!
                Case 1
                    'Yay! Perfect (this is exactly what should happen)
                    newMfrID = dt.Rows(0).Field(Of Int32)(0) 'query only selected one row
                Case Else
                    'Found more than one!!! Oh No!!! (shouldn't happen)
                    dbLog.logSys(0, "[Add Mfr] '" & MfrName & "' Matched multiple using Temp ID: " & tempUID)
                    Return -1 'ERROR!
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
                Return -1 'ERROR!!!
            End If
        End If
    End Function

    'Adds a name alias for an existing manufacturer
    'Returns a zero or negative number on failure to locate MfrName
    'Returns the MfrNameID of the new name record for manufacturer MfrID (positive integer) on success
    Public Function addMfrAlias(ByVal existingMfrID As String, ByVal MfrName As String) As Int32
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

    'Removes Whitespace from the beginning/ends of all variable-length fields. This is necessary 
    'because I didn't understand SQL Server data type nomenclature when I first imported the data
    'so now I need to clean it up (originally was imported into fixed-width fields).
    Public Sub trimAltiumTable()

        Dim dtOld As New DataTable
        dtOld = dbAcc.SelectRows(dtOld, "SELECT * FROM [FriedParts].[dbo].[cad-Altium];")

        '[RUN]
        Dim i As Integer
        Dim dr As DataRow
        Dim SQLcmd As String
        For i = 0 To dtOld.Rows.Count - 1
            If i Mod 10 = 0 Then
                dbLog.logDebug("[trimAltium]: Processing: " & i)
            End If
            dr = dtOld.Rows(i)
            SQLcmd = _
                "UPDATE [FriedParts].[dbo].[cad-Altium]" & _
                "   SET " & _
                "       [Library Ref] = '" & Trim(dr.Field(Of String)("Library Ref")) & "'" & _
                "      ,[Library Path] = '" & Trim(dr.Field(Of String)("Library Path")) & "'" & _
                "      ,[Symbol_Desc] = '" & Trim(dr.Field(Of String)("Symbol_Desc")) & "'" & _
                "      ,[Symbol_Desc2] = '" & Trim(dr.Field(Of String)("Symbol_Desc2")) & "'" & _
                "      ,[Symbol_Desc3] = '" & Trim(dr.Field(Of String)("Symbol_Desc3")) & "'" & _
                "      ,[PCB3D Ref] = '" & Trim(dr.Field(Of String)("PCB3D Ref")) & "'" & _
                "      ,[PCB3D Path] = '" & Trim(dr.Field(Of String)("PCB3D Path")) & "'" & _
                "      ,[[PCB3D_Desc] = '" & Trim(dr.Field(Of String)("[PCB3D_Desc")) & "'" & _
                "      ,[PCB3D Ref 2] = '" & Trim(dr.Field(Of String)("PCB3D Ref 2")) & "'" & _
                "      ,[PCB3D Path 2] = '" & Trim(dr.Field(Of String)("PCB3D Path 2")) & "'" & _
                "      ,[PCB3D_Desc2] = '" & Trim(dr.Field(Of String)("PCB3D_Desc2")) & "'" & _
                "      ,[PCB3D Ref 3] = '" & Trim(dr.Field(Of String)("PCB3D Ref 3")) & "'" & _
                "      ,[PCB3D Path 3] = '" & Trim(dr.Field(Of String)("PCB3D Path 3")) & "'" & _
                "      ,[PCB3D_Desc3] = '" & Trim(dr.Field(Of String)("PCB3D_Desc3")) & "'" & _
                "      ,[Footprint Ref] = '" & Trim(dr.Field(Of String)("Footprint Ref")) & "'" & _
                "      ,[Footprint Path] = '" & Trim(dr.Field(Of String)("Footprint Path")) & "'" & _
                "      ,[Footprint_Desc] = '" & Trim(dr.Field(Of String)("Footprint_Desc")) & "'" & _
                "      ,[Footprint Ref 2] = '" & Trim(dr.Field(Of String)("Footprint Ref 2")) & "'" & _
                "      ,[Footprint Path 2] = '" & Trim(dr.Field(Of String)("Footprint Path 2")) & "'" & _
                "      ,[Footprint_Desc2] = '" & Trim(dr.Field(Of String)("Footprint_Desc2")) & "'" & _
                "      ,[Footprint Ref 3] = '" & Trim(dr.Field(Of String)("Footprint Ref 3")) & "'" & _
                "      ,[Footprint Path 3] = '" & Trim(dr.Field(Of String)("Footprint Path 3")) & "'" & _
                "      ,[Footprint_Desc3] = '" & Trim(dr.Field(Of String)("Footprint_Desc3")) & "'" & _
                "   WHERE [PartID] = " & dr.Field(Of Int32)("PartID")
            dbAcc.SQLexe(SQLcmd)
        Next i
    End Sub

End Module 'THE END OF dbMigrate!!!

