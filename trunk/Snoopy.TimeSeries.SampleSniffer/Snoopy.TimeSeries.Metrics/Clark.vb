''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Clark
    Inherits Metric

    Protected Overrides Function OnCompute(v1() As Double, v2() As Double) As Double
        Dim dim1 As Integer = v1.Length
        Dim sqsum As Double = 0.0
        For i As Integer = 0 To dim1 - 1
            Dim xd As Double = v1(i)
            Dim yd As Double = v2(i)
            Dim v As Double = (xd - yd) / (Math.Abs(xd) + Math.Abs(yd))
            sqsum += v * v
        Next
        Return Math.Sqrt(sqsum / dim1)
    End Function

End Class
