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
        Public Const API As String = "http://octopart.com/api/v2/"

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
        ''' <summary>
        ''' The Octopart abbreviation for United States Dollars (used in pricing)
        ''' </summary>
        ''' <remarks></remarks>
        Public Const US_DOLLARS As String = "USD"

        ''' <summary>
        ''' Octopart module error codes
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum OctopartErrors As Integer
            ID_UNKNOWN = -1
        End Enum

        Public Function CreateTableUrlList() As DataTable
            'DEFINE THE TABLE
            Dim Table1 As DataTable = New DataTable("UrlTable")
            Dim col1 As DataColumn
            'Column 0: ParentID
            col1 = New DataColumn("UrlID")
            col1.DataType = System.Type.GetType("System.Int16")
            col1.AutoIncrement = True
            col1.AutoIncrementSeed = 1
            col1.AutoIncrementStep = 1
            Table1.Columns.Add(col1)
            'Column 1: MPN
            col1 = New DataColumn("URL")
            col1.DataType = System.Type.GetType("System.String")
            Table1.Columns.Add(col1)
            Return Table1
        End Function

        Public Function CreateTableMpnList() As DataTable
            'DEFINE THE TABLE
            Dim Table1 As DataTable = New DataTable("MpnTable")
            Dim col1 As DataColumn
            'Column 0: ParentID
            col1 = New DataColumn("OctoPartID")
            col1.DataType = System.Type.GetType("System.Int64")
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

        ''' <summary>
        ''' Takes in a JSON.NET Octopart API "Item" (Part object --> Item), searches all of the available descriptions,
        ''' and returns the one from Digikey if it exists.
        ''' </summary>
        ''' <param name="jsonItem">The JSON.NET object model object for this Octopart part's item namespace.</param>
        ''' <param name="ErrorOnNotFound">If true, will throw a FileNotFoundException; If false, will 
        ''' return the first description if one from Digikey is not found. If effect, when false, this tells the
        ''' function to give you the "best" description -- one from Digikey if available, or the first one if not.</param>
        ''' <returns>Text description</returns>
        ''' <remarks>ASSUMES AT LEAST ONE DESCRIPTION EXISTS AND jsonItem IS VALID!</remarks>
        Public Function GetDigikeyDescription(ByRef jsonItem As Json.Linq.JToken, Optional ByRef ErrorOnNotFound As Boolean = False) As String
            Dim jT As Json.Linq.JToken
            For i As UInt16 = 0 To DirectCast(jsonItem.Item("descriptions"), Json.Linq.JArray).Count - 1
                jT = jsonItem.SelectToken("descriptions[" & i & "]")
                If jT.Value(Of String)("credit_domain").CompareTo("digikey.com") = 0 Then
                    Return jT.Item("text").ToString
                End If
            Next
            If ErrorOnNotFound Then
                Throw New FileNotFoundException("Could not locate a Digikey description for " & jsonItem.Item("mpn").ToString)
            Else
                Return jsonItem.SelectToken("descriptions[0]").Item("text").ToString
            End If
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
