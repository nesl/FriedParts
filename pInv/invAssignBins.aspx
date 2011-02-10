<%@ Page Language="VB" AutoEventWireup="false" CodeFile="invAssignBins.aspx.vb" Inherits="pInv_invAssignBins" title="Untitled Page" %>

<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dx" %>

<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPanel" tagprefix="dx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Inventory Assign Bins</title>
</head>
<body>
<form id="form1" runat="server">
<div style="position:absolute; top:40px">
    <dx:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="200px">
    <PanelCollection>
    <dx:PanelContent runat="server">
    <asp:HiddenField ID="hidPartID" runat="server" />
    <table>
        <tr>
            <td class="fpTableLabel"><div>Select Warehouse:</div></td>
            <td>
                <div style="width:500px">
                    <dx:ASPxListBox ID="lbxWarehouse" runat="server" 
                        CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
                        DataSourceID="SqlDataSourceWarehouse" 
                        SpriteCssFilePath="~/App_Themes/Glass/{0}/sprite.css"
                        width="400px" ValueField="WarehouseID" ValueType="System.Int32" 
                        AutoPostBack="True">
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
        <tr id="BinRow" runat="server" visible="false">
            <td class="fpTableLabel"><div>Available Bins at this Location:</div></td>
            <td>
                <div>
                    <dx:ASPxListBox ID="lbxBins" runat="server" 
                        CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
                        DataSourceID="SqlDataSourceBins" 
                        SpriteCssFilePath="~/App_Themes/Glass/{0}/sprite.css"
                        width="400px" ValueField="BinName" AutoPostBack="True">
                        <Columns>
                            <dx:ListBoxColumn FieldName="BinName" Width="50px" />
                            <dx:ListBoxColumn FieldName="WarehouseID" Visible="False" />
                        </Columns>
                        <LoadingPanelImage Url="~/App_Themes/Glass/Editors/Loading.gif">
                        </LoadingPanelImage>
                        <ValidationSettings>
                            <ErrorFrameStyle ImageSpacing="4px">
                                <ErrorTextPaddings PaddingLeft="4px" />
                            </ErrorFrameStyle>
                        </ValidationSettings>
                    </dx:ASPxListBox>
                    <asp:SqlDataSource ID="SqlDataSourceBins" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
                        
                        SelectCommand="SELECT [BinName], [WarehouseID] FROM [inv-WarehouseBins] WHERE ([WarehouseID] = @WarehouseID) ORDER BY [BinName]">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="lbxWarehouse" Name="WarehouseID" 
                                PropertyName="Value" Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                </div>
            </td>
        </tr>
        <tr id="QtyRow" runat="server" visible="false">
            <td class="fpTableLabel"><div>Quantity On Hand:</div></td>
            <td>
                <asp:TextBox ID="txtQty" runat="server" AutoPostBack="True"></asp:TextBox>
            </td>
        </tr>
        <tr id="SubmitRow" runat="server" visible="false">
            <td class="fpTableLabel"><div>Submit:</div></td>
            <td>
                <asp:Button ID="btnAssignBin" runat="server" Text="Assign Part to Bin" />
            </td>
        </tr>
    </table>
    </dx:PanelContent>
    </PanelCollection>
    </dx:ASPxRoundPanel>
</div>
</form>
</body>
</html>

