Imports System.ComponentModel
''' <summary>
''' https://github.com/dhale/jtk/blob/master/src/main/java/edu/mines/jtk/dsp/SymmetricTridiagonalFilter.java
''' </summary>
''' <remarks></remarks>
<Description("")>
Public Class TridiagonalFilter2D
    Inherits TimeSeriesPostProcessor

    Private _TDF As New TridiagonalFilter

    Public Sub New()

    End Sub

    Protected Overloads Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()
        For i As Integer = 0 To series.Samples.Length - 1
            _TDF.Apply(series.Samples(i))
        Next
        Return series.Samples
    End Function

    <TypeConverter(GetType(ExpandableObjectConverter))>
    Public ReadOnly Property Filter As TridiagonalFilter
        Get
            Return _TDF
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "A 2D symmetric filter with three constant (shift-invariant) coefficients."
        End Get
    End Property

End Class
