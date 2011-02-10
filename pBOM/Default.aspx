<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="pBOM_Default" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxUploadControl" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <script type="text/javascript">
    // <![CDATA[
        var fieldSeparator = "|";
        function FileUploadStart() {
            document.getElementById("uploadedListFiles").innerHTML = "";
        }
        function FileUploaded(s, e) {
            if(e.isValid) {
                var linkFile = document.createElement("a");
                var indexSeparator = e.callbackData.indexOf(fieldSeparator);
                var fileName = e.callbackData.substring(0, indexSeparator);
                var pictureUrl = e.callbackData.substring(indexSeparator + fieldSeparator.length);
                var date = new Date();
                var imgSrc = "UploadImages/" + pictureUrl + "?dx=" + date.getTime();
                linkFile.innerHTML = fileName;
                linkFile.setAttribute("href", imgSrc);
                linkFile.setAttribute("target", "_blank");
                var container = document.getElementById("uploadedListFiles");
                container.appendChild(linkFile);
                container.appendChild(document.createElement("br"));
            }
        }
    // ]]> 
    </script>
    <dx:ASPxCheckBox ID="chbShowProgressPanel" runat="server" AutoPostBack="true" Checked="True"
        Text="ShowProgressPanel">
    </dx:ASPxCheckBox>
    <br />
    <table style="width: 100%;">
        <tr>
            <td valign="top">
                <dx:ASPxUploadControl ID="UploadControl" runat="server" ShowAddRemoveButtons="True"
                    Width="300px" ShowUploadButton="True" AddUploadButtonsHorizontalPosition="Left"
                    ShowProgressPanel="True" ClientInstanceName="UploadControl" OnFileUploadComplete="UploadControl_FileUploadComplete"
                    FileInputCount="3">
                    <ValidationSettings AllowedFileExtensions=".jpg,.jpeg,.jpe,.gif">
                    </ValidationSettings>
                    <ClientSideEvents FileUploadComplete="function(s, e) { FileUploaded(s, e) }" FileUploadStart="function(s, e) { FileUploadStart(); }" />
                </dx:ASPxUploadControl>
                <br />
                <p>
                    <b>Note</b>: The total size of files to upload is limited by 28Mb in this demo.</p>
                <br />
            </td>
            <td valign="top" style="width: 1%;">
                <dx:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" Width="300px" ClientInstanceName="RoundPanel"
                    HeaderText="Uploaded files (jpeg, gif)" Height="100%">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                            <div id="uploadedListFiles" style="height: 150px; font-family: Arial;">
                            </div>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>
            </td>
        </tr>
    </table>
   
    </div>

    <div>
        <hr />
            Username: <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
            Password: <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" Text="Login to Dropbox" />
        <hr />
            User Token: <asp:label ID="Label1" runat="server" text="Label"></asp:label>
            User Secret: <asp:label ID="Label2" runat="server" text="Label"></asp:label>        
    </div>
    </form>
</body>
</html>

