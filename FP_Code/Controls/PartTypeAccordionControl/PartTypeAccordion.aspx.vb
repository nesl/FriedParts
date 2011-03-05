
Partial Class FP_Code_Controls_PartTypeAccordionControl
    Inherits System.Web.UI.Page

    Private iSelectedID As Int32
    Public Property SelectedID As Int32
        Get
            Return iSelectedID
        End Get
        Set(ByVal value As Int32)
            iSelectedID = value
        End Set
    End Property

    '''<summary>The PartTypeHorizontalAccordion Psuedo-Control</summary>
    Protected ptAccordion As System.Web.UI.FriedParts.PartTypeAccordionControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '======================================================================
        '== PartTypeAccordion (Psuedo-Control)
        '======================================================================
        Const ptAccordianSessionName As String = "padd.PartTypeControl"
        ptAccordion = System.Web.UI.FriedParts.PartTypeAccordionControl.Reload(Me, ptAccordianSessionName)
        ptAccordion.UpdateControls(Me)
        For Each grid As DevExpress.Web.ASPxGridView.ASPxGridView In ptAccordion.GetGrids()
            AddHandler grid.CustomCallback, AddressOf ptAccordion.HandleRowChanged
        Next
        '======================================================================
    End Sub
End Class
