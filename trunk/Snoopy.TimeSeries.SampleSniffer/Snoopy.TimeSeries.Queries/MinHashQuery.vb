Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<DefaultProperty("UniverseSize")>
Public Class MinHashQuery
    Inherits TimeSeriesQuery

    Private _UniverseSize As Integer = 16581375
    Private _HashFunctions As Integer = 32
    Private Shared _Hash As Hashing.MinHash
    Private _QSet As New HashSet(Of Integer)

    Public Sub New(query As TimeSeries)
        MyBase.New(query)
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double

        Dim DSet As New HashSet(Of Integer)

        For i As Integer = 0 To data.Frames.Length - 1
            For j As Integer = 0 To data.Frames(i).Length - 1
                DSet.Add(GetRGB(Math.Abs(data(i)(j))))
            Next
        Next

        Dim sim As Double = _Hash.Similarity(_QSet, DSet)

        Return sim

    End Function

    Protected Overrides Sub OnQueryInit(criteria As TimeSeries)
        InitCriteria()
        If _Hash Is Nothing Then _Hash = New Hashing.MinHash(_UniverseSize, _HashFunctions)
        MyBase.OnQueryInit(criteria)
    End Sub

    Protected Overrides Sub OnQueryReset(criteria As TimeSeries)
        _HashFunctions = 32
        _UniverseSize = 16581375
        InitCriteria()
    End Sub

    Protected Overrides Sub OnQueryUpdate(criteria As TimeSeries)
        InitCriteria()
    End Sub

    Protected Overrides Sub OnQueryCriteriaChanged(criteria As TimeSeries)
        InitCriteria()
    End Sub

    Private Shared Function GetRGB(ByVal value As Double) As Integer
        If Double.IsNaN(value) Then Return 0
        If value = 0.0 Then Return 0

        Dim R As Double
        Dim G As Double
        Dim B As Double
        Dim R4 As Double
        Dim U As Double

        U = 255
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

        Return (CInt(R) << 16) Or (CInt(G) << 8) Or CInt(B)

    End Function

    Private Sub InitCriteria()

        _QSet.Clear()

        For i As Integer = 0 To Criteria.Frames.Length - 1
            For j As Integer = 0 To Criteria.Frames(i).Length - 1
                _QSet.Add(GetRGB(Math.Abs(Criteria(i)(j))))
            Next
        Next

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Description("Number of Hash functions to generate."), DefaultValue(32)>
    Public Property HashFunctions As Integer
        Get
            Return _HashFunctions
        End Get
        Set(value As Integer)
            If value <> _HashFunctions Then
                _HashFunctions = value
                _Hash = New Hashing.MinHash(_UniverseSize, _HashFunctions)
                'InitQuery()
            End If
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Description("Number of values that can possibly exist in the universe. For example, RGB values: 255 * 255 * 255 = 16,581,375"), DefaultValue(16581375)>
    Public Property UniverseSize As Integer
        Get
            Return _UniverseSize
        End Get
        Set(value As Integer)
            If value <> _UniverseSize Then
                _UniverseSize = value
                _Hash = New Hashing.MinHash(_UniverseSize, _HashFunctions)
                'InitQuery()
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "MinHash (Post)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Min-wise independent permutations locality sensitive hashing."
        End Get
    End Property

End Class
