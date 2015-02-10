Public Class TimeSeriesEventArgs
    Inherits EventArgs

    Private _TimeSeries As TimeSeries

    Public Sub New(query As TimeSeries)
        _TimeSeries = query
    End Sub

    Public ReadOnly Property TimeSeries As TimeSeries
        Get
            Return _TimeSeries
        End Get
    End Property

End Class
