﻿Public Class BRY256
    Implements IColorPallet

    Public Shared ReadOnly Pallet As New BRY256

    Private _Colors As Color() = New Color(255) {}

    Private _Blue2Red2Yellow As Integer() = New Integer() {
            &HFF030000, &HFF070000, &HFF0A0000, &HFF0D0000, &HFF100000, &HFF140000, &HFF190000, &HFF1E0000,
            &HFF220000, &HFF240000, &HFF270000, &HFF2B0000, &HFF2E0000, &HFF300000, &HFF340101, &HFF370101,
            &HFF3A0101, &HFF3D0101, &HFF410101, &HFF440101, &HFF470101, &HFF4A0101, &HFF4D0101, &HFF510101,
            &HFF520101, &HFF550101, &HFF590101, &HFF5C0000, &HFF5F0000, &HFF620000, &HFF660000, &HFF690000,
            &HFF6B0000, &HFF6F0000, &HFF720000, &HFF760000, &HFF7A0000, &HFF7E0000, &HFF820000, &HFF870000,
            &HFF8B0101, &HFF8F0000, &HFF940000, &HFF9A0000, &HFF9E0000, &HFFA20000, &HFFA80000, &HFFAD0000,
            &HFFB20000, &HFFB60000, &HFFBF0000, &HFFC40000, &HFFC70000, &HFFCB0000, &HFFD00000, &HFFD50000,
            &HFFD90000, &HFFDE0000, &HFFE20000, &HFFE70000, &HFFEA0000, &HFFEF0000, &HFFF30000, &HFFF70000,
            &HFFFD0000, &HFFFF0002, &HFFF70008, &HFFF1000E, &HFFEB001A, &HFFE40023, &HFFDE002B, &HFFD80030,
            &HFFD20036, &HFFCD003E, &HFFC80044, &HFFC2004D, &HFFBD0055, &HFFBA005C, &HFFB80061, &HFFB40067,
            &HFFB0006A, &HFFAD0071, &HFFA80077, &HFFA3007D, &HFF9D0083, &HFF990089, &HFF94008D, &HFF8F0094,
            &HFF8C0098, &HFF87009C, &HFF8300A0, &HFF7F00A4, &HFF7B00A8, &HFF7400AA, &HFF7100AE, &HFF6E00B1,
            &HFF6C00B4, &HFF6A00B7, &HFF6701BB, &HFF6401BE, &HFF6001C1, &HFF5C01C5, &HFF5700C8, &HFF5001CA,
            &HFF4C01CE, &HFF4501D1, &HFF4001D4, &HFF3B01D6, &HFF3701DA, &HFF3000DE, &HFF2B00E1, &HFF2700E2,
            &HFF2300E6, &HFF2000E8, &HFF1C00EA, &HFF1901EC, &HFF1601EE, &HFF1201EF, &HFF0E01F0, &HFF0C01F2,
            &HFF0901F3, &HFF0601F5, &HFF0302F6, &HFF0104F7, &HFF0007F9, &HFF020AFB, &HFF000EFD, &HFF0010FF,
            &HFF0014FF, &HFF0018FF, &HFF011BFF, &HFF011FFF, &HFF0123FF, &HFF0026FF, &HFF0028FF, &HFF002BFF,
            &HFF012FFF, &HFF0131FF, &HFF0135FF, &HFF0138FF, &HFF013BFF, &HFF013EFF, &HFF0141FF, &HFF0145FF,
            &HFF014AFF, &HFF014DFF, &HFF0150FF, &HFF0154FF, &HFF0157FF, &HFF015AFF, &HFF015FFF, &HFF0166FF,
            &HFF016AFF, &HFF016DFF, &HFF0172FF, &HFF0174FF, &HFF0079FF, &HFF007DFF, &HFF0082FF, &HFF0084FE,
            &HFF0087FE, &HFF018BFD, &HFF008FFE, &HFF0093FE, &HFF0097FE, &HFF009AFE, &HFF009DFE, &HFF00A1FE,
            &HFF00A5FE, &HFF00A8FE, &HFF00ACFE, &HFF00AFFE, &HFF00B2FE, &HFF00B9FE, &HFF00BCFE, &HFF00C1FF,
            &HFF00C6FF, &HFF00C9FF, &HFF00CEFF, &HFF00D2FF, &HFF00D6FF, &HFF00D9FF, &HFF00DDFF, &HFF00E1FF,
            &HFF01E5FF, &HFF00E7FF, &HFF00EAFF, &HFF00EEFF, &HFF00EFFF, &HFF00F2FF, &HFF00F6FF, &HFF00F8FF,
            &HFF00FAFF, &HFF00FDFF, &HFF02FFFF, &HFF04FFFF, &HFF09FFFF, &HFF0CFFFF, &HFF12FFFF, &HFF15FFFF,
            &HFF19FFFF, &HFF1EFFFF, &HFF22FFFF, &HFF26FFFF, &HFF29FFFF, &HFF2DFFFE, &HFF30FFFE, &HFF35FFFE,
            &HFF3AFFFE, &HFF3EFFFF, &HFF41FFFF, &HFF44FFFF, &HFF48FFFF, &HFF50FFFE, &HFF53FFFE, &HFF56FFFE,
            &HFF5BFFFF, &HFF5FFFFE, &HFF63FFFE, &HFF67FFFE, &HFF6BFFFE, &HFF71FFFE, &HFF75FFFE, &HFF7BFFFE,
            &HFF81FFFE, &HFF86FFFE, &HFF89FFFE, &HFF8EFFFE, &HFF93FFFE, &HFF96FFFF, &HFF9CFFFF, &HFFA2FFFF,
            &HFFA8FFFF, &HFFADFFFF, &HFFB5FFFF, &HFFBAFFFF, &HFFBEFFFF, &HFFC1FFFF, &HFFC4FFFF, &HFFC8FFFF,
            &HFFCAFFFF, &HFFCDFFFF, &HFFD0FFFF, &HFFD5FFFF, &HFFD8FFFF, &HFFDBFFFF, &HFFDEFFFF, &HFFE1FFFF,
            &HFFE4FFFF, &HFFE8FFFF, &HFFEAFFFF, &HFFEDFFFF, &HFFF2FFFF, &HFFF8FFFF, &HFFFDFFFF, &HFFFFFFFF}

    Public Sub New()

        Dim j As Integer = 0
        For i As Integer = 0 To _Blue2Red2Yellow.Length - 1
            _Colors(j) = Color.FromArgb(_Blue2Red2Yellow(i))
            j += 1
        Next

    End Sub

    Public ReadOnly Property Colors As Color() Implements IColorPallet.Colors
        Get
            Return _Colors
        End Get
    End Property

End Class