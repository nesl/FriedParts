﻿Imports Microsoft.VisualBasic
Imports System.Data

'========================================================================
'= This code module implements all of the "business logic" for managing 
'= part categories and their properties.
'========================================================================

Public Module fpPartTypes
    ''' <summary>
    ''' Determines if a given TypeName with a parent ParentID (or ParentName) currently exists
    '''   Useful in determining whether to add it or not (use ptAddNewPartType to do that)
    '''  The optional parameter "foundTypeID" is the TypeID of this PartType if it exists, -1 if it does not or otherwise
    ''' Parent must exist or error results!
    ''' </summary>
    ''' <param name="TypeName"></param>
    ''' <param name="ParentID"></param>
    ''' <param name="foundTypeID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ptExistsPartType(ByVal TypeName As String, ByVal ParentID As Int32, Optional ByVal foundTypeID As Int32 = -2) As Boolean
        Dim DT As New DataTable
        Dim SQLcmd As String
        SQLcmd = _
            "SELECT [TypeID]" & _
            "  FROM [FriedParts].[dbo].[part-PartTypes] " & _
            "  WHERE [Type] = '" & sysText.txtDefangSQL(TypeName) & "' AND [Parent] = " & ParentID & ";"
        dbAcc.SelectRows(DT, SQLcmd)
        If DT.Rows.Count > 0 Then
            foundTypeID = DT.Rows(0).Field(Of Int32)("TypeID")
            Return True
        Else
            foundTypeID = -1
            Return False
        End If
    End Function

    'OVERLOADS. This one is dangerous because ParentName may not be unique and if so this will fail.
    '           Better to know the ParentID and use the original function
    'Determines if a given TypeName with a parent ParentName currently exists
    '   Useful in determining whether to add it or not (use ptAddNewPartType to do that)
    'Parent must exist or error results!
    Public Function ptExistsPartType(ByVal TypeName As String, ByVal ParentName As String) As Boolean
        'Search for Parent by Name
        Dim pArr As Int32(,)
        pArr = ptFindPartType(ParentName)
        Select Case UBound(pArr, 1)
            Case 0
                If pArr(0, 0) = -1 Then
                    'not found
                    Err.Raise(-666, , "Parent does not exist!")
                    Return False
                Else
                    'found only one. Yay!
                    'Don't need to do anything here, just fall through
                End If
            Case Else
                'found more than one. Oh no!
                'ERROR FOR NOW!!!
                Err.Raise(-666, , "Multiple Parents found. This is bad!")
                Return False
        End Select

        Return ptExistsPartType(TypeName, pArr(0, 0))
    End Function

    ''' <summary>
    ''' Overload. Allows you to add a new part with a parent, when you only know the parent's name (not id)
    ''' </summary>
    ''' <param name="newName"></param>
    ''' <param name="parentName"></param>
    ''' <param name="newNotes"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ptAddNewPartType(ByVal newName As String, ByVal parentName As String, Optional ByVal newNotes As String = "") As Int32
        Dim Parents As Int32(,)
        Parents = ptFindPartType(parentName)
        If UBound(Parents, 1) = 0 And Parents(0, 0) <> -1 Then
            Return ptAddNewPartType(newName, Parents(0, 0), newNotes)
        Else
            logDebug("[ptAddNewPArtType] ARGH! Parent Name resolved ambiguously. " & UBound(Parents, 1) & " matches. " & Parents.ToString)
            logSys(0, -1, "[ptAddNewPArtType] ARGH! Parent Name resolved ambiguously. " & UBound(Parents, 1) & " matches. " & Parents.ToString)
            Return -1
        End If
    End Function

    ''' <summary>
    ''' Add a new category; Does not check for duplicates!!! Do this yourself prior to calling me!
    ''' </summary>
    ''' <param name="newName"></param>
    ''' <param name="parentID"></param>
    ''' <param name="newNotes"></param>
    ''' <param name="Me_Page"></param>
    ''' <returns>Returns the new TypeID</returns>
    ''' <remarks></remarks>
    Public Function ptAddNewPartType(ByVal newName As String, ByVal parentID As Int32, Optional ByVal newNotes As String = "", Optional ByRef Me_Page As Page = Nothing) As Int32
        Dim dt As New DataTable

        '[STEP 0] Sanity Checks

        '[STEP 1] Create the new Category 
        Dim SQLcmd As String = _
        "INSERT INTO [FriedParts].[dbo].[part-PartTypes]" & _
        "           ([Parent]" & _
        "           ,[Path]" & _
        "           ,[Type]" & _
        "           ,[TypeNotes])" & _
        "     VALUES (" & _
        "           " & parentID & "," & _
        "           '" & "" & "'," & _
        "           '" & sysText.txtDefangSQL(newName) & "'," & _
        "           '" & sysText.txtDefangSQL(newNotes) & "'" & _
        "           )"
        dbAcc.SQLexe(SQLcmd)

        '[STEP 2] Find the TypeID that we just created
        SQLcmd = _
            "SELECT [TypeID]" & _
            "  FROM [FriedParts].[dbo].[part-PartTypes] " & _
            "  WHERE [Type] = '" & sysText.txtDefangSQL(newName) & "' AND [Parent] = " & parentID & ";"
        dbAcc.SelectRows(dt, SQLcmd)

        '[STEP 3] Update the TypeID record to include its path back to the root node
        If dt.Rows.Count = 1 Then
            'FOUND! 
            SQLcmd = _
                "UPDATE [FriedParts].[dbo].[part-PartTypes] " & _
                "   SET [Path] = '" & ptTraceLineage(dt.Rows(0).Field(Of Int32)("TypeID")) & "' " & _
                " WHERE [TypeID] = " & dt.Rows(0).Field(Of Int32)("TypeID") & ";"
            dbAcc.SQLexe(SQLcmd)
            'Log!
            logPartType(Me_Page, HttpContext.Current.Session("user.UserID"), dt.Rows(0).Field(Of Int32)("TypeID"))
            Return dt.Rows(0).Field(Of Int32)("TypeID")
        Else
            'ERROR! Could not find the record we just created!
            Err.Raise(-24, , "[ptAddNewPartType] Could not locate the new Part Type we just created!")
            dbLog.logSys(0, -1, "[ptAddNewPartType] Could not locate the new Part Type we just created!")
            dbLog.logDebug("[ptAddNewPartType] Could not locate the new Part Type we just created!")
            Return -1
        End If
    End Function

    'Finds the TypeID of the category with name "TypeName"
    '--ESSENTIALLY CONVERTS A TypeName INTO A TypeID
    'Returns the TypeID if found
    'Returns {-1,-1} if not found (array of one element, where first element = -1)
    'Returns {[id1, parent1],[id2, parent2], ..} if more than one is found (2-D array of (1st Dim) TypeID's and (2nd Dim) the Parents of those TypeID's)
    '   in effect, this is the list of all parents who have children named "TypeName"
    Public Function ptFindPartType(ByVal TypeName As String) As Int32(,)
        Dim dt As New DataTable
        Dim SQLcmd As String
        Dim inttoarray(,) As Int32 = {{-1}, {-1}}

        SQLcmd = _
            "SELECT [TypeID]" & _
            "      ,[Parent]" & _
            "  FROM [FriedParts].[dbo].[part-PartTypes] " & _
            "  WHERE [Type] = '" & TypeName & "';"
        dbAcc.SelectRows(dt, SQLcmd)
        If dt.Rows.Count = 0 Then
            'Not found! report -1
            Return inttoarray
        Else
            'found one or more
            ReDim inttoarray(dt.Rows.Count - 1, 1)
            Dim i As Int32 = 0
            For Each r As DataRow In dt.Rows
                inttoarray(i, 0) = r.Field(Of Int32)("TypeID")
                inttoarray(i, 1) = r.Field(Of Int32)("Parent")
            Next
            Return inttoarray
        End If
    End Function

    ''' <summary>
    ''' Returns TypeID of the PartType described by its name and parentID
    ''' CAUTION: Will crash (Array out of bounds exception) if no match is found! Assumes you tested for existence earlier
    '''           Use ptExistsPartType first!
    ''' Returns the FIRST matching PartType if multiple matches
    ''' </summary>
    ''' <param name="TypeName"></param>
    ''' <param name="ParentID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ptFindPartType(ByVal TypeName As String, ByVal ParentID As Int32) As Int32
        Dim tArr(,) As Int32 = ptFindPartType(TypeName)
        Dim i As Byte = 0
        Do Until (tArr(i, 1) = ParentID)
            i = i + 1
        Loop
        Return tArr(i, 0)
    End Function

    ''' <summary>
    ''' Returns the level of the specified TypeID. For example, "All Parts"(1)-->"Electronic Parts"(2)-->"Resistors"(3)-->"SMT"-->(4)
    ''' </summary>
    ''' <param name="TypeID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ptGetLevel(ByRef TypeID As Int32) As Byte
        Dim LevelArray As Integer() = ptPathSplit(ptGetPath(TypeID))
        Return LevelArray.Length
    End Function

    ''' <summary>
    ''' Returns this category's tree lineage from current node back up to the root of the tree.
    ''' </summary>
    ''' <param name="TypeID"></param>
    ''' <returns>ex. "0, 12, 34"</returns>
    ''' <remarks>This function is used to generate the Path values for a new Part Type. 
    ''' Requires two pieces of information: this part type's id and its parent's id.</remarks>
    Public Function ptTraceLineage(ByVal TypeID As Int32) As String
        Dim dt As New DataTable
        Dim SQLcmd As String
        'dbLog.logDebug("[ptTraceLineage] Entry with TypeID = " & TypeID) 'xxx
        'Check for terminating condition
        If TypeID = 0 Then
            'Found the root node! Yay!
            Return "/0/"
        Else
            'Lookup the current node
            '[STEP 2] Find the TypeID that we just created
            SQLcmd = _
                "SELECT [TypeID]" & _
                "      ,[Parent]" & _
                "  FROM [FriedParts].[dbo].[part-PartTypes] " & _
                "  WHERE [TypeID] = " & TypeID & ";"
            dbAcc.SelectRows(dt, SQLcmd)

            'dbLog.logDebug("[ptTraceLineage] Found " & dt.Rows.Count & " matches to search on TypeID " & TypeID) 'xxx

            If dt.Rows.Count <> 1 Then
                'ERROR! Could not find the parent of the last node!!!
                Err.Raise(-667, , "[ptTraceLineage] Could not find the parent of the last node!!!")
                dbLog.logSys(0, -1, "[ptTraceLineage] Could not find the parent of the last node!!!")
                dbLog.logDebug("[ptTraceLineage] Could not find the parent of the last node!!!")
                Return -1
            Else
                Return "" & ptTraceLineage(dt.Rows(0).Field(Of Int32)("Parent")) & TypeID & "/"
            End If
        End If
    End Function


    ''' <summary>
    ''' Converts the Path descriptor to an array of TypeID's
    ''' </summary>
    ''' <param name="PartTypePath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ptPathSplit(ByVal PartTypePath As String) As Int32()
        Dim str() As String = PartTypePath.Split("/", 20, System.StringSplitOptions.RemoveEmptyEntries)
        Dim ia() As Int32
        If str.Length > 0 Then
            ReDim ia(str.Length - 1)
            Dim c As Byte = 0
            For Each s In str
                ia(c) = CInt(s)
                c = c + 1
            Next
            Return ia
            Exit Function 'not necessary, but just to be SURE...
        End If
        Return ia
    End Function

    ''' <summary>
    ''' Find the complete set of TypeID's in this TypeID's path (Including it)
    ''' </summary>
    ''' <param name="PartTypeID"></param>
    ''' <returns>Ex. "0, 12, 34"</returns>
    ''' <remarks>Caution! This splits the path DOWN TO this type (e.g. it's parents).
    ''' To see its *children* types use ptGetChildrenTypes</remarks>
    Public Function ptGetParentTypes(ByVal PartTypeID As Int32) As Int32()
        Return ptPathSplit(ptGetPath(PartTypeID))
    End Function

    ''' <summary>
    ''' Find the complete set of TypeID's in this TypeID's path (Including it)
    ''' </summary>
    ''' <param name="PartTypeID"></param>
    ''' <returns>Ex. "0, 12, 34"</returns>
    ''' <remarks>Caution! This splits the path UP TO this type (e.g. it's children).
    ''' To see its *parent* types use ptGetParentTypes</remarks>
    Public Function ptGetChildrenTypes(ByVal PartTypeID As Int32) As Int32()
        Dim dt As New DataTable
        Dim sqltxt As String = _
            "SELECT [TypeID],[Parent],[Path] " & _
            "FROM [FriedParts].[dbo].[part-PartTypes] " & _
            "WHERE [Path] LIKE %/" & PartTypeID & "/%"
        SelectRows(dt, sqltxt)
        Dim ia As New ArrayList
        For Each dr As DataRow In dt.Rows
            ia.Add(dr.Field(Of Int32)("TypeID"))
        Next
        Return ia.ToArray(GetType(Int32))
    End Function

    ''' <summary>
    ''' Returns the PartType Name given its ID
    ''' </summary>
    ''' <param name="TypeID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ptGetTypeName(ByVal TypeID As Int16) As String
        Dim dt As New DataTable
        dt = SelectRows(dt, "SELECT [Type] FROM [FriedParts].[dbo].[part-PartTypes] WHERE [TypeID] = " & TypeID)
        If dt.Rows.Count = 1 Then
            Return dt.Rows(0).Field(Of String)("Type")
        Else
            Err.Raise(-543, , "TypeID not found!")
            Return "" 'Silence Compiler Warning
        End If
    End Function

    ''' <summary>
    ''' Returns the ParentPartTypeID given it the Child Part Type ID
    ''' </summary>
    ''' <param name="TypeID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ptGetParentTypeID(ByVal TypeID As Int32) As Int32
        Dim dt As New DataTable
        dt = SelectRows(dt, "SELECT [Parent] FROM [FriedParts].[dbo].[part-PartTypes] WHERE [TypeID] = " & TypeID)
        If dt.Rows.Count = 1 Then
            Return dt.Rows(0).Field(Of Int32)("Parent")
        Else
            Err.Raise(-543, , "TypeID not found!")
        End If
    End Function

    ''' <summary>
    ''' Returns the ParentPartType Name given it the Child Part Type ID
    ''' </summary>
    ''' <param name="TypeID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ptGetParentTypeName(ByVal TypeID As Int32) As String
        Dim dt As New DataTable
        dt = SelectRows(dt, "SELECT [Parent] FROM [FriedParts].[dbo].[part-PartTypes] WHERE [TypeID] = " & TypeID)
        If dt.Rows.Count = 1 Then
            Return ptGetTypeName(dt.Rows(0).Field(Of Int16)("Parent"))
        Else
            Err.Raise(-543, , "TypeID not found!")
            Return "" 'For compiler warning
        End If
    End Function

    ''' <summary>
    ''' Returns the PartType's Value Label and Description via passed in parameters
    ''' </summary>
    ''' <param name="PartTypeID"></param>
    ''' <param name="TypeValue"></param>
    ''' <param name="TypeValueNotes"></param>
    ''' <param name="TypeValueNumeric"></param>
    ''' <param name="TypeUnits"></param>
    ''' <returns>Returns True if found / False otherwise</returns>
    ''' <remarks></remarks>
    Public Function ptGetTypeValue(ByVal PartTypeID As String, ByRef TypeValue As String, ByRef TypeValueNotes As String, ByRef TypeValueNumeric As Boolean, ByRef TypeUnits As String) As Boolean
        Dim thePath As String
        If (ptGetTheTypeValue(PartTypeID, TypeValue, thePath)) Then
            Dim TypeIDsInPath() As Int32 = ptPathSplit(thePath)
            Dim i As Byte
            Dim ptLbl As String
            For i = 0 To TypeIDsInPath.Length - 1
                If ptGetTheTypeValue(TypeIDsInPath(i), ptLbl) Then
                    If (Not ptLbl Is Nothing) Then
                        'Found a defined category!
                        Dim dt As New DataTable
                        Dim SQLcmd As String
                        SQLcmd = _
                            "SELECT [TypeID]" & _
                            "      ,[TypeValueNotes]" & _
                            "      ,[TypeValueNumeric]" & _
                            "      ,[TypeUnits]" & _
                            "  FROM [FriedParts].[dbo].[part-PartTypes] " & _
                            "  WHERE [TypeID] = " & TypeIDsInPath(i) & ";"
                        dbAcc.SelectRows(dt, SQLcmd)
                        TypeValue = ptLbl
                        TypeValueNotes = txtNullToEmpty(dt.Rows(0).Field(Of String)("TypeValueNotes"))
                        TypeValueNumeric = dt.Rows(0).Field(Of Boolean)("TypeValueNumeric")
                        TypeUnits = txtNullToEmpty(dt.Rows(0).Field(Of String)("TypeUnits"))
                    Else
                        'Lbl is null -- no value defined at this level
                        'Next!
                    End If
                End If
            Next
            Return True
        Else
            'PartTypeID not found!
            Err.Raise(978432, , "PartTypeId not found!")
            Return False
        End If
    End Function

    'Returns the PartType's Value Label and Description via passed in parameters
    'Returns True if found / False otherwise
    Private Function ptGetTheTypeValue(ByVal PartTypeID As String, ByRef TypeValue As String, Optional ByRef PartTypePath As String = "") As Boolean
        Dim dt As New DataTable
        Dim SQLcmd As String
        'Lookup the current node
        '[STEP 2] Find the TypeID that we just created
        SQLcmd = _
            "SELECT [TypeID]" & _
            "      ,[TypeValue]" & _
            "      ,[Path]" & _
            "  FROM [FriedParts].[dbo].[part-PartTypes] " & _
            "  WHERE [TypeID] = " & PartTypeID & ";"
        dbAcc.SelectRows(dt, SQLcmd)

        If dt.Rows.Count <> 1 Then
            'ERROR! Could not find the parent of the last node!!!
            Err.Raise(-667, , "[ptTraceLineage] Could not find the parent of the last node!!!")
            dbLog.logSys(0, -1, "[ptTraceLineage] Could not find the parent of the last node!!!")
            dbLog.logDebug("[ptTraceLineage] Could not find the parent of the last node!!!")
            Return False
        Else
            TypeValue = dt.Rows(0).Field(Of String)("TypeValue")
            PartTypePath = dt.Rows(0).Field(Of String)("Path")
            Return True
        End If
    End Function

    ''' <summary>
    ''' Returns the TypeID which has the specified Part Type Path -- e.g. "/0/206/114/"
    ''' </summary>
    ''' <param name="PartTypePath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ptGetTypeIDFromPath(ByVal PartTypePath As String) As Int32
        Dim dt As New DataTable
        Dim SQLcmd As String
        'Lookup the current node
        '[STEP 2] Find the TypeID that we just created
        SQLcmd = _
            "SELECT [TypeID]" & _
            "  FROM [FriedParts].[dbo].[part-PartTypes] " & _
            "  WHERE [Path] = '" & sysText.txtDefangSQL(PartTypePath) & "';"
        dbAcc.SelectRows(dt, SQLcmd)
        If dt.Rows.Count = 1 Then
            Return dt.Rows(0).Field(Of Int32)("TypeID")
        Else
            If dt.Rows.Count = 0 Then
                Err.Raise(-34559, , "PartTypePath NOT FOUND!")
            Else
                Err.Raise(-978432, , "Multiple Matching Paths! Not allowed!")
            End If
            Return -1 'Just to silence compiler warning
        End If
    End Function

    ''' <summary>
    ''' Returns the Path, in TypeID's, of the specified PartTypeID (ex. "/0/22/24/")
    ''' </summary>
    ''' <param name="PartTypeID">FriedParts Part Type ID</param>
    ''' <returns>The Path as stored in the database</returns>
    ''' <remarks>Use ptGetCompleteName(TypeID) for the human readable version.</remarks>
    Public Function ptGetPath(ByVal PartTypeID As Int32) As String
        Dim dt As New DataTable
        Dim SQLcmd As String
        'Lookup the current node
        '[STEP 2] Find the TypeID that we just created
        SQLcmd = _
            "SELECT [Path]" & _
            "  FROM [FriedParts].[dbo].[part-PartTypes] " & _
            "  WHERE [TypeID] = " & PartTypeID & ";"
        dbAcc.SelectRows(dt, SQLcmd)
        If dt.Rows.Count = 1 Then
            Return dt.Rows(0).Field(Of String)("Path")
        Else
            Err.Raise(-34560, , "PartTypeID NOT FOUND!")
            Return -1 'Just to silence compiler warning
        End If
    End Function

    ''' <summary>
    ''' Converts the type to a string sequence of names following its full path 
    ''' using the format: TypeName Delimiter TypeName Delimiter TypeName ...
    ''' </summary>
    ''' <param name="PartTypeID"></param>
    ''' <param name="Delimiter">The text that separates successive Part Types in the Path. 
    ''' Automatically padded by spaces, so don't include them (e.g. "->"), but if you do 
    ''' they will be respected (we don't Trim() inside here)</param>
    ''' <returns></returns>
    ''' <remarks>Effectively the toString function for this TypeID.</remarks>
    Public Function ptGetCompleteName(ByRef PartTypeID As Int32, Optional ByRef Delimiter As String = "->") As String
        Dim path As String = ptGetPath(PartTypeID)
        Dim ia As Int32() = ptPathSplit(path)
        Dim outStr As String = ptGetTypeName(sysEnv.sysPARTTYPEROOT)
        If UBound(ia) > 0 Then
            For i As Byte = 1 To UBound(ia)
                outStr = outStr & " " & Delimiter & " " & ptGetTypeName(ia(i))
            Next
        End If
        Return outStr
    End Function

    ''' <summary>
    ''' Logging function for UNDO and recovery, also writes to user activity log
    ''' </summary>
    ''' <param name="Me_Page"></param>
    ''' <param name="UserID"></param>
    ''' <param name="PartTypeID"></param>
    ''' <remarks></remarks>
    Public Sub logPartType(ByRef Me_Page As Page, ByVal UserID As Int16, ByVal PartTypeID As Int16)
        Dim sqlDo As String = _
            "INSERT INTO [FriedParts].[dbo].[part-PartTypes]" & _
            "           ([Parent]" & _
            "           ,[Path]" & _
            "           ,[Type]" & _
            "           ,[Notes])" & _
            "     VALUES (" & _
            "           " & ptGetParentTypeID(PartTypeID) & "," & _
            "           '" & ptTraceLineage(PartTypeID) & " '," & _
            "           '" & ptGetTypeName(PartTypeID) & "'," & _
            "           'REDO code created " & Now & "'" & _
            "           )"
        Dim sqlUndo As String = _
            "DELETE FROM [FriedParts].[dbo].[part-PartTypes]" & _
            " WHERE [TypeID] = " & PartTypeID
        logServiceActivity(Me_Page, UserID, suGetUserFirstName(UserID) & " added a new part type, " & ptGetTypeName(ptGetParentTypeID(PartTypeID)) & "/" & ptGetTypeName(PartTypeID), sqlDo, sqlUndo, "TypeID", PartTypeID, "TypeName", ptGetTypeName(PartTypeID))
    End Sub

    ''' <summary>
    ''' Finds all of the parts that are of the same type as the TypeID specified.
    ''' This is a bit more complex than it might first appear, since this includes
    ''' not just the parts of this exact type, BUT THE CHILDREN TYPES as well.
    ''' 
    ''' Ex. The "Capacitors" type includes all the parts of type "Ceramic" too!
    ''' </summary>
    ''' <param name="PartTypeID"></param>
    ''' <param name="SearchResultsTable">If you already have a sub-set of records you 
    ''' want to use as the basis (e.g. search results), pass it in here. If omitted,
    ''' we assume the set of all parts and work with that.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ptGetPartsOfThisType(ByVal PartTypeID As Int32, Optional ByRef SearchResultsTable As DataTable = Nothing) As DataTable
        'Define our source of records to work from
        Dim source As DataTable
        If SearchResultsTable Is Nothing Then
            source = New DataTable
            SelectRows(source, "SELECT * FROM [FriedParts].[dbo].[view-part] WHERE [TypePath]")
        Else
            source = SearchResultsTable
        End If
        'Init
        Dim retSrc As DataTable
        retSrc = source.Clone 'Copy the schema from source to retsrc
        'Find the records
        For Each dr As DataRow In source.Select("[TypePath] Like '%/" & PartTypeID & "/%'")
            retSrc.Rows.Add(dr.ItemArray())
        Next
        'Return
        Return retSrc
    End Function

End Module