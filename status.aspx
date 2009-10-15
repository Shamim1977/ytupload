<%@ Page Language="VB" AutoEventWireup="false" CodeFile="status.aspx.vb" Inherits="status" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>YouTube Video Upload Demo</title>
    <link href="StyleSheet.css" rel="stylesheet" type="text/css" />
</head>
<body class="fbbody">
    <form id="form1" runat="server">
    <div class="fbtab">
        <div class="fbbluebox">
            <asp:Label runat="server" ID="label1"></asp:Label><br />
            <p class="fbinfobox">
                <asp:HyperLink runat="server" ID="hyperlink1"></asp:HyperLink><br />
                <a href="AuthSubTest.aspx">Upload</a>
            </p>
        </div>
    </div>
    </form>
</body>
</html>
