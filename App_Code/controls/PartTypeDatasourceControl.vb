Imports Microsoft.VisualBasic
Imports System.Web.UI.FriedParts.PartTypeAccordionControl 'To get the MAX_DEPTH constant
Imports System.Data

Namespace System.Web.UI.FriedParts

    Public Class PartTypeDatasourceControl
#Region "Variables"
        ''' <summary>
        ''' Collection of DataTables
        ''' </summary>
        ''' <remarks></remarks>
        Protected Datasource As New Collection

        ''' <summary>
        ''' Contains the array of level "titles" -- the TypeName at this level
        ''' </summary>
        ''' <remarks></remarks>
        Protected LevelTitle(MAX_DEPTH) As String

        ''' <summary>
        ''' The ParentID of the current state reflected in the loaded
        ''' tables of the Datasource (DataTable collection)
        ''' </summary>
        ''' <remarks></remarks>
        Protected iParentID As Int32
#End Region

#Region "Properties"
        Public ReadOnly Property GetTitles As String()
            Get
                Return LevelTitle
            End Get
        End Property

        Public ReadOnly Property GetParentID As Int32
            Get
                Return iParentID
            End Get
        End Property

        Public ReadOnly Property GetDatasource(ByVal Level As Byte) As DataTable
            Get
                Return Datasource(CStr(Level)) 'String conversion to ensure it's treated as a Key
            End Get
        End Property
#End Region

        ''' <summary>
        ''' Initializes the internal DataTable collection
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub Init_Datasource()
            LevelTitle(1) = "All Parts"
            For i As Byte = 2 To MAX_DEPTH
                Datasource.Add(ptCreateTable(), i)
                LevelTitle(i) = ""
            Next
        End Sub

        ''' <summary>
        ''' Truncates the Titles to a maximum of Max_Length characters
        ''' Adds an ellipsis if too long.
        ''' </summary>
        ''' <param name="Max_Length"></param>
        ''' <remarks></remarks>
        Public Sub TrimTitles(ByRef Max_Length As Byte)
            For i As Byte = 2 To MAX_DEPTH
                If LevelTitle(i).Length > Max_Length Then
                    LevelTitle(i) = Left(LevelTitle(i), Max_Length) & "..."
                End If
            Next
        End Sub

        ''' <summary>
        ''' Clears out all the first child level and lower tables. The first child 
        ''' level will get new data repopulated. The second child level and lower
        ''' will be left blank since they should rightfully be blank.
        ''' </summary>
        ''' <param name="NewParentLevel"></param>
        ''' <remarks></remarks>
        Protected Sub updateDatasourceTables(ByRef NewParentLevel As Byte)
            LevelTitle(NewParentLevel + 1) = ptGetTypeName(iParentID)
            For i As Byte = 2 To MAX_DEPTH
                If i > NewParentLevel Then
                    Datasource.Remove(CStr(i)) 'We convert to String to force it to be recognized as a Key rather than an index
                    Datasource.Add(ptCreateTable(), i)
                End If
                If i > NewParentLevel + 1 Then
                    LevelTitle(i) = ""
                End If
            Next
        End Sub

        ''' <summary>
        ''' Updates the internal DataTables. You can access these from GetDatasource(i)
        ''' </summary>
        ''' <param name="TypeID">The TypeID the user just clicked on. This ID
        ''' will become the new parent ID</param>
        ''' <remarks></remarks>
        Public Sub updateDatasource(ByRef TypeID As Integer)
            'Query!
            Dim dt As New DataTable
            Dim sqlTxt As String = _
                "SELECT [TypeID]" & _
                "      ,[Parent]" & _
                "      ,[Path]" & _
                "      ,[Type]" & _
                "      ,[TypeNotes]" & _
                "      ,[TypeValue]" & _
                "      ,[TypeValueNotes]" & _
                "      ,[TypeValueNumeric]" & _
                "      ,[TypeUnits]" & _
                "  FROM [FriedParts].[dbo].[part-PartTypes]" & _
                " WHERE [Parent] = " & TypeID
            SelectRows(dt, sqlTxt)

            'Update internal state
            iParentID = TypeID

            'Learn the New Parent Level (this is the level that the user just clicked at)
            'Consequently we will now need to update one level below this one... and clear
            'out all the levels below that (two and below).
            Dim LevelArray As Integer() = ptPathSplit(ptGetPath(TypeID))
            Dim NewParentLevel = LevelArray.Length

            'Sanity check!
            If NewParentLevel < MAX_DEPTH Then
                'Clear out lower level tables
                updateDatasourceTables(NewParentLevel)

                'Sanity Check
                If dt.Rows.Count < 1 Then
                    'This Type/Category has no children...
                Else
                    'Populate the new 1-child table
                    For Each dr As DataRow In dt.Rows
                        Dim Path As Integer()
                        Dim newDR As DataRow
                        Path = ptPathSplit(dr.Field(Of String)("Path"))
                        If Path.Length = NewParentLevel + 1 Then
                            'Update this table!
                            newDR = DirectCast(Datasource(CStr(NewParentLevel + 1)), DataTable).NewRow 'CAREFUL! Must convert to String to ensure it gets treated as a key instead of an index
                            newDR.Item("TypeID") = dr.Field(Of Integer)("TypeID")
                            newDR.Item("Type") = dr.Field(Of String)("Type")
                            newDR.Item("TypeNotes") = dr.Field(Of String)("TypeNotes")
                            DirectCast(Datasource(CStr(NewParentLevel + 1)), DataTable).Rows.Add(newDR) 'CAREFUL! Must convert to String to ensure it gets treated as a key instead of an index
                        End If
                    Next
                End If
            End If
        End Sub

#Region "Shared Functions"
        ''' <summary>
        ''' Table schema
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ptCreateTable() As DataTable
            Dim Table1 As DataTable
            Table1 = New DataTable("TypeTable")

            'Column 0: UID
            Dim Col1 As DataColumn = New DataColumn("TypeID")
            Col1.DataType = System.Type.GetType("System.Int16")
            Table1.Columns.Add(Col1)

            'Column 1: Tab Index Number -- numbered from 0
            Col1 = New DataColumn("Type")
            Col1.DataType = System.Type.GetType("System.String")
            Table1.Columns.Add(Col1)

            'Column 2: extension
            Col1 = New DataColumn("TypeNotes")
            Col1.DataType = System.Type.GetType("System.String")
            Table1.Columns.Add(Col1)

            Return Table1
        End Function
#End Region

#Region "Construction"
        ''' <summary>
        ''' Builds a new Horizontal Part Type Accordion psuedo-control
        ''' </summary>
        ''' <param name="TypeID">The TypeID to start with -- typically 0</param>
        ''' <remarks>psuedo-controls are a Jonathan invention... when he's too lazy to reinvent the wheel and build true Server controls.</remarks>
        Public Sub New(Optional ByRef TypeID As Int32 = 0)
            Init_Datasource()
            iParentID = TypeID
            updateDatasource(iParentID)
        End Sub
#End Region

    End Class
End Namespace
