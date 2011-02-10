Imports System
Imports System.Net
Imports System.IO
Imports apiOctoparts
Imports System.Web.UI.WebControls
Imports System.Data
Imports System.Collections
Imports DevExpress.Web.ASPxTreeList
Imports DevExpress.Web.ASPxGridView

Partial Class pDevel_devCrawler
    Inherits System.Web.UI.Page

    'Variables used here
    Private source As String
    Private keyword As String
    Private OP As Octopart

    'This text box will do SUGGESTIVE SEARCH to give possible variations of mpn
    Protected Sub FuzzySearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles FuzzySearch.Click
        'TextBox1.TextChanged
        'Clear source
        source = ""

        'Format the query
        Dim url As String = TextBox1.Text
        Dim startQ As String = "http://octopart.com/api/v2/parts/search?q="
        url = startQ & url  'Append the user's search request to the end of the string

        'Get the source
        Dim myWebClient As New WebClient()
        source = myWebClient.DownloadString(url)    'Source is the text received from OCTOPARTS website

        Dim k As String = "start!"
        Dim stringMeUp As String = ""
        Dim MPNList As New Collection

        Do Until k = ENDOFSOURCE
            k = fetch(source)
            Select Case k
                Case "mpn"
                    k = fetch(source)
                    stringMeUp = stringMeUp & "  " & k & "  "
                    MPNList.Add(k)
            End Select
        Loop

        ResultofTextBox1.Text = stringMeUp

        'set parameters of drop down list
        mpnDropDown.AutoPostBack = True

        'add event handler
        '...

        'create data table
        Dim dt As DataTable = New DataTable()
        dt.Columns.Add(New DataColumn("MPNList", GetType(String)))

        'populate the table with values
        Dim MPNName As String
        dt.Rows.Add("")
        For Each MPNName In MPNList
            dt.Rows.Add(MPNName)
        Next

        'create a DataView from DataTable to act as the data source for mpnDropDown
        Dim dv As DataView = New DataView(dt)

        'Because the mpnDropDown control is created dynamically each 
        'time the page is loaded, the data must be bound to the
        'control each time the page is refreshed.

        'Specify the data source and field names for the MPNList
        'properties of the item (ListItem objects) in the 
        'mpnDropDown control.
        mpnDropDown.DataSource = dv
        mpnDropDown.DataTextField = "MPNList"
        mpnDropDown.DataValueField = "MPNList"

        'Bind the data to the control
        mpnDropDown.DataBind()

        'Set the default selected item when the page is first loaded
        If Not IsPostBack Then
            mpnDropDown.SelectedIndex = 0
        End If

    End Sub

    'This textbox will do a SPECIFIC SEARCH of a mpn and return the data for it, using octopart api.
    Protected Sub mpnDropDown_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles mpnDropDown.TextChanged

        'Restart image select panel
        ASPxSelectedImage.ClientVisible = "False"
        ASPxSelectedImage.ImageURL = "null"
        ImageDataView.ClientVisible = "True"
        ASPxReselectImage.ClientVisible = "False"

        'New part variable
        If OP Is Nothing Then
            OP = New Octopart(mpnDropDown.Text)
            OP.switch(mpnDropDown.Text)
        Else
            OP.switch(mpnDropDown.Text)
        End If

        If OP.PartReady = False Then
            Err.Raise(-23434, , "Part Not Ready! IT SHOULD HAVE BEEN!")
        End If

        'MANUFACTURER
        manuText.Text = OP.Manufacturer()

        'MPN
        mpnText.Text = OP.MPN()
        'DESCRIPTIONS
        descriptionsText.Text = OP.Desc()

        Dim blah As String = ""
        Dim blahblah As String = ""

        'AVERAGE AVAILABLE
        avg_avail.Text = OP.Avg_Avail()

        'NUMBER OF SUPPLIERS
        num_suppliers.Text = OP.Num_Suppliers()

        'SPECS METADATA
        blahblah = ""
        For Each blah In OP.SpecsMeta()
            If blahblah = "" Then
                blahblah = blah
            Else
                blahblah = blahblah & ";    " & blah
            End If
        Next
        specsMetaText.Text = blahblah

        'SPECS
        blahblah = ""
        For Each blah In OP.Specs()
            If blahblah = "" Then
                blahblah = blah
            Else
                blahblah = blahblah & ";    " & blah
            End If
        Next
        specsText.Text = blahblah

        'DATASHEETS
        blahblah = ""
        For Each blah In OP.Datasheets()
            If blahblah = "" Then
                blahblah = blah
            Else
                blahblah = blahblah & "; " & blah
            End If
        Next
        datasheetsText.Text = blahblah

        'IMAGES
        blahblah = ""
        Dim iURL As DataTable = New DataTable()
        iURL.Columns.Add(New DataColumn("ImageURL", GetType(String)))
        For Each blah In OP.Images()
            If blahblah = "" Then
                blahblah = blah
                iURL.Rows.Add(blah)
            Else
                blahblah = blahblah & "; " & blah
                iURL.Rows.Add(blah)
            End If
        Next
        imagesText.Text = blahblah
        ImageDataView.DataSource = iURL
        ImageDataView.DataBind()
       


        'OFFERS METADATA
        'blahblah = ""
        'For Each blah In OP.OffersMeta()
        '    If blahblah = "" Then
        '        blahblah = blah
        '    Else
        '        blahblah = blahblah & "; " & blah
        '    End If
        'Next
        'offersMetaText.Text = blahblah

        'OFFERS
        'blahblah = ""
        'Dim blah2 As Distributor
        'For Each blah2 In OP.Offers()
        '    If blahblah = "" Then
        '        blahblah = OP.lstDist(blah2)
        '    Else
        '        blahblah = blahblah & vbCrLf & OP.lstDist(blah2)
        '    End If
        'Next
        'offersText.Text = blahblah

        'DEBUGGING
        debugText.Text = OP.ToString

    End Sub

    'The Digikey Development Button
    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim doc As New apiDigikey(textboxDK.Text)
        xGridWeb.DataSource = doc.thePageText
        xGridWeb.DataBind()
        xGridWebLinks.DataSource = doc.thePageLinks
        xGridWebLinks.DataBind()
    End Sub

    Protected Sub ImageBtn_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        ASPxSelectedImage.ClientVisible = "True"
        ASPxSelectedImage.ImageURL = e.CommandName
        ImageDataView.ClientVisible = "False"
        ASPxReselectImage.ClientVisible = "True"
    End Sub

    Protected Sub ReselectBtn_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        ASPxSelectedImage.ClientVisible = "False"
        ASPxSelectedImage.ImageURL = "null"
        ImageDataView.ClientVisible = "True"
        ASPxReselectImage.ClientVisible = "False"
    End Sub
End Class
