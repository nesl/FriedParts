<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="devCrawler.aspx.vb" Inherits="pDevel_devCrawler" title="Untitled Page" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxHiddenField" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxDataView" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxCallback" TagPrefix="dxcb" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxLoadingPanel" TagPrefix="dxlp" %>

<%@ Register Assembly="DevExpress.Web.ASPxTreeList.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxTreeList" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>

<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dxe" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxTabControl" TagPrefix="dxtc" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxClasses" tagprefix="dxw" %>

<%@ Register assembly="System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="System.Web.UI" tagprefix="cc1" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div id="divOcto" style="float:left; width:600px">
        <asp:Panel ID="Panel4" runat="server" style="color:black;">            
            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel8" runat="server" Width="580px" HeaderText="Suggestive Search">
                <PanelCollection>
                    <dx:PanelContent ID="PanelContent9" runat="server">
                        <!-- Search Suggest Radio Buttons (SSRB) -->
                          Search mpn: <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox><asp:Button ID="FuzzySearch" runat="server" Text="Search" />
                        <p>
                            Chips Available: 
                            <asp:Label ID="ResultofTextBox1" runat="server" ForeColor="Black"></asp:Label>
                        </p>
                     </dx:PanelContent>
                </PanelCollection>
            </dxrp:ASPxRoundPanel>
            <br />
            
            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel9" runat="server" Width="580px" HeaderText="Confirm WebCrawl Data">
                <PanelCollection>
                    <dx:PanelContent ID="PanelContent10" runat="server">
                             Choose MPN: <asp:DropDownList ID="mpnDropDown" runat="server"> </asp:DropDownList>
                        <p>
                            MANUFACTURER: <asp:Label ID="manuText" runat="server"></asp:Label>
                            <br /><br />
                            MPN: <asp:Label ID="mpnText" runat="server"></asp:Label>
                            <br/><br />
                            DESCRIPTIONS: <asp:Label ID="descriptionsText" runat="server"></asp:Label>
                            <br /><br />
                            AVERAGE AVAILABLE: <asp:Label ID="avg_avail" runat="server"></asp:Label>
                            <br /><br />
                            NUMBER OF SUPPLIERS: <asp:Label ID="num_suppliers" runat="server"></asp:Label>
                            <br/><br />
                            SPECS METADATA: <asp:Label ID="specsMetaText" runat="server" ></asp:Label>
                            <br /><br />
                            SPECS: <asp:Label ID="specsText" runat="server"></asp:Label>
                            <br /><br />
                            DATASHEETS: <asp:Label ID="datasheetsText" runat="server"></asp:Label>
                            <br /><br />
                            IMAGES: <asp:Label ID="imagesText" runat="server"></asp:Label>
                            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel2" runat="server" Width="540px" HeaderText="Image List" ClientVisible="True">
                                <PanelCollection>
                                    <dx:PanelContent ID="PanelContent11" runat="server">
                                                                                
                                        <dxe:ASPxImage ID="ASPxSelectedImage" Height="100px" Width="100px" ImageURL="null" runat="server" ClientVisible="False">
                                        </dxe:ASPxImage>
                                        
                                        <dxe:ASPxButton ID="ASPxReselectImage" runat="server" Text="Select Another Image" oncommand="ReselectBtn_Click" ClientVisible="false">
                                        </dxe:ASPxButton>
                                        <dx:ASPxDataView ID="ImageDataView" runat="server" CollumnPerPage="3" allowpaging="false">
                                            <ItemTemplate>
                                                <dxe:ASPxImage ID="ASPxImage1" runat="server" Height="100px" Width="100px" ImageUrl='<%# Eval("ImageURL") %>'>
                                                </dxe:ASPxImage>
                                               <dxe:ASPxButton ID="ASPxImageButton" Image-Height = "100px" Image-Width= "100px" runat="server" OnCommand="ImageBtn_Click" CommandName='<%# Eval("ImageURL") %>' Text="Select Image">
                                               </dxe:ASPxButton>
                                            </ItemTemplate>
                                        </dx:ASPxDataView>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dxrp:ASPxRoundPanel>
                            <br /><br />
                            OFFERS METADATA: <asp:Label ID="offersMetaText" runat="server"></asp:Label>
                            <br /><br />
                            OFFERS: <asp:Label ID="offersText" runat="server"></asp:Label>
                            <br /><br />
                            DEBUG: <asp:Label ID="debugText" runat="server"></asp:Label>
                            <br /><br />
                       </p>
                    </dx:PanelContent>
                </PanelCollection>
            </dxrp:ASPxRoundPanel>                        
        </asp:Panel>
    </div>
    <div id="divDigikey" style="float:left; width:600px">            
                <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="200px" HeaderText="Digikey Webcrawler!">
                    <PanelCollection>
                    <dx:PanelContent>
                        <asp:TextBox ID="textboxDK" runat="server"></asp:TextBox>
                        <asp:Button ID="Button1" runat="server" Text="Button" />
                        <hr />
                        PAGE TEXT
                        <dxwgv:ASPxGridView ID="xGridWeb" runat="server" SettingsPager-PageSize="50">
                            <SettingsPager PageSize="50"></SettingsPager>
                        </dxwgv:ASPxGridView>
                        <br /><br />
                        PAGE LINKS
                        <dxwgv:ASPxGridView ID="xGridWebLinks" runat="server" SettingsPager-PageSize="50">
                            <SettingsPager PageSize="50"></SettingsPager>
                        </dxwgv:ASPxGridView>
                    </dx:PanelContent>
                    </PanelCollection>
                </dxrp:ASPxRoundPanel>
                
    </div>              
</asp:Content>

