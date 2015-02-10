Imports System.Threading
Imports System.Threading.Tasks
Public Class FormPleaseWait

    Private _Cancelled As Boolean
    Private _CancelCallback As Action

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        _Cancelled = True
        _CancelCallback()
        Me.Invoke(New MethodInvoker(AddressOf Me.Close), Nothing)
    End Sub

    Public Overloads Sub Show(parent As Control, executeAction As Action, cancelAction As Action)
        If executeAction Is Nothing Then Throw New ArgumentNullException("executeAction")
        If cancelAction Is Nothing Then Throw New ArgumentNullException("cancelAction")
        _Cancelled = False
        _CancelCallback = cancelAction
        Dim t As New Task(executeAction)
        Me.t = t
        t.Start()
        If parent IsNot Nothing Then
            Me.StartPosition = FormStartPosition.CenterParent
            Me.ShowDialog(parent)
        Else
            Me.ShowDialog()
        End If
    End Sub

    Public t As Task

    Public Overloads Sub Show(parent As Control, cancelAction As Action)
        Show(parent, Nothing, cancelAction)
    End Sub

    Public Overloads Sub Show(executeAction As Action, cancelAction As Action)
        Me.Show(Nothing, executeAction, cancelAction)
    End Sub

    Public Property Message As String
        Get
            Return Me.Text
        End Get
        Set(value As String)
            Me.Text = value
        End Set
    End Property

    Public Property Maximum() As Integer
        Get
            Return pbWait.Maximum
        End Get
        Set(value As Integer)
            pbWait.Maximum = value
        End Set
    End Property

    Public ReadOnly Property Cancelled As Boolean
        Get
            Return _Cancelled
        End Get
    End Property

    Public Sub SetProgress(value As Integer)
        If InvokeRequired Then
            Invoke(New Action(Sub() SetProgress(value)))
        Else
            If value > 0 AndAlso value >= pbWait.Maximum - 1 Then
                Me.Close()
            Else
                pbWait.Value = value
            End If
        End If
    End Sub

End Class