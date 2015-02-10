Imports System.ComponentModel
''' <summary>
''' https://github.com/marytts/marytts/blob/master/marytts-signalproc/src/main/java/marytts/util/math/MathUtils.java
''' </summary>
''' <remarks></remarks>
Public Class Autocorrelate
    Inherits TimeSeriesPreprocessor

    Private _FrameWidth As FrameWidth = FrameWidth._2048

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim R As Double() = New Double(_FrameWidth - 1) {}
        Dim samples As Double() = series.Samples
        Dim N1 As Integer = samples.Length
        Dim m As Integer, n As Integer

        For m = 0 To _FrameWidth - 1
            R(m) = 0.0
            For n = 0 To N1 - m - 1
                R(m) += samples(n) * samples(n + m)
            Next
        Next

        Return R
    End Function

    <TypeConverter(GetType(EnumIntValueConverter)), DefaultValue(FrameWidth._2048)>
    Public Overridable Property Width As FrameWidth
        Get
            Return _FrameWidth
        End Get
        Set(value As FrameWidth)
            _FrameWidth = value
        End Set
    End Property

End Class
