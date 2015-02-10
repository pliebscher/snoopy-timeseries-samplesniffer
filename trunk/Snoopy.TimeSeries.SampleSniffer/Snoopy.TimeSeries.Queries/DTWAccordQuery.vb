Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Serializable>
Public Class DTWAccordQuery
    Inherits TimeSeriesQuery

    Private _Alpha As Double = 1.0
    Private _Width As Integer = 64
    Private _Degree As Integer = 1
    Private _DTWAccord As DTWAccord

    Public Sub New(query As TimeSeries)
        MyBase.New(query)
    End Sub

    Protected Overrides Sub OnQueryInit(query As TimeSeries)
        InitDTW()
        MyBase.OnQueryInit(query)
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double

        Dim dist As Double = _DTWAccord.Compute(data.Samples, MyBase.Criteria.Samples)

        Return Math.Abs(dist)
    End Function

    Protected Overrides Sub OnQueryReset(query As TimeSeries)
        _Alpha = 1.0
        _Width = 64
        _Degree = 1
        InitDTW()
    End Sub

    Private Sub InitDTW()
        _DTWAccord = New DTWAccord(_Width, _Alpha, _Degree)
    End Sub

    <Description("Spherical projection distance."), DefaultValue(1.0)>
    Public Property Alpha As Double
        Get
            Return _Alpha
        End Get
        Set(value As Double)
            If value <> _Alpha Then
                _Alpha = value
                InitDTW()
            End If
        End Set
    End Property

    <Description("Length of the feature vectors."), DefaultValue(64)>
    Public Property Width As Integer
        Get
            Return _Width
        End Get
        Set(value As Integer)
            If value <> _Width Then
                _Width = value
                InitDTW()
            End If
        End Set
    End Property

    <Description("Polynomial kernel degree."), DefaultValue(1)>
    Public Property Degree As Integer
        Get
            Return _Degree
        End Get
        Set(value As Integer)
            If value <> _Degree Then
                _Degree = value
                InitDTW()
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "DTWAccord (Pre)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Dynamic Time Warping (Accord)"
        End Get
    End Property


End Class
