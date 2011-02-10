<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="userNew.aspx.vb" Inherits="pUser_New" title="Untitled Page" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <dxrp:ASPxRoundPanel ID="CreatePanel" runat="server" Width="600px" HeaderText="Welcome New User!">
    <PanelCollection>
    <dx:PanelContent>
        <table id="Table1" style="font-weight:normal">
            <colgroup>
                <col width="200" />
            </colgroup>
            <tr>
                <td style="text-align:right;">You have authenticated as:</td>
                <td><asp:Label runat="server" ID="lAuth" style="font-weight:bold"></asp:Label></td>
            </tr>
            <tr>
                <td style="text-align:right">Please enter your real name:</td>
                <td><asp:TextBox runat="server" AutoPostBack="True" ID="lName"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="text-align:right">Please enter your e-mail address:</td>
                <td><asp:Label runat="server" ID="lEmail" style="font-weight:bold"></asp:Label></td>
            </tr>
            <tr>
                <td></td>
                <td><asp:Button ID="Button1" runat="server" Text="Create My Account!" /></td>
            </tr>
        </Table>
    </dx:PanelContent>
    </PanelCollection>
    </dxrp:ASPxRoundPanel>    
</asp:Content>

