Imports System.ComponentModel
''' <summary>
''' http://www.dsprelated.com/showmessage/13193/1.php
''' </summary>
''' <remarks></remarks>
Public Class LOGCZT
    Inherits TimeSeriesFrameTransformer

    Private _L As Integer
    Private _LogBins As Integer = 64
    Private _LogFreqs As Double()

    Private _ArcCoefs As Double()
    Private _Theta0 As Double
    Private _Phi0 As Double

    Private _SampleRate As Integer = 22050
    Private _FrameWidth As FrameWidth
    Private _MaxFrequency As Double = 1855.0
    Private _MinFrequency As Double = 318.0
    Private _Phase As FFT.Phase = FFT.Phase.Signal

    Private _FFT As New FFT

    Public Sub New()

        _FrameWidth = MyBase.FrameWidth
        _FFT.PhaseMode = _Phase

        Reset()

    End Sub

    Protected Overloads Overrides Function OnTransformFrame(frame As Double()) As Double()

        Dim complexSignal As Double() = New Double(2 * _FrameWidth - 1) {}

        For j As Integer = 0 To _FrameWidth - 1
            complexSignal(2 * j) = frame(j)
            complexSignal(2 * j + 1) = 0
        Next

        Dim complex As Double() = czt(complexSignal, _FrameWidth, _LogBins, _MinFrequency, _MaxFrequency, _SampleRate)
        Dim freqs As Double() = New Double((complex.Length \ 2) - 1) {}

        ' take first half...
        For i As Integer = 0 To freqs.Length - 1
            freqs(i) = complex(i)
        Next

        freqs = ComputeBins(complex)

        Return freqs 'bins

    End Function

    Public Function createLogarithmicBands(lowFrequency As Double, highFrequency As Double, numberOfBands As Integer) As Double()
        'double factor = _LogBins / (Math.log(highFrequency / lowFrequency) / LOG2);
        Dim factor As Double = numberOfBands / (Math.Log(highFrequency / lowFrequency) / Math.Log(2.0))
        'float scale[] = new float[_LogBins+1];
        Dim scale As Double() = New Double(numberOfBands + 1) {}
        scale(0) = lowFrequency
        For i As Integer = 1 To numberOfBands
            scale(i) = lowFrequency * CSng(Math.Pow(2, i / factor))
        Next
        Return scale
    End Function

    Private Function ComputeBins(values As Double()) As Double()
        Dim bins As Double() = New Double(_LogBins - 1) {}
        Dim currentBin As Double = 0
        Dim binIndex As Integer = 0
        For i As Integer = 0 To _LogFreqs.Length - 1
            Dim freq As Double = _LogFreqs(i)

            If i = _LogFreqs.Length - 1 Then
                binIndex = binIndex
            End If

            'While binIndex < _LogFreqs.Length And freq >= _LogFreqs(binIndex)
            '    bins(binIndex) = currentBin
            '    currentBin = 0
            '    binIndex += 1
            'End While

            Dim re As Double = values(2 * i) '/ _FrameWidth
            Dim im As Double = values(2 * i + 1) '/ _FrameWidth
            currentBin += Math.Sqrt(re * re + im * im)

            If binIndex < _LogFreqs.Length - 2 And freq >= _LogFreqs(binIndex + 1) Then
                bins(binIndex) = currentBin
                currentBin = 0
                binIndex += 1
            End If


        Next
        bins(binIndex - 1) = currentBin
        ' remove outOfBand bins
        'Dim inBandBins As Double() = New Double(_LogBins - 1) {}
        'Array.Copy(bins, 1, inBandBins, 0, inBandBins.Length)
        Return bins 'inBandBins
    End Function

    Protected Overrides Sub OnFrameWidthChanged(width As Integer)
        Reset()
    End Sub

    Protected Overrides Sub OnSampleRateChanged(sampleRate As Integer)
        If Me._SampleRate <> sampleRate Then
            Me._SampleRate = sampleRate
            Me.Reset()
        End If
    End Sub

    Public Function Apply(signal As Double()) As Double()

        Dim Psi As Double
        Dim n1 As Integer
        Dim K As Integer
        Dim s As Double
        Dim c As Double

        For n1 = 0 To _FrameWidth - 1
            Psi = n1 * _Theta0 + n1 * n1 / 2.0 * _Phi0
            c = Math.Cos(Psi)
            s = -Math.Sin(Psi)
            signal(2 * n1) = c * signal(2 * n1) - s * signal(2 * n1 + 1)
            signal(2 * n1 + 1) = s * signal(2 * n1) + c * signal(2 * n1 + 1)
        Next

        ' Forward...
        _FFT.Complex(signal, True)

        For n1 = 0 To _FrameWidth - 1
            c = signal(2 * n1)
            s = signal(2 * n1 + 1)
            signal(2 * n1) = c * _ArcCoefs(2 * n1) - s * _ArcCoefs(2 * n1 + 1) / _FrameWidth
            signal(2 * n1 + 1) = s * _ArcCoefs(2 * n1) + c * _ArcCoefs(2 * n1 + 1) / _FrameWidth
        Next

        ' Reverse...
        _FFT.Complex(signal, False)

        Dim sig As Double() = New Double(_LogBins - 1) {}

        For K = 0 To _FrameWidth - 1
            Psi = K * K / 2.0 * _Phi0
            c = Math.Cos(Psi)
            s = -Math.Sin(Psi)
            signal(2 * K) = c * signal(2 * K) - s * signal(2 * K + 1)
            signal(2 * K + 1) = s * signal(2 * K) + c * signal(2 * K + 1)
        Next

        Return signal

    End Function

    Private Sub Reset()

        Dim M As Integer = _LogBins
        Dim N As Integer = _FrameWidth

        _L = _FrameWidth

        ' Generate Arc coefficients...
        ' ---------------------------------------------------------------------
        Dim n_ As Integer

        _ArcCoefs = New Double(_L * 2 - 1) {}

        _Phi0 = 2 * Math.PI * (_MaxFrequency - _MinFrequency) / _SampleRate / (M - 1)
        _Theta0 = 2 * Math.PI * _MinFrequency / _SampleRate

        ''* Create arc coefficients 
        ''    for( n_ = 0; n_ < M; n_++ ){
        ''      h[n_][0] = Math.cos( n_*n_/2.*phi0 );
        ''      h[n_][1] = Math.sin( n_*n_/2.*phi0 );
        ''    }
        For n_ = 0 To 2 * M - 1
            _ArcCoefs(2 * n_) = Math.Cos(n_ * n_ / 2 * _Phi0)
            _ArcCoefs(2 * n_ + 1) = Math.Sin(n_ * n_ / 2 * _Phi0)
        Next
        ''    for( n_ = M; n_ < L-N; n_++ ){
        ''      h[n_][0] = 0.;
        ''      h[n_][1] = 0.;
        ''    }
        For n_ = M To _L - N - 1
            _ArcCoefs(2 * n_) = 0
            _ArcCoefs(2 * n_ + 1) = 0
        Next
        ''    for( n_ = L-N; n_ < L; n_++){
        ''      h[n_][0] = Math.cos( (L-n_)*(L-n_)/2.*phi0 );
        ''      h[n_][1] = Math.sin( (L-n_)*(L-n_)/2.*phi0 );
        ''   {
        ''*
        For n_ = _L - N To _L - 1
            _ArcCoefs(2 * n_) = Math.Cos((_L - n_) * (_L - n_) / 2 * _Phi0)
            _ArcCoefs(2 * n_ + 1) = Math.Sin((_L - n_) * (_L - n_) / 2 * _Phi0)
        Next

        'h = fft_1d(h) 'fft of arc coeff
        _FFT.Complex(_ArcCoefs, True)

        _LogFreqs = createLogarithmicBands(_MinFrequency, _MaxFrequency, _LogBins)


    End Sub

    '***********Chirp Z Transform *****************
    '   * N = # input samples. M = # output samples.
    '   *
    '   * fsam   = the sample frequency in Hz.
    '   * fstart = the start frequency in Hz.
    '   * fstop  = the stop frequency in Hz for the band over which the transform is computed.
    '   *
    '   * (fstart - fstop)/M = new resolution  
    '   *
    '   * Note: this method returns an array of length L. 
    '   * L = the returned transform length. L will always be larger than M.
    '   * See code for how L is determined   
    '   *
    '   *********************************************
    ' public double[ ][ ] czt( double [ ][ ] array, int N, int M, double fStart, double fStop, double fSam )
    Private Function czt(complexArray As Double(), N As Integer, M As Integer, fStart As Double, fStop As Double, fSam As Double) As Double()

        'double[ ][ ] g = new double[L][2];  double[ ][ ] h = new double[L][2];  double theta0;
        Dim g As Double() = New Double(_L * 2 - 1) {} ' Need to make this an interleaved (re)(im)(re)(im)... array.
        ' arc coefficients
        'Dim h As Double() = New Double(_L * 2 - 1) {} ' Need to make this an interleaved (re)(im)(re)(im)... array.

        'Dim theta0 As Double
        'Dim phi0 As Double
        Dim psi As Double
        Dim a As Double
        Dim b As Double
        Dim n_ As Integer
        Dim k As Integer

        'phi0 = 2 * Math.PI * (fStop - fStart) / fSam / (M - 1) ' ....  phi0 = 2*Math.PI*(fStop-fStart)/fSam/(M-1);
        'theta0 = 2 * Math.PI * fStart / fSam

        ' ''* Create arc coefficients 
        ' ''    for( n_ = 0; n_ < M; n_++ ){
        ' ''      h[n_][0] = Math.cos( n_*n_/2.*phi0 );
        ' ''      h[n_][1] = Math.sin( n_*n_/2.*phi0 );
        ' ''    }
        'For n_ = 0 To 2 * M - 1
        '    h(2 * n_) = Math.Cos(n_ * n_ / 2 * phi0)
        '    h(2 * n_ + 1) = Math.Sin(n_ * n_ / 2 * phi0)
        'Next
        ' ''    for( n_ = M; n_ < L-N; n_++ ){
        ' ''      h[n_][0] = 0.;
        ' ''      h[n_][1] = 0.;
        ' ''    }
        'For n_ = M To _L - N - 1
        '    h(2 * n_) = 0
        '    h(2 * n_ + 1) = 0
        'Next
        ' ''    for( n_ = L-N; n_ < L; n_++){
        ' ''      h[n_][0] = Math.cos( (L-n_)*(L-n_)/2.*phi0 );
        ' ''      h[n_][1] = Math.sin( (L-n_)*(L-n_)/2.*phi0 );
        ' ''   {
        ' ''*
        'For n_ = _L - N To _L - 1
        '    h(2 * n_) = Math.Cos((_L - n_) * (_L - n_) / 2 * phi0)
        '    h(2 * n_ + 1) = Math.Sin((_L - n_) * (_L - n_) / 2 * phi0)
        'Next

        '** Prepare signal ** ' <---- do we need this? Can we process the input siganl inline?
        For n_ = 0 To N - 1
            g(2 * n_) = complexArray(2 * n_)
            g(2 * n_ + 1) = complexArray(2 * n_ + 1)
        Next

        ' Don't need to zer-pad for pow2 impl...
        'For n_ = N To _L - 1
        '    g(2 * n_) = 0
        '    g(2 * n_ + 1) = 0
        'Next

        Dim s As Double
        Dim c As Double

        For n_ = 0 To N - 1
            psi = n_ * _Theta0 + n_ * n_ / 2 * _Phi0
            c = Math.Cos(psi)
            s = -Math.Sin(psi)
            a = c * g(2 * n_) - s * g(2 * n_ + 1)
            b = s * g(2 * n_) + c * g(2 * n_ + 1)
            g(2 * n_) = a
            g(2 * n_ + 1) = b
        Next

        'use your favorite fft algorithm here.
        'g = fft_1d(g) 'fft of samples    
        _FFT.Complex(g, True)
        'h = fft_1d(h) 'fft of arc coeff
        '_FFT.Complex(h, True)

        '* convolution in the time domain is multiplication in the frequency domain *
        '* multiplication in the time domain is convolution in the frequency domain *
        For n_ = 0 To _L - 1
            c = g(2 * n_)
            s = g(2 * n_ + 1)
            'a = c * h(2 * n_) - s * h(2 * n_ + 1)
            'b = s * h(2 * n_) + c * h(2 * n_ + 1)
            a = c * _ArcCoefs(2 * n_) - s * _ArcCoefs(2 * n_ + 1)
            b = s * _ArcCoefs(2 * n_) + c * _ArcCoefs(2 * n_ + 1)
            ' for scaling purposes since fft_1d does not use scale 
            g(2 * n_) = a / _L
            g(2 * n_ + 1) = b / _L
        Next

        'g = ifft_1d(g) 'use your favorite fft algorithm here.
        _FFT.Complex(g, False)

        'Return g

        For k = 0 To M - 1
            psi = k * k / 2 * _Phi0
            c = Math.Cos(psi)
            s = -Math.Sin(psi)
            a = c * g(2 * k) - s * g(2 * k + 1)
            b = s * g(2 * k) + c * g(2 * k + 1)

            g(2 * k) = a
            g(2 * k + 1) = b
        Next

        Return g

    End Function

    <Description("Number of bins."), DefaultValue(64)>
    Public Property LogBins As Integer
        Get
            Return _LogBins
        End Get
        Set(value As Integer)
            If _LogBins <> value Then
                _LogBins = value
                Reset()
            End If
        End Set
    End Property

    <Description("Max Frequency"), DefaultValue(1855.0)>
    Public Property FrequencyMax As Double
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

    <Description("Min Frequency"), DefaultValue(318.0)>
    Public Property FrequencyMin As Double
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

    <DisplayName("Phase"), Description("FFT phase mode."), DefaultValue(FFT.Phase.Signal)>
    Public Property PhaseMode As FFT.Phase
        Get
            Return _Phase
        End Get
        Set(value As FFT.Phase)
            If value <> _Phase Then
                _Phase = value
                _FFT.PhaseMode = _Phase
                Reset()
            End If
            _Phase = value
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Log Chirp Z Transform"
        End Get
    End Property

End Class


