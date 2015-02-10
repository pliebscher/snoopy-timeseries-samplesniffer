Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Scaler
    Inherits TimeSeriesPreprocessor

    Private _Min As Double = -1
    Private _Max As Double = 1

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim sMin As Double = series.Min
        Dim sMax As Double = series.Max
        For i As Integer = 0 To series.Samples.Length - 1
            series.Samples(i) = Min + (series.Samples(i) - sMin) / (sMax - sMin) * (Max - Min) ' Inline...
        Next
        Return series.Samples ' ...but still need to return
    End Function

    <Description("Minimum value."), DefaultValue(-1)>
    Public Property Min As Double
        Get
            Return _Min
        End Get
        Set(value As Double)
            _Min = value
        End Set
    End Property

    <Description("Maximum value."), DefaultValue(1)>
    Public Property Max As Double
        Get
            Return _Max
        End Get
        Set(value As Double)
            _Max = value
        End Set
    End Property

    'Private _T As Double()() = New Double(2)() {}

    'Public Property T As Double()()
    '    Get
    '        Return _T
    '    End Get
    '    Set(value As Double()())
    '        _T = value
    '    End Set
    'End Property

End Class
