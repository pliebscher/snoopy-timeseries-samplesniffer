Imports System.ComponentModel
''' <summary>
''' http://code.google.com/p/toot2/source/browse/trunk/src/uk/org/toot/dsp/DCBlocker.java
''' </summary>
''' <remarks></remarks>
<Description("DC Blocker."), DefaultProperty("Alpha")>
Public Class DCBlocker
    Inherits TimeSeriesPreprocessor

    Private _A As Double = 0.999

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim P As Double = 0

        For i As Integer = 0 To series.Samples.Length - 1
            Dim m As Double = series.Samples(i) + _A * P
            Dim y As Double = m - P
            P = m
            series.Samples(i) = y
        Next

        Return series.Samples
    End Function

    <Description("Number of samples to shift."), DefaultValue(0.999)>
    Public Property Alpha As Double
        Get
            Return _A
        End Get
        Set(value As Double)
            If value <> _A Then
                _A = value
            End If
        End Set
    End Property

End Class
