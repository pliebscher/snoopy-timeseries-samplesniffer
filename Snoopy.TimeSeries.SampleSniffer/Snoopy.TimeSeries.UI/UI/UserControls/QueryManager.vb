Public Class QueryManager

    Private _ActiveQueries As New List(Of TimeSeriesQuery)
    Private _Queries As New List(Of TimeSeriesQuery)
    Private _ActiveLabels As New Dictionary(Of TimeSeriesQuery, Color)
    Private _Labels As New Dictionary(Of TimeSeriesQuery, Color)

    Private _Actions As New Dictionary(Of TimeSeriesQuery, Action(Of TimeSeriesQueryResult))

    Private _Logger As FormQueryMatchLogger
    Private _SelectedQuery As TimeSeriesQuery
    Private _TimeSeries As TimeSeries
    Private _MaxQueryResults As Integer

    Private _NullAction As New Action(Of TimeSeriesQueryResult)(Sub(result As TimeSeriesQueryResult) Return)

    Public Event SelectedQueryChanged(sender As Object, e As TimeSeriesQueryEventArgs)
    Public Event QueryRemoved(sender As Object, e As TimeSeriesQueryEventArgs)
    Public Event QueryChanged(sender As Object, e As TimeSeriesQueryEventArgs)
    Public Event QueryLabelChanged(sender As Object, e As TimeSeriesQueryEventArgs)

    Private _QueryTypes As New Dictionary(Of String, Type)

    Private Shared ReadOnly Rand As Random = New Random()

    Public Sub AddQuery(Of TQueryType As {TimeSeriesQuery})(name As String)
        cbQueryType.Items.Add(name)
        _QueryTypes.Add(name, GetType(TQueryType))
    End Sub

    Private Sub AddSelectedQuery(name As String)

        Dim qCount As Integer = _Queries.Count
        Dim label As Color = Color.FromArgb(Rand.Next(256), Rand.Next(256), Rand.Next(256))

        _TimeSeries = Nothing ' // hack

        gvQueries.ClearSelection()
        gvQueries.Rows.Add(New Object() {qCount + 1, 0, 0, name})
        gvQueries.Rows(gvQueries.Rows.Count - 1).Cells("LabelColor").Style.BackColor = label
        gvQueries.Rows(gvQueries.Rows.Count - 1).Cells("LabelColor").Style.SelectionBackColor = label

        _Queries.Add(_SelectedQuery)
        _Labels.Add(_SelectedQuery, label)
        _Actions.Add(_SelectedQuery, _NullAction)

        cbQueryType.SelectedIndex = 0

        _TimeSeries = _SelectedQuery.Criteria

        gvQueries.Rows(_Queries.Count - 1).Selected = True
        gvQueries.FirstDisplayedScrollingRowIndex = gvQueries.RowCount - 1

        pgQueryProperties.SelectedObject = _Queries.Last

        btnAddQuery.Enabled = False
        btnRemoveQuery.Enabled = True
        btnSaveQuery.Enabled = True
        btnQueryReset.Enabled = True

        AddHandler _SelectedQuery.VersionChanged, AddressOf Me.Query_VersionChanged

    End Sub

    Private Sub ShowResultsLogger()
        If _Logger Is Nothing OrElse _Logger.IsDisposed Then
            _Logger = New FormQueryMatchLogger(_Labels)
            _Logger.Show()
            If _ActiveQueries.Count > 0 Then
                For i As Integer = 0 To _ActiveQueries.Count - 1
                    Dim query As TimeSeriesQuery = _ActiveQueries(i)
                    For j As Integer = 0 To query.Results.Count - 1
                        Dim result As TimeSeriesQueryResult = query.Results(j)
                        If result.IsMatch Then
                            _Logger.Log(result)
                        End If
                    Next
                Next
            End If
            Exit Sub
        End If
        If _Logger.Visible Then
            '_Logger.Hide()
        Else
            _Logger.Show()
        End If
    End Sub

    Public Property MaxQueryResults As Integer
        Get
            Return _MaxQueryResults
        End Get
        Set(value As Integer)
            If value <> _MaxQueryResults Then
                _MaxQueryResults = value
                For Each query As TimeSeriesQuery In _Queries
                    query.MaxResults = _MaxQueryResults
                Next
                Me.pgQueryProperties.Refresh()
            End If
        End Set
    End Property

    Public ReadOnly Property Queries As List(Of TimeSeriesQuery)
        Get
            Return _ActiveQueries
        End Get
    End Property

    Public ReadOnly Property LabeledResults As Dictionary(Of TimeSeriesQuery, Color)
        Get
            Return _ActiveLabels
        End Get
    End Property

    Public ReadOnly Property Actions As Dictionary(Of TimeSeriesQuery, Action(Of TimeSeriesQueryResult))
        Get
            Return _Actions
        End Get
    End Property

    Public ReadOnly Property SelectedQuery As TimeSeriesQuery
        Get
            Return _SelectedQuery
        End Get
    End Property

    Public Property SelectedTimeSeries As TimeSeries
        Get
            Return _TimeSeries
        End Get
        Set(value As TimeSeries)
            _TimeSeries = value
            cbQueryType.Enabled = True
            'gvQueries.ClearSelection()
            'pgQueryProperties.SelectedObject = Nothing
            'btnRemoveQuery.Enabled = False
        End Set
    End Property

#Region " -- Form -- "

    Private Sub QueryManager_Load(sender As Object, e As EventArgs) Handles Me.Load

        cbQueryType.SelectedIndex = 0
        cbQueryType.Enabled = False

        btnSaveQuery.Enabled = False

    End Sub

    Private Sub QueryManager_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If _TimeSeries IsNot Nothing Then cbQueryType.Enabled = True
    End Sub

    Private Sub btnAddQuery_Click(sender As Object, e As EventArgs) Handles btnAddQuery.Click

        If _TimeSeries Is Nothing Then
            MsgBox("Select a TimeSeries...", MsgBoxStyle.Critical)
            Exit Sub
        End If

        _SelectedQuery = TimeSeriesQuery.CreateInstance(_QueryTypes(cbQueryType.SelectedItem.ToString), _TimeSeries)
        _SelectedQuery.MaxResults = _MaxQueryResults
        _SelectedQuery.Id = CStr(_Queries.Count + 1)

        AddSelectedQuery(_SelectedQuery.Name)

    End Sub

    Private Sub btnRemoveQuery_Click(sender As Object, e As EventArgs) Handles btnRemoveQuery.Click
        If gvQueries.SelectedRows.Count = 0 Then Exit Sub

        Dim query As TimeSeriesQuery = _Queries(gvQueries.SelectedRows(0).Index)

        RemoveHandler query.VersionChanged, AddressOf Me.Query_VersionChanged

        _Queries.Remove(query)
        If _ActiveQueries.Contains(query) Then
            _ActiveQueries.Remove(query)
        End If

        _Labels.Remove(query)
        If _ActiveLabels.ContainsKey(query) Then
            _ActiveLabels.Remove(query)
        End If

        _Actions.Remove(query)

        gvQueries.Rows.RemoveAt(gvQueries.SelectedRows(0).Index)
        gvQueries.ClearSelection()

        For Each row As DataGridViewRow In gvQueries.Rows
            row.Cells(0).Value = row.Index + 1
        Next

        pgQueryProperties.SelectedObject = Nothing

        btnAddQuery.Enabled = False
        btnRemoveQuery.Enabled = False
        btnSaveQuery.Enabled = False
        btnQueryReset.Enabled = False

        RaiseEvent QueryRemoved(Me, New TimeSeriesQueryEventArgs(query))

    End Sub

    Private Sub btnQueryReset_Click(sender As Object, e As EventArgs) Handles btnQueryReset.Click
        _SelectedQuery.Reset()
        pgQueryProperties.Refresh()
        RaiseEvent QueryChanged(Me, New TimeSeriesQueryEventArgs(_SelectedQuery))
    End Sub

    Private Sub cbQueryType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbQueryType.SelectedIndexChanged
        If Not Me.Created OrElse cbQueryType.SelectedIndex = 0 Then Exit Sub

        pgQueryProperties.SelectedObject = Nothing
        gvQueries.ClearSelection()

        btnAddQuery.Enabled = True
        btnRemoveQuery.Enabled = False

    End Sub

    Private Sub gvQueries_Click(sender As Object, e As EventArgs) Handles gvQueries.Click

        If gvQueries.SelectedRows.Count = 0 Then Exit Sub

        If _Queries(gvQueries.SelectedRows(0).Index) IsNot _SelectedQuery OrElse _Queries(gvQueries.SelectedRows(0).Index).Criteria IsNot _TimeSeries Then
            _SelectedQuery = _Queries(gvQueries.SelectedRows(0).Index)

            pgQueryProperties.SelectedObject = _SelectedQuery
            cbQueryType.SelectedIndex = 0

            btnAddQuery.Enabled = False
            btnRemoveQuery.Enabled = True
            btnQueryReset.Enabled = True

            RaiseEvent SelectedQueryChanged(Me, New TimeSeriesQueryEventArgs(_SelectedQuery))
        End If

    End Sub

    Private Sub gvQueries_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles gvQueries.CellContentClick

        If e.RowIndex = -1 Then Exit Sub

        Select Case gvQueries.Columns(e.ColumnIndex).Name
            Case "LabelColor" ' Color

                If dlgLabelColor.ShowDialog(Me) = DialogResult.OK AndAlso dlgLabelColor.Color <> _Labels(_Queries(e.RowIndex)) Then
                    Dim label As Color = dlgLabelColor.Color
                    gvQueries.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.BackColor = label
                    gvQueries.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.SelectionBackColor = label

                    _Labels(_Queries(e.RowIndex)) = label

                    If _Logger IsNot Nothing Then
                        _Logger.RefreshResults()
                    End If

                    Dim enabled As Boolean = CBool(gvQueries.Rows(e.RowIndex).Cells(1).Value)
                    If enabled Then
                        _ActiveLabels(_Queries(e.RowIndex)) = label
                        RaiseEvent QueryLabelChanged(Me, New TimeSeriesQueryEventArgs(_SelectedQuery))
                    End If

                End If

            Case "QueryEnabled" ' Enable/Disable query...

                gvQueries.CommitEdit(DataGridViewDataErrorContexts.Commit)

                Dim run As Boolean = CBool(gvQueries.Rows(e.RowIndex).Cells(e.ColumnIndex).Value)
                Dim tsq As TimeSeriesQuery = _Queries(e.RowIndex)

                _SelectedQuery = tsq
                pgQueryProperties.SelectedObject = _SelectedQuery

                If run Then
                    _ActiveQueries.Add(tsq)
                    If Not _ActiveLabels.ContainsKey(tsq) Then
                        _ActiveLabels.Add(tsq, _Labels(tsq))
                    End If
                    RaiseEvent QueryChanged(Me, New TimeSeriesQueryEventArgs(_SelectedQuery))
                Else
                    _ActiveQueries.Remove(tsq)
                    _ActiveLabels.Remove(tsq)
                    RaiseEvent QueryLabelChanged(Me, New TimeSeriesQueryEventArgs(_SelectedQuery)) ' HACK: so as to not execute all the queries again.
                End If

            Case "LogQuery" ' Logging On/Off...

                Dim log As Boolean = Not CBool(gvQueries.Rows(e.RowIndex).Cells(e.ColumnIndex).Value)
                gvQueries.CommitEdit(DataGridViewDataErrorContexts.Commit)

                _SelectedQuery = _Queries(e.RowIndex)
                pgQueryProperties.SelectedObject = _Queries(e.RowIndex)

                If log Then
                    ShowResultsLogger()
                    _Actions(_SelectedQuery) = New Action(Of TimeSeriesQueryResult)(AddressOf _Logger.Log)
                Else
                    _Actions(_SelectedQuery) = _NullAction
                End If

            Case "Name" ' Name

            Case "UpdateCriteria"

                If MsgBox("Are you sure you want to update the criteria for the selected query?", MsgBoxStyle.YesNo, "Confirm...") = MsgBoxResult.Yes Then
                    _SelectedQuery.Criteria = _TimeSeries
                    RaiseEvent QueryChanged(Me, New TimeSeriesQueryEventArgs(_SelectedQuery))
                End If

            Case "ViewCriteria"

                Dim viewer As New FormTimeSeriesViewer
                With viewer
                    .ViewControl.SelectedTimeSeries = _Queries(e.RowIndex).Criteria.Clone
                    '.ViewControl.Pallet = _AutoPallet
                    '.ViewControl.PalletThreshold = _SelectedPalletThreshold
                    '.ViewControl.ShowCentroids = ShowCentroidsToolStripMenuItem.Checked
                    .Text = .Text & " Query Id: " & _SelectedQuery.Id
                    .Show()
                End With

        End Select

    End Sub

    Private Sub gvQueries_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs) Handles gvQueries.CellPainting
        If gvQueries.Columns(e.ColumnIndex).Name = "UpdateCriteria" AndAlso e.RowIndex >= 0 Then
            e.Paint(e.CellBounds, DataGridViewPaintParts.All)
            Dim X As Integer = ((e.CellBounds.Width \ 2) - (My.Resources.wand.Width \ 2)) + e.CellBounds.X
            Dim Y As Integer = (((e.CellBounds.Height \ 2) - (My.Resources.wand.Height \ 2)) + e.CellBounds.Y) - 1
            e.Graphics.DrawImage(My.Resources.wand, X, Y)
            e.Handled = True
        ElseIf gvQueries.Columns(e.ColumnIndex).Name = "ViewCriteria" AndAlso e.RowIndex >= 0 Then
            e.Paint(e.CellBounds, DataGridViewPaintParts.All)
            Dim X As Integer = ((e.CellBounds.Width \ 2) - (My.Resources.wand.Width \ 2)) + e.CellBounds.X
            Dim Y As Integer = (((e.CellBounds.Height \ 2) - (My.Resources.wand.Height \ 2)) + e.CellBounds.Y) - 1
            e.Graphics.DrawImage(My.Resources.eye, X, Y)
            e.Handled = True
        End If
    End Sub

    Private Sub pgQueryProperties_PropertyValueChanged(s As Object, e As PropertyValueChangedEventArgs) Handles pgQueryProperties.PropertyValueChanged

        Dim tsq As TimeSeriesQuery = DirectCast(pgQueryProperties.SelectedObject, TimeSeriesQuery)
        If _ActiveLabels.ContainsKey(tsq) Then

            gvQueries.CommitEdit(DataGridViewDataErrorContexts.Commit)

            Select Case e.ChangedItem.Label
                Case "Label"
                    RaiseEvent QueryLabelChanged(Me, New TimeSeriesQueryEventArgs(tsq)) ' TODO: Is this needed?
                Case "Id"
                    If _Logger IsNot Nothing Then
                        _Logger.RefreshResults()
                    End If
                Case Else
                    RaiseEvent QueryChanged(Me, New TimeSeriesQueryEventArgs(tsq))
            End Select

        End If

        pgQueryProperties.Refresh()

    End Sub

    Private Sub btnOpenQuery_Click(sender As Object, e As EventArgs) Handles btnOpenQuery.Click

        If ofdTimeSeriesQuery.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then

            Try
                Dim QueryPath As String = ofdTimeSeriesQuery.FileName
                _SelectedQuery = TimeSeriesQuery.Load(QueryPath)
                AddSelectedQuery(String.Format("{0} [{1}]", _SelectedQuery.Name, System.IO.Path.GetFileName(QueryPath)))
                RaiseEvent SelectedQueryChanged(Me, New TimeSeriesQueryEventArgs(_SelectedQuery))
            Catch ex As Exception
                ShowException(ex)
            End Try

        Else
            Exit Sub
        End If

    End Sub

    Private Sub btnSaveQuery_Click(sender As Object, e As EventArgs) Handles btnSaveQuery.Click
        If sfdTimeSeriesQuery.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            Dim QueryPath As String = sfdTimeSeriesQuery.FileName
            _SelectedQuery.Save(QueryPath)
        Else
            Exit Sub
        End If
    End Sub

    Private Sub btnShowResults_Click(sender As Object, e As EventArgs) Handles btnShowResults.Click
        ShowResultsLogger()
    End Sub

#End Region

    Private Sub Query_VersionChanged(sender As Object, e As EventArgs)
        pgQueryProperties.Refresh()
    End Sub

    Private _ExViewer As FormExceptionViewer
    Private Sub ShowException(ex As Exception)
        If _ExViewer Is Nothing Then _ExViewer = New FormExceptionViewer
        _ExViewer.Show(ex)
    End Sub

End Class
