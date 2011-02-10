Imports System

Partial Class dbSQL
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim blah As String = TextBox1.Text
        Dim Output As String = ""
        Dim lineStop As Integer
        logDebug("Click Event Handler")

        Dim SQLType As String
        SQLType = firstWordInLine(blah)
        Select Case SQLType
            Case "INSERT" '=====================================================
                logDebug("Case [INSERT] found!")
                Dim howManyFields As Int16 = 0
                While blah.Length > 0
                    lineStop = blah.IndexOf(vbCr)
                    If lineStop = -1 Then
                        'Next Line NOT FOUND (must be EOF)
                        Output = Output & """" & blah & """"
                        logDebug("Finished String Processing")
                        Exit While
                    Else
                        'Continue to next line!
                        'dbgChars(blah.Substring(0, lineStop + 4))
                        If firstWordInLine(blah.Substring(0, lineStop)) = "VALUES" Then
                            logDebug("VALUES Found!")
                            'Output the VALUES tag
                            Output = Output & """" & blah.Substring(0, lineStop) & " ("" & _" & vbCrLf
                            'And all the field rows
                            For i = 1 To howManyFields - 1
                                Output = Output & """           '"" &  & ""',"" & _" & vbCrLf
                            Next
                            'Add final closing paren
                            Output = Output & """           )"""
                            Exit While
                        End If
                        Output = Output & """" & blah.Substring(0, lineStop) & """ & _" & vbCrLf
                        blah = blah.Remove(0, lineStop + 2) 'Remove this line (including the CR-LF that terminate it) and continue again
                        howManyFields += 1 'Increment field count
                    End If
                End While
                'Output Final Answer!


            Case "UPDATE" '=====================================================
                logDebug("Case [UPDATE] found!")
                Dim howManyFields As Int16 = 0
                While blah.Length > 0
                    lineStop = blah.IndexOf(vbCr)
                    If lineStop = -1 Then
                        'Next Line NOT FOUND (must be EOF)
                        Output = Output & """   WHERE <condition> = '"" & & ""'"""
                        logDebug("Finished String Processing")
                        Exit While
                    Else
                        'Continue to next line!
                        If firstWordInLine(blah) = "UPDATE" Then
                            'Handle the first line -- just pass through
                            Output = Output & """" & blah.Substring(0, lineStop) & """ & _" & vbCrLf
                        ElseIf firstWordInLine(blah) = "SET" Then
                            'Handle the SET first line by separating out the first field
                            'Create a line that looks like:  "   SET " & _
                            Output = Output & """   SET "" & _" & vbCrLf
                            'Create the data line that looks like: "       [PartID] = '" & dr.Field(Of String)("PartID") & "'" & _
                            '   Note: We'll assume string type (since it's most common), but all ID fields will have to be manually fixed
                            Output = Output & """       " & updateProcFieldLine(blah) & vbCrLf
                        Else
                            Output = Output & """      ," & updateProcFieldLine(blah) & vbCrLf
                        End If
                        blah = blah.Remove(0, lineStop + 2) 'Remove this line (including the CR-LF that terminate it) and continue again
                    End If
                End While


            Case "SELECT" '=====================================================
                logDebug("Case [SELECT] found!")
                Dim howManyFields As Int16 = 0
                While blah.Length > 0
                    lineStop = blah.IndexOf(vbCr)
                    If lineStop = -1 Then
                        'Next Line NOT FOUND (must be EOF)
                        Output = Output & """" & blah & """"
                        logDebug("Finished String Processing")
                        Exit While
                    Else
                        'Continue to next line!
                        'dbgChars(blah.Substring(0, lineStop + 4))
                        Output = Output & """" & blah.Substring(0, lineStop) & """ & _" & vbCrLf
                        blah = blah.Remove(0, lineStop + 2) 'Remove this line (including the CR-LF that terminate it) and continue again
                        howManyFields += 1 'Increment field count
                    End If
                End While
        End Select
        TextBox1.Text = Output
    End Sub

    'Returns -1 or first word in line; Space as delimiter
    'Ignores whitespace at the beginning of line
    Private Function firstWordInLine(ByVal lineIN As String) As String
        'Find end of line
        Dim EOL As Integer
        EOL = lineIN.IndexOf(vbCr)
        If EOL = -1 Then EOL = lineIN.Length

        'Find first word
        Dim lineOUT As String
        lineOUT = lineIN.Substring(0, EOL)
        lineOUT = lineOUT.TrimStart
        EOL = lineOUT.IndexOf(" ") 'We're just reusing the EOL variable to now mean end of *word*
        If EOL = -1 Then
            Return lineOUT
        Else
            Return lineOUT.Substring(0, EOL)
        End If
    End Function

    'Returns    "Symbol_Desc3"
    'FROM       "  ,[Symbol_Desc3] = <Symbol_Desc3, text,>  "
    'If multiple [] expressions, returns the first one.
    Private Function updateFindFieldName(ByVal lineIn As String) As String
        Dim inBkt As Integer
        Dim outBkt As Integer
        inBkt = lineIn.IndexOf("[") 'zero index and first hit on search
        outBkt = lineIn.IndexOf("]")
        Return lineIn.Substring(inBkt + 1, outBkt - inBkt - 1)
    End Function

    Private Function updateIsID(ByVal fieldName As String) As Boolean
        'Case sensitive search for "ID" which is likely an integer ID field and not a String
        If fieldName.IndexOf("ID", System.StringComparison.CurrentCulture) = -1 Then
            'Not Found
            Return False
        Else
            Return True
        End If
    End Function

    'FROM       "  ,[Symbol_Desc3] = <Symbol_Desc3, text,>  "
    'TO         "[Symbol_Desc3] = '" & dr.Field(Of String)("Symbol_Desc3") & "'" & _"
    'TO (if ID) "[PartID] = " & dr.Field(Of int32)("PartID") & _
    'DOES NOT RETURN ANY INITIAL WHITESPACE OR COMMA -- space it out yourself (allows flexibility for handling the first/last line)
    Private Function updateProcFieldLine(ByVal lineIn As String) As String
        Dim outStr As String
        lineIn = Trim(lineIn) 'Trim whitespace
        outStr = "[" & updateFindFieldName(lineIn) & "] = "
        If updateIsID(updateFindFieldName(lineIn)) Then
            'Is ID
            outStr = outStr & """ & dr.Field(Of Int32)(""" & updateFindFieldName(lineIn) & """) & _"
        Else
            'NOT ID
            outStr = outStr & "'"" & dr.Field(Of String)(""" & updateFindFieldName(lineIn) & """) & ""'"" & _"
        End If
        Return outStr
    End Function
End Class



