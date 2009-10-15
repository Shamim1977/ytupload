Imports Microsoft.VisualBasic
Imports System.Net
Imports System.IO

'This uses BrowserUpload class 
'But, whereas the BrowserUpload class uses Youtube login, AuthSub (this class) uses AuthSub which is Google Login.
'Using AuthSub, your users do not have to enter any credentials on your web site. They are redirected to Google Page and enter their credentials in a secure 
'and friendly environment.
'I suggest using AuthSub whereever possible

'Make sure that these code behind pages do not inherit from System.Web.UI.Page. 
'BrowserUpload class inherits from the Page class.

'In this class, change the variables as instructed below:

Public Class AuthSub
    Inherits BrowserUpload

    'myScope remains the same always. DO NOT CHANGE.
    Private myScope As String = Resources.AuthSubRes.myScope
    'NextUrl is the URL that users are redirected to after Google Login authenticates the user. This should logically be the URL where the video upload 
    'form is hosted on your web site. After Google authenticates the user, users need to be transferred to the video upload form on your web site
    'so that they may upload the video. NextUrl holds the page with input type=file.
    Private NextUrl As String = Resources.AuthSubRes.NextUrlForFileUploadControl
    'mySession is related to token lifetimes.MySession value 1 means
    'that token is long lived (lives while session stays alive).  
    'Read up on long lived session tokens at http://code.google.com/apis/accounts/docs/AuthSub.html#AuthSubSessionToken
    Private mySession As String = Resources.AuthSubRes.mySession
    'mySecure means whether token is secure or nonsecure. Secure tokens are issued only to websites that have registered with Google,
    'and video upload requests that use a secure token must be digitally signed.
    'Read more about this at http://code.google.com/apis/accounts/docs/AuthSub.html#signingrequests
    Private mySecure As String = Resources.AuthSubRes.mySecure

    Public Function AuthSubGoogleSignInLinkGenerator() As String
        Dim myHyperLink As New HyperLink()
        myHyperLink.NavigateUrl = String.Format("https://www.google.com/accounts/AuthSubRequest?next={0}&scope={1}&session={2}&secure={3}", NextUrl, myScope, MySession, MySecure)
        myHyperLink.Text = "Login With Google Accounts"
        Dim myStringWriter As New StringWriter()
        Dim myHtmlTextWriter As New HtmlTextWriter(myStringWriter)
        myHyperLink.RenderControl(myHtmlTextWriter)
        Return myStringWriter.ToString()

    End Function

    

End Class
