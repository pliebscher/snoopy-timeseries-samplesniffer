Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public MustInherit Class IIRFilter
    Inherits TimeSeriesPreprocessor

    Private _SampleRate As Double
    Private _Width As Integer = 70

    Private _leftCoefficients As Double()
    Private _rightCoefficients As Double()

    Private _buffer As Double()
    Private _offset As Integer
    Private _size As Integer
    Private _halfSize As Integer

    Protected MustOverride Function OnUpdateCoefficients(sampleRate As Double, width As Integer) As Double()

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()

        If series.SampleRate <> _SampleRate Then
            _SampleRate = series.SampleRate
            UpdateCoefficients()
        End If

        Dim filtered As Double() = New Double(series.Samples.Length - 1) {}

        For i As Integer = 0 To series.Samples.Length - 1
            filtered(i) = ProcessSample(series.Samples(i))
        Next

        For i As Integer = 0 To _buffer.Length - 1
            _buffer(i) = 0.0
        Next

        Return filtered

    End Function

    Public Function ProcessSample(sample As Double) As Double

        _offset = If((_offset <> 0), _offset - 1, _halfSize - 1)
        '_buffer(_offset) = sample

        Dim un As Double = _leftCoefficients(0) * sample
        Dim i As Integer = 0, j As Integer = _halfSize - _offset + 1
        While i < _halfSize - 1
            un = _buffer(i) * _leftCoefficients(j) ' <----------------- somthing broken in here! _buffer will be all zero's so 'un' will always be wiped out!!
            i += 1
            j += 1
        End While

        _buffer(_offset) = un - _buffer(_offset) * _leftCoefficients(1)

        Dim yn As Double = 0.0
        Dim k As Integer = 0, l As Integer = _halfSize - _offset
        While k < _halfSize
            yn += _buffer(k) * _rightCoefficients(l)
            k += 1
            l += 1
        End While

        Return yn
    End Function

    Protected Sub UpdateCoefficients()
        _offset = 0
        Dim Coeffs As Double() = OnUpdateCoefficients(_SampleRate, _Width)
        _size = Coeffs.Count
        _halfSize = _size >> 1
        _leftCoefficients = New Double(_size - 1) {}
        _rightCoefficients = New Double(_size - 1) {}
        _buffer = New Double(_size - 1) {}
        For i As Integer = 0 To _halfSize - 1
            _leftCoefficients(_halfSize + i) = Coeffs(i)
            _leftCoefficients(i) = Coeffs(i) 'InlineAssignHelper(_leftCoefficients(_halfSize + i), Coeffs(i))
            _rightCoefficients(_halfSize + i) = Coeffs(_halfSize + i)
            _rightCoefficients(i) = Coeffs(_halfSize + i) 'InlineAssignHelper(_rightCoefficients(_halfSize + i), Coeffs(_halfSize + i))
        Next
    End Sub

    Public Property Width As Integer
        Get
            Return _Width
        End Get
        Set(value As Integer)
            If value < 1 Then Throw New ArgumentOutOfRangeException("Width cannot be less than 1.")
            If value <> _Width Then
                _Width = value
                UpdateCoefficients()
            End If
        End Set
    End Property

End Class
