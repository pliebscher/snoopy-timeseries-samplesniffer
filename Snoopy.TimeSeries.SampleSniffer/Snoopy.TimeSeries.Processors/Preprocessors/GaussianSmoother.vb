''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class GaussianSmoother
    Inherits TimeSeriesPreprocessor

    Private _FilterWidth As Double = 32
    Private _Sigma As Double
    Private _MaxShift As Integer
    Private _NormalizingCoef As Double

    Public Sub New()
        Reset()
    End Sub

    Private Sub Reset()
        _Sigma = _FilterWidth / 2
        _MaxShift = CInt(Math.Floor(4 * _Sigma))
        _NormalizingCoef = Math.Sqrt(2 * Math.PI) * _Sigma
    End Sub

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()

        Dim smoothedSamples As Double() = New Double(series.Samples.Length - 1) {}

        For i As Integer = 0 To smoothedSamples.Length - 1
            smoothedSamples(i) = series.Samples(i)

            If _MaxShift < 1 Then Continue For

            For j As Integer = 1 To _MaxShift - 1

                Dim gaussFilt As Double = Math.Exp(-(j * j) / (2 * _Sigma * _Sigma))
                Dim leftAmp As Double
                Dim rightAmp As Double

                If (i - j) >= 0 Then
                    leftAmp = series.Samples(i - j)
                Else
                    leftAmp = series.Samples(i)
                End If

                If (i + j) <= smoothedSamples.Length - 1 Then
                    rightAmp = series.Samples(i + j)
                Else
                    rightAmp = series.Samples(i)
                End If

                smoothedSamples(i) += gaussFilt * (leftAmp + rightAmp)

            Next

            smoothedSamples(i) /= _NormalizingCoef

        Next

        Return smoothedSamples

    End Function

    Public Property FilterWidth As Double
        Get
            Return _FilterWidth
        End Get
        Set(value As Double)
            If CInt(Math.Floor(4 * (value / 2))) < 1 Then Throw New ArgumentOutOfRangeException("Filter width to small.")
            If value <> _FilterWidth Then
                _FilterWidth = value
                Reset()
            End If
        End Set
    End Property

End Class
