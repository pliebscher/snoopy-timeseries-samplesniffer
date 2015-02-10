''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class PeakSorter
    Inherits TimeSeriesPostProcessor

    Private _Direction As SortDirection = SortDirection.Center

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        Dim sorted As Double()() = New Double(series.Samples.Length - 1)() {}

        For i As Integer = 0 To series.Samples.Length - 1
            Dim indexes As Int32() = Enumerable.Range(0, series.Samples(i).Length).ToArray
            sorted(i) = New Double(series.Samples(i).Length - 1) {}
            Array.Copy(series.Samples(i), sorted(i), series.Samples(i).Length)
            Array.Sort(sorted(i), indexes, AbsComparer.Instance)
            'sorted(i) = snorm(frames(i), 16, 0.95)
            If _Direction = SortDirection.Center Then
                sorted(i) = CenterSort(sorted(i))
            End If
        Next

        Return sorted
    End Function

    Private Function CenterSort(frame() As Double) As Double()

        Dim len As Integer = frame.Length \ 2
        Dim sorted As Double() '= New Double(frame.Length - 1) {}

        Dim sortedU As Double() = New Double(len - 1) {}
        Dim sortedL As Double() = New Double(len - 1) {}

        For j As Integer = 0 To len - 1
            sortedU(j) = frame(2 * j)
            sortedL(j) = frame(2 * j + 1)
        Next

        sorted = sortedL.Reverse.ToArray().Concat(sortedU).ToArray

        Return sorted

    End Function

    Private Shared Function snorm(x As Double(), length As Integer, alpha As Double) As Double()
        ' Get the number of vectors in the sequence
        Dim n As Integer = x.Length \ length

        ' Create the augmented sequence projection
        Dim xs As Double() = New Double(x.Length + (n - 1)) {}

        ' For each vector in the sequence
        For j As Integer = 0 To n - 1
            ' Compute its starting position in the
            '  source and destination sequences
            Dim src As Integer = j * length
            Dim dst As Integer = j * (length + 1)

            ' Compute augmented vector norm
            Dim norm As Double = alpha * alpha
            For k As Integer = src To src + (length - 1)
                norm += x(k) * x(k)
            Next
            norm = System.Math.Sqrt(norm)

            ' Normalize the augmented vector and
            '  copy to the destination sequence
            xs(dst + length) = alpha / norm
            Dim i As Integer = dst
            While i < dst + length
                xs(i) = x(src) / norm
                i += 1
                src += 1
            End While
        Next

        Return xs

    End Function

    Public Property Direction As SortDirection
        Get
            Return _Direction
        End Get
        Set(value As SortDirection)
            If value <> _Direction Then
                _Direction = value

            End If
        End Set
    End Property

    Public Enum SortDirection
        Center
        Side
    End Enum

End Class
