Imports System.ComponentModel
''' <summary>
''' http://code.google.com/p/pjl/source/browse/trunk/DigitalAudioDemos/src/mediaframe/mpeg4/audio/AAC/MDCT.java
''' </summary>
''' <remarks></remarks>
Public Class MDCT
    Inherits TimeSeriesFrameTransformer

    Private _FFT As New FFT
    Private _1PI As Double = 3.14159265358979

    Private _Width As Integer = 512
    Private _Phase As FFT.Phase = FFT.Phase.Data
    Private _Factor As Double = 2.0
    Private _F As Integer = 4

    Public Sub New()

        MyBase.FrameWidth = FrameWidth._512
        MyBase.FrameStep = FrameStep._64

        _FFT.PhaseMode = _Phase

    End Sub

    Protected Overloads Overrides Function OnTransformFrame(frame As Double()) As Double()
        Return Forward(frame, _Width \ _F)
    End Function

    Protected Overrides Sub OnFrameWidthChanged(width As Integer)
        _Width = width
    End Sub

    Private Function Forward(data As Double(), b As Integer) As Double()

        Dim dtc As Double() = New Double((_Width \ 8) - 1) {}

        Dim real As Double
        Dim imag As Double
        Dim c As Double
        Dim s As Double
        Dim cold As Double
        Dim cfreq As Double
        Dim sfreq As Double
        Dim freq As Double
        'Dim fac As Double = 2.5 ' Choosing to allocate 2/N factor to Inverse Xform! 2 from MDCT inverse to forward 
        Dim n As Integer
        Dim a As Integer = _Width - b

        Dim FFTarray As Double() = New Double(_Width \ 2 - 1) {}

        ' TODO: REMOVE FROM HERE. THIS CAN BE PRE/RE-COMPUETED...
        ' prepare for recurrence relation in pre-twiddle 
        freq = (2 * _1PI / _Width)
        cfreq = Math.Cos(freq)
        sfreq = Math.Sin(freq)
        c = Math.Cos(freq * 0.125)
        s = Math.Sin(freq * 0.125)

        ' calculate real and imaginary parts of g(n) or G(p) 
        For i As Integer = 0 To (_Width \ 4) - 1

            ' Forward Transform 
            n = _Width \ 2 - 1 - 2 * i
            If i < (b / 4) Then
                ' use second form of e(n) for n = N / 2 - 1 - 2i 
                real = data(a \ 2 + n) + data(_Width + a \ 2 - 1 - n)
            Else
                ' use first form of e(n) for n = N / 2 - 1 - 2i 
                real = data(a \ 2 + n) - data(a \ 2 - 1 - n)
            End If

            n = 2 * i

            If i < (a / 4) Then
                ' use first form of e(n) for n = N / 2 - 1 - 2i 
                imag = data(a \ 2 + n) - data(a \ 2 - 1 - n)
            Else
                ' use second form of e(n) for n = N / 2 - 1 - 2i i
                imag = data(a \ 2 + n) + data(_Width + a \ 2 - 1 - n)
            End If

            ' calculate pre-twiddled FFT input 
            FFTarray(2 * i) = real * c + 1 * imag * s
            FFTarray(2 * i + 1) = imag * c - 1 * real * s

            ' use recurrence to prepare cosine and sine for next value of i 
            cold = c
            c = c * cfreq - s * sfreq
            s = s * cfreq + cold * sfreq
        Next

        ' Perform in-place complex FFT (or IFFT) of length N/4 
        ' Note: FFT has physics (opposite) sign convention and doesn't do 1/N factor 
        _FFT.Complex(FFTarray, True)

        ' prepare for recurrence relations in post-twiddle 
        c = Math.Cos(freq * 0.125)
        s = Math.Sin(freq * 0.125)

        ' post-twiddle FFT output and then get output data 
        For i As Integer = 0 To (_Width \ 4) - 1

            ' get post-twiddled FFT output  
            ' Note: _Factor allocates 4/N factor from IFFT to forward and inverse 
            real = _Factor * (FFTarray(2 * i) * c + 1 * FFTarray(2 * i + 1) * s)
            imag = _Factor * (FFTarray(2 * i + 1) * c - 1 * FFTarray(2 * i) * s)

            ' first half even 
            data(2 * i) = -real ' 0, 2, 4, 6, 8...56, 58, 60, 62, 64 -->
            ' first half odd 
            data(_Width \ 2 - 1 - 2 * i) = imag ' 63, 61, 59, 57, 55...7, 5, 3, 1 <--
            ' second half even 
            data(_Width \ 2 + 2 * i) = -imag ' 66, 68, 70, 72, 74...122, 124, 126, 128 -->
            ' second half odd 
            data(_Width - 1 - 2 * i) = real ' 127, 125, 123, 121...71, 69, 67, 65 <--
            ' use recurrence to prepare cosine and sine for next value of i 
            cold = c
            c = c * cfreq - s * sfreq
            s = s * cfreq + cold * sfreq
        Next

        ' Only taking the first half... second half is reversed, signed copy.
        For j As Integer = 0 To (_Width \ 8) - 1
            Dim re As Double = data(2 * j) + data(2 * j)
            Dim im As Double = data(2 * j + 1) + data(2 * j + 1)
            dtc(j) = Math.Sqrt(re * re + im * im) * 0.5
        Next

        Return dtc

    End Function

    <DisplayName("Width devisor..."), DefaultValue(4)>
    Public Property F As Integer
        Get
            Return _F
        End Get
        Set(value As Integer)
            _F = value
        End Set
    End Property

    <Description("FFT phase mode."), DefaultValue(FFT.Phase.Data)>
    Public Property FFTPhase As FFT.Phase
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

    <DisplayName("2/N Factor"), DefaultValue(2.0)>
    Public Property Factor As Double
        Get
            Return _Factor
        End Get
        Set(value As Double)
            If value <> _Factor Then
                _Factor = value
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Modified Discrete Cosine Transform (DCT-IV)"
        End Get
    End Property

End Class
