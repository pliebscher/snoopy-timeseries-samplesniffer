Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Text
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Serializable, StructLayout(LayoutKind.Sequential), TypeConverter(GetType(ExpandableObjectConverter)), DefaultProperty("Value")>
Public Class TimeSeriesQueryResult

    Private _Query As TimeSeriesQuery
    Private _Data As TimeSeries
    Private _Score As Double
    Private _IsMatch As Boolean

    Public Sub New(query As TimeSeriesQuery, data As TimeSeries, score As Double)
        If query Is Nothing Then Throw New ArgumentNullException("query")
        If data Is Nothing Then Throw New ArgumentNullException("data")

        _Query = query
        _Data = data
        _Score = score
        _IsMatch = (_Score >= _Query.MatchThreshLow AndAlso _Score <= _Query.MatchThreshHigh)

    End Sub

    <Browsable(True), TypeConverter(GetType(ExpandableObjectConverter))>
    Public ReadOnly Property Query As TimeSeriesQuery
        Get
            Return _Query
        End Get
    End Property

    <Browsable(True), TypeConverter(GetType(ExpandableObjectConverter))>
    Public Property Data As TimeSeries
        Get
            Return _Data
        End Get
        Friend Set(value As TimeSeries)
            _Data = value
        End Set
    End Property

    Public ReadOnly Property Score As Double
        Get
            Return _Score
        End Get
    End Property

    Public Property IsMatch As Boolean
        Get
            Return _IsMatch
        End Get
        Friend Set(value As Boolean)
            _IsMatch = value
        End Set
    End Property

    Public Sub Save(filename As String)
        Throw New NotImplementedException("Feel free...")
    End Sub

    Public Shared Function Load(filename As String) As TimeSeriesQueryResult
        Throw New NotImplementedException("Feel free...")
    End Function

    Public Overrides Function ToString() As String
        Return Me._Score.ToString
    End Function

End Class
