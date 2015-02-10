Imports System.Threading
Imports System.Runtime.InteropServices

Public Delegate Sub BufferDoneEventHandler(data As IntPtr, size As Integer)

Friend Class WaveInBuffer
    Implements IDisposable
    Public NextBuffer As WaveInBuffer

    Private m_RecordEvent As New AutoResetEvent(False)
    Private m_WaveIn As IntPtr

    Private m_Header As WaveNative.WaveHdr
    Private m_HeaderData As Byte()
    Private m_HeaderHandle As GCHandle
    Private m_HeaderDataHandle As GCHandle

    Private m_Recording As Boolean

    Friend Shared recNum As Integer = 0
    Friend Shared bufNum As Integer = 0

    Friend Shared Sub WaveInProc(hdrvr As IntPtr, uMsg As Integer, dwUser As Integer, ByRef wavhdr As WaveNative.WaveHdr, dwParam2 As Integer)
        If uMsg = WaveNative.MM_WIM_DATA Then
            Try
                Dim h As GCHandle = CType(wavhdr.dwUser, GCHandle)
                Dim buf As WaveInBuffer = DirectCast(h.Target, WaveInBuffer)
                buf.OnCompleted()
            Catch
            End Try
        End If
    End Sub

    Public Sub New(waveInHandle As IntPtr, size As Integer)
        bufNum += 1
        m_WaveIn = waveInHandle
        m_HeaderHandle = GCHandle.Alloc(m_Header, GCHandleType.Pinned)
        m_Header.dwUser = CType(GCHandle.Alloc(Me), IntPtr)
        m_HeaderData = New Byte(size - 1) {}
        m_HeaderDataHandle = GCHandle.Alloc(m_HeaderData, GCHandleType.Pinned)
        m_Header.lpData = m_HeaderDataHandle.AddrOfPinnedObject()
        m_Header.dwBufferLength = size
        WaveNative.[Try](WaveNative.waveInPrepareHeader(m_WaveIn, m_Header, Marshal.SizeOf(m_Header)))
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
            WaveNative.waveInUnprepareHeader(m_WaveIn, m_Header, Marshal.SizeOf(m_Header))
            m_HeaderHandle.Free()
            m_Header.lpData = IntPtr.Zero
        End If
        m_RecordEvent.Close()
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

    'Public Function Record() As Boolean
    '    SyncLock Me
    '        m_RecordEvent.Reset()
    '        m_Recording = WaveNative.waveInAddBuffer(m_WaveIn, m_Header, Marshal.SizeOf(m_Header)) = WaveNative.MMSYSERR_NOERROR
    '        Return m_Recording
    '    End SyncLock
    'End Function

    'Public Sub WaitFor()
    '    If m_Recording Then
    '        m_Recording = m_RecordEvent.WaitOne(1000)
    '    Else
    '        Thread.Sleep(0)
    '    End If
    'End Sub

    'Private Sub OnCompleted()
    '    m_RecordEvent.[Set]()
    '    m_Recording = False
    'End Sub

    Public Function Record() As Boolean
        SyncLock Me
            recNum += 1
            m_RecordEvent.Reset()
            m_Recording = WaveNative.waveInAddBuffer(m_WaveIn, m_Header, Marshal.SizeOf(m_Header)) = WaveNative.MMSYSERR_NOERROR
            Return m_Recording
        End SyncLock
    End Function

    Public Sub WaitFor()
        If recNum < 2 AndAlso bufNum > 1 Then
            Return
        End If
        If m_Recording Then
            m_Recording = m_RecordEvent.WaitOne(1000)
        Else
            Thread.Sleep(0)
        End If
    End Sub

    Private Sub OnCompleted()
        recNum -= 1
        m_RecordEvent.[Set]()
        m_Recording = False
    End Sub

End Class
