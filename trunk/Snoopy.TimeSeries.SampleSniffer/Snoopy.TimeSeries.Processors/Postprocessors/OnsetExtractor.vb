Imports System.ComponentModel
''' <summary>
''' ??
''' </summary>
''' <remarks></remarks>
Public Class OnsetExtractor
    Inherits TimeSeriesPostProcessor

    Private _Bands As Integer = 32
    Private _DeadTime As Integer = 128
    Private _OverFact As Double = 1.1
    Private _Alpha As Double = 0.98

    Private _PreEmpLen As Integer = 3
    Private _PreEmp As Double() = New Double(2) {0.1883, 0.423, 0.3392}

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double()())) As Double()()
        Return GetOnsets(series.Samples)
    End Function

    <Description(""), DefaultValue(32)>
    Public Property Bands As Integer
        Get
            Return _Bands
        End Get
        Set(value As Integer)
            _Bands = value
        End Set
    End Property

    Private Function GetOnsets(frames As Double()()) As Double()()

        Dim rows As Integer = frames.GetLength(0)
        Dim cols As Integer = frames(0).Length
        Dim concatenated As Double() = New Double(rows * cols - 1) {}      ' rows * cols - 1 

        For row As Integer = 0 To rows - 1
            Array.Copy(frames(row), 0, concatenated, row * frames(row).Length, frames(row).Length)
        Next

        'int deadtime = 128;
        'double H[SUBBANDS],taus[SUBBANDS], N[SUBBANDS];
        'int contact[SUBBANDS], lcontact[SUBBANDS], tsince[SUBBANDS];
        'double overfact = 1.1; /* threshold rel. to actual peak */
        'uint onset_counter = 0;

        '...

        'frames = Eb.size1();
        'bands = Eb.size2();
        'pE = &Eb.data()[0];

        'out = matrix_u(SUBBANDS, frames);
        'onset_counter_for_band = new uint[SUBBANDS];

        'double bn[] = {0.1883, 0.4230, 0.3392}; /* preemph filter */ // new
        'int nbn = 3;
        'double a1 = 0.98;
        'double Y0[SUBBANDS];

        Dim onsets As Double()() = New Double(frames.Length - 1)() {}
        Dim H As Double() = New Double(_Bands - 1) {}
        Dim N As Double() = New Double(_Bands - 1) {}
        Dim taus As Double() = New Double(_Bands - 1) {}
        Dim Y0 As Double() = New Double(_Bands - 1) {}

        Dim contact As Integer() = New Integer(_Bands - 1) {}
        Dim lcontact As Integer() = New Integer(_Bands - 1) {}
        Dim tsince As Integer() = New Integer(_Bands - 1) {}

        Dim frameLen As Integer = frames.Length
        Dim bandOnsetCounts As Integer() = New Integer(_Bands - 1) {}
        Dim onsetCount As Integer = 0

        For j As Integer = 0 To _Bands - 1
            taus(j) = 1.0
            H(j) = concatenated(j)
        Next

        For j As Integer = 0 To frameLen - 1
            onsets(j) = New Double(_Bands - 1) {}
        Next

        For i As Integer = 0 To frameLen - 1

            For j As Integer = 0 To _Bands - 1

                Dim xn As Double = 0

                '/* calculate the filter - FIR part */
                'if (i >= 2*nbn) {
                '    for (int k = 0; k < nbn; ++k) {
                '        xn += bn[k]*(pE[j-SUBBANDS*k] - pE[j-SUBBANDS*(2*nbn-k)]);
                '    }
                '}

                If i >= 2 * _PreEmp.Length Then
                    For k As Integer = 0 To _PreEmpLen - 1
                        ' xn += bn[k]*(pE[j-SUBBANDS*k] - pE[j-SUBBANDS*(2*nbn-k)]);
                        xn += _PreEmp(k) * (concatenated((_Bands - j) * k) - concatenated((_Bands - j) * (2 * _PreEmpLen - k)))
                    Next
                End If

                '/* IIR part */
                'xn = xn + a1*Y0[j];
                xn = xn + _Alpha * Y0(j)

                '/* remember the last filtered level */
                'Y0[j] = xn;
                Y0(j) = xn

                'contact[j] = (xn > H[j])? 1 : 0;
                If xn > H(j) Then contact(j) = 1 Else contact(j) = 0

                'if (contact[j] == 1 && lcontact[j] == 0) {
                '    /* attach - record the threshold level unless we have one */
                '    if(N[j] == 0) {
                '        N[j] = H[j];
                '    }
                '}

                If contact(j) = 1 And lcontact(j) = 0 Then
                    If N(j) = 0 Then
                        N(j) = H(j)
                    End If
                End If

                'if (contact[j] == 1) {
                '    /* update with new threshold */
                '    H[j] = xn * overfact;
                '} else {
                '    /* apply decays */
                '    H[j] = H[j] * exp(-1.0/(double)taus[j]);
                '}

                If contact(j) = 1 Then
                    H(j) = xn * _OverFact
                Else
                    H(j) *= Math.Exp(-1.0 / taus(j))
                End If

                'if (contact[j] == 0 && lcontact[j] == 1) {
                '    /* detach */
                '    if (onset_counter_for_band[j] > 0 && (int)out(j, onset_counter_for_band[j]-1) > i - deadtime) {
                '        // overwrite last-written time
                '        --onset_counter_for_band[j];
                '        --onset_counter;
                '    }
                '    out(j, onset_counter_for_band[j]++) = i;
                '    ++onset_counter;
                '    tsince[j] = 0;
                '}

                If contact(j) = 0 And lcontact(j) = 1 Then
                    If bandOnsetCounts(j) > 0 AndAlso onsets(i)(bandOnsetCounts(j) - 1) > i - _DeadTime Then
                        bandOnsetCounts(j) -= 1
                        onsetCount -= 1
                    End If
                    bandOnsetCounts(j) += 1

                    onsets(i) = N '(j) = xn

                    'onsets(j)(bandOnsetCount(j)) = i
                    'onsets(i)(System.Math.Max(System.Threading.Interlocked.Increment(bandOnsetCounts(j)), bandOnsetCounts(j) - 1)) = i
                    tsince(j) = 0
                End If

                '++tsince[j];
                tsince(j) += 1

                'if (tsince[j] > ttarg) {
                '    taus[j] = taus[j] - 1;
                '    if (taus[j] < 1) taus[j] = 1;
                '} else {
                '    taus[j] = taus[j] + 1;
                '}

                If tsince(j) > 345 Then
                    tsince(j) -= 1
                    If tsince(j) < 1 Then tsince(j) = 1
                Else
                    tsince(j) += 1
                End If

                'if ( (contact[j] == 0) && (tsince[j] > deadtime)) {
                '    /* forget the threshold where we recently hit */
                '    N[j] = 0;
                '}

                If contact(j) = 0 And tsince(j) > _DeadTime Then
                    N(j) = 0
                End If

                'lcontact[j] = contact[j];
                lcontact(j) = contact(j)

            Next
        Next


        Return onsets
    End Function

    <Description(""), DefaultValue(128)>
    Public Property DeadTime As Integer
        Get
            Return _DeadTime
        End Get
        Set(value As Integer)
            _DeadTime = value
        End Set
    End Property

    <Description("Threshold relative to actual peak."), DefaultValue(1.1)>
    Public Property OverFact As Double
        Get
            Return _OverFact
        End Get
        Set(value As Double)
            _OverFact = value
        End Set
    End Property

    <Description(""), DefaultValue(0.98)>
    Public Property Alpha As Double
        Get
            Return _Alpha
        End Get
        Set(value As Double)
            _Alpha = value
        End Set
    End Property

    <Description("Onset Extractor")>
    Public Overrides ReadOnly Property Description As String
        Get
            Return "Onset Extractor"
        End Get
    End Property

End Class
