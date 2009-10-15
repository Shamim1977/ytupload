Imports Microsoft.VisualBasic
Imports System.Net
Imports System.IO
Imports System.Xml.Linq

'This implements Direct Upload.
'Direct Upload is used when you want users to upload to your site as well as youtube.
'Therefore, videos are first uploaded to your web site, and then to youtube
'To upload directly to youtube, use BrowserUpload


Public Class yt
    'email is your username on youtube. it is NOT your email
    Private _email As String = ""
    Private _pw As String = ""
    Private _appName As String = "Testing"
    Private _clientID As String = ""
    Private _developerKey As String = ""
    Private _myboundary As String = System.DateTime.Now.Ticks.ToString()
    Private myauthinfo As AuthTokenReturn
   

    Public Structure AuthTokenReturn
        Dim AuthToken As String
        Dim UserNameReturned As String
    End Structure
   

    Private Function CreateXMLDOC(ByVal title As String, ByVal description As String, ByVal keywords As String, ByVal mycategory As String) As String
        Dim myDoc As New XDocument(New XDeclaration("1.0", "UTF-8", "true"))
        Dim myentryNS As XNamespace = "http://www.w3.org/2005/Atom"
        Dim mymediaNS As XNamespace = "http://search.yahoo.com/mrss/"

        Dim myytNS As XNamespace = "http://gdata.youtube.com/schemas/2007"
        Dim myrootElem As New XElement(myentryNS + "entry", New XAttribute("xmlns", myentryNS.NamespaceName), _
                                       New XAttribute(XNamespace.Xmlns + "media", mymediaNS.NamespaceName), _
                                       New XAttribute(XNamespace.Xmlns + "yt", myytNS.NamespaceName))


        Dim elemArray(4) As XElement

        elemArray(0) = (New XElement(mymediaNS + "title", New XAttribute("type", "plain"), New XText(title)))
        elemArray(1) = (New XElement(mymediaNS + "description", New XAttribute("type", "plain"), New XText(description)))
        elemArray(2) = (New XElement(mymediaNS + "category", New XAttribute("scheme", "http://gdata.youtube.com/schemas/2007/categories.cat"), _
                                     New XText(mycategory)))
        elemArray(3) = (New XElement(mymediaNS + "keywords", New XText(keywords)))

        Dim myMediaGroup As New XElement(mymediaNS + "group", elemArray)
        myrootElem.AddFirst(myMediaGroup)
        myDoc.AddFirst(myrootElem)
        Return myDoc.ToString()
    End Function
    Private Function CreatePostBody(ByVal title As String, ByVal description As String, ByVal keywords As String, ByVal mycategory As String) As String
        Dim myVideoXML As String = CreateXMLDOC(title, description, keywords, mycategory)

        Dim myPostBody As New StringBuilder()
        myPostBody.Append("--" & _myboundary & ControlChars.CrLf)
        myPostBody.Append("Content-Type: application/atom+xml; charset=UTF-8" & ControlChars.CrLf & ControlChars.CrLf)
        myPostBody.Append(myVideoXML)
        myPostBody.Append(ControlChars.CrLf)
        myPostBody.Append("--" & _myboundary & ControlChars.CrLf)
        myPostBody.Append("Content-Type: video/avi" & ControlChars.CrLf)
        myPostBody.Append("Content-Transfer-Encoding: binary" & ControlChars.CrLf & ControlChars.CrLf)

        Return myPostBody.ToString()
    End Function

    Public Function UploadVideo(ByVal filepath As String, ByVal title As String, ByVal description As String, ByVal keywords As String, ByVal mycategory As String) As VideoDetailsAfterUploading
        myauthinfo = GetToken()
        Dim buffer() As Byte = CreateFileBuffer(filepath)
        Dim myPostBody As String = CreatePostBody(title, description, keywords, mycategory)
        Dim filebuffer() As Byte = CreateFileBuffer(filepath)

        Dim myWebRequest As HttpWebRequest = SendRequest("http://uploads.gdata.youtube.com/feeds/api/users/" & myauthinfo.UserNameReturned & "/uploads", myPostBody, buffer, myauthinfo, filepath)
        Dim myResult As String = GetResponse(myWebRequest)
        Return PopulateDetailsAfterUpload(myResult)
       

        'Return myResult
    End Function
    Private Function CreateFileBuffer(ByVal filePath As String) As Byte()
        Dim getFileData As New FileStream(filePath, FileMode.Open)
        Dim buffer(getFileData.Length) As Byte
        getFileData.Read(buffer, 0, getFileData.Length - 1)
        getFileData.Close()

        Return buffer
    End Function
    Private Function GetToken() As AuthTokenReturn
        Dim myURL As String = ("https://www.google.com/youtube/accounts/ClientLogin")

        Dim myPostBody As New StringBuilder()
        With myPostBody
            .Append(String.Format("Email={0}&Passwd={1}&service=youtube&source={2}", HttpUtility.UrlEncode(_email), HttpUtility.UrlEncode(_pw), HttpUtility.UrlEncode(_appName)))
        End With
        Dim myWebRequest As HttpWebRequest = SendRequest(myURL, myPostBody.ToString(), "application/x-www-form-urlencoded", "POST")
        Dim myResult As String = GetResponse(myWebRequest)

        Dim extractInfo() As String = myResult.Split("=")
        Dim ReturnInfo As New AuthTokenReturn()
        ReturnInfo.AuthToken = extractInfo(1).Remove(extractInfo(1).IndexOf("YouTubeUser")).Trim()
        ReturnInfo.UserNameReturned = extractInfo(2).Trim()
        Return ReturnInfo
    End Function

    Private Overloads Function SendRequest(ByVal myUrlToPostTo As String, ByVal myBody As String, ByVal filebuffer() As Byte, ByVal myauthInfo As AuthTokenReturn, ByVal filepath As String) As HttpWebRequest

        Dim buffer() As Byte = System.Text.Encoding.UTF8.GetBytes(myBody.ToString())
        Dim endBuffer() As Byte = System.Text.Encoding.UTF8.GetBytes(ControlChars.CrLf & "--" & _myboundary & "--")

        Dim myWebClient As HttpWebRequest = DirectCast(WebRequest.Create(myUrlToPostTo), HttpWebRequest)
        myWebClient.Method = "POST"
        myWebClient.ContentType = String.Format("multipart/related; boundary={0};", _myboundary)
        myWebClient.ContentLength = buffer.Length + filebuffer.Length + endBuffer.Length
        myWebClient.Headers.Add("Authorization", "GoogleLogin auth=" + myauthInfo.AuthToken)
        myWebClient.Headers.Add("X-GData-Client", _clientID)
        myWebClient.Headers.Add("X-GData-Key", "key=" & _developerKey)
        myWebClient.Headers.Add("Slug", Path.GetFileName(filepath))

        Dim myRequestStream As Stream = myWebClient.GetRequestStream()
        myRequestStream.Write(buffer, 0, buffer.Length)
        myRequestStream.Write(filebuffer, 0, filebuffer.Length)
        myRequestStream.Write(endBuffer, 0, endBuffer.Length)
        myRequestStream.Close()
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
    Private Function GetResponse(ByVal myWebClient As HttpWebRequest) As String
        Dim myWebResponse As HttpWebResponse = DirectCast(myWebClient.GetResponse(), HttpWebResponse)
        Dim myStream As New StreamReader(myWebResponse.GetResponseStream)
        Return myStream.ReadToEnd()
    End Function


    Public Function GetCategories() As List(Of YTCategories)
        Dim myXPathDocument As New System.Xml.XPath.XPathDocument("http://gdata.youtube.com/schemas/2007/categories.cat")
        Dim myCatList As New List(Of YTCategories)
        Dim myNavigator As System.Xml.XPath.XPathNavigator = myXPathDocument.CreateNavigator()
        Dim myNS As New System.Xml.XmlNamespaceManager(myNavigator.NameTable)
        myNS.AddNamespace("atom", "http://www.w3.org/2005/Atom")
        myNS.AddNamespace("yt", "http://gdata.youtube.com/schemas/2007")
        Dim myNodeIterator As System.Xml.XPath.XPathNodeIterator = myNavigator.Select("//atom:category[yt:assignable]/@term", myNS)
        myNodeIterator.MoveNext()
        For Each myitem As System.Xml.XPath.XPathNavigator In myNodeIterator
            myCatList.Add(New YTCategories(myitem.Value))
        Next
        Return myCatList


    End Function

    Private Function PopulateDetailsAfterUpload(ByVal myResponseAfterUpload As String) As VideoDetailsAfterUploading
        Dim myXMLReader As New System.Xml.XmlTextReader(myResponseAfterUpload, System.Xml.XmlNodeType.Document, Nothing)

        Dim myXPathDocument As New System.Xml.XPath.XPathDocument(myXMLReader)
        Dim myNavigator As System.Xml.XPath.XPathNavigator = myXPathDocument.CreateNavigator()
        Dim myNS As New System.Xml.XmlNamespaceManager(myNavigator.NameTable)
        myNS.AddNamespace("media", "http://search.yahoo.com/mrss/")
        Dim myNodeIterator As System.Xml.XPath.XPathNavigator = myNavigator.SelectSingleNode("//media:group/media:player/@url", myNS)
        Dim myDetails As New VideoDetailsAfterUploading()
        myDetails.UploadedUrl = myNodeIterator.Value.Remove(myNodeIterator.Value.IndexOf("&feature="))
        myNodeIterator = myNavigator.SelectSingleNode("//media:group/media:title", myNS)
        myDetails.UploadedTitle = myNodeIterator.Value
        Return myDetails
    End Function

    Public Structure VideoDetailsAfterUploading
        Dim UploadedUrl As String
        Dim UploadedTitle As String
    End Structure

End Class

Public Class YTCategories
    Private _category As String = String.Empty
    Private _term As String = String.Empty

    Public Sub New(ByVal myCat As String)
        _category = myCat
        _term = term
    End Sub
    
    Public ReadOnly Property Term() As String
        Get
            Return _term
        End Get
    End Property
    Public ReadOnly Property Category() As String
        Get
            Return _category
        End Get

    End Property



End Class


