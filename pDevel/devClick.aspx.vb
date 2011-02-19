Imports System.Data
Imports UpdateService
Imports System.Diagnostics

Partial Class pDevel_devClick
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click

        devTestSemaphore()
        'devTestThreadManagement()

        'devTestUpdateService()
        'Dim blah As New txtPathAndFilename("/This/is/a/test.doc", False)
        'Label2.Text = blah.GetPath
        'Label2b.Text = blah.GetFilename

        'OLD TEST CODE
        '=============
        'devTestsysTextModule()
    End Sub

    Private MustInherit Class Base
        Public MustOverride Function LockRequest() As Boolean
        Public MustOverride Sub Release()
        Public Function KeystoneLock() As Boolean
            'This is the climax!
            Return LockRequest()
        End Function
        Public Sub New()
        End Sub
    End Class

    Private Class Deriv1
        Inherits Base
        Public Shared Shadows TheSem As Threading.Semaphore
        Public Overrides Function LockRequest() As Boolean
            Return TheSem.WaitOne(1)
        End Function
        Public Overrides Sub Release()
            TheSem.Release(1)
        End Sub
        Public Sub New()
            If TheSem Is Nothing Then TheSem = New Threading.Semaphore(1, 1)
        End Sub
    End Class

    Private Class Deriv2
        Inherits Base
        Public Shared Shadows TheSem As Threading.Semaphore
        Public Overrides Function LockRequest() As Boolean
            Return TheSem.WaitOne(1)
        End Function
        Public Overrides Sub Release()
            TheSem.Release(1)
        End Sub
        Public Sub New()
            If TheSem Is Nothing Then TheSem = New Threading.Semaphore(1, 1)
        End Sub
    End Class

    Private Sub devTestSemaphore()
        Dim theSem As New Threading.Semaphore(1, 1)
        Dim obj1a As New Deriv1
        Dim obj1b As New Deriv1
        Dim obj2a As New Deriv2
        Label2.Text = "Semaphore Lock Request Result:"
        Label2b.Text = "" & obj1a.KeystoneLock & ", " & obj1b.KeystoneLock & ", " & obj2a.KeystoneLock
        Label3.Text = "Subsequent Request:"
        Label3b.Text = ""


    End Sub

    Private Sub devTestThreadManagement()
        Dim IIS_Processes() As Process
        Dim AllProcesses() As Process
        IIS_Processes = Process.GetProcessesByName("w3wp")
        AllProcesses = Process.GetProcesses
        Dim i As Int32 = 45
    End Sub

    Private Sub devTestUpdateService()
        'Dim blah As New upService(True, False)
        'Label2.Text = "Result from UpdateServiceDispatcher"
        'Label2b.Text = blah.Start()
    End Sub

    Private Sub devTestsysTextModule()
        Label0a.Text = "Input String"
        Label0b.Text = "Delimiter"
        Label1b.Text = txtNumOfSubstrings(txtInput0a.Text, txtInput0b.Text)
        Label2b.Text = txtNumWords(txtInput0a.Text, txtInput0b.Text)
        Dim wordnum As Byte = 3
        Label3.Text = "Get Word Number " & wordnum
        Label3b.Text = txtGetWord(txtInput0a.Text, wordnum, txtInput0b.Text)
    End Sub

    Private Sub devTimespan()
        Dim W As Date
        Dim TS As New TimeSpan(1, 0, 0, 0)
        W = Now
        Label1.Text = W.Date.Month & "/" & W.Date.Day & "/" & W.Date.Year
        W = W + TS
        Label1.Text = Label1.Text & " ||| " & W.Date.Month & "/" & W.Date.Day & "/" & W.Date.Year
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dt As DataTable = New fpProj.fpBOM(8).GetDataSource
        xGrid.DataSource = dt
    End Sub
End Class
