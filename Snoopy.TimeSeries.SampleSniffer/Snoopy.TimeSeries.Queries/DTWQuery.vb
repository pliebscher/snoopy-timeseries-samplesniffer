Imports System.Runtime.Serialization
Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<DefaultProperty("MetricType")>
Public Class DTWQuery
    Inherits TimeSeriesQuery

    Private _Metric As Metric = Metric.Manhattan
    Private _MetricType As Metric.MetricType = Metric.MetricType.Manhattan
    Private _Direction As DTWDirection = DTWDirection.Neighbors

    Public Sub New(query As TimeSeries)
        MyBase.New(query)
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double

        Dim path As DTWPath = DTW.Calculate(data.Frames, Criteria.Frames, _Metric, _Direction)
        Dim dist As Double = (1 / (1 + path.Distance))

        Return dist
    End Function

    Protected Overrides Sub OnQueryReset(query As TimeSeries)
        _Metric = Metric.Manhattan
        _MetricType = Metric.MetricType.Manhattan
        _Direction = DTWDirection.Neighbors
    End Sub

    <Description("Metric"), DefaultValue(Metric.MetricType.Manhattan)>
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

    <Description("Warp direction"), DefaultValue(DTWDirection.Neighbors)>
    Public Property Direction As DTWDirection
        Get
            Return _Direction
        End Get
        Set(value As DTWDirection)
            If value <> _Direction Then
                _Direction = value
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "DTW (Post)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Dynamic Time Warping"
        End Get
    End Property

End Class
