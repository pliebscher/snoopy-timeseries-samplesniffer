Imports System.ComponentModel
''' <summary>
''' http://elki.dbs.ifi.lmu.de/browser/elki/trunk/src/de/lmu/ifi/dbs/elki/utilities/scaling
''' </summary>
''' <remarks></remarks>
<Description("Gamma scaling function.")>
Public Class LogNegativeScaler
    Inherits TimeSeriesPreprocessor

    Protected Overrides Function OnProcess(series As ITimeSeries(Of Double())) As Double()

        Dim h As Double() = series.Samples
        Dim N As Integer = series.Samples.Length

        Dim Ui As New List(Of Integer) ' Upper indices
        Dim Uv As New List(Of Double) ' values
        Dim Li As New List(Of Integer) ' Lower indices
        Dim Lv As New List(Of Double) ' values

        If h(0) < h(1) Then ' rising from the start?
            Ui.Add(0)
            Uv.Add(h(0))
            Lv.Add(0)
        Else
            Li.Add(0)
            Lv.Add(h(0))
            Uv.Add(0)
        End If

        For i As Integer = 2 To h.Length - 3

            If h(i - 1) < h(i) And h(i) > h(i + 1) Then ' at a peak?
                Ui.Add(i)
                Uv.Add(h(i))
                Lv.Add(0)
            ElseIf h(i - 1) > h(i) And h(i) < h(i + 1) Then ' at a valley?
                Li.Add(i)
                Lv.Add(h(i))
                Uv.Add(0)
            End If

        Next

        If h(N - 1) > h(N - 2) Then ' rising at the end?
            Ui.Add(N - 1)
            Uv.Add(h(N - 1))
            Lv.Add(0)
        Else
            Li.Add(N - 1)
            Lv.Add(h(N - 1))
            Uv.Add(0)
        End If

        '% find local max/min points
        'd = diff(h); % approximate derivative
        Dim d As Double() = New Double(N - 1) {}
        For i As Integer = 1 To d.Length - 2
            d(i) = (h(i - 1) - h(i))
        Next

        'maxmin = []; % to store the optima (min and max without distinction so far)
        Dim maxmin As New List(Of Integer)

        'for i=1:N-2
        '   if d(i)==0                        % we are on a zero
        '      maxmin = [maxmin, i];
        '   elseif sign(d(i))~=sign(d(i+1))   % we are straddling a zero so
        '      maxmin = [maxmin, i+1];        % define zero as at i+1 (not i)
        '   End
        'End

        For i As Integer = 1 To series.Samples.Length - 1
            If d(i) = 0 Then
                maxmin.Add(i)
            ElseIf (d(i) > 0 And d(i + 1) < 0) OrElse (d(i) < 0 And d(i + 1) > 0) Then
                maxmin.Add(i + 1)
            End If
        Next

        '% divide maxmin into maxes and mins
        'if maxmin(1)>maxmin(2)              % first one is a max not a min
        '   maxes = maxmin(1:2:length(maxmin));
        '   mins  = maxmin(2:2:length(maxmin));
        'else                                % is the other way around
        '   maxes = maxmin(2:2:length(maxmin));
        '   mins  = maxmin(1:2:length(maxmin));
        '                  End

        Dim maxes As Integer() = New Integer(maxmin.Count \ 2 + 1) {}
        Dim mins As Integer() = New Integer(maxmin.Count \ 2 + 1) {}

        '% make endpoints both maxes and mins
        'maxes = [1 maxes N];
        'mins  = [1 mins  N];

        maxes(0) = 0
        maxes(maxes.Length - 1) = N - 1
        mins(0) = 0
        mins(mins.Length - 1) = N - 1

        For i As Integer = 0 To maxmin.Count \ 2 - 1
            maxes(i + 1) = maxmin(2 * i + 1)
            mins(i + 1) = maxmin(2 * i)
        Next

        Dim interp As Double() = New Double(N - 1) {}

        If maxes(maxes.Length - 1) = maxes(maxes.Length - 2) Then
            maxes(maxes.Length - 1) += 1
        End If

        For i As Integer = 0 To maxes.Length - 2 Step 2

            '    Debug.Write(i & ": ")

            For j As Integer = maxes(i) To maxes(i + 1)
                '    Debug.Write(j & ", ")
                Dim w As Double ' -1
                Dim x As Double '  0 (center)
                Dim y As Double ' +1
                Dim z As Double ' +2

                Try
                    If j = 0 Then
                        w = 0
                        x = 0
                        y = h(j)
                        z = h(j + 1)
                    ElseIf j > h.Length - 1 Then
                        Exit For
                    ElseIf j = h.Length - 1 Then
                        w = h(j - 1)
                        x = h(j)
                        y = 0
                        z = 0
                    ElseIf j = h.Length - 2 Then
                        w = h(j - 1)
                        x = h(j)
                        y = h(j + 1)
                        z = 0
                    Else
                        w = h(j - 1)
                        x = h(j)
                        y = h(j + 1)
                        z = h(j + 2)
                    End If
                    interp(j) = cubicspline(_InterpLocation, w, x, y, z)
                Catch ex As Exception
                    Throw
                End Try

            Next



            'Debug.WriteLine("")

        Next




        Return interp

        For i As Integer = 0 To series.Samples.Length - 1
            series.Samples(i) = -Math.Log(series.Samples(i))
        Next

        Return series.Samples
    End Function

    '         * Cubic spline interpolation.
    '         * Also known as Catmull-Rom spline
    '         * Calculate sample = h(x) a + h(x) b + h(x) c + h(x) d,
    '         * where a,b,c,d are the values at the sample locations, and x (-2,2) is the location between
    '         * a and d.
    '         * @param x {0,1} the location at which to interpolate the sample
    '         * @param a,b,c,d the values of the samples at -1, 0, 1, 2
    '         * @return the interpolated value
    '         
    Private Shared Function cubicspline(x As Double, a As Double, b As Double, c As Double, d As Double) As Double
        Return h(-1.0 - x) * a + h(-x) * b + h(1.0 - x) * c + h(2.0 - x) * d
    End Function

    '*
    '         * Calculate spline h(x) value.
    '         *              h(-x)                           x<0
    '         * h(x) =       3/2x^3-5/2x^2+1                 0 <= x < 1
    '         *              -1/2x^3 + 5/2x^2 - 4x + 2       1 <= x < 2
    '         *              0                               otherwise
    '         *  @param x the value for which to calculate h.
    '         *  @return the h value as above.
    '         
    Private Shared Function h(x As Double) As Double
        If x < 0 Then
            x = -x
        End If
        If x >= 0 AndAlso x < 1 Then
            Return Math.Pow(x, 3) * 1.5 - 2.5 * Math.Pow(x, 2) + 1.0
        ElseIf x >= 1 AndAlso x < 2 Then
            Return -Math.Pow(x, 3) * 0.5 + 2.5 * Math.Pow(x, 2) - 4.0 * x + 2.0
        Else
            Return 0
        End If
    End Function

    Private _InterpLocation As Double = 0.5
    Public Property InterpLocation As Double
        Get
            Return _InterpLocation
        End Get
        Set(value As Double)
            _InterpLocation = value
        End Set
    End Property

End Class



'% EMD:  Emprical mode decomposition
'%
'% imf = emd(x)
'%
'% x   - input signal (must be a column vector)
'%
'% This version will calculate all the imf's (longer)
'%
'% imf - Matrix of intrinsic mode functions (each as a row)
'%       with residual in last row.
'%
'% See:  Huang et al, Royal Society Proceedings on Math, Physical, 
'%       and Engineering Sciences, vol. 454, no. 1971, pp. 903-995, 
'%       8 March 1998
'%
'% Author: Ivan Magrin-Chagnolleau  <ivan@ieee.org>
'% 

'function imf = emd(x);

'c = x(:)'; % copy of the input signal (as a row vector)
'N = length(x);

'%-------------------------------------------------------------------------
'% loop to decompose the input signal into successive IMF

'imf = []; % Matrix which will contain the successive IMF, and the residue

'while (1) % the stop criterion is tested at the end of the loop

'   %-------------------------------------------------------------------------
'   % inner loop to find each imf

'   h = c; % at the beginning of the sifting process, h is the signal
'   SD = 1; % Standard deviation which will be used to stop the sifting process



'        While SD > 0.3
'      % while the standard deviation is higher than 0.3 (typical value)

'      % find local max/min points
'      d = diff(h); % approximate derivative
'      maxmin = []; % to store the optima (min and max without distinction so far)
'      for i=1:N-2
'         if d(i)==0                        % we are on a zero
'            maxmin = [maxmin, i];
'         elseif sign(d(i))~=sign(d(i+1))   % we are straddling a zero so
'            maxmin = [maxmin, i+1];        % define zero as at i+1 (not i)
'         End
'      End

'      if size(maxmin,2) < 2 % then it is the residue
'                        break()
'                        End

'      % divide maxmin into maxes and mins
'      if maxmin(1)>maxmin(2)              % first one is a max not a min
'         maxes = maxmin(1:2:length(maxmin));
'         mins  = maxmin(2:2:length(maxmin));
'      else                                % is the other way around
'         maxes = maxmin(2:2:length(maxmin));
'         mins  = maxmin(1:2:length(maxmin));
'                            End

'      % make endpoints both maxes and mins
'      maxes = [1 maxes N];
'      mins  = [1 mins  N];


'      %-------------------------------------------------------------------------
'      % spline interpolate to get max and min envelopes; form imf
'      maxenv = spline(maxes,h(maxes),1:N);
'      minenv = spline(mins, h(mins),1:N);

'      m = (maxenv + minenv)/2; % mean of max and min enveloppes
'      prevh = h; % copy of the previous value of h before modifying it
'      h = h - m; % substract mean to h

'      % calculate standard deviation
'      eps = 0.0000001; % to avoid zero values
'      SD = sum ( ((prevh - h).^2) ./ (prevh.^2 + eps) );

'                            End



'   imf = [imf; h]; % store the extracted IMF in the matrix imf
'   % if size(maxmin,2)<2, then h is the residue

'   % stop criterion of the algo.
'                            If size(maxmin, 2) < 2 Then
'                                break()
'                                End

'   c = c - h; % substract the extracted IMF from the signal

'                                End

'                                Return



