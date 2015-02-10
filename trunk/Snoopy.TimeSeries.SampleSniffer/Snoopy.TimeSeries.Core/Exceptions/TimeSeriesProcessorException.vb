Imports System.Runtime.Serialization
Imports System.Security.Permissions

<Serializable()>
Public Class TimeSeriesProcessorException
    Inherits ApplicationException

    Private _Processor As TimeSeriesProcessor

    Public Sub New()
    End Sub

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(processor As TimeSeriesProcessor)
        _Processor = processor
    End Sub

    Public Sub New(ByVal message As String, processor As TimeSeriesProcessor)
        MyBase.New(message)
        _Processor = processor
    End Sub

    Public Sub New(ByVal message As String, processor As TimeSeriesProcessor, ByVal innerException As Exception)
        MyBase.New(message, innerException)
        _Processor = processor
    End Sub

    Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        MyBase.New(info, context)
    End Sub

    <SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter:=True)> _
    Public Overrides Sub GetObjectData(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        MyBase.GetObjectData(info, context)
    End Sub

    Public ReadOnly Property Processor As TimeSeriesProcessor
        Get
            Return _Processor
        End Get
    End Property

    Public Overrides ReadOnly Property Message As String
        Get
            If _Processor IsNot Nothing Then
                Return _Processor.Name & ": " & MyBase.Message
            Else
                Return MyBase.Message
            End If
        End Get
    End Property

End Class
