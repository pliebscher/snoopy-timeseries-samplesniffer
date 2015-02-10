Imports System.ComponentModel
''' <summary>
''' Symmetric Nearest Neighbor filter - http://www.gutgames.com/post/Symmetric-Nearest-Neighbor-in-C.aspx
''' </summary>
''' <remarks></remarks>
Public Class SNNFilter
    Inherits TimeSeriesPostProcessor

    Private _Size As Integer = 4

    Private _ApetureMinX As Integer = -(_Size \ 2)
    Private _ApetureMaxX As Integer = (_Size \ 2)
    Private _ApetureMinY As Integer = -(_Size \ 2)
    Private _ApetureMaxY As Integer = (_Size \ 2)

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        Dim xLen As Integer = series.Samples(0).Length
        Dim yLen As Integer = series.Samples.Length

        For x As Integer = 0 To xLen - 1
            For y As Integer = 0 To yLen - 1

                Dim Value As Double = 0
                Dim NumPixels As Integer = 0

                For x2 As Integer = _ApetureMinX To _ApetureMaxX - 1

                    Dim TempX1 As Integer = x + x2
                    Dim TempX2 As Integer = x - x2

                    If TempX1 >= 0 AndAlso TempX1 < xLen AndAlso TempX2 >= 0 AndAlso TempX2 < xLen Then

                        For y2 As Integer = _ApetureMinY To _ApetureMaxY - 1

                            Dim TempY1 As Integer = y + y2
                            Dim TempY2 As Integer = y - y2

                            If TempY1 >= 0 AndAlso TempY1 < yLen AndAlso TempY2 >= 0 AndAlso TempY2 < yLen Then

                                Try
                                    Dim a As Double = series.Samples(y)(x)
                                    Dim b As Double = series.Samples(TempY1)(TempX1)
                                    Dim c As Double = series.Samples(TempY2)(TempX2) ' + 4)

                                    If b > a Then
                                        Value += b
                                    Else
                                        Value += c
                                    End If

                                Catch ex As Exception
                                    Throw
                                End Try
                                
                                NumPixels += 1

                            End If

                        Next

                    End If
                Next

                series.Samples(y)(x) = Value / NumPixels

            Next
        Next

        Return series.Samples

    End Function

    Private Sub AllocateApetures()
        _ApetureMinX = -(_Size \ 2)
        _ApetureMaxX = (_Size \ 2)
        _ApetureMinY = -(_Size \ 2)
        _ApetureMaxY = (_Size \ 2)
    End Sub

    <Description("Apeture size. Should be an even number and greater then Zero."), DefaultValue(4)>
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

    <Description("Symmetric Nearest Neighbor filter")>
    Public Overrides ReadOnly Property Description As String
        Get
            Return "Symmetric Nearest Neighbor"
        End Get
    End Property

End Class
