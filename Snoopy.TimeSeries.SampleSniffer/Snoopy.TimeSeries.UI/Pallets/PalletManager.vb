Public Class PalletManager

    Private Shared _Pallets As New Dictionary(Of String, IColorPallet)

    Shared Sub New()
        _Pallets.Add(GetType(JET).Name, JET.Pallet)
        _Pallets.Add(GetType(RGB256).Name, RGB256.Pallet)
        _Pallets.Add(GetType(BGR256).Name, BGR256.Pallet)
        _Pallets.Add(GetType(BW256).Name, BW256.Pallet)
        _Pallets.Add(GetType(WB256).Name, WB256.Pallet)
        _Pallets.Add(GetType(BRY256).Name, BRY256.Pallet)
        _Pallets.Add(GetType(G256).Name, G256.Pallet)
        _Pallets.Add(GetType(G256R).Name, G256R.Pallet)
        _Pallets.Add(GetType(HS1V1).Name, HS1V1.Pallet)
        _Pallets.Add(GetType(Rainbow).Name, Rainbow.Pallet)
    End Sub

    Public Shared ReadOnly Property Pallets() As Dictionary(Of String, IColorPallet)
        Get
            Return _Pallets
        End Get
    End Property

    Public Shared ReadOnly Property DefaultPallet As IColorPallet
        Get
            Return JET.Pallet
        End Get
    End Property

End Class
