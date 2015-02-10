Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports System.Drawing.Imaging
Imports System.Drawing
Imports System.Linq

Imports System.Runtime.InteropServices.Marshal

Public Class Imaging

    Private Sub New()
    End Sub

    Public Shared Function CreateContourData(rows As Integer, cols As Integer) As Double()()
        Dim x As Double
        Dim y As Double
        Dim data As Double()() = New Double(rows)() {}
        For i As Integer = 0 To rows - 1
            data(i) = New Double(cols) {}
            For j As Integer = 0 To cols - 1
                x = (i - rows / 2.0) / rows
                y = (j - cols / 2.0) / cols
                data(i)(j) = Math.Exp(-x * x - y * y)
            Next
        Next
        Return data
    End Function

    Public Shared Sub DrawBoundingBox(img As Image, topLeft As System.Drawing.Point, pen As Pen, width As Integer, height As Integer)

        topLeft.X -= 1
        topLeft.Y -= 1

        Dim gfx As Graphics = Graphics.FromImage(img)
        gfx.DrawRectangle(pen, New Rectangle(topLeft, New Size(width, height)))

    End Sub

    Public Shared Function GetLogSpectrogramImage(ByVal seriesList As List(Of TimeSeries), pallet As IColorPallet, palletDbThresh As Integer, showCentroids As Boolean) As Bitmap

        Dim Buffer As New List(Of Double())
        Dim Centroids As New List(Of Integer)

        Dim Height As Integer = seriesList(0).Frames(0).Length

        For i As Integer = 0 To seriesList.Count - 1
            Dim bestCentroid As Double = 0
            For f As Integer = 0 To seriesList(i).Frames.Length - 1

                If showCentroids Then
                    Centroids.Add(CInt(seriesList(i).CentroidIndices(f)))
                End If

                Buffer.Add(seriesList(i).Frames(f))
                If seriesList(i).Frames(f).Length < Height Then Height = seriesList(i).Frames(f).Length ' Adjust the height if frame size has changed
            Next
            'If seriesList.Count > 1 AndAlso i <> seriesList.Count - 1 Then
            '    Buffer.Add(New Double(Buffer(0).Length - 1) {}) ' This'll give a frame seperator column.
            '    'Centroids.Add(0)
            'End If
        Next

        Dim Bmp As New Bitmap(Buffer.Count, Buffer(0).Length, PixelFormat.Format32bppArgb)
        Dim palletLen As Integer = pallet.Colors.Length

        For row As Integer = 0 To (Buffer.Count - 1)
            For col As Integer = 0 To Height - 1 ' band

                Dim LevelIndB As Double
                Dim PixelColor As Double
                Dim Sample As Double = Buffer(row)(col)

                If Sample = 0 OrElse (showCentroids AndAlso Centroids(row) = col) Then
                    PixelColor = 0
                Else

                    'LevelIndB = 20 * Math.Log(Sample) / 2.30258509299405 ' Log10
                    LevelIndB = 20 * Math.Log10(Math.Abs(Sample)) '/ 2.30258509299405 ' Log10

                    If LevelIndB < -palletDbThresh Then
                        PixelColor = 0
                    ElseIf LevelIndB > palletDbThresh Then
                        PixelColor = palletLen - 1 '_LevelPaletteRange - 1
                    Else
                        ' = _LevelPaletteRange * (LevelIndB + RangedB) / RangedB
                        'PixelColor = _LevelPaletteRange * (LevelIndB + -100) / -100
                        PixelColor = Math.Min(palletLen - 1, palletLen * (LevelIndB + palletDbThresh) / palletDbThresh)
                        'PixelColor = (col + (Sample - min) * (row - col) / (max - min))
                    End If
                End If

                If Double.IsInfinity(PixelColor) OrElse Double.IsNaN(PixelColor) Then PixelColor = 0
                Bmp.SetPixel(row, col, pallet.Colors(CInt(Int(PixelColor))))

            Next
        Next

        Bmp.RotateFlip(RotateFlipType.RotateNoneFlipY)

        Return Bmp

    End Function

    Public Shared Function GetFastLogSpectrogramImage(ByVal seriesList As List(Of TimeSeries), pallet As IColorPallet, palletDbThresh As Integer, showCentroids As Boolean) As Bitmap

        Dim Buffer As New List(Of Double()) ' Change to Double()() + Use Array.Copy below...
        Dim Centroids As New List(Of Integer)

        Dim PalletLen As Integer = pallet.Colors.Length
        Dim Height As Integer = seriesList(0).Frames(0).Length

        For i As Integer = 0 To seriesList.Count - 1
            For j As Integer = 0 To seriesList(i).Frames.Length - 1
                If showCentroids Then
                    Centroids.Add(CInt(seriesList(i).CentroidIndices(j)))
                End If
                Buffer.Add(seriesList(i).Frames(j))
                If seriesList(i).Frames(j).Length < Height Then Height = seriesList(i).Frames(j).Length ' Adjust the height if frame size has changed
            Next
        Next

        Dim Width As Integer = Buffer.Count
        Dim Bmp As New Bitmap(Width, Height)

        ' lock image
        Dim data As BitmapData = Bmp.LockBits(New Rectangle(0, 0, Width, Height), ImageLockMode.[ReadOnly], Bmp.PixelFormat)
        Dim offset As Integer = data.Stride - Width * 4
        Dim ptr As IntPtr = data.Scan0
        Dim bytes As Integer = data.Stride * Height
        Dim rgbValues(bytes - 1) As Byte

        For x As Integer = Height - 1 To 0 Step -1
            For y As Integer = 0 To Width - 1 ' band
                Dim Sample As Double = Buffer(y)(x)

                ' ----------------------------------------------------------------------
                Dim LevelIndB As Double
                Dim ColorIndex As Double = -1
                Dim c As Color = Nothing

                If Sample = 0 Then
                    ColorIndex = 0
                ElseIf showCentroids AndAlso Centroids(y) = x Then
                    c = Color.OrangeRed
                Else

                    LevelIndB = 20 * Math.Log10(Math.Abs(Sample))

                    If LevelIndB < -palletDbThresh Then
                        ColorIndex = 0
                    ElseIf LevelIndB > palletDbThresh Then
                        ColorIndex = PalletLen - 1
                    Else
                        ColorIndex = Math.Min(PalletLen - 1, PalletLen * (LevelIndB + palletDbThresh) / palletDbThresh)
                    End If
                    If Double.IsInfinity(ColorIndex) OrElse Double.IsNaN(ColorIndex) Then ColorIndex = 0
                End If

                If ColorIndex >= 0 Then
                    c = pallet.Colors(CInt((ColorIndex)))
                End If

                rgbValues(offset) = c.B
                rgbValues(offset + 1) = c.G
                rgbValues(offset + 2) = c.R
                rgbValues(offset + 3) = c.A
                offset += 4

            Next
        Next

        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes)
        Bmp.UnlockBits(data)

        Return Bmp

    End Function

    Public Shared Sub DrawFrameBufferToPinnedImage(pinnedPixelArray As Byte(), ByVal buffer As TimeSeriesBuffer, pallet As IColorPallet, palletDbThresh As Integer, showCentroids As Boolean)

        Dim seriesList As List(Of TimeSeries) = buffer.ToList
        Dim ConcatBuffer As New List(Of Double()) ' Change to Double()() + Use Array.Copy below...
        Dim Centroids As New List(Of Integer)

        Dim PalletLen As Integer = pallet.Colors.Length
        Dim Height As Integer = seriesList(0).Frames(0).Length

        For i As Integer = 0 To seriesList.Count - 1
            For j As Integer = 0 To seriesList(i).Frames.Length - 1
                If showCentroids Then
                    Centroids.Add(CInt(seriesList(i).CentroidIndices(j)))
                End If
                ConcatBuffer.Add(seriesList(i).Frames(j))
                If seriesList(i).Frames(j).Length < Height Then Height = seriesList(i).Frames(j).Length ' Adjust the height if frame size has changed
            Next
        Next

        Dim Width As Integer = ConcatBuffer.Count
        Dim offset As Integer = 0

        For x As Integer = Height - 1 To 0 Step -1
            For y As Integer = 0 To Width - 1 ' band

                Dim Sample As Double = ConcatBuffer(y)(x)
                Dim LevelIndB As Double
                Dim ColorIndex As Double = -1
                Dim c As Color = Nothing

                If Sample = 0 Then
                    ColorIndex = 0
                ElseIf showCentroids AndAlso Centroids(y) = x Then
                    c = Color.Red
                Else

                    LevelIndB = (20 * Math.Log10(Math.Abs(Sample))) ' / PalletLen) '/ PalletLen
                    'LevelIndB = Math.Abs(Sample) * 22050 / 1000

                    If LevelIndB < -palletDbThresh Then
                        ColorIndex = 0
                    ElseIf LevelIndB > palletDbThresh Then
                        ColorIndex = PalletLen - 1
                    Else
                        ColorIndex = Math.Min(PalletLen - 1, PalletLen * (LevelIndB + palletDbThresh) / palletDbThresh)
                    End If
                    If Double.IsInfinity(ColorIndex) OrElse Double.IsNaN(ColorIndex) Then ColorIndex = 0
                End If

                If ColorIndex >= 0 Then
                    c = pallet.Colors(CInt(ColorIndex))
                End If

                pinnedPixelArray(offset) = c.B
                pinnedPixelArray(offset + 1) = c.G
                pinnedPixelArray(offset + 2) = c.R
                pinnedPixelArray(offset + 3) = c.A
                offset += 4

            Next
        Next

    End Sub

    Public Shared Sub DrawFrameBufferToPinnedImage(pixels As Byte(), ByVal seriesList As List(Of TimeSeries), pallet As IColorPallet, palletDbThresh As Integer, showCentroids As Boolean)

        Dim Buffer As New List(Of Double()) ' Change to Double()() + Use Array.Copy below...
        Dim Centroids As New List(Of Integer)

        Dim PalletLen As Integer = pallet.Colors.Length
        Dim Height As Integer = seriesList(0).Frames(0).Length

        For i As Integer = 0 To seriesList.Count - 1
            For j As Integer = 0 To seriesList(i).Frames.Length - 1
                If showCentroids Then
                    Centroids.Add(CInt(seriesList(i).CentroidIndices(j)))
                End If
                Buffer.Add(seriesList(i).Frames(j).Reverse.ToArray)
                If seriesList(i).Frames(j).Length < Height Then Height = seriesList(i).Frames(j).Length ' Adjust the height if frame size has changed
            Next
        Next

        Dim Width As Integer = Buffer.Count
        Dim offset As Integer = 0

        For x As Integer = 0 To Height - 1
            For y As Integer = 0 To Width - 1 ' band
                Dim Sample As Double = Buffer(y)(x)

                ' ----------------------------------------------------------------------
                Dim LevelIndB As Double
                Dim PixelColor As Double = -1
                Dim c As Color = Nothing

                If Sample = 0 Then
                    PixelColor = 0
                ElseIf showCentroids AndAlso Centroids(y) = x Then
                    c = Color.Red
                Else

                    LevelIndB = 20 * Math.Log10(Math.Abs(Sample))

                    If LevelIndB < -palletDbThresh Then
                        PixelColor = 0
                    ElseIf LevelIndB > palletDbThresh Then
                        PixelColor = PalletLen - 1
                    Else
                        PixelColor = Math.Min(PalletLen - 1, PalletLen * (LevelIndB + palletDbThresh) / palletDbThresh)
                    End If
                    If Double.IsInfinity(PixelColor) OrElse Double.IsNaN(PixelColor) Then PixelColor = 0
                End If

                If PixelColor >= 0 Then
                    c = pallet.Colors(CInt(PixelColor))
                End If

                pixels(offset) = c.B
                pixels(offset + 1) = c.G
                pixels(offset + 2) = c.R
                pixels(offset + 3) = c.A
                offset += 4

            Next
        Next

    End Sub

    Public Shared Function GetLogSpectrogramImage(ByVal timeSeries As TimeSeries, pallet As IColorPallet, palletDbThresh As Integer, showCentroids As Boolean) As Bitmap

        Dim spectrum As Double()() = timeSeries.Frames
        Dim centroids As Integer() = timeSeries.CentroidIndices

        Dim Bmp As New Bitmap(spectrum.Length, spectrum(0).Length)

        For row As Integer = 0 To spectrum.Length - 1
            For col As Integer = 0 To (spectrum(0).Length - 1)

                Dim LevelIndB As Double
                Dim PixelColor As Double
                Dim Sample As Double = spectrum(row)(col)

                If Sample = 0 OrElse (showCentroids AndAlso centroids(row) = col) Then
                    PixelColor = 0
                Else
                    'LevelIndB = 20 * Math.Log(Sample) / 2.30258509299405
                    LevelIndB = 20 * Math.Log10(Math.Abs(Sample))
                    If Double.IsNaN(LevelIndB) OrElse LevelIndB < -palletDbThresh Then
                        PixelColor = 0
                    ElseIf LevelIndB > palletDbThresh Then
                        PixelColor = pallet.Colors.Length - 1
                    Else
                        PixelColor = Math.Min(pallet.Colors.Length - 1, pallet.Colors.Length * (LevelIndB + palletDbThresh) / palletDbThresh)
                    End If
                End If
                Bmp.SetPixel(row, col, pallet.Colors(CInt(Int(PixelColor))))
            Next
        Next
        Bmp.RotateFlip(RotateFlipType.RotateNoneFlipY)
        Return Bmp
    End Function

    Public Shared Sub DrawFastLogSpectrogramImage(pb As PictureBox, ByVal seriesList As List(Of TimeSeries), pallet As IColorPallet, palletDbThresh As Integer, showCentroids As Boolean)

        Dim Buffer As Double()() = New Double(seriesList.Count - 1)() {} ' New List(Of Double()) ' Change to Double()() + Use Array.Copy below...
        Dim Centroids As New List(Of Integer)

        Dim PalletLen As Integer = pallet.Colors.Length

        Dim Width As Integer = pb.Width 'Buffer.Count
        Dim Height As Integer = pb.Height 'seriesList(0).Frames(0).Length

        For i As Integer = 0 To seriesList.Count - 1
            For f As Integer = 0 To seriesList(i).Frames.Length - 1
                If showCentroids Then
                    Centroids.Add(CInt(seriesList(i).CentroidIndices(f)))
                End If
                'Buffer.Add(seriesList(i).Frames(f))
                Buffer(i) = seriesList(i).Frames(f)
                If seriesList(i).Frames(f).Length < Height Then Height = seriesList(i).Frames(f).Length ' Adjust the height if frame size has changed
            Next
        Next

        Dim Bmp As New Bitmap(Width, Height)
        'Dim Gfx As Graphics = Graphics.FromImage(Bmp)

        ' lock image
        Dim data As BitmapData = Bmp.LockBits(New Rectangle(0, 0, Width, Height), ImageLockMode.[ReadOnly], Bmp.PixelFormat)
        'Dim offset As Integer = data.Stride - Width * 4
        Dim offset As Integer = Width * 4 - 1
        Dim ptr As IntPtr = data.Scan0

        Dim bytes As Integer = data.Stride '* Height

        Dim rgbValues(bytes - 1) As Byte

        For x As Integer = Buffer.Length - 1 To 0 Step -1
            For y As Integer = 0 To Height - 1

                Dim x1 As Integer = x '(Buffer.Length \ Width) * x
                Dim y1 As Integer = Buffer(0).Length - y - 1

                Dim Sample As Double = Buffer(x1)(y1)

                ' ----------------------------------------------------------------------
                Dim LevelIndB As Double
                Dim PixelColor As Double

                If Sample = 0 OrElse (showCentroids AndAlso Centroids(y) = x) Then
                    PixelColor = 0
                Else

                    LevelIndB = 20 * Math.Log10(Math.Abs(Sample))

                    If LevelIndB < -palletDbThresh Then
                        PixelColor = 0
                    ElseIf LevelIndB > palletDbThresh Then
                        PixelColor = PalletLen - 1
                    Else
                        PixelColor = Math.Min(PalletLen - 1, PalletLen * (LevelIndB + palletDbThresh) / palletDbThresh)
                    End If
                End If

                If Double.IsInfinity(PixelColor) OrElse Double.IsNaN(PixelColor) Then PixelColor = 0

                ' ----------------------------------------------------------------------

                Dim c As Color = pallet.Colors(CInt(Int(PixelColor)))

                rgbValues(offset) = c.B
                rgbValues(offset - 1) = c.G
                rgbValues(offset - 2) = c.R
                rgbValues(offset - 3) = c.A
                offset -= 4

            Next

            Dim pointer As New IntPtr(data.Scan0.ToInt32() + (bytes * x))
            'System.Runtime.InteropServices.Marshal.Copy(pointer, rgbValues, data.Stride * (Bmp.Height - x - 1), data.Stride)
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, bytes * (Bmp.Height - x - 1), pointer, bytes)

        Next

        'System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes)
        Bmp.UnlockBits(data)

    End Sub

    Public Shared Function GetFingerprintImage(data As Integer(), width As Integer, height As Integer) As Image
        Dim image As New Bitmap(width, height, PixelFormat.Format16bppRgb565)
        For i As Integer = 0 To width - 1
            For j As Integer = 0 To height - 2
                'Dim c As Integer = If(data(height * i + 2 * j) OrElse data(height * i + 2 * j + 1), 255, 0)
                'image.SetPixel(i, j, Color.FromArgb(c, c, c))

                Dim pColor As Color = Color.Black
                If CBool(data(height * i + 2 * j)) Then
                    pColor = Color.Blue
                ElseIf CBool(data(height * i + 2 * j + 1)) Then
                    pColor = Color.Red
                End If

                image.SetPixel(i, j, pColor)
            Next
        Next
        Return image
    End Function

    Public Shared Function GetZeroCrossingImage(crossings As Boolean(), width As Integer, height As Integer) As Bitmap
        Dim image As New Bitmap(width, height, PixelFormat.Format16bppRgb565)
        For i As Integer = 0 To width - 1
            For j As Integer = 0 To height - 1

                Dim pColor As Color = Color.Black
                If crossings(height * i + 1 * j) Then
                    pColor = Color.Red
                End If

                image.SetPixel(i, j, pColor)
            Next
        Next
        Return image
    End Function

    Public Shared Function GetFingerprintsImage(fingerprints As List(Of Integer()), width As Integer, height As Integer) As Bitmap

        Const spaceBetweenImages As Integer = 1

        Dim fingersCount As Integer = fingerprints.Count
        Dim rowCount As Integer = CInt(Math.Ceiling(CSng(fingersCount) / fingersCount))

        Dim imageWidth As Integer = fingersCount * (width + spaceBetweenImages) + spaceBetweenImages
        Dim imageHeight As Integer = rowCount * (height + spaceBetweenImages) + spaceBetweenImages

        Dim image As New Bitmap(imageWidth, imageHeight, PixelFormat.Format16bppRgb565)

        For i As Integer = 0 To imageWidth - 1
            For j As Integer = 0 To imageHeight - 1
                image.SetPixel(i, j, Color.White)
            Next
        Next

        Dim verticalOffset As Integer = spaceBetweenImages
        Dim horizontalOffset As Integer = spaceBetweenImages
        Dim count As Integer = 0
        For z As Integer = 0 To fingerprints.Count - 1
            Dim finger As Integer() = fingerprints(z)

            For x As Integer = 0 To width - 1
                For y As Integer = 0 To height - 1
                    Dim pColor As Color = Color.Black
                    Dim idx As Integer = ((height * x) + y) * 2
                    If CBool(finger(idx)) Then
                        pColor = Color.LightGreen
                    ElseIf CBool(finger(idx + 1)) Then
                        pColor = Color.Red
                    End If
                    image.SetPixel(x + horizontalOffset, y + verticalOffset, pColor)
                Next
            Next

            count += 1
            If count Mod fingersCount = 0 Then
                verticalOffset += height + spaceBetweenImages
                horizontalOffset = spaceBetweenImages
            Else
                horizontalOffset += width + spaceBetweenImages
            End If
        Next
        Return image
    End Function

    Public Shared Function GetSignalImage(data As Double(), width As Integer, height As Integer) As Bitmap
        Dim img As New Bitmap(width, height)
        Dim gfx As Graphics = Graphics.FromImage(img)

        'Fill Back color
        Using brush As Brush = New SolidBrush(Color.Black)
            gfx.FillRectangle(brush, New Rectangle(0, 0, width, height))
        End Using

        Dim center As Integer = CInt(height / 2)
        'Draw lines
        Using pen As New Pen(Color.MediumSpringGreen, 1)
            'Find delta X, by which the lines will be drawn
            Dim deltaX As Double = CDbl(width) / data.Length
            Dim normalizeFactor As Double = data.Max(Function(a) Math.Abs(a)) / (CDbl(height) / 2)
            Dim i As Integer = 0, n As Integer = data.Length
            Dim X1 As Integer = 0
            Dim Y1 As Integer = 0
            Dim X2 As Integer = 0
            Dim Y2 As Integer = 0
            While i < n - 1
                X1 = CInt(i * deltaX)
                Y1 = CInt(center - (data(i) / normalizeFactor))
                X2 = CInt(i * deltaX) + 1
                Y2 = CInt(center - (data(i + 1) / normalizeFactor)) + 1
                ' gfx.DrawLine(pen, CSng(i * deltaX), center, CSng(i * deltaX), CSng(center - (data(i) / normalizeFactor)))
                gfx.DrawLine(pen, X1, Y1, X2, Y2)
                'img.SetPixel(Math.Min(CInt(i * deltaX), width - 1), Math.Min(CInt(center - (data(i) / normalizeFactor)), height - 1), Color.AliceBlue)
                i += 1
            End While
        End Using

        Using pen As New Pen(Color.Red, 1)
            gfx.DrawLine(pen, 0, center, width, center)
        End Using

        Return img
    End Function

    Public Shared Function GetSignalImage(frames As List(Of TimeSeries), width As Integer, height As Integer) As Bitmap

        Dim img As New Bitmap(width, height)
        Dim gfx As Graphics = Graphics.FromImage(img)

        'Fill Back color
        Using brush As Brush = New SolidBrush(Color.Black)
            gfx.FillRectangle(brush, New Rectangle(0, 0, width, height))
        End Using

        Dim center As Integer = CInt(height / 2)
        'Draw lines
        Using pen As New Pen(Color.MediumSpringGreen, 1)

            For i As Integer = 0 To frames.Count - 1

                Dim frameSamples As Double() = frames(i).Samples

                'Find delta X, by which the lines will be drawn
                Dim deltaX As Double = width / frameSamples.Length / frames.Count
                Dim normalizeFactor As Double = frameSamples.Max(Function(a) Math.Abs(a)) / (height / 2)

                Dim j As Integer = frameSamples.Length * i
                Dim k As Integer = 0
                Dim n As Integer = frameSamples.Length + j

                While j < n
                    If frameSamples(k) <> 0 Then
                        gfx.DrawLine(pen, CInt(j * deltaX), center, CInt(j * deltaX), CInt(center - (frameSamples(k) / normalizeFactor)))
                    End If
                    j += 1
                    k += 1
                End While

                'gfx.DrawLine(Pens.Red, CInt(j * deltaX) - 1, 0, CInt(j * deltaX) - 1, height)

            Next


        End Using

        'gfx.DrawLine(Pens.Red, 0, 0, 0, height)

        Using pen As New Pen(Color.Red, 1)
            gfx.DrawLine(pen, 0, center, width, center)
        End Using

        Return img

    End Function

    Public Shared Sub PlotBestSimilarities(bmp As Image, similarities As List(Of Double), targetSimilarity As Integer)
        PlotBestSimilarities(bmp, similarities, targetSimilarity, Pens.AliceBlue)
    End Sub

    Public Shared Sub PlotBestSimilarities(bmp As Image, similarities As List(Of Double), targetSimilarity As Integer, pen As Pen)

        If similarities Is Nothing OrElse similarities.Count = 0 OrElse Double.IsNaN(similarities(0)) Then Exit Sub

        Dim height As Integer = bmp.Height
        Dim SimMax As Integer = 1
        Dim ProbsGfx As Graphics = Graphics.FromImage(bmp)
        Dim [step] As Integer = CInt(bmp.Width / similarities.Count) '- 1
        Dim lastX As Integer = [step] \ 2
        Dim lastY As Integer = height - ((CInt(If(Double.IsInfinity(similarities(0)), 0, similarities(0)) * bmp.Height)) \ SimMax)

        If similarities(0) >= targetSimilarity / 100 Then
            ProbsGfx.DrawRectangle(Pens.Red, lastX - 1, lastY - 1, 2, 2)
        Else
            ProbsGfx.DrawRectangle(pen, lastX - 1, lastY - 1, 2, 2)
        End If

        For d As Integer = 1 To similarities.Count - 1
            If Double.IsInfinity(similarities(d)) Then similarities(d) = 0

            Dim x As Integer = lastX + [step] '+ 1
            Dim y As Integer = height - (CInt((similarities(d) * height)) \ SimMax)

            ProbsGfx.DrawLine(pen, lastX, lastY, x, y)

            If similarities(d) >= targetSimilarity / 100 Then
                ProbsGfx.DrawRectangle(Pens.Red, x - 1, y - 1, 2, 2)
            Else
                ProbsGfx.DrawRectangle(pen, x - 1, y - 1, 2, 2)
            End If

            lastY = y
            lastX = x '+ 1

        Next

    End Sub

    Public Shared Sub PlotCentroidPoints(bmp As Bitmap, centroids As List(Of Centroid), centroidPen As Pen, pointPen As Pen)

        Dim width As Integer = bmp.Width
        Dim height As Integer = bmp.Height
        Dim gfx As Graphics = Graphics.FromImage(bmp)

        For i As Integer = 0 To centroids.Count - 1
            Dim cent As Centroid = centroids(i)
            Dim centIdx As Integer = cent.Index
            For j As Integer = 0 To cent.PointCount - 1
                centIdx += cent.GetPoint(j).Index
            Next
            centIdx \= cent.PointCount + 1
            gfx.DrawRectangle(centroidPen, i, centIdx, 1, 1)
            For j As Integer = 0 To cent.PointCount - 1
                Dim point As PointVector = cent.GetPoint(j)
                gfx.DrawRectangle(pointPen, point.Index, cent.Index, 1, 1)
                If cent.PointCount > 1 Then
                    gfx.DrawLine(New Pen(Brushes.Brown, 1), i, centIdx, point.Index, cent.Index)
                End If

                'gfx.DrawRectangle(pointPen, cent.Index, point.Index, 1, 1)
            Next
        Next

    End Sub

    Public Shared Sub PlotQueryResults(bmp As Image, results As List(Of TimeSeriesQueryResult), pen As Pen)

        If results Is Nothing OrElse results.Count = 0 OrElse Double.IsNaN(results(0).Score) Then Exit Sub

        Dim height As Integer = bmp.Height
        Dim SimMax As Integer = 1
        Dim gfx As Graphics = Graphics.FromImage(bmp)

        Dim [step] As Double = CDbl(bmp.Width) / results.Count

        'Dim [step] As Integer = CInt(bmp.Width / results.Count) '- 1
        Dim lastX As Single = CSng([step] / 2)
        Dim lastY As Integer = height - ((CInt(If(Double.IsInfinity(results(0).Score), 0, results(0).Score) * bmp.Height)) \ SimMax)

        gfx.DrawRectangle(pen, lastX - 1, lastY - 1, 2, 2)

        For d As Integer = 1 To results.Count - 1
            Dim val As Double = results(d).Score
            If Double.IsInfinity(val) OrElse Double.IsNaN(val) Then val = 0

            Dim x As Single = CSng(lastX + [step]) '+ 1
            Dim y As Integer = height - (CInt((val * height)) \ SimMax)

            gfx.DrawLine(pen, lastX, lastY, x, y)

            If results(d).IsMatch Then
                gfx.DrawRectangle(Pens.Red, x - 2, y - 2, 4, 4)
            Else
                gfx.DrawRectangle(pen, x - 1, y - 1, 2, 2)
            End If

            lastY = y
            lastX = x '+ 1
            '[step] += 1

        Next

    End Sub

    Public Shared Sub PlotBestSimilarities(pb As PictureBox, results As List(Of TimeSeriesQueryResult), pen As Pen)

        Dim height As Integer = pb.Height
        Dim SimMax As Integer = 1
        Dim ProbsGfx As Graphics = pb.CreateGraphics ' Graphics.FromImage(bmp)
        Dim [step] As Integer = CInt(pb.Width / results.Count) '- 1
        Dim lastX As Integer = [step] \ 2
        Dim lastY As Integer = height - ((CInt(If(Double.IsInfinity(results(0).Score), 0, results(0).Score) * pb.Height)) \ SimMax)

        ProbsGfx.DrawRectangle(pen, lastX - 1, lastY - 1, 2, 2)

        For d As Integer = 1 To results.Count - 1
            Dim val As Double = results(d).Score
            If Double.IsInfinity(val) OrElse Double.IsNaN(val) Then val = 0

            Dim x As Integer = lastX + [step] '+ 1
            Dim y As Integer = height - (CInt((val * height)) \ SimMax)

            ProbsGfx.DrawLine(pen, lastX, lastY, x, y)

            If results(d).IsMatch Then
                ProbsGfx.DrawRectangle(Pens.Red, x - 2, y - 2, 4, 4)
            Else
                ProbsGfx.DrawRectangle(pen, x - 1, y - 1, 2, 2)
            End If

            lastY = y
            lastX = x '+ 1

        Next

    End Sub

    Public Shared Function GetPalletGradientBitmap(pallet As IColorPallet, verticle As Boolean) As Bitmap
        Dim bmp As New Bitmap(pallet.Colors.Length, 10)
        For i As Integer = 0 To pallet.Colors.Length - 1
            For j As Integer = 0 To 9
                bmp.SetPixel(i, j, pallet.Colors(i))
            Next
        Next
        If verticle Then bmp.RotateFlip(RotateFlipType.Rotate90FlipNone)
        Return bmp
    End Function

End Class
