<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormQueryMatchLogger
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
        Me.components = New System.ComponentModel.Container()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim RgB2561 As Snoopy.TimeSeries.UI.RGB256 = New Snoopy.TimeSeries.UI.RGB256()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormQueryMatchLogger))
        Me.btnHide = New System.Windows.Forms.Button()
        Me.dgvMatches = New System.Windows.Forms.DataGridView()
        Me.txtCount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Id = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.txtTimestamp = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.txtResult = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cmsResults = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.nudMaxResults = New System.Windows.Forms.NumericUpDown()
        Me.lblResultCount = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.sfdResults = New System.Windows.Forms.SaveFileDialog()
        Me.tsViewer = New Snoopy.TimeSeries.UI.TimeSeriesViewer()
        CType(Me.dgvMatches, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.cmsResults.SuspendLayout()
        CType(Me.nudMaxResults, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnHide
        '
        Me.btnHide.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnHide.Location = New System.Drawing.Point(283, 395)
        Me.btnHide.Name = "btnHide"
        Me.btnHide.Size = New System.Drawing.Size(75, 23)
        Me.btnHide.TabIndex = 1
        Me.btnHide.Text = "Hide"
        Me.btnHide.UseVisualStyleBackColor = True
        '
        'dgvMatches
        '
        Me.dgvMatches.AllowUserToAddRows = False
        Me.dgvMatches.AllowUserToDeleteRows = False
        Me.dgvMatches.AllowUserToResizeRows = False
        Me.dgvMatches.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvMatches.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvMatches.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMatches.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.txtCount, Me.Id, Me.txtTimestamp, Me.txtResult})
        Me.dgvMatches.ContextMenuStrip = Me.cmsResults
        Me.dgvMatches.Location = New System.Drawing.Point(1, 229)
        Me.dgvMatches.MultiSelect = False
        Me.dgvMatches.Name = "dgvMatches"
        Me.dgvMatches.ReadOnly = True
        Me.dgvMatches.RowHeadersVisible = False
        Me.dgvMatches.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvMatches.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvMatches.Size = New System.Drawing.Size(357, 161)
        Me.dgvMatches.TabIndex = 2
        '
        'txtCount
        '
        Me.txtCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.txtCount.FillWeight = 76.14216!
        Me.txtCount.HeaderText = "#"
        Me.txtCount.Name = "txtCount"
        Me.txtCount.ReadOnly = True
        Me.txtCount.Width = 40
        '
        'Id
        '
        Me.Id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.Id.HeaderText = "Id"
        Me.Id.Name = "Id"
        Me.Id.ReadOnly = True
        Me.Id.Width = 40
        '
        'txtTimestamp
        '
        DataGridViewCellStyle1.Format = "MM/dd/yyyy H:mm:ss.ffff"
        DataGridViewCellStyle1.NullValue = Nothing
        Me.txtTimestamp.DefaultCellStyle = DataGridViewCellStyle1
        Me.txtTimestamp.FillWeight = 154.7829!
        Me.txtTimestamp.HeaderText = "Timestamp"
        Me.txtTimestamp.Name = "txtTimestamp"
        Me.txtTimestamp.ReadOnly = True
        '
        'txtResult
        '
        Me.txtResult.FillWeight = 69.075!
        Me.txtResult.HeaderText = "Result"
        Me.txtResult.Name = "txtResult"
        Me.txtResult.ReadOnly = True
        '
        'cmsResults
        '
        Me.cmsResults.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SaveToolStripMenuItem})
        Me.cmsResults.Name = "cmsDataSpectrogram"
        Me.cmsResults.Size = New System.Drawing.Size(111, 26)
        Me.cmsResults.Text = "Actions"
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Enabled = False
        Me.SaveToolStripMenuItem.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.disk
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(110, 22)
        Me.SaveToolStripMenuItem.Text = "Save..."
        '
        'btnClear
        '
        Me.btnClear.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnClear.Location = New System.Drawing.Point(1, 395)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(75, 23)
        Me.btnClear.TabIndex = 3
        Me.btnClear.Text = "Clear"
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(170, 400)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(30, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Max:"
        '
        'nudMaxResults
        '
        Me.nudMaxResults.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.nudMaxResults.Increment = New Decimal(New Integer() {100, 0, 0, 0})
        Me.nudMaxResults.Location = New System.Drawing.Point(199, 396)
        Me.nudMaxResults.Maximum = New Decimal(New Integer() {1000000, 0, 0, 0})
        Me.nudMaxResults.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudMaxResults.Name = "nudMaxResults"
        Me.nudMaxResults.Size = New System.Drawing.Size(69, 20)
        Me.nudMaxResults.TabIndex = 5
        Me.nudMaxResults.Value = New Decimal(New Integer() {1000, 0, 0, 0})
        '
        'lblResultCount
        '
        Me.lblResultCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblResultCount.AutoSize = True
        Me.lblResultCount.Location = New System.Drawing.Point(125, 399)
        Me.lblResultCount.Name = "lblResultCount"
        Me.lblResultCount.Size = New System.Drawing.Size(13, 13)
        Me.lblResultCount.TabIndex = 7
        Me.lblResultCount.Text = "0"
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(82, 399)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(45, 13)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Results:"
        '
        'sfdResults
        '
        Me.sfdResults.DefaultExt = "csv"
        Me.sfdResults.Filter = "Comma separated (*.csv)|*.csv|All files (*.*)|*.*"
        '
        'tsViewer
        '
        Me.tsViewer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tsViewer.Location = New System.Drawing.Point(0, -1)
        Me.tsViewer.MinimumSize = New System.Drawing.Size(316, 232)
        Me.tsViewer.Name = "tsViewer"
        Me.tsViewer.Pallet = RgB2561
        Me.tsViewer.PalletThreshold = 100
        Me.tsViewer.ShowCentroids = False
        Me.tsViewer.Size = New System.Drawing.Size(359, 232)
        Me.tsViewer.TabIndex = 0
        Me.tsViewer.SelectedTimeSeries = Nothing
        '
        'FormQueryMatchLogger
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(358, 439)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblResultCount)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.nudMaxResults)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.dgvMatches)
        Me.Controls.Add(Me.btnHide)
        Me.Controls.Add(Me.tsViewer)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(366, 447)
        Me.Name = "FormQueryMatchLogger"
        Me.Text = "Snoopy: Query Logger"
        CType(Me.dgvMatches, System.ComponentModel.ISupportInitialize).EndInit()
        Me.cmsResults.ResumeLayout(False)
        CType(Me.nudMaxResults, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tsViewer As TimeSeriesViewer
    Friend WithEvents btnHide As System.Windows.Forms.Button
    Friend WithEvents dgvMatches As System.Windows.Forms.DataGridView
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents nudMaxResults As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblResultCount As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents sfdResults As System.Windows.Forms.SaveFileDialog
    Friend WithEvents cmsResults As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents txtCount As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Id As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents txtTimestamp As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents txtResult As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
