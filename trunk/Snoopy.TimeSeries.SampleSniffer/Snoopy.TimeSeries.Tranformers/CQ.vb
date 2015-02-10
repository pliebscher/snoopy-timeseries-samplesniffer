Imports System.ComponentModel
''' <summary>
''' http://code.google.com/p/neuralmusic/source/browse/trunk/neuralmusic/src/uk/co/drpj/audio/constantq/FFTConstantQ.java
''' </summary>
''' <remarks></remarks>
Public Class CQ
    Inherits TimeSeriesFrameTransformer

    Private _FFT As New FFT

    Private Const HUMAN_AUDITORY_THRESHOLD As Double = 2 * 0.000001

    Private _LogFrequenciesIndex As Integer()
    Private _LogBinFrequencies As Integer()
    Private _BinsPerOctave As Integer = 32

    Private _SampleRate As Integer = 22050
    Private _MaxFrequency As Double = 845.0 ' _SampleRate \ 2
    Private _MinFrequency As Double = 133.0 ' (_SampleRate \ 100) * Math.Pow(2, ((24 - 58) / 12.0)) '300

    Private _FFTWidth As Integer
    Private _FFTPhase As FFT.Phase = FFT.Phase.Signal

    Private _Threshold As Double = 0.001 ' // Lower number, better quality !!! 0 = best
    Private _Spread As Double = 24.0

    'double q; // Constant Q
    Private _Q As Double
    'int k; // Number of output bands
    Private _K As Integer
    'double[] freqs;
    Private _Freqs As Double()
    'double[][] qKernel;
    Private _QKernel As Double()()
    'int[][] qKernel_indexes;
    Private _QKernelIndexes As Integer()()

    Public Sub New()
        _FFT.PhaseMode = _FFTPhase
        Reset()
    End Sub

    Private Sub Reset()

        ' // Calculate Constant Q
        ' q = 1.0 / (Math.pow(2, 1.0 / binsPerOctave) - 1.0) / spread;
        _Q = 1.0 / (Math.Pow(2, 1.0 / _BinsPerOctave) - 1.0) / _Spread

        ' // Calculate number of output bins
        ' k = (int) Math.ceil(binsPerOctave * Math.log(maxFreq / minFreq) / Math.log(2));
        _K = CInt(Math.Ceiling(_BinsPerOctave * Math.Log(_MaxFrequency / _MinFrequency) / Math.Log(2)))

        ' // Calculate length of FFT
        ' double calc_fftlen = Math.ceil(q * sampleRate / minFreq);
        Dim fftlen As Double = Math.Ceiling(_Q * _SampleRate / _MinFrequency)

        ' fftlen = (int) Math.pow(2, Math.ceil(Math.log(calc_fftlen) / Math.log(2)));
        _FFTWidth = CInt(Math.Pow(2, Math.Ceiling(Math.Log(fftlen) / Math.Log(2)))) \ 2

        ' need this to generate the correct window len...
        MyBase.FrameWidth = CType([Enum].Parse(GetType(Snoopy.TimeSeries.FrameWidth), _FFTWidth.ToString), Snoopy.TimeSeries.FrameWidth)

        '_Window = Window.GetInstance(_WindowType).GetWindow(_FFTWidth)

        ' qKernel = new double[k][];
        _QKernel = New Double(_K - 1)() {}

        ' qKernel_indexes = new int[k][];
        _QKernelIndexes = New Integer(_K - 1)() {}

        ' freqs = new double[k];
        _Freqs = New Double(_K - 1) {}

        ' // Calculate Constant Q kernels
        ' double[] temp = new double[fftlen * 2];
        Dim temp As Double() = New Double((_FFTWidth * 2) - 1) {}

        ' double[] ctemp = new double[fftlen * 2];
        'Dim ctemp As Double() = New Double((_FFTWidth * 2) - 1) {}

        ' int[] cindexes = new int[fftlen];
        Dim cindexes As Integer() = New Integer(_FFTWidth - 1) {}

        ' for (int i = 0; i < k; i++) {
        For i As Integer = 0 To _K - 1
            ' double[] sKernel = temp;
            Dim sKernel As Double() = temp '              

            ' // Calculate the frequency of current bin
            ' freqs[i] = minFreq * Math.pow(2, i / binsPerOctave);
            _Freqs(i) = _MinFrequency * Math.Pow(2, i / _BinsPerOctave)

            ' 1. len = Math.ceil(q * sampleRate / freqs[i]);
            ' -- or --
            ' 2. double len = q * sampleRate / freqs[i];
            Dim len As Double = _Q * _SampleRate / _Freqs(i)

            ' // double halflen= len*0.5;
            ' // window is symmetric around center point of frame
            ' // calculate second half of the kernel

            ' for (int j = 0; j < fftlen / 2; j++) {
            For j As Integer = 0 To (_FFTWidth \ 2) - 1

                ' double aa;
                ' aa = (double) (j + 0.5) / len;    
                ' -- or --
                ' aa = (double) (j) / len;
                Dim aa As Double = (j + 0.5) / len

                ' if (aa < .5) {
                If (aa < 0.5) Then
                    ' double a = 2.0 * Math.PI * aa;
                    Dim a As Double = 2.0 * Math.PI * aa

                    ' double window = 0.5 * (1.0 + Math.cos(a)); // Hanning
                    ' window /= len;
                    Dim window As Double = 0.5 * (1.0 + Math.Cos(a))
                    window /= len

                    ' // Calculate kernel -------------------------------------------------
                    Dim x As Double
                    ' x = 2 * Math.PI * q * (double) (j + 0.5D) / (double) len;
                    x = 2 * Math.PI * Q * (j + 0.5D) / len
                    ' -- or --
                    ' x = 2.0 * Math.PI * freqs[i] * (double) j / sampleRate;
                    ' -- or --
                    ' double x = 2.0 * Math.PI * freqs[i] * (j + 0.5D) / sampleRate;
                    'x = 2.0 * Math.PI * _Freqs(i) * (j + 0.5) / _SampleRate

                    ' sKernel[fftlen + j * 2] = window * Math.cos(x);
                    ' sKernel[fftlen + j * 2 + 1] = window * Math.sin(x);
                    'sKernel(_FFTWidth + j * 2) = window * Math.Cos(x)
                    'sKernel(_FFTWidth + j * 2 + 1) = window * Math.Sin(x)
                    sKernel(_FFTWidth + (2 * j)) = Math.Cos(x) * window 'MyBase.Window(2 * j)
                    sKernel(_FFTWidth + (2 * j + 1)) = Math.Sin(x) * window 'MyBase.Window(2 * j + 1)

                    ' } else {
                Else '                                 
                    ' sKernel[fftlen + j * 2] = 0.0;
                    ' sKernel[fftlen + j * 2 + 1] = 0.0;
                    sKernel(_FFTWidth + (2 * j)) = 0.0
                    sKernel(_FFTWidth + (2 * j + 1)) = 0.0
                    ' }
                End If

            Next

            ' // reflect to genereate first half
            ' int halfway = fftlen / 2;
            Dim halfway As Integer = _FFTWidth \ 2

            ' 1. for (int j = 1; j < halfway; j++) {
            For j As Integer = 1 To halfway - 1
                ' -- or --
                ' 2. for (int j = 0; j < halfway; j++) {
                ' For j As Integer = 0 To halfway - 1

                ' 1. int i1 = halfway - j;
                Dim i1 As Integer = halfway - j
                ' -- or --
                ' 2. int i1 = halfway - j - 1;
                ' Dim i1 As Integer = halfway - j - 1
                ' int i2 = halfway + j;
                Dim i2 As Integer = halfway + j

                ' sKernel[i1 * 2] = sKernel[2 * i2];
                ' sKernel[i1 * 2 + 1] = -sKernel[2 * i2 + 1];
                sKernel(i1 * 2) = sKernel(2 * i2) '* MyBase.Window(2 * i2)
                sKernel(i1 * 2 + 1) = -sKernel(2 * i2 + 1) '* MyBase.Window(2 * i2 + 1)
                ' }
            Next

            ' 1. sKernel[0] = 0.0; sKernel[1] = 0.0;

            ' // Perform FFT on kernel
            ' fft.calc(sKernel, -1);
            _FFT.Complex(sKernel, True)
            'Fourier.Transform(sKernel, _FFTWidth, Fourier.Direction.Forward)

            ' // Remove all zeros from kernel to improve performance
            '  double[] cKernel = ctemp;
            Dim cKernel As Double() = New Double((_FFTWidth * 2) - 1) {} '= ctemp '           <---------- Do we need this?

            ' int k = 0;
            Dim k As Integer = 0

            ' for (int j = 0, j2 = sKernel.length - 2; j < sKernel.length / 2; j += 2, j2 -= 2) {
            For j As Integer = 0 To (sKernel.Length \ 2) - 1 Step 2
                Dim j2 As Integer = (sKernel.Length - 2) - 1

                ' double absval = Math.sqrt(sKernel[j] * sKernel[j] + sKernel[j + 1] * sKernel[j + 1]);
                ' absval += Math.sqrt(sKernel[j2] * sKernel[j2] + sKernel[j2 + 1] * sKernel[j2 + 1]);
                Dim absval As Double = Math.Sqrt(sKernel(j) * sKernel(j) + sKernel(j + 1) * sKernel(j + 1))
                absval += Math.Sqrt(sKernel(j2) * sKernel(j2) + sKernel(j2 + 1) * sKernel(j2 + 1))

                ' if (absval > threshold) {
                If (absval > _Threshold) Then
                    '   cindexes[k] = j;
                    cindexes(k) = j
                    '   cKernel[2 * k] = sKernel[j] + sKernel[j2];
                    '   cKernel[2 * k + 1] = sKernel[j + 1] + sKernel[j2 + 1];
                    cKernel(2 * k) = sKernel(j) + sKernel(j2)
                    cKernel(2 * k + 1) = sKernel(j + 1) + sKernel(j2 + 1)
                    '   k++;
                    k += 1
                    '   }
                End If

                j2 -= 2
                ' }
            Next

            ' sKernel = new double[k * 2];
            sKernel = New Double((k * 2) - 1) {}

            ' for (int j = 0; j < k * 2; j++)
            '   sKernel[j] = cKernel[j];
            For j As Integer = 0 To (k * 2) - 1
                sKernel(j) = cKernel(j)
            Next

            ' int[] indexes = new int[k];

            Dim indexes As Integer() = New Integer(k - 1) {}

            ' for (int j = 0; j < k; j++)
            '   indexes[j] = cindexes[j];

            For j As Integer = 0 To k - 1
                indexes(j) = cindexes(j)
            Next

            ' // Normalize fft output
            ' for (int j = 0; j < sKernel.length; j++)
            '   sKernel[j] /= fftlen;
            For j As Integer = 0 To sKernel.Length - 1
                sKernel(j) /= _FFTWidth '                   <------ should this be optional???
            Next

            ' // Perform complex conjugate on sKernel
            ' for (int j = 1; j < sKernel.length; j += 2)
            '   sKernel[j] = -sKernel[j];
            For j As Integer = 0 To sKernel.Length - 1 Step 2
                sKernel(j) = -sKernel(j)
            Next

            ' qKernel_indexes[i] = indexes;
            ' qKernel[i] = sKernel;
            _QKernelIndexes(i) = indexes
            _QKernel(i) = sKernel

            ' }
        Next

    End Sub

    Protected Overloads Overrides Function OnTransformFrame(frame As Double()) As Double()

        Dim complexSignal As Double() = New Double(2 * _FFTWidth - 1) {}

        For j As Integer = 0 To _FFTWidth - 1
            complexSignal(2 * j) = frame(j)
            complexSignal(2 * j + 1) = 0
        Next

        _FFT.Complex(complexSignal, True)
        '_FFT.Real(frame, True)
        Dim QCalc As Double() = Calculate(complexSignal)

        Return QCalc

    End Function

    Protected Overrides Sub OnWindowChanged(window() As Double)
        Reset()
    End Sub

    'Protected Function OnTransform1(series As ITimeSeries(Of Double())) As Double()()

    '    If _SampleRate <> series.SampleRate Then
    '        _SampleRate = series.SampleRate
    '        Reset()
    '    End If

    '    ' HACK: adjusting fft width is sample length is to short.
    '    If series.Samples.Length <= _FFTWidth Then _FFTWidth = CType((series.Samples.Length \ 2), FrameWidth)

    '    Dim width As Integer = (series.Samples.Length - _FFTWidth) \ _FrameStep
    '    Dim frames As Double()() = New Double(width - 1)() {}
    '    Dim complexSignal As Double() = New Double(2 * _FFTWidth - 1) {}

    '    Dim magn As Double() = New Double(_K - 1) {}
    '    Dim phase As Double() = New Double(_K - 1) {}

    '    'even - Re, odd - Img
    '    For i As Integer = 0 To width - 1

    '        For j As Integer = 0 To _FFTWidth - 1
    '            complexSignal(2 * j) = _Window(j) * series.Samples(i * _FrameStep + j)
    '            complexSignal(2 * j + 1) = 0
    '        Next

    '        _FFT.Real(complexSignal, True)
    '        'Fourier.Transform(complexSignal, _FFTWidth, Fourier.Direction.Forward)

    '        Dim QCalc As Double() = Calculate(complexSignal)

    '        'Dim QOut As Double() = New Double(QCalc.Length \ 2 - 1) {}

    '        'Dim sampleCount As Integer = i '* _FrameStep ' ?????
    '        'Dim t1 As Double = sampleCount / _SampleRate

    '        'For bin As Integer = 0 To _Freqs.Length - 1

    '        '    Dim freq As Double = _Freqs(bin)
    '        '    Dim period As Double = 1.0 / freq
    '        '    Dim phaseShift As Double = Math.PI * 2.0 * (t1 Mod period) / period

    '        '    Dim re As Double = QCalc(2 * bin)
    '        '    Dim im As Double = QCalc(2 * bin + 1)

    '        '    Dim magni As Double = Math.Sqrt(re * re + im * im)
    '        '    Dim phasei As Double = Math.Atan2(im, re) - phaseShift

    '        '    While (phasei < phase(bin) - Math.PI)
    '        '        phasei += Math.PI * 2
    '        '    End While

    '        '    While (phasei > phase(bin) + Math.PI)
    '        '        phasei -= Math.PI * 2
    '        '    End While

    '        '    Dim scale As Double = 1.0 / (_FrameStep - 1)

    '        '    For j As Integer = 0 To QOut.Length - 1 ' _FrameStep - 1
    '        '        If (sampleCount + j >= series.Samples.Length) Then Exit For

    '        '        Dim fact2 As Double = j * scale
    '        '        Dim fact1 As Double = 1.0 - fact2
    '        '        Dim mag As Double = (magn(bin) * fact1 + magni * fact2)
    '        '        Dim pha As Double = (phase(bin) * fact1 + phasei * fact2)

    '        '        Dim tt As Double = (sampleCount + j) * freq * Math.PI * 2.0 / _SampleRate

    '        '        'QOut(sampleCount + j) += mag * Math.Cos(tt + pha)
    '        '        QOut(j) += mag * Math.Cos(tt + pha)

    '        '    Next

    '        '    magn(bin) = magni
    '        '    phase(bin) = phasei

    '        'Next


    '        frames(i) = QCalc 'QOut

    '    Next

    '    Return frames
    'End Function

    Private Function Calculate(frame As Double()) As Double()
        'Dim coeffs As Double() = New Double((_QKernel.Length * 2) - 1) {}
        Dim coeffs As Double() = New Double(_QKernel.Length - 1) {}

        'public void calc(double[] buff_in, double[] buff_out) {
        '        fft.calcReal(buff_in, -1);
        '        for (int i = 0; i < qKernel.length; i++) {
        For i As Integer = 0 To _QKernel.Length - 1
            ' double[] kernel = qKernel[i];
            ' int[] indexes = qKernel_indexes[i];
            ' double t_r = 0;
            ' double t_i = 0;
            Dim kernel As Double() = _QKernel(i)
            Dim indexes As Integer() = _QKernelIndexes(i)
            Dim t_r As Double
            Dim t_i As Double
            Dim l As Integer = 0

            ' for (int j = 0, l = 0; j < kernel.length; j += 2, l++) {
            For j As Integer = 0 To kernel.Length - 1 Step 2
                '   int jj = indexes[l];
                Dim jj As Integer = indexes(l)

                '   double b_r = buff_in[jj];
                '   double b_i = buff_in[jj + 1];
                Dim b_r As Double = frame(jj)
                Dim b_i As Double = frame(jj + 1)

                '   double k_r = kernel[j];
                '   double k_i = kernel[j + 1];
                Dim k_r As Double = kernel(j)
                Dim k_i As Double = kernel(j + 1)

                '   // COMPLEX: T += B * K
                '   t_r += b_r * k_r - b_i * k_i;
                '   t_i += b_r * k_i + b_i * k_r;
                t_r += b_r * k_r - b_i * k_i
                t_i += b_r * k_i + b_i * k_r
                l += 1
            Next
            ' }
            'coeffs(i) = Math.Sqrt(t_r * t_r + t_i * t_i)
            'coeffs(i) = t_r 'Math.Sqrt(t_r * t_r + t_i * t_i)
            'coeffs(i * 2) = t_r
            'coeffs(i * 2 + 1) = t_i

            'coeffs(i) = 20 * Math.Log10(Math.Sqrt(t_r * t_r + t_i * t_i) / HUMAN_AUDITORY_THRESHOLD)
            coeffs(i) = Math.Sqrt(t_r * t_r + t_i * t_i)
            ' }
        Next i
        '}
        Return coeffs

    End Function

    'Private Sub GenerateKernel()

    '    Dim Q As Double = 1.0 / (Math.Pow(2, 1.0 / _BinsPerOctave) - 1)
    '    Dim K As Integer = CInt(Math.Ceiling(_BinsPerOctave * Math.Log(_MaxFrequency / _MinFrequency, 2)))
    '    'Dim fftLen As Integer = 2048
    '    ' NextPowerOfTwo((int)Math.Ceiling(Q * _frequencyRate / _minFreq));
    '    ' Dim specKernel As Complex()() = New Complex(K - 1)() {}
    '    Dim specKernel As Double()() = New Double(K - 1)() {}

    '    For k1 As Integer = K To 1 Step -1
    '        Dim len As Integer = CInt(Math.Ceiling(Q * _SampleRate / (_MinFrequency * Math.Pow(2, CDbl(k1 - 1) / _BinsPerOctave))))
    '        Dim win As Double() = Window.GetInstance(_WindowType).GetWindow(_FFTWidth)
    '        For i As Integer = 0 To len - 1
    '            ' Dim tempKernel As Complex() = New Complex(fftLen - 1) {}
    '            Dim tempKernel As Double() = New Double((_FFTWidth * 2) - 1) {}
    '            'Dim window As Double() = _Window.GetWindow(len)
    '            Dim complexI As New System.Numerics.Complex(0, 1)

    '            For j As Integer = 0 To _FFTWidth - 1 '(_FFTWidth * 2) - 1
    '                'window(j) /= len
    '                Dim expValue As Complex = Complex.Exp(2 * Math.PI * complexI * Q * j / len)
    '                Dim value As Complex = (win(j) / len) * expValue ' _Window(j) * expValue
    '                'tempKernel(j) = New Complex(value.Real, value.Imaginary)
    '                tempKernel(j * 2) = value.Real
    '                tempKernel(j * 2 + 1) = value.Imaginary
    '            Next

    '            _FFT.Complex(tempKernel, True)

    '            For t As Integer = 0 To _FFTWidth - 1
    '                Dim re As Double = tempKernel(t * 2)
    '                Dim im As Double = tempKernel(t * 2 + 1)
    '                If Math.Sqrt(re * re + im * im) <= _Threshold Then
    '                    tempKernel(t) = 0
    '                End If
    '            Next

    '            'specKernel(k1 - 1) = New Complex(_FFTWidth - 1) {}
    '            specKernel(k1 - 1) = New Double(_FFTWidth - 1) {}
    '            For t As Integer = 0 To _FFTWidth - 1
    '                specKernel(k1 - 1)(t) = tempKernel(t)
    '            Next
    '        Next
    '    Next

    '    For i As Integer = 0 To K - 1
    '        For j As Integer = 0 To _FFTWidth - 1
    '            ' specKernel(i)(j) = New Complex(specKernel(i)(j).Re / fftLen, -specKernel(i)(j).Im / fftLen)
    '            specKernel(i)(j * 2) /= _FFTWidth
    '            specKernel(i)(j * 2 + 1) = -specKernel(i)(j * 2 + 1) / _FFTWidth
    '        Next
    '    Next

    '    _QKernel = specKernel

    'End Sub

    <Description("CQ calc's it's own width.")>
    Public Shadows ReadOnly Property FrameWidth As Integer
        Get
            Return _FFTWidth
        End Get
    End Property

    <Description(""), DefaultValue(24.0)>
    Public Property Spread As Double
        Get
            Return _Spread
        End Get
        Set(value As Double)
            If value <> _Spread Then
                _Spread = value
                Reset()
            End If
        End Set
    End Property

    <Description("Lower number, better quality !!! 0 = Best"), DefaultValue(0.001)>
    Public Property Threshold As Double
        Get
            Return _Threshold
        End Get
        Set(value As Double)
            If value <> _Threshold Then
                _Threshold = value
                Reset()
            End If
        End Set
    End Property

    <Description("Bins per octave."), DefaultValue(32)>
    Public Property Bins As Integer
        Get
            Return _BinsPerOctave
        End Get
        Set(value As Integer)
            If _BinsPerOctave <> value Then
                _BinsPerOctave = value
                Reset()
            End If
        End Set
    End Property

    Public ReadOnly Property BinFreqs As Double()
        Get
            Return _Freqs
        End Get
    End Property

    <Description("Max Frequency."), DefaultValue(845.0)>
    Public Property MaxFrequency As Double
        Get
            Return _MaxFrequency
        End Get
        Set(value As Double)
            If value <> _MaxFrequency Then
                If value <= _MinFrequency Then Throw New ArgumentException("MaxFrequency cannot be less than or equal to MinFrequency.")
                If value >= _SampleRate Then Throw New ArgumentException("MaxFrequency cannot be greater than the SampleRate.")
                _MaxFrequency = value
                Reset()
            End If
        End Set
    End Property

    <Description("Min Frequency."), DefaultValue(133.0)>
    Public Property MinFrequency As Double
        Get
            Return _MinFrequency
        End Get
        Set(value As Double)
            If value <> _MinFrequency Then
                If value >= _MaxFrequency Then Throw New ArgumentException("MinFrequency cannot be greater than or equal to MaxFrequency.")
                If value <= 0 Then Throw New ArgumentException("MinFrequency must be greater than 0.")
                _MinFrequency = value
                Reset()
            End If
        End Set
    End Property

    <Description("The Constant Q.")>
    Public ReadOnly Property Q As Double
        Get
            Return _Q
        End Get
    End Property

    <DisplayName("Phase"), Description("FFT phase mode."), DefaultValue(FFT.Phase.Signal)>
    Public Property PhaseMode As FFT.Phase
        Get
            Return _FFTPhase
        End Get
        Set(value As FFT.Phase)
            If value <> _FFTPhase Then
                _FFTPhase = value
                _FFT.PhaseMode = _FFTPhase
                Reset()
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Constant Q Transform."
        End Get
    End Property

End Class
