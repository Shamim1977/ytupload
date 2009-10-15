

Partial Class AuthSubTest
    Inherits AuthSub
    
    'In the aspx page, look at the form that uploads
    'The form, in action attribute, has a nexturl value.
    'Change this value to redirect users to a page on your web site after video uploads.
    'This nexturl would contain a querystring parameter id with the id of the uploaded video.
    'Extract this id using request.querystring("id") and show your users a youtube watch video link like this:
    'myhyperlink.navigateurl="http://www.youtube.com/watch?v=request.querystring("id")"

    'There is another querystring called status.
    'if status has a value of 200 (that is, status=200 in the url), that means the video has uploaded successfully.

    'The above also applies to BrowserUpload using YouTube Login (Default2.aspx)

    'You would also need to hide the input type="file" until you get the token back.
    'This can be checked in the UploadVideo method
    'Uploadvideo returns 1 if token is successfully returned. In this case, show user the file input box
    'If UploadVideo returns 0 do not show the input type=file
    Protected Sub button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles button1.Click


        Dim retval As UploadObject = MyBase.UploadVideo(text1.Text, TextBox1.Text, TextBox2.Text, ddl1.SelectedValue, True)
        If String.Equals(retval.Message, "True") Then
            form2.Visible = False
            uploadform.Visible = True

        Else
            errorDiv.Visible = True
            errorlbl.Text = retval.Message & "<br/><br/>" & retval.Source & "<br/><br/>" & retval.Code & ": " & retval.ErrorCodeToString
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
                If Request.Url.Query.Contains("token") Then
            GetLengthySession()
            Response.Redirect("AuthSubTest.aspx")
        End If
        If Not Page.IsPostBack Then
            ddl1.DataSource = Me.GetCategories()
            ddl1.DataBind()
        End If
       

      
    End Sub
End Class
