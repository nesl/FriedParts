Imports Microsoft.VisualBasic
Imports System.Data

''' <summary>
''' This module encapsulates the SYSTEM ACCESS LOG.
''' That's the log file that Splunk mines.
''' </summary>
''' <remarks></remarks>
Public Module sysLog
    ''' <summary>
    ''' Should be called by all pages to log their loading event.
    ''' </summary>
    ''' <remarks>Called by FP.master (master page), so you don't need to add this if you derive from FP.master</remarks>
    Public Sub logPageLoad(ByVal ThePage As System.Web.UI.Page)
        'ThePage.Request.
    End Sub
End Module
