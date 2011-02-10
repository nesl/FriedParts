<%@ Page Language="VB" AutoEventWireup="false" CodeFile="invShowDetail.aspx.vb" Inherits="pInv_invShowDetail" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>

<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dxe" %>

<%@ Register assembly="DevExpress.XtraCharts.v10.2.Web, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.XtraCharts" tagprefix="dxchartsui" %>
<%@ Register assembly="DevExpress.XtraCharts.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.XtraCharts" tagprefix="cc1" %>
<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxRoundPanel" tagprefix="dxrp" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Part Details Page</title>
    
</head>
<body>
    <form id="form1" runat="server" style="font-family: Tahoma, Arial, Helvetica, sans-serif; font-size: 9pt; font-weight: normal; font-style: normal; font-variant: normal; color: #000000">
    
    <!-- Top Left Column -->
    <div id="TopBoxLeftCol" runat="server" style="position:absolute; top:0; left:0; width:350px; height:350px">        
        <asp:SqlDataSource ID="fpSQLData" runat="server"></asp:SqlDataSource>
        <div id="TitleMfr" runat="server" style="font-size:11pt; font-weight:bold;"></div>
        <div id="TitlePart" runat="server" style="font-size:14pt; font-weight:bold;"></div>
        <br />
        
        <b>Description:</b><br />
        <asp:TextBox ID="TextBoxDesc" runat="server" Height="30px" Width="325px" 
            BorderColor="#00CC66" BorderStyle="Solid" BorderWidth="1px" 
            TextMode="MultiLine" Rows="2">Blah!</asp:TextBox>
        <br />
        <b>Extended Description:</b><br />
        <asp:TextBox ID="TextBoxXDesc" runat="server" Height="125px" Width="325px" 
            BorderColor="#00CC66" BorderStyle="Solid" BorderWidth="1px"
            TextMode="MultiLine">Blah!</asp:TextBox>
        
        <br /><br />
        <b>Local Inventory:</b><br />
        <div style="padding:3px">
            [<asp:HyperLink ID="linkAssignBins" runat="server">Assign a Bin Location to this Part</asp:HyperLink>]
            [<a href="">Update Quantity</a>]
            <br />
        </div>
        <dxwgv:ASPxGridView ID="xGVLocal" runat="server">
            <SettingsBehavior AllowDragDrop="False" AllowFocusedRow="True" 
                AllowGroup="False" />
            <Columns>
            <dxwgv:GridViewDataTextColumn Caption="Warehouse" FieldName="Abbrev" 
                    ReadOnly="True" VisibleIndex="1" Width="100px">
            </dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataTextColumn Caption="Bin Location" FieldName="BinLocation" ReadOnly="True" VisibleIndex="2" Width="100px">
            </dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataColumn Caption="Qty Available" FieldName="QtyHere" ReadOnly="True" VisibleIndex="3" Width="100px">
            </dxwgv:GridViewDataColumn>
            </Columns>
        </dxwgv:ASPxGridView>
        
    </div> <!-- Top Left Column -->
    
    
    <!-- Top Right Column -->
    <div id="TopBoxRightCol" runat="server" style="position:absolute; top:0; right:0; width:225px; height:350px">
        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="225px" 
            HeaderText="Part Photo">
        <PanelCollection>
            <dx:PanelContent runat="server">
            <div style="overlap:hidden; width:200px; height:200px"> <!-- This div is a fixed size to trap large images before we get a chance to resize them -->
                <img id="ImagePart" alt="Image of the Part" src="" runat="server" width="200" height="200" /> <!-- Use javascript to resize image dynamically? onload="ResizeImage(this,200,200);"-->
            </div>
            </dx:PanelContent>
        </PanelCollection>
        </dxrp:ASPxRoundPanel>
        <br />
        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel2" runat="server" Width="225px" 
            HeaderText="Specifications">
            <PanelCollection>
                <dx:PanelContent runat="server">
                <a id="sDatasheet" href="" runat="server" target="_blank">Datasheet</a> <br />
                <div id="sValue" runat="server"> </div>
                <div id="sTemp" runat="server"></div>
                </dx:PanelContent>
            </PanelCollection>
        </dxrp:ASPxRoundPanel>
    </div> <!-- Top Right Column -->
    
    
    
    <div id="BottomPane" style="position:absolute; top:400px; left:0; width:570px;">
        <b>Distributor Inventory:</b><br />
        <dxwgv:ASPxGridView ID="xGVDist" runat="server" AutoGenerateColumns="false">
            <Settings ShowFilterRow="True" />
            <Columns>
            <dxwgv:GridViewDataTextColumn Caption="Distributor" FieldName="Distributor" 
                    ReadOnly="True" Width="10px" VisibleIndex="1">
            </dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataHyperLinkColumn Caption="Website" FieldName="Website" ReadOnly="True" Width="15px" VisibleIndex="2">
            </dxwgv:GridViewDataHyperLinkColumn>
            <dxwgv:GridViewDataTextColumn Caption="Part Number" FieldName="Part Number" ReadOnly="True" Width="50px" VisibleIndex="3">
            </dxwgv:GridViewDataTextColumn>
            </Columns>
            <SettingsBehavior AllowFocusedRow="True" />
        </dxwgv:ASPxGridView>
        <br />
        <b>Altium CADD Models:</b><br />
        <dxwgv:ASPxGridView ID="xGVAltium" runat="server" AutoGenerateColumns="true">
            <Settings ShowFilterRow="True" />
            <SettingsBehavior AllowFocusedRow="False" />
        </dxwgv:ASPxGridView>
    </div>
    </form>
</body>
</html>
