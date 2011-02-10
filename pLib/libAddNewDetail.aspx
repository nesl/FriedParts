<%@ Page Language="VB" AutoEventWireup="true" CodeFile="libAddNewDetail.aspx.vb" Inherits="pInv_invAddNewDetail" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxPopupControl" TagPrefix="dxpc" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>

<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dxe" %>

<%@ Register assembly="DevExpress.XtraCharts.v10.2.Web, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.XtraCharts" tagprefix="dxchartsui" %>
<%@ Register assembly="DevExpress.XtraCharts.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.XtraCharts" tagprefix="cc1" %>
<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxRoundPanel" tagprefix="dxrp" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add New Library Details Page</title>
</head>
<body>
    <form id="form1" runat="server" style="font-family: Tahoma, Arial, Helvetica, sans-serif; font-size: 9pt; font-weight: normal; font-style: normal; font-variant: normal; color: #000000">
    <!-- Top Left Column -->
    <div id="TopBoxLeftCol" runat="server" style="position:absolute; top:0; left:0; width:350px; height:350px">        
        <asp:SqlDataSource ID="fpSQLData" runat="server"></asp:SqlDataSource>
        <div id="TitleMain" runat="server" style="font-size:14pt; font-weight:bold;"></div>
        <div id="TitleSub" runat="server" style="font-size:11pt; font-weight:bold;"></div>        
        <br />
        
        <div style="margin-bottom:5px"><b>Library Name:</b><br />
            <asp:TextBox ID="txtLibName" runat="server" Height="20px" Width="325px" BackColor="#EEEEEE" 
                BorderColor="#00CC66" BorderStyle="Solid" BorderWidth="1px" Rows="1" ReadOnly="true"></asp:TextBox>
        </div>
        <div style="margin-bottom:5px"><b>Library Type:</b><br />
            <asp:TextBox ID="txtLibType" runat="server" Height="20px" Width="325px" BackColor="#EEEEEE" 
                BorderColor="#00CC66" BorderStyle="Solid" BorderWidth="1px" Rows="1" ReadOnly="true"></asp:TextBox>
        </div>
        <div style="margin-bottom:5px"><b>Filename:</b><br />
        <asp:TextBox ID="txtFileName" runat="server" Height="20px" Width="325px" BackColor="#EEEEEE" 
            BorderColor="#00CC66" BorderStyle="Solid" BorderWidth="1px" Rows="1" ReadOnly="true"></asp:TextBox>
        </div>
        <div style="margin-bottom:5px"><b>Owner:</b><br />
        <asp:TextBox ID="txtOwner" runat="server" Height="20px" Width="325px" BackColor="#EEEEEE" 
            BorderColor="#00CC66" BorderStyle="Solid" BorderWidth="1px" Rows="1" ReadOnly="true"></asp:TextBox>
        </div>
        <div style="margin-bottom:5px"><b>Description:</b><br />
        <asp:TextBox ID="txtDesc" runat="server" Height="125px" Width="325px" 
            BorderColor="#00CC66" BorderStyle="Solid" BorderWidth="1px"
            TextMode="MultiLine"></asp:TextBox>
        </div>
        <br /><hr />
        <b>Add this library to the FriedParts Database?</b><br /><br />
        <div style="text-align:center">
            <input id="Button_NewName" type="button" value="Hell YES!" runat="server" />
            <input id="Button_OldName" type="button" value="Use original Filename" runat="server" />
            &nbsp;&nbsp;&nbsp;&nbsp;
            <input id="Button2" type="button" value="Hell NO!" 
                onclick="{window.parent.xPopup.Hide();}" />
        </div>
    </div> <!-- Top Left Column -->
    <!--<!--#inlude file="../pSys/MsgBox.include"-->
    </form>
</body>
</html>
