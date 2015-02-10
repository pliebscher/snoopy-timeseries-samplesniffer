Public Class FIRLowPassFilter
    Inherits FIRFilter

    Private _Cutoff As Double = 500.0

    Protected Overrides Function OnUpdateCoefficients(sampleRate As Double, halfOrder As Integer) As Double()
        Dim nu As Double = 2.0 * _Cutoff / sampleRate
        Dim order As Integer = 2 * halfOrder + 1
        Dim c As Double() = New Double(order - 1) {}
        c(halfOrder) = nu
        Dim n As Integer = halfOrder
        For i As Integer = 0 To halfOrder - 1
            Dim npi As Double = n * Math.PI
            c(i) = Math.Sin(npi * nu) / npi
            c(n + halfOrder) = c(i)
            n -= 1
        Next
        Return c
    End Function

    Public Property Cutoff As Double
        Get
            Return _Cutoff
        End Get
        Set(value As Double)
            If value < 0 Then
                Throw New ArgumentOutOfRangeException("Cutoff must be greater than 0.")
            End If
            If value <> _Cutoff Then
                _Cutoff = value
                UpdateCoefficients()
            End If
        End Set
    End Property

End Class
