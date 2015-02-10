''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Manhattan
    Inherits Metric

    Protected Overrides Function OnCompute(v1 As Double(), v2 As Double()) As Double
        Dim dist As Double
        For i As Integer = 0 To v1.Length - 1
            dist += Math.Abs(v1(i) - v2(i))
        Next
        Return dist
    End Function

End Class
