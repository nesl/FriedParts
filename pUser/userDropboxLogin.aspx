<%@ Page Title="" Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="userDropboxLogin.aspx.vb" Inherits="pUser_userDropboxLogin" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div style="height:600px;">
        
        <!-- Link Account to Dropbox -->
        <div style="float:left;">
            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" HeaderText="Link Your Dropbox Account!" BackColor="LightYellow" Width="600px">
                <PanelCollection>
                    <dx:PanelContent>
                        <div>
                            <img src="../Images/User/Dropbox.png" alt="Dropbox Logo" />
                            <table>
                            <tr>
                                <td class="fpTableLabel">Username:</td>
                                <td><asp:TextBox ID="txtUsername" runat="server" Width="160px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="fpTableLabel">Password:</td>
                                <td><asp:TextBox ID="txtPassword" runat="server" Width="160px" TextMode="Password"></asp:TextBox></td>
                                <td class="fpTableComment">This information is used only once and never stored by FriedParts</td>
                            </tr>
                            <tr id="dev1" runat="server">
                                <td class="fpTableLabel">App Key:</td>
                                <td><asp:TextBox ID="txtAppKey" runat="server" Width="160px"></asp:TextBox></td>
                                <td class="fpTableComment">This information is NOT OPTIONAL. See below.</td>
                            </tr>
                            <tr id="dev2" runat="server">
                                <td class="fpTableLabel">App Secret:</td>
                                <td><asp:TextBox ID="txtAppSecret" runat="server" Width="160px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <asp:Button ID="btnDropboxLinkAccount" runat="server" Text="Link My Dropbox Account!" class="fpClass" /> 
                                </td>
                                <td>
                                    <div class="fpErrorBox" runat="server" id="errBox" visible="false">
                                        <asp:Label ID="lblErrMsg" runat="server" Text="Error Text"></asp:Label>
                                    </div>
                                </td>
                            </tr>
                            </table>
                        </div>
                    </dx:PanelContent>
                </PanelCollection>
            </dxrp:ASPxRoundPanel>  
        </div>

        <!-- What is Dropbox? -->
        <div style="float:left; margin-left:10px; margin-right:10px; margin-bottom:10px;">
            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel2" runat="server" HeaderText="What is Dropbox?" BackColor="LightYellow">
                <PanelCollection>
                    <dx:PanelContent>
                        <div style="border: solid 1px grey;">
                            <object width="280" height="182"><param name="movie" value="http://www.youtube-nocookie.com/v/w4eTR7tci6A?fs=1&amp;hl=en_US&amp;rel=0"></param><param name="allowFullScreen" value="true"></param><param name="allowscriptaccess" value="always"></param><embed src="http://www.youtube-nocookie.com/v/w4eTR7tci6A?fs=1&amp;hl=en_US&amp;rel=0" type="application/x-shockwave-flash" allowscriptaccess="always" allowfullscreen="true" width="280" height="182"></embed></object>
                        </div>
                        <br />
                        <a href="http://db.tt/FUCoETZ">Click here to get a free Dropbox account!</a>
                    </dx:PanelContent>
                </PanelCollection>
            </dxrp:ASPxRoundPanel>  
        </div>

        <!-- Setup Instructions -->
        <div id="dev3" runat="server" style="float:left; width:750px;">
            <dxrp:ASPxRoundPanel ID="ASPxRoundPanel3" runat="server" HeaderText="How do I get an App Key/Secret?" BackColor="LightYellow">
                <PanelCollection>
                    <dx:PanelContent>
                        <table>
                            <tr class="fpTableLabel">
                                <td class="fpInstTableLabel"></td>
                                <td>
                                    <div style="border:solid 1px grey; padding:3px;">
                                        YOU MUST PERFORM THIS PROCEDURE ONCE BEFORE YOU CAN USE YOUR DROPBOX ACCOUNT WITH FRIEDPARTS!
                                        (FriedParts will remember these settings so you will never have to do this again.)
                                    </div>
                                </td>
                            </tr>
                            <tr><td></td><td>&nbsp</td></tr>
                            <tr class="fpInstTableLabel">
                                <td class="fpInstTableLabel">STEP 1.</td>
                                <td>
                                    <ul>
                                        <li>You MUST have a Dropbox account to complete this procedure.</li>
                                        <li>If you do not have one, you can get one for free by <a href="http://db.tt/FUCoETZ">clicking here</a> (using this link will give you a slightly larger account than normal free users get).</li>
                                    </ul>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td><!-- No image for this step -->&nbsp</td>
                            </tr>
                            <tr class="fpInstTableLabel">
                                <td class="fpInstTableLabel">STEP 2.</td>
                                <td>
                                    <ol>
                                        <li>Go to <a href="https://www.dropbox.com/developers/apps">this page</a>.</li> 
                                        <li>You will be asked to login to your Dropbox account if you have not already.</li>
                                        <li>Click the 'Create an App' button.</li>
                                    </ol>
                                </td>
                            </tr>
                            <tr>
                                <td class="fpInstTableLabel"></td>
                                <td class="fpInstTableContent"><img class="fp" src="../Images/User/Dropbox-AppKey-1.gif" alt="Step 2." /></td>
                            </tr>
                            <tr class="fpInstTableLabel">
                                <td class="fpInstTableLabel">STEP 3.</td>
                                <td>
                                    <ol>
                                        <li>Enter an 'App name' and 'Description'. The values are irrelevant ('App name' must be globally unique however).</li>
                                        <li>Click the 'Create' button when you are finished.</li>
                                    </ol>
                                </td>
                            </tr>
                            <tr>
                                <td class="fpInstTableLabel"></td>
                                <td class="fpInstTableContent"><img class="fp" src="../Images/User/Dropbox-AppKey-2.gif" alt="Step 3." /></td>
                            </tr>
                            <tr class="fpInstTableLabel">
                                <td class="fpInstTableLabel">STEP 4.</td>
                                <td><ul><li>Find your 'App Name' in the table and click its 'Options' button.</li></ul></td>
                            </tr>
                            <tr>
                                <td class="fpInstTableLabel"></td>
                                <td class="fpInstTableContent"><img class="fp" src="../Images/User/Dropbox-AppKey-3.gif" alt="Step 4." /></td>
                            </tr>
                            <tr class="fpInstTableLabel">
                                <td class="fpInstTableLabel">STEP 5.</td>
                                <td>
                                    <ol>
                                        <li>SCROLL DOWN. Near the bottom of the page is a section entitled 'App keys'.</li>
                                        <li>Copy and paste the 'Key' value (example: v6vwy...) into the 'App Key:' box at the top of this page.</li>
                                        <li>Copy and paste the 'Secret' value into the 'App Secret:' box at the top of this page.</li>
                                        <li>Then click the 'Cancel' button on the Dropbox site to avoid any unintended changes.</li>
                                        <li>Click the 'Link My Dropbox Account!' button on this page and you are DONE! Yay!</li>
                                    </ol>
                                </td>
                            </tr>
                            <tr>
                                <td class="fpInstTableLabel"></td>
                                <td class="fpInstTableContent"><img class="fp" src="../Images/User/Dropbox-AppKey-4.gif" alt="Step 5." /></td>
                            </tr>
                        </table>
                    </dx:PanelContent>
                </PanelCollection>
            </dxrp:ASPxRoundPanel>  
        </div>
    </div>
</asp:Content>

