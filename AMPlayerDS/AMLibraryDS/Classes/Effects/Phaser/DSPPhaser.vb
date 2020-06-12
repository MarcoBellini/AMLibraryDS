Public Class DSPPhaser
    Implements ISoundEffect

    Private clsPhaser As Phaser
    Private nSamplerate As Integer = 44100

    Private ReadOnly Property ISoundEffect_Name As String Implements ISoundEffect.Name
        Get
            Return ".NET Phaser (Based on Foobar plugin)"
        End Get
    End Property

    Public Sub DeInit() Implements ISoundEffect.DeInit
        clsPhaser = Nothing
    End Sub

    Public Sub UpdateWaveFormat(wfx As WaveFormatEx) Implements ISoundEffect.UpdateWaveFormat
        clsPhaser.PhaserInit(wfx.Samplerate)
        nSamplerate = wfx.Samplerate

#If DEBUG Then
        DebugPrintLine("DSPPhaser", "Change samplerate: " & nSamplerate.ToString)
#End If
    End Sub

    Public Sub ConfigurationDialog() Implements ISoundEffect.ConfigurationDialog
        Me.Show()
    End Sub

    Public Sub ProcessSamples(ByRef left() As Double, ByRef right() As Double) Implements ISoundEffect.ProcessSamples
        ' Verify if Checkbox is Checked
        If bEnableCkeckBox.Checked = True Then
            For i As Integer = 0 To left.Length - 1
                left(i) = clsPhaser.PhaserProcess(left(i))
                right(i) = clsPhaser.PhaserProcess(right(i))
            Next
        End If
    End Sub
    Public Function Init() As Boolean Implements ISoundEffect.Init
        clsPhaser = New Phaser()

#If DEBUG Then
        DebugPrintLine("Phaser", "Init Successful")
#End If
    End Function

    Private Sub DSPPhaser_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        ' Prevent form closing
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True
            Me.Visible = False
        End If
    End Sub

    Private Sub Button_close_Click(sender As Object, e As EventArgs) Handles Button_close.Click
        Me.Visible = False
    End Sub

    Private Sub TrackBar_Stages_Scroll(sender As Object, e As EventArgs) Handles TrackBar_Stages.Scroll
        clsPhaser.Stages = TrackBar_Stages.Value
        Label_Stages.Text = "Stages: " & TrackBar_Stages.Value.ToString
    End Sub

    Private Sub TrackBar_DryWet_Scroll(sender As Object, e As EventArgs) Handles TrackBar_DryWet.Scroll
        clsPhaser.DryWet = TrackBar_DryWet.Value
        Label_DryWet.Text = "Dry/Wet: " & TrackBar_DryWet.Value.ToString
    End Sub

    Private Sub TrackBar_LFOFrequency_Scroll(sender As Object, e As EventArgs) Handles TrackBar_LFOFrequency.Scroll
        clsPhaser.LFOFrequency = TrackBar_LFOFrequency.Value
        Label_LFOFrequency.Text = "LFO Frequency: : " & TrackBar_LFOFrequency.Value.ToString & " Hz"
        clsPhaser.PhaserInit(nSamplerate)
    End Sub

    Private Sub TrackBar_LFOStartPhase_Scroll(sender As Object, e As EventArgs) Handles TrackBar_LFOStartPhase.Scroll
        clsPhaser.LFOStartPhase = TrackBar_LFOStartPhase.Value
        Label_LFOStartPhase.Text = "LFO Start Phase: " & TrackBar_LFOStartPhase.Value.ToString & " (.deg)"
        clsPhaser.PhaserInit(nSamplerate)
    End Sub

    Private Sub TrackBar_Depth_Scroll(sender As Object, e As EventArgs) Handles TrackBar_Depth.Scroll
        clsPhaser.Depth = TrackBar_Depth.Value
        Label_Depth.Text = "Depth: " & TrackBar_Depth.Value.ToString
    End Sub

    Private Sub TrackBar_Feedback_Scroll(sender As Object, e As EventArgs) Handles TrackBar_Feedback.Scroll
        clsPhaser.Feedback = TrackBar_Feedback.Value
        Label_Feedback.Text = "Feedback: " & TrackBar_Feedback.Value.ToString & "%"
    End Sub
End Class