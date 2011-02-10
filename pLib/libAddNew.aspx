<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="libAddNew.aspx.vb" Inherits="CODElibAddNew" title="Untitled Page" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>

<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPopupControl" tagprefix="dxpc" %>

<%@ Register assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dxe" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" HeaderText="Please note..." BackColor="LightYellow" Width="600px"><PanelCollection><dx:PanelContent>
        <ul>
            <li>Filenames must be in the case-sensitive format: <b>year_creator.libtype</b></li>
            <li>Libtypes (.SchLib, .PcbLib, etc) are case sensitive and will fail to be detected if not formatted properly.</li>
            <li>To ensure accuracy, use the file upload system here, rather than copying the files to the server directly.</li>
            <li>If you re-upload a library for which you are the owner, it overwrites the existing one</li>
        </ul>
    </dx:PanelContent></PanelCollection></dxrp:ASPxRoundPanel>  
    <hr />
    <asp:FileUpload ID="FileUpload" runat="server" />
    <asp:Button ID="btnUpload" runat="server" Text="Button" style="height: 26px" />
    <hr />
    New Schematic Symbol Libraries Detected:<br />
    <dxwgv:ASPxGridView ID="xGridNewSch" ClientInstanceName="xGridNewSch" runat="server">
        <SettingsBehavior AllowFocusedRow="True" />
        <ClientSideEvents RowClick="function(s, e) {OnRowClickSch(e);}" />
    </dxwgv:ASPxGridView>
    <script type="text/javascript">
        function OnRowClickSch(e){
            if (window.parent.document.readyState == 'complete') {
                xGridNewSch.GetRowValues(e.visibleIndex, 'filename;', OnGetRowValuesSch);
                xPopup.RefreshContentUrl();
                xPopup.Show();
            }
        }
        function OnGetRowValuesSch(values) {
            xPopup.SetContentUrl('libAddNewDetail.aspx?Filename=' + values[0] + '&LibType=1');
        }
    </script>
    <hr />
    New PCB Footprint Libraries Detected:<br />
    <dxwgv:ASPxGridView ID="xGridNewPCB" runat="server">
    </dxwgv:ASPxGridView>
    <hr />
    Missing Files:<br />
    <dxwgv:ASPxGridView ID="xGridMissing" runat="server">
    </dxwgv:ASPxGridView>
    
    <dxpc:ASPxPopupControl ID="xPopup" runat="server" ClientInstanceName="xPopup" 
        EnableClientSideAPI="True" BackColor="#FFFFCC" Height="550px" 
        PopupHorizontalAlign="Center" PopupVerticalAlign="Middle" Width="400px" 
        HeaderText="Library Detail">
        <ContentCollection>
            <dxpc:PopupControlContentControl runat="server"></dxpc:PopupControlContentControl>
        </ContentCollection>
    </dxpc:ASPxPopupControl>    
    <hr />
</asp:Content>

