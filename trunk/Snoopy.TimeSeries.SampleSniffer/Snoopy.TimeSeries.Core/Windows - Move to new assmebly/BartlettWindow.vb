Namespace Windows
    Public Class BartlettWindow
        Inherits Window

        Public Overrides Function GetWindow(ByVal N As Integer) As Double()
            Dim w As Double() = New Double(N - 1) {}
            For i As Integer = 0 To N - 1
                w(i) = 1.0 - (2.0 * Math.Abs(i - (N - 1) / 2.0)) / (N - 1)
            Next
            Return w
        End Function

    End Class
End Namespace

