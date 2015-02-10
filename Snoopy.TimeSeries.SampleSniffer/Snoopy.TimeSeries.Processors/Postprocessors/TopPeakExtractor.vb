''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class TopPeakExtractor
    Inherits TimeSeriesPostProcessor

    Private _Count As Integer = 256

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()
        Dim rows As Integer = series.Samples.GetLength(0)
        Dim cols As Integer = series.Samples(0).Length

        If Count > rows * cols Then
            Count = rows * cols
        End If

        Dim concatenated As Double() = New Double(rows * cols - 1) {}

        For row As Integer = 0 To rows - 1
            Array.Copy(series.Samples(row), 0, concatenated, row * series.Samples(row).Length, series.Samples(row).Length)
        Next

        Dim indexes As Int32() = Enumerable.Range(0, concatenated.Length).ToArray()

        Array.Sort(concatenated, indexes, AbsComparer.Instance)

        Dim peaks As Double()() = New Double(series.Samples.Length - 1)() {}

        For i As Integer = 0 To rows - 1
            peaks(i) = New Double(cols - 1) {}
        Next

        For i As Integer = 0 To Count - 1
            peaks(indexes(i) \ cols)(indexes(i) Mod cols) = concatenated(i)
        Next

        Return peaks
    End Function

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
