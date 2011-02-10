<%@ Page Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="dbSQL.aspx.vb" Inherits="dbSQL" title="SQL Parser" validateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
    <!-- NOTE! Validate Page request = false for this one to disable dangerous code checking since we deliberately insert SQL here -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="fpErrorBox">Note: This page is a security risk. Restrict access to trusted developers only prior to going live!</div>
    <asp:TextBox ID="TextBox1" runat="server" Height="528px" Rows="100" 
        Width="703px" TextMode="MultiLine" Wrap="False"></asp:TextBox><br /><br />    
    <asp:Button ID="Button1" runat="server" Text="Reformat!" />
</asp:Content>

