Imports System.ComponentModel
''' <summary>
''' http://code.google.com/p/musicg/source/browse/src/com/musicg/dsp/LinearInterpolation.java
''' </summary>
''' <remarks></remarks>
<Description("Do interpolation on the samples according to the original and new sample rates.")>
Public Class LinearInterpolator
    Inherits TimeSeriesPreprocessor

    Private _NewSampleRate As Integer = 22050

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()

        Dim oldsamplerate As Integer = series.SampleRate
        Dim samples As Double() = series.Samples

        If series.SampleRate = _NewSampleRate Then
            Return samples
        End If

        Dim newLength As Integer = CInt(Math.Round((samples.Length / oldsamplerate * _NewSampleRate)))
        Dim lengthMultiplier As Double = (newLength) / samples.Length
        Dim interpolatedSamples As Double() = New Double(newLength - 1) {}

        ' interpolate the value by the linear equation y=mx+c        
        For i As Integer = 0 To newLength - 1

            ' get the nearest positions for the interpolated point
            Dim currentPosition As Double = i / lengthMultiplier
            Dim nearestLeftPosition As Integer = CInt(Math.Truncate(currentPosition))
            Dim nearestRightPosition As Integer = nearestLeftPosition + 1

            If nearestRightPosition >= samples.Length Then
                nearestRightPosition = samples.Length - 1
            End If

            ' delta x is 1
            Dim slope As Double = samples(nearestRightPosition) - samples(nearestLeftPosition)
            Dim positionFromLeft As Double = currentPosition - nearestLeftPosition

            interpolatedSamples(i) = (slope * positionFromLeft + samples(nearestLeftPosition))
        Next

        Return interpolatedSamples
    End Function

    <Description(""), DefaultValue(22050)>
    Public Property NewSampleRate As Integer
        Get
            Return _NewSampleRate
        End Get
        Set(value As Integer)
            If value <> _NewSampleRate Then
                _NewSampleRate = value
            End If
        End Set
    End Property

End Class
