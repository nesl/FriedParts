<%@ WebService Language="VB" CodeBehind="~/App_Code/api/apiWebService.vb" Class="sysWebService" %>
Imports System.Web.Services

Public Class blah
    Inherits WebService

    Public Function testMe() As String
        Return "Hello World!"
    End Function
End Class