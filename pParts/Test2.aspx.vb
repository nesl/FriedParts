
Partial Class pParts_Test2
    Inherits System.Web.UI.Page

    Protected ptAccordion As System.Web.UI.FriedParts.PartTypeAccordionControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'sysUser.suLoginRequired(Me) 'Access control

        Const ptAccordianSessionName As String = "padd.PartTypeControl"
        ptAccordion = System.Web.UI.FriedParts.PartTypeAccordionControl.Reload(Me, ptAccordianSessionName)
        ptAccordion.UpdateControls(Me)
        For Each grid As DevExpress.Web.ASPxGridView.ASPxGridView In ptAccordion.GetGrids()
            AddHandler grid.CustomCallback, AddressOf ptAccordion.HandleRowChanged
        Next

    End Sub

End Class
