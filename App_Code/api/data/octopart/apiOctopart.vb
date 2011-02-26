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

    Public Class OctoCommands
        ''' <summary>
        ''' The API interaction point. Web address of the REST API base.
        ''' </summary>
        ''' <remarks></remarks>
        Public Const API As String = "http://octopart.com/api/v2/parts/search?q="

        ''' <summary>
        ''' Perform a parts search!
        ''' </summary>
        ''' <remarks></remarks>
        Public Const PartSearch As String = "parts/search?q="
    End Class

    Public Class OctopartStatic
        






        'Finish Parsing Tag
        Public Const ENDOFSOURCE As String = "ENDOFSOURCE"
    End Class

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
