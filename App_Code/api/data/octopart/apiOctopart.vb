Imports Microsoft.VisualBasic
Imports System
Imports System.Net
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Char
Imports System.Environment
Imports System.Collections
Imports System.String
Imports System.Data
Imports Newtonsoft

Namespace apiOctopart

    ''' <summary>
    ''' Encapsulates all of the Octopart API (REST URLs)
    ''' </summary>
    ''' <remarks>Usage: OctoCommands.API &amp; OctoCommands.(the command) &amp; (string parameters)</remarks>
    Public Module OctoCommands
        ''' <summary>
        ''' The API interaction point. Web address of the REST API base.
        ''' </summary>
        ''' <remarks></remarks>
        Public Const API As String = "http://octopart.com/api/v2/parts/search?q="

        ''' <summary>
        ''' Perform a parts search!
        ''' </summary>
        ''' <remarks></remarks>
        Public Const PartSearch As String = "parts/search?q="
    End Module

    ''' <summary>
    ''' This class contains all of the static and common content to 
    ''' Octopart API interactions.
    ''' </summary>
    ''' <remarks></remarks>
    Public Module OctopartStatic

        Public Enum OctopartErrors As Integer
            ID_UNKNOWN = -1
        End Enum

        Public Function CreateTableMpnList() As DataTable
            'DEFINE THE TABLE
            Dim Table1 As DataTable = New DataTable("MpnTable")
            Dim col1 As DataColumn
            'Column 0: ParentID
            col1 = New DataColumn("OctopartID")
            col1.DataType = System.Type.GetType("System.Int32")
            Table1.Columns.Add(col1)
            'Column 1: MPN
            col1 = New DataColumn("MfrPartNum")
            col1.DataType = System.Type.GetType("System.String")
            Table1.Columns.Add(col1)
            'Column 2: short description
            col1 = New DataColumn("Highlight")
            col1.DataType = System.Type.GetType("System.String")
            Table1.Columns.Add(col1)
            'Column 3: detail
            col1 = New DataColumn("Description")
            col1.DataType = System.Type.GetType("System.String")
            Table1.Columns.Add(col1)
            Return Table1
        End Function

    End Module

    '==========================
    '==  CLASS DISTRIBUTOR   ==
    '==========================
    'Mfr_List block
    Public Class Distributor
        Public sku As String
        Public avail As String
        Public sendrfq_url As String
        Public prices As String
        Public clickthrough_url As String
        Public displayname As String
        Public buynow_url As String
    End Class

End Namespace
