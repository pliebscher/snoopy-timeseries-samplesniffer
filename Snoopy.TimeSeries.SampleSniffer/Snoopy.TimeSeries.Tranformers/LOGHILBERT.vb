Imports System.ComponentModel
''' <summary>
''' https://github.com/dhale/jtk/blob/master/src/main/java/edu/mines/jtk/dsp/HilbertTransformFilter.java
''' </summary>
''' <remarks></remarks>
Public Class LOGHILBERT
    Inherits TimeSeriesFrameTransformer

    Private _LogFrequenciesIndex As Integer()
    Private _LogBinFrequencies As Integer()
    Private _LogBins As Integer = 128
    Private _LogBase As Double = Math.PI
    Private _SampleRate As Integer = 22050
    Private _FrameWidth As Integer = 256

    Private _MaxCoeffs As Integer = 1024 ' maximum number of coefficients in filter.
    Private _MaxError As Double = 0.01
    Private _MaxFrequency As Double = 1300.0 '0.075
    Private _MinFrequency As Double = 333.0 '0.0025
    Private _filter As Double()
    Private _FFT As New FFT
    Private _Phase As FFT.Phase = FFT.Phase.Default

    Public Sub New()
        _FFT.PhaseMode = _Phase
        MyBase.FrameWidth = Snoopy.TimeSeries.FrameWidth._256
        MyBase.FrameStep = Snoopy.TimeSeries.FrameStep._32
        Reset()
    End Sub

    Protected Overrides Function OnTransformFrame(frame() As Double) As Double()

        Dim n As Integer = frame.Length
        Dim y As Double() = New Double(frame.Length - 1) {}

        _FFT.Real(frame, True)

        Conv(_filter.Length, CInt(-(_filter.Length - 1) / 2), _filter, n, ky, frame, n, kz, y)

        '_FFT.Real(y, False)

        Return ExtractLogBins(y)

        'Dim complex As Complex() = New Complex(frame.Length - 1) {}

        'For i As Integer = 0 To frame.Length - 1
        '    complex(i) = New Complex(frame(i), 0)
        'Next

        'Return WingerTransform(complex, frame.Length - 1)

    End Function

    Protected Overrides Sub OnSampleRateChanged(sampleRate As Integer)
        _SampleRate = sampleRate
        Reset()
    End Sub

    Protected Overrides Sub OnFrameWidthChanged(width As Integer)
        _FrameWidth = width
        Reset()
    End Sub

    Private Sub Reset()
        _filter = CreateFilter(_MaxCoeffs, _MaxError, _MinFrequency / _SampleRate, _MaxFrequency / _SampleRate)
        GenerateLogFrequencies()
    End Sub

    Private Shared Function CreateFilter(nmax As Integer, emax As Double, fmin As Double, fmax As Double) As Double()
        If fmin < 0.5 - fmax Then
            fmax = 0.5 - fmin
        End If
        Dim width As Double = 2.0 * (0.5 - fmax)
        Dim emaxWindow As Double = 0.5 * emax
        Dim kw As KaiserWindow = KaiserWindow.FromErrorAndWidth(emaxWindow, width)
        Dim n As Integer = 1 + CInt(Math.Ceiling(kw.Length()))
        If n Mod 2 = 0 Then
            n += 1
        End If
        If n > nmax Then
            n = nmax
        End If
        If n Mod 2 = 0 Then
            n -= 1
        End If
        Dim length As Double = n
        kw = KaiserWindow.FromWidthAndLength(width, length)
        Dim f As Double() = New Double(n - 1) {}
        Dim k As Integer = (n - 1) \ 2
        Dim i As Integer = 0, j As Integer = n - 1
        While i < k
            Dim x As Double = i - k
            Dim fideal As Double = 0
            If x <> 0 Then
                Dim y As Double = 0.5 * Math.PI * x
                Dim s As Double = Math.Sin(y)
                fideal = -s * s / y
            End If
            Dim window As Double = kw.Evaluate(x)
            f(i) = fideal * window
            f(j) = -f(i)
            i += 1
            j -= 1
        End While
        f(k) = 0.0
        Return f
    End Function

    Private Shared Sub Conv(lx As Integer, kx As Integer, x As Double(), ly As Integer, ky As Integer, y As Double(), lz As Integer, kz As Integer, z As Double())
        ' If necessary, swap x and y so that x is the shorter sequence.
        ' This simplifies the logic below.
        If lx > ly Then
            Dim lt As Integer = lx
            lx = ly
            ly = lt
            Dim kt As Integer = kx
            kx = ky
            ky = kt
            Dim t As Double() = x
            x = y
            y = t
        End If

        ' Bounds for index i.
        Dim imin As Integer = kz - kx - ky
        Dim imax As Integer = imin + lz - 1

        ' Variables that we expect to reside in registers.
        Dim i As Integer, ilo As Integer, ihi As Integer, j As Integer, jlo As Integer, jhi As Integer, iz As Integer
        Dim sa As Double, sb As Double, xa As Double, xb As Double, ya As Double, yb As Double

        ' Off left: imin <= i <= -1
        ilo = imin
        ihi = Math.Min(-1, imax)
        i = ilo
        iz = i - imin
        While i <= ihi
            z(iz) = 0.0
            i += 1
            iz += 1
        End While

        ' Rolling on: 0 <= i <= lx-2 and 0 <= j <= i
        ilo = Math.Max(0, imin)
        ihi = Math.Min(lx - 2, imax)
        jlo = 0
        jhi = ilo
        i = ilo
        iz = i - imin
        While i < ihi
            sa = 0.0
            sb = 0.0
            yb = y(i - jlo + 1)
            For j = jlo To jhi - 1 Step 2
                xa = x(j)
                sb += xa * yb
                ya = y(i - j)
                sa += xa * ya
                xb = x(j + 1)
                sb += xb * ya
                yb = y(i - j - 1)
                sa += xb * yb
            Next
            xa = x(j)
            sb += xa * yb
            If j = jhi Then
                ya = y(i - j)
                sa += xa * ya
                xb = x(j + 1)
                sb += xb * ya
            End If
            z(iz) = sa
            z(iz + 1) = sb
            i += 2
            iz += 2
            jhi += 2
        End While
        If i = ihi Then
            jlo = 0
            jhi = i
            sa = 0.0
            For j = jlo To jhi
                sa += x(j) * y(i - j)
            Next
            z(iz) = sa
        End If

        ' Middle: lx-1 <= i <= ly-1 and 0 <= j <= lx-1
        ilo = Math.Max(lx - 1, imin)
        ihi = Math.Min(ly - 1, imax)
        jlo = 0
        jhi = lx - 1
        i = ilo
        iz = i - imin
        While i < ihi
            sa = 0.0
            sb = 0.0
            yb = y(i - jlo + 1)
            For j = jlo To jhi - 1 Step 2
                xa = x(j)
                sb += xa * yb
                ya = y(i - j)
                sa += xa * ya
                xb = x(j + 1)
                sb += xb * ya
                yb = y(i - j - 1)
                sa += xb * yb
            Next
            If j = jhi Then
                xa = x(j)
                sb += xa * yb
                ya = y(i - j)
                sa += xa * ya
            End If
            z(iz) = sa
            z(iz + 1) = sb
            i += 2
            iz += 2
        End While
        If i = ihi Then
            sa = 0.0
            For j = jlo To jhi
                sa += x(j) * y(i - j)
            Next
            z(iz) = sa
        End If

        ' Rolling off: ly <= i <= lx+ly-2 and i-ly+1 <= j <= lx-1
        ilo = Math.Max(ly, imin)
        ihi = Math.Min(lx + ly - 2, imax)
        jlo = ihi - ly + 1
        jhi = lx - 1
        i = ihi
        iz = i - imin
        While i > ilo
            sa = 0.0
            sb = 0.0
            yb = y(i - jhi - 1)
            For j = jhi To jlo + 1 Step -2
                xa = x(j)
                sb += xa * yb
                ya = y(i - j)
                sa += xa * ya
                xb = x(j - 1)
                sb += xb * ya
                yb = y(i - j + 1)
                sa += xb * yb
            Next
            xa = x(j)
            sb += xa * yb
            If j = jlo Then
                ya = y(i - j)
                sa += xa * ya
                xb = x(j - 1)
                sb += xb * ya
            End If
            z(iz) = sa
            z(iz - 1) = sb
            i -= 2
            iz -= 2
            jlo -= 2
        End While
        If i = ilo Then
            jlo = i - ly + 1
            jhi = lx - 1
            sa = 0.0
            For j = jhi To jlo Step -1
                sa += x(j) * y(i - j)
            Next
            z(iz) = sa
        End If

        ' Off right: lx+ly-1 <= i <= imax
        ilo = Math.Max(lx + ly - 1, imin)
        ihi = imax
        i = ilo
        iz = i - imin
        While i <= ihi
            z(iz) = 0.0
            i += 1
            iz += 1
        End While
    End Sub

    Public Function ExtractLogBins(ByVal spectrum As Double()) As Double()
        Dim sumFreq As Double() = New Double(_LogBins - 1) {}
        For i As Integer = 0 To _LogBins - 1
            Dim lowBound As Integer = _LogFrequenciesIndex(i)
            Dim hiBound As Integer = _LogFrequenciesIndex(i + 1)
            For k As Integer = lowBound To hiBound - 1
                Dim re As Double = spectrum(k)
                sumFreq(i) += re
            Next
            sumFreq(i) /= (hiBound - lowBound)
        Next
        Return sumFreq
    End Function

    Private Sub GenerateLogFrequencies()
        Dim logMin As Double = Math.Log(_MinFrequency, _LogBase)
        Dim logMax As Double = Math.Log(_MaxFrequency, _LogBase)
        Dim delta As Double = (logMax - logMin) / _LogBins

        _LogFrequenciesIndex = New Integer(_LogBins) {}
        _LogBinFrequencies = New Integer(_LogBins) {}

        Dim accDelta As Double = 0
        For i As Integer = 0 To _LogBins
            '32 octaves
            Dim freq As Double = Math.Pow(_LogBase, logMin + accDelta)
            accDelta += delta
            ' accDelta = delta * i
            'Find the start index in array from which to start the summation
            _LogFrequenciesIndex(i) = FreqToIndex(freq, _SampleRate, _FrameWidth)
            If i > 0 AndAlso _LogFrequenciesIndex(i) <= _LogFrequenciesIndex(i - 1) Then
                _LogFrequenciesIndex(i) = _LogFrequenciesIndex.Max + 1
            End If
            _LogBinFrequencies(i) = CInt(freq)
        Next
    End Sub

    Private Shared Function FreqToIndex(ByVal freq As Double, ByVal sampleRate As Integer, ByVal spectrumLength As Integer) As Integer
        Dim fraction As Double = (freq + 0.5) / (sampleRate / 2)
        'N sampled points in time correspond to [0, N/2] frequency range 
        Dim i As Integer = CInt(Math.Round((spectrumLength / 2 + 1) * fraction))
        'DFT N points defines [N/2 + 1] frequency points
        Return i
    End Function

    Public ReadOnly Property BinFreqs As Integer()
        Get
            Return _LogBinFrequencies
        End Get
    End Property

    Public Property ky As Integer = 0
    Public Property kz As Integer = 0

    <Description("Number of bins."), DefaultValue(128)>
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

    <Description("Number of filter coefficients."), DefaultValue(64)>
    Public ReadOnly Property Coeffs As Integer
        Get
            Return _filter.Length
        End Get
    End Property

    <Description("Maximum filter coefficients."), DefaultValue(1024)>
    Public Property CoeffsMax As Integer
        Get
            Return _MaxCoeffs
        End Get
        Set(value As Integer)
            If value <> _MaxCoeffs Then
                _MaxCoeffs = value
                Reset()
            End If
        End Set
    End Property

    <Description("Maximum Error."), DefaultValue(0.01)>
    Public Property ErrorMax As Double
        Get
            Return _MaxError
        End Get
        Set(value As Double)
            If value <> _MaxError Then
                _MaxError = value
                Reset()
            End If
        End Set
    End Property

    <Description("Maximum Frequency"), DefaultValue(1300.0)>
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

    <Description("Minimum Frequency"), DefaultValue(333.0)>
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

    <DisplayName("Phase"), Description("FFT phase mode."), DefaultValue(FFT.Phase.Default)>
    Public Property PhaseMode As FFT.Phase
        Get
            Return _Phase
        End Get
        Set(value As FFT.Phase)
            If value <> _Phase Then
                _Phase = value
                _FFT.PhaseMode = _Phase
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Log Hilbert Transform"
        End Get
    End Property

#Region " -- KaiserWindow -- "

    Public Class KaiserWindow

        Private Shared DBL_EPSILON As Double = 0.000000000000000222044604925031

        Private _error As Double
        Private _width As Double
        Private _length As Double
        Private _alpha As Double
        Private _scale As Double
        Private _xxmax As Double

        Public Shared Function FromErrorAndWidth([error] As Double, width As Double) As KaiserWindow
            'Check.argument([error] > 0.0, "error>0.0")
            'Check.argument([error] < 1.0, "error<1.0")
            'Check.argument(width > 0.0, "width>0.0")
            Dim a As Double = -20.0 * Math.Log10([error])
            Dim d As Double = If((a > 21.0), (a - 7.95) / 14.36, 0.9222)
            Dim length As Double = d / width
            Return New KaiserWindow([error], width, length)
        End Function

        Public Shared Function FromWidthAndLength(width As Double, length As Double) As KaiserWindow
            'Check.argument(width > 0.0, "width>0.0")
            'Check.argument(length > 0, "length>0")
            'Check.argument(width * length >= 1.0, "width*length>=1.0")
            Dim d As Double = width * length
            Dim a As Double = 14.36 * d + 7.95
            Dim [error] As Double = Math.Pow(10.0, -a / 20.0)
            Return New KaiserWindow([error], width, length)
        End Function

        Public Function Evaluate(x As Double) As Double
            Dim xx As Double = x * x
            Return If((xx <= _xxmax), _scale * ino(_alpha * Math.Sqrt(1.0 - xx / _xxmax)), 0.0)
        End Function

        Public ReadOnly Property Length() As Double
            Get
                Return _length
            End Get
        End Property

        Private Sub New([error] As Double, width As Double, length As Double)
            _error = [error]
            _width = width
            _length = length
            Dim a As Double = -20.0 * Math.Log10(_error)
            If a <= 21.0 Then
                _alpha = 0.0
            ElseIf a <= 50.0 Then
                _alpha = 0.5842 * Math.Pow(a - 21.0, 0.4) + 0.07886 * (a - 21.0)
            Else
                _alpha = 0.1102 * (a - 8.7)
            End If
            _scale = 1.0 / ino(_alpha)
            _xxmax = 0.25 * _length * _length
        End Sub

        Private Function ino(x As Double) As Double
            Dim s As Double = 1.0
            Dim ds As Double = 1.0
            Dim d As Double = 0.0
            Do
                d += 2.0
                ds *= (x * x) / (d * d)
                s += ds
            Loop While ds > s * DBL_EPSILON
            Return s
        End Function

    End Class

#End Region

End Class
