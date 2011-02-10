<%@ Page Language="VB" AutoEventWireup="false" CodeFile="bomViewProjectDetail.aspx.vb" Inherits="pBOM_bomViewProjectDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="StyleSheet" href="../FP_Code/fpStyle.css" type="text/css" media="screen" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div style="font-size:16pt; font-family:Arial Black"><asp:Label ID="lblTitle" runat="server" Text="Title"></asp:Label></div>
        <div style="font-size:10pt; font-weight:normal; font-family:Arial;"><asp:Literal ID="litRevisions" runat="server"></asp:Literal></div>

        <br /><hr />

        <div style="font-size:10pt; font-family:Arial"><asp:Label ID="lblDescription" runat="server" Text="Subtitle"></asp:Label></div>
        
        <br /><hr />
        
        <table>
            <tr>
                <td class="fpTableLabel">Project#:</td>
                <td><asp:Label ID="lblProjectID" runat="server" Text="Revision"></asp:Label></td>
            </tr>
            <tr>
                <td class="fpTableLabel">Revision:</td>
                <td><asp:Label ID="lblRev" runat="server" Text="Revision"></asp:Label></td>
            </tr>
            <tr>
                <td class="fpTableLabel">Responsible Party:</td>
                <td><asp:Label ID="lblOwner" runat="server" Text="Revisions"></asp:Label></td>
            </tr>
            <tr>
                <td class="fpTableLabel">Date Created:</td>
                <td><asp:Label ID="lblDateCreated" runat="server" Text="Revisions"></asp:Label></td>
            </tr>
        </table>
        
        <br /><hr />
                
        <asp:HyperLink ID="aViewReports" runat="server">View Reports</asp:HyperLink>
        
        <br /><hr />
        <asp:HyperLink ID="lnkAllFiles" runat="server">Download All Files (zip)</asp:HyperLink><br />
        
        Download Individual Files<br />
        <asp:Literal ID="litFiles" runat="server"></asp:Literal>
        
        
        <br />
        
        <hr />
        Notes? (multiple revisions?)

        
    </div><!-- Complete Container -->
    </form>
</body>
</html>
