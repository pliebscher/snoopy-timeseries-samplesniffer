Imports Snoopy.TimeSeries
Imports System.ComponentModel
Imports Snoopy.TimeSeries.Windows
''' <summary>
''' Provides a base class for all TimeSeries transformers.
''' </summary>
''' <remarks></remarks>
Public MustInherit Class TimeSeriesTransformer
    Inherits TimeSeriesProcessor

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Transforms the given TimeSeries.
    ''' </summary>
    ''' <param name="series">The TimeSeries to transform.</param>
    ''' <remarks></remarks>
    Public Sub Transform(series As TimeSeries)
        series._PostProcFrames = Me.OnTransform(series)
    End Sub

    ''' <summary>
    ''' Supplies the derived Class with the TimeSeries to transform.
    ''' </summary>
    ''' <param name="series">The TimeSeries to transform.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected MustOverride Function OnTransform(series As ITimeSeries(Of Double())) As Double()()

End Class
