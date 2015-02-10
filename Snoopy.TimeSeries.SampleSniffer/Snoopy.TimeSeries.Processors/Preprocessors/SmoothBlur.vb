' fast_smooth.cpp
' /*A class to perform smoothing/bluring on data using a hanning (cos shaped) window.
' This uses and fast internal rotation algorithm*/
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class SmoothBlur
    Inherits TimeSeriesPreprocessor

    Private _Step As Integer = 1
    Private _Size As Integer = 32
    Private _SizeLeft As Integer
    Private _SizeRight As Integer
    Private _Angle As Double
    Private _SinAngle As Double
    Private _CosAngle As Double
    Private _Sum As Double

    Public Sub New()
        Reset()
    End Sub

    Private Sub Reset()

        _SizeLeft = _Size \ 2
        _SizeRight = _Size - _SizeLeft
        _Angle = -2 * Math.PI / CDbl(_Size + 1)
        _SinAngle = Math.Sin(_Angle)
        _CosAngle = Math.Cos(_Angle)

        For j As Integer = 0 To _Size - 1
            _Sum += 1.0 - Math.Cos((j + 1) * _Angle)
        Next

    End Sub

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()

        Dim length As Integer = series.Samples.Length \ _Step
        ' //blur stays centered if odd
        Dim cos_sum As Double
        Dim sin_sum As Double
        Dim total_sum As Double
        Dim dest As Double() = New Double(series.Samples.Length - 1) {}

        For j As Integer = 0 To _SizeRight - 1
            cos_sum += series.Samples(j * _Step)
            Rotate(sin_sum, cos_sum, _SinAngle, _CosAngle)
            total_sum += series.Samples(j * _Step)
        Next

        For j As Integer = 0 To _SizeLeft - 1
            dest(j * _Step) = (total_sum - cos_sum) / _Sum
            cos_sum += series.Samples((j + _SizeRight) * _Step)
            Rotate(sin_sum, cos_sum, _SinAngle, _CosAngle)
            total_sum += series.Samples((j + _SizeRight) * _Step)
        Next

        For j As Integer = _SizeLeft To length - _SizeLeft - 1
            dest(j * _Step) = (total_sum - cos_sum) / _Sum
            cos_sum += series.Samples((j + _SizeRight) * _Step)
            Rotate(sin_sum, cos_sum, _SinAngle, _CosAngle)
            cos_sum -= series.Samples((j - _SizeLeft) * _Step)
            total_sum += series.Samples((j + _SizeRight) * _Step) - series.Samples((j - _SizeLeft) * _Step)
        Next

        For j As Integer = length - _SizeLeft - 1 To length - 1
            dest(j * _Step) = (total_sum - cos_sum) / _Sum
            Rotate(sin_sum, cos_sum, _SinAngle, _CosAngle)
            cos_sum -= series.Samples((j - _SizeLeft) * _Step)
            total_sum -= series.Samples((j - _SizeLeft) * _Step)
        Next

        Return dest
    End Function

    ' //rotates a the complex number (sin_sum, cos_sum) by an angle.
    Private Sub Rotate(ByRef x As Double, ByRef y As Double, sinAngle As Double, cosAngle As Double)
        Dim temp As Double = y * cosAngle - x * sinAngle
        x = y * sinAngle + x * cosAngle
        y = temp
    End Sub

    Public Property [Step] As Integer
        Get
            Return _Step
        End Get
        Set(value As Integer)
            If CInt(Math.Floor(4 * (value / 2))) < 1 Then Throw New ArgumentOutOfRangeException("Filter width to small.")
            If value <> _Step Then
                _Step = value
                Reset()
            End If
        End Set
    End Property

    Public Property Size As Integer
        Get
            Return _Size
        End Get
        Set(value As Integer)
            If CInt(Math.Floor(4 * (value / 2))) < 1 Then Throw New ArgumentOutOfRangeException("Filter width to small.")
            If value <> _Size Then
                _Size = value
                Reset()
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "A class to perform smoothing/bluring on data using a hanning (cos shaped) window. This uses and fast internal rotation algorithm."
        End Get
    End Property

End Class
