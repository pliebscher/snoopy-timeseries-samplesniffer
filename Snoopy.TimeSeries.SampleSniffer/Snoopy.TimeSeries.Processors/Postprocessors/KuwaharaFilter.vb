Imports System.ComponentModel
''' <summary>
''' http://www.gutgames.com/post/Kuwahara-Filter-in-C.aspx
''' </summary>
''' <remarks></remarks>
Public Class KuwaharaFilter
    Inherits TimeSeriesPostProcessor

    Private _Size As Integer = 5

    Private _ApetureMinX As Integer() = {-(_Size \ 2), 0, -(_Size \ 2), 0}
    Private _ApetureMaxX As Integer() = {0, (_Size \ 2), 0, (_Size \ 2)}
    Private _ApetureMinY As Integer() = {-(_Size \ 2), -(_Size \ 2), 0, 0}
    Private _ApetureMaxY As Integer() = {0, 0, (_Size \ 2), (_Size \ 2)}

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        For x As Integer = 0 To series.Samples(0).Length - 1
            For y As Integer = 0 To series.Samples.Length - 1

                Dim values As Double() = {0, 0, 0, 0}
                Dim nums As Integer() = {0, 0, 0, 0}
                Dim max As Double() = {Double.NegativeInfinity, Double.NegativeInfinity, Double.NegativeInfinity, Double.NegativeInfinity}
                Dim min As Double() = {Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity}

                For i As Integer = 0 To 3
                    For x2 As Integer = _ApetureMinX(i) To _ApetureMaxX(i) - 1

                        Dim TempX As Integer = x + x2
                        If TempX >= 0 AndAlso TempX < series.Samples(0).Length - 1 Then '.Width Then

                            For y2 As Integer = _ApetureMinY(i) To _ApetureMaxY(i) - 1

                                Dim TempY As Integer = y + y2
                                If TempY >= 0 AndAlso TempY < series.Samples.Length - 1 Then

                                    Dim val As Double = series.Samples(TempY)(TempX)

                                    values(i) += val

                                    If val > max(i) Then
                                        max(i) = val
                                    ElseIf val < min(i) Then
                                        min(i) = val
                                    End If

                                    nums(i) += 1

                                End If

                            Next

                        End If

                    Next
                Next

                Dim j As Integer = 0

                Dim MinDifference As Double = Double.PositiveInfinity

                For i As Integer = 0 To 3
                    Dim CurrentDifference As Double = (max(i) - min(i))
                    If CurrentDifference < MinDifference AndAlso nums(i) > 0 Then
                        j = i
                        MinDifference = CurrentDifference
                    End If
                Next

                series.Samples(y)(x) = values(j) / nums(j)

            Next
        Next

        Return series.Samples

    End Function

    Private Sub AllocateApetures()
        _ApetureMinX = New Integer() {-(_Size \ 2), 0, -(_Size \ 2), 0}
        _ApetureMaxX = New Integer() {0, (_Size \ 2), 0, (_Size \ 2)}
        _ApetureMinY = New Integer() {-(_Size \ 2), -(_Size \ 2), 0, 0}
        _ApetureMaxY = New Integer() {0, 0, (_Size \ 2), (_Size \ 2)}
    End Sub

    <Description("Apeture size."), DefaultValue(5)>
    Public Property Size As Integer
        Get
            Return _Size
        End Get
        Set(value As Integer)
            If value < 1 Then Throw New ArgumentException("Size must be greater then Zero.")
            If value <> _Size Then
                _Size = value
                AllocateApetures()
            End If
        End Set
    End Property

    <Description("http://www.gutgames.com/post/Kuwahara-Filter-in-C.aspx")>
    Public Overrides ReadOnly Property Description As String
        Get
            Return "Kuwahara Filter"
        End Get
    End Property

End Class
