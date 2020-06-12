' Porting of a part of foo_dsp_effect. Used only to show how to apply DSP in VB.NET
' Foobar player: https://www.foobar2000.org/
' Plugin Page: https://github.com/mudlord/foobar2000-plugins/tree/master/foo_dsp_effect
' Effect page: https://github.com/mudlord/foobar2000-plugins/blob/master/foo_dsp_effect/phaser.cpp

Public Class Phaser

    Private Const PHASER_LFO_SHAPE As Double = 4.0#
    Private Const LFO_SKIP_SAMPLES As Integer = 30

    Private OldArray(24) As Double
    Private SkipCount As ULong
    Private Gain As Double
    Private fbout As Double
    Private LFOskip As Double
    Private Phase As Double

    Public Property LFOFrequency As Double

    Public Property LFOStartPhase As Double

    Public Property Feedback As Double
    Public Property Depth As Integer
    Public Property Stages As Integer
    Public Property DryWet As Integer

    Public Sub New()
        Stages = 2
        DryWet = 128
        LFOStartPhase = 0.0#
        LFOFrequency = 1.0#
        Depth = 100
        Feedback = 20.0#
    End Sub

    Public Sub New(ByVal samplerate As Integer)
        Me.New()
        PhaserInit(samplerate)
    End Sub

    Public Sub PhaserInit(ByVal samplerate As Integer)
        SkipCount = 0
        Gain = 0.0#
        fbout = 0.0#

        LFOskip = LFOFrequency * 2.0# * Math.PI / samplerate
        Phase = LFOStartPhase * Math.PI / 180.0#

        Array.Clear(OldArray, 0, OldArray.Length)
    End Sub

    Public Function PhaserProcess(ByRef sample As Double) As Double
        Dim m, tmp As Double

        m = sample + fbout * Feedback / 100.0#

        SkipCount += 1

        If (SkipCount Mod LFO_SKIP_SAMPLES = 0) Then
            Gain = (1.0# + Math.Cos(SkipCount * LFOskip + Phase)) / 2.0#
            Gain = (Math.Exp(Gain * PHASER_LFO_SHAPE) - 1.0#) / (Math.Exp(PHASER_LFO_SHAPE) - 1.0#)
            Gain = 1 - Gain / 255.0# * Depth
        End If

        For i As Integer = 0 To Stages - 1
            tmp = OldArray(i)
            OldArray(i) = Gain * tmp + m
            m = tmp - Gain * OldArray(i)
        Next

        fbout = m

        Return (m * DryWet + sample * (255.0# - DryWet)) / 255.0#
    End Function
End Class
