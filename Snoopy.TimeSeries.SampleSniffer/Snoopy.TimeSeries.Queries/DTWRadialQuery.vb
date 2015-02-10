Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<DefaultProperty("Radius")>
Public Class DTWRadialQuery
    Inherits TimeSeriesQuery

    Private _MaxX As Integer = 0
    Private _MaxY As Integer = 0

    Private _Radius As Integer = 6

    Private _CriteriaRect As Double()()

    Public Sub New(query As TimeSeries)
        MyBase.New(query)
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double
        If data.Energy <= 0 Then Return 0

        Dim maxX As Integer = 0
        Dim maxY As Integer = 0

        For i As Integer = 0 To data.Frames.Length - 1
            For j As Integer = 0 To data.Frame(0).Length - 1
                If data.Frames(i)(j) > data.Frames(maxX)(maxY) Then
                    maxX = i
                    maxY = j
                End If
            Next
        Next

        Dim dRect As Double()() = GetRect(maxX, maxY, _Radius, data.Frames)

        Dim path As DTWPath = DTW.Calculate(dRect, _CriteriaRect, _Metric, _Direction)
        Dim dist As Double = (1 / (1 + path.Distance))


        'drawCircle(maxX, maxY, _Radius, data.Frames)

        'SetVal(maxX, maxY, data.Frames, 0)

        Return dist
    End Function

    Protected Overrides Sub OnQueryInit(criteria As TimeSeries)
        ResetParams()
        MyBase.OnQueryInit(criteria)
    End Sub

    Protected Overrides Sub OnQueryReset(criteria As TimeSeries)
        _Metric = Metric.Manhattan
        _MetricType = Metric.MetricType.Manhattan
        _Direction = DTWDirection.Neighbors
        _Radius = 6
        ResetParams()
        MyBase.OnQueryReset(criteria)
    End Sub

    Protected Overrides Sub OnQueryUpdate(criteria As TimeSeries)
        ResetParams()
        MyBase.OnQueryUpdate(criteria)
    End Sub

    Private Sub ResetParams()

        _MaxX = 0
        _MaxY = 0

        ' Find max co'ords
        For i As Integer = 0 To Criteria.Frames.Length - 1
            For j As Integer = 0 To Criteria.Frame(0).Length - 1
                If Criteria.Frames(i)(j) > Criteria.Frames(_MaxX)(_MaxY) Then
                    _MaxX = i
                    _MaxY = j
                End If
            Next
        Next

        _CriteriaRect = GetRect(_MaxX, _MaxY, _Radius, Criteria.Frames)

        'drawCircle(_MaxX, _MaxY, _Radius, Criteria.Frames)

        'SetVal(_MaxX, _MaxY, Criteria.Frames, 0)

    End Sub

    Private Function GetRect(centerX As Integer, centerY As Integer, radius As Integer, image As Double()()) As Double()()
        Dim X1 As Integer = (centerX - radius)
        Dim Y1 As Integer = (centerY - radius)

        Dim rect As Double()() = New Double(2 * radius)() {}
        Dim rx As Integer = 0
        Dim ry As Integer = 0

        For x As Integer = X1 To centerX + radius '- 1 ' Col

            rect(rx) = New Double(2 * radius) {}

            For y As Integer = Y1 To centerY + radius '- 1 ' Row
                rect(rx)(ry) = GetVal(x, y, image)
                ry += 1
            Next
            ry = 0
            rx += 1
        Next
        Return rect
    End Function

    Private Sub drawRect(centerX As Integer, centerY As Integer, radius As Integer, image As Double()())

        Dim X1 As Integer = (centerX - radius)
        Dim Y1 As Integer = (centerY - radius)

        For x As Integer = X1 To centerX + radius '- 1 ' Col
            For y As Integer = Y1 To centerY + radius '- 1 ' Row
                SetVal(x, y, image, 2)
            Next
        Next

    End Sub

    Private Sub drawCircle(centerX As Integer, centerY As Integer, radius As Integer, image As Double()())

        Dim d As Integer = (5 - radius * 4) \ 4
        Dim x As Integer = 0
        Dim y As Integer = radius

        Dim val As Double = 2

        Do  ' Working clock-wise...
            ' 1 - Upper right
            SetVal(centerX + x, centerY + y, image, val)
            ' 2
            SetVal(centerX + y, centerY + x, image, val)
            ' 3
            SetVal(centerX + y, centerY - x, image, val)
            ' 4
            SetVal(centerX + x, centerY - y, image, val)
            ' 5
            SetVal(centerX - x, centerY - y, image, val)
            ' 6
            SetVal(centerX - y, centerY - x, image, val)
            ' 7
            SetVal(centerX - y, centerY + x, image, val)
            ' 8 - Upper left
            SetVal(centerX - x, centerY + y, image, val)

            If d < 0 Then
                d += 2 * x + 1
            Else
                d += 2 * (x - y) + 1
                y -= 1
            End If
            x += 1
        Loop While x <= y

    End Sub

    Private Function GetParimeterVect(centerX As Integer, centerY As Integer, radius As Integer, image As Double()()) As Double()

        Dim d As Integer = (5 - radius * 4) \ 4
        Dim x As Integer = 0
        Dim y As Integer = radius

        Do
            SetVal(centerX + x, centerY + y, image, 0)
            SetVal(centerX + x, centerY - y, image, 0)
            SetVal(centerX - x, centerY + y, image, 0)
            SetVal(centerX - x, centerY - y, image, 0)
            SetVal(centerX + y, centerY + x, image, 0)
            SetVal(centerX + y, centerY - x, image, 0)
            SetVal(centerX - y, centerY + x, image, 0)
            SetVal(centerX - y, centerY - x, image, 0)
            If d < 0 Then
                d += 2 * x + 1
            Else
                d += 2 * (x - y) + 1
                y -= 1
            End If
            x += 1
        Loop While x <= y

        Return Nothing ' supressing compiler warning for now.
    End Function

    Private Sub SetVal(x As Integer, y As Integer, image As Double()(), val As Double)
        If x < 0 Then Exit Sub
        If x > image.Length - 1 Then Exit Sub
        If y < 0 Then Exit Sub
        If y > image(x).Length - 1 Then Exit Sub
        image(x)(y) *= val
    End Sub

    Private Function GetVal(x As Integer, y As Integer, image As Double()()) As Double
        If x < 0 Then Return 0
        If x > image.Length - 1 Then Return 0
        If y < 0 Then Return 0
        If y > image(x).Length - 1 Then Return 0
        Return image(x)(y)
    End Function


    Private _Metric As Metric = Metric.Manhattan
    Private _MetricType As Metric.MetricType = Metric.MetricType.Manhattan
    Private _Direction As DTWDirection = DTWDirection.Neighbors

    <Description("Metric"), DefaultValue(Metric.MetricType.Manhattan)>
    Public Property MetricType As Metric.MetricType
        Get
            Return _MetricType
        End Get
        Set(value As Metric.MetricType)
            If value <> _MetricType Then
                _MetricType = value
                _Metric = Metric.Get(_MetricType)
            End If
        End Set
    End Property

    <Description("Warp direction"), DefaultValue(DTWDirection.Neighbors)>
    Public Property Direction As DTWDirection
        Get
            Return _Direction
        End Get
        Set(value As DTWDirection)
            If value <> _Direction Then
                _Direction = value
            End If
        End Set
    End Property

    <Description(""), DefaultValue(6)>
    Public Property Radius As Integer
        Get
            Return _Radius
        End Get
        Set(value As Integer)
            If value <> _Radius Then
                _Radius = value
                ResetParams()
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "Radial Max (Post)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Performs DTW'ing for the given radius around the center of mass."
        End Get
    End Property


End Class
