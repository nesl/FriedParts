Imports Microsoft.VisualBasic
'Contains all code appertaining Part Values

'MODULE PREFIX IS: pv
Public Module fpPartValues

    'Test for BETWEEN
    'Includes the lower limit in the true range, but not the upper limit
    'Ex: pvIsBetween(0,0,1000) = true, pvIsBetween(1000,0,1000) = false
    Public Function pvIsBetween(ByRef A As Double, ByRef LowerLimit As Double, ByRef UpperLimit As Double) As Boolean
        If A < LowerLimit Or A >= UpperLimit Then
            Return False
        Else
            Return True
        End If
    End Function

    'Checks the string to see if it is a valid number including its pSpice SI prefix -- e.g. Meg, k, m, u, n, p, f
    'Returns the numeric value to numValue (pass by reference) if textIn is valid
    'NOTE: YOU MUST CHECK THE TRUE/FALSE RETURN VALUE or else you will get a corrupt value as numValue = 0 when textIn isn't valid!!!
    Public Function pvValidNumeric(ByVal textIn As String, Optional ByRef numValue As Double = 0) As Boolean
        'Sanity checks
        If textIn.Length = 0 Then Return False
        If textIn.Length > 255 Then Err.Raise(-83742, , "Numeric text value is too long! Must be less than 255 characters! Try using scientific notation!")
        'Process
        textIn = Trim(textIn) 'Trim off whitespace
        textIn = textIn.Replace(" ", "") 'Remove internal whitespace -- e.g. "3.4 pF" --> "3.4pF"
        textIn = UCase(textIn) 'Uppercase everything for simplicity

        'Work backwards
        Dim i As Byte = textIn.Length
        Do Until i = 0 OrElse IsNumeric(textIn.Substring(0, i))
            i = i - 1
        Loop

        'Calculate result
        If i = 0 Then
            'No numeric part found
            Return False
        Else
            'Numeric part found from indexes 0 to i-1
            'Index i is the start of the non-numeric part
            Dim SImultiplier As Double
            If pvValidPrefix(textIn.Substring(i, textIn.Length - i), SImultiplier) Then
                numValue = CDbl(textIn.Substring(0, i)) * SImultiplier
                Return True
            Else
                'No valid SI Prefix found
                Return False
            End If
        End If
    End Function

    'Checks the string to see if it is a valid pSpice SI prefix -- e.g. Meg, k, m, u, n, p, f
    'Empty string is valid as an x1 multiplier
    'Returns the numeric multiplier to numValue (pass by reference) if textIn is valid
    'NOTE: Pass ONLY THE PREFIX or you will get a false negative "42k" = false, "k" = true
    Private Function pvValidPrefix(ByVal textIn As String, Optional ByRef numValue As Double = 0) As Boolean
        'Sanity checks
        If textIn.Length = 0 Then
            numValue = 1
            Return True 'x1 multiplier implied by empty string
        End If
        If textIn.Length > 255 Then Err.Raise(-83742, , "Numeric text value is too long! Must be less than 255 characters! Try using scientific notation!")
        'Process
        textIn = Trim(textIn) 'Trim off whitespace
        textIn = textIn.Replace(" ", "") 'Remove internal whitespace -- e.g. "3.4 pF" --> "3.4pF"
        textIn = UCase(textIn) 'Uppercase everything for simplicity
        Select Case textIn
            Case "GIG"
                numValue = 1000000000.0
                Return True
            Case "MEG"
                numValue = 1000000.0
                Return True
            Case "K"
                numValue = 1000.0
                Return True
            Case "M"
                numValue = 0.001
                Return True
            Case "U"
                numValue = 0.000001
                Return True
            Case "N"
                numValue = 0.000000001
                Return True
            Case "P"
                numValue = 0.000000001
                Return True
            Case "F"
                numValue = 0.000000000000001
                Return True
            Case Else
                Return False
        End Select
    End Function

    'Converts a double into a human readable expression
    Public Function pvFormatNumber(ByVal numIn As Double, Optional ByVal verbose As Boolean = False) As String
        Const NumericStyle As String = "####0.###"
        Dim Multiplier As Double

        'Fempto numbers
        Multiplier = 1.0E-17 '1e-17
        If (numIn < Multiplier) Then
            If Not verbose Then
                Return Format(numIn / (Multiplier * 100), NumericStyle) & "f"
            Else
                Return "" & numIn / (Multiplier * 100) & " fempto"
            End If
        End If

        'Pico numbers
        Multiplier = 0.00000000000001 '1e-14
        If pvIsBetween(numIn, Multiplier, Multiplier * 1000.0) Then
            If Not verbose Then
                Return Format(numIn / (Multiplier * 100), NumericStyle) & "p"
            Else
                Return "" & numIn / (Multiplier * 100) & " pico"
            End If
        End If

        'Nano numbers
        Multiplier = 0.00000000001 '1e-11
        If pvIsBetween(numIn, Multiplier, Multiplier * 1000.0) Then
            If Not verbose Then
                Return Format(numIn / (Multiplier * 100), NumericStyle) & "n"
            Else
                Return "" & numIn / (Multiplier * 100) & " nano"
            End If
        End If

        'Micro numbers
        Multiplier = 0.00000001 '1e-8
        If pvIsBetween(numIn, Multiplier, Multiplier * 1000.0) Then
            If Not verbose Then
                Return Format(numIn / (Multiplier * 100), NumericStyle) & "u"
            Else
                Return "" & numIn / (Multiplier * 100) & " micro"
            End If
        End If

        'Milli numbers
        Multiplier = 0.00001 '1e-5
        If pvIsBetween(numIn, Multiplier, Multiplier * 1000.0) Then
            If Not verbose Then
                Return Format(numIn / (Multiplier * 100), NumericStyle) & "m"
            Else
                Return "" & numIn / (Multiplier * 100) & " milli"
            End If
        End If

        'Raw numbers
        If pvIsBetween(numIn, 0, 1000) Then
            If Not verbose Then
                Return Format(numIn, NumericStyle)
            Else
                Return numIn
            End If
        End If

        'Kilo numbers
        Multiplier = 1000.0
        If pvIsBetween(numIn, Multiplier, Multiplier * 1000.0) Then
            If Not verbose Then
                Return Format(numIn / Multiplier, NumericStyle) & "k"
            Else
                Return "" & numIn / Multiplier & " kilo"
            End If
        End If

        'Mega numbers
        Multiplier = 1000000.0
        If pvIsBetween(numIn, Multiplier, Multiplier * 1000.0) Then
            If Not verbose Then
                Return Format(numIn / Multiplier, NumericStyle) & "Meg"
            Else
                Return "" & numIn / Multiplier & " Mega"
            End If
        End If

        'Giga numbers
        Multiplier = 1000000000.0
        If numIn >= Multiplier Then
            If Not verbose Then
                Return Format(numIn / Multiplier, NumericStyle) & "Gig"
            Else
                Return "" & numIn / Multiplier & " Giga"
            End If
        End If

        Return "INVALID!" 'to suppress compiler warning
    End Function
End Module
