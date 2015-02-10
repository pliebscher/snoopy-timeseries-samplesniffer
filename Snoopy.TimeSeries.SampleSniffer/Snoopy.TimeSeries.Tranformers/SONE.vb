Imports System.ComponentModel
''' <summary>
''' http://www.cp.jku.at/people/schedl/Research/Development/CoMIRVA/webpage/CoMIRVA.html
''' </summary>
''' <remarks></remarks>
<Description("")>
Public Class SONE
    Inherits TimeSeriesFrameTransformer

    Private _SampleRate As Integer
    Private _FFT As New FFT

    Private _FrameWidth As Integer = 128
    Private _Phase As FFT.Phase = FFT.Phase.Data

    ' compute rescale factor to rescale and normalize at once (default is 96dB = 2^16)
    Private scale As Double = Math.Pow(10, 96 / 20)
    Private _BaseFreq As Double

    Private bark_upper As Integer()
    Private terhardtWeight As Double()

    Private _WindowSum As Double

    Public Sub New()

        MyBase.FrameStep = FrameStep._32
        MyBase.FrameWidth = FrameWidth._128
        MyBase.WindowType = Windows.WindowType.Hanning

        _FFT.PhaseMode = _Phase

        Reset()

    End Sub

    Protected Overloads Overrides Function OnTransformFrame(frame As Double()) As Double()

        Dim complexSignal As Double() = New Double(2 * _FrameWidth - 1) {}

        ' Make complex + scale...
        For j As Integer = 0 To _FrameWidth - 1
            complexSignal(2 * j) = frame(j) * scale
            complexSignal(2 * j + 1) = 0
        Next

        Dim value As Double = 0.0
        Dim fftSize As Integer = (_FrameWidth \ 2) + 1
        Dim barkSize As Integer = bark_upper.Length
        Dim output As Double() = New Double(barkSize - 1) {}

        _FFT.Complex(complexSignal, True)

        For j As Integer = 0 To _FrameWidth - 1
            Dim re As Double = complexSignal(2 * j) / _WindowSum * 2
            Dim im As Double = complexSignal(2 * j + 1) / _WindowSum * 2
            frame(j) = (re * re + im * im) * terhardtWeight(j) ' Power spec + calculate outer era model
        Next

        'calculate bark scale
        Dim freq As Double = 0
        value = 0
        Dim band As Integer = 0
        Dim i As Integer = 0
        While i < fftSize AndAlso band < bark_upper.Length
            If freq <= bark_upper(band) Then
                value += frame(i)
            Else
                frame(band) = value
                band += 1
                value = frame(i)
            End If
            freq += _BaseFreq
            i += 1
        End While

        If band < bark_upper.Length Then
            frame(band) = value
        End If

        Dim log As Double = 10 * (1 / Math.Log(10))

        'calculate loudness Sone (from db)
        For k As Integer = 0 To barkSize - 1
            'corrected version: without spectral masking step
            If frame(k) < 1 Then
                output(k) = 0.0
            Else
                output(k) = log * Math.Log(frame(k))
            End If

            If output(k) >= 40.0 Then
                output(k) = Math.Pow(2.0, (output(k) - 40.0) / 10.0)
            Else
                output(k) = Math.Pow(output(k) / 40.0, 2.642)
            End If
        Next

        Return output

    End Function

    Private Sub Reset()

        _BaseFreq = _SampleRate / _FrameWidth

        ' get upper boundaries for the used bark bands
        bark_upper = getBarkUpperBoundaries(_SampleRate)

        ' get weights for simulation of the perception of the outer ear
        terhardtWeight = getTerhardtWeights(_BaseFreq, _FrameWidth)

    End Sub

    Public Function getBarkUpperBoundaries(sampleRate As Double) As Integer()

        Dim bark_upper As Integer() = New Integer() {100, 200, 300, 400, 510, 630, 770, 920, 1080, 1270, 1480, 1720, 2000, 2320, 2700, 3150, 3700, 4400, 5300, 6400, 7700, 9500, 12000, 15500} ' Hz

        Dim max As Integer = 0
        Dim boundaries() As Integer

        ' ignore critical bands higher than the sampling frequency
        max = bark_upper.Length - 1
        While max >= 0 AndAlso bark_upper(max) > sampleRate / 2
            max -= 1
        End While

        'create new array of appropriate size
        boundaries = New Integer(max + 1) {}

        'copy upper boundaries
        For i As Integer = 0 To boundaries.Length - 1
            boundaries(i) = bark_upper(i)
        Next

        Return boundaries

    End Function

    Public Function getTerhardtWeights(baseFrequency As Double, vectorSize As Integer) As Double()

        Dim freq As Double = 0
        Dim weights As Double() = New Double(vectorSize - 1) {}

        For j As Integer = 0 To vectorSize - 1
            'compute frequency of the j-th component
            freq = (j * baseFrequency) / 1000
            'compute weight using Terhard formula
            weights(j) = Math.pow(10, (-3.64 * Math.pow(freq, -0.8) + 6.5 * Math.exp(-0.6 * Math.pow(freq - 3.3, 2)) - 0.001 * (Math.pow(freq, 4))) / 20)
            'take the power of the computed weight
            weights(j) = weights(j) * weights(j)
        Next

        Return weights

    End Function

    Protected Overrides Sub OnWindowChanged(window() As Double)
        _WindowSum = 0
        For i As Integer = 0 To window.Length - 1
            _WindowSum += window(i)
        Next
    End Sub

    Protected Overrides Sub OnSampleRateChanged(sampleRate As Integer)
        _SampleRate = sampleRate
        Reset()
    End Sub

    Protected Overrides Sub OnFrameWidthChanged(width As Integer)
        _FrameWidth = width
        Reset()
    End Sub

    <Description("Base frequency: SampleRate / FrameWidth")>
    Public ReadOnly Property BaseFreq As Double
        Get
            Return _BaseFreq
        End Get
    End Property

    <DisplayName("Phase"), Description("FFT phase mode."), DefaultValue(FFT.Phase.Data)>
    Public Property FFTPhaseMode As FFT.Phase
        Get
            Return _Phase
        End Get
        Set(value As FFT.Phase)
            If value <> _Phase Then
                _Phase = value
                _FFT.PhaseMode = _Phase
            End If
            _Phase = value
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Computes sonogram from a pcm signal. The specific loudness sensation (Sone) per critical-band (Bark)."
        End Get
    End Property

End Class
