Imports System.Data
Imports System.IO
Imports System.Diagnostics

Partial Class pAdmin_logApache
    Inherits System.Web.UI.Page
    Protected dt As DataTable

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        'Get Log
        Dim txtIn As String = TextBox1.Text
        Dim sr As New StringReader(TextBox1.Text)
        Dim line As String = sr.ReadLine()

        'Create Table
        dt = logCreateTable()

        'Populate Table
        Do While Not line Is Nothing
            line = sr.ReadLine()
            dt.Rows.Add(logParseLine(line))
        Loop

        'Handle UI
        TextBox1.Visible = False
        Button1.Visible = False
        xGrid.Visible = True
        xGrid.DataSource = dt
        HttpContext.Current.Session("dt") = dt
        xGrid.DataBind()
    End Sub

    'Creates the schema of the file table that is common to all file listing operations
    'Namely:  Filename, Extension, Path
    Protected Function logCreateTable() As Data.DataTable
        Dim Table1 As DataTable
        Table1 = New DataTable("Log")
        For i As Byte = 1 To 12
            'Column 0: UID
            Dim Col1 As DataColumn = New DataColumn("Col" & i)
            Col1.DataType = System.Type.GetType("System.String")
            Table1.Columns.Add(Col1)
        Next
        Return Table1
    End Function

    Protected Function logParseLine(ByVal lineIn As String) As DataRow
        'Debug
        Debug.WriteLine("")
        Debug.WriteLine("")
        Debug.WriteLine("[logParseLine]: " & lineIn)

        'Init New Row
        Dim Row1 As DataRow = dt.NewRow
        Dim tmpWord As String
        Dim i As Byte = 0
        Dim j As Byte = 1
        Dim tmp2 As String

        'Populate New Row
        Do Until j > sysText.txtNumWords(lineIn)
            tmpWord = txtGetWord(lineIn, j)
            'Deal with quoted strings
            If tmpWord(0) = Chr(34) Then
                If tmpWord(tmpWord.Length - 1) <> Chr(34) Then
                    Do
                        j = j + 1
                        tmp2 = txtGetWord(lineIn, j)
                        tmpWord = tmpWord & " " & tmp2
                        If (tmp2.Length > 0) Then
                            If tmp2(tmp2.Length - 1) = Chr(34) Then Exit Do
                        End If
                    Loop
                End If
            End If
            j = j + 1
            Debug.WriteLine("---[" & i & "] " & tmpWord) 'xxx
            Row1(i) = tmpWord
            i = i + 1
        Loop
        Return Row1
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        xGrid.DataSource = HttpContext.Current.Session("dt")
    End Sub
End Class
