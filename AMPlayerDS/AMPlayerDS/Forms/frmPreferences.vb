Public Class frmPreferences

    Private Sub LoadSettings()

        ' Add Enum values and names
        For Each ConvValue As Integer In [Enum].GetValues(GetType(LibSamplerate.ConverterType))
            EncoderQualityListView.Items.Add(New ListViewItem(
                                             {[Enum].GetName(GetType(LibSamplerate.ConverterType), ConvValue).ToString,
                                             ConvValue}))
        Next

        ' Select Saved Encoder quality
        EncoderQualityListView.Items.Item(My.Settings.LibSamplerate_EncoderQuality).Selected = True

        ' Add output values
        For Each OutputValue As String In [Enum].GetNames(GetType(DecoderManager.OutputPlugins))
            OutputComboBox.Items.Add(OutputValue)
        Next

        OutputComboBox.SelectedIndex = My.Settings.AMPlayer_OutputPlugin
    End Sub

    Private Sub SaveSettings()

        ' Update
        If EncoderQualityListView.SelectedItems.Count > 0 Then
#If DEBUG Then
            DebugPrintLine("Form Settings", EncoderQualityListView.SelectedItems.Item(0).SubItems.Item(1).Text)

#End If
            My.Settings.LibSamplerate_EncoderQuality = Integer.Parse(EncoderQualityListView.SelectedItems.Item(0).SubItems.Item(1).Text)
        End If

        ' Save output setting
        My.Settings.AMPlayer_OutputPlugin = OutputComboBox.SelectedIndex

    End Sub

    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButtonName.Click
        Close()
    End Sub

    Private Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        SaveSettings()
        Close()
    End Sub

    Private Sub frmPreferences_Load(sender As Object, e As EventArgs) Handles Me.Load
        LoadSettings()
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged

        ' Maintain focus on ListView to highlight the selected index
        If TabControl1.SelectedTab Is TabPage2 Then
            EncoderQualityListView.Select()
        End If

    End Sub

End Class