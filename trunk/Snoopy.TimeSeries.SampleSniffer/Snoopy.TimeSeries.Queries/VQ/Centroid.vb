<Serializable>
Public Class Centroid
    Inherits PointVector

    Private _Points As New List(Of PointVector)
    Private _PointCount As Integer
    Private _Distortion As Double = 0.0

    Public Sub New(coordinates As Double(), index As Integer)
        MyBase.New(coordinates, index)
    End Sub

    Public Sub AddPoint(point As PointVector, distortion As Double)
        _Points.Add(point)
        _Distortion += distortion
        _PointCount += _PointCount
    End Sub

    Public Sub RemovePoint(point As PointVector, distortion As Double)
        _Points.Remove(point)
        _Distortion -= distortion
        _PointCount -= _PointCount
    End Sub

    Public Function GetPoint(index As Integer) As PointVector
        Return _Points(index)
    End Function

    ''' <summary>
    ''' update Centroid by taking average of all points in the cell
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Update()

        Dim coordSum As Double() = New Double(MyBase.Vector.Length - 1) {}

        For i As Integer = 0 To _Points.Count - 1

        Next

        For Each p As PointVector In _Points
            For k As Integer = 0 To p.Vector.Length - 1
                coordSum(k) += p.Vector(k) ' column avg??
            Next
        Next

        ' Move centroid to average of all points...
        For k As Integer = 0 To MyBase.Vector.Length - 1
            MyBase.Vector(k) = coordSum(k) / _Points.Count
        Next

        _Points.Clear()
        _Distortion = 0
        _PointCount = 0
    End Sub

    Public ReadOnly Property Distortion As Double
        Get
            Return _Distortion
        End Get
    End Property

    Public ReadOnly Property PointCount As Integer
        Get
            Return _Points.Count
        End Get
    End Property

End Class
