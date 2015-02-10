''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class JeffreyDivergence
    Inherits Metric

    Protected Overrides Function OnCompute(v1() As Double, v2() As Double) As Double
        Dim dim1 As Integer = v1.Length
        Dim dist As Double = 0
        For i As Integer = 0 To dim1 - 1
            Dim xi As Double = v1(i)
            Dim yi As Double = v2(i)
            Dim mi As Double = 0.5 * (xi + yi)
            dist += xi * Math.Log(xi / mi)
            dist += yi * Math.Log(yi / mi)
        Next
        Return dist
    End Function

End Class
