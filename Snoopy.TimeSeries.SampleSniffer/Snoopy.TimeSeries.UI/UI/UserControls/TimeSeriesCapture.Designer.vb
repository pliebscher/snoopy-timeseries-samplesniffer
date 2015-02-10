<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TimeSeriesCapture
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TimeSeriesCapture))
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.btnCaptureSettings = New System.Windows.Forms.Button()
        Me.cmsDataSpectrogram = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SampleRateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleRate5k = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleRate8k = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleRate11k = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleRate12k = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleRate16k = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleRate22k = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleRate44k = New System.Windows.Forms.ToolStripMenuItem()
        Me.SampleSizeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleSize1024 = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleSize2048 = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleSize4096 = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSampleSize8192 = New System.Windows.Forms.ToolStripMenuItem()
        Me.btnMonitorStart = New System.Windows.Forms.Button()
        Me.btnMonitorStop = New System.Windows.Forms.Button()
        Me.ToolStripComboBox2 = New System.Windows.Forms.ToolStripComboBox()
        Me.Panel2.SuspendLayout()
        Me.cmsDataSpectrogram.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.SystemColors.AppWorkspace
        Me.Panel2.Controls.Add(Me.btnCaptureSettings)
        Me.Panel2.Controls.Add(Me.btnMonitorStart)
        Me.Panel2.Controls.Add(Me.btnMonitorStop)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(31, 87)
        Me.Panel2.TabIndex = 88
        '
        'btnCaptureSettings
        '
        Me.btnCaptureSettings.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnCaptureSettings.BackColor = System.Drawing.SystemColors.Control
        Me.btnCaptureSettings.ContextMenuStrip = Me.cmsDataSpectrogram
        Me.btnCaptureSettings.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.time
        Me.btnCaptureSettings.ImageAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnCaptureSettings.Location = New System.Drawing.Point(3, 61)
        Me.btnCaptureSettings.Name = "btnCaptureSettings"
        Me.btnCaptureSettings.Size = New System.Drawing.Size(25, 23)
        Me.btnCaptureSettings.TabIndex = 88
        Me.btnCaptureSettings.UseVisualStyleBackColor = False
        '
        'cmsDataSpectrogram
        '
        Me.cmsDataSpectrogram.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SampleRateToolStripMenuItem, Me.SampleSizeToolStripMenuItem})
        Me.cmsDataSpectrogram.Name = "cmsDataSpectrogram"
        Me.cmsDataSpectrogram.Size = New System.Drawing.Size(135, 48)
        Me.cmsDataSpectrogram.Text = "Actions"
        '
        'SampleRateToolStripMenuItem
        '
        Me.SampleRateToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmSampleRate5k, Me.tsmSampleRate8k, Me.tsmSampleRate11k, Me.tsmSampleRate12k, Me.tsmSampleRate16k, Me.tsmSampleRate22k, Me.tsmSampleRate44k})
        Me.SampleRateToolStripMenuItem.Name = "SampleRateToolStripMenuItem"
        Me.SampleRateToolStripMenuItem.Size = New System.Drawing.Size(134, 22)
        Me.SampleRateToolStripMenuItem.Text = "Sample Rate"
        '
        'tsmSampleRate5k
        '
        Me.tsmSampleRate5k.Name = "tsmSampleRate5k"
        Me.tsmSampleRate5k.Size = New System.Drawing.Size(152, 22)
        Me.tsmSampleRate5k.Text = "5512"
        '
        'tsmSampleRate8k
        '
        Me.tsmSampleRate8k.Name = "tsmSampleRate8k"
        Me.tsmSampleRate8k.Size = New System.Drawing.Size(152, 22)
        Me.tsmSampleRate8k.Text = "8000"
        '
        'tsmSampleRate11k
        '
        Me.tsmSampleRate11k.Name = "tsmSampleRate11k"
        Me.tsmSampleRate11k.Size = New System.Drawing.Size(152, 22)
        Me.tsmSampleRate11k.Text = "11025"
        '
        'tsmSampleRate12k
        '
        Me.tsmSampleRate12k.Name = "tsmSampleRate12k"
        Me.tsmSampleRate12k.Size = New System.Drawing.Size(152, 22)
        Me.tsmSampleRate12k.Text = "12500"
        '
        'tsmSampleRate16k
        '
        Me.tsmSampleRate16k.Name = "tsmSampleRate16k"
        Me.tsmSampleRate16k.Size = New System.Drawing.Size(152, 22)
        Me.tsmSampleRate16k.Text = "16000"
        '
        'tsmSampleRate22k
        '
        Me.tsmSampleRate22k.Checked = True
        Me.tsmSampleRate22k.CheckState = System.Windows.Forms.CheckState.Checked
        Me.tsmSampleRate22k.Name = "tsmSampleRate22k"
        Me.tsmSampleRate22k.Size = New System.Drawing.Size(152, 22)
        Me.tsmSampleRate22k.Text = "22050"
        '
        'tsmSampleRate44k
        '
        Me.tsmSampleRate44k.Name = "tsmSampleRate44k"
        Me.tsmSampleRate44k.Size = New System.Drawing.Size(152, 22)
        Me.tsmSampleRate44k.Text = "44100"
        '
        'SampleSizeToolStripMenuItem
        '
        Me.SampleSizeToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmSampleSize1024, Me.tsmSampleSize2048, Me.tsmSampleSize4096, Me.tsmSampleSize8192})
        Me.SampleSizeToolStripMenuItem.Name = "SampleSizeToolStripMenuItem"
        Me.SampleSizeToolStripMenuItem.Size = New System.Drawing.Size(134, 22)
        Me.SampleSizeToolStripMenuItem.Text = "Sample Size"
        '
        'tsmSampleSize1024
        '
        Me.tsmSampleSize1024.Name = "tsmSampleSize1024"
        Me.tsmSampleSize1024.Size = New System.Drawing.Size(98, 22)
        Me.tsmSampleSize1024.Text = "1024"
        '
        'tsmSampleSize2048
        '
        Me.tsmSampleSize2048.Checked = True
        Me.tsmSampleSize2048.CheckState = System.Windows.Forms.CheckState.Checked
        Me.tsmSampleSize2048.Name = "tsmSampleSize2048"
        Me.tsmSampleSize2048.Size = New System.Drawing.Size(98, 22)
        Me.tsmSampleSize2048.Text = "2048"
        '
        'tsmSampleSize4096
        '
        Me.tsmSampleSize4096.Name = "tsmSampleSize4096"
        Me.tsmSampleSize4096.Size = New System.Drawing.Size(98, 22)
        Me.tsmSampleSize4096.Text = "4096"
        '
        'tsmSampleSize8192
        '
        Me.tsmSampleSize8192.Name = "tsmSampleSize8192"
        Me.tsmSampleSize8192.Size = New System.Drawing.Size(98, 22)
        Me.tsmSampleSize8192.Text = "8192"
        '
        'btnMonitorStart
        '
        Me.btnMonitorStart.BackColor = System.Drawing.SystemColors.Control
        Me.btnMonitorStart.Image = CType(resources.GetObject("btnMonitorStart.Image"), System.Drawing.Image)
        Me.btnMonitorStart.ImageAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnMonitorStart.Location = New System.Drawing.Point(3, 3)
        Me.btnMonitorStart.Name = "btnMonitorStart"
        Me.btnMonitorStart.Size = New System.Drawing.Size(25, 23)
        Me.btnMonitorStart.TabIndex = 53
        Me.btnMonitorStart.UseVisualStyleBackColor = False
        '
        'btnMonitorStop
        '
        Me.btnMonitorStop.BackColor = System.Drawing.SystemColors.Control
        Me.btnMonitorStop.Enabled = False
        Me.btnMonitorStop.Image = CType(resources.GetObject("btnMonitorStop.Image"), System.Drawing.Image)
        Me.btnMonitorStop.ImageAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnMonitorStop.Location = New System.Drawing.Point(3, 32)
        Me.btnMonitorStop.Name = "btnMonitorStop"
        Me.btnMonitorStop.Size = New System.Drawing.Size(25, 23)
        Me.btnMonitorStop.TabIndex = 54
        Me.btnMonitorStop.UseVisualStyleBackColor = False
        '
        'ToolStripComboBox2
        '
        Me.ToolStripComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ToolStripComboBox2.Items.AddRange(New Object() {"8000", "11025", "22050", "44100"})
        Me.ToolStripComboBox2.Margin = New System.Windows.Forms.Padding(2)
        Me.ToolStripComboBox2.Name = "ToolStripComboBox2"
        Me.ToolStripComboBox2.Size = New System.Drawing.Size(121, 21)
        Me.ToolStripComboBox2.ToolTipText = "Sample Rate"
        '
        'TimeSeriesCapture
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.Panel2)
        Me.Name = "TimeSeriesCapture"
        Me.Size = New System.Drawing.Size(31, 87)
        Me.Panel2.ResumeLayout(False)
        Me.cmsDataSpectrogram.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents btnMonitorStart As System.Windows.Forms.Button
    Friend WithEvents btnMonitorStop As System.Windows.Forms.Button
    Friend WithEvents cmsDataSpectrogram As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents SampleRateToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleRate5k As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleRate8k As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleRate11k As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleRate22k As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleRate44k As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SampleSizeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripComboBox2 As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents tsmSampleSize1024 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleSize2048 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleSize4096 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleSize8192 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleRate12k As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSampleRate16k As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnCaptureSettings As System.Windows.Forms.Button

End Class
