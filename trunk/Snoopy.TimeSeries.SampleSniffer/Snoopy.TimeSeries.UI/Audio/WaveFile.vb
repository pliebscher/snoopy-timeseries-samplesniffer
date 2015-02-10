Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO

Public Class WaveFile

    Public Sub New()
    End Sub

    Public Const WAVE_FORMAT_PCM As Integer = 1

    ''' <summary>
    ''' Create a wav file from the given TimeSeries.
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <param name="series"></param>
    ''' <param name="bitsPerSample"></param>
    ''' <param name="channels"></param>
    ''' <remarks></remarks>
    Public Shared Sub Create(fileName As String, series As TimeSeries, bitsPerSample As Integer, channels As Integer)

        Dim wav As Byte() = New Byte(series.Samples.Length * 2 - 1) {}

        For i As Integer = 0 To series.Samples.Length - 1
            Dim shorty As Short = ToShort(series.Samples(i))
            Dim bytes As Byte() = BitConverter.GetBytes(shorty)
            wav(2 * i) = bytes(0)
            wav(2 * i + 1) = bytes(1)
        Next

        WaveFile.Create(fileName, series.SampleRate, 16, 1, wav)
    End Sub

    Private Shared Function ToShort(x As Double) As Short
        x *= 32768.0 - 1
        Return CShort(If(x > Double.MaxValue, Double.MaxValue, If(x < Double.MinValue, Double.MinValue, x)))
    End Function

    ''' <summary>
    ''' WriteNew
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <param name="data"></param>
    Public Shared Sub Create(fileName As String, samplesPerSecond As Integer, bitsPerSample As Integer, channels As Integer, data As [Byte]())
        If System.IO.File.Exists(fileName) Then
            System.IO.File.Delete(fileName)
        End If
        Dim header As WaveFileHeader = CreateNewWaveFileHeader(samplesPerSecond, bitsPerSample, channels, (data.Length), 44 + data.Length)
        WriteHeader(fileName, header)
        WriteData(fileName, header.DATAPos, data)
    End Sub

    ''' <summary>
    ''' AppendData
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <param name="data"></param>
    Public Shared Sub AppendData(fileName As String, data As [Byte]())
        'Header auslesen
        Dim header As WaveFileHeader = ReadHeader(fileName)

        'Wenn Daten vorhanden
        If header.DATASize > 0 Then
            'Daten anfügen
            WriteData(fileName, CInt(header.DATAPos + header.DATASize), data)

            'Header aktualisieren
            header.DATASize += (data.Length)
            header.RiffSize += (data.Length)

            'Header überschreiben
            WriteHeader(fileName, header)
        End If
    End Sub

    ''' <summary>
    ''' Read
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <returns></returns>
    Public Shared Function Read(fileName As String) As WaveFileHeader
        'Header lesen
        Dim header As WaveFileHeader = ReadHeader(fileName)

        'Fertig
        Return header
    End Function

    ''' <summary>
    ''' CreateWaveFileHeader
    ''' </summary>
    ''' <param name="SamplesPerSecond"></param>
    ''' <param name="BitsPerSample"></param>
    ''' <param name="Channels"></param>
    ''' <param name="dataSize"></param>
    ''' <returns></returns>
    Private Shared Function CreateNewWaveFileHeader(SamplesPerSecond As Integer, BitsPerSample As Integer, Channels As Integer, dataSize As Integer, fileSize As Integer) As WaveFileHeader
        'Header erstellen
        Dim Header As New WaveFileHeader()

        'Werte setzen
        ' Array.Copy("RIFF".ToArray(Of Char)(), Header.RIFF, 4)
        Array.Copy("RIFF".ToArray(), Header.RIFF, 4)
        Header.RiffSize = (fileSize - 8)
        Array.Copy("WAVE".ToArray(), Header.RiffFormat, 4)
        Array.Copy("fmt ".ToArray(), Header.FMT, 4)
        Header.FMTSize = 16
        Header.AudioFormat = WAVE_FORMAT_PCM
        Header.Channels = (Channels)
        Header.SamplesPerSecond = (SamplesPerSecond)
        Header.BitsPerSample = (BitsPerSample)
        Header.BlockAlign = ((BitsPerSample * Channels) >> 3)
        Header.BytesPerSecond = (Header.BlockAlign * Header.SamplesPerSecond)
        Array.Copy("data".ToArray(), Header.DATA, 4)
        Header.DATASize = dataSize

        'Fertig
        Return Header
    End Function

    ''' <summary>
    ''' ReadHeader
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <returns></returns>
    Private Shared Function ReadHeader(fileName As String) As WaveFileHeader
        'Ergebnis
        Dim header As New WaveFileHeader()

        'Wenn die Datei existiert
        If File.Exists(fileName) Then
            'Datei öffnen
            Dim fs As New FileStream(fileName, FileMode.Open, FileAccess.Read)
            Dim rd As New System.IO.BinaryReader(fs, Encoding.UTF8)

            'Lesen
            If fs.CanRead Then
                'Chunk 1
                header.RIFF = rd.ReadChars(4)
                header.RiffSize = (rd.ReadInt32())
                header.RiffFormat = rd.ReadChars(4)

                'Chunk 2
                header.FMT = rd.ReadChars(4)
                header.FMTSize = (rd.ReadInt32())
                header.FMTPos = fs.Position
                header.AudioFormat = (rd.ReadInt16())
                header.Channels = (rd.ReadInt16())
                header.SamplesPerSecond = (rd.ReadInt32())
                header.BytesPerSecond = (rd.ReadInt32())
                header.BlockAlign = (rd.ReadInt16())
                header.BitsPerSample = (rd.ReadInt16())

                'Zu Beginn von Chunk3 gehen
                fs.Seek(header.FMTPos + header.FMTSize, SeekOrigin.Begin)

                'Chunk 3
                header.DATA = rd.ReadChars(4)
                header.DATASize = (rd.ReadInt32())
                header.DATAPos = CInt(fs.Position)

                'Wenn nicht DATA
                If New [String](header.DATA).ToUpper() <> "DATA" Then
                    Dim DataChunkSize As UInteger = CUInt(header.DATASize + 8)
                    fs.Seek(DataChunkSize, SeekOrigin.Current)
                    header.DATASize = CInt(fs.Length - header.DATAPos - DataChunkSize)
                End If

                'Payload einlesen
                header.Payload = rd.ReadBytes(CInt(header.DATASize))
            End If

            'Schliessen
            rd.Close()
            fs.Close()
        End If

        'Fertig
        Return header
    End Function

    ''' <summary>
    ''' WriteHeader
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <param name="header"></param>
    ''' <param name="dataSize"></param>
    Private Shared Sub WriteHeader(fileName As String, header As WaveFileHeader)
        'Datei öffnen
        Dim fs As New FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite)
        Dim wr As New System.IO.BinaryWriter(fs, Encoding.UTF8)

        'Chunk 1
        wr.Write(header.RIFF)
        wr.Write(Int32ToBytes(CInt(header.RiffSize)))
        wr.Write(header.RiffFormat)

        'Chunk 2
        wr.Write(header.FMT)
        wr.Write(Int32ToBytes(CInt(header.FMTSize)))
        wr.Write(Int16ToBytes(header.AudioFormat))
        wr.Write(Int16ToBytes(header.Channels))
        wr.Write(Int32ToBytes(CInt(header.SamplesPerSecond)))
        wr.Write(Int32ToBytes(CInt(header.BytesPerSecond)))
        wr.Write(Int16ToBytes(CShort(header.BlockAlign)))
        wr.Write(Int16ToBytes(CShort(header.BitsPerSample)))

        'Chunk 3
        wr.Write(header.DATA)
        wr.Write(Int32ToBytes(CInt(header.DATASize)))

        'Datei schliessen
        wr.Close()
        fs.Close()
    End Sub

    ''' <summary>
    ''' WriteData
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <param name="pos"></param>
    Private Shared Sub WriteData(fileName As String, pos As Integer, data As [Byte]())
        'Datei öffnen
        Dim fs As New FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite)
        Dim wr As New System.IO.BinaryWriter(fs, Encoding.UTF8)

        'An Schreibposition gehen
        wr.Seek(pos, System.IO.SeekOrigin.Begin)
        'Daten schreiben
        wr.Write(data)
        'Fertig
        wr.Close()
        fs.Close()
    End Sub

    '--------------------------------------------------------------------------------------------
    ' BytesToInt32
    '--------------------------------------------------------------------------------------------
    Private Shared Function BytesToInt32(ByRef bytes As [Byte]()) As Integer
        Dim Int32 As Integer = 0
        Int32 = (Int32 << 8) + bytes(3)
        Int32 = (Int32 << 8) + bytes(2)
        Int32 = (Int32 << 8) + bytes(1)
        Int32 = (Int32 << 8) + bytes(0)
        Return Int32
    End Function

    '--------------------------------------------------------------------------------------------
    ' BytesToInt16
    '--------------------------------------------------------------------------------------------
    Private Shared Function BytesToInt16(ByRef bytes As [Byte]()) As Short
        Dim Int16 As Short = 0
        Int16 = CShort((Int16 << 8) + bytes(1))
        Int16 = CShort((Int16 << 8) + bytes(0))
        Return Int16
    End Function

    '--------------------------------------------------------------------------------------------
    ' Int32ToByte
    '--------------------------------------------------------------------------------------------
    Private Shared Function Int32ToBytes(value As Integer) As [Byte]()
        Dim bytes As [Byte]() = New [Byte](3) {}
        bytes(0) = CType(value And &HFF, [Byte])
        bytes(1) = CType(value >> 8 And &HFF, [Byte])
        bytes(2) = CType(value >> 16 And &HFF, [Byte])
        bytes(3) = CType(value >> 24 And &HFF, [Byte])
        Return bytes
    End Function

    '--------------------------------------------------------------------------------------------
    ' Int16ToBytes
    '--------------------------------------------------------------------------------------------
    Private Shared Function Int16ToBytes(value As Integer) As [Byte]()
        Dim bytes As [Byte]() = New [Byte](1) {}
        bytes(0) = CType(value And &HFF, [Byte])
        bytes(1) = CType(value >> 8 And &HFF, [Byte])
        Return bytes
    End Function

End Class

Public Class WaveFileHeader

    Public Sub New()
    End Sub

    Private _Samples As Double() = Nothing

    'Chunk 1
    Public RIFF As [Char]() = New [Char](3) {}
    Public RiffSize As Integer = 8
    Public RiffFormat As [Char]() = New [Char](3) {}

    'Chunk 2
    Public FMT As [Char]() = New [Char](3) {}
    Public FMTSize As Integer = 16
    Public AudioFormat As Short
    Public Channels As Integer
    Public SamplesPerSecond As Integer
    Public BytesPerSecond As Integer
    Public BlockAlign As Integer
    Public BitsPerSample As Integer

    'Chunk 3
    Public DATA As [Char]() = New [Char](3) {}
    Public DATASize As Integer

    'Data
    Public Payload As [Byte]() = New [Byte](-1) {}

    'HeaderLength
    Public DATAPos As Integer = 44
    'Position FormatSize
    Public FMTPos As Long = 20

    ''' <summary>
    '''Duration 
    ''' </summary>
    ''' <param name="header"></param>
    ''' <returns></returns>
    Public ReadOnly Property Duration() As TimeSpan
        Get
            Dim blockAlign As Integer = ((BitsPerSample * Channels) >> 3)
            Dim bytesPerSec As Integer = CInt(blockAlign * SamplesPerSecond)
            Dim value As Double = CDbl(Payload.Length) / CDbl(bytesPerSec)

            'Fertig
            Return New TimeSpan(0, 0, CInt(Math.Truncate(value)))
        End Get
    End Property

    Public ReadOnly Property Samples As Double()
        Get
            If _Samples Is Nothing Then
                _Samples = New Double(Payload.Length \ 2 - 1) {}
                Dim h As Integer = 0
                For i As Integer = 0 To Payload.Length - 2 Step 2
                    _Samples(h) = BitConverter.ToInt16(Payload, i) / 32768.0
                    h += 1
                Next
            End If
            Return _Samples
        End Get
    End Property

End Class
