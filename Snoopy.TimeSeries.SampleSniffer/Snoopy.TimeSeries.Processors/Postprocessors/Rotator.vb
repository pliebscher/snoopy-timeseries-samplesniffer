Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Rotator
    Inherits TimeSeriesPostProcessor

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        Dim rotated As Double()() = New Double(series.Samples(0).Length - 1)() {}

        For i As Integer = series.Samples(0).Length - 1 To 0 Step -1
            rotated(series.Samples(0).Length - i - 1) = New Double(series.Samples.Length - 1) {}
            For j As Integer = series.Samples.Length - 1 To 0 Step -1
                rotated(series.Samples(0).Length - i - 1)(j) = series.Samples(j)(i)
            Next
        Next

        Return rotated
    End Function

End Class
