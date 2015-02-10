Imports System.Runtime.InteropServices

Friend Class WaveNative
    ' consts
    Public Const MMSYSERR_NOERROR As Integer = 0
    ' no error
    Public Const MM_WOM_OPEN As Integer = &H3BB
    Public Const MM_WOM_CLOSE As Integer = &H3BC
    Public Const MM_WOM_DONE As Integer = &H3BD
    Public Const MM_WIM_OPEN As Integer = &H3BE
    Public Const MM_WIM_CLOSE As Integer = &H3BF
    Public Const MM_WIM_DATA As Integer = &H3C0

    Public Const CALLBACK_FUNCTION As Integer = &H30000
    ' dwCallback is a FARPROC
    Public Const TIME_MS As Integer = &H1
    ' time in milliseconds
    Public Const TIME_SAMPLES As Integer = &H2
    ' number of wave samples
    Public Const TIME_BYTES As Integer = &H4
    ' current byte offset
    ' callbacks
    Public Delegate Sub WaveDelegate(ByVal hdrvr As IntPtr, ByVal uMsg As Integer, ByVal dwUser As Integer, ByRef wavhdr As WaveHdr, ByVal dwParam2 As Integer)

    ' structs

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure WaveHdr
        Public lpData As IntPtr
        ' pointer to locked data buffer
        Public dwBufferLength As Integer
        ' length of data buffer
        Public dwBytesRecorded As Integer
        ' used for input only
        Public dwUser As IntPtr
        ' for client's use
        Public dwFlags As Integer
        ' assorted flags (see defines)
        Public dwLoops As Integer
        ' loop control counter
        Public lpNext As IntPtr
        ' PWaveHdr, reserved for driver
        Public reserved As Integer
        ' reserved for driver
    End Structure

    Private Const mmdll As String = "winmm.dll"

    ' WaveOut calls
    <DllImport(mmdll)>
    Public Shared Function waveOutGetNumDevs() As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveOutPrepareHeader(ByVal hWaveOut As IntPtr, ByRef lpWaveOutHdr As WaveHdr, ByVal uSize As Integer) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveOutUnprepareHeader(ByVal hWaveOut As IntPtr, ByRef lpWaveOutHdr As WaveHdr, ByVal uSize As Integer) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveOutWrite(ByVal hWaveOut As IntPtr, ByRef lpWaveOutHdr As WaveHdr, ByVal uSize As Integer) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveOutOpen(ByRef hWaveOut As IntPtr, ByVal uDeviceID As Integer, ByVal lpFormat As WaveFormat, ByVal dwCallback As WaveDelegate, ByVal dwInstance As IntPtr, ByVal dwFlags As Integer) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveOutReset(ByVal hWaveOut As IntPtr) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveOutClose(ByVal hWaveOut As IntPtr) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveOutPause(ByVal hWaveOut As IntPtr) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveOutRestart(ByVal hWaveOut As IntPtr) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveOutGetPosition(ByVal hWaveOut As IntPtr, ByVal lpInfo As Integer, ByVal uSize As Integer) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveOutSetVolume(ByVal hWaveOut As IntPtr, ByVal dwVolume As Integer) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveOutGetVolume(ByVal hWaveOut As IntPtr, ByVal dwVolume As Integer) As Integer
    End Function

    ' WaveIn calls
    <DllImport(mmdll)>
    Public Shared Function waveInGetNumDevs() As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveInAddBuffer(ByVal hwi As IntPtr, ByRef pwh As WaveHdr, ByVal cbwh As Integer) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveInClose(ByVal hwi As IntPtr) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveInOpen(ByRef phwi As IntPtr, ByVal uDeviceID As Integer, ByVal lpFormat As WaveFormat, ByVal dwCallback As WaveDelegate, ByVal dwInstance As IntPtr, ByVal dwFlags As Integer) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveInPrepareHeader(ByVal hWaveIn As IntPtr, ByRef lpWaveInHdr As WaveHdr, ByVal uSize As Integer) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveInUnprepareHeader(ByVal hWaveIn As IntPtr, ByRef lpWaveInHdr As WaveHdr, ByVal uSize As Integer) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveInReset(ByVal hwi As IntPtr) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveInStart(ByVal hwi As IntPtr) As Integer
    End Function

    <DllImport(mmdll)>
    Public Shared Function waveInStop(ByVal hwi As IntPtr) As Integer
    End Function

    Public Shared Sub [Try](ByVal err As Integer)
        If err <> WaveNative.MMSYSERR_NOERROR Then
            Throw New Exception(err.ToString())
        End If
    End Sub

End Class
