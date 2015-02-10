Namespace Windows
    Public Class FlattopWindow
        Inherits Window

        Public Overrides Function GetWindow(ByVal N As Integer) As Double()
            Dim w As Double() = New Double(N - 1) {}
            For i As Integer = 0 To N - 1
                w(i) = 1.0 - 1.93 * Math.Cos(2.0 * Math.PI * i / N - 1) + _
                           1.29 * Math.Cos(4.0 * Math.PI * i / N - 1) - _
                           0.388 * Math.Cos(6.0 * Math.PI * i / N - 1) + _
                           0.0322 * Math.Cos(8.0 * Math.PI * i / N - 1)
            Next
            Return w
        End Function

    End Class
End Namespace

