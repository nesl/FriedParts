Option Explicit On
Imports Microsoft.VisualBasic
Imports System.Data

Public Module fpParts

    ''' <summary>
    ''' Returns the SQL command string to execute the part add
    ''' </summary>
    ''' <param name="MEmfrID">The FriedParts Manufacturer ID</param>
    ''' <param name="mfrPartNum">The Manufacturer's Part Number</param>
    ''' <param name="TypeID">The FriedParts ID for this Part Type (e.g. Capacitors --> Ceramic)</param>
    ''' <param name="PkgTypeID">This feature isn't implemented yet...</param>
    ''' <param name="Desc">Summary (short) description</param>
    ''' <param name="Extra_Desc">Extended (long) description</param>
    ''' <param name="Value">The part's value</param>
    ''' <param name="Value_Units"></param>
    ''' <param name="Value_Tolerance"></param>
    ''' <param name="URL_Image"></param>
    ''' <param name="URL_Datasheet"></param>
    ''' <param name="DistID"></param>
    ''' <param name="DistPartNum"></param>
    ''' <param name="CaddLibRef"></param>
    ''' <param name="CaddLibPath"></param>
    ''' <param name="CaddFootRef"></param>
    ''' <param name="CaddFootPath"></param>
    ''' <param name="Verified"></param>
    ''' <param name="Obsolete"></param>
    ''' <param name="RoHS"></param>
    ''' <param name="Temp_Max"></param>
    ''' <param name="Temp_Min"></param>
    ''' <returns>A SQL query which may be executed on a the database to effect the addition of the specified part</returns>
    ''' <remarks></remarks>
    Public Function partAddSql(ByVal MEmfrID As Int32, ByVal mfrPartNum As String, ByVal TypeID As Int16, ByVal PkgTypeID As Int16, _
                            ByVal Desc As String, ByVal Extra_Desc As String, _
                            ByVal Value As String, ByVal Value_Units As String, ByVal Value_Tolerance As String, _
                            ByVal URL_Image As String, ByVal URL_Datasheet As String, _
                            ByVal DistID As Int32, ByVal DistPartNum As String, _
                            ByVal CaddLibRef As String, ByVal CaddLibPath As String, ByVal CaddFootRef As String, ByVal CaddFootPath As String, _
                            Optional ByVal Verified As Boolean = False, Optional ByVal Obsolete As Boolean = False, _
                            Optional ByVal RoHS As Boolean = True, Optional ByVal Temp_Max As Int16 = 0, Optional ByVal Temp_Min As Int16 = 0) As String
        'Deal with Value Terms
        Dim Value_Terms_Fields As String
        Dim Value_Terms_Values As String
        If Value = "" Then
            Value_Terms_Fields = ""
            Value_Terms_Values = ""
        Else
            Value_Terms_Fields = _
                "           ,[Value]" & _
                "           ,[Value_Units]" & _
                "           ,[Value_Tolerance]"
            Value_Terms_Values = _
                "           '" & txtDefangSQL(Value) & "'," & _
                "           '" & txtDefangSQL(Value_Units) & "'," & _
                "           '" & txtDefangSQL(Value_Tolerance) & "',"
        End If

        'Create Query
        Dim SqlPartAdd As String = _
        "INSERT INTO [FriedParts].[dbo].[part-Common]" & _
        "           ([TypeID]" & _
        "           ,[PkgTypeID]" & _
        "           ,[Description]" & _
        "           ,[Extra_Description]" & _
        "           ,[mfrID]" & _
        "           ,[mfrPartNum]" & Value_Terms_Fields & _
        "           ,[URL_Image]" & _
        "           ,[URL_Datasheet]" & _
        "           ,[Date_Created]" & _
        "           ,[Date_LastModified]" & _
        "           ,[Verified]" & _
        "           ,[Obsolete]" & _
        "           ,[RoHS]" & _
        "           ,[Temp_Max]" & _
        "           ,[Temp_Min]" & _
        "           ,[OwnerID])" & _
        "     VALUES (" & _
        "           " & TypeID & "," & _
        "           " & PkgTypeID & "," & _
        "           '" & txtDefangSQL(Desc) & "'," & _
        "           '" & txtDefangSQL(Extra_Desc) & "'," & _
        "           '" & MEmfrID & "'," & _
        "           '" & txtDefangSQL(mfrPartNum) & "'," & Value_Terms_Values & _
        "           '" & txtDefangSQL(URL_Image) & "'," & _
        "           '" & txtDefangSQL(URL_Datasheet) & "'," & _
        "           '" & Now & "'," & _
        "           '" & Now & "'," & _
        "           '" & Verified & "'," & _
        "           '" & Obsolete & "'," & _
        "           '" & RoHS & "'," & _
        "           '" & Temp_Max & "'," & _
        "           '" & Temp_Min & "'," & _
        "           " & HttpContext.Current.Session("user.UserID") & _
        "           )"
        Return SqlPartAdd
    End Function

    ''' <summary>
    ''' Adds new part and returns its PartID. This is the main entry point for adding a new part!
    ''' </summary>
    ''' <param name="MEmfrID"></param>
    ''' <param name="mfrPartNum"></param>
    ''' <param name="TypeID"></param>
    ''' <param name="PkgTypeID"></param>
    ''' <param name="Desc"></param>
    ''' <param name="Extra_Desc"></param>
    ''' <param name="Value"></param>
    ''' <param name="Value_Units"></param>
    ''' <param name="Value_Tolerance"></param>
    ''' <param name="URL_Image"></param>
    ''' <param name="URL_Datasheet"></param>
    ''' <param name="DistID"></param>
    ''' <param name="DistPartNum"></param>
    ''' <param name="CaddLibRef"></param>
    ''' <param name="CaddLibPath"></param>
    ''' <param name="CaddFootRef"></param>
    ''' <param name="CaddFootPath"></param>
    ''' <param name="Verified"></param>
    ''' <param name="Obsolete"></param>
    ''' <param name="RoHS"></param>
    ''' <param name="Temp_Max"></param>
    ''' <param name="Temp_Min"></param>
    ''' <returns>The PartID of the newly added part.</returns>
    ''' <remarks></remarks>
    Public Function partAdd(ByVal MEmfrID As Int32, ByVal mfrPartNum As String, ByVal TypeID As Int16, ByVal PkgTypeID As Int16, _
                            ByVal Desc As String, ByVal Extra_Desc As String, _
                            ByVal Value As String, ByVal Value_Units As String, ByVal Value_Tolerance As String, _
                            ByVal URL_Image As String, ByVal URL_Datasheet As String, _
                            ByVal DistID As Int32, ByVal DistPartNum As String, _
                            ByVal CaddLibRef As String, ByVal CaddLibPath As String, ByVal CaddFootRef As String, ByVal CaddFootPath As String, _
                            Optional ByVal Verified As Boolean = False, Optional ByVal Obsolete As Boolean = False, _
                            Optional ByVal RoHS As Boolean = True, Optional ByVal Temp_Max As Int16 = 0, Optional ByVal Temp_Min As Int16 = 0) As Int16

        'Insert Values
        dbAcc.SQLexe( _
            partAddSql(MEmfrID, mfrPartNum, TypeID, PkgTypeID, _
                Desc, Extra_Desc, Value, Value_Units, Value_Tolerance, _
                URL_Image, URL_Datasheet, DistID, DistPartNum, CaddLibRef, CaddLibPath, _
                CaddFootRef, CaddFootPath, Verified, Obsolete, RoHS, Temp_Max, Temp_Min) _
            )

        'Find new PartID
        Dim newPartID As Int32
        newPartID = partExistsID(MEmfrID, mfrPartNum, True)
        If newPartID < 0 Then
            'ERROR CONDITIONS! Just report and abort! At least only one table has been messed up! (part-Common)
            Return sysErrors.PARTADD_CREATEFAILED
        End If

        'Add Distributor Information
        fpDist.distAddPart(DistID, newPartID, DistPartNum)

        'Add CADD Information
        fpAltium.fpaltAddPart(newPartID, CaddLibRef, CaddLibPath, CaddFootRef, CaddFootPath)

        Return newPartID
    End Function

    ''' <summary>
    ''' Use this function to return the FriedParts unique ID for a part (PartID) when you know 
    ''' only its Manufacturer's Name and Part Number.
    ''' </summary>
    ''' <param name="MfrName"></param>
    ''' <param name="MfrPartNum"></param>
    ''' <param name="Exact">If EXACT = TRUE then only an exact match of the Manufacturer Name will 
    ''' match, otherwise *MfrName* is matched.
    ''' However, be warned, if an inexact search returns multiple hits an error will result 
    ''' (sysErrors.ERR_NOTUNIQUE)
    '''</param>
    ''' <returns>If the part exists this function returns the PartID(s), otherwise it returns 
    ''' sysErrors.ERR_NOTFOUND. If multiple manufacturers match then it will error out with 
    ''' sysErrors.ERR_NOTUNIQUE
    ''' </returns>
    ''' <remarks>PartID's should be unique and the combination of Manfacturer and Mfr Part 
    ''' Number is also unique</remarks>
    Public Function partExistsID(ByVal MfrName As String, ByVal MfrPartNum As String, Optional ByVal Exact As Boolean = False) As Int32
        'Look up MfrName (to find ID)
        Dim mfrID As Int32()
        mfrID = fpMfr.mfrExistsName(MfrName, True)
        If UBound(mfrID) = 0 Then
            'Returns only one value (either not found or the found ID)
            If mfrID(0) <> sysErrors.ERR_NOTFOUND Then
                'Call ID version of me!
                Return partExistsID(mfrID(0), MfrPartNum, False)
            Else
                Return sysErrors.ERR_NOTFOUND
            End If
        Else
            'Returned multiple values... Oh no!
            Return sysErrors.ERR_NOTUNIQUE
        End If
    End Function

    Public Function partExistsID(ByVal MfrID As Int32, ByVal MfrPartNumber As String, Optional ByVal Exact As Boolean = False) As Int32
        Dim dt As New DataTable
        Dim MfrPartNum As String = txtDefangSQL(MfrPartNumber)

        If Exact Then
            dt = dbAcc.SelectRows(dt, _
                "SELECT [FriedParts].[dbo].[part-Common].[partID] " & _
                "FROM [FriedParts].[dbo].[part-Common] " & _
                "WHERE [FriedParts].[dbo].[part-Common].[mfrID] = " & MfrID & " AND [FriedParts].[dbo].[part-Common].[mfrPartNum] = '" & MfrPartNum & "';")
        Else
            dt = dbAcc.SelectRows(dt, _
                "SELECT [FriedParts].[dbo].[part-Common].[partID] " & _
                "FROM [FriedParts].[dbo].[part-Common] " & _
                "WHERE [FriedParts].[dbo].[part-Common].[mfrID] = " & MfrID & " AND [FriedParts].[dbo].[part-Common].[mfrPartNum] LIKE '%" & Trim(MfrPartNum) & "%';")
        End If
        Select Case dt.Rows.Count
            Case 0
                Return sysErrors.ERR_NOTFOUND
            Case 1
                Return dt.Rows(0).Field(Of Int32)("PartID")
            Case Else
                Return sysErrors.PARTADD_MFRNUMNOTUNIQUE
        End Select
    End Function

    ''' <summary>
    ''' True/False determination as to whether the specified PartID exists in the 
    ''' FriedParts' database
    ''' </summary>
    ''' <param name="PartID"></param>
    ''' <returns>Returns true if the specified PartID exists in the FriedParts database</returns>
    ''' <remarks></remarks>
    Public Function partExistsID(ByVal PartID As Int32) As Boolean
        Dim dt As New DataTable
        dt = dbAcc.SelectRows(dt, _
                "SELECT [FriedParts].[dbo].[part-Common].[partID] " & _
                "FROM [FriedParts].[dbo].[part-Common] " & _
                "WHERE [FriedParts].[dbo].[part-Common].[partID] = " & PartID)
        If dt.Rows.Count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    'Returns a short descriptive name of a part based on its FriedParts PartID
    Public Function partGetShortName(ByVal PartID As Integer) As String
        Dim sqlTxt As String = _
            "SELECT [PartID]" & _
            "      ,[mfrName]" & _
            "      ,[mfrPartNum]" & _
            "  FROM [FriedParts].[dbo].[view-part] " & _
            " WHERE [PartID] = " & PartID
        Dim dt As New DataTable
        SelectRows(dt, sqlTxt)
        Return dt.Rows(0).Field(Of String)("mfrName") & " " & dt.Rows(0).Field(Of String)("mfrPartNum")
    End Function



    '==========================================================================================
    '==========================================================================================
    '==========================================================================================



    ''' <summary>
    ''' USE THIS CLASS TO RETRIEVE DATA ABOUT A SPECIFIC PART EASILY.
    ''' This class is used to access and modify a FriedPart, but not to create a new part. 
    ''' To do that use the partAdd() function.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class fpPart
        Private dt As DataTable
        Private pID As Integer
        ReadOnly Property getPartID() As Integer
            Get
                Return pID
            End Get
        End Property
        ReadOnly Property getManufacturer() As String
            Get
                Return fpMfr.mfrGetName(dt.Rows(0).Field(Of Integer)("mfrID"))
            End Get
        End Property
        ReadOnly Property getPartNumber() As String
            Get
                Return dt.Rows(0).Field(Of String)("mfrPartNum")
            End Get
        End Property
        ReadOnly Property getDescription() As String
            Get
                Return dt.Rows(0).Field(Of String)("Description")
            End Get
        End Property
        ReadOnly Property getPartType() As String
            Get
                Return ptGetTypeName(dt.Rows(0).Field(Of Integer)("TypeID"))
            End Get
        End Property
        ReadOnly Property getValue() As String
            Get
                Return dt.Rows(0).Field(Of String)("Value") & " " & dt.Rows(0).Field(Of String)("Value_Tolerance") & " " & dt.Rows(0).Field(Of String)("Value_Units")
            End Get
        End Property

        Public Sub New(ByRef PartID As Integer)
            pID = PartID
            Dim sqlTxt As String = _
                "SELECT [PartID]" & _
                "      ,[TypeID]" & _
                "      ,[PkgTypeID]" & _
                "      ,[Description]" & _
                "      ,[Extra_Description]" & _
                "      ,[Value]" & _
                "      ,[Value_Numeric]" & _
                "      ,[Value_Units]" & _
                "      ,[Value_Tolerance]" & _
                "      ,[mfrID]" & _
                "      ,[mfrPartNum]" & _
                "      ,[URL_Image]" & _
                "      ,[URL_Datasheet]" & _
                "      ,[Date_Created]" & _
                "      ,[Date_LastModified]" & _
                "      ,[Date_LastScanned]" & _
                "      ,[Verified]" & _
                "      ,[Obsolete]" & _
                "      ,[RoHS]" & _
                "      ,[Temp_Max]" & _
                "      ,[Temp_Min]" & _
                "      ,[OwnerID]" & _
                "  FROM [FriedParts].[dbo].[part-Common]" & _
                " WHERE [PartID] = " & PartID
            dt = New DataTable
            SelectRows(dt, sqlTxt)
            If dt.Rows.Count = 0 Then
                Throw New ObjectNotFoundException("You cannot instantiate an fpPart Object with an invalid PartID! You entered: " & PartID)
            End If
            'Return dt.Rows(0).Field(Of String)("mfrName") & " " & dt.Rows(0).Field(Of String)("mfrPartNum")
        End Sub
    End Class
End Module
