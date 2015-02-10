Imports System.ComponentModel
Imports Snoopy.TimeSeries.Windows
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class RASPEC
    Inherits TimeSeriesTransformer

    Private _FrameWidth As FrameWidth = FrameWidth._512
    Private _FrameStep As FrameStep = FrameStep._64
    Private _WindowType As WindowType = WindowType.Hanning
    Private _Window As Double() = Windows.Window.Hanning.GetWindow(_FrameWidth)

    Private _LogFrequenciesIndex As Integer()
    Private _LogBins As Integer = 64
    Private _LogBase As Double = Math.E
    Private _SampleRate As Integer = 22050
    Private _MaxFrequency As Double = 2855.0
    Private _MinFrequency As Double = 318.0
    Private _Phase As FFT.Phase = FFT.Phase.Data
    Private _FFT As New FFT

    Private _TimeDerivWindow As Double()

    Public Sub New()

        _FFT.PhaseMode = _Phase

        GenerateLogFrequencies()
        timederivwindow(_Window, _SampleRate, 1)

    End Sub

    Protected Overrides Function OnTransform(series As ITimeSeries(Of Double())) As Double()()

        Dim width As Integer = (series.Samples.Length - _FrameWidth) \ _FrameStep
        Dim frames As Double()() = New Double(width - 1)() {}

        Dim X As Double() = New Double(2 * _FrameWidth - 1) {}
        Dim Xdt As Double() = New Double(2 * _FrameWidth - 1) {}
        Dim magsqrd As Double() = New Double(_FrameWidth - 1) {}


        Dim k As Integer

        For i As Integer = 0 To width - 1

            For j As Integer = 0 To _FrameWidth - 1
                k = i * _FrameStep + j
                X(2 * j) = _Window(j) * series.Samples(k)
                X(2 * j + 1) = 0
                Xdt(2 * j) = _TimeDerivWindow(j) * series.Samples(k)
                Xdt(2 * j + 1) = 0
            Next

            ' % -- compute spectra --
            _FFT.Complex(X, True)

            ' % used to compute time and freq corrections
            For j As Integer = 0 To _FrameWidth - 1
                Dim re As Double = (X(2 * j))
                Dim im As Double = (X(2 * j + 1))
                magsqrd(j) = Math.Sqrt(re * re + im * im)
            Next

            ' % -- compute auxiliary spectra --
            _FFT.Complex(Xdt, True)

            Dim fcorrect As Double() = New Double(_FrameWidth \ 2 - 1) {}

            ' % -- compute frequency corrections --
            ' fcorrect = -imag( Xdt * conj(X) ) / magsqrd 
            For j As Integer = 0 To fcorrect.Length - 1

                ' conj(X)
                Dim XRe As Double = X(2 * j)
                Dim XIm As Double = -X(2 * j + 1)

                If XRe <> 0 And XIm <> 0 Then '  Avoid div by zero!

                    ' Xdt 
                    Dim XdtRe As Double = Xdt(2 * j)
                    Dim XdtIm As Double = Xdt(2 * j + 1)

                    ' Xdt * conj(X)
                    Dim a As New Complex(XRe, XIm)      ' X
                    Dim b As New Complex(XdtRe, XdtIm)  ' Xdt
                    Dim m As New Complex(magsqrd(2 * j), magsqrd(2 * j + 1))
                    Dim c As Complex = (a * b) / m ' / magsqrd

                    fcorrect(j) = -c.Im / magsqrd.Length  ' Math.Sqrt(c.Re * c.Re + c.Im * c.Im)
                End If

            Next

            Dim bins As Double() = ExtractLogBins(fcorrect)

            frames(i) = bins

        Next

        Return frames

    End Function

    Private Sub timederivwindow(W As Double(), sampleRate As Double, nderivs As Integer)

        Dim Nw As Integer = W.Length
        Dim Mw As Integer = Nw \ 2

        _TimeDerivWindow = New Double(Nw - 1) {}

        '  % -- construct frequency ramp --
        ' Nw = length(W);
        ' Mw = Nw/2;
        ' framp = [(0:Mw-1),(-Mw:-1)]' + 0.5; <---- two rows, re & im, transposed into two columns, adding .5 to each element??? {0, 1, 2, 3, 4}, {-4, -3, -2, -1}
        Dim framp As Double() = New Double(Nw - 1) {}

        ' First half of ramp... 0, 1, 2, 4, 5...
        For i As Integer = 0 To Mw - 1
            framp(i) = (i + 0.5) / Nw ' Normalize too!
        Next

        ' Second half of ramp... -5, -4, -3, -2, -1, 0...
        For i As Integer = Nw - 1 To Mw Step -1
            framp(i) = (i + 0.5 - Nw) / Nw ' Normalize too!
        Next

        ' framp = framp / Nw;

        ' % -- apply frequency ramp --
        ' for i = 1:nderivs
        '    W = -imag(ifft(framp.*fft(W)));

        ' Make the window complex...
        Dim Wc As Double() = New Double(2 * Nw - 1) {}
        For j As Integer = 0 To Nw - 1
            Wc(2 * j) = W(j)
            Wc(2 * j + 1) = 0
        Next

        ' Then FFT it...
        _FFT.Complex(Wc, True)

        ' Apply the ramp... framp.*fft(W)...
        For i As Integer = 0 To Nw - 1
            Wc(2 * i) *= framp(i)
            Wc(2 * i + 1) *= framp(i)
        Next

        ' Inverse FFT...
        _FFT.Complex(Wc, False)

        ' Keep only the Real... -imag(...) and...
        ' % -- scale to correct units --
        ' Wd = W * (fs^nderivs);
        For i As Integer = 0 To Nw - 1
            _TimeDerivWindow(i) = Wc(2 * i) * Math.Pow(sampleRate, nderivs)
        Next

    End Sub

    Protected Overrides Sub OnSampleRateChanged(sampleRate As Integer)
        _SampleRate = sampleRate
        GenerateLogFrequencies()
        timederivwindow(_Window, _SampleRate, 1)
    End Sub

    ''' <summary>
    '''   Logarithmic spacing of a frequency in a linear domain
    ''' </summary>
    ''' <param name = "spectrum">Spectrum to space</param>
    ''' <returns>Logarithmically spaced signal</returns>
    Public Function ExtractLogBins(ByVal spectrum As Double()) As Double()
        Dim sumFreq As Double() = New Double(_LogBins - 1) {}
        For i As Integer = 0 To _LogBins - 1
            Dim lowBound As Integer = _LogFrequenciesIndex(i)
            Dim hiBound As Integer = _LogFrequenciesIndex(i + 1)
            'If lowBound = hiBound Then hiBound += 1
            For k As Integer = lowBound To hiBound '- 1
                Dim re As Double = spectrum(k)
                'Dim im As Double = spectrum(2 * k + 1)
                're /= _FrameWidth / 2    '//normalize img/re part
                'im /= _FrameWidth / 2   '//doesn't introduce any change in final image (linear normalization)
                sumFreq(i) += re '(Math.Sqrt(re * re + im * im))
            Next
            sumFreq(i) /= (hiBound - lowBound) + 1
        Next
        Return sumFreq
    End Function

    ''' <summary>
    '''        * An array of WDFT [0, 2048], contains a range of [0, 5512] frequency components.
    '''        * Only 1024 contain actual data. In order to find the Index, the fraction is found by dividing the frequency by max frequency
    '''        
    '''   Gets the index in the spectrum vector from according to the starting frequency specified as the parameter
    ''' </summary>
    ''' <param name = "freq">Frequency to be found in the spectrum vector [E.g. 300Hz]</param>
    ''' <param name = "sampleRate">Frequency rate at which the signal was processed [E.g. 5512Hz]</param>
    ''' <param name = "spectrumLength">Length of the spectrum [2048 elements generated by WDFT from which only 1024 are with the actual data]</param>
    ''' <returns>Index of the frequency in the spectrum array</returns>
    ''' <remarks>
    '''   The Bandwidth of the spectrum runs from 0 until SampleRate / 2 [E.g. 5512 / 2]
    '''   Important to remember:
    '''   N points in time domain correspond to N/2 + 1 points in frequency domain
    '''   E.g. 300 Hz applies to 112'th element in the array
    ''' </remarks>
    Private Shared Function FreqToIndex(ByVal freq As Double, ByVal sampleRate As Integer, ByVal spectrumLength As Integer) As Integer
        ' freq/((float) sampleRate/2)
        Dim fraction As Double = freq / (sampleRate / 2) '(freq + 0.5) / (sampleRate / 2)
        'N sampled points in time correspond to [0, N/2] frequency range 
        Dim i As Integer = CInt(Math.Round((spectrumLength / 2 + 1) * fraction))
        'DFT N points defines [N/2 + 1] frequency points
        Return i
    End Function

    Private Sub GenerateLogFrequencies()
        Dim logMin As Double = Math.Log(_MinFrequency, _LogBase)
        Dim logMax As Double = Math.Log(_MaxFrequency, _LogBase)
        Dim delta As Double = (logMax - logMin) / _LogBins

        _LogFrequenciesIndex = New Integer(_LogBins) {}

        Dim accDelta As Double = 0
        For i As Integer = 0 To _LogBins
            '32 octaves
            Dim freq As Double = Math.Pow(_LogBase, logMin + accDelta)
            accDelta += delta
            'Find the start index in array from which to start the summation
            _LogFrequenciesIndex(i) = FreqToIndex(freq, _SampleRate, _FrameWidth)
        Next

        'timederivwindow(Window, _SampleRate, 1)

    End Sub

    <Description("Number of bins."), DefaultValue(64)>
    Public Property LogBins As Integer
        Get
            Return _LogBins
        End Get
        Set(value As Integer)
            If _LogBins <> value Then
                _LogBins = value
                GenerateLogFrequencies()
            End If
        End Set
    End Property

    <Description("Maximum Frequency"), DefaultValue(2855.0)>
    Public Property FrequencyMax As Double
        Get
            Return _MaxFrequency
        End Get
        Set(value As Double)
            If value <> _MaxFrequency Then
                If value <= _MinFrequency Then Throw New ArgumentException("MaxFrequency cannot be less than or equal to MinFrequency.")
                If value >= _SampleRate Then Throw New ArgumentException("MaxFrequency cannot be greater than the SampleRate.")
                _MaxFrequency = value
                GenerateLogFrequencies()
            End If
        End Set
    End Property

    <Description("Minimum Frequency"), DefaultValue(318.0)>
    Public Property FrequencyMin As Double
        Get
            Return _MinFrequency
        End Get
        Set(value As Double)
            If value <> _MinFrequency Then
                If value >= _MaxFrequency Then Throw New ArgumentException("MinFrequency cannot be greater than or equal to MaxFrequency.")
                If value <= 0 Then Throw New ArgumentException("MinFrequency must be greater than 0.")
                _MinFrequency = value
                GenerateLogFrequencies()
            End If
        End Set
    End Property

    ''' <summary>
    ''' The width of each frame.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <TypeConverter(GetType(EnumIntValueConverter)), DefaultValue(FrameWidth._512)>
    Public Overridable Property FrameWidth As FrameWidth
        Get
            Return _FrameWidth
        End Get
        Set(value As FrameWidth)
            If _FrameWidth <> value Then
                _FrameWidth = value
                _Window = Windows.Window.GetInstance(_WindowType).GetWindow(_FrameWidth)
                timederivwindow(_Window, _SampleRate, 1)
            End If
        End Set
    End Property

    ''' <summary>
    ''' The amount of step between each overlapping frame.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <TypeConverter(GetType(EnumIntValueConverter)), Description(""), DefaultValue(FrameStep._64)>
    Public Overridable Property FrameStep As FrameStep
        Get
            Return _FrameStep
        End Get
        Set(value As FrameStep)
            _FrameStep = value
        End Set
    End Property

    '<DisplayName("Phase"), Description("FFT phase mode."), DefaultValue(FFT.Phase.Signal)>
    'Public Property PhaseMode As FFT.Phase
    '    Get
    '        Return _Phase
    '    End Get
    '    Set(value As FFT.Phase)
    '        If value <> _Phase Then
    '            _Phase = value
    '            _FFT.PhaseMode = _Phase
    '        End If
    '    End Set
    'End Property

    ''' <summary>
    ''' The type of window to be applied to each frame.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("Window"), Description("The type of window to be applied."), DefaultValue(WindowType.Hanning)>
    Public Property WindowType As WindowType
        Get
            Return _WindowType
        End Get
        Set(value As WindowType)
            If value <> _WindowType Then
                _WindowType = value
                _Window = Windows.Window.GetInstance(_WindowType).GetWindow(_FrameWidth)
                timederivwindow(_Window, _SampleRate, 1)
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Frequency Reassigned Spectrogram"
        End Get
    End Property

 
End Class
