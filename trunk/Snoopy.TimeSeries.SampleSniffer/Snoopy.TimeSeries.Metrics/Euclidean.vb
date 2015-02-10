''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Euclidean
    Inherits Metric

    Protected Overrides Function OnCompute(v1 As Double(), v2 As Double()) As Double
        Dim dist As Double
        For i As Integer = 0 To v1.Length - 1
            dist += Math.Pow(v1(i) - v2(i), 2.0)
        Next
        Return Math.Sqrt(dist)
    End Function

End Class

