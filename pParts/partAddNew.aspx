<%@ Page Language="VB" MasterPageFile="../FP.master" AutoEventWireup="false" CodeFile="partAddNew.aspx.vb" Inherits="pParts_partAddNew" title="Add New Part" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxDataView" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxCallback" TagPrefix="dxcb" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxLoadingPanel" TagPrefix="dxlp" %>

<%@ Register Assembly="DevExpress.Web.ASPxTreeList.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxTreeList" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>

<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dxe" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxTabControl" TagPrefix="dxtc" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxClasses" tagprefix="dxw" %>

<%@ Register assembly="System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="System.Web.UI" tagprefix="cc1" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
    <script type="text/javascript" src="partAddNew.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <!-- LOADING PANEL MECHANISM -->
        <dxcb:ASPxCallback ID="ASPxCallback1" runat="server" ClientInstanceName="Callback">
            <ClientSideEvents CallbackComplete="function(s, e) { LoadingPanel.Hide(); }" />
        </dxcb:ASPxCallback>
        <dxlp:ASPxLoadingPanel ID="LoadingPanel" runat="server" ClientInstanceName="LoadingPanel" Modal="True"></dxlp:ASPxLoadingPanel>

    <!-- OVERALL CONTENT CONTAINER -->    
    <div style="position:relative">
        <asp:HiddenField ID="hiddenMfrPartNumber" runat="server" />
        <asp:HiddenField ID="hPartTypePath" runat="server" />

    <!-- TAB PAGES -->        
        <dxtc:ASPxPageControl ID="xTabPages" ClientInstanceName="xTabPages" 
            runat="server" ActiveTabIndex="3" 
            Height="200px" Width="700px" 
            CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
            TabSpacing="0px" EnableCallBacks="True">
            <LoadingPanelImage Url="~/App_Themes/Glass/Web/Loading.gif">
            </LoadingPanelImage>
            <ContentStyle>
                <Border BorderColor="#4986A2" />
            </ContentStyle>

            <TabPages>
                
                <dxtc:TabPage Text="Search"><ContentCollection><dxw:ContentControl ID="ContentControl1" runat="server">                    
                    <asp:Panel ID="Panel5" runat="server" style="color:black;">
                        <div style="margin-bottom:10px">
                            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel13" runat="server" Width="810px" HeaderText="Suggestive Search">
                                <PanelCollection>
                                    <dx:PanelContent ID="PanelContent14" runat="server">
                                    <div style="float:left;">
                                        <dxe:ASPxTextBox ID="xTextBoxSearch" runat="server" Width="170px">
                                        </dxe:ASPxTextBox>
                                        <dxe:ASPxButton ID="xbtnSearch" runat="server" Text="Search!" AutoPostBack="False">
                                            <ClientSideEvents Click="function(s, e) {Callback.PerformCallback(); LoadingPanel.Show();}" />
                                        </dxe:ASPxButton>
                                    </div>
                                    <div style="float:left; margin-left:10px;">
                                        Enter a Digikey part number or keywords for a generic part search.
                                    </div>
                                    <div style="float:right; margin-right:10px;">
                                        <asp:ImageButton ID="easyButton" runat="server" ImageUrl="~/Images/Parts/easy_button.gif" />
                                        <div id="divEasyError" class="fpErrorBox" runat="server" visible="false"></div>
                                    </div>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dxrp:ASPxRoundPanel>
                        </div>
                        <div style="float:left">
                            <dxrp:ASPxRoundPanel ID="xrpOctopart" runat="server" Width="600px" HeaderText="Octopart">
                                <PanelCollection>
                                    <dx:PanelContent ID="PanelContent15" runat="server">
                                        <div id="divSearchStats" class="fpHighlightBox" runat="server" visible="false"></div>
                                        <div id="divTooManyResults" class="fpErrorBox" runat="server" visible="false"></div>
                                        <dxwgv:ASPxGridView ID="xGridOctopartMaster" KeyFieldName="OctoPartID" runat="server">
                                            <Columns>
                                                <dxwgv:GridViewDataColumn FieldName="MpnID" Visible="false" VisibleIndex="1" />                            
                                                <dxwgv:GridViewDataColumn FieldName="MfrPartNum" Caption="Part Number" VisibleIndex="2" />                            
                                                <dxwgv:GridViewDataColumn FieldName="Highlight" VisibleIndex="2" />                            
                                            </Columns>
                                            <SettingsBehavior AllowFocusedRow="true" />
                                        </dxwgv:ASPxGridView>                                        
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dxrp:ASPxRoundPanel>  
                        </div>                       
                                                
                    </asp:Panel>
                </dxw:ContentControl></ContentCollection></dxtc:TabPage>

             




                <dxtc:TabPage Text="STEP 1. MANUFACTURER">
                <ContentCollection><dxw:ContentControl runat="server">
                    <asp:Panel runat="server" style="color:black;">
                        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1002" runat="server" Width="700px" HeaderText="Select the Manufacturer">
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent2" runat="server"><div>
                                <table>
                                    <tr>
                                        <td class="fpTableLabel">Choose Manufacturer:</td>
                                        <td><asp:DropDownList ID="SelectMfr" runat="server" AutoPostBack="true" DataTextField="mfrName" DataValueField="mfrName"></asp:DropDownList></td>
                                    </tr>
                                    <tr id="rowSelectMfr" runat="server">
                                        <td class="fpTableLabel"></td>
                                        <td><div style="width:400px; margin-bottom:8px;">Manufacturers found during Digikey searches are added automatically or you may <a href="partAddMfr.aspx" target="_blank">add a manufacturer manually</a>.</div></td>
                                    </tr>
                                    <tr>
                                        <td class="fpTableLabel">Manufacturer Part#:</td>
                                        <td><asp:TextBox ID="MfrPartNum" runat="server" AutoPostBack="true"></asp:TextBox></td>
                                    </tr>
                                </table>
                                </div></dx:PanelContent>
                            </PanelCollection>
                        </dxrp:ASPxRoundPanel>                        
                </asp:Panel>
                </dxw:ContentControl></ContentCollection>
                </dxtc:TabPage>
                





                <dxtc:TabPage Text="STEP 2. PART TYPE">
                <ContentCollection><dxw:ContentControl ID="ContentControl2" runat="server">
                
                    <asp:Panel ID="Panel3" runat="server" style="color:black;">
                    
                        <asp:ScriptManager ID="ScriptManager1" runat="server" />
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server"><ContentTemplate>
                        
                        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel12" runat="server" HeaderText="Part Type" 
                            Width="700px" BackColor="White">
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent13" runat="server">
                                    <asp:Label ID="lblCurrentPartType" runat="server" Text="No Part Type Selected..."></asp:Label><hr />
                                    <dx:ASPxTreeList ID="xPartTypesTree" ClientInstanceName="ASPxTreeList1" runat="server" AutoGenerateColumns="False" 
                                        CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
                                        DataSourceID="xTreeSqlDataSource" KeyFieldName="TypeID" ParentFieldName="Parent" EnableCallbacks="False" SettingsBehavior-ProcessFocusedNodeChangedOnServer="True" SettingsBehavior-AllowFocusedNode="True" 
                                        ClientSideEvents-FocusedNodeChanged="function(s, e) { }">
                                        <Columns>
                                            <dx:TreeListTextColumn FieldName="Type" VisibleIndex="0">
                                            </dx:TreeListTextColumn>
                                            <dx:TreeListTextColumn FieldName="TypeNotes" VisibleIndex="1">
                                            </dx:TreeListTextColumn>
                                            <dx:TreeListTextColumn FieldName="Path" VisibleIndex="2" Visible="False">
                                            </dx:TreeListTextColumn>
                                        </Columns>
                                        <SettingsBehavior AllowFocusedNode="True" />
                                        <SettingsPager Mode="ShowPager">
                                        </SettingsPager>
                                        <Images SpriteCssFilePath="~/App_Themes/Glass/{0}/sprite.css">
                                            <LoadingPanel Url="~/App_Themes/Glass/TreeList/Loading.gif">
                                            </LoadingPanel>
                                        </Images>
                                        <Styles CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass">
                                        </Styles>
                                    </dx:ASPxTreeList>
                                    <asp:SqlDataSource ID="xTreeSqlDataSource" runat="server" 
                                        ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
                                        SelectCommand="SELECT * FROM [part-PartTypes] ORDER BY [Type]"></asp:SqlDataSource>
                                </dx:PanelContent>
                            </PanelCollection>
                        </dxrp:ASPxRoundPanel>
                        <br />
                        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel10" runat="server" 
                            HeaderText="Part Value" Width="700px">
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent11" runat="server">                                    
                                <div id="PartValueBoxContainer">
                                    <table>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <dxe:ASPxCheckBox ID="xCheckNoValue" runat="server" AutoPostBack="true">
                                            </dxe:ASPxCheckBox>
                                            Check to use the Manufacturer's Part Number instead.
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="fpTableLabel"><asp:Label ID="lblValue" runat="server" Text="Value:"></asp:Label></td>
                                        <td>    
                                            <asp:TextBox ID="txtValue" runat="server" 
                                                OnKeyUp="
                                                ctl00_ContentPlaceHolder1_xTabPages_ASPxRoundPanel10_lblComputedValue.innerHTML = pvDisplayValue(ctl00_ContentPlaceHolder1_xTabPages_ASPxRoundPanel10_txtValue.value, ctl00_ContentPlaceHolder1_xTabPages_ASPxRoundPanel10_txtValueUnits.value)
                                                " >
                                            </asp:TextBox>
                                        </td>
                                        <td>
                                            <div class="fpHighlightBox">
                                                <asp:Label ID="lblComputedValue" runat="server" Text="Not valid!" Visible="false"></asp:Label>
                                            </div>                                    
                                        </td>
                                    </tr>

                                    <tr>
                                        <td class="fpTableLabel">
                                            <asp:Label ID="lblValueUnits" runat="server" Text="Units:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtValueUnits" runat="server"
                                            OnKeyUp="
                                                ctl00_ContentPlaceHolder1_xTabPages_ASPxRoundPanel10_lblComputedValue.innerHTML = pvDisplayValue(ctl00_ContentPlaceHolder1_xTabPages_ASPxRoundPanel10_txtValue.value, ctl00_ContentPlaceHolder1_xTabPages_ASPxRoundPanel10_txtValueUnits.value)
                                                ">
                                            </asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="fpTableLabel">
                                            <asp:Label ID="lblValueTol" runat="server" Text="Tolerance:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtValueTol" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    </table>
                                    
                                    <hr />
                                        <asp:Label ID="lblValueNotes" runat="server" Text="" Visible="false"></asp:Label>
                                    <hr />
                                        This is the value used for search and sorting components of the same type (like 
                                        resistors).
                                    <hr />
                                        This value also appears with the component in the schematic and is only 
                                        appropriate when there is a primary part characteristic (as in passives: 
                                        resistors, capacitors, etc).
                                    <hr />
                                        Check the box above to use the Manufacturer&#39;s Part Number as its schematic comment 
                                        instead. This is appropriate for all other components. If in doubt, just check the box.
                                    <hr />
                                        Units must be in pSpice compatible nomenclature and are case-insensitive. 
                                        Therefore &#39;m&#39; = &#39;M&#39; = milli. Use &#39;Meg&#39; for Mega.
                                </div>
                                </dx:PanelContent>
                            </PanelCollection>
                        </dxrp:ASPxRoundPanel>
                        
                        </ContentTemplate></asp:UpdatePanel>
                    </asp:Panel>
                </dxw:ContentControl></ContentCollection>
                </dxtc:TabPage>



                <dxtc:TabPage Text="STEP 3. PARAMETERS">
                <ContentCollection><dxw:ContentControl runat="server">
                    <asp:Panel ID="Panel4" runat="server" style="color:black;">
                        <br />
                        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel2" runat="server" HeaderText="Datasheet" 
                            Width="700px">
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent3" runat="server">
                                <div style="position:relative">
                                    Datasheet URL:
                                    <asp:DropDownList ID="DatasheetURL" runat="server" Width="600px" autopostback="true">
                                    </asp:DropDownList><br /><br />
                                    <asp:TextBox ID="txtDatasheetURL" runat="server" Width="600px" Visible="false"></asp:TextBox>
                                    <dxe:ASPxHyperLink ID="xDatasheetPreviewLink" runat="server" Text="Preview" Target="_blank" ClientVisible="false">
                                    </dxe:ASPxHyperLink>
                                    <div id="MoreDatasheetURL" class="fpHighlightBox" runat="server" style="position:absolute; top:10px; right:3px; width:60px; font-size:6pt;">
                                        More found. Use Pull-down.
                                    </div>
                                </div>    
                                </dx:PanelContent>
                            </PanelCollection>                            
                        </dxrp:ASPxRoundPanel>
                        <br />
                        <dxrp:ASPxRoundPanel ID="ImagePanel" runat="server" Width="700px" HeaderText="Image" ClientVisible="True">
                                <PanelCollection>
                                    <dx:PanelContent ID="PanelContent9" runat="server">
                                    <div>
                                    <table>
                                        <tr>
                                            <td class="fpTableLabel">Image URL:</td>
                                            <td><asp:TextBox ID="ImageURL" runat="server" Width="500px"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td class="fpTableLabel"></td>
                                            <td>
                                                <dxe:ASPxCheckBox ID="xCheckNoImage" runat="server">
                                                </dxe:ASPxCheckBox>
                                                Check for default &#39;No Image&#39; image.
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class=""></td>
                                            <td>
                                                <dxe:ASPxImage ID="ASPxSelectedImage" Height="100px" Width="100px" ImageURL="null" runat="server" ClientVisible="False">
                                                </dxe:ASPxImage>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>
                                                <dxe:ASPxButton ID="ASPxReselectImage" runat="server" Text="Select Another Image" oncommand="ReselectBtn_Click" ClientVisible="false">
                                                </dxe:ASPxButton>
                                            </td>
                                        </tr>
                                    </table>
                                                                      
                                    <dx:ASPxDataView ID="ImageDataView" runat="server" collumnPerPage="3" AllowPaging="false">
                                            <ItemTemplate>
                                                <dxe:ASPxImage ID="ASPxImage2" runat="server" ImageUrl='<%# Eval("URL") %>' Height="150px" Width="150px"></dxe:ASPxImage>
                                                <dxe:ASPxButton ID="ASPxImageButton" runat="server" Text="Select Image" OnCommand="ImageBtn_Click" CommandName='<%# Eval("URL") %>'></dxe:ASPxButton>
                                            </ItemTemplate>
                                    </dx:ASPxDataView>
                                    </div>
                                    </dx:PanelContent>
                                </PanelCollection>
                        </dxrp:ASPxRoundPanel>
                        <br />                                               
                        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel7" runat="server" HeaderText="Part Description" Width="700px">
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent8" runat="server"><div>
                                    Short Description:
                                    <asp:TextBox ID="SrtDesBox" runat="server"></asp:TextBox>
                                    <br />
                                    <br />
                                    Long Description:
                                    <asp:TextBox ID="LongDesBox" runat="server" Height="150px" TextMode="MultiLine" 
                                        Width="300px"></asp:TextBox>
                                </div></dx:PanelContent>
                            </PanelCollection>
                        </dxrp:ASPxRoundPanel>
                        
                        <div style="position:absolute; top:582px; left:136px;">
                        </div>
                        <div style="position:absolute; top:110px; right:0px;">
                        </div>
                    </asp:Panel>
                </dxw:ContentControl></ContentCollection>
                </dxtc:TabPage>
                
                
                
                
                
                
                
                
                
                
                
                
                <dxtc:TabPage Text="STEP 4. CADD">
                <ContentCollection><dxw:ContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server" style="color:black;">
                <dxrp:ASPxRoundPanel ID="ASPxRoundPanel4" runat="server" Width="730px" HeaderText="Altium Schematic Symbols">
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent5" runat="server">
                                    <table width="650px" style="table-layout:fixed">
                                    <col width="300px" />
                                    <col width="320px" />
                                    <tr>
                                        <td><div>Schematic Library</div></td>
                                        <td><div>Schematic Symbol</div></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="SelectSchematic" runat="server"  DataTextField="LibName" 
                                                DataValueField="LibName" AutoPostBack="True"></asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="LibraryRef" runat="server"  DataTextField="LibRef" 
                                                DataValueField="LibRef"></asp:DropDownList>
                                        </td>
                                    </tr>
                                    </table>
                                </dx:PanelContent>
                            </PanelCollection>
                        </dxrp:ASPxRoundPanel>
                        <br />
                        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel5" runat="server" Width="730px" HeaderText="Altium PCB Footprints">
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent6" runat="server">
                                    <table width="650px" style="table-layout:fixed">
                                        <col width="300px" />
                                        <col width="320px" />
                                        <tr>
                                            <td><div>Footprint Library</div></td>
                                            <td><div>Footprint Assignment</div></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="SelectFootprint" runat="server" DataTextField="LibName" DataValueField="LibName" AutoPostBack="True"></asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="FootprintRef" runat="server"  DataTextField="LibRef" 
                                                    DataValueField="LibRef"></asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>   
                                </dx:PanelContent>
                            </PanelCollection>
                        </dxrp:ASPxRoundPanel>               
                </asp:Panel>
                </dxw:ContentControl></ContentCollection>
                </dxtc:TabPage>
                












                <dxtc:TabPage Text="STEP 5. SOURCING">
                <ContentCollection><dxw:ContentControl runat="server">
                <asp:Panel ID="Panel2" runat="server" style="color:black">
                        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel6" runat="server" Width="700px" HeaderText="Distributor">
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent7" runat="server">
                                    Choose Dist:
                                    <asp:DropDownList ID="SelectDist" runat="server" 
                                        DataTextField="distName" 
                                        DataValueField="distName">
                                    </asp:DropDownList>                                    
                                    OR Enter Dist: 
                                    <asp:TextBox ID="ManualEnterDist" runat="server"></asp:TextBox>
                                    <br />
                                    Dist Part #:
                                    <asp:TextBox ID="DistPartNum" runat="server"></asp:TextBox>
                                </dx:PanelContent>
                            </PanelCollection>
                        </dxrp:ASPxRoundPanel>                           
                        <br />
                        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel3" runat="server" Width="700px" HeaderText="Local Inventory">
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent4" runat="server">
                                    Choose Bin:
                                    <asp:DropDownList ID="SelectBin" runat="server" 
                                        DataSourceID="MaxwellPartsDB3" DataTextField="BinLocation" 
                                        DataValueField="BinLocation">
                                    </asp:DropDownList>
                                    <asp:SqlDataSource ID="MaxwellPartsDB3" runat="server" 
                                        ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
                                        
                                        SelectCommand="SELECT [BinLocation] FROM [inv-Bins] ORDER BY [BinLocation]">
                                    </asp:SqlDataSource>
                                    OR Enter Bin: 
                                    <asp:TextBox ID="ManualEnterBin" runat="server"></asp:TextBox>
                                    <br />
                                    Qty:
                                    <asp:TextBox ID="Qty" runat="server"></asp:TextBox>
                                </dx:PanelContent>
                            </PanelCollection>
                        </dxrp:ASPxRoundPanel>                       
                </asp:Panel>
                </dxw:ContentControl></ContentCollection>
                </dxtc:TabPage>
            </TabPages>
            <Paddings PaddingLeft="0px" />
        </dxtc:ASPxPageControl>
        
        <!-- SUBMIT / VALIDATION BOX -->
        <div style="position:absolute; right:0px; top:35px;">
            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="200px" 
                HeaderText="Submit New Part" BackColor="#FFFFCC">
                <PanelCollection>
                    <dx:PanelContent ID="PanelContent10" runat="server">
                        <table>
                            <tr>
                                <td>
                                    <dxe:ASPxButton ID="EnterThePart" runat="server" Text="" 
                                        Image-Url="../Images/Parts/submit_button_overlay.gif"
                                        BackColor="#FFFFCC" 
                                        Height="58px" Width="50px">
                                        <ClientSideEvents Click="function(s, e) {Callback.PerformCallback(); LoadingPanel.Show();}" />
                                        <Image Url="../Images/Parts/submit_button_overlay.gif"></Image>
                                        <HoverStyle>
                                            <BackgroundImage HorizontalPosition="center" 
                                                ImageUrl="./Images/Parts/submit_button_pressed.gif" Repeat="NoRepeat" 
                                                VerticalPosition="center" />
                                        </HoverStyle>
                                        <BackgroundImage HorizontalPosition="center" 
                                            ImageUrl="../Images/Parts/submit_button.gif" Repeat="NoRepeat" 
                                            VerticalPosition="center" />
                                        <Border BorderStyle="None" />
                                    </dxe:ASPxButton>
                                </td>
                                <td style="vertical-align:middle;">
                                    &lt -- Click Here!
                                </td>
                            </tr>
                        </table>
                        <br />
                        <dxwgv:ASPxGridView ID="xGridSubmit" runat="server" 
                            CssFilePath="~/App_Themes/RedWine/{0}/styles.css" CssPostfix="RedWine" 
                            ClientInstanceName="xGridSubmit" Visible="false">
                            <ClientSideEvents FocusedRowChanged="function(s, e) {xGridSubmit_Clicked_Part1();}" />
                            <Columns>
                                <dxwgv:GridViewDataTextColumn Caption="UID" FieldName="UID" ReadOnly="True" Visible="false" VisibleIndex="1">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn Caption="Step" FieldName="Step" ReadOnly="True" Visible="false" VisibleIndex="2">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn Caption="Code" FieldName="Code" ReadOnly="True" Visible="false" VisibleIndex="3">
                                </dxwgv:GridViewDataTextColumn>
                                <dxwgv:GridViewDataTextColumn Caption="Error(s)" FieldName="Error" ReadOnly="True" Visible="true" VisibleIndex="4">
                                </dxwgv:GridViewDataTextColumn>
                            </Columns>
                            <SettingsBehavior AllowFocusedRow="true" />
                            <SettingsLoadingPanel ImagePosition="Top" />
                            <Images SpriteCssFilePath="~/App_Themes/RedWine/{0}/sprite.css">
                                <LoadingPanelOnStatusBar Url="~/App_Themes/RedWine/GridView/gvLoadingOnStatusBar.gif">
                                </LoadingPanelOnStatusBar>
                                <LoadingPanel Url="~/App_Themes/RedWine/GridView/Loading.gif">
                                </LoadingPanel>
                            </Images>
                            <ImagesEditors>
                                <DropDownEditDropDown>
                                    <SpriteProperties HottrackedCssClass="dxEditors_edtDropDownHover_RedWine" />
                                </DropDownEditDropDown>
                            </ImagesEditors>
                            <ImagesFilterControl>
                                <LoadingPanel Url="~/App_Themes/RedWine/Editors/Loading.gif">
                                </LoadingPanel>
                            </ImagesFilterControl>
                            <Styles CssFilePath="~/App_Themes/RedWine/{0}/styles.css" CssPostfix="RedWine">
                                <Cell Font-Size="8pt" HorizontalAlign="Justify">
                                </Cell>
                                <LoadingPanel ImageSpacing="8px">
                                </LoadingPanel>
                            </Styles>
                            <StylesEditors>
                                <CalendarHeader Spacing="1px">
                                </CalendarHeader>
                                <ProgressBar Height="25px">
                                </ProgressBar>
                            </StylesEditors>
                        </dxwgv:ASPxGridView>
                    </dx:PanelContent>
                </PanelCollection>
            </dxrp:ASPxRoundPanel>
        </div>  
                      
    </div> <!-- ALL CONTENT GOES IN THIS DIV -->
    
</asp:Content>

