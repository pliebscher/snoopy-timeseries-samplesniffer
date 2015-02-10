''' <summary>
''' Source: Accord framework.
''' </summary>
''' <remarks></remarks>
<Serializable>
Public Class DTWAccord

    ' Spherical projection distance
    Private m_alpha As Double = 1.0
    ' Length of the feature vectors
    Private m_length As Integer = 1
    ' Polynomial kernel degree
    Private m_degree As Integer = 1

    ''' <summary>
    '''   Gets or sets the length for the feature vectors
    '''   contained in each sequence used by the kernel.
    ''' </summary>
    '''
    Public Property Length() As Integer
        Get
            Return m_length
        End Get
        Set(value As Integer)
            m_length = value
        End Set
    End Property

    ''' <summary>
    '''   Gets or sets the hypersphere ratio.
    ''' </summary>
    '''
    Public Property Alpha() As Double
        Get
            Return m_alpha
        End Get
        Set(value As Double)
            m_alpha = value
        End Set
    End Property

    ''' <summary>
    '''   Gets or sets the polynomial degree for this kernel.
    ''' </summary>
    '''
    Public Property Degree() As Integer
        Get
            Return m_degree
        End Get
        Set(value As Integer)
            m_degree = value
        End Set
    End Property

    ''' <summary>
    '''   Constructs a new Dynamic Time Warping kernel.
    ''' </summary>
    '''
    ''' <param name="length">
    '''    The length of the feature vectors
    '''    contained in each sequence.
    ''' </param>
    '''
    Public Sub New(length As Integer)
        Me.m_length = length
    End Sub

    ''' <summary>
    '''   Constructs a new Dynamic Time Warping kernel.
    ''' </summary>
    '''
    ''' <param name="length">
    '''    The length of the feature vectors
    '''    contained in each sequence.
    ''' </param>
    '''
    ''' <param name="alpha">
    '''    The hypersphere ratio. Default value is 1.
    ''' </param>
    '''
    Public Sub New(length As Integer, alpha As Double)
        Me.m_length = length
        Me.m_alpha = alpha
    End Sub

    ''' <summary>
    '''   Constructs a new Dynamic Time Warping kernel.
    ''' </summary>
    '''
    ''' <param name="length">
    '''    The length of the feature vectors
    '''    contained in each sequence.
    ''' </param>
    '''
    ''' <param name="alpha">
    '''    The hypersphere ratio. Default value is 1.
    ''' </param>
    '''
    ''' <param name="degree">
    '''    The degree of the kernel. Default value is 1 (linear kernel).
    ''' </param>
    '''
    Public Sub New(length As Integer, alpha As Double, degree As Integer)
        Me.m_alpha = alpha
        Me.m_degree = degree
        Me.m_length = length
    End Sub

    ''' <summary>
    '''   Dynamic Time Warping kernel function.
    ''' </summary>
    '''
    ''' <param name="x">Vector <c>x</c> in input space.</param>
    ''' <param name="y">Vector <c>y</c> in input space.</param>
    ''' <returns>Dot product in feature (kernel) space.</returns>
    '''
    Public Function Compute(x As Double(), y As Double()) As Double
        If x Is y Then
            Return 1.0
        End If

        ' Compute the cosine of the global distance
        Dim distance As Double = LocalDistance(snorm(x), snorm(y))
        Dim cos As Double = System.Math.Cos(distance)

        ' Return cos for the linear kernel, cos^n for polynomial
        Return If((m_degree = 1), cos, System.Math.Pow(cos, m_degree))
    End Function

    ''' <summary>
    '''   Global distance D(X,Y) between two sequences of vectors.
    ''' </summary>
    '''
    ''' <param name="X">A sequence of vectors.</param>
    ''' <param name="Y">A sequence of vectors.</param>
    '''
    ''' <returns>The global distance between X and Y.</returns>
    '''
    Private Function LocalDistance(X As Double(), Y As Double()) As Double
        ' Get the number of vectors in each sequence. The vectors
        ' have been projected, so the length is augmented by one.
        Dim n As Integer = X.Length \ (m_length + 1)
        Dim m As Integer = Y.Length \ (m_length + 1)

        ' Application of the Dynamic Time Warping
        ' algorithm by using dynamic programming.
        Dim DTW As Double(,) = New Double(n, m) {}

        For i As Integer = 1 To n
            DTW(i, 0) = Double.PositiveInfinity
        Next

        For i As Integer = 1 To m
            DTW(0, i) = Double.PositiveInfinity
        Next

        For i As Integer = 1 To n
            For j As Integer = 1 To m
                Dim cost As Double = d(X, i - 1, Y, j - 1, m_length + 1)
                ' insertion
                ' deletion
                ' match
                DTW(i, j) = cost + Math.Min(Math.Min(DTW(i - 1, j), DTW(i, j - 1)), DTW(i - 1, j - 1))
            Next
        Next

        Return DTW(n, m)
        ' return the minimum global distance
    End Function

    ''' <summary>
    '''   Local distance d(x,y) between two vectors.
    ''' </summary>
    '''
    ''' <param name="X">A sequence of fixed-length vectors X.</param>
    ''' <param name="Y">A sequence of fixed-length vectors Y.</param>
    ''' <param name="ix">The index of the vector in the sequence x.</param>
    ''' <param name="iy">The index of the vector in the sequence y.</param>
    ''' <param name="length">The fixed-length of the vectors in the sequences.</param>
    '''
    ''' <returns>The local distance between x and y.</returns>
    '''
    Private Shared Function d(X As Double(), ix As Integer, Y As Double(), iy As Integer, length As Integer) As Double
        Dim p As Double = 0
        ' the product <x,y>
        ' Get the vectors' starting positions in the sequences
        Dim i As Integer = ix * length
        Dim j As Integer = iy * length

        ' Compute the inner product between the vectors
        For k As Integer = 0 To length - 1
            ' p += X(System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)) * Y(System.Math.Max(System.Threading.Interlocked.Increment(j), j - 1))
            ' p += X[i++] * Y[j++]
            p += X(i) * Y(j)
            i += 1 : j += 1
        Next

        ' Assert the value is in the [-1;+1] range
        If p > +1.0 Then
            p = +1.0
        ElseIf p < -1.0 Then
            p = -1.0
        End If

        ' Return the arc-cosine of the inner product
        Return Math.Acos(p)
    End Function

    ''' <summary>
    '''   Projects vectors from a sequence of vectors into
    '''   a hypersphere, augmenting their size in one unit
    '''   and normalizing them to be unit vectors.
    ''' </summary>
    '''
    ''' <param name="x">A sequence of vectors.</param>
    '''
    ''' <returns>A sequence of vector projections.</returns>
    '''
    Private Function snorm(x As Double()) As Double()
        ' Get the number of vectors in the sequence
        Dim n As Integer = x.Length \ m_length

        ' Create the augmented sequence projection
        Dim xs As Double() = New Double(x.Length + (n - 1)) {}

        ' For each vector in the sequence
        For j As Integer = 0 To n - 1
            ' Compute its starting position in the
            '  source and destination sequences
            Dim src As Integer = j * m_length
            Dim dst As Integer = j * (m_length + 1)

            ' Compute augmented vector norm
            Dim norm As Double = m_alpha * m_alpha
            For k As Integer = src To src + (m_length - 1)
                norm += x(k) * x(k)
            Next
            norm = System.Math.Sqrt(norm)

            ' Normalize the augmented vector and
            '  copy to the destination sequence
            xs(dst + m_length) = m_alpha / norm
            Dim i As Integer = dst
            While i < dst + m_length
                xs(i) = x(src) / norm
                i += 1
                src += 1
            End While
        Next

        Return xs
        ' return the projected sequence
        ' Remarks: the above could be done much more
        ' efficiently using unsafe pointer arithmetic.
    End Function

End Class
