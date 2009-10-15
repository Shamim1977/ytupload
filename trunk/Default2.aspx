<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default2.aspx.vb" Inherits="Default2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>


<form runat="server" id="form2" >
<h3>Comments</h3>
<ul>
<li>Inherits from BrowserUpload class</li>
<li>Requires your youtube username and password to work</li>
<li>Place your youtube username and password in BrowserUpload.vb class level variables <b>_email</b> and <b>_pw</b></li>
<li>It is better not to ask your users for youtube credentials. Look at <b>AuthSubTest.aspx</b> to see how to do that</li>
<li>Also refer to AuthSubTest.aspx to see how to hide the file upload form until users have logged in and meta data has been sent.</li>
<li>Make sure to read all comments in code behind and class files. Everything has been properly commented for your convenience.</li>
</ul>
<asp:TextBox runat="server" ID="text1">
Enter Title Here
</asp:TextBox>
<asp:TextBox runat="server" ID="TextBox1">
Enter Description Here
</asp:TextBox>
<asp:TextBox runat="server" ID="TextBox2">
Enter Keywords here
</asp:TextBox>

<asp:DropDownList runat="server" ID="ddl1"></asp:DropDownList>
<asp:Button runat="server" ID="button1" Text="Send MetaData" 
    style="direction: ltr" />
</form>
    <form id="form1" method="post" enctype="multipart/form-data" action="<%=UploadUrl %>?nexturl=http://www.ecubicle.net">
    
    <input type="file" name="file" />
    <input type="hidden" name="token" value="<%=Token %>" />
    <input type="submit" value="Upload Using Browser Upload" />
        </form>
</body>
</html>
