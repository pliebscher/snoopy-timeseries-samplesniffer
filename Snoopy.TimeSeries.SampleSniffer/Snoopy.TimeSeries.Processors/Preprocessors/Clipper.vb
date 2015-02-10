Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Description("Sets values outside the Min/Max to Zero.")>
Public Class Clipper
    Inherits TimeSeriesPreprocessor

    Private _Min As Double = 0
    Private _Max As Double = 1

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()
        For i As Integer = 0 To series.Samples.Length - 1
            Dim sample As Double = series.Samples(i)
            If sample < _Min Or sample > Max Then
                series.Samples(i) = 0
            End If
        Next
        Return series.Samples
    End Function

    <Description("Sets values below to Zero."), DefaultValue(0)>
    Public Property Min As Double
        Get
            Return _Min
        End Get
        Set(value As Double)
            If value <> _Min Then
                _Min = value

            End If
        End Set
    End Property

    <Description("Sets values above to Zero."), DefaultValue(1)>
    Public Property Max As Double
        Get
            Return _Max
        End Get
        Set(value As Double)
            If value <> _Min Then
                _Max = value

            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Sets values outside the Min/Max to Zero."
        End Get
    End Property

End Class
