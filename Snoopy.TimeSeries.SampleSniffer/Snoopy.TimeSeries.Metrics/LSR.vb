''' <summary>
''' Least Squares Regression for Quadratic Curve Fitting
''' http://www.codeproject.com/Articles/63170/Least-Squares-Regression-for-Quadratic-Curve-Fitti
''' </summary>
''' <remarks></remarks>
Public Class LSR
    Inherits Metric

    Public a As Double
    Public b As Double
    Public c As Double
    Public R As Double

    Protected Overrides Function OnCompute(v1() As Double, v2() As Double) As Double
        Return Calculate(v1, v2).R
    End Function

    Public Shared Function Calculate(vectorX As IEnumerable(Of Double), vectorY As IEnumerable(Of Double)) As LSR

        Dim n As Integer = 0
        Dim sx As Double = 0, sy As Double = 0, sx2 As Double = 0, sx3 As Double = 0, sx4 As Double = 0, sxy As Double = 0, sx2y As Double = 0
        Dim lsr As New LSR

        For Each item As Pair(Of Double) In vectorX.Zip(vectorY, Function(x, y) New Pair(Of Double)(x, y))
            n += 1
            sx += item.x
            sx2 += item.x * item.x
            sx3 += item.x * item.x * item.x
            sx4 += item.x * item.x * item.x * item.x
            sy += item.y
            sxy += item.x * item.y
            sx2y += item.x * item.x * item.y
        Next

        If n < 3 Then
            Throw New ArgumentException("Dimension of data set have to be 3 or more.")
        End If

        Dim d As Double = (sx4 * (sx2 * n - sx * sx) - sx3 * (sx3 * n - sx * sx2) + sx2 * (sx3 * sx - sx2 * sx2))

        lsr.c = (sx2y * (sx2 * n - sx * sx) - sxy * (sx3 * n - sx * sx2) + sy * (sx3 * sx - sx2 * sx2)) / d
        lsr.b = (sx4 * (sxy * n - sy * sx) - sx3 * (sx2y * n - sy * sx2) + sx2 * (sx2y * sx - sxy * sx2)) / d
        lsr.a = (sx4 * (sx2 * sy - sx * sxy) - sx3 * (sx3 * sy - sx * sx2y) + sx2 * (sx3 * sxy - sx2 * sx2y)) / d

        Dim ssErr As Double = 0, ssTot As Double = 0, yMean As Double = sy / CDbl(n)
        For Each item As Pair(Of Double) In vectorY.Zip(vectorY, Function(x, y) New Pair(Of Double)(x, y))
            Dim err As Double = item.y - (lsr.a + lsr.b * item.x + lsr.c * item.x * item.x)
            ssErr += err * err
            Dim dif As Double = item.y - yMean
            ssTot += dif * dif
        Next

        lsr.R = Math.Sqrt(1.0 - ssErr / ssTot)

        If Double.IsNaN(lsr.R) Then lsr.R = 0

        Return lsr

    End Function

    Public Shared Function CalculateFitMatrix(a As Double()(), b As Double()()) As Double()()
        Dim aLen As Integer = a.Length
        Dim bLen As Integer = b.Length
        Dim dist As Double()() = New Double(aLen - 1)() {}

        For i As Integer = 0 To aLen - 1
            dist(i) = New Double(bLen - 1) {}
            For j As Integer = 0 To bLen - 1
                dist(i)(j) = Calculate(a(i), b(j)).R
            Next
        Next

        Return dist
    End Function

    Private Structure Pair(Of TItem)
        Public x As TItem
        Public y As TItem
        Public Sub New(x As TItem, y As TItem)
            Me.x = x
            Me.y = y
        End Sub
    End Structure

    Public Overrides Function ToString() As String
        Return String.Format("a: {0}, b: {1}, c: {2}, R: {3}", a, b, c, R)
    End Function

End Class
