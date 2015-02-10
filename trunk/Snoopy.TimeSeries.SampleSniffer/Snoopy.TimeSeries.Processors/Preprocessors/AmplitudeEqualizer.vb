Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class AmplitudeEqualizer
    Inherits TimeSeriesPreprocessor

    Private _PreEmpAlpha As Double = 0.95

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim eq As Double() = New Double(series.Samples.Length - 1) {}
        For i As Integer = 1 To series.Samples.Length - 1
            eq(i) = series.Samples(i) - _PreEmpAlpha * series.Samples(i - 1)
        Next
        Return eq
    End Function

    <Description("Alpha value must be between 0 and .99"), DefaultValue(0.95)>
    Public Property PreEmpAlpha As Double
        Get
            Return _PreEmpAlpha
        End Get
        Set(value As Double)
            If value < 0 Or value > 0.99 Then Throw New ArgumentException("Alpha value must be between 0 and .99")
            If value <> _PreEmpAlpha Then
                _PreEmpAlpha = value
            End If
        End Set
    End Property

End Class
