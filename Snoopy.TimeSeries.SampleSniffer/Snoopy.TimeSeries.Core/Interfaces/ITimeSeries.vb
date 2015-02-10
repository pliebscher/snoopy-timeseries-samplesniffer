''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Interface ITimeSeries

    ReadOnly Property SampleRate As Integer
    ReadOnly Property TimeStamp As Date
    ReadOnly Property TimeSpan As TimeSpan

    ReadOnly Property Mean As Double
    ReadOnly Property Energy As Double
    ReadOnly Property Power As Double
    ReadOnly Property Norm As Double
    ReadOnly Property StdDev As Double
    ReadOnly Property StdErr As Double
    ReadOnly Property Variance As Double
    ReadOnly Property Min As Double
    ReadOnly Property Max As Double
    ReadOnly Property Kurtosis As Double
    ReadOnly Property Skewness As Double

End Interface

''' <summary>
''' 
''' </summary>
''' <typeparam name="TSamples"></typeparam>
''' <remarks></remarks>
Public Interface ITimeSeries(Of TSamples)
    Inherits ITimeSeries

    ReadOnly Property Samples As TSamples

End Interface

