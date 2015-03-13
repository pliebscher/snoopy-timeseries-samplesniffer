''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class MeanOffset
    Inherits TimeSeriesPreprocessor

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim offset As Double() = New Double(series.Samples.Length - 1) {}
        Dim mean As Double = series.Mean
        Dim i As Integer
        For i = 0 To series.Samples.Length - 1
            Dim sample As Double = series.Samples(i)
            'offset(i) = (num3 - mean)
            If sample <= mean Then
                offset(i) = mean
            End If
        Next i
        Return offset
    End Function

End Class
