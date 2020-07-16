Public Class frmAbout
    Private Sub frmAbout_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PlatformLabel.Text = Commons.CompiledPlatform
        VersionLabel.Text = My.Application.Info.Version.ToString
        OSBitsLabel.Text = IIf(Environment.Is64BitOperatingSystem, "64 bits", "32 bits")
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Close()
    End Sub
End Class