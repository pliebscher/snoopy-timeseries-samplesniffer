Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class ThresholdExtractor
    Inherits TimeSeriesPreprocessor

    Private _ThreshHigh As Double = 0.1
    Private _ThreshLow As Double = -0.1
    Private _Count As Integer = 512

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()

        Dim len As Integer = series.Samples.Length - 1
        Dim peaks As Double() = New Double(len) {}
        Dim sorted As Double() = New Double(len) {}
        Dim indexes As Int32() = Enumerable.Range(0, peaks.Length).ToArray()

        Array.Copy(series.Samples, sorted, series.Samples.Length)
        Array.Sort(sorted, indexes, AbsComparer.Instance)

        Dim i As Integer = 0
        Dim match As Integer = 0

        While match <= _Count And i <= len
            Dim sample As Double = Math.Abs(series.Samples(indexes(i)))
            If sample >= _ThreshLow AndAlso sample <= _ThreshHigh Then
                peaks(indexes(i)) = series.Samples(indexes(i))
                match += 1
            End If
            i += 1
        End While

        Return peaks
    End Function

    <Description("Max samples that fall within the threshold to extract."), DefaultValue(512)>
    Public Property Count As Integer
        Get
            Return _Count
        End Get
        Set(value As Integer)
            If value <> _Count Then
                _Count = value
            End If
        End Set
    End Property

    <Description("Threshold High."), DefaultValue(0.1)>
    Public Property ThreshHigh As Double
        Get
            Return _ThreshHigh
        End Get
        Set(value As Double)
            _ThreshHigh = value
        End Set
    End Property

    <Description("Threshold Low."), DefaultValue(-0.1)>
    Public Property ThreshLow As Double
        Get
            Return _ThreshLow
        End Get
        Set(value As Double)
            _ThreshLow = value
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Removes (zeros) values outside the threshold bounds."
        End Get
    End Property

End Class
