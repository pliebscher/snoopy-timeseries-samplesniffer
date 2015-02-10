Namespace Windows
    Public Class Gaussian
        Inherits Window

        Public Overrides Function GetWindow(N As Integer) As Double()
            Dim window As Double() = New Double(N - 1) {}
            Dim start As Integer = (window.Length - N) \ 2
            Dim [stop] As Integer = (window.Length + N) \ 2
            Dim delta As Double = 5.0 / N
            Dim x As Double = (1 - N) / 2.0 * delta
            Dim c As Double = -Math.PI * Math.Exp(1.0) / 10.0
            Dim sum As Double = 0

            For i As Integer = start To [stop] - 1
                window(i) = Math.Exp(c * x * x)
                x += delta
                sum += window(i)
            Next

            For i As Integer = start To [stop] - 1
                window(i) /= sum
            Next
            Return window
        End Function

    End Class
End Namespace

