''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class MeanOffset
    Inherits TimeSeriesPreprocessor

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim offset As Double() = New Double(series.Samples.Length - 1) {}
        Dim num As Double = series.Mean
        Dim i As Integer
        For i = 0 To series.Samples.Length - 1
            Dim num3 As Double = series.Samples(i)
            offset(i) = (num3 - num)
        Next i
        Return offset
    End Function

End Class
