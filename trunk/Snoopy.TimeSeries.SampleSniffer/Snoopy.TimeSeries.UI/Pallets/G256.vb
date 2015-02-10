Public Class G256
    Implements IColorPallet

    Public Shared ReadOnly Pallet As New G256

    Private _Colors As Color() = New Color(255) {}

    Public Sub New()
        For i As Integer = 0 To 255
            _Colors(i) = Color.FromArgb(0, i, 0)
        Next
    End Sub

    Public ReadOnly Property Colors As Color() Implements IColorPallet.Colors
        Get
            Return _Colors
        End Get
    End Property

End Class
