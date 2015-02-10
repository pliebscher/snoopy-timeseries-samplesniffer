Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.Text
Imports System.ComponentModel
''' <summary>
''' A thread-safe FIFO buffer.
''' </summary>
''' <remarks></remarks>
<Serializable>
Public Class TimeSeriesBuffer

    <NonSerialized>
    Private _FileName As String = String.Empty
    <NonSerialized>
    Private _BufferCount As Integer = 0

    Private _Lock As New Object
    Private _Buffer As List(Of TimeSeries)
    Private _BufferMaxSize As Integer

    <NonSerialized()>
    Public Event TimeSeriesAdded(sender As Object, e As EventArgs)
    <NonSerialized()>
    Public Event BufferCleared(sender As Object, e As EventArgs)
    <NonSerialized()>
    Public Event BufferSizeChanged(sender As Object, e As EventArgs)

    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="bufferMaxSize">Initial size of the buffer.</param>
    ''' <remarks></remarks>
    Public Sub New(bufferMaxSize As Integer)
        If bufferMaxSize < 1 Then Throw New ArgumentException("bufferSize must be greater than zero.")
        _BufferMaxSize = bufferMaxSize
        _Buffer = New List(Of TimeSeries)(_BufferMaxSize)
    End Sub

    ''' <summary>
    ''' Add a series to the buffer.
    ''' </summary>
    ''' <param name="timeSeries"></param>
    ''' <remarks></remarks>
    Public Sub Add(timeSeries As TimeSeries)
        SyncLock _Lock
            _BufferCount = _Buffer.Count
            If _BufferCount >= _BufferMaxSize Then
                _Buffer.RemoveAt(0)
                _BufferCount -= 1
                If (_BufferCount) - (_BufferMaxSize - 1) > 0 Then
                    ' Remove an extra one until we sync down... Should we have a flag to do this in one pass?
                    _Buffer.RemoveAt(0)
                    _BufferCount -= 1
                End If
            End If
            _Buffer.Add(timeSeries)
            _BufferCount += 1
            RaiseEvent TimeSeriesAdded(Me, EventArgs.Empty)
        End SyncLock
    End Sub

    ''' <summary>
    ''' Clear the buffer.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Clear()
        _FileName = String.Empty
        _Buffer.Clear()
        _BufferCount = 0
        RaiseEvent BufferCleared(Me, EventArgs.Empty)
    End Sub

    ''' <summary>
    ''' Loads a series from a file.
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Load(filename As String) As TimeSeriesBuffer
        If String.IsNullOrWhiteSpace(filename) Then Throw New ArgumentNullException("filename")
        Try
            Using fs As New FileStream(filename, FileMode.Open)
                Dim serializer As New DataContractSerializer(GetType(TimeSeriesBuffer))
                Dim reader As New XmlTextReader(fs)
                Dim ts As TimeSeriesBuffer = DirectCast(serializer.ReadObject(reader), TimeSeriesBuffer)
                reader.Close()
                ts._FileName = filename
                ts._BufferCount = ts._Buffer.Count
                ts._BufferMaxSize = ts._Buffer.Count
                Return ts
            End Using
        Catch sex As SerializationException
            Throw New SerializationException("Unable to deserialize TimeSeries: " & sex.Message, sex)
        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Saves the current buffer to a file.
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <remarks></remarks>
    Public Sub Save(filename As String)
        If String.IsNullOrWhiteSpace(filename) Then Throw New ArgumentNullException("filename")
        Dim serializer As New DataContractSerializer(Me.GetType)
        Using fs As New FileStream(filename, FileMode.Create)
            Dim writer As New XmlTextWriter(fs, Encoding.UTF8)
            serializer.WriteObject(writer, Me)
            writer.Close()
            'fs.Dispose()
        End Using
        _FileName = filename
    End Sub

    ''' <summary>
    ''' Returns a new buffer with containing the selected TimeSeries range.
    ''' </summary>
    ''' <param name="startIndex"></param>
    ''' <param name="endIndex"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Crop(startIndex As Integer, endIndex As Integer) As TimeSeriesBuffer
        If startIndex < 0 Then Throw New ArgumentException("startIndex must be greater than or equal to zero.")
        If endIndex > _Buffer.Count - 1 Then Throw New ArgumentException("endIndex must be less than the buffer size.")
        Dim buffer As New TimeSeriesBuffer((endIndex - startIndex) + 1)
        For i As Integer = startIndex To endIndex
            buffer.Add(_Buffer(i))
        Next
        Return buffer
    End Function

    ''' <summary>
    ''' Merges the buffer into a single TimeSeries.
    ''' </summary>
    ''' <returns>A new TimeSeries</returns>
    ''' <remarks></remarks>
    Public Function ToTimeSeries() As TimeSeries
        If _Buffer.Count = 0 Then Throw New TimeSeriesException("Buffer must contain at least one TimeSeries.")
        If _Buffer.Count = 1 Then Return _Buffer(0)
        Dim ts As TimeSeries = _Buffer(0)
        For i As Integer = 1 To _Buffer.Count - 1
            ts = ts.Join(_Buffer(i))
        Next
        Return ts
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ToList() As List(Of TimeSeries)
        Return _Buffer
    End Function

    ''' <summary>
    ''' Return a single TimeSeries at the given index.
    ''' </summary>
    ''' <param name="index"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Default Public ReadOnly Property TimeSeries(index As Integer) As TimeSeries
        Get
            SyncLock _Lock
                Return _Buffer(index)
            End SyncLock
        End Get
    End Property

    ''' <summary>
    ''' Returns the last TimeSeries in the buffer.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Last As TimeSeries
        Get
            SyncLock _Lock
                Return _Buffer(_Buffer.Count - 1)
            End SyncLock
        End Get
    End Property

    ''' <summary>
    ''' Returns the first TimeSeries in the buffer.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property First As TimeSeries
        Get
            SyncLock _Lock
                Return _Buffer(0)
            End SyncLock
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsFull As Boolean
        Get
            Return _BufferCount = _BufferMaxSize
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MaxSize As Integer
        Get
            Return _BufferMaxSize
        End Get
        Set(value As Integer)
            If value <> _BufferMaxSize Then
                _FileName = String.Empty ' Not sure if this is the best place for this, but dont want to check on every Add() for performance...
                _BufferMaxSize = value
                RaiseEvent BufferSizeChanged(Me, EventArgs.Empty)
            End If
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Count As Integer
        Get
            Return _BufferCount
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property FileName As String
        Get
            Return _FileName
        End Get
    End Property

End Class
