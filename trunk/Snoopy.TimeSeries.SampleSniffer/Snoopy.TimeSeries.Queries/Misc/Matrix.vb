
Public Class Matrix

    Private _Cols As Integer = 0
    Private _Rows As Integer = 0
    Private _Data As Double()

    Public Sub New()
        Data = Nothing
    End Sub

    Public Sub New(cols As Integer, rows As Integer)
        Resize(cols, rows)
    End Sub

    Public Sub New(m As Matrix)
        [Set](m)
    End Sub

    Public Sub [Set](m As Matrix)

        Resize(m.Columns, m.Rows)
        For i As Integer = 0 To m.Data.Length - 1
            Data(i) = m.Data(i)
        Next
    End Sub

    Public Property Columns() As Integer
        Get
            Return _Cols
        End Get
        Set(value As Integer)
            Resize(value, _Rows)
        End Set
    End Property

    Public Property Rows() As Integer
        Get
            Return _Rows
        End Get
        Set(value As Integer)
            Resize(_Cols, value)
        End Set
    End Property

    Public Sub Resize(cols As Integer, rows As Integer)
        If (_Cols = cols) AndAlso (_Rows = rows) Then
            Return
        End If
        _Cols = cols
        _Rows = rows
        Data = New Double(cols * rows - 1) {}
        Zero()
    End Sub

    Public Function Clone() As Matrix
        Dim m As New Matrix()
        m.Resize(Me.Columns, Me.Rows)
        For i As Integer = 0 To Data.Length - 1
            m.Data(i) = Data(i)
        Next
        Return m
    End Function

    Public Function [Get](x As Integer, y As Integer) As Double
        Return Data(x + y * _Cols)
    End Function

    Public Function Trace(index As Integer) As Double
        Return [Get](index, index)
    End Function

    Public Sub [Set](x As Integer, y As Integer, v As Double)
        Data(x + (y * _Cols)) = v
    End Sub

    Public Property Data() As Double()
        Get
            Return _Data
        End Get
        Set(value As Double())
            _Data = value
        End Set
    End Property

    Public Sub Multiply(scalar As Double)
        For i As Integer = 0 To Data.Length - 1
            Data(i) *= scalar
        Next
    End Sub

    Public Shared Function Multiply(m As Matrix, scalar As Double) As Matrix
        Dim rv As Matrix = m.Clone()
        rv.Multiply(scalar)
        Return rv
    End Function

    Public Shared Function Multiply(a As Matrix, b As Matrix) As Matrix
        Dim rv As New Matrix(b.Columns, a.Rows)
        Dim min As Integer = If(a.Columns < b.Rows, a.Columns, b.Rows)
        For i As Integer = 0 To a.Rows - 1
            For j As Integer = 0 To b.Columns - 1
                Dim s As Double = 0
                For k As Integer = 0 To min - 1
                    Dim av As Double = a.[Get](k, i)
                    Dim bv As Double = b.[Get](j, k)
                    s += av * bv
                Next
                rv.[Set](j, i, s)
            Next
        Next
        Return rv
    End Function

    Public Sub Multiply(b As Matrix)
        Dim tmp As Matrix = Matrix.Multiply(Me, b)
        Me.[Set](tmp)
    End Sub

    Public Shared Function MultiplyABAT(a As Matrix, b As Matrix) As Matrix
        Dim rv As Matrix = Multiply(a, b)
        Dim t As Matrix = Matrix.Transpose(a)
        rv.Multiply(t)
        Return rv
    End Function

    Public Shared Function Add(a As Matrix, scalar As Double) As Matrix
        Dim rv As New Matrix(a)
        rv.Add(scalar)
        Return rv
    End Function

    Public Sub Add(scalar As Double)
        For i As Integer = 0 To Data.Length - 1
            Data(i) += scalar
        Next
    End Sub

    Public Shared Function Add(a As Matrix, b As Matrix) As Matrix
        Dim rv As New Matrix(a)
        rv.Add(b)
        Return rv
    End Function

    Public Sub Add(a As Matrix)
        For i As Integer = 0 To Data.Length - 1
            Data(i) += a.Data(i)
        Next
    End Sub

    Public Shared Function Subtract(a As Matrix, scalar As Double) As Matrix
        Dim rv As New Matrix(a)
        rv.Subtract(scalar)
        Return rv
    End Function

    Public Sub Subtract(scalar As Double)
        For i As Integer = 0 To Data.Length - 1
            Data(i) -= scalar
        Next
    End Sub

    Public Shared Function Subtract(a As Matrix, b As Matrix) As Matrix
        Dim rv As New Matrix(a)
        rv.Subtract(b)
        Return rv
    End Function

    Public Sub Subtract(a As Matrix)
        For i As Integer = 0 To Data.Length - 1
            Data(i) -= a.Data(i)
        Next
    End Sub

    Public Shared Function Transpose(m As Matrix) As Matrix
        Dim rv As New Matrix(m._Rows, m._Cols)
        For i As Integer = 0 To m._Cols - 1
            For j As Integer = 0 To m._Rows - 1
                rv.[Set](j, i, m.[Get](i, j))
            Next
        Next
        Return rv
    End Function

    Public Sub Transpose()
        Dim rv As New Matrix(Me._Rows, Me._Cols)
        For i As Integer = 0 To _Cols - 1
            For j As Integer = 0 To _Rows - 1
                rv.[Set](j, i, Me.[Get](i, j))
            Next
        Next
        Me.[Set](rv)
    End Sub

    Public Function IsIdentity() As Boolean
        If _Cols <> _Rows Then
            Return False
        End If
        Dim check As Integer = _Cols + 1
        Dim j As Integer = 0
        For i As Integer = 0 To Data.Length - 1
            If j = check Then
                j = 0
                If Data(i) <> 1 Then
                    Return False
                End If
            Else
                If Data(i) <> 0 Then
                    Return False
                End If
            End If
            j += 1
        Next
        Return True
    End Function

    Public Sub SetIdentity()
        If _Cols <> _Rows Then
            Return
        End If
        Dim check As Integer = _Cols + 1
        Dim j As Integer = 0
        For i As Integer = 0 To Data.Length - 1
            Data(i) = If((j = check), 1, 0)
            j = If(j = check, 1, j + 1)
        Next
    End Sub

    Public Sub Zero()
        For i As Integer = 0 To Data.Length - 1
            Data(i) = 0
        Next
    End Sub

    Public ReadOnly Property Determinant() As Double
        Get
            If _Cols <> _Rows Then
                Return 0
            End If

            If _Cols = 0 Then
                Return 0
            End If
            If _Cols = 1 Then
                Return Data(0)
            End If
            If _Cols = 2 Then
                Return (Data(0) * Data(3)) - (Data(1) * Data(2))
            End If
            If _Cols = 3 Then
                Return (Data(0) * ((Data(8) * Data(4)) - (Data(7) * Data(5)))) - (Data(3) * ((Data(8) * Data(1)) - (Data(7) * Data(2)))) + (Data(6) * ((Data(5) * Data(1)) - (Data(4) * Data(2))))
            End If

            ' only supporting 1x1, 2x2 and 3x3
            Return 0
        End Get
    End Property

    Public Shared Function Invert(m As Matrix) As Matrix
        If m._Cols <> m._Rows Then
            Return Nothing
        End If
        Dim det As Double = m.Determinant
        If det = 0 Then
            Return Nothing
        End If

        Dim rv As New Matrix(m)
        If m._Cols = 1 Then
            rv.Data(0) = 1 / rv.Data(0)
        End If
        det = 1 / det
        If m._Cols = 2 Then
            rv.Data(0) = det * m.Data(3)
            rv.Data(3) = det * m.Data(0)
            rv.Data(1) = -det * m.Data(2)
            rv.Data(2) = -det * m.Data(1)
        End If
        If m._Cols = 3 Then
            rv.Data(0) = det * (m.Data(8) * m.Data(4)) - (m.Data(7) * m.Data(5))
            rv.Data(1) = -det * (m.Data(8) * m.Data(1)) - (m.Data(7) * m.Data(2))
            rv.Data(2) = det * (m.Data(5) * m.Data(1)) - (m.Data(4) * m.Data(2))

            rv.Data(3) = -det * (m.Data(8) * m.Data(3)) - (m.Data(6) * m.Data(5))
            rv.Data(4) = det * (m.Data(8) * m.Data(0)) - (m.Data(6) * m.Data(2))
            rv.Data(5) = -det * (m.Data(5) * m.Data(0)) - (m.Data(3) * m.Data(2))

            rv.Data(6) = det * (m.Data(7) * m.Data(3)) - (m.Data(6) * m.Data(4))
            rv.Data(7) = -det * (m.Data(7) * m.Data(0)) - (m.Data(6) * m.Data(2))
            rv.Data(8) = det * (m.Data(4) * m.Data(0)) - (m.Data(3) * m.Data(1))
        End If
        Return rv
    End Function

    Public Shared Function Reshape(samples As Double()(), n As Integer, m As Integer) As Double()()
        Dim cEl As Integer = 0
        Dim aRows As Integer = samples.Length
        Dim mat As Double()() = New Double(n - 1)() {}
        For i As Integer = 0 To n - 1
            mat(i) = New Double(m - 1) {}
        Next
        For j As Integer = 0 To m - 1
            For i As Integer = 0 To n - 1
                Dim r As Integer = cEl Mod aRows
                mat(i)(j) = samples(r)(cEl \ aRows)
                cEl += 1
            Next
        Next
        Return mat
    End Function

    Public Shared Function Create(samples As Double(), rows As Integer, columns As Integer) As Double()()
        Dim mat As Double()() = New Double(rows - 1)() {}
        For i As Integer = 0 To rows - 1
            mat(i) = New Double(columns - 1) {}
            For j As Integer = 0 To columns - 1
                mat(i)(j) = samples(i * columns + j)
            Next
        Next
        Return mat
    End Function

    Public Shared Function GetColumnMeans(matrix As Double()()) As Double()
        Dim means As Double() = New Double(matrix(0).Length - 1) {}
        For j As Integer = 0 To matrix(0).Length - 1
            Dim sum As Double
            For i As Integer = 0 To matrix.Length - 1
                sum += matrix(i)(j)
            Next
            means(j) = sum / matrix.Length
        Next
        Return means
    End Function

End Class
