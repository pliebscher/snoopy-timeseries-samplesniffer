Namespace Windows
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class Window
        Implements IITimeSeriesWindow

        Public Shared ReadOnly Hanning As Window = New HanningWindow
        Public Shared ReadOnly Hamming As Window = New HammingWindow
        Public Shared ReadOnly Barlett As Window = New BartlettWindow
        Public Shared ReadOnly Blackman As Window = New BlackmanWindow
        Public Shared ReadOnly Flattop As Window = New FlattopWindow
        Public Shared ReadOnly Gaussian As Window = New Gaussian
        Public Shared ReadOnly Kaiser As Window = New KaiserWindow
        Public Shared ReadOnly Box As Window = New Box
        Public Shared ReadOnly Empty As Window = New EmptyWindow

        Public MustOverride Function GetWindow(ByVal N As Integer) As Double() Implements IITimeSeriesWindow.GetWindow

        Public Shared Function GetInstance(windowType As WindowType) As Window
            Select Case windowType
                Case windowType.Hanning
                    Return Window.Hanning
                Case windowType.Hamming
                    Return Window.Hamming
                Case windowType.Barlett
                    Return Window.Barlett
                Case windowType.Blackman
                    Return Window.Blackman
                Case windowType.Flattop
                    Return Window.Flattop
                Case Windows.WindowType.Kaiser
                    Return Window.Kaiser
                Case windowType.Box
                    Return Window.Box
                Case windowType.Gaussian
                    Return Window.Gaussian
                Case windowType.None
                    Return Window.Empty
                Case Else
                    Return Window.Empty
            End Select
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetType.Name & " Window"
        End Function

    End Class

    Public Enum WindowType
        Hanning
        Hamming
        Barlett
        Blackman
        Flattop
        Kaiser
        Box
        Gaussian
        None
    End Enum

End Namespace

