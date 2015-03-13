<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TimeSeriesExplorer
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.vsPalletThresh = New System.Windows.Forms.VScrollBar()
        Me.pbColorPallet = New System.Windows.Forms.PictureBox()
        Me.cmsDataSpectrogram = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.PlayStopSelectionMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearSelectionMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CropSelectionMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveSelectionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenInViewerMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.SampleBuffersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleBuffers16 = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleBuffers32 = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleBuffers64 = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleBuffers128 = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleBuffers256 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ColorPalletToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ShowCentroidsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowSpectrogramToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ofdTimeSeries = New System.Windows.Forms.OpenFileDialog()
        Me.sfdTimeSeries = New System.Windows.Forms.SaveFileDialog()
        Me.pbData = New System.Windows.Forms.PictureBox()
        Me.pnlData = New System.Windows.Forms.Panel()
        Me.hsbData = New System.Windows.Forms.HScrollBar()
        Me.Panel2.SuspendLayout()
        CType(Me.pbColorPallet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.cmsDataSpectrogram.SuspendLayout()
        CType(Me.pbData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlData.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.SystemColors.AppWorkspace
        Me.Panel2.Controls.Add(Me.vsPalletThresh)
        Me.Panel2.Controls.Add(Me.pbColorPallet)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(30, 148)
        Me.Panel2.TabIndex = 88
        '
        'vsPalletThresh
        '
        Me.vsPalletThresh.Dock = System.Windows.Forms.DockStyle.Left
        Me.vsPalletThresh.LargeChange = 5
        Me.vsPalletThresh.Location = New System.Drawing.Point(0, 0)
        Me.vsPalletThresh.Maximum = 255
        Me.vsPalletThresh.Minimum = 1
        Me.vsPalletThresh.Name = "vsPalletThresh"
        Me.vsPalletThresh.Size = New System.Drawing.Size(16, 148)
        Me.vsPalletThresh.TabIndex = 86
        Me.vsPalletThresh.Value = 75
        '
        'pbColorPallet
        '
        Me.pbColorPallet.BackColor = System.Drawing.Color.DimGray
        Me.pbColorPallet.Dock = System.Windows.Forms.DockStyle.Right
        Me.pbColorPallet.Location = New System.Drawing.Point(15, 0)
        Me.pbColorPallet.Name = "pbColorPallet"
        Me.pbColorPallet.Size = New System.Drawing.Size(15, 148)
        Me.pbColorPallet.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbColorPallet.TabIndex = 87
        Me.pbColorPallet.TabStop = False
        '
        'cmsDataSpectrogram
        '
        Me.cmsDataSpectrogram.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PlayStopSelectionMenuItem, Me.ClearSelectionMenuItem, Me.CropSelectionMenuItem, Me.SaveSelectionToolStripMenuItem, Me.OpenInViewerMenuItem, Me.ToolStripSeparator1, Me.SampleBuffersToolStripMenuItem, Me.ColorPalletToolStripMenuItem, Me.ToolStripSeparator3, Me.ShowCentroidsToolStripMenuItem, Me.ShowSpectrogramToolStripMenuItem, Me.ToolStripSeparator2, Me.SaveToolStripMenuItem, Me.LoadToolStripMenuItem})
        Me.cmsDataSpectrogram.Name = "cmsDataSpectrogram"
        Me.cmsDataSpectrogram.Size = New System.Drawing.Size(170, 286)
        Me.cmsDataSpectrogram.Text = "Actions"
        '
        'PlayStopSelectionMenuItem
        '
        Me.PlayStopSelectionMenuItem.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.control_play_blue
        Me.PlayStopSelectionMenuItem.Name = "PlayStopSelectionMenuItem"
        Me.PlayStopSelectionMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.PlayStopSelectionMenuItem.Text = "Play"
        '
        'ClearSelectionMenuItem
        '
        Me.ClearSelectionMenuItem.Enabled = False
        Me.ClearSelectionMenuItem.Name = "ClearSelectionMenuItem"
        Me.ClearSelectionMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.ClearSelectionMenuItem.Text = "Clear Selection"
        '
        'CropSelectionMenuItem
        '
        Me.CropSelectionMenuItem.Enabled = False
        Me.CropSelectionMenuItem.Name = "CropSelectionMenuItem"
        Me.CropSelectionMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.CropSelectionMenuItem.Text = "Crop Selection"
        '
        'SaveSelectionToolStripMenuItem
        '
        Me.SaveSelectionToolStripMenuItem.Enabled = False
        Me.SaveSelectionToolStripMenuItem.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.disk
        Me.SaveSelectionToolStripMenuItem.Name = "SaveSelectionToolStripMenuItem"
        Me.SaveSelectionToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.SaveSelectionToolStripMenuItem.Text = "Save Selection..."
        '
        'OpenInViewerMenuItem
        '
        Me.OpenInViewerMenuItem.Enabled = False
        Me.OpenInViewerMenuItem.Name = "OpenInViewerMenuItem"
        Me.OpenInViewerMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.OpenInViewerMenuItem.Text = "Open in new viewer"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(166, 6)
        '
        'SampleBuffersToolStripMenuItem
        '
        Me.SampleBuffersToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmSampleBuffers16, Me.tsmSampleBuffers32, Me.tsmSampleBuffers64, Me.tsmSampleBuffers128, Me.tsmSampleBuffers256})
        Me.SampleBuffersToolStripMenuItem.Name = "SampleBuffersToolStripMenuItem"
        Me.SampleBuffersToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.SampleBuffersToolStripMenuItem.Text = "Sample Buffers"
        '
        'tsmSampleBuffers16
        '
        Me.tsmSampleBuffers16.Name = "tsmSampleBuffers16"
        Me.tsmSampleBuffers16.Size = New System.Drawing.Size(92, 22)
        Me.tsmSampleBuffers16.Text = "16"
        '
        'tsmSampleBuffers32
        '
        Me.tsmSampleBuffers32.Checked = True
        Me.tsmSampleBuffers32.CheckState = System.Windows.Forms.CheckState.Checked
        Me.tsmSampleBuffers32.Name = "tsmSampleBuffers32"
        Me.tsmSampleBuffers32.Size = New System.Drawing.Size(92, 22)
        Me.tsmSampleBuffers32.Text = "32"
        '
        'tsmSampleBuffers64
        '
        Me.tsmSampleBuffers64.Name = "tsmSampleBuffers64"
        Me.tsmSampleBuffers64.Size = New System.Drawing.Size(92, 22)
        Me.tsmSampleBuffers64.Text = "64"
        '
        'tsmSampleBuffers128
        '
        Me.tsmSampleBuffers128.Name = "tsmSampleBuffers128"
        Me.tsmSampleBuffers128.Size = New System.Drawing.Size(92, 22)
        Me.tsmSampleBuffers128.Text = "128"
        '
        'tsmSampleBuffers256
        '
        Me.tsmSampleBuffers256.Name = "tsmSampleBuffers256"
        Me.tsmSampleBuffers256.Size = New System.Drawing.Size(92, 22)
        Me.tsmSampleBuffers256.Text = "256"
        '
        'ColorPalletToolStripMenuItem
        '
        Me.ColorPalletToolStripMenuItem.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.palette
        Me.ColorPalletToolStripMenuItem.Name = "ColorPalletToolStripMenuItem"
        Me.ColorPalletToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.ColorPalletToolStripMenuItem.Text = "Color Palette"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(166, 6)
        '
        'ShowCentroidsToolStripMenuItem
        '
        Me.ShowCentroidsToolStripMenuItem.CheckOnClick = True
        Me.ShowCentroidsToolStripMenuItem.Name = "ShowCentroidsToolStripMenuItem"
        Me.ShowCentroidsToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.ShowCentroidsToolStripMenuItem.Text = "Show Centroids"
        '
        'ShowSpectrogramToolStripMenuItem
        '
        Me.ShowSpectrogramToolStripMenuItem.Checked = True
        Me.ShowSpectrogramToolStripMenuItem.CheckOnClick = True
        Me.ShowSpectrogramToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ShowSpectrogramToolStripMenuItem.Name = "ShowSpectrogramToolStripMenuItem"
        Me.ShowSpectrogramToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.ShowSpectrogramToolStripMenuItem.Text = "Show Spectrogram"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(166, 6)
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Enabled = False
        Me.SaveToolStripMenuItem.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.disk
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.SaveToolStripMenuItem.Text = "Save Buffer As..."
        '
        'LoadToolStripMenuItem
        '
        Me.LoadToolStripMenuItem.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.folder
        Me.LoadToolStripMenuItem.Name = "LoadToolStripMenuItem"
        Me.LoadToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.LoadToolStripMenuItem.Text = "Load Buffer..."
        '
        'ofdTimeSeries
        '
        Me.ofdTimeSeries.DefaultExt = "tsb"
        Me.ofdTimeSeries.Filter = "TimeSeriesBuffer (*.tsb)|*.tsb|All files (*.*)|*.*"
        '
        'sfdTimeSeries
        '
        Me.sfdTimeSeries.DefaultExt = "tsb"
        Me.sfdTimeSeries.Filter = "TimeSeriesBuffer (*.tsb)|*.tsb|Wave (*.wav)|*.wav|Image (*.png)|*.png|All files (" & _
    "*.*)|*.*"
        '
        'pbData
        '
        Me.pbData.BackColor = System.Drawing.Color.Black
        Me.pbData.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.pbData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbData.ContextMenuStrip = Me.cmsDataSpectrogram
        Me.pbData.Cursor = System.Windows.Forms.Cursors.Hand
        Me.pbData.InitialImage = Nothing
        Me.pbData.Location = New System.Drawing.Point(0, 0)
        Me.pbData.Name = "pbData"
        Me.pbData.Size = New System.Drawing.Size(759, 129)
        Me.pbData.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.pbData.TabIndex = 85
        Me.pbData.TabStop = False
        '
        'pnlData
        '
        Me.pnlData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlData.BackColor = System.Drawing.Color.Black
        Me.pnlData.Controls.Add(Me.pbData)
        Me.pnlData.Location = New System.Drawing.Point(30, 0)
        Me.pnlData.Name = "pnlData"
        Me.pnlData.Size = New System.Drawing.Size(775, 129)
        Me.pnlData.TabIndex = 89
        '
        'hsbData
        '
        Me.hsbData.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.hsbData.LargeChange = 5
        Me.hsbData.Location = New System.Drawing.Point(30, 132)
        Me.hsbData.Name = "hsbData"
        Me.hsbData.Size = New System.Drawing.Size(775, 16)
        Me.hsbData.TabIndex = 90
        '
        'TimeSeriesExplorer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.hsbData)
        Me.Controls.Add(Me.pnlData)
        Me.Controls.Add(Me.Panel2)
        Me.Name = "TimeSeriesExplorer"
        Me.Size = New System.Drawing.Size(805, 148)
        Me.Panel2.ResumeLayout(False)
        CType(Me.pbColorPallet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.cmsDataSpectrogram.ResumeLayout(False)
        CType(Me.pbData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlData.ResumeLayout(False)
        Me.pnlData.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents pbColorPallet As System.Windows.Forms.PictureBox
    Friend WithEvents vsPalletThresh As System.Windows.Forms.VScrollBar
    Friend WithEvents cmsDataSpectrogram As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents OpenInViewerMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents pbData As System.Windows.Forms.PictureBox
    'Friend WithEvents ToolStripComboBox2 As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents ColorPalletToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShowCentroidsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SampleBuffersToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleBuffers16 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleBuffers32 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleBuffers64 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleBuffers128 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleBuffers256 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LoadToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ofdTimeSeries As System.Windows.Forms.OpenFileDialog
    Friend WithEvents sfdTimeSeries As System.Windows.Forms.SaveFileDialog
    Friend WithEvents ClearSelectionMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CropSelectionMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ShowSpectrogramToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PlayStopSelectionMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SaveSelectionToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents pnlData As System.Windows.Forms.Panel
    Friend WithEvents hsbData As System.Windows.Forms.HScrollBar

End Class
