''' <summary>
''' http://www.cns.nyu.edu/pub/eero/wang03-reprint.pdf
''' Addapted from http://www.lomont.org/Software/Misc/SSIM/SSIM.html
''' </summary>
''' <remarks></remarks>
Public Class SSIM

    Private _WindowSigma As Double = 1.5
    Private _Window As Grid = Gaussian(11, _WindowSigma) ' Default = 11, 1.5

    Public Property K1 As Double = 0.0001 ' K1 << 1 is a small constant.
    Public Property K2 As Double = 0.0003 ' K2 << 1 is a small constant.
    Public Property L As Double = 255 ' The dynamic range of the pixel values (255 for 8-bit grayscale images).

    Public Sub New()
    End Sub

    Public Function Compute(frame1 As Double()(), frame2 As Double()()) As Double
        Return Compute(ConvertFrame(frame1), ConvertFrame(frame2))
    End Function

    ''' <summary>
    ''' Compute the SSIM index of two same sized Grids
    ''' </summary>
    ''' <param name="img1">The first Grid</param>
    ''' <param name="img2">The second Grid</param>
    ''' <returns>SSIM index</returns>
    Private Function Compute(img1 As Grid, img2 As Grid) As Double
        ' uses notation from paper
        ' automatic downsampling
        Dim f As Integer = CInt(Math.Truncate(Math.Max(1, Math.Round(Math.Min(img1.Width, img1.Height) / 256.0)))) ' TODO: Expose as property for manual control...
        If f > 1 Then
            ' downsampling by f
            ' use a simple low-pass filter and subsample by f
            img1 = SubSample(img1, f)
            img2 = SubSample(img2, f)
        End If

        ' normalize window - todo - do in window set {}
        Dim scale As Double = 1.0 / _Window.Total
        Grid.Op(Function(i, j) _Window(i, j) * scale, _Window)

        ' image statistics
        Dim mu1 As Grid = Filter(img1, _Window)
        Dim mu2 As Grid = Filter(img2, _Window)

        Dim mu1mu2 As Grid = mu1 * mu2
        Dim mu1SQ As Grid = mu1 * mu1
        Dim mu2SQ As Grid = mu2 * mu2

        Dim sigma12 As Grid = Filter(img1 * img2, _Window) - mu1mu2
        Dim sigma1SQ As Grid = Filter(img1 * img1, _Window) - mu1SQ
        Dim sigma2SQ As Grid = Filter(img2 * img2, _Window) - mu2SQ

        ' constants from the paper
        Dim C1 As Double = _K1 * _L
        C1 *= C1
        Dim C2 As Double = _K2 * _L
        C2 *= C2

        Dim ssim_map As Grid = Nothing
        If (C1 > 0) AndAlso (C2 > 0) Then
            ssim_map = Grid.Op(Function(i, j) (2 * mu1mu2(i, j) + C1) * (2 * sigma12(i, j) + C2) / (mu1SQ(i, j) + mu2SQ(i, j) + C1) / (sigma1SQ(i, j) + sigma2SQ(i, j) + C2), New Grid(mu1mu2.Width, mu1mu2.Height))
        Else
            Dim num1 As Grid = Linear(2, mu1mu2, C1)
            Dim num2 As Grid = Linear(2, sigma12, C2)
            Dim den1 As Grid = Linear(1, mu1SQ + mu2SQ, C1)
            Dim den2 As Grid = Linear(1, sigma1SQ + sigma2SQ, C2)

            Dim den As Grid = den1 * den2
            ' total denominator
            ssim_map = New Grid(mu1.Width, mu1.Height)
            For i As Integer = 0 To ssim_map.Width - 1
                For j As Integer = 0 To ssim_map.Height - 1
                    ssim_map(i, j) = 1
                    If den(i, j) > 0 Then
                        ssim_map(i, j) = num1(i, j) * num2(i, j) / (den1(i, j) * den2(i, j))
                    ElseIf (den1(i, j) <> 0) AndAlso (den2(i, j) = 0) Then
                        ssim_map(i, j) = num1(i, j) / den1(i, j)
                    End If
                Next
            Next
        End If

        ' average all values
        Return ssim_map.Total / (ssim_map.Width * ssim_map.Height)
    End Function

    ''' <summary>
    ''' Create a gaussian window of the given size and standard deviation
    ''' </summary>
    ''' <param name="size">Odd number</param>
    ''' <param name="sigma">Gaussian std deviation</param>
    ''' <returns></returns>
    Private Shared Function Gaussian(size As Integer, sigma As Double) As Grid

        Dim filter As Grid = New Grid(size, size)
        Dim s2 As Double = sigma * sigma
        Dim c As Double = (size - 1) / 2.0
        Dim dx As Double
        Dim dy As Double

        Grid.Op(Function(i, j)
                    dx = i - c
                    dy = j - c
                    Return Math.Exp(-(dx * dx + dy * dy) / (2 * s2))

                End Function, filter)
        Dim scale As Double = 1.0 / filter.Total
        Grid.Op(Function(i, j) filter(i, j) * scale, filter)
        Return filter
    End Function

    ''' <summary>
    ''' subsample a grid by step size, averaging each box into the result value
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function SubSample(img As Grid, skip As Integer) As Grid
        Dim w As Integer = img.Width
        Dim h As Integer = img.Height
        Dim scale As Double = 1.0 / (skip * skip)
        Dim ans As Grid = New Grid(w \ skip, h \ skip)
        Dim i As Integer = 0
        While i < w - skip
            Dim j As Integer = 0
            While j < h - skip
                Dim sum As Double = 0
                For x As Integer = i To i + (skip - 1)
                    For y As Integer = j To j + (skip - 1)
                        sum += img(x, y)
                    Next
                Next
                ans(i \ skip, j \ skip) = sum * scale
                j += skip
            End While
            i += skip
        End While
        Return ans
    End Function

    ''' <summary>
    ''' Apply filter, return only center part.
    ''' C = Filter(A,B) should be same as matlab filter2( ,'valid')
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function Filter(a As Grid, b As Grid) As Grid
#If False Then
			Dim ax As Integer = a.width, ay As Integer = a.height
			Dim bx As Integer = b.width, by As Integer = b.height
			Dim bcx As Integer = (bx + 1) \ 2, bcy As Integer = (by + 1) \ 2
			' center position
			Dim c = New Grid(ax - bx + 1, ay - by + 1)
			For i As Integer = bx - bcx + 1 To ax - bx - 1
				For j As Integer = by - bcy + 1 To ay - by - 1
					Dim sum As Double = 0
					For x As Integer = bcx - bx + 1 + i To 1 + i + (bcx - 1)
						For y As Integer = bcy - by + 1 + j To 1 + j + (bcy - 1)
							sum += a(x, y) * b(bx - bcx - 1 - i + x, by - bcy - 1 - j + y)
						Next
					Next
					c(i - bcx, j - bcy) = sum
				Next
			Next
			Return c
#Else
        ' todo? - check and clean this
        Dim ax As Integer = a.Width, ay As Integer = a.Height
        Dim bx As Integer = b.Width, by As Integer = b.Height
        Dim bcx As Integer = (bx + 1) \ 2, bcy As Integer = (by + 1) \ 2
        ' center position
        Dim c As Grid = New Grid(ax - bx + 1, ay - by + 1)
        For i As Integer = bx - bcx + 1 To ax - bx - 1
            For j As Integer = by - bcy + 1 To ay - by - 1
                Dim sum As Double = 0
                For x As Integer = bcx - bx + 1 + i To 1 + i + (bcx - 1)
                    For y As Integer = bcy - by + 1 + j To 1 + j + (bcy - 1)
                        sum += a(x, y) * b(bx - bcx - 1 - i + x, by - bcy - 1 - j + y)
                    Next
                Next
                c(i - bcx, j - bcy) = sum
            Next
        Next
        Return c
#End If
    End Function

    ''' <summary>
    ''' componentwise s*a[i,j]+c->a[i,j]
    ''' </summary>
    ''' <param name="s"></param>
    ''' <param name="a"></param>
    ''' <param name="c"></param>
    ''' <returns></returns>
    Private Shared Function Linear(s As Double, a As Grid, c As Double) As Grid
        Return Grid.Op(Function(i, j) s * a(i, j) + c, New Grid(a.Width, a.Height))
    End Function

    Private Shared Function ConvertFrame(frame As Double()()) As Grid
        Return Grid.Op(Function(i, j)
                           Return frame(i)(j)
                       End Function, New Grid(frame.Length, frame(0).Length))
    End Function

    Public Property WindowWidth As Integer
        Get
            Return _Window.Width
        End Get
        Set(value As Integer)
            If value < 3 Then Throw New ArgumentException("Invalid Window Width.")
            If value Mod 2 = 0 Then Throw New ArgumentException("Window Width must be an odd number.")
            If value <> _Window.Width Then
                _Window = Gaussian(value, 1.5)
            End If
        End Set
    End Property

    Public Property WindowSigma As Double
        Get
            Return _WindowSigma
        End Get
        Set(value As Double)
            If value <> _WindowSigma Then
                _WindowSigma = value
                _Window = Gaussian(_Window.Width, value)
            End If
        End Set
    End Property

#Region "Grid"

    ''' <summary>
    ''' Hold a grid of doubles as an array with appropriate operators
    ''' </summary>
    Private Structure Grid

        Private _Data As Double()()

        Friend Width As Integer
        Friend Height As Integer

        Friend Sub New(width As Integer, height As Integer)
            _Data = New Double(width - 1)() {}

            For i As Integer = 0 To width - 1
                _Data(i) = New Double(height - 1) {}
            Next

            Me.Width = width
            Me.Height = height
        End Sub

        Default Friend Property Item(i As Integer, j As Integer) As Double
            Get
                Return _Data(i)(j)
            End Get
            Set(value As Double)
                _Data(i)(j) = value
            End Set
        End Property

        Public ReadOnly Property Total() As Double
            Get
                Dim s As Double = 0
                For Each d As Double() In _Data
                    s += d.Sum
                Next
                Return s
            End Get
        End Property

        Public ReadOnly Property Data As Double()()
            Get
                Return _Data
            End Get
        End Property

        Public Shared Operator +(a As Grid, b As Grid) As Grid
            Return Op(Function(i, j) a(i, j) + b(i, j), New Grid(a.Width, a.Height))
        End Operator

        Public Shared Operator +(a As Grid, b As Integer) As Grid
            Return Op(Function(i, j) a(i, j) + b, New Grid(a.Width, a.Height))
        End Operator

        Public Shared Operator -(a As Grid, b As Grid) As Grid
            Return Op(Function(i, j) a(i, j) - b(i, j), New Grid(a.Width, a.Height))
        End Operator

        Public Shared Operator -(a As Grid, b As Integer) As Grid
            Return Op(Function(i, j) a(i, j) - b, New Grid(a.Width, a.Height))
        End Operator

        Public Shared Operator *(a As Grid, b As Grid) As Grid
            Return Op(Function(i, j) a(i, j) * b(i, j), New Grid(a.Width, a.Height))
        End Operator

        Public Shared Operator *(a As Grid, b As Integer) As Grid
            Return Op(Function(i, j) a(i, j) * b, New Grid(a.Width, a.Height))
        End Operator

        Public Shared Operator /(a As Grid, b As Grid) As Grid
            Return Op(Function(i, j) a(i, j) / b(i, j), New Grid(a.Width, a.Height))
        End Operator

        Public Shared Operator /(a As Grid, b As Integer) As Grid
            Return Op(Function(i, j) a(i, j) / b, New Grid(a.Width, a.Height))
        End Operator

        Friend Shared Function Op(f As Func(Of Integer, Integer, Double), g As Grid) As Grid
            Dim w As Integer = g.Width, h As Integer = g.Height
            For i As Integer = 0 To w - 1
                For j As Integer = 0 To h - 1
                    g(i, j) = f(i, j)
                Next
            Next
            Return g
        End Function

        Public Shared Function Create(width As Integer, height As Integer, op As Func(Of Integer, Integer, Double)) As Grid
            Return Grid.Op(op, New Grid(width, height))
        End Function

    End Structure

#End Region

End Class


