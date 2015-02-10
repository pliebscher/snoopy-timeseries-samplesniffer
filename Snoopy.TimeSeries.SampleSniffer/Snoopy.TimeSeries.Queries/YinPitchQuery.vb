Imports System.ComponentModel
''' <summary>
''' Tarsos DSP???
''' </summary>
''' <remarks></remarks>
<DefaultProperty("Threshhold")>
Public Class YinPitchQuery
    Inherits TimeSeriesQuery

    Private _YinPitch As YinPitchDetector
    Private _Threshhold As Double = 0.5
    Private _SampleRate As Integer
    Private _Length As Integer
    Private _ResultsIn As ResultType = ResultType.Probability

    Public Sub New(query As TimeSeries)
        MyBase.New(query)

        _YinPitch = New YinPitchDetector(query.SampleRate, query.Samples.Length, _Threshhold)
        _SampleRate = query.SampleRate
        _Length = query.Samples.Length

    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double

        If data.Energy <= 0 Then Return 0

        Dim yinPitch As Pitch = _YinPitch.GetPitch(data.Samples)

        If yinPitch.Hertz < 0 Then Return 0

        If _ResultsIn = ResultType.Probability Then
            Return yinPitch.Probability
        Else
            Return yinPitch.Hertz / 1000
        End If

    End Function

    <Description(""), DefaultValue(0.5)>
    Public Property Threshhold As Double
        Get
            Return _Threshhold
        End Get
        Set(value As Double)
            If value < 0 OrElse value > 0.99 Then Exit Property
            If value <> _Threshhold Then
                _Threshhold = value
                _YinPitch = New YinPitchDetector(_SampleRate, _Length, _Threshhold)
            End If
        End Set
    End Property

    <Description("Probability that pitch exists or the pitch in Hz (if any). Multiply Hz by 1000."), DefaultValue(ResultType.Probability)>
    Public Property ResultsIn As ResultType
        Get
            Return _ResultsIn
        End Get
        Set(value As ResultType)
            _ResultsIn = value
        End Set
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "Yin Pitch (Pre)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Yin Pitch Query"
        End Get
    End Property

    Public Enum ResultType
        Probability
        PitchInHertz
    End Enum

End Class
