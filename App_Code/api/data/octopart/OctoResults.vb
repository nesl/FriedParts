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

        Protected table_MpnList As DataTable
        Public ReadOnly Property MPN_List() As DataTable
            Get
                Return table_MpnList
            End Get
        End Property




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

        'Clears the specific selected manufacturer part number data, without 
        '   requiring another trip to the Octopart server. Use this to switch 
        '   between MPN's when multiple match the initial query.
        '
        '   WE MAY NOT ACTUALLY WIND UP NEEDING THIS FUNCTION
        '
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

            'Parse the JSON!
            rJson = JObject.Parse(Source)

            'Learn stuff from it!
            Dim obj As JToken = rJson.SelectToken("results[0].item")


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
    End Class

End Namespace
