Namespace Wavelets
    Public Class WaveletTransform

        Private _Filter As WaveletFilter

        Public Sub New(filter As WaveletFilter)
            Me._Filter = filter
        End Sub

        Private Function mod_1_N(number As Integer, divisor As Integer) As Integer
            ' GMA 
            ' Compute positive number between 1 and divisor, inclusive 
            If number = 0 Then
                Return 0
            End If
            Dim [Mod] As Integer = number Mod divisor
            [Mod] = If([Mod] < 1, [Mod] + divisor, [Mod])
            Return ([Mod])
        End Function

        '  Compute the discrete wavelet transform (DWT).  This method uses the  
        '    pyramid algorithm and was adapted from pseudo-code written by  
        '    D. B. Percival.  Periodic boundary conditions are assumed. 
        ' 
        '    Input: 
        '    Vin = dvector of wavelet smooths (data if first iteration) 
        '    M   = length of Vin 
        '    f   = wavelet filter structure (e.g., Haar, D(4), LA(8), ...) 
        ' 
        '    Output: 
        '    Wout = dvector of wavelet coefficients 
        '    Vout = dvector of wavelet smooths 
        Private Sub DWT(Vin As Double(), M As Integer, ByRef Wout As Double(), ByRef Vout As Double())

            'Wout = New Double(M - 1) {}
            'Vout = New Double(M - 1) {}
            Dim l As Integer, k As Integer, t As Integer

            For t = 0 To M \ 2 - 1
                k = 2 * t
                'Wout(t) = _Filter.H(1) * Vin(k)
                'Vout(t) = _Filter.G(1) * Vin(k)
                Wout(t) = _Filter.H(0) * Vin(k)
                Vout(t) = _Filter.G(0) * Vin(k)
                For l = 1 To _Filter.L - 1
                    k -= 1
                    If k < 1 Then
                        k = M - 1
                    End If
                    Wout(t) += _Filter.H(l) * Vin(k)
                    Vout(t) += _Filter.G(l) * Vin(k)
                Next
            Next
        End Sub

        '  Compute the inverse DWT via the pyramid algorithm.  This code was 
        '    adapted from pseudo-code written by D. B. Percival.  Periodic  
        '    boundary conditions are assumed. 
        ' 
        '    Input: 
        '    Win = dvector of wavelet coefficients 
        '    Vin = dvector of wavelet smooths 
        '    M   = length of Win, Vin 
        '    f   = wavelet filter structure (e.g., Haar, D(4), LA(8), ...) 
        ' 
        '    Output: 
        '    Xout = dvector of reconstructed wavelet smooths (eventually the data) 
        '

        Private Sub IDWT(Win As Double(), Vin As Double(), M As Integer, ByRef Xout As Double()) ' TODO: Convert to Function

            'Dim M As Integer = Win.Length

            Xout = New Double(M - 1) {}

            Dim i As Integer = -1
            Dim j As Integer
            Dim l As Integer
            Dim t As Integer
            Dim u As Integer
            Dim _m As Integer = -1, n As Integer = 0

            For t = 0 To M - 1 ' for(t = 1; t <= M; t++) <--- ????
                _m += 2
                n += 2
                u = t
                i = 1 '2
                j = 0 '1

                Xout(_m) = _Filter.H(i) * Win(u) + _Filter.G(i) * Vin(u)
                Xout(n) = _Filter.H(j) * Win(u) + _Filter.G(j) * Vin(u)

                If _Filter.L > 2 Then
                    For l = 0 To _Filter.L \ 2 - 1 ' for(l = 2; l <= f.L / 2; l++)           ???  < -------------------------
                        u += 1
                        If u >= M + 1 Then
                            u = 1
                        End If
                        i += 1 '2
                        j += 1 '2
                        Xout(_m) += _Filter.H(i) * Win(u) + _Filter.G(i) * Vin(u)
                        Xout(n) += _Filter.H(j) * Win(u) + _Filter.G(j) * Vin(u)
                    Next
                End If
            Next
        End Sub

        Private Sub MODWT(Vin As Double(), N As Integer, j As Integer, ByRef Wout As Double(), ByRef Vout As Double())
            '  Compute the maximal overlap discrete wavelet transform (MODWT). 
            '    This method uses the pyramid algorithm and was adapted from  
            '    pseudo-code written by D. B. Percival. 
            ' 
            '    Input: 
            '    Vin  = dvector of wavelet smooths (data if j=1) 
            '    N    = length of Vin to analyze 
            '    j    = iteration (1, 2, ...) 
            '    f    = wavelet filter structure (e.g., Haar, D(4), LA(8), ...) 
            ' 
            '    Output: 
            '    Wout = dvector of wavelet coefficients 
            '    Vout = dvector of wavelet smooths  
            '

            Dim D As Double = Math.Pow(2.0, j)
            If D > N Then
                D = N
            End If
            'Wout = New Double(N - 1) {}
            'Vout = New Double(N - 1) {}
            Dim ht As Double() = New Double(_Filter.L - 1) {}
            Dim gt As Double() = New Double(_Filter.L - 1) {}

            For l As Integer = 0 To _Filter.L - 1
                ht(l) = _Filter.H(l) / Math.Sqrt(2.0)
                gt(l) = _Filter.G(l) / Math.Sqrt(2.0)
            Next

            For t As Integer = 0 To N - 1
                Dim k As Integer = t

                Wout(t) = ht(0) * Vin(t)
                Vout(t) = gt(0) * Vin(t)

                For l As Integer = 1 To _Filter.L - 1
                    If k >= D Then
                        k -= CInt(Math.Truncate(D))
                    Else
                        k = CInt(Math.Truncate(N + k - D))
                    End If
                    'k -= (int) Math.Pow(2.0, j - 1); 
                    'k = mod_1_N(k, N); /* GMA bug fix */ 
                    Wout(t) += ht(l) * Vin(k)
                    Vout(t) += gt(l) * Vin(k)
                Next
            Next
        End Sub

        Private Sub imodwt(Win As Double(), Vin As Double(), N As Integer, j As Integer, ByRef Vout As Double())
            '  Compute the inverse MODWT via the pyramid algorithm.  Adapted from  
            '    pseudo-code written by D. B. Percival. 
            ' 
            '    Input: 
            '    Win  = dvector of wavelet coefficients 
            '    Vin  = dvector of wavelet smooths 
            '    N    = length of Win, Vin 
            '    j    = detail number 
            '    f    = wavelet filter structure 
            ' 
            '    Output: 
            '    Vout = dvector of wavelet smooths 

            Vout = New Double(N - 1) {}
            Dim k As Integer, l As Integer, t As Integer
            Dim ht As Double() = New Double(_Filter.L - 1) {}
            Dim gt As Double() = New Double(_Filter.L - 1) {}
            Dim D As Double = Math.Pow(2.0, j)
            If D > N Then
                D = N
            End If

            For l = 0 To _Filter.L - 1
                ht(l) = _Filter.H(l) / Math.Sqrt(2.0)
                gt(l) = _Filter.G(l) / Math.Sqrt(2.0)
            Next

            For t = 0 To N - 1
                k = t
                Vout(t) = (ht(0) * Win(k)) + (gt(0) * Vin(k))
                ' GMA 
                For l = 1 To _Filter.L - 1
                    k += CInt(Math.Truncate(D))
                    If k >= N Then
                        k -= N
                    End If
                    ' GMA 
                    Vout(t) = Vout(t) + (ht(l) * Win(k)) + (gt(l) * Vin(k))
                Next
            Next
        End Sub

        Protected Sub reflect_vector(Xin As Double(), N As Integer, Xout As Double())
            ' The functions for computing wavelet transforms assume periodic 
            '    boundary conditions, regardless of the data's true nature.  By 
            '    adding a `backwards' version of the data to the end of the current 
            '    data vector, we are essentially reflecting the data.  This allows 
            '    the periodic methods to work properly. 
            Dim t As Integer
            For t = 0 To N - 1
                Xout(t) = Xin(t)
            Next
            For t = 0 To N - 1
                Xout(N + t) = Xin(N - t - 1)
            Next
        End Sub

        Public Function Decompose(X As Double(), N As Integer, numCoef As Integer, transform As WaveletTransformType, boundary As WaveletBoundaryCondition) As Double()() ', ByRef Xout As Double(,))

            ' Peform the discrete wavelet transform (DWT) or maximal overlap 
            '    discrete wavelet transform (MODWT) to a time-series and obtain a 
            '    specified number (K) of wavelet coefficients and subsequent 
            '    wavelet smooth.  Reflection is used as the default boundary 
            '    condition. 
            ' 
            '    Input: 
            '    X        = time-series (dvector of data) 
            '    K        = number of details desired 
            '    f        = wavelet filter structure  
            '    method   = character string (either "dwt" or "modwt") 
            '    boundary = boundary condition (either "period" or "reflect") 
            ' 
            '    Output: Xout = dmatrix of wavelet coefficients and smooth  
            '                   (length = N for "period" and 2N for "reflect") 

            Dim scale As Integer, length As Integer
            Dim Wout As Double(), Vout As Double(), Vin As Double()
            Dim Xout As Double()()

            ' Dyadic vector length required for DWT 
            If transform = WaveletTransformType.DWT AndAlso N Mod 2 <> 0 Then
                Throw New Exception("...data must have dyadic length for DWT...")
            End If

            ' The choice of boundary methods affect things... 
            If boundary = WaveletBoundaryCondition.Reflect Then
                length = 2 * N
                scale = length
                Wout = New Double(length - 1) {}
                Vout = New Double(length - 1) {}
                Vin = New Double(length - 1) {}
                Xout = New Double(length - 1)() {} ', K__1 - 1) {}

                reflect_vector(X, N, Vin)

            Else
                length = N
                scale = N
                Wout = New Double(length - 1) {}
                Vout = New Double(length - 1) {}
                Vin = New Double(length - 1) {}
                Xout = New Double(length - 1)() {} ', K__1 - 1) {}

                For i As Integer = 0 To length - 1
                    Vin(i) = X(i)
                Next

            End If

            For i As Integer = 0 To length - 1
                Xout(i) = New Double(numCoef - 1) {}
            Next

            For i As Integer = 0 To numCoef - 1 ' < ------------------- 'k' not declared
                'For j As Integer = 0 To length - 1
                '    Wout(j) = 0.0
                '    Vout(j) = 0.0
                'Next

                If transform = WaveletTransformType.DWT Then
                    DWT(Vin, scale, Wout, Vout)
                ElseIf transform = WaveletTransformType.MODWT Then
                    MODWT(Vin, length, i, Wout, Vout)
                End If

                For j As Integer = 0 To N - 1
                    'Xout(i, k__2) = Wout(i)
                    Xout(j)(i) = Wout(j)
                Next
                For j As Integer = 0 To length - 1
                    Vin(j) = Vout(j)
                Next
                scale -= scale \ 2
            Next

            For i As Integer = 0 To length - 1
                'Xout(i, K__1 - 1) = Wout(i)
                Xout(i)(numCoef - 1) = Wout(i)
            Next
            ' GMA 
            Return Xout

        End Function

        Public Function Multiresolution(Xin As Double()(), numCoef As Integer, transform As WaveletTransformType, boundary As WaveletBoundaryCondition) As Double()()
            ' Peform a multirsolution analysis using the DWT or MODWT matrix 
            '    obtained from `decompose.'  The inverse transform will be applied 
            '    to selected wavelet detail coefficients.  The wavelet smooth 
            '    coefficients from the original transform are added to the K+1 
            '    column in order to preserve the additive decomposition. 
            '    Reflection is used as the default boundary condition. 
            ' 
            '    Input: 
            '    Xin    = dmatrix from `decompose' 
            '    N      = number of rows in Xin 
            '    K      = number of details in Xin 
            '    f      = wavelet filter structure 
            '    method = character string (either "dwt" or "modwt") 
            ' 
            '    Output: 
            '    Xmra = dmatrix containg K wavelet details and 1 wavelet smooth 

            Dim N As Integer = Xin.Length
            Dim t As Integer, length As Integer
            Dim zero As Double(), Xout As Double(), Win As Double()

            If boundary = WaveletBoundaryCondition.Reflect Then
                length = 2 * N
            Else
                length = N
            End If

            zero = New Double(length - 1) {}
            Xout = New Double(length - 1) {}
            Win = New Double(length - 1) {}
            Dim Xmra As Double()() = New Double(N - 1)() {}

            For k As Integer = 0 To numCoef - 1

                Xmra(k) = New Double(numCoef - 1) {}

                For t = 0 To length - 1
                    Win(t) = Xin(t)(k)
                    zero(t) = 0.0
                Next
                If transform = WaveletTransformType.DWT Then
                    IDWT(Win, zero, CInt(N / Math.Pow(2, k)), Xout)
                ElseIf transform = WaveletTransformType.MODWT Then
                    imodwt(Win, zero, length, k, Xout)
                End If

                For i As Integer = k To 0 Step -1
                    For t = 0 To length - 1
                        Win(t) = Xout(t)
                    Next

                    If transform = WaveletTransformType.DWT Then
                        IDWT(zero, Win, CInt(N / Math.Pow(2, i)), Xout)
                    Else
                        imodwt(zero, Win, length, i, Xout)
                    End If
                Next

                For t = 0 To N - 1
                    Xmra(t)(k) = Xout(t)
                Next

            Next

            ' One more iteration is required on the wavelet smooth coefficients to complete the additive decomposition. 

            For t = 0 To length - 1
                Win(t) = Xin(t)(numCoef - 1)
                zero(t) = 0.0
            Next
            If transform = WaveletTransformType.DWT Then
                IDWT(zero, Win, CInt(N / Math.Pow(2, numCoef)), Xout)
            Else
                imodwt(zero, Win, length, numCoef, Xout)
            End If
            For i As Integer = numCoef To 0 Step -1
                For t = 0 To length - 1
                    Win(t) = Xout(t)
                Next
                If transform = WaveletTransformType.DWT Then
                    IDWT(zero, Win, CInt(N / Math.Pow(2, i)), Xout)
                Else
                    imodwt(zero, Win, length, i, Xout)
                End If
                For t = 0 To length - 1
                    Win(t) = Xout(t)
                    zero(t) = 0.0
                Next
            Next
            For t = 0 To N - 1
                Xmra(t)(numCoef) = Xout(t)
            Next

            Return Xmra

        End Function
    End Class

End Namespace
