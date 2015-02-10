<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TransformChooser
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TransformChooser))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ToolStrip2 = New System.Windows.Forms.ToolStrip()
        Me.btnOpen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.btnSave = New System.Windows.Forms.ToolStripButton()
        Me.lblSource = New System.Windows.Forms.ToolStripLabel()
        Me.lblQueryFrameSize = New System.Windows.Forms.ToolStripLabel()
        Me.lvTransform = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.pgTransform = New System.Windows.Forms.PropertyGrid()
        Me.ofdTimeSeries = New System.Windows.Forms.OpenFileDialog()
        Me.sfdTimeSeries = New System.Windows.Forms.SaveFileDialog()
        Me.GroupBox1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.ToolStrip2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Panel1)
        Me.GroupBox1.Controls.Add(Me.lvTransform)
        Me.GroupBox1.Controls.Add(Me.pgTransform)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(322, 205)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.ToolStrip2)
        Me.Panel1.Location = New System.Drawing.Point(4, 10)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(113, 23)
        Me.Panel1.TabIndex = 54
        '
        'ToolStrip2
        '
        Me.ToolStrip2.BackColor = System.Drawing.SystemColors.ActiveBorder
        Me.ToolStrip2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnOpen, Me.ToolStripSeparator1, Me.btnSave, Me.lblSource, Me.lblQueryFrameSize})
        Me.ToolStrip2.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip2.Name = "ToolStrip2"
        Me.ToolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.ToolStrip2.Size = New System.Drawing.Size(113, 23)
        Me.ToolStrip2.TabIndex = 95
        Me.ToolStrip2.Text = "ToolStrip2"
        '
        'btnOpen
        '
        Me.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnOpen.Image = CType(resources.GetObject("btnOpen.Image"), System.Drawing.Image)
        Me.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnOpen.Name = "btnOpen"
        Me.btnOpen.Size = New System.Drawing.Size(23, 20)
        Me.btnOpen.Text = "&Load"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 23)
        '
        'btnSave
        '
        Me.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnSave.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.disk
        Me.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(23, 20)
        Me.btnSave.Text = "&Save Transform Settings"
        Me.btnSave.ToolTipText = "Save Transform Settings"
        '
        'lblSource
        '
        Me.lblSource.Name = "lblSource"
        Me.lblSource.Size = New System.Drawing.Size(16, 20)
        Me.lblSource.Text = "   "
        '
        'lblQueryFrameSize
        '
        Me.lblQueryFrameSize.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.lblQueryFrameSize.Margin = New System.Windows.Forms.Padding(0, 1, 2, 2)
        Me.lblQueryFrameSize.Name = "lblQueryFrameSize"
        Me.lblQueryFrameSize.Size = New System.Drawing.Size(16, 20)
        Me.lblQueryFrameSize.Text = "   "
        '
        'lvTransform
        '
        Me.lvTransform.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lvTransform.AutoArrange = False
        Me.lvTransform.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lvTransform.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.lvTransform.FullRowSelect = True
        Me.lvTransform.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lvTransform.HideSelection = False
        Me.lvTransform.Location = New System.Drawing.Point(4, 35)
        Me.lvTransform.MultiSelect = False
        Me.lvTransform.Name = "lvTransform"
        Me.lvTransform.ShowGroups = False
        Me.lvTransform.Size = New System.Drawing.Size(113, 166)
        Me.lvTransform.TabIndex = 53
        Me.lvTransform.UseCompatibleStateImageBehavior = False
        Me.lvTransform.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = ""
        Me.ColumnHeader1.Width = 89
        '
        'pgTransform
        '
        Me.pgTransform.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pgTransform.CausesValidation = False
        Me.pgTransform.CommandsVisibleIfAvailable = False
        Me.pgTransform.Location = New System.Drawing.Point(121, 9)
        Me.pgTransform.Name = "pgTransform"
        Me.pgTransform.Size = New System.Drawing.Size(197, 192)
        Me.pgTransform.TabIndex = 0
        Me.pgTransform.ToolbarVisible = False
        '
        'ofdTimeSeries
        '
        Me.ofdTimeSeries.DefaultExt = "tst"
        Me.ofdTimeSeries.Filter = "TimeSeries Transformer (*.tst)|*.tst|All files (*.*)|*.*"
        '
        'sfdTimeSeries
        '
        Me.sfdTimeSeries.DefaultExt = "tst"
        Me.sfdTimeSeries.Filter = "TimeSeries Transformer (*.tst)|*.tst|All files (*.*)|*.*"
        '
        'TransformChooser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "TransformChooser"
        Me.Size = New System.Drawing.Size(322, 205)
        Me.GroupBox1.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ToolStrip2.ResumeLayout(False)
        Me.ToolStrip2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents pgTransform As System.Windows.Forms.PropertyGrid
    Friend WithEvents lvTransform As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents ToolStrip2 As System.Windows.Forms.ToolStrip
    Friend WithEvents btnOpen As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents lblSource As System.Windows.Forms.ToolStripLabel
    Friend WithEvents lblQueryFrameSize As System.Windows.Forms.ToolStripLabel
    Friend WithEvents ofdTimeSeries As System.Windows.Forms.OpenFileDialog
    Friend WithEvents sfdTimeSeries As System.Windows.Forms.SaveFileDialog

End Class
