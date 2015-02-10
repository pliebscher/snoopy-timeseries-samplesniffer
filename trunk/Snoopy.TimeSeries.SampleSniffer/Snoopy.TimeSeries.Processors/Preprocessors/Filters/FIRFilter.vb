Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public MustInherit Class FIRFilter
    Inherits TimeSeriesPreprocessor

    Private _SampleRate As Double
    Private _HalfOrder As Integer = 64

    Private _coefficients As Double()
    Private _buffer As Double()
    Private _offset As Integer
    Private _size As Integer

    Protected MustOverride Function OnUpdateCoefficients(sampleRate As Double, halfOrder As Integer) As Double()

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()

        If series.SampleRate <> _SampleRate Then
            _SampleRate = series.SampleRate
            UpdateCoefficients()
        End If

        Dim filtered As Double() = New Double(series.Samples.Length - 1) {}

        For i As Integer = 0 To series.Samples.Length - 1
            _offset = If((_offset <> 0), _offset - 1, _size - 1)
            _buffer(_offset) = series.Samples(i)
            Dim k As Integer = _size - _offset
            For j As Integer = 0 To _size - 1
                filtered(i) += _buffer(j) * _coefficients(k)
                k += 1
            Next
        Next

        For i As Integer = 0 To _buffer.Length - 1
            _buffer(i) = 0.0
        Next

        Return filtered

    End Function

    Protected Sub UpdateCoefficients()
        _offset = 0
        Dim Coeffs As Double() = OnUpdateCoefficients(_SampleRate, _HalfOrder)
        _size = Coeffs.Count
        _buffer = New Double(_size - 1) {}
        _coefficients = New Double((_size << 1) - 1) {}
        For i As Integer = 0 To _size - 1
            _coefficients(i) = Coeffs(i)
            _coefficients(_size + i) = Coeffs(i)
        Next
    End Sub

    Public Property HalfOrder As Integer
        Get
            Return _HalfOrder
        End Get
        Set(value As Integer)
            If value < 0 Then Throw New ArgumentOutOfRangeException("HalfOrder cannot be less than 0.")
            If value <> _HalfOrder Then
                _HalfOrder = value
                UpdateCoefficients()
            End If
        End Set
    End Property

End Class
