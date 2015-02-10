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
    'Private _ShowFPS As Boolean

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

    Public Event SampleRateChanged(sender As Object, e As EventArgs)
    Public Event SampleSizeChanged(sender As Object, e As EventArgs)


    Public Sub New()

        InitializeComponent()

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, False)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

    End Sub

#End Region

#Region " -- Form -- "

    Private Sub TimeSeriesCapture_Load(sender As Object, e As EventArgs) Handles Me.Load

        If WaveNative.waveInGetNumDevs() = 0 Then
            MsgBox("No input devices.", MsgBoxStyle.Information)
            btnMonitorStart.Enabled = False
        End If

        ' Load previous settings...
        Me.SetSelectedMenuItem(SampleRateToolStripMenuItem, My.Settings.SampleRate.ToString)
        Me.SetSelectedMenuItem(SampleSizeToolStripMenuItem, My.Settings.SampleSize.ToString)

    End Sub

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
        Me.Start()
    End Sub

    Private Sub btnMonitorStop_Click(sender As Object, e As EventArgs) Handles btnMonitorStop.Click
        Me.Stop()
    End Sub

    Private Sub btnCaptureSettings_Click(sender As Object, e As EventArgs) Handles btnCaptureSettings.Click
        cmsDataSpectrogram.Show(btnCaptureSettings, btnCaptureSettings.Width \ 2, btnCaptureSettings.Height \ 2)
    End Sub

#Region " -- Context Menu -- "

    Private Sub SetSelectedMenuItem(menu As ToolStripMenuItem, item As ToolStripMenuItem)
        For Each menuItem As ToolStripMenuItem In menu.DropDownItems
            menuItem.Checked = False
        Next
        item.Checked = True
    End Sub

    Private Sub SetSelectedMenuItem(menu As ToolStripMenuItem, value As String)
        For Each menuItem As ToolStripMenuItem In menu.DropDownItems
            If menuItem.Text = value Then
                menuItem.Checked = True
            Else
                menuItem.Checked = False
            End If
        Next
    End Sub

    Private Sub SetSampleRate(rate As Integer)
        RaiseEvent SampleRateChanged(Me, EventArgs.Empty)
        App.SampleRate = rate
        Me.Reset()
    End Sub

    Private Sub tsmSampleRate5k_Click(sender As Object, e As EventArgs) Handles tsmSampleRate5k.Click
        SetSampleRate(5512)
        SetSelectedMenuItem(SampleRateToolStripMenuItem, tsmSampleRate5k)
    End Sub

    Private Sub tsmSampleRate8k_Click(sender As Object, e As EventArgs) Handles tsmSampleRate8k.Click
        SetSampleRate(8000)
        SetSelectedMenuItem(SampleRateToolStripMenuItem, tsmSampleRate8k)
    End Sub

    Private Sub tsmSampleRate11k_Click(sender As Object, e As EventArgs) Handles tsmSampleRate11k.Click
        SetSampleRate(11025)
        SetSelectedMenuItem(SampleRateToolStripMenuItem, tsmSampleRate11k)
    End Sub

    Private Sub tsmSampleRate12k_Click(sender As Object, e As EventArgs) Handles tsmSampleRate12k.Click
        SetSampleRate(12500)
        SetSelectedMenuItem(SampleRateToolStripMenuItem, tsmSampleRate12k)
    End Sub

    Private Sub tsmSampleRate16k_Click(sender As Object, e As EventArgs) Handles tsmSampleRate16k.Click
        SetSampleRate(16000)
        SetSelectedMenuItem(SampleRateToolStripMenuItem, tsmSampleRate16k)
    End Sub

    Private Sub tsmSampleRate22k_Click(sender As Object, e As EventArgs) Handles tsmSampleRate22k.Click
        SetSampleRate(22050)
        SetSelectedMenuItem(SampleRateToolStripMenuItem, tsmSampleRate22k)
    End Sub

    Private Sub tsmSampleRate44k_Click(sender As Object, e As EventArgs) Handles tsmSampleRate44k.Click
        SetSampleRate(44100)
        SetSelectedMenuItem(SampleRateToolStripMenuItem, tsmSampleRate44k)
    End Sub

    ' ---------------------------------------------------------------------------------------------------

    Private Sub SetSampleSize(size As Integer)
        RaiseEvent SampleSizeChanged(Me, EventArgs.Empty)
        App.SampleSize = size '* 2
        Me.Reset()
    End Sub

    Private Sub tsmSampleSize1024_Click(sender As Object, e As EventArgs) Handles tsmSampleSize1024.Click
        SetSampleSize(1024)
        SetSelectedMenuItem(SampleSizeToolStripMenuItem, tsmSampleSize1024)
    End Sub

    Private Sub tsmSampleSize2048_Click(sender As Object, e As EventArgs) Handles tsmSampleSize2048.Click
        SetSampleSize(2048)
        SetSelectedMenuItem(SampleSizeToolStripMenuItem, tsmSampleSize2048)
    End Sub

    Private Sub tsmSampleSize4096_Click(sender As Object, e As EventArgs) Handles tsmSampleSize4096.Click
        SetSampleSize(4096)
        SetSelectedMenuItem(SampleSizeToolStripMenuItem, tsmSampleSize4096)
    End Sub

    Private Sub tsmSampleSize8192_Click(sender As Object, e As EventArgs) Handles tsmSampleSize8192.Click
        SetSampleSize(8192)
        SetSelectedMenuItem(SampleSizeToolStripMenuItem, tsmSampleSize8192)
    End Sub

#End Region

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

        For Each item As ToolStripItem In SampleRateToolStripMenuItem.DropDownItems
            item.Enabled = False
        Next

        For Each item As ToolStripItem In SampleSizeToolStripMenuItem.DropDownItems
            item.Enabled = False
        Next

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

        For Each item As ToolStripItem In SampleRateToolStripMenuItem.DropDownItems
            item.Enabled = True
        Next

        For Each item As ToolStripItem In SampleSizeToolStripMenuItem.DropDownItems
            item.Enabled = True
        Next

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

