''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Minkowski
    Inherits Metric

    Private Const P As Double = 0.33

    Protected Overrides Function OnCompute(v1 As Double(), v2 As Double()) As Double
        Dim dist As Double
        For i As Integer = 0 To v1.Length - 1
            dist += Math.Pow(Math.Abs(v1(i) - v2(i)), P)
        Next
        Return dist
    End Function

End Class
