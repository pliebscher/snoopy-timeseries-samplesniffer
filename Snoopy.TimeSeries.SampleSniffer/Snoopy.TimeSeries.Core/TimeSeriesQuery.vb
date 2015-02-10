Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.Text
Imports System.ComponentModel
Imports System.Reflection
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Serializable, RefreshProperties(RefreshProperties.All)>
Public MustInherit Class TimeSeriesQuery

    Public Event IdChanged(sender As Object, e As EventArgs)
    Public Event VersionChanged(sender As Object, e As EventArgs)
    Public Event CriteriaChanged(sender As Object, e As EventArgs)

    Private _Id As String
    Private _Version As Integer = 1

    Private _Criteria As TimeSeries
    Private _MaxResults As Integer = 1
    Private _Results As List(Of TimeSeriesQueryResult)
    Private _IsInitialized As Boolean

    Private _MatchThreshHigh As Double = 1.0
    Private _MatchThreshLow As Double = 0.95

    Private _MatchSkipOnTrueResult As Integer = 0
    Private _MatchMergeOnSkipResult As Boolean = True
    Private _MatchMergeOnSkipMax As Integer = 2

    Private _LastMatchIgnored As Integer = 0

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="criteria"></param>
    ''' <remarks></remarks>
    Public Sub New(criteria As TimeSeries)
        Me.New(criteria, 1)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="criteria"></param>
    ''' <param name="maxResults"></param>
    ''' <remarks></remarks>
    Public Sub New(criteria As TimeSeries, maxResults As Integer)
        Me.New(criteria, maxResults, "")
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="criteria"></param>
    ''' <param name="maxResults"></param>
    ''' <param name="id"></param>
    ''' <remarks></remarks>
    Public Sub New(criteria As TimeSeries, maxResults As Integer, id As String)
        If criteria Is Nothing Then Throw New ArgumentNullException("criteria")
        If maxResults <= 0 Then Throw New ArgumentException("maxResults must be greater than zero.")
        _Criteria = criteria
        _MaxResults = maxResults
        _Results = New List(Of TimeSeriesQueryResult)(maxResults)
        _Id = id
    End Sub

    Public Sub Execute(data As TimeSeries, actionOnMatch As Action(Of TimeSeriesQueryResult))
        Me.Execute(data)
        If Me.Results.Count > 0 Then
            If Me.Results.Last.IsMatch Then
                actionOnMatch(Me.Results.Last)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Query the data and return a dis/similarity result. Result is stored as last value in Query.Results().
    ''' </summary>
    ''' <param name="data"></param>
    ''' <remarks></remarks>
    Public Sub Execute(data As TimeSeries)
        If data Is Nothing Then Throw New ArgumentNullException("data")
        If Not _IsInitialized Then
            Try
                Me.OnQueryInit(Me._Criteria)
            Catch qex As TimeSeriesQueryException
                Throw
            Catch ex As Exception
                Throw New TimeSeriesQueryException(String.Format("Error initializing query: {0}", ex.Message), ex, Me)
            End Try
        End If

        Dim val As Double = 0.0

        Try
            If data.Energy = 0 Then Exit Try ' Should we make this user selectable?
            val = Me.OnQueryExecute(data)
        Catch qex As TimeSeriesQueryException
            Throw
        Catch ex As Exception
            Throw New TimeSeriesQueryException(String.Format("Error executing query: {0}", ex.Message), ex, Me)
        End Try

        Dim res As New TimeSeriesQueryResult(Me, data, val)

        ' Are we skipping subsequent matches?
        If _MatchSkipOnTrueResult > 0 AndAlso (res.IsMatch And (Me._Results.Count > 0 AndAlso Me._Results.Last.IsMatch)) Then
            _LastMatchIgnored = 1
            res.IsMatch = False
            If _MatchMergeOnSkipResult Then
                Me._Results(Me._Results.Count - 1).Data = Me._Results(Me._Results.Count - 1).Data.Join(data)
            End If
        Else
            If _LastMatchIgnored > 0 AndAlso _LastMatchIgnored < _MatchSkipOnTrueResult Then
                _LastMatchIgnored += 1
                res.IsMatch = False
                If _MatchMergeOnSkipResult AndAlso _LastMatchIgnored <= _MatchMergeOnSkipMax Then
                    Me._Results(Me._Results.Count - _LastMatchIgnored).Data = Me._Results(Me._Results.Count - _LastMatchIgnored).Data.Join(data)
                End If
            Else
                _LastMatchIgnored = 0
            End If
        End If

        Me._Results.Add(res)
        Me.SyncResultSet()
    End Sub

    ''' <summary>
    ''' Query.Results is cleared and Query should reset internal parameters to default.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Reset()
        Me.Version += 1
        _Results.Clear()
        OnQueryReset(_Criteria)
    End Sub

    ''' <summary>
    ''' The Query has been updated. Query should update any internal criteria/parameters.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Update()
        Me.Version += 1
        Me.OnQueryUpdate(_Criteria)
    End Sub

    Protected MustOverride Function OnQueryExecute(data As TimeSeries) As Double

    Protected Overridable Sub OnQueryInit(criteria As TimeSeries)
        _IsInitialized = True
    End Sub

    Protected Overridable Sub OnQueryReset(criteria As TimeSeries)
    End Sub

    Protected Overridable Sub OnQueryUpdate(criteria As TimeSeries)
    End Sub

    Protected Overridable Sub OnQueryIdChanged(criteria As TimeSeries)
        RaiseEvent IdChanged(Me, EventArgs.Empty)
    End Sub

    Protected Overridable Sub OnQueryVersionChanged(criteria As TimeSeries)
        RaiseEvent VersionChanged(Me, EventArgs.Empty)
    End Sub

    Protected Overridable Sub OnQueryCriteriaChanged(criteria As TimeSeries)
        RaiseEvent CriteriaChanged(Me, EventArgs.Empty)
    End Sub

    Protected Overridable Sub OnQuerySave(properties As Dictionary(Of String, Object))

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

    Protected Overridable Sub OnQueryLoad(properties As Dictionary(Of String, Object))

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
    ''' Save query and all Read/Write properties.
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <remarks></remarks>
    Public Sub Save(filename As String)

        Dim fs As FileStream = Nothing
        Dim writer As XmlTextWriter = Nothing
        Dim serializer As DataContractSerializer = Nothing

        Try

            Dim query As New TSQuery With {.Criteria = Me._Criteria}

            query.Params.Add("_Id", _Id)
            Me.OnQuerySave(query.Params)

            query.Params.Add("_Type_", Me.GetType.AssemblyQualifiedName)

            fs = New FileStream(filename, FileMode.Create)
            writer = New XmlTextWriter(fs, Encoding.UTF8)
            serializer = New DataContractSerializer(GetType(TSQuery))

            serializer.WriteObject(writer, query)

        Catch ex As Exception
            Throw New TimeSeriesQueryException(String.Format("Error saving query: {0}", ex.Message), ex)
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
    ''' Load a query.
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Load(filename As String) As TimeSeriesQuery

        Dim tsq As TimeSeriesQuery = Nothing
        Dim fs As FileStream = Nothing
        Dim reader As XmlTextReader = Nothing
        Dim serializer As DataContractSerializer = Nothing

        Try
            serializer = New DataContractSerializer(GetType(TSQuery))
            fs = New FileStream(filename, FileMode.Open)
            reader = New XmlTextReader(fs)

            Dim params As TSQuery = DirectCast(serializer.ReadObject(reader), TSQuery)

            tsq = TryCast(Activator.CreateInstance(Type.GetType(params.Params("_Type_").ToString), New Object() {params.Criteria}), TimeSeriesQuery)
            tsq.Id = params.Params("_Id").ToString
            tsq.OnQueryLoad(params.Params)

        Catch ex As Exception
            Throw New TimeSeriesQueryException(String.Format("Error loading query: {0}", ex.Message), ex)
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

        Return tsq
    End Function

    ''' <summary>
    ''' Create a query instance of the specified type with default parameters and the given criteria.
    ''' </summary>
    ''' <param name="queryType"></param>
    ''' <param name="criteria"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateInstance(queryType As Type, criteria As TimeSeries) As TimeSeriesQuery
        Try
            Return DirectCast(Activator.CreateInstance(queryType, New Object() {criteria}), TimeSeriesQuery)
        Catch ex As Exception
            Throw New TimeSeriesQueryException(String.Format("Error creating query instance. Type: {0}, Exception: {1}", queryType.FullName, ex.Message), ex)
        End Try
    End Function

    Private Sub SyncResultSet()
        If _Results.Count > _MaxResults Then
            _Results.RemoveRange(0, _Results.Count - _MaxResults)
        End If
    End Sub

    <Description("Query identifier.")>
    Public Property Id As String
        Get
            If String.IsNullOrWhiteSpace(_Id) Then Return Me.Name
            Return _Id
        End Get
        Set(value As String)
            If _Id <> value Then
                _Id = value
                Me.OnQueryIdChanged(_Criteria)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Query version.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Value is incremented each time the Query changes.</remarks>
    Public Property Version As Integer
        Get
            Return _Version
        End Get
        Private Set(value As Integer)
            If _Version <> value Then
                _Version = value
                Me.OnQueryVersionChanged(_Criteria)
            End If
        End Set
    End Property

    <Browsable(False)>
    Public Property Criteria As TimeSeries
        Get
            Return _Criteria
        End Get
        Set(value As TimeSeries)
            If value Is Nothing Then Throw New ArgumentNullException("value")
            If value <> _Criteria Then
                _Criteria = value
                Me.OnQueryCriteriaChanged(_Criteria)
            End If
        End Set
    End Property

    <XmlIgnore, Description("The set of results produced by this query. One Result is returned each time the query is executed, until MaxResults is reached.")>
    Public ReadOnly Property Results As List(Of TimeSeriesQueryResult)
        Get
            Return _Results
        End Get
    End Property

    <[ReadOnly](True), XmlIgnore, Description("The maximum nuber of results that will be kept. When this number is reached, the oldest Result is discarded for each new result added.")>
    Public Property MaxResults As Integer
        Get
            Return _MaxResults
        End Get
        Set(value As Integer)
            If value <> _MaxResults Then
                _MaxResults = value
                SyncResultSet()
            End If
        End Set
    End Property

    <XmlIgnore>
    Public Overridable ReadOnly Property Name As String
        Get
            Return Me.GetType.Name
        End Get
    End Property

    <XmlIgnore>
    Public Overridable ReadOnly Property Description As String
        Get
            Return String.Empty
        End Get
    End Property

    <Description("Upper match threshhold. For this query to be considered a ""Match"", the ""Result"" should be between MatchThreshHigh and MatchThreshLow."), DefaultValue(1.0)>
    Public Property MatchThreshHigh As Double
        Get
            Return _MatchThreshHigh
        End Get
        Set(value As Double)
            _MatchThreshHigh = value
        End Set
    End Property

    <Description("Lower match threshhold. For this query to be considered a ""Match"", the ""Result"" should be between MatchThreshHigh and MatchThreshLow."), DefaultValue(0.95)>
    Public Property MatchThreshLow As Double
        Get
            Return _MatchThreshLow
        End Get
        Set(value As Double)
            _MatchThreshLow = value
        End Set
    End Property

    <Description("Number of subsequent matches to ignore after each match. Set this if a single match may be spread over multiple data frames."), DefaultValue(0)>
    Public Property MatchSkipOnTrueResult As Integer
        Get
            Return _MatchSkipOnTrueResult
        End Get
        Set(value As Integer)
            If value < 0 Then Throw New ArgumentException("MatchSkipOnTrueResult must be greater then Zero.")
            _MatchSkipOnTrueResult = value
            If _MatchMergeOnSkipMax = 0 OrElse _MatchMergeOnSkipMax > _MatchSkipOnTrueResult Then
                _MatchMergeOnSkipMax = value
            End If
        End Set
    End Property

    <Description("If True and MatchSkipOnTrueResult > 0, then append the skipped matches to the first match."), DefaultValue(True)>
    Public Property MatchMergeOnSkipResult As Boolean
        Get
            Return _MatchMergeOnSkipResult
        End Get
        Set(value As Boolean)
            _MatchMergeOnSkipResult = value
        End Set
    End Property

    <Description("Maximum subsequent results to merge if MatchMergeOnSkipResult = True."), DefaultValue(2)>
    Public Property MatchMergeOnSkipMax As Integer
        Get
            Return _MatchMergeOnSkipMax
        End Get
        Set(value As Integer)
            If value < 0 Then Throw New ArgumentException("MatchMergeOnSkipMax must be greater then Zero.")
            _MatchMergeOnSkipMax = value
        End Set
    End Property

    ''' <summary>
    ''' Container class to work around serialization of derived queries. This way we don't need to serialize the entire query, which won't
    ''' work anyway!
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable>
    Friend Class TSQuery
        Public Params As New Dictionary(Of String, Object)
        Public Criteria As TimeSeries
    End Class

End Class

Public Class MetaAttribute
    Public Property Name As String = "New Attribue"
    Public Property Data As String

    Public Overrides Function ToString() As String
        Return Me.Name
    End Function
End Class
