Imports System.Net
Imports System.IO
Imports System.Xml.Linq
'this pages uses direct uploading class yt.
'Direct Uploading is NOT SUITABLE for scenarios where you want users to upload to YouTube from your web site. 
'Look at AuthSubTest.aspx, Default2.aspx and BrowserUpload.vb and AuthSub.vb 
'Direct Upload first uploads to your server and then to youtube.

Partial Class _Default
    Inherits System.Web.UI.Page
   

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
     
        Dim myYT As New yt()

        Dim myDetails As yt.VideoDetailsAfterUploading = myYT.UploadVideo(Server.MapPath(Request.ApplicationPath & "\vid.avi"), "Welcome to YT", "This is sample description", "keyword1, test, new, class", "Film")


        Dim myHyperlink As New HyperLink()
        myHyperlink.Text = myDetails.UploadedTitle
        myHyperlink.NavigateUrl = myDetails.UploadedUrl
        form1.Controls.Add(myHyperlink)
      

        'Show Categories in DropDownList
        Dim myDDL As New DropDownList()
        myDDL.DataSource = myYT.GetCategories()
        myDDL.DataTextField = "category"
        myDDL.DataValueField = "category"
        myDDL.DataBind()
        form1.Controls.Add(myDDL)










    End Sub

   
End Class
