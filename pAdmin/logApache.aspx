<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="logApache.aspx.vb" Inherits="pAdmin_logApache" title="Untitled Page" %>

<%@ Register assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGridView" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<div>Note: GMT -4 is Los Angeles time +3 hours</div>
<div>Note: Los Angeles time is GMT-4 -3 hours</div>
    <dx:ASPxGridView ID="xGrid" runat="server" Visible=false>
        <Settings ShowFilterRow="True" ShowGroupPanel="True" />
    </dx:ASPxGridView>
    
    <asp:TextBox ID="TextBox1" runat="server" TextMode="MultiLine" Width="800px" Height="600px"></asp:TextBox>
    <br /><asp:Button ID="Button1" runat="server" Text="Button" />
</asp:Content>

