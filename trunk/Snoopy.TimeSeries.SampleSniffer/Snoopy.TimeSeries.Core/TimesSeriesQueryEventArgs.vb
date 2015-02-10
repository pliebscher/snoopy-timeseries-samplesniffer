Public Class TimeSeriesQueryEventArgs
    Inherits EventArgs

    Private _Query As TimeSeriesQuery

    Public Sub New(query As TimeSeriesQuery)
        _Query = query
    End Sub

    Public ReadOnly Property Query As TimeSeriesQuery
        Get
            Return _Query
        End Get
    End Property

End Class
