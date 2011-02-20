Imports Microsoft.VisualBasic
Imports System.Data

'THIS CLASS HANDLES ALL OF THE DATA MERGING FROM THE VARIOUS WEB API'S AND CRAWLING OPERATIONS
'DO ALL MERGE CODING IN THIS CLASS!!!

'Initially, I am exposing the api classes directly (rather than using properties) to ease 
'the development of other pages, but this should change in the future...

Public Class apiWebPart
    Public Part_Digikey As apiDigikey
    Public Part_Octopart As apiOctoparts.Octopart
    Private UserWebPage As System.Web.UI.Page
    Public Const ADD_YOUR_OWN As String = "(...or enter your own URL in the box!)"

    'Part Ready
    Public ReadOnly Property PartReady_Octopart() As Boolean
        Get
            If Part_Octopart IsNot Nothing AndAlso Part_Octopart.PartReady Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    Public ReadOnly Property PartReady_Digikey() As Boolean
        Get
            If Part_Digikey IsNot Nothing AndAlso Part_Digikey.PartReady Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property


    'DATASHEET
    Private iDatasheet As DataTable
    Public ReadOnly Property DatasheetURL_List() As DataTable
        Get
            Return iDatasheet
        End Get
    End Property
    'Used to set the fieldname in the combobox datasource
    Public Const DatasheetURL_FieldName As String = "DatasheetURL"
    'Get the Default Value (first row)
    Public ReadOnly Property DatasheetURL_Default() As String
        Get
            If Not iDatasheet Is Nothing AndAlso iDatasheet.Rows.Count > 0 Then
                Return iDatasheet.Rows(0).Field(Of String)(DatasheetURL_FieldName)
            Else
                Return ""
            End If
        End Get
    End Property

    'IMAGE
    Protected iImages As DataTable
    Public ReadOnly Property ImageURL_List() As DataTable
        Get
            Return iImages
        End Get
    End Property
    'Used to set the fieldname in the combobox datasource
    Public Const ImageURL_FieldName As String = "ImageURL"
    'Get the Default Value (first row)
    Public ReadOnly Property ImageURL_Default() As String
        Get
            If Not iImages Is Nothing AndAlso iImages.Rows.Count > 0 Then
                Return iImages.Rows(0).Field(Of String)(ImageURL_FieldName)
            Else
                Return ""
            End If
        End Get
    End Property

    'Manufacturer Name
    Private iMfr As String
    Public ReadOnly Property getManufacturer() As String
        Get
            Return iMfr
        End Get
    End Property

    'Manufacturer Part Number
    Private iMfrPN As String
    Public ReadOnly Property getMfrPartNumber() As String
        Get
            Return iMfrPN
        End Get
    End Property

    'Short Description
    Private iDesc As String
    Public ReadOnly Property getShortDesc() As String
        Get
            Return iDesc
        End Get
    End Property

    'Long Description
    Private iDescLong As String
    Public ReadOnly Property getLongDesc() As String
        Get
            Return iDescLong
        End Get
    End Property

    Public ReadOnly Property mergeReady() As Boolean
        Get
            Dim Sane As Boolean = False
            If (Not Part_Digikey Is Nothing) AndAlso (Part_Digikey.PartReady) Then Sane = True
            If (Not Part_Octopart Is Nothing) AndAlso (Part_Octopart.PartReady) Then Sane = True
            Return Sane
        End Get
    End Property

    'Common constructor
    'Instantiate WebPart by Dim blah As New apiWebPart(Me)
    Public Sub New(ByVal WhichWebPage As System.Web.UI.Page)
        UserWebPage = WhichWebPage
    End Sub


    'DATA FUSION CONDITION BLOCK CODE TEMPLATE
    'Template
    '   If (Not Part_Digikey Is Nothing) AndAlso (Part_Digikey.PartReady) Then
    '       'Digikey Prioritized Over Octopart (use Digikey only, even if both avail)
    '       << CODE HERE FOR -- USE DIGIKEY DATA WHETHER OR NOT OCTOPART DATA EXISTS >>
    '       If (Not Part_Octopart Is Nothing) AndAlso (Part_Octopart.PartReady) Then
    '           'Both Digikey AND Octopart Available
    '           << CODE HERE FOR -- MERGE CODE BECAUSE BOTH DIGIKEY AND OCTOPART DATA IS AVAIL >>
    '       Else
    '           'ONLY Digikey Available
    '           << CODE HERE FOR THE SITUATION WHEN ONLY DIGIKEY DATA IS AVAILABLE >>
    '       End If
    '   ElseIf (Not Part_Octopart Is Nothing) AndAlso (Part_Octopart.PartReady) Then
    '           'ONLY Octopart Data Available
    '           << CODE HERE FOR THE SITUATION WHEN ONLY OCTOPART DATA IS AVAILABLE >>
    '   End If

    'Fuse the digikey and octopart data
    Public Sub merge()
        'Sanity Check -- At Least ONE source must be available
        Dim Sane As Boolean = False
        If (Not Part_Digikey Is Nothing) AndAlso (Part_Digikey.PartReady) Then Sane = True
        If (Not Part_Octopart Is Nothing) AndAlso (Part_Octopart.PartReady) Then Sane = True
        If Sane = False Then
            Err.Raise(-8843, , "Cannot merge(). At least ONE web datasource must be available.")
            Exit Sub
        End If

        '[Manufacturer Information]
        If (Not Part_Digikey Is Nothing) AndAlso (Part_Digikey.PartReady) Then
            'Digikey Prioritized Over Octopart (use Digikey only, even if both avail)
            Dim mfrID As Int32 = fpMfr.mfrAdd(Part_Digikey.getMfrName, , UserWebPage) 'Find if exist, add otherwise
            If mfrID < 0 Then Err.Raise(-34234, , "Cannot merge(). MfrID is missing or not unique!")
            iMfr = fpMfr.mfrGetName(mfrID)
            iMfrPN = Part_Digikey.getMfrPartNum
            If (Not Part_Octopart Is Nothing) AndAlso (Part_Octopart.PartReady) Then
                'Both Digikey AND Octopart Available
            Else
                'ONLY Digikey Available
            End If
        ElseIf (Not Part_Octopart Is Nothing) AndAlso (Part_Octopart.PartReady) Then
            'ONLY Octopart Data Available
        End If

        '[Part Descriptions]
        If (Not Part_Digikey Is Nothing) AndAlso (Part_Digikey.PartReady) Then
            'Digikey Prioritized Over Octopart (use Digikey only, even if both avail)
            iDesc = Part_Digikey.getShortDesc()
            iDescLong = Part_Digikey.getLongDesc()
            If (Not Part_Octopart Is Nothing) AndAlso (Part_Octopart.PartReady) Then
                'Both Digikey AND Octopart Available
            Else
                'ONLY Digikey Available
            End If
        ElseIf (Not Part_Octopart Is Nothing) AndAlso (Part_Octopart.PartReady) Then
            'ONLY Octopart Data Available
            iDesc = Part_Octopart.Desc
            iDescLong = "" 'xxx ADD LATER!
        End If

        '[Datasheet]
        If Part_Octopart.PartReady Then
            'Merge the Digikey data in (if available)
            iDatasheet = Part_Octopart.DatasheetURL_List.Copy
            If Not Part_Digikey Is Nothing AndAlso Part_Digikey.PartReady Then
                Dim DRow As DataRow = iDatasheet.NewRow
                DRow.Item(0) = Part_Digikey.getDatasheetURL
                iDatasheet.Rows.InsertAt(DRow, 0)
            End If
        ElseIf Part_Digikey.PartReady Then
            If iDatasheet Is Nothing Then
                'Instantiate iDatasheet table
                iDatasheet = createTableDatasheetURL()
            End If
            Dim DRow As DataRow = iDatasheet.NewRow
            DRow.Item(0) = Part_Digikey.getDatasheetURL
            iDatasheet.Rows.InsertAt(DRow, 0)
        End If 'OP.PartReady
        'Add the Manual Option at the end
        iDatasheet.Rows.Add(ADD_YOUR_OWN) 'Add blank or separator

        '[Images]
        If Part_Octopart.PartReady Then
            'Merge the Digikey data (if available)
            iImages = Part_Octopart.ImageURL_List.Copy
            If Not Part_Digikey Is Nothing AndAlso Part_Digikey.PartReady Then
                'Add it to front of list
                Dim DRow As DataRow = iImages.NewRow
                DRow.Item(0) = Part_Digikey.getImageURL
                iImages.Rows.InsertAt(DRow, 0)
            End If
        ElseIf Part_Digikey.PartReady Then
            If iImages Is Nothing Then
                'Instantiate if empty
                iImages = createTableImageURL()
            End If
            Dim DRow As DataRow = iImages.NewRow
            DRow.Item(0) = Part_Digikey.getImageURL
            iImages.Rows.InsertAt(DRow, 0)
        End If

    End Sub 'merge()

    'Static function to create the URL Table Schema
    Public Shared Function createTableDatasheetURL() As DataTable
        Dim outTable As New DataTable
        outTable.Columns.Add(New DataColumn("DatasheetURL", GetType(String)))
        Return outTable
    End Function

    'Static function to create the URL Table Schema
    Public Shared Function createTableImageURL() As DataTable
        Dim outTable As New DataTable
        outTable.Columns.Add(New DataColumn("ImageURL", GetType(String)))
        Return outTable
    End Function
End Class
