''' <summary>
''' http://www.schmittmachine.com/dywapitchtrack.html
''' </summary>
''' <remarks></remarks>
Public Class DynamicWaveletPitchDetector
    Implements IPitchDetector

    Private _MaxFLWTLevels As Integer = 6
    Private _DiffLevelsN As Integer = 3

    Private _SampleRate As Double
    Private _MaxFreq As Double = 3000
    Private _MaximThreshholdRatio As Double = 0.5

    Private _LastPitchHertz As Double = -1
    Private _LastPitchProb As Double = -1

    Public Sub New(sampleRate As Double, maxFreq As Double, maxThreshRation As Double) ', buffSize As Integer)
        _SampleRate = sampleRate
        _MaxFreq = maxFreq
        _MaximThreshholdRatio = maxThreshRation
    End Sub

    Public Function GetPitch(ByVal samples As Double()) As Pitch Implements IPitchDetector.GetPitch

        Dim SampBuff As Double() = New Double(samples.Length - 1) {}
        Array.Copy(samples, SampBuff, samples.Length)

        Dim Pitch As Double = -1
        Dim curSamNb As Integer = samples.Length
        Dim nbMins As Integer
        Dim nbMaxs As Integer

        Dim Mins As Integer() = New Integer(SampBuff.Length - 1) {}
        Dim Maxs As Integer() = New Integer(SampBuff.Length - 1) {}

        Dim ampltitudeThreshold As Double
        Dim theDC As Double = 0.0

        'compute ampltitudeThreshold and theDC
        'first compute the DC and maxAMplitude
        Dim maxValue As Double = 0.0
        Dim minValue As Double = 0.0
        For i As Integer = 0 To SampBuff.Length - 1
            Dim sample As Double = SampBuff(i)
            theDC = theDC + sample
            maxValue = Math.Max(maxValue, sample)
            minValue = Math.Min(sample, minValue)
        Next

        theDC = theDC / SampBuff.Length
        maxValue = maxValue - theDC
        minValue = minValue - theDC

        Dim amplitudeMax As Double = (If(maxValue > -minValue, maxValue, -minValue))
        ampltitudeThreshold = amplitudeMax * _MaximThreshholdRatio

        ' levels, start without downsampling.
        Dim curLevel As Integer = 0
        Dim curModeDistance As Double = -1
        Dim delta As Integer

        While True

            delta = CInt(_SampleRate / (Math.Pow(2, curLevel) * _MaxFreq))
            If curSamNb < 2 Then Exit While

            ' compute the first maximums and minumums after zero-crossing
            ' store if greater than the min threshold
            ' and if at a greater distance than delta
            Dim dv As Double, previousDV As Double = -1000

            nbMins = 0
            nbMaxs = 0

            Dim lastMinIndex As Integer = -1000000
            Dim lastmaxIndex As Integer = -1000000
            Dim findMax As [Boolean] = False
            Dim findMin As [Boolean] = False

            ' for (int i = 2; i < curSamNb; i++)
            For i As Integer = 2 To curSamNb - 1

                Dim si As Double = SampBuff(i) - theDC
                Dim si1 As Double = SampBuff(i - 1) - theDC

                If si1 <= 0 And si > 0 Then
                    findMax = True
                End If

                If si1 >= 0 And si < 0 Then
                    findMin = True
                End If

                ' min or max ?
                dv = si - si1

                ' if (previousDV > -1000) {
                If previousDV > -1000 Then
                    ' if (findMin && previousDV < 0 && dv >= 0) {
                    If findMin And previousDV < 0 And dv >= 0 Then
                        ' minimum
                        If Math.Abs(si) >= ampltitudeThreshold Then
                            If i > lastMinIndex + delta Then
                                ' 	mins[nbMins++] = i;
                                Mins(System.Math.Max(System.Threading.Interlocked.Increment(nbMins), nbMins - 1)) = i ' ???
                                lastMinIndex = i
                                findMin = False
                            End If
                        End If
                    End If

                    If findMax And previousDV > 0 And dv <= 0 Then
                        ' maximum
                        If Math.Abs(si) >= ampltitudeThreshold Then
                            If i > lastmaxIndex + delta Then
                                ' 	maxs[nbMaxs++] = i;
                                Maxs(System.Math.Max(System.Threading.Interlocked.Increment(nbMaxs), nbMaxs - 1)) = i ' ???
                                lastmaxIndex = i
                                findMax = False
                            End If
                        End If
                    End If

                End If

                previousDV = dv

            Next

            ' if (nbMins == 0 && nbMaxs == 0) {
            If nbMins = 0 And nbMaxs = 0 Then
                Exit While
            End If

            Dim d As Integer
            Dim Distances As Integer() = New Integer(SampBuff.Length - 1) {}

            For i As Integer = 0 To nbMins - 1
                For j As Integer = 1 To _DiffLevelsN - 1
                    If i + j < nbMins Then
                        d = Math.Abs(Mins(i) - Mins(i + j))
                        'asLog("dywapitch i=%ld j=%ld d=%ld\n", i, j, d);
                        Distances(d) = Distances(d) + 1
                    End If
                Next
            Next

            Dim bestDistance As Integer = -1
            Dim bestValue As Integer = -1
            For i As Integer = 0 To curSamNb - 1
                Dim summed As Integer = 0
                For j As Integer = -delta To delta
                    If i + j >= 0 And i + j < curSamNb Then
                        summed += Distances(i + j)
                    End If
                Next
                'asLog("dywapitch i=%ld summed=%ld bestDistance=%ld\n", i, summed, bestDistance);
                If summed = bestValue Then
                    If i = 2 * bestDistance Then
                        bestDistance = i
                    End If
                ElseIf summed > bestValue Then
                    bestValue = summed
                    bestDistance = i
                End If
            Next

            ' averaging
            Dim distAvg As Double = 0.0
            Dim nbDists As Double = 0
            For j As Integer = -delta To delta
                If bestDistance + j >= 0 And bestDistance + j < SampBuff.Length Then
                    Dim nbDist As Integer = Distances(bestDistance + j)
                    If nbDist > 0 Then
                        nbDists += nbDist
                        distAvg += (bestDistance + j) * nbDist
                    End If
                End If
            Next

            ' this is our mode distance !
            distAvg /= nbDists

            ' continue the levels ?
            If curModeDistance > -1 Then
                Dim similarity As Double = Math.Abs(distAvg * 2 - curModeDistance)
                If similarity <= 2 * delta Then
                    ' two consecutive similar mode distances : ok !
                    Pitch = _SampleRate / (Math.Pow(2, curLevel - 1) * curModeDistance)
                    ' break search;;
                    Exit While
                End If
            End If

            ' not similar, continue next level
            curModeDistance = distAvg
            curLevel = curLevel + 1

            If curLevel >= _MaxFLWTLevels Then
                Exit While
            End If

            ' downsample
            If curSamNb < 2 Then
                Exit While
            End If

            For i As Integer = 0 To curSamNb \ 2 - 1
                SampBuff(i) = (SampBuff(2 * i) + SampBuff(2 * i + 1)) / 2
            Next
            curSamNb \= 2

        End While

        ' ------------------------------------------------------------------------------------------
        ' Calc probability...

        ' equivalence
        If Pitch = 0.0 Then Pitch = -1.0

        Dim estimatedPitch As Double = -1
        Dim acceptedError As Double = 0.2F
        Dim maxConfidence As Integer = 5
        'Dim Probability As Double = -1

        If Pitch <> -1 Then

            If _LastPitchHertz = -1 Then
                ' no previous
                estimatedPitch = Pitch
                _LastPitchHertz = Pitch
                _LastPitchProb = 1

            ElseIf Math.Abs(_LastPitchHertz - Pitch) / Pitch < acceptedError Then

                ' similar : remember and increment pitch
                _LastPitchHertz = Pitch
                estimatedPitch = Pitch
                _LastPitchProb = Math.Min(maxConfidence, _LastPitchProb + 1) ' maximum 3

            ElseIf (_LastPitchProb >= maxConfidence - 2) And Math.Abs(_LastPitchHertz - 2 * Pitch) / (2 * Pitch) < acceptedError Then

                ' close to half the last pitch, which is trusted
                estimatedPitch = 2 * Pitch
                _LastPitchHertz = estimatedPitch

            ElseIf (_LastPitchProb >= maxConfidence - 2) And Math.Abs(_LastPitchHertz - 0.5 * Pitch) / (0.5 * Pitch) < acceptedError Then

                ' close to twice the last pitch, which is trusted
                estimatedPitch = 0.5 * Pitch
                _LastPitchHertz = estimatedPitch

            Else
                ' nothing like this : very different value
                If _LastPitchProb >= 1 Then
                    ' previous trusted : keep previous
                    estimatedPitch = _LastPitchHertz
                    _LastPitchProb = Math.Max(0, _LastPitchProb - 1)
                Else
                    ' previous not trusted : take current
                    estimatedPitch = Pitch
                    _LastPitchHertz = Pitch
                    _LastPitchProb = 1
                End If
            End If

        Else
            ' no pitch now
            If _LastPitchHertz <> -1 Then
                ' was pitch before
                If _LastPitchProb >= 1 Then
                    ' continue previous
                    estimatedPitch = _LastPitchHertz
                    _LastPitchProb = Math.Max(0, _LastPitchProb - 1)
                Else
                    _LastPitchHertz = -1
                    estimatedPitch = -1
                    _LastPitchProb = 0
                End If
            End If
        End If

        If _LastPitchProb >= 1 Then
            ' ok
            Pitch = estimatedPitch
        Else
            Pitch = -1
        End If

        ' equivalence
        If Pitch = -1 Then
            Pitch = 0.0
        End If

        Return New Pitch(Pitch, _LastPitchProb)

    End Function


End Class
