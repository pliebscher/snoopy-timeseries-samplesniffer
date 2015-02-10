Public Class DWT1

    ' http://eeweb.poly.edu/iselesni/WaveletSoftware/allcode/dwt.m
    Public Shared Function FDWT(x As Double(), J As Integer, af As Double()()) As Double()()
        Dim w As Double()() = New Double(J + 1)() {}
        For k As Integer = 0 To J - 1
            Dim a As Double()() = AFB(x, af)
            x = a(0)
            w(k) = a(1)
        Next
        w(J) = x
        Return w
    End Function

    Public Shared Function IDWT(w As Double()(), J As Integer, sf As Double()()) As Double()
        Dim y As Double() = w(J)
        For k As Integer = J - 1 To 0 Step -1
            y = SFB(y, w(k), sf)
        Next
        Return y
    End Function

    Public Shared Function AFB(x As Double(), af As Double()()) As Double()()

        Dim N As Integer = x.Length
        Dim L As Integer = CInt(af.Length / 2)
        x = Rotate(x, -L)

        ' lowpass filter
        Dim lo As Double() = upfirdn(x, af(0), 1, 2)
        ' lo(1:L) = lo(N/2+[1:L]) + lo(1:L);
        For i As Integer = 0 To L - 1
            lo(i) = lo(CInt(N / 2) + i) + lo(i) ' <------ ADDED CInt()
        Next

        Dim loHalf As Double() = New Double(CInt(N / 2) - 1) {} ' <------ ADDED CInt()
        ' Arrays.copy(loHalf, lo, N/2);
        Array.Copy(lo, loHalf, CInt(N / 2)) ' <------ ADDED CInt()

        ' highpass filter
        Dim hi As Double() = upfirdn(x, af(1), 1, 2)
        'hi(1:L) = hi(N/2+[1:L]) + hi(1:L);
        For h As Integer = 0 To L - 1
            hi(h) = hi(CInt(N / 2) + h) + hi(h) ' <------ ADDED CInt()
        Next

        Dim hiHalf As Double() = New Double(CInt(N / 2) - 1) {} ' <------ ADDED CInt()
        ' Arrays.copy(hiHalf, hi, N/2);
        Array.Copy(hi, hiHalf, CInt(N / 2)) ' <------ ADDED CInt()

        Return New Double()() {loHalf, hiHalf}

    End Function

    Private Shared Function SFB(lo As Double(), hi As Double(), sf As Double()()) As Double()
        Dim N As Integer = 2 * lo.Length
        Dim L As Integer = sf(0).Length

        lo = upfirdn(lo, sf(0), 2, 1)
        hi = upfirdn(hi, sf(1), 2, 1)

        ' Recombine the signal
        Dim y As Double() = add(lo, hi)
        'y(1:L-2) = y(1:L-2) + y(N+[1:L-2]);
        For i As Integer = 0 To L - 3
            y(i) += y(N + i)
        Next
        'y = y(1:N);
        'XXX

        'y = cshift(y, 1-L/2);
        y = Rotate(y, 1 - CInt(L / 2)) ' <------ ADDED CInt()
        Return y
    End Function

    Public Shared Function Rotate(x As Double(), m As Integer) As Double()
        Dim L As Integer = x.Length
        Dim y As Double() = New Double(L) {}
        For n As Integer = 0 To L - 1
            Dim i As Integer = (n - m) Mod L
            If i < 0 Then i += L
            y(n) = x(i)
        Next
        Return y
    End Function

    Public Shared Function farras() As Double()()
        Return New Double()() {AnalysisFilter(0), AnalysisFilter(1), SynthesisFilter(0), SynthesisFilter(1)}
    End Function

    Public Shared Function add(x As Double(), y As Double()) As Double()

        Dim z As Double() = New Double(If(x.Length >= y.Length, x.Length, y.Length) - 1) {}

        For i As Integer = 0 To x.Length - 1
            If i >= x.Length Then
                z(i) = y(i)
            ElseIf i >= y.Length Then
                z(i) = x(i)
            Else
                z(i) = x(i) + y(i)
            End If
        Next

        Return z
    End Function

    Public Shared Function upfirdn(input As Double(), filter As Double(), upRate As Integer, downRate As Integer) As Double()

        ' Create the Resampler
        Dim theResampler As New Resampler(upRate, downRate, filter)

        ' pad input by length of one polyphase of filter to flush all values out
        Dim padding As Integer = theResampler.coefsPerPhase() - 1
        Dim inputPadded As Double() = New Double(input.Length + (padding - 1)) {}

        For i As Integer = 0 To inputPadded.Length - 1
            If i < input.Length Then
                inputPadded(i) = input(i)
            Else
                inputPadded(i) = 0
            End If
        Next

        ' calc size of output
        Dim resultsCount As Integer = theResampler.neededOutCount(inputPadded.Length)
        Dim results As Double() = New Double(resultsCount) {} ' New Double(resultsCount - 1) {}
        Dim numSamplesComputed As Integer = theResampler.Apply(inputPadded, results)

        If numSamplesComputed <> resultsCount Then
            Debug.WriteLine("upfirdn: numSamplesComputed != resultsCount: " & numSamplesComputed & " != " & resultsCount)
        End If

        inputPadded = Nothing

        Return results

    End Function

#Region " -- Filter Bank Arrays -- "

    Public Shared ReadOnly AnalysisFilter As Double()() = New Double()() {
    New Double() {
             0,
             0,
            -0.08838834764832,
             0.08838834764832,
             0.695879989034,
             0.695879989034,
             0.08838834764832,
            -0.08838834764832,
             0.01122679215254,
             0.01122679215254
    },
    New Double() {
            -0.01122679215254,
             0.01122679215254,
             0.08838834764832,
             0.08838834764832,
            -0.695879989034,
             0.695879989034,
            -0.08838834764832,
            -0.08838834764832,
             0,
             0
    }
}

    Public Shared ReadOnly SynthesisFilter As Double()() = New Double()() {
        New Double() {
                 0.01122679215254,
                 0.01122679215254,
                -0.08838834764832,
                 0.08838834764832,
                 0.695879989034,
                 0.695879989034,
                 0.08838834764832,
                -0.08838834764832,
                 0,
                 0
        },
        New Double() {
                 0,
                 0,
                -0.08838834764832,
                -0.08838834764832,
                 0.695879989034,
                -0.695879989034,
                 0.08838834764832,
                 0.08838834764832,
                 0.01122679215254,
                -0.01122679215254
        }
    }

#End Region

End Class


Public Class Resampler


    Private _upRate As Integer
    Private _downRate As Integer

    Private _transposedCoefs As Double()
    Private _state As Double()
    'private double[] _stateEnd;
    Private _stateEnd As Integer

    Private _paddedCoefCount As Integer
    ' ceil(len(coefs)/upRate)*upRate
    Private _coefsPerPhase As Integer
    ' _paddedCoefCount / upRate
    Private _t As Integer
    ' "time" (modulo upRate)
    Private _xOffset As Integer


    Public Sub New(upRate As Integer, downRate As Integer, coeficients As Double())
        Me._upRate = upRate
        Me._downRate = downRate
        _t = 0
        _xOffset = 0

        _paddedCoefCount = coeficients.Length

        While _paddedCoefCount Mod _upRate <> 0
            _paddedCoefCount += 1
        End While

        _coefsPerPhase = CInt(_paddedCoefCount / _upRate) ' added CInt - PPL
        _transposedCoefs = New Double(_paddedCoefCount - 1) {}

        For i As Integer = 0 To _transposedCoefs.Length - 1 ' !!!!!!! DO WE NEED THIS???
            _transposedCoefs(i) = 0
        Next

        _state = New Double(_coefsPerPhase - 2) {}
        '_stateEnd = _state + _coefsPerPhase - 1;
        _stateEnd = _coefsPerPhase - 1

        For i As Integer = 0 To _state.Length - 1 ' !!!!!!! DO WE NEED THIS???
            _state(i) = 0
        Next

        ' This both transposes, and "flips" each phase, while
        '* copying the defined coefficients into local storage.
        '* There is probably a faster way to do this
        For i As Integer = 0 To _upRate - 1
            For j As Integer = 0 To _coefsPerPhase - 1
                If j * _upRate + i < coeficients.Length Then
                    _transposedCoefs((_coefsPerPhase - 1 - j) + i * _coefsPerPhase) = coeficients(j * _upRate + i)
                End If
            Next
        Next

    End Sub

    Public Function Apply([in] As Double(), [out] As Double()) As Integer

        Dim x As Integer = _xOffset
        Dim y As Integer = 0
        Dim [end] As Integer = [in].Length

        While (x < [end])

            'outputType acc = 0.;
            Dim acc As Double = 0.0

            'coefType *h = _transposedCoefs + _t*_coefsPerPhase;
            Dim h As Integer = _t * (_coefsPerPhase - 1)

            'inputType *xPtr = x - _coefsPerPhase + 1;
            Dim xPtr As Integer = x - _coefsPerPhase + 1

            'int offset = in - xPtr;
            Dim offset As Integer = -xPtr

            If offset > 0 Then
                ' need to draw from the _state buffer
                'inputType *statePtr = _stateEnd - offset;
                Dim statePtr As Integer = _stateEnd - offset

                While statePtr < _stateEnd - 1
                    'acc += _state[statePtr++] * in[h++];
                    acc += _state(System.Math.Max(System.Threading.Interlocked.Increment(statePtr), statePtr - 1)) * _transposedCoefs(System.Math.Max(System.Threading.Interlocked.Increment(h), h - 1))
                End While

                xPtr += offset
            End If

            While (xPtr <= x)
                'acc += in[xPtr++] * _transposedCoefs[h++]; 
                'acc += [in](System.Math.Max(System.Threading.Interlocked.Increment(xPtr), xPtr - 1)) * _transposedCoefs(System.Math.Max(System.Threading.Interlocked.Increment(h), h - 1))
                xPtr += 1
            End While

            '*y++ = acc;
            'out[y++] = acc; 
            [out](System.Math.Max(System.Threading.Interlocked.Increment(y), y - 1)) = acc
            _t += _downRate

            Dim advanceAmount As Integer = CInt(_t / _upRate)

            x += advanceAmount

            ' which phase of the filter to use
            _t = _t Mod _upRate

        End While

        _xOffset = x - [end]

        ' manage _state buffer
        ' find number of samples retained in buffer:
        'int retain = (_coefsPerPhase - 1) - [in].length;
        Dim retain As Integer = (_coefsPerPhase - 1) - [in].Length

        If (retain > 0) Then

            ' for inCount smaller than state buffer, copy end of buffer
            ' to beginning:
            'Arrays.copy(_state, 0, _state, _stateEnd - retain, retain)
            Array.Copy(_state, 0, _state, (_stateEnd - retain), retain)

            ' Then, copy the entire (short) input to end of buffer
            'Arrays.copy(_state, _stateEnd - in.length, in, in.length)
            Array.Copy(_state, 0, [in], (_stateEnd - [in].Length), [in].Length) ' <-------------- ????????????????????
        Else
            ' Arrays.copy(_state, 0, in, end - (_coefsPerPhase - 1), (_coefsPerPhase - 1));
            Array.Copy(_state, 0, [in], [end] - (_coefsPerPhase - 1), (_coefsPerPhase - 1))
        End If

        Return y

    End Function

    Public Function neededOutCount(inCount As Integer) As Integer
        Dim np As Integer = inCount * _upRate
        Dim need As Integer = CInt(np / _downRate)

        If (_t + _upRate * _xOffset) < (np Mod _downRate) Then
            need += 1
        End If

        Return need
    End Function

    Public ReadOnly Property coefsPerPhase() As Integer
        Get
            Return _coefsPerPhase
        End Get
    End Property

End Class
