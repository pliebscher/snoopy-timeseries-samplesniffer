''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Cosine
    Inherits Metric

    Protected Overrides Function OnCompute(v1 As Double(), v2 As Double()) As Double

        Dim num As Double = 0
        Dim num2 As Double = 0
        Dim d As Double = 0
        Dim num4 As Double = 0
        Dim index As Integer = 0
        Dim length As Integer = v1.Length
        Do While (index < length)
            Dim num5 As Double = v1(index)
            Dim num6 As Double = v2(index)
            num2 = (num2 + (num5 * num6))
            d = (d + (num5 * num5))
            num4 = (num4 + (num6 * num6))
            index += 1
        Loop
        num = (Math.Sqrt(d) * Math.Sqrt(num4))
        If (num <> 0) Then
            Return 1 - (num2 / num)
        End If
        Return 0
    End Function

End Class
