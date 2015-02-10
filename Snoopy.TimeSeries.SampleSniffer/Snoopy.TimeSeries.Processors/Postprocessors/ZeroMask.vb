Imports System.ComponentModel
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class ZeroMask
    Inherits TimeSeriesPostProcessor

    Private _X1 As Integer = 5
    Private _Y1 As Integer = 5

    Private _X2 As Integer = 10
    Private _Y2 As Integer = 10

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()

        Dim len As Integer = series.Samples(0).Length
        Dim masked As Double()() = New Double(series.Samples.Length - 1)() {}

        For i As Integer = 0 To series.Samples.Length - 1
            masked(i) = New Double(series.Samples(0).Length - 1) {}
        Next

        For y As Integer = _Y1 To _Y2 - 1

            For x As Integer = _X1 To _X2 - 1

                masked(y)(x) = series.Samples(y)(x)

            Next
        Next

        Return masked
    End Function

    <Description("Upper left X.")>
    Public Property X1 As Integer
        Get
            Return _X1
        End Get
        Set(value As Integer)
            _X1 = value
        End Set
    End Property

    <Description("Upper left Y.")>
    Public Property Y1 As Integer
        Get
            Return _Y1
        End Get
        Set(value As Integer)
            _Y1 = value
        End Set
    End Property

    <Description("Lower left X.")>
    Public Property X2 As Integer
        Get
            Return _X2
        End Get
        Set(value As Integer)
            _X2 = value
        End Set
    End Property

    <Description("Lower left Y.")>
    Public Property Y2 As Integer
        Get
            Return _Y2
        End Get
        Set(value As Integer)
            _Y2 = value
        End Set
    End Property

End Class
