''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class LinearTrend
    Inherits TimeSeriesPreprocessor

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim trend As Double() = New Double(series.Samples.Length - 1) {}
        Dim coeffs As Tuple(Of Double, Double) = GetLinearTrendCoeff(series.Samples)
        Dim a As Double = coeffs.Item1
        Dim b As Double = coeffs.Item2
        Dim i As Integer
        For i = 0 To series.Samples.Length - 1
            Dim num4 As Double = ((a * i) + b)
            trend(i) = (series.Samples(i) - num4)
        Next i
        Return trend

    End Function

    Private Shared Function GetLinearTrendCoeff(ByVal values As Double()) As Tuple(Of Double, Double)
        Dim num As Double = 0
        Dim num2 As Double = 0
        Dim num3 As Double = 0
        Dim x As Double = 0
        Dim num5 As Double = 0
        Dim length As Integer = values.Length

        For i As Integer = 0 To length - 1
            Dim num8 As Double = values(i)
            x += i
            num += Math.Pow(num8, 2)
            num2 += Math.Pow(CDbl(i), 2)
            num3 += num8
            num5 += (i * num8)
        Next

        Dim a As Double = (((length * num5) - (x * num3)) / ((length * num2) - Math.Pow(x, 2)))
        Dim b As Double = (((num3 * num2) - (x * num5)) / ((length * num2) - Math.Pow(x, 2)))

        Return New Tuple(Of Double, Double)(a, b)

    End Function

End Class
