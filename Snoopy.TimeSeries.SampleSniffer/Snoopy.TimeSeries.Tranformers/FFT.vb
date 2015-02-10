''' <summary>
''' http://www.lomont.org/Software/Misc/FFT/LomontFFT.html
''' </summary>
''' <remarks></remarks>
Public Class FFT

    ''' <summary>                                                                                            
    ''' Pre-computed sine/cosine tables for speed                                                            
    ''' </summary>                                                                                           
    Private cosTable As Double()
    Private sinTable As Double()

    Private _Phase As Phase
    Private _A As Integer = 0
    Private _B As Integer = 1

    ''' <summary>                                                                                            
    ''' Compute the forward or inverse Fourier Transform of data, with                                       
    ''' data containing complex valued data as alternating real and                                          
    ''' imaginary parts. The length must be a power of 2. The data is                                        
    ''' modified in place.                                                                                   
    ''' </summary>                                                                                           
    ''' <param name="data">The complex data stored as alternating real                                       
    ''' and imaginary parts</param>                                                                          
    ''' <param name="forward">true for a forward transform, false for                                        
    ''' inverse transform</param>                                                                            
    'Public Sub FFT(data As Double(), forward As Boolean)
    '    Dim n As Integer = data.Length
    '    ' checks n is a power of 2 in 2's complement format                                                 
    '    If (n And (n - 1)) <> 0 Then
    '        Throw New ArgumentException("data length " & n & " in FFT is not a power of 2")
    '    End If
    '    n \= 2
    '    ' n is the number of samples                                                             
    '    Reverse(data, n)
    '    ' bit index data reversal                                                         
    '    ' do transform: so single point transforms, then doubles, etc.                                      
    '    Dim sign As Double = If(forward, _B, -_B)
    '    Dim mmax As Integer = 1
    '    While n > mmax
    '        Dim istep As Integer = 2 * mmax
    '        Dim theta As Double = sign * Math.PI / mmax
    '        Dim wr As Double = 1, wi As Double = 0
    '        Dim wpr As Double = Math.Cos(theta)
    '        Dim wpi As Double = Math.Sin(theta)
    '        For m As Integer = 0 To istep - 1 Step 2
    '            Dim k As Integer = m
    '            While k < 2 * n
    '                Dim j As Integer = k + istep
    '                Dim tempr As Double = wr * data(j) - wi * data(j + 1)
    '                Dim tempi As Double = wi * data(j) + wr * data(j + 1)
    '                data(j) = data(k) - tempr
    '                data(j + 1) = data(k + 1) - tempi
    '                data(k) = data(k) + tempr
    '                data(k + 1) = data(k + 1) + tempi
    '                k += 2 * istep
    '            End While
    '            Dim t As Double = wr
    '            ' trig recurrence                                                               
    '            wr = wr * wpr - wi * wpi
    '            wi = wi * wpr + t * wpi
    '        Next
    '        mmax = istep
    '    End While

    '    ' perform data scaling as needed                                                                    
    '    Scale(data, n, forward)
    'End Sub

    Public Function ComplexToReal(data As Complex(), forward As Boolean) As Double()

        Complex(data, forward)

        Dim real As Double() = New Double(data.Length - 1) {}

        For i As Integer = 0 To real.Length - 1
            Dim re As Double = data(i).Re
            Dim im As Double = data(i).Im
            real(i) = Math.Sqrt(re * re + im * im)
        Next

        Return real
    End Function

    Public Function RealToComplex(data As Double(), forward As Boolean) As Complex()

        Dim N As Integer = data.Length
        Dim complexSignal As Double() = New Double(2 * N - 1) {}

        For i As Integer = 0 To N - 1
            complexSignal(2 * i) = data(i)
            complexSignal(2 * i + 1) = 0
        Next

        Me.Complex(complexSignal, forward)

        Dim complex As Complex() = New Complex(N - 1) {}

        For j As Integer = 0 To N - 1
            complex(j).Re = complexSignal(2 * j)
            complex(j).Im = complexSignal(2 * j + 1)
        Next

        Return complex
    End Function

    Public Sub Complex(data As Complex(), forward As Boolean)

        Dim N As Integer = data.Length
        Dim complexSignal As Double() = New Double(2 * N - 1) {}

        For i As Integer = 0 To N - 1
            complexSignal(2 * i) = data(i).Re
            complexSignal(2 * i + 1) = data(i).Im
        Next

        Complex(complexSignal, forward)

        For i As Integer = 0 To N - 1
            data(i).Re = complexSignal(2 * i)
            data(i).Im = complexSignal(2 * i + 1)
        Next

    End Sub

    ''' <summary>                                                                                            
    ''' Compute the forward or inverse Fourier Transform of data, with data                                  
    ''' containing complex valued data as alternating real and imaginary                                     
    ''' parts. The length must be a power of 2. This method caches values                                    
    ''' and should be slightly faster on than the FFT method for repeated uses.                              
    ''' It is also slightly more accurate. Data is transformed in place.                                     
    ''' </summary>                                                                                           
    ''' <param name="data">The complex data stored as alternating real                                       
    ''' and imaginary parts</param>                                                                          
    ''' <param name="forward">true for a forward transform, false for                                        
    ''' inverse transform</param>                                                                            
    Public Sub Complex(data As Double(), forward As Boolean)
        Dim n As Integer = data.Length
        ' checks n is a power of 2 in 2's complement format                                                 
        If (n And (n - 1)) <> 0 Then
            Throw New ArgumentException("data length " & n & " in FFT is not a power of 2")
        End If
        n \= 2
        ' n is the number of samples                                                             
        Reverse(data, n)
        ' bit index data reversal                                                         
        ' make table if needed                                                                              
        If (cosTable Is Nothing) OrElse (cosTable.Length <> n) Then
            Initialize(n)
        End If

        ' do transform: so single point transforms, then doubles, etc.                                      
        Dim sign As Double = If(forward, _B, -_B)
        Dim mmax As Integer = 1
        Dim tptr As Integer = 0
        While n > mmax
            Dim istep As Integer = 2 * mmax
            For m As Integer = 0 To istep - 1 Step 2
                Dim wr As Double = cosTable(tptr)
                ' Dim wi As Double = sign * sinTable(System.Math.Max(System.Threading.Interlocked.Increment(tptr), tptr - 1))
                ' var wi = sign * sinTable[tptr++];
                tptr += 1
                Dim wi As Double = sign * sinTable(tptr)
                Dim k As Integer = m
                While k < 2 * n
                    Dim j As Integer = k + istep
                    Dim tempr As Double = wr * data(j) - wi * data(j + 1)
                    Dim tempi As Double = wi * data(j) + wr * data(j + 1)
                    data(j) = data(k) - tempr
                    data(j + 1) = data(k + 1) - tempi
                    data(k) = data(k) + tempr
                    data(k + 1) = data(k + 1) + tempi
                    k += 2 * istep
                End While
            Next
            mmax = istep
        End While


        ' perform data scaling as needed                                                                    
        Scale(data, n, forward)
    End Sub

    ''' <summary>                                                                                            
    ''' Compute the forward or inverse Fourier Transform of data, with                                       
    ''' data containing real valued data only. The output is complex                                         
    ''' valued after the first two entries, stored in alternating real                                       
    ''' and imaginary parts. The first two returned entries are the real                                     
    ''' parts of the first and last value from the conjugate symmetric                                       
    ''' output, which are necessarily real. The length must be a power                                       
    ''' of 2.                                                                                                
    ''' </summary>                                                                                           
    ''' <param name="data">The complex data stored as alternating real                                       
    ''' and imaginary parts</param>                                                                          
    ''' <param name="forward">true for a forward transform, false for                                        
    ''' inverse transform</param>                                                                            
    Public Sub Real(data As Double(), forward As Boolean)

        Dim n As Integer = data.Length
        ' # of real inputs, 1/2 the complex length                                     
        ' checks n is a power of 2 in 2's complement format                                                 
        If (n And (n - 1)) <> 0 Then
            Throw New ArgumentException("data length " & n & " in FFT is not a power of 2")
        End If

        Dim sign As Double = -1.0
        ' assume inverse FFT, this controls how algebra below works                        
        If forward Then
            ' do packed FFT. This can be changed to FFT to save memory                                        
            Complex(data, True)
            sign = 1.0
            ' scaling - divide by scaling for N/2, then mult by scaling for N                               
            If _A <> 1 Then
                Dim scale As Double = Math.Pow(2.0, (_A - 1) / 2.0)
                For i As Integer = 0 To data.Length - 1
                    data(i) *= scale
                Next
            End If
        End If

        Dim theta As Double = _B * sign * 2 * Math.PI / n
        Dim wpr As Double = Math.Cos(theta)
        Dim wpi As Double = Math.Sin(theta)
        Dim wjr As Double = wpr
        Dim wji As Double = wpi

        For j As Integer = 1 To n \ 4
            Dim k As Double = n \ 2 - j
            Dim tkr As Double = data(CInt(2 * k))
            ' real and imaginary parts of t_k  = t_(n/2 - j)                      
            Dim tki As Double = data(CInt(2 * k + 1))
            Dim tjr As Double = data(2 * j)
            ' real and imaginary parts of t_j                                     
            Dim tji As Double = data(2 * j + 1)

            Dim a As Double = (tjr - tkr) * wji
            Dim b As Double = (tji + tki) * wjr
            Dim c As Double = (tjr - tkr) * wjr
            Dim d As Double = (tji + tki) * wji
            Dim e As Double = (tjr + tkr)
            Dim f As Double = (tji - tki)

            ' compute entry y[j]                                                                            
            data(2 * j) = 0.5 * (e + sign * (a + b))
            data(2 * j + 1) = 0.5 * (f + sign * (d - c))

            ' compute entry y[k]                                                                            
            data(CInt(2 * k)) = 0.5 * (e - sign * (b + a))
            data(CInt(2 * k + 1)) = 0.5 * (sign * (d - c) - f)

            Dim temp As Double = wjr
            ' allow more accurate version here? make option?                                         
            wjr = wjr * wpr - wji * wpi
            wji = temp * wpi + wji * wpr
        Next

        If forward Then
            ' compute final y0 and y_{N/2}, store in data[0], data[1]                                       
            Dim temp As Double = data(0)
            data(0) += data(1)
            data(1) = temp - data(1)
        Else
            Dim temp As Double = data(0)
            ' unpack the y0 and y_{N/2}, then invert FFT                                
            data(0) = 0.5 * (temp + data(1))
            data(1) = 0.5 * (temp - data(1))
            ' do packed inverse (table based) FFT. This can be changed to regular inverse FFT to save memory
            Complex(data, False)
            ' scaling - divide by scaling for N, then mult by scaling for N/2                               
            'if (A != -1) // todo - off by factor of 2? this works, but something seems weird               
            If True Then
                Dim scale As Double = Math.Pow(2.0, -(_A + 1) / 2.0) * 2
                For i As Integer = 0 To data.Length - 1
                    data(i) *= scale
                Next
            End If
        End If
    End Sub

    ''' <summary>                                                                                            
    ''' Determine how phase works on the forward and inverse transforms.  
    '''  
    ''' A: For size N=2^n transforms, the forward transform gets divided by                                     
    '''    N^((1-a)/2) and the inverse gets divided by N^((1+a)/2).
    '''                       
    ''' B: For size N=2^n transforms, the forward transform uses an                                             
    '''    exp(B*2*pi/N) term and the inverse uses an exp(-B*2*pi/N) term.   
    '''                                    
    ''' Common values for (A,B) are                                                                          
    '''     ( 0, 1)  - default                                                                               
    '''     (-1, 1)  - data processing                                                                       
    '''     ( 1,-1)  - signal processing                                                                     
    ''' Abs(B) should be relatively prime to N.                                                              
    ''' Setting B=-1 effectively corresponds to conjugating both input and                                   
    ''' output data.
    ''' 
    ''' Usual values for A are 1, 0, or -1                                                                           
    ''' Usual values for B are 1 or -1.                                                                      
    ''' </summary> 
    Public Property PhaseMode As Phase
        Get
            Return _Phase
        End Get
        Set(value As Phase)
            _Phase = value
            Select Case _Phase
                Case Phase.Signal
                    _A = 1 : _B = -1
                Case Phase.Data
                    _A = -1 : _B = 1
                Case Else
                    _A = 0 : _B = 1
            End Select
        End Set
    End Property

#Region "Internals"

    ''' <summary>                                                                                            
    ''' Scale data using n samples for forward and inverse transforms as needed                              
    ''' </summary>                                                                                           
    ''' <param name="data"></param>                                                                          
    ''' <param name="n"></param>                                                                             
    ''' <param name="forward"></param>                                                                       
    Private Sub Scale(data As Double(), n As Integer, forward As Boolean)
        ' forward scaling if needed                                                                         
        If (forward) AndAlso (_A <> 1) Then
            Dim scale__1 As Double = Math.Pow(n, (_A - 1) / 2.0)
            For i As Integer = 0 To data.Length - 1
                data(i) *= scale__1
            Next
        End If

        ' inverse scaling if needed                                                                         
        If (Not forward) AndAlso (_A <> -1) Then
            Dim scale__1 As Double = Math.Pow(n, -(_A + 1) / 2.0)
            For i As Integer = 0 To data.Length - 1
                data(i) *= scale__1
            Next
        End If
    End Sub

    ''' <summary>                                                                                            
    ''' Call this with the size before using the TableFFT version                                            
    ''' Fills in tables for speed. Done automatically in TableFFT                                            
    ''' </summary>                                                                                           
    ''' <param name="size">The size of the FFT in samples</param>                                            
    Private Sub Initialize(size As Integer)
        ' NOTE: if you port to non garbage collected languages                                              
        ' like C# or Java be sure to free these correctly                                                   
        cosTable = New Double(size - 1) {}
        sinTable = New Double(size - 1) {}

        ' forward pass                                                                                      
        Dim n As Integer = size
        Dim mmax As Integer = 1, pos As Integer = 0
        While n > mmax
            Dim istep As Integer = 2 * mmax
            Dim theta As Double = Math.PI / mmax
            Dim wr As Double = 1, wi As Double = 0
            Dim wpi As Double = Math.Sin(theta)
            ' compute in a slightly slower yet more accurate manner                                         
            Dim wpr As Double = Math.Sin(theta / 2) ' <------------------------ \ -> /
            wpr = -2 * wpr * wpr
            For m As Integer = 0 To istep - 1 Step 2
                cosTable(pos) = wr
                'sinTable(System.Math.Max(System.Threading.Interlocked.Increment(pos), pos - 1)) = wi
                pos += 1
                sinTable(pos) = wi
                Dim t As Double = wr
                wr = wr * wpr - wi * wpi + wr
                wi = wi * wpr + t * wpi + wi
            Next
            mmax = istep
        End While
    End Sub

    ''' <summary>                                                                                            
    ''' Swap data indices whenever index i has binary                                                        
    ''' digits reversed from index j, where data is                                                          
    ''' two doubles per index.                                                                               
    ''' </summary>                                                                                           
    ''' <param name="data"></param>                                                                          
    ''' <param name="n"></param>                                                                             
    Private Sub Reverse(data As Double(), n As Integer)
        ' bit reverse the indices. This is exercise 5 in section                                            
        ' 7.2.1.1 of Knuth's TAOCP the idea is a binary counter                                             
        ' in k and one with bits reversed in j                                                              
        Dim j As Integer = 0, k As Integer = 0
        ' Knuth R1: initialize                                                            
        Dim top As Double = n \ 2
        ' this is Knuth's 2^(n-1)                                                         
        While True
            ' Knuth R2: swap - swap j+1 and k+2^(n-1), 2 entries each                                       
            Dim t As Double = data(j + 2)
            data(j + 2) = data(k + n)
            data(k + n) = t
            t = data(j + 3)
            data(j + 3) = data(k + n + 1)
            data(k + n + 1) = t
            If j > k Then
                ' swap two more                                                                               
                ' j and k                                                                                   
                t = data(j)
                data(j) = data(k)
                data(k) = t
                t = data(j + 1)
                data(j + 1) = data(k + 1)
                data(k + 1) = t
                ' j + top + 1 and k+top + 1                                                                 
                t = data(j + n + 2)
                data(j + n + 2) = data(k + n + 2)
                data(k + n + 2) = t
                t = data(j + n + 3)
                data(j + n + 3) = data(k + n + 3)
                data(k + n + 3) = t
            End If
            ' Knuth R3: advance k                                                                           
            k += 4
            If k >= n Then
                Exit While
            End If
            ' Knuth R4: advance j                                                                           
            Dim h As Double = top

            While j >= h
                j -= CInt(h)
                h /= 2
            End While
            j += CInt(h)
        End While
        ' bit reverse loop                                                                                
    End Sub

#End Region

    Public Enum Phase
        [Default]
        Signal
        Data
    End Enum

End Class
