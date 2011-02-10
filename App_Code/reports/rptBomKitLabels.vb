Public Class rptBomKitLabels
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
        Me.DataSource = theBOM.GetDataSource(False)
        Me.DataMember = Me.DataMember '-This line looks stupid, but it is actually VERY necessary if you 
        '                               dynamically assign the datasource -- otherwise you get just one row in the report
        '                               http://www.devexpress.com/Support/Center/p/Q243967.aspx
    End Sub

    'XtraReport overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub
    Private WithEvents xrPanel1 As DevExpress.XtraReports.UI.XRPanel
    Private WithEvents topMarginBand1 As DevExpress.XtraReports.UI.TopMarginBand
    Private WithEvents bottomMarginBand1 As DevExpress.XtraReports.UI.BottomMarginBand
    Private WithEvents rptBomBuildDatasource1 As rptBomBuildDatasource
    Private WithEvents xrLabel1 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel2 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel3 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel4 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel5 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel6 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel7 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel8 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrLabel9 As DevExpress.XtraReports.UI.XRLabel

    'Required by the Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Designer
    'It can be modified using the Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resourceFileName As String = "rptBomKitLabels.resx"
        Me.Detail = New DevExpress.XtraReports.UI.DetailBand
        Me.xrPanel1 = New DevExpress.XtraReports.UI.XRPanel
        Me.xrLabel6 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel5 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel4 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel3 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel2 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel1 = New DevExpress.XtraReports.UI.XRLabel
        Me.topMarginBand1 = New DevExpress.XtraReports.UI.TopMarginBand
        Me.bottomMarginBand1 = New DevExpress.XtraReports.UI.BottomMarginBand
        Me.rptBomBuildDatasource1 = New rptBomBuildDatasource
        Me.xrLabel7 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel8 = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLabel9 = New DevExpress.XtraReports.UI.XRLabel
        CType(Me.rptBomBuildDatasource1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.xrPanel1})
        Me.Detail.MultiColumn.ColumnSpacing = 13.0!
        Me.Detail.MultiColumn.ColumnWidth = 262.0!
        Me.Detail.MultiColumn.Layout = DevExpress.XtraPrinting.ColumnLayout.AcrossThenDown
        Me.Detail.MultiColumn.Mode = DevExpress.XtraReports.UI.MultiColumnMode.UseColumnWidth
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'xrPanel1
        '
        Me.xrPanel1.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.xrPanel1.CanGrow = False
        Me.xrPanel1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.xrLabel7, Me.xrLabel6, Me.xrLabel5, Me.xrLabel4, Me.xrLabel3, Me.xrLabel2, Me.xrLabel1, Me.xrLabel8, Me.xrLabel9})
        Me.xrPanel1.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.xrPanel1.Name = "xrPanel1"
        Me.xrPanel1.SizeF = New System.Drawing.SizeF(262.0!, 100.0!)
        '
        'xrLabel6
        '
        Me.xrLabel6.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.xrLabel6.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "proj-BOM-Report.Qty")})
        Me.xrLabel6.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel6.LocationFloat = New DevExpress.Utils.PointFloat(8.333339!, 76.53831!)
        Me.xrLabel6.Name = "xrLabel6"
        Me.xrLabel6.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel6.SizeF = New System.Drawing.SizeF(42.0!, 14.0!)
        Me.xrLabel6.StylePriority.UseBorders = False
        Me.xrLabel6.StylePriority.UseFont = False
        Me.xrLabel6.StylePriority.UseTextAlignment = False
        Me.xrLabel6.Text = "xrLabel6"
        Me.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'xrLabel5
        '
        Me.xrLabel5.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.xrLabel5.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "proj-BOM-Report.RefDes")})
        Me.xrLabel5.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel5.LocationFloat = New DevExpress.Utils.PointFloat(52.08002!, 5.458322!)
        Me.xrLabel5.Name = "xrLabel5"
        Me.xrLabel5.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel5.SizeF = New System.Drawing.SizeF(199.92!, 56.0!)
        Me.xrLabel5.StylePriority.UseBorders = False
        Me.xrLabel5.StylePriority.UseFont = False
        Me.xrLabel5.Text = "xrLabel5"
        '
        'xrLabel4
        '
        Me.xrLabel4.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.xrLabel4.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "proj-BOM-Report.ID Number")})
        Me.xrLabel4.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel4.LocationFloat = New DevExpress.Utils.PointFloat(8.333339!, 52.53831!)
        Me.xrLabel4.Name = "xrLabel4"
        Me.xrLabel4.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel4.SizeF = New System.Drawing.SizeF(42.0!, 14.0!)
        Me.xrLabel4.StylePriority.UseBorders = False
        Me.xrLabel4.StylePriority.UseFont = False
        Me.xrLabel4.StylePriority.UseTextAlignment = False
        Me.xrLabel4.Text = "xrLabel4"
        Me.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'xrLabel3
        '
        Me.xrLabel3.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.xrLabel3.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "proj-BOM-Report.Item")})
        Me.xrLabel3.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel3.LocationFloat = New DevExpress.Utils.PointFloat(8.333339!, 17.45832!)
        Me.xrLabel3.Name = "xrLabel3"
        Me.xrLabel3.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel3.SizeF = New System.Drawing.SizeF(42.0!, 25.08!)
        Me.xrLabel3.StylePriority.UseBorders = False
        Me.xrLabel3.StylePriority.UseFont = False
        Me.xrLabel3.Text = "xrLabel3"
        '
        'xrLabel2
        '
        Me.xrLabel2.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.xrLabel2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "proj-BOM-Report.Description")})
        Me.xrLabel2.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel2.LocationFloat = New DevExpress.Utils.PointFloat(52.0866!, 75.45832!)
        Me.xrLabel2.Name = "xrLabel2"
        Me.xrLabel2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel2.SizeF = New System.Drawing.SizeF(199.92!, 14.0!)
        Me.xrLabel2.StylePriority.UseBorders = False
        Me.xrLabel2.StylePriority.UseFont = False
        Me.xrLabel2.Text = "xrLabel2"
        Me.xrLabel2.WordWrap = False
        '
        'xrLabel1
        '
        Me.xrLabel1.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.xrLabel1.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "proj-BOM-Report.Part Number")})
        Me.xrLabel1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel1.LocationFloat = New DevExpress.Utils.PointFloat(52.0866!, 61.45832!)
        Me.xrLabel1.Name = "xrLabel1"
        Me.xrLabel1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel1.SizeF = New System.Drawing.SizeF(199.92!, 14.0!)
        Me.xrLabel1.StylePriority.UseBorders = False
        Me.xrLabel1.StylePriority.UseFont = False
        Me.xrLabel1.Text = "xrLabel1"
        Me.xrLabel1.WordWrap = False
        '
        'topMarginBand1
        '
        Me.topMarginBand1.HeightF = 50.0!
        Me.topMarginBand1.Name = "topMarginBand1"
        '
        'bottomMarginBand1
        '
        Me.bottomMarginBand1.HeightF = 0.0!
        Me.bottomMarginBand1.Name = "bottomMarginBand1"
        '
        'rptBomBuildDatasource1
        '
        Me.rptBomBuildDatasource1.DataSetName = "rptBomBuildDatasource"
        Me.rptBomBuildDatasource1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'xrLabel7
        '
        Me.xrLabel7.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right), DevExpress.XtraPrinting.BorderSide)
        Me.xrLabel7.Font = New System.Drawing.Font("Arial Narrow", 6.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel7.LocationFloat = New DevExpress.Utils.PointFloat(8.333333!, 5.458323!)
        Me.xrLabel7.Name = "xrLabel7"
        Me.xrLabel7.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96.0!)
        Me.xrLabel7.SizeF = New System.Drawing.SizeF(42.0!, 12.0!)
        Me.xrLabel7.StylePriority.UseBorders = False
        Me.xrLabel7.StylePriority.UseFont = False
        Me.xrLabel7.Text = "Line#"
        '
        'xrLabel8
        '
        Me.xrLabel8.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.xrLabel8.Font = New System.Drawing.Font("Arial Narrow", 6.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel8.LocationFloat = New DevExpress.Utils.PointFloat(8.333339!, 42.53831!)
        Me.xrLabel8.Name = "xrLabel8"
        Me.xrLabel8.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel8.SizeF = New System.Drawing.SizeF(42.0!, 10.0!)
        Me.xrLabel8.StylePriority.UseBorders = False
        Me.xrLabel8.StylePriority.UseFont = False
        Me.xrLabel8.Text = "FPID#"
        '
        'xrLabel9
        '
        Me.xrLabel9.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.xrLabel9.Font = New System.Drawing.Font("Arial Narrow", 6.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xrLabel9.LocationFloat = New DevExpress.Utils.PointFloat(8.333339!, 66.53831!)
        Me.xrLabel9.Name = "xrLabel9"
        Me.xrLabel9.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLabel9.SizeF = New System.Drawing.SizeF(42.0!, 10.0!)
        Me.xrLabel9.StylePriority.UseBorders = False
        Me.xrLabel9.StylePriority.UseFont = False
        Me.xrLabel9.Text = "Qty / Unit"
        '
        'rptBomKitLabels
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.topMarginBand1, Me.bottomMarginBand1})
        Me.DataMember = "proj-BOM-Report"
        Me.DataSource = Me.rptBomBuildDatasource1
        Me.DetailPrintCount = 30
        Me.Font = New System.Drawing.Font("Arial Narrow", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margins = New System.Drawing.Printing.Margins(19, 0, 50, 0)
        Me.Version = "10.2"
        CType(Me.rptBomBuildDatasource1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand

#End Region

End Class