Imports System.ComponentModel
''' <summary>
''' http://www.gutgames.com/post/Image-Dilation-in-C.aspx
''' </summary>
''' <remarks></remarks>
Public Class DilationFilter
    Inherits TimeSeriesPostProcessor

    Private _Size As Integer = 4

    Private _ApetureMin As Integer = -(_Size \ 2)
    Private _ApetureMax As Integer = (_Size \ 2)

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        Dim xLen As Integer = series.Samples(0).Length
        Dim yLen As Integer = series.Samples.Length

        For x1 As Integer = 0 To xLen - 1
            For y1 As Integer = 0 To yLen - 1

                Dim val As Double = 0

                For x2 As Integer = _ApetureMin To _ApetureMax - 1

                    Dim x As Integer = x1 + x2
                    If x >= 0 AndAlso x < xLen Then

                        For y2 As Integer = _ApetureMin To _ApetureMax - 1

                            Dim y As Integer = y1 + y2
                            If y >= 0 AndAlso y < yLen Then
                                If series.Samples(y)(x) > val AndAlso series.Samples(y)(x) <> val Then
                                    val = series.Samples(y)(x)
                                End If
                            End If

                        Next

                    End If
                Next

                series.Samples(y1)(x1) = val - series.Samples(y1)(x1)

            Next
        Next

        Return series.Samples

    End Function

    Private Sub AllocateApetures()
        _ApetureMin = -(_Size \ 2)
        _ApetureMax = (_Size \ 2)
    End Sub

    <Description("Apeture size."), DefaultValue(4)>
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
            Return "Dilation Filter"
        End Get
    End Property

End Class
