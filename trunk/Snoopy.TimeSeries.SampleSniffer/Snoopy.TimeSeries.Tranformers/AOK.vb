Imports Snoopy.TimeSeries
Imports System.ComponentModel
Imports Snoopy.TimeSeries.Windows
''' <summary>
''' http://sourceforge.net/p/pamguard/svn/HEAD/tree/PamguardJava/trunk/core/src/pamMaths/WignerTransform.java
''' </summary>
''' <remarks></remarks>
Public Class AOK
    Inherits TimeSeriesTransformer

    Private _FrameWidth As FrameWidth = FrameWidth._128

    Private _FFT As New FFT
    Private _FFTPhase As FFT.Phase = FFT.Phase.Signal

    Private _WindowType As WindowType = WindowType.Hanning
    Private _Window As Window = Windows.Window.Hanning
    Private _Win As Double() = _Window.GetWindow(_FrameWidth)

    Dim i As Integer, j As Integer, k As Integer, ii As Integer
    Dim itemp As Integer

    Dim xlen As Integer = 2048 ' Number of signal samples in input file?
    Dim tlag As Integer = 256 ' Length of sliding analysis window?  (power of two, no larger than 256)
    Dim fftlen As Integer = 256 ' Number of output frequency samples per time-slice?  (power of two)
    Dim tstep As Integer = 128 ' Time increment in samples between time-slice outputs?
    Dim vol As Double = 2.5 ' total kernel volume	' Normalized volume of optimal kernel? (Typically between 1 and 5)

    Dim tlen As Integer, nraf As Integer

    Dim nlag As Integer, mfft As Integer, _
        nrad As Integer, nphi As Integer, nits As Integer
    Dim slen As Integer


    Dim fstep As Integer, outct As Integer

    ' int	maxrad[1024];	/* max radius for each phi s.t. theta < pi */
    Dim maxrad As Integer() = New Integer(1023) {}

    Dim pi As Double, rtemp As Double, rtemp1 As Double, rtemp2 As Double, mu As Double, forget As Double

    Dim outdelay As Double    ' delay in samples of output time from current sample time

    'double	xr[1024],xi[1024];	/* recent data samples	*/
    Dim xr As Double() = New Double(1023) {}
    Dim xi As Double() = New Double(1023) {}
    'double	rectafr[70000];		/* real part of rect running AF	*/
    Dim rectafr As Double() = New Double(69999) {}
    'double	rectafi[70000];		/* imag part of rect running AF	*/
    Dim rectafi As Double() = New Double(69999) {}
    'double  rectafm2[70000];	/* rect |AF|^2	*/
    Dim rectafm2 As Double() = New Double(69999) {}
    'double  polafm2[70000];		/* polar |AF|^2	*/
    Dim polafm2 As Double() = New Double(69999) {}
    'double	rectrotr[70000];	/* real part of rect AF phase shift	*/
    Dim rectrotr As Double() = New Double(69999) {}
    'double	rectroti[70000];	/* imag part of rect AF phase shift	*/
    Dim rectroti As Double() = New Double(69999) {}
    'double	req[70000];		/* rad corresp. to rect coord	*/
    Dim req As Double() = New Double(69999) {}
    'double  pheq[70000];		/* phi corresp. to rect coord	*/
    Dim pheq As Double() = New Double(69999) {}
    'double	plag[70000];		/* lag index at polar AF sample points	*/
    Dim plag As Double() = New Double(69999) {}
    'double	ptheta[70000];		/* theta index at polar AF sample points	*/
    Dim ptheta As Double() = New Double(69999) {}
    'double  sigma[1024];		/* optimal kernel spreads	*/
    Dim sigma As Double() = New Double(1023) {}
    'double  rar[1024],rai[1024];	/* poles for running FFTs for rect AF	*/
    Dim rar As Double() = New Double(1023) {}
    Dim rai As Double() = New Double(1023) {}
    'double  rarN[70000];		/* poles for running FFTs for rect AF	*/
    Dim rarN As Double() = New Double(69999) {}
    'double  raiN[70000];
    Dim raiN As Double() = New Double(69999) {}
    'double  tfslicer[1024];		/* freq slice at current time	*/
    Dim tfslicer As Double() = New Double(1023) {}
    'double  tfslicei[1024];
    Dim tfslicei As Double() = New Double(1023) {}

    'dim tfslice As Double() = New Double(2047) {}

    Public Sub New()
        _FFT.PhaseMode = _FFTPhase

        '	tlag = 64; 
        ' total number of rectangular AF lags	

        nits = CInt(Math.Log(CDbl(tstep) + 2, 2))        ' number of gradient steps to take each time	

        '	nits = 2; 
        '	vol = 2.0;         ' kernel volume (1.0=Heisenberg limit)	

        mu = 0.5        ' gradient descent factor	

        forget = 0.001        ' set no. samples to 0.5 weight on running AF	
        nraf = tlag        ' theta size of rectangular AF	
        nrad = tlag        ' number of radial samples in polar AF	
        nphi = tlag        ' number of angular samples in polar AF 
        outdelay = tlag / 2        ' delay in effective output time in samples	
        ' nlag-1 < outdelay < nraf to prevent "echo" effect 

        nlag = tlag + 1        ' one-sided number of AF lags	
        mfft = fftlen 'po2(fftlen)
        slen = CInt(1.42 * (nlag - 1) + nraf + 3)        ' number of delayed samples to maintain	

        pi = 3.141592654
        vol = (2.0 * vol * nphi * nrad * nrad) / (pi * tlag)        ' normalize volume

        kfill((nrad * nphi), 0.0, polafm2)
        kfill((nraf * nlag), 0.0, rectafr)
        kfill((nraf * nlag), 0.0, rectafi)
        kfill(slen, 0.0, xr)
        kfill(slen, 0.0, xi)
        kfill(nphi, 1.0, sigma)

        tlen = xlen + nraf + 2
        rectamake(nlag, nraf, forget, rar, rai, rarN, raiN)        ' make running rect AF parms	
        plagmake(nrad, nphi, nlag, plag)
        pthetamake(nrad, nphi, nraf, ptheta, maxrad)        ' make running polar AF parms	
        rectrotmake(nraf, nlag, outdelay, rectrotr, rectroti)
        rectopol(nraf, nlag, nrad, nphi, req, pheq)

    End Sub


    Protected Overrides Function OnTransform(series As ITimeSeries(Of Double())) As Double()()

        Dim tf As Double()() = New Double(fftlen - 1)() {}

        xr = series.Samples

        outct = 0

        For ii = 0 To tlen - 1
            cshift(slen, xr)
            cshift(slen, xi)
            If ii < xlen Then
                ' get data	
                'fscanf(ifp, "%lf %lf", AddressOfxr(0), AddressOfxi(0))
            Else
                xr(0) = 0.0
                xi(0) = 0.0
            End If

            rectaf(xr, xi, nlag, nraf, rar, rai, rarN, raiN, rectafr, rectafi)

            If (ii - (ii / tstep) * tstep) = 0 Then  ' output t-f slice	

                outct = outct + 1

                mkmag2((nlag * nraf), rectafr, rectafi, rectafm2)
                polafint(nrad, nphi, nraf, maxrad, nlag, plag, ptheta, rectafm2, polafm2)
                sigupdate(nrad, nphi, nits, vol, mu, maxrad, polafm2, sigma)

                For i = 0 To nlag - 2                    ' for each tau	
                    tfslicer(i) = 0.0
                    tfslicei(i) = 0.0

                    For j = 0 To nraf - 1
                        ' integrate over theta	
                        rtemp = ccmr(rectafr(i * nraf + j), rectafi(i * nraf + j), rectrotr(i * nraf + j), rectroti(i * nraf + j))
                        rtemp1 = ccmi(rectafr(i * nraf + j), rectafi(i * nraf + j), rectrotr(i * nraf + j), rectroti(i * nraf + j))

                        rtemp2 = rectkern(i, j, nraf, nphi, req, pheq, sigma)
                        tfslicer(i) = tfslicer(i) + rtemp * rtemp2
                        '	fprintf(ofp," %d , %d , %g, %g, %g , %g , %g \n", i,j,rectafr[i*nraf+j],rectafi[i*nraf+j],rtemp,rtemp1,rtemp2); 

                        tfslicei(i) = tfslicei(i) + rtemp1 * rtemp2
                    Next

                Next

                For i = nlag - 1 To (fftlen - nlag + 2) - 1
                    ' zero pad for FFT	
                    tfslicer(i) = 0.0
                    tfslicei(i) = 0.0
                Next

                For i = (fftlen - nlag + 2) To fftlen - 1  ' fill in c.c. symmetric half of array	
                    tfslicer(i) = tfslicer(fftlen - i)
                    tfslicei(i) = -tfslicei(fftlen - i)
                Next

                Dim complexSignal As Double() = New Double(2 * tfslicer.Length - 1) {}

                For i As Integer = 0 To tfslicer.Length - 1
                    complexSignal(2 * i) = tfslicer(i)
                    complexSignal(2 * i + 1) = tfslicei(i)
                Next

                _FFT.Complex(tfslicer, True)
                'FFT(fftlen, mfft, tfslicer, tfslicei)

                'tf(ii) = tfslicer 'complexSignal


                'Continue For

                itemp = fftlen \ 2 + fstep
                j = 1
                ' print output slice	
                i = itemp
                While i < fftlen
                    '	    fprintf(ofp, "x( %d , %d )= %g ; \n", outct, j, tfslicer[i]); 

                    'fprintf(ofp, "%g ", tfslicer(i))
                    j = j + 1
                    i = i + fstep
                End While
                i = 0
                While i < itemp
                    '	    fprintf(ofp, "x( %d , %d )= %g ; \n", outct, j, tfslicer[i]); 

                    'fprintf(ofp, "%g ", tfslicer(i))
                    j = j + 1
                    i = i + fstep
                End While
                'fprintf(ofp, vbLf)
                '	    printf("outct = %d \n", outct);

            End If





        Next


        Return tf

    End Function

    '								
    '   rectkern: generate kernel samples on rectangular grid	
    '								
    Private Function rectkern(itau As Integer, itheta As Integer, ntheta As Integer, nphi As Integer, req As Double(), pheq As Double(), sigma As Double()) As Double
        Dim iphi As Integer, iphi1 As Integer
        Dim kern As Double, tsigma As Double

        iphi = CInt(pheq(itau * ntheta + itheta))
        iphi1 = iphi + 1
        If iphi1 > (nphi - 1) Then
            iphi1 = 0
        End If

        tsigma = sigma(iphi) + (pheq(itau * ntheta + itheta) - iphi) * (sigma(iphi1) - sigma(iphi))
        kern = Math.Exp(-req(itau * ntheta + itheta) * req(itau * ntheta + itheta) / (tsigma * tsigma))

        Return (kern)
    End Function

    '								
    '   sigupdate: update RG kernel parameters			
    '								
    Private Sub sigupdate(nrad As Integer, nphi As Integer, nits As Integer, vol As Double, mu0 As Double, maxrad As Integer(), polafm2 As Double(), sigma As Double())

        Dim ii As Integer, i As Integer, j As Integer
        'double	grad[1024]
        Dim grad As Double() = New Double(1023) {}
        Dim gradsum As Double, gradsum1 As Double, tvol As Double, volfac As Double, eec As Double, ee1 As Double, _
            ee2 As Double, mu As Double

        For ii = 0 To nits - 1
            gradsum = 0.0
            gradsum1 = 0.0

            For i = 0 To nphi - 1
                grad(i) = 0.0

                ee1 = Math.Exp(-1.0 / (sigma(i) * sigma(i)))                ' use Kaiser's efficient method 
                ee2 = 1.0
                eec = ee1 * ee1

                For j = 1 To maxrad(i) - 1
                    ee2 = ee1 * ee2
                    ee1 = eec * ee1

                    grad(i) = grad(i) + j * j * j * ee2 * polafm2(i * nrad + j)
                Next
                grad(i) = grad(i) / (sigma(i) * sigma(i) * sigma(i))

                gradsum = gradsum + grad(i) * grad(i)
                gradsum1 = gradsum1 + sigma(i) * grad(i)
            Next

            gradsum1 = 2.0 * gradsum1
            If gradsum < 0.0000001 Then
                gradsum = 0.0000001
            End If
            If gradsum1 < 0.0000001 Then
                gradsum1 = 0.0000001
            End If

            mu = (Math.Sqrt(gradsum1 * gradsum1 + 4.0 * gradsum * vol * mu0) - gradsum1) / (2.0 * gradsum)

            tvol = 0.0

            For i = 0 To nphi - 1
                sigma(i) = sigma(i) + mu * grad(i)
                If sigma(i) < 0.5 Then
                    sigma(i) = 0.5
                End If
                '	    printf("sigma[%d] = %g\n", i,sigma[i]); 

                tvol = tvol + sigma(i) * sigma(i)
            Next

            volfac = Math.Sqrt(vol / tvol)
            For i = 0 To nphi - 1
                sigma(i) = volfac * sigma(i)
            Next
        Next

    End Sub

    '								
    '   polafint: interpolate AF on polar grid;			
    '								
    Private Sub polafint(nrad As Integer, nphi As Integer, ntheta As Integer, maxrad As Integer(), nlag As Integer, plag As Double(), _
        ptheta As Double(), rectafm2 As Double(), polafm2 As Double())
        Dim i As Integer, j As Integer
        Dim ilag As Integer, itheta As Integer, itheta1 As Integer
        Dim rlag As Double, rtheta As Double, rtemp As Double, rtemp1 As Double

        For i = 0 To nphi \ 2 - 1
            ' for all phi ...	
            For j = 0 To maxrad(i) - 1
                ' and all radii with |theta| < pi 
                ilag = CInt(plag(i * nrad + j))
                rlag = plag(i * nrad + j) - ilag

                If ilag >= nlag Then
                    polafm2(i * nrad + j) = 0.0
                Else
                    itheta = CInt(ptheta(i * nrad + j))
                    rtheta = ptheta(i * nrad + j) - itheta

                    itheta1 = itheta + 1
                    If itheta1 >= ntheta Then
                        itheta1 = 0
                    End If

                    rtemp = (rectafm2(ilag * ntheta + itheta1) - rectafm2(ilag * ntheta + itheta)) * rtheta + rectafm2(ilag * ntheta + itheta)
                    rtemp1 = (rectafm2((ilag + 1) * ntheta + itheta1) - rectafm2((ilag + 1) * ntheta + itheta)) * rtheta + rectafm2((ilag + 1) * ntheta + itheta)
                    polafm2(i * nrad + j) = (rtemp1 - rtemp) * rlag + rtemp
                End If
            Next
        Next


        For i = nphi \ 2 To nphi - 1
            ' for all phi ...	
            For j = 0 To maxrad(i) - 1
                ' and all radii with |theta| < pi 
                ilag = CInt(plag(i * nrad + j))
                rlag = plag(i * nrad + j) - ilag

                If ilag >= nlag Then
                    polafm2(i * nrad + j) = 0.0
                Else
                    itheta = CInt(ptheta(i * nrad + j))
                    rtheta = ptheta(i * nrad + j) - itheta

                    rtemp = (rectafm2(ilag * ntheta + itheta + 1) - rectafm2(ilag * ntheta + itheta)) * rtheta + rectafm2(ilag * ntheta + itheta)
                    rtemp1 = (rectafm2((ilag + 1) * ntheta + itheta + 1) - rectafm2((ilag + 1) * ntheta + itheta)) * rtheta + rectafm2((ilag + 1) * ntheta + itheta)
                    polafm2(i * nrad + j) = (rtemp1 - rtemp) * rlag + rtemp
                End If
            Next
        Next
    End Sub

    '								
    '   mkmag2: compute squared magnitude of an array		
    '								
    Private Sub mkmag2(tlen As Integer, xr As Double(), xi As Double(), xm2 As Double())
        Dim i As Integer
        For i = 0 To tlen - 1
            xm2(i) = xr(i) * xr(i) + xi(i) * xi(i)

        Next
    End Sub

    '   rectaf: generate running AF on rectangular grid;		
    '	     negative lags, all DFT frequencies			
    '								
    Private Sub rectaf(xr As Double(), xi As Double(), laglen As Integer, freqlen As Integer, alphar As Double(), alphai As Double(), _
        alpharN As Double(), alphaiN As Double(), afr As Double(), afi As Double())

        Dim i As Integer, j As Integer
        Dim rtemp As Double, rr As Double, ri As Double, rrN As Double, riN As Double
        'extern	double	cmr(),cmi();
        'extern	double	ccmr(),ccmi();

        For i = 0 To laglen - 1
            rr = ccmr(xr(0), xi(0), xr(i), xi(i))
            ri = ccmi(xr(0), xi(0), xr(i), xi(i))

            rrN = ccmr(xr(freqlen - i), xi(freqlen - i), xr(freqlen), xi(freqlen))
            riN = ccmi(xr(freqlen - i), xi(freqlen - i), xr(freqlen), xi(freqlen))

            For j = 0 To freqlen - 1
                rtemp = cmr(afr(i * freqlen + j), afi(i * freqlen + j), alphar(j), alphai(j)) - cmr(rrN, riN, alpharN(i * freqlen + j), alphaiN(i * freqlen + j)) + rr
                afi(i * freqlen + j) = cmi(afr(i * freqlen + j), afi(i * freqlen + j), alphar(j), alphai(j)) - cmi(rrN, riN, alpharN(i * freqlen + j), alphaiN(i * freqlen + j)) + ri
                afr(i * freqlen + j) = rtemp
            Next

        Next
    End Sub

    '								
    '   cmr: computes real part of x times y			
    '								
    Private Function cmr(xr As Double, xi As Double, yr As Double, yi As Double) As Double
        Dim rtemp As Double
        rtemp = xr * yr - xi * yi
        Return (rtemp)
    End Function

    '								
    '   cmi: computes imaginary part of x times y			
    '								
    Private Function cmi(xr As Double, xi As Double, yr As Double, yi As Double) As Double
        Dim rtemp As Double
        rtemp = xi * yr + xr * yi
        Return (rtemp)
    End Function

    '								
    '   ccmr: computes real part of x times y*			
    '								
    Private Function ccmr(xr As Double, xi As Double, yr As Double, yi As Double) As Double
        Dim rtemp As Double
        rtemp = xr * yr + xi * yi
        Return (rtemp)
    End Function

    '								
    '   ccmi: computes imaginary part of x times y*		
    '								
    Private Function ccmi(xr As Double, xi As Double, yr As Double, yi As Double) As Double
        Dim rtemp As Double
        rtemp = xi * yr - xr * yi
        Return (rtemp)
    End Function

    '								
    '   cshift: circularly shift an array				
    '								
    Private Sub cshift(len As Integer, x As Double())

        Dim i As Integer
        Dim rtemp As Double

        rtemp = x(len - 1)

        For i = len - 1 To 1 Step -1
            x(i) = x(i - 1)
        Next

        x(0) = rtemp
    End Sub

    '								
    '   rectopol: find polar indices corresponding to rect samples	
    '								
    Private Sub rectopol(nraf As Integer, nlag As Integer, nrad As Integer, nphi As Integer, req As Double(), pheq As Double())

        Dim i As Integer, j As Integer, jt As Integer
        Dim pi As Double, deltau As Double, deltheta As Double, delrad As Double, delphi As Double

        pi = 3.141592654

        deltau = Math.Sqrt(pi / (nlag - 1))
        deltheta = 2.0 * Math.Sqrt((nlag - 1) * pi) / nraf
        delrad = Math.Sqrt(2.0 * pi * (nlag - 1)) / nrad
        delphi = pi / nphi

        For i = 0 To nlag - 1
            For j = 0 To nraf \ 2 - 1
                req(i * nraf + j) = Math.Sqrt(i * i * deltau * deltau + j * j * deltheta * deltheta) / delrad
                If i = 0 Then
                    pheq(i * nraf + j) = 0.0
                Else
                    pheq(i * nraf + j) = (Math.Atan((j * deltheta) / (i * deltau)) + 1.570796327) / delphi
                End If
            Next

            For j = 0 To nraf \ 2 - 1
                jt = j - nraf \ 2
                req(i * nraf + nraf \ 2 + j) = Math.Sqrt(i * i * deltau * deltau + jt * jt * deltheta * deltheta) / delrad
                If i = 0 Then
                    pheq(i * nraf + nraf \ 2 + j) = 0.0
                Else
                    pheq(i * nraf + nraf \ 2 + j) = (Math.Atan((jt * deltheta) / (i * deltau)) + 1.570796327) / delphi
                End If
            Next

        Next
    End Sub

    '								
    '   rectrotmake: make array of rect AF phase shifts		
    '								
    Private Sub rectrotmake(nraf As Integer, nlag As Integer, outdelay As Double, rectrotr As Double(), rectroti As Double())
        Dim i As Integer, j As Integer
        Dim twopin As Double

        twopin = 6.283185307 / nraf

        For i = 0 To nlag - 1
            For j = 0 To nraf \ 2 - 1 ' / --> \
                rectrotr(i * nraf + j) = Math.Cos((twopin * j) * (outdelay - CDbl(i) / 2.0))
                rectroti(i * nraf + j) = Math.Sin((twopin * j) * (outdelay - CDbl(i) / 2.0))
            Next
            For j = nraf \ 2 To nraf - 1 ' / --> \
                rectrotr(i * nraf + j) = Math.Cos((twopin * (j - nraf)) * (outdelay - CDbl(i) / 2.0))
                rectroti(i * nraf + j) = Math.Sin((twopin * (j - nraf)) * (outdelay - CDbl(i) / 2.0))
            Next

        Next
    End Sub

    '								
    '   pthetamake: make matrix of theta indices for polar samples	
    '								
    Private Sub pthetamake(nrad As Integer, nphi As Integer, ntheta As Integer, ptheta As Double(), maxrad As Integer())
        Dim i As Integer, j As Integer
        Dim theta As Double, rtemp As Double, deltheta As Double

        deltheta = 6.283185307 / ntheta

        For i = 0 To nphi - 1
            ' for all phi ...	
            maxrad(i) = nrad

            For j = 0 To nrad - 1
                ' and all radii	
                theta = -((4.442882938 / nrad) * j) * Math.Cos((3.141592654 * i) / nphi)
                If theta >= 0.0 Then
                    rtemp = theta / deltheta
                    If rtemp > (ntheta / 2 - 1) Then
                        rtemp = -1.0
                        If j < maxrad(i) Then
                            maxrad(i) = j
                        End If
                    End If
                Else
                    rtemp = (theta + 6.283185307) / deltheta
                    If rtemp < (ntheta / 2 + 1) Then
                        rtemp = -1.0
                        If j < maxrad(i) Then
                            maxrad(i) = j
                        End If
                    End If
                End If

                ptheta(i * nrad + j) = rtemp
            Next

        Next
    End Sub

    '								
    '   plagmake: make matrix of lags for polar running AF		
    '								
    Private Sub plagmake(nrad As Integer, nphi As Integer, nlag As Integer, plag As Double())
        Dim i As Integer, j As Integer

        ' extern	double	mklag();

        For i = 0 To nphi - 1
            ' for all phi ...      
            For j = 0 To nrad - 1
                ' and all radii        
                plag(i * nrad + j) = mklag(nrad, nphi, nlag, i, j)
            Next

        Next
    End Sub

    '								
    '   mklag: compute radial sample lag				
    '								
    Private Function mklag(nrad As Integer, nphi As Integer, nlag As Integer, iphi As Integer, jrad As Integer) As Double
        Dim delay As Double

        delay = ((1.414213562 * (nlag - 1) * jrad) / nrad) * Math.Sin((3.141592654 * iphi) / nphi)

        Return (delay)
    End Function

    '								
    '   rectamake: make vector of poles for rect running AF	
    '								
    Private Sub rectamake(nlag As Integer, n As Integer, forget As Double, ar As Double(), ai As Double(), arN As Double(), aiN As Double())
        Dim i As Integer, j As Integer
        Dim trig As Double, decay As Double
        Dim trigN As Double, decayN As Double

        trig = 6.283185307 / n
        decay = Math.Exp(-forget)

        For j = 0 To n - 1
            ar(j) = decay * Math.Cos(trig * j)
            ai(j) = decay * Math.Sin(trig * j)
        Next

        For i = 0 To nlag - 1
            trigN = 6.283185307 * (n - i)
            trigN = trigN / n
            decayN = Math.Exp(-forget * (n - i))

            For j = 0 To n - 1
                arN(i * n + j) = decayN * Math.Cos(trigN * j)
                aiN(i * n + j) = decayN * Math.Sin(trigN * j)
            Next

        Next
    End Sub

    '								
    '   zerofill: set array elements to constant			
    '								
    Private Sub kfill(len As Integer, k As Double, x As Double())
        Dim i As Integer
        For i = 0 To len - 1
            x(i) = k
        Next
    End Sub
 
    <TypeConverter(GetType(EnumIntValueConverter)), DefaultValue(FrameWidth._128)>
    Public Overridable Property FrameWidth As FrameWidth
        Get
            Return _FrameWidth
        End Get
        Set(value As FrameWidth)
            If _FrameWidth <> value Then
                _FrameWidth = value
                _Win = Windows.Window.GetInstance(_WindowType).GetWindow(_FrameWidth)
            End If
        End Set
    End Property

    <DisplayName("Phase"), Description("FFT phase mode."), DefaultValue(FFT.Phase.Signal)>
    Public Property PhaseMode As FFT.Phase
        Get
            Return _FFTPhase
        End Get
        Set(value As FFT.Phase)
            If value <> _FFTPhase Then
                _FFTPhase = value
                _FFT.PhaseMode = _FFTPhase
            End If
        End Set
    End Property

    ''' <summary>
    ''' The type of window to be applied to each frame.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Description("The type of window to be applied."), DefaultValue(WindowType.Hanning)>
    Public Property Window As WindowType
        Get
            Return _WindowType
        End Get
        Set(value As WindowType)
            If value <> _WindowType Then
                _WindowType = value
                _Window = Windows.Window.GetInstance(_WindowType)
                _Win = _Window.GetWindow(_FrameWidth)
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Adaptive Optimal-Kernel (AOK)"
        End Get
    End Property

End Class
