''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Hamming
    Inherits Metric

    Protected Overrides Function OnCompute(v1() As Double, v2() As Double) As Double
        Dim num As Double = 0
        Dim index As Integer = 0
        Dim length As Integer = v1.Length
        Do While (index < length)
            If (v1(index) <> v2(index)) Then
                num += 1
            End If
            index += 1
        Loop
        Return num
    End Function

End Class
