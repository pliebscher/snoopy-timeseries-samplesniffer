Public Class HS1V1
    Implements IColorPallet

    Public Shared ReadOnly Pallet As New HS1V1

    Private _Steps As Integer = 255
    Private _Colors As Color() = New Color(_Steps - 1) {}

    Public Sub New()

        For i As Integer = 0 To _Steps - 1
            _Colors(i) = ColorPallet.HSV2RGB(CSng(i * 1.5 * Math.PI) / CSng(_Steps), 1, _Steps - i)
        Next

        _Colors = _Colors.Reverse().ToArray

    End Sub

    Public ReadOnly Property Colors As Color() Implements IColorPallet.Colors
        Get
            Return _Colors
        End Get
    End Property

End Class
