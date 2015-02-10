Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ComponentModel
''' <summary>
''' Simple Parabolic Regression - Operates on pre-processed TimeSeries samples.
''' </summary>
''' <remarks></remarks>
<Serializable>
Public Class LSRQuery
    Inherits TimeSeriesQuery

    Private _FitMatrix As Double()()

    Public Sub New(criteria As TimeSeries)
        MyBase.New(criteria)
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double

        Dim _lsr As LSR = LSR.Calculate(Criteria.Samples, data.Samples)
        Dim dist1 As Double = Math.Sqrt((_lsr.a * _lsr.a) + (_lsr.b * _lsr.b) + (_lsr.c * _lsr.c))
        Return dist1

    End Function

    Protected Overrides Sub OnQueryInit(query As TimeSeries)
        InitFitMatrix()
    End Sub

    Protected Overrides Sub OnQueryUpdate(query As TimeSeries)
        InitFitMatrix()
    End Sub

    Protected Overrides Sub OnQueryReset(query As TimeSeries)
        InitFitMatrix()
    End Sub

    Protected Overrides Sub OnQueryCriteriaChanged(criteria As TimeSeries)
        InitFitMatrix()
    End Sub

    Private Sub InitFitMatrix()
        _FitMatrix = LSR.CalculateFitMatrix(Criteria.Frames, Criteria.Frames)
    End Sub

    Public Overrides ReadOnly Property Name As String
        Get
            Return "LSR (Pre)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Least Squares Regression for Quadratic Curve Fitting"
        End Get
    End Property

End Class
