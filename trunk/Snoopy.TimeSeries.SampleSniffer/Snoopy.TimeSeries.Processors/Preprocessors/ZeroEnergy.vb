Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Description("Sets all values in the current series to zero if its' energy is outside the lower and upper threshold.")>
Public Class ZeroEnergy
    Inherits TimeSeriesPreprocessor

    Private _ThreshLow As Double = 4.0
    Private _ThreshHigh As Double = 40.0

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim energy As Double = series.Energy
        If energy < _ThreshLow OrElse energy > _ThreshHigh Then Return New Double(series.Samples.Length - 1) {}
        Return series.Samples
    End Function

    Protected Overrides Sub OnSampleRateChanged(sampleRate As Integer)
        MyBase.OnSampleRateChanged(sampleRate)
    End Sub

    <Description("Energy threshold lower bound."), DefaultValue(4.0)>
    Public Property ThreshLow As Double
        Get
            Return _ThreshLow
        End Get
        Set(value As Double)
            If value <> _ThreshLow Then
                _ThreshLow = value

            End If
        End Set
    End Property

    <Description("Energy threshold upper bound."), DefaultValue(40.0)>
    Public Property ThreshHigh As Double
        Get
            Return _ThreshHigh
        End Get
        Set(value As Double)
            If value <> _ThreshHigh Then
                _ThreshHigh = value

            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Sets all samples to zero where energy is outside the given bounds."
        End Get
    End Property

End Class
