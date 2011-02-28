Imports Microsoft.VisualBasic
Imports System.Data
Imports System

'MODULE PREFIX: pg -- Part Good
Public Class fpPartValidation
    '========================================================================================
    '== PRIVATE COMMON VARIABLES
    '========================================================================================
    Protected pgDT As DataTable
    Protected pgErrors As Boolean

    '========================================================================================
    '== PROPERTIES
    '========================================================================================
    Public ReadOnly Property GetDataSource() As DataTable
        Get
            Return pgDT
        End Get
    End Property

    'Returns true if no errors
    Public ReadOnly Property PartValid() As Boolean
        Get
            Return Not pgErrors
        End Get
    End Property


    '========================================================================================
    '== CONSTRUCTION / DESTRUCTION
    '========================================================================================
    'Default Constructor
    Public Sub New()
        'Constructor!
        pgDT = pgDataSource()
        pgErrors = False
    End Sub

    '========================================================================================
    '== SHARED METHODS
    '========================================================================================

    Private Function pgDataSource() As DataTable
        Dim Table1 As DataTable
        Table1 = New DataTable("ValidationErrors")

        'Column 0: UID
        Dim Col1 As DataColumn = New DataColumn("UID")
        Col1.DataType = System.Type.GetType("System.Int16")
        Col1.AutoIncrement = True
        Col1.AutoIncrementSeed = 1
        Col1.AutoIncrementStep = 1
        Table1.Columns.Add(Col1)

        'Column 1: Tab Index Number -- numbered from 0
        Col1 = New DataColumn("Step")
        Col1.DataType = System.Type.GetType("System.Byte")
        Table1.Columns.Add(Col1)

        'Column 2: extension
        Col1 = New DataColumn("Code")
        Col1.DataType = System.Type.GetType("System.Int16")
        Table1.Columns.Add(Col1)

        'Column 3: path
        Col1 = New DataColumn("Error")
        Col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(Col1)

        Return Table1
    End Function

    '========================================================================================
    '== METHODS
    '========================================================================================

    Public Sub AddError(ByVal OnTabPage As Byte, ByVal ErrCode As Int16, ByVal ErrMsg As String)
        Dim Row1 As DataRow = pgDT.NewRow()
        Row1.Item("Step") = OnTabPage
        Row1.Item("Code") = ErrCode
        Row1.Item("Error") = ErrMsg
        pgDT.Rows.Add(Row1)
        pgErrors = True
    End Sub
End Class
