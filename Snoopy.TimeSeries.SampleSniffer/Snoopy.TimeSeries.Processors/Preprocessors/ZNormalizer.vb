''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class ZNormalizer
    Inherits TimeSeriesPreprocessor

    Private _Method As _NormMethod = _NormMethod.MeanStdDev

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Select Case _Method
            Case _NormMethod.MeanStdDev
                Return MeanStdDev(series)
            Case Else
                Return SumSquared(series)
        End Select
    End Function

    Public Shared Function MeanStdDev(series As ITimeSeries(Of Double())) As Double()
        Dim zNormSamples As Double() = New Double(series.Samples.Length - 1) {}
        Dim mean As Double = series.Samples.Average
        Dim sd As Double = series.StdDev
        For i As Integer = 0 To series.Samples.Length - 1
            zNormSamples(i) = (series.Samples(i) - mean) / sd
        Next
        Return zNormSamples
    End Function

    Public Shared Function SumSquared(series As ITimeSeries(Of Double())) As Double()
        Dim norm As Double() = New Double(series.Samples.Length - 1) {}
        Dim inv As Double = 1 / Math.Sqrt(series.Samples.Sum)
        For i As Integer = 0 To series.Samples.Length - 1
            norm(i) = series.Samples(i) * inv
        Next
        Return norm
    End Function

    Enum _NormMethod
        MeanStdDev
        SumSquared
    End Enum

    Public Property Method As _NormMethod
        Get
            Return _Method
        End Get
        Set(value As _NormMethod)
            If value <> _Method Then
                _Method = value
            End If
        End Set
    End Property

End Class


