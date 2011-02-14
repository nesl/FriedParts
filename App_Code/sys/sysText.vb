Imports Microsoft.VisualBasic
Imports System
Imports System.Web
Imports System.Text.RegularExpressions

Public Module sysText

    Public Class txtPathAndFilename
        Private PaF As String = Nothing
        Private P As String = Nothing
        Private F As String = Nothing
        Private X As String = Nothing

        Public ReadOnly Property GetPath As String
            Get
                Return P
            End Get
        End Property

        'Filename includes the extension (e.g. F = "TextFile.txt" for "TextFile.txt")
        Public ReadOnly Property GetFilename As String
            Get
                Return F
            End Get
        End Property

        'Extension does not include the delimiter (e.g. X = "txt" for "TextFile.txt")
        Public ReadOnly Property GetExtension As String
            Get
                Return X
            End Get
        End Property
        Public ReadOnly Property GetPathSubDirsOnly As String
            Get
                If txtNumOfSubstrings(P, "/") > 2 Then
                    Return P.Substring(1).Substring(P.Substring(1).IndexOf("/"))
                Else
                    Return "/"
                End If
            End Get
        End Property
        'Gets the last folder -- e.g. "/Dropbox/FriedParts/SchLibs/" ==> "/SchLibs/"
        Public ReadOnly Property GetPathLastDirOnly As String
            Get
                If txtNumOfSubstrings(P, "/") > 2 Then
                    Dim startIdx As Int16 = Left(P, P.Length - 1).LastIndexOf("/")
                    Return P.Substring(startIdx)
                Else
                    Return "/"
                End If
            End Get
        End Property

        'Set IsFile to DropNet.Models.MetaData.Is_Dir() -- true if the path contains a file, false if not
        Public Sub New(ByRef PathAndFilename As String, Optional ByRef IsFile As Boolean = True)
            PaF = PathAndFilename.Replace("\", "/")

            'Deal with empty string
            If PaF.Length = 0 Then Exit Sub

            'Deal with no Filename!
            If Not IsFile Then
                If Right(PaF, 1).CompareTo("/") = 0 Then
                    P = PaF
                Else
                    P = PaF & "/"
                End If
                Exit Sub
            End If

            'Deal with the usual case...
            F = txtGetWord(PaF, txtNumWords(PaF, "/"), "/")

            'Deal with no extension
            If txtNumOfSubstrings(F, ".") < 1 Then
                'No extension! (move on)
            Else
                'Has an extension!
                X = txtGetWord(F, txtNumWords(F, "."), ".")
            End If

            'Deal with no Path!
            If txtNumOfSubstrings(PaF, "/") = 0 Then
                'No path!
                P = ""
            Else
                'Normal case...
                P = Left(PaF, PaF.LastIndexOf("/") + 1)
            End If
        End Sub
    End Class

    Public Function txtSplitPath(ByRef PathAndFilename As String, Optional ByRef IsFile As Boolean = True) As txtPathAndFilename
        Return New txtPathAndFilename(PathAndFilename, IsFile)
    End Function

    'Takes in an array of strings and returns a single concatenated string separated by the separator string
    Public Function txtArrayToString(ByRef StringArray() As String, Optional ByRef Separator As String = ", ") As String
        Dim sb As New StringBuilder
        For Each s In StringArray
            sb.Append(s)
            sb.Append(Separator)
        Next
        sb.Remove(sb.Length - Separator.Length, Separator.Length) 'Delete the last separator
        Return sb.ToString
    End Function

    'Converts a Microsoft Date into a SQL valid date string
    'When inserting into SQL strings do not preceed/proceed with apostrophe (e.g. ')
    'Ex.  "INSERT INTO [Table] ([Column1], [Column2]) VALUES (" & txtSQLDate(now) & ", '" & string_variable & "')"
    Public Function txtSqlDate(ByVal microsoftDate As Date) As String
        Return "convert(datetime, '" & FormatDateTime(microsoftDate, DateFormat.GeneralDate) & "')"
    End Function

    'Converts a Microsoft Date expression into a Date-Only text string in the 
    'format: 1/14/2010
    Public Function txtDateOnly(ByVal microsoftDateTime As Date) As String
        Return microsoftDateTime.Month & "/" & microsoftDateTime.Day & "/" & microsoftDateTime.Year
    End Function

    ' Return a token. If new_txt is blank, return the
    ' next token from the previous string. If new_txt
    ' is not blank, return its first token.
    'Demo Code:
    'txt = GetToken(Text1.Text, ";") & vbCrLf
    '    Do
    '        token = GetToken("", ";")
    '        If token = "" Then Exit Do
    '        txt = txt & token & vbCrLf
    '    Loop
    Public Function txtGetToken(ByVal new_txt As String, ByVal delimiter As _
        String) As String
        Static txt As String
        Dim pos As Integer

        ' Save new text.
        If new_txt <> "" Then txt = new_txt

        pos = InStr(txt, delimiter)
        If pos < 1 Then pos = Len(txt) + 1
        txtGetToken = Left$(txt, pos - 1)
        pos = Len(txt) - pos + 1 - Len(delimiter)
        If pos < 1 Then
            txt = ""
        Else
            txt = Right$(txt, pos)
        End If
    End Function

    'Reports the character contents of the string to logDebug
    Public Sub txtDbgChars(ByVal txtIn As String)
        Dim i As Int16
        Dim c As Char
        If txtIn.Length > 0 Then
            For i = 0 To txtIn.Length - 1
                c = txtIn.Substring(i, 1)
                dbLog.logDebug("  [" & i & "]   ASCII: " & c & "   Code: " & Asc(c))
            Next
        Else
            dbLog.logDebug("Empty String Received")
        End If
    End Sub

    Public Function txtDbgArray(ByRef arrayIn() As Int32) As String
        Dim str As String = ""
        Dim counter As Int16 = 0
        For Each i As Int32 In arrayIn
            str = str & "[" & counter & "] " & i.ToString & ", "
        Next
        Return str
    End Function

    'Defangs (makes safe) text strings for use as values in SQL queries
    'I add to this as I encounter stuff
    'Handles NULL values by replacing with empty string
    Public Function txtDefangSQL(ByVal txtIn As String) As String
        If txtIn Is DBNull.Value Or txtIn Is Nothing Then
            Return ""
        Else
            txtIn = txtIn.Replace("'", "`") 'APOSTROPHE: Can cause SQL string escape
            txtIn = txtIn.Replace(Chr(34), "``") 'QUOTES: Can cause SQL string escape
            txtIn = txtIn.Replace(Chr(127), "") 'The ASCII Delete character
            Return txtIn
        End If
    End Function

    'Returns a count of the number of whitespace characters in a string
    Public Function txtNumOfSpaces(ByVal txtIn As String) As Int16
        'Convert all white space to spaces... then count them!
        Return txtNumOfSubstrings(txtWhitesToSpaces(txtIn), " ")
    End Function

    'Returns a count of the number of times the search string (searchChar) appears in txtIn
    Public Function txtNumOfSubstrings(ByVal txtIn As String, ByVal searchChar As String) As Int16
        Dim i As Int16 = 0
        Dim Count As Int16 = -1
        If txtIn.StartsWith(searchChar) Then
            Count = 0 'Incremement Count if first character is a match -- deal with initial condition
        End If
        While (i < Len(txtIn) And i >= 0)
            Count += 1
            i = txtIn.IndexOf(searchChar, i + 1)
        End While
        Return Count
    End Function

    'Returns the number of 'Words' in the string. Words are numbered from 1 and demarkated by spaces
    'Leading/trailing white space ignored
    Public Function txtNumWords(ByVal txtIn As String, Optional ByRef wordDelim As String = " ") As Byte
        Return txtNumOfSubstrings(Trim(txtIn), wordDelim) + 1
    End Function

    'Returns the word indicated. Words are separated by the spacebar. Words are numbered from 1. 
    'Example: txtIn = "This is a test."; txtGetWord(txtIn, 4) ==> "test."
    Public Function txtGetWord(ByVal txtIn As String, ByVal wordNumber As Byte, Optional ByRef wordDelim As String = " ") As String
        Dim txtWorking As String
        'Format txtIn
        txtWorking = Trim(txtWhitesToSpaces(txtIn))
        'Error checking: wordNumber is valid
        If (wordNumber > 0) Then
            If wordNumber <= txtNumWords(txtWorking, wordDelim) Then
                'Simplify finding the last word (append a final space)
                txtWorking = txtWorking & wordDelim
                'Find the requested word
                Dim lastidx As Int16 = -1
                Dim startidx As Int16
                For i As Byte = 1 To wordNumber
                    startidx = lastidx + 1
                    lastidx = txtWorking.IndexOf(wordDelim, lastidx + 1)
                Next
                Return txtWorking.Substring(startidx, lastidx - startidx)
            Else
                Err.Raise("Requested word is greater than the total number of words in the string!")
                Return 0 'intended behavior is program crash/halt, since this is a developer mistake; return added to make compiler warning shutup
            End If
        Else
            Err.Raise("Invalid word number requested. Words are numbered from 1 to 255.")
            Return 0 'to make compiler warning shutup
        End If
    End Function

    'Converts all white space characters to space characters. Useful for logging, debugging, input defanging
    Public Function txtWhitesToSpaces(ByVal txtIn As String) As String
        txtIn = txtIn.Replace(Chr(13), " ") 'CR
        txtIn = txtIn.Replace(Chr(10), " ") 'LF
        txtIn = txtIn.Replace(Chr(9), " ") 'Tab
        txtIn = txtIn.Replace(Chr(27), " ") 'ESC
        txtIn = txtIn.Replace(Chr(11), " ") 'Vertical Tab
        Return txtIn
    End Function

    'Encodes the specified text in a manner safe for passing as a string field for storage in a SQL database
    Public Function txtSqlEncode(ByVal txtIn As String) As String
        Dim strI As String = HttpContext.Current.Server.UrlEncode(txtIn)
        Return strI.Replace("'", "''") 'escape the apostrophe since URL-encoding doesn't
    End Function
    'Overloads: Converts Microsoft Boolean to a SQL Boolean (decode is not needed because VB.NET does it automatically)
    Public Function txtSqlEncode(ByVal msBoolean As Boolean) As Byte
        If msBoolean Then
            Return sqlTRUE
        Else
            Return sqlFALSE
        End If
    End Function

    'Decodes the previously encoded specified text in a manner safe for passing as a string field for storage in a SQL database
    Public Function txtSqlDecode(ByVal txtIn As String) As String
        Dim strI As String = txtIn
        'The following line might be necessary, but I don't think so -- needs testing!
        'strI = txtIn.Replace("''", "'") 'reverse the apostrophe encoding
        Return HttpContext.Current.Server.UrlDecode(strI)
    End Function

    Public Function txtRemoveNonPrintable(ByVal txtIn As String) As String
        Dim i As Int32 = 0
        Dim txtOut As String = ""
        If txtIn.Length < 1 Then
            Return ""
        End If
        For i = 0 To txtIn.Length - 1
            If Asc(txtIn(i)) < 128 Then
                txtOut = txtOut & txtIn(i)
            Else
            End If
        Next
        Return txtOut
    End Function

    'Also removes newlines and stuff.
    'Also removes non-printable characters.
    Public Function txtRemoveWhiteSpace(ByVal txtIn As String) As String
        txtIn = txtRemoveNonPrintable(txtIn)
        txtIn = txtWhitesToSpaces(txtIn)
        txtIn = txtIn.Replace(" ", "")
        Return txtIn
    End Function

    'Makes a database returned string display safe -- converts nulls to empty strings
    Public Function txtNullToEmpty(ByRef strIn As String) As String
        If strIn Is Nothing Then
            Return ""
        Else
            Return strIn
        End If
    End Function

    'Checks if the string is in valid URL format
    Public Function txtValidURL(ByVal urlIn As String) As Boolean
        If urlIn <> txtRemoveWhiteSpace(urlIn) Then Return False
        urlIn = urlIn.ToLower
        If urlIn.StartsWith("http://") Then
            Return True
        Else
            Return False
        End If
    End Function
End Module
