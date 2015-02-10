Imports System.ComponentModel
''' <summary>
''' https://sites.google.com/site/piotrwendykier/software/jtransforms
''' </summary>
''' <remarks></remarks>
<DefaultProperty("FrameWidth")>
Public Class DHT
    Inherits TimeSeriesFrameTransformer

    Private ReadOnly _FFT As New FFT
    Private _FFTPhaseMode As FFT.Phase = FFT.Phase.Default

    Public Sub New()
        MyBase.FrameWidth = FrameWidth._256
        MyBase.FrameStep = FrameStep._64
        _FFT.PhaseMode = _FFTPhaseMode
    End Sub

    Protected Overrides Function OnTransformFrame(frame As Double()) As Double()

        Dim n As Integer = frame.Length
        Dim a As Double() = frame
        Dim b As Double() = New Double(n - 1) {}
        Dim c As Double() = New Double(n \ 2 - 1) {}

        _FFT.Real(a, True)

        Array.Copy(a, b, n)

        Dim nd2 As Integer = n \ 2
        Dim idx1 As Integer
        Dim idx2 As Integer

        For i As Integer = 1 To nd2 - 1
            idx1 = 2 * i
            idx2 = idx1 + 1
            a(i) = b(idx1) - b(idx2)
            a(n - i) = b(idx1) + b(idx2)
        Next

        If (n Mod 2) = 0 Then
            a(nd2) = b(1)
        Else
            a(nd2) = b(n - 1) - b(1)
            a(nd2 + 1) = b(n - 1) + b(1)
        End If

        ' take first half...
        For i As Integer = 0 To c.Length - 1
            c(i) = a(i)
        Next

        Return c

    End Function

    <Description("FFT phase mode."), DefaultValue(FFT.Phase.Default)>
    Public Property FFTPhase As FFT.Phase
        Get
            Return _FFTPhaseMode
        End Get
        Set(value As FFT.Phase)
            If value <> _FFTPhaseMode Then
                _FFTPhaseMode = value
                _FFT.PhaseMode = _FFTPhaseMode
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Discrete Hartley Transform (DHT) of real, double precision data"
        End Get
    End Property

End Class
