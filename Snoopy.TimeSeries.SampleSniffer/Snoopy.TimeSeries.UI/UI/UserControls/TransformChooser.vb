Public Class TransformChooser

    Private _SampleRate As Integer = 1
    Private _RaiseChangedEvent As Boolean
    Private _SelectedTransformer As TimeSeriesTransformer
    Private _Transformers As New Dictionary(Of String, TimeSeriesTransformer)

    Public Event Changed(sender As Object, e As EventArgs)

    Public Sub AddTransformer(Of T As {TimeSeriesTransformer})()
        AddTransformer(TimeSeriesProcessor.Create(Of T)(_SampleRate))
    End Sub

    Public Sub AddTransformer(transform As TimeSeriesTransformer)
        Dim name As String = transform.GetType.Name
        Dim i As Integer = 1
        While _Transformers.ContainsKey(name)
            name = String.Format("{0}{1}", transform.GetType.Name, i)
            i += 1
        End While
        _Transformers.Add(name, transform)
        lvTransform.SuspendLayout()
        lvTransform.Items.Add(name)
        If _SelectedTransformer Is Nothing Then
            lvTransform.Items(0).Selected = True
            _SelectedTransformer = _Transformers(name)
        End If
        lvTransform.ResumeLayout()
    End Sub

#Region " -- Form -- "

    Private Sub pgTransform_PropertyValueChanged(s As Object, e As PropertyValueChangedEventArgs) Handles pgTransform.PropertyValueChanged
        If e.ChangedItem.Label = "TimeOut" Then Exit Sub
        If _RaiseChangedEvent Then
            Try
                pgTransform.Refresh()
                RaiseEvent Changed(Me, e)
                pgTransform.BackColor = pgTransform.HelpBackColor
            Catch ex As Exception
                pgTransform.BackColor = Color.Red

                ShowException("Bad property value! The new propetry value caused an exception to be thrown.", ex)

            End Try

        End If
    End Sub

    Private Sub lvTransform_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvTransform.SelectedIndexChanged
        If lvTransform.SelectedItems.Count = 0 Then Exit Sub
        _SelectedTransformer = _Transformers(lvTransform.SelectedItems(0).Text)
        pgTransform.SelectedObject = _Transformers(lvTransform.SelectedItems(0).Text)
        If _RaiseChangedEvent Then
            RaiseEvent Changed(Me, e)
        End If
    End Sub

    Private Sub Preprocessor_Load(sender As Object, e As EventArgs) Handles Me.Load
        _RaiseChangedEvent = True
    End Sub

    Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        If ofdTimeSeries.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            Dim path As String = ofdTimeSeries.FileName
            Try

                Dim xformer As TimeSeriesFrameTransformer = TryCast(TimeSeriesProcessor.Load(path), TimeSeriesFrameTransformer)

                If xformer IsNot Nothing Then

                    Me.AddTransformer(xformer)

                    lvTransform.SelectedItems.Clear()
                    lvTransform.Items(lvTransform.Items.Count - 1).Selected = True
                    lvTransform.Items(lvTransform.Items.Count - 1).Text = lvTransform.Items(lvTransform.Items.Count - 1).Text & "*"
                    lvTransform.Items(lvTransform.Items.Count - 1).ToolTipText = System.IO.Path.GetFileName(path)
                    lvTransform.Focus()

                    RaiseEvent Changed(Me, EventArgs.Empty)
                Else
                    MsgBox("Unable to load transformer settings.", MsgBoxStyle.Exclamation, "Error...")
                End If

            Catch ex As Exception
                ShowException("Error loading tranform settings...", ex)
            Finally

            End Try
        End If

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If sfdTimeSeries.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            Dim path As String = sfdTimeSeries.FileName
            Try

                _SelectedTransformer.Save(path)

            Catch ex As Exception
                ShowException("Error saving tranform settings...", ex)
            Finally

            End Try
        End If
    End Sub

#End Region

    Public ReadOnly Property SelectedTransformer As TimeSeriesTransformer
        Get
            Return _SelectedTransformer
        End Get
    End Property

    Public Property SampleRate As Integer
        Get
            Return _SampleRate
        End Get
        Set(value As Integer)
            If _SampleRate <> value Then
                _SampleRate = value
                For Each proc As KeyValuePair(Of String, TimeSeriesTransformer) In _Transformers
                    proc.Value.SampleRate = _SampleRate
                Next
                pgTransform.Refresh()
            End If
        End Set
    End Property

    Private Sub ShowException(message As String, ex As Exception)
        Dim ExViewer As New FormExceptionViewer
        ExViewer.Show(Me.ParentForm, message, ex)
    End Sub

End Class
