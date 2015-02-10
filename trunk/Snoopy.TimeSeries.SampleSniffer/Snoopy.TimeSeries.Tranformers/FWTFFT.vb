Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class FWTFFT
    Inherits TimeSeriesFrameTransformer

    Private _FrameWidth As Integer = 512
    Private _Phase As FFT.Phase = FFT.Phase.Default
    Private _FFT As New FFT

    Private _Type As TransformType = TransformType.Fast
    Private _WaveletType As WaveletType = WaveletType.Daub02
    Private _Wavelet As Wavelet = New Daub02()

    Public Sub New()

        MyBase.FrameStep = FrameStep._64
        MyBase.FrameWidth = FrameWidth._512

        _FFT.PhaseMode = _Phase

    End Sub

    Protected Overloads Overrides Function OnTransformFrame(frame As Double()) As Double()

        If _Type = TransformType.Fast Then
            frame = ForwardFast(frame)
        Else
            frame = ForwardPacket(frame)
        End If

        _FFT.Real(frame, True)

        Dim halfLen As Integer = (_FrameWidth \ 2)
        Dim freqs As Double() = New Double(halfLen - 1) {}
        Dim freqs2 As Double() = New Double((halfLen \ 2) - 1) {}

        ' First half is the energy (approximation) - low pass
        ' Second half is the details - high pass
        For i As Integer = 0 To halfLen - 1
            freqs(i) = frame(i) + frame((_FrameWidth - i) - 1)
        Next

        _FFT.Real(freqs, False)

        ' Keep first half...
        For i As Integer = 0 To halfLen \ 2 - 1
            freqs2(i) = freqs(i)
        Next

        Return freqs2

    End Function

    ''' <summary>
    ''' http://code.google.com/p/jwave/source/browse/trunk/src/main/java/math/transform/jwave/handlers/FastWaveletTransform.java
    ''' </summary>
    ''' <param name="arrTime"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ForwardFast(arrTime As Double()) As Double()

        Dim arrHilb As Double() = New Double(arrTime.Length - 1) {}
        For i As Integer = 0 To arrTime.Length - 1
            arrHilb(i) = arrTime(i)
        Next

        'Dim level As Integer = 0
        Dim h As Integer = arrTime.Length
        Dim minWaveLength As Integer = _Wavelet.Coefficients.Length
        If h >= minWaveLength Then

            While h >= minWaveLength

                Dim iBuf As Double() = New Double(h - 1) {}

                For i As Integer = 0 To h - 1
                    iBuf(i) = arrHilb(i)
                Next

                Dim oBuf As Double() = _Wavelet.Forward(iBuf)

                For i As Integer = 0 To h - 1
                    arrHilb(i) = oBuf(i)
                Next

                h = h >> 1
                'level += 1

            End While
        End If
        ' if
        Return arrHilb
    End Function

    ''' <summary>
    ''' http://code.google.com/p/jwave/source/browse/trunk/src/main/java/math/transform/jwave/handlers/WaveletPacketTransform.java
    ''' </summary>
    ''' <param name="arrTime"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ForwardPacket(arrTime As Double()) As Double()

        Dim arrHilb As Double() = New Double(arrTime.Length - 1) {}
        For i As Integer = 0 To arrTime.Length - 1
            arrHilb(i) = arrTime(i)
        Next

        'Dim level As Integer = 0
        Dim k As Integer = arrTime.Length
        Dim h As Integer = arrTime.Length
        Dim minWaveLength As Integer = _Wavelet.Coefficients.Length
        If h >= minWaveLength Then

            While h >= minWaveLength

                Dim g As Integer = k \ h
                ' 1 -> 2 -> 4 -> 8 -> ...
                For p As Integer = 0 To g - 1

                    Dim iBuf As Double() = New Double(h - 1) {}

                    For i As Integer = 0 To h - 1
                        iBuf(i) = arrHilb(i + (p * h))
                    Next

                    Dim oBuf As Double() = _Wavelet.Forward(iBuf)

                    For i As Integer = 0 To h - 1
                        arrHilb(i + (p * h)) = oBuf(i)

                    Next
                Next
                ' packets
                h = h >> 1


                'level += 1
                ' levels
            End While
        End If
        ' if
        Return arrHilb
    End Function

    ''' <summary>
    ''' http://stevehanov.ca/wavelet/
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="scale"></param>
    ''' <param name="f0"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FTWavelet(x As Double, scale As Double, f0 As Single) As Double
        'If x < 0.9 / scale OrElse x > 1.1 / scale Then
        '    Return 0.0
        'End If

        Dim pi As Double = 3.14159265358979
        Dim two_pi_f0 As Double = 2.0 * pi * f0
        Dim multiplier As Double = 1.88279252755343

        scale *= f0

        ' 1.88279*exp(-0.5*(2*pi*x*10-2*pi*10)^2)
        Dim basic As Double = (multiplier * Math.Exp(-0.5 * (2 * pi * x * scale - two_pi_f0) * (2 * pi * x * scale - two_pi_f0)))

        ' pi^0.25*sqrt(2.0)*exp(-0.5*(2*pi*x*scale-2*pi*0.849)^2)
        Return Math.Sqrt(scale) * basic

    End Function

    Protected Overrides Sub OnFrameWidthChanged(width As Integer)
        _FrameWidth = width
    End Sub

    <Description("The tranform function type."), DefaultValue(TransformType.Fast)>
    Public Property [Type]() As TransformType
        Get
            Return _Type
        End Get
        Set(value As TransformType)
            If value <> _Type Then
                _Type = value
            End If
        End Set
    End Property

    <Description("The wavelet type."), DefaultValue(WaveletType.Daub02)>
    Public Property Wavelet As WaveletType
        Get
            Return _WaveletType
        End Get
        Set(value As WaveletType)
            If value <> _WaveletType Then
                _WaveletType = value
                Select Case _WaveletType
                    Case WaveletType.Haar02
                        _Wavelet = New Haar02
                    Case WaveletType.Daub02
                        _Wavelet = New Daub02
                    Case WaveletType.Daub03
                        _Wavelet = New Daub03
                    Case WaveletType.Daub04
                        _Wavelet = New Daub04
                    Case WaveletType.Daub08
                        _Wavelet = New Daub08
                    Case WaveletType.Daub16
                        _Wavelet = New Daub16
                    Case WaveletType.Coif02
                        _Wavelet = New Coif02
                    Case WaveletType.Coif06
                        _Wavelet = New Coif06
                    Case WaveletType.Lege02
                        _Wavelet = New Lege02
                    Case WaveletType.Lege04
                        _Wavelet = New Lege04
                    Case WaveletType.Lege06
                        _Wavelet = New Lege06
                    Case WaveletType.DMeyer
                        _Wavelet = New DMeyer
                End Select
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

    Public Enum WaveletType
        Daub02
        Daub03
        Daub04
        Daub08
        Daub16
        Coif02
        Coif06
        Haar02
        Lege02
        Lege04
        Lege06
        DMeyer
    End Enum

    Public Enum TransformType
        Fast
        Packet
    End Enum

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Fast Wavelt Transform + Fast Fourier Transform"
        End Get
    End Property





End Class
