Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Serializable, DefaultValue("Threshhold")>
Public Class DWPitchQuery
    Inherits TimeSeriesQuery

    Private _WaveletPitch As DynamicWaveletPitchDetector
    Private _Threshhold As Double = 0.5
    Private _MaxHz As Double = 1300.0
    Private _SampleRate As Integer

    Public Sub New(query As TimeSeries)
        MyBase.New(query)
        _SampleRate = query.SampleRate
        ResetPitchDetector()
    End Sub

    Protected Overrides Function OnQueryExecute(data As TimeSeries) As Double
        If data.Energy <= 0 Then Return 0
        Dim pitch As Pitch = _WaveletPitch.GetPitch(data.Samples)
        Dim pitchHz As Double = pitch.Hertz / _MaxHz ') ' / samples.Length)
        Return pitchHz
    End Function

    Private Sub ResetPitchDetector()
        _WaveletPitch = New DynamicWaveletPitchDetector(_SampleRate, _MaxHz, _Threshhold)
    End Sub

    <Description(""), DefaultValue(0.5)>
    Public Property Threshhold As Double
        Get
            Return _Threshhold
        End Get
        Set(value As Double)
            If value < 0 OrElse value > 0.99 Then Exit Property
            If value <> _Threshhold Then
                _Threshhold = value
                ResetPitchDetector()
            End If
        End Set
    End Property

    <Description(""), DefaultValue(1300.0)>
    Public Property MaxHz As Double
        Get
            Return _MaxHz
        End Get
        Set(value As Double)
            If value <> _MaxHz Then
                _MaxHz = value
                ResetPitchDetector()
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "DW Pitch (Pre)"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Dynamic Wavelet Pitch Query. This is a stateful query; it tracks the previous frames pitch probability."
        End Get
    End Property


End Class
