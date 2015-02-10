Public Class BGR256
    Implements IColorPallet

    Public Shared ReadOnly Pallet As New BGR256

    Private _LevelPaletteRange As Integer = 256
    Private _Colors As Color() = New Color(_LevelPaletteRange - 1) {}

    Public Sub New()

        Dim c As Integer = _LevelPaletteRange '- 1
        For i As Integer = 0 To _LevelPaletteRange - 1
            _Colors(i) = GetColor(c, _LevelPaletteRange)
            c -= 1
        Next

    End Sub

    Public ReadOnly Property Colors As Color() Implements IColorPallet.Colors
        Get
            Return _Colors
        End Get
    End Property

    Public Shared Function GetColor(ByVal value As Double, ByVal range As Integer) As Color
        If Double.IsNaN(value) Then Return Color.Black
        If range = 0 OrElse value = 0.0 Then Return Color.Black

        Dim R As Double
        Dim G As Double
        Dim B As Double
        Dim R4 As Double
        Dim U As Double

        If value > range Then value = range

        R4 = range / 4
        U = 255

        If value < R4 Then  ' B
            B = value / R4
            G = 0
            R = 0
        ElseIf value < 2 * R4 Then  ' B, G
            B = (1 - (value - R4) / R4)
            G = 1 - B
            R = 0
        ElseIf value < 3 * R4 Then  ' G, R
            B = 0
            G = (2 - (value - R4) / R4)
            R = 1 - G
        Else                             ' B, R
            B = (value - 3 * R4) / R4
            G = 0
            R = 1 - B
        End If

        R = (Math.Sqrt(R) * U) 'And &HFF
        G = (Math.Sqrt(G) * U) 'And &HFF
        B = (Math.Sqrt(B) * U) 'And &HFF

        Return Color.FromArgb(255, CInt(R) And &HFF, (CInt(G) And &HFF), (CInt(B) And &HFF))

    End Function

End Class
