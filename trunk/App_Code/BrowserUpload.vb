Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Net
Imports System.Xml.Linq
Imports System.Resources
Imports System.Reflection
Imports System.Windows.Forms

'This implements Browser Upload Pattern.
'Browser Upload lets your users to login with their youtube accounts and upload to youtube directly
'BrowserUpload class uses YouTube UserName and Password for login
'AuthSub class uses Google Login for login

Public Class BrowserUpload
    Inherits System.Web.UI.Page

    'email is youtube username NOT YOUR EMAIL. You dont need this if using AUTHSUB. Leave it blank for AUTHSUB.
    Private _email As String = Resources.Resources._email
    '_pw is youtube password. You dont need this if using AUTHSUB. Leave it blank for AUTHSUB.
    Private _pw As String = Resources.Resources._pw
    Private _appName As String = Resources.Resources._appName
    Private _clientID As String = Resources.Resources._clientID
    Private _developerKey As String = Resources.Resources._developerKey
    Private _myboundary As String = System.DateTime.Now.Ticks.ToString()
    Private myauthinfo As AuthTokenReturn

    Public Structure AuthTokenReturn
        Dim AuthToken As String
        Dim UserNameReturned As String
    End Structure


    Protected Function CreateXMLDOC(ByVal title As String, ByVal description As String, ByVal keywords As String, ByVal mycategory As String) As String
        Dim myDoc As New XDocument(New XDeclaration("1.0", "UTF-8", "true"))
        Dim myentryNS As XNamespace = Resources.Resources.AtomNS
        Dim mymediaNS As XNamespace = Resources.Resources.mediaNS
        Dim myytNS As XNamespace = Resources.Resources.YtSchemaNS

        Dim myrootElem As New XElement(myentryNS + "entry", New XAttribute("xmlns", myentryNS.NamespaceName), _
                                       New XAttribute(XNamespace.Xmlns + "media", mymediaNS.NamespaceName), _
                                       New XAttribute(XNamespace.Xmlns + "yt", myytNS.NamespaceName))


        Dim elemArray(4) As XElement

        elemArray(0) = (New XElement(mymediaNS + "title", New XAttribute("type", "plain"), New XText(title)))
        elemArray(1) = (New XElement(mymediaNS + "description", New XAttribute("type", "plain"), New XText(description)))
        elemArray(2) = (New XElement(mymediaNS + "category", New XAttribute("scheme", Resources.Resources.YTCategoriesSchemaUrl), _
                                     New XText(mycategory)))
        elemArray(3) = (New XElement(mymediaNS + "keywords", New XText(keywords)))

        Dim myMediaGroup As New XElement(mymediaNS + "group", elemArray)
        myrootElem.AddFirst(myMediaGroup)
        myDoc.Declaration = New XDeclaration("1.0", "UTF-8", "true")
        myDoc.AddFirst(myrootElem)

        Return myDoc.ToString()
    End Function
    ''' <summary>
    ''' This method only appends a XML declaration to the XML doc created by CreateXMLDoc(,,,)
    ''' It is used because when creating XML doc with LINQ, the declaration is omitted for some reason unless other techniques such as XMLTextWriter are used.
    ''' </summary>
    ''' <param name="title"></param>
    ''' <param name="description"></param>
    ''' <param name="keywords"></param>
    ''' <param name="mycategory"></param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Protected Function CreatePostBody(ByVal title As String, ByVal description As String, ByVal keywords As String, ByVal mycategory As String) As String
        Dim myVideoXML As String = CreateXMLDOC(title, description, keywords, mycategory)
        Dim myPostBody As New StringBuilder()
        myPostBody.Append("<?xml version=""1.0"" encoding=""utf-8""?>" & ControlChars.CrLf)
        myPostBody.Append(myVideoXML)
        Return myPostBody.ToString()
    End Function

    Public Function UploadVideo(ByVal title As String, ByVal description As String, ByVal keywords As String, ByVal mycategory As String, ByVal myAuth As Boolean) As UploadObject
        If myAuth = False Then
            myauthinfo = GetToken()
        End If
        Dim myUploadObjectInfo As New UploadObject()
        Dim myPostBody As String = CreatePostBody(title, description, keywords, mycategory)
        Dim myWebRequest As HttpWebRequest
        Dim myResult As String
        Try
            myWebRequest = SendRequest(Resources.Resources.UploadTokenUrl, myPostBody, myauthinfo, myAuth)
            myResult = GetResponse(myWebRequest)
            'Return 1
            myUploadObjectInfo.Message = "True"
            myUploadObjectInfo.Source = ""
            Return myUploadObjectInfo
        Catch ex As System.Net.WebException
            myUploadObjectInfo.Message = CType(ex.Response, HttpWebResponse).StatusDescription
            myUploadObjectInfo.Code = CType(ex.Response, HttpWebResponse).StatusCode
            myUploadObjectInfo.Source = ex.StackTrace
            Return myUploadObjectInfo
        Catch ex As Exception
            myUploadObjectInfo.Message = ex.Message
            myUploadObjectInfo.Source = ex.StackTrace
            myUploadObjectInfo.Code = Nothing
            Return myUploadObjectInfo

        End Try

    End Function

    Private Function GetToken() As AuthTokenReturn
        Dim myURL As String = (Resources.Resources.GoogleLoginTokenUrl)
        Dim myPostBody As New StringBuilder()
        With myPostBody
            .Append(String.Format(Resources.Resources.GetTokenMethodString, HttpUtility.UrlEncode(_email), HttpUtility.UrlEncode(_pw), HttpUtility.UrlEncode(_appName)))
        End With
        Dim myWebRequest As HttpWebRequest = SendRequest(myURL, myPostBody.ToString(), "application/x-www-form-urlencoded", "POST")
        Dim myResult As String = GetResponse(myWebRequest)
        Dim extractInfo() As String = myResult.Split("=")
        Dim ReturnInfo As New AuthTokenReturn()
        ReturnInfo.AuthToken = extractInfo(1).Remove(extractInfo(1).IndexOf("YouTubeUser")).Trim()
        ReturnInfo.UserNameReturned = extractInfo(2).Trim()
        Return ReturnInfo
    End Function

    Protected Sub ParseXMLResponse(ByVal xmlResponseAsString As String)
        Dim myXdoc As XDocument = XDocument.Parse(xmlResponseAsString)
        Dim myResponse = (From myDoc In myXdoc.Descendants("response") _
                         Select New With { _
                         .uploadToken = myDoc.Element("token").Value, _
                         .uploadUrl = myDoc.Element("url").Value}).SingleOrDefault
        _uploadurl = myResponse.uploadUrl
        _token = myResponse.uploadToken
    End Sub
    Protected Sub GetLengthySession()
        Dim myWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(Resources.Resources.LengthySessionUrl), HttpWebRequest)
        myWebRequest.Method = "GET"
        myWebRequest.Headers.Add("Authorization", "AuthSub token=" + HttpContext.Current.Request.QueryString("token"))
        myWebRequest.ContentType = "application/x-www-form-urlencoded"
        myWebRequest.Accept = "text/html, image/gif, image/jpeg, *; q=.2, */*; q=.2"
        Dim myWebResponse As HttpWebResponse = DirectCast(myWebRequest.GetResponse(), HttpWebResponse)
        Dim myResponse As New StreamReader(myWebResponse.GetResponseStream())
        Dim myresult As String = myResponse.ReadToEnd()
        HttpContext.Current.Session("yttoken") = myresult.Replace("Token=", "").Trim()
    End Sub
    ''' <summary>
    ''' This method creates the POST request for meta data. Youtube requires metadata to be sent first. After metadata is sent to youtube, 
    ''' youtube sends a response with URL and Token. The URL value must be used by the file upload form and the video must be posted to this URL
    ''' The Token is used for authorization headers.
    ''' </summary>
    ''' <param name="myUrlToPostTo">URL to which request is sent. Look at UploadVideo method to check its value</param>
    ''' <param name="myBody">String value that is posted. This contains meta data in xml format</param>
    ''' <param name="myauthInfo">only used when Google Login is used. With Google Login, it sets the token and username returned by Google. Not used in AuthSub</param>
    ''' <param name="myAuth">Indicates whether to use AuthSub or Google Login. True=AuthSub, False=Google Login</param>
    ''' <returns>object of type httpwebrequest</returns>
    ''' <remarks></remarks>

    Private Overloads Function SendRequest(ByVal myUrlToPostTo As String, ByVal myBody As String, ByVal myauthInfo As AuthTokenReturn, ByVal myAuth As Boolean) As HttpWebRequest
        Dim buffer() As Byte = System.Text.Encoding.UTF8.GetBytes(myBody.ToString())
        Dim myWebClient As HttpWebRequest = DirectCast(WebRequest.Create(myUrlToPostTo), HttpWebRequest)

        myWebClient.Method = "POST"
        myWebClient.ContentType = "application/atom+xml; charset=UTF-8"
        If myAuth = True Then
            'for long sessions, set the value of mysession class variable to 1 in AuthSub.vb
            If Session("yttoken") Is Nothing Then
                GetLengthySession()
            End If
            'myWebClient.Headers.Add("Authorization", "AuthSub token=" + HttpContext.Current.Request.QueryString("token"))
            myWebClient.Headers.Add("Authorization", "AuthSub token=" + CType(Session("yttoken"), String))
        Else
            myWebClient.Headers.Add("Authorization", "GoogleLogin auth=" + myauthInfo.AuthToken)
        End If
        myWebClient.ContentLength = buffer.Length
        myWebClient.Headers.Add("X-GData-Client", _clientID)
        myWebClient.Headers.Add("X-GData-Key", "key=" & _developerKey)
        Dim myRequestStream As Stream = myWebClient.GetRequestStream()
        myRequestStream.Write(buffer, 0, buffer.Length)
        myRequestStream.Close()
        Dim myResponse As String = GetResponse(myWebClient)
        ParseXMLResponse(myResponse)
        Return myWebClient
    End Function

    Private Overloads Function SendRequest(ByVal myUrlToPostTo As String, ByVal myBody As String, ByVal myCType As String, ByVal myMethod As String) As HttpWebRequest
        Dim buffer() As Byte = System.Text.Encoding.UTF8.GetBytes(myBody.ToString())
        Dim myWebClient As HttpWebRequest = DirectCast(WebRequest.Create(myUrlToPostTo), HttpWebRequest)
        myWebClient.Method = myMethod
        myWebClient.ContentLength = myBody.Length
        myWebClient.ContentType = myCType
        Dim myRequestStream As Stream = myWebClient.GetRequestStream()
        myRequestStream.Write(buffer, 0, buffer.Length)
        myRequestStream.Close()
        Return myWebClient
    End Function
    Protected Function GetResponse(ByVal myWebClient As HttpWebRequest) As String
        Dim myWebResponse As HttpWebResponse = DirectCast(myWebClient.GetResponse(), HttpWebResponse)
        Dim myStream As New StreamReader(myWebResponse.GetResponseStream)
        Return myStream.ReadToEnd()


    End Function
    ''' <summary>
    ''' Gets Categories From Youtube categories schema. Avoids hardcoding the categories.
    ''' </summary>
    ''' <returns>List of Type String</returns>
    ''' <remarks></remarks>
    Public Function GetCategories() As List(Of String)
        Dim myXPathDocument As New System.Xml.XPath.XPathDocument(Resources.Resources.YTCategoriesSchemaUrl)
        Dim myCatList As New List(Of String)
        Dim myNavigator As System.Xml.XPath.XPathNavigator = myXPathDocument.CreateNavigator()
        Dim myNS As New System.Xml.XmlNamespaceManager(myNavigator.NameTable)
        myNS.AddNamespace("atom", Resources.Resources.AtomNS)
        myNS.AddNamespace("yt", Resources.Resources.YtSchemaNS)
        Dim myNodeIterator As System.Xml.XPath.XPathNodeIterator = myNavigator.Select("//atom:category[yt:assignable]/@term", myNS)
        myNodeIterator.MoveNext()
        For Each myitem As System.Xml.XPath.XPathNavigator In myNodeIterator
            myCatList.Add(myitem.Value)
        Next
        Return myCatList
    End Function
    ''' <summary>
    ''' This method is not used.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateUploadForm() As String
        Dim mySW As New StringWriter()
        Dim myHW As New HtmlTextWriter(mySW)

        myHW.AddAttribute("action", uploadURL & "?nexturl=http://www.ecubicle.net")
        myHW.AddAttribute("enctype", "multipart/form-data")
        myHW.AddAttribute("method", "post")
        myHW.RenderBeginTag(HtmlTextWriterTag.Form)

        myHW.AddAttribute("type", "file")
        myHW.AddAttribute("name", "file")
        myHW.RenderBeginTag(HtmlTextWriterTag.Input)
        myHW.RenderEndTag()

        myHW.AddAttribute("type", "hidden")
        myHW.AddAttribute("name", "token")
        myHW.AddAttribute("value", Token)
        myHW.RenderBeginTag(HtmlTextWriterTag.Input)
        myHW.RenderEndTag()

        myHW.AddAttribute("type", "submit")
        myHW.AddAttribute("value", "Upload Video")
        myHW.RenderBeginTag(HtmlTextWriterTag.Input)
        myHW.RenderEndTag()
        myHW.RenderEndTag()

        Return mySW.ToString()

    End Function
   


    Private _uploadurl As String = String.Empty
    Public Property uploadURL() As String
        Get
            Return _uploadurl
        End Get
        Set(ByVal value As String)
            _uploadurl = value
        End Set
    End Property

    Private _token As String = String.Empty
    Public Property Token() As String
        Get
            Return _token
        End Get
        Set(ByVal value As String)
            _token = value
        End Set
    End Property

End Class
