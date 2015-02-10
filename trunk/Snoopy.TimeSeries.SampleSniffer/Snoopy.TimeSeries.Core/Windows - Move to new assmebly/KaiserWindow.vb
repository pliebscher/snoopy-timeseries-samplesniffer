Namespace Windows
    ''' <summary>
    ''' https://ccrma.stanford.edu/courses/422/projects/kbd/
    ''' </summary>
    ''' <remarks></remarks>
    Public Class KaiserWindow
        Inherits Window

        Private _Alpha As Double = 4.0

        Public Overrides Function GetWindow(ByVal N As Integer) As Double()
            Dim w As Double() = New Double(N - 1) {}

            Dim sumvalue As Double = 0.0
            Dim i As Integer

            For i = 0 To N \ 2 - 1
                sumvalue += BesselI0(Math.PI * _Alpha * Math.Sqrt(1.0 - Math.Pow(4.0 * i / N - 1.0, 2)))
                w(i) = sumvalue
            Next

            ' need to add one more value to the nomalization factor at size/2:
            sumvalue += BesselI0(Math.PI * _Alpha * Math.Sqrt(1.0 - Math.Pow(4.0 * (N / 2) / N - 1.0, 2)))

            ' normalize the window and fill in the righthand side of the window:
            For i = 0 To N \ 2 - 1
                w(i) = Math.Sqrt(w(i) / sumvalue)
                w(N - 1 - i) = w(i)
            Next

            Return w
        End Function

        Private Function BesselI0(x As Double) As Double
            Dim denominator As Double
            Dim numerator As Double
            Dim z As Double

            If x = 0.0 Then
                Return 1.0
            Else
                z = x * x
                numerator = (z * (z * (z * (z * (z * (z * (z * (z * (z * (z * (z * (z * (z * (z * 2.10580722890567E-23 + 3.80715242345326E-20) + 4.794402575483E-17) + 0.0000000000000435125971262668) + 0.000000000030093112711296) + 0.0000000160224679395361) + 0.00000654858370096785) + 0.00202591084143397) + 0.463076284721) + 75.4337328948189) + 8307.92541809429) + 571661.130563785) + 21641557.2361227) + 356644482.244025) + 1440482982.27235)

                denominator = (z * (z * (z - 3076.46912682801) + 3476263.32405882) - 1440482982.27235)
            End If

            Return -numerator / denominator
        End Function

        Public Property Alpha As Double
            Get
                Return _Alpha
            End Get
            Set(value As Double)
                If value <> _Alpha Then
                    _Alpha = value

                End If
            End Set
        End Property

    End Class
End Namespace

