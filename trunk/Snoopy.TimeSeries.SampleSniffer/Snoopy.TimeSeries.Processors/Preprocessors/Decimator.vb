Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Decimator
    Inherits TimeSeriesPreprocessor

    Private _Factor As Double = 0.5
    Private _Func As Func(Of Double(), Double) = Function(n) Math.Abs(n.Min) ' The decimation function.

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()

        Dim inLen As Integer = series.Samples.Length
        Dim width As Integer = CInt((inLen * _Factor))
        Dim [step] As Integer = series.Samples.Length \ width
        Dim remStep As Integer = series.Samples.Length - ([step] * width)
        Dim pos As Integer
        Dim remainder As Integer
        Dim prev As Integer

        Dim decimated As Double() = New Double(width - 1) {}
        Dim chunk As Double() = New Double([step] - 1) {}

        For i As Integer = 0 To width - 1
            pos += [step]
            remainder += remStep
            If (remainder >= width) Then
                pos += 1
                remainder -= width
            End If
            Array.Copy(series.Samples, prev, chunk, 0, [step])
            decimated(i) = _Func(chunk)
            prev = pos
        Next

        Return decimated
    End Function

    <Description("Decimation factor."), DefaultValue(0.5)>
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

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Sample rate reduction by decimation."
        End Get
    End Property

End Class
