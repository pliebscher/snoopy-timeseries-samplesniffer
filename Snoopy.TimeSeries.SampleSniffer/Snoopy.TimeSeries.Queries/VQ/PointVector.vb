<Serializable>
Public Class PointVector

    Private _Vector As Double()
    Private _Index As Integer

    Public Sub New(coordinates As Double(), index As Integer)
        _Vector = coordinates
        _Index = index
    End Sub

    Public ReadOnly Property Vector As Double()
        Get
            Return _Vector
        End Get
    End Property

    Public ReadOnly Property Index As Integer
        Get
            Return _Index
        End Get
    End Property

    Public Shared Operator =(a As PointVector, b As PointVector) As Boolean
        Dim aCoord As Double() = a.Vector
        Dim bCoord As Double() = b.Vector
        For i As Integer = 0 To aCoord.Length - 1
            If aCoord(i) <> bCoord(i) Then Return False
        Next
        Return True
    End Operator

    Public Shared Operator <>(a As PointVector, b As PointVector) As Boolean
        Dim aCoord As Double() = a.Vector
        Dim bCoord As Double() = b.Vector
        For i As Integer = 0 To aCoord.Length - 1
            If aCoord(i) <> bCoord(i) Then Return True
        Next
        Return False
    End Operator

End Class
