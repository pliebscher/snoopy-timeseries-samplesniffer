Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Module Extensions

    Const SB_HORZ As Integer = 0

    <Extension>
    Public Sub ShowScrollBar(control As Control, show As Boolean)
        ShowScrollBar(control.Handle, SB_HORZ, show)
    End Sub

    <DllImport("user32.dll")> _
    Private Function ShowScrollBar(hWnd As IntPtr, wBar As Integer, bShow As Boolean) As Boolean
    End Function

    <Extension>
    Public Sub MoveItems(listView As ListView, processors As List(Of TimeSeriesProcessor), direction As MoveDirection)
        If listView.SelectedItems.Count > 0 Then
            Dim dir As Integer = CInt(direction)
            listView.SuspendLayout()
            Dim currentIndex As Integer = listView.SelectedItems(0).Index
            If currentIndex + dir = -1 Or currentIndex + dir = listView.Items.Count Then Exit Sub
            Dim item As ListViewItem = listView.Items(currentIndex)

            If currentIndex >= 0 AndAlso currentIndex < listView.Items.Count Then
                '_RaiseChangedEvent = False
                listView.Items.RemoveAt(currentIndex) ' for some reason the RemoveAt and Insert cause the ItemChecked event to be raised.
                listView.Items.Insert(currentIndex + dir, item)

                Dim processor As TimeSeriesProcessor = processors(currentIndex)
                processors.RemoveAt(currentIndex)
                processors.Insert(currentIndex + dir, processor) ' Use .Reverse(currentIndex + dir, 2) ???????

                '_RaiseChangedEvent = True
            End If
            listView.ResumeLayout()
            'If item.Checked Then RaiseEvent Changed(Me, EventArgs.Empty)
        End If
    End Sub

    Public Enum MoveDirection
        Up = -1
        Down = 1
    End Enum

End Module
