Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<DefaultProperty("Count")>
Public Class CentroidExtractor
    Inherits TimeSeriesPostProcessor

    '* Neighbours x coordinates of search kernel.
    Private dx As Integer() = {+1, +1, 0, -1, -1, -1, 0, +1}
    '* Neighbours y coordinates of search kernel.
    Private dy As Integer() = {0, -1, -1, -1, 0, +1, +1, 0}

    Private _Count As Integer = 6
    Private _MinNeighbors As Integer = 1

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        Dim rows As Integer = series.Samples.Length
        Dim cols As Integer = series.Samples(0).Length

        Dim CentroidMap As Integer()() = New Integer(rows - 1)() {}

        For i As Integer = 0 To rows - 1
            Dim indices As Integer() = GetCentroidIndices(series.Samples(i))
            CentroidMap(i) = New Integer(cols - 1) {}
            For j As Integer = 0 To cols - 1
                If indices.Contains(j) Then
                    CentroidMap(i)(j) = 1
                End If
            Next
        Next

        ' TODO: Maybe able to do this in the above loop by keeping Last, Current & Next indices arrays...
        For i As Integer = 0 To rows - 1
            For j As Integer = 0 To cols - 1

                If CentroidMap(i)(j) = 1 Then
                    Dim sum As Integer = 0
                    For k As Integer = 0 To 7
                        Dim x As Integer = i + dx(k)
                        Dim y As Integer = j + dy(k)
                        If x < 0 Then x = 0 ' Handle edges.
                        If x > rows - 1 Then x = rows - 1 ' ...
                        If y < 0 Then y = 0 ' ...
                        If y > cols - 1 Then y = cols - 1 ' ...
                        sum += CentroidMap(x)(y)
                    Next
                    If sum < _MinNeighbors Then
                        series.Samples(i)(j) = 0
                    End If
                Else
                    series.Samples(i)(j) = 0
                End If
            Next
        Next

        Return series.Samples

    End Function

    Private Function GetCentroidIndices(window As Double()) As Integer()

        Dim sorted As Double() = New Double(window.Length - 1) {}
        Dim indexes As Integer() = Enumerable.Range(0, window.Length).ToArray()

        If window.Sum = 0 Then Return New Integer(window.Length - 1) {}

        Array.Copy(window, sorted, window.Length)
        Array.Sort(sorted, indexes, AbsComparer.Instance)

        Return indexes.Take(_Count).ToArray

    End Function

    <Description("Number of top samples per window to extracts"), DefaultValue(6)>
    Public Property Count As Integer
        Get
            Return _Count
        End Get
        Set(value As Integer)
            If value <> _Count Then
                _Count = value
            End If
        End Set
    End Property

    <Description("Minimum number of neighbors required to be included."), DefaultValue(1)>
    Public Property MinNeighbors As Integer
        Get
            Return _MinNeighbors
        End Get
        Set(value As Integer)
            If value <> _MinNeighbors Then
                _MinNeighbors = value
            End If
        End Set
    End Property

End Class
