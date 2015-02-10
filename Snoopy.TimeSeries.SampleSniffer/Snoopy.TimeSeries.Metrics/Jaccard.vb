''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Jaccard
    Inherits Metric

    Protected Overrides Function OnCompute(v1() As Double, v2() As Double) As Double
        Dim num2 As Integer = 0
        Dim num3 As Integer = 0
        Dim index As Integer = 0
        Dim length As Integer = v1.Length
        Do While (index < length)
            If ((v1(index) <> 0) OrElse (v2(index) <> 0)) Then
                If (v1(index) = v2(index)) Then
                    num2 += 1
                End If
                num3 += 1
            End If
            index += 1
        Loop
        If (num3 <> 0) Then
            Return (1 - (CDbl(num2) / CDbl(num3)))
        End If
        Return 0
    End Function

End Class
