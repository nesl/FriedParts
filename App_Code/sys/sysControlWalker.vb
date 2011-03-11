Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Web.UI.Page
Imports System.Text.RegularExpressions

Public Class sysControlWalker
    Protected m_controls As Collection
    Protected iControlNames As ArrayList

    Protected Sub Worker(ByRef c As Control)

        'CHECK FOR LEAF
        If c.Controls.Count = 0 Then
            'Done!
            Dim temp As String = GetID(c.UniqueID)
            If temp.Length > 0 Then
                'Valid control ADD!
                Try
                    Dim name As String = GetID(c.UniqueID)
                    If (iControlNames IsNot Nothing And (iControlNames.BinarySearch(name) > 0)) _
                        Or (iControlNames Is Nothing) _
                        Then
                        'ADD
                        m_controls.Add(c, GetID(c.UniqueID))
                    End If
                Catch ex As ArgumentException
                    'Control key already exists (obviously, not one of the developer's manually added or named controls so just ignore)
                End Try
            Else
                'Do nothing (don't add to controls collection)
            End If
        Else
            'RECURSE!
            For Each c2 As Control In c.Controls
                Worker(c2)
            Next
            Try
                Dim name As String = GetID(c.UniqueID)
                If (iControlNames IsNot Nothing And (iControlNames.BinarySearch(name) >= 0)) _
                        Or (iControlNames Is Nothing) _
                        Then
                    'ADD
                    m_controls.Add(c, GetID(c.UniqueID))
                End If
            Catch ex As ArgumentException
                'Control key already exists (obviously, not one of the developer's manually added or named controls so just ignore)
            End Try
        End If
    End Sub

    Public Function FindControl(ByRef ServerID As String) As Control
        Try
            Return m_controls(ServerID)
        Catch ex As System.ArgumentException
            Throw New Exception("Cannot find " & ServerID)
        End Try
    End Function

    Protected Function GetID(ByRef UniqueID As String) As String
        Dim lIdx As Int16 = UniqueID.LastIndexOf("$")
        If lIdx < 1 Then
            Return ""
        Else
            Dim temp As String = UniqueID.Substring(lIdx + 1)
            If Not temp.StartsWith("ctl") Then
                Return temp
            Else
                Return ""
            End If
        End If
    End Function

    Public Sub New(ByRef myForm As Page, Optional ByRef ControlNames As ArrayList = Nothing)
        m_controls = New Collection
        iControlNames = ControlNames
        iControlNames.Sort()
        'create a control walker to get 
        'all controls on the form
        For Each c As Control In myForm.Controls
            Worker(c)
        Next
    End Sub
    'This property returns the collection of all controls
    'on the form
    ReadOnly Property Controls() As Collection
        Get
            Return m_controls
        End Get
    End Property
End Class
