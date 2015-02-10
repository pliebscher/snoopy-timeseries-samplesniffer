Public Class FormExceptionViewer

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()

    End Sub

    Public Overloads Sub Show(owner As Form, message As String, ex As Exception)

        txtMessage.Text = message
        txtExceptionDetail.Text = ex.ToString
        Me.ShowDialog(owner)

    End Sub

    Public Overloads Sub Show(owner As Form, ex As Exception)
        Show(owner, ex.Message, ex)
    End Sub

    Public Overloads Sub Show(ex As Exception)
        txtMessage.Text = ex.Message
        txtExceptionDetail.Text = ex.ToString
        Me.ShowDialog()
    End Sub

    Private Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click
        txtExceptionDetail.SelectAll()
        txtExceptionDetail.Copy()
    End Sub

End Class