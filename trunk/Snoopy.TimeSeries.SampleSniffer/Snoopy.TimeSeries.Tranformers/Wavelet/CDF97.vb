Public Class CDF97
    Implements IDecompose

    Private Shared ReadOnly _Instance As CDF97 = New CDF97

    Public Shared Sub Decompose(data As Double()(), levels As Integer)
        Dim w As Integer = data(0).Length
        Dim h As Integer = data.GetLength(0)

        While (w + h) > 2
            Decompose(data, w, h)
            Decompose(data, w, h)
            w = w >> 1
            h = h >> 1
        End While

        For i As Integer = 0 To levels - 1

        Next
    End Sub

    Public Shared Function Decompose(x As Double()(), width As Integer, height As Integer) As Double()()
        For j As Integer = 0 To width - 1
            For i As Integer = 1 To (height - 1) - 1 Step 2
                Dim num1 As Double = x(i)(j)
                num1 += -1.586134342 * (x(i - 1)(j) + x(i + 1)(j))
            Next
            Dim num2 As Double = x(height - 1)(j)
            num2 += -3.172268684 * x(height - 2)(j)
            For i As Integer = 2 To height - 2 Step 2 ' 1 Step 2
                Dim num3 As Double = x(i)(j)
                num3 += -0.05298011854 * (x(i - 1)(j) + x(i + 1)(j))
            Next
            Dim num4 As Double = x(0)(j)
            num4 += -0.10596023708 * x(1)(j)
            For i As Integer = 1 To (height - 1) - 1 Step 2
                Dim num5 As Double = x(i)(j)
                num5 += 0.8829110762 * (x(i - 1)(j) + x(i + 1)(j))
            Next
            Dim num6 As Double = x(height - 1)(j)
            num6 += 1.7658221524 * x(height - 2)(j)
            For i As Integer = 2 To height - 2 Step 2
                Dim num7 As Double = x(i)(j)
                num7 += 0.4435068522 * (x(i - 1)(j) + x(i + 1)(j))
            Next
            Dim num8 As Double = x(0)(j)
            num8 += 0.8870137044 * x(1)(j)
        Next
        Dim tempbank As Double(,) = New Double(width - 1, height - 1) {}
        For i As Integer = 0 To height - 1
            For j As Integer = 0 To width - 1
                If (i Mod 2) = 0 Then
                    tempbank(j, i \ 2) = 0.869864452275695 * x(i)(j)
                Else
                    tempbank(j, (i \ 2) + (height \ 2)) = 0.574802199 * x(i)(j)
                End If
            Next
        Next
        For i As Integer = 0 To width - 1
            For j As Integer = 0 To width - 1
                x(i)(j) = tempbank(i, j)
            Next
        Next
        Return x
    End Function

    Public Sub Decompose(samples As Double()()) Implements IDecompose.Decompose
        Dim rows As Integer = samples.Length
        Dim cols As Integer = samples(0).Length

        'Debug.Assert(rows >= cols, String.Format("Cannot Haar decompose. Rows ({0}) must be >= Cols ({1})", rows, cols))

        For row As Integer = 0 To rows - 1
            'Decompose each row
            Decompose(samples(row))
        Next

        For col As Integer = 0 To cols - 1
            'Decompose each column
            Dim column As Double() = New Double(rows - 1) {}
            'Length of each column is equal to number of rows
            For row As Integer = 0 To rows - 1
                column(row) = samples(row)(col)
            Next
            'Copying Column vector
            Decompose(column)
            For row As Integer = 0 To rows - 1
                samples(row)(col) = column(row)
            Next
        Next
    End Sub

    Public Shared Sub Decompose(samples As Double())
        Dim n As Integer = samples.Length
        For i As Integer = 1 To (n - 2) - 1 Step 2
            samples(i) += -1.586134342 * (samples(i - 1) + samples(i + 1))
        Next
        samples(n - 1) += -3.172268684 * samples(n - 2)
        For i As Integer = 2 To n - 2 Step 2
            samples(i) += -0.05298011854 * (samples(i - 1) + samples(i + 1))
        Next
        samples(0) += -0.10596023708 * samples(1)
        For i As Integer = 1 To (n - 2) - 1 Step 2
            samples(i) += 0.8829110762 * (samples(i - 1) + samples(i + 1))
        Next
        samples(n - 1) += 1.7658221524 * samples(n - 2)
        For i As Integer = 2 To n - 2 Step 2
            samples(i) += 0.4435068522 * (samples(i - 1) + samples(i + 1))
        Next
        samples(0) += 0.8870137044 * samples(1)
        For i As Integer = 0 To n - 1
            If (i Mod 2) <> 0 Then
                samples(i) *= 0.869864452275695
            Else
                samples(i) /= 0.869864452275695
            End If
        Next
        Dim tempbank As Double() = New Double(n - 1) {}
        For i As Integer = 0 To n - 1
            If (i Mod 2) = 0 Then
                tempbank(i \ 2) = samples(i)
            Else
                tempbank((n \ 2) + (i \ 2)) = samples(i)
            End If
        Next
        For i As Integer = 0 To n - 1
            samples(i) = tempbank(i)
        Next
    End Sub

    Public Shared ReadOnly Property Instance As IDecompose
        Get
            Return _Instance
        End Get
    End Property

End Class
