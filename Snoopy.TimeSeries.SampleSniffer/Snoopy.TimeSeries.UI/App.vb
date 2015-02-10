Imports System.Threading

Public NotInheritable Class App

    Public Shared Event SampleSizeChanged(sender As Object, e As EventArgs)
    Public Shared Event SampleRateChanged(sender As Object, e As EventArgs)

    Private Shared _AppForm As Form
    Private Shared _App As App

    Private Sub New()
    End Sub

    <STAThread>
    Public Shared Sub Main()

        'Dim a As Double() = New Double() {0, 1, 2, 3, 4}
        'Dim b As Double() = New Double() {5, 6, 7, 8, 9}
        'Dim c As Double() = New Double() {5, 6, 7, 8, 9}

        'Dim ts1 As New TimeSeries(a, 22050)
        'Dim ts2 As New TimeSeries(b, 22050)

        'Dim ts3 As New TimeSeries(c, 22050)

        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf UnhandledException
        AddHandler Application.ThreadException, AddressOf ThreadException

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        _AppForm = New FormMain
        Application.Run(_AppForm)

        If _AppForm IsNot Nothing Then
            _AppForm.Dispose()
            _AppForm = Nothing
        End If

    End Sub

    Public Sub Run1()
        Application.Run(_AppForm)
    End Sub

    Private Shared Sub UnhandledException(sender As Object, e As UnhandledExceptionEventArgs)
        ShowException(New Exception(e.ExceptionObject.ToString))
    End Sub

    Private Shared Sub ThreadException(sender As Object, e As ThreadExceptionEventArgs)
        ShowException(e.Exception)
    End Sub

    Public Shared Sub ShowException(ex As Exception)
        If Form.ActiveForm IsNot Nothing Then
            Form.ActiveForm.UseWaitCursor = False
        End If
        If _AppForm.InvokeRequired Then
            _AppForm.Invoke(New Action(Sub() ShowException(ex)))
        Else
            Dim ExViewer As New FormExceptionViewer
            ExViewer.Show(_AppForm, ex)
        End If
    End Sub

    Private Sub Test()


        Dim ts As TimeSeries = TimeSeries.Load("d:\mySeries.ts")

        ' Create a Transformer and couple of processors...
        Dim FFT As New LOGFFT
        Dim TDF As New TridiagonalFilter
        Dim LPF As New FIRLowPassFilter
        Dim TPE As New TopPeakExtractor

        ' Option 1. Perform manual, one-by-one processing...
        LPF.Process(ts)
        TDF.Process(ts)
        FFT.Transform(ts)
        TPE.Process(ts)

        ' Option 2. Create a processing pipeline...
        Dim Pipeline As New TimeSeriesProcessingPipeline(FFT)

        Pipeline.PreProcessors.Add(LPF)
        Pipeline.PreProcessors.Add(TDF)
        Pipeline.PostProcessors.Add(TPE)
        Pipeline.Process(ts)

        ' Try a different transformer.
        Pipeline.Transformer = New MFCC
        ' Get rid of a processor.
        Pipeline.PreProcessors.Remove(TDF)
        Pipeline.Process(ts)

        ' 1st. Load a reference series; the Criteria...
        Dim ts1 As TimeSeries = TimeSeries.Load("d:\mySeries1.ts")

        ' 2nd. Load a candidate series; the Data...
        Dim ts2 As TimeSeries = TimeSeries.Load("d:\mySeries2.ts")

        ' 3rd. Create a query...
        Dim dtw As New DTWQuery(ts1)

        dtw.Execute(ts2)


    End Sub

    Public Shared Property AudioInputDevice As Integer
        Get
            Return My.Settings.AudioInputDevice
        End Get
        Set(value As Integer)
            My.Settings.AudioInputDevice = value
            My.Settings.Save()
        End Set
    End Property

    Public Shared Property SampleRate As Integer
        Get
            Return My.Settings.SampleRate
        End Get
        Set(value As Integer)
            If value <> My.Settings.SampleRate Then
                My.Settings.SampleRate = value
                My.Settings.Save()
                RaiseEvent SampleRateChanged(Nothing, EventArgs.Empty)
            End If
        End Set
    End Property

    Public Shared Property SampleSize As Integer
        Get
            Return My.Settings.SampleSize
        End Get
        Set(value As Integer)
            If value <> My.Settings.SampleSize Then
                My.Settings.SampleSize = value
                My.Settings.Save()
                RaiseEvent SampleSizeChanged(Nothing, EventArgs.Empty)
            End If
        End Set
    End Property

    Public Shared Property BitsPerSample As Integer
        Get
            Return My.Settings.BitsPerSample
        End Get
        Set(value As Integer)
            My.Settings.BitsPerSample = value
            My.Settings.Save()
        End Set
    End Property


    Public Shared Function conv(a As Double(), b As Double()) As Double()
        Dim y As Double() = New Double(a.length + b.length - 2) {}

        ' make sure that a is the shorter sequence
        If a.length > b.length Then
            Dim tmp As Double() = a
            a = b
            b = tmp
        End If

        For lag As Integer = 0 To y.length - 1
            y(lag) = 0

            ' where do the two signals overlap?
            Dim start As Integer = 0
            ' we can't go past the left end of (time reversed) a
            If lag > a.length - 1 Then
                start = lag - a.length + 1
            End If

            Dim [end] As Integer = lag
            ' we can't go past the right end of b
            If [end] > b.length - 1 Then
                [end] = b.length - 1
            End If

            Debug.WriteLine("lag = " & lag & ": " & start & " to " & [end])
            For n As Integer = start To [end]
                Debug.WriteLine(" ai = " & (lag - n) & ", bi = " & n)
                y(lag) += b(n) * a(lag - n)
            Next
        Next

        Return (y)
    End Function

End Class
