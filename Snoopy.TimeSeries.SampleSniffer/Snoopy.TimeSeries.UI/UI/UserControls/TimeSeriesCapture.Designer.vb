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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TimeSeriesCapture))
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.btnMonitorStart = New System.Windows.Forms.Button()
        Me.btnMonitorStop = New System.Windows.Forms.Button()
        Me.ToolStripComboBox2 = New System.Windows.Forms.ToolStripComboBox()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.SystemColors.AppWorkspace
        Me.Panel2.Controls.Add(Me.btnMonitorStart)
        Me.Panel2.Controls.Add(Me.btnMonitorStop)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(31, 87)
        Me.Panel2.TabIndex = 88
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
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents btnMonitorStart As System.Windows.Forms.Button
    Friend WithEvents btnMonitorStop As System.Windows.Forms.Button
    Friend WithEvents ToolStripComboBox2 As System.Windows.Forms.ToolStripComboBox

End Class
