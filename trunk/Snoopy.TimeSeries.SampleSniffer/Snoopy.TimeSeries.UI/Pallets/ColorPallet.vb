''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class ColorPallet

    Public Shared Function getHSVPalette(steps As Integer) As Color()
        Dim ergebnis As Color() = New Color(steps - 1) {}
        For i As Integer = 0 To steps - 1
            ergebnis(i) = HSV2RGB(CInt(i * 2 * Math.PI) \ CInt(steps), 1, 1)
        Next
        Return ergebnis
    End Function

    Public Shared Function HSV2RGB(h As Single, s As Single, v As Single) As Color
        Dim h_i As Single = CSng(Math.Floor((h / Math.PI * 3.0)))
        Dim f As Single = CSng(h / Math.PI * 3D - h_i)
        Dim p As Single = v * (1 - s)
        Dim q As Single = (v * (1 - s * f))
        Dim t As Single = (v * (1 - s * (1 - f)))

        Dim _q As Integer = CInt(q)
        Dim _v As Integer = CInt(v)
        Dim _p As Integer = CInt(p)
        Dim _t As Integer = CInt(t)

        Select Case CInt(Math.Truncate(h_i)) Mod 6
            Case 1
                Return Color.FromArgb(_q, _v, _p)
            Case 2
                Return Color.FromArgb(_p, _v, _t)
            Case 3
                Return Color.FromArgb(_p, _q, _v)
            Case 4
                Return Color.FromArgb(_t, _p, _v)
            Case 5
                Return Color.FromArgb(_v, _p, _q)
            Case Else
                Return Color.FromArgb(_v, _t, _p)
        End Select
    End Function

End Class
