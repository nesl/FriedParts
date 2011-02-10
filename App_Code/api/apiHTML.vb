Imports Microsoft.VisualBasic
Imports System.Data
Imports MIL.Html
Imports System.Diagnostics

Public Class apiHTML
    '========================================================================================
    '== LOCAL VARIABLES
    '========================================================================================
    Protected pageText As DataTable
    Protected pageLinks As DataTable
    Protected pageDoc As MIL.Html.HtmlDocument
    Protected pageURL As String

    '========================================================================================
    '== PUBLIC PROPERTIES
    '========================================================================================
    Public ReadOnly Property thePageText() As DataTable
        Get
            Return pageText
        End Get
    End Property

    Public ReadOnly Property thePageLinks() As DataTable
        Get
            Return pageLinks
        End Get
    End Property

    Public ReadOnly Property thePage() As MIL.Html.HtmlDocument
        Get
            Return pageDoc
        End Get
    End Property

    Public ReadOnly Property theURL() As String
        Get
            Return pageURL
        End Get
    End Property

    '========================================================================================
    '== CONSTRUCTION / DESTRUCTION
    '========================================================================================
    Public Sub New(ByVal URL As String, Optional ByVal TimeoutSeconds As Integer = 10)
        'Constructor!
        pageURL = URL
        pageDoc = MIL.Html.HtmlDocument.Create(getPageHTML(URL, TimeoutSeconds), False)
        pageLinks = htmlCreateLinkTable() 'Creates schema, but is populated by getTableRows() implicitly
        pageText = getTableRows(pageDoc.Nodes)
    End Sub

    Public Sub New()
        'Default Constructor!
        Err.Raise(-276565, , "Cannot construct! Invalid parameters! apiHTML classes must be called with a URL.")
    End Sub

    '========================================================================================
    '== PUBLIC METHODS
    '========================================================================================
    'returns the first valid link from HTML text
    Public Function htmlExtractURL(ByVal HTML_text As String) As String
        Dim idxStart As Int16
        Dim idxStop As Int16
        Dim HtmlText As String
        HtmlText = UCase(HTML_text) 'upcase to make things case agnostic
        idxStart = HtmlText.IndexOf("HREF=")
        If idxStart = -1 Then
            'Not Found!
            Return ""
        Else
            idxStart = idxStart + 6
        End If
        idxStop = HtmlText.Substring(idxStart).IndexOf(Chr(34))
        Debug.WriteLine("xxx--- " & HtmlText.Substring(idxStart, idxStop))
        Return HtmlText.Substring(idxStart, idxStop)
    End Function

    'From Karl Moore
    'http://www.developer.com/services/article.php/3113371/How-to-Snatch-HTML-Using-Visual-Basic-Code.htm
    Public Shared Function getPageHTML(ByVal URL As String, _
      Optional ByVal TimeoutSeconds As Integer = 10) _
      As String
        ' Retrieves the HTML from the specified URL,
        ' using a default timeout of 10 seconds
        Dim objRequest As Net.WebRequest
        Dim objResponse As Net.WebResponse
        Dim objStreamReceive As System.IO.Stream
        Dim objEncoding As System.Text.Encoding
        Dim objStreamRead As System.IO.StreamReader

        Try
            ' Setup our Web request
            objRequest = Net.WebRequest.Create(URL)
            objRequest.Timeout = TimeoutSeconds * 1000
            ' Retrieve data from request
            objResponse = objRequest.GetResponse
            objStreamReceive = objResponse.GetResponseStream
            objEncoding = System.Text.Encoding.GetEncoding( _
                "utf-8")
            objStreamRead = New System.IO.StreamReader( _
                objStreamReceive, objEncoding)
            ' Set function return value
            getPageHTML = objStreamRead.ReadToEnd()
            ' Check if available, then close response
            If Not objResponse Is Nothing Then
                objResponse.Close()
            End If
        Catch
            ' Error occured grabbing data, simply return nothing
            Return ""
        End Try
    End Function

    '========================================================================================
    '== NON-PRIMARY KEY TABLE SEARCH
    '========================================================================================

    'Searchs the first column of the passed in DataTable and returns a DataTable containing only 
    '   the rows that matched the SearchTxt string.
    'The range-search option
    '   If the third parameter is specified all rows of the source data table are returned between 
    '   (and including) the row whose 2nd column matches the SearchTxt and up to
    '   (but not including) the row whose 2nd column matches the EndOfRangeTxt or
    '   the end of the table (whichever comes first)
    Protected Function dtSearch(ByRef dt As DataTable, ByVal SearchTxt As String, Optional ByVal EndOfRangeTxt As String = Nothing) As DataTable
        Dim FindRange As Boolean = False
        Dim InRange As Boolean = False
        Dim dtOut As DataTable

        'Look for a range of records?
        If Not EndOfRangeTxt Is Nothing Then
            FindRange = True
        End If

        dtOut = dt.Clone
        For Each dr As DataRow In dt.Rows
            If InRange Then
                'Check for Terminating Condition
                If Trim(dr.Field(Of String)(1)) = EndOfRangeTxt Then
                    InRange = False
                Else 'Else: add this row!
                    Dim dr2 As DataRow = dtOut.NewRow
                    dr2.ItemArray = dr.ItemArray
                    dtOut.Rows.Add(dr2)
                End If
            ElseIf Trim(dr.Field(Of String)(1)) = SearchTxt Then
                'Munchkin matched! Found a row!
                If FindRange Then InRange = True 'Deal with range searches
                Dim dr2 As DataRow = dtOut.NewRow
                dr2.ItemArray = dr.ItemArray
                dtOut.Rows.Add(dr2)
            End If
        Next
        Return dtOut
    End Function

    '========================================================================================
    '== HTMLDOCUMENT PROCESSING
    '========================================================================================

    'Returns a DataTable object containing the text of all table cell contents
    'Usage: getTableRows(apiDigikey.thePage.Nodes)
    Private Function getTableRows(ByRef RootNodes As MIL.Html.HtmlNodeCollection) As System.Data.DataTable
        Debug.WriteLine("")
        Debug.WriteLine("")
        Debug.WriteLine("[getTableRows] Start!")
        Dim dt As DataTable = htmlCreateRowTable()
        Dim RootNode As HtmlNode
        'Search for head of tree
        For Each RootNode In RootNodes
            If RootNode.IsElement Then
                If UCase(CType(RootNode, HtmlElement).Name) = "BODY" Then
                    Debug.WriteLine("--- Is element! Found body! Yay!")
                    Return getTableRows(RootNode, dt)
                Else
                    Debug.WriteLine("--- Is element! Not body :(")
                End If
            Else
                Debug.WriteLine("--- Not element")
            End If
        Next
        Return dt
    End Function

    'Helper function -- implements DFS via recursion
    Private Function getTableRows(ByRef RootNode As MIL.Html.HtmlElement, ByVal dt As DataTable) As System.Data.DataTable
        'THIS NODE!
        If UCase(RootNode.Name) = "TR" Then
            'Table Row found at this node! (enumerate columns!)
            Dim newRow As DataRow
            Dim m As HtmlNode
            Dim n As HtmlElement
            Dim i As Int16 = 0
            newRow = dt.NewRow()
            For Each m In RootNode.Nodes
                If m.IsElement Then
                    n = CType(m, HtmlElement)
                    If UCase(n.Name) = "TD" Or UCase(n.Name) = "TH" Then
                        i = i + 1 'Pre-increment
                        If n.Nodes.Count > 0 Then
                            Dim aNode As HtmlElement
                            Dim theNode As HtmlNode
                            theNode = n.Nodes(0)
                            If theNode.IsElement Then
                                aNode = CType(theNode, HtmlElement)
                                If UCase(aNode.Name) = "A" Then
                                    'Found a link! Add it's text to the page text table
                                    newRow.Item(i) = aNode.Text
                                    '...and its text and link to the links table
                                    Dim newLinkRow As DataRow
                                    newLinkRow = pageLinks.NewRow()
                                    newLinkRow.Item(0) = RootNode.GetHashCode()
                                    newLinkRow.Item(1) = newRow.Item(i - 1)
                                    newLinkRow.Item(2) = aNode.Text
                                    newLinkRow.Item(3) = htmlExtractURL(aNode.HTML)
                                    pageLinks.Rows.Add(newLinkRow)
                                    'Debug.WriteLine("-----L " & aNode.Text & " [Text]: " & aNode.HTML & " " & n.IsElement)
                                End If
                            Else
                                newRow.Item(i) = n.Text
                                Debug.WriteLine("------ " & n.Name & " [Text]: " & n.Text & " " & n.IsElement)
                            End If
                        End If
                    End If
                End If
            Next
            If i > 0 Then
                'Add the row if it contains anything
                newRow.Item(0) = RootNode.GetHashCode()
                dt.Rows.Add(newRow)
            End If
        End If

        'CHILDREN NODES!
        Dim nchild As HtmlNode
        For Each nchild In RootNode.Nodes
            If nchild.IsElement Then
                Dim next_dt As DataTable
                next_dt = getTableRows(CType(nchild, HtmlElement), dt)
                'Debug.WriteLine("---next_dt.rows.count: " & next_dt.Rows.Count & " [Hash Code] " & RootNode.GetHashCode())
                dt = mergeTables(dt, next_dt, "ParentID")
            End If
        Next

        'DONE!
        Return dt
    End Function

    '========================================================================================
    '== TABLE SCHEMA
    '========================================================================================

    'Creates the table schema
    Private Function htmlCreateRowTable() As Data.DataTable
        Dim Table1 As DataTable
        Table1 = New DataTable("TextTable")
        'Init
        Dim col1 As DataColumn
        'Column 0: ParentID
        col1 = New DataColumn("ParentID")
        col1.DataType = System.Type.GetType("System.Int32")
        Table1.Columns.Add(col1)
        'Column 1: filename
        col1 = New DataColumn("Col1")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        'Column 2: extension
        col1 = New DataColumn("Col2")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        'Column 3: path
        col1 = New DataColumn("Col3")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        'Column 3: path
        col1 = New DataColumn("Col4")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        'Column 3: path
        col1 = New DataColumn("Col5")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        'Column 3: path
        col1 = New DataColumn("Col6")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        'Column 3: path
        col1 = New DataColumn("Col7")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        'Column 3: path
        col1 = New DataColumn("Col8")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        'Column 3: path
        col1 = New DataColumn("Col9")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        'Column 3: path
        col1 = New DataColumn("Col10")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        'Column 3: path
        col1 = New DataColumn("Col11")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        Return Table1
    End Function

    'Creates the table schema
    Private Function htmlCreateLinkTable() As Data.DataTable
        Dim Table1 As DataTable
        Table1 = New DataTable("LinkTable")
        'Init
        Dim col1 As DataColumn
        'Column 0: ParentID
        col1 = New DataColumn("ParentID")
        col1.DataType = System.Type.GetType("System.Int32")
        Table1.Columns.Add(col1)
        'Column 1: Field Name
        col1 = New DataColumn("FieldName")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        'Column 2: Link Text
        col1 = New DataColumn("Text")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        'Column 3: Link URL
        col1 = New DataColumn("Link")
        col1.DataType = System.Type.GetType("System.String")
        Table1.Columns.Add(col1)
        Return Table1
    End Function
End Class
