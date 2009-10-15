'README FIRST
'In this sample, we are inheriting from BrowserUpload and the BrowserUpload class inherits from System.Web.UI.Page. Please keep this in mind.

'If you would rather inherit your page from System.Web.Ui.Page, then uncomment all commented code lines (total 3 below), and comment out all active lines.
'In your aspx source, change action and token values to myyt.UploadUrl and myyt.Token (here myyt is the class level object of BrowserUpload.

'I suggest inheriting your aspx page from BrowserUpload (first approach) because this is needed to access the Token and URL in the aspx source.
'Otherwise, you would need a class level object of BrowserUpload

'Make sure to change the nexturl in the ASPX PAGE to the url on your web site.
'nexturl is shown to users after video is uploaded (or not uploaded and error).
'nexturl would contain a querystring status. if status=200, that means video uploaded.
'next url would also contain querystring id. this is the id of the new video on youtube.
Partial Class Default2
    Inherits BrowserUpload
    'Public myyt As New BrowserUpload()

    Protected Sub button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles button1.Click
        'myyt.UploadVideo(text1.Text, TextBox1.Text, TextBox2.Text, ddl1.SelectedValue)
        Me.UploadVideo(text1.Text, TextBox1.Text, TextBox2.Text, ddl1.SelectedValue, False)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            'ddl1.DataSource = myyt.GetCategories()
            ddl1.DataSource = Me.GetCategories()
            ddl1.DataBind()
        End If



    End Sub
End Class
