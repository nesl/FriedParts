<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="dbUtils.aspx.vb" Inherits="pAdmin_dbUtils" title="Untitled Page" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxClasses" tagprefix="dxw" %>
<%@ Register assembly="System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="System.Web.UI" tagprefix="cc1" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="800px" HeaderText="Database State">
        <PanelCollection>  
            <dx:PanelContent id="haha" runat="server">
                <hr />
                <asp:Label ID="orphrecsLbl" runat="server" Text="Label"></asp:Label><br />
                <asp:Button ID="orphrecsBtn" runat="server" Text="Correct this?" />    
                <hr />
                <asp:Button ID="btnCreateCadAltiumLibTable" runat="server" Text="Create cad-AltiumLib Table" />    
            </dx:PanelContent>
        </PanelCollection>
    </dxrp:ASPxRoundPanel>
    
</asp:Content>

