Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<DefaultProperty("Kernel")>
Public Class Convolver
    Inherits TimeSeriesPostProcessor

    Private _KernelType As KernelType = KernelType.Sobel
    Private _KernelX As Double()()
    Private _KernelY As Double()()

    Public Sub New()
        _KernelX = New Double(2)() {}
        _KernelY = New Double(2)() {}
        SetKernel()
    End Sub

    Private Sub SetKernel()
        Select Case _KernelType
            Case KernelType.Sobel
                ' Estimates df/dx using Sobel kernel
                _KernelX(0) = New Double(2) {-1.0, 0.0, 1.0}
                _KernelX(1) = New Double(2) {-2.0, 0.0, 2.0}
                _KernelX(2) = New Double(2) {-1.0, 0.0, 1.0}
                ' Estimates df/dy using Sobel kernel
                _KernelY(0) = New Double(2) {-1.0, -2.0, -1.0}
                _KernelY(1) = New Double(2) {0.0, 0.0, 0.0}
                _KernelY(2) = New Double(2) {1.0, 2.0, 1.0}
            Case KernelType.Scharr
                'Estimates df/dx using Scharr kernel
                _KernelX(0) = New Double(2) {-3.0, 0.0, 3.0}
                _KernelX(1) = New Double(2) {-10.0, 0.0, 10.0}
                _KernelX(2) = New Double(2) {-3.0, 0.0, 3.0}
                'Estimates df/dy using Scharr kernel
                _KernelY(0) = New Double(2) {-3.0, -10.0, -3.0}
                _KernelY(1) = New Double(2) {0.0, 0.0, 0.0}
                _KernelY(2) = New Double(2) {3.0, 10.0, 3.0}
        End Select
    End Sub

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        Dim xLen As Integer = series.Samples.Length
        Dim yLen As Integer = series.Samples(0).Length
        Dim dx As Double()()
        Dim dy As Double()()
        Dim magnitude As Double()() = New Double(series.Samples.Length - 1)() {}
        'Dim direction As Double()() = New Double(series.Samples.Length - 1)() {}
        Dim maximum As Double
        Dim minimum As Double

        dx = series.Convolve(_KernelX)
        dy = series.Convolve(_KernelY)

        maximum = 0
        minimum = Double.MaxValue
        For x As Integer = 0 To xLen - 1
            magnitude(x) = New Double(yLen - 1) {}
            'direction(x) = New Double(yLen - 1) {}
            For y As Integer = 0 To yLen - 1
                magnitude(x)(y) = Math.Sqrt(Math.Pow(dx(x)(y), 2) + Math.Pow(dy(x)(y), 2))
                'maximum = Math.Max(maximum, magnitude(x)(y))
                'minimum = Math.Min(minimum, magnitude(x)(y))
                'magnitude(x)(y) = (magnitude(x)(y) - minimum) / (maximum - minimum)
                ' Direction is in the range [-pi/2, pi/2]
                'direction(x)(y) = Math.Atan(dy(x)(y) / dx(x)(y))
            Next
        Next

        Return magnitude 'series.Convolve(_KernelX).Convolve(_KernelY)

    End Function

    <Description("The type of kernel to use in the convolution."), DefaultValue(KernelType.Sobel)>
    Public Property Kernel As KernelType
        Get
            Return _KernelType
        End Get
        Set(value As KernelType)
            If value <> _KernelType Then
                _KernelType = value
                SetKernel()
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Convolution."
        End Get
    End Property

    Public Enum KernelType
        Sobel
        Scharr
    End Enum

End Class
