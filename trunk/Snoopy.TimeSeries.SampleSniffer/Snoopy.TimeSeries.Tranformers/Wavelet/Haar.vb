Public Class Haar
    Inherits Wavelet
    Implements IDecompose

    Private Shared ReadOnly _Instance As Haar = New Haar

    Private _coeffs As Double()
    Private _scales As Double()

    Protected Overrides Function GetCoefficients() As Double()
        _coeffs = New Double(2) {}
        _coeffs(0) = 1
        _coeffs(1) = -1
        Return _coeffs
    End Function

    Protected Overrides Function GetScales() As Double()
        _scales = New Double(2) {}
        _scales(0) = 1
        _scales(1) = 0
        Return _scales
    End Function

    Public Shared Sub FWT(ByVal data As Double())
        Dim temp As Double() = New Double(data.Length - 1) {}
        Dim h As Integer = (data.Length >> 1)

        For i As Integer = 0 To h - 1
            Dim k As Integer = (i << 1)
            temp(i) = (data(k) * 0.5) + (data((k + 1)) * 0.5)
            temp((i + h)) = (data(k) * 0.5) + (data((k + 1)) * -0.5)
        Next i

        For i As Integer = 0 To data.Length - 1
            data(i) = temp(i)
        Next i
    End Sub

    ''' <summary>
    '''   The standard 2-dimensional Haar wavelet decomposition involves one-dimensional decomposition of each row
    '''   followed by a one-dimensional decomposition of each column of the result.
    ''' </summary>
    ''' <param name = "samples">Samples to be decomposed</param>
    Public Sub DecomposeInPlace(ByVal samples As Double()()) Implements IDecompose.Decompose
        Dim rows As Integer = samples.GetLength(0)
        Dim cols As Integer = samples(0).Length

        'Debug.Assert(rows >= cols, String.Format("Cannot Haar decompose. Rows ({0}) must be >= Cols ({1})", rows, cols))

        For row As Integer = 0 To rows - 1
            'Decomposition of each row
            DecomposeArray(samples(row))
        Next

        For col As Integer = 0 To cols - 1
            'Decomposition of each column
            Dim column As Double() = New Double(rows - 1) {}
            'Length of each column is equal to number of rows
            For row As Integer = 0 To rows - 1
                column(row) = samples(row)(col)
            Next
            'Copying Column vector
            DecomposeArray(column)
            'FWT(column)
            For row As Integer = 0 To rows - 1
                samples(row)(col) = column(row)
            Next
        Next
    End Sub

    Private Shared Sub DecomposeArray(ByVal array As Double())

        Dim h As Integer = array.Length
        Dim temp As Double() = New Double(h - 1) {}
        Dim temp1 As Double() = New Double(h - 1) {}

        While h > 1
            h \= 2 'CInt(h / 2) 'h /= 2
            For i As Integer = 0 To h - 1
                temp(i) = (array(2 * i) + array(2 * i + 1)) / Math.Sqrt(2)
                temp(h + i) = (array(2 * i) - array(2 * i + 1)) / Math.Sqrt(2)
            Next
            For i As Integer = 0 To 2 * h - 1
                array(i) = temp(i)
            Next
        End While

    End Sub

    Public Shared Function Decompose(ByVal samples As Double()()) As Double()()
        Dim decomp As Double()() = New Double(samples.Length - 1)() {}
        For i As Integer = 0 To samples.Length - 1
            For j As Integer = 0 To samples(i).Length - 1
                decomp(i) = New Double(samples(i).Length - 1) {}
                Array.Copy(samples(i), decomp(i), samples(i).Length)
            Next
        Next
        _Instance.DecomposeInPlace(decomp)
        Return decomp
    End Function

    Public Shared ReadOnly Property Instance As IDecompose
        Get
            Return _Instance
        End Get
    End Property

End Class
