Imports Microsoft.VisualBasic
Imports Newtonsoft.Json.Linq
Imports System.Data
Imports fpManufacturer

Namespace apiOctopart

    '=======================
    '==  CLASS OCTOPART   ==
    '=======================

    ''' <summary>
    ''' Represents a single part (manufacturer part number) and all of the associated
    ''' data. 
    ''' STILL NEED TO DEAL WITH DISTRIBUTION AND PRICING!!! 'xxx
    ''' STILL NEED TO DEAL WITH PART CATEGORIES!!! 'xxx
    ''' </summary>
    ''' <remarks></remarks>
    Public Class OctoPart
        'Internal Variables
        ''' <summary>
        ''' The entire result from Octopart in JSON.NET format for this
        ''' specific part.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Part As JToken 'The entire results from Octopart

        ''' <summary>
        ''' The Octopart summary description (Highlight)
        ''' </summary>
        ''' <remarks>This is the only value stored outside the "item" object 
        ''' of the "result" object so for development expediency we'll just store 
        ''' this one directly and the rest will be available under the "item"
        ''' object stored as variable "Part"</remarks>
        Protected iHighlight As String

        ''' <summary>
        ''' The UID for this part as defined by Octopart
        ''' </summary>
        ''' <returns>These ID's are SQL bigints!</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property OctopartID As Int64
            Get
                Return Part.Item("uid")
            End Get
        End Property

        ''' <summary>
        ''' Internal variable. The manufacturer object.
        ''' </summary>
        ''' <remarks></remarks>
        Protected iMfr As fpMfr

        ''' <summary>
        ''' The Manufacturer information. fpMfr object. Contains full details about this manufacturer.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Use return_value.DefaultName to get the Manufacturer's name.</remarks>
        Public ReadOnly Property Manufacturer As fpMfr
            Get
                Return iMfr
            End Get
        End Property

        ''' <summary>
        ''' The Manufacturer Part Number
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property MfrPartNum() As String
            Get
                Return Part.Item("mpn")
            End Get
        End Property

        ''' <summary>
        ''' The short description.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Uses Octopart's "Highlight" summary</remarks>
        Public ReadOnly Property DescriptionShort() As String
            Get
                Return iHighlight
            End Get
        End Property

        ''' <summary>
        ''' The long description.  -- currently unimplemented.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DescriptionLong() As String
            Get
                Static Desc As String = Nothing
                If Desc Is Nothing Then
                    'Long description = One distributors description + a table of properties
                    Desc = Part.SelectToken("descriptions[0]").Value(Of String)("text") & vbCrLf & vbCrLf
                    For i As UInt16 = 0 To DirectCast(Part.Item("specs"), JArray).Count - 1
                        Dim NumValues As UInt16 = DirectCast(Part.SelectToken("specs[" & i & "]").Item("values"), JArray).Count
                        Select Case NumValues
                            Case 0
                                'Skip this one... no values known.
                            Case 1
                                'Only one value so format it inline.
                                Desc += Part.SelectToken("specs[" & i & "]").Item("attribute").Value(Of String)("displayname") & ": " & _
                                    Part.SelectToken("specs[" & i & "]").SelectToken("values[0]").ToString & vbCrLf
                            Case Else
                                'Multiple part values so format it with indentation.
                                'This gets really complex because the data structure here depends on the metadata in the "attribute" property so for now we ignore it!
                                'xxx
                                'Desc += Part.Item("specs[" & i & "]").Item("attribute").Item("displayname").ToString & ": "
                                'For j As UInt16 = 0 To NumValues - 1
                                '   Desc += "     " & Part.Item("specs[" & i & "]").Item("values[" & j & "]").ToString
                                'Next
                        End Select
                    Next
                End If
                Return Desc
            End Get
        End Property

        ''' <summary>
        ''' The URL for detailed information about this part at Octopart.com
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property UrlOctopart() As String
            Get
                Return Part.Item("detail_url")
            End Get
        End Property

        ''' <summary>
        ''' The datasource containing a list of Datasheet URLs for this part.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property UrlDatasheetsDatasource() As DataTable
            Get
                Static iDatasheets As DataTable = Nothing
                If iDatasheets Is Nothing Then
                    Dim dr As DataRow
                    iDatasheets = apiOctopart.CreateTableUrlList
                    For i As UInt16 = 0 To DirectCast(Part.Item("datasheets"), JArray).Count - 1
                        dr = iDatasheets.NewRow
                        dr.Item("URL") = Part.SelectToken("datasheets[" & i & "]").Value(Of String)("url")
                        iDatasheets.Rows.Add(dr)
                    Next
                End If
                Return iDatasheets
            End Get
        End Property

        ''' <summary>
        ''' The datasource containing a list of Image URLs for this part.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property UrlImagesDatasource() As DataTable
            Get
                Static iImages As DataTable = Nothing
                If iImages Is Nothing Then
                    Dim dr As DataRow
                    iImages = apiOctopart.CreateTableUrlList
                    For i As UInt16 = 0 To DirectCast(Part.Item("images"), JArray).Count - 1
                        dr = iImages.NewRow
                        dr.Item("URL") = Part.SelectToken("images[" & i & "]").Value(Of String)("url")
                        iImages.Rows.Add(dr)
                    Next
                End If
                Return iImages
            End Get
        End Property

        ''' <summary>
        ''' Average number of this part available at *each* distributor
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property AvgAvailability() As UInt32
            Get
                Return Part.Item("avg_avail")
            End Get
        End Property

        ''' <summary>
        ''' The average selling price for a single unit across all known distributors.
        ''' Returns 0 if unknown
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property AvgPrice() As Single
            Get
                If Part.Item("avg_price[1]").ToString.CompareTo(OctopartStatic.US_DOLLARS) = 0 Then
                    Return Part.Item("avg_price[0]")
                Else
                    'Pricing not in US dollars! Uh oh!
                    Return 0
                End If
            End Get
        End Property

        ''' <summary>
        ''' Internal Variable.
        ''' </summary>
        ''' <remarks></remarks>
        Protected iPartTypeID As Int32

        ''' <summary>
        ''' The ID of this part's type (category)
        ''' </summary>
        ''' <value></value>
        ''' <returns>FriedParts PartTypeID</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property getPartTypeID As Int32
            Get
                Return iPartTypeID
            End Get
        End Property

        ''' <summary>
        ''' The name of this part's type (category)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property getPartTypeName As String
            Get
                Return ptGetTypeName(iPartTypeID)
            End Get
        End Property

        ''' <summary>
        ''' Worker function. Does the processing of construction.
        ''' </summary>
        ''' <remarks>Assumes the JTokens are all valid at this point.</remarks>
        Protected Sub PopulateMe()
            '[Manufacturer]
            '   We add manufacturers to our FriedParts database the first time we see them. 
            '   Even if the user is not going to add this part. That way, in the future, if
            '   some elects to manually add a part, the list of existing Mfr's is bigger and
            '   aligned with Octopart's representation of them.
            Dim opMfrName As String = Part.Item("manufacturer").Item("displayname")
            If fpMfr.Exists(opMfrName) Then
                'We have this Manufacturer in FriedParts already -- and we just populated iMfr with it
                'Nothing left to do.
                iMfr = New fpMfr(opMfrName)
            Else
                'This guy is missing! Let's add it!
                iMfr = New fpMfr( _
                            fpMfr.Add(Part.Item("manufacturer").Item("displayname"), _
                                                Part.Item("manufacturer").Item("homepage_url"), _
                                                Part.Item("manufacturer").Item("id") _
                                              ) _
                                )
            End If

            '[PartType]/[Category]
            'Add all of the encountered Part Types to FriedParts (heck, we'll need 'em eventually)
            Dim opCategories As JArray = DirectCast(Part.SelectToken("category_ids"), JArray)
            For Each opCategory As JValue In opCategories
                'Does this category exist?
                apiOctopart.OctoPartType.opProcessPartTypes(DirectCast(opCategory.Value, Int64))
            Next
            'If this part has multiple categories, use the first one...
            If Not ptExistsPartType(DirectCast(opCategories(0), JValue).Value, iPartTypeID) Then
                Throw New Exception("Cannot find this part type, but I just added it, so something when horribly wrong!")
            End If
        End Sub



        'Constructor
        Public Sub New()
            Throw New Exception("Class Octopart cannot be constructed with default (e.g. 0) parameters")
        End Sub

        ''' <summary>
        ''' The only constructor. Populate this part's data from the JSON.NET formatted results.
        ''' </summary>
        ''' <param name="PartResultJToken">The JSON.NET JToken corresponding to 
        ''' this part record in the results set. Namely, rJSON.SelectToken("results[3]")
        ''' where 3 is the appropriate index into the list of parts results from 
        ''' the search</param>
        ''' <remarks></remarks>
        Public Sub New(ByRef PartResultJToken As Newtonsoft.Json.Linq.JToken)
            If PartResultJToken Is Nothing Then
                Throw New Exception("Invalid Octopart Token provided.")
            End If
            iHighlight = PartResultJToken.Item("highlight")
            Part = PartResultJToken.Item("item")
            PopulateMe()
        End Sub
    End Class

End Namespace
