''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class PearsonCorrelation
    Inherits Metric

    Protected Overrides Function OnCompute(v1() As Double, v2() As Double) As Double
        Dim num1 As Double = 0
        Dim num2 As Double = 0
        Dim num3 As Double = 0
        Dim num4 As Double = 0
        Dim num5 As Double = 0
        Dim length As Double = v1.Length
        Dim i As Integer
        For i = 0 To v1.Length - 1
            Dim num6 As Double = v1(i)
            Dim num7 As Double = v2(i)
            num1 = (num1 + num6)
            num2 = (num2 + num7)
            num3 = (num3 + (num6 * num6))
            num4 = (num4 + (num7 * num7))
            num5 = (num5 + (num6 * num7))
        Next i
        Dim num10 As Double = (num5 - ((num1 * num2) / length))
        Dim num11 As Double = Math.Sqrt(((num3 - ((num1 * num1) / length)) * (num4 - ((num2 * num2) / length))))
        If (num11 <> 0) Then
            Return (num10 / num11)
        End If
        Return 0
    End Function

End Class
