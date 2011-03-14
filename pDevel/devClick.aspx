<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="devClick.aspx.vb" Inherits="pDevel_devClick" title="Untitled Page" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:Button ID="Button_Del_User" runat="server" Text="Delete User John Bakedman" />    
    <hr />
    <table>
        <tr>
            <td class="fpTableLabel">
                <asp:Label ID="Label0a" runat="server" Text="Label"></asp:Label><br />
            </td>
            <td>
                <asp:TextBox ID="txtInput0a" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="fpTableLabel">
                <asp:Label ID="Label0b" runat="server" Text="Label"></asp:Label><br />
            </td>
            <td>
                <asp:TextBox ID="txtInput0b" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="fpTableLabel">
                <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label><br />
            </td>
            <td>
                <asp:Label ID="Label1b" runat="server" Text="Label"></asp:Label><br />
            </td>
        </tr>
        <tr>
            <td class="fpTableLabel">
                <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label><br />
            </td>
            <td>
                <asp:Label ID="Label2b" runat="server" Text="Label"></asp:Label><br />
            </td>
        </tr>
        <tr>
            <td class="fpTableLabel">
                <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label><br />
            </td>
            <td>
                <asp:Label ID="Label3b" runat="server" Text="Label"></asp:Label><br />
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <asp:Button ID="Button1" runat="server" Text="Execute Test Routine" />    
            </td>
        </tr>
    </table>    
    <hr />
        <form action='http://friedparts.nesl.ucla.edu/FriedParts/psys/sysWebService.asmx/HelloWorld' method="post">                      
            <table cellspacing="0" cellpadding="4" frame="box" rules="none" style="border-collapse: collapse;">
                <tr>
                    <td></td>
                    <td align="right"> <input type="submit" value="Invoke" class="button" /></td>
                </tr>
            </table>
        </form>
    <hr />
    <dx:ASPxGridView ID="xGrid" runat="server">
    </dx:ASPxGridView>
</asp:Content>

