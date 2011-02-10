<%@ Page Title="" Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="bomViewProject.aspx.vb" Inherits="pBOM_bomViewProject" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxUploadControl" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxPopupControl" TagPrefix="dxpc" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>
<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxTabControl" tagprefix="dxtc" %>
<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxClasses" tagprefix="dxw" %>
<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxClasses" tagprefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dxp" %>
<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxLoadingPanel" TagPrefix="dxlp" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxCallback" TagPrefix="dxcb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
        <!-- Datasource -->
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
            ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
            SelectCommand="SELECT [ProjectID], [Title], [Revision], [Description] FROM [proj-Common] ORDER BY [Title], [DateCreated] DESC">
        </asp:SqlDataSource>
        
        <!-- Project Grid -->
        <dx:ASPxGridView ID="xGV" runat="server" AutoGenerateColumns="False" 
            CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
            DataSourceID="SqlDataSource1" KeyFieldName="ProjectID" SettingsBehavior-AllowFocusedRow="True">
            <Images SpriteCssFilePath="~/App_Themes/Glass/{0}/sprite.css">
                <LoadingPanelOnStatusBar Url="~/App_Themes/Glass/GridView/gvLoadingOnStatusBar.gif">
                </LoadingPanelOnStatusBar>
                <LoadingPanel Url="~/App_Themes/Glass/GridView/Loading.gif">
                </LoadingPanel>
            </Images>
            <Columns>
                <dx:GridViewDataTextColumn FieldName="ProjectID" ReadOnly="True" 
                    VisibleIndex="0" Width="50px">
                    <EditFormSettings Visible="False" />
                    <CellStyle HorizontalAlign="Center">
                    </CellStyle>
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="Title" VisibleIndex="1" Width="150px">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="Revision" VisibleIndex="2" Width="30px">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="Description" VisibleIndex="3" 
                    Width="300px">
                </dx:GridViewDataTextColumn>
            </Columns>
            <Settings ShowFilterRow="True" />
            <ImagesFilterControl>
                <LoadingPanel Url="~/App_Themes/Glass/Editors/Loading.gif">
                </LoadingPanel>
            </ImagesFilterControl>
            <Styles CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass">
                <Header ImageSpacing="5px" SortingImageSpacing="5px">
                </Header>
            </Styles>
            <StylesEditors>
                <CalendarHeader Spacing="1px">
                </CalendarHeader>
                <ProgressBar Height="25px">
                </ProgressBar>
            </StylesEditors>
            <ClientSideEvents RowClick="function(s, e) { OnGridRowClicked(e.visibleIndex); }"/>
        </dx:ASPxGridView>
    </div> <!-- Contains All Content -->
    
    <!-- POPUP MADNESS!!! -->
    <dxpc:ASPxPopupControl ID="xPopup" runat="server" ContentUrl="~/pInv/invShowDetail.aspx" 
        AllowDragging="True" EnableAnimation="true" 
        ShowShadow="true" HeaderText="Project Details" 
        EnableClientSideAPI="True" EnableViewState="False" PopupAction="None" 
        BackColor="#FFFF99" Left="5" Top="40" Height="800px" Width="625px">
    </dxpc:ASPxPopupControl>    

    <script type="text/javascript">
        // <!CDATA[
        //function is called on changing focused row
        function OnGridRowClicked(visIdx) {
            if (window.parent.document.readyState == 'complete') {
                    ctl00_ContentPlaceHolder1_xGV.GetRowValues(visIdx, 'ProjectID;', OnGetRowValues);
                    ctl00_ContentPlaceHolder1_xPopup.RefreshContentUrl();
                    ctl00_ContentPlaceHolder1_xPopup.Show();
            }
        }
        //Value array contains "EmployeeID" and "Notes" field values returned from the server 
        function OnGetRowValues(values) {
            ctl00_ContentPlaceHolder1_xPopup.SetContentUrl("bomViewProjectDetail.aspx?detail=" + values[0]);        
        }
    </script>    
</asp:Content>

