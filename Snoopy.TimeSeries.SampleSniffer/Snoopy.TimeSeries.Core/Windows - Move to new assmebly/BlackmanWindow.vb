Namespace Windows
    Public Class BlackmanWindow
        Inherits Window

        Public Overrides Function GetWindow(ByVal N As Integer) As Double()
            Dim w As Double() = New Double(N - 1) {}
            For i As Integer = 0 To N - 1
                w(i) = 0.42 - 0.5 * Math.Cos(2.0 * Math.PI * i / N - 1) + 0.08 * Math.Cos(4.0 * Math.PI * i / N - 1)
            Next
            Return w
        End Function

    End Class
End Namespace

