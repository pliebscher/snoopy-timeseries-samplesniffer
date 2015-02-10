''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class ThresholdCrossing
    Inherits TimeSeriesPreprocessor

    Private _Threshold As Double = 0.01

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        Dim dps As Double() = series.Samples
        Dim count As Integer = dps.Length

        Dim min As Double = series.Min
        Dim max As Double = series.Max
        Dim avg As Double = series.Mean

        Dim i As Integer = 0

        While i < count
            Dim dp As Double = dps(i)
            If dp <= _Threshold Then
                series.Samples(i) = 0
                i += 1
            Else
                'Dim start As Double = i

                i += 1
                While i < count AndAlso dps(i) > _Threshold
                    series.Samples(i) = 0
                    i += 1
                End While

                dp = dps(i - 1)

                i += 1
            End If
        End While

        Return series.Samples
    End Function

    Public Property Threshold As Double
        Get
            Return _Threshold
        End Get
        Set(value As Double)
            If _Threshold <> value Then
                _Threshold = value
            End If
        End Set
    End Property

End Class
