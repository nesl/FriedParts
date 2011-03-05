<%@ Page Title="" Language="VB" MasterPageFile="~/FP.master" AutoEventWireup="false" CodeFile="Test2.aspx.vb" Inherits="pParts_Test2" %>
<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>

<%@ Register assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" Runat="Server">
    <!--#include file="/friedparts/fp_code/controls/parttypeaccordioncontrol/PartTypeAccordion.head"-->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div style="border:1px solid red;">
        <!--#include file="/friedparts/fp_code/controls/parttypeaccordioncontrol/PartTypeAccordion.body"-->
    </div>
    <div>

    <asp:Label ID="lbl" runat="server" Text="Label"></asp:Label>

        <dxwgv:ASPxGridView ID="ASPxGridView1" runat="server" 
            ClientIDMode="AutoID" 
            ClientInstanceName="G1" EnableCallBacks="false">

<ClientSideEvents RowClick="function(s, e) {s.PerformCallback(''+e.visibleIndex);}"></ClientSideEvents>

        </dxwgv:ASPxGridView>
        

    </div>

    <div>
        <iframe src="/friedparts/fp_code/controls/parttypeaccordioncontrol/PartTypeAccordion.aspx" width="100%" height="300px">
            Not supported by your browser.
        </iframe>
    </div>

</asp:Content>

