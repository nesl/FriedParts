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

'VB.NET Happy Octopart, Inc. API
'By Newton Truong
Public Module apiOctoparts
    'Web addresses
    Private Const OCTOPART_URL_PREFIX As String = "http://octopart.com/api/v2/parts/search?q="
    'Finish Parsing Tag
    Public Const ENDOFSOURCE As String = "ENDOFSOURCE"

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

    '=======================
    '==  CLASS OCTOPART   ==
    '=======================
    Public Class Octopart
        'Internal Variables
        Private json As String        'Raw text of webpage (JSON format)
        Private Octopart_ToString As String     'Debug style summary of the object parsing

        'Public Properties
        Protected table_MpnList As DataTable
        Public ReadOnly Property MPN_List() As DataTable
            Get
                Return table_MpnList
            End Get
        End Property

        Protected table_DatasheetUrl As DataTable
        Public ReadOnly Property DatasheetURL_List() As DataTable
            Get
                Return table_DatasheetUrl
            End Get
        End Property
        'Used to set the fieldname in the combobox datasource
        Public ReadOnly Property DatsheetURL_FieldName() As String
            Get
                Return "DatasheetURL"
            End Get
        End Property

        Protected table_ImageUrl As DataTable
        Public ReadOnly Property ImageURL_List() As DataTable
            Get
                Return table_ImageUrl
            End Get
        End Property
        'Used to set the fieldname in the combobox datasource
        Public ReadOnly Property ImageURL_FieldName() As String
            Get
                Return "ImageURL"
            End Get
        End Property

        Protected MpnLoaded As Boolean         'MPN data loaded?
        Public ReadOnly Property PartReady() As Boolean
            Get
                Return MpnLoaded
            End Get
        End Property

        Protected lmpn As String                    'Manufacturer Part Number
        Public ReadOnly Property MPN() As String
            Get
                Return lmpn
            End Get
        End Property

        Protected ldesc As String                   'Descriptions
        Public ReadOnly Property Desc() As String
            Get
                Return ldesc
            End Get
        End Property

        Protected lavg_Avail As String              'Average distributors available
        Public ReadOnly Property avg_Avail() As String
            Get
                Return lavg_Avail
            End Get
        End Property

        Protected lnum_Suppliers As String          'Number of distributors
        Public ReadOnly Property Num_Suppliers() As String
            Get
                Return lnum_Suppliers
            End Get
        End Property

        Protected lspecs As New Collection          'Specification data of part(key = specMeta)
        Public ReadOnly Property Specs() As Collection
            Get
                Return lspecs
            End Get
        End Property

        Protected lspecsMeta As New Collection      'Specification metadata (list specs data available)
        Public ReadOnly Property SpecsMeta() As Collection
            Get
                Return lspecsMeta
            End Get
        End Property

        Protected ldatasheets As New Collection     'Datasheet url links
        Public ReadOnly Property Datasheets() As Collection
            Get
                Return ldatasheets
            End Get
        End Property

        Protected lmfr_List As New Collection         'Distributor data (key = mfr_ListMeta)
        Public ReadOnly Property Mfr_List() As Collection
            Get
                Return lmfr_List
            End Get
        End Property

        Protected lmfr_ListMeta As New Collection     'Distributor metadata (list distributors)
        Public ReadOnly Property mfr_ListMeta() As Collection
            Get
                Return lmfr_ListMeta
            End Get
        End Property

        Protected limages As New Collection         'Image links
        Public ReadOnly Property Images() As Collection
            Get
                Return limages
            End Get
        End Property

        Protected ldetail_Url As String             'Octopart URL
        Public ReadOnly Property Detail_URL() As String
            Get
                Return ldetail_Url
            End Get
        End Property

        Protected lmanufacturer As String           'Manufacturer name
        Public ReadOnly Property Manufacturer() As String
            Get
                Return lmanufacturer
            End Get
        End Property

        'Parser State Machine States
        Private Enum ParserStates As Byte
            Top_Level = 0
            Item_Level = 1
            Descriptions_Level = 2
        End Enum

        'Constructor
        Public Sub New()
            Err.Raise(-234324, , "Class Octopart cannot be constructed with default (e.g. 0) parameters")
        End Sub

        Public Sub New(ByVal PartToSearch As String)
            'Format the query
            Dim URL As String = OCTOPART_URL_PREFIX & _
                                HttpContext.Current.Server.UrlEncode(PartToSearch) _
                                'Append the user's search request to the end of the string
            'Get the source
            Dim myWebClient As New WebClient()
            Dim Source As String = myWebClient.DownloadString(URL)    'Source is the text received from OCTOPARTS website
            json = Source 'Save the raw page text for later...

            'Find out how many MPN's match these search terms
            parseMpnTable()

            'Process the part... (if only one matching mpn to query, populate me, otherwise will not do much)
            If MPN_List.Rows.Count = 1 Then
                'Pass "Source" not "json" because fetch functions consume it
                Octopart_ToString = fetchBlock(Source, MPN_List.Rows(0).Field(Of String)("MfrPartNum"))
                MpnLoaded = True
            Else
                MpnLoaded = False
            End If
        End Sub

        'Loads the Image and Datasheet URL Tables
        'Call on loaded part changed
        Protected Sub Load_URL_Tables()
            '[DATASHEET]
            table_DatasheetUrl = apiWebPart.createTableDatasheetURL()

            'Octoparts
            For Each urlstring In ldatasheets
                table_DatasheetUrl.Rows.Add(urlstring)
            Next

            '[IMAGES]
            table_ImageUrl = apiWebPart.createTableImageURL()

            'Octoparts
            For Each urlstring In limages
                table_ImageUrl.Rows.Add(urlstring)
            Next

        End Sub

        'Clears the specific selected manufacturer part number data, without 
        '   requiring another trip to the Octopart server. Use this to switch 
        '   between MPN's when multiple match the initial query.
        Public Sub clear()
            MpnLoaded = False
            Me.lmpn = ""
            Me.ldesc = ""
            Me.lavg_Avail = ""
            Me.lnum_Suppliers = ""
            Me.lspecs.Clear()
            Me.lspecsMeta.Clear()
            Me.ldatasheets.Clear()
            Me.lmfr_List.Clear()
            Me.lmfr_ListMeta.Clear()
            Me.limages.Clear()
            Me.ldetail_Url = ""
            Me.lmanufacturer = ""
            table_DatasheetUrl = Nothing
            table_ImageUrl = Nothing
        End Sub

        'Switches the specific selected manufacturer part number data, without 
        '   requiring another trip to the Octopart server. Use this to switch 
        '   between MPN's when multiple match the initial query.
        Public Sub switch(ByVal ManufacturerPartNumber As String)
            'Process the part... (if only one matching mpn to query, populate me, otherwise will not do much)
            Me.clear() 'clear out the current part so that we may replace it
            If MPN_List.Rows.Count > 0 Then
                Dim source As String = json 'must pass in a copy because the fetch functions consume the string
                Octopart_ToString = fetchBlock(source, ManufacturerPartNumber)
                Load_URL_Tables()
                MpnLoaded = True
            End If
        End Sub

        'Meaningful ToString function!
        Public Overrides Function ToString() As String
            Return Octopart_ToString
        End Function

        'Parses the webpage to create the MPN_List datatable object
        Private Sub parseMpnTable()

            'DEFINE THE TABLE

            Dim Table1 As DataTable = New DataTable("MpnTable")
            Dim col1 As DataColumn
            'Column 0: ParentID
            col1 = New DataColumn("MpnID")
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



            'POPULATE THE TABLE

            Dim Source As String = json
            Dim K As String
            Dim parserState As ParserStates = ParserStates.Top_Level
            Dim dr As DataRow
            Dim item_counter As Byte = 0

            K = fetch(Source)
            Do Until K = ENDOFSOURCE
                K = fetch(Source)
                Select Case parserState
                    Case ParserStates.Top_Level
                        'Normal level to find "item"
                        Select Case K
                            Case "item"
                                item_counter = item_counter + 1
                                If Not dr Is Nothing Then
                                    'Save previous item data if it exists
                                    Table1.Rows.Add(dr)
                                End If
                                dr = Table1.NewRow()
                                dr.Item(0) = item_counter 'Row UID
                                parserState = ParserStates.Item_Level
                            Case "descriptions"
                                'Oh no! Something is out of sync! Stay at top level!
                                Err.Raise(-123, , "@Top Level, found Descriptions")
                            Case "mpn"
                                Err.Raise(-124, , "@Top Leve, found Mpn")
                            Case Else
                                'Keep looking...
                        End Select
                    Case ParserStates.Item_Level
                        'Normal level to find "descriptions", "highlight" and "mpn"
                        Select Case K
                            Case "item"
                                'Next ITEM!
                                item_counter = item_counter + 1
                                If Not dr Is Nothing Then
                                    'Save previous item data if it exists
                                    Table1.Rows.Add(dr)
                                End If
                                dr = Table1.NewRow()
                                dr.Item(0) = item_counter 'Row UID
                                parserState = ParserStates.Item_Level
                            Case "descriptions"
                                parserState = ParserStates.Descriptions_Level
                            Case "mpn"
                                K = fetch(Source)
                                dr.Item(1) = K 'MPN added to row
                            Case "highlight"
                                K = fetch(Source)
                                dr.Item(2) = K 'Short description added to row
                            Case Else
                        End Select
                    Case ParserStates.Descriptions_Level
                        'Normal level to find "text"
                        Select Case K
                            Case "text"
                                K = fetch(Source)
                                dr.Item(3) = K
                                parserState = ParserStates.Item_Level
                            Case "item"
                            Case "descriptions"
                                Err.Raise(-130, , "@Desc Level, found Desc")
                            Case "mpn"
                                'Found an Item-Level property! Must
                                'mean that the Description was empty
                                parserState = ParserStates.Item_Level 'Go back to item-level to recover
                                K = fetch(Source)
                                dr.Item(1) = K 'MPN added to row
                            Case "highlight"
                                'Found an Item-Level property! Must
                                'mean that the Description was empty
                                parserState = ParserStates.Item_Level 'Go back to item-level to recover
                                K = fetch(Source)
                                dr.Item(2) = K 'Short description added to row
                            Case Else
                        End Select
                End Select
            Loop

            'Save last item
            If Not dr Is Nothing Then
                'Save previous item data if it exists
                Table1.Rows.Add(dr)
            End If

            'RETURN TABLE! YAY!
            table_MpnList = Table1
        End Sub

        'Populates the OP object (must declare NEW prior to pass-in)
        'Requires page text (source) and the Manufacturer Part Number you're looking for (searchParam)
        'OP will populate all fields if MPN is an "EXACT" match
        Private Function fetchBlock(ByRef source As String, ByVal searchParam As String) As String

            Dim k As String = ""
            Dim charsToTrim() As Char = {"{"c, "["c, "]"c, "}"c, ","c, ":", " "c, """"c}
            searchParam = searchParam.Trim(charsToTrim)

            'PART I'
            'Chop the source string down to begin with the desired part
            'Along the way, fetchSmart collects the descriptions because descriptions is lost during the chopping processes
            'The last "descriptions" that function fetchSmart processes will belong to the description of our desired part
            Do Until k = ENDOFSOURCE
                'FetchSmart will wittle the "source" text down and organize the data it consumes
                k = fetchSmart(source, "object")
                Select Case k
                    Case "descriptions"
                        'Just collect the descriptions as they come
                        fetchSmart(source, "descriptions")
                    Case "mpn"
                        'Examines the MPN for a match with searchParam
                        k = fetchSmart(source, "object")
                        If StrComp(k, searchParam, CompareMethod.Text) Then   'If string match then strcomp => 0
                            'If EXACT match fails then clear desc
                            ldesc = ""
                            'if no EXACT match is found then then source will reach ENDOFSOURCE and exit this function
                        Else
                            'If EXACT match found
                            lmpn = k
                            'Arrived at desired location in string
                            'Exit while loop
                            Exit Do
                        End If
                End Select
            Loop

            'PART II'
            'Parse the remaining info of our desired part
            If k <> ENDOFSOURCE Then
                'Find the "highlight" keyword
                'This will give us index of end of json block for the specific part
                Dim endOfBlock As Integer = source.IndexOf("highlight")

                'Cut out the block for parsing
                source = source.Substring(0, endOfBlock)

                Do Until k = ENDOFSOURCE
                    k = fetchSmart(source, "object")
                    Select Case k
                        Case "avg_Avail"
                            k = fetchSmart(source, "object")
                            lavg_Avail = k
                        Case "num_suppliers"
                            k = fetchSmart(source, "object")
                            lnum_Suppliers = k
                        Case "attribute"
                            fetchSmart(source, "attribute")
                        Case "datasheets"
                            'datasheet urls
                            fetchSmart(source, "datasheets")
                        Case "mfr_List"
                            'will parse an mfr_List block
                            fetchSmart(source, "mfr_List")
                        Case "images"
                            'image urls
                            fetchSmart(source, "images")
                        Case "manufacturer"
                            fetchSmart(source, "manufacturer")
                        Case ""
                    End Select
                Loop
            End If

            Return source
        End Function

        Private Function fetchSmart(ByRef source As String, Optional ByVal attribute As String = "") As String

            'WARNING! these are temp variables used throughout this function
            'Changing them may cause error in the flow of this function
            Dim index As Integer = -1
            Dim endex As Integer = -1
            Dim indexBrace As Integer = -1
            Dim endexBrace As Integer = -1
            Dim length As Integer = -1
            Dim keyword As String = ""
            Dim keytext As String = ""
            Dim charsToTrim() As Char = {"{"c, "["c, "]"c, "}"c, ","c, ":", " "c, """"c}

            Select Case attribute
                Case "object"
                    If source = "" Then
                        Return ENDOFSOURCE
                    End If

                    'Find index of standard json object (e.g. "object":"value",)
                    index = -1  'initial index temp variable
                    endex = -1  'end index temp variable
                    'index is to get the index of the begining of the object
                    index = source.IndexOf(""""c)
                    'colon denotes end of a "string" object in JSON
                    Dim colon As Integer = source.IndexOf(":"c)
                    'comma denotes the end of a "value" object in JSON
                    Dim comma As Integer = source.IndexOf(","c)
                    endex = -1

                    'if you detect neither then there is no more oject
                    If colon = -1 And comma = -1 And source.Length() <> 0 Then
                        keyword = source
                        keyword = keyword.Trim(charsToTrim)
                        source = ""
                        Return keyword
                    End If

                    'set a condition to see if colon is in between quotes that is not a valid end tag(e.g. "http://blah.com":"value,)
                    'if object is an http address then must use quotes to judge end index so use the function quoteIndexes(source)
                    Dim enclosed() As Integer = quoteIndexes(source)
                    Dim indexTemp As Integer = enclosed(0)
                    Dim endexTemp As Integer = enclosed(1)
                    If indexTemp <> -1 And endexTemp <> -1 And colon > indexTemp And colon < endexTemp Then
                        colon = -1
                    End If

                    'If we find that colon does not belong in a http address then it is a valid end index
                    If colon <> -1 Then
                        If colon < comma Or comma = -1 Then
                            endex = colon
                        End If
                    End If

                    If comma <> -1 Then
                        'if the "value" object is not inclosed in quotes then we need to get the first
                        'space that comes before it
                        If comma < colon Or colon = -1 Then
                            If comma < index Then
                                index = source.IndexOf(" "c)
                            End If
                            endex = comma
                        End If
                    End If

                    'Now we should have the the index of the begining and end of the object or value of json
                    'Extract and return the keyword
                    If index >= 0 And endex >= 0 Then
                        length = endex - index + 1
                        keyword = source.Substring(index, length)
                        keyword = keyword.Trim(charsToTrim)
                        source = source.Remove(0, endex + 1)
                        Return keyword
                    ElseIf comma < 0 And colon < 0 Then
                        'if there are no valid criteria to find the endex, set endex to the end of the source
                        length = source.Length() - 1
                        keyword = source.Substring(index, length)
                        keyword = keyword.Trim(charsToTrim)
                        source = source.Remove(0, length)
                        Return keyword
                    End If
                    Return "object parse error"

                Case "descriptions"
                    endexBrace = source.IndexOf("]"c)
                    index = source.IndexOf("text")
                    Do While index < endexBrace
                        source = source.Remove(0, index + 8)
                        endex = source.IndexOf(""""c)
                        If ldesc <> "" Then
                            ldesc = ldesc & "; " & source.Substring(0, endex)
                        Else
                            ldesc = source.Substring(0, endex)
                        End If
                        source = source.Remove(0, endex + 1)
                        index = source.IndexOf("text")
                        endexBrace = source.IndexOf("]"c)
                    Loop
                    source = source.Remove(0, endexBrace + 1)

                Case "attribute"
                    'Find the name of the attribute
                    Dim displayname As String = ""
                    displayname = fetchSmart(source, "object")
                    If displayname = "displayname" Then
                        displayname = fetchSmart(source, "object")
                    Else
                        displayname = "noname"
                    End If
                    'collect the spec names for later ref
                    'if there is an error with a lot of "noname" strings showing up then meatadata collection will reject
                    lspecsMeta.Add(displayname)

                    'Find the values to the attribute
                    'Do while loop does blind search until values is found (no checking) can cause error if no string named "value is found"
                    Dim values As String = ""
                    Do
                        values = fetchSmart(source, "object")
                    Loop Until values = "values"
                    indexBrace = source.IndexOf("["c)
                    endexBrace = source.IndexOf("]"c)
                    length = endexBrace - indexBrace - 1
                    keytext = source.Substring(indexBrace + 1, length)
                    lspecs.Add(keytext, displayname)
                    source = source.Remove(0, endexBrace + 1)

                Case "datasheets"
                    index = -1
                    endex = -1

                    endexBrace = source.IndexOf("]"c)
                    index = source.IndexOf("url")
                    Do While index < endexBrace And index <> -1
                        source = source.Remove(0, index + 7)
                        endex = source.IndexOf(""""c)
                        ldatasheets.Add(source.Substring(0, endex))
                        source = source.Remove(0, endex + 1)
                        index = source.IndexOf("url")
                        endexBrace = source.IndexOf("]"c)
                    Loop

                Case "mfr_List"
                    'take the string that belong in the mfr_List block and place it in a variable
                    Dim indexes() As Integer = bracketIndexes(source)
                    Dim indexArray As Integer = indexes(0)
                    Dim endexArray As Integer = indexes(1)
                    length = endexArray - indexArray + 1
                    Dim mfr_ListBlock As String = source.Substring(indexArray, length)
                    source = source.Remove(indexArray, length)

                    'now to parse the mfr_List box.
                    'Return mfr_ListBlock 'uncommnet for debugging purpose
                    'Return parsemfr_ListBlock(mfr_ListBlock) ' uncomment for debugging purpose
                    parseMfrListBlock(mfr_ListBlock)

                Case "images"
                    index = -1
                    endex = -1

                    Dim endbrace As String = source.IndexOf("]"c)
                    Dim curlybraces() As Integer = curlyBracketIndexes(source)
                    Do While curlybraces(1) < endbrace And curlybraces(0) <> -1 And curlybraces(1) <> -1
                        index = curlybraces(0)
                        endex = curlybraces(1)
                        length = endex - index + 1

                        Dim imgBlock As String = source.Substring(index, length)    'parse out the block of url images
                        source = source.Remove(index, length)   'remove the block from the original source

                        'process the block of url code
                        Dim singleImgURL As String
                        Dim urlStart As Integer
                        Dim urlLength As Integer
                        Do While imgBlock.Length() > 0  'parse out the block of html code
                            urlStart = imgBlock.IndexOf("h")
                            If urlStart < 0 Then    'if there is no 'h' for http then there are no address to parse
                                Exit Do
                            Else
                                imgBlock = imgBlock.Remove(0, urlStart) 'remove text before http address
                                urlLength = imgBlock.IndexOf(""""c) 'the end of http string ends with a quote
                                If urlLength < 0 Then   ' no end quote then take the rest of the string
                                    urlLength = imgBlock.Length()
                                End If

                                singleImgURL = imgBlock.Substring(0, urlLength)  'parse out the http string
                                imgBlock = imgBlock.Remove(0, urlLength)    'remove the http string

                                'check for valid image extension (jpg, jpeg, gif, bmp, png)
                                'if valid then save http string into OP image collections
                                If singleImgURL.IndexOf(".jpg") > 0 Then
                                    limages.Add(singleImgURL)
                                ElseIf singleImgURL.IndexOf(".jpeg") > 0 Then
                                    limages.Add(singleImgURL)
                                ElseIf singleImgURL.IndexOf(".gif") > 0 Then
                                    limages.Add(singleImgURL)
                                ElseIf singleImgURL.IndexOf(".bmp") > 0 Then
                                    limages.Add(singleImgURL)
                                ElseIf singleImgURL.IndexOf(".png") > 0 Then
                                    limages.Add(singleImgURL)
                                End If
                                'singleImgURL will not be saved if we cannot find the extensions
                            End If
                        Loop

                        endbrace = source.IndexOf("]")
                        curlybraces = curlyBracketIndexes(source)
                    Loop

                Case "manufacturer"
                    keyword = ""
                    Do
                        keyword = fetchSmart(source, "object")
                    Loop Until keyword = "displayname"
                    keyword = fetchSmart(source, "object")
                    lmanufacturer = keyword
                Case ""
            End Select

            Return Nothing
        End Function

        Private Function bracketIndexes(ByVal source As String) As Array
            Dim bounds() As Integer = {-1, -1}
            'find frist '['
            Dim i As Integer = 0
            Dim length As Integer = source.Length
            While (i < length)
                If source(i) <> "[" Then
                    i = i + 1
                Else
                    Exit While
                End If
            End While

            If i <> length Then
                bounds(0) = i
            Else
                Return bounds
            End If
            i = i + 1

            'now to find ']'
            'need to keep other '[' in check
            Dim token As Integer = 0 'for counting '['
            While (i < length)
                If source(i) = "["c Then
                    token = token + 1
                End If
                If source(i) = "]" And token > 0 Then
                    token = token - 1
                ElseIf source(i) = "]" And token = 0 Then
                    bounds(1) = i
                    Exit While
                End If
                i = i + 1
            End While

            Return bounds
        End Function

        Private Function curlyBracketIndexes(ByVal source As String) As Array
            Dim bounds() As Integer = {-1, -1}
            'find frist '{'
            Dim i As Integer = 0
            Dim length As Integer = source.Length

            While (i < length)
                If source(i) <> "{" Then
                    i = i + 1
                Else
                    Exit While
                End If
            End While

            If i <> length Then
                bounds(0) = i
            Else
                Return bounds
            End If
            i = i + 1

            'now to find '}'
            'need to keep other '{' in check
            Dim token As Integer = 0 'for counting '{'
            While (i < length)
                If source(i) = "{"c Then
                    token = token + 1
                End If
                If source(i) = "}" And token > 0 Then
                    token = token - 1
                ElseIf source(i) = "}" And token = 0 Then
                    bounds(1) = i
                    Exit While
                End If
                i = i + 1
            End While

            Return bounds
        End Function

        Private Function quoteIndexes(ByVal source As String) As Array
            Dim bounds() As Integer = {-1, -1}
            Dim i As Integer = 0
            Dim length As Integer = source.Length

            'find frist '"'
            While (i < length)
                If source(i) <> """" Then
                    i = i + 1
                Else
                    Exit While
                End If
            End While

            If i <> length Then
                bounds(0) = i
            Else
                Return bounds
            End If
            i = i + 1

            'now to find second '"'
            While (i < length)
                If source(i) <> """" Then
                    i = i + 1
                Else
                    Exit While
                End If
            End While

            If i <> length Then
                bounds(1) = i
            Else
                Return bounds
            End If

            Return bounds
        End Function

        Private Function parseMfrListBlock(ByRef mfr_ListBlock As String) As String
            'index temp values
            Dim indexes() As Integer
            Dim index As Integer
            Dim endex As Integer
            Dim length As Integer
            'collects each value block in offer
            Dim subBlock As String = ""
            Dim distName As String = ""
            Dim keyword As String = ""

            Dim debugger As String = ""

            Do
                'clear the distData collection and clear distName
                Dim distData As New Distributor
                distName = ""

                'begin collecting data of one of the distributors
                indexes = curlyBracketIndexes(mfr_ListBlock)
                index = indexes(0)
                endex = indexes(1)

                'if there are no more values of "offer" exit
                If index = -1 Or endex = -1 Then
                    Exit Do
                End If

                'get the subdata of a distributor (Newark, Jameco, etc...)
                length = endex - index + 1
                subBlock = mfr_ListBlock.Substring(index, length)
                mfr_ListBlock = mfr_ListBlock.Remove(index, length + 1)
                'do not trip curly brackets "{" and "}", it will mess up the tags for the parser

                'crawl through the subBlock to organize the data
                Do
                    keyword = fetchSmart(subBlock, "object")
                    Select Case keyword
                        Case "sku"
                            keyword = fetchSmart(subBlock, "object")
                            distData.sku = keyword
                        Case "avail"
                            keyword = fetchSmart(subBlock, "object")
                            distData.avail = keyword
                        Case "sendrfq_url"
                            keyword = fetchSmart(subBlock, "object")
                            distData.sendrfq_url = keyword
                        Case "prices"
                            Dim indexesTemp() As Integer = bracketIndexes(subBlock)
                            If indexesTemp(0) <> -1 And indexesTemp(1) <> -1 Then
                                Dim indexTemp As String = indexesTemp(0)
                                Dim endexTemp As String = indexesTemp(1)
                                'collect the prices block
                                distData.prices = subBlock.Substring(indexTemp, endexTemp - indexTemp + 1)
                                subBlock.Remove(indexTemp, endexTemp - indexTemp + 1)
                            End If
                        Case "clickthrough_url"
                            keyword = fetchSmart(subBlock, "object")
                            distData.clickthrough_url = keyword
                        Case "displayname"
                            'distributor name will be the key to accessing its data
                            distName = fetchSmart(subBlock, "object")
                            lmfr_ListMeta.Add(distName)
                            distData.displayname = distName
                        Case "buynow_url"
                            keyword = fetchSmart(subBlock, "object")
                            distData.buynow_url = keyword
                    End Select
                Loop While keyword <> ENDOFSOURCE

                'enter the collection of data and distributor name as key into the mfr_List collection
                lmfr_List.Add(distData, distName)
            Loop


            Return Nothing
        End Function

        Function clrDist(ByRef dist As Distributor) As String
            dist.avail = ""
            dist.buynow_url = ""
            dist.clickthrough_url = ""
            dist.displayname = ""
            dist.prices = ""
            dist.sendrfq_url = ""
            dist.sku = ""

            Return Nothing
        End Function

        Function lstDist(ByVal dist As Distributor) As String
            Return "sku: " & dist.sku & vbCrLf & _
                    "avail: " & dist.avail & vbCrLf & _
                    "sendrfq_url: " & dist.sendrfq_url & vbCrLf & _
                    "prices: " & dist.prices & vbCrLf & _
                    "clickthrough_url: " & dist.clickthrough_url & vbCrLf & _
                    "distributor: " & dist.displayname & vbCrLf & _
                    "buynow_url: " & dist.buynow_url & vbCrLf
        End Function

    End Class
    '==========================
    '== [END] CLASS OCTOPART ==
    '==========================

    Function fetch(ByRef source As String) As String

        'clear keyword
        Dim keyword As String = ""

        Dim charsToTrimFromSource() As Char = {"{"c, "["c, "]"c, "}"c, ","c, ":", " "c}
        Dim charsToTrimFromKeyword(charsToTrimFromSource.Length) As Char
        charsToTrimFromSource.CopyTo(charsToTrimFromKeyword, 0)
        charsToTrimFromKeyword(charsToTrimFromSource.Length) = """"c

        'look for the next ':' character
        Dim find1 As Integer = source.IndexOf(":"c)  'colon
        Dim find2 As Integer = source.IndexOf(","c)  'comma
        Dim findQ As Integer = source.IndexOf(""""c) 'quotes
        Dim endex As Integer    'end index of keyword

        'Short-circuit: Test for EOF now, so we don't waste processing
        '-1 means that there is no ':' or ',' which means the text is empty or full of meaningless characters
        If find1 = -1 Or find2 = -1 Then
            Return ENDOFSOURCE
        End If

        'Find the Keyword's boundaries

        'Deal with escaped text
        If findQ = 0 Then
            'If we start with quotes, find the next quotes, since quotes escape regular command/keyword processing
            Dim findQ2 As Integer = source.IndexOf(""""c, findQ + 1) 'find the next quotes
            If findQ2 > find1 Then
                find1 = source.IndexOf(":"c, findQ2) 'find the next colon (because the last one was escaped)
            End If
            If findQ2 > find2 Then
                find2 = source.IndexOf(","c, findQ2) 'find the next comma (because the last one was escaped)
            End If
        End If

        'Normal Keyword processing (not in an escaped block)
        If find1 > find2 Then  'find where the next object ends
            endex = find2
        Else
            endex = find1
        End If

        'Extract Keyword, Cleanup, and Return
        keyword = source.Substring(0, endex + 1)    'get the keyword'
        source = source.Remove(0, endex + 1)    'trim the keyword from source
        source = source.TrimStart(charsToTrimFromSource)  'trim extra from source
        keyword = keyword.TrimStart(charsToTrimFromKeyword)    'clean up keyword
        keyword = keyword.TrimEnd(charsToTrimFromKeyword)  'clean up keyword
        Return keyword

    End Function
End Module
