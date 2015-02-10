Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class ThresholdZero
    Inherits TimeSeriesPostProcessor

    Private _ThreshHigh As Double = 1
    Private _ThreshLow As Double = -1

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        For i As Integer = 0 To series.Samples.Length - 1
            For j As Integer = 0 To series.Samples(0).Length - 1
                If series.Samples(i)(j) < _ThreshLow OrElse series.Samples(i)(j) > _ThreshHigh Then
                    series.Samples(i)(j) = 0
                End If
            Next
        Next

        Return series.Samples
    End Function

    <Description("Threshold High."), DefaultValue(1)>
    Public Property ThreshHigh As Double
        Get
            Return _ThreshHigh
        End Get
        Set(value As Double)
            _ThreshHigh = value
        End Set
    End Property

    <Description("Threshold Low."), DefaultValue(-1)>
    Public Property ThreshLow As Double
        Get
            Return _ThreshLow
        End Get
        Set(value As Double)
            _ThreshLow = value
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Sets values outside the threshold bounds to zero."
        End Get
    End Property

End Class
