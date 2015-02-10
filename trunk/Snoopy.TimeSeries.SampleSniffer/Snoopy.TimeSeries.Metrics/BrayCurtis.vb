''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class BrayCurtis
    Inherits Metric

    Protected Overrides Function OnCompute(v1() As Double, v2() As Double) As Double
        Dim dim1 As Integer = v1.Length
        Dim sumdiff As Double = 0.0
        Dim sumsum As Double = 0.0
        For i As Integer = 0 To dim1 - 1
            Dim xd As Double = v1(i)
            Dim yd As Double = v2(i)
            sumdiff += Math.Abs(xd - yd)
            sumsum += Math.Abs(xd) + Math.Abs(yd)
        Next
        Return sumdiff / sumsum
    End Function

End Class
