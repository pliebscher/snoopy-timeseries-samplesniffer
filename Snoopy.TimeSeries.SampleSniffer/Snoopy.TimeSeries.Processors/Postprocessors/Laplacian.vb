''' <summary>
''' http://cs.brown.edu/~pff/bp/
''' </summary>
''' <remarks></remarks>
Public Class Laplacian
    Inherits TimeSeriesPostProcessor

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        Dim width As Integer = series.Samples(0).Length
        Dim height As Integer = series.Samples.Length
        Dim laplace As Double()() = New Double(height - 1)() {}
        Dim y As Integer
        Dim x As Integer

        For i As Integer = 0 To height - 1
            laplace(i) = New Double(width - 1) {}
        Next

        For x = 1 To height - 2
            For y = 1 To width - 2
                Dim d2x As Double = series.Samples(x - 1)(y) + series.Samples(x + 1)(y) - 2 * series.Samples(x)(y)
                Dim d2y As Double = series.Samples(x)(y - 1) + series.Samples(x)(y + 1) - 2 * series.Samples(x)(y)
                laplace(x)(y) = d2x + d2y
            Next
        Next

        Return laplace
    End Function

End Class
