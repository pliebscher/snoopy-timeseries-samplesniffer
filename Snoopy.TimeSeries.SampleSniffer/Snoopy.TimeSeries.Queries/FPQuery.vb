Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ComponentModel
''' <summary>
''' Haar Wavelet fingerprint query.
''' </summary>
''' <remarks></remarks>
<DefaultProperty("TopPeaks")>
Public Class FPQuery
    Inherits TimeSeriesQuery

    Private Shared _AbsComp As New AbsComparer()

    Private _QueryPrint As Integer()
    Private _TopPeaks As Integer = 64

    Public Sub New(query As TimeSeries)
        MyBase.New(query)
        GeneratePrint()
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double
        Dim print As Integer() = CreateFingerprint(data.Frames, _TopPeaks)
        Dim sim As Double = CalculateSimilarity(print, _QueryPrint)
        Return sim
    End Function

    Protected Overrides Sub OnQueryUpdate(query As TimeSeries)
        GeneratePrint()
    End Sub

    Protected Overrides Sub OnQueryReset(query As TimeSeries)
        _TopPeaks = 64
        GeneratePrint()
    End Sub

    Protected Overrides Sub OnQueryCriteriaChanged(criteria As TimeSeries)
        GeneratePrint()
    End Sub

    Private Sub GeneratePrint()
        _QueryPrint = CreateFingerprint(Criteria.Frames, _TopPeaks)
    End Sub

    <Description("Top peaks to extract from the Haar Wavelet."), DefaultValue(64)>
    Public Property TopPeaks As Integer
        Get
            Return _TopPeaks
        End Get
        Set(value As Integer)
            If value <> _TopPeaks Then
                _TopPeaks = value
                GeneratePrint()
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "FP (Post)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return ""
        End Get
    End Property

    Private Shared Function CreateFingerprint(ByVal frame As Double()(), ByVal topWavelets As Integer) As Integer()
        Dim newFrame As Double()() = New Double(frame.Length - 1)() {}
        For i As Integer = 0 To frame.Length - 1
            newFrame(i) = New Double(frame(i).Length - 1) {}
            Array.Copy(frame(i), newFrame(i), frame(i).Length)
        Next
        Return ExtractIndexes(newFrame, topWavelets)
    End Function

    Private Shared Function ExtractIndexes(ByVal frames As Double()(), ByVal count As Integer) As Integer()

        Dim rows As Integer = frames.GetLength(0)     '128
        Dim cols As Integer = frames(0).Length        '32
        Dim concatenated As Double() = New Double(rows * cols - 1) {}      ' 128 * 32 

        For row As Integer = 0 To rows - 1
            Array.Copy(frames(row), 0, concatenated, row * frames(row).Length, frames(row).Length)
        Next

        Dim indexes As Int32() = Enumerable.Range(0, concatenated.Length).ToArray()

        Array.Sort(concatenated, indexes, _AbsComp)

        Dim result As Integer() = EncodeFingerprint(concatenated, indexes, count)

        Return result
    End Function

    Private Shared Function EncodeFingerprint(ByVal concatenated As Double(), ByVal indexes As Integer(), ByVal topWavelets As Integer) As Integer()
        If concatenated.Length < topWavelets Then topWavelets = concatenated.Length
        '   Negative Numbers = 01
        '   Positive Numbers = 10
        '   Zeros            = 00
        Dim result As Integer() = New Integer(concatenated.Length * 2 - 1) {}

        For i As Integer = 0 To topWavelets - 1
            Dim index As Integer = indexes(i)
            Dim value As Double = concatenated(i)
            If value > 0 Then
                'positive wavelet
                result(index * 2) = 1
            Else
                'negative wavelet
                result(index * 2 + 1) = 1
            End If
        Next
        Return result
    End Function

    Private Shared Function CalculateSimilarity(x As Integer(), y As Integer()) As Double

        If x.Length > y.Length Then
            Dim t As Integer() = x
            x = y
            y = t
        End If

        Dim a As Integer = 0, b As Integer = 0
        Dim i As Integer = 0, n As Integer = x.Length
        While i < n
            If x(i) = y(i) AndAlso x(i) = 1 Then
                '1 1
                a += 1
            ElseIf x(i) <> y(i) Then
                '1 0 0 1
                b += 1
            End If
            i += 1
        End While
        If a + b = 0 Then
            Return 0
        End If
        Return CDbl(a) / (a + b)
    End Function

End Class
