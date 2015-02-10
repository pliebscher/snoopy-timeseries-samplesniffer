Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Transposer
    Inherits TimeSeriesPostProcessor

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()
        Return Transpose(series.Samples)
    End Function

    Public Shared Function Transpose(a As Double()()) As Double()()

        Dim y As Double()() = New Double(a(0).Length - 1)() {}

        

        Return y
    End Function

End Class
