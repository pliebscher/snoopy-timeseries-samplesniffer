Imports Snoopy.TimeSeries
Imports System.ComponentModel
Imports Snoopy.TimeSeries.Windows
''' <summary>
''' https://code.google.com/p/tspl/source/browse/trunk/include/dgt-impl.h
' ****************************************************************************
' * Discrete Gabor Transform.
' *
' * These routines are designed for calculating discrete Gabor transform and
' * its inversion of 1D signals. In order to eliminate the border effect, the
' * input signal("signal") is extended by three forms: zeros padded("zpd"),
' * periodized extension("ppd") and symetric extension("sym").
' *
' * The analysis/synthesis function is given by users, and it's daul
' * (synthesis/analysis) function can be computed by "daul" routine. The over
' * sampling rate is equal to N/dM, where N denotes frequency sampling numbers
' * and dM denotes the time sampling interval.
' *
' * N and dM should can be devided evenly by the window length "Lw". The
' * recovered signal just has the elements from 1 to dM*floor(Ls/dM) of the
' * original signal. So you'd better let dM can be deviede evenly by  the
' * original signal length "Ls".
' *
' * Zhang Ming, 2010-03, Xi'an Jiaotong University.
' ****************************************************************************
''' </summary>
''' <remarks></remarks>
Public Class DGT
    Inherits TimeSeriesTransformer

    Private _FrameWidth As FrameWidth = FrameWidth._128

    Private _FFT As New FFT
    Private _FFTPhase As FFT.Phase = FFT.Phase.Default

    Private _WindowType As WindowType = WindowType.Hanning
    Private _Window As Window = Windows.Window.Hanning
    Private _Win As Double() = _Window.GetWindow(128) '_FrameWidth)

    Private _DualWindow As Double()

    Public Sub New()
        _FFT.PhaseMode = _FFTPhase
        creatDualeWindow()
    End Sub

    Protected Overrides Function OnTransform(series As ITimeSeries(Of Double())) As Double()()

        Return dgt(series.Samples, _DualWindow, 128, 8, "sym")

    End Function

    Private Sub creatDualeWindow()

        Dim Lg As Integer = 55
        Dim N As Integer = 15
        Dim dM As Integer = 8

        ' r = sqrt( dM*N / Type(2*PI) );
        Dim r As Double = Math.Sqrt(dM * N / (2 * Math.PI))
        ' u = (Lg-1) / Type(2.0);
        Dim u As Double = (Lg - 1) / 2.0

        Dim gn As Double() = gauss(_Win, u, r)

        norm(gn)

        _DualWindow = computeDualWindow(gn, N, dM)

    End Sub

    Private Function dgt(signal As Double(), anaWin As Double(), N As Integer, dM As Integer, mode As String) As Double()()

        Dim N2 As Integer = N \ 2

        Dim Ls As Integer = signal.Length
        Dim Lw As Integer = anaWin.Length
        Dim M As Integer = 128 ' (Ls + Lw) \ dM

        Dim sn As Double() = signal ' wextend(signal, Lw, "both", mode)

        Dim coefs As Double()() = New Double(M - 1)() {} ' Matrix< complex<Type> > coefs(N,M);

        Dim segment As Double() = New Double(Lw - 1) {}
        Dim segDFT As Double() = New Double(Lw - 1) {}
        Dim tmp As Complex() = New Complex(N2 - 1) {}

        Dim W As Complex ' complex<Type> W = polar( Type(1), Type(-2*PI/N) );
        Dim rho As Double = 1
        Dim theta As Double = -2 * Math.PI / N

        W.Re = rho * Math.Cos(theta)
        W.Im = rho * Math.Sin(theta)

        For _m As Integer = 0 To M - 1

            coefs(_m) = New Double(N2 - 1) {}

            ' intercept signal by window function
            For i As Integer = 0 To Lw - 1
                segment(i) = sn(i + _m * dM) * anaWin(i)
            Next

            ' Fourier transform
            _FFT.Complex(segment, True)

            ' calculate the mth culumn coefficients
            For _n As Integer = 0 To N2 - 1
                tmp(_n) = Complex.Pow(W, _n * _m * dM) * segment(_n * Lw \ N)

                'coefs.setColumn( tmp, _m );
                coefs(_m)(_n) = Math.Sqrt(tmp(_n).GetModulusSquared) 'tmp(_n).Re '
            Next
        Next

        Return coefs

    End Function

    Private Function computeDualWindow(gn As Double(), N As Integer, dM As Integer) As Double()

        Dim L As Integer = gn.Length
        Dim NL As Integer = 2 * L - N

        Dim hn As Double() = New Double(L - 1) {}
        Dim gg As Double() = wextend(gn, L, "both", "sym")

        'Dim H As New Matrix(NL \ N, L \ dM)
        Dim H As Double()() = New Double(NL \ N - 1)() {}
        Dim u As Double() = New Double(NL \ N - 1) {}

        u(0) = 1 / N

        For k As Integer = 0 To dM - 1
            For q As Integer = 0 To NL \ N - 1 ' row
                H(q) = New Double(L \ dM - 1) {}
                For p As Integer = 0 To L \ dM - 1 ' col
                    Dim index As Integer = [mod](k + p * dM + q * N, NL)
                    H(q)(p) = gg(index)
                Next
            Next

            ' calculate the kth part value of h
            'Vector<Type> tmp = trMult( H, luSolver( multTr(H,H), u ) );
            Dim tmp As Double() = H.TransposeMultiply(luSolver(H.MultiplyTranspose(H), u))

            '      //  Vector<Type> tmp = tsvd( H, u ); ' <-----------------------------------
            For i As Integer = 0 To tmp.Length - 1
                hn(k + i * dM) = tmp(i)
            Next
        Next

        Return hn

    End Function

    Private Function [mod](m As Integer, n As Integer) As Integer
        If n <> 0 Then
            Dim r As Integer = m Mod n
            If r < 0 Then
                r += n
            End If

            Return r
        Else
            Return 0
        End If
    End Function

    '    Vector<Type> gauss( const Vector<Type> &x, const Type &u, const Type &r )
    '   {
    '        Vector<Type> tmp(x);

    '       tmp = (tmp-u)*(tmp-u) / ( -2*r*r );
    '       tmp = exp(tmp) / Type( (sqrt(2*PI)*r) );

    '       return tmp;
    '   }

    '*
    ' * Normal distribution with expectation "u" and variance "r".
    ' 
    Private Function gauss(x As Double(), u As Double, r As Double) As Double()

        Dim tmp As Double() = New Double(x.Length - 1) {}
        Dim tmpA As Double() = New Double(x.Length - 1) {}
        Dim tmpB As Double() = New Double(x.Length - 1) {}

        'tmp = (tmp - u) * (tmp - u) / (-2 * r * r)
        'tmp = exp(tmp) / (sqrt(2 * PI) * r)
        For i As Integer = 0 To x.Length - 1
            Dim a As Double = x(i) - u
            Dim b As Double = a
            tmp(i) = (a * b) / (-2 * r * r)
            tmp(i) = Math.Exp(tmp(i)) / (Math.Sqrt(2 * Math.PI) * r)
        Next

        Return tmp
    End Function

    Private Sub norm(v As Double())

        Dim sumSqr As Double

        For i As Integer = 0 To v.Length - 1
            sumSqr += v(i) * v(i)
        Next

        For i As Integer = 0 To v.Length - 1
            v(i) /= sumSqr
        Next

    End Sub


    Private Function luSolver(A As Double()(), b As Double()) As Double()

        Dim lu As New LUD
        lu.dec(A)
        Return lu.solve(b)

    End Function

    Private Function wextend(v As Double(), extLength As Integer, direction As String, mode As String) As Double()

        Dim lv As Integer = v.Length
        Dim tmp As Double()

        If direction = "right" Then

            'tmp.Resize(lv + extLength)
            tmp = New Double(lv + extLength) {}

            For i As Integer = 0 To lv - 1
                tmp(i) = v(i)
            Next

            If mode = "sym" Then
                For i As Integer = 0 To extLength - 1
                    tmp(lv + i) = v(lv - 1 - i)
                Next
            ElseIf mode = "ppd" Then
                For i As Integer = 0 To extLength - 1
                    tmp(lv + i) = v(i)
                Next
            Else
                For i As Integer = 0 To extLength - 1
                    tmp(lv + i) = 0
                Next
            End If

        ElseIf direction = "left" Then

            'tmp.Resize(lv + extLength)
            tmp = New Double(lv + extLength) {}

            If mode = "sym" Then
                For i As Integer = 0 To extLength - 1
                    tmp(i) = v(extLength - 1 - i)
                Next
            ElseIf mode = "ppd" Then
                For i As Integer = 0 To extLength - 1
                    tmp(i) = v(lv - extLength + i)
                Next
            Else
                For i As Integer = 0 To extLength - 1
                    tmp(i) = 0
                Next
            End If

            For i As Integer = 0 To lv - 1
                tmp(i + extLength) = v(i)
            Next

        Else ' Both

            'tmp.Resize(lv + 2 * extLength)
            tmp = New Double(lv + 2 * extLength) {}

            For i As Integer = 0 To lv - 1
                tmp(i + extLength) = v(i)
            Next

            If mode = "sym" Then
                For i As Integer = 0 To extLength - 1
                    tmp(i) = v(extLength - 1 - i)
                    tmp(lv + extLength + i) = v(lv - 1 - i)
                Next
            ElseIf mode = "ppd" Then
                For i As Integer = 0 To extLength - 1
                    tmp(i) = v(lv - extLength + i)
                    tmp(lv + extLength + i) = v(i)
                Next
            Else
                For i As Integer = 0 To extLength - 1
                    tmp(i) = 0
                    tmp(lv + extLength + i) = 0
                Next
            End If

        End If

        Return tmp

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
                creatDualeWindow()
            End If
        End Set
    End Property

    <DisplayName("Phase"), Description("FFT phase mode."), DefaultValue(FFT.Phase.Default)>
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
                creatDualeWindow()
            End If
        End Set
    End Property

    <Description("Discrete Gabor Transform")>
    Public Overrides ReadOnly Property Description As String
        Get
            Return "Discrete Gabor Transform"
        End Get
    End Property

End Class

''' <summary>
''' https://code.google.com/p/tspl/source/browse/trunk/include/lud-impl.h
''' </summary>
''' <remarks></remarks>
Public Class LUD

    Private LU As Double()()
    Private m As Integer
    Private n As Integer

    Private pivsign As Integer
    Private piv As Integer()

    Public Sub dec(A As Double()())

        m = A.Length
        n = A(0).Length

        'piv.Resize(m)
        piv = New Integer(m - 1) {}
        LU = A

        ' Use a "left-looking", dot-product, Crout/Doolittle algorithm.
        For i As Integer = 0 To m - 1
            piv(i) = i
        Next

        pivsign = 1
        Dim LUrowi As Double() ' = 0
        Dim LUcolj As Double() = New Double(m - 1) {}

        ' outer loop
        For j As Integer = 0 To n - 1

            ' Make a copy of the j-th column to localize references.
            For i As Integer = 0 To m - 1
                LUcolj(i) = LU(i)(j)
            Next

            ' Apply previous transformations.
            For i As Integer = 0 To m - 1
                LUrowi = LU(i)

                ' Most of the time is spent in the following dot product.
                Dim kmax As Integer = If((i < j), i, j)
                Dim s As Double = 0

                For k As Integer = 0 To kmax - 1
                    s += LUrowi(k) * LUcolj(k)
                Next

                ' LUrowi(j) = LUcolj(i) -= s
                LUrowi(j) -= s
                LUcolj(i) -= s

            Next

            ' Find pivot and exchange if necessary.
            Dim p As Integer = j
            For i As Integer = j + 1 To m - 1
                If Math.Abs(LUcolj(i)) > Math.Abs(LUcolj(p)) Then
                    p = i
                End If
            Next

            If p <> j Then
                Dim k As Integer = 0
                For k = 0 To n - 1
                    swap(LU(p)(k), LU(j)(k))
                Next

                swap(piv(p), piv(j))
                pivsign = -pivsign
            End If

            ' compute multipliers
            If (j < m) AndAlso (Math.Abs(LU(j)(j)) <> 0) Then
                For i As Integer = j + 1 To m - 1
                    LU(i)(j) /= LU(j)(j)
                Next
            End If
        Next

    End Sub

    Public Function solve(b As Double()) As Double()

        ' Vector<Type> x = permuteCopy( b, piv );
        Dim x As Double() = permuteCopy(b, piv)

        ' solve L*Y = B(piv)
        For k As Integer = 0 To n - 1
            For i As Integer = k + 1 To n - 1
                x(i) -= x(k) * LU(i)(k)
            Next
        Next

        ' solve U*x = y;
        For k As Integer = n - 1 To 0 Step -1
            x(k) /= LU(k)(k)
            For i As Integer = 0 To k - 1
                x(i) -= x(k) * LU(i)(k)
            Next
        Next

        Return x
    End Function

    Private Function permuteCopy(A As Double(), piv As Integer()) As Double()

        Dim pivLength As Integer = piv.Length

        'If pivLength <> A.Length Then
        '    Return Vector(Of Type)()
        'End If

        'Vector<Type> x(pivLength);
        Dim x As Double() = New Double(pivLength - 1) {}

        For i As Integer = 0 To pivLength - 1
            x(i) = A(piv(i))
        Next

        Return x

    End Function

    Private Sub swap(ByRef a As Double, ByRef b As Double)
        Dim c As Double = a
        a = b
        b = c
    End Sub

    Private Sub swap(ByRef a As Integer, ByRef b As Integer)
        Dim c As Integer = a
        a = b
        b = c
    End Sub

    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function

End Class
