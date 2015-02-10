Namespace Wavelets

    Public Class WaveletFilter

        Private m_h As Double()
        Private m_g As Double()
        Private m_l As Integer
        Private filterType As WaveletFilterType

        Public Sub New(fType As WaveletFilterType)
            filterType = fType

            '  Determines which wavelet filter, and corresponding scaling filter,  
            '    based on the character string `choice.' 
            ' 
            '    Input: 
            '    choice = character string (e.g., "haar", "d4, "d6", "d8", "la8", "la16") 
            ' 
            '    Output: 
            '    fills in h, g, and L for a wavelet filter structure 
            '

            Dim hMatrix As Double() = Nothing
            Dim gMatrix As Double() = Nothing

            If fType = WaveletFilterType.Haar Then
                L = 2
                hMatrix = hhaar
                gMatrix = ghaar
            ElseIf fType = WaveletFilterType.D4 Then
                L = 4
                hMatrix = hd4
                gMatrix = gd4
            ElseIf fType = WaveletFilterType.D6 Then
                L = 6
                hMatrix = hd6
                gMatrix = gd6
            ElseIf fType = WaveletFilterType.D8 Then
                L = 8
                hMatrix = hd8
                gMatrix = gd8
            ElseIf fType = WaveletFilterType.D12 Then
                L = 12
                hMatrix = hd12
                gMatrix = gd12
            ElseIf fType = WaveletFilterType.LA8 Then
                L = 8
                hMatrix = hla8
                gMatrix = gla8
            ElseIf fType = WaveletFilterType.LA16 Then
                L = 16
                hMatrix = hla16
                gMatrix = gla16
            End If

            If hMatrix Is Nothing OrElse gMatrix Is Nothing Then
                Throw New NullReferenceException("no wavelet filter selected")
            End If

            H = New Double(L - 1) {}
            G = New Double(L - 1) {}
            Array.Copy(hMatrix, H, L)
            Array.Copy(gMatrix, G, L)
        End Sub

        Public Property L() As Integer
            Get
                Return m_l
            End Get
            Set(value As Integer)
                m_l = value
            End Set
        End Property

        Public Property G() As Double()
            Get
                Return m_g
            End Get
            Set(value As Double())
                m_g = value
            End Set
        End Property

        Public Property H() As Double()
            Get
                Return m_h
            End Get
            Set(value As Double())
                m_h = value
            End Set
        End Property

        '  This function converts the basic Haar wavelet filter  
        '    (h0 = -0.7017, h1 = 0.7017) into a filter of length `scale.' 
        '    The filter is re-normalized so that it's sum of squares equals 1. 
        ' 
        '    Input: 
        '    f     = haar wavelet filter structure 
        '    scale = integer 
        ' 
        '    Output: 
        '    fills in h, g, and L for a wavelet filter structure 
        '
        Public Sub convert_haar(f As WaveletFilter, scale As Integer)
            If f.filterType <> WaveletFilterType.Haar Then
                Throw New Exception("must be a haar wavelet structure")
            End If

            Dim sqrt_scale As Double = Math.Sqrt(scale)

            Me.L = CInt(f.L) * scale
            Me.H = New Double(f.L * scale) {}
            Me.G = New Double(f.L * scale) {}

            For l As Integer = 1 To scale
                Me.H(m_l) = f.H(1) / sqrt_scale
                Me.G(m_l) = f.G(1) / sqrt_scale
                Me.H(m_l + scale) = f.H(2) / sqrt_scale
                Me.G(m_l + scale) = f.G(2) / sqrt_scale
            Next
        End Sub

#Region "Wavelet Scaling Coefficients"

        Private Shared ReadOnly hhaar As Double() = {0.707106781186547, -0.707106781186547}
        Private Shared ReadOnly ghaar As Double() = {0.707106781186547, 0.707106781186547}

        Private Shared ReadOnly hd4 As Double() = {-0.12940952255126, -0.224143868042013, 0.836516303737808, -0.482962913144534}

        Private Shared ReadOnly gd4 As Double() = {0.482962913144534, 0.836516303737808, 0.224143868042013, -0.12940952255126}

        Private Shared ReadOnly hd6 As Double() = {0.0352262918857096, 0.0854412738820267, -0.135011020010255, -0.459877502118491, 0.806891509311093, -0.332670552950083}

        Private Shared ReadOnly gd6 As Double() = {0.332670552950083, 0.806891509311093, 0.459877502118491, -0.135011020010255, -0.0854412738820267, 0.0352262918857096}

        Private Shared ReadOnly hd8 As Double() = {-0.0105974017850021, -0.0328830116666778, 0.0308413818353661, 0.187034811717913, -0.0279837694166834, -0.630880767935879, _
            0.714846570548406, -0.230377813307443}

        Private Shared ReadOnly gd8 As Double() = {0.230377813307443, 0.714846570548406, 0.630880767935879, -0.0279837694166834, -0.187034811717913, 0.0308413818353661, _
            0.0328830116666778, -0.0105974017850021}

        Private Shared ReadOnly hd12 As Double() = {-0.0010773010853085, -0.0047772575109455, 0.0005538422011614, 0.0315820393174862, 0.0275228655303053, -0.0975016055873224, _
            -0.129766867567262, 0.22626469396544, 0.315250351709198, -0.751133908021095, 0.494623890398453, -0.111540743350109}

        Private Shared ReadOnly gd12 As Double() = {0.111540743350109, 0.494623890398453, 0.751133908021095, 0.315250351709198, -0.22626469396544, -0.129766867567262, _
            0.0975016055873224, 0.0275228655303053, -0.0315820393174862, 0.0005538422011614, 0.0047772575109455, -0.0010773010853085}

        Private Shared ReadOnly hla8 As Double() = {0.0322231006040782, 0.0126039672622638, -0.0992195435769564, -0.297857795605605, 0.803738751805386, -0.497618667632563, _
            -0.0296355276459604, 0.0757657147893567}

        Private Shared ReadOnly gla8 As Double() = {-0.0757657147893567, -0.0296355276459604, 0.497618667632563, 0.803738751805386, 0.297857795605605, -0.0992195435769564, _
            -0.0126039672622638, 0.0322231006040782}

        Private Shared ReadOnly hla16 As Double() = {0.0018899503329007, 0.0003029205145516, -0.0149522583367926, -0.0038087520140601, 0.0491371796734768, 0.0272190299168137, _
            -0.0519458381078751, -0.364441894835956, 0.777185751699748, -0.481359651259201, -0.0612733590679088, 0.143294238351054, _
            0.0076074873252848, -0.0316950878103452, -0.0005421323316355, 0.0033824159513594}

        Private Shared ReadOnly gla16 As Double() = {-0.0033824159513594, -0.0005421323316355, 0.0316950878103452, 0.0076074873252848, -0.143294238351054, -0.0612733590679088, _
            0.481359651259201, 0.777185751699748, 0.364441894835956, -0.0519458381078751, -0.0272190299168137, 0.0491371796734768, _
            0.0038087520140601, -0.0149522583367926, -0.0003029205145516, 0.0018899503329007}

#End Region

    End Class

    Public Enum WaveletFilterType
        Haar
        D4
        D6
        D8
        D12
        LA8
        LA16
    End Enum

    Public Enum WaveletTransformType
        DWT
        MODWT
    End Enum

    Public Enum WaveletBoundaryCondition
        Period
        Reflect
    End Enum

    Public Enum WaveletOperation
        Decomposition
        MultiResolutionAnalysis
    End Enum

End Namespace
