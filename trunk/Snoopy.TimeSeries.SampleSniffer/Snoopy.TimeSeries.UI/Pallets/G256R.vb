Public Class G256R
    Implements IColorPallet

    Public Shared ReadOnly Pallet As New G256R

    Private _Colors As Color() = New Color(255) {}

    Public Sub New()

        Dim c As Integer = 255
        For i As Integer = 0 To 255
            _Colors(i) = Color.FromArgb(0, c, 0)
            c -= 1
        Next

    End Sub

    Public ReadOnly Property Colors As Color() Implements IColorPallet.Colors
        Get
            Return _Colors
        End Get
    End Property

End Class
