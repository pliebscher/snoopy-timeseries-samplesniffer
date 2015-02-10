Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<DefaultProperty("Wavelength")>
Public Class GaborFilter
    Inherits TimeSeriesPostProcessor

    Private _Wavelength As Double = 7.5
    Private _Direction As Double = 0.0
    Private _Kernel As Double()()

    Public Sub New()
        CreateKernel()
    End Sub

    ''' <summary>
    ''' http://patrick-fuller.com/gabor-filter-image-processing-for-scientists-and-engineers-part-6/
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateKernel()

        ' Overwriting sigma with octaval value (the standard is to use one octave)
        Dim octave As Double = 1
        Dim sigma As Double = _Wavelength * 1 / Math.PI * Math.Sqrt(Math.Log(2) / 2) * (Math.Pow(2, octave) + 1) / (Math.Pow(2, octave) - 1)

        ' Getting the required kernel size from the sigma
        ' threshold = 0.005, x is minimum odd integer required
        Dim x As Integer = CInt(Math.Ceiling(Math.Sqrt(-2 * sigma * sigma * Math.Log(0.005))))
        If x Mod 2 = 1 Then
            x += 1
        End If

        ' Generate a kernel by sampling the Gaussian and Fourier functions
        _Kernel = New Double(2 * x + 1)() {}
        Dim uc As Integer
        Dim vc As Integer
        Dim f As Double
        Dim g As Double
        Dim theta As Double

        For u As Integer = 0 To _Kernel.Length - 1
            _Kernel(u) = New Double(2 * x + 1) {}
            For v As Integer = 0 To _Kernel(0).Length - 1
                ' Center the Gaussian sample so max is at u,v = 10,10
                uc = u - (_Kernel.Length - 1) \ 2
                vc = v - (_Kernel(0).Length - 1) \ 2

                ' Calculate the Gaussian
                g = Math.Exp(-(uc * uc + vc * vc) / (2 * sigma * sigma))
                ' Calculate the real portion of the Fourier transform
                theta = uc * Math.Cos(_Direction) + vc * Math.Sin(_Direction)
                f = Math.Cos(2 * Math.PI * theta / _Wavelength)
                _Kernel(u)(v) = f * g
            Next
        Next

    End Sub

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        Return series.Convolve(_Kernel)

    End Function

    <Description(""), DefaultValue(7.5)>
    Public Property Wavelength As Double
        Get
            Return _Wavelength
        End Get
        Set(value As Double)
            If value <> _Wavelength Then
                _Wavelength = value
                CreateKernel()
            End If
        End Set
    End Property

    <Description(""), DefaultValue(0.0)>
    Public Property Direction As Double
        Get
            Return _Direction
        End Get
        Set(value As Double)
            If value <> _Direction Then
                _Direction = value
                CreateKernel()
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "A Gabor filter."
        End Get
    End Property

End Class
