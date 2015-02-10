Imports System.ComponentModel
''' <summary>
''' http://elki.dbs.ifi.lmu.de/browser/elki/trunk/src/de/lmu/ifi/dbs/elki/utilities/scaling
''' </summary>
''' <remarks></remarks>
<Description("Circular shifter.")>
Public Class CShifter
    Inherits TimeSeriesPreprocessor

    Private _Count As Integer = 0

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Shift(series.Samples, _Count)
        Return series.Samples
    End Function

    Public Shared Sub Shift(a As Double(), count As Integer)
        Dim tail As Double() = New Double(count - 1) {}
        Array.Copy(a, a.Length - count, tail, 0, count)
        Array.Copy(a, 0, a, count, a.Length - count)
        Array.Copy(tail, a, count)
    End Sub

    <Description("Number of samples to shift.")>
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
