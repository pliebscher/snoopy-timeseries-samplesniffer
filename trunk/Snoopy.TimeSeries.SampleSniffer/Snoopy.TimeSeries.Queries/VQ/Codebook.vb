''' <summary>
''' http://ocvolume.sourceforge.net
''' </summary>
''' <remarks></remarks>
Public Class Codebook

    Private _MaxCentroids As Integer = 256

    Private _SplitFactor As Double = 0.01 ' split factor (should be in the range of 0.01 <= SPLIT <= 0.05)
    Private _MinDistortion As Double = 0.5
    Private _Distortion As Double
    Private _CoordLength As Integer

    Private _Centroids As List(Of Centroid)
    Private _TrainingPoints As List(Of PointVector)

    Private _Distance As Metric

    Public Sub New(trainingPoints As List(Of PointVector), distFunc As Metric)
        Me.New(trainingPoints, 0.01, 0.5, distFunc)
    End Sub

    Public Sub New(trainingPoints As List(Of PointVector), splitFactor As Double, minDistortion As Double, distFunc As Metric)
        If trainingPoints Is Nothing Then Throw New ArgumentNullException("trainingPoints")
        If trainingPoints.Count = 0 Then Throw New ArgumentException("trainingPoints list is empty!")
        If distFunc Is Nothing Then Throw New ArgumentNullException("distFunc")

        _SplitFactor = splitFactor
        _MinDistortion = minDistortion

        _TrainingPoints = trainingPoints
        _CoordLength = _TrainingPoints(0).Vector.Length
        _Distance = distFunc

        Dim PreDistortion As Double = 0
        Dim Orgin As Double() = New Double(_CoordLength - 1) {}

        _Centroids = New List(Of Centroid)(1)
        _Centroids.Add(New Centroid(Orgin, 0))

        For i As Integer = 0 To _TrainingPoints.Count - 1
            _Centroids(0).AddPoint(_TrainingPoints(i), 0)
        Next

        _Centroids(0).Update()

        Dim it As Integer
        Dim it2 As Integer

        While (_Centroids.Count * 2) < trainingPoints.Count

            If it = 1024 OrElse _Centroids.Count >= _MaxCentroids Then Exit While

            SplitCentroids()
            GroupTrainingPointsToNearestCentroid()

            Do
                If it2 > 1024 Then Exit Do

                For i As Integer = 0 To _Centroids.Count - 1
                    PreDistortion += _Centroids(i).Distortion
                    _Centroids(i).Update()
                Next

                GroupTrainingPointsToNearestCentroid()

                For i As Integer = 0 To _Centroids.Count - 1
                    _Distortion += _Centroids(i).Distortion
                Next

                it2 += 1

            Loop While (Math.Abs(_Distortion - PreDistortion) <> 0 AndAlso Math.Abs(_Distortion - PreDistortion) < _MinDistortion)

            it += 1

        End While

        'Debug.WriteLine(String.Format("VQ Iterations: {0}, {1} | Centroids: {2} | Distortion: {3}", it, it2, _Centroids.Count, _Distortion))

    End Sub

    Public Function Quantize(points As List(Of PointVector)) As Integer()
        Dim quant As Integer() = New Integer(points.Count - 1) {}
        For i As Integer = 0 To points.Count - 1
            quant(i) = GetClosestCentroidToPoint(points(i))
        Next
        Return quant
    End Function

    Public Function GetDistortion(points As List(Of PointVector)) As Double
        Dim dist As Double = 0
        For i As Integer = 0 To points.Count - 1
            Dim index As Integer = GetClosestCentroidToPoint(points(i))
            dist += GetPointToCentroidDistance(points(i), _Centroids(index))
        Next

        'Return dist / (points.Count + _TrainingPoints.Count) 'Math.Abs((points.Count / dist) - 1)
        Return Math.Sqrt(dist / ((points.Count + _TrainingPoints.Count) * (points.Count + _TrainingPoints.Count)))
    End Function

    Public Function GetCentroidPointDistribution() As Integer()
        Dim distro As Integer() = New Integer(_Centroids.Count - 1) {}
        For i As Integer = 0 To _Centroids.Count - 1
            distro(i) = _Centroids(i).PointCount
        Next
        Return distro
    End Function

    Private Sub SplitCentroids()

        Dim SplitSet As New List(Of Centroid)(_Centroids.Count - 1)
        Dim PosCoords As Double()
        Dim NegCoords As Double()
        'Dim index As Integer

        For i As Integer = 0 To (_Centroids.Count - 1) '* 2 Step 2
            PosCoords = New Double(_CoordLength - 1) {}
            NegCoords = New Double(_CoordLength - 1) {}

            For j As Integer = 0 To _CoordLength - 1
                PosCoords(j) = _Centroids(i).Vector(j) * (1 + _SplitFactor)
                NegCoords(j) = _Centroids(i).Vector(j) * (1 - _SplitFactor)
            Next

            SplitSet.Add(New Centroid(PosCoords, i))
            SplitSet.Add(New Centroid(NegCoords, -i))
            'index += 1
        Next

        '_Centroids.Clear()
        '_Centroids.AddRange(SplitSet)
        _Centroids = SplitSet

    End Sub

    Private Sub GroupTrainingPointsToNearestCentroid()

        For i As Integer = 0 To _TrainingPoints.Count - 1
            Dim index As Integer = GetClosestCentroidToPoint(_TrainingPoints(i))
            Dim distance As Double = GetPointToCentroidDistance(_TrainingPoints(i), _Centroids(index))
            ' centroids[index].add(pt[i], getDistance(pt[i], centroids[index]));
            _Centroids(index).AddPoint(_TrainingPoints(i), distance)
        Next

        For i As Integer = 0 To _Centroids.Count - 1
            If _Centroids(i).PointCount = 0 Then
                Dim index As Integer = GetClosestCentroidToCentroid(_Centroids(i))
                Dim closestPointIndex As Integer = GetClosestPoint(_Centroids(i), _Centroids(index))
                ' point closestPt = centroids[index].getPoint(closestIndex)
                Dim closestPoint As PointVector = _Centroids(index).GetPoint(closestPointIndex)

                'centroids[index].remove(closestPt, getDistance(closestPt, centroids[index]))
                _Centroids(index).RemovePoint(closestPoint, GetPointToCentroidDistance(closestPoint, _Centroids(index)))

                'centroids[i].add(closestPt, getDistance(closestPt, centroids[i]))
                _Centroids(i).AddPoint(closestPoint, GetPointToCentroidDistance(closestPoint, _Centroids(i)))

            End If
        Next

    End Sub

    Private Function GetClosestCentroidToPoint(point As PointVector) As Integer
        Dim dist As Double = 0
        Dim low_dist As Double = Double.MaxValue
        Dim low_idx As Integer
        For i As Integer = 0 To _Centroids.Count - 1
            ' tmp_dist = getDistance(c, centroids[i]);
            dist = GetPointToCentroidDistance(point, _Centroids(i))
            ' if (tmp_dist < lowest_dist && centroids[i].getNumPts() > 1)
            If dist < low_dist Then ' Or i = 0 Then
                low_dist = dist
                low_idx = i
            End If
        Next
        Return low_idx
    End Function

    Private Function GetClosestCentroidDistance(point As PointVector) As Double
        Dim dist As Double = 0
        Dim low_dist As Double = Double.MaxValue
        For i As Integer = 0 To _Centroids.Count - 1
            ' tmp_dist = getDistance(c, centroids[i]);
            dist = GetPointToCentroidDistance(point, _Centroids(i))
            ' if (tmp_dist < lowest_dist && centroids[i].getNumPts() > 1)
            If dist < low_dist Then ' Or i = 0 Then
                low_dist = dist
            End If
        Next
        Return low_dist
    End Function

    Private Function GetClosestCentroidToCentroid(centroid As Centroid) As Integer
        Dim dist As Double = 0
        Dim low_dist As Double = Double.MaxValue
        Dim low_idx As Integer
        For i As Integer = 0 To _Centroids.Count - 1
            ' tmp_dist = getDistance(c, centroids[i]);
            dist = GetPointToCentroidDistance(centroid, _Centroids(i))
            ' if (tmp_dist < lowest_dist && centroids[i].getNumPts() > 1)
            If dist < low_dist AndAlso _Centroids(i).PointCount > 1 Then
                low_dist = dist
                low_idx = i
            End If
        Next
        Return low_idx
    End Function

    Private Function GetClosestPoint(centroid1 As Centroid, centroid2 As Centroid) As Integer
        Dim dist As Double = 0
        Dim low_dist As Double = Double.MaxValue
        Dim low_indx As Integer
        For i As Integer = 0 To centroid2.PointCount - 1
            ' dist = getDistance(c2.getPoint(i), c1)
            dist = GetPointToCentroidDistance(centroid2.GetPoint(i), centroid1)
            If dist < low_dist Then
                low_dist = dist
                low_indx = i
            End If
        Next
        Return low_indx
    End Function

    Private Function GetPointToCentroidDistance(point As PointVector, centroid As Centroid) As Double
        'Dim dist As Double
        'Dim sum As Double
        'For i As Integer = 0 To _CoordLength - 1
        '    sum = point.Coordinates(i) - centroid.Coordinates(i)
        '    dist += sum * sum
        'Next
        'Return Math.Sqrt(dist)
        Return _Distance.Compute(point.Vector, centroid.Vector)
    End Function

    Public ReadOnly Property TrainingPoints As List(Of PointVector)
        Get
            Return _TrainingPoints
        End Get
    End Property

    Public ReadOnly Property Centroids As List(Of Centroid)
        Get
            Return _Centroids
        End Get
    End Property

    Public ReadOnly Property Distortion As Double
        Get
            Return _Distortion
        End Get
    End Property

End Class
