Imports Snoopy.TimeSeries
Imports System.ComponentModel
Imports Snoopy.TimeSeries.Windows
''' <summary>
''' http://sourceforge.net/p/pamguard/svn/HEAD/tree/PamguardJava/trunk/core/src/pamMaths/WignerTransform.java
''' </summary>
''' <remarks></remarks>
Public Class WT
    Inherits TimeSeriesTransformer

    Private _FrameWidth As FrameWidth = FrameWidth._128

    Private _FFT As New FFT
    Private _FFTPhase As FFT.Phase = FFT.Phase.Signal

    Private _WindowType As WindowType = WindowType.Hanning
    Private _Window As Window = Windows.Window.Hanning
    Private _Win As Double() = _Window.GetWindow(_FrameWidth)

    Public Sub New()
        _FFT.PhaseMode = _FFTPhase
    End Sub

    Protected Overrides Function OnTransform(series As ITimeSeries(Of Double())) As Double()()

        If _Win.Length <> series.Samples.Length Then
            _Win = _Window.GetWindow(series.Samples.Length)
        End If

        Dim complex As Complex() = New Complex(series.Samples.Length - 1) {}

        For i As Integer = 0 To series.Samples.Length - 1
            complex(i) = New Complex(_Win(i) * series.Samples(i), 0)
        Next

        Return WingerTransform(complex, _FrameWidth)

    End Function

    Private Function WingerTransform(x As Complex(), N As Integer) As Double()()

        Dim _FrameStep As Integer = 32
        ' Width
        Dim W As Integer = (x.Length - N) \ _FrameStep

        ' Complex[][] tfr = Complex.allocateComplexArray(N, N);
        Dim tfr As Complex()() = New Complex(N - 1)() {}
        ' double[][] d = new double[N][N];
        Dim d As Double()() = New Double(N - 1)() {}
        ' for (int i = 0; i < N; i++) {
        '   d[i] = new double[N];
        ' } 
        For i As Integer = 0 To N - 1
            d(i) = New Double(N - 1) {}
        Next

        Dim xrow As Integer, xcol As Integer, taumax As Integer
        Dim tau As Integer, indices As Integer
        'Dim fft As New FastFFT()

        xrow = N
        xcol = 2
        For iCol As Integer = 0 To N - 1
            tfr(iCol) = New Complex(N - 1) {}
            taumax = min(iCol, xrow - iCol - 1, CInt(Math.Round(N)) \ 2 - 1)

            For tau = -taumax To taumax
                indices = (N + tau) Mod N
                'tfr(iCol)(indices) = x(iCol + tau).times(0.5).times(x(iCol - tau).conj())
                tfr(iCol)(indices) = x(iCol + tau) * (0.5) * (x(iCol - tau).GetConjugate)
            Next
            tau = CInt(Math.Round(N / 2))
            If iCol < xrow - tau AndAlso iCol >= tau Then
                'tfr(iCol)(tau) = x(iCol + tau).times(x(iCol - tau).conj()).plus(x(iCol - tau).times(x(iCol + tau).conj())).times(0.5)
                tfr(iCol)(tau) = x(iCol + tau) * (x(iCol - tau).GetConjugate) + (x(iCol - tau) * (x(iCol + tau).GetConjugate)) * (0.5)
            End If

        Next

        For i As Integer = 0 To N - 1
            Dim complex As Double() = New Double(2 * N - 1) {}
            For j As Integer = 0 To N - 1
                complex(2 * j) = tfr(i)(j).Re
                complex(2 * j + 1) = tfr(i)(j).Im
            Next

            '_FFT.Complex(tfr(i), True)
            _FFT.Complex(Complex, True)

            Dim real As Double() = New Double((N \ 2) - 1) {}
            For j As Integer = 0 To (N \ 2) - 1
                Dim re As Double = complex(2 * j) + complex((2 * j) + N - 1)
                'Dim im As Double = complex(2 * j + 1)
                real(j) = re ' Math.Sqrt(re * re + im * im)
            Next
            d(i) = real
        Next

        'Dim rotated As Double()() = New Double(d(0).Length - 1)() {}

        'For i As Integer = 0 To d(0).Length - 1
        '    rotated(i) = New Double(d.Length - 1) {}
        '    For j As Integer = d.Length - 1 To 0
        '        rotated(i)(j) = d(j)(i)
        '    Next
        'Next

        Return d 'rotated
    End Function

    Private Shared Function min(a As Integer, b As Integer, c As Integer) As Integer
        Return Math.Min(a, Math.Min(b, c))
    End Function

    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function

    <TypeConverter(GetType(EnumIntValueConverter)), DefaultValue(FrameWidth._128)>
    Public Overridable Property FrameWidth As FrameWidth
        Get
            Return _FrameWidth
        End Get
        Set(value As FrameWidth)
            If _FrameWidth <> value Then
                _FrameWidth = value
                _Win = Windows.Window.GetInstance(_WindowType).GetWindow(_FrameWidth)
            End If
        End Set
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
            End If
        End Set
    End Property

    ''' <summary>
    ''' The type of window to be applied to each frame.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Description("The type of window to be applied."), DefaultValue(WindowType.Hanning)>
    Public Property Window As WindowType
        Get
            Return _WindowType
        End Get
        Set(value As WindowType)
            If value <> _WindowType Then
                _WindowType = value
                _Window = Windows.Window.GetInstance(_WindowType)
                _Win = _Window.GetWindow(_FrameWidth)
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Wigner Transform"
        End Get
    End Property

End Class
