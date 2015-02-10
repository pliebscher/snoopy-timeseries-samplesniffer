Public Class WB256
    Implements IColorPallet

    Public Shared ReadOnly Pallet As New WB256

    Private _Colors As Color() = New Color(255) {}

    Public Sub New()

        Dim c As Integer = 255
        For i As Integer = 0 To 255
            _Colors(i) = Color.FromArgb(c, c, c)
            c -= 1
        Next

    End Sub

    Public ReadOnly Property Colors As Color() Implements IColorPallet.Colors
        Get
            Return _Colors
        End Get
    End Property

End Class
