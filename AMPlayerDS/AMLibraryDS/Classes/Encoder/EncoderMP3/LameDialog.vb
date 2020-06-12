Imports System.Runtime.InteropServices
Public Class LameDialog
    Private Sub ButtonOK_Click(sender As Object, e As EventArgs) Handles ButtonOK.Click
        SaveSettings()
        Close()
    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        Close()
    End Sub

    Private Sub LameDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LameVersionLabel.Text = "Lame Encoder ver: " & Marshal.PtrToStringAnsi(Lame.get_lame_version)
        LoadSettings()
    End Sub

    Private Sub LoadSettings()

        ' Load target. 0 = Bitrate 1 = Quality
        ' NB. BitrateRadioButton or QualityRadioButton checked
        ' eved are raised (see below)
        If My.Settings.LAME_Target = 0 Then
            BitrateRadioButton.Checked = True
        Else
            QualityRadioButton.Checked = True
        End If

        EncoderQualityCombobox.SelectedIndex = My.Settings.LAME_EncoderQualityIndex
        VBRQualityCombobox.SelectedIndex = My.Settings.LAME_VBRModeIndex
        ForceCBR.Checked = My.Settings.LAME_ForceCBR
        BitrateTrackbar.Value = My.Settings.LAME_Bitrate
        QualityTrackbar.Value = My.Settings.LAME_VBRQuality
    End Sub

    Private Sub SaveSettings()
        My.Settings.LAME_Target = IIf(BitrateRadioButton.Checked, 0, 1)
        My.Settings.LAME_EncoderQualityIndex = EncoderQualityCombobox.SelectedIndex
        My.Settings.LAME_VBRModeIndex = VBRQualityCombobox.SelectedIndex
        My.Settings.LAME_ForceCBR = ForceCBR.Checked
        My.Settings.LAME_Bitrate = BitrateTrackbar.Value
        My.Settings.LAME_VBRQuality = QualityTrackbar.Value
        My.Settings.Save()
    End Sub

    Private Sub BitrateRadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles BitrateRadioButton.CheckedChanged
        BitrateGroupbox.Enabled = True
        QualityGroupBox.Enabled = False
    End Sub

    Private Sub QualityRadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles QualityRadioButton.CheckedChanged
        BitrateGroupbox.Enabled = False
        QualityGroupBox.Enabled = True
    End Sub

    Private Sub BitrateTrackbar_ValueChanged(sender As Object, e As EventArgs) Handles BitrateTrackbar.ValueChanged
        BitrateLabel.Text = BitrateTrackbar.Value.ToString & " kbps"
    End Sub

    Private Sub QualityTrackbar_ValueChanged(sender As Object, e As EventArgs) Handles QualityTrackbar.ValueChanged
        QualityLabel.Text = QualityTrackbar.Value.ToString & " %"
    End Sub
End Class