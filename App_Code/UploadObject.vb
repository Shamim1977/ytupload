Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Net
Imports System.Xml.Linq


Public Class UploadObject
    Private _message As String = String.Empty
    Private _source As String = String.Empty
    Private _code As HttpStatusCode = Nothing


    Public Property Message() As String
        Get
            Return _message
        End Get
        Set(ByVal value As String)
            _message = value
        End Set
    End Property

    Public Property Source() As String
        Get
            Return _source
        End Get
        Set(ByVal value As String)
            _source = value
        End Set
    End Property

    Public Property Code() As HttpStatusCode
        Get
            Return _code
        End Get
        Set(ByVal value As HttpStatusCode)
            _code = value

        End Set
    End Property

    Public ReadOnly Property ErrorCodeToString() As String
        Get
            Return _code.ToString()

        End Get
    End Property
End Class
