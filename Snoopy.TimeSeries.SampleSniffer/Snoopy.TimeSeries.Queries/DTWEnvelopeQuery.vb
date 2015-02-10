Imports System.Runtime.Serialization
Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class DTWEnvelopeQuery
    Inherits TimeSeriesQuery

    Private _Metric As Metric = Metric.Manhattan
    Private _MetricType As Metric.MetricType = Metric.MetricType.Manhattan
    Private _Direction As DTWDirection = DTWDirection.Diagonals
    Private _QueryDistEnvMatrix As Double()()

    Public Sub New(query As TimeSeries)
        MyBase.New(query)
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double

        Dim dataDistMatrix As Double()() = DTW.CalculateCostMatrix(data.Frames, data.Frames, _Metric)
        Dim path As DTWPath = DTW.Calculate(dataDistMatrix, _QueryDistEnvMatrix, _Metric, _Direction)

        Return (1 / (1 + path.Distance))
    End Function

    Protected Overrides Sub OnQueryUpdate(query As TimeSeries)
        UpdateEnvelope()
    End Sub

    Protected Overrides Sub OnQueryInit(query As TimeSeries)
        UpdateEnvelope()
        MyBase.OnQueryInit(query)
    End Sub

    Protected Overrides Sub OnQueryReset(query As TimeSeries)
        SetDefaults()
        UpdateEnvelope()
    End Sub

    Private Sub SetDefaults()
        _Metric = Metric.Manhattan
        _MetricType = Metric.MetricType.Manhattan
    End Sub

    Private Sub UpdateEnvelope()
        _QueryDistEnvMatrix = DTW.CalculateCostMatrix(Criteria.Frames, Criteria.Frames, _Metric)
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
                Me.UpdateEnvelope()
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
            Return "DTWEnv (Post)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Dynamic Time Warping - Envelope distance."
        End Get
    End Property


End Class
