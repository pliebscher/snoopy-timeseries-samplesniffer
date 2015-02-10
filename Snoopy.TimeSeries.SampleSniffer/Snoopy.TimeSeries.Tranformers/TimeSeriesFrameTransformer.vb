Imports Snoopy.TimeSeries
Imports System.ComponentModel
Imports Snoopy.TimeSeries.Windows
''' <summary>
''' This base Class supplies the derived class with individual overlapping and windowed frames to transform.
''' </summary>
''' <remarks>
''' This is a convenience class to handle the framing, stepping and windowing.
''' </remarks>
Public MustInherit Class TimeSeriesFrameTransformer
    Inherits TimeSeriesTransformer

    Private _FrameWidth As FrameWidth = FrameWidth._512
    Private _FrameStep As FrameStep = FrameStep._64
    Private _WindowType As WindowType = WindowType.Hanning
    Private _Window As Double() = Windows.Window.Hanning.GetWindow(_FrameWidth)

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Supplies the derived class with a frame to transform.
    ''' </summary>
    ''' <param name="series"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function OnTransform(series As ITimeSeries(Of Double())) As Double()()
        If series.Samples.Length <= _FrameWidth Then Throw New TimeSeriesProcessorException(String.Format("The number of TimeSeries samples ({0}) must be > FrameWidth ({1}).", series.Samples.Length, _FrameWidth), Me)

        Dim width As Integer = (series.Samples.Length - _FrameWidth) \ _FrameStep
        Dim frames As Double()() = New Double(width - 1)() {}
        Dim frame As Double() = New Double(_FrameWidth - 1) {}

        For i As Integer = 0 To width - 1

            For j As Integer = 0 To _FrameWidth - 1
                Dim k As Integer = i * _FrameStep + j
                frame(j) = _Window(j) * series.Samples(k)
            Next

            frames(i) = Me.OnTransformFrame(frame)
        Next

        Return frames
    End Function

    ''' <summary>
    ''' Supplies the derived Class with a frame of TimeSeries samples to transform.
    ''' </summary>
    ''' <param name="frame">The TimeSeries frame to transform.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected MustOverride Function OnTransformFrame(frame As Double()) As Double()

    ''' <summary>
    ''' Notifies the derived class that the frame Width has changed.
    ''' </summary>
    ''' <param name="width"></param>
    ''' <remarks></remarks>
    Protected Overridable Sub OnFrameWidthChanged(width As Integer)
    End Sub

    ''' <summary>
    ''' Notifies the derived class that the frame Step has changed.
    ''' </summary>
    ''' <param name="step"></param>
    ''' <remarks></remarks>
    Protected Overridable Sub OnFrameStepChanged([step] As Integer)
    End Sub

    ''' <summary>
    ''' Notifies the derived class that the frame Window has changed.
    ''' </summary>
    ''' <param name="window"></param>
    ''' <remarks></remarks>
    Protected Overridable Sub OnWindowChanged(window As Double())
    End Sub

    ''' <summary>
    ''' The width of each frame.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <TypeConverter(GetType(EnumIntValueConverter)), DefaultValue(FrameWidth._512)>
    Public Overridable Property FrameWidth As FrameWidth
        Get
            Return _FrameWidth
        End Get
        Set(value As FrameWidth)
            If _FrameWidth <> value Then
                _FrameWidth = value
                _Window = Windows.Window.GetInstance(_WindowType).GetWindow(_FrameWidth)
                Me.OnFrameWidthChanged(_FrameWidth)
                Me.OnWindowChanged(_Window)
            End If
        End Set
    End Property

    ''' <summary>
    ''' The amount of step between each overlapping frame.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <TypeConverter(GetType(EnumIntValueConverter)), Description(""), DefaultValue(FrameStep._64)>
    Public Overridable Property FrameStep As FrameStep
        Get
            Return _FrameStep
        End Get
        Set(value As FrameStep)
            _FrameStep = value
            OnFrameStepChanged(_FrameStep)
        End Set
    End Property

    ''' <summary>
    ''' The type of window to be applied to each frame.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("Window"), Description("The type of window to be applied."), DefaultValue(WindowType.Hanning)>
    Public Property WindowType As WindowType
        Get
            Return _WindowType
        End Get
        Set(value As WindowType)
            If value <> _WindowType Then
                _WindowType = value
                _Window = Windows.Window.GetInstance(_WindowType).GetWindow(_FrameWidth)
                Me.OnWindowChanged(_Window)
            End If
        End Set
    End Property

    Protected ReadOnly Property Window As Double()
        Get
            Return _Window
        End Get
    End Property

End Class
