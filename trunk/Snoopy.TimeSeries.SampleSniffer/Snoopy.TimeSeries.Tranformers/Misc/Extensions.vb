Imports System.Runtime.CompilerServices
Public Module Extensions

    ''' <summary>
    ''' Matrix-Matrix tranpose multiplication: A * B^T.
    ''' </summary>
    ''' <param name="A1"></param>
    ''' <param name="A2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension>
    Public Function MultiplyTranspose(A1 As Double()(), A2 As Double()()) As Double()()

        If A1.Length <> A2.Length Then Throw New ArgumentException("Matrix dimensions must be the same.")

        Dim rows As Integer = A1.Length
        Dim columns As Integer = A2(0).Length
        Dim K As Integer = A1.Length

        Dim tmp As Double()() = New Double(rows - 1)() {}

        For i As Integer = 0 To rows - 1
            tmp(i) = New Double(columns - 1) {}
            For j As Integer = 0 To columns - 1
                For _k As Integer = 0 To K - 1
                    tmp(i)(j) += A1(i)(_k) * A2(_k)(j)
                Next
            Next
        Next

        Return tmp
    End Function

    ''' <summary>
    ''' Matrix-Vector tranpose multiplication: A^T * b.
    ''' </summary>
    ''' <param name="A"></param>
    ''' <param name="V"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension>
    Public Function TransposeMultiply(A As Double()(), V As Double()) As Double()

        If A.Length <> V.Length Then Throw New ArgumentException("Matrix dimensions must be the same.")

        Dim rows As Integer = A.Length
        Dim columns As Integer = A(0).Length

        Dim tmp As Double() = New Double(columns - 1) {}

        For i As Integer = 0 To columns - 1
            For j As Integer = 0 To rows - 1
                tmp(i) += A(j)(i) * V(j)
            Next
        Next

        'For i As Integer = 0 To tmp.Length - 1
        '    Dim num2 As Double = 0
        '    For j As Integer = 0 To V.Length - 1
        '        num2 = (num2 + (A(j)(i) * V(j)))
        '    Next j
        '    tmp(i) = num2
        'Next


        Return tmp
    End Function

    <Extension>
    Public Sub Subtract(a As Double(), b As Double)
        For i As Integer = 0 To a.Length - 1
            a(i) -= b
        Next
    End Sub

    <Extension>
    Public Sub MultiplyBy(a As Double(), b As Double())
        For i As Integer = 0 To a.Length - 1
            a(i) *= b(i)
        Next
    End Sub

    <Extension>
    Public Sub DivideBy(a As Double(), b As Double)
        For i As Integer = 0 To a.Length - 1
            a(i) /= b
        Next
    End Sub

End Module
