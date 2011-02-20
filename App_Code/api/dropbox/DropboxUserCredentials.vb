Imports Microsoft.VisualBasic
Imports System.Data
Imports System.IO

''' <summary>
''' The class is just a convenient container for storing the four keys of a dropbox access token
''' </summary>
''' <remarks></remarks>
Public Class DropboxUserCredentials
    Private uToken As String
    Private uSecret As String
    Private aToken As String
    Private aSecret As String
    Public ReadOnly Property getUserKey As String
        Get
            Return uToken
        End Get
    End Property
    Public ReadOnly Property getUserSecret As String
        Get
            Return uSecret
        End Get
    End Property
    Public ReadOnly Property getAppKey As String
        Get
            Return aToken
        End Get
    End Property
    Public ReadOnly Property getAppSecret As String
        Get
            Return aSecret
        End Get
    End Property
    Public Sub New(ByVal userToken As String, ByVal userSecret As String, Optional ByVal appToken As String = "", Optional ByVal appSecret As String = "")
        uToken = userToken
        uSecret = userSecret
        aToken = appToken
        aSecret = appSecret
    End Sub
End Class

