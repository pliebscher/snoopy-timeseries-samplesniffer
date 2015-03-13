Imports System.ComponentModel
''' <summary>
''' MARF
''' </summary>
''' <remarks></remarks>
Public Class LPC
    Inherits TimeSeriesFrameTransformer

    Private _Poles As Integer = 32

    Private _FFTPhase As FFT.Phase = FFT.Phase.Signal
    Private _FFT As New FFT

    Public Sub New()

    End Sub

    Protected Overrides Function OnTransformFrame(frame() As Double) As Double()

        Dim lpc As Double() = New Double(frame.Length - 1) {}

        '_FFT.Real(frame, True)
        lpc = Encode(frame, _Poles)
        '_FFT.Real(lpc, False)

        Return lpc

    End Function

    Public Shared Function Encode(frame As Double(), poles As Integer) As Double()

        poles += 1

        Dim coeffs As Double() = New Double(poles - 2) {}
        Dim E As Double() = New Double(poles - 1) {}
        Dim k As Double() = New Double(poles - 1) {}
        Dim A As Double()() = New Double(poles - 1)() {}

        E(0) = AutoCorrelate(frame, 0)

        For b As Integer = 0 To A.Length - 1
            A(b) = New Double(poles - 1) {}
        Next

        For m As Integer = 1 To poles - 1

            Dim cor As Double = AutoCorrelate(frame, m)

            For n As Integer = 1 To m - 1
                cor -= A(m - 1)(n) * AutoCorrelate(frame, m - n)
            Next

            k(m) = cor / E(m - 1)

            For n As Integer = 0 To m - 1
                A(m)(n) = A(m - 1)(n) - k(m) * A(m - 1)(m - n)
            Next

            A(m)(m) = k(m)
            E(m) = (1 - (k(m) * k(m))) * E(m - 1)

        Next

        For n As Integer = 1 To poles - 1
            If Double.IsNaN(A(poles - 1)(n)) Then
                coeffs(n - 1) = 0
            Else
                coeffs(n - 1) = A(poles - 1)(n)
            End If
        Next

        'coeffs(0) /= coeffs.Length

        Return coeffs
    End Function

    ''' <summary>
    ''' Autocorrelation LPC coeff generation algorithm invented by
    ''' N. Levinson in 1947, modified by J. Durbin in 1959.
    ''' Port from JOrbis - lpc.java
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="n"></param>
    ''' <param name="m"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function FromTimeDomain(data As Double(), n As Integer, m As Integer) As Double()
        Dim lpc As Double() = New Double(m - 1) {}
        Dim aut As Double() = New Double(m) {}
        Dim [error] As Double
        Dim i As Integer, j As Integer

        ' autocorrelation, p+1 lag coefficients
        j = m + 1
        While System.Math.Max(System.Threading.Interlocked.Decrement(j), j + 1) <> 0
            Dim d As Double = 0
            For i = j To n - 1
                d += data(i) * data(i - j)
            Next
            aut(j) = d
        End While

        ' Generate lpc coefficients from autocorr values
        [error] = aut(0)

        For i = 0 To m - 1
            Dim r As Double = -aut(i + 1)

            If [error] = 0 Then
                For k As Integer = 0 To m - 1
                    lpc(k) = 0.0F
                Next
                Return lpc '0

            End If

            ' Sum up this iteration's reflection coefficient; note that in
            ' Vorbis we don't save it.  If anyone wants to recycle this code
            ' and needs reflection coefficients, save the results of 'r' from
            ' each iteration.

            For j = 0 To i - 1
                r -= lpc(j) * aut(i - j)
            Next
            r /= [error]

            ' Update LPC coefficients and total error

            lpc(i) = r
            For j = 0 To i \ 2 - 1 ' "/" --> "\" CInt ???
                Dim tmp As Double = lpc(j)
                lpc(j) += r * lpc(i - 1 - j)
                lpc(i - 1 - j) += r * tmp
            Next
            If i Mod 2 <> 0 Then
                lpc(j) += lpc(j) * r
            End If

            [error] *= 1.0 - r * r

        Next
        Return lpc
    End Function

    Private Shared Function AutoCorrelate(frame As Double(), coeffNum As Integer) As Double
        Dim correlation As Double
        For i As Integer = coeffNum To frame.Length - 1
            correlation += frame(i) * frame(i - coeffNum)
        Next
        Return correlation
    End Function

    <DefaultValue(32)>
    Public Property Poles As Integer
        Get
            Return _Poles
        End Get
        Set(value As Integer)
            If value <> _Poles Then
                _Poles = value
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

End Class
