''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class SimpleSmoother
    Inherits TimeSeriesPreprocessor

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim length As Integer = series.Samples.Length
        Dim smoothed As Double() = New Double(length - 1) {}
        Array.Copy(series.Samples, smoothed, length)
        For i As Integer = 1 To (length - 1) - 1
            smoothed(i) = ((series.Samples((i - 1)) + series.Samples((i + 1))) / 2)
        Next i
        Return smoothed
    End Function

End Class
