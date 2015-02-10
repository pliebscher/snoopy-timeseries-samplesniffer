Imports System.ComponentModel
''' <summary>
''' Fast Sine/Cosine Transform
''' </summary>
''' <remarks></remarks>
<DefaultProperty("TransformType")>
Public Class FSCT
    Inherits TimeSeriesFrameTransformer

    Private ReadOnly _FFT As New FFT

    Private _FFTPhaseMode As FFT.Phase = FFT.Phase.Data
    Private _Type As FSCT.Type = Type.COS
    Private _Normalize As Scale = Scale.STANDARD

    Public Sub New()
        MyBase.FrameWidth = FrameWidth._256
        MyBase.FrameStep = FrameStep._64
        _FFT.PhaseMode = _FFTPhaseMode
    End Sub

    Protected Overrides Function OnTransformFrame(frame As Double()) As Double()

        Dim newframe As Double()

        If _Type = Type.COS Then
            newframe = COS(frame)
        Else
            newframe = SIN(frame)
        End If

        If _Normalize = Scale.ORTHOGONAL Then
            Dim fact As Double = Math.Sqrt(2.0 / (frame.Count - 1))
            Norm(newframe, fact)
        End If

        Return newframe

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="f"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function COS(f As Double()) As Double()

        Dim transformed As Double() = New Double(f.Length \ 2 - 1) {}
        Dim n As Integer = f.Length - 1

        ' construct a new array and perform FFT on it
        Dim x As Double() = New Double(n) {}
        x(0) = 0.5 * (f(0) + f(n))
        x(n >> 1) = f(n >> 1)

        ' temporary variable for transformed[1]
        Dim t1 As Double = 0.5 * (f(0) - f(n))

        For i As Integer = 1 To (n >> 1) - 1
            Dim a As Double = 0.5 * (f(i) + f(n - i))
            Dim b As Double = Math.Sin(i * Math.PI / n) * (f(i) - f(n - i))
            Dim c As Double = Math.Cos(i * Math.PI / n) * (f(i) - f(n - i))
            x(i) = a - b
            x(n - i) = a + b
            t1 += c
        Next

        _FFT.Real(x, True)

        ' reconstruct the FCT result for the original array
        transformed(0) = x(0)
        transformed(1) = t1
        For i As Integer = 1 To (n >> 1) '- 1
            Dim re As Double = x(2 * i)
            Dim im As Double = x(2 * i - 1) - x(2 * i + 1)
            transformed(i) = Math.Sqrt(re * re + im * im)
        Next

        Return transformed

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="f"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SIN(f As Double()) As Double()

        Dim n As Integer = f.Length - 1
        Dim transformed As Double() = New Double(f.Length \ 2 - 1) {}
        Dim x As Double() = New Double(n) {}

        x(0) = 0.0
        x(n >> 1) = 2.0 * f(n >> 1)

        For i As Integer = 1 To (n >> 1) - 1
            Dim a As Double = Math.Sin(i * Math.PI / n) * (f(i) + f(n - i))
            Dim b As Double = 0.5 * (f(i) - f(n - i))
            x(i) = a + b
            x(n - i) = a - b
        Next

        _FFT.Real(x, True)

        transformed(0) = 0.5 * x(0)
        For i As Integer = 1 To (n >> 1) '- 1
            Dim re As Double = x(2 * i) + x(2 * i - 1)
            Dim im As Double = -x(2 * i + 1)
            transformed(i) = Math.Sqrt(re * re + im * im)
        Next

        Return transformed

    End Function

    Private Shared Sub Norm(f As Double(), d As Double)
        For i As Integer = 0 To f.Length - 1
            f(i) *= d
        Next
    End Sub

    <Description("FFT phase mode."), DefaultValue(FFT.Phase.Data)>
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

    <DisplayName("Type"), DefaultValue(Type.COS)>
    Public Property TransformType As FSCT.Type
        Get
            Return _Type
        End Get
        Set(value As FSCT.Type)
            If value <> _Type Then
                _Type = value
            End If
        End Set
    End Property

    <Description(""), DefaultValue(Scale.STANDARD)>
    Public Property Normalize As Scale
        Get
            Return _Normalize
        End Get
        Set(value As Scale)
            _Normalize = value
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Fast Sine/Cosine Transform"
        End Get
    End Property

    Public Enum [Type]
        SIN
        COS
    End Enum

    Public Enum Scale
        STANDARD
        ORTHOGONAL
    End Enum

End Class
