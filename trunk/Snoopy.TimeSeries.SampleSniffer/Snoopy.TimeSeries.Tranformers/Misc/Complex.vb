Imports System.Runtime.InteropServices
<StructLayout(LayoutKind.Sequential)> _
Public Structure Complex
    Implements IComparable
    Implements ICloneable

    Public Re As Double
    Public Im As Double

    Public Sub New(ByVal real As Double, ByVal imaginary As Double)
        Re = real
        Im = imaginary
    End Sub

    Public Sub New(ByVal c As Complex)
        Re = c.Re
        Im = c.Im
    End Sub

    Public Shared Function FromRealImaginary(ByVal real As Double, ByVal imaginary As Double) As Complex
        Dim c As Complex
        c.Re = real
        c.Im = imaginary
        Return c
    End Function

    Public Shared Function FromModulusArgument(ByVal modulus As Double, ByVal argument As Double) As Complex
        Dim c As Complex
        c.Re = (modulus * Math.Cos(argument))
        c.Im = (modulus * Math.Sin(argument))
        Return c
    End Function

    Private Function ICloneable_Clone() As Object Implements ICloneable.Clone
        Return New Complex(Me)
    End Function

    Public Function Clone() As Complex
        Return New Complex(Me)
    End Function

    Public Function GetModulus() As Double
        Dim x As Double = Re
        Dim y As Double = Im
        Return Math.Sqrt(x * x + y * y)
    End Function

    Public Function GetModulusSquared() As Double
        Dim x As Double = Re
        Dim y As Double = Im
        Return x * x + y * y
    End Function

    Public Shared Function Pow(ByVal value As Complex, ByVal power As Double) As Complex
        Return Complex.Pow(value, New Complex(power, 0))
    End Function

    Public Shared Function Pow(ByVal value As Complex, ByVal power As Complex) As Complex
        If (power = Complex.Zero) Then
            Return Complex.One
        End If
        If (value = Complex.Zero) Then
            Return Complex.Zero
        End If
        Dim real As Double = value.Re
        Dim imaginary As Double = value.Im
        Dim y As Double = power.Re
        Dim num4 As Double = power.Im
        Dim d As Double = Complex.Abs(value)
        Dim num6 As Double = Math.Atan2(imaginary, real)
        Dim num7 As Double = ((y * num6) + (num4 * Math.Log(d)))
        Dim num8 As Double = (Math.Pow(d, y) * Math.Pow(2.7182818284590451, (-num4 * num6)))
        Return New Complex((num8 * Math.Cos(num7)), (num8 * Math.Sin(num7)))
    End Function

    Public Shared Function Abs(ByVal value As Complex) As Double
        If (Double.IsInfinity(value.Re) OrElse Double.IsInfinity(value.Im)) Then
            Return Double.PositiveInfinity
        End If
        Dim num As Double = Math.Abs(value.Re)
        Dim num2 As Double = Math.Abs(value.Im)
        If (num > num2) Then
            Dim num3 As Double = (num2 / num)
            Return (num * Math.Sqrt((1 + (num3 * num3))))
        End If
        If (num2 = 0) Then
            Return num
        End If
        Dim num4 As Double = (num / num2)
        Return (num2 * Math.Sqrt((1 + (num4 * num4))))
    End Function

    Public Function GetArgument() As Double
        Return Math.Atan2(Im, Re)
    End Function

    Public Function GetConjugate() As Complex
        Return FromRealImaginary(Re, -Im)
    End Function

    Public Sub Normalize()
        Dim modulus As Double = GetModulus()
        If modulus = 0 Then
            Throw New DivideByZeroException("Can not normalize a complex number that is zero.")
        End If
        Re = (Re / modulus)
        Im = (Im / modulus)
    End Sub

    Public Overrides Function Equals(ByVal o As Object) As Boolean
        If TypeOf o Is Complex Then
            Dim c As Complex = CType(o, Complex)
            Return (Me = c)
        End If
        Return False
    End Function

    Public Function CompareTo(ByVal o As Object) As Integer Implements IComparable.CompareTo
        If o Is Nothing Then
            ' null sorts before current
            Return 1
        End If
        If TypeOf o Is Complex Then
            Return GetModulus().CompareTo(CType(o, Complex).GetModulus())
        End If
        If TypeOf o Is Double Then
            Return GetModulus().CompareTo(CDbl(o))
        End If
        If TypeOf o Is Single Then
            Return GetModulus().CompareTo(CSng(o))
        End If
        Throw New ArgumentException()
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return (Re.GetHashCode() Xor Im.GetHashCode())
    End Function

#Region " -- Operators -- "

    Public Shared Narrowing Operator CType(ByVal d As Double) As Complex
        Dim c As Complex
        c.Re = d
        c.Im = 0
        Return c
    End Operator

    Public Shared Narrowing Operator CType(ByVal c As Complex) As Double
        Return c.Re
    End Operator

    Public Shared Operator =(ByVal a As Complex, ByVal b As Complex) As Boolean
        Return (a.Re = b.Re) AndAlso (a.Im = b.Im)
    End Operator

    Public Shared Operator <>(ByVal a As Complex, ByVal b As Complex) As Boolean
        Return (a.Re <> b.Re) OrElse (a.Im <> b.Im)
    End Operator

#Region " -- Add -- "

    Public Shared Operator +(ByVal a As Complex) As Complex
        Return a
    End Operator

    Public Shared Operator +(ByVal a As Complex, ByVal f As Double) As Complex
        a.Re = (a.Re + f)
        Return a
    End Operator

    Public Shared Operator +(ByVal f As Double, ByVal a As Complex) As Complex
        a.Re = (a.Re + f)
        Return a
    End Operator

    Public Shared Operator +(ByVal a As Complex, ByVal b As Complex) As Complex
        a.Re = a.Re + b.Re
        a.Im = a.Im + b.Im
        Return a
    End Operator

#End Region

#Region " -- Sub -- "

    Public Shared Operator -(ByVal a As Complex) As Complex
        a.Re = -a.Re
        a.Im = -a.Im
        Return a
    End Operator

    Public Shared Operator -(ByVal a As Complex, ByVal f As Double) As Complex
        a.Re = (a.Re - f)
        Return a
    End Operator

    Public Shared Operator -(ByVal f As Double, ByVal a As Complex) As Complex
        a.Re = CSng(f - a.Re)
        a.Im = CSng(0 - a.Im)
        Return a
    End Operator

    Public Shared Operator -(ByVal a As Complex, ByVal b As Complex) As Complex
        a.Re = a.Re - b.Re
        a.Im = a.Im - b.Im
        Return a
    End Operator

#End Region

#Region " -- Mul -- "

    Public Shared Operator *(ByVal a As Complex, ByVal f As Double) As Complex
        a.Re = (a.Re * f)
        a.Im = (a.Im * f)
        Return a
    End Operator

    Public Shared Operator *(ByVal f As Double, ByVal a As Complex) As Complex
        a.Re = (a.Re * f)
        a.Im = (a.Im * f)
        Return a
    End Operator

    Public Shared Operator *(ByVal a As Complex, ByVal b As Complex) As Complex
        Dim x As Double = a.Re, y As Double = a.Im
        Dim u As Double = b.Re, v As Double = b.Im
        a.Re = (x * u - y * v)
        a.Im = (x * v + y * u)
        Return a
    End Operator

#End Region

#Region " -- Div -- "

    Public Shared Operator /(ByVal a As Complex, ByVal f As Double) As Complex
        If f = 0 Then
            Throw New DivideByZeroException()
        End If
        a.Re = (a.Re / f)
        a.Im = (a.Im / f)
        Return a
    End Operator

    Public Shared Operator /(ByVal a As Complex, ByVal b As Complex) As Complex
        Dim x As Double = a.Re, y As Double = a.Im
        Dim u As Double = b.Re, v As Double = b.Im
        Dim denom As Double = u * u + v * v

        If denom = 0 Then
            Throw New DivideByZeroException()
        End If

        a.Re = ((x * u + y * v) / denom)
        a.Im = ((y * u - x * v) / denom)

        Return a
    End Operator

#End Region

#End Region

    Public Overrides Function ToString() As String
        Return [String].Format("( {0}, {1}i )", Re, Im)
    End Function

    Public Shared Function IsEqual(ByVal a As Complex, ByVal b As Complex, ByVal tolerance As Double) As Boolean
        Return (Math.Abs(a.Re - b.Re) < tolerance) AndAlso (Math.Abs(a.Im - b.Im) < tolerance)
    End Function

    Public Shared ReadOnly Property Zero() As Complex
        Get
            Return New Complex(0, 0)
        End Get
    End Property

    Public Shared ReadOnly Property One() As Complex
        Get
            Return New Complex(1, 0)
        End Get
    End Property

    Public Shared ReadOnly Property I() As Complex
        Get
            Return New Complex(0, 1)
        End Get
    End Property

    Public Shared ReadOnly Property MaxValue() As Complex
        Get
            Return New Complex(Double.MaxValue, Double.MaxValue)
        End Get
    End Property

    Public Shared ReadOnly Property MinValue() As Complex
        Get
            Return New Complex(Double.MinValue, Double.MinValue)
        End Get
    End Property

End Structure

