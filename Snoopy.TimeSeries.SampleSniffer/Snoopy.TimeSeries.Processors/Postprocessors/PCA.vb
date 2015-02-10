''' <summary>
''' StatLib ?? Ardour3-3.3 ??
''' </summary>
''' <remarks></remarks>
Public Class PCA
    Inherits TimeSeriesPostProcessor

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        pca_project(series.Samples, series.Samples.Length, series.Samples(0).Length, _Components)

        Return series.Samples
    End Function

    ''' <summary>
    ''' Create m * m covariance matrix from given n * m data matrix. 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="n"></param>
    ''' <param name="m"></param>
    ''' <param name="symmat"></param>
    ''' <remarks></remarks>
    Private Sub covcol(data As Double()(), n As Integer, m As Integer, symmat As Double()())

        Dim mean As Double() = New Double(m - 1) {}
        Dim i As Integer
        Dim j As Integer
        Dim j1 As Integer
        Dim j2 As Integer

        ' mean = (double*) malloc(m*sizeof(double));

        ' Determine mean of column vectors of input data matrix 
        For j = 0 To m - 1
            mean(j) = 0.0
            For i = 0 To n - 1
                mean(j) += data(i)(j)
            Next
            mean(j) /= CDbl(n)
        Next

        ' Center the column vectors. 
        For i = 0 To n - 1
            For j = 0 To m - 1
                data(i)(j) -= mean(j)
            Next
        Next

        ' Calculate the m * m covariance matrix. 
        For j1 = 0 To m - 1
            For j2 = j1 To m - 1
                symmat(j1)(j2) = 0.0
                For i = 0 To n - 1
                    symmat(j1)(j2) += data(i)(j1) * data(i)(j2)
                Next
                symmat(j2)(j1) = symmat(j1)(j2)
            Next
        Next

    End Sub

    ''' <summary>
    ''' Reduce a real, symmetric matrix to a symmetric, tridiag. matrix.
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="n"></param>
    ''' <param name="d"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub tred2(a As Double()(), n As Integer, d As Double(), e As Double())

        Dim l As Integer
        Dim k As Integer
        Dim j As Integer
        Dim i As Integer
        Dim scale As Double
        Dim hh As Double
        Dim h As Double
        Dim g As Double
        Dim f As Double

        For i = n - 1 To 1 Step -1
            l = i - 1
            h = InlineAssignHelper(scale, 0.0)
            If l > 0 Then
                For k = 0 To l
                    scale += Math.Abs(a(i)(k))
                Next
                If scale = 0.0 Then
                    e(i) = a(i)(l)
                Else
                    For k = 0 To l
                        a(i)(k) /= scale
                        h += a(i)(k) * a(i)(k)
                    Next
                    f = a(i)(l)
                    g = If(f > 0, -Math.Sqrt(h), Math.Sqrt(h))
                    e(i) = scale * g
                    h -= f * g
                    a(i)(l) = f - g
                    f = 0.0
                    For j = 0 To l
                        a(j)(i) = a(i)(j) / h
                        g = 0.0
                        For k = 0 To j
                            g += a(j)(k) * a(i)(k)
                        Next
                        For k = j + 1 To l
                            g += a(k)(j) * a(i)(k)
                        Next
                        e(j) = g / h
                        f += e(j) * a(i)(j)
                    Next
                    hh = f / (h + h)
                    For j = 0 To l
                        f = a(i)(j)
                        e(j) = InlineAssignHelper(g, e(j) - hh * f)
                        For k = 0 To j
                            a(j)(k) -= (f * e(k) + g * a(i)(k))
                        Next
                    Next
                End If
            Else
                e(i) = a(i)(l)
            End If
            d(i) = h
        Next

        d(0) = 0.0
        e(0) = 0.0
        For i = 0 To n - 1
            l = i - 1
            If d(i) > 0.0 Then ' if (d[i])
                For j = 0 To l
                    g = 0.0
                    For k = 0 To l
                        g += a(i)(k) * a(k)(j)
                    Next
                    For k = 0 To l
                        a(k)(j) -= g * a(k)(i)
                    Next
                Next
            End If
            d(i) = a(i)(i)
            a(i)(i) = 1.0
            For j = 0 To l
                a(j)(i) = InlineAssignHelper(a(i)(j), 0.0)
            Next
        Next

    End Sub

    ''' <summary>
    ''' Tridiagonal QL algorithm -- Implicit
    ''' </summary>
    ''' <param name="d"></param>
    ''' <param name="e"></param>
    ''' <param name="n"></param>
    ''' <param name="z"></param>
    ''' <remarks></remarks>
    Private Sub tqli(d As Double(), e As Double(), n As Integer, z As Double()())

        Dim m As Integer, l As Integer, iter As Integer, i As Integer, k As Integer
        Dim s As Double, r As Double, p As Double, g As Double, f As Double, dd As Double, _
            c As Double, b As Double

        For i = 1 To n - 1
            e(i - 1) = e(i)
        Next
        e(n - 1) = 0.0
        For l = 0 To n - 1
            iter = 0
            Do
                For m = l To n - 2
                    dd = Math.Abs(d(m)) + Math.Abs(d(m + 1))
                    If Math.Abs(e(m)) + dd = dd Then
                        Exit For
                    End If
                Next
                If m <> l Then

                    iter += 1
                    If iter >= 30 Then
                        Throw New TimeSeriesProcessorException("PCA not converging in TQLI!", Me)
                    End If

                    g = (d(l + 1) - d(l)) / (2.0 * e(l))
                    r = Math.Sqrt((g * g) + 1.0)
                    g = d(m) - d(l) + e(l) / (g + SIGN(r, g))
                    s = InlineAssignHelper(c, 1.0)

                    p = 0.0
                    For i = m - 1 To l Step -1
                        f = s * e(i)
                        b = c * e(i)
                        If Math.Abs(f) >= Math.Abs(g) Then
                            c = g / f
                            r = Math.Sqrt((c * c) + 1.0)
                            e(i + 1) = f * r
                            c *= (InlineAssignHelper(s, 1.0 / r))
                        Else
                            s = f / g
                            r = Math.Sqrt((s * s) + 1.0)
                            e(i + 1) = g * r
                            s *= (InlineAssignHelper(c, 1.0 / r))
                        End If
                        g = d(i + 1) - p
                        r = (d(i) - g) * s + 2.0 * c * b
                        p = s * r
                        d(i + 1) = g + p
                        g = c * r - b
                        For k = 0 To n - 1
                            f = z(k)(i + 1)
                            z(k)(i + 1) = s * z(k)(i) + c * f
                            z(k)(i) = c * z(k)(i) - s * f
                        Next
                    Next
                    d(l) = d(l) - p
                    e(l) = g
                    e(m) = 0.0
                End If
            Loop While m <> l
        Next
    End Sub

    Private Shared Function SIGN(a As Double, b As Double) As Double
        ' #define SIGN(a, b) ( (b) < 0 ? -fabs(a) : fabs(a) )
        If b < 0 Then Return -Math.Abs(a) Else Return Math.Abs(a)
    End Function

    ''' <summary>
    ''' In place projection onto basis vectors 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="n"></param>
    ''' <param name="m"></param>
    ''' <param name="ncomponents"></param>
    ''' <remarks></remarks>
    Private Sub pca_project(data As Double()(), n As Integer, m As Integer, ncomponents As Integer)

        If _Components > m Then _Components = m

        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim k2 As Integer

        Dim symmat As Double()() = New Double(m - 1)() {}
        'Dim symmat2 As Double()()
        Dim evals As Double() = New Double(m - 1) {} ' Storage alloc. for vector of eigenvalues 
        Dim interm As Double() = New Double(m - 1) {} ' Storage alloc. for 'intermediate' vector

        For i = 0 To symmat.Length - 1
            symmat(i) = New Double(m - 1) {}
        Next

        covcol(data, n, m, symmat)
        tred2(symmat, m, evals, interm)
        tqli(evals, interm, m, symmat)

        ' evals now contains the eigenvalues, columns of symmat now contain the associated eigenvectors. 
        ' Form projections of row-points on prin. components. 
        ' Store in 'data', overwriting original data. 
        For i = 0 To n - 1
            For j = 0 To m - 1
                interm(j) = data(i)(j)
            Next
            ' data[i][j] will be overwritten 
            For k = 0 To ncomponents - 1
                data(i)(k) = 0.0
                For k2 = 0 To m - 1
                    data(i)(k) += interm(k2) * symmat(k2)(m - k - 1)
                Next
            Next
        Next

    End Sub

    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function

    Private _Components As Integer = 32

    Public Property Components As Integer
        Get
            Return _Components
        End Get
        Set(value As Integer)
            If value <> _Components Then
                _Components = value

            End If
        End Set
    End Property

End Class
