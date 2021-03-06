﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Postprocessor
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Postprocessor))
        Me.GroupBox22 = New System.Windows.Forms.GroupBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ToolStrip2 = New System.Windows.Forms.ToolStrip()
        Me.btnOpen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.btnSave = New System.Windows.Forms.ToolStripButton()
        Me.lblSource = New System.Windows.Forms.ToolStripLabel()
        Me.lblQueryFrameSize = New System.Windows.Forms.ToolStripLabel()
        Me.btnRemove = New System.Windows.Forms.Button()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.cbPostprocessors = New System.Windows.Forms.ComboBox()
        Me.pgPostprocessor = New System.Windows.Forms.PropertyGrid()
        Me.btnPostProcDown = New System.Windows.Forms.Button()
        Me.btnPostProcUp = New System.Windows.Forms.Button()
        Me.lvPostproc = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ofdTimeSeries = New System.Windows.Forms.OpenFileDialog()
        Me.sfdTimeSeries = New System.Windows.Forms.SaveFileDialog()
        Me.GroupBox22.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.ToolStrip2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox22
        '
        Me.GroupBox22.Controls.Add(Me.Panel1)
        Me.GroupBox22.Controls.Add(Me.btnRemove)
        Me.GroupBox22.Controls.Add(Me.btnAdd)
        Me.GroupBox22.Controls.Add(Me.cbPostprocessors)
        Me.GroupBox22.Controls.Add(Me.pgPostprocessor)
        Me.GroupBox22.Controls.Add(Me.btnPostProcDown)
        Me.GroupBox22.Controls.Add(Me.btnPostProcUp)
        Me.GroupBox22.Controls.Add(Me.lvPostproc)
        Me.GroupBox22.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox22.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox22.Name = "GroupBox22"
        Me.GroupBox22.Size = New System.Drawing.Size(296, 207)
        Me.GroupBox22.TabIndex = 68
        Me.GroupBox22.TabStop = False
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.ToolStrip2)
        Me.Panel1.Location = New System.Drawing.Point(4, 9)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(123, 23)
        Me.Panel1.TabIndex = 89
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
        Me.ToolStrip2.Size = New System.Drawing.Size(123, 23)
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
        Me.btnSave.Text = "&Save Processor Settings"
        Me.btnSave.ToolTipText = "Save Processor Settings"
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
        'btnRemove
        '
        Me.btnRemove.AutoEllipsis = True
        Me.btnRemove.BackgroundImage = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.delete
        Me.btnRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.btnRemove.Enabled = False
        Me.btnRemove.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRemove.Location = New System.Drawing.Point(130, 57)
        Me.btnRemove.Margin = New System.Windows.Forms.Padding(0)
        Me.btnRemove.Name = "btnRemove"
        Me.btnRemove.Size = New System.Drawing.Size(20, 20)
        Me.btnRemove.TabIndex = 88
        Me.btnRemove.UseVisualStyleBackColor = True
        '
        'btnAdd
        '
        Me.btnAdd.AutoEllipsis = True
        Me.btnAdd.BackgroundImage = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.add
        Me.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.btnAdd.Enabled = False
        Me.btnAdd.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAdd.Location = New System.Drawing.Point(130, 33)
        Me.btnAdd.Margin = New System.Windows.Forms.Padding(0)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(20, 21)
        Me.btnAdd.TabIndex = 87
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'cbPostprocessors
        '
        Me.cbPostprocessors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbPostprocessors.FormattingEnabled = True
        Me.cbPostprocessors.Location = New System.Drawing.Point(4, 34)
        Me.cbPostprocessors.Name = "cbPostprocessors"
        Me.cbPostprocessors.Size = New System.Drawing.Size(123, 21)
        Me.cbPostprocessors.TabIndex = 85
        '
        'pgPostprocessor
        '
        Me.pgPostprocessor.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pgPostprocessor.Location = New System.Drawing.Point(154, 9)
        Me.pgPostprocessor.Name = "pgPostprocessor"
        Me.pgPostprocessor.Size = New System.Drawing.Size(138, 194)
        Me.pgPostprocessor.TabIndex = 83
        Me.pgPostprocessor.ToolbarVisible = False
        '
        'btnPostProcDown
        '
        Me.btnPostProcDown.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnPostProcDown.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.bullet_arrow_down
        Me.btnPostProcDown.ImageAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnPostProcDown.Location = New System.Drawing.Point(130, 181)
        Me.btnPostProcDown.Name = "btnPostProcDown"
        Me.btnPostProcDown.Size = New System.Drawing.Size(20, 20)
        Me.btnPostProcDown.TabIndex = 54
        Me.btnPostProcDown.UseVisualStyleBackColor = True
        '
        'btnPostProcUp
        '
        Me.btnPostProcUp.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnPostProcUp.Image = Global.Snoopy.TimeSeries.UI.My.Resources.Resources.bullet_arrow_up
        Me.btnPostProcUp.ImageAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnPostProcUp.Location = New System.Drawing.Point(130, 155)
        Me.btnPostProcUp.Name = "btnPostProcUp"
        Me.btnPostProcUp.Size = New System.Drawing.Size(20, 20)
        Me.btnPostProcUp.TabIndex = 53
        Me.btnPostProcUp.UseVisualStyleBackColor = True
        '
        'lvPostproc
        '
        Me.lvPostproc.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lvPostproc.AutoArrange = False
        Me.lvPostproc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lvPostproc.CheckBoxes = True
        Me.lvPostproc.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.lvPostproc.FullRowSelect = True
        Me.lvPostproc.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lvPostproc.HideSelection = False
        Me.lvPostproc.Location = New System.Drawing.Point(4, 57)
        Me.lvPostproc.MultiSelect = False
        Me.lvPostproc.Name = "lvPostproc"
        Me.lvPostproc.ShowGroups = False
        Me.lvPostproc.Size = New System.Drawing.Size(123, 145)
        Me.lvPostproc.TabIndex = 52
        Me.lvPostproc.UseCompatibleStateImageBehavior = False
        Me.lvPostproc.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = ""
        Me.ColumnHeader1.Width = 105
        '
        'ofdTimeSeries
        '
        Me.ofdTimeSeries.DefaultExt = "tpo"
        Me.ofdTimeSeries.Filter = "TimeSeries Post-Processor (*.tpo)|*.tpr|All files (*.*)|*.*"
        '
        'sfdTimeSeries
        '
        Me.sfdTimeSeries.DefaultExt = "tpo"
        Me.sfdTimeSeries.Filter = "TimeSeries Post-Processor (*.tpo)|*.tpr|All files (*.*)|*.*"
        '
        'Postprocessor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBox22)
        Me.Name = "Postprocessor"
        Me.Size = New System.Drawing.Size(296, 207)
        Me.GroupBox22.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ToolStrip2.ResumeLayout(False)
        Me.ToolStrip2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox22 As System.Windows.Forms.GroupBox
    Friend WithEvents pgPostprocessor As System.Windows.Forms.PropertyGrid
    Friend WithEvents btnPostProcDown As System.Windows.Forms.Button
    Friend WithEvents btnPostProcUp As System.Windows.Forms.Button
    Friend WithEvents lvPostproc As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents cbPostprocessors As System.Windows.Forms.ComboBox
    Friend WithEvents btnRemove As System.Windows.Forms.Button
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents ofdTimeSeries As System.Windows.Forms.OpenFileDialog
    Friend WithEvents sfdTimeSeries As System.Windows.Forms.SaveFileDialog
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents ToolStrip2 As System.Windows.Forms.ToolStrip
    Friend WithEvents btnOpen As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents lblSource As System.Windows.Forms.ToolStripLabel
    Friend WithEvents lblQueryFrameSize As System.Windows.Forms.ToolStripLabel

End Class
