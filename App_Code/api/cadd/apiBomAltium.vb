Imports Microsoft.VisualBasic
Imports System.Data
Imports System.IO

Public Module apiBomAltium
    'Altium XML
    Private Const xmlCOLUMN As String = "<COLUMN "
    Private Const xmlROW As String = "<ROW "
    Private Const xmlCAPTION As String = "Caption"
    Private Const xmlNAME As String = "Name"

    Private Const xmlNUMCOLS As Byte = 6
    Private Const xmlCOLLIBREF As String = "LibRef"
    Private Const xmlCOLDESIG As String = "Designator"
    Private Const xmlCOLFOOTPRINT As String = "Footprint"
    Private Const xmlCOLDESC As String = "Description"
    Private Const xmlCOLCOMMENT As String = "Comment"

    Public Const xmlDOCTYPE As String = "<?xml version = "

    'Creates the schema of the file table that is common to all file listing operations
    'Namely:  Filename, Extension, Path
    Public Function baCreateBomTable() As Data.DataTable
        Dim Table1 As DataTable
        Table1 = New DataTable("BomList")
        'Column 0: UID
        Dim Col1 As DataColumn = New DataColumn("UID")
        Col1.DataType = System.Type.GetType("System.Int16")
        Col1.AutoIncrement = True
        Col1.AutoIncrementSeed = 1
        Col1.AutoIncrementStep = 1
        Table1.Columns.Add(Col1)
        '   Mark as Primary Key
        Dim ColArr() As DataColumn = {Col1}
        Table1.PrimaryKey = ColArr
        'Column 1: isFP
        Col1 = New DataColumn("isFP")
        Col1.DataType = System.Type.GetType("System.Boolean")
        Table1.Columns.Add(Col1)
        'Column 2: Library Reference
        Col1 = New DataColumn("PartID")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)
        'Column 3: Designator
        Col1 = New DataColumn("Designator")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)
        'Column 2: Library Reference
        Col1 = New DataColumn("Value")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)
        'Column 2: Library Reference
        Col1 = New DataColumn("Description")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)
        'Column 2: Library Reference
        Col1 = New DataColumn("Footprint")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)
        'Column 1: DNP
        Col1 = New DataColumn("DoNotPopulate")
        Col1.DataType = System.Type.GetType("System.Boolean")
        Table1.Columns.Add(Col1)
        'Return
        Return Table1
    End Function

    'This class is used as a return value container for the DNP overview UI
    Public Class DoNotPopulateOverview
        Private pCnt As Integer
        Public ReadOnly Property Count() As Integer
            Get
                Return pCnt
            End Get
        End Property
        Private pDesc As String
        Public Shadows ReadOnly Property ToString() As String
            Get
                Return pDesc
            End Get
        End Property
        Public Sub New(ByRef Count As Integer, ByRef Summary As String)
            pCnt = Count
            pDesc = Summary
        End Sub
    End Class

    Public Class baBomAnalysis
        Private isErr As Boolean = False
        Public ReadOnly Property isError() As Boolean
            Get
                Return isErr
            End Get
        End Property
        Private errMsg As String = ""
        Public ReadOnly Property ErrorMessage() As String
            Get
                Return errMsg
            End Get
        End Property
        Private dt As DataTable
        Public ReadOnly Property DataSource() As DataTable
            Get
                Return dt
            End Get
        End Property
        Private gdt As DataTable
        Public ReadOnly Property GroupedDataSource() As DataTable
            Get
                Return gdt
            End Get
        End Property

        'This project has had an xml BOM file loaded
        Public ReadOnly Property HasImportedBOM() As Boolean
            Get
                If gdt.rows.count > 0 Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Public Sub New(Optional ByRef ErrorMessage As String = "", Optional ByRef DataSource As DataTable = Nothing, Optional ByRef GroupedDataSource As DataTable = Nothing)
            If ErrorMessage <> "" Then
                'We are an error build!
                errMsg = ErrorMessage
                isErr = True
            Else
                dt = DataSource
                gdt = GroupedDataSource
            End If
        End Sub

        ''' <summary>
        ''' Returns true if the specified PartID is used in this BOM
        ''' </summary>
        ''' <param name="PartID">The PartID to search for</param>
        ''' <returns>True if the specified PartID is contained in the BOM represented by this baBomAnalysis class</returns>
        ''' <remarks></remarks>
        Public Function Contains(ByVal PartID As Integer) As Boolean
            If gdt IsNot Nothing Then
                For Each dr As DataRow In gdt.Rows
                    'dr.Field(Of Boolean)("isFP")
                    'dr.Field(Of String)("PartID")
                    If dr.Field(Of Boolean)("isFP") = True And String.Compare(dr.Field(Of String)("PartID"), CStr(PartID), False) = 0 Then
                        Return True
                    End If
                Next
                Return False
            Else
                'No BOM imported yet...
                Throw New Exception("Attempted to search for a part in an imported BOM, but no BOM has been imported yet!")
                Return False
            End If
        End Function

        Public ReadOnly Property DoNotPopulateSummary() As DoNotPopulateOverview
            Get
                Dim result As Integer = 0
                Dim desc As String = ""
                For Each dr As DataRow In dt.Rows
                    If dr.Field(Of Boolean)("DoNotPopulate") = True Then
                        result = result + 1 'Count
                        desc = desc & dr.Field(Of String)("Designator") & "(" & dr.Field(Of String)("PartID") & "), "
                    End If
                Next
                If desc.Length > 0 Then desc = desc.Remove(desc.Length - 2) 'Trim the extra separator
                Return New DoNotPopulateOverview(result, desc)
            End Get
        End Property

        'Sets the Do Not Populate status for the specified PartID to the value of the DNP parameter
        'Marks EVERY RefDes associated with this PartID as the same state (DNP parameter)
        Public Sub SetDoNotPopulate(ByRef PartID As Integer, ByVal DNP As Boolean)
            Dim dr As DataRow
            'Front-End
            For Each dr In gdt.Rows
                If String.Compare(dr.Field(Of String)("PartID"), CStr(PartID), False) = 0 Then
                    dr.Item("DoNotPopulate") = DNP
                End If
            Next
            'Back-End
            For Each dr In dt.Rows
                If String.Compare(dr.Field(Of String)("PartID"), CStr(PartID), False) = 0 Then
                    dr.Item("DoNotPopulate") = DNP
                End If
            Next
        End Sub

        'Sets the Do Not Populate status for the specified PartID to the value of the DNP parameter
        'Marks ONLY ONE SPECIFIC RefDes associated with this PartID as the state (DNP parameter)
        Public Sub SetDoNotPopulate(ByRef RefDes As String, ByVal DNP As Boolean)
            Dim dr As DataRow
            'Front-End (complicated because only part of its RefDes's may be excluded)
            For Each dr In gdt.Rows
                If String.Compare(dr.Field(Of String)("Designator"), RefDes, False) = 0 Then
                    'xxx Should really do something more complicated with this like checking if all RefDes of this part are at the same state or mixed state
                    dr.Item("DoNotPopulate") = DNP
                End If
            Next
            'Back-End
            For Each dr In dt.Rows
                If String.Compare(dr.Field(Of String)("Designator"), RefDes, False) = 0 Then
                    dr.Item("DoNotPopulate") = DNP
                End If
            Next
        End Sub

        Public Sub SetIsFriedPart(ByRef PartID As Integer, ByVal IsFP As Boolean)
            Dim dr As DataRow
            Dim priorIsFP As Boolean
            'Front-End
            For Each dr In gdt.Rows
                If String.Compare(dr.Field(Of String)("PartID"), CStr(PartID), False) = 0 Then
                    priorIsFP = dr.Item("isFP")
                    dr.Item("isFP") = IsFP
                End If
            Next
            'Back-End
            For Each dr In dt.Rows
                If String.Compare(dr.Field(Of String)("PartID"), CStr(PartID), False) = 0 Then
                    dr.Item("isFP") = IsFP
                End If
            Next
            'Do Not Populate Non-FriedParts (also restores parts marked as FP to populate status (not DNP))
            SetDoNotPopulate(PartID, Not IsFP)
        End Sub

        'Write this Imported BOM to the database -- using the specified ProjectID
        Public Sub Save(ByRef ProjectID As Integer)
            Dim BakedPartID As Integer = 10 'We'll flip this to negatives before we write to the database, but it is easier to think of positives for now
            Dim i As Integer
            Dim dr As DataRow
            Dim drp As DataRow 'Data Row Prior (dr -1)
            'Save the Imported Parts
            For i = 0 To dt.Rows.Count - 1
                dr = dt.Rows(i)
                If dr.Field(Of Boolean)("isFP") Then
                    'Save the FriedPart
                    fpProj.bomAddFriedRefDes(ProjectID, dr.Item("PartID"), dr.Item("Designator"), dr.Item("Footprint"), dr.Item("DoNotPopulate"), dr.Item("Value"), dr.Item("Description"))
                Else
                    'Save the BakedPart
                    If i > 0 Then
                        drp = dt.Rows(i - 1) 'Load the previous row for comparison
                        If (String.Compare(dr.Field(Of String)("PartID"), drp.Field(Of String)("PartID")) = 0) And (String.Compare(dr.Field(Of String)("Footprint"), drp.Field(Of String)("Footprint")) = 0) Then
                            'Same BakedPart as the last one!
                            'Don't decrement the PartID
                        Else
                            'Different BakedPart than the previous one
                            BakedPartID = BakedPartID + 1
                        End If
                    Else
                        'Do nothing -- obviously the first part is going to be a unique PartID
                    End If
                    'Save this BakedPart
                    fpProj.bomAddBakedRefDes(ProjectID, BakedPartID, dr.Item("Designator"), dr.Item("Description"), dr.Item("PartID"), dr.Item("Value"), dr.Item("Footprint"))
                End If
            Next
        End Sub
    End Class

    Public Function baImportBOM(ByRef FileName As String) As baBomAnalysis
        Dim TheBomXmlText As New StreamReader(uploadBOMIMPORT() & FileName)
        Dim RetVal As baBomAnalysis        
        RetVal = apiBomAltium.baImportBOM(TheBomXmlText)
        TheBomXmlText.Close()
        Return RetVal
    End Function

    Public Function baImportBOM(ByRef TextStream As TextReader) As baBomAnalysis
        'Init
        Dim RetVal As baBomAnalysis
        Dim dt As DataTable = baCreateBomTable()
        Dim gdt As DataTable = baCreateBomTable()
        Dim txtLine As String
        Dim ColNames As New baColumnNames
        Dim ColFields As baColumnFields
        Dim RowFields As baRowFields
        Dim i As Integer = 0

        'Check if XML file
        Dim firstLine As String = TextStream.ReadLine()
        If firstLine.StartsWith(xmlDOCTYPE) = False Then
            RetVal = New baBomAnalysis("The uploaded file is not a valid XML file!")
            Return RetVal
            Exit Function
        End If

        While TextStream.Peek() <> -1
            'Find the Column Names (Schema)
            txtLine = Trim(TextStream.ReadLine)
            If txtLine.StartsWith(xmlCOLUMN) Then
                'Found a <COLUMN> tag!
                ColFields = parseColumnTag(txtLine)
                ColNames.addColumnName(ColFields.Caption, ColFields.Name)
            Else
                'Find the Row Values (Bom Items)
                If txtLine.StartsWith(xmlROW) Then
                    'Found a <ROW> tag!
                    If Not ColNames.gotAllColumns Then
                        RetVal = New baBomAnalysis("The uploaded file was not formatted properly. It contained too many or too few columns.")
                        Return RetVal
                        Exit Function
                    End If
                    RowFields = parseRowTag(txtLine, ColNames)
                    'Check if BOM was properly exported
                    If RowFields.PartID.Contains(",") Then
                        'ERROR! Grouping of BOM is wrong!
                        RetVal = New baBomAnalysis("The uploaded BOM is incorrectly grouped! The BOM must be grouped exclusively by the 'LibRef' column.")
                        Return RetVal
                    End If

                    '("UID")
                    '("isFP")
                    '("PartID")
                    '("Designator")
                    '("Value")
                    '("Description")
                    '("Footprint")

                    'Backend Table
                    For Each n In RowFields.RefDes
                        i = i + 1
                        Dim o() As Object = {i, RowFields.isFriedPart, RowFields.PartID, n.Trim(), RowFields.Value, RowFields.Description, RowFields.Footprint, Not RowFields.isFriedPart}
                        dt.Rows.Add(o)
                    Next

                    'Frontend Table
                    Dim o2() As Object = {i, RowFields.isFriedPart, RowFields.PartID, txtArrayToString(RowFields.RefDes), RowFields.Value, RowFields.Description, RowFields.Footprint, Not RowFields.isFriedPart}
                    gdt.Rows.Add(o2)
                End If
            End If
        End While
        RetVal = New baBomAnalysis(, dt, gdt)
        Return RetVal
    End Function

    'Parses a single <COLUMN> row of the BOM.
    Private Function parseColumnTag(ByVal colIn As String) As baColumnFields
        Dim retval As New baColumnFields
        retval.Caption = parseTagField(colIn, xmlCAPTION)
        retval.Name = parseTagField(colIn, xmlNAME)
        Return retval
    End Function

    'Parses a single <ROW> row of the BOM.
    Private Function parseRowTag(ByVal rowIn As String, ByRef ColumnNames As baColumnNames) As baRowFields
        '("isFP")
        '("PartID")
        '("Designator")
        '("Value")
        '("Description")
        '("Footprint")

        Dim retval As New baRowFields
        Dim str As String
        Dim intPartID As Integer

        'Is FriedPart? (and PartID)
        str = parseTagField(rowIn, ColumnNames.getColumnName(xmlCOLLIBREF))
        retval.PartID = Trim(str)
        If Int32.TryParse(str, intPartID) Then
            'Confirm valid FriedPart (already know it's an integer)
            retval.isFriedPart = fpParts.partExistsID(intPartID)
        Else
            'Non FriedParts part or user did not group the BOM correctly
            retval.isFriedPart = False
        End If

        'Reference Designators
        str = parseTagField(rowIn, ColumnNames.getColumnName(xmlCOLDESIG))
        retval.RefDes = str.Split(", ")

        'Value
        retval.Value = parseTagField(rowIn, ColumnNames.getColumnName(xmlCOLCOMMENT))

        'Description
        retval.Description = parseTagField(rowIn, ColumnNames.getColumnName(xmlCOLDESC))

        'Footprint
        retval.Footprint = parseTagField(rowIn, ColumnNames.getColumnName(xmlCOLFOOTPRINT))

        Return retval
    End Function

    'Return Column Name and Caption Fields
    'Assumes you passed in a valid <COLUMN>...</COLUMN> row
    Private Function parseTagField(ByVal tagIn As String, ByRef fieldName As String) As String
        'Var
        Dim idxF As Integer
        Dim idxQ As Integer

        'Look for field names
        idxF = tagIn.IndexOf(fieldName & "=")
        If idxF = -1 Then
            Err.Raise(-23498590, , "TagLine is missing <FieldName>= substring")
        Else
            'Save Caption Column Name
            Dim str As String
            str = tagIn.Substring(idxF + fieldName.Length + 1)
            idxQ = str.IndexOf(Chr(34), 1) 'Find the quotes character starting from the second char in the string, since the first char is a quote!
            If idxQ = -1 Then
                Err.Raise(-8748574, , "TagLine --CAPTION misformatted or missing")
            Else
                Return str.Substring(1, idxQ - 1)
            End If
        End If
        Return "" 'Return empty string if not found!
    End Function

    Private Class baColumnFields
        Public Name As String
        Public Caption As String
    End Class

    Private Class baRowFields
        '("UID")
        '("isFP")
        '("PartID")
        '("Designator")
        '("Value")
        '("Description")
        '("Footprint")
        Public isFriedPart As Boolean
        Public PartID As String
        Public RefDes() As String
        Public Value As String
        Public Description As String
        Public Footprint As String
    End Class

    Private Class baColumnNames
        'Private Const xmlCOLLIBREF As String = "LibRef"
        'Private Const xmlCOLDESIG As String = "Designator"
        'Private Const xmlCOLFOOTPRINT As String = "Footprint"
        'Private Const xmlCOLDESC As String = "Description"
        'Private Const xmlCOLCOMMENT As String = "Comment"
        Public ColNames As New Collection

        Public Sub addColumnName(ByRef Caption As String, ByRef Name As String)
            ColNames.Add(Name, Caption)
        End Sub

        Public Function gotAllColumns() As Boolean
            'Private Const xmlCOLLIBREF As String = "LibRef"
            'Private Const xmlCOLDESIG As String = "Designator"
            'Private Const xmlCOLFOOTPRINT As String = "Footprint"
            'Private Const xmlCOLDESC As String = "Description"
            'Private Const xmlCOLCOMMENT As String = "Comment"
            If _
                ColNames.Count = xmlNUMCOLS And _
                ColNames.Contains(xmlCOLCOMMENT) And _
                ColNames.Contains(xmlCOLDESIG) And _
                ColNames.Contains(xmlCOLLIBREF) And _
                ColNames.Contains(xmlCOLFOOTPRINT) And _
                ColNames.Contains(xmlCOLDESC) _
            Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function getColumnName(ByRef xmlCaption As String) As String
            If ColNames.Contains(xmlCaption) Then
                Return ColNames.Item(xmlCaption)
            Else
                Return sysErrors.ERR_NOTFOUND
            End If
        End Function
    End Class
End Module
