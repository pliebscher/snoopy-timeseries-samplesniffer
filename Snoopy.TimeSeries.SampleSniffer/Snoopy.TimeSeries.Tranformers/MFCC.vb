Imports System.ComponentModel
''' <summary>
''' Where did I piece this together from?
''' </summary>
''' <remarks></remarks>
Public Class MFCC
    Inherits TimeSeriesFrameTransformer

    Private _FilterBankMatrix As Double()()
    Private _DCTMatrix As Double()()

    Private _KeepZeroTh As Boolean
    Private _CMN As Boolean = False
    Private _ISIP As Boolean = False

    Private _NumFilters As Integer = 40
    Private _NumCoefficients As Integer = 32
    Private _MinFrequency As Double = 133.3333
    Private _MaxFrequency As Double = 1855.555

    Private _SampleRate As Integer = 22050
    Private _FrameWidth As Integer = 512

    Private _FFT As New FFT
    Private _Phase As FFT.Phase = FFT.Phase.Default

    Public Sub New()

        MyBase.FrameStep = FrameStep._64
        MyBase.FrameWidth = FrameWidth._512

        InitMatrices()
        InitCMN()
    End Sub

    Protected Overloads Overrides Function OnTransformFrame(frame As Double()) As Double()

        Dim complexSignal As Double() = New Double(2 * _FrameWidth - 1) {}

        For j As Integer = 0 To _FrameWidth - 1
            complexSignal(2 * j) = frame(j)
            complexSignal(2 * j + 1) = 0
        Next

        _FFT.Complex(complexSignal, True)

        Dim real As Double() = New Double(_FrameWidth - 1) {}
        For n As Integer = 0 To _FrameWidth - 1
            Dim re As Double = complexSignal(2 * n)
            Dim im As Double = complexSignal(2 * n + 1)
            If Not _ISIP Then
                real(n) = Math.Sqrt(re * re + im * im) ' <---- Mag spect.
            Else
                real(n) = Math.Log(Math.Sqrt(re * re + im * im))
            End If
        Next

        Dim mfcc As Double() = Apply(real)

        If _CMN Then
            Normalize(mfcc)
        End If

        Return mfcc

    End Function

    Protected Overrides Sub OnFrameWidthChanged(width As Integer)
        _FrameWidth = width
        InitMatrices()
        MyBase.OnFrameWidthChanged(width)
    End Sub

    Protected Overrides Sub OnSampleRateChanged(sampleRate As Integer)
        InitMatrices()
        MyBase.OnSampleRateChanged(sampleRate)
    End Sub

    Private Function HzToMel(freq As Double) As Double
        ' dmelFrequencies[k] = 2595.0*( Math.log(1.0 + (dhzFrequencies[k] / 700.0) ) / Math.log(10) );
        Return 2595.0 * (Math.Log(1.0 + (freq / 700.0)) / Math.Log(10.0))
    End Function

    Private Function MelToHz(freq As Double) As Double
        Return (700.0 * (Math.Pow(10.0, (freq / 2595.0)) - 1.0))
    End Function

    Private Function GetFilterWeight(filterBank As Integer, freq As Double, boundaries As Double()) As Double
        'for most frequencies the filter weight is 0
        Dim result As Double = 0

        'compute start- , center- and endpoint as well as the height of the filter
        Dim start As Double = boundaries(filterBank - 1)
        Dim center As Double = boundaries(filterBank)
        Dim [end] As Double = boundaries(filterBank + 1)
        Dim height As Double = 2.0 / ([end] - start)

        'is the frequency within the triangular part of the filter
        If freq >= start AndAlso freq <= [end] Then
            'depending on frequency position within the triangle
            If freq < center Then
                '...use a ascending linear function
                result = (freq - start) * (height / (center - start))
            Else
                '..use a descending linear function
                result = height + ((freq - center) * ((-height) / ([end] - center)))
            End If
        End If

        Return result
    End Function

    Private Function Apply(frame As Double()) As Double()

        Dim result As Double() = New Double(_NumFilters - 1) {}
        Dim sum As Double = 0.0

        ' Calculate the filterbank response
        For i As Integer = 0 To _NumFilters - 1
            sum = 0.0
            For k As Integer = 0 To _FrameWidth - 1
                sum += _FilterBankMatrix(i)(k) * frame(k)
            Next
            If sum > 0.0 Then
                result(i) = Math.Log10(sum)
            Else
                result(i) = 0.0
            End If
        Next

        Dim start As Integer = 0
        If Not _KeepZeroTh Then
            start = 1
        End If
        Dim dct As Double() = New Double((_DCTMatrix.Length - 1) - start) {}
        ' Take the DCT 
        For i As Integer = start To _DCTMatrix.Length - 1
            sum = 0.0
            For k As Integer = 0 To _NumFilters - 1
                sum += _DCTMatrix(i)(k) * result(k)
            Next
            dct(i - start) = sum
        Next

        Return dct

    End Function

    Private Sub InitMatrices()

        _FilterBankMatrix = New Double(_NumFilters - 1)() {}

        Dim Centers As Double() = New Double(_NumFilters + 1) {}
        Dim BaseFreq As Double = _SampleRate / _FrameWidth
        Dim MaxFreqMel As Double
        Dim MinFreqMel As Double
        Dim DeltaFreqMel As Double
        Dim NextCenterMel As Double

        'compute mel min./max. frequency
        MinFreqMel = HzToMel(_MinFrequency)
        MaxFreqMel = HzToMel(_MaxFrequency)
        DeltaFreqMel = (MaxFreqMel - MinFreqMel) / (_NumFilters + 1)

        'create (numberFilters + 2) equidistant points for the triangles
        NextCenterMel = MinFreqMel
        For i As Integer = 0 To Centers.Length - 2
            'transform the points back to linear scale
            Centers(i) = MelToHz(NextCenterMel)
            NextCenterMel += DeltaFreqMel
        Next

        'ajust boundaries to exactly fit the given min./max. frequency
        Centers(0) = _MinFrequency
        Centers(Centers.Length - 1) = _MaxFrequency

        'fill each row of the filter bank matrix with one triangular mel filter
        For i As Integer = 1 To _NumFilters
            Dim filter As Double() = New Double(_FrameWidth - 1) {} ' New Double((CInt(windowSize / 2) - 1)) {}
            'for each frequency of the fft
            For j As Integer = 0 To filter.Length - 1
                'compute the filter weight of the current triangular mel filter
                Dim freq As Double = BaseFreq * j
                filter(j) = GetFilterWeight(i, freq, Centers)
            Next
            'add the computed mel filter to the filter bank
            _FilterBankMatrix(i - 1) = filter
        Next

        ' ----------------------------------------------------------------------------------------------
        ' DCT Matrix...
        Dim k As Double = Math.PI / _NumFilters
        Dim w1 As Double = 1.0 / (Math.Sqrt(_NumFilters))
        Dim w2 As Double = Math.Sqrt(2.0 / _NumFilters)

        _DCTMatrix = New Double(_NumCoefficients - 1)() {}

        For i As Integer = 0 To _DCTMatrix.Length - 1
            _DCTMatrix(i) = New Double(_NumFilters - 1) {}
            For j As Integer = 0 To _NumFilters - 1
                If i = 0 Then
                    _DCTMatrix(i)(j) = w1 * Math.Cos(k * i + 1 * (j + 0.5))
                Else
                    _DCTMatrix(i)(j) = w2 * Math.Cos(k * (i + 1) * (j + 0.5))
                End If
            Next
        Next

    End Sub

    <Description("Keep the 0th coefficient?"), DefaultValue(False)>
    Public Property KeepZeroTh As Boolean
        Get
            Return _KeepZeroTh
        End Get
        Set(value As Boolean)
            If value <> _KeepZeroTh Then
                _KeepZeroTh = value
                InitMatrices()
            End If
        End Set
    End Property

    <Description("Number of Mel filters."), DefaultValue(40)>
    Public Property Filters As Integer
        Get
            Return _NumFilters
        End Get
        Set(value As Integer)
            If value <> _NumFilters Then
                _NumFilters = value
                InitMatrices()
            End If
        End Set
    End Property

    <Description("The resulting number of Cepstrum Coefficients. Should be greater than number of filters."), DefaultValue(32)>
    Public Property Coefficients As Integer
        Get
            Return _NumCoefficients
        End Get
        Set(value As Integer)
            If value <> _NumCoefficients Then
                _NumCoefficients = value
                InitMatrices()
                InitCMN()
            End If
        End Set
    End Property

    <Description("Maximum Frequency"), DefaultValue(1855.555)>
    Public Property MaxFrequency As Double
        Get
            Return _MaxFrequency
        End Get
        Set(value As Double)
            If value <> _MaxFrequency Then
                _MaxFrequency = value
                InitMatrices()
            End If
        End Set
    End Property

    <Description("Minimum Frequency"), DefaultValue(133.3333)>
    Public Property MinFrequency As Double
        Get
            Return _MinFrequency
        End Get
        Set(value As Double)
            If value <> _MinFrequency Then
                _MinFrequency = value
                InitMatrices()
            End If
        End Set
    End Property

    <Description("FFT phase mode."), DefaultValue(FFT.Phase.Default)>
    Public Property FFTPhase As FFT.Phase
        Get
            Return _Phase
        End Get
        Set(value As FFT.Phase)
            If value <> _Phase Then
                _Phase = value
                _FFT.PhaseMode = _Phase
                InitCMN()
            End If
        End Set
    End Property

    <Description("ISIP (Mississipi univ.) implementation. Takes Log of FFT Mag spectrum."), DefaultValue(False)>
    Public Property ISIP As Boolean
        Get
            Return _ISIP
        End Get
        Set(value As Boolean)
            If value <> _ISIP Then
                _ISIP = value
            End If
        End Set
    End Property

    <Description("Cepstrum Mean Normalization. This is a running calculation and should be used for real-time processing."), DefaultValue(False)>
    Public Property CMN As Boolean
        Get
            Return _CMN
        End Get
        Set(value As Boolean)
            If value <> _CMN Then
                _CMN = value
                If value Then
                    InitCMN()
                End If
            End If
        End Set
    End Property

    <Description("# of Cepstrum to recalculate mean."), DefaultValue(160)>
    Public Property CMNShift As Integer
        Get
            Return _CMNShiftWindow
        End Get
        Set(value As Integer)
            If value <> _CMNShiftWindow Then
                _CMNShiftWindow = value
            End If
        End Set
    End Property

    <Description("The CMN window size."), DefaultValue(100)>
    Public Property CMNWindow As Integer
        Get
            Return _CMNWindow
        End Get
        Set(value As Integer)
            If value <> _CMNWindow Then
                _CMNWindow = value
            End If
        End Set
    End Property

    <Description("The CMN initial mean, magic number."), DefaultValue(12.0)>
    Public Property CMNMean As Double
        Get
            Return _InitialMean
        End Get
        Set(value As Double)
            If value <> _InitialMean Then
                _InitialMean = value
                InitCMN()
            End If
        End Set
    End Property

#Region " -- Cepstrum Mean Normalization (CMN) -- "

    ''' <summary>
    ''' Cepstrum Mean Normalization
    ''' http://sourceforge.net/p/cmusphinx/code/11917/tree/trunk/sphinx4/src/sphinx4/edu/cmu/sphinx/frontend/feature/LiveCMN.java
    ''' </summary>
    ''' <remarks></remarks>

    Private _InitialMean As Double = 12 ' initial mean, magic number
    Private _CMNWindow As Integer = 100 ' The property for the live CMN window size. 
    Private _CMNShiftWindow As Integer = 160 ' # of Cepstrum to recalculate mean
    Private _CurrentMean As Double() ' array of current means
    Private _CMNRunningSum As Double() ' array of current sums
    Private _CMNRunningFrameNum As Integer ' total number of input Cepstrum

    Private Sub InitCMN()
        _CurrentMean = New Double(_NumCoefficients - 1) {}
        _CurrentMean(0) = _InitialMean
        _CMNRunningSum = New Double(_NumCoefficients - 1) {}
        _CMNRunningFrameNum = 0
    End Sub

    Private Sub Normalize(cepstrum As Double())

        For j As Integer = 0 To cepstrum.Length - 1
            _CMNRunningSum(j) += cepstrum(j)
            cepstrum(j) -= _CurrentMean(j)
        Next

        _CMNRunningFrameNum += 1

        If _CMNRunningFrameNum > _CMNShiftWindow Then
            updateMeanSumBuffers()
        End If

    End Sub

    ''' <summary>
    ''' Updates the currentMean buffer with the values in the sum buffer. Then decay the sum buffer exponentially, i.e.,
    ''' divide the sum with numberFrames.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub updateMeanSumBuffers()

        If _CMNRunningFrameNum > 0 Then

            ' update the currentMean buffer with the sum buffer
            Dim sf As Double = 1.0 / _CMNRunningFrameNum

            Array.Copy(_CMNRunningSum, 0, _CurrentMean, 0, _CMNRunningSum.Length)

            multiplyArray(_CurrentMean, sf)

            ' decay the sum buffer exponentially
            If _CMNRunningFrameNum >= _CMNShiftWindow Then
                multiplyArray(_CMNRunningSum, (sf * _CMNWindow))
                _CMNRunningFrameNum = _CMNWindow
            End If
        End If
    End Sub

    ''' <summary>
    ''' Multiplies each element of the given array by the multiplier.
    ''' </summary>
    ''' <param name="array"></param>
    ''' <param name="multiplier"></param>
    ''' <remarks></remarks>
    Private Shared Sub multiplyArray(array As Double(), multiplier As Double)
        For i As Integer = 0 To array.Length - 1
            array(i) *= multiplier
        Next
    End Sub

#End Region

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Mel-Frequency Cepstral Coefficients"
        End Get
    End Property

End Class
