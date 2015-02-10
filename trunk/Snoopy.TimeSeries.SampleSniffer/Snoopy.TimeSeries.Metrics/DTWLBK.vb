Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text

Partial Public Class DTWLBK

    Private Shared _Comp As New IndexComparer()

    Private _Query As Double()
    Private _QueryOrder As Double()
    Private _Query_sorted As IndexValue()
    Private _QueryLength As Integer
    Private _OrderIndex As Integer()
    Private _LEnv As Double()
    Private _UEnv As Double()

    Private _EX As Double
    Private _EX2 As Double
    Private _Mean As Double
    Private _Std As Double

    Private _R As Integer = -1 ' Warping window.

    Public Sub New(query As Double(), queryLength As Integer, wrappingWindow As Double)

        _QueryLength = queryLength

        If wrappingWindow >= 0 Then
            If wrappingWindow <= 1 Then
                _R = CInt(Math.Truncate(Math.Floor(wrappingWindow * queryLength)))
            Else
                _R = CInt(Math.Truncate(Math.Floor(wrappingWindow)))
            End If
        End If

        _Query = New Double(query.Length - 1) {}
        _QueryOrder = New Double(_QueryLength - 1) {}
        _Query_sorted = New IndexValue(_QueryLength - 1) {}
        _OrderIndex = New Integer(_QueryLength - 1) {}

        For i As Integer = 0 To query.Length - 1
            _EX += query(i)
            _EX2 += query(i) * query(i)
            _Query(i) = query(i)
        Next

        ' Do z-normalize the query, keep in same array, q
        _Mean = _EX / query.Length
        _Std = _EX2 / query.Length
        _Std = CDbl(Math.Sqrt(_Std - _Mean * _Mean))

        For i As Integer = 0 To query.Length - 1
            _Query(i) = (_Query(i) - _Mean) / _Std
        Next

        _UEnv = New Double(query.Length - 1) {}
        _LEnv = New Double(query.Length - 1) {}

        ' Create envelop of the query: lower envelop, l, and upper envelop, u
        lower_upper_lemire(_Query, queryLength, _R, _LEnv, _UEnv)

        ' Sort the query one time by abs(z-norm(q[i]))
        For i As Integer = 0 To _QueryLength - 1
            _Query_sorted(i) = New IndexValue() With { _
                .Index = i, _
                .Value = _Query(i) _
            }
        Next

        Array.Sort(_Query_sorted, _Comp)

        For i As Integer = 0 To _QueryLength - 1
            Dim o As Integer = _Query_sorted(i).Index
            _OrderIndex(i) = o
            _QueryOrder(i) = _Query(o)
        Next

    End Sub

    Private Shared Function dist(x As Double, y As Double) As Double
        Return (x - y) * (x - y)
    End Function

    ''' Data structure (circular array) for finding minimum and maximum for LB_Keogh envolop
    Private Structure Deque
        Public dq As Integer()
        Public size As Integer, capacity As Integer
        Public f As Integer, r As Integer

        Public ReadOnly Property Empty() As Boolean
            Get
                Return size = 0
            End Get
        End Property
    End Structure

    ''' Initial the queue at the begining step of envelop calculation
    Private Shared Sub init(ByRef d As Deque, capacity As Integer)
        d.capacity = capacity
        d.size = 0
        d.dq = New Integer(d.capacity - 1) {}
        d.f = 0
        d.r = d.capacity - 1
    End Sub

    ''' Insert to the queue at the back
    Private Shared Sub push_back(ByRef d As Deque, v As Integer)
        d.dq(d.r) = v
        d.r -= 1
        If d.r < 0 Then
            d.r = d.capacity - 1
        End If
        d.size += 1
    End Sub

    ''' Delete the current (front) element from queue
    Private Shared Sub pop_front(ByRef d As Deque)
        d.f -= 1
        If d.f < 0 Then
            d.f = d.capacity - 1
        End If
        d.size -= 1
    End Sub

    ''' Delete the last element from queue
    Private Shared Sub pop_back(ByRef d As Deque)
        d.r = (d.r + 1) Mod d.capacity
        d.size -= 1
    End Sub

    ''' Get the value at the current position of the circular queue
    Private Shared Function front(ByRef d As Deque) As Integer
        Dim aux As Integer = d.f - 1

        If aux < 0 Then
            aux = d.capacity - 1
        End If
        Return d.dq(aux)
    End Function

    ''' Get the value at the last position of the circular queueint back(struct deque *d)
    Private Shared Function back(ByRef d As Deque) As Integer
        Dim aux As Integer = (d.r + 1) Mod d.capacity
        Return d.dq(aux)
    End Function

    ''' Finding the envelop of min and max value for LB_Keogh
    ''' Implementation idea is intoruduced by Danial Lemire in his paper
    ''' "Faster Retrieval with a Two-Pass Dynamic-Time-Warping Lower Bound", Pattern Recognition 42(9), 2009.
    Private Shared Sub lower_upper_lemire(t As Double(), len As Integer, r As Integer, l As Double(), u As Double())
        Dim du As New Deque()
        Dim dl As New Deque()

        init(du, 2 * r + 2)
        init(dl, 2 * r + 2)

        push_back(du, 0)
        push_back(dl, 0)

        For i As Integer = 1 To len - 1
            If i > r Then
                u(i - r - 1) = t(front(du))
                l(i - r - 1) = t(front(dl))
            End If
            If t(i) > t(i - 1) Then
                pop_back(du)
                While Not du.Empty AndAlso t(i) > t(back(du))
                    pop_back(du)
                End While
            Else
                pop_back(dl)
                While Not dl.Empty AndAlso t(i) < t(back(dl))
                    pop_back(dl)
                End While
            End If
            push_back(du, i)
            push_back(dl, i)
            If i = 2 * r + 1 + front(du) Then
                pop_front(du)
            ElseIf i = 2 * r + 1 + front(dl) Then
                pop_front(dl)
            End If
        Next
        For i As Integer = len To len + r
            u(i - r - 1) = t(front(du))
            l(i - r - 1) = t(front(dl))
            If i - front(du) >= 2 * r + 1 Then
                pop_front(du)
            End If
            If i - front(dl) >= 2 * r + 1 Then
                pop_front(dl)
            End If
        Next
    End Sub

    ''' Calculate quick lower bound
    ''' Usually, LB_Kim take time O(m) for finding top,bottom,fist and last.
    ''' However, because of z-normalization the top and bottom cannot give siginifant benefits.
    ''' And using the first and last points can be computed in constant time.
    ''' The prunning power of LB_Kim is non-trivial, especially when the query is not long, say in length 128.
    Private Shared Function lb_kim_hierarchy(t As Double(), q As Double(), j As Integer, len As Integer, mean As Double, std As Double, _
        Optional bsf As Double = Double.PositiveInfinity) As Double
        ' 1 point at front and back
        Dim d As Double, lb As Double
        Dim x0 As Double = (t(j) - mean) / std
        Dim y0 As Double = (t((len - 1 + j)) - mean) / std
        lb = dist(x0, q(0)) + dist(y0, q(len - 1))
        If lb >= bsf Then
            Return lb
        End If

        ' 2 points at front
        Dim x1 As Double = (t((j + 1)) - mean) / std
        d = Math.Min(dist(x1, q(0)), dist(x0, q(1)))
        d = Math.Min(d, dist(x1, q(1)))
        lb += d
        If lb >= bsf Then
            Return lb
        End If

        ' 2 points at back
        Dim y1 As Double = (t((len - 2 + j)) - mean) / std
        d = Math.Min(dist(y1, q(len - 1)), dist(y0, q(len - 2)))
        d = Math.Min(d, dist(y1, q(len - 2)))
        lb += d
        If lb >= bsf Then
            Return lb
        End If

        ' 3 points at front
        Dim x2 As Double = (t((j + 2)) - mean) / std
        d = Math.Min(dist(x0, q(2)), dist(x1, q(2)))
        d = Math.Min(d, dist(x2, q(2)))
        d = Math.Min(d, dist(x2, q(1)))
        d = Math.Min(d, dist(x2, q(0)))
        lb += d
        If lb >= bsf Then
            Return lb
        End If

        ' 3 points at back
        Dim y2 As Double = (t((len - 3 + j)) - mean) / std
        d = Math.Min(dist(y0, q(len - 3)), dist(y1, q(len - 3)))
        d = Math.Min(d, dist(y2, q(len - 3)))
        d = Math.Min(d, dist(y2, q(len - 2)))
        d = Math.Min(d, dist(y2, q(len - 1)))
        lb += d

        Return lb
    End Function

    ''' LB_Keogh 1: Create Envelop for the query
    ''' Note that because the query is known, envelop can be created once at the begenining.
    '''
    ''' Variable Explanation,
    ''' order : sorted indices for the query.
    ''' uo, lo: upper and lower envelops for the query, which already sorted.
    ''' t : a circular array keeping the current data.
    ''' j : index of the starting location in t
    ''' cb : (output) current bound at each position. It will be used later for early abandoning in DTW.
    Private Shared Function lb_keogh_cumulative(order As Integer(), t As Double(), uo As Double(), lo As Double(), cb As Double(), j As Integer, _
        len As Integer, mean As Double, std As Double, Optional best_so_far As Double = Double.PositiveInfinity) As Double
        Dim lb As Double = 0
        Dim x As Double, d As Double

        Dim i As Integer = 0
        While i < len AndAlso lb < best_so_far
            x = (t((order(i) + j)) - mean) / std
            d = 0
            If x > uo(i) Then
                d = dist(x, uo(i))
            ElseIf x < lo(i) Then
                d = dist(x, lo(i))
            End If
            lb += d
            cb(order(i)) = d
            i += 1
        End While
        Return lb
    End Function

    ''' LB_Keogh 2: Create Envelop for the data
    ''' Note that the envelops have been created (in main function) when each data point has been read.
    '''
    ''' Variable Explanation,
    ''' tz: Z-normalized data
    ''' qo: sorted query
    ''' cb: (output) current bound at each position. Used later for early abandoning in DTW.
    ''' l,u: lower and upper envelop of the current data
    ''' I: array pointer
    Private Shared Function lb_keogh_data_cumulative(order As Integer(), tz As Double(), qo As Double(), cb As Double(), l As Double(), u As Double(), I__1 As Integer, len As Integer, mean As Double, std As Double, Optional best_so_far As Double = Double.PositiveInfinity) As Double

        Dim lb As Double = 0
        Dim uu As Double, ll As Double, d As Double

        Dim i As Integer = 0
        While i < len AndAlso lb < best_so_far
            uu = (u(order(i) + I__1) - mean) / std
            ll = (l(order(i) + I__1) - mean) / std
            d = 0
            If qo(i) > uu Then
                d = dist(qo(i), uu)
            Else
                If qo(i) < ll Then
                    d = dist(qo(i), ll)
                End If
            End If
            lb += d
            cb(order(i)) = d
            i += 1
        End While
        Return lb
    End Function

    ''' Calculate Dynamic Time Wrapping distance
    ''' A,B: data and query, respectively
    ''' cb : cummulative bound used for early abandoning
    ''' r : size of Sakoe-Chiba warpping band
    Private Shared Function dtw(A As Double(), B As Double(), cb As Double(), m As Integer, r As Integer, Optional bsf As Double = Double.PositiveInfinity) As Double

        Dim cost As Double()
        Dim cost_prev As Double()
        Dim cost_tmp As Double()
        Dim i As Integer, j As Integer, k As Integer
        Dim x As Double, y As Double, z As Double, min_cost As Double

        ' Instead of using matrix of size O(m^2) or O(mr), we will reuse two array of size O(r).
        cost = New Double(2 * r) {}
        cost_prev = New Double(2 * r) {}

        For k = 0 To 2 * r
            cost(k) = Double.PositiveInfinity
            cost_prev(k) = Double.PositiveInfinity
        Next

        For i = 0 To m - 1
            k = Math.Max(0, r - i)
            min_cost = Double.PositiveInfinity

            j = Math.Max(0, i - r)
            While j <= Math.Min(m - 1, i + r)
                ' Initialize all row and column
                If (i = 0) AndAlso (j = 0) Then
                    cost(k) = dist(A(0), B(0))
                    min_cost = cost(k)
                    j += 1
                    k += 1
                    Continue While
                End If

                If (j - 1 < 0) OrElse (k - 1 < 0) Then
                    y = Double.PositiveInfinity
                Else
                    y = cost(k - 1)
                End If
                If (i - 1 < 0) OrElse (k + 1 > 2 * r) Then
                    x = Double.PositiveInfinity
                Else
                    x = cost_prev(k + 1)
                End If
                If (i - 1 < 0) OrElse (j - 1 < 0) Then
                    z = Double.PositiveInfinity
                Else
                    z = cost_prev(k)
                End If

                ' Classic DTW calculation
                cost(k) = Math.Min(Math.Min(x, y), z) + dist(A(i), B(j))

                ' Find minimum cost in row for early abandoning (possibly to use column instead of row).
                If cost(k) < min_cost Then
                    min_cost = cost(k)
                End If
                j += 1
                k += 1
            End While

            ' We can abandon early if the current cummulative distace with lower bound together are larger than bsf
            If i + r < m - 1 AndAlso min_cost + cb(i + r + 1) >= bsf Then
                Return min_cost + cb(i + r + 1)
            End If

            ' Move current array to previous array.
            cost_tmp = cost
            cost = cost_prev
            cost_prev = cost_tmp
        Next
        k -= 1

        ' the DTW distance is in the last cell in the matrix of size O(m^2) or at the middle of our array.
        Dim final_dtw As Double = cost_prev(k)

        Return final_dtw
    End Function

    ' Main Function
    Public Function Compute(data As Double()) As Double

        'Dim queryLength As Integer = _Query.Length '\ 2

        If data.Length < _Query.Length Then
            'queryLength = data.Length '\ 4
        End If
        'queryLength = _Query.Length - 1

        ' For every EPOCH points, all cummulative values, such as ex (sum), ex2 (sum square), 
        ' will be restarted for reducing the doubleing point error.
        Dim EPOCH As Integer = data.Length  ' 100000

        ' best-so-far
        Dim bsf As Double
        ' data array and query array
        'Dim t As Double()
        'Dim q As Double()
        'Dim d As Double
        Dim i As Integer
        Dim j As Integer
        Dim ex As Double
        Dim ex2 As Double
        Dim mean As Double
        Dim std As Double
        'Dim r As Integer = -1
        Dim loc As Long = 0

        Dim kim As Integer = 0
        Dim keogh As Integer = 0
        Dim keogh2 As Integer = 0
        Dim dist As Double = 0
        Dim lb_kim As Double = 0
        Dim lb_k As Double = 0
        Dim lb_k2 As Double = 0

        Dim t1 As Double
        Dim tt As Double
        ' start the clock
        t1 = DateTime.Now.Ticks

        ' malloc everything here
        ' new order of the query
        Dim t As Double() = New Double(_QueryLength * 2 - 1) {}
        'Dim order As Integer() = New Integer(_QueryLength - 1) {}
        'Dim u As Double() = New Double(queryLength - 1) {}
        'Dim l As Double() = New Double(queryLength - 1) {}
        'Dim qo As Double() = New Double(_QueryLength - 1) {}
        'Dim uo As Double() = New Double(queryLength - 1) {}
        'Dim lo As Double() = New Double(queryLength - 1) {}
        Dim tz As Double() = New Double(_QueryLength - 1) {}
        Dim cb As Double() = New Double(_QueryLength - 1) {}
        Dim cb1 As Double() = New Double(_QueryLength - 1) {}
        Dim cb2 As Double() = New Double(_QueryLength - 1) {}
        Dim u_d As Double() = New Double(_QueryLength - 1) {}
        Dim l_d As Double() = New Double(_QueryLength - 1) {}

        Dim buffer As Double() = data 'New Double(EPOCH - 1) {}
        Dim u_buff As Double() = New Double(data.Length - 1) {}
        Dim l_buff As Double() = New Double(data.Length - 1) {}

        ' Read query file
        bsf = Double.PositiveInfinity
        'i = 0
        'j = 0
        'ex = 0 ' InlineAssignHelper(ex2, 0)
        'ex2 = 0

        'While i < queryLength
        '    d = _Query(i)
        '    'ex += d
        '    'ex2 += d * d
        '    '_Q(i) = d
        '    i += 1
        'End While

        'ex = _EX
        'ex2 = _EX2

        ' Create envelop of the query: lower envelop, l, and upper envelop, u
        'lower_upper_lemire(_Q, queryLength, r, l, u)

        ' also create another arrays for keeping sorted envelop
        'For i = 0 To _QueryLength - 1
        '    Dim o As Integer = _Query_sorted(i).Index
        '    'order(i) = o
        '    qo(i) = _Query(o)
        '    'uo(i) = _UEnv(o) 'u(o)
        '    'lo(i) = _LEnv(o) 'l(o)
        '    ' Initial the cummulative lower bound
        '    cb(i) = 0
        '    cb1(i) = 0
        '    cb2(i) = 0
        'Next

        i = 0 ' current index of the data in current chunk of size EPOCH
        j = 0 ' the starting index of the data in the circular array, t

        'ex = 0 'InlineAssignHelper(ex2, 0)
        'ex2 = 0
        Dim done As Boolean = False
        Dim it As Integer = 0, ep As Integer = 0, k As Integer = 0
        Dim startIndex As Integer ' the starting index of the data in current chunk of size EPOCH

        'Dim dataQueue As New Queue(Of Double)()

        'For x As Integer = 0 To data.Length - 1
        '    dataQueue.Enqueue(data(x))
        'Next

        't3 = DateTime.Now.Ticks

        Dim lb_kim__lt__bsf As Integer
        Dim lb_k__lt__bsf As Integer
        Dim lb_k2__lt__bsf As Integer
        Dim dist__lt__bsf As Integer

        'While Not done
        '    ' Read first m-1 points
        '    ep = 0
        '    If it = 0 Then
        '        For k = 0 To _QueryLength - 2
        '            If dataQueue.Count > 0 Then
        '                buffer(k) = dataQueue.Dequeue()
        '            End If
        '        Next
        '    Else
        '        For k = 0 To _QueryLength - 2
        '            buffer(k) = buffer(EPOCH - _QueryLength + 1 + k)
        '        Next
        '    End If

        '    ' Read buffer of size EPOCH or when all data has been read.
        '    ep = _QueryLength - 1

        '    While ep < EPOCH
        '        If dataQueue.Count = 0 Then
        '            Exit While
        '        End If
        '        'd = dataQueue.Dequeue()
        '        buffer(ep) = dataQueue.Dequeue() ' = d
        '        ep += 1
        '    End While

        '    ' Data are read in chunk of size EPOCH.
        '    ' When there is nothing to read, the loop is end.
        '    If ep <= _QueryLength - 1 Then
        '        Dim ttttt As Double = (DateTime.Now.Ticks - t3) / TimeSpan.TicksPerSecond
        '        done = True
        '    Else

        'lower_upper_lemire(buffer, ep, _R, l_buff, u_buff)
        lower_upper_lemire(buffer, _QueryLength - 1, _R, l_buff, u_buff)

        ' Do main task here..
        ex = 0
        ex2 = 0
        For i = 0 To buffer.Length - 1 ' ep - 1

            ' A bunch of data has been read and pick one of them at a time to use
            Dim d As Double = buffer(i)

            ' Calcualte sum and sum square
            ex += d
            ex2 += d * d

            ' t is a circular array for keeping current data
            t(i Mod _QueryLength) = d

            ' double the size for avoiding using modulo "%" operator
            t((i Mod _QueryLength) + _QueryLength) = d

            ' Start the task when there are more than m-1 points in the current chunk
            If i >= _QueryLength - 1 Then

                mean = ex / _QueryLength
                std = ex2 / _QueryLength
                std = CDbl(Math.Sqrt(std - mean * mean))

                ' compute the start location of the data in the current circular array, t
                j = (i + 1) Mod _QueryLength
                ' the start location of the data in the current chunk
                startIndex = i - (_QueryLength - 1)

                ' Use a constant lower bound to prune the obvious subsequence
                lb_kim = lb_kim_hierarchy(t, _Query, j, _QueryLength, mean, std, bsf)

                'Dim tttt As Double = (DateTime.Now.Ticks - t3) / TimeSpan.TicksPerSecond

                If lb_kim < bsf Then

                    lb_kim__lt__bsf += 1 ' <---------

                    ' Use a linear time lower bound to prune; z_normalization of t will be computed on the fly.
                    ' uo, lo are envelop of the query.
                    lb_k = lb_keogh_cumulative(_OrderIndex, t, _UEnv, _LEnv, cb1, j, _QueryLength, mean, std, bsf)

                    If lb_k < bsf Then

                        lb_k__lt__bsf += 1 ' <---------------

                        ' Take another linear time to compute z_normalization of t.
                        ' Note that for better optimization, this can merge to the previous function.
                        For k = 0 To _QueryLength - 1
                            tz(k) = (t((k + j)) - mean) / std
                        Next

                        ' Use another lb_keogh to prune
                        ' qo is the sorted query. tz is unsorted z_normalized data.
                        ' l_buff, u_buff are big envelop for all data in this chunk

                        lb_k2 = lb_keogh_data_cumulative(_OrderIndex, tz, _QueryOrder, cb2, l_buff, u_buff, startIndex, _QueryLength, mean, std, bsf)

                        If lb_k2 < bsf Then

                            lb_k2__lt__bsf += 1 ' <-------------------

                            ' Choose better lower bound between lb_keogh and lb_keogh2 to be used in early abandoning DTW
                            ' Note that cb and cb2 will be cumulative summed here.
                            If lb_k > lb_k2 Then
                                cb(_QueryLength - 1) = cb1(_QueryLength - 1)
                                For k = _QueryLength - 2 To 0 Step -1
                                    cb(k) = cb(k + 1) + cb1(k)
                                Next
                            Else
                                cb(_QueryLength - 1) = cb2(_QueryLength - 1)
                                For k = _QueryLength - 2 To 0 Step -1
                                    cb(k) = cb(k + 1) + cb2(k)
                                Next
                            End If

                            ' Compute DTW and early abandoning if possible
                            dist = dtw(tz, _Query, cb, _QueryLength, _R, bsf)

                            If dist < bsf Then

                                dist__lt__bsf += 1 ' <-------------------

                                ' Update bsf
                                ' loc is the real starting location of the nearest neighbor in the file
                                bsf = dist
                                loc = (it) * (EPOCH - _QueryLength + 1) + i - _QueryLength + 1
                            End If
                        Else
                            keogh2 += 1
                        End If
                    Else
                        keogh += 1
                    End If
                Else
                    kim += 1
                End If

                'Dim ttt As Double = (DateTime.Now.Ticks - t3) / TimeSpan.TicksPerSecond

                ' Reduce obsolute points from sum and sum square
                ex -= t(j)
                ex2 -= t(j) * t(j)

            End If

        Next



        ' If the size of last chunk is less then EPOCH, then no more data and terminate.
        'If ep < EPOCH Then
        '    done = True
        'Else
        '    it += 1
        'End If
        '    End If
        'End While

        i = (it) * (EPOCH - _QueryLength + 1) + ep

        't2 = DateTime.Now.Ticks
        tt = (DateTime.Now.Ticks - t1) / TimeSpan.TicksPerSecond

        Debug.WriteLine("{0}, {1}, {2}, {3}, It: {10}, Loc: {4}, D: {5:n}, S: {6}, P1: {7:P2}, P2: {8:P2}, P3: {9:P2}", lb_kim__lt__bsf, lb_k__lt__bsf, lb_k2__lt__bsf, dist__lt__bsf, loc, Math.Sqrt(bsf), tt, (kim / i), (keogh / i), (keogh2 / i), it)

        '' Note that loc and i are long long.
        'Debug.WriteLine("Loc : {0}, Dist: {1}" & loc)
        'Debug.WriteLine("Distance : " & Math.Sqrt(bsf))
        'Console.WriteLine("Data Scanned : " & i__1)
        'Debug.WriteLine("Total Execution Time : " & tt & " sec")

        '' printf is just easier for formating ;)
        'Console.WriteLine()
        'Console.WriteLine("Pruned by LB_Kim : {0:P2}", (CDbl(kim) / i__1))
        'Console.WriteLine("Pruned by LB_Keogh : {0:P2}", (CDbl(keogh) / i__1))
        'Console.WriteLine("Pruned by LB_Keogh2 : {0:P2}", (CDbl(keogh2) / i__1))
        'Console.WriteLine("DTW Calculation : {0:P2}", 1 - ((CDbl(kim) + keogh + keogh2) / i__1))

        Return Math.Sqrt(bsf)

    End Function

End Class

Friend Structure IndexValue
    Public Index As Integer
    Public Value As Double
End Structure

Friend Class IndexComparer
    Implements IComparer(Of IndexValue)

    Public Function Compare(x As IndexValue, y As IndexValue) As Integer Implements IComparer(Of IndexValue).Compare
        Return Math.Abs(y.Value).CompareTo(Math.Abs(x.Value))
    End Function

End Class
