<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="libBrowse.aspx.vb" Inherits="pLib_libBrowse" title="Untitled Page" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>

<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPopupControl" tagprefix="dxpc" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>

<%@ Register assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" HeaderText="Please note..." BackColor="LightYellow" Width="600px"><PanelCollection><dx:PanelContent>
        <ul>
            <li>Click to download from installed list below</li>
            <li>Eventually, I'll have a nightly zip file of all libraries available here for download as well</li>
        </ul>
    </div>
    </dx:PanelContent></PanelCollection></dxrp:ASPxRoundPanel>
    
    <hr />
    Available Schematic Symbol Libraries:<br />
    <dxwgv:ASPxGridView ID="xGridViewSch" runat="server" 
        AutoGenerateColumns="False" DataSourceID="SqlDataSourceSch">
        <Columns>
            <dxwgv:GridViewDataHyperLinkColumn FieldName="LibName" VisibleIndex="0">
            </dxwgv:GridViewDataHyperLinkColumn>
            <dxwgv:GridViewDataTextColumn FieldName="UserName" VisibleIndex="1">
                <PropertiesTextEdit DisplayFormatString="{0}">
                </PropertiesTextEdit>
            </dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataDateColumn FieldName="DateLastUpdate" VisibleIndex="2">
            </dxwgv:GridViewDataDateColumn>
            <dxwgv:GridViewDataTextColumn FieldName="Desc" VisibleIndex="3">
            </dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataDateColumn FieldName="DateCreated" VisibleIndex="4">
            </dxwgv:GridViewDataDateColumn>
            <dxwgv:GridViewDataTextColumn FieldName="LibID" VisibleIndex="5">
            </dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataTextColumn FieldName="LibType" Visible="False" 
                VisibleIndex="6">
            </dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataTextColumn FieldName="UserEmail" Visible="False" 
                VisibleIndex="7">
            </dxwgv:GridViewDataTextColumn>
        </Columns>
    </dxwgv:ASPxGridView>
    <asp:SqlDataSource ID="SqlDataSourceSch" runat="server" 
        ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
        SelectCommand="SELECT [DateLastUpdate], [DateCreated], [Desc], [UserName], [LibID], [LibName], [LibType], [UserEmail] FROM [view-cadAltiumLib] WHERE ([LibType] = @LibType)">
        <SelectParameters>
            <asp:Parameter DefaultValue="1" Name="LibType" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <hr />
    Available PCB Footprint Libraries:<br />
    <dxwgv:ASPxGridView ID="xGridViewPcb" runat="server" 
        AutoGenerateColumns="False" DataSourceID="SqlDataSourcePcb">
        <Columns>
            <dxwgv:GridViewDataHyperLinkColumn FieldName="LibName" VisibleIndex="0">
            </dxwgv:GridViewDataHyperLinkColumn>
            <dxwgv:GridViewDataTextColumn FieldName="UserName" VisibleIndex="1">
                <PropertiesTextEdit DisplayFormatString="{0}">
                </PropertiesTextEdit>
            </dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataDateColumn FieldName="DateLastUpdate" VisibleIndex="2">
            </dxwgv:GridViewDataDateColumn>
            <dxwgv:GridViewDataTextColumn FieldName="Desc" VisibleIndex="3">
            </dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataDateColumn FieldName="DateCreated" VisibleIndex="4">
            </dxwgv:GridViewDataDateColumn>
            <dxwgv:GridViewDataTextColumn FieldName="LibID" VisibleIndex="5">
            </dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataTextColumn FieldName="LibType" Visible="False" 
                VisibleIndex="6">
            </dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataTextColumn FieldName="UserEmail" Visible="False" 
                VisibleIndex="7">
            </dxwgv:GridViewDataTextColumn>
        </Columns>
    </dxwgv:ASPxGridView>
    <asp:SqlDataSource ID="SqlDataSourcePcb" runat="server" 
        ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
        SelectCommand="SELECT [DateLastUpdate], [DateCreated], [Desc], [UserName], [LibID], [LibName], [LibType], [UserEmail] FROM [view-cadAltiumLib] WHERE ([LibType] = @LibType)">
        <SelectParameters>
            <asp:Parameter DefaultValue="2" Name="LibType" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <hr />
</asp:Content>

