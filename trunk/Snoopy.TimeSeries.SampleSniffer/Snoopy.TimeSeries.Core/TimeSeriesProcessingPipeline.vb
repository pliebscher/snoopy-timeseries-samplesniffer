Imports System.Collections.Specialized
Imports System.Collections.ObjectModel
Imports System.Threading.Tasks
Imports System.Threading
''' <summary>
''' TODO: SampleRate .ctor arg + property. Set sample rate on each processor on change.
''' </summary>
''' <remarks></remarks>
Public Class TimeSeriesProcessingPipeline

    Private _PreProcessors As New List(Of TimeSeriesPreprocessor)
    Private _Transformer As TimeSeriesTransformer
    Private _PostProcessors As New List(Of TimeSeriesPostProcessor)

    'Private _State As PipelineState
    Private _Lock As New Object

    Private _Cancel As Boolean

    ''' <summary>
    ''' Contruct a pipeline with the specified transformer.
    ''' </summary>
    ''' <param name="transformer"></param>
    ''' <remarks></remarks>
    Public Sub New(transformer As TimeSeriesTransformer)
        If transformer Is Nothing Then Throw New ArgumentNullException("transformer")
        _Transformer = transformer
    End Sub

    ''' <summary>
    ''' Process a list full of TimeSeries.
    ''' </summary>
    ''' <param name="buffer"></param>
    ''' <remarks></remarks>
    Public Sub Process(buffer As List(Of TimeSeries))
        Process(buffer, Nothing)
    End Sub

    ''' <summary>
    ''' Process a buffer full of TimeSeries.
    ''' </summary>
    ''' <param name="buffer"></param>
    ''' <remarks></remarks>
    Public Sub Process(buffer As TimeSeriesBuffer)
        Me.Process(buffer.ToList, Nothing)
    End Sub

    ''' <summary>
    ''' Process a buffer full of TimeSeries.
    ''' </summary>
    ''' <param name="buffer"></param>
    ''' <param name="progressCallback"></param>
    ''' <remarks></remarks>
    Public Sub Process(buffer As TimeSeriesBuffer, progressCallback As Action(Of Integer))
        Me.Process(buffer.ToList, progressCallback)
    End Sub

    ''' <summary>
    ''' Process a list of TimeSeries with a callback to indicate the current processing index.
    ''' </summary>
    ''' <param name="buffer"></param>
    ''' <param name="progressCallback"></param>
    ''' <remarks></remarks>
    Public Sub Process(buffer As List(Of TimeSeries), progressCallback As Action(Of Integer))
        _Cancel = False
        For i As Integer = 0 To buffer.Count - 1
            If _Cancel Then '_State <> PipelineState.Ready OrElse _Cancel Then
                '_State = PipelineState.Ready
                Exit For
            End If
            Process(buffer(i))
            If progressCallback IsNot Nothing Then progressCallback(i)
        Next
    End Sub

    ''' <summary>
    ''' Canceles processing of the current buffer.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Cancel()
        _Cancel = True
    End Sub

    ''' <summary>
    ''' Process a single TimeSeries.
    ''' </summary>
    ''' <param name="series"></param>
    ''' <remarks></remarks>
    Public Sub Process(series As TimeSeries)

        If series Is Nothing Then Throw New ArgumentNullException("series")

        'If _State = PipelineState.Error Then Exit Sub

        SyncLock _Lock

            '_State = PipelineState.Running

            With series

                If ._IsProcessed Then
                    .Reset()
                End If

                ._IsProcessed = True

                ' 1. ------------------------------------------------------------------------------------------------
                For i As Integer = 0 To _PreProcessors.Count - 1
                    Dim Preproc As TimeSeriesPreprocessor = _PreProcessors(i)
                    Try
                        Preproc.Process(series)
                    Catch ex As Exception
                        'Preproc.Enabled = False
                        Throw '                     ???? Should we reset the TimeSeries if an exception is thrown? It may be in a bad state.
                    End Try
                Next

                ' 2. ------------------------------------------------------------------------------------------------
                _Transformer.Transform(series)

                ' 3. ------------------------------------------------------------------------------------------------
                For i As Integer = 0 To _PostProcessors.Count - 1
                    Dim PostProc As TimeSeriesPostProcessor = _PostProcessors(i)
                    Try
                        PostProc.Process(series)
                    Catch ex As Exception
                        'PostProc.Enabled = False
                        Throw
                    End Try
                Next

            End With

        End SyncLock

        '_State = PipelineState.Ready

    End Sub

    Public Sub Save(filename As String)
        Throw New NotImplementedException("Feel free...")
    End Sub

    Public Shared Function Load(filename As String) As TimeSeriesQueryResult
        Throw New NotImplementedException("Feel free...")
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PreProcessors As List(Of TimeSeriesPreprocessor)
        Get
            Return _PreProcessors
        End Get
        Set(processor As List(Of TimeSeriesPreprocessor))
            If processor Is Nothing Then Throw New ArgumentNullException("processor")
            _PreProcessors = processor
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Transformer As TimeSeriesTransformer
        Get
            Return _Transformer
        End Get
        Set(transformer As TimeSeriesTransformer)
            If transformer Is Nothing Then Throw New ArgumentNullException("transformer")
            _Transformer = transformer
            '_State = PipelineState.Ready
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PostProcessors As List(Of TimeSeriesPostProcessor)
        Get
            Return _PostProcessors
        End Get
        Set(processor As List(Of TimeSeriesPostProcessor))
            If processor Is Nothing Then Throw New ArgumentNullException("processor")
            _PostProcessors = processor
        End Set
    End Property

    Public Enum PipelineState
        Ready
        'Running
        '[Error]
    End Enum

End Class
