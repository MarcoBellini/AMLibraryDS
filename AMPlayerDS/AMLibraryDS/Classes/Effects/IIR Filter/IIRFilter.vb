Imports System.Math
Public Class IIRFilter
    Private Const LN2 As Double = 0.693147180559945

    Private Enum IIR_TYPE
        IIR_LOW_PASS
        IIR_HIGH_PASS
        IIR_BAND_PASS
        IIR_PEAK_EQ
    End Enum

    Private m_fa0 As Double
    Private m_fa1 As Double
    Private m_fa2 As Double
    Private m_fa3 As Double
    Private m_fa4 As Double

    Private m_fX1 As Double
    Private m_fX2 As Double
    Private m_fY1 As Double
    Private m_fY2 As Double

    Private m_intSR As Integer
    Private m_sngDB As Double
    Private m_sngFreq As Double
    Private m_sngBW As Double

    Private m_blnEnabled As Boolean

    ''' <summary>
    ''' Equalizerband aktiviert
    ''' </summary>
    Public Property Enabled(
    ) As Boolean

        Get
            Return m_blnEnabled
        End Get

        Set(ByVal value As Boolean)
            m_blnEnabled = value
        End Set

    End Property

    ''' <summary>
    ''' Sample rate Of the input signal, In Herz
    ''' </summary>
    Public Property InputSamplerate(
    ) As Integer

        Get
            Return m_intSR
        End Get

        Set(ByVal value As Integer)
            m_intSR = value
            UpdateSettings()
        End Set

    End Property

    ''' <summary>
    ''' Bandwidth in octaves
    ''' </summary>
    Public Property Bandwidth(
    ) As Double

        Get
            Return m_sngBW
        End Get

        Set(ByVal value As Double)
            m_sngBW = value
            UpdateSettings()
        End Set

    End Property

    ''' <summary>
    ''' Verstärkung des Bands in Dezibel
    ''' </summary>
    Public Property Gain(
    ) As Double

        Get
            Return m_sngDB
        End Get

        Set(ByVal value As Double)
            m_sngDB = value
            UpdateSettings()
        End Set

    End Property

    ''' <summary>
    ''' Zentrale Frequenz des Bands in Herz
    ''' </summary>
    Public Property Frequency(
    ) As Double

        Get
            Return m_sngFreq
        End Get

        Set(ByVal value As Double)
            m_sngFreq = value
            UpdateSettings()
        End Set

    End Property

    ''' <summary>
    ''' Filter auf 16 Bit Samples anwenden
    ''' </summary>
    Friend Sub ProcessSamples_Float(
        ByRef dblSamples() As Double
    )

        Dim result As Double
        Dim i As Integer

        If Not m_blnEnabled Then Exit Sub

        For i = 0 To dblSamples.Length - 1
            result = m_fa0 * dblSamples(i) + m_fa1 * m_fX1 + m_fa2 * m_fX2 - m_fa3 * m_fY1 - m_fa4 * m_fY2

            m_fX2 = m_fX1
            m_fX1 = dblSamples(i)

            m_fY2 = m_fY1
            m_fY1 = result

            If result > 1.0# Then
                dblSamples(i) = 1.0#
            ElseIf result < -1.0# Then
                dblSamples(i) = -1.0#
            Else
                dblSamples(i) = result
            End If
        Next

    End Sub

    ''' <summary>
    ''' Filter auf 16 Bit Samples anwenden
    ''' </summary>
    Friend Sub ProcessSamples_16bit(
        ByRef intSamples() As Short
    )

        Dim result As Double
        Dim Sample As Double
        Dim i As Integer

        If Not m_blnEnabled Then Exit Sub

        For i = 0 To intSamples.Length - 1
            Sample = intSamples(i) / 32767
            result = m_fa0 * Sample + m_fa1 * m_fX1 + m_fa2 * m_fX2 - m_fa3 * m_fY1 - m_fa4 * m_fY2

            m_fX2 = m_fX1
            m_fX1 = Sample

            m_fY2 = m_fY1
            m_fY1 = result

            If result > 1.0# Then
                intSamples(i) = 32767
            ElseIf result < -1.0# Then
                intSamples(i) = -32768
            Else
                intSamples(i) = CShort(result * 32767)
            End If
        Next

    End Sub

    ''' <summary>
    ''' Aktualisiert das Filter bei geänderten Parametern
    ''' </summary>
    Private Sub UpdateSettings(
    )

        If m_intSR > 0 Then
            CreateBiquadIIR(IIR_TYPE.IIR_PEAK_EQ, m_sngDB, m_sngFreq, m_sngBW, m_intSR)
        End If

    End Sub

    ''' <summary>
    ''' IIR Filter Koeffizienten berechnen
    ''' </summary>
    ''' <param name="iirtype">Typ des Filters (Lowpass, Highpass, Bandpass, Peak EQ)</param>
    ''' <param name="dBGain">Verstärkung des Bands in dB</param>
    ''' <param name="freq">Zentralfrequenz des Bands in Hz</param>
    ''' <param name="bw">Bandbreite in Oktaven</param>
    ''' <param name="srate">Samplerate des Eingangssignals</param>
    ''' <remarks>von http://www.dspguru.com/sw/lib/biquad.c</remarks>
    Private Sub CreateBiquadIIR(
        ByVal iirtype As IIR_TYPE,
        ByVal dBGain As Double,
        ByVal freq As Double,
        ByVal bw As Double,
        ByVal srate As Integer
    )

        Dim A As Double, omega As Double
        Dim sn As Double, cs As Double
        Dim Alpha As Double, Beta As Double

        Dim a0 As Double, a1 As Double, a2 As Double
        Dim b0 As Double, b1 As Double, b2 As Double

        m_intSR = srate
        m_sngBW = bw
        m_sngDB = dBGain
        m_sngFreq = freq

        A = 10 ^ (dBGain / 40)
        omega = 2 * PI * freq / srate

        ' Edit: Check if omega is = PI
        ' prevent division by 0
        If omega = Math.PI Then
            m_fa0 = 1
            m_fa1 = 0
            m_fa2 = 0
            m_fa3 = 0
            m_fa4 = 0

            Exit Sub
        End If

        sn = Sin(omega)
        cs = Cos(omega)
        Alpha = sn * Sinh(LN2 / 2 * bw * omega / sn)
        Beta = Sqrt(A + A)

        Select Case iirtype
            Case IIR_TYPE.IIR_LOW_PASS
                b0 = (1 - cs) / 2
                b1 = 1 - cs
                b2 = (1 - cs) / 2
                a0 = 1 + Alpha
                a1 = -2 * cs
                a2 = 1 - Alpha
            Case IIR_TYPE.IIR_HIGH_PASS
                b0 = (1 + cs) / 2
                b1 = -(1 + cs)
                b2 = (1 + cs) / 2
                a0 = 1 + Alpha
                a1 = -2 * cs
                a2 = 1 - Alpha
            Case IIR_TYPE.IIR_BAND_PASS
                b0 = Alpha
                b1 = 0
                b2 = -Alpha
                a0 = 1 + Alpha
                a1 = -2 * cs
                a2 = 1 - Alpha
            Case IIR_TYPE.IIR_PEAK_EQ
                b0 = 1 + (Alpha * A)
                b1 = -2 * cs
                b2 = 1 - (Alpha * A)
                a0 = 1 + (Alpha / A)
                a1 = -2 * cs
                a2 = 1 - (Alpha / A)
        End Select

        m_fa0 = b0 / a0
        m_fa1 = b1 / a0
        m_fa2 = b2 / a0
        m_fa3 = a1 / a0
        m_fa4 = a2 / a0

        m_fX1 = 0 : m_fX2 = 0
        m_fY1 = 0 : m_fY2 = 0

    End Sub
End Class