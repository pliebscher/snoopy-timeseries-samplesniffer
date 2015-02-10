Imports System.ComponentModel
''' <summary>
''' http://kurage.nimh.nih.gov/library/Meg/st.c
''' http://sourceforge.net/p/fst-uofc/code/ci/master/tree/
''' </summary>
''' <remarks></remarks>
Public Class ST
    Inherits TimeSeriesTransformer

    Private _FreqLo As Integer = 33
    Private _FreqHi As Integer = 133

    Private _FrameWidth As Integer = 2048

    Private _FFT As New FFT
    Private _Phase As FFT.Phase = FFT.Phase.Default

    Private _Pars As Integer()

    Public Sub New()

        _Pars = gft_1dPartitions(_FrameWidth)

    End Sub

    Protected Overrides Function OnTransform(series As ITimeSeries(Of Double())) As Double()()


        Dim frame As Double() = Me.OnTransformFrame(series.Samples, series.SampleRate)

        Dim frameWidth As Integer = 64
        Dim frameHeight As Integer = CInt(frame.Length \ frameWidth)

        Dim f As Double()() = New Double(frameHeight - 1)() {}

        For y As Integer = 0 To frameHeight - 1
            f(y) = New Double(frameWidth - 1) {}
            For x As Integer = 0 To frameWidth - 1
                f(y)(x) = frame(x * y + x)
            Next
        Next

        Return f
        'Dim complexSignal As Double() = New Double(2 * _FrameWidth - 1) {}

        'For j As Integer = 0 To _FrameWidth - 1
        '    complexSignal(2 * j) = series.Samples(j)
        '    complexSignal(2 * j + 1) = 0
        'Next

        'Dim win As Double() = New Double(2 * _FrameWidth - 1) {}

        'gaussian(win, _FrameWidth, 800)

        'gft_1dComplex64(complexSignal, _FrameWidth, win, _Pars)

    End Function

    ' Stockwell transform of the real array data. The len argument is the
    'number of time points, and it need not be a power of two. The lo and hi
    'arguments specify the range of frequencies to return, in Hz. If they are
    'both zero, they default to lo = 0 and hi = len / 2. The result is
    'returned in the complex array result, which must be preallocated, with
    'n rows and len columns, where n is hi - lo + 1. For the default values of
    'lo and hi, n is len / 2 + 1. 
    Protected Function OnTransformFrame(frame() As Double, sampleRate As Integer) As Double()

        Dim len As Integer = frame.Count * 2
        Dim i As Integer, k As Integer, n As Integer, l2 As Integer

        ' planlen = len;
        ' h = fftw_malloc(sizeof(fftw_complex) * len);
        ' H = fftw_malloc(sizeof(fftw_complex) * len);
        ' G = fftw_malloc(sizeof(fftw_complex) * len);
        ' g = (double *)malloc(sizeof(double) * len);

        ' p1 = fftw_plan_dft_1d(len, h, H, FFTW_FORWARD, FFTW_MEASURE);
        ' p2 = fftw_plan_dft_1d(len, G, h, FFTW_BACKWARD, FFTW_MEASURE);

        Dim real As Double() = New Double((len \ 2) - 1) {}
        Dim g1 As Double() = New Double(len - 1) {}
        Dim G2 As Double() = New Double(len - 1) {}
        Dim H As Double() = New Double(len - 1) {}

        Dim s As Double

        ' Make complex...
        For j As Integer = 0 To (len \ 2) - 1
            H(2 * j) = frame(j)
            H(2 * j + 1) = 0
            s += frame(j)
        Next

        ' Compute the mean...
        s /= len

        _FFT.Complex(H, True)

        ' Hilbert transform. The upper half-circle gets multiplied by
        ' two, and the lower half-circle gets set to zero.  The real axis
        ' is left alone. 

        l2 = (len + 1) \ 2
        For i = 1 To l2 - 1
            'H(i)(0) *= 2.0 ' Re
            'H(i)(1) *= 2.0 ' Im
            H(i) *= 2.0
        Next

        l2 = len \ 2 + 1
        For i = l2 To len - 1
            'H(i)(0) = 0.0 ' Re
            'H(i)(1) = 0.0 ' Im
            H(i) = 0
        Next

        n = FreqToIndex(_FreqLo, sampleRate, len \ 2)
        'If n = 0 Then
        '    For i = 0 To len - 1
        '        ' *p++ = s;
        '        ' *p++ = 0.;

        '    Next
        '    n += 1
        'End If

        ' Subsequent rows contain the inverse FFT of the spectrum
        ' multiplied with the FFT of scaled gaussians. 
        Dim fhi As Integer = FreqToIndex(_FreqHi, 22050, len \ 2)
        While n <= fhi '_FreqHi

            ' Scale the FFT of the gaussian. Negative frequencies wrap around. 
            'g(0) = gauss(n, 0)
            g1(0) = Gauss(n, 0)
            l2 = len \ 2 + 1
            For i = 1 To l2 - 1
                ' g[i] = g[len - i] = gauss(n, i);
                'g(i) = InlineAssignHelper(g(len - i), gauss(n, i))
                'g1(len - i) = Gauss(n, i)
                'g1(i) = Gauss(n, i)
                g1(i) = Gauss(n, i)
                g1(len - i) = Gauss(n, i)
            Next

            Try
                For i = 0 To len - 1
                    s = g1(i)
                    k = n + i
                    ' If k >= len Then
                    If k >= len Then
                        k -= len
                    End If
                    'G(i)(0) = H(k)(0) * s
                    'G(i)(1) = H(k)(1) * s
                    G2(i) = (H(k) * s) '/ len
                Next
            Catch ex As Exception
                Throw
            End Try


            'fftw_execute(p2) ' inverse ' G -> h 
            _FFT.Complex(G2, False)

            'For i = 0 To G2.Length - 1
            '    ' *p++ = h[i][0] / len;
            '    ' *p++ = h[i][1] / len;
            '    G2(i) /= len
            'Next

            ' Go to the next row. 
            n += 1

        End While

        For i = 0 To real.Length - 1
            Dim re As Double = G2(2 * i)
            Dim im As Double = G2(2 * i + 1)
            real(i) = Math.Sqrt(re * re + im * im)
        Next

        Return real 'real

    End Function

    Private Shared Function FreqToIndex(ByVal freq As Double, ByVal sampleRate As Integer, ByVal spectrumLength As Integer) As Integer
        Dim fraction As Double = (freq + 0.5) / (sampleRate / 2)
        'N sampled points in time correspond to [0, N/2] frequency range 
        Dim i As Integer = CInt(Math.Round((spectrumLength / 2 + 1) * fraction))
        'DFT N points defines [N/2 + 1] frequency points
        Return i
    End Function

    ' This is the Fourier Transform of a Gaussian. 
    Private Shared Function Gauss(n As Integer, m As Integer) As Double
        'return exp(-2. * M_PI * M_PI * m * m / (n * n));
        Return Math.Exp(-2.0 * Math.PI * Math.PI * m * m / (n * n))
    End Function

    ' Convert frequencies in Hz into rows of the ST, given sampling rate and
    ' length. 
    Private Function st_freq(f As Double, len As Integer, srate As Double) As Integer
        Return CInt(Math.Floor(srate + 0.5 / f * len))
    End Function

    ' ----------------------------------------------------------------------------------------------------------------------

    Private Sub cmul(x As Double(), y As Double())
        Dim ac As Double, bd As Double, abcd As Double
        ac = x(0) * y(0)
        bd = x(1) * y(1)
        abcd = (x(0) + x(1)) * (y(0) + y(1))
        x(0) = ac - bd
        x(1) = abcd - ac - bd
    End Sub

    Private Sub cmul(xRe As Double, xIm As Double, yRe As Double, yIm As Double)
        Dim ac As Double, bd As Double, abcd As Double
        ac = xRe * yRe
        bd = xIm * yIm
        abcd = (xRe + xIm) * (yRe + yIm)
        xRe = ac - bd
        xIm = abcd - ac - bd
    End Sub

    Private Sub gaussian(win As Double(), N As Integer, freq As Integer)
        Dim i As Integer
        Dim x As Double
        Dim sum As Double
        For i = 0 To N * 2 - 1 Step 2
            x = i / (N * 2 + 0.0)
            win(i) = Math.Abs(freq) / Math.Sqrt(2 * Math.PI) * Math.Exp(-Math.Pow((x - 0.5), 2) * Math.Pow(freq, 2) / 2.0)
            win(i + 1) = 0.0
            sum += win(i)
        Next
        ' Make sure the window area is 1.0
        For i = 0 To N * 2 - 1 Step 2
            win(i) /= sum
        Next
        shift(win, N, -N \ 2)
        'FFT(N, win, 1)
        _FFT.Complex(win, True)
    End Sub

    Private Sub shift(sig As Double(), N As Integer, amount As Integer)
        Dim temp As Double() = New Double(2 * N - 1) {}
        Dim i As Integer, j As Integer
        'temp = (double *) malloc(N*2*sizeof(double));
        'memcpy(temp, sig, N*2*sizeof(double));
        Array.Copy(sig, temp, 2 * N - 1)
        For i = 0 To N - 1
            j = i - amount
            If j < 0 Then
                j = N - j
            End If
            j = j Mod N
            sig(i * 2) = temp(j * 2) ' re
            sig(i * 2 + 1) = temp(j * 2 + 1) ' im
        Next
        'free(temp);	
    End Sub

    Private Function gft_1dPartitions(N As Integer) As Integer()
        Dim sf As Integer = 1
        Dim ef As Integer = 2
        Dim cf As Integer = 1
        Dim width As Integer = 1
        Dim pcount As Integer = 0
        Dim pOff As Integer
        Dim partitions As Integer()
        Dim sp As Integer, ep As Integer, sn As Integer, en As Integer
        'partitions = (int *)malloc(sizeof(int)*round(log2(N))*2+1);
        partitions = New Integer(CInt(Math.Round(Math.Log(N, 2))) * 2) {}
        pOff = CInt(Math.Round(Math.Log(N, 2)) * 2 - 1)
        While sf < N \ 2
            sp = cf - width \ 2 - 1
            ep = cf + width \ 2 - 1
            sn = N - cf - width \ 2 + 1
            en = N - cf + width \ 2 + 1
            If ep > N Then
                ep = N
            End If
            If sn < 0 Then
                sn = 0
            End If
            If width \ 2 = 0 Then
                ep += 1
            End If
            sn -= 1
            partitions(pcount) = ep
            partitions(pOff - pcount) = en
            pcount += 1
            sf = sf + width
            If sf > 2 Then
                width *= 2
            End If
            ef = sf + width
            cf = sf + width \ 2
        End While
        partitions(pOff + 1) = -1
        Return partitions
    End Function

    Private Sub gft_1dComplex64(signal As Double(), N As Integer, win As Double(), pars As Integer())

        Dim fstart As Integer, fend As Integer, fcount As Integer
        Dim fband As Double()
        Dim i As Integer

        ' Do the initial FFT of the signal
        'FFT(N, signal, stride)
        _FFT.Complex(signal, True)

        ' Apply the windows
        For i = 0 To N - 1 ' 2 * N - 1 'Step 2
            'cmul(signal + i * stride, win + i)

            Dim xRe As Double = signal(2 * i)
            Dim xIm As Double = signal(2 * i + 1)
            Dim yRe As Double = win(2 * i)
            Dim yIm As Double = win(2 * i + 1)

            Dim ac As Double = xRe * yRe
            Dim bd As Double = xIm * yIm
            Dim abcd As Double = (xRe + xIm) * (yRe + yIm)

            signal(2 * i) = ac - bd
            signal(2 * i + 1) = abcd - ac - bd

        Next

        ' For each of the GFT frequency bands
        fcount = 0
        fstart = 0
        'Dim arr As New List(Of Double)
        While pars(fcount) >= 0
            fend = pars(fcount)
            ' frequency band that we're working with
            'fband = signal + fstart * 2 * stride
            fband = New Double((2 * fend) - fstart - 1) {}

            Dim k As Integer = 0
            For j As Integer = fstart To 2 * fend - 1
                fband(k) = signal(j)
                k += 1
                'arr.Add(signal(j))
            Next

            ' inverse FFT to transform to S-space
            'ifft(fend - fstart, fband, stride)

            fstart = pars(fcount)
            fcount += 1
        End While

    End Sub

    ' ----------------------------------------------------------------------------------------------------------------------


    Public Property FreqLo As Integer
        Get
            Return _FreqLo
        End Get
        Set(value As Integer)
            If value <> _FreqLo Then
                _FreqLo = value
            End If
        End Set
    End Property

    Public Property FreqHi As Integer
        Get
            Return _FreqHi
        End Get
        Set(value As Integer)
            If value <> _FreqHi Then
                _FreqHi = value
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Stockwell Transform"
        End Get
    End Property

End Class
