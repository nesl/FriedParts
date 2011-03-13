Imports Microsoft.VisualBasic
Imports System.Data
Imports fpSearch

Namespace fpSearch

    Public Class searchResults
        ''' <summary>
        ''' Internal Variable. Holds Search Results Table
        ''' </summary>
        ''' <remarks></remarks>
        Private iDt As DataTable

        ''' <summary>
        ''' Internal variable. The original search terms.
        ''' </summary>
        ''' <remarks></remarks>
        Private iTerms As String

        ''' <summary>
        ''' Internal variable. The part type id searched for.
        ''' </summary>
        ''' <remarks></remarks>
        Private iPartTypeIdFilter As Int32

        Public ReadOnly Property getPartTypeID As Int32
            Get
                Return iPartTypeIdFilter
            End Get
        End Property

        Public ReadOnly Property getSearchTerms() As String
            Get
                Return iTerms
            End Get
        End Property

        ''' <summary>
        ''' The datatable containing the search results for use as the datasource in a 
        ''' grid view or other display type.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property getDataSource() As DataTable
            Get
                Return iDt
            End Get
        End Property

        ''' <summary>
        ''' Filters the search result table by PartTypeID. Updates the internal datatable.
        ''' Access the results via the getDataSource() property
        ''' </summary>
        ''' <param name="PartTypeID"></param>
        ''' <remarks></remarks>
        Public Function filterByPartType(ByRef PartTypeID As Int32) As DataTable
            Return ptGetPartsOfThisType(PartTypeID, iDt)
        End Function

        ''' <summary>
        ''' Default constructor.
        ''' </summary>
        ''' <param name="SearchTerms"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal SearchTerms As String)
            If SearchTerms Is Nothing Then
                iTerms = ""
            Else
                iTerms = SearchTerms
            End If
            iDt = fpSearchDataSource(iTerms)
        End Sub
    End Class

End Namespace


