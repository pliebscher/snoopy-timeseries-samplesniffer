Imports System.IO
Public Class FormQueryMatchLogger

    Private _MaxResults As Integer = 1000
    Private _Matches As New List(Of TimeSeriesQueryResult)
    Private _MatcheCounts As New Dictionary(Of TimeSeriesQuery, Integer)
    Private _Labels As Dictionary(Of TimeSeriesQuery, Color)

    Public Sub New(labels As Dictionary(Of TimeSeriesQuery, Color))
        InitializeComponent()

        _Labels = labels

        For Each tsq As TimeSeriesQuery In _Labels.Keys
            _MatcheCounts.Add(tsq, 0)
        Next

    End Sub

    Public Sub Log(result As TimeSeriesQueryResult)

        If _Matches.Contains(result, TimeSeriesQueryResultComparer.Instance) Then
            Exit Sub
        End If

        If InvokeRequired Then
            Invoke(Sub() Log(result))
        Else
            If Not _MatcheCounts.Keys.Contains(result.Query) Then
                _MatcheCounts.Add(result.Query, 0)
            End If

            If _Matches.Count >= _MaxResults Then
                nudMaxResults.BackColor = Color.Red
                Exit Sub
            End If

            _Matches.Add(result)
            _MatcheCounts(result.Query) += 1

            lblResultCount.Text = _Matches.Count.ToString

            dgvMatches.Rows.Add(New Object() {_MatcheCounts(result.Query), String.Format("{0} (v.{1})", result.Query.Id, result.Query.Version), result.Data.TimeStamp, result.Score})
            dgvMatches.Rows(dgvMatches.Rows.Count - 1).Cells(0).Style.BackColor = _Labels(result.Query)
            dgvMatches.Rows(dgvMatches.Rows.Count - 1).Cells(0).Style.SelectionBackColor = _Labels(result.Query)
            dgvMatches.Rows(_Matches.Count - 1).Selected = True
            dgvMatches.FirstDisplayedScrollingRowIndex = dgvMatches.RowCount - 1

            tsViewer.SelectedTimeSeries = result.Data

        End If

    End Sub

    Public Sub RefreshResults()

        Dim selectedIndex As Integer = -1
        If dgvMatches.SelectedRows.Count = 1 Then
            selectedIndex = dgvMatches.SelectedRows(0).Index
        End If

        dgvMatches.Rows.Clear()
        tsViewer.SelectedTimeSeries = Nothing

        For i As Integer = 0 To _Matches.Count - 1

            Dim result As TimeSeriesQueryResult = _Matches(i)

            dgvMatches.Rows.Add(New Object() {_MatcheCounts(result.Query), String.Format("{0} (v.{1})", result.Query.Id, result.Query.Version), result.Data.TimeStamp, result.Score})
            dgvMatches.Rows(dgvMatches.Rows.Count - 1).Cells(0).Style.BackColor = _Labels(result.Query)
            dgvMatches.Rows(dgvMatches.Rows.Count - 1).Cells(0).Style.SelectionBackColor = _Labels(result.Query)
        Next

        If selectedIndex > -1 Then
            dgvMatches.Rows(selectedIndex).Selected = True
            dgvMatches.FirstDisplayedScrollingRowIndex = selectedIndex
            tsViewer.SelectedTimeSeries = _Matches(selectedIndex).Data
        ElseIf dgvMatches.RowCount > 0 Then
            dgvMatches.FirstDisplayedScrollingRowIndex = dgvMatches.RowCount - 1
        End If

    End Sub

#Region " -- Form -- "

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        _Matches.Clear()
        _MatcheCounts.Clear()
        dgvMatches.Rows.Clear()
        tsViewer.SelectedTimeSeries = Nothing
        nudMaxResults.BackColor = Color.White
        lblResultCount.Text = "0"
    End Sub

    Private Sub btnHide_Click(sender As Object, e As EventArgs) Handles btnHide.Click
        Me.Hide()
    End Sub

    Private Sub dgvMatches_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvMatches.KeyDown
        If dgvMatches.SelectedRows.Count = 1 Then
            Dim match As TimeSeriesQueryResult = _Matches(dgvMatches.SelectedRows(0).Index)
            tsViewer.SelectedTimeSeries = match.Data
        End If
    End Sub

    Private Sub dgvMatches_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgvMatches.RowEnter
        If dgvMatches.SelectedRows.Count = 1 Then
            Dim match As TimeSeriesQueryResult = _Matches(dgvMatches.SelectedRows(0).Index)
            tsViewer.SelectedTimeSeries = match.Data
        End If
    End Sub

    Private Sub nudMaxResults_ValueChanged(sender As Object, e As EventArgs) Handles nudMaxResults.ValueChanged
        _MaxResults = CInt(nudMaxResults.Value)
        If _MaxResults < _Matches.Count - 1 Then
            nudMaxResults.BackColor = Color.Red
        Else
            nudMaxResults.BackColor = Color.White
        End If
    End Sub

    Private Sub FormQueryMatchLogger_Load(sender As Object, e As EventArgs) Handles Me.Load
        nudMaxResults.Value = _MaxResults
    End Sub

#End Region

    Public Sub writeCSV(gridIn As DataGridView, outputFile As String)
        'test to see if the DataGridView has any rows
        If gridIn.RowCount > 0 Then
            Dim value As String = ""
            Dim dr As New DataGridViewRow()
            Dim swOut As New StreamWriter(outputFile)

            'write header rows to csv
            For i As Integer = 0 To gridIn.Columns.Count - 1
                If i > 0 Then
                    swOut.Write(",")
                End If
                swOut.Write(gridIn.Columns(i).HeaderText)
            Next

            swOut.WriteLine()

            'write DataGridView rows to csv
            For j As Integer = 0 To gridIn.Rows.Count - 1
                If j > 0 Then
                    swOut.WriteLine()
                End If

                dr = gridIn.Rows(j)

                For i As Integer = 0 To gridIn.Columns.Count - 1
                    If i > 0 Then
                        swOut.Write(",")
                    End If

                    value = dr.Cells(i).Value.ToString()
                    'replace comma's with spaces
                    value = value.Replace(","c, " "c)
                    'replace embedded newlines with spaces
                    value = value.Replace(Environment.NewLine, " ")

                    swOut.Write(value)
                Next
            Next
            swOut.Close()
        End If
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        If sfdResults.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            Dim path As String = sfdResults.FileName
            writeCSV(dgvMatches, path)
        End If
    End Sub

    Private Sub cmsResults_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles cmsResults.Opening
        SaveToolStripMenuItem.Enabled = dgvMatches.RowCount > 0
    End Sub

End Class