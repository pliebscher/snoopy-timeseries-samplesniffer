Imports System.Runtime.Serialization
Imports System.Security.Permissions

<Serializable()>
Public Class TimeSeriesQueryException
    Inherits ApplicationException

    Private _Query As TimeSeriesQuery

    Public Sub New()
    End Sub

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, query As TimeSeriesQuery)
        MyBase.New(message)
        _Query = query
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As Exception)
        MyBase.New(message, innerException)
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As Exception, query As TimeSeriesQuery)
        MyBase.New(message, innerException)
        _Query = query
    End Sub

    Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        MyBase.New(info, context)
    End Sub

    <SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter:=True)> _
    Public Overrides Sub GetObjectData(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        MyBase.GetObjectData(info, context)
    End Sub

    Public ReadOnly Property Query As TimeSeriesQuery
        Get
            Return _Query
        End Get
    End Property

End Class
