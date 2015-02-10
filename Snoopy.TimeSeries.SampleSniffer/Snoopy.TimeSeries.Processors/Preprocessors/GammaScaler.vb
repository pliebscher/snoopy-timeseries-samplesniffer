Imports System.ComponentModel
''' <summary>
''' http://elki.dbs.ifi.lmu.de/browser/elki/trunk/src/de/lmu/ifi/dbs/elki/utilities/scaling
''' </summary>
''' <remarks></remarks>
<Description("Gamma scaling function.")>
Public Class GammaScaler
    Inherits TimeSeriesPreprocessor

    Private _Gamma As Double = 2.0

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        'For i As Integer = 0 To series.Samples.Length - 1
        '    series.Samples(i) = Math.Pow(series.Samples(i), _Gamma)
        'Next
        extrapolate(series.Samples)
        Return series.Samples
    End Function

    <Description("Gamma value.")>
    Public Property Gamma As Double
        Get
            Return _Gamma
        End Get
        Set(value As Double)
            If value <> _Gamma Then
                _Gamma = value
            End If
        End Set
    End Property

    Public Property _nx1 As Integer = 1
    Public Property _kh1 As Integer = 1
    Public Property _nh1 As Integer = 1
    Public Property _nfft1 As Integer = 256

    Private Sub extrapolate(xfft As Double())

        Dim mr1 As Integer = _nx1 + _kh1
        Dim xr1 As Double = xfft(_nx1 - 1)
        For i1 As Integer = _nx1 To mr1 - 1
            xfft(i1) = xr1
        Next
        Dim ml1 As Integer = _nfft1 + _kh1 - _nh1 + 1
        Dim xl1 As Double = xfft(0)
        For i1 As Integer = ml1 To _nfft1 - 1
            xfft(i1) = xl1
        Next

    End Sub

End Class
