<%@ Page Language="VB" Debug="true" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="bomAddNew.aspx.vb" Inherits="pBOM_bomAddNew" title="Add New Project" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxUploadControl" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>
<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxTabControl" tagprefix="dxtc" %>
<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxClasses" tagprefix="dxw" %>
<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxClasses" tagprefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dxp" %>
<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxLoadingPanel" TagPrefix="dxlp" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxCallback" TagPrefix="dxcb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <!-- LOADING PANEL MECHANISM -->
        <dxcb:ASPxCallback ID="ASPxCallback1" runat="server" ClientInstanceName="Callback">
            <ClientSideEvents CallbackComplete="function(s, e) { LoadingPanel.Hide(); }" />
        </dxcb:ASPxCallback>
        <dxlp:ASPxLoadingPanel ID="LoadingPanel" runat="server" ClientInstanceName="LoadingPanel" Modal="True"></dxlp:ASPxLoadingPanel>

    <!-- OVERALL CONTENT CONTAINER -->    
    <div style="position:relative">
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
            ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
            SelectCommand="SELECT [ProjectID], [Title], [Revision], [Description] FROM [proj-Common] ORDER BY [Title], [DateCreated] DESC">
        </asp:SqlDataSource>
        
    <!-- TAB PAGES -->
    <dxtc:ASPxPageControl ID="xTabPageControl" runat="server" ActiveTabIndex="3" 
        CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
        TabSpacing="0px" Width="750px">
        <LoadingPanelImage Url="~/App_Themes/Glass/Web/Loading.gif">
        </LoadingPanelImage>
        <ContentStyle>
            <Border BorderColor="#4986A2" />
        </ContentStyle>
        <TabPages>
            <dxtc:TabPage Text="Project Details">
                <ContentCollection>
                    <dx:ContentControl ID="ContentControl1" runat="server">
                        <div style="float:left">
                            <!-- CREATE OR UPDATE SELECTOR -->
                            <dxtc:ASPxPageControl ID="xtabProjectType" runat="server" ActiveTabIndex="1" 
                                CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
                                TabSpacing="0px" Width="600px">
                                <LoadingPanelImage Url="~/App_Themes/Glass/Web/Loading.gif">
                                </LoadingPanelImage>
                                <ContentStyle>
                                    <Border BorderColor="#4986A2" />
                                </ContentStyle>
                                <TabPages>
                                    <dxtc:TabPage Text="Create a New Project">
                                        <ContentCollection>
                                            <dx:ContentControl ID="ContentControl3" runat="server">
                                                <!-- CREATE NEW! -->
                                                <dxrp:ASPxRoundPanel ID="xrpNewProject" runat="server" Width="400px" HeaderText="Create a New Project">
                                                <PanelCollection>
                                                    <dxp:PanelContent id="Panel1" runat="server">
                                                        <table>
                                                            <tr>
                                                                <td class="fpTableLabel"><div>Project Title:</div></td>
                                                                <td><asp:TextBox ID="txtTitle" runat="server"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="fpTableLabelMultiline"><div>Project Description:</div></td>
                                                                <td><asp:TextBox ID="txtDesc" runat="server" TextMode="MultiLine" Wrap="True" Rows="5"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="fpTableLabel"><div>Revision:</div></td>
                                                                <td><asp:TextBox ID="txtRevision" runat="server"></asp:TextBox></td>
                                                            </tr>
                                                        </table>
                                                    </dxp:PanelContent>
                                                </PanelCollection>
                                                </dxrp:ASPxRoundPanel>
                                            </dx:ContentControl>
                                        </ContentCollection>
                                    </dxtc:TabPage>
                                    <dxtc:TabPage Text="Create a New Project Revision">
                                        <ContentCollection>
                                            <dx:ContentControl ID="ContentControl4" runat="server">
                                                <!-- CREATE NEW REVISION -->
                                                <dxrp:ASPxRoundPanel ID="xrpUpdateProject" runat="server" Width="200px" HeaderText="Update an Existing Project">
                                                <PanelCollection>
                                                    <dxp:PanelContent id="PanelContent1" runat="server">
                                                        <table>
                                                            <tr>
                                                                <td class="fpTableLabelMultiline"><div>Select:</div></td>
                                                                <td>
                                                                    <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" 
                                                                        CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
                                                                        DataSourceID="SqlDataSource1" KeyFieldName="ProjectID">
                                                                        <Images SpriteCssFilePath="~/App_Themes/Glass/{0}/sprite.css">
                                                                            <LoadingPanelOnStatusBar Url="~/App_Themes/Glass/GridView/gvLoadingOnStatusBar.gif">
                                                                            </LoadingPanelOnStatusBar>
                                                                            <LoadingPanel Url="~/App_Themes/Glass/GridView/Loading.gif">
                                                                            </LoadingPanel>
                                                                        </Images>
                                                                        <Columns>
                                                                            <dx:GridViewDataTextColumn FieldName="ProjectID" ReadOnly="True" 
                                                                                VisibleIndex="0" Width="50px">
                                                                                <EditFormSettings Visible="False" />
                                                                                <CellStyle HorizontalAlign="Center">
                                                                                </CellStyle>
                                                                            </dx:GridViewDataTextColumn>
                                                                            <dx:GridViewDataTextColumn FieldName="Title" VisibleIndex="1" Width="150px">
                                                                            </dx:GridViewDataTextColumn>
                                                                            <dx:GridViewDataTextColumn FieldName="Revision" VisibleIndex="2" Width="30px">
                                                                            </dx:GridViewDataTextColumn>
                                                                            <dx:GridViewDataTextColumn FieldName="Description" VisibleIndex="3" 
                                                                                Width="300px">
                                                                            </dx:GridViewDataTextColumn>
                                                                        </Columns>
                                                                        <Settings ShowFilterRow="True" />
                                                                        <ImagesFilterControl>
                                                                            <LoadingPanel Url="~/App_Themes/Glass/Editors/Loading.gif">
                                                                            </LoadingPanel>
                                                                        </ImagesFilterControl>
                                                                        <Styles CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass">
                                                                            <Header ImageSpacing="5px" SortingImageSpacing="5px">
                                                                            </Header>
                                                                        </Styles>
                                                                        <StylesEditors>
                                                                            <CalendarHeader Spacing="1px">
                                                                            </CalendarHeader>
                                                                            <ProgressBar Height="25px">
                                                                            </ProgressBar>
                                                                        </StylesEditors>
                                                                    </dx:ASPxGridView>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="fpTableLabel"><div>New Revision:</div></td>
                                                                <td><asp:TextBox ID="txtNextRevision" runat="server"></asp:TextBox></td>
                                                            </tr>
                                                        </table>
                                                    </dxp:PanelContent>
                                                </PanelCollection>
                                                </dxrp:ASPxRoundPanel>
                                            </dx:ContentControl>
                                        </ContentCollection>
                                    </dxtc:TabPage>
                                    <dxtc:TabPage Text="Update an Existing Project">
                                        <ContentCollection>
                                            <dx:ContentControl ID="ContentControl5" runat="server">
                                                <!-- UPDATE EXISTING -->
                                                <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="200px" HeaderText="Update an Existing Project">
                                                <PanelCollection>
                                                    <dxp:PanelContent id="PanelContent2" runat="server">
                                                        <table>
                                                            <tr>
                                                                <td class="fpTableLabelMultiline"><div>Select:</div></td>
                                                                <td>
                                                                    <dx:ASPxGridView ID="ASPxGridView2" runat="server" AutoGenerateColumns="False" 
                                                                        CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
                                                                        DataSourceID="SqlDataSource1" KeyFieldName="ProjectID">
                                                                        <Images SpriteCssFilePath="~/App_Themes/Glass/{0}/sprite.css">
                                                                            <LoadingPanelOnStatusBar Url="~/App_Themes/Glass/GridView/gvLoadingOnStatusBar.gif">
                                                                            </LoadingPanelOnStatusBar>
                                                                            <LoadingPanel Url="~/App_Themes/Glass/GridView/Loading.gif">
                                                                            </LoadingPanel>
                                                                        </Images>
                                                                        <Columns>
                                                                            <dx:GridViewDataTextColumn FieldName="ProjectID" ReadOnly="True" 
                                                                                VisibleIndex="0" Width="50px">
                                                                                <EditFormSettings Visible="False" />
                                                                                <CellStyle HorizontalAlign="Center">
                                                                                </CellStyle>
                                                                            </dx:GridViewDataTextColumn>
                                                                            <dx:GridViewDataTextColumn FieldName="Title" VisibleIndex="1" Width="150px">
                                                                            </dx:GridViewDataTextColumn>
                                                                            <dx:GridViewDataTextColumn FieldName="Revision" VisibleIndex="2" Width="30px">
                                                                            </dx:GridViewDataTextColumn>
                                                                            <dx:GridViewDataTextColumn FieldName="Description" VisibleIndex="3" 
                                                                                Width="300px">
                                                                            </dx:GridViewDataTextColumn>
                                                                        </Columns>
                                                                        <Settings ShowFilterRow="True" />
                                                                        <ImagesFilterControl>
                                                                            <LoadingPanel Url="~/App_Themes/Glass/Editors/Loading.gif">
                                                                            </LoadingPanel>
                                                                        </ImagesFilterControl>
                                                                        <Styles CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass">
                                                                            <Header ImageSpacing="5px" SortingImageSpacing="5px">
                                                                            </Header>
                                                                        </Styles>
                                                                        <StylesEditors>
                                                                            <CalendarHeader Spacing="1px">
                                                                            </CalendarHeader>
                                                                            <ProgressBar Height="25px">
                                                                            </ProgressBar>
                                                                        </StylesEditors>
                                                                    </dx:ASPxGridView>
                                                                </td>
                                                            </tr>                                                        </table>
                                                    </dxp:PanelContent>
                                                </PanelCollection>
                                                </dxrp:ASPxRoundPanel>
                                            </dx:ContentControl>
                                        </ContentCollection>
                                    </dxtc:TabPage>
                                </TabPages>
                            </dxtc:ASPxPageControl>
                            
                            
                        
                            
                        </div>
                    </dx:ContentControl>
                </ContentCollection>
            </dxtc:TabPage>
            <dxtc:TabPage Text="CADD Import">
                <ContentCollection>
                    <dx:ContentControl runat="server">
                        <table>
                            <tr>
                                <td class="fpTableLabel">Import your Altium XML BOM File: </td>
                                <td>
                                    <dx:ASPxUploadControl ID="xBomUpload" runat="server">
                                    </dx:ASPxUploadControl>
                                </td>    
                            </tr>
                            <tr>
                                <td></td>
                                <td>                        
                                    <asp:Button ID="btnImport" runat="server" Text="Import!" />
                                </td>
                            </tr>
                        </table>
                        <div id="importErrors" runat="server" visible="false" class="fpErrorBox">
                            <table>
                                <tr>
                                <td><img src="../Images/System/warning.gif" /></td>
                                <td><asp:Label ID="lblErrors" runat="server" Text=""></asp:Label></td>
                                </tr>
                            </table>
                        </div>
                        <div id="importOK" runat="server" visible="false" class="fpHighlightBox">
                            <table>
                                <tr>
                                <td><img src="../Images/System/check.gif" /></td>
                                <td><asp:Label ID="lblOK" runat="server" Text=""></asp:Label></td>
                                </tr>   
                            </table>
                        </div>
                        <br /><hr /><br />
                        <table>
                            <tr class="fpTableLabel">
                                <td class="fpInstTableLabel"></td>
                                <td>
                                    <div style="border:solid 1px grey; padding:3px;">
                                        YOU MUST PREPARE YOUR BILL OF MATERIALS ACCORDING TO THE FOLLOWING PROCEDURE! <br />
                                        (Altium will remember these settings on a per project basis so you should not 
                                        have to do this more than once per project.)
                                    </div>
                                </td>
                            </tr>
                            <tr><td></td><td>&nbsp</td></tr>
                            <tr class="fpInstTableLabel">
                                <td class="fpInstTableLabel">STEP 1.</td>
                                <td>With a schematic document active, select Reports -> Bill of Materials</td>
                            </tr>
                            <tr>
                                <td class="fpInstTableLabel"></td>
                                <td class="fpInstTableContent"><img class="fp" src="../Images/Projects/Altium-Bom-1.gif" alt="Step 1." /></td>
                            </tr>
                            <tr class="fpInstTableLabel">
                                <td class="fpInstTableLabel">STEP 2.</td>
                                <td>Check 'Show' for the Footprint field. Drag all of the fields in the 'Grouped Columns' box to the 'All Columns' box below it. Drag the 'LibRef' field from the 'All Columns' box to the 'Grouped Columns' box above it.</td>
                            </tr>
                            <tr>
                                <td class="fpInstTableLabel"></td>
                                <td class="fpInstTableContent"><img class="fp" src="../Images/Projects/Altium-Bom-2.gif" alt="Step 2." /></td>
                            </tr>
                            <tr class="fpInstTableLabel">
                                <td class="fpInstTableLabel">STEP 3.</td>
                                <td>
                                    Check 'Show' for the LibRef field and for the Description field.<br />
                                    There should be <b><i>six</i></b> fields marked 'Show' in the 'All Columns' listing:<br />
                                    <span style="font-style:italic;">Comment, Description, Designator, Footprint, LibRef, Quantity</span>
                                </td>
                            </tr>
                            <tr>
                                <td class="fpInstTableLabel"></td>
                                <td class="fpInstTableContent"><img class="fp" src="../Images/Projects/Altium-Bom-3.gif" alt="Step 3." /></td>
                            </tr>
                            <tr class="fpInstTableLabel">
                                <td class="fpInstTableLabel">STEP 4.</td>
                                <td>Under 'Export Options', change the File Format to 'XML Spreadsheet'. 
                                    Click the 'Export...' button to save the file and then click the 
                                    'Choose File' button above and select your new XML file. Click 'Import!' 
                                    to complete the upload.</td>
                            </tr>
                            <tr>
                                <td class="fpTableLabel"></td>
                                <td class="fpInstTableContent"><img class="fp" src="../Images/Projects/Altium-Bom-4.gif" alt="Step 4." /></td>
                            </tr>
                        </table>
                    </dx:ContentControl>
                </ContentCollection>
            </dxtc:TabPage>
            <dxtc:TabPage Text="Review Import">
                <ContentCollection>
                    <dx:ContentControl runat="server">
                        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel4" runat="server" Width="100%" HeaderText="Please confirm your import">
                        <PanelCollection>
                            <dxp:PanelContent id="PanelContent4" runat="server">
                                <div>
                                    <ul>
                                    <li> VERIFY that each of the parts marked as a FriedPart is indeed intended.</li>
                                    <li> BakedParts are imported parts (included in the BOM) which are NOT FriedParts (Mount holes, test points, etc)</li>
                                    </ul>
                                </div>
                            </dxp:PanelContent>
                        </PanelCollection>
                        </dxrp:ASPxRoundPanel>
                        <br /><br />
                        <dx:ASPxGridView ID="xGridReview" runat="server" 
                            CssFilePath="~/App_Themes/Glass/{0}/styles.css" 
                            CssPostfix="Glass" Settings-ShowGroupPanel="true"
                            KeyFieldName="UID"
                            EnableCallBacks="False">
                            <Images SpriteCssFilePath="~/App_Themes/Glass/{0}/sprite.css">
                                <LoadingPanelOnStatusBar Url="~/App_Themes/Glass/GridView/gvLoadingOnStatusBar.gif">
                                </LoadingPanelOnStatusBar>
                                <LoadingPanel Url="~/App_Themes/Glass/GridView/Loading.gif">
                                </LoadingPanel>
                            </Images>
                            <Columns>
                                <dx:GridViewCommandColumn VisibleIndex="0">
                                    <CustomButtons>
                                        <dxwgv:GridViewCommandColumnCustomButton Text="Fried/Baked" ID="GridViewCommandColumnCustomButton1">
                                        </dxwgv:GridViewCommandColumnCustomButton>
                                    </CustomButtons>
                                </dx:GridViewCommandColumn>
                                <dx:GridViewDataTextColumn FieldName="UID" VisibleIndex="1" Visible="false">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataCheckColumn FieldName="isFP" Caption="FriedPart?" VisibleIndex="2">
                                </dx:GridViewDataCheckColumn>
                                <dx:GridViewDataTextColumn FieldName="PartID" VisibleIndex="3">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Designator" Caption="RefDes" VisibleIndex="4">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Value" VisibleIndex="5">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Description" VisibleIndex="6">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Footprint" VisibleIndex="7">
                                </dx:GridViewDataTextColumn>                                
                            </Columns>
                            <Settings ShowGroupPanel="False"></Settings>
                            <ImagesFilterControl>
                                <LoadingPanel Url="~/App_Themes/Glass/Editors/Loading.gif">
                                </LoadingPanel>
                            </ImagesFilterControl>
                            <Styles CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass">
                                <Header ImageSpacing="5px" SortingImageSpacing="5px">
                                </Header>
                            </Styles>
                            <StylesEditors>
                                <CalendarHeader Spacing="1px">
                                </CalendarHeader>
                                <ProgressBar Height="25px">
                                </ProgressBar>
                            </StylesEditors>
                        </dx:ASPxGridView>
                    </dx:ContentControl>
                </ContentCollection>
            </dxtc:TabPage>
            <dxtc:TabPage Text="Mechanical">
                <ContentCollection>
                    <dx:ContentControl runat="server">
                        <div>
                            <table>
                            <colgroup>
                                <col width="90" />
                                <col width="120" />
                                <col width="0*" />
                            </colgroup>
                                <tr>
                                    <td colspan="3">
                                        Add mechanical parts and any parts that are not directly soldered to the PCB here:
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        ...such as the enclosure, screws, standoffs, fuses, socket parts, etc...
                                    </td>
                                </tr>

                                <tr><td colspan="3"><hr /></td></tr>
                                <tr>
                                    <td class="fpTableLabel">FriedParts' PartID:</td>
                                    <td><asp:TextBox ID="txtBomAdd" runat="server" Width="100px" AutoPostBack="true"></asp:TextBox></td>
                                    <td><div runat="server" id="validBomAddID" visible="false"></div></td>
                                </tr>
                                <tr>
                                    <td class="fpTableLabel">Quantity:</td>
                                    <td><asp:TextBox ID="txtBomAddQty" runat="server" Width="100px" AutoPostBack="true"></asp:TextBox></td>
                                    <td><div runat="server" id="validBomAddQty" visible="false"></div></td>
                                </tr>
                                <tr>
                                    <td class="fpTableLabel"></td>
                                    <td><asp:Button ID="btnBomAdd" runat="server" Text="Add Part" /></td>
                                    <td></td>
                                </tr>
                                <tr><td>&nbsp</td></tr>
                            </table>
                        </div>
                        <dx:ASPxGridView ID="xGridBomAdditional" runat="server" 
                            CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
                            AutoGenerateColumns="False"
                            KeyFieldName="PartID">
                            <Images SpriteCssFilePath="~/App_Themes/Glass/{0}/sprite.css">
                                <LoadingPanelOnStatusBar Url="~/App_Themes/Glass/GridView/gvLoadingOnStatusBar.gif">
                                </LoadingPanelOnStatusBar>
                                <LoadingPanel Url="~/App_Themes/Glass/GridView/Loading.gif">
                                </LoadingPanel>
                            </Images>
                            <Columns>
                                <dx:GridViewCommandColumn VisibleIndex="0">
                                    <CustomButtons>
                                        <dxwgv:GridViewCommandColumnCustomButton Text="Delete" ID="del">
                                        </dxwgv:GridViewCommandColumnCustomButton>
                                    </CustomButtons>
                                </dx:GridViewCommandColumn>
                                <dx:GridViewDataTextColumn FieldName="Qty" Caption="Quantity" VisibleIndex="1">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="PartID" Caption="FriedParts ID" VisibleIndex="2">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Type" Caption="Type of Part" VisibleIndex="3">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Mfr" Caption="Manufacturer" VisibleIndex="4">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="MPN" Caption="Part Number" VisibleIndex="5">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Desc" Caption="Description" VisibleIndex="6">
                                </dx:GridViewDataTextColumn>
                            </Columns>
                            <SettingsBehavior AllowFocusedRow="True" />
                            <Settings ShowFilterRow="True" />
                            <ImagesFilterControl>
                                <LoadingPanel Url="~/App_Themes/Glass/Editors/Loading.gif">
                                </LoadingPanel>
                            </ImagesFilterControl>
                            <Styles CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass">
                                <Header ImageSpacing="5px" SortingImageSpacing="5px">
                                </Header>
                            </Styles>
                            <StylesEditors>
                                <CalendarHeader Spacing="1px">
                                </CalendarHeader>
                                <ProgressBar Height="25px">
                                </ProgressBar>
                            </StylesEditors>
                        </dx:ASPxGridView>
                        <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
                            SelectCommand="SELECT [ProjectID], [Title], [Revision], [DateCreated] FROM [proj-Common]">
                        </asp:SqlDataSource>
                    </dx:ContentControl>
                </ContentCollection>
            </dxtc:TabPage>
            
            
            
            
            
            <dxtc:TabPage Text="Exclusions (DNS/DNP)">
                <ContentCollection>
                    <dx:ContentControl runat="server">
                        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel2" runat="server" Width="100%" HeaderText="Exclude Specific Parts From Assembly">
                        <PanelCollection>
                            <dxp:PanelContent id="PanelContent3" runat="server">
                                <div>
                                    <ul>
                                    <li> This means that the factory will be explicitly instructed not to solder components to the PCB at these locations. </li>
                                    <li> RefDes exclusions are done on an instance, rather than component, level basis. You may have R1 marked DNP and R2 left included, even if R1 and R2 are use the same part number.</li>
                                    <li> Baked Parts (imported non-FriedParts) are automatically maked for exclusion</li>
                                    <li> Use the filter function of the table to find Reference Designators more efficiently. The percent character is the wildcard. Example, R%, matches R1, R2, etc... </li>
                                    </ul>
                                    <asp:Label ID="lblDnpCount" runat="server" Text="The following Reference Designators (RefDes) have been marked DO NOT POPULATE (DNP):"></asp:Label>
                                </div>
                                <div style="margin-left:20px; margin-top:10px">
                                    <asp:Label ID="lblDnpDescription" runat="server" Text=""></asp:Label>
                                </div>
                            </dxp:PanelContent>
                        </PanelCollection>
                        </dxrp:ASPxRoundPanel>
                        <br /><br />
                        <dx:ASPxGridView ID="xGridExclusions" runat="server" 
                            CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass" 
                            AutoGenerateColumns="False"
                            KeyFieldName="Designator"
                            EnableCallBacks="False">
                            <Images SpriteCssFilePath="~/App_Themes/Glass/{0}/sprite.css">
                                <LoadingPanelOnStatusBar Url="~/App_Themes/Glass/GridView/gvLoadingOnStatusBar.gif">
                                </LoadingPanelOnStatusBar>
                                <LoadingPanel Url="~/App_Themes/Glass/GridView/Loading.gif">
                                </LoadingPanel>
                            </Images>
                            <Columns>
                                <dx:GridViewCommandColumn VisibleIndex="0">
                                    <CustomButtons>
                                        <dxwgv:GridViewCommandColumnCustomButton Text="Toggle Exclusion" ID="GridViewCommandColumnCustomButton2">
                                        </dxwgv:GridViewCommandColumnCustomButton>
                                    </CustomButtons>
                                </dx:GridViewCommandColumn>
                                <dx:GridViewDataCheckColumn FieldName="DoNotPopulate" Caption="Exclude From BOM" VisibleIndex="1">
                                </dx:GridViewDataCheckColumn>
                                <dx:GridViewDataCheckColumn FieldName="isFP" Caption="FriedPart?" VisibleIndex="2">
                                </dx:GridViewDataCheckColumn>
                                <dx:GridViewDataTextColumn FieldName="PartID" GroupIndex="0" SortIndex="0" SortOrder="Ascending" VisibleIndex="3">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Designator" Caption="RefDes" VisibleIndex="4">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Value" VisibleIndex="5">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Description" VisibleIndex="6">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Footprint" VisibleIndex="7">
                                </dx:GridViewDataTextColumn>           
                                <dx:GridViewDataTextColumn FieldName="UID" VisibleIndex="8" Visible="false">
                                </dx:GridViewDataTextColumn>                     
                            </Columns>
                            <SettingsBehavior AllowFocusedRow="True" />
                            <Settings ShowFilterRow="True" ShowGroupPanel="False" />
                            <ImagesFilterControl>
                                <LoadingPanel Url="~/App_Themes/Glass/Editors/Loading.gif">
                                </LoadingPanel>
                            </ImagesFilterControl>
                            <Styles CssFilePath="~/App_Themes/Glass/{0}/styles.css" CssPostfix="Glass">
                                <Header ImageSpacing="5px" SortingImageSpacing="5px">
                                </Header>
                            </Styles>
                            <StylesEditors>
                                <CalendarHeader Spacing="1px">
                                </CalendarHeader>
                                <ProgressBar Height="25px">
                                </ProgressBar>
                            </StylesEditors>
                        </dx:ASPxGridView>
                    </dx:ContentControl>
                </ContentCollection>
            </dxtc:TabPage>
            <dxtc:TabPage Text="File Upload">
                <ContentCollection>
                    <dx:ContentControl ID="ContentControl2" runat="server">
                        <table>
                            <tr>
                               <td class="fpTableLabel">Project Image:</td>
                               <td>
                                   <asp:FileUpload ID="PrjImage" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="fpTableLabel">PrjPcb:</td>
                               <td>
                                   <asp:FileUpload ID="PrjPcbUpload" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="fpTableLabel">SchDoc:</td>
                                <td><asp:FileUpload ID="Schdocupload" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="fpTableLabel">PcbDoc:</td>
                                <td><asp:FileUpload ID="PcbDocUpload" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="fpTableLabel">OutJob:</td>
                                <td><asp:FileUpload ID="OutJobUpload" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="fpTableLabel">TO_FAB Gerbers (zip):</td>
                                <td><asp:FileUpload ID="TOFABGerbersUpload" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="fpTableLabel">TO_ASM Gerbers (zip):</td>
                                <td><asp:FileUpload ID="TOASMGerbersUpload" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="fpTableLabel">Schematic PDF:</td>
                                <td><asp:FileUpload ID="SchPDFUpload" runat="server" /></td>
                            </tr>
                            <tr>
                                <td></td><td><hr /></td>
                            </tr>
                            <tr>
                                <td></td><td><asp:Button ID="btnUploadProject" runat="server" Text="CONFIRM & UPLOAD!" /></td>
                            </tr>
                            <asp:Label ID="DebugLabel" runat="server" Text="Label"></asp:Label>
                        </table>
                    </dx:ContentControl>
                </ContentCollection>
            </dxtc:TabPage>
        </TabPages>
        <Paddings PaddingLeft="0px" />
    </dxtc:ASPxPageControl>
    
    
    
    <!-- SUBMIT / VALIDATION BOX -->
        <div style="position:absolute; right:0px; top:35px;">
            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel3" runat="server" Width="200px" 
                HeaderText="Submit New Project" BackColor="#FFFFCC">
                <PanelCollection>
                    <dxp:PanelContent ID="PanelContent10" runat="server">
                        <table>
                            <tr>
                                <td>
                                    <dx:ASPxButton ID="btnSubmitProject" runat="server" Text="" 
                                        Image-Url="../Images/Parts/submit_button_overlay.gif"
                                        BackColor="#FFFFCC" 
                                        Height="58px" Width="50px">
                                        <ClientSideEvents Click="function(s, e) {Callback.PerformCallback(); LoadingPanel.Show();}" />
                                        <Image Url="../Images/Parts/submit_button_overlay.gif"></Image>
                                        <HoverStyle>
                                            <BackgroundImage HorizontalPosition="center" 
                                                ImageUrl="../Images/Parts/submit_button_pressed.gif" Repeat="NoRepeat" 
                                                VerticalPosition="center" />
                                        </HoverStyle>
                                        <BackgroundImage HorizontalPosition="center" 
                                            ImageUrl="../Images/Parts/submit_button.gif" Repeat="NoRepeat" 
                                            VerticalPosition="center" />
                                        <Border BorderStyle="None" />
                                    </dx:ASPxButton>
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
                    </dxp:PanelContent>
                </PanelCollection>
            </dxrp:ASPxRoundPanel>
        </div> <!-- SUBMIT PANEL -->         
               
    </div> <!-- OVERALL CONTENT CONTAINER -->

</asp:Content>

