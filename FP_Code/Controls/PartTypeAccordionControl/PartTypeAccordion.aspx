<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PartTypeAccordion.aspx.vb" Inherits="FP_Code_Controls_PartTypeAccordionControl" %>
<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
            Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="/FriedParts/FP_Code/jquery-1.4.4.min.js"></script>
    <script type="text/javascript" src="/FriedParts/FP_Code/Controls/PartTypeAccordionControl/Lib/easyAccordion.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#accordion-1').easyAccordion({
                autoStart: false,
            });
        });
    </script>
    <link rel="stylesheet" type="text/css" href="/FriedParts/FP_Code/Controls/PartTypeAccordionControl/Lib/easyAccordion.css" />
    <link rel="stylesheet" type="text/css" href="/FriedParts/FP_Code/Controls/PartTypeAccordionControl/Lib/PartTypeAccordion.css" />
</head>
<body>
    <form id="form1" runat="server">
             
        <asp:HiddenField ID="SelectedPartTypeID" runat="server" />

        <div id="pthaBreadcrumbs" runat="server" class="pthaBreadcrumbsContent"></div>

        <div id="resetCSSinheritance" style="padding:0px;margin:0px;">
            <div id="accordion-1">
                <dl>
                    <dt id="L2a" runat="server">One more</dt>
                        <dd id="L2b" runat="server">
                            <dxwgv:ASPxGridView ID="L2g" runat="server" EnableCallBacks="False" 
                                KeyFieldName="TypeID" Font-Names="Tahoma" Font-Size="7pt"
                                ClientIDMode="AutoID">
                                <ClientSideEvents RowClick="function(s, e) {s.PerformCallback(''+e.visibleIndex);}"></ClientSideEvents>
                                <Columns>
                                    <dxwgv:GridViewDataTextColumn FieldName="TypeID" ShowInCustomizationForm="True" 
                                        VisibleIndex="0" Visible="false">
                                    </dxwgv:GridViewDataTextColumn>
                                    <dxwgv:GridViewDataTextColumn FieldName="Type" ShowInCustomizationForm="True" 
                                        VisibleIndex="1" CellStyle-Font-Bold="true">
                                    </dxwgv:GridViewDataTextColumn>
                                    <dxwgv:GridViewDataTextColumn FieldName="TypeNotes" ShowInCustomizationForm="True" 
                                        VisibleIndex="2">
                                    </dxwgv:GridViewDataTextColumn>
                                </Columns>
                                <SettingsBehavior AllowDragDrop="False"></SettingsBehavior>
                                <SettingsPager PageSize="8">
                                </SettingsPager>
                                <Settings ShowFilterRow="True" />
                            </dxwgv:ASPxGridView>
                        </dd>
                    <dt id="L3a" runat="server">Title slide</dt>
                        <dd id="L3b" runat="server">
                            <dxwgv:ASPxGridView ID="L3g" runat="server" EnableCallBacks="False" 
                                KeyFieldName="TypeID" Font-Names="Tahoma" Font-Size="8pt"
                                ClientIDMode="AutoID">
                                <ClientSideEvents RowClick="function(s, e) {s.PerformCallback(''+e.visibleIndex);}"></ClientSideEvents>
                                <Columns>
                                    <dxwgv:GridViewDataTextColumn FieldName="TypeID" ShowInCustomizationForm="True" 
                                        VisibleIndex="0" Visible="false">
                                    </dxwgv:GridViewDataTextColumn>
                                    <dxwgv:GridViewDataTextColumn FieldName="Type" ShowInCustomizationForm="True" 
                                        VisibleIndex="1" CellStyle-Font-Bold="true">
                                    </dxwgv:GridViewDataTextColumn>
                                    <dxwgv:GridViewDataTextColumn FieldName="TypeNotes" ShowInCustomizationForm="True" 
                                        VisibleIndex="2">
                                    </dxwgv:GridViewDataTextColumn>
                                </Columns>
                                <SettingsBehavior AllowDragDrop="False"></SettingsBehavior>
                                <SettingsPager PageSize="8">
                                </SettingsPager>
                                <Settings ShowFilterRow="True" />
                            </dxwgv:ASPxGridView>
                        </dd>        
                     <dt id="L4a" runat="server">Title slide</dt>
                        <dd id="L4b" runat="server">
                            <dxwgv:ASPxGridView ID="L4g" runat="server" EnableCallBacks="False" 
                                KeyFieldName="TypeID" Font-Names="Tahoma" Font-Size="8pt"
                                ClientIDMode="AutoID">
                                <ClientSideEvents RowClick="function(s, e) {s.PerformCallback(''+e.visibleIndex);}"></ClientSideEvents>
                                <Columns>
                                    <dxwgv:GridViewDataTextColumn FieldName="TypeID" ShowInCustomizationForm="True" 
                                        VisibleIndex="0" Visible="false">
                                    </dxwgv:GridViewDataTextColumn>
                                    <dxwgv:GridViewDataTextColumn FieldName="Type" ShowInCustomizationForm="True" 
                                        VisibleIndex="1" CellStyle-Font-Bold="true">
                                    </dxwgv:GridViewDataTextColumn>
                                    <dxwgv:GridViewDataTextColumn FieldName="TypeNotes" ShowInCustomizationForm="True" 
                                        VisibleIndex="2">
                                    </dxwgv:GridViewDataTextColumn>
                                </Columns>
                                <SettingsBehavior AllowDragDrop="False"></SettingsBehavior>
                                <SettingsPager PageSize="8">
                                </SettingsPager>
                                <Settings ShowFilterRow="True" />
                            </dxwgv:ASPxGridView>
                        </dd>


                    <dt id="L5a" runat="server">Title slide</dt>
                    <dd id="L5b" runat="server">
                        <dxwgv:ASPxGridView ID="L5g" runat="server" EnableCallBacks="False"
                            KeyFieldName="TypeID" Font-Names="Tahoma" Font-Size="8pt"
                            ClientIDMode="AutoID">
                            <ClientSideEvents RowClick="function(s, e) {s.PerformCallback(''+e.visibleIndex);}"></ClientSideEvents>
                            <Columns>
                                <dxwgv:GridViewDataTextColumn FieldName="TypeID" ShowInCustomizationForm="True"
                                    VisibleIndex="0" Visible="false">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn FieldName="Type" ShowInCustomizationForm="True"
                                    VisibleIndex="1" CellStyle-Font-Bold="true">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn FieldName="TypeNotes" ShowInCustomizationForm="True"
                                    VisibleIndex="2">
                                </dxwgv:GridViewDataTextColumn>
                            </Columns>
                            <SettingsBehavior AllowDragDrop="False"></SettingsBehavior>
                            <SettingsPager PageSize="8">
                            </SettingsPager>
                            <Settings ShowFilterRow="True" />
                        </dxwgv:ASPxGridView>
                    </dd>

                    <dt id="L6a" runat="server">Title slide</dt>
                    <dd id="L6b" runat="server">
                        <dxwgv:ASPxGridView ID="L6g" runat="server" EnableCallBacks="False"
                            KeyFieldName="TypeID" Font-Names="Tahoma" Font-Size="8pt"
                            ClientIDMode="AutoID">
                            <ClientSideEvents RowClick="function(s, e) {s.PerformCallback(''+e.visibleIndex);}"></ClientSideEvents>
                            <Columns>
                                <dxwgv:GridViewDataTextColumn FieldName="TypeID" ShowInCustomizationForm="True"
                                    VisibleIndex="0" Visible="false">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn FieldName="Type" ShowInCustomizationForm="True"
                                    VisibleIndex="1" CellStyle-Font-Bold="true">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn FieldName="TypeNotes" ShowInCustomizationForm="True"
                                    VisibleIndex="2">
                                </dxwgv:GridViewDataTextColumn>
                            </Columns>
                            <SettingsBehavior AllowDragDrop="False"></SettingsBehavior>
                            <SettingsPager PageSize="8">
                            </SettingsPager>
                            <Settings ShowFilterRow="True" />
                        </dxwgv:ASPxGridView>
                    </dd>

                    <dt id="L7a" runat="server">Title slide</dt>
                    <dd id="L7b" runat="server">
                        <dxwgv:ASPxGridView ID="L7g" runat="server" EnableCallBacks="False"
                            KeyFieldName="TypeID" Font-Names="Tahoma" Font-Size="8pt"
                            ClientIDMode="AutoID">
                            <ClientSideEvents RowClick="function(s, e) {s.PerformCallback(''+e.visibleIndex);}"></ClientSideEvents>
                            <Columns>
                                <dxwgv:GridViewDataTextColumn FieldName="TypeID" ShowInCustomizationForm="True"
                                    VisibleIndex="0" Visible="false">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn FieldName="Type" ShowInCustomizationForm="True"
                                    VisibleIndex="1" CellStyle-Font-Bold="true">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn FieldName="TypeNotes" ShowInCustomizationForm="True"
                                    VisibleIndex="2">
                                </dxwgv:GridViewDataTextColumn>
                            </Columns>
                            <SettingsBehavior AllowDragDrop="False"></SettingsBehavior>
                            <SettingsPager PageSize="8">
                            </SettingsPager>
                            <Settings ShowFilterRow="True" />
                        </dxwgv:ASPxGridView>
                    </dd>

                    <dt id="L8a" runat="server">Title slide</dt>
                    <dd id="L8b" runat="server">
                        <dxwgv:ASPxGridView ID="L8g" runat="server" EnableCallBacks="False"
                            KeyFieldName="TypeID" Font-Names="Tahoma" Font-Size="8pt"
                            ClientIDMode="AutoID">
                            <ClientSideEvents RowClick="function(s, e) {s.PerformCallback(''+e.visibleIndex);}"></ClientSideEvents>
                            <Columns>
                                <dxwgv:GridViewDataTextColumn FieldName="TypeID" ShowInCustomizationForm="True"
                                    VisibleIndex="0" Visible="false">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn FieldName="Type" ShowInCustomizationForm="True"
                                    VisibleIndex="1" CellStyle-Font-Bold="true">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn FieldName="TypeNotes" ShowInCustomizationForm="True"
                                    VisibleIndex="2">
                                </dxwgv:GridViewDataTextColumn>
                            </Columns>
                            <SettingsBehavior AllowDragDrop="False"></SettingsBehavior>
                            <SettingsPager PageSize="8">
                            </SettingsPager>
                            <Settings ShowFilterRow="True" />
                        </dxwgv:ASPxGridView>
                    </dd>

                    <dt id="L9a" runat="server">Title slide</dt>
                    <dd id="L9b" runat="server">
                        <dxwgv:ASPxGridView ID="L9g" runat="server" EnableCallBacks="False"
                            KeyFieldName="TypeID" Font-Names="Tahoma" Font-Size="8pt"
                            ClientIDMode="AutoID">
                            <ClientSideEvents RowClick="function(s, e) {s.PerformCallback(''+e.visibleIndex);}"></ClientSideEvents>
                            <Columns>
                                <dxwgv:GridViewDataTextColumn FieldName="TypeID" ShowInCustomizationForm="True"
                                    VisibleIndex="0" Visible="false">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn FieldName="Type" ShowInCustomizationForm="True"
                                    VisibleIndex="1" CellStyle-Font-Bold="true">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn FieldName="TypeNotes" ShowInCustomizationForm="True"
                                    VisibleIndex="2">
                                </dxwgv:GridViewDataTextColumn>
                            </Columns>
                            <SettingsBehavior AllowDragDrop="False"></SettingsBehavior>
                            <SettingsPager PageSize="8">
                            </SettingsPager>
                            <Settings ShowFilterRow="True" />
                        </dxwgv:ASPxGridView>
                    </dd>

                </dl>
            </div>
        </div>
    </form>
</body>
</html>
