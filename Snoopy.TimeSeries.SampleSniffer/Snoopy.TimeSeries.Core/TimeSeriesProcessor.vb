Imports System.ComponentModel
Imports System.Threading.Tasks
Imports System.Threading
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Reflection

Public MustInherit Class TimeSeriesProcessor(Of TIn As ITimeSeries(Of TIn), TOut)

    Public Function Process(series As TIn) As TOut

    End Function

    Protected Overridable Function OnProcessTimeSeries(series As ITimeSeries(Of TIn)) As TOut

    End Function

End Class

<RefreshProperties(RefreshProperties.All), DefaultProperty("Description")>
Public MustInherit Class TimeSeriesProcessor

    Private _Enabled As Boolean
    Private _SampleRate As Integer = 1

    Public Sub New()
    End Sub

    Public Sub New(sampleRate As Integer)
        _SampleRate = sampleRate
    End Sub

    ' Protected Friend Function Process(Of TIn As IEquatable(Of TIn), TOut)(series As TIn, processFunc As Func(Of TIn, TOut)) As TOut
    Protected Friend Function Process1(Of TIn, TOut)(series As TIn, processFunc As Func(Of TIn, TOut)) As TOut

        Try

            Dim out As TOut = processFunc(series)

            If out Is Nothing Then Throw New TimeSeriesProcessorException(String.Format("Processor ({0}) returned nothing!", Me.Name), Me)

            Return out

        Catch tex As TimeSeriesProcessorException
            Throw
        Catch ex As Exception
            If ex.InnerException IsNot Nothing Then ex = ex.InnerException
            Dim tex As New TimeSeriesProcessorException(ex.Message, Me, ex)
            Throw tex
        End Try

    End Function



    Protected Overridable Sub OnSampleRateChanged(sampleRate As Integer)
    End Sub

    Protected Overridable Sub OnProcessorSave(properties As Dictionary(Of String, Object))
        For Each pi As PropertyInfo In Me.GetType.GetProperties(BindingFlags.Instance Or BindingFlags.Public)
            If pi.CanRead AndAlso pi.CanWrite Then
                Dim val As Object = pi.GetValue(Me, Nothing)
                If pi.PropertyType.BaseType Is GetType(System.ValueType) Then
                    properties.Add(pi.Name, val)
                ElseIf pi.PropertyType.BaseType Is GetType(System.Enum) Then
                    Dim enumType As String = pi.PropertyType.AssemblyQualifiedName
                    properties.Add(pi.Name, String.Format("{0}:{1}", enumType, val.ToString))
                End If
            End If
        Next
    End Sub

    Protected Overridable Sub OnProcessorLoad(properties As Dictionary(Of String, Object))
        For Each prop As KeyValuePair(Of String, Object) In properties
            Dim pi As PropertyInfo = Me.GetType.GetProperty(prop.Key, BindingFlags.Instance Or BindingFlags.Public)
            If pi IsNot Nothing Then
                If pi.PropertyType.BaseType Is GetType(System.Enum) Then
                    Dim enumType As String = prop.Value.ToString.Split(":".ToCharArray)(0)
                    Dim enumVal As String = prop.Value.ToString.Split(":".ToCharArray)(1)
                    pi.SetValue(Me, [Enum].Parse(Type.GetType(enumType), enumVal), Nothing)
                Else
                    pi.SetValue(Me, prop.Value, Nothing)
                End If
            End If
        Next
    End Sub

    ''' <summary>
    ''' Saves the values of all Public properties along with Type information for this processor instance.
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <remarks></remarks>
    Public Sub Save(filename As String)

        Dim fs As FileStream = Nothing
        Dim writer As XmlTextWriter = Nothing
        Dim serializer As DataContractSerializer = Nothing

        Try

            Dim processor As New TSProcessor

            Me.OnProcessorSave(processor.Parameters)

            processor.Parameters.Add("_Type_", Me.GetType.AssemblyQualifiedName)

            fs = New FileStream(filename, FileMode.Create)
            writer = New XmlTextWriter(fs, Encoding.UTF8)
            serializer = New DataContractSerializer(GetType(TSProcessor))

            serializer.WriteObject(writer, processor)

        Catch ex As Exception
            Throw New TimeSeriesQueryException(String.Format("Error saving processor: {0}", ex.Message), ex)
        Finally
            If writer IsNot Nothing Then
                writer.Close()
                writer = Nothing
            End If
            If fs IsNot Nothing Then
                fs.Dispose()
                fs = Nothing
            End If
        End Try

    End Sub

    ''' <summary>
    ''' Returns a new instance of a processor with all Public property values restored from a previously saved processor instance.
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Load(filename As String) As TimeSeriesProcessor

        Dim tsp As TimeSeriesProcessor = Nothing
        Dim fs As FileStream = Nothing
        Dim reader As XmlTextReader = Nothing
        Dim serializer As DataContractSerializer = Nothing

        Try
            serializer = New DataContractSerializer(GetType(TSProcessor))
            fs = New FileStream(filename, FileMode.Open)
            reader = New XmlTextReader(fs)

            Dim processorInfo As TSProcessor = DirectCast(serializer.ReadObject(reader), TSProcessor)

            tsp = TryCast(Activator.CreateInstance(Type.GetType(processorInfo.Parameters("_Type_").ToString)), TimeSeriesProcessor)
            tsp.OnProcessorLoad(processorInfo.Parameters)

        Catch ex As Exception
            Throw New TimeSeriesProcessorException(String.Format("Error loading processor: {0}", ex.ToString))
        Finally
            If reader IsNot Nothing Then
                reader.Close()
                reader = Nothing
            End If

            If fs IsNot Nothing Then
                fs.Dispose()
                fs = Nothing
            End If
        End Try

        Return tsp
    End Function

    ''' <summary>
    ''' Creates a new instance of a TimeSeriesProcessor from the given Type with the given sample rate.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="sampleRate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Create(Of T As TimeSeriesProcessor)(sampleRate As Integer) As T
        Dim tsp As T = Nothing
        Try
            tsp = DirectCast(Activator.CreateInstance(GetType(T)), T)
            tsp._SampleRate = sampleRate
        Catch ex As Exception
            Throw New TimeSeriesProcessorException(String.Format("Error creating processor: {0}", ex.ToString))
        End Try
        Return tsp
    End Function

    <Browsable(False)>
    Public Property SampleRate As Integer
        Get
            Return _SampleRate
        End Get
        Set(value As Integer)
            If _SampleRate <> value Then
                _SampleRate = value
                OnSampleRateChanged(_SampleRate)
            End If
        End Set
    End Property

    <Browsable(False)>
    Public Property Enabled As Boolean
        Get
            Return _Enabled
        End Get
        Set(value As Boolean)
            _Enabled = value
        End Set
    End Property

    <Browsable(False)>
    Public Overridable ReadOnly Property Name As String
        Get
            Return Me.GetType.FullName
        End Get
    End Property

    Public Overridable ReadOnly Property Description As String
        Get
            Return String.Empty
        End Get
    End Property

    ''' <summary>
    ''' Container class to work around serialization issues. This way we don't need to serialize the entire processor, which won't
    ''' work anyway if there are unknown types!
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable>
    Friend Class TSProcessor
        Public Parameters As New Dictionary(Of String, Object)
        Public Name As String
        Public Description As String
    End Class

End Class
