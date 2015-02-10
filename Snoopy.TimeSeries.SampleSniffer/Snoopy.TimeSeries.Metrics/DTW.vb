Public Class DTW

    ' Calculate Dynamic Time Wrapping distance
    ' A,B: data and query, respectively
    ' cb : cummulative bound used for early abandoning
    ' r : size of Sakoe-Chiba warpping band
    'Public Shared Function Calculate(A As Double(), B As Double()) As Double ', cb1 As Double()) As Double ', m As Integer, r1 As Integer, distance As Distance, Optional bsf As Double = Double.PositiveInfinity) As Double

    '    Dim r As Integer = A.Length - 1 ' warping window
    '    Dim m As Integer = A.Length - 1 ' size of the query
    '    Dim bsf As Double = Double.PositiveInfinity

    '    Dim cost As Double()
    '    Dim cost_prev As Double()
    '    Dim cost_tmp As Double()
    '    Dim i As Integer, j As Integer, k As Integer
    '    Dim x As Double, y As Double, z As Double, min_cost As Double

    '    cost = New Double(2 * r) {}
    '    cost_prev = New Double(2 * r) {}

    '    For k = 0 To 2 * r
    '        cost(k) = Double.PositiveInfinity
    '        cost_prev(k) = Double.PositiveInfinity
    '    Next

    '    For i = 0 To m - 1
    '        k = Math.Max(0, r - i)
    '        min_cost = Double.PositiveInfinity

    '        j = Math.Max(0, i - r)
    '        While j <= Math.Min(m - 1, i + r)
    '            ' Initialize all row and column
    '            If (i = 0) AndAlso (j = 0) Then
    '                cost(k) = (A(0) - B(0)) * (A(0) - B(0)) 'distance.Compute(A(0), B(0))
    '                min_cost = cost(k)
    '                'Continue While
    '            End If

    '            If (j - 1 < 0) OrElse (k - 1 < 0) Then
    '                y = Double.PositiveInfinity
    '            Else
    '                y = cost(k - 1)
    '            End If
    '            If (i - 1 < 0) OrElse (k + 1 > 2 * r) Then
    '                x = Double.PositiveInfinity
    '            Else
    '                x = cost_prev(k + 1)
    '            End If
    '            If (i - 1 < 0) OrElse (j - 1 < 0) Then
    '                z = Double.PositiveInfinity
    '            Else
    '                z = cost_prev(k)
    '            End If

    '            ' Classic DTW calculation
    '            cost(k) = Math.Min(Math.Min(x, y), z) + (A(i) - B(j)) * (A(i) - B(j)) 'dist(A(i), B(j))

    '            ' Find minimum cost in row for early abandoning (possibly to use column instead of row).
    '            If cost(k) < min_cost Then
    '                min_cost = cost(k)
    '            End If
    '            j += 1
    '            k += 1
    '        End While

    '        ' We can abandon early if the current cummulative distace with lower bound together are larger than bsf
    '        'If i + r < m - 1 AndAlso min_cost + cb(i + r + 1) >= bsf Then
    '        '    Return min_cost + cb(i + r + 1)
    '        'End If

    '        ' Move current array to previous array.
    '        cost_tmp = cost
    '        cost = cost_prev
    '        cost_prev = cost_tmp
    '    Next
    '    k -= 1

    '    ' the DTW distance is in the last cell in the matrix of size O(m^2) or at the middle of our array.
    '    Dim final_dtw As Double = cost_prev(k)

    '    Return final_dtw
    'End Function

    Public Shared Function Calculate(a As Double(), b As Double(), distFunc As Metric, direction As DTWDirection) As DTWPath

        Dim aLen As Integer = a.Length
        Dim bLen As Integer = b.Length

        Dim points As DTWPoint()() = New DTWPoint(aLen - 1)() {}
        Dim path As New DTWPath

        For i As Integer = 0 To aLen - 1
            points(i) = New DTWPoint(bLen - 1) {}
            For j As Integer = 0 To bLen - 1
                'points(i)(j) = New DTWPoint With {.X = i, .Y = j, .LocalDistance = distFunc.Compute(a(i), a(i), b(j), b(j))}
                ' TODO: Norm LocalDistance
            Next
        Next

        Dim top As DTWPoint
        Dim bottom As DTWPoint
        Dim center As DTWPoint
        Dim last As DTWPoint

        For i As Integer = 1 To aLen - 1

            For j As Integer = 1 To bLen - 1

                center = points(i - 1)(j - 1)

                If direction = DTWDirection.Neighbors Then
                    top = points(i - 1)(j)
                    bottom = points(i)(j - 1)
                Else

                    If i > 1 And j > 1 Then
                        top = points(i - 2)(j - 1)
                        bottom = points(i - 1)(j - 2)
                    Else
                        top = points(i - 1)(j)
                        bottom = points(i)(j - 1)
                    End If

                End If

                If top.AccumDistance < center.AccumDistance Then
                    last = top
                Else
                    last = center
                End If

                If bottom.AccumDistance < last.AccumDistance Then
                    last = bottom
                End If

                points(i)(j).AccumDistance = points(i)(j).LocalDistance + last.AccumDistance
                points(i)(j).Last = last

            Next

        Next

        last = points(aLen - 1)(bLen - 1)
        Dim distance As Double = last.AccumDistance

        'If Double.IsNaN(distance) Then distance = 0

        If direction = DTWDirection.Neighbors Then
            distance /= a.Length + b.Length
        Else
            distance /= Math.Sqrt(aLen * aLen + bLen * bLen)
        End If

        path.Distance = distance

        While last IsNot Nothing
            path.Steps.Add(last)
            last = last.Last
        End While

        Return path

    End Function

    Public Shared Function CalculateCostMatrix(a As Double()(), b As Double()(), distFunc As Metric) As Double()()
        Dim aLen As Integer = a.Length
        Dim bLen As Integer = b.Length
        Dim dist As Double()() = New Double(aLen - 1)() {}

        For i As Integer = 0 To aLen - 1
            dist(i) = New Double(bLen - 1) {}
            For j As Integer = 0 To bLen - 1
                dist(i)(j) = distFunc.Compute(a(i), b(j))
            Next
        Next

        Return dist
    End Function

    ''' <summary>
    ''' http://code.google.com/p/fastdtw/
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <param name="distFunc"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CalculateWarpCostBetween(a As Double()(), b As Double()(), distFunc As Metric) As Double

        If a.Length < b.Length Then
            Return CalculateWarpCostBetween(b, a, distFunc)
        End If

        ' double[] lastCol = new double[tsJ.size()];
        ' double[] currCol = new double[tsJ.size()];
        ' final int maxI = tsI.size()-1;
        ' final int maxJ = tsJ.size()-1;

        Dim lastCol As Double() = New Double(b.Length) {}
        Dim currCol As Double() = New Double(b.Length) {}
        Dim maxI As Integer = a.Length - 1
        Dim maxJ As Integer = b.Length - 1

        ' // Calculate the values for the first column, from the bottom up.
        ' currCol[0] = distFn.calcDistance(tsI.getMeasurementVector(0), tsJ.getMeasurementVector(0));  // first cell
        ' for (int j=1; j<=maxJ; j++)  // the rest of the first column
        '   currCol[j] = currCol[j-1] + distFn.calcDistance(tsI.getMeasurementVector(0), tsJ.getMeasurementVector(j));

        currCol(0) = distFunc.Compute(a(0), b(0))
        For j As Integer = 1 To maxJ
            currCol(j) = currCol(j - 1) + distFunc.Compute(a(0), b(j))
        Next

        'for (int i=1; i<=maxI; i++)   // i = columns
        '{
        '   // Swap the references between the two arrays.
        '   final double[] temp = lastCol;
        '   lastCol = currCol;
        '   currCol = temp;

        '   // Calculate the value for the bottom row of the current column
        '   //    (i,0) = LocalCost(i,0) + GlobalCost(i-1,0)
        '   currCol[0] = lastCol[0] + distFn.calcDistance(tsI.getMeasurementVector(i), tsJ.getMeasurementVector(0));

        '   for (int j=1; j<=maxJ; j++)  // j = rows
        '   {
        '      // (i,j) = LocalCost(i,j) + minGlobalCost{(i-1,j),(i-1,j-1),(i,j-1)}
        '      final double minGlobalCost = Math.min(lastCol[j], Math.min(lastCol[j-1], currCol[j-1]));
        '      currCol[j] = minGlobalCost + distFn.calcDistance(tsI.getMeasurementVector(i), tsJ.getMeasurementVector(j));
        '   }  // end for loop
        '}  // end for loop

        '// Minimum Cost is at (maxI,maxJ)
        'return currCol[maxJ];

        For i As Integer = 1 To maxI

            Dim temp As Double() = lastCol
            lastCol = currCol
            currCol = temp

            currCol(0) = lastCol(0) + distFunc.Compute(a(i), b(0))

            For j As Integer = 1 To maxJ
                Dim minGlobalCost As Double = Math.Min(lastCol(j), Math.Min(lastCol(j - 1), currCol(j - 1)))
                currCol(j) = minGlobalCost + distFunc.Compute(a(i), b(j))
            Next

        Next

        Return currCol(maxJ)

    End Function

    ' TODO: Forward/outward looking algo...
    Public Shared Function Calculate(a As Double()(), b As Double()(), distFunc As Metric, direction As DTWDirection) As DTWPath

        Dim aLen As Integer = a.Length
        Dim bLen As Integer = b.Length

        Dim points As DTWPoint()() = New DTWPoint(aLen - 1)() {}
        Dim path As New DTWPath

        For i As Integer = 0 To aLen - 1
            points(i) = New DTWPoint(bLen - 1) {}
            For j As Integer = 0 To bLen - 1
                points(i)(j) = New DTWPoint With {.X = i, .Y = j, .LocalDistance = distFunc.Compute(a(i), b(j))}
            Next
        Next

        Dim top As DTWPoint
        Dim bottom As DTWPoint
        Dim center As DTWPoint
        Dim last As DTWPoint

        For i As Integer = 1 To aLen - 1

            For j As Integer = 1 To bLen - 1

                center = points(i - 1)(j - 1)

                If direction = DTWDirection.Neighbors Then
                    top = points(i - 1)(j)
                    bottom = points(i)(j - 1)
                Else

                    If i > 1 And j > 1 Then
                        top = points(i - 2)(j - 1)
                        bottom = points(i - 1)(j - 2)
                    Else
                        top = points(i - 1)(j)
                        bottom = points(i)(j - 1)
                    End If

                End If

                If top.AccumDistance < center.AccumDistance Then
                    last = top
                Else
                    last = center
                End If

                If bottom.AccumDistance < last.AccumDistance Then
                    last = bottom
                End If

                points(i)(j).AccumDistance = points(i)(j).LocalDistance + last.AccumDistance
                points(i)(j).Last = last

            Next

        Next

        last = points(aLen - 1)(bLen - 1)
        Dim distance As Double = last.AccumDistance

        If direction = DTWDirection.Neighbors Then
            distance /= aLen + b.Length
        Else
            distance /= Math.Sqrt(aLen * aLen + bLen * bLen)
        End If

        path.Distance = distance

        While last IsNot Nothing
            path.Steps.Add(last)
            last = last.Last
        End While

        Return path

    End Function

    Private Shared Function snorm(x As Double(), length As Integer, alpha As Double) As Double()
        ' Get the number of vectors in the sequence
        Dim n As Integer = x.Length \ length

        ' Create the augmented sequence projection
        Dim xs As Double() = New Double(x.Length + (n - 1)) {}

        ' For each vector in the sequence
        For j As Integer = 0 To n - 1
            ' Compute its starting position in the
            '  source and destination sequences
            Dim src As Integer = j * length
            Dim dst As Integer = j * (length + 1)

            ' Compute augmented vector norm
            Dim norm As Double = alpha * alpha
            For k As Integer = src To src + (length - 1)
                norm += x(k) * x(k)
            Next
            norm = System.Math.Sqrt(norm)

            ' Normalize the augmented vector and
            '  copy to the destination sequence
            xs(dst + length) = alpha / norm
            Dim i As Integer = dst
            While i < dst + length
                xs(i) = x(src) / norm
                i += 1
                src += 1
            End While
        Next

        Return xs
        
    End Function

End Class

Public Class DTWPoint
    Public X As Integer
    Public Y As Integer
    Public LocalDistance As Double
    Public AccumDistance As Double
    Public Last As DTWPoint
End Class

Public Class DTWPath
    Public Distance As Double
    Public Steps As New List(Of DTWPoint)
End Class

Public Enum DTWDirection
    Neighbors
    Diagonals
End Enum