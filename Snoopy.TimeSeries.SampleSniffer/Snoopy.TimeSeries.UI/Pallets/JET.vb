''' <summary>
''' https://github.com/dhale/jtk/blob/master/src/main/java/edu/mines/jtk/awt/ColorMap.java
''' </summary>
''' <remarks></remarks>
Public Class JET
    Implements IColorPallet

    Public Shared ReadOnly Pallet As New JET

    Private _LevelPaletteRange As Integer = 256
    Private _Colors As Color() = New Color(_LevelPaletteRange - 1) {}

    Public Sub New()
        GetJet()
        'GetSunset()
    End Sub

    Public ReadOnly Property Colors As Color() Implements IColorPallet.Colors
        Get
            Return _Colors
        End Get
    End Property

    Private Sub GetJet()
        For i As Integer = 0 To 255
            Dim x As Double = i / 255.0
            If x < 0.125F Then
                ' 0.0, 0.0, 0.5:1.0
                Dim y As Double = (x / 0.125) * 254
                _Colors(i) = Color.FromArgb(0, 0, CInt(y))
            ElseIf x < 0.375F Then
                ' 0.0, 0.0:1.0, 1.0
                Dim y As Double = ((x - 0.125) / 0.25) * 254
                _Colors(i) = Color.FromArgb(0, CInt(y), 255)
            ElseIf x < 0.625F Then
                ' 0.0:1.0, 1.0, 1.0:0.0
                Dim y As Double = ((x - 0.375) / 0.25) * 254
                _Colors(i) = Color.FromArgb(CInt(y), 255, CInt(255 - y))
            ElseIf x < 0.875F Then
                ' 1.0, 1.0:0.0, 0.0
                Dim y As Double = (1 - (x - 0.625) / 0.25) * 254
                _Colors(i) = Color.FromArgb(255, CInt(y), 0)
            Else
                ' 1.0:0.5, 0.0, 0.0
                Dim y As Double = (1 - (x - 0.875) / 0.125) * 254
                _Colors(i) = Color.FromArgb(CInt(y), 0, 0)
            End If
        Next
    End Sub

    Private Sub GetSunset()

        For i As Integer = 0 To 255

            Dim norm As Double = i / 255.0

            Dim r As Double = Math.Abs(((norm - 0.24) * 2.38)) * 255
            If (r > 255) Then r = 255
            If (r < 0) Then r = 0

            Dim g As Double = Math.Abs(((norm - 0.64) * 2.777)) * 255
            If (g > 255) Then g = 255
            If (g < 0) Then g = 0

            Dim b As Double = Math.Abs(((3.6 * norm))) * 255
            If (norm > 2.77) Then b = 2.0 - b * 255
            If (b > 255) Then b = 255
            If (b < 0) Then b = 0

            _Colors(i) = Color.FromArgb(CInt(r), CInt(g), CInt(b))

        Next


    End Sub

End Class
