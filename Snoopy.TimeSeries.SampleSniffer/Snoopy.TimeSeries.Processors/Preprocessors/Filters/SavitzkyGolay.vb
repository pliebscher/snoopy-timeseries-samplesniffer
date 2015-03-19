Imports System.ComponentModel
''' <summary>
''' http://sourceforge.net/p/groovylab/code/HEAD/tree/trunk/GroovyLabSrc/com/nr/stat/SavitzkyGolayFilter.java#l13
''' </summary>
''' <remarks></remarks>
<Description("")>
Public Class SavitzkyGolay
    Inherits TimeSeriesPreprocessor

    Private _nl As Integer = 2
    Private _nr As Integer = 2
    Private _np As Integer = 6
    Private _ld As Integer = 2
    Private _m As Integer = 4

    Private _Decomp As LUDecomp
    Private _b As Double()

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        If _Decomp Is Nothing Then GenDecomp()
        Apply(series.Samples)
        Return series.Samples
    End Function

    '* Returns in c[0..np-1], in wraparound order (N.B.!) consistent with the
    '* argument respns in routine convlv, a set of Savitzky-Golay filter
    '* coefficients. nl is the number of leftward (past) data points used, while
    '* nr is the number of rightward (future) data points, making the total number
    '* of data points used nl+nr+1. ld is the order of the derivative desired
    '* (e.g., ld D 0 for smoothed function. For the derivative of order k, you
    '* must multiply the array c by k!) m is the order of the smoothing
    '* polynomial, also equal to the highest conserved moment; usual values are
    '* m=2 or m=4

    Public Sub Apply(c As Double())
        'Dim j As Integer
        Dim k As Integer
        'Dim imj As Integer
        'Dim ipj As Integer
        Dim kk As Integer
        Dim mm As Integer
        Dim fac As Double
        Dim sum As Double

        For kk = 0 To _np - 1
            c(kk) = 0.0
        Next

        For k = -_nl To _nr
            sum = _b(0)
            fac = 1.0
            For mm = 1 To _m
                Dim fack As Double
                fack *= k
                sum += _b(mm) * fack '(fac *= k)
            Next
            kk = (_np - k) Mod _np
            c(kk) = sum
        Next

    End Sub

    Private Sub GenDecomp()

        Dim j As Integer
        Dim k As Integer
        Dim imj As Integer
        Dim ipj As Integer
        Dim sum As Double
        Dim mm As Integer

        If _np < _nl + _nr + 1 Then Throw New ArgumentException(String.Format("NP must be greater than or equal to NL+NR+1 ({0})", _nl + _nr + 1))
        If _nl < 0 Then Throw New ArgumentException("NL must be greater than or equal to 0")
        If _nr < 0 Then Throw New ArgumentException("NR must be greater than or equal to 0")
        If _ld > _m Then Throw New ArgumentException("LD must be less than or equal to M")
        If _nl + _nr > _m Then Throw New ArgumentException(String.Format("NL+NR ({0}) must be greater than or equal to M ({1})", _nl + _nr, _m))

        Dim a As Double()() = New Double(_m)() {}

        For i As Integer = 0 To a.Length - 1
            a(i) = New Double(_m) {}
        Next

        For ipj = 0 To (_m << 1) '+ 1
            sum = (If(ipj <> 0, 0.0, 1.0))
            For k = 1 To _nr
                sum += Math.Pow(k, ipj)
            Next
            For k = 1 To _nl
                sum += Math.Pow(-k, ipj)
            Next
            mm = Math.Min(ipj, 2 * M - ipj)
            For imj = -mm To mm Step 2
                a((ipj + imj) \ 2)((ipj - imj) \ 2) = sum
            Next
        Next

        _Decomp = New LUDecomp(a)

        _b = New Double(_m) {}

        For j = 0 To M
            _b(j) = 0.0
        Next

        _b(_ld) = 1.0

        _Decomp.Solve(_b, _b)

    End Sub

    <Description("Number of Left points to use."), DefaultValue(2)>
    Public Property NL As Integer
        Get
            Return _nl
        End Get
        Set(value As Integer)
            If value <> _nl Then
                _nl = value
                GenDecomp()
            End If
        End Set
    End Property

    <Description("Number of Right points to use."), DefaultValue(2)>
    Public Property NR As Integer
        Get
            Return _nr
        End Get
        Set(value As Integer)
            If value <> _nr Then
                _nr = value
                GenDecomp()
            End If
        End Set
    End Property

    <Description(""), DefaultValue(6)>
    Public Property NP As Integer
        Get
            Return _np
        End Get
        Set(value As Integer)
            If value <> _np Then
                _np = value
                GenDecomp()
            End If
        End Set
    End Property

    <Description(""), DefaultValue(2)>
    Public Property LD As Integer
        Get
            Return _ld
        End Get
        Set(value As Integer)
            If value <> _ld Then
                _ld = value
                GenDecomp()
            End If
        End Set
    End Property

    <Description(""), DefaultValue(4)>
    Public Property M As Integer
        Get
            Return _m
        End Get
        Set(value As Integer)
            If value <> _m Then
                _m = value
                GenDecomp()
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "A Savitzky-Golay filter."
        End Get
    End Property

End Class

Public Class LUDecomp

    Private n As Integer '* Stores the decomposition
    Private lu As Double()() '* Stores the permutation.
    Private indx As Integer() '* Used by det.
    Private d As Double
    Private aref As Double()()

    '* Given a matrix a[0..n-1][0..n-1], this routine replaces it by the LU decomposition of a
    '* rowwise permutation of itself. a is input. On output, it is arranged as in equation (2.3.14)
    '* above; indx[0..n-1] is an output vector that records the row permutation effected by the
    '* partial pivoting; d is output as +/-1 depending on whether the number of row interchanges
    '* was even or odd, respectively. This routine is used in combination with solve to solve linear
    '* equations or invert a matrix.
    Public Sub New(a As Double()())
        n = a.Length
        lu = buildMatrix(a)
        aref = a
        indx = New Integer(n - 1) {}
        Dim TINY As Double = 1.0E-40
        Dim i As Integer, imax As Integer, j As Integer, k As Integer
        Dim big As Double, temp As Double
        Dim vv As Double() = New Double(n - 1) {}
        d = 1.0
        For i = 0 To n - 1
            big = 0.0
            For j = 0 To n - 1
                If (InlineAssignHelper(temp, Math.Abs(lu(i)(j)))) > big Then
                    big = temp
                End If
            Next
            If big = 0.0 Then
                Throw New ArgumentException("Singular matrix in LUDecomp!")
            End If
            vv(i) = 1.0 / big
        Next
        For k = 0 To n - 1
            big = 0.0
            imax = k
            For i = k To n - 1
                temp = vv(i) * Math.Abs(lu(i)(k))
                If temp > big Then
                    big = temp
                    imax = i
                End If
            Next
            If k <> imax Then
                For j = 0 To n - 1
                    temp = lu(imax)(j)
                    lu(imax)(j) = lu(k)(j)
                    lu(k)(j) = temp
                Next
                d = -d
                vv(imax) = vv(k)
            End If
            indx(k) = imax
            If lu(k)(k) = 0.0 Then
                lu(k)(k) = TINY
            End If
            For i = k + 1 To n - 1
                lu(i)(k) /= lu(k)(k)
                temp = lu(i)(k) '/= lu(k)(k)
                For j = k + 1 To n - 1
                    lu(i)(j) -= temp * lu(k)(j)
                Next
            Next
        Next
    End Sub

    Public Sub Solve(b As Double(), x As Double())
        Dim i As Integer, ii As Integer = 0, ip As Integer, j As Integer
        Dim sum As Double
        If b.length <> n OrElse x.length <> n Then
            Throw New ArgumentException("Solve bad sizes in LUDecomp!")
        End If
        For i = 0 To n - 1
            x(i) = b(i)
        Next
        For i = 0 To n - 1
            ip = indx(i)
            sum = x(ip)
            x(ip) = x(i)
            If ii <> 0 Then
                For j = ii - 1 To i - 1
                    sum -= lu(i)(j) * x(j)
                Next
            ElseIf sum <> 0.0 Then
                ii = i + 1
            End If
            x(i) = sum
        Next
        For i = n - 1 To 0 Step -1
            sum = x(i)
            For j = i + 1 To n - 1
                sum -= lu(i)(j) * x(j)
            Next
            x(i) = sum / lu(i)(i)
        Next
    End Sub

    Public Shared Function buildMatrix(b As Double()()) As Double()()
        Dim nn As Integer = b.length
        Dim mm As Integer = b(0).length
        'double[][] v = new double[nn][mm];
        Dim v As Double()() = New Double(nn)() {}
        For i As Integer = 0 To nn - 1
            v(i) = New Double(mm) {}
            For j As Integer = 0 To mm - 1
                v(i)(j) = b(i)(j)
            Next
        Next
        Return v
    End Function

    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function

End Class
