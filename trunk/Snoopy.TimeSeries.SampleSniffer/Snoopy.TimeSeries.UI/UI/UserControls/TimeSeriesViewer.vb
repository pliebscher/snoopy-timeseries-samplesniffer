Public Class TimeSeriesViewer

    Private _TimeSeries As TimeSeries

    'Private _SelectedPallet As IColorPallet = PalletManager.Instance.SelectedPallet
    'Private _SelectedPalletThreshold As Integer = 25
    'Private _AutoPallet As IColorPallet = PalletManager.Instance.SelectedPallet
    'Private _AutoPalletThreshold As Integer = 25

    Public Event Open(sender As Object, e As TimeSeriesEventArgs)
    Public Event Save(sender As Object, e As TimeSeriesEventArgs)

    Public Sub Reset()
        pbTimeSeries.Image = Nothing
        pbSignal.Image = Nothing
        _TimeSeries = Nothing
        pgTimeSeries.SelectedObject = Nothing
    End Sub

    Public Sub UpdateDisplay()

        If _TimeSeries Is Nothing Then Exit Sub

        pgTimeSeries.SelectedObject = _TimeSeries

        UpdateTimeSeriesImage()
        UpdateSignalImage()

    End Sub

    Private Sub UpdateTimeSeriesImage()

        If _TimeSeries Is Nothing Then Exit Sub

        Dim series As New List(Of TimeSeries)
        series.Add(_TimeSeries)
        Dim Bmp As Bitmap = Imaging.GetFastLogSpectrogramImage(series, PalettetManager.Instance.SelectedPalette, PalettetManager.Instance.PaletteThreshold, ShowCentroidsToolStripMenuItem.Checked)

        'If pbTimeSeries.SizeMode = PictureBoxSizeMode.StretchImage Then
        '    Dim bmp2 As New Bitmap(Bmp.Width * 2, Bmp.Height * 2)
        '    Dim gfx As Graphics = Graphics.FromImage(bmp2)
        '    gfx.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
        '    'gfx.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
        '    gfx.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        '    gfx.DrawImage(Bmp, 0, 0, Bmp.Width * 2, Bmp.Height * 2)
        '    Bmp = bmp2
        '    pbTimeSeries.SizeMode = PictureBoxSizeMode.Normal
        'End If

        pbTimeSeries.Image = Bmp

        UpdateTimeSeriesPictureBox()

    End Sub

    Private Sub UpdateSignalImage()

        Dim Bmp As Bitmap = Imaging.GetSignalImage(_TimeSeries.Samples, pbSignal.Width, pbSignal.Height)
        pbSignal.Image = Bmp

    End Sub

    Private Sub UpdateTimeSeriesPictureBox()

        If _TimeSeries Is Nothing Then Exit Sub

        If ZoomToolStripMenuItem.Checked AndAlso Not pbTimeSeries.SizeMode = PictureBoxSizeMode.Zoom Then

            pbTimeSeries.SizeMode = PictureBoxSizeMode.Zoom
            Panel3.Dock = DockStyle.Fill
            pbTimeSeries.Dock = DockStyle.Fill
            pbTimeSeries.BorderStyle = BorderStyle.None

        ElseIf FillToolStripMenuItem.Checked AndAlso Not pbTimeSeries.SizeMode = PictureBoxSizeMode.StretchImage Then

            pbTimeSeries.SizeMode = PictureBoxSizeMode.StretchImage
            Panel3.Dock = DockStyle.Fill
            pbTimeSeries.Dock = DockStyle.Fill
            pbTimeSeries.BorderStyle = BorderStyle.FixedSingle

        ElseIf AutoToolStripMenuItem.Checked Then

            If Not pbTimeSeries.SizeMode = PictureBoxSizeMode.AutoSize Then
                pbTimeSeries.SizeMode = PictureBoxSizeMode.AutoSize
                pbTimeSeries.BorderStyle = BorderStyle.FixedSingle

                Dim P3Size As Size = Panel3.Size
                Panel3.Dock = DockStyle.None
                Panel3.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right Or AnchorStyles.Top Or AnchorStyles.Left
                pbTimeSeries.Dock = DockStyle.None
                Panel3.Size = P3Size
            End If

            Dim X As Integer = (Panel3.Width \ 2) - (pbTimeSeries.Width \ 2)
            Dim Y As Integer = (Panel3.Height \ 2) - (pbTimeSeries.Height \ 2)
            Dim P As Point

            ' Keep the PictureBox center aligned in the Panel...
            If _TimeSeries.Frames.Length > Panel3.Size.Width Then
                P = New Point(0, Y)
            ElseIf _TimeSeries.Frames(0).Length > Panel3.Size.Height Then
                P = New Point(X, 0)
            Else
                P = New Point(X, Y)
            End If

            pbTimeSeries.Location = P

        End If



    End Sub

#Region " -- Form -- "

    Private Sub TimeSeriesViewer_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Private Sub Panel3_Resize(sender As Object, e As EventArgs) Handles Panel3.Resize
        If Not Me.Created Then Exit Sub
        UpdateTimeSeriesPictureBox()
    End Sub

    Private Sub pbTimeSeries_MouseLeave(sender As Object, e As EventArgs) Handles pbTimeSeries.MouseLeave
        lblFreq.Text = String.Empty
    End Sub

    Private Sub pbTimeSeries_MouseMove(sender As Object, e As MouseEventArgs) Handles pbTimeSeries.MouseMove

        If _TimeSeries IsNot Nothing Then

            Dim ImageScale As Double = pbTimeSeries.Image.Width / pbTimeSeries.Image.Height
            Dim PbScale As Double = pbTimeSeries.Width / pbTimeSeries.Height
            Dim newX As Double = e.X
            Dim newY As Double = e.Y

            Select Case pbTimeSeries.SizeMode

                Case PictureBoxSizeMode.AutoSize

                Case PictureBoxSizeMode.Zoom

                    If ImageScale > PbScale Then ' image fills up the entire control from left to right

                        Dim ratioWidth As Double = pbTimeSeries.Image.Width / pbTimeSeries.Width
                        Dim scale As Double = pbTimeSeries.Width / pbTimeSeries.Image.Width
                        Dim displayHeight As Double = scale * pbTimeSeries.Image.Height
                        Dim diffHeight As Double = pbTimeSeries.Height - displayHeight

                        diffHeight /= 2
                        newX *= ratioWidth
                        newY -= diffHeight
                        newY /= scale
                        newY = Int(_TimeSeries.Frames(0).Length - newY)

                        If newY >= 0 AndAlso newY < _TimeSeries.FrameLength Then
                            lblFreq.Text = String.Format("{0},{1}: {2:G6}", CInt(newX), CInt(newY), _TimeSeries.Frames(CInt(newX))(CInt(newY)))
                        Else
                            lblFreq.Text = String.Empty
                        End If

                    Else

                        Dim ratioHeight As Double = pbTimeSeries.Image.Height / pbTimeSeries.Height
                        Dim scale As Double = pbTimeSeries.Height / pbTimeSeries.Image.Height
                        Dim displayWidth As Double = scale * pbTimeSeries.Image.Width
                        Dim diffWidth As Double = pbTimeSeries.Width - displayWidth

                        diffWidth /= 2
                        newX -= diffWidth
                        newX /= scale
                        newY *= ratioHeight

                        newX = Int(newX)
                        newY = Int(Math.Min(_TimeSeries.Frames(0).Length - newY, _TimeSeries.Frames(0).Length - 1))

                        If newX >= 0 AndAlso newX < _TimeSeries.FrameCount Then
                            lblFreq.Text = String.Format("{0},{1}: {2:G6}", CInt(newX), CInt(newY), _TimeSeries.Frames(CInt(newX))(CInt(newY)))
                        Else
                            lblFreq.Text = String.Empty
                        End If


                    End If

                Case PictureBoxSizeMode.StretchImage

                    Dim ratioWidth As Double = pbTimeSeries.Image.Width / pbTimeSeries.Width
                    Dim ratioHeight As Double = pbTimeSeries.Image.Height / pbTimeSeries.Height
                    newX *= ratioWidth
                    newY *= ratioHeight

                    newX = Int(newX)
                    newY = Int(Math.Min(_TimeSeries.Frames(0).Length - newY, _TimeSeries.Frames(0).Length - 1))

                    If newX >= 0 AndAlso newX < _TimeSeries.FrameCount Then
                        lblFreq.Text = String.Format("{0},{1}: {2:G6}", CInt(newX), CInt(newY), _TimeSeries.Frames(CInt(newX))(CInt(newY)))
                    Else
                        lblFreq.Text = String.Empty
                    End If

            End Select

        End If


    End Sub

#Region " -- Spectro Context Menu -- "

    Private Sub AutoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AutoToolStripMenuItem.Click
        ZoomToolStripMenuItem.Checked = False
        FillToolStripMenuItem.Checked = False
        UpdateTimeSeriesPictureBox()
    End Sub

    Private Sub FillToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FillToolStripMenuItem.Click
        AutoToolStripMenuItem.Checked = False
        ZoomToolStripMenuItem.Checked = False
        UpdateTimeSeriesPictureBox()
    End Sub

    Private Sub ZoomToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ZoomToolStripMenuItem.Click
        AutoToolStripMenuItem.Checked = False
        FillToolStripMenuItem.Checked = False
        UpdateTimeSeriesPictureBox()
    End Sub

    Private Sub ShowCentroidsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowCentroidsToolStripMenuItem.Click
        UpdateTimeSeriesImage()
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        If sfdTimeSeries.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            Dim path As String = sfdTimeSeries.FileName
            Try
                If path.EndsWith(".wav") Then
                    WaveFile.Create(path, _TimeSeries, _TimeSeries.SampleRate, 1)
                ElseIf path.EndsWith(".png") Then
                    pbTimeSeries.Image.Save(path, System.Drawing.Imaging.ImageFormat.Png)
                Else
                    _TimeSeries.Save(path)
                End If
                RaiseEvent Save(Me, New TimeSeriesEventArgs(_TimeSeries))
            Catch ex As Exception
                ShowException(ex)
            End Try
        Else
            Exit Sub
        End If
    End Sub

    Private Sub LoadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadToolStripMenuItem.Click
        Me.Reset()
        If ofdTimeSeries.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            Try
                Dim ts As TimeSeries = Nothing
                Dim path As String = ofdTimeSeries.FileName

                If path.EndsWith(".wav") Then

                    Dim wav As WaveFileHeader = WaveFile.Read(path)

                    ts = New TimeSeries(wav.Samples, wav.SamplesPerSecond)

                Else
                    ts = TimeSeries.Load(path)
                End If

                If ts.SampleRate <> My.Settings.SampleRate Then
                    MsgBox(String.Format("Application Sample Rate ({0}) differes from TimeSeries Sample Rate ({1})", My.Settings.SampleRate, ts.SampleRate), MsgBoxStyle.Exclamation, "Warning...")
                End If

                Me.Reset()
                Me._TimeSeries = ts
                'btnSave.Enabled = True
                'lblSource.Text = System.IO.Path.GetFileName(path)
                RaiseEvent Open(Me, New TimeSeriesEventArgs(ts))
            Catch ex As Exception
                ShowException(ex)
            End Try
        End If
    End Sub

    Private Sub OpenInViewerMenuItem_Click(sender As Object, e As EventArgs) Handles OpenInViewerMenuItem.Click
        Dim viewer As New FormTimeSeriesViewer
        With viewer
            .ViewControl.SelectedTimeSeries = _TimeSeries.Clone
            .ViewControl.ShowCentroids = ShowCentroidsToolStripMenuItem.Checked
            .Show()
        End With
    End Sub

    Private Sub cmsSpectrogram_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles cmsSpectrogram.Opening

        SaveToolStripMenuItem.Enabled = _TimeSeries IsNot Nothing
        OpenInViewerMenuItem.Enabled = _TimeSeries IsNot Nothing

    End Sub

#End Region

#End Region

#Region " -- PalletManager -- "

    Private WithEvents _PalletManager As PalettetManager = PalettetManager.Instance

    Private Sub PalletManager_SelectedPalletChanged(sender As Object, e As EventArgs) Handles _PalletManager.SelectedPalletChanged
        UpdateTimeSeriesImage()
    End Sub

    Private Sub PalletManager_PalletThresholdChanged(sender As Object, e As EventArgs) Handles _PalletManager.PalletThresholdChanged
        UpdateTimeSeriesImage()
    End Sub

#End Region

    Private Sub SetSelectedMenuItem(menu As ToolStripMenuItem, item As ToolStripMenuItem)
        For Each menuItem As ToolStripItem In menu.DropDownItems
            If TypeOf (menuItem) Is ToolStripMenuItem Then
                DirectCast(menuItem, ToolStripMenuItem).Checked = False
            End If
        Next
        item.Checked = True
    End Sub

    Public Property SelectedTimeSeries As TimeSeries
        Get
            Return _TimeSeries
        End Get
        Set(value As TimeSeries)
            If value Is Nothing Then
                Reset()
                Exit Property
            End If
            _TimeSeries = value
            If _TimeSeries.Frames.Length > pbTimeSeries.Width Then
                'AutoToolStripMenuItem.Checked = True
                'ZoomToolStripMenuItem.Checked = False
                'FillToolStripMenuItem.Checked = False
            End If
            Me.UpdateDisplay()
        End Set
    End Property

    Public Property ShowCentroids As Boolean
        Get
            Return ShowCentroidsToolStripMenuItem.Checked
        End Get
        Set(value As Boolean)
            If value <> ShowCentroidsToolStripMenuItem.Checked Then
                ShowCentroidsToolStripMenuItem.Checked = value
                Me.UpdateTimeSeriesImage()
            End If
        End Set
    End Property

    Private _ExViewer As FormExceptionViewer
    Private Sub ShowException(ex As Exception)
        If _ExViewer Is Nothing Then _ExViewer = New FormExceptionViewer
        _ExViewer.Show(ex)
    End Sub

End Class
