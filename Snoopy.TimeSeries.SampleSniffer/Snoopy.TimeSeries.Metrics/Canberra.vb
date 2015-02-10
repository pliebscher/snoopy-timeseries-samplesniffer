''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Canberra
    Inherits Metric

    Protected Overrides Function OnCompute(v1() As Double, v2() As Double) As Double
        Dim d As Integer = v1.Length
        Dim sum As Double = 0.0
        For i As Integer = 0 To d - 1
            Dim a As Double = v1(i)
            Dim b As Double = v2(i)
            Dim div As Double = Math.Abs(a) + Math.Abs(b)
            If (div > 0) Then
                sum += Math.Abs(a - b) / div
            End If
        Next
        Return sum
    End Function

End Class
