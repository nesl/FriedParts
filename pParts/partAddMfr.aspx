<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="partAddMfr.aspx.vb" Inherits="pParts_partAddMfr" title="Untitled Page" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <dx:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="200px" HeaderText="Add New Manufacturer">
        <PanelCollection>
            <dx:PanelContent>
                <table>
                    <tr>
                        <td class="fpTableLabel">Manufacturer Name:</td>
                        <td><asp:TextBox ID="txtMfrName" Width="400px" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="fpTableLabel">Website:</td>
                        <td><asp:TextBox ID="txtMfrWebsite" Width="400px" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr id="errRow" visible="false" class="fpErrorBox" runat="server">
                        <td class="fpTableLabel"></td>
                        <td>
                            <asp:Label ID="errMsg" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fpTableLabel"></td>
                        <td>
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
                        </td>
                    </tr>
                </table>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxRoundPanel>
</asp:Content>

