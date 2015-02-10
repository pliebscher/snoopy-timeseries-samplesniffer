Public Class FIRBandStopFilter
    Inherits FIRFilter

    Private _CutoffLow As Double = 500.0
    Private _CutoffHigh As Double = 2300.0

    Protected Overrides Function OnUpdateCoefficients(sampleRate As Double, halfOrder As Integer) As Double()
        Dim nu1 As Double = 2.0 * _CutoffLow / sampleRate
        Dim nu2 As Double = 2.0 * _CutoffHigh / sampleRate
        Dim order As Integer = 2 * halfOrder + 1
        Dim c As Double() = New Double(order - 1) {}
        c(halfOrder) = 1 - (nu2 - nu1)
        Dim i As Integer = 0, n As Integer = halfOrder
        While i < halfOrder
            Dim npi As Double = n * Math.PI
            c(i) = (Math.Sin(npi * nu1) - Math.Sin(npi * nu2)) / npi
            c(n + halfOrder) = c(i)
            i += 1
            n -= 1
        End While
        Return c
    End Function

    Public Property CutoffLow As Double
        Get
            Return _CutoffLow
        End Get
        Set(value As Double)
            If value < 0 Or value = _CutoffHigh Then
                Throw New ArgumentOutOfRangeException("CutoffLow must be greater than 0 and not equal to CutoffHigh.")
            End If
            If value <> _CutoffLow Then
                _CutoffLow = value
                UpdateCoefficients()
            End If
        End Set
    End Property

    Public Property CutoffHigh As Double
        Get
            Return _CutoffHigh
        End Get
        Set(value As Double)
            If value < 0 Or value = _CutoffHigh Then
                Throw New ArgumentOutOfRangeException("CutoffHigh must be greater than 0 and not equal to CutoffLow.")
            End If
            If value <> _CutoffHigh Then
                _CutoffHigh = value
                UpdateCoefficients()
            End If
        End Set
    End Property

End Class
