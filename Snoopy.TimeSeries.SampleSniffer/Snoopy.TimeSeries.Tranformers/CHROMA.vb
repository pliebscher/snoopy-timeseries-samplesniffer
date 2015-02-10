Imports System.ComponentModel
''' <summary>
''' http://labrosa.ee.columbia.edu/meapsoft/
''' </summary>
''' <remarks></remarks>
Public Class CHROMA
    Inherits TimeSeriesFrameTransformer

    Private _Bins As Integer = 32
    Private _SampleRate As Integer = 22050
    Private _FrameWidth As Integer = 512 '2048
    Private _Phase As FFT.Phase = FFT.Phase.Signal
    Private _FFT As New FFT

    Private _FIRSTBAND As Integer = 3
    Private chromaWts As Double()()
    Private _Hz As Double = 440.0

    Public Sub New()

        _FFT.PhaseMode = _Phase

        InitChroma()

    End Sub

    Protected Overloads Overrides Function OnTransformFrame(frame As Double()) As Double()

        'Dim complexSignal As Double() = New Double(2 * _FrameWidth - 1) {}

        'For j As Integer = 0 To _FrameWidth - 1
        '    complexSignal(2 * j) = frame(j)
        '    complexSignal(2 * j + 1) = 0
        'Next

        ' _FFT.Complex(complexSignal, True)

        _FFT.Real(frame, True)

        Dim bins As Double() = ExtractChromaBins(frame)

        Return bins

    End Function

    Public Function ExtractChromaBins(ByVal curFrame As Double()) As Double()

        Dim length As Integer = curFrame.Length '(2048 - _FrameWidth) \ MyBase.FrameStep  '(series.Samples.Length - _FrameWidth) \ _FrameStep

        For band As Integer = 0 To curFrame.Length - 1
            curFrame(band) = Math.Pow(10, curFrame(band) / 10) / length
        Next

        Dim chromSpec As Double() = New Double(_Bins - 1) {}
        Dim sum As Double = 0
        Dim sum2 As Double = 0

        ' matrix multiply to find bins
        For bin As Integer = 0 To _Bins - 1
            Dim val As Double = 0
            For band As Integer = _FIRSTBAND To curFrame.Length - 1
                val += curFrame(band) * chromaWts(bin)(band)
            Next
            chromSpec(bin) = val
            sum += val
            sum2 += val * val
        Next

        ' chroma vectors have unit norm
        'Dim rms As Double = Math.Sqrt(sum2 / _Bins)
        'For bin As Integer = 0 To _Bins - 1
        '    chromSpec(bin) = (chromSpec(bin) / rms) / _Bins
        'Next

        Return chromSpec

    End Function

    Private Sub InitChroma()

        'linSpec = New Double(N - 1) {}

        'chromaWts = new double[outDim][N];
        chromaWts = New Double(_Bins - 1)() {}

        ' Create the chroma inner products

        Dim bin2hz As Double = _SampleRate / (_FrameWidth - 1) '_SampleRate / (2 * (_FrameWidth - 1))

        For i As Integer = _FIRSTBAND To _FrameWidth - 1
            'Dim tot As Double = 0
            ' 1/12 = 1 semi = 0.083333
            ' double binwidth = max(0.08333333, hz2octs(bin2hz*(i+1)) -
            ' hz2octs(bin2hz*(i-1)))/2;
            Dim binwidth As Double = hz2octs(bin2hz * (i + 1)) - hz2octs(bin2hz * (i - 1))
            If binwidth < 0.083333 Then
                binwidth = 0.083333
            End If
            binwidth /= 4
            Dim binocts As Double = hz2octs(bin2hz * i)
            ' fade out bins above 1 kHz
            Dim binwt As Double = 1.0
            If bin2hz * i > 1000 Then
                binwt = Math.Exp(-(bin2hz * i - 1000) / 500)
            End If
            For j As Integer = 0 To _Bins - 1
                If chromaWts(j) Is Nothing Then
                    chromaWts(j) = New Double(_FrameWidth - 1) {}
                End If
                Dim bindelta As Double = binocts - (CDbl(j) / _Bins)
                bindelta = bindelta - Math.Round(bindelta, MidpointRounding.ToEven) 'Math.rint(bindelta)
                chromaWts(j)(i) = binwt * Math.Exp(-0.5 * Math.Pow(bindelta / binwidth, 2))
                'tot = tot + chromaWts(j)(i)
            Next
        Next

    End Sub

    Public Function hz2octs(fq As Double) As Double
        'return Math.log(fq / 370.0) / 0.69314718055995;
        'return Math.log(fq / 392.00) / 0.69314718055995; G4
        '261.63 is a C4
        Return Math.Log(fq / _Hz) / 0.69314718055995
    End Function

    Protected Overrides Sub OnSampleRateChanged(sampleRate As Integer)
        _SampleRate = sampleRate
        InitChroma()
    End Sub

    Protected Overrides Sub OnFrameWidthChanged(width As Integer)
        _FrameWidth = width '* 2
        InitChroma()
    End Sub

    <Description("http://www.phy.mtu.edu/~suits/notefreqs.html"), DefaultValue(440.0)>
    Public Property Hz As Double
        Get
            Return _Hz
        End Get
        Set(value As Double)
            If value <> _Hz Then
                _Hz = value
                InitChroma()
            End If
        End Set
    End Property

    <Description("Number of bins."), DefaultValue(32)>
    Public Property Bins As Integer
        Get
            Return _Bins
        End Get
        Set(value As Integer)
            If _Bins <> value Then
                _Bins = value
                InitChroma()
            End If
        End Set
    End Property

    <Description("Number of frequency bands to skip."), DefaultValue(3)>
    Public Property FirstBand As Integer
        Get
            Return _FIRSTBAND
        End Get
        Set(value As Integer)
            If value <> _FIRSTBAND Then
                _FIRSTBAND = value
                InitChroma()
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Chromagram"
        End Get
    End Property

End Class
