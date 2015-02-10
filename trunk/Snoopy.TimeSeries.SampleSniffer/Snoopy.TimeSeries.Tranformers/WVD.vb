Imports System.ComponentModel
''' <summary>
''' https://code.google.com/p/tspl/source/browse/trunk/include/wvd-impl.h
''' </summary>
''' <remarks></remarks>
Public Class WVD
    Inherits TimeSeriesFrameTransformer

    Private _FFT As New FFT
    Private _Phase As FFT.Phase = FFT.Phase.Default

    Public Sub New()

        MyBase.FrameStep = FrameStep._64
        MyBase.FrameWidth = FrameWidth._512

    End Sub

    Protected Overrides Function OnTransformFrame(frame() As Double) As Double()

        '    int N = sn.size(),
        Dim N As Integer = frame.Length
        '    dN = 2*N;
        Dim N2 As Integer = 2 * N

        Dim coefs As Double() = New Double((N \ 2) - 1) {}

        'Vector<Type> yn( dN );
        'Dim yn As Double() = New Double(N2 - 1) {}

        'Matrix<Type> coefs( N, N );
        'Dim coefs As Double() = New Double(N - 1) {}

        'Vector<Type> fn( 3*dN );
        'Dim fn As Double() = New Double((3 * N2) - 1) {}

        'Vector<Type> xn = fftInterp( sn, 2 );
        Dim xn As Double() = fftInterp(frame, 2)

        ' ------------------------------------

        _FFT.Real(xn, True)

        For i As Integer = 0 To (N \ 2) - 1
            ' ??? Mag spec ???
            Dim re As Double = xn(2 * i) + xn(N2 - (2 * i) - 2)
            Dim im As Double = xn(2 * i + 1) + xn(N2 - (2 * i) - 1)
            coefs(i) = Math.Sqrt(re * re + im * im)
        Next

        ' Why are we not down sampling here?
        ' coefs.setColumn( dyadDown(real(fft(yn)),0), n-1 );

        Return coefs

    End Function

    Private Function fftInterp(signal As Double(), factor As Integer) As Double()

        '    int N = sn.size(),
        Dim N As Integer = signal.Length
        '    halfN = N/2,
        Dim halfN As Integer = N \ 2
        '    offset = (factor-1)*N;
        Dim offset As Integer = (factor - 1) * N

        '   Vector< complex<Type> > Sk = fft(sn);
        Dim Sk As Complex() = _FFT.RealToComplex(signal, True)

        '   Vector< complex<Type> > Xk(factor*N);
        Dim Xk As Complex() = New Complex((factor * N) - 1) {}

        '   for( int i=0; i<=halfN; ++i )
        '        Xk[i] = Type(factor)*Sk[i];

        For i As Integer = 0 To halfN
            Xk(i) = factor * Sk(i)
        Next

        '   for( int i=halfN+1; i<N; ++i )
        '        Xk[offset+i] = Type(factor)*Sk[i];

        For i As Integer = halfN + 1 To N - 1
            Xk(offset + i) = factor * Sk(i)
        Next

        '   return ifftc2r(Xk);

        Return _FFT.ComplexToReal(Xk, False)

    End Function

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Wigner-Ville Distribution"
        End Get
    End Property

End Class
