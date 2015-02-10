Imports System.Runtime.Serialization
Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class VQQuery
    Inherits TimeSeriesQuery

    Private _Metric As Metric = Metric.Manhattan
    Private _MetricType As Metric.MetricType = Metric.MetricType.Manhattan

    Private _SplitFactor As Double
    Private _MinDistortion As Double
    Private _CodeBook As Codebook
    'Private _Quant As Double()

    Public Sub New(criteria As TimeSeries)
        MyBase.New(criteria)
        SetDefaults()
    End Sub

    Private Sub SetDefaults()
        _SplitFactor = 1.5 ' split factor (should be in the range of 0.01 <= SPLIT <= 0.05)
        _MinDistortion = 0.75
        _Metric = Metric.Manhattan
        _MetricType = Metric.MetricType.Manhattan
    End Sub

    Protected Overrides Sub OnQueryInit(query As TimeSeries)
        If _CodeBook Is Nothing Then GenerateCodebook()
        MyBase.OnQueryInit(query)
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double

        Dim points As New List(Of PointVector)
        Dim dist As Double
        For i As Integer = 0 To data.Frames.Count - 1
            points.Add(New PointVector(data.Frames(i), i))
        Next
        dist = (1 / (1 + _CodeBook.GetDistortion(points)))
        'dist = (1 / (1 + _Metric.Compute(_Quant, _CodeBook.Quantize(points))))

        Return dist
    End Function

    Protected Overrides Sub OnQueryUpdate(query As TimeSeries)
        GenerateCodebook()
    End Sub

    Protected Overrides Sub OnQueryReset(query As TimeSeries)
        SetDefaults()
        GenerateCodebook()
    End Sub

    Protected Overrides Sub OnQueryCriteriaChanged(criteria As TimeSeries)
        GenerateCodebook()
    End Sub

    Private Sub GenerateCodebook()
        Dim points As New List(Of PointVector)
        For j As Integer = 0 To Criteria.Frames.Length - 1
            points.Add(New PointVector(Criteria.Frames(j), j))
        Next
        _CodeBook = New Codebook(points, _SplitFactor, _MinDistortion, _Metric)
        '_Quant = _CodeBook.Quantize(points)

    End Sub

    Public Property SplitFactor As Double
        Get
            Return _SplitFactor
        End Get
        Set(value As Double)
            If value <> _SplitFactor Then
                _SplitFactor = value
                GenerateCodebook()
            End If
        End Set
    End Property

    Public Property MinDistortion As Double
        Get
            Return _MinDistortion
        End Get
        Set(value As Double)
            If value <> _MinDistortion Then
                _MinDistortion = value
                GenerateCodebook()
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
                Me.GenerateCodebook()
            End If
        End Set
    End Property

    Public ReadOnly Property Centroids As Integer
        Get
            If _CodeBook Is Nothing Then GenerateCodebook()
            Return _CodeBook.Centroids.Count
        End Get
    End Property

    Public ReadOnly Property Points As Integer
        Get
            If _CodeBook Is Nothing Then GenerateCodebook()
            Return _CodeBook.TrainingPoints.Count
        End Get
    End Property

    Public ReadOnly Property Distortion As Double
        Get
            If _CodeBook Is Nothing Then GenerateCodebook()
            Return _CodeBook.Distortion
        End Get
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "VQ (Post)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Vector Quantization Query"
        End Get
    End Property

End Class
