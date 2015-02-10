Imports System.ComponentModel
''' <summary>
''' http://code.google.com/p/glycoworkbench/source/browse/src/org/eurocarbdb/application/glycoworkbench/plugin/peakpicker/TopHatFilter.java
''' </summary>
''' <remarks></remarks>
Public Class Erosion
    Inherits TimeSeriesPostProcessor

    Private _Len As Integer = 4

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        'Dim no_points As Integer = series.Samples(0).Length
        'Dim first_mz As Double = series.Samples(0)(0)
        'Dim last_mz As Double = series.Samples(0)(no_points - 1)
        'Dim spacing As Double = (last_mz - first_mz) / CDbl(no_points)
        'Dim struc_elem_no_points As Integer = CInt(Math.Ceiling(0.001 / spacing))

        '' the number has to be odd
        'struc_elem_no_points += ((struc_elem_no_points + 1) Mod 2)

        Dim eroded As Double()() = Erode(series.Samples, _Len)

        Return eroded
    End Function

    Private Shared Function Erode(data As Double()(), l As Integer) As Double()()

        Dim first As Integer = 0
        Dim last As Integer = data(0).Length

        Dim length As Integer = data(0).Length
        Dim middle As Integer = l \ 2

        Dim results As Double()() = New Double(1)() {}
        results(0) = New Double(length - 1) {}
        results(1) = New Double(length - 1) {}

        Dim g As Double() = New Double(l - 1) {}
        Dim h As Double() = New Double(l - 1) {}
        Dim k As Integer = length - (length Mod l) - 1

        calcGErosion(data, first, last, l, g, True)
        calcHErosion(data, first + l - 1, l, h, True)

        Dim i As Integer
        Dim it As Integer = 0
        i = 0
        While i < middle
            results(0)(it) = data(0)(first)
            results(1)(it) = 0.0
            i += 1
            it += 1
            first += 1
        End While

        Dim m As Integer = l - 1
        Dim n As Integer = 0
        i = middle
        While i < (length - middle)
            If (i Mod l) = (middle + 1) Then
                If i = k Then
                    calcGErosion(data, (first + middle), last, l, g, False)
                Else
                    calcGErosion(data, (first + middle), last, l, g, True)
                End If
                m = 0
            End If
            If (i Mod l) = middle AndAlso (i > middle) Then
                If i > k Then
                    calcHErosion(data, (first + middle), l, h, False)
                Else
                    calcHErosion(data, (first + middle), l, h, True)
                End If
                n = 0
            End If

            results(0)(it) = data(0)(first)
            results(1)(it) = Math.Min(g(m), h(n))

            i += 1
            it += 1
            first += 1
            m += 1
            n += 1
        End While

        i = 0
        While i < middle
            results(0)(it) = data(0)(first)
            results(1)(it) = 0.0
            i += 1
            it += 1
            first += 1
        End While

        Return results

    End Function

    ''' Compute the auxiliary fields g and h for the erosion
    Private Shared Sub calcGErosion(data As Double()(), first As Integer, last As Integer, l As Integer, g As Double(), b As [Boolean])
        Dim i As Integer, j As Integer

        If b Then
            For j = 0 To l - 1
                If first < last Then
                    If j = 0 Then
                        g(j) = data(1)(first)
                    Else
                        g(j) = Math.min(data(1)(first), g(j - 1))
                    End If
                    first += 1
                Else
                    Exit For
                End If
            Next
        Else
            j = 0
            While first <> last
                If j = 0 Then
                    g(j) = data(1)(first)
                Else
                    g(j) = Math.min(data(1)(first), g(j - 1))
                End If
                first += 1
                j += 1
            End While

            For i = j To l - 1
                g(i) = 0
            Next
        End If
    End Sub

    Private Shared Sub calcHErosion(data As Double()(), first As Integer, l As Integer, h As Double(), b As [Boolean])

        Dim j As Integer
        If b Then
            For j = l - 1 To 0 Step -1
                If j = (l - 1) Then
                    h(j) = data(1)(first)
                Else
                    h(j) = Math.min(data(1)(first), h(j + 1))
                End If
                first -= 1
            Next
        Else
            For j = 0 To l - 1
                h(j) = 0
            Next
        End If
    End Sub

    <Description("Structuring element of length."), DefaultValue(4)>
    Public Property Length As Integer
        Get
            Return _Len
        End Get
        Set(value As Integer)
            If value <> _Len Then
                _Len = value
            End If
        End Set
    End Property

End Class
