Imports System.ComponentModel
''' <summary>
''' https://github.com/dhale/jtk/blob/master/src/main/java/edu/mines/jtk/dsp/SymmetricTridiagonalFilter.java
''' </summary>
''' <remarks></remarks>
<Description("Sets values outside the Min/Max to Zero.")>
Public Class TridiagonalFilter
    Inherits TimeSeriesPreprocessor

    Private _af As Double = 0.25
    Private _ai As Double = 0.5
    Private _al As Double = 0.25
    Private _b As Double = 0.5

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Return Apply(series.Samples)
    End Function

    Public Function Apply(x As Double()) As Double()

        Dim n As Integer = x.Length
        Dim nm1 As Integer = n - 1
        Dim y As Double() = New Double(n - 1) {}
        Dim xim1 As Double
        Dim xi As Double = x(0)
        Dim xip1 As Double = x(1)

        y(0) = _af * xi + _b * xip1

        For i As Integer = 1 To nm1 - 1
            xim1 = xi
            xi = xip1
            xip1 = x(i + 1)
            y(i) = ai * xi + _b * (xim1 + xip1)
        Next

        xim1 = xi
        xi = xip1
        y(n - 1) = _al * xi + _b * xim1

        Return y
    End Function

    <Description("The diagonal coefficient a for the first sample."), DefaultValue(0)>
    Public Property af As Double
        Get
            Return _af
        End Get
        Set(value As Double)
            If value <> _af Then
                _af = value
            End If
        End Set
    End Property

    <Description("The diagonal coefficient a for interior samples."), DefaultValue(1)>
    Public Property ai As Double
        Get
            Return _ai
        End Get
        Set(value As Double)
            If value <> _ai Then
                _ai = value
            End If
        End Set
    End Property

    <Description("The diagonal coefficient a for the last sample."), DefaultValue(1)>
    Public Property al As Double
        Get
            Return _al
        End Get
        Set(value As Double)
            If value <> _al Then
                _al = value
            End If
        End Set
    End Property

    <Description("The off-diagonal coefficient b."), DefaultValue(1)>
    Public Property b As Double
        Get
            Return _b
        End Get
        Set(value As Double)
            If value <> _b Then
                _b = value
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "A symmetric filter with three constant (shift-invariant) coefficients."
        End Get
    End Property

End Class
