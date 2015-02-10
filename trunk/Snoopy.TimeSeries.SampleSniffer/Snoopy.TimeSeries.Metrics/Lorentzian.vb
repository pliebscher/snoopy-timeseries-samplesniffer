''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Lorentzian
    Inherits Metric

    Protected Overrides Function OnCompute(v1() As Double, v2() As Double) As Double
        Dim dim1 As Integer = v1.Length
        Dim sum As Double = 0.0
        For i As Integer = 0 To dim1 - 1
            Dim xi As Double = v1(i)
            Dim yi As Double = v2(i)
            sum += Math.Log(1 + Math.Abs(xi - yi))
        Next
        Return sum
    End Function

End Class
