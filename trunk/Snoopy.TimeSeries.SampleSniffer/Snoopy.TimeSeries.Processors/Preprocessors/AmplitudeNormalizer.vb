''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class AmplitudeNormalizer
    Inherits TimeSeriesPreprocessor

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim num As Double = series.StdDev
        If (num = 0) Then
            Return series.Samples
        End If
        Dim norm As Double() = New Double(series.Samples.Length - 1) {}
        For i As Integer = 0 To series.Samples.Length - 1
            norm(i) = (series.Samples(i) / num)
        Next i
        Return norm
    End Function

End Class
