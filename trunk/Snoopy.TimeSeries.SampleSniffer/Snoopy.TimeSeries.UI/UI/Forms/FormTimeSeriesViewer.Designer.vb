<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormTimeSeriesViewer
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
        Dim RgB2561 As Snoopy.TimeSeries.UI.RGB256 = New Snoopy.TimeSeries.UI.RGB256()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormTimeSeriesViewer))
        Me.btnClose = New System.Windows.Forms.Button()
        Me.TimeSeriesViewer = New Snoopy.TimeSeries.UI.TimeSeriesViewer()
        Me.SuspendLayout()
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(313, 284)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 23)
        Me.btnClose.TabIndex = 1
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'TimeSeriesViewer
        '
        Me.TimeSeriesViewer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TimeSeriesViewer.Location = New System.Drawing.Point(-2, 0)
        Me.TimeSeriesViewer.MinimumSize = New System.Drawing.Size(389, 281)
        Me.TimeSeriesViewer.Name = "TimeSeriesViewer"
        Me.TimeSeriesViewer.Pallet = RgB2561
        Me.TimeSeriesViewer.PalletThreshold = 100
        Me.TimeSeriesViewer.ShowCentroids = False
        Me.TimeSeriesViewer.Size = New System.Drawing.Size(392, 282)
        Me.TimeSeriesViewer.TabIndex = 0
        Me.TimeSeriesViewer.SelectedTimeSeries = Nothing
        '
        'FormTimeSeriesViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(388, 308)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.TimeSeriesViewer)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(396, 335)
        Me.Name = "FormTimeSeriesViewer"
        Me.Text = "Snoopy: TimeSeries Viewer"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TimeSeriesViewer As TimeSeriesViewer
    Friend WithEvents btnClose As System.Windows.Forms.Button
End Class
