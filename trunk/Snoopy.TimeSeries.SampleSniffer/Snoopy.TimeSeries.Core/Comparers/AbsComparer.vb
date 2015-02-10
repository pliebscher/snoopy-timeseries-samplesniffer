Public Class AbsComparer
    Implements IComparer(Of Double)

    Private Shared ReadOnly _Instance As New AbsComparer

    ''' <summary>
    '''   Compare descending
    ''' </summary>
    ''' <param name = "x">X (first item)</param>
    ''' <param name = "y">Y (second item)</param>
    ''' <returns>Return details related to magnitude comparison</returns>
    Public Function Compare(ByVal x As Double, ByVal y As Double) As Integer Implements IComparer(Of Double).Compare
        Return Math.Abs(y).CompareTo(Math.Abs(x))
    End Function

    Public Shared ReadOnly Property Instance As AbsComparer
        Get
            Return _Instance
        End Get
    End Property

End Class
