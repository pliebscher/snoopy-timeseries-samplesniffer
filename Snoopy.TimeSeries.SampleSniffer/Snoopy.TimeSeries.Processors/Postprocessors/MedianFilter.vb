Imports System.ComponentModel
''' <summary>
''' http://www.gutgames.com/post/Noise-Reduction-of-an-Image-in-C-using-Median-Filters.aspx
''' </summary>
''' <remarks></remarks>
Public Class MedianFilter
    Inherits TimeSeriesPostProcessor

    Private _Size As Integer = 4

    Private _ApetureMin As Integer = -(_Size \ 2)
    Private _ApetureMax As Integer = (_Size \ 2)

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        Dim xLen As Integer = series.Samples(0).Length
        Dim yLen As Integer = series.Samples.Length

        Dim Values As New List(Of Double)()

        For x1 As Integer = 0 To xLen - 1
            For y1 As Integer = 0 To yLen - 1

                For x2 As Integer = _ApetureMin To _ApetureMax - 1

                    Dim x As Integer = x1 + x2
                    If x >= 0 AndAlso x < xLen Then

                        For y2 As Integer = _ApetureMin To _ApetureMax - 1

                            Dim y As Integer = y1 + y2
                            If y >= 0 AndAlso y < yLen Then
                                Values.Add(series.Samples(y)(x))
                            End If

                        Next

                    End If
                Next

                Values.Sort(AbsComparer.Instance)
                series.Samples(y1)(x1) = series.Samples(y1)(x1) - Values(Values.Count \ 2)
                Values.Clear()

            Next
        Next

        Return series.Samples

    End Function

    Private Sub AllocateApetures()
        _ApetureMin = -(_Size \ 2)
        _ApetureMax = (_Size \ 2)
    End Sub

    <Description("Apeture size. Should be an even value."), DefaultValue(4)>
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

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Median Filter: Sorts neighbors and takes the median."
        End Get
    End Property

End Class
