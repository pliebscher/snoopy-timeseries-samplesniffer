Imports System.Threading
Imports System.Runtime.InteropServices

Public Class WaveInRecorder
    Implements IDisposable

    Private _WaveIn As IntPtr
    Private _Buffers As WaveInBuffer
    Private _CurrentBuffer As WaveInBuffer
    Private _AdvanceBufferThread As Thread

    Private _Device As Integer
    Private _Format As WaveFormat
    Private _BufferSize As Integer
    Private _BufferCount As Integer
    Private _OnBufferFull As BufferDoneEventHandler

    Private _IsInitialized As Boolean
    Private _Finished As Boolean

    Private _BufferProc As New WaveNative.WaveDelegate(AddressOf WaveInBuffer.WaveInProc)

    Public Shared ReadOnly Property DeviceCount() As Integer
        Get
            Return WaveNative.waveInGetNumDevs()
        End Get
    End Property

    Public Sub New(ByVal device As Integer, ByVal format As WaveFormat, ByVal bufferSize As Integer, ByVal bufferCount As Integer, ByVal onBufferFullProc As BufferDoneEventHandler)
        _Device = device
        _Format = format
        _BufferSize = bufferSize
        _BufferCount = bufferCount
        _OnBufferFull = onBufferFullProc
    End Sub

    Public Sub Start()

        If Not _IsInitialized Then
            Init()
        End If

        WaveNative.[Try](WaveNative.waveInStart(_WaveIn))

        _Finished = False

        _AdvanceBufferThread = New Thread(New ThreadStart(AddressOf OnAdvanceBuffer))
        _AdvanceBufferThread.Name = "WaveInRecorder"
        'm_Thread.Priority = ThreadPriority.Highest
        _AdvanceBufferThread.Start()
    End Sub

    Public Sub [Stop]()
        _Finished = True
        _AdvanceBufferThread = Nothing
        WaveNative.[Try](WaveNative.waveInStop(_WaveIn))
    End Sub

    Private Sub Init()
        WaveNative.[Try](WaveNative.waveInOpen(_WaveIn, _Device, _Format, _BufferProc, IntPtr.Zero, WaveNative.CALLBACK_FUNCTION))

        AllocateBuffers(_BufferSize, _BufferCount)

        For i As Integer = 0 To _BufferCount - 1
            SelectNextBuffer()
            _CurrentBuffer.Record()
        Next

        _IsInitialized = True
    End Sub

    Private Sub OnAdvanceBuffer()
        While Not _Finished
            Advance()
            If Not _Finished AndAlso _OnBufferFull IsNot Nothing Then
                _OnBufferFull(_CurrentBuffer.Data, _CurrentBuffer.Size)
                If _Finished OrElse _CurrentBuffer Is Nothing Then Exit While
                _CurrentBuffer.Record()
            End If
        End While
    End Sub

    Private Sub AllocateBuffers(ByVal bufferSize As Integer, ByVal bufferCount As Integer)
        FreeBuffers()
        If bufferCount > 0 Then
            _Buffers = New WaveInBuffer(_WaveIn, bufferSize)
            Dim Prev As WaveInBuffer = _Buffers
            Try
                For i As Integer = 1 To bufferCount - 1
                    Dim Buf As New WaveInBuffer(_WaveIn, bufferSize)
                    Prev.NextBuffer = Buf
                    Prev = Buf
                Next
            Finally
                Prev.NextBuffer = _Buffers
            End Try
        End If
    End Sub

    Private Sub FreeBuffers()
        _CurrentBuffer = Nothing
        If _Buffers IsNot Nothing Then
            Dim First As WaveInBuffer = _Buffers
            _Buffers = Nothing

            Dim Current As WaveInBuffer = First
            Do
                Dim [Next] As WaveInBuffer = Current.NextBuffer
                Current.Dispose()
                Current = [Next]
            Loop While Current IsNot First
        End If
    End Sub

    Private Sub Advance()
        SelectNextBuffer()
        _CurrentBuffer.WaitFor()
    End Sub

    Private Sub SelectNextBuffer()
        _CurrentBuffer = If(_CurrentBuffer Is Nothing, _Buffers, _CurrentBuffer.NextBuffer)
    End Sub

    Private Sub WaitForAllBuffers()
        Dim Buf As WaveInBuffer = _Buffers
        While Buf.NextBuffer IsNot _Buffers
            Buf.WaitFor()
            Buf = Buf.NextBuffer
        End While
    End Sub

#Region " -- IDisposable -- "

    Private _IsDisposed As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me._IsDisposed Then
            If disposing Then
                ' Managed...
            End If
            ' Unmanaged...
            If _AdvanceBufferThread IsNot Nothing Then
                Try
                    _Finished = True
                    If _WaveIn <> IntPtr.Zero Then
                        WaveNative.waveInReset(_WaveIn)
                    End If
                    WaitForAllBuffers()
                    _AdvanceBufferThread.Join(500)
                    _OnBufferFull = Nothing
                    FreeBuffers()
                    If _WaveIn <> IntPtr.Zero Then
                        WaveNative.waveInClose(_WaveIn)
                    End If
                Finally
                    _AdvanceBufferThread = Nothing
                    _WaveIn = IntPtr.Zero
                End Try
            End If

        End If
        Me._IsDisposed = True
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
        MyBase.Finalize()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class
