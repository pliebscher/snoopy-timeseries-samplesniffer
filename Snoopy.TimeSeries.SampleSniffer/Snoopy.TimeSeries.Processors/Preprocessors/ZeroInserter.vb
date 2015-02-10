Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Description("Inserts (n) zeros evern (x) samples.")>
Public Class ZeroInserter
    Inherits TimeSeriesPreprocessor

    Private _Count As Integer = 1

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim len As Integer = series.Samples.Length + (_Count * series.Samples.Length)
        Dim samples As Double() = New Double(len - 1) {}
        For i As Integer = 0 To series.Samples.Length - 1
            samples((_Count + 1) * i) = series.Samples(i)
        Next
        Return samples
    End Function

    <Description("Number of zeros to insert for each sample."), DefaultValue(1)>
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

End Class
