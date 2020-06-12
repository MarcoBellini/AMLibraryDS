Imports System.ComponentModel
Imports System.Windows.Forms

Public Class DirectSoundConfiguration

    ' Filled in DirectSound Output Class
    Public LstDevice As List(Of DirectSoundNative.DirectSoundDeviceInfo)

    Private Sub Button_close_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_close.Click
        'Close Dialog
        Me.Close()
    End Sub

    Private Sub BufferTrackBar_Scroll(sender As Object, e As EventArgs) Handles BufferTrackBar.Scroll
        ' Update label
        BufferSizeLabel.Text = BufferTrackBar.Value.ToString
    End Sub

    Private Sub PreBufferTrackBar_Scroll(sender As Object, e As EventArgs) Handles PreBufferTrackBar.Scroll
        ' Update Label
        PreBufferSizeLabel.Text = PreBufferTrackBar.Value.ToString
    End Sub

    Private Sub DirectSoundConfiguration_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ' Save new settings (Write in My.Setting)
        My.Settings.DirectSound_BufferLen = BufferTrackBar.Value
        My.Settings.DirectSound_PreBufferLen = PreBufferTrackBar.Value
        My.Settings.DirectSound_EnableFading = CheckBoxFading.Checked
        My.Settings.DirectSound_DeviceGuid = LstDevice(DeviceListControl.SelectedIndex).DeviceGuid

        ' Force save setting
        My.Settings.Save()
    End Sub

    Private Sub DirectSoundConfiguration_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim IndexToSelect As Integer = 0

        ' Buffer len setting
        BufferTrackBar.Value = My.Settings.DirectSound_BufferLen
        BufferSizeLabel.Text = BufferTrackBar.Value.ToString

        ' Pre bufferlen setting
        PreBufferTrackBar.Value = My.Settings.DirectSound_PreBufferLen
        PreBufferSizeLabel.Text = PreBufferTrackBar.Value.ToString

        ' Check if fading is enabled
        CheckBoxFading.Checked = My.Settings.DirectSound_EnableFading

        ' Load device found in Directsound Output class in the combobox
        For i As Integer = 0 To LstDevice.Count - 1
            ' Fill combobox
            DeviceListControl.Items.Add(LstDevice(i).DeviceName)

            If LstDevice(i).DeviceGuid = My.Settings.DirectSound_DeviceGuid Then
                IndexToSelect = i
            End If
        Next

        ' Select Used device
        DeviceListControl.SelectedIndex = IndexToSelect
    End Sub
End Class
