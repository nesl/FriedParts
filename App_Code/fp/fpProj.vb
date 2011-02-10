Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Web

Public Module fpProj
    Public Enum PartClass
        ImportedFriedPart = 64
        ImportedNotFriedPart = 65
        ManualAddonFriedPart = 66
        ManualAddonNotFriedPart = 67
    End Enum

    'Add a FriedPart reference designator to a bill of materials
    Public Sub bomAddFriedRefDes(ByVal ProjectID As Integer, ByVal PartID As Integer, ByVal RefDes As String, ByVal Footprint As String, ByVal DoNotPopulate As Boolean, Optional ByVal Value As String = "", Optional ByVal Description As String = "")
        Dim sqlcmdtxt As String = _
            "INSERT INTO [FriedParts].[dbo].[proj-BOM]" & _
            "           ([ProjectID]" & _
            "           ,[Class]" & _
            "           ,[PartID]" & _
            "           ,[RefDes]" & _
            "           ,[Deleted]" & _
            "           ,[UserID]" & _
            "           ,[Footprint]" & _
            "           ,[Value]" & _
            "           ,[DoNotPopulate]" & _
            "           ,[Description])" & _
            "     VALUES (" & _
            "           " & ProjectID & "," & _
            "           " & fpProj.PartClass.ImportedFriedPart & "," & _
            "           " & PartID & "," & _
            "           '" & RefDes & "'," & _
            "           " & sqlFALSE & "," & _
            "           " & HttpContext.Current.Session("user.UserID") & "," & _
            "           '" & Footprint & "'," & _
            "           '" & Value & "'," & _
            "           " & txtSqlEncode(DoNotPopulate) & "," & _
            "           '" & Description & "'" & _
            "           )"
        SQLexe(sqlcmdtxt)
    End Sub

    'Add a manually added-in part to the Project's BOM (parts must be Fried)
    Public Sub bomAddAddonRefDes(ByVal ProjectID As Integer, ByVal PartID As Integer, ByVal Qty As Integer, Optional ByRef Notes As String = "")
        Dim sqlcmdtxt As String = _
            "INSERT INTO [FriedParts].[dbo].[proj-BOM-Addon]" & _
            "           ([ProjectID]" & _
            "           ,[Class]" & _
            "           ,[PartID]" & _
            "           ,[Qty]" & _
            "           ,[Deleted]" & _
            "           ,[UserID]" & _
            "           ,[Notes])" & _
            "     VALUES (" & _
            "           " & ProjectID & "," & _
            "           " & PartClass.ManualAddonFriedPart & "," & _
            "           " & PartID & "," & _
            "           " & Qty & "," & _
            "           " & sqlFALSE & "," & _
            "           " & HttpContext.Current.Session("user.UserID") & "," & _
            "           '" & Notes & "'" & _
            "           )"
        SQLexe(sqlcmdtxt)
    End Sub

    'Add a NON-FRIEDPART (baked part) reference designator to the Bill of Materials
    'Baked parts are automatically marked DO NOT POPULATE and ImportedNotFriedPart
    Public Sub bomAddBakedRefDes(ByVal ProjectID As Integer, ByVal UID As Integer, ByVal RefDes As String, ByRef Description As String, ByRef PartID As String, ByRef Value As String, ByRef Footprint As String)
        Dim strDesc As String
        If Description.Length = 0 Then
            strDesc = PartID
        Else
            strDesc = Description
        End If
        Dim sqlcmdtxt As String = _
            "INSERT INTO [FriedParts].[dbo].[proj-BOM]" & _
            "           ([ProjectID]" & _
            "           ,[Class]" & _
            "           ,[PartID]" & _
            "           ,[RefDes]" & _
            "           ,[Deleted]" & _
            "           ,[UserID]" & _
            "           ,[Footprint]" & _
            "           ,[Value]" & _
            "           ,[DoNotPopulate]" & _
            "           ,[Description])" & _
            "     VALUES (" & _
            "           " & ProjectID & "," & _
            "           " & fpProj.PartClass.ImportedNotFriedPart & "," & _
            "           " & -1 * UID & "," & _
            "           '" & RefDes & "'," & _
            "           " & sqlFALSE & "," & _
            "           " & HttpContext.Current.Session("user.UserID") & "," & _
            "           '" & Footprint & "'," & _
            "           '" & Value & "'," & _
            "           " & sqlTRUE & "," & _
            "           '" & strDesc & "'" & _
            "           )"
        SQLexe(sqlcmdtxt)
    End Sub

    Public Function projExists(ByVal projName As String, ByVal projRev As String) As Integer
        Dim dt As New DataTable
        Dim retval() As Int32 = {sysErrors.ERR_NOTFOUND}

        dt = dbAcc.SelectRows(dt, _
            "SELECT [FriedParts].[dbo].[proj-Common].[ProjectID], [Title], [Revision] " & _
            "FROM [FriedParts].[dbo].[proj-Common] " & _
            "WHERE [Title] = '" & Trim(projName) & "' AND [Revision] = '" & Trim(projRev) & "';")

        If dt.Rows.Count > 0 Then
            'Found!
            Return dt.Rows(0).Field(Of Integer)("ProjectID")
        Else
            'Not Found!
            Return sysErrors.ERR_NOTFOUND
        End If
    End Function

    'Tests to see if the string is in valid revision format: MMYY
    Public Function projValidRevision(ByRef Revision As String) As Boolean
        Try
            If IsNumeric(Revision) AndAlso (Revision > 0) AndAlso (Revision < 10000) AndAlso (Revision.Length = 4) Then
                Return True
            Else
                Return False
            End If
        Catch e As System.FormatException
            Return False
        End Try
    End Function

    'Creates the schema of the file table that is common to all file listing operations
    'Namely:  Filename, Extension, Path
    Public Function projCreateAdditionalBomTable() As Data.DataTable
        Dim Table1 As DataTable
        Table1 = New DataTable("AdditionalBomList")

        'Column 0: UID
        Dim Col1 As DataColumn = New DataColumn("PartID")
        Col1.DataType = System.Type.GetType("System.Int32")
        Table1.Columns.Add(Col1)
        Dim ColArr() As DataColumn = {Col1}
        Table1.PrimaryKey = ColArr
        'Column 2: Library Reference
        Col1 = New DataColumn("Type")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)
        'Column 3: Designator
        Col1 = New DataColumn("Mfr")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)
        'Column 2: Library Reference
        Col1 = New DataColumn("MPN")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)
        'Column 2: Library Reference
        Col1 = New DataColumn("Desc")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)
        'Column
        Col1 = New DataColumn("Qty")
        Col1.DataType = System.Type.GetType("System.Int32")
        Table1.Columns.Add(Col1)

        'Return
        Return Table1
    End Function

    Private Class fpBomRow
        Public ItemNum As Integer
        Public PartClass As Byte
        Public RefDes As String
        Public Qty As Integer
        Public PartID As Integer
        Public Mfr As String
        Public MPN As String
        Public Desc As String
        Public DoNotPopulate As Boolean
    End Class

    '=============================================================
    '== BOM CLASS -- PROJECT DATA ==> BOM REPORT
    '=============================================================
    '-  The BOM class takes in a project and produces the formatted
    '-      datasource for the Bill of Materials Report
    '-------------------------------------------------------------
    Public Class fpBOM
        Private DTout As DataTable
        Private DTout_Labels As DataTable 'The output table (does not include DNP, provides short part number to make sure the labels work)
        Private DTi As New DataTable 'BOM Part Input -- imported
        Private DTia As New DataTable 'BOM Part Input -- manual addons
        Private ProjCommonDataRow As DataRow
        Private LineNumber As Integer = 1 'Use post-increment

        Public ReadOnly Property GetTitle() As String
            Get
                Return ProjCommonDataRow.Field(Of String)("Title")
            End Get
        End Property
        Public ReadOnly Property GetDescription() As String
            Get
                Return ProjCommonDataRow.Field(Of String)("Description")
            End Get
        End Property
        'Get's this projectID's revision
        Public ReadOnly Property GetRevision() As String
            Get
                Return ProjCommonDataRow.Field(Of String)("Revision")
            End Get
        End Property
        'Gets all of the revisions with the same title as this projectID
        'HTML property turns on HTML style formatting -- bolds this revision
        Public ReadOnly Property GetRevisions(Optional ByVal HTML As Boolean = False) As String
            Get
                Dim thisRev As String = ProjCommonDataRow.Field(Of String)("Revision")
                Dim sqltxt As String = _
                   "USE [FriedParts] " & _
                   "SELECT [Revision] FROM [proj-Common] " & _
                   "WHERE [Title] = '" & ProjCommonDataRow.Field(Of String)("Title") & "'"
                Dim dt As New DataTable
                SelectRows(dt, sqltxt)
                Dim Revs As String = ""
                Dim Revs_HTML As String = ""
                For Each dr As DataRow In dt.Rows
                    Revs = Revs & " -- " & dr.Field(Of String)("Revision")
                    If String.Compare(dr.Field(Of String)("Revision"), Me.GetRevision()) = 0 Then
                        Revs_HTML = Revs_HTML & " -- <span style=""font-family:Arial Black;"">" & dr.Field(Of String)("Revision") & "</span>"
                    Else
                        Revs_HTML = Revs_HTML & " -- " & dr.Field(Of String)("Revision")
                    End If
                Next
                If HTML Then
                    Return Revs_HTML
                Else
                    Return Revs
                End If
            End Get
        End Property
        Public ReadOnly Property GetOwnerID() As Integer
            Get
                Return ProjCommonDataRow.Field(Of Integer)("OwnerID")
            End Get
        End Property
        Public ReadOnly Property GetOwnerName() As String
            Get
                Return suGetUsername(ProjCommonDataRow.Field(Of Integer)("OwnerID"))
            End Get
        End Property
        Public ReadOnly Property GetDateCreated() As Date
            Get
                Return ProjCommonDataRow.Field(Of Date)("DateCreated")
            End Get
        End Property
        Public ReadOnly Property GetDataSource(Optional ByVal IncludeDoNotPopulate As Boolean = True) As DataTable
            Get
                If IncludeDoNotPopulate Then
                    Return DTout
                Else
                    Return DTout_Labels
                End If
            End Get
        End Property

        Protected Sub processBomImported()
            'The DTi is sorted by Class then PartID so all refdes of the same part should be grouped together
            'DTi is an INNER JOINed table. Field names that conflict are native from [proj-BOM] and numbered from [view-part]
            '   ex. "Description" is from [proj-BOM], but "Description1" is from [view-part]
            Dim LineItem As fpBomRow
            For Each dr As DataRow In DTi.Rows
                'Check if this refdes matches prior
                If LineItem Is Nothing OrElse dr.Field(Of Integer)("PartID") <> LineItem.PartID Then
                    If LineItem IsNot Nothing Then
                        'Save the last line before starting a new one
                        Dim o() As Object = {LineItem.ItemNum, convertPartClassToString(LineItem.PartClass), LineItem.Qty, LineItem.RefDes, LineItem.PartID, LineItem.Mfr & " " & LineItem.MPN, LineItem.Desc, LineItem.DoNotPopulate}
                        DTout.Rows.Add(o)
                        If Not LineItem.DoNotPopulate Then
                            formatShortMfrPartNum(LineItem)
                            Dim r() As Object = {LineItem.ItemNum, convertPartClassToString(LineItem.PartClass), LineItem.Qty, LineItem.RefDes, LineItem.PartID, LineItem.Mfr & " " & LineItem.MPN, LineItem.Desc, LineItem.DoNotPopulate}
                            DTout_Labels.Rows.Add(r)
                        End If
                    End If
                    'New part
                    LineItem = New fpBomRow
                    LineItem.ItemNum = LineNumber
                    LineNumber = LineNumber + 1
                    LineItem.PartID = dr.Field(Of Integer)("PartID")
                    LineItem.Mfr = dr.Field(Of String)("mfrName")
                    LineItem.MPN = dr.Field(Of String)("mfrPartNum")
                    LineItem.RefDes = dr.Field(Of String)("RefDes")
                    LineItem.Qty = 1
                    LineItem.PartClass = dr.Field(Of Byte)("Class")
                    'Description (the hardest one!)
                    Select Case LineItem.PartClass
                        Case PartClass.ImportedFriedPart
                            'FriedPart (easier)
                            LineItem.Desc = dr.Field(Of String)("Description1")
                        Case PartClass.ImportedNotFriedPart
                            'Not a FriedPart (messier)
                            LineItem.Desc = dr.Field(Of String)("Description") & " -- " & dr.Field(Of String)("Value") & " (" & dr.Field(Of String)("Footprint") & ")"
                        Case Else
                            'Error!
                            Err.Raise(-34320, , "Should not happen!")
                    End Select
                    LineItem.DoNotPopulate = dr.Field(Of Boolean)("DoNotPopulate")
                Else
                    'Matches prior! -- just update RefDes string
                    LineItem.RefDes = LineItem.RefDes & ", " & dr.Field(Of String)("RefDes")
                    LineItem.Qty = LineItem.Qty + 1
                End If
            Next
            'Save the last line (if it exists)
            If LineItem IsNot Nothing Then
                Dim p() As Object = {LineItem.ItemNum, convertPartClassToString(LineItem.PartClass), LineItem.Qty, LineItem.RefDes, LineItem.PartID, LineItem.Mfr & " " & LineItem.MPN, LineItem.Desc, LineItem.DoNotPopulate}
                DTout.Rows.Add(p)
                If Not LineItem.DoNotPopulate Then
                    formatShortMfrPartNum(LineItem)
                    Dim q() As Object = {LineItem.ItemNum, convertPartClassToString(LineItem.PartClass), LineItem.Qty, LineItem.RefDes, LineItem.PartID, LineItem.Mfr & " " & LineItem.MPN, LineItem.Desc, LineItem.DoNotPopulate}
                    DTout_Labels.Rows.Add(q)
                End If
            End If
        End Sub

        'FINISH ME!!!
        Protected Sub processBomManual()
            'The DTia is sorted by Class, but PartID's should be unique (quantity is explicitly specified)
            'DTia is an INNER JOINed table. Field names that conflict are native from [proj-BOM-Addon] and numbered from [view-part]
            '   ex. "PartID" is from [proj-BOM], but "PartID1" is from [view-part]
            Dim LineItem As fpBomRow
            For Each dr As DataRow In DTia.Rows
                'New part
                LineItem = New fpBomRow
                LineItem.ItemNum = LineNumber
                LineNumber = LineNumber + 1
                LineItem.PartID = dr.Field(Of Integer)("PartID")
                LineItem.Mfr = dr.Field(Of String)("mfrName")
                LineItem.MPN = dr.Field(Of String)("mfrPartNum")
                LineItem.RefDes = dr.Field(Of String)("Notes")
                LineItem.Qty = dr.Field(Of Integer)("Qty")
                LineItem.PartClass = dr.Field(Of Byte)("Class")
                'Description (the hardest one!)
                Select Case LineItem.PartClass
                    Case PartClass.ManualAddonFriedPart
                        'FriedPart (easier)
                        LineItem.Desc = dr.Field(Of String)("Description")
                    Case PartClass.ManualAddonNotFriedPart
                        'Not a FriedPart (messier)
                        LineItem.Desc = "NOT SUPPORTED YET!"
                    Case Else
                        'Error!
                        Err.Raise(-34320, , "Should not happen!")
                End Select
                LineItem.DoNotPopulate = True

                'Save the last line before starting a new one
                Dim o() As Object = {LineItem.ItemNum, convertPartClassToString(LineItem.PartClass), LineItem.Qty, LineItem.RefDes, LineItem.PartID, LineItem.Mfr & " " & LineItem.MPN, LineItem.Desc, LineItem.DoNotPopulate}
                DTout.Rows.Add(o)
            Next
        End Sub

        'Modifies the passed in LineItem object to shorten the mfr + part number field (if necessary)
        Private Sub formatShortMfrPartNum(ByRef LineItem As fpBomRow)
            Const Total_Length As Integer = 34
            If LineItem.Mfr.Length + LineItem.MPN.Length + 1 <= Total_Length Then
                'Room to print complete name (this is a guess because the font is not fixed width)
                Exit Sub
            Else
                LineItem.Mfr = Left(LineItem.Mfr, Total_Length - LineItem.MPN.Length - 1)
            End If
        End Sub


        Protected Function convertPartClassToString(ByRef ClassID As Byte) As String
            Select Case ClassID
                Case PartClass.ImportedFriedPart
                    Return "Fried (CAD)"
                Case PartClass.ImportedNotFriedPart
                    Return "Baked (CAD)"
                Case PartClass.ManualAddonFriedPart
                    Return "Fried (Addon)"
                Case PartClass.ManualAddonNotFriedPart
                    Return "Baked (Addon)"
                Case Else
                    Err.Raise(-6765, , "Oh no!")
            End Select
            Return "" 'Just to keep compiler happy... will never reach
        End Function

        Public Function createDataSource() As DataTable
            Dim Table1 As New DataTable("Project BOM Report")
            SelectRows(Table1, "SELECT * FROM [FriedParts].[dbo].[proj-BOM-Report]")
            Return Table1
        End Function

        'Instantiate Read-Only class by passing in ProjectID
        Public Sub New(ByRef ProjectID As Integer)
            'Get Common Project Data
            Dim sqltxt As String = _
                "USE [FriedParts] " & _
                "SELECT * FROM [proj-Common] " & _
                "WHERE [ProjectID] = " & ProjectID
            Dim dt As New DataTable
            SelectRows(dt, sqltxt)
            Select Case dt.Rows.Count
                Case 0
                    'Not found... ignore
                Case 1
                    ProjCommonDataRow = dt.Rows(0)
                    'Get BOM Import Parts
                    sqltxt = _
                        "USE [FriedParts] " & _
                        "SELECT * FROM [proj-BOM] LEFT JOIN [view-part] " & _
                        "           ON [proj-BOM].[PartID] = [view-part].[PartID] " & _
                        " WHERE [ProjectID] = " & ProjectID & " AND [Deleted] = " & sqlFALSE & _
                        " ORDER BY [DoNotPopulate] ASC, [Class] ASC, [proj-BOM].[PartID] ASC, [proj-BOM].[RefDes] ASC"
                    SelectRows(DTi, sqltxt)
                    'Get BOM Manual Parts
                    sqltxt = _
                        "USE [FriedParts] " & _
                        "SELECT * FROM [proj-BOM-Addon] LEFT JOIN [view-part] " & _
                        "           ON [proj-BOM-Addon].[PartID] = [view-part].[PartID] " & _
                        " WHERE [ProjectID] = " & ProjectID & " AND [Deleted] = " & sqlFALSE & _
                        " ORDER BY [Class] ASC, [proj-BOM-Addon].[PartID] ASC"
                    SelectRows(DTia, sqltxt)
                    DTout = createDataSource()
                    DTout_Labels = createDataSource()
                    processBomImported()
                    processBomManual()
                Case Else
                    Err.Raise(-2454, , "Proj-Common is corrupt. More than one project ID found for (" & ProjectID & ")")
                    Exit Sub
            End Select
        End Sub
        'Do not allow null projectID
        Public Sub New()
            Err.Raise(-343, , "fpBOM class cannot be instantiated without specifying a ProjectID")
        End Sub
    End Class

    '=============================================================
    '== PROJECT CLASS -- GROUPS ALL PROJECT DATA INTO ONE OBJECT
    '=============================================================
    '-  The Project class is used as a temporary holding place for
    '-      all project data until the project is committed to the 
    '-      database
    '-------------------------------------------------------------
    Public Class fpProject
        Public BomAnalysis As baBomAnalysis
        Private iMyProjectID As Integer = 0
        Public Property MyProjectID() As Integer
            Get
                If iMyProjectID = 0 Then
                    Return Nothing
                Else
                    Return iMyProjectID
                End If
            End Get
            Set(ByVal value As Integer)
                If value > 0 Then iMyProjectID = value
            End Set
        End Property

        Private iFilename As String
        Public Property ZipFilename() As String
            Get
                Return iFilename
            End Get
            Set(ByVal value As String)
                iFilename = value
            End Set
        End Property

        Private iMyProjectTitle As String = ""
        Public Property MyProjectTitle() As String
            Get
                Return iMyProjectTitle
            End Get
            Set(ByVal value As String)
                If iMyProjectTitle.Length = 0 Then
                    iMyProjectTitle = value
                Else
                    Err.Raise(-342, , "This project already has a title. You cannot set it twice.")
                End If
            End Set
        End Property

        Private iMyProjectRev As String = ""
        Public Property MyProjectRevision() As String
            Get
                Return iMyProjectRev
            End Get
            Set(ByVal value As String)
                If iMyProjectRev.Length = 0 Then
                    iMyProjectRev = value
                Else
                    Err.Raise(-342, , "This project already has a revision. You cannot set it twice.")
                End If
            End Set
        End Property

        Public ReadOnly Property BomDataSource() As DataTable
            Get
                Return BomAnalysis.DataSource
            End Get
        End Property

        Public ReadOnly Property GroupedBomDataSource() As DataTable
            Get
                Return BomAnalysis.GroupedDataSource
            End Get
        End Property

        Private abdt As DataTable
        Public ReadOnly Property AdditionalBomDataSource() As DataTable
            Get
                Return abdt
            End Get
        End Property

        Public ReadOnly Property DoNotPopulateSummary() As DoNotPopulateOverview
            Get
                Return BomAnalysis.DoNotPopulateSummary
            End Get
        End Property
        'This project has had an xml BOM file loaded
        Public ReadOnly Property HasImportedBOM() As Boolean
            Get
                If BomAnalysis Is Nothing Then
                    Return False
                Else
                    Return BomAnalysis.HasImportedBOM
                End If
            End Get
        End Property

        Public Sub New()
            'Create the table to hold the bom add-ons
            abdt = projCreateAdditionalBomTable()
        End Sub

        'Write this Project's BOM to the Database
        Public Sub Save()
            'Am I a valid Databased Project (created yet or modifying valid existing)
            If iMyProjectID <= 0 Then
                Err.Raise(-767, , "Cannot add BOM items to an invalid ProjectID (" & iMyProjectID & ")!")
            End If
            'Save the Imported BOM
            If Not HasImportedBOM Then
                Err.Raise(-3343, , "Cannot write BOM as XML has not been imported.")
            End If
            BomAnalysis.Save(iMyProjectID)
            'Save the Additional BOM
            For Each dr As DataRow In abdt.Rows
                bomAddAddonRefDes(iMyProjectID, dr.Item("PartID"), dr.Item("Qty"))
            Next
        End Sub

        'Delete a line-item (part) from the additional BOM table
        'Assumes you entered a valid PartID (e.g. it already exists)
        Public Sub DeleteAdditionalPart(ByVal PartID As Integer)
            abdt.Rows.Find(PartID).Delete()
        End Sub

        'Add a line-item (part) to the additional BOM table
        'Assumes you entered a valid PartID and valid Qty
        Public Sub AddAdditionalPart(ByVal PartID As Integer, ByVal Qty As Integer)
            Dim Part As New fpPart(PartID)
            Dim dr As DataRow
            dr = abdt.NewRow()
            dr("PartID") = PartID
            dr("Type") = Part.getPartType
            dr("Mfr") = Part.getManufacturer
            dr("MPN") = Part.getPartNumber
            dr("Desc") = Part.getDescription
            dr("Qty") = Qty
            abdt.Rows.Add(dr)
        End Sub


    End Class
End Module
