Public Class rptBomBuild
    Inherits DevExpress.XtraReports.UI.XtraReport
    Private theBOM As fpBOM

#Region " Designer generated code "

    Public Sub New()
        MyBase.New()

        theBOM = New fpBOM(HttpContext.Current.Session("bom.ProjectID"))

        'This call is required by the Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

        'Datasource
        Me.DataSource = theBOM.GetDataSource
        Me.DataMember = Me.DataMember '-This line looks stupid, but it is actually VERY necessary if you 
        '                               dynamically assign the datasource -- otherwise you get just one row in the report
        '                               http://www.devexpress.com/Support/Center/p/Q243967.aspx

        'Title Block
        xrLabelTitle.Text = theBOM.GetTitle & " (" & theBOM.GetRevision & ")"
        xrLabelDesc.Text = theBOM.GetDescription
    End Sub

    'XtraReport overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub
    Private WithEvents xrLabel9 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel10 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel11 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel12 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel13 As DevExpress.XtraReports.UI.XRLabel
    Private proj_BOM_ReportAdapter1 As rptBomBuildDatasourceTableAdapters.proj_BOM_ReportAdapter
    Private rptBomBuildDatasource1 As rptBomBuildDatasource
    Private WithEvents Title As DevExpress.XtraReports.UI.XRControlStyle
    Private WithEvents FieldCaption As DevExpress.XtraReports.UI.XRControlStyle
    Private WithEvents PageInfo As DevExpress.XtraReports.UI.XRControlStyle
    Private WithEvents DataField As DevExpress.XtraReports.UI.XRControlStyle
    Private WithEvents groupHeaderBand1 As DevExpress.XtraReports.UI.GroupHeaderBand
    Private WithEvents xrCheckDNP As DevExpress.XtraReports.UI.XRCheckBox
    Private WithEvents xrLabelDNP As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLine1 As DevExpress.XtraReports.UI.XRLine
    Private WithEvents groupHeaderBand2 As DevExpress.XtraReports.UI.GroupHeaderBand
    Private WithEvents xrLabel3 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel2 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLine2 As DevExpress.XtraReports.UI.XRLine
    Private WithEvents groupHeaderBand3 As DevExpress.XtraReports.UI.GroupHeaderBand
    Private WithEvents xrLabel4 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel5 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel6 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel7 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel8 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents pageFooterBand1 As DevExpress.XtraReports.UI.PageFooterBand
    Private WithEvents xrLine3 As DevExpress.XtraReports.UI.XRLine
    Private WithEvents xrPageInfo1 As DevExpress.XtraReports.UI.XRPageInfo
    Private WithEvents xrPageInfo2 As DevExpress.XtraReports.UI.XRPageInfo
    Private WithEvents reportHeaderBand1 As DevExpress.XtraReports.UI.ReportHeaderBand
    Private WithEvents xrLine4 As DevExpress.XtraReports.UI.XRLine
    Private WithEvents xrLine5 As DevExpress.XtraReports.UI.XRLine
    Private WithEvents groupFooterBand1 As DevExpress.XtraReports.UI.GroupFooterBand
    Private WithEvents topMarginBand1 As DevExpress.XtraReports.UI.TopMarginBand
    Private WithEvents bottomMarginBand1 As DevExpress.XtraReports.UI.BottomMarginBand
    Private WithEvents xrLabelTitle As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabelDesc As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel22 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrControlStyle1 As DevExpress.XtraReports.UI.XRControlStyle
    Private WithEvents xrControlStyle2 As DevExpress.XtraReports.UI.XRControlStyle
    Private WithEvents xrLabel1 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel14 As DevExpress.XtraReports.UI.XRLabel

    'Required by the Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Designer
    'It can be modified using the Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resourceFileName As String = "rptBomBuild.resx"
        Me.Detail = New DevExpress.XtraReports.UI.DetailBand
        Me.xrLabel9 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel10 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel11 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel12 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel13 = New DevExpress.XtraReports.UI.XRLabel
        Me.proj_BOM_ReportAdapter1 = New rptBomBuildDatasourceTableAdapters.proj_BOM_ReportAdapter
        Me.rptBomBuildDatasource1 = New rptBomBuildDatasource
        Me.Title = New DevExpress.XtraReports.UI.XRControlStyle
        Me.FieldCaption = New DevExpress.XtraReports.UI.XRControlStyle
        Me.PageInfo = New DevExpress.XtraReports.UI.XRControlStyle
        Me.DataField = New DevExpress.XtraReports.UI.XRControlStyle
        Me.groupHeaderBand1 = New DevExpress.XtraReports.UI.GroupHeaderBand
        Me.xrCheckDNP = New DevExpress.XtraReports.UI.XRCheckBox
        Me.xrLabelDNP = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLine1 = New DevExpress.XtraReports.UI.XRLine
        Me.groupHeaderBand2 = New DevExpress.XtraReports.UI.GroupHeaderBand
        Me.xrLabel3 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel2 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLine2 = New DevExpress.XtraReports.UI.XRLine
        Me.groupHeaderBand3 = New DevExpress.XtraReports.UI.GroupHeaderBand
        Me.xrLabel4 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel5 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel6 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel7 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel8 = New DevExpress.XtraReports.UI.XRLabel
        Me.pageFooterBand1 = New DevExpress.XtraReports.UI.PageFooterBand
        Me.xrLine3 = New DevExpress.XtraReports.UI.XRLine
        Me.xrPageInfo1 = New DevExpress.XtraReports.UI.XRPageInfo
        Me.xrPageInfo2 = New DevExpress.XtraReports.UI.XRPageInfo
        Me.reportHeaderBand1 = New DevExpress.XtraReports.UI.ReportHeaderBand
        Me.xrLabel22 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabelDesc = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabelTitle = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLine4 = New DevExpress.XtraReports.UI.XRLine
        Me.xrLine5 = New DevExpress.XtraReports.UI.XRLine
        Me.groupFooterBand1 = New DevExpress.XtraReports.UI.GroupFooterBand
        Me.topMarginBand1 = New DevExpress.XtraReports.UI.TopMarginBand
        Me.bottomMarginBand1 = New DevExpress.XtraReports.UI.BottomMarginBand
        Me.xrControlStyle1 = New DevExpress.XtraReports.UI.XRControlStyle
        Me.xrControlStyle2 = New DevExpress.XtraReports.UI.XRControlStyle
        Me.xrLabel1 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel14 = New DevExpress.XtraReports.UI.XRLabel
        CType(Me.rptBomBuildDatasource1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.xrLabel9, Me.xrLabel10, Me.xrLabel11, Me.xrLabel12, Me.xrLabel13, Me.xrLabel1})
        Me.Detail.EvenStyleName = "xrControlStyle1"
        Me.Detail.HeightF = 23.00002!
        Me.Detail.Name = "Detail"
        Me.Detail.OddStyleName = "xrControlStyle2"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.StyleName = "DataField"
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'xrLabel9
        '
        Me.xrLabel9.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "proj-BOM-Report.Qty")})
        Me.xrLabel9.Font = New System.Drawing.Font("Arial Narrow", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel9.LocationFloat = New DevExpress.Utils.PointFloat(63.76392!, 0.0!)
        Me.xrLabel9.Name = "xrLabel9"
        Me.xrLabel9.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel9.SizeF = New System.Drawing.SizeF(35.0!, 17.00001!)
        Me.xrLabel9.StylePriority.UseFont = False
        Me.xrLabel9.StylePriority.UseTextAlignment = False
        Me.xrLabel9.Text = "xrLabel9"
        Me.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'xrLabel10
        '
        Me.xrLabel10.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "proj-BOM-Report.RefDes")})
        Me.xrLabel10.Font = New System.Drawing.Font("Arial Narrow", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel10.LocationFloat = New DevExpress.Utils.PointFloat(98.76389!, 0.0!)
        Me.xrLabel10.Name = "xrLabel10"
        Me.xrLabel10.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel10.SizeF = New System.Drawing.SizeF(110.7639!, 17.0!)
        Me.xrLabel10.StylePriority.UseFont = False
        Me.xrLabel10.Text = "xrLabel10"
        '
        'xrLabel11
        '
        Me.xrLabel11.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "proj-BOM-Report.ID Number")})
        Me.xrLabel11.Font = New System.Drawing.Font("Arial Narrow", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel11.LocationFloat = New DevExpress.Utils.PointFloat(34.99997!, 0.0!)
        Me.xrLabel11.Name = "xrLabel11"
        Me.xrLabel11.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel11.SizeF = New System.Drawing.SizeF(28.7639!, 17.00001!)
        Me.xrLabel11.StylePriority.UseFont = False
        Me.xrLabel11.StylePriority.UseTextAlignment = False
        Me.xrLabel11.Text = "xrLabel11"
        Me.xrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'xrLabel12
        '
        Me.xrLabel12.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "proj-BOM-Report.Part Number")})
        Me.xrLabel12.Font = New System.Drawing.Font("Arial Narrow", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel12.LocationFloat = New DevExpress.Utils.PointFloat(209.5278!, 0.0!)
        Me.xrLabel12.Name = "xrLabel12"
        Me.xrLabel12.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel12.SizeF = New System.Drawing.SizeF(192.1388!, 17.00002!)
        Me.xrLabel12.StylePriority.UseFont = False
        Me.xrLabel12.Text = "xrLabel12"
        '
        'xrLabel13
        '
        Me.xrLabel13.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "proj-BOM-Report.Description")})
        Me.xrLabel13.Font = New System.Drawing.Font("Arial Narrow", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel13.LocationFloat = New DevExpress.Utils.PointFloat(401.6667!, 0.0!)
        Me.xrLabel13.Name = "xrLabel13"
        Me.xrLabel13.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel13.SizeF = New System.Drawing.SizeF(238.3333!, 17.00001!)
        Me.xrLabel13.StylePriority.UseFont = False
        Me.xrLabel13.Text = "xrLabel13"
        '
        'proj_BOM_ReportAdapter1
        '
        Me.proj_BOM_ReportAdapter1.ClearBeforeFill = True
        '
        'rptBomBuildDatasource1
        '
        Me.rptBomBuildDatasource1.DataSetName = "rptBomBuildDatasource"
        Me.rptBomBuildDatasource1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'Title
        '
        Me.Title.BackColor = System.Drawing.Color.White
        Me.Title.BorderColor = System.Drawing.SystemColors.ControlText
        Me.Title.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.Title.BorderWidth = 1
        Me.Title.Font = New System.Drawing.Font("Times New Roman", 21.0!)
        Me.Title.ForeColor = System.Drawing.Color.Black
        Me.Title.Name = "Title"
        '
        'FieldCaption
        '
        Me.FieldCaption.BackColor = System.Drawing.Color.White
        Me.FieldCaption.BorderColor = System.Drawing.SystemColors.ControlText
        Me.FieldCaption.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.FieldCaption.BorderWidth = 1
        Me.FieldCaption.Font = New System.Drawing.Font("Times New Roman", 10.0!)
        Me.FieldCaption.ForeColor = System.Drawing.Color.Black
        Me.FieldCaption.Name = "FieldCaption"
        '
        'PageInfo
        '
        Me.PageInfo.BackColor = System.Drawing.Color.White
        Me.PageInfo.BorderColor = System.Drawing.SystemColors.ControlText
        Me.PageInfo.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.PageInfo.BorderWidth = 1
        Me.PageInfo.Font = New System.Drawing.Font("Arial", 8.0!)
        Me.PageInfo.ForeColor = System.Drawing.SystemColors.ControlText
        Me.PageInfo.Name = "PageInfo"
        '
        'DataField
        '
        Me.DataField.BackColor = System.Drawing.Color.White
        Me.DataField.BorderColor = System.Drawing.SystemColors.ControlText
        Me.DataField.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.DataField.BorderWidth = 1
        Me.DataField.Font = New System.Drawing.Font("Arial", 9.0!)
        Me.DataField.ForeColor = System.Drawing.SystemColors.ControlText
        Me.DataField.Name = "DataField"
        Me.DataField.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        '
        'groupHeaderBand1
        '
        Me.groupHeaderBand1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.xrCheckDNP, Me.xrLabelDNP, Me.xrLine1})
        Me.groupHeaderBand1.GroupFields.AddRange(New DevExpress.XtraReports.UI.GroupField() {New DevExpress.XtraReports.UI.GroupField("DoNotPopulate", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)})
        Me.groupHeaderBand1.HeightF = 34.12501!
        Me.groupHeaderBand1.Level = 2
        Me.groupHeaderBand1.Name = "groupHeaderBand1"
        Me.groupHeaderBand1.RepeatEveryPage = True
        '
        'xrCheckDNP
        '
        Me.xrCheckDNP.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("CheckState", Nothing, "proj-BOM-Report.DoNotPopulate")})
        Me.xrCheckDNP.LocationFloat = New DevExpress.Utils.PointFloat(618.6667!, 10.00001!)
        Me.xrCheckDNP.Name = "xrCheckDNP"
        Me.xrCheckDNP.SizeF = New System.Drawing.SizeF(31.33331!, 24.125!)
        Me.xrCheckDNP.StyleName = "DataField"
        Me.xrCheckDNP.Text = "xrCheckDNP"
        Me.xrCheckDNP.Visible = False
        '
        'xrLabelDNP
        '
        Me.xrLabelDNP.BackColor = System.Drawing.Color.White
        Me.xrLabelDNP.Font = New System.Drawing.Font("Arial Narrow", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabelDNP.LocationFloat = New DevExpress.Utils.PointFloat(6.00001!, 1.999982!)
        Me.xrLabelDNP.Name = "xrLabelDNP"
        Me.xrLabelDNP.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabelDNP.SizeF = New System.Drawing.SizeF(612.6667!, 22.04167!)
        Me.xrLabelDNP.StyleName = "FieldCaption"
        Me.xrLabelDNP.StylePriority.UseBackColor = False
        Me.xrLabelDNP.StylePriority.UseFont = False
        Me.xrLabelDNP.Text = "Do Not Populate"
        '
        'xrLine1
        '
        Me.xrLine1.LocationFloat = New DevExpress.Utils.PointFloat(6.0!, 0.0!)
        Me.xrLine1.Name = "xrLine1"
        Me.xrLine1.SizeF = New System.Drawing.SizeF(202.0!, 2.0!)
        '
        'groupHeaderBand2
        '
        Me.groupHeaderBand2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.xrLabel3, Me.xrLabel2, Me.xrLine2})
        Me.groupHeaderBand2.GroupFields.AddRange(New DevExpress.XtraReports.UI.GroupField() {New DevExpress.XtraReports.UI.GroupField("Class", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)})
        Me.groupHeaderBand2.HeightF = 21.33335!
        Me.groupHeaderBand2.Level = 1
        Me.groupHeaderBand2.Name = "groupHeaderBand2"
        Me.groupHeaderBand2.RepeatEveryPage = True
        '
        'xrLabel3
        '
        Me.xrLabel3.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "proj-BOM-Report.Class")})
        Me.xrLabel3.LocationFloat = New DevExpress.Utils.PointFloat(40.99998!, 2.000014!)
        Me.xrLabel3.Name = "xrLabel3"
        Me.xrLabel3.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel3.SizeF = New System.Drawing.SizeF(603.0!, 15.16666!)
        Me.xrLabel3.StyleName = "DataField"
        Me.xrLabel3.Text = "xrLabel3"
        '
        'xrLabel2
        '
        Me.xrLabel2.Font = New System.Drawing.Font("Arial Narrow", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel2.LocationFloat = New DevExpress.Utils.PointFloat(6.00001!, 2.000014!)
        Me.xrLabel2.Name = "xrLabel2"
        Me.xrLabel2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel2.SizeF = New System.Drawing.SizeF(35.0!, 15.16666!)
        Me.xrLabel2.StyleName = "FieldCaption"
        Me.xrLabel2.StylePriority.UseFont = False
        Me.xrLabel2.StylePriority.UseTextAlignment = False
        Me.xrLabel2.Text = "Class"
        Me.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'xrLine2
        '
        Me.xrLine2.LocationFloat = New DevExpress.Utils.PointFloat(6.0!, 0.0!)
        Me.xrLine2.Name = "xrLine2"
        Me.xrLine2.SizeF = New System.Drawing.SizeF(76.0!, 2.0!)
        '
        'groupHeaderBand3
        '
        Me.groupHeaderBand3.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.xrLabel14, Me.xrLabel4, Me.xrLabel5, Me.xrLabel6, Me.xrLabel7, Me.xrLabel8})
        Me.groupHeaderBand3.HeightF = 24.00001!
        Me.groupHeaderBand3.Name = "groupHeaderBand3"
        Me.groupHeaderBand3.RepeatEveryPage = True
        Me.groupHeaderBand3.StyleName = "FieldCaption"
        '
        'xrLabel4
        '
        Me.xrLabel4.Font = New System.Drawing.Font("Arial Narrow", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel4.LocationFloat = New DevExpress.Utils.PointFloat(63.76392!, 6.0!)
        Me.xrLabel4.Name = "xrLabel4"
        Me.xrLabel4.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel4.SizeF = New System.Drawing.SizeF(35.0!, 18.0!)
        Me.xrLabel4.StylePriority.UseFont = False
        Me.xrLabel4.StylePriority.UseTextAlignment = False
        Me.xrLabel4.Text = "Qty"
        Me.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'xrLabel5
        '
        Me.xrLabel5.Font = New System.Drawing.Font("Arial Narrow", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel5.LocationFloat = New DevExpress.Utils.PointFloat(98.76389!, 6.0!)
        Me.xrLabel5.Name = "xrLabel5"
        Me.xrLabel5.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel5.SizeF = New System.Drawing.SizeF(65.00002!, 18.0!)
        Me.xrLabel5.StylePriority.UseFont = False
        Me.xrLabel5.StylePriority.UseTextAlignment = False
        Me.xrLabel5.Text = "Ref Des"
        Me.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'xrLabel6
        '
        Me.xrLabel6.Font = New System.Drawing.Font("Arial Narrow", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel6.LocationFloat = New DevExpress.Utils.PointFloat(34.99997!, 6.0!)
        Me.xrLabel6.Name = "xrLabel6"
        Me.xrLabel6.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel6.SizeF = New System.Drawing.SizeF(28.76392!, 18.0!)
        Me.xrLabel6.StylePriority.UseFont = False
        Me.xrLabel6.StylePriority.UseTextAlignment = False
        Me.xrLabel6.Text = "ID#"
        Me.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'xrLabel7
        '
        Me.xrLabel7.Font = New System.Drawing.Font("Arial Narrow", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel7.LocationFloat = New DevExpress.Utils.PointFloat(209.5278!, 6.0!)
        Me.xrLabel7.Name = "xrLabel7"
        Me.xrLabel7.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel7.SizeF = New System.Drawing.SizeF(138.4721!, 18.0!)
        Me.xrLabel7.StylePriority.UseFont = False
        Me.xrLabel7.StylePriority.UseTextAlignment = False
        Me.xrLabel7.Text = "Part Number"
        Me.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'xrLabel8
        '
        Me.xrLabel8.Font = New System.Drawing.Font("Arial Narrow", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel8.LocationFloat = New DevExpress.Utils.PointFloat(401.6667!, 6.00001!)
        Me.xrLabel8.Name = "xrLabel8"
        Me.xrLabel8.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel8.SizeF = New System.Drawing.SizeF(238.3333!, 18.0!)
        Me.xrLabel8.StylePriority.UseFont = False
        Me.xrLabel8.StylePriority.UseTextAlignment = False
        Me.xrLabel8.Text = "Description"
        Me.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'pageFooterBand1
        '
        Me.pageFooterBand1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.xrLine3, Me.xrPageInfo1, Me.xrPageInfo2})
        Me.pageFooterBand1.HeightF = 31.0!
        Me.pageFooterBand1.Name = "pageFooterBand1"
        '
        'xrLine3
        '
        Me.xrLine3.LocationFloat = New DevExpress.Utils.PointFloat(6.0!, 0.0!)
        Me.xrLine3.Name = "xrLine3"
        Me.xrLine3.SizeF = New System.Drawing.SizeF(638.0!, 2.0!)
        '
        'xrPageInfo1
        '
        Me.xrPageInfo1.LocationFloat = New DevExpress.Utils.PointFloat(6.0!, 8.0!)
        Me.xrPageInfo1.Name = "xrPageInfo1"
        Me.xrPageInfo1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrPageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime
        Me.xrPageInfo1.SizeF = New System.Drawing.SizeF(313.0!, 23.0!)
        Me.xrPageInfo1.StyleName = "PageInfo"
        '
        'xrPageInfo2
        '
        Me.xrPageInfo2.Format = "Page {0} of {1}"
        Me.xrPageInfo2.LocationFloat = New DevExpress.Utils.PointFloat(331.0!, 8.0!)
        Me.xrPageInfo2.Name = "xrPageInfo2"
        Me.xrPageInfo2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrPageInfo2.SizeF = New System.Drawing.SizeF(313.0!, 23.0!)
        Me.xrPageInfo2.StyleName = "PageInfo"
        Me.xrPageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'reportHeaderBand1
        '
        Me.reportHeaderBand1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.xrLabel22, Me.xrLabelDesc, Me.xrLabelTitle, Me.xrLine4, Me.xrLine5})
        Me.reportHeaderBand1.HeightF = 44.37497!
        Me.reportHeaderBand1.Name = "reportHeaderBand1"
        '
        'xrLabel22
        '
        Me.xrLabel22.Font = New System.Drawing.Font("Arial Narrow", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel22.LocationFloat = New DevExpress.Utils.PointFloat(459.7916!, 2.000014!)
        Me.xrLabel22.Name = "xrLabel22"
        Me.xrLabel22.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel22.SizeF = New System.Drawing.SizeF(180.2084!, 25.08329!)
        Me.xrLabel22.StylePriority.UseFont = False
        Me.xrLabel22.StylePriority.UseTextAlignment = False
        Me.xrLabel22.Text = "Bill of Materials"
        Me.xrLabel22.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'xrLabelDesc
        '
        Me.xrLabelDesc.Font = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabelDesc.LocationFloat = New DevExpress.Utils.PointFloat(9.999974!, 27.0833!)
        Me.xrLabelDesc.Name = "xrLabelDesc"
        Me.xrLabelDesc.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.xrLabelDesc.SizeF = New System.Drawing.SizeF(630.0!, 10.50002!)
        Me.xrLabelDesc.StylePriority.UseFont = False
        Me.xrLabelDesc.StylePriority.UsePadding = False
        Me.xrLabelDesc.StylePriority.UseTextAlignment = False
        Me.xrLabelDesc.Text = "Project Description"
        Me.xrLabelDesc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomLeft
        '
        'xrLabelTitle
        '
        Me.xrLabelTitle.Font = New System.Drawing.Font("Arial Narrow", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabelTitle.LocationFloat = New DevExpress.Utils.PointFloat(9.999974!, 2.000014!)
        Me.xrLabelTitle.Name = "xrLabelTitle"
        Me.xrLabelTitle.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabelTitle.SizeF = New System.Drawing.SizeF(391.6667!, 25.08329!)
        Me.xrLabelTitle.StylePriority.UseFont = False
        Me.xrLabelTitle.Text = "PROJECT TITLE"
        '
        'xrLine4
        '
        Me.xrLine4.LocationFloat = New DevExpress.Utils.PointFloat(6.0!, 0.0!)
        Me.xrLine4.Name = "xrLine4"
        Me.xrLine4.SizeF = New System.Drawing.SizeF(638.0!, 2.0!)
        '
        'xrLine5
        '
        Me.xrLine5.LocationFloat = New DevExpress.Utils.PointFloat(6.00001!, 37.58332!)
        Me.xrLine5.Name = "xrLine5"
        Me.xrLine5.SizeF = New System.Drawing.SizeF(638.0!, 2.0!)
        '
        'groupFooterBand1
        '
        Me.groupFooterBand1.HeightF = 1.0!
        Me.groupFooterBand1.Name = "groupFooterBand1"
        '
        'topMarginBand1
        '
        Me.topMarginBand1.Name = "topMarginBand1"
        '
        'bottomMarginBand1
        '
        Me.bottomMarginBand1.Name = "bottomMarginBand1"
        '
        'xrControlStyle1
        '
        Me.xrControlStyle1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.xrControlStyle1.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrControlStyle1.Name = "xrControlStyle1"
        Me.xrControlStyle1.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        '
        'xrControlStyle2
        '
        Me.xrControlStyle2.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrControlStyle2.Name = "xrControlStyle2"
        Me.xrControlStyle2.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        '
        'xrLabel1
        '
        Me.xrLabel1.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "proj-BOM-Report.Item")})
        Me.xrLabel1.Font = New System.Drawing.Font("Arial Narrow", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel1.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.xrLabel1.Name = "xrLabel1"
        Me.xrLabel1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel1.SizeF = New System.Drawing.SizeF(35.0!, 17.0!)
        Me.xrLabel1.StylePriority.UseFont = False
        Me.xrLabel1.StylePriority.UseTextAlignment = False
        Me.xrLabel1.Text = "xrLabel1"
        Me.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'xrLabel14
        '
        Me.xrLabel14.Font = New System.Drawing.Font("Arial Narrow", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel14.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 6.0!)
        Me.xrLabel14.Name = "xrLabel14"
        Me.xrLabel14.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel14.SizeF = New System.Drawing.SizeF(34.99997!, 18.0!)
        Me.xrLabel14.StylePriority.UseFont = False
        Me.xrLabel14.StylePriority.UseTextAlignment = False
        Me.xrLabel14.Text = "Line#"
        Me.xrLabel14.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'rptBomBuild
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.groupHeaderBand1, Me.groupHeaderBand2, Me.groupHeaderBand3, Me.pageFooterBand1, Me.reportHeaderBand1, Me.topMarginBand1, Me.bottomMarginBand1})
        Me.DataAdapter = Me.proj_BOM_ReportAdapter1
        Me.DataMember = "proj-BOM-Report"
        Me.DataSource = Me.rptBomBuildDatasource1
        Me.Font = New System.Drawing.Font("Arial Narrow", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StyleSheet.AddRange(New DevExpress.XtraReports.UI.XRControlStyle() {Me.Title, Me.FieldCaption, Me.PageInfo, Me.DataField, Me.xrControlStyle1, Me.xrControlStyle2})
        Me.Version = "10.2"
        CType(Me.rptBomBuildDatasource1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand

#End Region

    Private Sub groupHeaderBand1_AfterPrint(ByVal sender As Object, ByVal e As System.EventArgs) Handles groupHeaderBand1.AfterPrint

    End Sub

    Private Sub Detail_BeforePrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles Detail.BeforePrint

    End Sub

    'DO NOT POPULATE LEVEL
    Private Sub xrLabelDNP_BeforePrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles xrLabelDNP.BeforePrint
        If xrCheckDNP.Checked Then
            'Do Not Populate!
            xrLabelDNP.Text = "DO NOT POPULATE"
            xrLabelDNP.BackColor = Drawing.Color.Yellow
        Else
            'Populate These!
            xrLabelDNP.Text = "Included Parts"
            xrLabelDNP.BackColor = Drawing.Color.White
        End If
    End Sub
End Class