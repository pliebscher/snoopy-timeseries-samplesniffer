Imports System.ComponentModel
''' <summary>
''' Time Series Threshold Similarity
''' http://www.dbs.ifi.lmu.de/Publikationen/Papers/paper-edbt06_final.pdf
''' </summary>
''' <remarks></remarks>
<DefaultProperty("Threshold")>
Public Class TSTSQuery
    Inherits TimeSeriesQuery

    Private _TCTIS As List(Of DataPoint)  'TimeSeries
    Private _Threshold As Double

    Public Sub New(criteria As TimeSeries)
        MyBase.New(criteria)
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double

        Dim crit As List(Of DataPoint) = _TCTIS 'MyBase.Criteria
        Dim data2 As List(Of DataPoint) = GetTCTIS(data)

        Dim len1 As Integer = crit.Count
        Dim len2 As Integer = data2.Count

        Dim min1() As Double = New Double(len1 - 1) {}
        Dim min2() As Double = New Double(len2 - 1) {}

        Dim sum1 As Double = 0
        Dim sum2 As Double = 0

        For i As Integer = 0 To len1 - 1
            min1(i) = Double.PositiveInfinity
        Next
        For j As Integer = 0 To len2 - 1
            min2(j) = Double.PositiveInfinity
        Next

        For i As Integer = 0 To len1 - 1
            'min1(i) = Double.PositiveInfinity

            Dim x1 As Double = 1 / (i + 1) 'data1.getPoint(i).getX()
            Dim y1 As Double = crit(i).Y  'data1.getPoint(i).getY()

            For j As Integer = 0 To len2 - 1
                'min2(j) = Double.PositiveInfinity

                Dim x2 As Double = 1 / (j + 1) 'data2.getPoint(j).getX()
                Dim y2 As Double = data.Samples(j) 'data2.getPoint(j).getY()

                Dim x As Double = x1 - x2
                Dim y As Double = y1 - y2

                Dim dist As Double = Math.Sqrt(x * x + y * y)

                If dist < min1(i) Then
                    min1(i) = dist
                End If
                If dist < min2(j) Then
                    min2(j) = dist
                End If
                'sum2 += min2(j)
            Next
            sum1 += min1(i)
        Next

        For j As Integer = 0 To len2 - 1
            sum2 += min2(j)
        Next

        Dim result As Double = 0

        If len1 > 0 Then
            result += sum1 / len1
        End If
        If len2 > 0 Then
            result += sum2 / len2
        End If

        Return result

    End Function

    ' Fetch the Threshold-Crossing Time intervals...
    Private Function GetTCTIS(series As TimeSeries) As List(Of DataPoint)  'TimeSeries

        Dim dps As Double() = series.Samples
        Dim TCTdps As New List(Of DataPoint) ' Double() = New Double(dps.Length - 1) {}

        Dim count As Integer = dps.Length

        Dim i As Integer = 0

        While i < count
            Dim dp As Double = dps(i)
            If dp <= _Threshold Then

                series.Samples(i) = 0
                i += 1
                'dp = 0.0
            Else
                Dim start As Double = i

                i += 1
                While i < count AndAlso dps(i) > _Threshold
                    series.Samples(i) = 0
                    i += 1
                End While

                dp = dps(i - 1)

                'TCTdps.addPoint(New DataPoint(start, dp.getX()))
                TCTdps.Add(New DataPoint(start, dp))

                i += 1
            End If
            'TCTdps.Add(dp)
        End While

        Return TCTdps ' TCTdps.ToArray().ToTimeSeries(MyBase.Criteria.SampleRate) ' New Serie(TCTdps)

    End Function

    Protected Overrides Sub OnQueryInit(query As TimeSeries)
        _Threshold = Criteria.Mean
        _TCTIS = GetTCTIS(Criteria)
        MyBase.OnQueryInit(query)
    End Sub

    Protected Overrides Sub OnQueryUpdate(query As TimeSeries)
        _Threshold = Criteria.Mean
        _TCTIS = GetTCTIS(Criteria)
    End Sub

    Protected Overrides Sub OnQueryReset(query As TimeSeries)
        _Threshold = Criteria.Mean
        _TCTIS = GetTCTIS(Criteria)
    End Sub

    Protected Overrides Sub OnQueryCriteriaChanged(criteria As TimeSeries)
        _TCTIS = GetTCTIS(criteria)
    End Sub

    <Description("Threshold")>
    Public Property Threshold As Double
        Get
            Return _Threshold
        End Get
        Set(value As Double)
            If _Threshold <> value Then
                _Threshold = value
                _TCTIS = GetTCTIS(Criteria)
            End If
        End Set
    End Property

    Private Structure DataPoint
        Dim X As Double
        Dim Y As Double
        Public Sub New(x As Double, y As Double)
            Me.X = x
            Me.Y = y
        End Sub
    End Structure

    Public Overrides ReadOnly Property Name As String
        Get
            Return "TSTS (Pre)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Time Series Threshold Similarity"
        End Get
    End Property

End Class
