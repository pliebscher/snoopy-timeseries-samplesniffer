Imports System.Runtime.CompilerServices

Public Module TimeSeriesExtentions

    Private Sub Test()

        Dim SampleRate As Integer = 22050
        Dim Samples As Double() = New Double(2047) {}

        Dim Series As New TimeSeries(Samples, SampleRate)

        ' Manipulate your samples...
        Dim Min As Double = -1
        Dim Max As Double = 1
        For i As Integer = 0 To Series.Samples.Length - 1
            ' Lets scale each sample to no less than -1 and no greater then +1.
            Series.Samples(i) = Min + (Series.Samples(i) - Series.Min) / (Series.Max - Series.Min) * (Max - Min)
        Next

        ' Not happy with the results, so let's start over...
        Series.Reset()

        ' Now let's make a clean copy so we can have somthing to compare with...
        Dim Series2 As TimeSeries = Series.Clone

        ' We may want to work with some smaller time series...
        Dim SplitCount As Integer = 3
        Dim SplitOverlap As Integer = 0 ' This is usefull for a number of things discussed later.

        Dim SeriesList As List(Of TimeSeries) = Series2.Split(SplitCount, SplitOverlap)

        ' And we may want to put Humpty back together again...
        Dim Series3 As TimeSeries = Series2

        For i As Integer = 0 To SeriesList.Count - 1
            Series3 = SeriesList(i).Join(Series3)
        Next

        ' And remember, the properties are lazy so they'll be reset and _ 
        ' recalculated the next time their accessed.

        ' Lastly we should probably save our work...

        Series3.Save("d:\series3.ts")

        ' and start over again...
        Dim Series4 As TimeSeries = TimeSeries.Load("d:\series3.ts")




    End Sub

    <Extension>
    Public Function ToTimeSeries(samples As Double(), sampleRate As Integer) As TimeSeries
        Return New TimeSeries(samples, sampleRate)
    End Function

    ''' <summary>
    ''' http://jaudio.cvs.sourceforge.net/viewvc/jaudio/jAudio1.0/src/cern/jet/stat/Descriptive.java
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension>
    Public Function Covariance(a As TimeSeries, b As TimeSeries) As Double
        Dim len As Integer = a.Samples.Length
        If len <> b.Samples.Length Then Throw New ArgumentException(String.Format("TimesSeries must be of the same length. a={0}, b={1}", len, b.Samples.Length))
        Dim sumX As Double = a.Samples(0)
        Dim sumY As Double = b.Samples(0)
        Dim sumXY As Double = 0

        For i As Integer = 1 To len - 1
            Dim x As Double = a.Samples(i)
            Dim y As Double = b.Samples(i)
            sumX += x
            sumXY += (x - sumX / (i + 1)) * (y - sumY / i)
            sumY += y
        Next

        Return sumXY / (len - 1)
    End Function

    ''' <summary>
    ''' Returns the correlation of two data sequences.
    ''' http://jaudio.cvs.sourceforge.net/viewvc/jaudio/jAudio1.0/src/cern/jet/stat/Descriptive.java
    ''' </summary>
    ''' <param name="b"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension>
    Public Function Correlation(a As TimeSeries, b As TimeSeries) As Double
        Return Covariance(a, b) / (a.StdDev * b.StdDev)
    End Function

    ''' <summary>
    ''' http://jaudio.cvs.sourceforge.net/viewvc/jaudio/jAudio1.0/src/cern/jet/stat/Descriptive.java
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension>
    Public Function VarianceFrom(a As TimeSeries, b As TimeSeries) As Double
        Return (a.Samples.Length * a.Variance + b.Samples.Length * b.Variance) / (a.Samples.Length + b.Samples.Length)
    End Function

    ''' <summary>
    ''' http://jaudio.cvs.sourceforge.net/viewvc/jaudio/jAudio1.0/src/cern/jet/stat/Descriptive.java
    ''' </summary>
    ''' <param name="series"></param>
    ''' <param name="phi">The percentage; Must satisfy 0 &lt;= phi &lt;= 1</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension>
    Public Function Quantile(series As TimeSeries, phi As Double) As Double

        Dim n As Integer = series.Samples.Length
        Dim index As Double = phi * (n - 1)
        Dim lhs As Integer = CInt(index)
        Dim delta As Double = index - lhs
        Dim sortedSamples As Double() = New Double(n - 1) {}

        Array.Copy(series.Samples, sortedSamples, n)
        Array.Sort(sortedSamples, AbsComparer.Instance)

        If (lhs = n - 1) Then
            Return sortedSamples(lhs)
        End If

        Return (1 - delta) * sortedSamples(lhs) + delta * sortedSamples(lhs + 1)

    End Function

    ''' <summary>
    ''' Returns the sum of logarithms of a data sequence, which is: Sum(Log(series.Samples(i))
    ''' </summary>
    ''' <param name="series"></param>
    ''' <param name="fromIndex">index of the first data element (inclusive)</param>
    ''' <param name="toIndex">index of the last data element (inclusive)</param>
    ''' <returns></returns>
    ''' <remarks>http://jaudio.cvs.sourceforge.net/viewvc/jaudio/jAudio1.0/src/cern/jet/stat/Descriptive.java</remarks>
    <Extension>
    Public Function LogSum(series As TimeSeries, fromIndex As Integer, toIndex As Integer) As Double
        Dim sum As Double = 0.0
        If fromIndex < 0 Then fromIndex = 0
        If toIndex > series.Samples.Length Then toIndex = series.Samples.Length - 1
        For i As Integer = fromIndex - 1 To toIndex
            sum += Math.Log(series.Samples(i))
        Next
        Return sum
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="series"></param>
    ''' <param name="kernel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension>
    Public Function Convolve(series As ITimeSeries(Of Double()()), kernel As Double()()) As Double()()
        Return Convolve(series.Samples, kernel)
    End Function

    <Extension>
    Public Function Convolve1(series As Double()(), kernel As Double()()) As Double()()
        Dim xn As Integer, yn As Integer
        Dim average As Double

        ' Showcasing how to access width and height of nested array
        Dim w As Integer = series.Length
        Dim h As Integer = series(0).Length
        'float[][] output = new float[w][h];
        Dim output As Double()() = New Double(w - 1)() {}

        ' Iterate through image pixels
        For x As Integer = 0 To w - 1
            output(x) = New Double(h - 1) {}
            For y As Integer = 0 To h - 1

                ' Iterate through kernel values to get weighted average
                average = 0
                For u As Integer = 0 To kernel.Length - 1
                    For v As Integer = 0 To kernel(0).Length - 1

                        ' Get associated neighbor pixel coordinates
                        xn = x + u - kernel.Length \ 2
                        yn = y + v - kernel(0).Length \ 2

                        ' Make sure we don't go off of an edge of the image
                        'xn = constrain(xn, 0, w - 1)
                        If xn < 0 Then xn = 0
                        If xn > w - 1 Then xn = w - 1

                        'yn = constrain(yn, 0, h - 1)
                        If yn < 0 Then yn = 0
                        If yn > y - 1 Then yn = h - 1

                        Try
                            ' Add weighted neighbor to average
                            average += series(xn)(yn) * kernel(u)(v)
                        Catch ex As Exception
                            Throw
                        End Try



                    Next
                Next
                ' Set output pixel to weighted average value
                output(x)(y) = average
            Next
        Next
        Return output
    End Function

    <Extension>
    Public Function Convolve(series As Double()(), kernel As Double()()) As Double()()

        Dim kernelH As Integer = kernel.Length
        Dim kernelW As Integer = kernel(0).Length

        If kernelH <> kernelW Then
            Throw New Exception("Kernel must be square.")
        End If

        Dim kernelSize As Integer = kernel.Length \ 2 'GetLength(0) \ 2
        Dim height As Integer = series.Length 'table.GetLength(0)
        Dim width As Integer = series(0).Length '.GetLength(1)
        Dim wTable As Double()() = CreateBorders(series, kernelSize, kernelSize)
        Dim res As Double()() = New Double(height - 1)() {} ', width - 1) {}

        For i As Integer = kernelSize To height + (kernelSize - 1)
            res(i - kernelSize) = New Double(width - 1) {}
            For j As Integer = kernelSize To width + (kernelSize - 1)

                Dim left As Integer = j - kernelSize
                Dim right As Integer = j + kernelSize
                Dim top As Integer = i - kernelSize
                Dim bottom As Integer = i + kernelSize
                Dim convValue As Double = 0.0

                For i1 As Integer = top To bottom - 1
                    Dim x As Integer = 0
                    Dim y As Integer = i1 - top
                    Dim j1 As Integer = left
                    While j1 <= right - 1 ' While j1 <= right

                        Try
                            convValue += wTable(i1)(j1) * kernel(y)(x)
                        Catch ex As Exception
                            Throw
                        End Try

                        j1 += 1
                        x += 1
                    End While
                Next

                'res(i - kernelSize, j - kernelSize) = ValidateIntensityValue(CInt(Math.Truncate(convValue)))
                res(i - kernelSize)(j - kernelSize) = convValue
            Next
        Next

        Return res
    End Function

    Private Function CreateBorders(src As Double()(), hBorder As Integer, wBorder As Integer) As Double()()
        Dim height As Integer = src.Length
        Dim width As Integer = src(0).Length
        Dim resH As Integer = height + 2 * hBorder
        Dim resW As Integer = width + 2 * wBorder
        Dim res As Double()() = New Double(resH - 1)() {} ' (resH - 1, resW - 1) {}

        For i As Integer = 0 To hBorder - 1
            res(i) = New Double(resW - 1) {}
            res((resH - 1) - i) = New Double(resW - 1) {}
        Next

        For i As Integer = 0 To height - 1
            Dim idxI As Integer = i + hBorder
            Dim idxJ As Integer = wBorder
            Dim j As Integer = 0
            res(idxI) = New Double(resW - 1) {}
            While j < width
                res(idxI)(idxJ) = src(i)(j)
                j += 1
                idxJ += 1
            End While
        Next

        Return res
    End Function

    ''' <summary>
    ''' Converts the TimeSeries Samples array into a Byte array.
    ''' </summary>
    ''' <param name="series"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension>
    Public Function ToByteArray(series As TimeSeries) As Byte()
        Return series.ToByteArray(0, series.Samples.Length - 1)
    End Function

    ''' <summary>
    ''' Converts a section of the TimeSeries Samples array into a Byte array.
    ''' </summary>
    ''' <param name="series"></param>
    ''' <param name="startIndex"></param>
    ''' <param name="endIndex"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension>
    Public Function ToByteArray(series As TimeSeries, startIndex As Integer, endIndex As Integer) As Byte()

        Dim len As Integer = (endIndex - startIndex)
        Dim tsBytes As Byte() = New Byte(len * 2) {}

        For i As Integer = 0 To len - 1
            Dim shorty As Short = ToShort(series.Samples(i + startIndex))
            Dim bytes As Byte() = BitConverter.GetBytes(shorty)
            tsBytes(2 * i) = bytes(0)
            tsBytes(2 * i + 1) = bytes(1)
        Next
        Return tsBytes
    End Function

    Private Function ToShort(x As Double) As Short
        x *= 32768.0 '- 1
        Return CShort(If(x > Double.MaxValue, Double.MaxValue, If(x < Double.MinValue, Double.MinValue, x)))
    End Function

End Module
