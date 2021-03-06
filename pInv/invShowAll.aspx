<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="invShowAll.aspx.vb" Inherits="pInv_invShowAll" title="Browse Current Inventory" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxPopupControl" TagPrefix="dxpc" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>

<%@ Register assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dxe" %>

<asp:Content ID="ExtraHeadContent" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
    <script type="text/javascript" src="/FriedParts/pInv/invShowAll.js"></script>
    <!--#include file="/friedparts/fp_code/controls/parttypeaccordioncontrol/PartTypeAccordion.head"-->
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <!-- Search Terms Box -->
    <div id="SearchWarning" visible="false" runat="server" style="
                        border:1px solid rgb(126,172,177);
                        background-color: #fff9a8;
                        margin-bottom: 10px;
                        padding:4px; padding-left:10px;
                        font-size: 12pt;
                        ">
    </div>

    <!-- Bounding Update Frame -->
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <script type="text/javascript">
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                prm.add_endRequest(HandleUpdateRequest);

                function HandleUpdateRequest() {
                    //Do stuff here
                    $('div[id*="FilterHandle"]').click(function () { ToggleFilterShade(); }); //attach window shade js code

                    //Continue processing
                    UpdatePanelReady();
                }
            </script>

            <!-- Category Selector Pane -->
            <div>
                <div id="FilterHandle" 
                    style="
                        border:1px solid rgb(126,172,177);
                        border-bottom-style:none;
                        background-color:rgb(44,111,143); height:20px;
                        padding-top:8px; padding-left:10px;
                        ">
                        <span style=" font-weight:bold; color:Yellow;">Click here to browse parts by type...</span>
                </div>
                <div id="FilterPane" style="border: 1px solid rgb(126,172,177);padding:10px;background-color:White;">
                    <!--#include file="/friedparts/fp_code/controls/parttypeaccordioncontrol/PartTypeAccordion.body"-->
                </div>
                <div id="Div1" style="
                        border:1px solid rgb(126,172,177);
                        border-top-style:none;
                        background-color:rgb(44,111,143); height:5px;
                        ">
                </div>
            </div>

            <!-- Search/Browse Results Pane -->
            <div>
                <dxwgv:ASPxGridView ID="xGV" runat="server"
                    CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
                    AutoGenerateColumns="false" Font-Names="Tahoma" Font-Size="X-Small" 
                    SettingsPager-PageSize="20">

                    <Images ImageFolder="~/App_Themes/Glass/{0}/">
                    </Images>

                    <Settings ShowFilterRow="True" ShowGroupPanel="True" />

                    <SettingsLoadingPanel Text="Searching&amp;hellip;" />

                    <Styles CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass">
                        <Header ImageSpacing="5px" SortingImageSpacing="5px">
                        </Header>
                    </Styles>

                    <Columns>
                        <dxwgv:GridViewDataTextColumn Caption="Type of Part" FieldName="Type" ReadOnly="True" VisibleIndex="1">
                        </dxwgv:GridViewDataTextColumn>
                        <dxwgv:GridViewDataTextColumn Caption="Manufacturer" FieldName="mfrName" VisibleIndex="2">
                        </dxwgv:GridViewDataTextColumn>
                        <dxwgv:GridViewDataTextColumn Caption="Part Number" FieldName="mfrPartNum" VisibleIndex="3">
                        </dxwgv:GridViewDataTextColumn>
                        <dxwgv:GridViewDataTextColumn Caption="Description" FieldName="Description" VisibleIndex="4"
                            Width="200px">
                        </dxwgv:GridViewDataTextColumn>
                        <dxwgv:GridViewDataTextColumn Caption="Value" FieldName="Value" VisibleIndex="5">
                        </dxwgv:GridViewDataTextColumn>
                        <dxwgv:GridViewDataTextColumn Caption="Units" FieldName="Value_Units" VisibleIndex="6">
                        </dxwgv:GridViewDataTextColumn>
                        <dxwgv:GridViewDataTextColumn Caption="Tolerance" FieldName="Value_Tolerance" VisibleIndex="7">
                        </dxwgv:GridViewDataTextColumn>
                        <dxwgv:GridViewDataCheckColumn Caption="Verified" FieldName="Verified" VisibleIndex="8"
                            Width="25px">
                        </dxwgv:GridViewDataCheckColumn>
                        <dxwgv:GridViewDataCheckColumn Caption="Obsolete" FieldName="Obsolete" VisibleIndex="9"
                            Width="25px">
                        </dxwgv:GridViewDataCheckColumn>
                        <dxwgv:GridViewDataCheckColumn Caption="RoHS" FieldName="RoHS" VisibleIndex="10"
                            Width="25px">
                        </dxwgv:GridViewDataCheckColumn>
                        <dxwgv:GridViewDataTextColumn Caption="PartID" FieldName="PartID" VisibleIndex="11"
                            Width="25px">
                        </dxwgv:GridViewDataTextColumn>
                    </Columns>

                    <SettingsBehavior AllowFocusedRow="True" />

                    <ClientSideEvents RowClick="function(s, e) { OnGridRowClicked(e.visibleIndex); }"/>
                </dxwgv:ASPxGridView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- Detail View -->
    <dxpc:ASPxPopupControl ID="xPopup" runat="server" ContentUrl="~/pInv/invShowDetail.aspx" 
    AllowDragging="True" EnableAnimation="true" 
        ShowShadow="true" HeaderText="Part Details" 
    EnableClientSideAPI="True" EnableViewState="False" PopupAction="None" 
        BackColor="#FFFF99" Left="5" Top="40" Height="800px" Width="625px">
    </dxpc:ASPxPopupControl>    

    <script type="text/javascript">
        // <!CDATA[
        //function is called on changing focused row
        function OnGridRowClicked(visIdx) {
            if (window.parent.document.readyState == 'complete') {
                    ctl00_ContentPlaceHolder1_xGV.GetRowValues(visIdx, 'PartID;', OnGetRowValues);
                    ctl00_ContentPlaceHolder1_xPopup.RefreshContentUrl();
                    ctl00_ContentPlaceHolder1_xPopup.Show();
            }
        }
        //Value array contains "EmployeeID" and "Notes" field values returned from the server 
        function OnGetRowValues(values) {
            ctl00_ContentPlaceHolder1_xPopup.SetContentUrl("invShowDetail.aspx?detail=" + values[0]);        
        }
    </script>    
</asp:Content>

