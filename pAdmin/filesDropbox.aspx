<%@ Page Title="" Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="filesDropbox.aspx.vb" Inherits="pAdmin_filesDropbox" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>

<%@ Register assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<!-- RECENT EVENTS -->
            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="600px" HeaderText="ACCOUNT STATUS">
                <PanelCollection>
                    <dx:PanelContent>
                        <div>
                            <table>
                                <tr>
                                    <td class="fpTableLabel">Dropbox Account:</td>
                                    <td>
                                        <asp:Label ID="lblAccountHolder" runat="server" Text="Label"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="fpTableLabel">Current Status:</td>
                                    <td>
                                        <asp:Label ID="lblStatus" runat="server" Text="Label"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </dx:PanelContent>
                </PanelCollection>
            </dxrp:ASPxRoundPanel>

            <br /><br />
                <asp:Button ID="btnTest" runat="server" Text="Test Button" />
            <br /><br />

            <dxrp:ASPxRoundPanel ID="xrpDropboxEvents" runat="server" Width="600px" HeaderText="RECENT EVENTS">
            <PanelCollection>
            <dx:PanelContent>
                <div style="margin-bottom:10px">
                    This table shows only the activity for the current user. Files may be currently in transfer. Refresh frequently to see updates.
                </div>
                <dxwgv:ASPxGridView ID="xGridUserLog" runat="server" 
                    AutoGenerateColumns="False" DataSourceID="SqlDataSource1">
                    <Columns>
                        <dxwgv:GridViewDataDateColumn FieldName="TimestampDate" 
                            ShowInCustomizationForm="True" VisibleIndex="0">
                        </dxwgv:GridViewDataDateColumn>
                        <dxwgv:GridViewDataTextColumn FieldName="OpDesc" ShowInCustomizationForm="True" 
                            VisibleIndex="1">
                        </dxwgv:GridViewDataTextColumn>
                        <dxwgv:GridViewDataTextColumn ShowInCustomizationForm="True" 
                            VisibleIndex="2" FieldName="Filename">
                        </dxwgv:GridViewDataTextColumn>
                        <dxwgv:GridViewDataTextColumn ShowInCustomizationForm="True" 
                            VisibleIndex="3" FieldName="Path">
                        </dxwgv:GridViewDataTextColumn>
                        <dxwgv:GridViewDataTextColumn ShowInCustomizationForm="True" 
                            VisibleIndex="4" FieldName="Message">
                        </dxwgv:GridViewDataTextColumn>
                    </Columns>
                </dxwgv:ASPxGridView>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
                    SelectCommand="SELECT [Filename], [Path], [OpDesc], [Message], [TimestampDate], [Timestamp] FROM [view-logDropbox] WHERE ([UserID] = @UserID) ORDER BY [Timestamp] DESC, [TimestampDate] DESC">
                    <SelectParameters>
                        <asp:SessionParameter Name="UserID" SessionField="user.UserID" Type="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </dx:PanelContent>
            </PanelCollection>
            </dxrp:ASPxRoundPanel>

</asp:Content>

