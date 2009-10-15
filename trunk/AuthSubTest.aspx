<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AuthSubTest.aspx.vb" Inherits="AuthSubTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AuthSub Demo</title>
    <link href="StyleSheet.css" rel="stylesheet" type="text/css" />
</head>
<body class="fbbody">
    <form runat="server" id="form2">
    <%
        If HttpContext.Current.Session("yttoken") Is Nothing Then
    %>
    <%=AuthSubGoogleSignInLinkGenerator()%>
    <%
    End If

    %>
    <br />
    <br />
    <asp:Panel runat="server" ID="panel1" CssClass="fbbluebox" Width="50%">
        <table border="0">
            <tr>
                <td>
                    <asp:Label runat="server" ID="label1" Text="Video Title"></asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="text1">

                    </asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="label2" Text="Description"></asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="TextBox1" Rows="5" Columns="30" TextMode="MultiLine"
                        Wrap="true">

                    </asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="label3" Text="Keywords"></asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="TextBox2">

                    </asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Category
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddl1">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <asp:Button runat="server" ID="button1" Text="Send MetaData" Style="direction: ltr" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <div class="fbtab" runat="server" id="errorDiv" visible="false">
    <asp:Label runat="server" ID="errorlbl" /></div>
    </form>
    <div id="uploadform" runat="server" visible="false" class="fbbluebox" style="width: 50%;">
        <form id="form1" method="post" enctype="multipart/form-data" action="<%=UploadUrl() %>?nexturl=<%=Resources.AuthSubRes.NextUrlForFormQueryString %>">
        <input type="file" name="file" class="myfile" />
        <input type="hidden" name="token" value="<%=Token %>" />
        <input type="submit" value="Upload Using Browser Upload" />
        </form>
    </div>
</body>
</html>
