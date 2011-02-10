<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" title="Untitled Page" %>

<%@ Register Assembly="DevExpress.XtraCharts.v10.2.Web, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.XtraCharts.Web" TagPrefix="dxchartsui" %>

<%@ Register Assembly="DevExpress.Web.ASPxGauges.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGauges" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>
<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxNewsControl" tagprefix="dxnc" %>
<%@ Register assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dxe" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>

<%@ Register assembly="DevExpress.Web.ASPxGauges.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges.Linear" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges.Circular" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges.State" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges.Digital" tagprefix="dx" %>

<%@ Register assembly="DevExpress.XtraCharts.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.XtraCharts" tagprefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div style="position:relative; height:620px">
        <div style="float:left">
        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="600px" 
            HeaderText="NEWS &amp; UPDATES">
            <PanelCollection>
            <dx:PanelContent>
                <dxnc:ASPxNewsControl ID="ASPxNewsControl" runat="server" 
                    CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
                    ImageFolder="~/App_Themes/Glass/{0}/" PagerPanelSpacing="20px" 
                    Width="100%" DataSourceID="fpSQLNews" DateField="newsDate" 
                    TextField="newsStory" HeaderTextField="newsTitle" ImageUrlField="newsImageURL" 
                    RowPerPage="5">
                    <ItemDateStyle Spacing="2px">
                    </ItemDateStyle>
                    <BackToTopImage Url="~/App_Themes/Glass/Web/ncBackToTop.png" />
                    <PagerStyle ItemSpacing="4px" Spacing="4px" />
                    <Border BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" />
                    <ItemSettings>
                        <TailImage Url="~/App_Themes/Glass/Web/ncTail.gif" />
                    </ItemSettings>
                    <PagerSettings ShowDefaultImages="True">
                        <FirstPageButton Text="">
                        </FirstPageButton>
                        <LastPageButton ImagePosition="Left" Text="">
                        </LastPageButton>
                        <NextPageButton Text="">
                        </NextPageButton>
                        <PrevPageButton Text="">
                        </PrevPageButton>
                        <Summary AllPagesText="" Text="" />
                    </PagerSettings>
                </dxnc:ASPxNewsControl>
                <asp:SqlDataSource ID="fpSQLNews" runat="server"></asp:SqlDataSource>
            </dx:PanelContent>
            </PanelCollection>
            </dxrp:ASPxRoundPanel>
        </div>
    
        <div style="float:right;">
            <!-- DASHBOARD -->
            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel2" runat="server" Width="600px" HeaderText="DASHBOARD">
            <PanelCollection>
            <dx:PanelContent>
                <dxchartsui:WebChartControl ID="WebChartControl1" runat="server" 
                    DataSourceID="SqlDataSource_UserActivity"  
                    Width="590px">
                    <DiagramSerializable>
<cc1:SwiftPlotDiagram>
                        <axisx visibleinpanesserializable="-1">
                            



<range sidemarginsenabled="True"></range>
                        



</axisx>
                        <axisy visibleinpanesserializable="-1">
                            



<range sidemarginsenabled="True" auto="False" maxvalueserializable="80" minvalueserializable="0"></range>
                        



</axisy>
                    </cc1:SwiftPlotDiagram>
</DiagramSerializable>
                    <FillStyle  FillMode="Gradient">
                        <OptionsSerializable>
<cc1:RectangleGradientFillOptions HiddenSerializableString="to be serialized"></cc1:RectangleGradientFillOptions>
</OptionsSerializable>
                    </FillStyle>

                    <SeriesSerializable>
                        <cc1:Series ArgumentDataMember="Date" Name="User Activity" 
                              
                            ValueDataMembersSerializable="UserActivity" ArgumentScaleType="DateTime">
                            <ViewSerializable>
<cc1:SwiftPlotSeriesView Antialiasing="True" HiddenSerializableString="to be serialized">
                                <linestyle thickness="3"></linestyle>
                            </cc1:SwiftPlotSeriesView>
</ViewSerializable>
                            <PointOptionsSerializable>
<cc1:PointOptions HiddenSerializableString="to be serialized">
                            </cc1:PointOptions>
</PointOptionsSerializable>
                            <LegendPointOptionsSerializable>
<cc1:PointOptions HiddenSerializableString="to be serialized">
                            </cc1:PointOptions>
</LegendPointOptionsSerializable>
                        </cc1:Series>
                        <cc1:Series ArgumentDataMember="Date" ArgumentScaleType="DateTime" 
                            Name="# of Searches"  
                             
                            ValueDataMembersSerializable="NumSearches">
                            <ViewSerializable>
<cc1:SwiftPlotSeriesView HiddenSerializableString="to be serialized">
                                <linestyle thickness="3"></linestyle>
                            </cc1:SwiftPlotSeriesView>
</ViewSerializable>
                            <PointOptionsSerializable>
<cc1:PointOptions HiddenSerializableString="to be serialized">
                            </cc1:PointOptions>
</PointOptionsSerializable>
                            <LegendPointOptionsSerializable>
<cc1:PointOptions HiddenSerializableString="to be serialized">
                            </cc1:PointOptions>
</LegendPointOptionsSerializable>
                        </cc1:Series>
                    </SeriesSerializable>

                    <SeriesTemplate  >
                        <ViewSerializable>
<cc1:SwiftPlotSeriesView HiddenSerializableString="to be serialized"></cc1:SwiftPlotSeriesView>
</ViewSerializable>
                        <PointOptionsSerializable>
<cc1:PointOptions HiddenSerializableString="to be serialized"></cc1:PointOptions>
</PointOptionsSerializable>
                        <LegendPointOptionsSerializable>
<cc1:PointOptions HiddenSerializableString="to be serialized"></cc1:PointOptions>
</LegendPointOptionsSerializable>
                    </SeriesTemplate>
                </dxchartsui:WebChartControl>
                <asp:SqlDataSource ID="SqlDataSource_UserActivity" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
                    
                    SelectCommand="SELECT [Date], [NumSearches], [UserActivity] FROM [view-logStats]">
                </asp:SqlDataSource>
            </dx:PanelContent>
            </PanelCollection>
            </dxrp:ASPxRoundPanel>
            <br /><br />
            <!-- RECENT EVENTS -->
            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel3" runat="server" Width="600px" HeaderText="RECENT EVENTS">
            <PanelCollection>
            <dx:PanelContent>
                <dxwgv:ASPxGridView ID="xGridUserLog" runat="server">
                </dxwgv:ASPxGridView>
            </dx:PanelContent>
            </PanelCollection>
            </dxrp:ASPxRoundPanel>
        </div>
    </div>
</asp:Content>
