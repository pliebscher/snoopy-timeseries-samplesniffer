Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class SSIMQuery
    Inherits TimeSeriesQuery

    Private _SSIM As New SSIM

    Public Sub New(query As TimeSeries)
        MyBase.New(query)
        _SSIM.L = 1.0
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double
        Dim sim As Double = _SSIM.Compute(Criteria.Frames, data.Frames)
        Return sim
    End Function

    Protected Overrides Sub OnQueryReset(query As TimeSeries)
        L = 1.0
        K1 = 0.0001
        K2 = 0.0003
    End Sub

    <Description("K1 << 1 is a small constant."), DefaultValue(0.0001)>
    Public Property K1 As Double
        Get
            Return _SSIM.K1
        End Get
        Set(value As Double)
            _SSIM.K1 = value
        End Set
    End Property

    <Description("K2 << 1 is a small constant."), DefaultValue(0.0003)>
    Public Property K2 As Double
        Get
            Return _SSIM.K2
        End Get
        Set(value As Double)
            _SSIM.K2 = value
        End Set
    End Property

    <Description("The dynamic range of the pixel values (255 for 8-bit grayscale images)."), DefaultValue(1.0)>
    Public Property L As Double
        Get
            Return _SSIM.L
        End Get
        Set(value As Double)
            _SSIM.L = value
        End Set
    End Property

    <Description("The width of the Gaussian window. Odd number."), DefaultValue(11)>
    Public Property WindowWidth As Integer
        Get
            Return _SSIM.WindowWidth
        End Get
        Set(value As Integer)
            If value < 2 Then Throw New ArgumentException("Invalid Window Width.")
            If value Mod 2 = 0 Then Throw New ArgumentException("Window Width must be an odd number.")
            If value <> _SSIM.WindowWidth Then
                _SSIM.WindowWidth = value
            End If
        End Set
    End Property

    <Description("Std. deviation of the Gaussian window."), DefaultValue(1.5)>
    Public Property WindowSigma As Double
        Get
            Return _SSIM.WindowSigma
        End Get
        Set(value As Double)
            If value <> _SSIM.WindowSigma Then
                _SSIM.WindowSigma = value
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "SSIM (Post)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Structural SIMilarity"
        End Get
    End Property

End Class
