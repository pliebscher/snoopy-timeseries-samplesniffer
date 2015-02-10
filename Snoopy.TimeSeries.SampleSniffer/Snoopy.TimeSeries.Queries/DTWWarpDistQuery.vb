Imports System.Runtime.Serialization
Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class DTWWarpDistQuery
    Inherits TimeSeriesQuery

    Private _Metric As Metric = Metric.Manhattan
    Private _MetricType As Metric.MetricType = Metric.MetricType.Manhattan

    Public Sub New(query As TimeSeries)
        MyBase.New(query)
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double

        Dim dist As Double = DTW.CalculateWarpCostBetween(data.Frames, Criteria.Frames, _Metric)

        Return (1 / (1 + dist))
    End Function

    Protected Overrides Sub OnQueryReset(query As TimeSeries)
        _Metric = Metric.Manhattan
        _MetricType = Metric.MetricType.Manhattan
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

    Public Overrides ReadOnly Property Name As String
        Get
            Return "DTWWarpDist (Post)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Dynamic Time Warping Cost Matrix distance."
        End Get
    End Property


End Class
