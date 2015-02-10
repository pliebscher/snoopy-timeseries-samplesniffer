Namespace Windows
    Public Class EmptyWindow
        Inherits Window

        Public Overrides Function GetWindow(length As Integer) As Double()
            Dim w As Double() = New Double(length - 1) {}
            For i As Integer = 0 To length - 1
                w(i) = 1
            Next
            Return w
        End Function

    End Class
End Namespace

