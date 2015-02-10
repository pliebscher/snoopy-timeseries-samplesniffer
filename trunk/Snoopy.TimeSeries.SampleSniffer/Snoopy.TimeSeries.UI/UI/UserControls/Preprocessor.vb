Public Class Preprocessor

    Private _RaiseChangedEvent As Boolean

    Private _SampleRate As Integer = 1
    Private _PreProcessors As New List(Of TimeSeriesPreprocessor)
    Private _PreProcessorsEnabled As New List(Of TimeSeriesPreprocessor)
    Private _Factories As New Dictionary(Of String, Func(Of TimeSeriesPreprocessor))

    Public Event Changed(sender As Object, e As EventArgs)

#Region " -- Form -- "

    Private Sub Preprocessor_Load(sender As Object, e As EventArgs) Handles Me.Load
        cbPreprocessors.Items.Insert(0, "Select...")
        cbPreprocessors.SelectedIndex = 0
        _RaiseChangedEvent = True
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim name As String = cbPreprocessors.SelectedItem.ToString
        AddSelectedProcessor(_Factories(name)(), False)
        cbPreprocessors.SelectedIndex = 0
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        Dim RaiseChanged As Boolean
        For Each item As ListViewItem In lvPreproc.SelectedItems
            RaiseChanged = item.Checked ' No need to raise changed if the processor was not active.
            If item.Checked Then
                _PreProcessorsEnabled.RemoveAt(item.Index)
            End If
            _PreProcessors.RemoveAt(item.Index)
            lvPreproc.Items.Remove(item)
        Next
        pgPreprocessor.SelectedObject = Nothing
        btnRemove.Enabled = False
        btnSave.Enabled = False
        If RaiseChanged Then
            RaiseEvent Changed(Me, e)
        End If
    End Sub

    Private Sub btnPreprocUp_Click(sender As Object, e As EventArgs) Handles btnPreprocUp.Click
        MoveListViewItems(lvPreproc, MoveDirection.Up)
    End Sub

    Private Sub btnPreprocDown_Click(sender As Object, e As EventArgs) Handles btnPreprocDown.Click
        MoveListViewItems(lvPreproc, MoveDirection.Down)
    End Sub

    Private Sub lvPreproc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvPreproc.SelectedIndexChanged
        If lvPreproc.SelectedItems.Count = 0 Then Exit Sub
        pgPreprocessor.SelectedObject = _PreProcessors(lvPreproc.SelectedIndices(0))
        btnSave.Enabled = True
        btnRemove.Enabled = True
    End Sub

    Private Sub lvPreproc_ItemChecked(sender As Object, e As ItemCheckedEventArgs) Handles lvPreproc.ItemChecked
        e.Item.Selected = True
        If _RaiseChangedEvent Then
            _PreProcessors(e.Item.Index).Enabled = e.Item.Checked
            pgPreprocessor.Refresh()
            RaiseEvent Changed(Me, e)
        End If
    End Sub

    Private Sub pgPreprocessor_PropertyValueChanged(s As Object, e As PropertyValueChangedEventArgs) Handles pgPreprocessor.PropertyValueChanged
        ' Only raise the changed event if processor is active...
        If lvPreproc.SelectedItems(0).Checked Then
            Try
                pgPreprocessor.Refresh()
                RaiseEvent Changed(Me, e)
                pgPreprocessor.BackColor = pgPreprocessor.HelpBackColor
            Catch ex As Exception
                pgPreprocessor.BackColor = Color.Red
                ShowException("Bad property value! The new propetry value caused an exception to be thrown.", ex)
            End Try
        End If
    End Sub

    Private Sub cbPreprocessors_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbPreprocessors.SelectedIndexChanged
        If cbPreprocessors.SelectedIndex > 0 Then
            btnAdd.Enabled = True
        Else
            btnAdd.Enabled = False
        End If
    End Sub

    Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        If ofdTimeSeries.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            Dim path As String = ofdTimeSeries.FileName
            Try

                Dim processor As TimeSeriesPreprocessor = TryCast(TimeSeriesProcessor.Load(path), TimeSeriesPreprocessor)

                If processor IsNot Nothing Then

                    Me.AddSelectedProcessor(processor, False)

                    lvPreproc.SelectedItems.Clear()
                    lvPreproc.Items(lvPreproc.Items.Count - 1).Selected = True
                    lvPreproc.Items(lvPreproc.Items.Count - 1).Text = lvPreproc.Items(lvPreproc.Items.Count - 1).Text & "*"
                    lvPreproc.Items(lvPreproc.Items.Count - 1).ToolTipText = System.IO.Path.GetFileName(path)
                    lvPreproc.Focus()

                    RaiseEvent Changed(Me, EventArgs.Empty)
                Else
                    MsgBox("Unable to load processor settings.", MsgBoxStyle.Exclamation, "Error...")
                End If

            Catch ex As Exception
                ShowException("Error loading processor settings...", ex)
            Finally

            End Try
        End If

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If sfdTimeSeries.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            Dim path As String = sfdTimeSeries.FileName
            Try

                _PreProcessors(lvPreproc.SelectedIndices(0)).Save(path)

            Catch ex As Exception
                ShowException("Error saving processor settings...", ex)
            Finally

            End Try
        End If
    End Sub

#End Region

    Public Sub AddProcessor(Of T As {TimeSeriesPreprocessor})()
        AddProcessor(Of T)(False, False)
    End Sub

    Public Sub AddProcessor(Of T As {TimeSeriesPreprocessor})(visible As Boolean, enabled As Boolean)

        Dim f As Func(Of TimeSeriesPreprocessor) = Function() As TimeSeriesPreprocessor
                                                       Return TimeSeriesProcessor.Create(Of T)(My.Settings.SampleRate)
                                                   End Function
        If visible Then
            Dim proc As TimeSeriesPreprocessor = f()
            proc.Enabled = enabled
            AddSelectedProcessor(proc, enabled)
        End If

        cbPreprocessors.Items.Add(GetType(T).Name)

        If Not _Factories.ContainsKey(GetType(T).Name) Then
            _Factories.Add(GetType(T).Name, f)
        End If

    End Sub

    Private Sub AddSelectedProcessor(processor As TimeSeriesPreprocessor, enabled As Boolean)
        _RaiseChangedEvent = False
        _PreProcessors.Add(processor)

        If enabled Then
            _PreProcessorsEnabled.Add(processor)
        End If

        Dim item As New ListViewItem(processor.GetType.Name) With {.Name = processor.GetType.Name, .ToolTipText = processor.GetType.Name}

        lvPreproc.SuspendLayout()
        lvPreproc.Items.Add(item)
        lvPreproc.ResumeLayout()

        item.Checked = processor.Enabled
        item.Selected = True
        pgPreprocessor.SelectedObject = _PreProcessors(lvPreproc.SelectedIndices(0))
        btnRemove.Enabled = True
        _RaiseChangedEvent = True
    End Sub

    Private Sub MoveListViewItems(sender As ListView, direction As MoveDirection)
        If sender.SelectedItems.Count > 0 Then
            Dim dir As Integer = CInt(direction)
            sender.SuspendLayout()
            Dim currentIndex As Integer = sender.SelectedItems(0).Index
            If currentIndex + dir = -1 Or currentIndex + dir = sender.Items.Count Then Exit Sub
            Dim item As ListViewItem = sender.Items(currentIndex)
            If currentIndex >= 0 AndAlso currentIndex < sender.Items.Count Then
                _RaiseChangedEvent = False
                sender.Items.RemoveAt(currentIndex) ' for some reason the RemoveAt and Insert cause the ItemChecked event to be raised.
                sender.Items.Insert(currentIndex + dir, item)

                Dim processor As TimeSeriesPreprocessor = _PreProcessors(currentIndex)
                _PreProcessors.RemoveAt(currentIndex)
                _PreProcessors.Insert(currentIndex + dir, processor) ' Use .Reverse(currentIndex + dir, 2) ???????

                _RaiseChangedEvent = True
            End If
            sender.ResumeLayout()
            If item.Checked Then RaiseEvent Changed(Me, EventArgs.Empty)
        End If
    End Sub

    Private Enum MoveDirection
        Up = -1
        Down = 1
    End Enum

    Public ReadOnly Property Processors As List(Of TimeSeriesPreprocessor)
        Get
            Return _PreProcessors
        End Get
    End Property

    Public Property SampleRate As Integer
        Get
            Return _SampleRate
        End Get
        Set(value As Integer)
            If _SampleRate <> value Then
                _SampleRate = value
                For Each proc As TimeSeriesPreprocessor In _PreProcessors
                    proc.SampleRate = App.SampleRate
                Next
                pgPreprocessor.Refresh()
            End If
        End Set
    End Property

    Private Sub ShowException(message As String, ex As Exception)
        Dim ExViewer As New FormExceptionViewer
        ExViewer.Show(Me.ParentForm, message, ex)
    End Sub

End Class
