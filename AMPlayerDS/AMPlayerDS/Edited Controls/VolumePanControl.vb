Imports System.Runtime.InteropServices
Public Class VolumePanControl

    ' Events
    Public Event VolumeChanged(ByVal value As Integer)
    Public Event PanChanged(ByVal value As Integer)
    Public Event OutputDialog()

    ''' <summary>
    ''' Show control on Mouse Location
    ''' </summary>
    Public Sub ShowControl(ByVal MousePosition As Point)
        Me.Location = MousePosition
        Me.Visible = True
    End Sub

    ''' <summary>
    ''' Get or Set Volume Trackbar value
    ''' </summary>
    Public Property Volume() As Integer
        Get
            Return TrackBarVolume.Value
        End Get
        Set(value As Integer)
            TrackBarVolume.Value = value
        End Set
    End Property

    ''' <summary>
    ''' Get or Set Pan Trackbar value
    ''' </summary>
    Public Property Pan() As Integer
        Get
            Return TrackBarPan.Value
        End Get
        Set(value As Integer)
            TrackBarPan.Value = value
        End Set
    End Property
    Private Sub CloseButton_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
        Me.Visible = False
    End Sub
    Private Sub TrackBarVolume_Scroll(sender As Object, e As EventArgs) Handles TrackBarVolume.Scroll
        RaiseEvent VolumeChanged(TrackBarVolume.Value)
    End Sub

    Private Sub TrackBarPan_Scroll(sender As Object, e As EventArgs) Handles TrackBarPan.Scroll
        RaiseEvent PanChanged(TrackBarPan.Value)
    End Sub

    Private Sub OutputSettingsButton_Click(sender As Object, e As EventArgs) Handles OutputSettingsButton.Click
        RaiseEvent OutputDialog()
    End Sub
End Class
