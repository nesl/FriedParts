<%@ Page Title="" Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="bomReport.aspx.vb" Inherits="pBOM_bomReport" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxTabControl" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.XtraReports.v10.2.Web, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.XtraReports.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dxp" %>
<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxTabControl" TagPrefix="dxtc" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxClasses" tagprefix="dxw" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div style="height:1120px;">
    <!-- Project Data Source -->
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
            ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
            SelectCommand="SELECT [ProjectID], [Title], [Revision], [Description] FROM [proj-Common] ORDER BY [Title], [DateCreated] DESC">
    </asp:SqlDataSource>
    
    <!-- TAB PAGES -->
    <div id="TabPages" runat="server" style="float:left">
        <dx:ASPxPageControl ID="xTabPages" ClientInstanceName="xTabPages" 
            runat="server" ActiveTabIndex="1" Width="1024px"
            CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
            TabSpacing="0px" EnableCallBacks="True">
        <LoadingPanelImage Url="~/App_Themes/Glass/Web/Loading.gif">
        </LoadingPanelImage>
        <ContentStyle>
            <Border BorderColor="#4986A2" />
        </ContentStyle>
        <TabPages>
            <dxtc:TabPage Text="[STEP 1.] Select Project"><ContentCollection><dxw:ContentControl ID="ContentControl3" runat="server">                    
                <asp:Panel ID="Panel3" runat="server" style="color:black;">
                    <table>
                        <tr>
                            <td class="fpTableLabelMultiline"><div>Select:</div></td>
                            <td>
                                <dx:ASPxGridView ID="xGridProjects" runat="server" AutoGenerateColumns="False" 
                                    CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
                                    DataSourceID="SqlDataSource1" KeyFieldName="ProjectID" 
                                    EnableCallBacks="False">
                                    <Images SpriteCssFilePath="~/App_Themes/Glass/{0}/sprite.css">
                                        <LoadingPanelOnStatusBar Url="~/App_Themes/Glass/GridView/gvLoadingOnStatusBar.gif">
                                        </LoadingPanelOnStatusBar>
                                        <LoadingPanel Url="~/App_Themes/Glass/GridView/Loading.gif">
                                        </LoadingPanel>
                                    </Images>
                                    <ClientSideEvents FocusedRowChanged="function(s, e) {viewer.Refresh();}" />
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
                                    <SettingsBehavior AllowFocusedRow="true" ProcessFocusedRowChangedOnServer="true" />
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
                                </dx:ASPxGridView>
                            </td>
                        </tr>
                    </table>    
                </asp:Panel></dxw:ContentControl></ContentCollection>
            </dxtc:TabPage>
            
            
            
            <dxtc:TabPage Text="[STEP 2.] Bill of Materials"><ContentCollection><dxw:ContentControl ID="ContentControl1" runat="server">                    
                <asp:Panel ID="Panel5" runat="server" style="color:black;">
                <div style="float:left; border-right: solid 1px lightgrey;">
                    <div style="background-image:url('../Images/Projects/notebook-Top.png');width:680px;height:41px;">
                        <div style="margin-left: 10px; border-top: solid 1px lightgrey"></div>
                            <div style="margin-left:50px; padding-top:8px;">
                                <dx:ReportToolbar ID="ReportToolbar1" ClientInstanceName="viewer" runat='server' ShowDefaultButtons='False' ReportViewer="<%# ReportViewer1 %>">
                                    <Items>
                                        <dx:ReportToolbarButton ItemKind='Search' />
                                        <dx:ReportToolbarSeparator />
                                        <dx:ReportToolbarButton ItemKind='PrintReport' />
                                        <dx:ReportToolbarButton ItemKind='PrintPage' />
                                        <dx:ReportToolbarSeparator />
                                        <dx:ReportToolbarButton Enabled='False' ItemKind='FirstPage' />
                                        <dx:ReportToolbarButton Enabled='False' ItemKind='PreviousPage' />
                                        <dx:ReportToolbarLabel ItemKind='PageLabel' />
                                        <dx:ReportToolbarComboBox ItemKind='PageNumber' Width='65px'>
                                        </dx:ReportToolbarComboBox>
                                        <dx:ReportToolbarLabel ItemKind='OfLabel' />
                                        <dx:ReportToolbarTextBox IsReadOnly='True' ItemKind='PageCount' />
                                        <dx:ReportToolbarButton ItemKind='NextPage' />
                                        <dx:ReportToolbarButton ItemKind='LastPage' />
                                        <dx:ReportToolbarSeparator />
                                        <dx:ReportToolbarButton ItemKind='SaveToDisk' />
                                        <dx:ReportToolbarButton ItemKind='SaveToWindow' />
                                        <dx:ReportToolbarComboBox ItemKind='SaveFormat' Width='70px'>
                                            <Elements>
                                                <dx:ListElement Value='pdf' />
                                                <dx:ListElement Value='xls' />
                                                <dx:ListElement Value='xlsx' />
                                                <dx:ListElement Value='rtf' />
                                                <dx:ListElement Value='mht' />
                                                <dx:ListElement Value='txt' />
                                                <dx:ListElement Value='csv' />
                                                <dx:ListElement Value='png' />
                                            </Elements>
                                        </dx:ReportToolbarComboBox>
                                    </Items>
                                    <Styles>
                                        <LabelStyle>
                                            <Margins MarginLeft='3px' MarginRight='3px' />
                                        </LabelStyle>
                                    </Styles>
                                </dx:ReportToolbar>
                            </div>
                        </div>
                        <div style="background-image:url('../Images/Projects/notebook-Content.png'); background-repeat:repeat-y; min-height:888px;">
                            <div style="margin-left:35px;">
                                <dx:ReportViewer ID="ReportViewer1" runat="server" Report="<%# New rptBomBuild() %>" ReportName="rptBomBuild"></dx:ReportViewer>
                            </div>
                        </div>
                        <div style="background-image:url('../Images/Projects/notebook-Bottom.png');width:680px;height:34px;"></div>
                    </div>
                    <div style="float:left;"></div><!-- Needed to allow the content div to shrink since the tabpages control actually emits <table> code so all content areas are fixed (max) width -->
                </asp:Panel>
                </dxw:ContentControl></ContentCollection>
            </dxtc:TabPage>
            
            
            
            <dxtc:TabPage Text="[STEP 3.] Parts Kit Labels"><ContentCollection><dxw:ContentControl ID="ContentControl2" runat="server">                    
                <asp:Panel ID="Panel2" runat="server" style="color:black;">
                    <div style="margin-bottom:10px;">
                    <dx:ReportToolbar ID="ReportToolbar2" runat='server' ShowDefaultButtons='False' 
                        ReportViewer="<%# ReportViewer2 %>">
                        <Items>
                            <dx:ReportToolbarButton ItemKind='Search' />
                            <dx:ReportToolbarSeparator />
                            <dx:ReportToolbarButton ItemKind='PrintReport' />
                            <dx:ReportToolbarButton ItemKind='PrintPage' />
                            <dx:ReportToolbarSeparator />
                            <dx:ReportToolbarButton Enabled='False' ItemKind='FirstPage' />
                            <dx:ReportToolbarButton Enabled='False' ItemKind='PreviousPage' />
                            <dx:ReportToolbarLabel ItemKind='PageLabel' />
                            <dx:ReportToolbarComboBox ItemKind='PageNumber' Width='65px'>
                            </dx:ReportToolbarComboBox>
                            <dx:ReportToolbarLabel ItemKind='OfLabel' />
                            <dx:ReportToolbarTextBox IsReadOnly='True' ItemKind='PageCount' />
                            <dx:ReportToolbarButton ItemKind='NextPage' />
                            <dx:ReportToolbarButton ItemKind='LastPage' />
                            <dx:ReportToolbarSeparator />
                            <dx:ReportToolbarButton ItemKind='SaveToDisk' />
                            <dx:ReportToolbarButton ItemKind='SaveToWindow' />
                            <dx:ReportToolbarComboBox ItemKind='SaveFormat' Width='70px'>
                                <Elements>
                                    <dx:ListElement Value='pdf' />
                                    <dx:ListElement Value='xls' />
                                    <dx:ListElement Value='xlsx' />
                                    <dx:ListElement Value='rtf' />
                                    <dx:ListElement Value='mht' />
                                    <dx:ListElement Value='html' />
                                    <dx:ListElement Value='txt' />
                                    <dx:ListElement Value='csv' />
                                    <dx:ListElement Value='png' />
                                </Elements>
                            </dx:ReportToolbarComboBox>
                        </Items>
                        <Styles>
                            <LabelStyle>
                                <Margins MarginLeft='3px' MarginRight='3px' />
                            </LabelStyle>
                        </Styles>
                    </dx:ReportToolbar>
                    </div>
                    <div style="float:left; border:solid 1px lightgrey;">
                    <dx:ReportViewer ID="ReportViewer2" runat="server" 
                        Report="<%# New rptBomKitLabels() %>" ReportName="rptBomKitLabels">
                    </dx:ReportViewer>
                    </div>
                    <div style="float:left;"></div><!-- Needed to allow the content div to shrink since the tabpages control actually emits <table> code so all content areas are fixed (max) width -->
                </asp:Panel>
                </dxw:ContentControl></ContentCollection>
            </dxtc:TabPage>
        </TabPages>
        </dx:ASPxPageControl>      
    </div> <!-- This div encloses all of the tabpages -->
</div> <!-- This div encloses the entire content area (used to force page height) -->    
    
</asp:Content>

