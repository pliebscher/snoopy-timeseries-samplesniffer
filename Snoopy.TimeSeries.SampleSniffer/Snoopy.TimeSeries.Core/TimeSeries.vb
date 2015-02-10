Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.Text
Imports System.ComponentModel
'
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Serializable>
Public NotInheritable Class TimeSeries
    Implements ITimeSeries(Of Double()), ITimeSeries(Of Double()()), IEquatable(Of TimeSeries)

    Private _Samples As Double() ' Original unprocessed samples.

    Friend _PreProcSamples As Double()
    Friend _PostProcFrames As Double()()
    Friend _IsProcessed As Boolean

    Private _SampleRate As Integer
    Private _TimeStamp As Date = Date.Now
    Private _TimeSpan As TimeSpan
    Private _Centroid As Double = Double.PositiveInfinity

    Private Sub New()
    End Sub

    Public Sub New(samples As Double(), sampleRate As Integer)
        Me.New(samples, sampleRate, Date.Now)
    End Sub

    Public Sub New(samples As Double(), sampleRate As Integer, timestamp As Date)
        _Samples = New Double(samples.Length - 1) {}
        Array.Copy(samples, _Samples, _Samples.Length)
        _SampleRate = sampleRate
        _TimeStamp = timestamp
        _TimeSpan = New TimeSpan(0, 0, 0, 0, _Samples.Length * 1000 \ _SampleRate)

        _PreProcSamples = New Double(_Samples.Length - 1) {}

        Me.Reset()
    End Sub

    ''' <summary>
    ''' Resets the TimeSeries to it's initial state prior to any processing.
    ''' </summary>
    ''' <remarks>Original samples are restored and all properties are reset.</remarks>
    Public Sub Reset()

        If _PreProcSamples.Length <> _Samples.Length Then
            _PreProcSamples = New Double(_Samples.Length - 1) {}
        End If

        Array.Copy(_Samples, _PreProcSamples, _Samples.Length)

        ' Only one frame before transform...
        _PostProcFrames = New Double(0)() {}
        _PostProcFrames(0) = _PreProcSamples

        _Centroid = Double.PositiveInfinity

        _IsProcessed = False

    End Sub

    ''' <summary>
    ''' Creates a deep copy of the current instance.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Clone() As TimeSeries
        Dim cloned As New TimeSeries()

        cloned._SampleRate = _SampleRate
        cloned._TimeStamp = _TimeStamp
        cloned._TimeSpan = _TimeSpan
        cloned._Centroid = _Centroid
        cloned._Samples = New Double(_Samples.Length - 1) {}
        cloned._PreProcSamples = New Double(_PreProcSamples.Length - 1) {}
        cloned._PostProcFrames = New Double(_PostProcFrames.Length - 1)() {}

        Array.Copy(_Samples, cloned._Samples, _Samples.Length)
        Array.Copy(_PreProcSamples, cloned._PreProcSamples, _PreProcSamples.Length)

        For i As Integer = 0 To _PostProcFrames.Length - 1
            cloned._PostProcFrames(i) = New Double(_PostProcFrames(i).Length - 1) {}
            Array.Copy(_PostProcFrames(i), cloned._PostProcFrames(i), cloned._PostProcFrames(i).Length)
        Next

        Return cloned
    End Function

    ''' <summary>
    ''' Splits the current series into the specified number of series with an optional overlap.
    ''' </summary>
    ''' <param name="count"></param>
    ''' <param name="overlap"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Split(count As Integer, overlap As Double) As List(Of TimeSeries)
        If count < 1 Then Throw New ArgumentException("count must be greater than zero.")
        If overlap < 0 Then Throw New ArgumentException("overlap must be greater than or equal to zero.")

        Dim splitSeries As New List(Of TimeSeries)

        If count = 1 Then
            splitSeries.Add(Me)
            Return splitSeries
        End If

        Dim width As Integer = _Samples.Length \ count
        Dim hop As Integer = CInt(width * overlap)
        Dim len As Integer = width + hop
        Dim start As Integer = 0

        Dim frameWidth As Integer = _PostProcFrames.Length \ count
        Dim frameHop As Integer = CInt(frameWidth * overlap)
        Dim frameLen As Integer = CInt(frameWidth + frameHop)
        Dim frameStart As Integer = 0

        For i As Integer = 0 To count - 1
            Dim slice As Double() = New Double(len - 1) {}

            Array.Copy(_Samples, start, slice, 0, slice.Length)

            Dim series As New TimeSeries(slice, _SampleRate, _TimeStamp) ' TODO: Ajust the timestamp!!

            Array.Copy(_PreProcSamples, start, series._PreProcSamples, 0, series._PreProcSamples.Length)

            series._PostProcFrames = New Double(frameLen - 1)() {}
            For j As Integer = 0 To frameLen - 1
                series._PostProcFrames(j) = _PostProcFrames(frameStart + j)
            Next
            frameStart += frameWidth - frameHop

            splitSeries.Add(series)

            start += width - hop
        Next
        Return splitSeries

    End Function

    ''' <summary>
    ''' Joins the supplied series to the current series and returns a new series. Original series are
    ''' not modified. Statistics are recalculated for the new series. Series are joined in clronologic order.
    ''' </summary>
    ''' <param name="series"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Join(series As TimeSeries) As TimeSeries
        If series Is Nothing Then Throw New ArgumentNullException("series")
        If Me.SampleRate <> series.SampleRate Then Throw New ArgumentException(String.Format("Cannot join two TimeSeries with different samples rates. {0} <> {1}", Me._SampleRate, series._SampleRate))

        Dim A As TimeSeries = Me
        Dim B As TimeSeries = series

        ' Join in clronologic order...
        If Date.Compare(A.TimeStamp, B.TimeStamp) > 0 Then
            Dim S As TimeSeries = A
            A = B
            B = S
        End If

        Dim joinedLength As Integer = A._Samples.Length + B._Samples.Length
        Dim joinedPreLength As Integer = A._PreProcSamples.Length + B._PreProcSamples.Length
        Dim joinedPostLength As Integer = A._PostProcFrames.Length + B._PostProcFrames.Length
        Dim joined As New TimeSeries()

        joined._SampleRate = A._SampleRate
        joined._TimeStamp = A._TimeStamp
        'joined._TimeSpan = New TimeSpan(0, 0, 0, 0, joinedPreLength * 1000 \ A._SampleRate)
        joined._TimeSpan = New TimeSpan(0, 0, 0, 0, joinedLength * 1000 \ A._SampleRate)

        'joined._CentroidIndices = New Integer(joinedPostLength - 1) {}
        'Array.Copy(A.CentroidIndices, joined._CentroidIndices, A._CentroidIndices.Length)
        'Array.Copy(B.CentroidIndices, 0, joined._CentroidIndices, A._CentroidIndices.Length, B._CentroidIndices.Length)

        'joined._CentroidValues = New Double(joinedPostLength - 1) {}
        'Array.Copy(A.CentroidValues, joined._CentroidValues, A._CentroidValues.Length)
        'Array.Copy(B.CentroidValues, 0, joined._CentroidValues, A._CentroidValues.Length, B._CentroidValues.Length)

        joined._Samples = New Double(joinedLength - 1) {}
        Array.Copy(A._Samples, joined._Samples, A._Samples.Length)
        Array.Copy(B._Samples, 0, joined._Samples, A._Samples.Length, B._Samples.Length)

        joined._PreProcSamples = New Double(joinedPreLength - 1) {}
        Array.Copy(A._PreProcSamples, joined._PreProcSamples, A._PreProcSamples.Length)
        Array.Copy(B._PreProcSamples, 0, joined._PreProcSamples, A._PreProcSamples.Length, B._PreProcSamples.Length)

        joined._PostProcFrames = New Double(joinedPostLength - 1)() {}
        For i As Integer = 0 To Me._PostProcFrames.Length - 1
            joined._PostProcFrames(i) = New Double(A._PostProcFrames(i).Length - 1) {}
            Array.Copy(A._PostProcFrames(i), joined._PostProcFrames(i), joined._PostProcFrames(i).Length)
        Next

        For i As Integer = A._PostProcFrames.Length To joinedPostLength - 1
            joined._PostProcFrames(i) = New Double(B._PostProcFrames(i - A._PostProcFrames.Length).Length - 1) {}
            Array.Copy(B._PostProcFrames(i - A._PostProcFrames.Length), joined._PostProcFrames(i), B._PostProcFrames(i - A._PostProcFrames.Length).Length)
        Next

        Return joined

    End Function

    ''' <summary>
    ''' Loads a series from a file.
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Load(filename As String) As TimeSeries
        Dim serializer As New DataContractSerializer(GetType(TimeSeries))
        Using fs As New FileStream(filename, FileMode.Open)
            Dim reader As New XmlTextReader(fs)
            Dim ts As TimeSeries = DirectCast(serializer.ReadObject(reader), TimeSeries)
            reader.Close()
            Return ts
        End Using
    End Function

    ''' <summary>
    ''' Saves the current instance to a file.
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <remarks></remarks>
    Public Sub Save(filename As String)
        Dim serializer As New DataContractSerializer(Me.GetType)
        Using fs As New FileStream(filename, FileMode.Create)
            Dim writer As New XmlTextWriter(fs, Encoding.UTF8)
            serializer.WriteObject(writer, Me)
            writer.Close()
        End Using
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TimeStamp As Date Implements ITimeSeries.TimeStamp
        Get
            Return _TimeStamp
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TimeSpan As TimeSpan Implements ITimeSeries.TimeSpan
        Get
            Return _TimeSpan
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SampleRate As Integer Implements ITimeSeries.SampleRate
        Get
            Return _SampleRate
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SampleSize As Integer
        Get
            Return _PreProcSamples.Length
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Browsable(False)>
    Public ReadOnly Property Samples As Double() Implements ITimeSeries(Of Double()).Samples
        Get
            Return _PreProcSamples
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Browsable(False)>
    Public ReadOnly Property Frames As Double()() Implements ITimeSeries(Of Double()()).Samples
        Get
            Return _PostProcFrames
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property FrameCount As Integer
        Get
            Return _PostProcFrames.Length
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property FrameLength As Integer
        Get
            If _PostProcFrames(0) Is Nothing OrElse _PostProcFrames(0).Length = 0 Then Return 0
            Return _PostProcFrames(0).Length
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="index"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Default Public ReadOnly Property Frame(index As Integer) As Double()
        Get
            Return _PostProcFrames(index)
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Energy As Double Implements ITimeSeries.Energy
        Get
            Dim totalEnergy As Double = 0
            For i As Integer = 0 To _PreProcSamples.Length - 1
                totalEnergy += (_PreProcSamples(i) * _PreProcSamples(i))
            Next
            Return totalEnergy
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Power As Double Implements ITimeSeries.Power
        Get
            Return Me.Energy / _PreProcSamples.Length
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Norm As Double Implements ITimeSeries.Norm
        Get
            Return Math.Sqrt(Me.Energy)
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property StdDev As Double Implements ITimeSeries.StdDev
        Get
            Return Math.Sqrt(Me.Variance)
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property StdErr As Double Implements ITimeSeries.StdErr
        Get
            Return Math.Sqrt(Me.Variance / Me._Samples.Length)
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Variance As Double Implements ITimeSeries.Variance
        Get
            Dim mean As Double
            Dim mean_last As Double
            Dim var As Double
            For i As Integer = 0 To _PreProcSamples.Length - 1
                mean = mean_last + (_PreProcSamples(i) - mean_last) / (i + 1)
                var = (i * var + (_PreProcSamples(i) - mean) * (_PreProcSamples(i) - mean_last)) / (i + 1)
                mean_last = mean
            Next
            Return var
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Min As Double Implements ITimeSeries.Min
        Get
            Return _PreProcSamples.Min
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Mean As Double Implements ITimeSeries.Mean
        Get
            Return _PreProcSamples.Average
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Max As Double Implements ITimeSeries.Max
        Get
            Return _PreProcSamples.Max
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Kurtosis As Double Implements ITimeSeries.Kurtosis
        Get
            Dim stddev As Double = Me.StdDev
            Dim mean As Double = Me.Mean
            Dim result As Double = 0
            For i As Integer = 0 To _PreProcSamples.Length - 1
                result += Math.Pow((_PreProcSamples(i) - mean) / stddev, 4)
            Next
            Return (result / _PreProcSamples.Length) - 3.0
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Skewness As Double Implements ITimeSeries.Skewness
        Get
            Dim stddev As Double = Me.StdDev
            Dim mean As Double = Me.Mean
            Dim result As Double = 0
            For i As Integer = 0 To _PreProcSamples.Length - 1
                result += Math.Pow((_PreProcSamples(i) - mean) / stddev, 3)
            Next
            Return (result / _PreProcSamples.Length)
        End Get
    End Property

    ''' <summary>
    ''' Centroid of the Pre-processed samples.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Browsable(False)>
    Public ReadOnly Property Centroid As Double
        Get
            Return GetCentroidIndex(_PreProcSamples)
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Browsable(False)>
    Public ReadOnly Property CentroidIndices As Integer()
        Get

            Dim Indices As Integer() = New Integer(_PostProcFrames.Length - 1) {}

            For i As Integer = 0 To _PostProcFrames.Length - 1
                Indices(i) = GetCentroidIndex(_PostProcFrames(i))
            Next

            Return Indices

        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Browsable(False)>
    Public ReadOnly Property CentroidValues As Double()
        Get

            Dim Values As Double() = New Double(_PostProcFrames.Length - 1) {}

            For i As Integer = 0 To CentroidIndices.Length - 1
                If Me.CentroidIndices(i) = -1 Then
                    Values(i) = 0
                Else
                    Values(i) = _PostProcFrames(i)(CentroidIndices(i))
                End If
            Next

            Return Values

        End Get
    End Property

    Private Function GetCentroidIndex(window As Double()) As Integer

        Dim max As Double = Double.MinValue
        Dim index As Integer = -1

        For i As Integer = 0 To window.Length - 1
            If Math.Abs(window(i)) > max Then
                max = window(i)
                index = i
            End If
        Next

        Return index

    End Function

#Region " -- IEquatable(Of TimeSeries) -- "

    Public Overloads Function Equals(other As TimeSeries) As Boolean Implements IEquatable(Of TimeSeries).Equals
        If other Is Nothing Then Return False
        Return Me.GetHashCode = other.GetHashCode
    End Function

#End Region

#Region " -- Object Overrides -- "

    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then Return False
        Dim series As TimeSeries = TryCast(obj, TimeSeries)
        If series Is Nothing Then
            Return False
        Else
            Return Equals(series)
        End If
    End Function

    Public Overrides Function GetHashCode() As Integer
        'Return CInt(((((_PreProcSamples.Max * 251) + _PreProcSamples.Average) * 251) + _PreProcSamples.Min) * 251 + _PreProcSamples.Sum)
        Return _Samples.GetHashCode
    End Function

    Public Overrides Function ToString() As String
        ' TODO : 
        Return MyBase.ToString()
    End Function

#End Region

#Region " -- Operators -- "

    Public Shared Operator =(series1 As TimeSeries, series2 As TimeSeries) As Boolean
        If series1 Is Nothing OrElse series2 Is Nothing Then
            Return Object.Equals(series1, series2)
        End If
        Return series1.Equals(series2)
    End Operator

    Public Shared Operator <>(series1 As TimeSeries, series2 As TimeSeries) As Boolean
        If series1 Is Nothing OrElse series2 Is Nothing Then
            Return Not Object.Equals(series1, series2)
        End If
        Return Not series1.Equals(series2)
    End Operator

#End Region

End Class
