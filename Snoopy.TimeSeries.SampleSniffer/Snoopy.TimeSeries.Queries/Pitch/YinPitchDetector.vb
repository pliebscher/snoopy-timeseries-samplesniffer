''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class YinPitchDetector
    Implements IPitchDetector

    Private Shared DEFAULT_THRESHOLD As Single = 0.1
    Private _Threshold As Double
    Private _SampleRate As Integer
    Private _YinBuffer() As Double
    Private _Probability As Double

    Public Sub New(ByVal audioSampleRate As Integer, ByVal bufferSize As Integer)
        Me.New(audioSampleRate, bufferSize, DEFAULT_THRESHOLD)
    End Sub

    Public Sub New(ByVal audioSampleRate As Integer, ByVal bufferSize As Integer, ByVal yinThreshold As Double)
        Me._SampleRate = audioSampleRate
        Me._Threshold = yinThreshold
        _YinBuffer = New Double(CInt(bufferSize / 2) - 1) {}
    End Sub

    Public Function GetPitch(ByVal samples As Double()) As Pitch Implements IPitchDetector.GetPitch

        Dim tauEstimate As Integer = -1
        Dim pitchInHertz As Double = -1

        '	// step 2
        difference(samples)

        '	// step 3
        cumulativeMeanNormalizedDifference()

        '	// step 4
        tauEstimate = absoluteThreshold()

        '	// step 5
        If (tauEstimate <> -1) Then
            Dim betterTau As Double = parabolicInterpolation(tauEstimate)
            pitchInHertz = _SampleRate / betterTau
        End If

        Return New Pitch(pitchInHertz, _Probability)
    End Function

    Private Sub difference(ByVal audioBuffer As Double())
        'Dim index, tau As Integer
        Dim delta As Double
        For i As Integer = 0 To _YinBuffer.Length - 1
            _YinBuffer(i) = 0
        Next
        For offset As Integer = 1 To _YinBuffer.Length - 1
            For i As Integer = 0 To _YinBuffer.Length - 1
                Try
                    delta = audioBuffer(i) - audioBuffer(i + offset)
                    _YinBuffer(offset) += (delta * delta)
                Catch ex As Exception
                    Throw
                End Try
                
            Next
        Next
    End Sub

    Private Sub cumulativeMeanNormalizedDifference()
        Dim tau As Integer
        _YinBuffer(0) = 1
        Dim runningSum As Double = 0
        For tau = 1 To _YinBuffer.Length - 1
            runningSum += _YinBuffer(tau)
            _YinBuffer(tau) *= CSng(tau / runningSum)
        Next
    End Sub

    Private Function absoluteThreshold() As Integer
        Dim tau As Integer
        For tau = 2 To _YinBuffer.Length - 1
            If (_YinBuffer(tau) < _Threshold) Then
                Do While (tau + 1 < _YinBuffer.Length AndAlso _YinBuffer(tau + 1) < _YinBuffer(tau))
                    tau += 1
                Loop
                _Probability = 1 - _YinBuffer(tau)
                Exit For
            End If
        Next

        If (tau = _YinBuffer.Length OrElse _YinBuffer(tau) >= _Threshold) Then
            tau = -1
            _Probability = 0
        End If

        Return tau
    End Function

    Private Function parabolicInterpolation(ByVal tauEstimate As Integer) As Double
        Dim betterTau As Double
        Dim x0 As Integer
        Dim x2 As Integer

        If (tauEstimate < 1) Then
            x0 = tauEstimate
        Else
            x0 = tauEstimate - 1
        End If
        If (tauEstimate + 1 < _YinBuffer.Length) Then
            x2 = tauEstimate + 1
        Else
            x2 = tauEstimate
        End If
        If (x0 = tauEstimate) Then
            If (_YinBuffer(tauEstimate) <= _YinBuffer(x2)) Then
                betterTau = tauEstimate
            Else
                betterTau = x2
            End If
        ElseIf (x2 = tauEstimate) Then
            If (_YinBuffer(tauEstimate) <= _YinBuffer(x0)) Then
                betterTau = tauEstimate
            Else
                betterTau = x0
            End If
        Else
            Dim s0, s1, s2 As Double
            s0 = _YinBuffer(x0)
            s1 = _YinBuffer(tauEstimate)
            s2 = _YinBuffer(x2)
            betterTau = tauEstimate + (s2 - s0) / (2 * (2 * s1 - s2 - s0))
        End If
        Return betterTau
    End Function

End Class
