<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="sysService.aspx.vb" Inherits="pSys_sysService" title="Untitled Page" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dxe" %>

<%@ Register assembly="DevExpress.Web.ASPxGauges.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges.Linear" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges.Circular" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges.State" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges.Digital" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        
    <%-- Script Manager is required on any page with AJAX functionality--%>
    <asp:ScriptManager runat="server" ID="ScriptManager"></asp:ScriptManager>
    <%-- Timer controls update rate / animation --%>
     <asp:Timer ID="tmrUpdateRate" runat="server" Interval="1000"></asp:Timer>

     <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div align="center" style="height:300px">
                <div style="float:left">
                    <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="200px" HeaderText="Control Panel">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent1" runat="server">
                        <div class="fpSmall">
                            <table>
                                <tr>
                                    <td colspan="3">
                                        <dxe:ASPxProgressBar ID="xProgress" runat="server" Height="6px" Width="200px" ShowPosition="false"
                                            CssFilePath="~/App_Themes/PlasticBlue/{0}/styles.css" CssPostfix="PlasticBlue" >
                                        </dxe:ASPxProgressBar>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <dx:ASPxGaugeControl ID="xGaugeDispatcherState" runat="server" 
                                            BackColor="Transparent" ClientIDMode="AutoID" Height="30px" Value="0" 
                                            Width="30px">
                                            <Gauges>
                                                <dx:StateIndicatorGauge Bounds="0, 0, 30, 30" Name="Gauge0">
                                                    <indicators>
                                                        <dx:StateIndicatorComponent Center="124, 124" Name="stateIndicatorComponent1" 
                                                            StateIndex="0">
                                                            <states>
                                                                <dx:IndicatorStateWeb Name="State1" ShapeType="ElectricLight1" />
                                                                <dx:IndicatorStateWeb Name="State2" ShapeType="ElectricLight2" />
                                                                <dx:IndicatorStateWeb Name="State3" ShapeType="ElectricLight3" />
                                                                <dx:IndicatorStateWeb Name="State4" ShapeType="ElectricLight4" />
                                                            </states>
                                                        </dx:StateIndicatorComponent>
                                                    </indicators>
                                                </dx:StateIndicatorGauge>
                                            </Gauges>
                                        </dx:ASPxGaugeControl>
                                    </td>
                                    <td colspan="2">
                                        <asp:Button ID="btnStart" runat="server" Text="Start Service" Font-Names="Tahoma" Font-Size="8pt" />
                                        <asp:Button ID="btnStop" runat="server" Text="Stop Service" Font-Names="Tahoma" Font-Size="8pt" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3"><asp:Label ID="lblDispatcherState" runat="server" Text=""></asp:Label></td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td class="fpTableLabel">Number of Server App Pools:</td>
                                    <td><asp:Label ID="lblNumAppPools" runat="server" Text=""></asp:Label></td>
                                </tr>
                                <tr>
                                    <td class="fpTableLabel">Number of Dispatchers:</td>
                                    <td><asp:Label ID="lblNumDispatchers" runat="server" Text=""></asp:Label></td>
                                </tr>
                                <tr>
                                    <td class="fpTableLabel">Number of Workers:</td>
                                    <td><asp:Label ID="lblNumWorkers" runat="server" Text=""></asp:Label></td>
                                </tr>
                            </table>
                        </div>
                        </dx:PanelContent>    
                    </PanelCollection>
                    </dxrp:ASPxRoundPanel>
                </div>
                                
                
                <!-- WORKER THREADS TABLE -->                
                <div style="float:left; margin-left:20px">
                    <dxrp:ASPxRoundPanel ID="ASPxRoundPanel3" runat="server" Width="200px" HeaderText="Update-Service Workers">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent3" runat="server">
                            <dx:ASPxGridView ID="xGridThreads" runat="server" Font-Names="Tahoma" 
                                Font-Size="X-Small" EnableCallBacks="False">
                            </dx:ASPxGridView>
                        </dx:PanelContent>    
                    </PanelCollection>
                    </dxrp:ASPxRoundPanel>
                </div>


                <!-- EVENT LOG TABLE -->
                <div style="float:left; margin-left:20px">
                    <dxrp:ASPxRoundPanel ID="ASPxRoundPanel2" runat="server" Width="500px" HeaderText="Event Log">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent2" runat="server">
                            <dx:ASPxGridView ID="xGridEvents" runat="server" Font-Names="Tahoma" 
                                Font-Size="X-Small" EnableCallBacks="False">
                            </dx:ASPxGridView>
                        </dx:PanelContent>    
                    </PanelCollection>
                    </dxrp:ASPxRoundPanel>
                </div>



            </div>
        </ContentTemplate>
        <Triggers>
            <%-- The update panel will postback when the timer 'tick' event fires --%>
            <asp:AsyncPostBackTrigger ControlID="tmrUpdateRate" EventName="Tick" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

