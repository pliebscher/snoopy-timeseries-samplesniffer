Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ComponentModel
Imports Snoopy.TimeSeries.Queries.Hashing
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class MurMurHash3Query
    Inherits TimeSeriesQuery

    Private _MurMur As MurMurHash3
    Private _QueryHash As Integer

    Public Sub New(query As TimeSeries)
        MyBase.New(query)
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double

        Dim Vals As Integer() = New Integer(data.Samples.Length - 1) {}
        For i As Integer = 0 To data.Samples.Length - 1
            Vals(i) = GetRGB(data.Samples(i))
        Next

        Dim hash As Integer = _MurMur.Hash(Vals)
        Dim sim As Double = _QueryHash - hash

        Return 1 / sim

    End Function

    Protected Overrides Sub OnQueryInit(query As TimeSeries)
        InitQuery()
        MyBase.OnQueryInit(query)
    End Sub

    Protected Overrides Sub OnQueryReset(query As TimeSeries)
        InitQuery()
        MyBase.OnQueryReset(query)
    End Sub

    Private Sub InitQuery()
        _MurMur = New MurMurHash3
        Dim Vals As Integer() = New Integer(MyBase.Criteria.Samples.Length - 1) {}
        For i As Integer = 0 To MyBase.Criteria.Samples.Length - 1
            Try
                Vals(i) = GetRGB(Math.Abs(MyBase.Criteria.Samples(i)))
            Catch ex As Exception
                Throw
            End Try

        Next
        _QueryHash = _MurMur.Hash(Vals)
    End Sub

    Private Function GetRGB(ByVal value As Double) As Integer
        If Double.IsNaN(value) Then Return 0
        If value = 0.0 Then Return 0

        Dim c As Integer
        Dim R As Double
        Dim G As Double
        Dim B As Double
        Dim R4 As Double
        Dim U As Double

        Try
            U = 256
            R4 = U / 3

            If value > U Then value = U

            If value < R4 Then  ' B, G
                B = value / R4
                G = 1 - B
                R = 0
            ElseIf value < 2 * R4 Then  ' B, R
                B = (1 - (value - R4) / R4)
                G = 0
                R = 1 - B
            Else ' G, R
                B = 0
                G = (2 - (value - R4) / R4)
                R = 1 - G
            End If

            R = (Math.Sqrt(R) * U)
            G = (Math.Sqrt(G) * U)
            B = (Math.Sqrt(B) * U)

            c = ((CInt(R) << 16) Or (CInt(G) << 8) Or CInt(B)) - 65000

        Catch ex As Exception
            Throw

        End Try
        Return c

    End Function

    Public Overrides ReadOnly Property Name As String
        Get
            Return "MurMurHash3 (Post)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Query based on the 3rd. variant of the MurMurHash algorithm by Austin Appleby."
        End Get
    End Property

End Class
