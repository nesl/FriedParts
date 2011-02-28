Imports Microsoft.VisualBasic
Imports Newtonsoft.Json.Linq
Imports System.Data
Imports System.Net

Namespace apiOctopart
    ''' <summary>
    ''' This classes handles part search over the Octopart API and stores the results
    ''' The results are an array of OctoPart objects
    ''' </summary>
    ''' <remarks></remarks>
    Public Class OctoResults

        ''' <summary>
        ''' Stores the parsed out results from Octopart so that we don't keep 
        ''' going back to their webserver.
        ''' </summary>
        ''' <remarks>rJson = Results JSON object</remarks>
        Private rJson As JObject

        ''' <summary>
        ''' Internal. Stores list of MPNs returned resulting from this Octopart Search
        ''' </summary>
        ''' <remarks></remarks>
        Protected iDtMpns As DataTable

        ''' <summary>
        ''' Returns a datatable (for use as a datasource) of the Manufacturer Part Numbers (MPN) 
        ''' returned from the Octopart search.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DataSouceResults() As DataTable
            Get
                Return iDtMpns
            End Get
        End Property

        ''' <summary>
        ''' Internal variable. The Parts collection of OctoPart objects. 
        ''' </summary>
        ''' <remarks></remarks>
        Protected iParts As Collection

        ''' <summary>
        ''' The Parts collection.
        ''' </summary>
        ''' <value></value>
        ''' <returns>A collection of OctoPart objects. Each representing 
        ''' a single manufacturer part number from the result set.</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Parts As Collection
            Get
                Return iParts
            End Get
        End Property

        ''' <summary>
        ''' The total amount of time in seconds that it took to complete
        ''' this search at Octopart.com.
        ''' </summary>
        ''' <returns>The server crunch time in seconds.</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property RunTime() As Single
            Get
                Return rJson.SelectToken("time")
            End Get
        End Property

        ''' <summary>
        ''' The total number of results returned by the search.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>For large result sets, most of them will NOT be 
        ''' returned in detail. Therefore the actual content of the
        ''' search is more limited. Useful for warning the user to
        ''' narrow their search.</remarks>
        Public ReadOnly Property NumResultsTotal() As UInt32
            Get
                Return rJson.SelectToken("hits")
            End Get
        End Property

        ''' <summary>
        ''' The number of parts (manufacturer part numbers) found in this Octopart 
        ''' search -- and also available for use.
        ''' </summary>
        ''' <returns>The result count</returns>
        ''' <remarks>Sometimes a search returns too many results. We only download
        ''' a few for efficiency since the user will most likely have to narrow
        ''' the search anyway.</remarks>
        Public ReadOnly Property Count() As Integer
            Get
                Return iDtMpns.Rows.Count
            End Get
        End Property

        ''' <summary>
        ''' Empty construction not allowed. 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            Throw New Exception("No search specified. You asked me to search for nothing at Octopart!")
        End Sub

        ''' <summary>
        ''' Only valid constructor. Takes in the search string and populates an array of 
        ''' OctoPart objects
        ''' </summary>
        ''' <param name="TheSearchTerms">The text to search</param>
        ''' <remarks></remarks>
        Public Sub New(ByRef TheSearchTerms As String)
            'Format the query
            Dim URL As String = OctoCommands.API & OctoCommands.PartSearch & _
                                System.Web.HttpUtility.UrlEncode(TheSearchTerms) _
                                'Append the user's search request to the end of the string

            'Get the source
            Dim myWebClient As New System.Net.WebClient()
            Dim Source As String
            Try
                Source = myWebClient.DownloadString(URL)    'Source is the text received from OCTOPARTS website
            Catch ex As Exception
                If TypeOf ex Is System.Net.WebException Then
                    'Server did not respond in time! ...or other server error!
                ElseIf TypeOf ex Is NotSupportedException Then
                    'This function may not be called sumultaneously from multiple threads!
                Else
                    Throw New Exception(ex.Message)
                End If
            End Try
            'Sanity/Safety check
            If Source Is Nothing Then
                Throw New NullReferenceException("Missing HTML content! The WebClient did not return anything!")
            End If

            'Parse the JSON!
            rJson = JObject.Parse(Source)

            '[Learn stuff from it!]
            '----------------------
            Dim jT As JToken
            Dim dr As DataRow
            Dim Result As JToken
            Dim Part As OctoPart
            iParts = New Collection

            jT = rJson.SelectToken("results")
            iDtMpns = apiOctopart.CreateTableMpnList
            For i As UInt32 = 0 To DirectCast(jT, JArray).Count - 1
                'Extract this result record
                Result = rJson.SelectToken("results[" & i & "]")

                'Create the results table (summary)
                dr = iDtMpns.NewRow
                dr.Item("OctopartID") = Result.Item("item").Item("uid")
                dr.Item("MfrPartNum") = Result.Item("item").Item("mpn")
                dr.Item("Highlight") = Result.Item("highlight")
                dr.Item("Description") = Result.Item("item").Item("descriptions[0]") 'We use the first description, since there might be many.

                '...and the Part Object (detailed)
                Part = New OctoPart(Result)
                iParts.Add(Part, Part.OctopartID)
            Next

        End Sub

    End Class

End Namespace
