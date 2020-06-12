

Public Class DSPEqualizer
    Implements ISoundEffect

    Private sWaveFormat As WaveFormatEx

    Private pEQVal As Integer = 0
    Private pEQ As New List(Of IIRFilter)
    Private bEnabled As Boolean = False
    Private nGainValue As Double = 0

    ''' <summary>
    ''' Close current instance
    ''' </summary>
    Public Sub DeInit() Implements ISoundEffect.DeInit

        ' Close equalizer
        EqualizerBand = 0
    End Sub

    ''' <summary>
    ''' Show dialog
    ''' </summary>
    Public Sub ConfigurationDialog() Implements ISoundEffect.ConfigurationDialog
        Me.Show()
    End Sub

    ''' <summary>
    ''' Setup Equalization bands
    ''' </summary>
    ''' <returns></returns>
    Public Function InitPlugin() As Boolean Implements ISoundEffect.Init

        ' Init 10 classes of IIRFIlter
        EqualizerBand = 10

        ' Reload saved settings
        With CurrentEqualizerBand(0)
            .Bandwidth = 2
            .Enabled = True
            .Gain = My.Settings.DSPEqualizer_30_Gain
            .Frequency = 30
        End With

        With CurrentEqualizerBand(1)
            .Bandwidth = 2
            .Enabled = True
            .Gain = My.Settings.DSPEqualizer_60_Gain
            .Frequency = 60
        End With

        With CurrentEqualizerBand(2)
            .Bandwidth = 2
            .Enabled = True
            .Gain = My.Settings.DSPEqualizer_125_Gain
            .Frequency = 125
        End With

        With CurrentEqualizerBand(3)
            .Bandwidth = 2
            .Enabled = True
            .Gain = My.Settings.DSPEqualizer_250_Gain
            .Frequency = 250
        End With

        With CurrentEqualizerBand(4)
            .Bandwidth = 2
            .Enabled = True
            .Gain = My.Settings.DSPEqualizer_500_Gain
            .Frequency = 500
        End With

        With CurrentEqualizerBand(5)
            .Bandwidth = 2
            .Enabled = True
            .Gain = My.Settings.DSPEqualizer_1K_Gain
            .Frequency = 1000
        End With

        With CurrentEqualizerBand(6)
            .Bandwidth = 2
            .Enabled = True
            .Gain = My.Settings.DSPEqualizer_2K_Gain
            .Frequency = 2000
        End With

        With CurrentEqualizerBand(7)
            .Bandwidth = 2
            .Enabled = True
            .Gain = My.Settings.DSPEqualizer_4K_Gain
            .Frequency = 4000
        End With

        With CurrentEqualizerBand(8)
            .Bandwidth = 2
            .Enabled = True
            .Gain = My.Settings.DSPEqualizer_8K_Gain
            .Frequency = 8000
        End With

        With CurrentEqualizerBand(9)
            .Bandwidth = 2
            .Enabled = True
            .Gain = My.Settings.DSPEqualizer_16K_Gain
            .Frequency = 16000
        End With

        ' Load Enabled setting
        bEnabled = My.Settings.DSPEqualizer_Enabled
        CheckBox_Enabled.Checked = bEnabled

        ' Load Gain Values
        TrackBar_Gain.Value = My.Settings.DSPEqualizer_Gain
        nGainValue = CDbl(My.Settings.DSPEqualizer_Gain)

        'Reload Setting in the Track bars
        TrackBar_30.Value = CInt(CurrentEqualizerBand(0).Gain)
        TrackBar_60.Value = CInt(CurrentEqualizerBand(1).Gain)
        TrackBar_125.Value = CInt(CurrentEqualizerBand(2).Gain)
        TrackBar_250.Value = CInt(CurrentEqualizerBand(3).Gain)
        TrackBar_500.Value = CInt(CurrentEqualizerBand(4).Gain)
        TrackBar_1k.Value = CInt(CurrentEqualizerBand(5).Gain)
        TrackBar_2k.Value = CInt(CurrentEqualizerBand(6).Gain)
        TrackBar_4k.Value = CInt(CurrentEqualizerBand(7).Gain)
        TrackBar_8k.Value = CInt(CurrentEqualizerBand(8).Gain)
        TrackBar_16k.Value = CInt(CurrentEqualizerBand(9).Gain)

#If DEBUG Then
        DebugPrintLine("DSPEqualizer", "Init Successful")
#End If

        Return True
    End Function

    Public ReadOnly Property PluginName() As String Implements ISoundEffect.Name
        Get
            Return "IIR Filter (Biquad)"
        End Get
    End Property

    Public Sub ProcessSamples(ByRef left() As Double, ByRef right() As Double) Implements ISoundEffect.ProcessSamples
        If bEnabled = True Then
            For i As Integer = 0 To pEQVal - 1
                pEQ(i).ProcessSamples_Float(left)
                pEQ(i).ProcessSamples_Float(right)
            Next

            ApplyGain(left, right)
        End If
    End Sub

    Private Sub ApplyGain(ByRef left() As Double, ByRef right() As Double)

        If nGainValue <> 0 Then

            ' Gain for left channel
            For i As Integer = 0 To left.Length - 1
                left(i) = left(i) * nGainValue / 20.0#
                Normalize(left(i))
            Next

            ' Gain for right channel
            For i As Integer = 0 To right.Length - 1
                right(i) = right(i) * nGainValue / 20.0#
                Normalize(right(i))
            Next
        End If

    End Sub

    Private Sub Normalize(ByRef value As Double)
        If value > 1.0# Then
            value = 1.0#
        ElseIf value < -1.0# Then
            value = -1.0#
        End If
    End Sub

    Public Sub WaveFormat(ByVal wfx As WaveFormatEx) Implements ISoundEffect.UpdateWaveFormat

        sWaveFormat = wfx
        UpdateEQ()

#If DEBUG Then
        DebugPrintLine("DSPEqualizer", "Change samplerate: " & wfx.Samplerate.ToString)
#End If
    End Sub

    ''' <summary>
    ''' Set o Get the number of IIRFilter class opened
    ''' </summary>
    Private Property EqualizerBand() As Integer
        Get
            Return pEQVal
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then Exit Property

            If value = 0 Then
                pEQ.Clear()

            ElseIf value < pEQVal Then

                pEQ.RemoveRange(value, pEQVal - value)
            ElseIf value > pEQVal Then

                For i As Integer = 0 To value - pEQVal - 1
                    pEQ.Add(New IIRFilter)
                Next

            End If
            pEQVal = value
        End Set
    End Property

    ''' <summary>
    ''' Manipulate an IIRFilter class by Index
    ''' </summary>
    ''' <param name="index">Index of Class to manipulate</param>
    ''' <returns></returns>
    Private ReadOnly Property CurrentEqualizerBand(ByVal index As Integer) As IIRFilter
        Get
            Return pEQ(index)
        End Get
    End Property

    Private Sub UpdateEQ()
        ' Update Samplerate in IIRFilters
        For i As Integer = 0 To pEQVal - 1
            pEQ(i).InputSamplerate = sWaveFormat.Samplerate
        Next
    End Sub

    Private Sub DrawBaizer()
        Dim GradientPen As New Pen(New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, PicVis.Width, PicVis.Height), Color.Red, Color.Lime, Drawing2D.LinearGradientMode.Vertical), 1)
        Dim bit As New Bitmap(PicVis.Width, PicVis.Height)
        Dim grp As Graphics = Graphics.FromImage(bit)

        Dim lstPoint(EqualizerBand - 1) As PointF
        Dim X As Integer = 0
        Dim MidPoint As Integer

        ' Set up graphics
        grp.Clear(Color.Black)
        grp.SmoothingMode = Drawing2D.SmoothingMode.Default

        ' Calculate Middle point
        MidPoint = ((TrackBar_Gain.Maximum - TrackBar_Gain.Minimum) \ 2) + 1

        ' Create points to draw
        For i As Int32 = 0 To lstPoint.Length - 1
            lstPoint(i) = New PointF(X, PicVis.Height \ 2 - CInt(CurrentEqualizerBand(i).Gain * 2))
            X += PicVis.Width \ (lstPoint.Length - 1)
        Next

        ' Draw graph
        grp.DrawLine(Pens.White,
                     0,
                     PicVis.Height \ 2 + (MidPoint - TrackBar_Gain.Value),
                     PicVis.Width,
                     PicVis.Height \ 2 + (MidPoint - TrackBar_Gain.Value))
        grp.DrawBeziers(GradientPen, lstPoint)

        ' View image
        PicVis.Image = bit

        ' Release resources
        grp.Dispose()
        GradientPen.Dispose()
    End Sub

    Private Sub TrackBar30_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar_30.Scroll
        CurrentEqualizerBand(0).Gain = TrackBar_30.Value
        DrawBaizer()
    End Sub

    Private Sub TrackBar60_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar_60.Scroll
        CurrentEqualizerBand(1).Gain = TrackBar_60.Value
        DrawBaizer()
    End Sub

    Private Sub TrackBar125_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar_125.Scroll
        CurrentEqualizerBand(2).Gain = TrackBar_125.Value
        DrawBaizer()
    End Sub

    Private Sub TrackBar250_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar_250.Scroll
        CurrentEqualizerBand(3).Gain = TrackBar_250.Value
        DrawBaizer()
    End Sub

    Private Sub TrackBar500_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar_500.Scroll
        CurrentEqualizerBand(4).Gain = TrackBar_500.Value
        DrawBaizer()
    End Sub

    Private Sub TrackBar1k_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar_1k.Scroll
        CurrentEqualizerBand(5).Gain = TrackBar_1k.Value
        DrawBaizer()
    End Sub

    Private Sub TrackBar2k_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar_2k.Scroll
        CurrentEqualizerBand(6).Gain = TrackBar_2k.Value
        DrawBaizer()
    End Sub

    Private Sub TrackBar4k_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar_4k.Scroll
        CurrentEqualizerBand(7).Gain = TrackBar_4k.Value
        DrawBaizer()
    End Sub

    Private Sub TrackBar8k_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar_8k.Scroll
        CurrentEqualizerBand(8).Gain = TrackBar_8k.Value
        DrawBaizer()
    End Sub

    Private Sub TrackBar16k_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar_16k.Scroll
        CurrentEqualizerBand(9).Gain = TrackBar_16k.Value
        DrawBaizer()
    End Sub

    Private Sub clsEqualizer_frm__FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        ' Prevent form closing
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True
            Me.Visible = False
        End If

        ' Save setting on exit
        My.Settings.DSPEqualizer_30_Gain = TrackBar_30.Value
        My.Settings.DSPEqualizer_60_Gain = TrackBar_60.Value
        My.Settings.DSPEqualizer_125_Gain = TrackBar_125.Value
        My.Settings.DSPEqualizer_250_Gain = TrackBar_250.Value
        My.Settings.DSPEqualizer_500_Gain = TrackBar_500.Value
        My.Settings.DSPEqualizer_1K_Gain = TrackBar_1k.Value
        My.Settings.DSPEqualizer_2K_Gain = TrackBar_2k.Value
        My.Settings.DSPEqualizer_4K_Gain = TrackBar_4k.Value
        My.Settings.DSPEqualizer_8K_Gain = TrackBar_8k.Value
        My.Settings.DSPEqualizer_16K_Gain = TrackBar_16k.Value
        My.Settings.DSPEqualizer_Gain = TrackBar_Gain.Value
        My.Settings.DSPEqualizer_Enabled = bEnabled

    End Sub

    Private Sub clsEqualizer_frm__Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DrawBaizer()
    End Sub

    Private Sub TrackBar_Gain_Scroll(sender As Object, e As EventArgs) Handles TrackBar_Gain.Scroll
        nGainValue = TrackBar_Gain.Value

#If DEBUG Then
        DebugPrintLine("DSPEqualizer", "New Gain value: " & nGainValue.ToString)
#End If

        DrawBaizer()
    End Sub

    Private Sub CheckBox_Enabled_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_Enabled.CheckedChanged
        bEnabled = CheckBox_Enabled.Checked
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click

        ' Reset values
        CurrentEqualizerBand(0).Gain = 0
        CurrentEqualizerBand(1).Gain = 0
        CurrentEqualizerBand(2).Gain = 0
        CurrentEqualizerBand(3).Gain = 0
        CurrentEqualizerBand(4).Gain = 0
        CurrentEqualizerBand(5).Gain = 0
        CurrentEqualizerBand(6).Gain = 0
        CurrentEqualizerBand(7).Gain = 0
        CurrentEqualizerBand(8).Gain = 0
        CurrentEqualizerBand(9).Gain = 0

        bEnabled = False
        TrackBar_Gain.Value = 20

        'Reload Setting
        TrackBar_30.Value = CInt(CurrentEqualizerBand(0).Gain)
        TrackBar_60.Value = CInt(CurrentEqualizerBand(1).Gain)
        TrackBar_125.Value = CInt(CurrentEqualizerBand(2).Gain)
        TrackBar_250.Value = CInt(CurrentEqualizerBand(3).Gain)
        TrackBar_500.Value = CInt(CurrentEqualizerBand(4).Gain)
        TrackBar_1k.Value = CInt(CurrentEqualizerBand(5).Gain)
        TrackBar_2k.Value = CInt(CurrentEqualizerBand(6).Gain)
        TrackBar_4k.Value = CInt(CurrentEqualizerBand(7).Gain)
        TrackBar_8k.Value = CInt(CurrentEqualizerBand(8).Gain)
        TrackBar_16k.Value = CInt(CurrentEqualizerBand(9).Gain)

        DrawBaizer()
    End Sub
End Class

