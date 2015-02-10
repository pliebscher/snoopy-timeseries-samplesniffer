Public Class IIRLowPassFilter
    Inherits IIRFilter

    Private _Cutoff As Double = 500.0

    Protected Overrides Function OnUpdateCoefficients(sampleRate As Double, width As Integer) As Double()

        Dim beta As Double, gamma As Double, theta As Double


        BetaGamma(beta, gamma, theta, sampleRate, _Cutoff, 0.0, width)

        'Return BuildCoefficients(beta, gamma, (0.5 + beta - gamma) * 0.25, 2, 1)
        Return New Double() {2.0 * ((0.5 + beta - gamma) * 0.25), 2.0 * gamma, -2.0 * beta, 1, 2, 1}

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

    Private Shared Function BuildCoefficients(beta As Double, gamma As Double, alpha As Double, mu As Double, sigma As Double) As Double()
        Return New Double() {2.0 * alpha, 2.0 * gamma, -2.0 * beta, 1, mu, sigma}
    End Function

    Private Shared Sub BetaGamma(ByRef beta As Double, ByRef gamma As Double, ByRef theta As Double, sampling As Double, cutoff As Double, lowHalfPower As Double, highHalfPower As Double)
        Dim tan As Double = Math.Tan(Math.PI * (highHalfPower - lowHalfPower) / sampling)
        beta = 0.5 * (1 - tan) / (1 + tan)
        theta = 2 * Math.PI * cutoff / sampling
        gamma = (0.5 + beta) * Math.Cos(theta)
    End Sub

End Class
