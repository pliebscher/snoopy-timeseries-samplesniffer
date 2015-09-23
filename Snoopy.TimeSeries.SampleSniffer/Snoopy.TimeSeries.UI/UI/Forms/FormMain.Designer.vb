<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormMain))
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.tsSampleRate = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tsStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tsDataSamples = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tsSelectedSeries = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tsFilename = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tsFPS = New System.Windows.Forms.ToolStripStatusLabel()
        Me.OptionsToolStripDropDown = New System.Windows.Forms.ToolStripDropDownButton()
        Me.ShowFPSToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage7 = New System.Windows.Forms.TabPage()
        Me.Preprocessor = New Snoopy.TimeSeries.UI.Preprocessor()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.TransformChooser = New Snoopy.TimeSeries.UI.TransformChooser()
        Me.TabPage8 = New System.Windows.Forms.TabPage()
        Me.Postprocessor = New Snoopy.TimeSeries.UI.Postprocessor()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.QueryBuilder = New Snoopy.TimeSeries.UI.QueryManager()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.TimeSeriesViewer = New Snoopy.TimeSeries.UI.TimeSeriesViewer()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TimeSeriesExplorer = New Snoopy.TimeSeries.UI.TimeSeriesExplorer()
        Me.TimeSeriesCapture = New Snoopy.TimeSeries.UI.TimeSeriesCapture()
        Me.StatusStrip.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage7.SuspendLayout()
        Me.TabPage6.SuspendLayout()
        Me.TabPage8.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        Me.StatusStrip.BackColor = System.Drawing.SystemColors.Control
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsSampleRate, Me.tsStatusLabel, Me.tsDataSamples, Me.tsSelectedSeries, Me.tsFilename, Me.tsFPS, Me.OptionsToolStripDropDown})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 501)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(842, 22)
        Me.StatusStrip.TabIndex = 61
        Me.StatusStrip.Text = "StatusStrip1"
        '
        'tsSampleRate
        '
        Me.tsSampleRate.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.tsSampleRate.BorderSides = CType((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.tsSampleRate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsSampleRate.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.tsSampleRate.Name = "tsSampleRate"
        Me.tsSampleRate.Size = New System.Drawing.Size(68, 17)
        Me.tsSampleRate.Text = "SampleRate"
        '
        'tsStatusLabel
        '
        Me.tsStatusLabel.BorderSides = CType((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.tsStatusLabel.Margin = New System.Windows.Forms.Padding(2, 3, 0, 2)
        Me.tsStatusLabel.Name = "tsStatusLabel"
        Me.tsStatusLabel.Size = New System.Drawing.Size(23, 17)
        Me.tsStatusLabel.Tag = ""
        Me.tsStatusLabel.Text = "..."
        '
        'tsDataSamples
        '
        Me.tsDataSamples.BorderSides = CType((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.tsDataSamples.Margin = New System.Windows.Forms.Padding(2, 3, 0, 2)
        Me.tsDataSamples.Name = "tsDataSamples"
        Me.tsDataSamples.Size = New System.Drawing.Size(23, 17)
        Me.tsDataSamples.Text = "..."
        '
        'tsSelectedSeries
        '
        Me.tsSelectedSeries.BorderSides = CType((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.tsSelectedSeries.Margin = New System.Windows.Forms.Padding(2, 3, 0, 2)
        Me.tsSelectedSeries.Name = "tsSelectedSeries"
        Me.tsSelectedSeries.Size = New System.Drawing.Size(57, 17)
        Me.tsSelectedSeries.Text = "No Frame"
        '
        'tsFilename
        '
        Me.tsFilename.BackColor = System.Drawing.SystemColors.Control
        Me.tsFilename.BorderSides = CType((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.tsFilename.Margin = New System.Windows.Forms.Padding(2, 3, 0, 2)
        Me.tsFilename.Name = "tsFilename"
        Me.tsFilename.Size = New System.Drawing.Size(43, 17)
        Me.tsFilename.Text = "No File"
        Me.tsFilename.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal
        '
        'tsFPS
        '
        Me.tsFPS.BackColor = System.Drawing.SystemColors.Control
        Me.tsFPS.BorderSides = CType((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.tsFPS.Margin = New System.Windows.Forms.Padding(2, 3, 0, 2)
        Me.tsFPS.Name = "tsFPS"
        Me.tsFPS.Size = New System.Drawing.Size(42, 17)
        Me.tsFPS.Text = "FPS: 0"
        Me.tsFPS.Visible = False
        '
        'OptionsToolStripDropDown
        '
        Me.OptionsToolStripDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.OptionsToolStripDropDown.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowFPSToolStripMenuItem})
        Me.OptionsToolStripDropDown.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.wrench
        Me.OptionsToolStripDropDown.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.OptionsToolStripDropDown.Margin = New System.Windows.Forms.Padding(10, 2, 0, 0)
        Me.OptionsToolStripDropDown.Name = "OptionsToolStripDropDown"
        Me.OptionsToolStripDropDown.Size = New System.Drawing.Size(29, 20)
        Me.OptionsToolStripDropDown.Text = "Show FPS"
        '
        'ShowFPSToolStripMenuItem
        '
        Me.ShowFPSToolStripMenuItem.CheckOnClick = True
        Me.ShowFPSToolStripMenuItem.Name = "ShowFPSToolStripMenuItem"
        Me.ShowFPSToolStripMenuItem.Size = New System.Drawing.Size(121, 22)
        Me.ShowFPSToolStripMenuItem.Text = "Show FPS"
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.TabPage7)
        Me.TabControl1.Controls.Add(Me.TabPage6)
        Me.TabControl1.Controls.Add(Me.TabPage8)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Location = New System.Drawing.Point(7, 27)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(360, 309)
        Me.TabControl1.TabIndex = 65
        '
        'TabPage7
        '
        Me.TabPage7.Controls.Add(Me.Preprocessor)
        Me.TabPage7.Location = New System.Drawing.Point(4, 22)
        Me.TabPage7.Name = "TabPage7"
        Me.TabPage7.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage7.Size = New System.Drawing.Size(352, 283)
        Me.TabPage7.TabIndex = 6
        Me.TabPage7.Text = "Pre-Process ->"
        Me.TabPage7.UseVisualStyleBackColor = True
        '
        'Preprocessor
        '
        Me.Preprocessor.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Preprocessor.Location = New System.Drawing.Point(0, 0)
        Me.Preprocessor.Name = "Preprocessor"
        Me.Preprocessor.SampleRate = 1
        Me.Preprocessor.Size = New System.Drawing.Size(352, 283)
        Me.Preprocessor.TabIndex = 1
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.TransformChooser)
        Me.TabPage6.Location = New System.Drawing.Point(4, 22)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Size = New System.Drawing.Size(352, 283)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "Transform ->"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'TransformChooser
        '
        Me.TransformChooser.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TransformChooser.Location = New System.Drawing.Point(0, 0)
        Me.TransformChooser.Name = "TransformChooser"
        Me.TransformChooser.SampleRate = 1
        Me.TransformChooser.Size = New System.Drawing.Size(352, 283)
        Me.TransformChooser.TabIndex = 70
        '
        'TabPage8
        '
        Me.TabPage8.Controls.Add(Me.Postprocessor)
        Me.TabPage8.Location = New System.Drawing.Point(4, 22)
        Me.TabPage8.Name = "TabPage8"
        Me.TabPage8.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage8.Size = New System.Drawing.Size(352, 283)
        Me.TabPage8.TabIndex = 7
        Me.TabPage8.Text = "Post-Process ->"
        Me.TabPage8.UseVisualStyleBackColor = True
        '
        'Postprocessor
        '
        Me.Postprocessor.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Postprocessor.Location = New System.Drawing.Point(0, 0)
        Me.Postprocessor.Name = "Postprocessor"
        Me.Postprocessor.SampleRate = 1
        Me.Postprocessor.Size = New System.Drawing.Size(352, 304)
        Me.Postprocessor.TabIndex = 73
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.QueryBuilder)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Size = New System.Drawing.Size(352, 283)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "Query!"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'QueryBuilder
        '
        Me.QueryBuilder.Dock = System.Windows.Forms.DockStyle.Fill
        Me.QueryBuilder.Location = New System.Drawing.Point(0, 0)
        Me.QueryBuilder.MaxQueryResults = 0
        Me.QueryBuilder.Name = "QueryBuilder"
        Me.QueryBuilder.SelectedTimeSeries = Nothing
        Me.QueryBuilder.Size = New System.Drawing.Size(352, 283)
        Me.QueryBuilder.TabIndex = 2
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.TimeSeriesViewer)
        Me.Panel1.Location = New System.Drawing.Point(373, 27)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(466, 308)
        Me.Panel1.TabIndex = 29
        '
        'TimeSeriesViewer
        '
        Me.TimeSeriesViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TimeSeriesViewer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TimeSeriesViewer.ForeColor = System.Drawing.SystemColors.ControlText
        Me.TimeSeriesViewer.Location = New System.Drawing.Point(0, 0)
        Me.TimeSeriesViewer.MinimumSize = New System.Drawing.Size(389, 281)
        Me.TimeSeriesViewer.Name = "TimeSeriesViewer"
        Me.TimeSeriesViewer.SelectedTimeSeries = Nothing
        Me.TimeSeriesViewer.ShowCentroids = False
        Me.TimeSeriesViewer.Size = New System.Drawing.Size(466, 308)
        Me.TimeSeriesViewer.TabIndex = 71
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.OptionsToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(842, 24)
        Me.MenuStrip1.TabIndex = 92
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(35, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(92, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SettingsToolStripMenuItem})
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(56, 20)
        Me.OptionsToolStripMenuItem.Text = "Options"
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.SettingsToolStripMenuItem.Text = "Settings..."
        '
        'TimeSeriesExplorer
        '
        Me.TimeSeriesExplorer.AllowLoadSave = True
        Me.TimeSeriesExplorer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TimeSeriesExplorer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TimeSeriesExplorer.Location = New System.Drawing.Point(42, 339)
        Me.TimeSeriesExplorer.Name = "TimeSeriesExplorer"
        Me.TimeSeriesExplorer.ShowCentroids = False
        Me.TimeSeriesExplorer.Size = New System.Drawing.Size(798, 159)
        Me.TimeSeriesExplorer.TabIndex = 91
        '
        'TimeSeriesCapture
        '
        Me.TimeSeriesCapture.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.TimeSeriesCapture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TimeSeriesCapture.Location = New System.Drawing.Point(7, 339)
        Me.TimeSeriesCapture.Name = "TimeSeriesCapture"
        Me.TimeSeriesCapture.Size = New System.Drawing.Size(32, 159)
        Me.TimeSeriesCapture.TabIndex = 90
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(842, 523)
        Me.Controls.Add(Me.TimeSeriesExplorer)
        Me.Controls.Add(Me.TimeSeriesCapture)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.MenuStrip1)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MinimumSize = New System.Drawing.Size(800, 550)
        Me.Name = "FormMain"
        Me.Text = "Snoopy: Workbench"
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage7.ResumeLayout(False)
        Me.TabPage6.ResumeLayout(False)
        Me.TabPage8.ResumeLayout(False)
        Me.TabPage4.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents tsStatusLabel As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents tsDataSamples As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents TimeSeriesViewer As TimeSeriesViewer
    Friend WithEvents TransformChooser As TransformChooser
    Friend WithEvents TabPage7 As System.Windows.Forms.TabPage
    Friend WithEvents Preprocessor As Preprocessor
    Friend WithEvents TabPage8 As System.Windows.Forms.TabPage
    Friend WithEvents Postprocessor As Postprocessor
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents QueryBuilder As QueryManager
    Friend WithEvents tsSelectedSeries As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents TimeSeriesCapture As TimeSeriesCapture
    Friend WithEvents TimeSeriesExplorer As TimeSeriesExplorer
    Friend WithEvents tsFilename As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tsFPS As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents OptionsToolStripDropDown As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents ShowFPSToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsSampleRate As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
