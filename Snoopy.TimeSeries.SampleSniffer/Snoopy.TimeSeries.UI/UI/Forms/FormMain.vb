Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Text
Imports System.Windows.Forms
Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks

Public Class FormMain

#Region " -- Locals -- "

    Private _SampleMaxFrames As Integer = 32
    Private _SampleRate As Integer

    Private _ProcessingPipeline As TimeSeriesProcessingPipeline

    Public Sub New()

        InitializeComponent()

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, False)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

        AddHandler App.SampleRateChanged, AddressOf App_SampleRateChanged
        AddHandler App.SampleSizeChanged, AddressOf App_SampleSizeChanged

    End Sub

#End Region

    'Protected Overrides ReadOnly Property CreateParams() As CreateParams
    '    Get
    '        Dim cp As CreateParams = MyBase.CreateParams
    '        ' Turn on WS_EX_COMPOSITED
    '        cp.ExStyle = cp.ExStyle Or &H2000000
    '        Return cp
    '    End Get
    'End Property

    Private Sub ProcessSelectedTimeSeries()

        Dim series As TimeSeries = TimeSeriesViewer.SelectedTimeSeries
        If series Is Nothing Then Exit Sub

        Try
            Cursor = Cursors.WaitCursor

            _ProcessingPipeline.Process(series)
            TimeSeriesViewer.SelectedTimeSeries = series

        Catch ex As Exception
            TimeSeriesCapture.Stop()
            Throw
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub ProcessAllQueryCriteria()

        If QueryBuilder.Queries.Count = 0 Then Exit Sub

        Cursor = Cursors.WaitCursor

        'Dim wait As New FormPleaseWait With {.Maximum = QueryBuilder.Queries.Count}

        Try

            'Dim exe As Action = Sub()

            For i As Integer = 0 To QueryBuilder.Queries.Count - 1
                Dim query As TimeSeriesQuery = QueryBuilder.Queries(i)
                _ProcessingPipeline.Process(query.Criteria)
                query.Update()
            Next

            '                    End Sub


        Catch ex As Exception
            TimeSeriesCapture.Stop()
            Throw
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private _UpdatingStatusStrip As Boolean

    Private Sub StartStatusStripUpdateThread()
        Dim StatusThread As Thread
        StatusThread = New Thread(AddressOf UpdateStatusStrip)
        StatusThread.Name = "UpdateStatusStrip"
        StatusThread.Priority = ThreadPriority.Lowest
        StatusThread.Start()
    End Sub

    Private Sub UpdateStatusStrip()

        If Not _UpdatingStatusStrip Then
            _UpdatingStatusStrip = True ' Make sure we don't enter twice.

            While TimeSeriesExplorer.FrameBuffer.Count = 0
                ' Wait for frames...
                Thread.Sleep(100)
            End While

            Dim frameLen As Integer = App.SampleSize * 1000 \ App.SampleRate
            While TimeSeriesCapture.IsRunning
                Invoke(Sub(flen As Integer) RefreshStatusStrip(flen), New Object() {frameLen})
                Thread.Sleep(1000)
            End While

            _UpdatingStatusStrip = False

        End If

    End Sub

    ' For cross-threaded performance...
    Private Sub RefreshStatusStrip(flen As Integer)

        Dim frameCount As Integer

        If frameCount <> _SampleMaxFrames Then
            frameCount = TimeSeriesExplorer.FrameBuffer.Count
            tsStatusLabel.Text = String.Format("Showing {0} / {1} frames ({2:F3}s.)", frameCount, _SampleMaxFrames, (flen * frameCount) / 1000)
            'tsStatusLabel.Text = String.Concat("Showing ", frameCount.ToString, " / ", _SampleMaxFrames.ToString, " frames (", ((flen * frameCount) / 1000).ToString, "s.)") ' TODO: Performance
        End If

        Dim fps As Double = TimeSeriesCapture.FPS

        tsFPS.Text = String.Format("FPS: {0}", fps)

        'Dim lastSeries As TimeSeries = TimeSeriesExplorer.FrameBuffer(0) '.ToList.Last
        'tsDataSamples.Text = String.Format("Frame Size: {0}x{1} ({2}ms.)", lastSeries.Frames.Length, lastSeries.Frames(0).Length, flen)

        StatusStrip.Refresh()

    End Sub

    Private Sub UpdateStatusStripLables()

        Dim frameLen As Integer = App.SampleSize * 1000 \ App.SampleRate
        Dim lastSeries As TimeSeries
        Dim frameCount As Integer

        lastSeries = TimeSeriesExplorer.FrameBuffer.ToList.Last
        frameCount = TimeSeriesExplorer.FrameBuffer.Count

        tsStatusLabel.Text = String.Format("Showing {0} frames ({1:F3}s.)", frameCount, (frameLen * _SampleMaxFrames) / 1000)
        tsDataSamples.Text = String.Format("Frame Size: {0}x{1} ({2}ms.)", lastSeries.Frames.Length, lastSeries.Frames(0).Length, frameLen)

    End Sub

    Private Sub ExecuteStaticQuery(query As TimeSeriesQuery)
        query.Results.Clear()

        Dim wait As New FormPleaseWait() With {.Maximum = TimeSeriesExplorer.FrameBuffer.Count}
        wait.Message = String.Format("Executing query: {0}, Id: {1}", query.Name, query.Id)

        Dim cancel As Boolean
        Dim cancelAction As New Action(Sub() cancel = True)

        Try

            Dim exe As Action = Sub()
                                    Try
                                        For i As Integer = 0 To TimeSeriesExplorer.FrameBuffer.Count - 1
                                            If cancel Then Exit For
                                            query.Execute(TimeSeriesExplorer.FrameBuffer(i), QueryBuilder.Actions(query)) ' <--- this is funky -- move into the QueryMan!
                                            wait.SetProgress(i)
                                        Next
                                    Catch ex As Exception
                                        Invoke(New Action(Sub() wait.Close()))
                                        App.ShowException(ex)
                                    End Try
                                End Sub

            wait.Show(Me, exe, cancelAction)

            If wait.Cancelled Then
                ' No longer in sync with the buffer...
                query.Results.Clear()
            End If

        Catch ex As Exception
            TimeSeriesCapture.Stop()
            Throw
        Finally

        End Try

    End Sub

    Private Sub RefreshDataDisplay()

        If TimeSeriesCapture.IsRunning Then


        Else

            TimeSeriesExplorer.Redraw()
            TimeSeriesExplorer.PlotLabeledQueryResults(QueryBuilder.LabeledResults)

        End If

    End Sub

    Private Sub ProcessCurrentBuffer()

        Cursor = Cursors.WaitCursor

        Dim wait As New FormPleaseWait() With {.Maximum = TimeSeriesExplorer.FrameBuffer.Count}

        Dim proc As Action = Sub()
                                 Try
                                     _ProcessingPipeline.Process(TimeSeriesExplorer.FrameBuffer, AddressOf wait.SetProgress)
                                 Catch ex As Exception
                                     Invoke(New Action(Sub() wait.Close()))
                                     App.ShowException(ex)
                                 End Try
                             End Sub

        wait.Message = "Processing frame buffer..."
        wait.Show(Me, proc, AddressOf _ProcessingPipeline.Cancel)

        If Not wait.Cancelled Then

            ProcessAllQueryCriteria()

            If QueryBuilder.Queries.Count > 0 Then

                ' TODO: Move this into the QueryManager!
                For i As Integer = 0 To QueryBuilder.Queries.Count - 1
                    ExecuteStaticQuery(QueryBuilder.Queries(i))
                Next

            End If

            TimeSeriesExplorer.Redraw()

        Else
            ' What should we do here? FrameBuffer and queries may no longer be in sync...
        End If

        Cursor = Cursors.Default

    End Sub

    Private Sub ProcessAndUpdateDataDisplayFull()

        Try

            If TimeSeriesCapture.IsRunning Then

                TimeSeriesCapture.Pause()

                ProcessCurrentBuffer()
                ProcessSelectedTimeSeries()
                ProcessAllQueryCriteria()

                TimeSeriesCapture.Resume()

            Else

                ProcessSelectedTimeSeries()
                ProcessAllQueryCriteria()

                If TimeSeriesExplorer.FrameBuffer.Count > 0 Then
                    ProcessCurrentBuffer()
                End If

            End If

            UpdateStatusStripLables()

            TimeSeriesExplorer.PlotLabeledQueryResults(QueryBuilder.LabeledResults)

        Catch ex As Exception
            Throw
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub OnSampleBufferUpdate(series As TimeSeries)

        Try

            _ProcessingPipeline.Process(series)

            TimeSeriesExplorer.FrameBuffer.Add(series)

            ' TODO: Move this into a QueryManager...
            For i As Integer = 0 To QueryBuilder.Queries.Count - 1
                Dim query As TimeSeriesQuery = QueryBuilder.Queries(i)
                query.Execute(series, QueryBuilder.Actions(query))
            Next

            TimeSeriesExplorer.PlotLabeledQueryResults(QueryBuilder.LabeledResults)

        Catch ex As Exception

            TimeSeriesCapture.Stop()

            Dim ExViewer As New FormExceptionViewer
            ExViewer.Show(Me, ex)

        End Try

    End Sub

#Region " -- Form -- "

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles Me.Load

        ' TODO: Make dynamic... load by Type from app settings...

        Preprocessor.SampleRate = App.SampleRate
        Preprocessor.AddProcessor(Of Autocorrelate)(True, False)
        Preprocessor.AddProcessor(Of Scaler)(True, False)
        Preprocessor.AddProcessor(Of LinearInterpolator)()
        Preprocessor.AddProcessor(Of ZeroInserter)(True, False)
        Preprocessor.AddProcessor(Of Dither)()
        Preprocessor.AddProcessor(Of DCBlocker)()
        Preprocessor.AddProcessor(Of CShifter)()
        Preprocessor.AddProcessor(Of GammaScaler)()
        Preprocessor.AddProcessor(Of EnvelopeExtractor)()
        Preprocessor.AddProcessor(Of LogNegativeScaler)()
        Preprocessor.AddProcessor(Of ThresholdCrossing)()
        Preprocessor.AddProcessor(Of ThresholdExtractor)()
        Preprocessor.AddProcessor(Of SavitzkyGolay)()
        Preprocessor.AddProcessor(Of Clipper)()
        Preprocessor.AddProcessor(Of ZeroEnergy)(True, False)
        Preprocessor.AddProcessor(Of Decimator)()
        Preprocessor.AddProcessor(Of LinearTrend)()
        Preprocessor.AddProcessor(Of MeanSubtractor)()
        Preprocessor.AddProcessor(Of SimpleSmoother)()
        Preprocessor.AddProcessor(Of GaussianSmoother)()
        Preprocessor.AddProcessor(Of TemporalSmoother)()
        Preprocessor.AddProcessor(Of Standerdizer)()
        Preprocessor.AddProcessor(Of TridiagonalFilter)()
        Preprocessor.AddProcessor(Of FIRLowPassFilter)()
        Preprocessor.AddProcessor(Of FIRHighPassFilter)()
        Preprocessor.AddProcessor(Of FIRBandPassFilter)()
        Preprocessor.AddProcessor(Of FIRBandStopFilter)()
        Preprocessor.AddProcessor(Of Compressor)(True, False)
        Preprocessor.AddProcessor(Of ZNormalizer)()
        Preprocessor.AddProcessor(Of AmplitudeNormalizer)()
        Preprocessor.AddProcessor(Of AmplitudeEqualizer)()
        Preprocessor.AddProcessor(Of Whitener)()
        Preprocessor.AddProcessor(Of QuadMirror)()

        TransformChooser.SampleRate = App.SampleRate
        TransformChooser.AddTransformer(Of LOGFFT)()
        TransformChooser.AddTransformer(Of RASPEC)()
        TransformChooser.AddTransformer(Of FWTFFT)()
        TransformChooser.AddTransformer(Of LOGCZT)()
        TransformChooser.AddTransformer(Of LOGHILBERT)()
        TransformChooser.AddTransformer(Of LPC)()
        TransformChooser.AddTransformer(Of MFCC)()
        TransformChooser.AddTransformer(Of WVD)()
        TransformChooser.AddTransformer(Of DHT)()
        TransformChooser.AddTransformer(Of FSCT)()
        TransformChooser.AddTransformer(Of MDCT)()
        TransformChooser.AddTransformer(Of SONE)()
        TransformChooser.AddTransformer(Of CHROMA)()

        Postprocessor.SampleRate = App.SampleRate
        Postprocessor.AddProcessor(Of SNNFilter)()
        Postprocessor.AddProcessor(Of GaborFilter)()
        Postprocessor.AddProcessor(Of DilationFilter)()
        Postprocessor.AddProcessor(Of KuwaharaFilter)()
        Postprocessor.AddProcessor(Of MedianFilter)()
        Postprocessor.AddProcessor(Of HaarDecomposer)()
        Postprocessor.AddProcessor(Of Laplacian)()
        Postprocessor.AddProcessor(Of Rotator)()
        Postprocessor.AddProcessor(Of PeakSorter)()
        Postprocessor.AddProcessor(Of Delta2D)()
        Postprocessor.AddProcessor(Of Convolver)()
        Postprocessor.AddProcessor(Of ThresholdZero)()
        Postprocessor.AddProcessor(Of TopPeakExtractor)()
        Postprocessor.AddProcessor(Of CentroidExtractor)()
        Postprocessor.AddProcessor(Of ZeroMask)()

        'QueryBuilder.AddQuery(Of DTWRadialQuery)("RadialMax (Post)")
        QueryBuilder.AddQuery(Of LSRQuery)("LSR (Pre)")
        QueryBuilder.AddQuery(Of DTWQuery)("DTW (Post)")
        QueryBuilder.AddQuery(Of DTWAccordQuery)("DTWAccord (Pre)")
        QueryBuilder.AddQuery(Of VQQuery)("VQ (Post)")
        QueryBuilder.AddQuery(Of FPQuery)("FP (Post)")
        QueryBuilder.AddQuery(Of MinHashQuery)("MINHASH (Post)")
        QueryBuilder.AddQuery(Of PAAQuery)("PAA (Pre)")
        QueryBuilder.AddQuery(Of PAADTWQuery)("PAADTW (Pre)")
        QueryBuilder.AddQuery(Of SSIMQuery)("SSIM (Post)")
        QueryBuilder.AddQuery(Of TSTSQuery)("TSTS (Pre)")
        QueryBuilder.AddQuery(Of YinPitchQuery)("YinPitch (Pre)")
        QueryBuilder.AddQuery(Of DWPitchQuery)("DWPitch (Pre)")
        QueryBuilder.AddQuery(Of CorrelationQuery)("Correlation (Pre)")

        _ProcessingPipeline = New TimeSeriesProcessingPipeline(TransformChooser.SelectedTransformer)
        _ProcessingPipeline.PreProcessors = Preprocessor.Processors
        _ProcessingPipeline.PostProcessors = Postprocessor.Processors

        'TimeSeriesViewer.Pallet = TimeSeriesExplorer.SelectedPallet
        'TimeSeriesViewer.PalletThreshold = TimeSeriesExplorer.PalletThreshold

        _SampleMaxFrames = TimeSeriesExplorer.SampleMaxFrames

        ' Cap the results at the number of buffered frames.
        QueryBuilder.MaxQueryResults = TimeSeriesExplorer.SampleMaxFrames

        Cursor = Cursors.Default

        _SampleRate = App.SampleRate
        tsSampleRate.Text = String.Format("{0} Hz.", _SampleRate.ToString)
        tsSampleRate.BackColor = Color.AliceBlue

    End Sub

    Private Sub FormMain_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        RefreshDataDisplay()
        TimeSeriesViewer.UpdateDisplay()
    End Sub

    Private Sub TransformChooser_Changed(sender As Object, e As EventArgs) Handles TransformChooser.Changed
        _ProcessingPipeline.Transformer = TransformChooser.SelectedTransformer
        ProcessAndUpdateDataDisplayFull()
    End Sub

    Private Sub Preprocessor_Changed(sender As Object, e As EventArgs) Handles Preprocessor.Changed
        ProcessAndUpdateDataDisplayFull()
    End Sub

    Private Sub Postprocessor_Changed(sender As Object, e As EventArgs) Handles Postprocessor.Changed
        ProcessAndUpdateDataDisplayFull()
    End Sub

    Private Sub ShowFPSToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowFPSToolStripMenuItem.Click
        tsFPS.Visible = ShowFPSToolStripMenuItem.Checked
    End Sub

#Region " -- QueryBuilder -- "

    Private Sub QueryBuilder_QueryChanged(sender As Object, e As TimeSeriesQueryEventArgs) Handles QueryBuilder.QueryChanged
        ExecuteStaticQuery(e.Query)
        RefreshDataDisplay()
    End Sub

    Private Sub QueryBuilder_QueryLabelChanged(sender As Object, e As TimeSeriesQueryEventArgs) Handles QueryBuilder.QueryLabelChanged
        RefreshDataDisplay()
    End Sub

    Private Sub QueryBuilder_QueryRemoved(sender As Object, e As TimeSeriesQueryEventArgs) Handles QueryBuilder.QueryRemoved
        RefreshDataDisplay()
    End Sub

    Private Sub QueryBuilder_SelectedQueryChanged(sender As Object, e As TimeSeriesQueryEventArgs) Handles QueryBuilder.SelectedQueryChanged
        TimeSeriesViewer.SelectedTimeSeries = e.Query.Criteria
    End Sub

#End Region

#Region " -- PalletManager -- "

    Private WithEvents _PalletManager As PalettetManager = PalettetManager.Instance

    Private Sub PalletManager_SelectedPalletChanged(sender As Object, e As EventArgs) Handles _PalletManager.SelectedPalletChanged ' TimeSeriesExplorer.SelectedPalletChanged
        TimeSeriesExplorer.PlotLabeledQueryResults(QueryBuilder.LabeledResults)
    End Sub

    Private Sub PalletManager_PalletThresholdChanged(sender As Object, e As EventArgs) Handles _PalletManager.PalletThresholdChanged ' TimeSeriesExplorer.SelectedPalletChanged
        TimeSeriesExplorer.PlotLabeledQueryResults(QueryBuilder.LabeledResults)
    End Sub

#End Region

#Region " -- TimeSeriesExplorer -- "

    Private Sub TimeSeriesExplorer_SelectedTimeSeriesChanged(sender As Object, e As EventArgs) Handles TimeSeriesExplorer.SelectedTimeSeriesChanged
        If TimeSeriesExplorer.SelectedTimeSeriesIndexStart > -1 AndAlso TimeSeriesExplorer.SelectedTimeSeriesIndexEnd > -1 Then
            If TimeSeriesExplorer.SelectedTimeSeriesIndexStart = TimeSeriesExplorer.SelectedTimeSeriesIndexEnd Then
                tsSelectedSeries.Text = String.Format("Selected frame: {0}", TimeSeriesExplorer.SelectedTimeSeriesIndexStart + 1)
            Else
                tsSelectedSeries.Text = String.Format("Selected frames: {0} - {1}", TimeSeriesExplorer.SelectedTimeSeriesIndexStart + 1, TimeSeriesExplorer.SelectedTimeSeriesIndexEnd + 1)
            End If
        Else
            tsSelectedSeries.Text = "No Frame"
        End If
        TimeSeriesViewer.SelectedTimeSeries = TimeSeriesExplorer.SelectedTimeSeries
        QueryBuilder.SelectedTimeSeries = TimeSeriesViewer.SelectedTimeSeries
        TimeSeriesExplorer.PlotLabeledQueryResults(QueryBuilder.LabeledResults)
    End Sub

    Private Sub TimeSeriesExplorer_SampleMaxFramesChanged(sender As Object, e As EventArgs) Handles TimeSeriesExplorer.SampleMaxFramesChanged
        _SampleMaxFrames = TimeSeriesExplorer.SampleMaxFrames
        QueryBuilder.MaxQueryResults = _SampleMaxFrames
        If TimeSeriesCapture.IsRunning Then
            tsFilename.Text = String.Format("Capturing... Buffer Max Frames: {0}", TimeSeriesExplorer.SampleMaxFrames)
        End If
    End Sub

    Private Sub TimeSeriesExplorer_TimeSeriesBufferLoaded(sender As Object, e As EventArgs) Handles TimeSeriesExplorer.TimeSeriesBufferLoaded
        ProcessCurrentBuffer()
        UpdateStatusStripLables()
        TimeSeriesExplorer.PlotLabeledQueryResults(QueryBuilder.LabeledResults)
        If Not String.IsNullOrWhiteSpace(TimeSeriesExplorer.FrameBuffer.FileName) Then
            tsFilename.Text = String.Format("File: {0}", Path.GetFileName(TimeSeriesExplorer.FrameBuffer.FileName))
        End If
    End Sub

    Private Sub TimeSeriesExplorer_TimeSeriesBufferSaved(sender As Object, e As EventArgs) Handles TimeSeriesExplorer.TimeSeriesBufferSaved
        tsFilename.Text = String.Format("File: {0}", Path.GetFileName(TimeSeriesExplorer.FrameBuffer.FileName))
    End Sub

    Private Sub TimeSeriesExplorer_ShowCentroidsChanged(sender As Object, e As EventArgs) Handles TimeSeriesExplorer.ShowCentroidsChanged
        TimeSeriesExplorer.PlotLabeledQueryResults(QueryBuilder.LabeledResults)
    End Sub

#End Region

#Region " -- TimeSeriesCapture -- "

    Private Sub TimeSeriesCapture_CaptureBufferFull(sender As Object, e As CaptureBufferEventArgs) Handles TimeSeriesCapture.CaptureBufferFull
        OnSampleBufferUpdate(e.TimeSeries)
    End Sub

    Private Sub TimeSeriesCapture_CapturePause(sender As Object, e As EventArgs) Handles TimeSeriesCapture.CapturePause
        tsFilename.Text = "Capture Paused..."
    End Sub

    Private Sub TimeSeriesCapture_CaptureResume(sender As Object, e As EventArgs) Handles TimeSeriesCapture.CaptureResume
        tsFilename.Text = String.Format("Capturing... Buffer Max Frames: {0}", TimeSeriesExplorer.SampleMaxFrames)
    End Sub

    Private Sub TimeSeriesCapture_CaptureStart(sender As Object, e As EventArgs) Handles TimeSeriesCapture.CaptureStart
        TimeSeriesExplorer.FrameBuffer.Clear()
        QueryBuilder.LabeledResults.Clear()
        TimeSeriesExplorer.Redraw()

        If TimeSeriesExplorer.FrameBuffer.MaxSize <> TimeSeriesExplorer.SampleMaxFrames Then
            TimeSeriesExplorer.FrameBuffer.MaxSize = TimeSeriesExplorer.SampleMaxFrames
        End If

        TimeSeriesExplorer.AllowLoadSave = False

        tsFilename.ForeColor = Color.Red
        tsFilename.Text = String.Format("Capturing... Buffer Max Frames: {0}", TimeSeriesExplorer.SampleMaxFrames)
        tsSelectedSeries.Text = "No Frame"

        StartStatusStripUpdateThread()

    End Sub

    Private Sub TimeSeriesCapture_CaptureStop(sender As Object, e As EventArgs) Handles TimeSeriesCapture.CaptureStop
        tsFilename.ForeColor = Color.Black
        tsFilename.Text = "No File"
        TimeSeriesExplorer.AllowLoadSave = True
    End Sub

    Private Sub TimeSeriesCapture_SampleRateChanged(sender As Object, e As EventArgs) Handles TimeSeriesCapture.SampleRateChanged

    End Sub

    Private Sub TimeSeriesCapture_SampleSizeChanged(sender As Object, e As EventArgs) Handles TimeSeriesCapture.SampleSizeChanged

    End Sub

#End Region

#Region " -- TimeSeriesViewer -- "

    Private Sub TimeSeriesViewer_Open(sender As Object, e As TimeSeriesEventArgs) Handles TimeSeriesViewer.Open
        ProcessSelectedTimeSeries()
        Me.QueryBuilder.SelectedTimeSeries = Me.TimeSeriesViewer.SelectedTimeSeries
    End Sub

#End Region

#End Region

#Region " -- App -- "

    Private Sub App_SampleRateChanged(sender As Object, e As EventArgs)
        _SampleRate = App.SampleRate

        Dim tsv As Boolean
        Dim tse As Boolean

        If TimeSeriesViewer.SelectedTimeSeries IsNot Nothing AndAlso TimeSeriesViewer.SelectedTimeSeries.SampleRate <> _SampleRate Then
            tsv = True
        End If
        If TimeSeriesExplorer.FrameBuffer.Count > 0 AndAlso TimeSeriesExplorer.FrameBuffer(0).SampleRate <> _SampleRate Then
            tse = True
        End If
        If tse Or tsv Then
            MsgBox("Warning: The application Sample Rate does not match the currently loaded TimeSeries data.", MsgBoxStyle.Exclamation, "Sample Rate Warning...")
        End If

        tsSampleRate.Text = String.Format("{0} Hz.", _SampleRate.ToString)

        Preprocessor.SampleRate = App.SampleRate
        TransformChooser.SampleRate = App.SampleRate
        Postprocessor.SampleRate = App.SampleRate

    End Sub

    Private Sub App_SampleSizeChanged(sender As Object, e As EventArgs)
        TimeSeriesCapture.Pause()
        TimeSeriesExplorer.FrameBuffer.Clear()
        ' May need to clear the Query results here too!
        TimeSeriesCapture.Resume()
    End Sub

#End Region

End Class
