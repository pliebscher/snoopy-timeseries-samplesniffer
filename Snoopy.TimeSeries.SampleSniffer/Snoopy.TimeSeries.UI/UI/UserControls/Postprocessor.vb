Public Class Postprocessor

    Private _RaiseChangedEvent As Boolean

    Private _SampleRate As Integer = 1
    Private _PostProcessors As New List(Of TimeSeriesPostProcessor)
    Private _Factories As New Dictionary(Of String, Func(Of TimeSeriesPostProcessor))

    Public Event Changed(sender As Object, e As EventArgs)

#Region " -- Form -- "

    Private Sub Preprocessor_Load(sender As Object, e As EventArgs) Handles Me.Load
        cbPostprocessors.Items.Insert(0, "Select...")
        cbPostprocessors.SelectedIndex = 0
        _RaiseChangedEvent = True
    End Sub

    Private Sub btnUp_Click(sender As Object, e As EventArgs) Handles btnPostProcUp.Click
        MoveListViewItems(lvPostproc, MoveDirection.Up)
    End Sub

    Private Sub btnDown_Click(sender As Object, e As EventArgs) Handles btnPostProcDown.Click
        MoveListViewItems(lvPostproc, MoveDirection.Down)
    End Sub

    Private Sub lvPostproc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvPostproc.SelectedIndexChanged
        If lvPostproc.SelectedItems.Count = 0 Then Exit Sub
        pgPostprocessor.SelectedObject = _PostProcessors(lvPostproc.SelectedIndices(0))
        btnRemove.Enabled = True
    End Sub

    Private Sub lvPostproc_ItemChecked(sender As Object, e As ItemCheckedEventArgs) Handles lvPostproc.ItemChecked
        e.Item.Selected = True
        If _RaiseChangedEvent Then
            _PostProcessors(e.Item.Index).Enabled = e.Item.Checked
            pgPostprocessor.Refresh()
            RaiseEvent Changed(Me, e)
        End If
    End Sub

    Private Sub pgPostprocessor_PropertyValueChanged(s As Object, e As PropertyValueChangedEventArgs) Handles pgPostprocessor.PropertyValueChanged
        If lvPostproc.SelectedItems(0).Checked Then
            Try
                pgPostprocessor.Refresh()
                RaiseEvent Changed(Me, e)
                pgPostprocessor.BackColor = pgPostprocessor.HelpBackColor
            Catch ex As Exception
                pgPostprocessor.BackColor = Color.Red
                ShowException("Bad property value! The new propetry value caused an exception to be thrown.", ex)
            End Try
        End If
    End Sub

    Private Sub Postprocessor_Load(sender As Object, e As EventArgs) Handles Me.Load
        _RaiseChangedEvent = True
    End Sub

    Private Sub cbPostprocessors_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbPostprocessors.SelectedIndexChanged
        If cbPostprocessors.SelectedIndex > 0 Then
            btnAdd.Enabled = True
        Else
            btnAdd.Enabled = False
        End If
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim name As String = cbPostprocessors.SelectedItem.ToString
        AddSelectedProcessor(_Factories(name)())
        cbPostprocessors.SelectedIndex = 0
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        Dim RaiseChanged As Boolean
        For Each item As ListViewItem In lvPostproc.SelectedItems
            RaiseChanged = item.Checked ' No need to raise changed if the processor was not active.
            _PostProcessors.RemoveAt(item.Index)
            lvPostproc.Items.Remove(item)
        Next
        pgPostprocessor.SelectedObject = Nothing
        btnRemove.Enabled = False
        If RaiseChanged Then
            RaiseEvent Changed(Me, e)
        End If
    End Sub

    Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        If ofdTimeSeries.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            Dim path As String = ofdTimeSeries.FileName
            Try

                Dim processor As TimeSeriesPostProcessor = TryCast(TimeSeriesProcessor.Load(path), TimeSeriesPostProcessor)

                If processor IsNot Nothing Then

                    Me.AddSelectedProcessor(processor)

                    lvPostproc.SelectedItems.Clear()
                    lvPostproc.Items(lvPostproc.Items.Count - 1).Selected = True
                    lvPostproc.Items(lvPostproc.Items.Count - 1).Text = lvPostproc.Items(lvPostproc.Items.Count - 1).Text & "*"
                    lvPostproc.Items(lvPostproc.Items.Count - 1).ToolTipText = System.IO.Path.GetFileName(path)
                    lvPostproc.Focus()

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

                _PostProcessors(lvPostproc.SelectedIndices(0)).Save(path)

            Catch ex As Exception
                ShowException("Error saving processor settings...", ex)
            Finally

            End Try
        End If
    End Sub

#End Region

    Public Sub AddProcessor(Of T As {TimeSeriesPostProcessor})()
        AddProcessor(Of T)(False)
    End Sub

    Public Sub AddProcessor(Of T As {TimeSeriesPostProcessor})(enabled As Boolean)

        Dim f As Func(Of TimeSeriesPostProcessor) = Function() As TimeSeriesPostProcessor
                                                        Return TimeSeriesProcessor.Create(Of T)(My.Settings.SampleRate)
                                                    End Function
        If enabled Then
            Dim proc As TimeSeriesPostProcessor = f()
            proc.Enabled = True
            AddSelectedProcessor(proc)
        End If

        cbPostprocessors.Items.Add(GetType(T).Name)

        If Not _Factories.ContainsKey(GetType(T).Name) Then
            _Factories.Add(GetType(T).Name, f)
        End If

    End Sub

    Private Sub AddSelectedProcessor(processor As TimeSeriesPostProcessor)
        _RaiseChangedEvent = False
        _PostProcessors.Add(processor)

        Dim item As New ListViewItem(processor.GetType.Name) With {.Name = processor.GetType.Name, .ToolTipText = processor.GetType.Name}
        lvPostproc.SuspendLayout()
        lvPostproc.Items.Add(item)
        lvPostproc.ResumeLayout()

        item.Checked = processor.Enabled
        item.Selected = True
        pgPostprocessor.SelectedObject = _PostProcessors(lvPostproc.SelectedIndices(0))
        btnRemove.Enabled = True
        _RaiseChangedEvent = True

    End Sub

    Private Enum MoveDirection
        Up = -1
        Down = 1
    End Enum

    Private Sub MoveListViewItems(listView As ListView, direction As MoveDirection)
        If listView.SelectedItems.Count > 0 Then
            Dim dir As Integer = CInt(direction)
            listView.SuspendLayout()
            Dim currentIndex As Integer = listView.SelectedItems(0).Index
            If currentIndex + dir = -1 Or currentIndex + dir = listView.Items.Count Then Exit Sub
            Dim item As ListViewItem = listView.Items(currentIndex)

            If currentIndex >= 0 AndAlso currentIndex < listView.Items.Count Then
                _RaiseChangedEvent = False
                listView.Items.RemoveAt(currentIndex) ' for some reason the RemoveAt and Insert cause the ItemChecked event to be raised.
                listView.Items.Insert(currentIndex + dir, item)

                Dim processor As TimeSeriesPostProcessor = _PostProcessors(currentIndex)
                _PostProcessors.RemoveAt(currentIndex)
                _PostProcessors.Insert(currentIndex + dir, processor)

                _RaiseChangedEvent = True
            End If
            listView.ResumeLayout()
            If item.Checked Then RaiseEvent Changed(Me, EventArgs.Empty)
        End If
    End Sub

    Public ReadOnly Property Processors As List(Of TimeSeriesPostProcessor)
        Get
            Return _PostProcessors
        End Get
    End Property

    Public Property SampleRate As Integer
        Get
            Return _SampleRate
        End Get
        Set(value As Integer)
            If _SampleRate <> value Then
                _SampleRate = value
                For Each proc As TimeSeriesPostProcessor In _PostProcessors
                    proc.SampleRate = _SampleRate
                Next
                pgPostprocessor.Refresh()
            End If
        End Set
    End Property

    Private Sub ShowException(message As String, ex As Exception)
        Dim ExViewer As New FormExceptionViewer
        ExViewer.Show(Me.ParentForm, message, ex)
    End Sub

End Class
