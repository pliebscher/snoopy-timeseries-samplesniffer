''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Chebyshev
    Inherits Metric

    Protected Overrides Function OnCompute(v1 As Double(), v2 As Double()) As Double
        Dim dist As Double
        Dim max As Double
        For i As Integer = 0 To v1.Length - 1
            dist = Math.Abs(v1(i) - v2(i))
            If dist > max Then max = dist
        Next
        Return max
    End Function

End Class
