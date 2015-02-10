Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Diagnostics
' http://code.google.com/p/kanzi/source/browse/java/src/kanzi/util/MurMurHash3.java
Namespace Hashing
    Class MurMurHash3

        Private Shared _C1 As Integer = &HCC9E2D51 'UI
        Private Shared _C2 As Integer = &H1B873593
        Private Shared _C3 As Integer = &HE6546B64 'UI
        Private Shared _C4 As Integer = &H85EBCA6B 'UI
        Private Shared _C5 As Integer = &HC2B2AE35 'UI

        Private Shared _Rand As New Random()
        Private _Seed As Integer

        Public Sub New()
            Me.New(_Rand.Next(255))
            'Me.New(CInt(Date.Now.Ticks And &HFFFFFFFFL))
        End Sub

        Public Sub New(seed As Integer)
            Me._Seed = seed
        End Sub

        Public Function Hash(data As Integer()) As Integer
            Return Me.Hash(data, 0, data.Length)
        End Function

        Public Function Hash(data As Integer(), offset As Integer, len As Integer) As Integer
            Dim h1 As Integer = Me._Seed
            ' aliasing
            Dim end4 As Integer = offset + (len And -4)
            Dim k1 As UInteger ' UInteger

            For i As Integer = offset To end4 - 1 Step 4

                k1 = CUInt((data(i) And &HFF) Or ((data(i + 1) And &HFF) << 8) Or ((data(i + 2) And &HFF) << 16) Or ((data(i + 3) And &HFF) << 24))
                k1 = CUInt(k1 * _C1)
                k1 = (k1 << 15) Or (k1 >> 17)
                k1 = CUInt(k1 * _C2)
                h1 = CInt(h1 Xor k1)
                h1 = (h1 << 13) Or (h1 >> 19)
                h1 = (h1 * 5) + _C3

            Next

            ' Tail
            k1 = 0

            Select Case len And 3
                Case 3
                    k1 = CUInt(((data(end4 + 2) And &HFF) << 16))

                Case 2
                    k1 = CUInt(((data(end4 + 2) And &HFF) << 16))

                    k1 = CUInt(k1 Or ((data(end4 + 1) And &HFF) << 8))

                Case 1
                    k1 = CUInt(((data(end4 + 2) And &HFF) << 16))
                    k1 = CUInt(k1 Or ((data(end4 + 1) And &HFF) << 8))

                    k1 = CUInt(k1 Or (data(end4) And &HFF))
                    k1 = CUInt(k1 * _C1)
                    k1 = (k1 << 15) Or (k1 >> 17)
                    k1 = CUInt(k1 * _C2)
                    h1 = CInt(h1 Xor k1)

            End Select

            ' Finalization
            h1 = h1 Xor len
            h1 = h1 Xor (h1 >> 16)
            h1 *= _C4
            h1 = h1 Xor (h1 >> 13)
            h1 *= _C5
            Return h1 Xor (h1 >> 16)

        End Function

        Public Property Seed As Integer
            Get
                Return _Seed
            End Get
            Set(value As Integer)
                _Seed = value
            End Set
        End Property

    End Class
End Namespace