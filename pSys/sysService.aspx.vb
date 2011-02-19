
Partial Class pSys_sysService
    Inherits System.Web.UI.Page
    Protected FriedPartsUpdateService As UpdateService.upDispatcher

    Protected Const ResetPageInSeconds As Byte = 20

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        sysUser.suLoginRequired(Me)

        'Handle initial page data presentation
        If Not IsCallback Then
            UpdatePage()
        End If

        'xxx
        'We add this because there is a bug when you run DevExpress controls unlicensed. Every 
        'Timer tick is adding another "You have installed in Evaluation Mode" box to the file.
        'This causes server memory to explode (adds to html and viewstate).
        'To compensate, every 20 seconds we do a true Post-back instead of an AJAX Call-back 
        'to reset the page and server memory
        Response.AppendHeader("refresh", ResetPageInSeconds) 'Not yet authenticated so try again!

    End Sub

    Protected Sub btnStart_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStart.Click
        FriedPartsUpdateService = New UpdateService.upDispatcher()
        FriedPartsUpdateService.Start()
        xGaugeDispatcherState.Value = 2
    End Sub

    Protected Sub btnStop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStop.Click
        UpdateService.upThreadList.StopAllThreads()
        xGaugeDispatcherState.Value = 2
    End Sub

    ''' <summary>
    ''' Every tmrUpdateRate.Interval milliseconds, this event fires. This is the function that 
    ''' updates the page smoothly (without all the screen flickering caused by a full-page postback).
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub tmrUpdateRate_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrUpdateRate.Tick
        'Update countdown to reset
        xProgress.Maximum = ResetPageInSeconds
        xProgress.Value = xProgress.Value + 1
        xProgress.ForeColor = Drawing.Color.AliceBlue

        If xProgress.Value = xProgress.Maximum Then
            xProgress.Value = 0
        End If
        UpdatePage()
    End Sub

    Protected Sub UpdatePage()
        'HANDLE UPDATE HERE!

        'Process
        Dim theData As New UpdateService.upReport

        'Threads/Processes
        xGridThreads.DataSource = theData.DataSource
        xGridThreads.DataBind()

        'Events
        Dim sqlTxt As String = _
            "USE [FriedParts] " & _
            "SELECT TOP 10 [Date], [ThreadID], [Msg] " & _
            "FROM [update-Log] " & _
            "ORDER BY [Date] DESC"
        Dim dt As New System.Data.DataTable
        SelectRows(dt, sqlTxt)
        xGridEvents.DataSource = dt
        xGridEvents.DataBind()

        'Stats
        lblNumDispatchers.Text = theData.NumDispatcherThreads
        lblNumWorkers.Text = theData.NumWorkerThreads
        lblNumAppPools.Text = theData.NumWebserverProcesses

        'Overall State indications
        Select Case theData.NumDispatcherThreads
            Case 0
                'No Dispatcher!
                lblDispatcherState.Text = "The Update-Service is NOT running!"
                xGaugeDispatcherState.Value = 0
                btnStart.Visible = True
                btnStop.Visible = False
            Case 1
                lblDispatcherState.Text = "Update-Service running normally..."
                xGaugeDispatcherState.Value = 3
                btnStart.Visible = False
                btnStop.Visible = True
            Case Else
                'Too many dispatchers!
                lblDispatcherState.Text = "Their are too many dispatchers! Restart!"
                xGaugeDispatcherState.Value = 1
                btnStart.Visible = False
                btnStop.Visible = True
        End Select
    End Sub
End Class
