Imports DevExpress.Web.ASPxGridView
Imports System.Data

Namespace System.Web.UI.FriedParts

    ''' <summary>
    ''' This implements all of the setup and operational logic of the 
    ''' PartTypeAccordion Web Server Control
    ''' </summary>
    ''' <remarks>We couldn't package this as an actual control because it couldn't emit 
    ''' the DevExpress controls on its own. So we sort of hack-tasticly make our own
    ''' loose approximate of a true server control using includes.</remarks>
    Public Class PartTypeAccordionControl

        ''' <summary>
        ''' Internal datasource.
        ''' </summary>
        ''' <remarks></remarks>
        Protected ptData As PartTypeDatasourceControl

        ''' <summary>
        ''' Contains all of the controls on the hosting webpage.
        ''' We need this in order to manipulate the controls on the host page and implement UI behavior.
        ''' </summary>
        ''' <remarks></remarks>
        Protected allControls As sysControlWalker

        ''' <summary>
        ''' Get's the currently selected PartTypeID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetSelectedTypeID As Int32
            Get
                Return ptData.GetParentID
            End Get
        End Property

        ''' <summary>
        ''' The name of the variable in the user's session where this control 
        ''' will be stored between requests to maintain state.
        ''' </summary>
        ''' <remarks></remarks>
        Protected TheSessionStoreLocation As String

        ''' <summary>
        ''' Maximum number of user-parttype levels supported for a category.
        ''' Levels start at 2. Level 1 is "All Parts". Users may not add 
        ''' additional categories (PartTypes) at level 1.
        ''' </summary>
        ''' <remarks></remarks>
        Public Const MAX_DEPTH As Byte = 4

        ''' <summary>
        ''' Call this function everytime the page reloads (e.g. on Callbacks, Postbacks, etc...)
        ''' </summary>
        ''' <param name="SessionControlName">The control's variable name in the user's session state</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Reload(ByRef SessionControlName As String) As PartTypeAccordionControl
            If HttpContext.Current.Session(SessionControlName) Is Nothing Then
                Throw New NullReferenceException("The PartTypeAccordionControl must be initialized before you reload it! Use New(SessionName)")
            Else
                Dim hPTa As PartTypeAccordionControl = HttpContext.Current.Session(SessionControlName)
                hPTa.Update(hPTa.GetSelectedTypeID)
                Return hPTa
            End If
        End Function

        Public Sub Init(ByRef TheHostPage As System.Web.UI.Page)
            'Collect the applicable controls
            allControls = New sysControlWalker(TheHostPage)

            'Add event handlers
            Dim gv As ASPxGridView
            For i As Byte = 2 To MAX_DEPTH
                gv = DirectCast(allControls.FindControl("L" & i & "g"), ASPxGridView)
                AddHandler gv.FocusedRowChanged, AddressOf HandleRowChanged
            Next

            'Set Initial Values
            ptData = New PartTypeDatasourceControl()
        End Sub

        Protected Sub Update(Optional ByVal TypeID As Integer = 0)
            Dim ParentLevel As Byte = ptGetLevel(TypeID)
            Dim gv As ASPxGridView
            ptData.updateDatasource(TypeID)
            For i As Byte = 2 To MAX_DEPTH
                If i > ParentLevel + 1 Then
                    'Hide these!
                    DirectCast(allControls.FindControl("L" & i & "a"), HtmlGenericControl).Visible = False
                    DirectCast(allControls.FindControl("L" & i & "b"), HtmlGenericControl).Visible = False
                Else
                    'Show these!
                    DirectCast(allControls.FindControl("L" & i & "a"), HtmlGenericControl).Visible = True
                    DirectCast(allControls.FindControl("L" & i & "b"), HtmlGenericControl).Visible = True
                    'Default tab...
                    If i = ParentLevel + 1 Then
                        'This one is the one we open to...
                        DirectCast(allControls.FindControl("L" & i & "a"), HtmlGenericControl).Attributes.Add("class", "active")
                    Else
                        '...not these!
                    End If
                    'Load 'em up!
                    gv = DirectCast(allControls.FindControl("L" & i & "g"), ASPxGridView)
                    gv.DataSource = ptData.GetDatasource(i)
                    gv.DataBind()
                    DirectCast(allControls.FindControl("L" & i & "a"), HtmlGenericControl).InnerText = ptData.GetTitles(i)
                End If
            Next
        End Sub


        ' TAB HANDLERS
        '======================================
        Protected Sub HandleRowChanged(ByVal WhichGrid As Object, ByVal e As System.EventArgs)
            'Get data from the grid -- what did user click on?
            Dim gv As ASPxGridView = DirectCast(WhichGrid, ASPxGridView)
            Dim hf As System.Web.UI.WebControls.HiddenField
            Dim drow As DataRow = gv.GetDataRow(gv.FocusedRowIndex)
            'Get the new TypeID the user has just selected
            If drow IsNot Nothing Then
                hf = DirectCast(allControls.FindControl("SelectedPartTypeID"), System.Web.UI.WebControls.HiddenField)
                hf.Value = (drow.Field(Of Int16)("TypeID"))
                'Reflect that change in the backend
                Update(hf.Value)
            End If
        End Sub

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="ThisPage">Pass in "Me"</param>
        ''' <param name="SessionControlName">The variable name in the user's session state. 
        ''' Must be globally unique for this instance.</param>
        ''' <remarks>Instantiate the psuedo-control!</remarks>
        Public Sub New(ByRef ThisPage As System.Web.UI.Page, ByRef SessionControlName As String)
            TheSessionStoreLocation = SessionControlName
            Init(ThisPage)

            'Save State
            HttpContext.Current.Session(TheSessionStoreLocation) = Me
        End Sub

    End Class

End Namespace
