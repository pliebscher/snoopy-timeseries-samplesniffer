Imports System.ComponentModel
''' <summary>
''' https://github.com/JorenSix/TarsosDSP/blob/master/src/be/hogent/tarsos/dsp/EnvelopeFollower.java
''' </summary>
''' <remarks></remarks>
<Description("Extracts a signals envelope.")>
Public Class EnvelopeExtractor
    Inherits TimeSeriesPreprocessor

    Private _SampleRate As Integer = 22050 ' Assumption. Don't have a way to get at this time...
    Private _AttackTime As Double = 0.0002 ' Defines how fast the envelope raises, defined in seconds.
    Private _ReleaseTime As Double = 0.0004 ' Defines how fast the envelope goes down, defined in seconds.

    Private _gainAttack As Double
    Private _gainRelease As Double
    Private _envelopeOut As Double = 0.0

    Public Sub New()
        CalcGain()
    End Sub

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()

        For i As Integer = 0 To series.Samples.Length - 1
            Dim envelopeIn As Double = Math.Abs(series.Samples(i))
            If _envelopeOut < envelopeIn Then
                _envelopeOut = envelopeIn + _gainAttack * (_envelopeOut - envelopeIn)
            Else
                _envelopeOut = envelopeIn + _gainRelease * (_envelopeOut - envelopeIn)
            End If
            series.Samples(i) = _envelopeOut
        Next

        Return series.Samples
    End Function

    Protected Overrides Sub OnSampleRateChanged(sampleRate As Integer)
        _SampleRate = sampleRate ' Local for performance.
        CalcGain()
        MyBase.OnSampleRateChanged(sampleRate)
    End Sub

    Private Sub CalcGain()
        _gainAttack = Math.Exp(-1.0 / (_SampleRate * _AttackTime))
        _gainRelease = Math.Exp(-1.0 / (_SampleRate * _ReleaseTime))
    End Sub

    <Description("Defines how fast the envelope raises, defined in seconds."), DefaultValue(0.0002)>
    Public Property AttackTime As Double
        Get
            Return _AttackTime
        End Get
        Set(value As Double)
            If value <> _AttackTime Then
                _AttackTime = value
                CalcGain()
            End If
        End Set
    End Property

    <Description("Defines how fast the envelope goes down, defined in seconds."), DefaultValue(0.0004)>
    Public Property ReleaseTime As Double
        Get
            Return _ReleaseTime
        End Get
        Set(value As Double)
            If value <> _ReleaseTime Then
                _ReleaseTime = value
                CalcGain()
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Extracts a signals envelope."
        End Get
    End Property

End Class
