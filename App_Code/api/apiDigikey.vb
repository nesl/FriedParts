Imports Microsoft.VisualBasic
Imports MIL.Html
Imports System.Web
Imports System.Data

'DIGIKEY SPECIFIC PAGE INFORMATION
Public Class apiDigikey
    Inherits apiHTML

    Protected Const dkUrlPrefix As String = "http://search.digikey.com/scripts/DkSearch/dksus.dll?Detail&name="
    Protected Const dkDigikeyPartNum As String = "Digi-Key Part Number"
    Protected Const dkQtyAvail As String = "Quantity Available"
    Protected Const dkMfrName As String = "Manufacturer"
    Protected Const dkMPN As String = "Manufacturer Part Number"
    Protected Const dkDesc As String = "Description"
    Protected Const dkUrlImage As String = "Product Photos"
    Protected Const dkUrlDatasheet As String = "Datasheets"
    Protected Const dkTypeCategory As String = "Category"
    Protected Const dkTypeFamily As String = "Family"
    Protected Const dkLongDescStart As String = "Standard Package"

    Protected dkPartReady As Boolean

    '========================================================================================
    '== PUBLIC PROPERTIES
    '========================================================================================
    'Reports the status of this object. True when this object has part data, False 
    '   when your search did not return a result
    Public ReadOnly Property PartReady() As Boolean
        Get
            Return dkPartReady
        End Get
    End Property

    'Image URL
    Public ReadOnly Property getImageURL() As String
        Get
            If PartReady Then
                Dim dt As DataTable = dtSearch(thePageLinks, dkUrlImage)
                If dt.Rows.Count = 0 Then
                    'No Image Found!
                    Return ""
                Else
                    Return dt.Rows(0).Field(Of String)(3)
                End If
            Else
                'No Part Loaded
                Err.Raise(-42, , "No part loaded in this Digikey Object! Check apiDigikey.PartReady() before calling getValue() functions.")
                Return ""
            End If
        End Get
    End Property

    'Datasheet URL
    Public ReadOnly Property getDatasheetURL() As String
        Get
            If PartReady Then
                Dim dt As DataTable = dtSearch(thePageLinks, dkUrlDatasheet)
                If dt.Rows.Count = 0 Then
                    'No Image Found!
                    Return ""
                Else
                    Return dt.Rows(0).Field(Of String)(3)
                End If
            Else
                'No Part Loaded
                Err.Raise(-42, , "No part loaded in this Digikey Object! Check apiDigikey.PartReady() before calling getValue() functions.")
                Return ""
            End If
        End Get
    End Property

    'Manufacturer Part Number
    Public ReadOnly Property getMfrPartNum() As String
        Get
            If PartReady Then
                Dim dt As DataTable = dtSearch(thePageText, dkMPN)
                If dt.Rows.Count = 0 Then
                    'No Image Found!
                    Return ""
                Else
                    Return dt.Rows(0).Field(Of String)(2)
                End If
            Else
                'No Part Loaded
                Err.Raise(-42, , "No part loaded in this Digikey Object! Check apiDigikey.PartReady() before calling getValue() functions.")
                Return ""
            End If
        End Get
    End Property

    'Manufacturer Name
    Public ReadOnly Property getMfrName() As String
        Get
            If PartReady Then
                Dim dt As DataTable = dtSearch(thePageText, dkMfrName)
                If dt.Rows.Count = 0 Then
                    'No Image Found!
                    Return ""
                Else
                    Return Trim(dt.Rows(0).Field(Of String)(2))
                End If
            Else
                'No Part Loaded
                Err.Raise(-42, , "No part loaded in this Digikey Object! Check apiDigikey.PartReady() before calling getValue() functions.")
                Return ""
            End If
        End Get
    End Property

    'Short Description
    Public ReadOnly Property getShortDesc() As String
        Get
            If PartReady Then
                Dim dt As DataTable = dtSearch(thePageText, dkDesc)
                If dt.Rows.Count = 0 Then
                    'No Image Found!
                    Return ""
                Else
                    Return dt.Rows(0).Field(Of String)(2)
                End If
            Else
                'No Part Loaded
                Err.Raise(-42, , "No part loaded in this Digikey Object! Check apiDigikey.PartReady() before calling getValue() functions.")
                Return ""
            End If
        End Get
    End Property

    'Long Description
    Public ReadOnly Property getLongDesc() As String
        Get
            If PartReady Then
                Dim dt As DataTable = dtSearch(thePageText, dkLongDescStart, "")
                If dt.Rows.Count = 0 Then
                    'No Long Description Start Tag Found!
                    Return ""
                Else
                    Return dkTableToDesc(dt)
                End If
            Else
                'No Part Loaded
                Err.Raise(-42, , "No part loaded in this Digikey Object! Check apiDigikey.PartReady() before calling getValue() functions.")
                Return ""
            End If
        End Get
    End Property

    'Digikey Part Number
    Public ReadOnly Property getDigikeyPartNum() As String
        Get
            If PartReady Then
                Dim dt As DataTable = dtSearch(thePageText, dkDigikeyPartNum)
                If dt.Rows.Count = 0 Then
                    'No Image Found!
                    Return ""
                Else
                    Return dt.Rows(0).Field(Of String)(2)
                End If
            Else
                'No Part Loaded
                Err.Raise(-42, , "No part loaded in this Digikey Object! Check apiDigikey.PartReady() before calling getValue() functions.")
                Return ""
            End If
        End Get
    End Property

    'Part Type "Category" (This is the parent type)
    Public ReadOnly Property getTypeCategory() As String
        Get
            If PartReady Then
                Dim dt As DataTable = dtSearch(thePageText, dkTypeCategory)
                If dt.Rows.Count = 0 Then
                    'Not Found!
                    Return ""
                Else
                    Return dt.Rows(0).Field(Of String)(2)
                End If
            Else
                'No Part Loaded
                Err.Raise(-42, , "No part loaded in this Digikey Object! Check apiDigikey.PartReady() before calling getValue() functions.")
                Return ""
            End If
        End Get
    End Property

    'Part Type "Family" (This is the child type)
    Public ReadOnly Property getTypeFamily() As String
        Get
            If PartReady Then
                Dim dt As DataTable = dtSearch(thePageText, dkTypeFamily)
                If dt.Rows.Count = 0 Then
                    'Not Found!
                    Return ""
                Else
                    Return dt.Rows(0).Field(Of String)(2)
                End If
            Else
                'No Part Loaded
                Err.Raise(-42, , "No part loaded in this Digikey Object! Check apiDigikey.PartReady() before calling getValue() functions.")
                Return ""
            End If
        End Get
    End Property



    '========================================================================================
    '== CONSTRUCTION / DESTRUCTION
    '========================================================================================
    Public Sub New(ByVal DigikeyPartNumber As String, Optional ByVal TimeoutSeconds As Integer = 10)
        'Constructor!
        MyBase.New(dkUrlPrefix & System.Web.HttpContext.Current.Server.UrlEncode(DigikeyPartNumber), TimeoutSeconds)

        'Part Ready?
        Dim dt As DataTable
        dt = dtSearch(Me.thePageText, dkDigikeyPartNum)
        If dt.Rows.Count > 0 Then
            dkPartReady = True
        Else
            dkPartReady = False
        End If

        'Make "Col1" the Primary Key, therefore searchable with the "find" method
        'All this syntax is necessary to convert a column to a 1-D array of columns
        'pageText.PrimaryKey() = New DataColumn() {pageText.Columns(1)}
        'pageLinks.PrimaryKey() = New DataColumn() {pageLinks.Columns(1)}

    End Sub

    '========================================================================================
    '== SHARED METHODS
    '========================================================================================
    'Gets the Digikey part number from a FriedParts part number if it exists
    'Returns the part number or sysErrors.ERR_NOTFOUND
    Public Shared Function dkPartNumber(ByVal fpPartID As Int16) As String
        Dim dkID As Int32() = distExistsID("Digikey", True)
        Dim sqlTxt As String = _
            "SELECT " & _
            "      [DistPartNum]" & _
            "  FROM [FriedParts].[dbo].[view-distParts]" & _
            " WHERE [PartID] = " & fpPartID & "" & _
            " AND [DistID] = " & dkID(0)
        Dim dt As New DataTable
        dt = dbAcc.SelectRows(dt, sqlTxt)
        If dt.Rows.Count = 0 Then
            Return sysErrors.ERR_NOTFOUND
        ElseIf dt.Rows.Count > 1 Then
            Return sysErrors.ERR_NOTUNIQUE
        Else
            'Found a match!
            Return dt.Rows(0).Field(Of String)("DistPartNum")
        End If
        Return sysErrors.ERR_NOTFOUND
    End Function

    'Takes in a dtSearch results formatted table and returns a string representation
    '...the long description of the part
    Public Shared Function dkTableToDesc(ByRef dt As DataTable) As String
        Dim outTxt As String = ""
        For Each dr As DataRow In dt.Rows
            outTxt = outTxt & dr.Field(Of String)(1) & " == " & dr.Field(Of String)(2) & vbCrLf
        Next
        Return Trim(outTxt)
    End Function
End Class
