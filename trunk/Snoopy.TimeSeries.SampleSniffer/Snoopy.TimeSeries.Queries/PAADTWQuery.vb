Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ComponentModel

<Serializable>
Public Class PAADTWQuery
    Inherits TimeSeriesQuery

    Private _PAAQuery As Double()
    Private _PAALength As Integer = 1024
    Private _DTWAlpha As Double = 1
    Private _DTWLength As Integer = 256
    Private _DTWDegree As Integer = 1
    Private _DTWK As DTWAccord

    Public Sub New(query As TimeSeries)
        MyBase.New(query)
        GeneratePAAQuery()
        InitDTW()
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double
        Dim dataPAA As Double() = Transform(data.Samples, _PAALength)
        Dim dist As Double = _DTWK.Compute(_PAAQuery, dataPAA)
        Return Math.Abs(dist)
    End Function

    Protected Overrides Sub OnQueryUpdate(query As TimeSeries)
        GeneratePAAQuery()
    End Sub

    Protected Overrides Sub OnQueryCriteriaChanged(criteria As TimeSeries)
        GeneratePAAQuery()
    End Sub

    Private Sub GeneratePAAQuery()
        _PAAQuery = Transform(MyBase.Criteria.Samples, _PAALength)
    End Sub

    Private Sub InitDTW()
        _DTWK = New DTWAccord(_DTWLength, _DTWAlpha, _DTWDegree)
    End Sub

    <Description("The PAA output length.")>
    Public Property PAALength As Integer
        Get
            Return _PAALength
        End Get
        Set(value As Integer)
            If _PAALength <> value Then
                If _DTWLength > value Then Throw New ArgumentException("PAA length cannot be less than DTW length.")
                If _PAALength > MyBase.Criteria.Samples.Length Then Throw New ArgumentException("PAA length cannot be greater than the query samples length: " & MyBase.Criteria.Samples.Length)
                _PAALength = value
                GeneratePAAQuery()
            End If
        End Set
    End Property

    <Description("Spherical projection distance.")>
    Public Property DTWAlpha As Double
        Get
            Return _DTWAlpha
        End Get
        Set(value As Double)
            If value <> _DTWAlpha Then
                _DTWAlpha = value
                InitDTW()
            End If
        End Set
    End Property

    <Description("The length of the feature vector contained in each sequence used by the kernel")>
    Public Property DTWLength As Integer
        Get
            Return _DTWLength
        End Get
        Set(value As Integer)
            If value <> _DTWLength Then
                If value > _PAALength Then Throw New ArgumentException("Warping length cannot be greater the PAA length.")
                _DTWLength = value
                InitDTW()
            End If
        End Set
    End Property

    <Description("The polynomial degree for this kernel")>
    Public Property DTWDegree As Integer
        Get
            Return _DTWDegree
        End Get
        Set(value As Integer)
            If value <> _DTWDegree Then
                _DTWDegree = value
                InitDTW()
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "PAA+DTW (Pre)"
        End Get
    End Property

    Private Shared Function Transform(samples As Double(), paaWindowWidth As Integer) As Double()

        If paaWindowWidth >= samples.Length Then Return samples

        Dim t As Double()() = New Double(paaWindowWidth - 1)() {}
        For i As Integer = 0 To paaWindowWidth - 1
            t(i) = New Double(samples.Length - 1) {}
            For j As Integer = 0 To samples.Length - 1
                t(i)(j) = samples(j)
            Next
        Next

        Dim exp As Double()() = Matrix.Reshape(t, 1, samples.Length * paaWindowWidth)
        Dim res As Double()() = Matrix.Reshape(exp, samples.Length, paaWindowWidth)

        Return Matrix.GetColumnMeans(res)

    End Function

End Class
