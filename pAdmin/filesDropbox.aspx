<%@ Page Title="" Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="filesDropbox.aspx.vb" Inherits="pAdmin_filesDropbox" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>
<%@ Register assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxTabControl" TagPrefix="dxtc" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxClasses" tagprefix="dxw" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<!-- RECENT EVENTS -->
    <!-- TAB PAGES -->     
<div>
    <dxtc:ASPxPageControl ID="xTabPages" ClientInstanceName="xTabPages" 
        runat="server" ActiveTabIndex="0" 
        Height="200px" Width="700px" 
        CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
        TabSpacing="0px" EnableCallBacks="True">
        <LoadingPanelImage Url="~/App_Themes/Glass/Web/Loading.gif">
        </LoadingPanelImage>
        <ContentStyle>
            <Border BorderColor="#4986A2" />
        </ContentStyle>

        <TabPages>
                
            <dxtc:TabPage Text="Event Log"><ContentCollection><dxw:ContentControl id="ContentControl2" runat="server">                    
                <asp:Panel ID="Panel5" runat="server" style="color:black;">
                    <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="600px" HeaderText="ACCOUNT STATUS">
                        <PanelCollection>
                            <dx:PanelContent>
                                <div>
                                    <table>
                                        <tr>
                                            <td class="fpTableLabel">Dropbox Account:</td>
                                            <td>
                                                <asp:Label ID="lblAccountHolder" runat="server" Text="Label"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="fpTableLabel">Current Status:</td>
                                            <td>
                                                <asp:Label ID="lblStatus" runat="server" Text="Label"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dxrp:ASPxRoundPanel>

                    <br />

                    <dxrp:ASPxRoundPanel ID="xrpDropboxEvents" runat="server" Width="700px" HeaderText="RECENT EVENTS">
                    <PanelCollection>
                    <dx:PanelContent>
                        <div style="margin-bottom:10px; width:500px;">
                            This table shows only the activity logged by the Update-Service
                            Dropbox Worker (major events). 
                            Files may be currently in transfer. Refresh frequently to see 
                            updates.
                        </div>
                        <dxwgv:ASPxGridView ID="xGridUserLog" runat="server" Font-Names="Tahoma" 
                            Font-Size="X-Small"
                            AutoGenerateColumns="False" DataSourceID="SqlDataSource1">
                            <Columns>
                                <dxwgv:GridViewCommandColumn ShowInCustomizationForm="True" VisibleIndex="0">
                                    <ClearFilterButton Visible="True">
                                    </ClearFilterButton>
                                </dxwgv:GridViewCommandColumn>
                                <dxwgv:GridViewDataDateColumn FieldName="TimestampDate" 
                                    ShowInCustomizationForm="True" VisibleIndex="0">
                                </dxwgv:GridViewDataDateColumn>
                                <dxwgv:GridViewDataTextColumn FieldName="UserName" ShowInCustomizationForm="True" 
                                    VisibleIndex="1">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn ShowInCustomizationForm="True" 
                                    VisibleIndex="2" FieldName="OpDesc">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn ShowInCustomizationForm="True" 
                                    VisibleIndex="3" FieldName="Filename">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn ShowInCustomizationForm="True" 
                                    VisibleIndex="4" FieldName="Message">
                                </dxwgv:GridViewDataTextColumn>
                            </Columns>
                            <Settings ShowFilterRow="True" />
                        </dxwgv:ASPxGridView>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
                            
                            SelectCommand="SELECT [TimestampDate], [UserName], [OpDesc], [Filename], [Message] FROM [view-logDropbox] ORDER BY [TimestampDate] DESC">
                        </asp:SqlDataSource>
                    </dx:PanelContent>
                    </PanelCollection>
                    </dxrp:ASPxRoundPanel>
                </asp:Panel>
            </dxw:ContentControl></ContentCollection></dxtc:TabPage>
            
            <dxtc:TabPage Text="User Dropbox Contents"><ContentCollection><dxw:ContentControl ID="ContentControl1" runat="server">                    
                <asp:Panel ID="Panel1" runat="server" style="color:black;">

                    <dxrp:ASPxRoundPanel ID="ASPxRoundPanel3" runat="server" Width="600px" HeaderText="Select User">
                        <PanelCollection>
                            <dx:PanelContent ID="PanelContent2" runat="server">
                                <dxwgv:ASPxGridView ID="xGridSelectUser" runat="server" 
                                    AutoGenerateColumns="False" DataSourceID="SqlDataSourceUsers"
                                    EnableCallBacks="false" KeyFieldName="UserID">

                                    <Columns>
                                        <dxwgv:GridViewDataTextColumn FieldName="UserID" ReadOnly="True" 
                                            ShowInCustomizationForm="True" VisibleIndex="0" Width="25px">
                                            <EditFormSettings Visible="False" />
                                        </dxwgv:GridViewDataTextColumn>
                                        <dxwgv:GridViewDataTextColumn FieldName="UserName" 
                                            ShowInCustomizationForm="True" VisibleIndex="1">
                                        </dxwgv:GridViewDataTextColumn>
                                        <dxwgv:GridViewDataTextColumn FieldName="UserEmail" 
                                            ShowInCustomizationForm="True" VisibleIndex="2">
                                        </dxwgv:GridViewDataTextColumn>
                                        <dxwgv:GridViewDataDateColumn FieldName="DateCreated" 
                                            ShowInCustomizationForm="True" VisibleIndex="3">
                                        </dxwgv:GridViewDataDateColumn>
                                    </Columns>

                                    <SettingsBehavior 
                                        AllowFocusedRow="True" 
                                        ProcessFocusedRowChangedOnServer="True">
                                    </SettingsBehavior>

                                    <Settings ShowFilterRow="True" />

                                </dxwgv:ASPxGridView>
                                <asp:SqlDataSource ID="SqlDataSourceUsers" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
                                    
                                    SelectCommand="SELECT [UserID], [UserName], [UserEmail], [DateCreated] FROM [user-Accounts] WHERE ([DropboxAppKey] IS NOT NULL) ORDER BY [UserName]"></asp:SqlDataSource>

                                <table>
                                    <tr>
                                        <td class="fpTableLabel">Reset Account:</td>
                                        <td>
                                            <asp:Button ID="btnDeleteAllFiles" runat="server" Text="DELETE ALL FILES?!" />
                                        </td>
                                    </tr>
                                </table>

                            </dx:PanelContent>    
                        </PanelCollection>
                    </dxrp:ASPxRoundPanel>
                    <br />
                    <dxrp:ASPxRoundPanel ID="ASPxRoundPanel2" runat="server" Width="600px" HeaderText="Dropbox Contents">
                        <PanelCollection>
                            <dx:PanelContent ID="PanelContent1" runat="server">
                                <dxwgv:ASPxGridView ID="xGridDropboxContents" runat="server" Font-Names="Tahoma" 
                                    Font-Size="X-Small"
                                    AutoGenerateColumns="False">
                                    <Columns>
                                        <dxwgv:GridViewDataColumn FieldName="UID" 
                                            ShowInCustomizationForm="True" VisibleIndex="0" ReadOnly="true" Visible="false">
                                            <EditFormSettings Visible="False" />
                                        </dxwgv:GridViewDataColumn>
                                        <dxwgv:GridViewDataTextColumn FieldName="Path" ShowInCustomizationForm="True" 
                                            VisibleIndex="1">
                                        </dxwgv:GridViewDataTextColumn>
                                        <dxwgv:GridViewDataTextColumn FieldName="Filename" ShowInCustomizationForm="True" 
                                            VisibleIndex="1">
                                        </dxwgv:GridViewDataTextColumn>
                                        <dxwgv:GridViewDataTextColumn FieldName="Notes" ShowInCustomizationForm="True" 
                                            VisibleIndex="1">
                                        </dxwgv:GridViewDataTextColumn>
                                    </Columns>
                                    <SettingsPager PageSize="50">
                                    </SettingsPager>
                                    <Settings ShowFilterRow="True" />
                                </dxwgv:ASPxGridView>
                            </dx:PanelContent>    
                        </PanelCollection>
                    </dxrp:ASPxRoundPanel>




                    
                </asp:Panel>
            </dxw:ContentControl></ContentCollection></dxtc:TabPage>

            <dxtc:TabPage Text="Server Dropbox Contents"><ContentCollection><dxw:ContentControl ID="ContentControl3" runat="server">                    
                <asp:Panel ID="Panel2" runat="server" style="color:black;">
                    <dxrp:ASPxRoundPanel ID="ASPxRoundPanel4" runat="server" Width="600px" HeaderText="Server-Local Files">
                        <PanelCollection>
                            <dx:PanelContent ID="PanelContent3" runat="server">
                                <dxwgv:ASPxGridView ID="xGridServer" runat="server" Font-Names="Tahoma" 
                                    Font-Size="X-Small"
                                    AutoGenerateColumns="False">
                                    <Columns>
                                        <dxwgv:GridViewDataColumn FieldName="UID" 
                                            ShowInCustomizationForm="True" VisibleIndex="0" ReadOnly="true" Visible="false">
                                            <EditFormSettings Visible="False" />
                                        </dxwgv:GridViewDataColumn>
                                        <dxwgv:GridViewDataTextColumn FieldName="Path" ShowInCustomizationForm="True" 
                                            VisibleIndex="1">
                                        </dxwgv:GridViewDataTextColumn>
                                        <dxwgv:GridViewDataTextColumn FieldName="Filename" ShowInCustomizationForm="True" 
                                            VisibleIndex="1">
                                        </dxwgv:GridViewDataTextColumn>
                                        <dxwgv:GridViewDataTextColumn FieldName="Notes" ShowInCustomizationForm="True" 
                                            VisibleIndex="1">
                                        </dxwgv:GridViewDataTextColumn>
                                    </Columns>
                                    <SettingsPager PageSize="50">
                                    </SettingsPager>
                                    <Settings ShowFilterRow="True" />
                                </dxwgv:ASPxGridView>
                            </dx:PanelContent>    
                        </PanelCollection>
                    </dxrp:ASPxRoundPanel>
                </asp:Panel>
            </dxw:ContentControl></ContentCollection></dxtc:TabPage>

        </TabPages>
    </dxtc:ASPxPageControl>            
</div>   
</asp:Content>

