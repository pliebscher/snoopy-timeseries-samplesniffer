Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Serializable>
Public Class CorrelationQuery
    Inherits TimeSeriesQuery

    Public Sub New(criteria As TimeSeries)
        MyBase.New(criteria)
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double

        Dim mx As Double = data.Mean
        Dim my As Double = Criteria.Mean
        ' standard deviation
        Dim sx As Double = Math.Sqrt(data.Variance)
        Dim sy As Double = Math.Sqrt(Criteria.Variance)

        Dim n As Integer = data.Samples.Length
        Dim nval As Double = 0.0
        For i As Integer = 0 To n - 1
            nval += (data.Samples(i) - mx) * (Criteria.Samples(i) - my)
        Next

        Dim r As Double = nval / ((n - 1) * sx * sy)

        Return r

    End Function

    Public Overrides ReadOnly Property Name As String
        Get
            Return "Correlation (Pre)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Returns the correlation between two time series."
        End Get
    End Property

End Class
