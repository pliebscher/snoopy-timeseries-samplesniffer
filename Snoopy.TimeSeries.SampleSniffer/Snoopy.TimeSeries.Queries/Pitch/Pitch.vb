Imports System.Runtime.InteropServices

<StructLayout(LayoutKind.Sequential)>
Public Structure Pitch

    Public Shared ReadOnly Empty As New Pitch(-1, 1)

    Private _Hertz As Double
    Private _Probability As Double

    Public Sub New(hertz As Double, probability As Double)
        _Hertz = hertz
        _Probability = probability
    End Sub

    Public ReadOnly Property Hertz As Double
        Get
            Return _Hertz
        End Get
    End Property

    Public ReadOnly Property Probability As Double
        Get
            Return _Probability
        End Get
    End Property

End Structure
