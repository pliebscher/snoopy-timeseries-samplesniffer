Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Description("")>
Public Class FFTPAA
    Inherits TimeSeriesFrameTransformer

    Private ReadOnly _FFT As New FFT

    Private _FrameWidth As Integer = 128
    Private _PAASize As PAASize = PAASize._64
    Private _Phase As FFT.Phase = FFT.Phase.Data

    Public Sub New()

        MyBase.FrameStep = FrameStep._32
        MyBase.FrameWidth = FrameWidth._128

        _FFT.PhaseMode = _Phase

    End Sub

    Protected Overloads Overrides Function OnTransformFrame(frame As Double()) As Double()

        Dim complexSignal As Double() = New Double(2 * _FrameWidth - 1) {}

        For j As Integer = 0 To _FrameWidth - 1
            complexSignal(2 * j) = frame(j)
            complexSignal(2 * j + 1) = 0
        Next

        _FFT.Complex(complexSignal, True)

        Dim fFrame As Double() = New Double(_FrameWidth \ 2 - 1) {}
        For j As Integer = 0 To _FrameWidth \ 2 - 1
            Dim re As Double = complexSignal(2 * j)
            Dim im As Double = complexSignal(2 * j + 1)
            fFrame(j) = Math.Sqrt(re * re + im * im)
        Next

        Return Transform(fFrame, _PAASize)

    End Function

    Private Overloads Shared Function Transform(samples As Double(), paaWindowWidth As Integer) As Double()

        If paaWindowWidth >= samples.Length Then Return samples

        Dim t As Double()() = New Double(paaWindowWidth - 1)() {}
        For i As Integer = 0 To paaWindowWidth - 1
            t(i) = New Double(samples.Length - 1) {}
            For j As Integer = 0 To samples.Length - 1
                t(i)(j) = samples(j)
            Next
        Next

        Dim exp As Double()() = Matrix.Reshape(t, 1, samples.Length * paaWindowWidth)
        Dim res As Double()() = Matrix.Reshape(exp, samples.Length, paaWindowWidth)

        Return Matrix.GetColumnMeans(res)

    End Function

    Protected Overrides Sub OnFrameWidthChanged(width As Integer)
        _FrameWidth = width
    End Sub

    <Description("PAA width."), DefaultValue(PAASize._64), TypeConverter(GetType(EnumIntValueConverter))>
    Public Property Width As PAASize
        Get
            Return _PAASize
        End Get
        Set(value As PAASize)
            If value > _FrameWidth Then
                Throw New ArgumentException("PAAWidth should be less than FrameWidth")
                Exit Property
            End If
            _PAASize = value
        End Set
    End Property

    <DisplayName("Phase"), Description("FFT phase mode."), DefaultValue(FFT.Phase.Data)>
    Public Property FFTPhaseMode As FFT.Phase
        Get
            Return _Phase
        End Get
        Set(value As FFT.Phase)
            If value <> _Phase Then
                _Phase = value
                _FFT.PhaseMode = _Phase
            End If
            _Phase = value
        End Set
    End Property

    Public Enum PAASize
        _16 = 16
        _32 = 32
        _64 = 64
        _128 = 128
        _256 = 256
        _512 = 512
    End Enum

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Fast Fourier Transform + Piecewise Aggregate Approximation"
        End Get
    End Property

End Class
