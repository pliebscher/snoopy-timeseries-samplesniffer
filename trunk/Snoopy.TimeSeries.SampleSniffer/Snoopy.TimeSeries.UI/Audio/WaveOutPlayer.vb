Imports System.Threading
Imports System.Runtime.InteropServices

Public Class WaveOutPlayer
    Implements IDisposable

    Private m_WaveOut As IntPtr
    Private m_Buffers As WaveOutBuffer
    ' linked list
    Private m_CurrentBuffer As WaveOutBuffer
    Private m_Thread As Thread
    Private m_FillProc As BufferFillEventHandler
    Private m_Finished As Boolean
    Private m_zero As Byte

    Private m_BufferProc As New WaveNative.WaveDelegate(AddressOf WaveOutBuffer.WaveOutProc)

    Public Sub New(device As Integer, format As WaveFormat, bufferSize As Integer, bufferCount As Integer, fillProc As BufferFillEventHandler)
        m_zero = If(format.wBitsPerSample = 8, CByte(128), CByte(0))
        m_FillProc = fillProc

        WaveNative.[Try](WaveNative.waveOutOpen(m_WaveOut, device, format, m_BufferProc, IntPtr.Zero, WaveNative.CALLBACK_FUNCTION))

        AllocateBuffers(bufferSize, bufferCount)

        m_Thread = New Thread(New ThreadStart(AddressOf ThreadProc))
        m_Thread.Start()

    End Sub

    Private Sub ThreadProc()
        While Not m_Finished
            Advance()
            If m_FillProc IsNot Nothing AndAlso Not m_Finished Then
                m_FillProc(m_CurrentBuffer.Data, m_CurrentBuffer.Size)
            Else
                ' zero out buffer
                Dim v As Byte = m_zero
                Dim b As Byte() = New Byte(m_CurrentBuffer.Size - 1) {}
                For i As Integer = 0 To b.Length - 1
                    b(i) = v
                Next

                Marshal.Copy(b, 0, m_CurrentBuffer.Data, b.Length)
            End If
            m_CurrentBuffer.Play()
        End While
        WaitForAllBuffers()
    End Sub

    Private Sub AllocateBuffers(bufferSize As Integer, bufferCount As Integer)
        FreeBuffers()
        If bufferCount > 0 Then
            m_Buffers = New WaveOutBuffer(m_WaveOut, bufferSize)
            Dim Prev As WaveOutBuffer = m_Buffers
            Try
                For i As Integer = 1 To bufferCount - 1
                    Dim Buf As New WaveOutBuffer(m_WaveOut, bufferSize)
                    Prev.NextBuffer = Buf
                    Prev = Buf
                Next
            Finally
                Prev.NextBuffer = m_Buffers
            End Try
        End If
    End Sub

    Private Sub FreeBuffers()
        m_CurrentBuffer = Nothing
        If m_Buffers IsNot Nothing Then
            Dim First As WaveOutBuffer = m_Buffers
            m_Buffers = Nothing

            Dim Current As WaveOutBuffer = First
            Do
                Dim [Next] As WaveOutBuffer = Current.NextBuffer
                Current.Dispose()
                Current = [Next]
            Loop While Current IsNot First
        End If
    End Sub

    Private Sub Advance()
        m_CurrentBuffer = If(m_CurrentBuffer Is Nothing, m_Buffers, m_CurrentBuffer.NextBuffer)
        m_CurrentBuffer.WaitFor()
    End Sub

    Private Sub WaitForAllBuffers()
        Dim Buf As WaveOutBuffer = m_Buffers
        While Buf.NextBuffer IsNot m_Buffers
            Buf.WaitFor()
            Buf = Buf.NextBuffer
        End While
    End Sub

#Region " -- IDisposable -- "

    Protected Overrides Sub Finalize()
        Try
            Dispose()
        Finally
            MyBase.Finalize()
        End Try
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If m_Thread IsNot Nothing Then
            Try
                m_Finished = True
                If m_WaveOut <> IntPtr.Zero Then
                    WaveNative.waveOutReset(m_WaveOut)
                End If
                m_Thread.Join()
                m_FillProc = Nothing
                FreeBuffers()
                If m_WaveOut <> IntPtr.Zero Then
                    WaveNative.waveOutClose(m_WaveOut)
                End If
            Finally
                m_Thread = Nothing
                m_WaveOut = IntPtr.Zero
            End Try
        End If
        GC.SuppressFinalize(Me)
    End Sub

#End Region

    Public Shared ReadOnly Property DeviceCount() As Integer
        Get
            Return WaveNative.waveOutGetNumDevs()
        End Get
    End Property

End Class
