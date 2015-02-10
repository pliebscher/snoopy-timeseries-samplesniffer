''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Intersection
    Inherits Metric

    Protected Overrides Function OnCompute(v1() As Double, v2() As Double) As Double
        Dim intersectionSum As Double
        Dim thisArea As Double
        Dim otherArea As Double
        For i As Integer = 0 To v1.Length - 1
            intersectionSum += Math.Min(v1(i), v2(i))
            thisArea += v1(i)
            otherArea += v2(i)
        Next
        Dim area As Double = Math.Max(thisArea, otherArea)
        Return 1 - intersectionSum / area
    End Function

End Class
