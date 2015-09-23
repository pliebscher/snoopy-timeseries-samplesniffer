Imports System.Threading
Imports System.Runtime.InteropServices
Public Class TimeSeriesCapture

#Region " -- Locals -- "

    Private _SampleRate As Integer
    Private _SampleSize As Integer

    Private _Recorder As WaveInRecorder
    Private _Running As Boolean
    Private _Pause As Boolean

    Private _FPSThread As Thread

    Private _WaveInByteBuffer As Byte()
    Private _WaveInSampleBuffer As Double()

    Private _MI As MethodInvoker

    Private _Args As New CaptureBufferEventArgs

    Public Event CaptureStart(sender As Object, e As EventArgs)
    Public Event CapturePause(sender As Object, e As EventArgs)
    Public Event CaptureResume(sender As Object, e As EventArgs)
    Public Event CaptureStop(sender As Object, e As EventArgs)
    Public Event CaptureReset(sender As Object, e As EventArgs)
    Public Event CaptureBufferFull(sender As Object, e As CaptureBufferEventArgs)

    Public Sub New()

        InitializeComponent()

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, False)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

    End Sub

#End Region

#Region " -- Form -- "

    Private Sub TimeSeriesCapture_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        If _MI IsNot Nothing Then
            _MI = Nothing
        End If

        If _Recorder IsNot Nothing Then
            Try
                _Recorder.Dispose()
            Finally
                _Recorder = Nothing
            End Try
        End If
    End Sub

    Private Sub btnMonitorStart_Click(sender As Object, e As EventArgs) Handles btnMonitorStart.Click
        If WaveNative.waveInGetNumDevs() = 0 Then
            MsgBox("No input devices or input device not selected.", MsgBoxStyle.Information)
            Exit Sub
        End If
        Me.Start()
    End Sub

    Private Sub btnMonitorStop_Click(sender As Object, e As EventArgs) Handles btnMonitorStop.Click
        Me.Stop()
    End Sub

#End Region

    Public Sub Start()

        _SampleRate = App.SampleRate
        _SampleSize = App.SampleSize

        If _MI Is Nothing Then _MI = New MethodInvoker(AddressOf OnSampleBufferUpdate)

        '_WaveInSampleBuffer = New Double(_SampleSize - 1) {}

        btnMonitorStart.Enabled = False
        btnMonitorStart.Image = My.Resources.control_play
        btnMonitorStop.Enabled = True
        btnMonitorStop.Image = My.Resources.control_stop_blue

        Dim WaveFormat As New WaveFormat(_SampleRate, App.BitsPerSample, 1)

        Try

            If _WaveInByteBuffer Is Nothing OrElse _WaveInByteBuffer.Length <> _SampleSize * 2 Then
                _WaveInByteBuffer = New Byte(_SampleSize * 2 - 1) {}
            End If

            If _Recorder Is Nothing Then
                _Recorder = New WaveInRecorder(App.AudioInputDevice, WaveFormat, _SampleSize * 2, 3, AddressOf OnCaptureBufferFull)
            End If

            _Pause = False
            _Recorder.Start()
            _Running = True

            StartFPSThread()

            RaiseEvent CaptureStart(Me, EventArgs.Empty)

        Catch ex As Exception
            Debug.Write(ex)
            Dim msg As String = ex.Message.Trim
            If IsNumeric(msg) Then
                msg = [Enum].Parse(GetType(MMSYSERR), msg).ToString
            End If
            Me.Stop()
            Throw New Exception(String.Format("Error initializing the WaveInRecorder: {0}", msg), ex)
        End Try

    End Sub

    Private Sub StopFPSThread()
        If _FPSThread IsNot Nothing Then
            _FPSThread = Nothing
        End If
    End Sub

    Public Sub [Stop]()

        If _MI IsNot Nothing Then
            _MI = Nothing
        End If

        If _Recorder IsNot Nothing Then
            Try
                _Recorder.Stop()
                _Recorder.Dispose()
            Finally
                _Recorder = Nothing
            End Try
        End If

        btnMonitorStart.Enabled = True
        btnMonitorStart.Image = My.Resources.control_play_blue
        btnMonitorStop.Enabled = False
        btnMonitorStop.Image = My.Resources.control_stop

        FPS = 0
        _Pause = False
        _Running = False

        RaiseEvent CaptureStop(Me, EventArgs.Empty)

    End Sub

    Public Sub Reset()
        Dim Restart As Boolean
        If _Running Then
            Restart = True
            Me.Stop()
        End If
        If Restart Then
            Me.Start()
        End If
    End Sub

    Public Sub Pause()
        _Pause = True
        RaiseEvent CapturePause(Me, EventArgs.Empty)
    End Sub

    Public Sub [Resume]()
        _Pause = False
        RaiseEvent CaptureResume(Me, EventArgs.Empty)
    End Sub

    Public FPS As Double = 0
    Private _Ticks As Integer

    Private Sub StartFPSThread()
        _FPSThread = New Thread(AddressOf CalcFPS)
        _FPSThread.Priority = ThreadPriority.Lowest
        _FPSThread.Name = "CalcFPS"
        _FPSThread.Start()
    End Sub

    Private Sub CalcFPS()
        _Ticks = 0
        While _Running
            Thread.Sleep(1000)
            FPS = _Ticks * 2 ' Compensating because our capture buffer is twice the size as the resulting number of samples returned after byte->double conversion.
            _Ticks = 0
        End While
    End Sub

    Private _SigGen As New SignalGenerator

    Private Sub OnCaptureBufferFull(ByVal data As IntPtr, ByVal size As Integer)

        If _Pause OrElse _MI Is Nothing Then Exit Sub

        Marshal.Copy(data, _WaveInByteBuffer, 0, size)

        _WaveInSampleBuffer = New Double(_SampleSize - 1) {}

        Dim h As Integer = 0
        For i As Integer = 0 To _WaveInByteBuffer.Length - 2 Step 2
            _WaveInSampleBuffer(h) = BitConverter.ToInt16(_WaveInByteBuffer, i) / 32768.0
            h += 1
        Next

        _Args.TimeSeries = _WaveInSampleBuffer.ToTimeSeries(_SampleRate)

        '_Args.TimeSeries = _SigGen.GenerateSignal.ToTimeSeries(_SampleRate)

        Try
            Invoke(_MI)
            _Ticks += 1
        Catch ex As Exception
        End Try

    End Sub

    Private Sub OnSampleBufferUpdate()
        RaiseEvent CaptureBufferFull(Me, _Args)
    End Sub

#Region " -- Properties -- "

    Public ReadOnly Property IsRunning As Boolean
        Get
            Return _Running
        End Get
    End Property

    Public ReadOnly Property IsPaused As Boolean
        Get
            Return _Pause
        End Get
    End Property

    'Public Property ShowFPS As Boolean
    '    Get
    '        Return _ShowFPS
    '    End Get
    '    Set(value As Boolean)
    '        _ShowFPS = value
    '        If _ShowFPS AndAlso _Running AndAlso _FPSThread Is Nothing Then
    '            StartFPSThread()
    '        Else
    '            StopFPSThread()
    '        End If
    '    End Set
    'End Property

#End Region

End Class

Public Class CaptureBufferEventArgs
    Inherits EventArgs

    Public TimeSeries As TimeSeries

End Class


Class SignalGenerator

    Private _waveForm As String = "Square"
    Private _amplitude As Double = 64.0
    Private _samplingRate As Double = 22050 '44100
    Private _frequency As Double = 500.0
    Private _dcLevel As Double = 0.0
    Private _noise As Double = 0.5
    Private _samples As Integer = 2048 '16384
    Private _addDCLevel As Boolean = False
    Private _addNoise As Boolean = True ' False

    Private _r As New Random()

    Public Sub New()
    End Sub

    Public Sub SetWaveform(waveForm As String)
        _waveForm = waveForm
    End Sub

    Public Function GetWaveform() As [String]
        Return _waveForm
    End Function

    Public Sub SetAmplitude(amplitude As Double)
        _amplitude = amplitude
    End Sub

    Public Function GetAmplitude() As Double
        Return _amplitude
    End Function

    Public Sub SetFrequency(frequency As Double)
        _frequency = frequency
    End Sub

    Public Function GetFrequency() As Double
        Return _frequency
    End Function

    Public Sub SetSamplingRate(rate As Double)
        _samplingRate = rate
    End Sub

    Public Function GetSamplingRate() As Double
        Return _samplingRate
    End Function

    Public Sub SetSamples(samples As Integer)
        _samples = samples
    End Sub

    Public Function GetSamples() As Integer
        Return _samples
    End Function

    Public Sub SetDCLevel(dc As Double)
        _dcLevel = dc
    End Sub

    Public Function GetDCLevel() As Double
        Return _dcLevel
    End Function

    Public Sub SetNoise(noise As Double)
        _noise = noise
    End Sub

    Public Function GetNoise() As Double
        Return _noise
    End Function

    Public Sub SetDCLevelState(dcstate As Boolean)
        _addDCLevel = dcstate
    End Sub

    Public Function IsDCLevel() As Boolean
        Return _addDCLevel
    End Function

    Public Sub SetNoiseState(noisestate As Boolean)
        _addNoise = noisestate
    End Sub

    Public Function IsNoise() As Boolean
        Return _addNoise
    End Function

    Public Function GenerateSignal() As Double()

        Dim values As Double() = New Double(_samples - 1) {}
        If _waveForm.Equals("Sine") Then
            Dim theta As Double = 2.0 * Math.PI * _frequency / _samplingRate
            For i As Integer = 0 To _samples - 1
                values(i) = _amplitude * Math.Sin(i * theta)
            Next
        End If
        If _waveForm.Equals("Cosine") Then
            Dim theta As Double = 2.0F * CDbl(Math.PI) * _frequency / _samplingRate
            For i As Integer = 0 To _samples - 1
                values(i) = _amplitude * Math.Cos(i * theta)
            Next
        End If
        If _waveForm.Equals("Square") Then
            Dim p As Double = 2.0 * _frequency / _samplingRate
            For i As Integer = 0 To _samples - 1
                values(i) = If(Math.Round(i * p) Mod 2 = 0, _amplitude, -_amplitude)
            Next
        End If
        If _waveForm.Equals("Triangular") Then
            Dim p As Double = 2.0 * _frequency / _samplingRate
            For i As Integer = 0 To _samples - 1
                Dim ip As Integer = CInt(Math.Round(i * p))
                values(i) = 2.0 * _amplitude * (1 - 2 * (ip Mod 2)) * (i * p - ip)
            Next
        End If
        If _waveForm.Equals("Sawtooth") Then
            For i As Integer = 0 To _samples - 1
                Dim q As Double = i * _frequency / _samplingRate
                values(i) = 2.0 * _amplitude * (q - Math.Round(q))
            Next
        End If

        If _addDCLevel Then
            For i As Integer = 0 To _samples - 1
                values(i) += _dcLevel
            Next
        End If

        If _addNoise Then
            For i As Integer = 0 To _samples - 1
                values(i) += _noise * (100 / _r.[Next](1, _samples))
            Next
        End If

        Return values

    End Function

End Class

