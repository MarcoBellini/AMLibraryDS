Public Class frmOpenCD

    Public chrSelectedDrive As Char = Nothing

    Private Sub CloseButton_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
        DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub OpenButton_Click(sender As Object, e As EventArgs) Handles OpenButton.Click
        DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub frmOpenCD_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim cdrom As New CdDrive
        Dim drives() As Char

        ' fill check box with drivers list
        drives = cdrom.GetCDDriveLetters()

        For i As Integer = 0 To drives.Length - 1
            lstDrivers.Items.Add(drives(i))
        Next
    End Sub

    Private Sub lstDrivers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstDrivers.SelectedIndexChanged
        chrSelectedDrive = lstDrivers.SelectedItem
    End Sub
End Class