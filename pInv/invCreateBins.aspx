<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="invCreateBins.aspx.vb" Inherits="pInv_invCreateBins" title="Untitled Page" %>

<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxRoundPanel" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPanel" tagprefix="dx" %>

<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPopupControl" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <dx:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="200px" HeaderText="Add New Bin Box Locations to a Warehouse">
    <PanelCollection>
    <dx:PanelContent>
        <table>
        <tr>
            <td class="fpTableLabel"><div>Select Warehouse:</div></td>
            <td>
                <div style="width:500px">
                <dx:ASPxListBox ID="lbxWarehouse" runat="server" 
                    CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
                    DataSourceID="SqlDataSourceWarehouse" 
                    SpriteCssFilePath="~/App_Themes/Glass/{0}/sprite.css"
                    width="400px" ValueField="WarehouseID" ValueType="System.Int32">
                    <Columns>
                        <dx:ListBoxColumn FieldName="WarehouseID" Visible="False" />
                        <dx:ListBoxColumn FieldName="Abbrev" Width="100px" />
                        <dx:ListBoxColumn FieldName="Description" Width="400px" />
                    </Columns>
                    <LoadingPanelImage Url="~/App_Themes/Glass/Editors/Loading.gif">
                    </LoadingPanelImage>
                    <ValidationSettings>
                        <ErrorFrameStyle ImageSpacing="4px">
                            <ErrorTextPaddings PaddingLeft="4px" />
                        </ErrorFrameStyle>
                    </ValidationSettings>
                </dx:ASPxListBox>
                <asp:SqlDataSource ID="SqlDataSourceWarehouse" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
                    SelectCommand="SELECT [WarehouseID], [Abbrev], [Description] FROM [inv-Warehouses]"></asp:SqlDataSource>
                </div>
            </td>
        </tr>
        <tr>
            <td class="fpTableLabel"><div>Bin Box Name:</div></td>
            <td>
                <div style="width:250px">
                    <table>
                    <tr>
                    <td>
                        <dx:ASPxTextBox ID="xTxtBin1" runat="server" Width="20px" HorizontalAlign="Center"></dx:ASPxTextBox>
                    </td>
                    <td>
                        <dx:ASPxTextBox ID="xTxtBin2" runat="server" Width="40px" HorizontalAlign="Center"></dx:ASPxTextBox>
                    </td>
                    <td>
                        .
                    </td>
                    <td>
                        <dx:ASPxTextBox ID="xTxtBin3" runat="server" Width="20px" HorizontalAlign="Center"></dx:ASPxTextBox>
                    </td>
                    <td>
                        Through
                    </td>
                    <td>
                        <dx:ASPxTextBox ID="xTxtBin4" runat="server" Width="20px" HorizontalAlign="Center"></dx:ASPxTextBox>
                    </td>
                    </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
        <td></td>
        <td>
            <div>
                <ul>
                <li>Bins are organized by cabinet (letter), drawer (number), period, slot-in-drawer (number)</li>
                <li>To add multiple slots to a drawer fill in both boxes before and after 'Through'</li>
                <li>If a drawer is not subdivided leave both slot boxes blank</li>
                <li>If you are only adding one slot in a drawer, leave the second slot box blank (duh).</li>
                </ul>
            </div>
        </td>
        </tr>
        <tr>
        <td></td>
        <td>
            <dx:ASPxButton ID="xBtnAddBins" runat="server" Text="Add New Bin Locations">
            </dx:ASPxButton>
        </td>
        </tr>
        <tr>
        <td></td>
        <td>
            <div class="fpErrorBox" runat="server" id="divErrorBox" visible="false">
                <asp:Label ID="lblError" runat="server" Text="" Visible="true"></asp:Label>
            </div>
        </td>
        </tr>
        </table>
        <dx:ASPxPopupControl ID="xPopupReplace" runat="server" Modal="True" 
            CloseAction="None" CssFilePath="~/App_Themes/Glass/{0}/styles.css" 
            CssPostfix="Glass" PopupHorizontalAlign="WindowCenter" 
            PopupVerticalAlign="WindowCenter" 
            SpriteCssFilePath="~/App_Themes/Glass/{0}/sprite.css" 
            ShowCloseButton="False" Height="200px" Width="300px" 
            HeaderText="BIN BOX IS ALREADY IN USE">
            <HeaderStyle>
            <Paddings PaddingLeft="10px" PaddingRight="6px" PaddingTop="1px" />
            </HeaderStyle>
            <contentcollection>
                <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
                    <asp:Label ID="lblPopHeader" runat="server" Text="Label"></asp:Label>
                    <table>
                    <tr>
                        <td class="fpTableLabel"><div>Warehouse:</div></td>
                        <td><asp:Label ID="lblPopWarehouse" runat="server" Text="Label"></asp:Label></td>
                    </tr>
                    <tr>
                        <td class="fpTableLabel"><div>Bin Location:</div></td>
                        <td><asp:Label ID="lblPopBin" runat="server" Text="Label"></asp:Label></td>
                    </tr>
                    <tr>
                        <td class="fpTableLabel"><div>Currently Contains:</div></td>
                        <td><asp:Label ID="lblPopCurrentContents" runat="server" Text="Label"></asp:Label></td>
                    </tr>
                    </table>
                    <br />
                    <hr />
                    Empty out this Bin Box Location and mark it as available for a new part?
                    <table align="right" style="padding-top:4px">
                    <tr><td>
                        <asp:Button ID="btnPopYes" runat="server" Text="YES" />
                        <asp:Button ID="btnPopCancel" runat="server" Text="NO (CANCEL)" />
                    </td></tr>
                    </table>
                </dx:PopupControlContentControl>
            </contentcollection>
        </dx:ASPxPopupControl>
    </dx:PanelContent>
    </PanelCollection>
    </dx:ASPxRoundPanel>
</asp:Content>

