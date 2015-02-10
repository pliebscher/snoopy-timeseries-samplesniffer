Imports Snoopy.TimeSeries
Imports System.ComponentModel
Imports Snoopy.TimeSeries.Windows
''' <summary>
''' https://github.com/marytts/marytts/blob/master/marytts-signalproc/src/main/java/marytts/signalproc/adaptation/smoothing/TemporalSmoother.java
''' </summary>
''' <remarks></remarks>
Public Class TemporalSmoother
    Inherits TimeSeriesPreprocessor

    Private _Neighbours As Integer = 4
    Private _WindowSize As Integer = 2 * _Neighbours + 1
    Private _WindowType As WindowType = WindowType.Hamming
    Private _Window As Window = Windows.Window.Hamming
    Private _Win As Double() = _Window.GetWindow(_WindowSize)

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()

        Dim i As Integer
        Dim x As Double() = series.Samples
        'double[][] xx = new double[x.length][1];
        Dim xx As Double()() = New Double(x.Length - 1)() {}

        For i = 0 To x.Length - 1
            xx(i) = New Double(0) {}
            xx(i)(0) = x(i)
        Next

        xx = smooth(xx, Neighbours)

        'double[] y = new double[x.length];
        Dim y As Double() = New Double(x.Length - 1) {}
        For i = 0 To x.length - 1
            y(i) = xx(i)(0)
        Next

        Return y

    End Function

    Public Function smooth(x As Double()(), neighbours As Integer) As Double()()

        Dim weightSum As Double
        'double[][] y = new double[x.length][x[0].length];
        Dim y As Double()() = New Double(x.Length)() {}
        Dim i As Integer, j As Integer, k As Integer
        Dim windowSize As Integer = 2 * neighbours + 1
        'Dim w As New DynamicWindow(windowType)
        Dim weights As Double() = _Win   '= w.values(windowSize)


        For i = 0 To x(0).Length - 1
            For j = 0 To x.Length - 1
                y(j) = New Double(0) {} '0.0
                weightSum = 0.0
                For k = -neighbours To neighbours
                    If j + k >= 0 AndAlso j + k < x.Length Then
                        y(j)(i) += weights(k + neighbours) * x(j + k)(i)
                        weightSum += weights(k + neighbours)
                    End If
                Next

                If weightSum > 0.0 Then
                    y(j)(i) /= weightSum
                End If
            Next
        Next

        Return y

    End Function

    <Description("Neighbours to use on the left and on the right (separately)"), DefaultValue(4)>
    Public Property Neighbours As Integer
        Get
            Return _Neighbours
        End Get
        Set(value As Integer)
            If value <> _Neighbours Then
                _Neighbours = value
                _WindowSize = 2 * _Neighbours + 1
                _Win = _Window.GetWindow(_WindowSize)
            End If
        End Set
    End Property

    ''' <summary>
    ''' The type of window to be applied to each frame.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Description("The type of window to be applied."), DefaultValue(WindowType.Hanning)>
    Public Property Window As WindowType
        Get
            Return _WindowType
        End Get
        Set(value As WindowType)
            If value <> _WindowType Then
                _WindowType = value
                _Window = Windows.Window.GetInstance(_WindowType)
                _Win = _Window.GetWindow(_WindowSize)
            End If
        End Set
    End Property

End Class
