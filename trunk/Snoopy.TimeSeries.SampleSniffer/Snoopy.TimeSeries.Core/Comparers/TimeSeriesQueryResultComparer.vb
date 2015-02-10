Public Class TimeSeriesQueryResultComparer
    Implements IComparer(Of TimeSeriesQueryResult), IEqualityComparer(Of TimeSeriesQueryResult)

    Private Shared ReadOnly _Instance As New TimeSeriesQueryResultComparer

    ''' <summary>
    '''   Compare two query results by TimeStamp and Score.
    ''' </summary>
    ''' <param name = "a">First item</param>
    ''' <param name = "b">Second item</param>
    ''' <returns>a.Score &gt; b.Score = 1, a.Data.TimeStamp = b.Data.TimeStamp + a.Score = b.Score = 0, a.Score &lt; b.Score = -1</returns>
    Public Function Compare(ByVal a As TimeSeriesQueryResult, ByVal b As TimeSeriesQueryResult) As Integer Implements IComparer(Of TimeSeriesQueryResult).Compare
        If a.Data.TimeStamp = b.Data.TimeStamp AndAlso a.Score = b.Score Then
            Return 0
        ElseIf a.Score > b.Score Then
            Return 1
        Else
            Return -1
        End If
    End Function

    Public Shared ReadOnly Property Instance As TimeSeriesQueryResultComparer
        Get
            Return _Instance
        End Get
    End Property

    Public Shadows Function Equals(a As TimeSeriesQueryResult, b As TimeSeriesQueryResult) As Boolean Implements IEqualityComparer(Of TimeSeriesQueryResult).Equals
        If a.Data.GetHashCode = b.Data.GetHashCode AndAlso a.Data.TimeStamp = b.Data.TimeStamp AndAlso a.Score = b.Score Then
            Return True
        End If
        Return False
    End Function

    Public Shadows Function GetHashCode(obj As TimeSeriesQueryResult) As Integer Implements IEqualityComparer(Of TimeSeriesQueryResult).GetHashCode
        Dim src As Integer = obj.Score.GetHashCode Xor obj.Data.TimeStamp.GetHashCode Xor obj.Data.GetHashCode
        Return src.GetHashCode
    End Function

End Class
