
Imports System.Threading
Imports System.Runtime.InteropServices

Public Delegate Sub BufferFillEventHandler(data As IntPtr, size As Integer)

Friend Class WaveOutBuffer
    Implements IDisposable

    Public NextBuffer As WaveOutBuffer

    Private m_PlayEvent As New AutoResetEvent(False)
    Private m_WaveOut As IntPtr

    Private m_Header As WaveNative.WaveHdr
    Private m_HeaderData As Byte()
    Private m_HeaderHandle As GCHandle
    Private m_HeaderDataHandle As GCHandle

    Private m_Playing As Boolean

    Friend Shared Sub WaveOutProc(hdrvr As IntPtr, uMsg As Integer, dwUser As Integer, ByRef wavhdr As WaveNative.WaveHdr, dwParam2 As Integer)
        If uMsg = WaveNative.MM_WOM_DONE Then
            Try
                Dim h As GCHandle = CType(wavhdr.dwUser, GCHandle)
                Dim buf As WaveOutBuffer = DirectCast(h.Target, WaveOutBuffer)
                buf.OnCompleted()
            Catch
            End Try
        End If
    End Sub

    Public Sub New(waveOutHandle As IntPtr, size As Integer)
        m_WaveOut = waveOutHandle
        m_HeaderHandle = GCHandle.Alloc(m_Header, GCHandleType.Pinned)
        m_Header.dwUser = CType(GCHandle.Alloc(Me), IntPtr)
        m_HeaderData = New Byte(size - 1) {}
        m_HeaderDataHandle = GCHandle.Alloc(m_HeaderData, GCHandleType.Pinned)
        m_Header.lpData = m_HeaderDataHandle.AddrOfPinnedObject()
        m_Header.dwBufferLength = size

        WaveNative.[Try](WaveNative.waveOutPrepareHeader(m_WaveOut, m_Header, Marshal.SizeOf(m_Header)))

    End Sub

    Protected Overrides Sub Finalize()
        Try
            Dispose()
        Finally
            MyBase.Finalize()
        End Try
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If m_Header.lpData <> IntPtr.Zero Then
            WaveNative.waveOutUnprepareHeader(m_WaveOut, m_Header, Marshal.SizeOf(m_Header))
            m_HeaderHandle.Free()
            m_Header.lpData = IntPtr.Zero
        End If
        m_PlayEvent.Close()
        If m_HeaderDataHandle.IsAllocated Then
            m_HeaderDataHandle.Free()
        End If
        GC.SuppressFinalize(Me)
    End Sub

    Public ReadOnly Property Size() As Integer
        Get
            Return m_Header.dwBufferLength
        End Get
    End Property

    Public ReadOnly Property Data() As IntPtr
        Get
            Return m_Header.lpData
        End Get
    End Property

    Public Function Play() As Boolean
        SyncLock Me
            m_PlayEvent.Reset()
            m_Playing = WaveNative.waveOutWrite(m_WaveOut, m_Header, Marshal.SizeOf(m_Header)) = WaveNative.MMSYSERR_NOERROR
            Return m_Playing
        End SyncLock
    End Function

    Public Sub WaitFor()
        If m_Playing Then
            m_Playing = m_PlayEvent.WaitOne()
        Else
            Thread.Sleep(0)
        End If
    End Sub

    Public Sub OnCompleted()
        m_PlayEvent.[Set]()
        m_Playing = False
    End Sub

End Class
