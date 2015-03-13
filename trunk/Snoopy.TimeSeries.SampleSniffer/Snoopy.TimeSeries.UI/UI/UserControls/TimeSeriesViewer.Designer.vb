<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TimeSeriesViewer
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
        Me.pbSignal = New System.Windows.Forms.PictureBox()
        Me.pgTimeSeries = New System.Windows.Forms.PropertyGrid()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.lblFreq = New System.Windows.Forms.Label()
        Me.pbTimeSeries = New System.Windows.Forms.PictureBox()
        Me.cmsSpectrogram = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.AutoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FillToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ZoomToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ShowCentroidsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenInViewerMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ofdTimeSeries = New System.Windows.Forms.OpenFileDialog()
        Me.sfdTimeSeries = New System.Windows.Forms.SaveFileDialog()
        CType(Me.pbSignal, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.pbTimeSeries, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.cmsSpectrogram.SuspendLayout()
        Me.SuspendLayout()
        '
        'pbSignal
        '
        Me.pbSignal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbSignal.BackColor = System.Drawing.SystemColors.ControlDark
        Me.pbSignal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbSignal.Location = New System.Drawing.Point(1, 172)
        Me.pbSignal.Name = "pbSignal"
        Me.pbSignal.Size = New System.Drawing.Size(314, 59)
        Me.pbSignal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbSignal.TabIndex = 81
        Me.pbSignal.TabStop = False
        '
        'pgTimeSeries
        '
        Me.pgTimeSeries.CausesValidation = False
        Me.pgTimeSeries.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pgTimeSeries.HelpVisible = False
        Me.pgTimeSeries.Location = New System.Drawing.Point(160, 3)
        Me.pgTimeSeries.Margin = New System.Windows.Forms.Padding(0, 3, 3, 3)
        Me.pgTimeSeries.Name = "pgTimeSeries"
        Me.pgTimeSeries.PropertySort = System.Windows.Forms.PropertySort.Alphabetical
        Me.pgTimeSeries.Size = New System.Drawing.Size(157, 165)
        Me.pgTimeSeries.TabIndex = 90
        Me.pgTimeSeries.ToolbarVisible = False
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.pgTimeSeries, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Panel3, 0, 1)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(-2, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(320, 171)
        Me.TableLayoutPanel1.TabIndex = 95
        '
        'Panel3
        '
        Me.Panel3.AutoScroll = True
        Me.Panel3.BackColor = System.Drawing.SystemColors.ControlDark
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel3.Controls.Add(Me.lblFreq)
        Me.Panel3.Controls.Add(Me.pbTimeSeries)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(3, 3)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(154, 165)
        Me.Panel3.TabIndex = 93
        '
        'lblFreq
        '
        Me.lblFreq.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblFreq.AutoSize = True
        Me.lblFreq.BackColor = System.Drawing.Color.Transparent
        Me.lblFreq.Location = New System.Drawing.Point(1, 149)
        Me.lblFreq.Name = "lblFreq"
        Me.lblFreq.Size = New System.Drawing.Size(16, 13)
        Me.lblFreq.TabIndex = 1
        Me.lblFreq.Text = "..."
        '
        'pbTimeSeries
        '
        Me.pbTimeSeries.BackColor = System.Drawing.SystemColors.ControlDark
        Me.pbTimeSeries.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbTimeSeries.ContextMenuStrip = Me.cmsSpectrogram
        Me.pbTimeSeries.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbTimeSeries.Location = New System.Drawing.Point(0, 0)
        Me.pbTimeSeries.Name = "pbTimeSeries"
        Me.pbTimeSeries.Size = New System.Drawing.Size(152, 163)
        Me.pbTimeSeries.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pbTimeSeries.TabIndex = 0
        Me.pbTimeSeries.TabStop = False
        '
        'cmsSpectrogram
        '
        Me.cmsSpectrogram.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AutoToolStripMenuItem, Me.FillToolStripMenuItem, Me.ZoomToolStripMenuItem, Me.ToolStripSeparator2, Me.ToolStripSeparator4, Me.ShowCentroidsToolStripMenuItem, Me.OpenInViewerMenuItem, Me.ToolStripSeparator3, Me.SaveToolStripMenuItem, Me.LoadToolStripMenuItem})
        Me.cmsSpectrogram.Name = "cmsDataSpectrogram"
        Me.cmsSpectrogram.Size = New System.Drawing.Size(170, 198)
        Me.cmsSpectrogram.Text = "Actions"
        '
        'AutoToolStripMenuItem
        '
        Me.AutoToolStripMenuItem.CheckOnClick = True
        Me.AutoToolStripMenuItem.Name = "AutoToolStripMenuItem"
        Me.AutoToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.AutoToolStripMenuItem.Text = "Auto"
        '
        'FillToolStripMenuItem
        '
        Me.FillToolStripMenuItem.CheckOnClick = True
        Me.FillToolStripMenuItem.Name = "FillToolStripMenuItem"
        Me.FillToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.FillToolStripMenuItem.Text = "Fill"
        '
        'ZoomToolStripMenuItem
        '
        Me.ZoomToolStripMenuItem.Checked = True
        Me.ZoomToolStripMenuItem.CheckOnClick = True
        Me.ZoomToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ZoomToolStripMenuItem.Name = "ZoomToolStripMenuItem"
        Me.ZoomToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.ZoomToolStripMenuItem.Text = "Zoom"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(166, 6)
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(166, 6)
        '
        'ShowCentroidsToolStripMenuItem
        '
        Me.ShowCentroidsToolStripMenuItem.CheckOnClick = True
        Me.ShowCentroidsToolStripMenuItem.Name = "ShowCentroidsToolStripMenuItem"
        Me.ShowCentroidsToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.ShowCentroidsToolStripMenuItem.Text = "Show Centroids"
        '
        'OpenInViewerMenuItem
        '
        Me.OpenInViewerMenuItem.Enabled = False
        Me.OpenInViewerMenuItem.Name = "OpenInViewerMenuItem"
        Me.OpenInViewerMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.OpenInViewerMenuItem.Text = "Open in new viewer"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(166, 6)
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Enabled = False
        Me.SaveToolStripMenuItem.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.disk
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.SaveToolStripMenuItem.Text = "Save As..."
        '
        'LoadToolStripMenuItem
        '
        Me.LoadToolStripMenuItem.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.folder
        Me.LoadToolStripMenuItem.Name = "LoadToolStripMenuItem"
        Me.LoadToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.LoadToolStripMenuItem.Text = "Load..."
        '
        'ofdTimeSeries
        '
        Me.ofdTimeSeries.DefaultExt = "ts"
        Me.ofdTimeSeries.Filter = "TimeSeries (*.ts)|*.ts|Wave (*.wav)|*.wav|All files (*.*)|*.*"
        '
        'sfdTimeSeries
        '
        Me.sfdTimeSeries.DefaultExt = "ts"
        Me.sfdTimeSeries.Filter = "TimeSeries (*.ts)|*.ts|Wave (*.wav)|*.wav|Image (*.png)|*.png|All files (*.*)|*.*" & _
    ""
        '
        'TimeSeriesViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.pbSignal)
        Me.MinimumSize = New System.Drawing.Size(316, 232)
        Me.Name = "TimeSeriesViewer"
        Me.Size = New System.Drawing.Size(316, 232)
        CType(Me.pbSignal, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        CType(Me.pbTimeSeries, System.ComponentModel.ISupportInitialize).EndInit()
        Me.cmsSpectrogram.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pbSignal As System.Windows.Forms.PictureBox
    Friend WithEvents pgTimeSeries As System.Windows.Forms.PropertyGrid
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ofdTimeSeries As System.Windows.Forms.OpenFileDialog
    Friend WithEvents sfdTimeSeries As System.Windows.Forms.SaveFileDialog
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents pbTimeSeries As System.Windows.Forms.PictureBox
    Friend WithEvents cmsSpectrogram As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents OpenInViewerMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ShowCentroidsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LoadToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AutoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FillToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ZoomToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents lblFreq As System.Windows.Forms.Label

End Class
