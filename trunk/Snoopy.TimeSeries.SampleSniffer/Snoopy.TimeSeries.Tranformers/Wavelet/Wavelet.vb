''' <summary>
''' https://github.com/hkrish/DiscreteWaveletExperiments/blob/master/DWT_Test/src/math/transform/jwave/handlers/wavelets/Wavelet.java
''' </summary>
''' <remarks></remarks>
Public MustInherit Class Wavelet

    Private _Length As Integer
    Private _Scales As Double()
    Private _Coeffs As Double()

    Public Sub New()
        _Scales = GetScales()
        _Coeffs = GetCoefficients()
        _Length = _Coeffs.Length
    End Sub

    Protected MustOverride Function GetCoefficients() As Double()
    Protected MustOverride Function GetScales() As Double()

    Public Shared Function Forward(samples As Double(), coeffs As Double(), scales As Double()) As Double()
        Dim arrHilb As Double() = New Double(samples.Length - 1) {}
        Dim k As Integer = 0
        Dim h As Integer = samples.Length >> 1
        For i As Integer = 0 To h - 1
            For j As Integer = 0 To coeffs.Length - 1
                k = (i << 1) + j
                While k >= samples.Length
                    k -= samples.Length
                End While
                arrHilb(i) += samples(k) * scales(j) ' low pass filter - energy (approximation)
                arrHilb(i + h) += samples(k) * coeffs(j) ' high pass filter - details
            Next
        Next
        Return arrHilb
    End Function

    Public Function Forward(samples As Double()) As Double()
        Return Forward(samples, _Coeffs, _Scales)
    End Function

    Public Function Reverse(samples As Double()) As Double()
        Dim arrTime As Double() = New Double(samples.Length - 1) {}
        Dim k As Integer = 0
        Dim h As Integer = samples.Length >> 1
        For i As Integer = 0 To h - 1
            For j As Integer = 0 To _Length - 1
                k = (i << 1) + j
                While k >= samples.Length
                    k -= samples.Length
                End While
                ' adding up details times energy (approximation)
                arrTime(k) += (samples(i) * _Scales(j) + samples(i + h) * _Coeffs(j))
                ' wavelet
            Next
        Next
        Return arrTime
    End Function

    Public ReadOnly Property Coefficients() As Double()
        Get
            Return _Coeffs
        End Get
    End Property

    Public ReadOnly Property Scales As Double()
        Get
            Return _Scales
        End Get
    End Property

End Class

Public Class Coif06
    Inherits Wavelet

    Private _sqrt15 As Double = Math.Sqrt(15)
    Private _coeffs As Double()
    Private _scales As Double()

    Protected Overrides Function GetCoefficients() As Double()
        Return _coeffs
    End Function

    Protected Overrides Function GetScales() As Double()
        _scales = New Double(5) {}
        _scales(0) = 1.4142135623730951 * (_sqrt15 - 3.0) / 32.0
        _scales(1) = 1.4142135623730951 * (10.0 - _sqrt15) / 32.0
        _scales(2) = 1.4142135623730951 * (6.0 - 2 * _sqrt15) / 32.0
        _scales(3) = 1.4142135623730951 * (2.0 * _sqrt15 + 6.0) / 32.0
        _scales(4) = 1.4142135623730951 * (_sqrt15 + 13.0) / 32.0
        _scales(5) = 1.4142135623730951 * (9.0 - _sqrt15) / 32.0

        _coeffs = New Double(5) {}
        _coeffs(0) = _scales(5)
        _coeffs(1) = -_scales(4)
        _coeffs(2) = _scales(3)
        _coeffs(3) = -_scales(2)
        _coeffs(4) = _scales(1)
        _coeffs(5) = -_scales(0)

        Return _scales
    End Function

End Class

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class Haar02
    Inherits Wavelet

    Private _coeffs As Double()
    Private _scales As Double()

    Protected Overrides Function GetCoefficients() As Double()
        _coeffs = New Double(1) {}
        _coeffs(0) = -0.7071067812
        _coeffs(1) = 0.7071067812
        Return _coeffs
    End Function

    Protected Overrides Function GetScales() As Double()
        _scales = New Double(1) {}
        _scales(0) = 0.7071067812
        _scales(1) = 0.7071067812
        Return _scales
    End Function

End Class

''' <summary>
''' http://wavelets.pybytes.com/wavelet/db2/
''' </summary>
''' <remarks></remarks>
Public Class Daub02
    Inherits Wavelet

    Private _coeffs As Double()
    Private _scales As Double()

    Protected Overrides Function GetCoefficients() As Double()
        _coeffs = New Double(3) {}
        _coeffs(0) = -0.4829629131
        _coeffs(1) = 0.8365163037
        _coeffs(2) = -0.224143868
        _coeffs(3) = -0.1294095226
        Return _coeffs
    End Function

    Protected Overrides Function GetScales() As Double()
        _scales = New Double(3) {}
        _scales(0) = -0.1294095226
        _scales(1) = 0.224143868
        _scales(2) = 0.8365163037
        _scales(3) = 0.4829629131
        Return _scales
    End Function

End Class

''' <summary>
''' http://wavelets.pybytes.com/wavelet/db3/
''' </summary>
''' <remarks></remarks>
Public Class Daub03
    Inherits Wavelet

    Private _coeffs As Double()
    Private _scales As Double()

    Protected Overrides Function GetCoefficients() As Double()
        _coeffs = New Double(5) {}
        _coeffs(0) = -0.332670553
        _coeffs(1) = 0.8068915093
        _coeffs(2) = -0.4598775021
        _coeffs(3) = -0.13501102
        _coeffs(4) = 0.0854412739
        _coeffs(5) = 0.0352262919
        Return _coeffs
    End Function

    Protected Overrides Function GetScales() As Double()
        _scales = New Double(5) {}
        _scales(0) = 0.0352262919
        _scales(1) = -0.0854412739
        _scales(2) = -0.13501102
        _scales(3) = 0.4598775021
        _scales(4) = 0.8068915093
        _scales(5) = 0.332670553
        Return _scales
    End Function

End Class

''' <summary>
''' http://wavelets.pybytes.com/wavelet/db4/
''' </summary>
''' <remarks></remarks>
Public Class Daub04
    Inherits Wavelet

    Private _coeffs As Double()
    Private _scales As Double()

    Private Shared sqrt02 As Double = 1.4142135623730951
    Private Shared sqrt10 As Double = Math.Sqrt(10.0)
    Private Shared constA As Double = Math.Sqrt(5.0 + 2.0 * sqrt10)

    Protected Overrides Function GetCoefficients() As Double()
        _coeffs = New Double(7) {}
        _coeffs(0) = -0.2303778133
        _coeffs(1) = 0.7148465706
        _coeffs(2) = -0.6308807679
        _coeffs(3) = -0.0279837694
        _coeffs(4) = 0.1870348117
        _coeffs(5) = 0.0308413818
        _coeffs(6) = -0.0328830117
        _coeffs(7) = -0.0105974018
        Return _coeffs
    End Function

    Protected Overrides Function GetScales() As Double()
        _scales = New Double(7) {}
        _scales(0) = -0.0105974018
        _scales(1) = 0.0328830117
        _scales(2) = 0.0308413818
        _scales(3) = -0.1870348117
        _scales(4) = -0.0279837694
        _scales(5) = 0.6308807679
        _scales(6) = 0.7148465706
        _scales(7) = 0.2303778133
        Return _scales
    End Function

End Class

''' <summary>
''' http://wavelets.pybytes.com/wavelet/db8/
''' </summary>
''' <remarks></remarks>
Public Class Daub08
    Inherits Wavelet

    Private _coeffs As Double()
    Private _scales As Double()

    Protected Overrides Function GetCoefficients() As Double()
        _coeffs = New Double(15) {}
        _coeffs(0) = -0.0544158422
        _coeffs(1) = 0.3128715909
        _coeffs(2) = -0.6756307363
        _coeffs(3) = 0.5853546837
        _coeffs(4) = 0.0158291053
        _coeffs(5) = -0.284015543
        _coeffs(6) = -0.0004724846
        _coeffs(7) = 0.1287474266
        _coeffs(8) = 0.017369301
        _coeffs(9) = -0.0440882539
        _coeffs(10) = -0.0139810279
        _coeffs(11) = 0.008746094
        _coeffs(12) = 0.004870353
        _coeffs(13) = -0.0003917404
        _coeffs(14) = -0.0006754494
        _coeffs(15) = -0.000117476
        Return _coeffs
    End Function

    Protected Overrides Function GetScales() As Double()
        _scales = New Double(15) {}
        _scales(0) = -0.0001174768
        _scales(1) = 0.0006754494
        _scales(2) = -0.0003917404
        _scales(3) = -0.004870353
        _scales(4) = 0.008746094
        _scales(5) = 0.0139810279
        _scales(6) = -0.0440882539
        _scales(7) = -0.017369301
        _scales(8) = 0.1287474266
        _scales(9) = 0.0004724846
        _scales(10) = -0.284015543
        _scales(11) = -0.0158291053
        _scales(12) = 0.5853546837
        _scales(13) = 0.6756307363
        _scales(14) = 0.3128715909
        _scales(15) = 0.0544158422
        Return _scales
    End Function

End Class

''' <summary>
''' http://wavelets.pybytes.com/wavelet/db16/
''' </summary>
''' <remarks></remarks>
Public Class Daub16
    Inherits Wavelet

    Private _coeffs As Double()
    Private _scales As Double()

    Protected Overrides Function GetCoefficients() As Double()
        _coeffs = New Double(31) {}
        _coeffs(0) = -0.0031892209
        _coeffs(1) = 0.0349077143
        _coeffs(2) = -0.1650642835
        _coeffs(3) = 0.4303127228
        _coeffs(4) = -0.6373563321
        _coeffs(5) = 0.4402902569
        _coeffs(6) = 0.0897510894
        _coeffs(7) = -0.3270633105
        _coeffs(8) = 0.0279182081
        _coeffs(9) = 0.2111906939
        _coeffs(10) = -0.0273402638
        _coeffs(11) = -0.1323883056
        _coeffs(12) = 0.0062397228
        _coeffs(13) = 0.075924236
        _coeffs(14) = 0.0075889744
        _coeffs(15) = -0.0368883977
        _coeffs(16) = -0.0102976596
        _coeffs(17) = 0.0139937689
        _coeffs(18) = 0.0069900146
        _coeffs(19) = -0.0036442796
        _coeffs(20) = -0.0031280234
        _coeffs(21) = 0.000407897
        _coeffs(22) = 0.0009410217
        _coeffs(23) = 0.0001142415
        _coeffs(24) = -0.0001747872
        _coeffs(25) = -0.000061036
        _coeffs(26) = 0.0000139457
        _coeffs(27) = 0.0000113366
        _coeffs(28) = 0.0000010436
        _coeffs(29) = -0.0
        _coeffs(30) = -0.0
        _coeffs(31) = -0.0
        Return _coeffs
    End Function

    Protected Overrides Function GetScales() As Double()
        _scales = New Double(31) {}
        _scales(0) = -0.0
        _scales(1) = 0.0
        _scales(2) = -0.0
        _scales(3) = -0.0000010436
        _scales(4) = 0.0000113366
        _scales(5) = -0.0000139457
        _scales(6) = -0.000061036
        _scales(7) = 0.0001747872
        _scales(8) = 0.0001142415
        _scales(9) = -0.0009410217
        _scales(10) = 0.000407897
        _scales(11) = 0.0031280234
        _scales(12) = -0.0036442796
        _scales(13) = -0.0069900146
        _scales(14) = 0.0139937689
        _scales(15) = 0.0102976596
        _scales(16) = -0.0368883977
        _scales(17) = -0.0075889744
        _scales(18) = 0.075924236
        _scales(19) = -0.0062397228
        _scales(20) = -0.1323883056
        _scales(21) = 0.0273402638
        _scales(22) = 0.2111906939
        _scales(23) = -0.0279182081
        _scales(24) = -0.3270633105
        _scales(25) = -0.0897510894
        _scales(26) = 0.4402902569
        _scales(27) = 0.6373563321
        _scales(28) = 0.4303127228
        _scales(29) = 0.1650642835
        _scales(30) = 0.0349077143
        _scales(31) = 0.0031892209
        Return _scales
    End Function

End Class

Public Class Lege02
    Inherits Wavelet

    Private _coeffs As Double()
    Private _scales As Double()

    Protected Overrides Function GetCoefficients() As Double()
        Return _coeffs
    End Function

    Protected Overrides Function GetScales() As Double()
        _coeffs = New Double(1) {}
        _coeffs(0) = -1.0 / 1.4142135623730951 ' w0 - normed by sqrt( 2 )
        _coeffs(1) = 1.0 / 1.4142135623730951 ' w1 - normed by sqrt( 2 )

        _scales = New Double(1) {}
        _scales(0) = -_coeffs(1) ' -w1 -> -1. / sqrt(2.)
        _scales(1) = _coeffs(0) ' w0 -> -1. / sqrt(2.)
        _scales(1) = 1.0 / 1.4142135623730951
        Return _scales
    End Function

End Class

Public Class Lege04
    Inherits Wavelet

    Private _coeffs As Double()
    Private _scales As Double()

    Protected Overrides Function GetCoefficients() As Double()
        Return _coeffs
    End Function

    Protected Overrides Function GetScales() As Double()
        _scales = New Double(3) {}
        _scales(0) = (-5.0 / 8.0) / 1.4142135623730951
        _scales(1) = (-3.0 / 8.0) / 1.4142135623730951
        _scales(2) = (-3.0 / 8.0) / 1.4142135623730951
        _scales(3) = (-5.0 / 8.0) / 1.4142135623730951

        _coeffs = New Double(3) {}
        _coeffs(0) = _scales(3) ' h3
        _coeffs(1) = -_scales(2) ' -h2
        _coeffs(2) = _scales(1) ' h1
        _coeffs(3) = -_scales(0) ' -h0
        Return _scales
    End Function

End Class

Public Class Lege06
    Inherits Wavelet

    Private _coeffs As Double()
    Private _scales As Double()

    Protected Overrides Function GetCoefficients() As Double()
        Return _coeffs
    End Function

    Protected Overrides Function GetScales() As Double()
        _scales = New Double(5) {}
        _scales(0) = -63.0 / 128.0 / 1.4142135623730951 ' h0
        _scales(1) = -35.0 / 128.0 / 1.4142135623730951 ' h1
        _scales(2) = -30.0 / 128.0 / 1.4142135623730951 ' h2
        _scales(3) = -30.0 / 128.0 / 1.4142135623730951 ' h3
        _scales(4) = -35.0 / 128.0 / 1.4142135623730951 ' h4
        _scales(5) = -63.0 / 128.0 / 1.4142135623730951 ' h5

        _coeffs = New Double(5) {}
        _coeffs(0) = _scales(5) ' h5
        _coeffs(1) = -_scales(4) ' -h4
        _coeffs(2) = _scales(3) ' h3
        _coeffs(3) = -_scales(2) ' -h2
        _coeffs(4) = _scales(1) ' h1
        _coeffs(5) = -_scales(0) ' -h0
        Return _scales
    End Function

End Class

''' <summary>
''' http://wavelets.pybytes.com/wavelet/coif2/
''' </summary>
''' <remarks></remarks>
Public Class Coif02
    Inherits Wavelet

    Private _coeffs As Double()
    Private _scales As Double()

    Protected Overrides Function GetCoefficients() As Double()
        _coeffs = New Double(11) {}
        _coeffs(0) = -0.0163873365
        _coeffs(1) = -0.0414649368
        _coeffs(2) = 0.0673725547
        _coeffs(3) = 0.3861100668
        _coeffs(4) = -0.8127236354
        _coeffs(5) = 0.4170051844
        _coeffs(6) = 0.0764885991
        _coeffs(7) = -0.0594344186
        _coeffs(8) = -0.0236801719
        _coeffs(9) = 0.0056114348
        _coeffs(10) = 0.0018232089
        _coeffs(11) = -0.0007205494
        Return _coeffs
    End Function

    Protected Overrides Function GetScales() As Double()
        _scales = New Double(11) {}
        _scales(0) = -0.0007205494
        _scales(1) = -0.0018232089
        _scales(2) = 0.0056114348
        _scales(3) = 0.0236801719
        _scales(4) = -0.0594344186
        _scales(5) = -0.0764885991
        _scales(6) = 0.4170051844
        _scales(7) = 0.8127236354
        _scales(8) = 0.3861100668
        _scales(9) = -0.0673725547
        _scales(10) = -0.0414649368
        _scales(11) = 0.0163873365
        Return _scales
    End Function

End Class

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class DMeyer
    Inherits Wavelet

    Private _coeffs As Double()
    Private _scales As Double()

    Protected Overrides Function GetCoefficients() As Double()
        _coeffs = New Double(61) {}
        _coeffs(0) = 0
        _coeffs(1) = 8.5E-90
        _coeffs(2) = 1.11E-80
        _coeffs(3) = -1.08E-80
        _coeffs(4) = -6.07E-80
        _coeffs(5) = -1.087E-70
        _coeffs(6) = -8.2E-80
        _coeffs(7) = 1.178E-70
        _coeffs(8) = 5.506E-70
        _coeffs(9) = 0.0000011308
        _coeffs(10) = 0.0000014895
        _coeffs(11) = 7.368E-70
        _coeffs(12) = -0.0000032054
        _coeffs(13) = -0.0000163127
        _coeffs(14) = -0.0000655431
        _coeffs(15) = -0.0006011502
        _coeffs(16) = 0.0027046721
        _coeffs(17) = 0.0022025341
        _coeffs(18) = -0.0060458141
        _coeffs(19) = -0.0063877183
        _coeffs(20) = 0.0110614964
        _coeffs(21) = 0.0152700151
        _coeffs(22) = -0.0174234341
        _coeffs(23) = -0.032130794
        _coeffs(24) = 0.0243487459
        _coeffs(25) = 0.0637390243
        _coeffs(26) = -0.030655092
        _coeffs(27) = -0.1328452004
        _coeffs(28) = 0.0350875557
        _coeffs(29) = 0.4445930028
        _coeffs(30) = -0.7445855923
        _coeffs(31) = 0.4445930028
        _coeffs(32) = 0.0350875557
        _coeffs(33) = -0.1328452004
        _coeffs(34) = -0.030655092
        _coeffs(35) = 0.0637390243
        _coeffs(36) = 0.0243487459
        _coeffs(37) = -0.032130794
        _coeffs(38) = -0.0174234341
        _coeffs(39) = 0.0152700151
        _coeffs(40) = 0.0110614964
        _coeffs(41) = -0.0063877183
        _coeffs(42) = -0.0060458141
        _coeffs(43) = 0.0022025341
        _coeffs(44) = 0.0027046721
        _coeffs(45) = -0.0006011502
        _coeffs(46) = -0.0000655431
        _coeffs(47) = -0.0000163127
        _coeffs(48) = -0.0000032054
        _coeffs(49) = 7.368E-70
        _coeffs(50) = 0.0000014895
        _coeffs(51) = 0.0000011308
        _coeffs(52) = 5.506E-70
        _coeffs(53) = 1.178E-70
        _coeffs(54) = -8.2E-80
        _coeffs(55) = -1.087E-70
        _coeffs(56) = -6.07E-80
        _coeffs(57) = -1.08E-80
        _coeffs(58) = 1.11E-80
        _coeffs(59) = 8.5E-90
        _coeffs(60) = 0.0
        _coeffs(61) = 0
        Return _coeffs
    End Function

    Protected Overrides Function GetScales() As Double()
        _scales = New Double(61) {}
        _scales(0) = 0
        _scales(1) = 0.0 ' 0E-10.0000000000
        _scales(2) = 8.5E-90 ' 8.5E-9000000
        _scales(3) = -1.11E-80
        _scales(4) = -1.08E-80
        _scales(5) = 6.07E-80
        _scales(6) = -1.087E-70
        _scales(7) = 8.2E-90
        _scales(8) = 1.178E-70
        _scales(9) = -5.506E-70
        _scales(10) = 0.0000011308
        _scales(11) = -0.0000014895
        _scales(12) = 7.368E-70
        _scales(13) = 0.0000032054
        _scales(14) = -0.0000163127
        _scales(15) = 0.0000655431
        _scales(16) = -0.0006011502
        _scales(17) = -0.0027046721
        _scales(18) = 0.0022025341
        _scales(19) = 0.0060458141
        _scales(20) = -0.0063877183
        _scales(21) = -0.0110614964
        _scales(22) = 0.0152700151
        _scales(23) = 0.0174234341
        _scales(24) = -0.032130794
        _scales(25) = -0.0243487459
        _scales(26) = 0.0637390243
        _scales(27) = 0.030655092
        _scales(28) = -0.1328452004
        _scales(29) = -0.0350875557
        _scales(30) = 0.4445930028
        _scales(31) = 0.7445855923
        _scales(32) = 0.4445930028
        _scales(33) = -0.0350875557
        _scales(34) = -0.1328452004
        _scales(35) = 0.030655092
        _scales(36) = 0.0637390243
        _scales(37) = -0.0243487459
        _scales(38) = -0.032130794
        _scales(39) = 0.0174234341
        _scales(40) = 0.0152700151
        _scales(41) = -0.0110614964
        _scales(42) = -0.0063877183
        _scales(43) = 0.0060458141
        _scales(44) = 0.0022025341
        _scales(45) = -0.0027046721
        _scales(46) = -0.0006011502
        _scales(47) = 0.0000655431
        _scales(48) = -0.0000163127
        _scales(49) = 0.0000032054
        _scales(50) = 7.368E-70
        _scales(51) = -0.0000014895
        _scales(52) = 0.0000011308
        _scales(53) = -5.506E-70
        _scales(54) = 1.178E-70
        _scales(55) = 8.2E-90
        _scales(56) = -1.087E-70
        _scales(57) = 6.07E-80
        _scales(58) = -1.08E-80
        _scales(59) = -1.11E-80
        _scales(60) = 8.5E-90
        _scales(61) = 0
        Return _scales
    End Function

End Class