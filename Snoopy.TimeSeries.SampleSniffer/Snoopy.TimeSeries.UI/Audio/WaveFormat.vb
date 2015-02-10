Imports System.Runtime.InteropServices

<StructLayout(LayoutKind.Sequential)> _
Public Class WaveFormat
    Public wFormatTag As Short
    Public nChannels As Short
    Public nSamplesPerSec As Integer
    Public nAvgBytesPerSec As Integer
    Public nBlockAlign As Short
    Public wBitsPerSample As Short
    Public cbSize As Short

    Public Sub New(ByVal rate As Integer, ByVal bits As Integer, ByVal channels As Integer)
        wFormatTag = CShort(WaveFormats.Pcm)
        nChannels = CShort(channels)
        nSamplesPerSec = rate
        wBitsPerSample = CShort(bits)
        cbSize = 0

        nBlockAlign = CShort(channels * (bits / 8))
        nAvgBytesPerSec = nSamplesPerSec * nBlockAlign
    End Sub
End Class

Public Enum WaveFormats
    PCM = 1
    Float = 3
End Enum
