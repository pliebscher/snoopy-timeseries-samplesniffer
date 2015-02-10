''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class QuadMirror
    Inherits TimeSeriesPreprocessor

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim qmf As Double() = DirectCast(series.Samples.Clone(), Double())
        Array.Reverse(qmf)
        For i As Integer = 1 To qmf.Length - 1 Step 2
            qmf(i) *= -1
        Next
        Return qmf
    End Function

End Class
