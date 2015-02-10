Imports System.ComponentModel
''' <summary>
''' http://sourceforge.net/p/cmusphinx/code/11917/tree/trunk/sphinx4/src/sphinx4/edu/cmu/sphinx/frontend/filter/Dither.java
''' </summary>
''' <remarks></remarks>
<DefaultProperty("DitherMax")>
Public Class Dither
    Inherits TimeSeriesPreprocessor

    Private _DitherMax As Double = 0.5 ' The maximal value which could be added/subtracted to/from the signal
    Private _MaxValue As Double = Double.MaxValue   ' The maximal value of dithered values.
    Private _MinValue As Double = -Double.MaxValue

    Private _r As Random = New Random()

    Public Sub New()
    End Sub

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()

        Dim outFeatures As Double() = New Double(series.Samples.Length - 1) {}
        For i As Integer = 0 To series.Samples.Length - 1
            outFeatures(i) = _r.NextDouble * 2 * _DitherMax - _DitherMax + series.Samples(i)
            outFeatures(i) = Math.Max(Math.Min(outFeatures(i), _MaxValue), _MinValue)
        Next

        Return outFeatures

    End Function

    <Description("The maximal value which could be added/subtracted to/from the signal"), DefaultValue(0.5)>
    Public Property DitherMax As Double
        Get
            Return _DitherMax
        End Get
        Set(value As Double)
            If value <> _DitherMax Then
                _DitherMax = value

            End If
        End Set
    End Property

    <Description("The maximal value of dithered values."), DefaultValue(Double.MaxValue)>
    Public Property MaxValue As Double
        Get
            Return _MaxValue
        End Get
        Set(value As Double)
            If value <> _MaxValue Then
                _MaxValue = value
            End If
        End Set
    End Property

    <Description("The minimal value of dithered values."), DefaultValue(-Double.MaxValue)>
    Public Property MinValue As Double
        Get
            Return _MinValue
        End Get
        Set(value As Double)
            If value <> _MinValue Then
                _MinValue = value
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Applies random noise to the incoming signal."
        End Get
    End Property

End Class
