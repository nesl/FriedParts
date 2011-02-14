<%@ WebService Language="VB" Class="sysWebService" %>
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://friedparts.com/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class sysWebService
    Inherits System.Web.Services.WebService
    
    

    <WebMethod()> Public Function Update() As String
        Return "Hello World..." 'fpusDispatch()
    End Function   
    
End Class