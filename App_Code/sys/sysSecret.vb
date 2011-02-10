Imports Microsoft.VisualBasic

'DO NOT SOURCE RELEASE THIS MODULE!!! 
'THIS IS WHERE ALL THE PRIVATE -- NON-RELEASE -- DATA IS STORED
'We collect it here to simplify redaction during code release events.

Public Module sysSecret
    Public Function apiDropboxAppKey(ByRef Secret As Boolean) As String
        If Secret Then
            Return "k2h2q545lej4rbt"
        Else
            Return "e02gmg4by83wk5q"
        End If
    End Function

End Module
