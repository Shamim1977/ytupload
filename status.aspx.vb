
Partial Class status
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.UrlReferrer IsNot Nothing AndAlso Request.UrlReferrer.AbsolutePath.Contains("AuthSubTest.aspx") Then
            If Request.QueryString("status") = "200" Then
                hyperlink1.NavigateUrl = String.Format("http://www.youtube.com/watch?v={0}", Request.QueryString("id"))
                hyperlink1.Text = "Watch Video"
                label1.Text = "Your video has been uploaded successfully."
            End If
        Else
            label1.Text = "Please upload video first and then check status"

        End If
    End Sub
End Class
