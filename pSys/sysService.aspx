<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="sysService.aspx.vb" Inherits="pSys_sysService" title="Untitled Page" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dxe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div align=center style="height:300px">
        <div style="float:left">
            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="200px" HeaderText="Control Panel">
            <PanelCollection>
                <dx:PanelContent ID="PanelContent1" runat="server">
                    <asp:Button ID="ButtonStart" runat="server" Text="Start" />
                    <asp:Button ID="ButtonStop" runat="server" Text="Stop" />
                </dx:PanelContent>    
            </PanelCollection>
            </dxrp:ASPxRoundPanel>
        </div>
        <div style="float:left; margin-left:20px">
            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel2" runat="server" Width="200px" HeaderText="Progress">
            <PanelCollection>
                <dx:PanelContent ID="PanelContent2" runat="server">
                    Processing Record:
                    <hr />
                    <dxe:ASPxProgressBar ID="ASPxProgressBar2"
                        runat="server" Height="21px" Position="50" Width="200px">
                    </dxe:ASPxProgressBar>
                </dx:PanelContent>    
            </PanelCollection>
            </dxrp:ASPxRoundPanel>
        </div>
    </div>
</asp:Content>

