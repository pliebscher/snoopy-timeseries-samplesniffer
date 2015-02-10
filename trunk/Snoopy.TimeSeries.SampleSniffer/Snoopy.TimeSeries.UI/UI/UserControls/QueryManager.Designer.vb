<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class QueryManager
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(QueryManager))
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.cbQueryType = New System.Windows.Forms.ToolStripComboBox()
        Me.pgQueryProperties = New System.Windows.Forms.PropertyGrid()
        Me.gvQueries = New System.Windows.Forms.DataGridView()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.ofdTimeSeriesQuery = New System.Windows.Forms.OpenFileDialog()
        Me.sfdTimeSeriesQuery = New System.Windows.Forms.SaveFileDialog()
        Me.dlgLabelColor = New System.Windows.Forms.ColorDialog()
        Me.btnOpenQuery = New System.Windows.Forms.ToolStripButton()
        Me.btnSaveQuery = New System.Windows.Forms.ToolStripButton()
        Me.btnRemoveQuery = New System.Windows.Forms.ToolStripButton()
        Me.btnAddQuery = New System.Windows.Forms.ToolStripButton()
        Me.btnQueryReset = New System.Windows.Forms.ToolStripButton()
        Me.btnShowResults = New System.Windows.Forms.ToolStripButton()
        Me.LabelColor = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QueryEnabled = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.LogQuery = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.QueryName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.UpdateCriteria = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.ViewCriteria = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.ToolStrip1.SuspendLayout()
        CType(Me.gvQueries, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStrip1
        '
        Me.ToolStrip1.CanOverflow = False
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnOpenQuery, Me.btnSaveQuery, Me.btnRemoveQuery, Me.btnAddQuery, Me.cbQueryType, Me.btnQueryReset, Me.btnShowResults})
        Me.ToolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(258, 25)
        Me.ToolStrip1.TabIndex = 93
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'cbQueryType
        '
        Me.cbQueryType.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.cbQueryType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbQueryType.DropDownWidth = 125
        Me.cbQueryType.Enabled = False
        Me.cbQueryType.Items.AddRange(New Object() {"New..."})
        Me.cbQueryType.MaxDropDownItems = 32
        Me.cbQueryType.Name = "cbQueryType"
        Me.cbQueryType.Size = New System.Drawing.Size(85, 25)
        Me.cbQueryType.ToolTipText = "Queries..."
        '
        'pgQueryProperties
        '
        Me.pgQueryProperties.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pgQueryProperties.Location = New System.Drawing.Point(0, 0)
        Me.pgQueryProperties.Name = "pgQueryProperties"
        Me.pgQueryProperties.PropertySort = System.Windows.Forms.PropertySort.Alphabetical
        Me.pgQueryProperties.Size = New System.Drawing.Size(258, 218)
        Me.pgQueryProperties.TabIndex = 79
        Me.pgQueryProperties.ToolbarVisible = False
        '
        'gvQueries
        '
        Me.gvQueries.AllowUserToAddRows = False
        Me.gvQueries.AllowUserToDeleteRows = False
        Me.gvQueries.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvQueries.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.gvQueries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.gvQueries.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.LabelColor, Me.QueryEnabled, Me.LogQuery, Me.QueryName, Me.UpdateCriteria, Me.ViewCriteria})
        Me.gvQueries.Cursor = System.Windows.Forms.Cursors.Hand
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.gvQueries.DefaultCellStyle = DataGridViewCellStyle4
        Me.gvQueries.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gvQueries.Location = New System.Drawing.Point(0, 0)
        Me.gvQueries.Name = "gvQueries"
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvQueries.RowHeadersDefaultCellStyle = DataGridViewCellStyle5
        Me.gvQueries.RowHeadersVisible = False
        Me.gvQueries.RowHeadersWidth = 25
        Me.gvQueries.RowTemplate.Height = 20
        Me.gvQueries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.gvQueries.Size = New System.Drawing.Size(258, 150)
        Me.gvQueries.TabIndex = 75
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 25)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.gvQueries)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.pgQueryProperties)
        Me.SplitContainer1.Size = New System.Drawing.Size(258, 372)
        Me.SplitContainer1.SplitterDistance = 150
        Me.SplitContainer1.TabIndex = 94
        '
        'ofdTimeSeriesQuery
        '
        Me.ofdTimeSeriesQuery.DefaultExt = "tsq"
        Me.ofdTimeSeriesQuery.Filter = "TimeSeries Query (*.tsq)|*.tsq|All files (*.*)|*.*"
        '
        'sfdTimeSeriesQuery
        '
        Me.sfdTimeSeriesQuery.DefaultExt = "tsq"
        Me.sfdTimeSeriesQuery.Filter = "TimeSeries Query (*.tsq)|*.tsq|All files (*.*)|*.*"
        '
        'btnOpenQuery
        '
        Me.btnOpenQuery.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnOpenQuery.Image = CType(resources.GetObject("btnOpenQuery.Image"), System.Drawing.Image)
        Me.btnOpenQuery.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnOpenQuery.Name = "btnOpenQuery"
        Me.btnOpenQuery.Size = New System.Drawing.Size(23, 22)
        Me.btnOpenQuery.Text = "&Open"
        Me.btnOpenQuery.ToolTipText = "Open Query"
        '
        'btnSaveQuery
        '
        Me.btnSaveQuery.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnSaveQuery.Enabled = False
        Me.btnSaveQuery.Image = CType(resources.GetObject("btnSaveQuery.Image"), System.Drawing.Image)
        Me.btnSaveQuery.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnSaveQuery.Name = "btnSaveQuery"
        Me.btnSaveQuery.Size = New System.Drawing.Size(23, 22)
        Me.btnSaveQuery.Text = "&Save"
        Me.btnSaveQuery.ToolTipText = "Save Query"
        '
        'btnRemoveQuery
        '
        Me.btnRemoveQuery.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.btnRemoveQuery.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnRemoveQuery.Enabled = False
        Me.btnRemoveQuery.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.delete
        Me.btnRemoveQuery.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnRemoveQuery.Name = "btnRemoveQuery"
        Me.btnRemoveQuery.Size = New System.Drawing.Size(23, 22)
        Me.btnRemoveQuery.Text = "&Remove"
        Me.btnRemoveQuery.ToolTipText = "Remove Query"
        '
        'btnAddQuery
        '
        Me.btnAddQuery.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.btnAddQuery.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnAddQuery.Enabled = False
        Me.btnAddQuery.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.add
        Me.btnAddQuery.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnAddQuery.Name = "btnAddQuery"
        Me.btnAddQuery.Size = New System.Drawing.Size(23, 22)
        Me.btnAddQuery.Text = "&Add"
        Me.btnAddQuery.ToolTipText = "Add Query"
        '
        'btnQueryReset
        '
        Me.btnQueryReset.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.btnQueryReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnQueryReset.Enabled = False
        Me.btnQueryReset.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.lightning
        Me.btnQueryReset.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnQueryReset.Name = "btnQueryReset"
        Me.btnQueryReset.Size = New System.Drawing.Size(23, 22)
        Me.btnQueryReset.Text = "ToolStripButton1"
        Me.btnQueryReset.ToolTipText = "Reset Query"
        '
        'btnShowResults
        '
        Me.btnShowResults.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnShowResults.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.sum
        Me.btnShowResults.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnShowResults.Name = "btnShowResults"
        Me.btnShowResults.Size = New System.Drawing.Size(23, 22)
        Me.btnShowResults.Text = "&Results"
        Me.btnShowResults.ToolTipText = "Show Results Viewer"
        '
        'LabelColor
        '
        Me.LabelColor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.LabelColor.FillWeight = 25.0!
        Me.LabelColor.Frozen = True
        Me.LabelColor.HeaderText = "#"
        Me.LabelColor.Name = "LabelColor"
        Me.LabelColor.ReadOnly = True
        Me.LabelColor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.LabelColor.Width = 25
        '
        'QueryEnabled
        '
        Me.QueryEnabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.QueryEnabled.Frozen = True
        Me.QueryEnabled.HeaderText = "Run"
        Me.QueryEnabled.Name = "QueryEnabled"
        Me.QueryEnabled.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.QueryEnabled.Width = 30
        '
        'LogQuery
        '
        Me.LogQuery.Frozen = True
        Me.LogQuery.HeaderText = "Log"
        Me.LogQuery.Name = "LogQuery"
        Me.LogQuery.Width = 30
        '
        'QueryName
        '
        Me.QueryName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.QueryName.HeaderText = "Query"
        Me.QueryName.Name = "QueryName"
        Me.QueryName.ReadOnly = True
        Me.QueryName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'UpdateCriteria
        '
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ButtonFace
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ButtonFace
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black
        Me.UpdateCriteria.DefaultCellStyle = DataGridViewCellStyle2
        Me.UpdateCriteria.FillWeight = 10.0!
        Me.UpdateCriteria.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.UpdateCriteria.HeaderText = ""
        Me.UpdateCriteria.MinimumWidth = 22
        Me.UpdateCriteria.Name = "UpdateCriteria"
        Me.UpdateCriteria.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.UpdateCriteria.ToolTipText = "Update Criteria"
        Me.UpdateCriteria.Width = 22
        '
        'ViewCriteria
        '
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.ViewCriteria.DefaultCellStyle = DataGridViewCellStyle3
        Me.ViewCriteria.FillWeight = 5.0!
        Me.ViewCriteria.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ViewCriteria.HeaderText = ""
        Me.ViewCriteria.MinimumWidth = 22
        Me.ViewCriteria.Name = "ViewCriteria"
        Me.ViewCriteria.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ViewCriteria.ToolTipText = "View Criteria"
        Me.ViewCriteria.Width = 22
        '
        'QueryManager
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Name = "QueryManager"
        Me.Size = New System.Drawing.Size(258, 397)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        CType(Me.gvQueries, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents btnOpenQuery As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnSaveQuery As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnRemoveQuery As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnAddQuery As System.Windows.Forms.ToolStripButton
    Friend WithEvents cbQueryType As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents pgQueryProperties As System.Windows.Forms.PropertyGrid
    Friend WithEvents gvQueries As System.Windows.Forms.DataGridView
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents ofdTimeSeriesQuery As System.Windows.Forms.OpenFileDialog
    Friend WithEvents sfdTimeSeriesQuery As System.Windows.Forms.SaveFileDialog
    Friend WithEvents btnQueryReset As System.Windows.Forms.ToolStripButton
    Friend WithEvents dlgLabelColor As System.Windows.Forms.ColorDialog
    Friend WithEvents btnShowResults As System.Windows.Forms.ToolStripButton
    Friend WithEvents LabelColor As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents QueryEnabled As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents LogQuery As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents QueryName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents UpdateCriteria As System.Windows.Forms.DataGridViewButtonColumn
    Friend WithEvents ViewCriteria As System.Windows.Forms.DataGridViewButtonColumn

End Class
