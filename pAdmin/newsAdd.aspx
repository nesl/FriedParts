<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="newsAdd.aspx.vb" Inherits="pAdmin_newsAdd" title="Add News Item" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>
<%@ Register assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dxe" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
    <script type="text/JavaScript" src="http://friedparts.nesl.ucla.edu/friedparts/FP_Code/fpCopyToClipboard.js"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="Wrapper">
    <div id="Left" style="float:left">
        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="100%"
            HeaderText="ADD or EDIT a News Story">
        <PanelCollection>
        <dx:PanelContent>
            <dxwgv:ASPxGridView ID="NewsGV" runat="server" AutoGenerateColumns="False"
                DataSourceID="fpSqlNews" KeyFieldName="NewsID" Width="532px">
                <GroupSummary>
                    <dxwgv:ASPxSummaryItem ShowInColumn="news Story" />
                </GroupSummary>
                <Columns>
                    <dxwgv:GridViewCommandColumn VisibleIndex="0">
                        <NewButton Visible="True">
                        </NewButton>
                        <EditButton Visible="True">
                        </EditButton>
                        <DeleteButton Visible="True">
                        </DeleteButton>
                        <ClearFilterButton Visible="True">
                        </ClearFilterButton>
                    </dxwgv:GridViewCommandColumn>
                    <dxwgv:GridViewDataTextColumn FieldName="NewsID" ReadOnly="True" 
                        VisibleIndex="1">
                        <EditFormSettings Visible="False" />
                    </dxwgv:GridViewDataTextColumn>
                    <dxwgv:GridViewDataMemoColumn FieldName="newsStory" Visible="false" VisibleIndex="2">
                      <EditFormSettings RowSpan="10" Visible="True" VisibleIndex="7" ColumnSpan="3" CaptionLocation="Near"/>                      
                    </dxwgv:GridViewDataMemoColumn>
                    <dxwgv:GridViewDataDateColumn FieldName="newsDate" VisibleIndex="3">
                        <EditFormSettings ColumnSpan="1" />
                    </dxwgv:GridViewDataDateColumn>
                    <dxwgv:GridViewDataTextColumn FieldName="UserID" VisibleIndex="4">
                    </dxwgv:GridViewDataTextColumn>
                    <dxwgv:GridViewDataTextColumn FieldName="newsTitle" VisibleIndex="5">
                        <EditFormSettings ColumnSpan="2" />
                    </dxwgv:GridViewDataTextColumn>
                    <dxwgv:GridViewDataTextColumn FieldName="newsImageURL" VisibleIndex="6">
                        <EditFormSettings ColumnSpan="3" CaptionLocation="Near" />                        
                    </dxwgv:GridViewDataTextColumn>
                </Columns>
                <SettingsBehavior AllowFocusedRow="True" ConfirmDelete="True" />
                <SettingsEditing Mode="EditForm" EditFormColumnCount="3" />
                <Settings ShowFilterRow="True" ShowGroupedColumns="True" ShowGroupPanel="True" 
                    ShowPreview="True" />
                <Templates>
                    <DetailRow>
                         <%# Eval("newsStory") %>
                    </DetailRow>
                </Templates>
                <SettingsDetail ShowDetailRow="true" />                   
            </dxwgv:ASPxGridView>
            
            <asp:SqlDataSource ID="fpSqlNews" runat="server" 
                ConnectionString="<%$ ConnectionStrings:FriedPartsConnectionString %>" 
                SelectCommand="SELECT * FROM [msg-News]"
                UpdateCommand="UPDATE [msg-News] SET [newsStory] = @newsStory, [newsDate] = @newsDate, [UserID] = @UserID, [newsTitle] = @newsTitle, [newsImageURL] = @newsImageURL WHERE [NewsID] = @NewsID" 
                DeleteCommand="DELETE FROM [msg-News] WHERE [NewsID] = @NewsID" 
                InsertCommand="INSERT INTO [msg-News] ([newsStory], [newsDate], [UserID], [newsTitle], [newsImageURL]) VALUES (@newsStory, @newsDate, @UserID, @newsTitle, @newsImageURL)"
                >
                <DeleteParameters>
                    <asp:Parameter Name="NewsID" Type="Int32" />
                </DeleteParameters>
                <InsertParameters>
                    <asp:Parameter Name="newsStory" Type="String" />
                    <asp:Parameter DbType="DateTime" Name="newsDate" />
                    <asp:Parameter Name="UserID" Type="Int32" />
                    <asp:Parameter Name="newsTitle" Type="String" />
                    <asp:Parameter Name="newsImageURL" Type="String" />
                </InsertParameters>
                <UpdateParameters>
                    <asp:Parameter Name="newsStory" Type="String" />
                    <asp:Parameter DbType="DateTime" Name="newsDate" />
                    <asp:Parameter Name="UserID" Type="Int32" />
                    <asp:Parameter Name="newsTitle" Type="String" />
                    <asp:Parameter Name="newsImageURL" Type="String" />
                    <asp:Parameter Name="NewsID" Type="Int32" />
                </UpdateParameters>
            </asp:SqlDataSource>
        </dx:PanelContent>
        </PanelCollection>
        </dxrp:ASPxRoundPanel>
        <br /><br />
        <dxrp:ASPxRoundPanel ID="ASPxRoundPanel2" runat="server" Width="200px" HeaderText="News Icons">
        <PanelCollection>
        <dx:PanelContent>
            <table>
                <tr>
                    <td><img alt="News Message" src="../Images/News/Message.png" /></td>
                    <td style="font-size:6pt">http://friedparts.nesl.ucla.edu/friedparts/Images/News/Message.png</td>
                    <td><img alt="News New" src="../Images/News/New.png" /></td>
                    <td style="font-size:6pt">http://friedparts.nesl.ucla.edu/friedparts/Images/News/New.png</td>
                </tr>
               <tr>
                    <td><img alt="News Security" src="../Images/News/Security.png" /></td>
                    <td style="font-size:6pt">http://friedparts.nesl.ucla.edu/friedparts/Images/News/Security.png</td>
                    <td><img alt="News Message" src="../Images/News/Search.png" /></td>
                    <td style="font-size:6pt">http://friedparts.nesl.ucla.edu/friedparts/Images/News/Search.png</td>
                </tr>
                <tr>
                    <td><img alt="News Tools" src="../Images/News/Tools.png" /></td>
                    <td style="font-size:6pt">http://friedparts.nesl.ucla.edu/friedparts/Images/News/Tools.png</td>
                </tr>
            </table>
        </dx:PanelContent>
        </PanelCollection>
        </dxrp:ASPxRoundPanel>
    </div> <!-- Left Column -->
    </div> <!-- Wrapper -->
</asp:Content>

