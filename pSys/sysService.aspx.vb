
Partial Class pSys_sysService
    Inherits System.Web.UI.Page
    Public Shared blah As Boolean = False
    Protected FriedPartsUpdateService As UpdateService.upDispatcher

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        sysUser.suLoginRequired(Me)
    End Sub

    Protected Sub btnStart_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStart.Click
        xGaugeDispatcherState.Value = 3
        FriedPartsUpdateService = New UpdateService.upDispatcher()
        FriedPartsUpdateService.Start()
        blah = True
    End Sub

    Protected Sub btnStop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStop.Click
        Console.WriteLine("STOP!!! CLICK!")
        xGaugeDispatcherState.Value = 1
        FriedPartsUpdateService.Abort()
        blah = False
        xGaugeDispatcherState.Value = 0
    End Sub

    ''' <summary>
    ''' Every tmrUpdateRate.Interval milliseconds, this event fires. This is the function that 
    ''' updates the page smoothly (without all the screen flickering caused by a full-page postback).
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub tmrUpdateRate_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrUpdateRate.Tick
        'HANDLE UPDATE HERE!
        If blah Then
            ASPxProgressBar2.Value = ASPxProgressBar2.Value + 1
            If ASPxProgressBar2.Value > 100 Then
                ASPxProgressBar2.Value = 0
            End If
        End If

        DoIt()

    End Sub

    Private Sub DoIt()
        'Process
        Dim theData As New UpdateService.upReport

        'Report
        xGridThreads.DataSource = theData.DataSource
        xGridThreads.DataBind()

        lblNumDispatchers.Text = theData.NumDispatcherThreads
        lblNumWorkers.Text = theData.NumWorkerThreads
        lblNumAppPools.Text = theData.NumWebserverProcesses

    End Sub

    Protected Sub btnStop_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles btnStop.Command

    End Sub
End Class
