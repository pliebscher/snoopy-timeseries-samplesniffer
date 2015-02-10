Imports System.Runtime.InteropServices
Public Class TimeSeriesExplorer

#Region " -- Locals -- "

    Private _PinnedBmpArray As Byte()
    Private _PinnedBmpArrayHandle As GCHandle
    Private _PinnedBmp As Bitmap

    Private _BufferMaxFrames As Integer = 32
    Private _FrameBuffer As New TimeSeriesBuffer(32)

    Private _SelectedTimeSeries As TimeSeries
    Private _SelectedTimeSeriesIndexStart As Integer = -1
    Private _SelectedTimeSeriesIndexEnd As Integer = -1

    Private _SelectedPallet As IColorPallet = PalletManager.DefaultPallet
    Private _PalletThreshhold As Integer = 75

    Private _AllowLoadSave As Boolean = True
    Private _ShowCentroids As Boolean
    Private _ShowSpectrogram As Boolean = True

    Public Event TimeSeriesBufferLoaded(sender As Object, e As EventArgs)
    Public Event TimeSeriesBufferSaved(sender As Object, e As EventArgs)
    Public Event SelectedTimeSeriesChanged(sender As Object, e As EventArgs)
    Public Event SelectedPalletChanged(sender As Object, e As EventArgs)
    Public Event PalletThreshholdChanged(sender As Object, e As EventArgs)
    Public Event ShowCentroidsChanged(sender As Object, e As EventArgs)
    Public Event SampleMaxFramesChanged(sender As Object, e As EventArgs)

    Public Sub New()

        InitializeComponent()

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, False)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

        AddHandler _FrameBuffer.TimeSeriesAdded, AddressOf Me.OnTimeSeriesAdded
        AddHandler _FrameBuffer.BufferSizeChanged, AddressOf Me.OnTimeSeriesBufferSizeChanged
        AddHandler _FrameBuffer.BufferCleared, AddressOf Me.OnBufferCleared

    End Sub

#End Region

#Region " -- Form -- "

    Private Sub TimeSeriesCapture_Load(sender As Object, e As EventArgs) Handles Me.Load

        For Each pallet As KeyValuePair(Of String, IColorPallet) In PalletManager.Pallets
            Dim MenuItem As New ToolStripMenuItem(pallet.Key, Nothing, AddressOf ColorPalletToolStripMenu_PalletChanged) With {.CheckOnClick = True}
            ColorPalletToolStripMenuItem.DropDownItems.Add(MenuItem)
        Next

        DirectCast(ColorPalletToolStripMenuItem.DropDownItems(0), ToolStripMenuItem).Checked = True

        vsPalletThresh.Value = _PalletThreshhold

        pbColorPallet.Image = Imaging.GetPalletGradientBitmap(_SelectedPallet, True)

        ResetDataScrollBar(False)

        pbData.Size = pnlData.Size

    End Sub

    Private Sub TimeSeriesExplorer_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        DestroyPinnedBmp()
    End Sub

    Private Sub pbData_MouseClick(sender As Object, e As MouseEventArgs) Handles pbData.MouseClick

        If _FrameBuffer.Count = 0 Then Exit Sub

        Dim selectedFrameIndex As Integer = CInt(((e.X + 0.5) * _FrameBuffer.Count) / pbData.Width + 0.5) - 1
        Dim rawSeries As TimeSeries = _FrameBuffer(selectedFrameIndex).Clone

        If My.Computer.Keyboard.ShiftKeyDown AndAlso _SelectedTimeSeriesIndexStart <> selectedFrameIndex Then
            ' Muilti-Select... TODO: Not quite right when moving Right to Left... but close enough for now.
            If _SelectedTimeSeriesIndexStart > selectedFrameIndex Then
                ' (End) Left <-- Right (Start)
                _SelectedTimeSeriesIndexStart = selectedFrameIndex
            Else
                ' (Start) Left --> Right (End)
                rawSeries = _FrameBuffer(_SelectedTimeSeriesIndexStart).Clone
                _SelectedTimeSeriesIndexEnd = selectedFrameIndex
            End If

            For i As Integer = _SelectedTimeSeriesIndexStart + 1 To _SelectedTimeSeriesIndexEnd
                rawSeries = rawSeries.Join(_FrameBuffer(i))
            Next

        Else
            ' Single-Select...
            _SelectedTimeSeriesIndexStart = selectedFrameIndex
            _SelectedTimeSeriesIndexEnd = selectedFrameIndex
        End If

        _SelectedTimeSeries = rawSeries

        Redraw()

        RaiseEvent SelectedTimeSeriesChanged(Me, EventArgs.Empty)

    End Sub

    Private Sub OpenInViewerMenuItem_Click(sender As Object, e As EventArgs) Handles OpenInViewerMenuItem.Click
        Dim viewer As New FormTimeSeriesViewer
        With viewer
            .ViewControl.SelectedTimeSeries = _SelectedTimeSeries.Clone
            .ViewControl.Pallet = _SelectedPallet
            .ViewControl.PalletThreshold = _PalletThreshhold
            .ViewControl.ShowCentroids = _ShowCentroids
            .Show()
        End With
    End Sub

    Private Sub vsPalletThresh_Scroll(sender As Object, e As ScrollEventArgs) Handles vsPalletThresh.Scroll
        If _PalletThreshhold <> vsPalletThresh.Value Then
            _PalletThreshhold = vsPalletThresh.Value
            Redraw()
            RaiseEvent PalletThreshholdChanged(Me, e)
        End If
    End Sub

    Private Sub tsmSampleBuffers16_Click(sender As Object, e As EventArgs) Handles tsmSampleBuffers16.Click
        SetMaxSampleFrames(16)
        SetSelectedMenuItem(SampleBuffersToolStripMenuItem, tsmSampleBuffers16)
    End Sub

    Private Sub tsmSampleBuffers32_Click(sender As Object, e As EventArgs) Handles tsmSampleBuffers32.Click
        SetMaxSampleFrames(32)
        SetSelectedMenuItem(SampleBuffersToolStripMenuItem, tsmSampleBuffers32)
    End Sub

    Private Sub tsmSampleBuffers64_Click(sender As Object, e As EventArgs) Handles tsmSampleBuffers64.Click
        SetMaxSampleFrames(64)
        SetSelectedMenuItem(SampleBuffersToolStripMenuItem, tsmSampleBuffers64)
    End Sub

    Private Sub tsmSampleBuffers128_Click(sender As Object, e As EventArgs) Handles tsmSampleBuffers128.Click
        SetMaxSampleFrames(128)
        SetSelectedMenuItem(SampleBuffersToolStripMenuItem, tsmSampleBuffers128)
    End Sub

    Private Sub tsmSampleBuffers256_Click(sender As Object, e As EventArgs) Handles tsmSampleBuffers256.Click
        SetMaxSampleFrames(256)
        SetSelectedMenuItem(SampleBuffersToolStripMenuItem, tsmSampleBuffers256)
    End Sub

    Private Sub ColorPalletToolStripMenu_PalletChanged(sender As Object, e As EventArgs)
        Dim item As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        _SelectedPallet = PalletManager.Pallets(item.Text)
        pbColorPallet.Image = Imaging.GetPalletGradientBitmap(_SelectedPallet, True)
        Redraw()
        SetSelectedMenuItem(ColorPalletToolStripMenuItem, item)
        RaiseEvent SelectedPalletChanged(Me, e)
    End Sub

    Private Sub ShowCentroidsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowCentroidsToolStripMenuItem.Click
        _ShowCentroids = ShowCentroidsToolStripMenuItem.Checked
        Redraw()
        RaiseEvent ShowCentroidsChanged(Me, EventArgs.Empty)
    End Sub

    Private Sub ShowSpectrogramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowSpectrogramToolStripMenuItem.Click
        _ShowSpectrogram = ShowSpectrogramToolStripMenuItem.Checked
        If _ShowSpectrogram Then
            Redraw()
        Else
            Dim bmp As New Bitmap(pbData.Width, pbData.Height)
            pbData.Image = bmp
            Redraw()
        End If
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        If sfdTimeSeries.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            Dim path As String = sfdTimeSeries.FileName
            If path.EndsWith(".wav") Then
                WaveFile.Create(path, _FrameBuffer.ToTimeSeries(), My.Settings.BitsPerSample, 1)
            ElseIf path.EndsWith(".png") Then
                pbData.Image.Save(path, System.Drawing.Imaging.ImageFormat.Png)
            Else
                _FrameBuffer.Save(path)
                RaiseEvent TimeSeriesBufferSaved(Me, EventArgs.Empty)
            End If
        End If
    End Sub

    Private Sub SaveSelectionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveSelectionToolStripMenuItem.Click
        If sfdTimeSeries.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            Dim path As String = sfdTimeSeries.FileName
            If path.EndsWith(".wav") Then
                Dim ts As TimeSeries = _FrameBuffer.Crop(_SelectedTimeSeriesIndexStart, _SelectedTimeSeriesIndexEnd).ToTimeSeries
                WaveFile.Create(path, ts, My.Settings.BitsPerSample, 1)
            ElseIf path.EndsWith(".png") Then
                pbData.Image.Save(path, System.Drawing.Imaging.ImageFormat.Png) ' TODO: This needs to be cropped!!!
            Else
                _FrameBuffer.Crop(_SelectedTimeSeriesIndexStart, _SelectedTimeSeriesIndexEnd).Save(path)
            End If
        End If
    End Sub

    Private Sub LoadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadToolStripMenuItem.Click
        If ofdTimeSeries.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            Dim path As String = ofdTimeSeries.FileName
            Me.LoadBuffer(TimeSeriesBuffer.Load(path))
        End If
    End Sub

    Private Sub ClearSelectionMenuItem_Click(sender As Object, e As EventArgs) Handles ClearSelectionMenuItem.Click
        _SelectedTimeSeries = Nothing
        _SelectedTimeSeriesIndexStart = -1
        _SelectedTimeSeriesIndexEnd = -1
        Redraw()
        RaiseEvent SelectedTimeSeriesChanged(Me, EventArgs.Empty)
    End Sub

    Private Sub cmsDataSpectrogram_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles cmsDataSpectrogram.Opening

        ShowSpectrogramToolStripMenuItem.Checked = _ShowSpectrogram

        SaveSelectionToolStripMenuItem.Enabled = _SelectedTimeSeriesIndexStart > -1
        SaveToolStripMenuItem.Enabled = _FrameBuffer.Count > 0 And _AllowLoadSave

        LoadToolStripMenuItem.Enabled = _AllowLoadSave

        ClearSelectionMenuItem.Enabled = _SelectedTimeSeries IsNot Nothing
        OpenInViewerMenuItem.Enabled = _SelectedTimeSeries IsNot Nothing
        CropSelectionMenuItem.Enabled = _SelectedTimeSeries IsNot Nothing

        PlayStopSelectionMenuItem.Enabled = _FrameBuffer.Count > 0

    End Sub

    Private Sub CropSelectionMenuItem_Click(sender As Object, e As EventArgs) Handles CropSelectionMenuItem.Click
        Me.LoadBuffer(_FrameBuffer.Crop(_SelectedTimeSeriesIndexStart, _SelectedTimeSeriesIndexEnd))
    End Sub

    Private Sub pnlData_Resize(sender As Object, e As EventArgs) Handles pnlData.Resize
        If pbData.Location.X < 0 AndAlso hsbData.Value = hsbData.Maximum Then
            ResetDataScrollBar(True)
        Else
            ResetDataScrollBar(False)
        End If
    End Sub

    Private Sub hsbData_Scroll(sender As Object, e As ScrollEventArgs) Handles hsbData.Scroll

        If _FrameBuffer.Count = 0 Then Exit Sub
        If e.NewValue = e.OldValue Then
            Exit Sub
        End If

        Select Case e.Type
            Case ScrollEventType.SmallDecrement, ScrollEventType.LargeDecrement
                If e.NewValue = 0 Then
                    ' Compensate for ThumbTrack
                    pbData.Location = New Point(0, 0)
                Else
                    pbData.Location = New Point(pbData.Location.X + hsbData.SmallChange, 0)
                End If
            Case ScrollEventType.SmallIncrement, ScrollEventType.LargeIncrement
                If e.NewValue < e.OldValue Then
                    Exit Sub
                End If
                If (hsbData.Maximum - e.NewValue) < hsbData.SmallChange Then
                    ' Compensate for ThumbTrack
                    Dim buffWidth As Integer = _FrameBuffer.Count * _FrameBuffer(0).FrameCount
                    Dim diff As Integer = buffWidth - pnlData.Width
                    pbData.Location = New Point(-diff, 0)
                Else
                    pbData.Location = New Point(pbData.Location.X - hsbData.SmallChange, 0)
                End If

            Case ScrollEventType.ThumbTrack

                If e.NewValue > e.OldValue Then
                    ' Scrolling Right
                    pbData.Location = New Point(pbData.Location.X - (e.NewValue - e.OldValue), 0)
                ElseIf e.NewValue < e.OldValue Then
                    ' Scrolling Left
                    If e.NewValue = 0 Then
                        pbData.Location = New Point(0, 0)
                    Else
                        pbData.Location = New Point(pbData.Location.X + (e.OldValue - e.NewValue), 0)
                    End If
                End If

        End Select

    End Sub

#End Region

    ''' <summary>
    ''' This is called when a TimeSeries is added to the buffer...
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnTimeSeriesAdded(sender As Object, e As EventArgs)
        ' Shift the index of the selected series each time a new series is added...
        If _SelectedTimeSeriesIndexStart >= 0 Then
            _SelectedTimeSeriesIndexStart -= 1
        End If
        If _SelectedTimeSeriesIndexEnd >= 0 Then
            _SelectedTimeSeriesIndexEnd -= 1
        End If
        Redraw()
    End Sub

    Private Sub OnTimeSeriesBufferSizeChanged(sender As Object, e As EventArgs)
        InitPinnedBmp()
        ResetDataScrollBar(False)
    End Sub

    Private Sub OnBufferCleared(sender As Object, e As EventArgs)
        _SelectedTimeSeriesIndexStart = -1
        _SelectedTimeSeriesIndexEnd = -1

        'InitPinnedBmp()
        ResetDataScrollBar(False)

    End Sub

    Private Sub LoadBuffer(buffer As TimeSeriesBuffer)

        _FrameBuffer = buffer

        If _FrameBuffer(0).SampleRate <> App.SampleRate Then
            MsgBox(String.Format("Application Sample Rate ({0}) differes from TimeSeriesBuffer Sample Rate ({1})", My.Settings.SampleRate, _FrameBuffer(0).SampleRate), MsgBoxStyle.Exclamation, "Warning...")
        End If

        _SelectedTimeSeriesIndexStart = -1
        _SelectedTimeSeriesIndexEnd = -1

        If _BufferMaxFrames <> _FrameBuffer.MaxSize Then
            _BufferMaxFrames = _FrameBuffer.MaxSize
            Dim found As Boolean
            For Each tsItem As ToolStripMenuItem In SampleBuffersToolStripMenuItem.DropDownItems
                tsItem.Checked = False
                If CInt(tsItem.Text) = _BufferMaxFrames Then
                    found = True
                    tsItem.Checked = True
                End If
            Next
            If Not found Then
                Dim item As New ToolStripMenuItem(_BufferMaxFrames.ToString)
                item.Enabled = False
                item.Checked = True
                SampleBuffersToolStripMenuItem.DropDownItems.Add(item)
            End If
            RaiseEvent SampleMaxFramesChanged(Me, EventArgs.Empty)
        End If

        ' Re-add the handler as we overwrote the buffer. May be better to resize and copy the loaded buffer into the existing???
        AddHandler _FrameBuffer.TimeSeriesAdded, AddressOf Me.OnTimeSeriesAdded
        AddHandler _FrameBuffer.BufferSizeChanged, AddressOf Me.OnTimeSeriesBufferSizeChanged

        Redraw()

        ResetDataScrollBar(False)

        RaiseEvent TimeSeriesBufferLoaded(Me, EventArgs.Empty)

    End Sub

    Private Sub ResetDataScrollBar(focusLast As Boolean)

        If _FrameBuffer.Count <= 1 Then
            pbData.Location = New Point(0, 0)
            hsbData.Value = 0
            hsbData.Enabled = False
        Else

            Dim buffWidth As Integer = _FrameBuffer.Count * _FrameBuffer(0).FrameCount
            Dim frameWidth As Integer = _FrameBuffer(0).FrameCount

            If buffWidth > pnlData.Width Then

                hsbData.Maximum = buffWidth - pnlData.Width
                hsbData.Minimum = 0
                hsbData.SmallChange = frameWidth
                hsbData.LargeChange = frameWidth

                If focusLast Then
                    hsbData.Value = hsbData.Maximum
                    Dim diff As Integer = buffWidth - pnlData.Width
                    pbData.Location = New Point(-diff, 0)
                End If

                hsbData.Enabled = True
            Else
                ' Buffer is smaller than picturebox panel...
                pbData.Location = New Point(0, 0)
                hsbData.Value = 0
                hsbData.Enabled = False
            End If

        End If

    End Sub

    Private _LastSize As Size = New Size(0, 0) ' Track if dimensions have changed so so we can reset the pinned bitmap.

    Private Sub InitPinnedBmp()

        If _FrameBuffer.Count = 0 Then Exit Sub

        Dim w As Integer = _FrameBuffer.Count * _FrameBuffer(0).FrameCount
        Dim h As Integer = _FrameBuffer(0).FrameLength

        _LastSize.Width = w
        _LastSize.Height = h

        If _PinnedBmpArrayHandle.IsAllocated Then
            _PinnedBmpArrayHandle.Free()
        End If

        Dim format As Drawing.Imaging.PixelFormat = Drawing.Imaging.PixelFormat.Format32bppArgb
        Dim pixelBytes As Integer = Image.GetPixelFormatSize(format) \ 8
        Dim stride As Integer = w * pixelBytes

        _PinnedBmpArray = New Byte(stride * h - 1) {}
        _PinnedBmpArrayHandle = GCHandle.Alloc(_PinnedBmpArray, GCHandleType.Pinned)
        _PinnedBmp = New Bitmap(w, h, stride, format, Marshal.UnsafeAddrOfPinnedArrayElement(_PinnedBmpArray, 0))

        ResetDataScrollBar(True)

    End Sub

    Private Sub DestroyPinnedBmp()
        If _PinnedBmp IsNot Nothing Then
            _PinnedBmp.Dispose()
            _PinnedBmp = Nothing
        End If
        If _PinnedBmpArrayHandle.IsAllocated Then
            _PinnedBmpArrayHandle.Free()
        End If
    End Sub

    Public Sub Redraw()

        If pbData.Width = 0 Then Exit Sub ' Width is 0 when minimized.
        If _FrameBuffer.Count = 0 Then Exit Sub

        If _ShowSpectrogram Then

            Dim w As Integer = _FrameBuffer.Count * _FrameBuffer(0).FrameCount
            Dim h As Integer = _FrameBuffer(0).FrameLength
            Dim pixelBytes As Integer = 4
            Dim bmp As Bitmap = Nothing

            If _PinnedBmpArray Is Nothing OrElse _LastSize.Width <> w OrElse _LastSize.Height <> h Then ' (w * pixelBytes) * h <> _PinnedBmpArray.Length Then
                InitPinnedBmp()
            End If

            Imaging.DrawFrameBufferToPinnedImage(_PinnedBmpArray, _FrameBuffer, _SelectedPallet, _PalletThreshhold, _ShowCentroids)
            bmp = New Bitmap(_PinnedBmp, _PinnedBmp.Width, pnlData.Height - 1)

            DrawBoundingBox(bmp)

            pbData.Image = bmp

        Else

            Dim bmp As Bitmap

            If (_SelectedTimeSeriesIndexStart + _SelectedTimeSeriesIndexEnd >= 0) Then
                bmp = Imaging.GetSignalImage(_SelectedTimeSeries.Samples, _SelectedTimeSeries.Samples.Length, pnlData.Height - 1)
            Else
                bmp = Imaging.GetSignalImage(_FrameBuffer.ToList, _FrameBuffer.Count * _FrameBuffer(0).FrameCount, pnlData.Height)
            End If

            DrawBoundingBox(bmp)

            pbData.Image = bmp

        End If

    End Sub

    Private Sub DrawBoundingBox(bmp As Image)
        If (_SelectedTimeSeriesIndexStart + _SelectedTimeSeriesIndexEnd >= 0) Then

            Dim left As Double = 0
            Dim len As Integer = 0 ' Need to get true length of buffer as not all frames may be the same length if processing params changed...
            Dim width As Double = 0

            For i As Integer = 0 To _FrameBuffer.Count - 1
                len += _FrameBuffer(i).Frames.Length
                If i >= _SelectedTimeSeriesIndexStart And i <= _SelectedTimeSeriesIndexEnd Then
                    width += _FrameBuffer(i).Frames.Length
                    left = len - width '- 1
                End If
            Next

            width = (width * bmp.Width) / len
            left = (left * bmp.Width) / len

            Imaging.DrawBoundingBox(bmp, New System.Drawing.Point(CInt(left) + 1, 1), Pens.Red, CInt(width) - 1, bmp.Height - 2)

        End If
    End Sub

    Public Sub PlotLabeledQueryResults(results As Dictionary(Of TimeSeriesQuery, Color))
        If results.Count > 0 Then
            For i As Integer = 0 To results.Count - 1
                Dim query As TimeSeriesQuery = results.Keys(i)
                Imaging.PlotQueryResults(pbData.Image, query.Results, New Pen(results.Values(i), 1))
            Next
            pbData.Refresh()
        End If
    End Sub

    Private Sub SetSelectedMenuItem(menu As ToolStripMenuItem, item As ToolStripMenuItem)
        For Each menuItem As ToolStripMenuItem In menu.DropDownItems
            menuItem.Checked = False
        Next
        item.Checked = True
    End Sub

    Private Sub SetMaxSampleFrames(num As Integer)
        _BufferMaxFrames = num
        _FrameBuffer.MaxSize = _BufferMaxFrames
        RaiseEvent SampleMaxFramesChanged(Me, EventArgs.Empty)
    End Sub

#Region " -- Properties -- "

    Public ReadOnly Property SelectedTimeSeriesIndexStart As Integer
        Get
            Return _SelectedTimeSeriesIndexStart
        End Get
    End Property

    Public ReadOnly Property SelectedTimeSeriesIndexEnd As Integer
        Get
            Return _SelectedTimeSeriesIndexEnd
        End Get
    End Property

    Public ReadOnly Property SelectedTimeSeries As TimeSeries
        Get
            Return _SelectedTimeSeries
        End Get
    End Property

    Public ReadOnly Property SelectedPallet As IColorPallet
        Get
            Return _SelectedPallet
        End Get
    End Property

    Public Property PalletThreshold As Integer
        Get
            Return vsPalletThresh.Value
        End Get
        Set(value As Integer)
            If value <> vsPalletThresh.Value Then
                vsPalletThresh.Value = value
                _PalletThreshhold = value
            End If
        End Set
    End Property

    Public Property ShowCentroids As Boolean
        Get
            Return _ShowCentroids
        End Get
        Set(value As Boolean)
            If value <> _ShowCentroids Then
                _ShowCentroids = value
            End If
        End Set
    End Property

    Public ReadOnly Property SampleMaxFrames As Integer
        Get
            Return _BufferMaxFrames
        End Get
    End Property

    Public ReadOnly Property FrameBuffer As TimeSeriesBuffer
        Get
            Return _FrameBuffer
        End Get
    End Property

    Public Property AllowLoadSave As Boolean
        Get
            Return _AllowLoadSave
        End Get
        Set(value As Boolean)
            _AllowLoadSave = value
        End Set
    End Property

#End Region

#Region " -- Playback -- "

    'Private _Playing As Boolean
    'Private _Player As WaveOutPlayer
    'Private _PlayFrameIndex As Integer = -1

    'Private Sub Play()
    '    If Not _Playing Then
    '        Try
    '            _PlayFrameIndex = -1
    '            Dim buffSize As Integer = _FrameBuffer(0).Samples.Length
    '            Dim format As New WaveFormat(App.SampleRate, App.BitsPerSample, 1)
    '            _Player = New WaveOutPlayer(0, format, buffSize, 8, AddressOf OnFillPlayBuffer)
    '            PlayStopSelectionMenuItem.Text = "Stop"
    '            _Playing = True
    '            PlayStopSelectionMenuItem.Image = My.Resources.Resources.control_stop_blue
    '        Catch ex As Exception
    '            Debug.Write(ex)
    '            [Stop]()

    '            Dim msg As String = ex.Message.Trim
    '            If IsNumeric(msg) Then
    '                msg = [Enum].Parse(GetType(MMSYSERR), msg).ToString
    '            End If
    '            Throw New Exception(String.Format("Error initializing the WaveOutPlayer: {0}", msg), ex)
    '        End Try
    '    End If
    'End Sub

    'Private Sub [Stop]()
    '    If _Playing Then
    '        PlayStopSelectionMenuItem.Text = "Play"
    '        Try
    '            If _Player IsNot Nothing Then
    '                _Playing = False
    '                _Player.Dispose()
    '                _Player = Nothing
    '            End If
    '        Catch ex As Exception
    '        End Try
    '        PlayStopSelectionMenuItem.Image = My.Resources.Resources.control_play_blue
    '    End If
    'End Sub

    'Private Sub OnFillPlayBuffer(data As IntPtr, size As Integer)
    '    _PlayFrameIndex += 1
    '    Dim bytes As Byte() = _FrameBuffer(_PlayFrameIndex).ToByteArray
    '    If _PlayFrameIndex = _FrameBuffer.Count - 1 Then
    '        _PlayFrameIndex = -1
    '    End If
    '    System.Runtime.InteropServices.Marshal.Copy(bytes, 0, data, size)
    'End Sub

#End Region






 
End Class
