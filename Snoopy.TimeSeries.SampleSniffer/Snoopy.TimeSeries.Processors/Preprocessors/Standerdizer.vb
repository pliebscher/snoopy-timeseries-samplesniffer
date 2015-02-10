''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Standerdizer
    Inherits TimeSeriesPreprocessor

    ''' <summary>
    ''' Modifies a data sequence to be standardized.
    ''' </summary>
    ''' <param name="series"></param>
    ''' <returns></returns>
    ''' <remarks>http://jaudio.cvs.sourceforge.net/viewvc/jaudio/jAudio1.0/src/cern/jet/stat/Descriptive.java</remarks>
    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim mean As Double = series.Mean
        Dim stdDev As Double = series.StdDev
        For i As Integer = series.Samples.Length - 1 To 0 Step -1
            series.Samples(i) = (series.Samples(i) - mean) / stdDev  ' Inline
        Next
        Return series.Samples
    End Function

End Class
