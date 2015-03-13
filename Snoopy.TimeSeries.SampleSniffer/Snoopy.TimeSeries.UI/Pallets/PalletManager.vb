Public Class PalettetManager

    Public Event PalletThresholdChanged(sender As Object, e As EventArgs)
    Public Event SelectedPalletChanged(sender As Object, e As EventArgs)

    Private Shared ReadOnly _Instance As New PalettetManager
    Private _Palettes As New Dictionary(Of String, IColorPallet)
    Private _SelectedPalette As IColorPallet = JET.Pallet
    Private _PaletteThreshold As Integer = 75

    Sub New()
        _Palettes.Add(GetType(JET).Name, JET.Pallet)
        _Palettes.Add(GetType(RGB256).Name, RGB256.Pallet)
        _Palettes.Add(GetType(BGR256).Name, BGR256.Pallet)
        _Palettes.Add(GetType(BW256).Name, BW256.Pallet)
        _Palettes.Add(GetType(WB256).Name, WB256.Pallet)
        _Palettes.Add(GetType(BRY256).Name, BRY256.Pallet)
        _Palettes.Add(GetType(G256).Name, G256.Pallet)
        _Palettes.Add(GetType(G256R).Name, G256R.Pallet)
        _Palettes.Add(GetType(HS1V1).Name, HS1V1.Pallet)
        _Palettes.Add(GetType(Rainbow).Name, Rainbow.Pallet)
    End Sub

    Public ReadOnly Property Palettes() As Dictionary(Of String, IColorPallet)
        Get
            Return _Palettes
        End Get
    End Property

    Public Property SelectedPalette As IColorPallet
        Get
            Return _SelectedPalette
        End Get
        Set(value As IColorPallet)
            _SelectedPalette = value
            RaiseEvent SelectedPalletChanged(Me, EventArgs.Empty)
        End Set
    End Property

    Public Property PaletteThreshold As Integer
        Get
            Return _PaletteThreshold
        End Get
        Set(value As Integer)
            If value <> _PaletteThreshold Then
                _PaletteThreshold = value
                RaiseEvent PalletThresholdChanged(Me, EventArgs.Empty)
            End If
        End Set
    End Property

    Public Shared ReadOnly Property Instance As PalettetManager
        Get
            Return _Instance
        End Get
    End Property

End Class
