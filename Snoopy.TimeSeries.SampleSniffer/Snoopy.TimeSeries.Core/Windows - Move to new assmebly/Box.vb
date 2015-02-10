Namespace Windows
    Public Class Box
        Inherits Window

        Public Overrides Function GetWindow(ByVal N As Integer) As Double()
            Dim w As Double() = New Double(N - 1) {}
            Dim i As Integer
            For i = 0 To N - 1 Step 2
                w(i) = 1.0
                w(i + 1) = 0.0
            Next
            Return w
        End Function

    End Class
End Namespace

