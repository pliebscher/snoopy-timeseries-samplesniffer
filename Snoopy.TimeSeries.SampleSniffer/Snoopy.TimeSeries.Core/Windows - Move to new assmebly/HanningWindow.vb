Namespace Windows
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class HanningWindow
        Inherits Window

        Public Overrides Function GetWindow(N As Integer) As Double()
            Dim w As Double() = New Double(N - 1) {}
            For i As Integer = 0 To N - 1
                w(i) = 0.5 * (1 - Math.Cos(2 * Math.PI * i / (N - 1))) ' "\" -> "/"
            Next
            Return w
        End Function

    End Class
End Namespace


