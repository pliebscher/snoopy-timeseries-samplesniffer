Imports System.ComponentModel
''' <summary>
''' http://code.google.com/p/speech-recognition-java-hidden-markov-model-vq-mfcc/source/browse/trunk/SpeechRecognitionHMM/src/org/ioe/tprsa/audio/feature/Delta.java
''' </summary>
''' <remarks></remarks>
Public Class Delta2D
    Inherits TimeSeriesPostProcessor

    Private _M As Integer = 2 ' length of regression window

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        Dim coeffCount As Integer = series.Samples(0).Length
        Dim frameCount As Integer = series.Samples.Length

        ' 1. calculate sum of mSquare i.e., denominator
        Dim mSqSum As Double = 0
        For i As Integer = -_M To _M - 1
            mSqSum += Math.Pow(i, 2)
        Next

        Dim delta As Double()() = New Double(frameCount - 1)() {}
        For i As Integer = 0 To frameCount - 1
            delta(i) = New Double(coeffCount - 1) {}
        Next

        For i As Integer = 0 To coeffCount - 1

            For k As Integer = 0 To _M - 1
                delta(k)(i) = series.Samples(k)(i)
            Next

            For k As Integer = frameCount - _M To frameCount - 1
                delta(k)(i) = series.Samples(k)(i)
            Next

            For j As Integer = _M To frameCount - _M - 1
                Dim sumDataMulM As Double = 0
                For m As Integer = -_M To _M
                    sumDataMulM += m * series.Samples(m + j)(i)
                Next
                delta(j)(i) = sumDataMulM / mSqSum
            Next

        Next

        Return delta
    End Function

    <Description("Length of regression window.")>
    Public Property WindowLen As Integer
        Get
            Return _M
        End Get
        Set(value As Integer)
            If value <= 0 Then
                Throw New ArgumentException("Invalid Window Length. Must be greater then 0.")
            End If
            If value <> _M Then
                _M = value

            End If
        End Set
    End Property

End Class
