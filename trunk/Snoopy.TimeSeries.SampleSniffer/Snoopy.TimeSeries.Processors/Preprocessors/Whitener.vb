''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Whitener
    Inherits TimeSeriesPreprocessor

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()

        Dim p As Integer = 40
        Dim R As Double() = New Double(p + 1) {}
        Dim Xo As Double() = New Double(p + 1) {}
        Dim ai As Double() = New Double(p + 1) {}
        Dim Whitened As Double() = New Double(series.Samples.Length - 1) {}

        Dim i, j As Integer
        Dim T As Double = 8D
        Dim alpha As Double = 1.0 / T
        Dim E As Double = 0D
        Dim ki As Double = 0D

        For i = 0 To p
            R(i) = 0D
            Xo(i) = 0D
            ai(i) = 0D
        Next

        R(0) = 0.001

        For i = 0 To p
            Dim acc As Double = 0
            For j = i To series.Samples.Length - 1
                ' acc += _pSamples[j+start] * _pSamples[j-i+start];
                acc += series.Samples(j) * series.Samples(j - i)
            Next
            R(i) += alpha * (acc - R(i))
        Next

        E = R(0)
        For i = 1 To p
            Dim sumalphaR As Double = 0
            For j = 1 To i - 1
                ' sumalphaR += _ai[j]*_R[i-j];
                sumalphaR += ai(j) * R(i - j)
            Next

            ki = (R(i) - sumalphaR) / E
            ai(i) = ki

            For j = 1 To i \ 2
                Dim aj As Double = ai(j)
                Dim aimj As Double = ai(i - j)
                ai(j) = aj - ki * aimj
                ai(i - j) = aimj - ki * aj
            Next

            E = (1 - ki * ki) * E

        Next

        For i = 0 To series.Samples.Length - 1
            Dim acc As Double = series.Samples(i)
            Dim minip As Integer = i

            If p < minip Then
                minip = p
            End If

            For j = i + 1 To p
                acc -= ai(j) * Xo(p + i - j)
            Next

            For j = 1 To minip
                acc -= ai(j) * series.Samples(i - j)
            Next

            Whitened(i) = acc

        Next

        Return Whitened

    End Function

End Class
