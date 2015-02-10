Public Class FormTimeSeriesViewer

    Private _TimeSeries As TimeSeries

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Public ReadOnly Property ViewControl As TimeSeriesViewer
        Get
            Return TimeSeriesViewer
        End Get
    End Property

End Class