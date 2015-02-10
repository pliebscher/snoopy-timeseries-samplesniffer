Imports System.ComponentModel
''' <summary>
''' VolumeJ
''' http://webscreen.ophth.uiowa.edu/bij/
''' </summary>
''' <remarks></remarks>
<Description("")>
Public Class SplineInterpolator
    Inherits TimeSeriesPreprocessor

    Private _Location As Double = 1
    Private _SubSamples As Double = 4

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        For i As Integer = 0 To series.Samples.Length - 1
            Dim sample As Double = series.Samples(i)
            ' TODO: Finish me!
        Next
        Return series.Samples
    End Function

    <Description("The location at which to interpolate the sample."), DefaultValue(1)>
    Public Property Location As Double
        Get
            Return _Location
        End Get
        Set(value As Double)
            If value <> _Location Then
                _Location = value

            End If
        End Set
    End Property

    <Description("Number of samples in each interpolation."), DefaultValue(4)>
    Public Property SubSamples As Double
        Get
            Return _SubSamples
        End Get
        Set(value As Double)
            If value <> _Location Then
                _SubSamples = value
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Cubic spline interpolation."
        End Get
    End Property

    '         * Cubic spline interpolation.
    '         * Also known as Catmull-Rom spline
    '         * Calculate sample = h(x) a + h(x) b + h(x) c + h(x) d,
    '         * where a,b,c,d are the values at the sample locations, and x (-2,2) is the location between
    '         * a and d.
    '         * @param x {0,1} the location at which to interpolate the sample
    '         * @param a,b,c,d the values of the samples at -1, 0, 1, 2
    '         * @return the interpolated value
    '         
    Private Shared Function cubicspline(x As Double, a As Double, b As Double, c As Double, d As Double) As Double
        Return h(-1.0 - x) * a + h(-x) * b + h(1.0 - x) * c + h(2.0 - x) * d
    End Function

    '*
    '         * Calculate spline h(x) value.
    '         *              h(-x)                           x<0
    '         * h(x) =       3/2x^3-5/2x^2+1                 0 <= x < 1
    '         *              -1/2x^3 + 5/2x^2 - 4x + 2       1 <= x < 2
    '         *              0                               otherwise
    '         *  @param x the value for which to calculate h.
    '         *  @return the h value as above.
    '         
    Private Shared Function h(x As Double) As Double
        If x < 0 Then
            x = -x
        End If
        If x >= 0 AndAlso x < 1 Then
            Return Math.Pow(x, 3) * 1.5 - 2.5 * Math.Pow(x, 2) + 1.0
        ElseIf x >= 1 AndAlso x < 2 Then
            Return -Math.Pow(x, 3) * 0.5 + 2.5 * Math.Pow(x, 2) - 4.0 * x + 2.0
        Else
            Return 0
        End If
    End Function

End Class
