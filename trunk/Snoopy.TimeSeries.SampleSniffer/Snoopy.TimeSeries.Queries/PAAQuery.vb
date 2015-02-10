Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Serializable>
Public Class PAAQuery
    Inherits TimeSeriesQuery

    Private _PAAQuery As Double()
    Private _Width As Integer = 32
    Private _Metric As Metric = Metric.Euclidean
    Private _MetricType As Metric.MetricType = Metric.MetricType.Euclidean

    Public Sub New(query As TimeSeries)
        MyBase.New(query)
        GeneratePAAQuery()
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double
        Dim dataPAA As Double() = Transform(data.Samples, Me.Width)
        Dim dist As Double = _Metric.Compute(_PAAQuery, dataPAA)
        Return (1 / (1 + dist))
    End Function

    Protected Overrides Sub OnQueryUpdate(query As TimeSeries)
        GeneratePAAQuery()
    End Sub

    Protected Overrides Sub OnQueryReset(query As TimeSeries)
        _Width = 32
        _Metric = Metric.Euclidean
        _MetricType = Metric.MetricType.Euclidean
    End Sub

    Protected Overrides Sub OnQueryCriteriaChanged(criteria As TimeSeries)
        GeneratePAAQuery()
    End Sub

    Private Sub GeneratePAAQuery()
        _PAAQuery = Transform(MyBase.Criteria.Samples, Me.Width)
    End Sub

    Public Property Width As Integer
        Get
            Return _Width
        End Get
        Set(value As Integer)
            If _Width <> value Then
                _Width = value
                GeneratePAAQuery()
            End If
        End Set
    End Property

    <Description("Metric")>
    Public Property MetricType As Metric.MetricType
        Get
            Return _MetricType
        End Get
        Set(value As Metric.MetricType)
            If value <> _MetricType Then
                _MetricType = value
                _Metric = Metric.Get(_MetricType)
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "PAA (Pre)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Piecewise Aggregate Approximation"
        End Get
    End Property

    Private Shared Function Transform(samples As Double(), paaWindowWidth As Integer) As Double()

        If paaWindowWidth >= samples.Length Then Return samples

        Dim t As Double()() = New Double(paaWindowWidth - 1)() {}
        For i As Integer = 0 To paaWindowWidth - 1
            t(i) = New Double(samples.Length - 1) {}
            For j As Integer = 0 To samples.Length - 1
                t(i)(j) = samples(j)
            Next
        Next

        Dim exp As Double()() = Matrix.Reshape(t, 1, samples.Length * paaWindowWidth)
        Dim res As Double()() = Matrix.Reshape(exp, samples.Length, paaWindowWidth)

        Return Matrix.GetColumnMeans(res)

    End Function

End Class
