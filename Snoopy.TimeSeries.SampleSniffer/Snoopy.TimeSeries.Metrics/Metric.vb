
Public MustInherit Class Metric
    Implements ITimeSeriesMetric

    Public Shared ReadOnly Euclidean As Metric = New Euclidean
    Public Shared ReadOnly Hamming As Metric = New Hamming
    Public Shared ReadOnly Manhattan As Metric = New Manhattan
    Public Shared ReadOnly Minkowski As Metric = New Minkowski
    Public Shared ReadOnly Chebyshev As Metric = New Chebyshev
    Public Shared ReadOnly Cosine As Metric = New Cosine
    Public Shared ReadOnly Intersection As Metric = New Intersection
    Public Shared ReadOnly Canberra As Metric = New Canberra
    Public Shared ReadOnly Clark As Metric = New Clark
    Public Shared ReadOnly BrayCurtis As Metric = New BrayCurtis
    Public Shared ReadOnly Lorentzian As Metric = New Lorentzian
    Public Shared ReadOnly Kulczynski1 As Metric = New Kulczynski1
    Public Shared ReadOnly JeffreyDivergence As Metric = New JeffreyDivergence
    Public Shared ReadOnly PearsonCorrelation As Metric = New PearsonCorrelation
    Public Shared ReadOnly Jaccard As Metric = New Jaccard
    Public Shared ReadOnly SimpleParabolicRegression As Metric = New LSR

    Public Function Compute(v1() As Double, v2() As Double) As Double Implements ITimeSeriesMetric.Compute
        If (v1.Length <> v2.Length) Then
            Throw New ArgumentException("Input vectors must be of the same length.")
        End If
        Return OnCompute(v1, v2)
    End Function

    Protected MustOverride Function OnCompute(v1 As Double(), v2 As Double()) As Double

    Public Shared Function [Get](metric As MetricType) As Metric
        Select Case metric
            Case MetricType.Euclidean
                Return Euclidean
            Case MetricType.Intersection
                Return Intersection
            Case MetricType.Manhattan
                Return Manhattan
            Case MetricType.Minkowski
                Return Minkowski
            Case MetricType.Chebyshev
                Return Chebyshev
            Case MetricType.Cosine
                Return Cosine
            Case MetricType.Canberra
                Return Canberra
            Case MetricType.Clark
                Return Clark
            Case MetricType.BrayCurtis
                Return BrayCurtis
            Case MetricType.Lorentzian
                Return Lorentzian
            Case MetricType.Kulczynski1
                Return Kulczynski1
            Case MetricType.PearsonCorrelation
                Return PearsonCorrelation
            Case MetricType.Jaccard
                Return Jaccard
            Case MetricType.Hamming
                Return Hamming
            Case MetricType.SimpleParabolicRegression
                Return SimpleParabolicRegression
            Case Else
                Return Euclidean
        End Select
    End Function

    Public Enum MetricType
        Euclidean
        Intersection
        Manhattan
        Minkowski
        Chebyshev
        Cosine
        Canberra
        Clark
        BrayCurtis
        Lorentzian
        Kulczynski1
        SimpleParabolicRegression
        PearsonCorrelation
        Jaccard
        Hamming
    End Enum

End Class
