''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class MeanSubtractor
    Inherits TimeSeriesPreprocessor

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim mean As Double = series.Mean
        For i As Integer = 0 To series.Samples.Length - 1
            series.Samples(i) -= mean
        Next i
        Return series.Samples
    End Function

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Subtracts the Mean from each sample in the TimeSeries."
        End Get
    End Property

End Class
