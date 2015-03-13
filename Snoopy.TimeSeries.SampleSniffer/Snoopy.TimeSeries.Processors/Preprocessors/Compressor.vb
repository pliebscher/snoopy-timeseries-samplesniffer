Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Description("")>
Public Class Compressor
    Inherits TimeSeriesPreprocessor

    Private _Factor As Double = 0.75

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        For i As Integer = 0 To series.Samples.Length - 1
            Dim sign As Integer = If((series.Samples(i) < 0), -1, 1)
            Dim norm As Double = Math.Abs(series.Samples(i))
            norm = 1.0 - Math.Pow(1.0 - norm, _Factor)
            If Double.IsNaN(norm) Then norm = 0 ' Should we be doing this? Or would an exception be better?
            series.Samples(i) = norm * sign ' Inline processing
        Next
        Return series.Samples
    End Function

    <Description("Compression factor."), DefaultValue(0.75)>
    Public Property Factor As Double
        Get
            Return _Factor
        End Get
        Set(value As Double)
            If value <> _Factor Then
                _Factor = value
            End If
        End Set
    End Property

End Class
