Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public MustInherit Class TimeSeriesPreprocessor
    Inherits TimeSeriesProcessor

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overloads Sub Process(series As TimeSeries)
        If Not Enabled Then Exit Sub
        series._PreProcSamples = Me.OnProcess(series)
    End Sub

    Protected MustOverride Function OnProcess(series As ITimeSeries(Of Double())) As Double()

End Class
