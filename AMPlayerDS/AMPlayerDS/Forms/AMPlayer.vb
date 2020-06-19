' Winamp style trackbar source code:
' https://www.codeproject.com/Articles/997101/Custom-Winamp-Style-TrackBar-Slider
Public Class AMPlayer

    ' TODO: Update code (use Open file function helper)
    ' Order functions
    ' Add About form
    ' Add Rip CD form

    Private Const INVALID_INDEX As Integer = -1

    Private WithEvents Decoder As DecoderManager
    Private clsVis As clsVisualization

    Public PlaylistList As New List(Of StreamInformations)

    Private nCurrentPlayIndex As Integer = INVALID_INDEX
    Private dblSamples() As Double

    Private Delegate Sub UpdateUIDelegate(ByRef Info As StreamInformations)
    Private Delegate Sub UpdatePlaylistEntryDelegate(ByRef info As StreamInformations, ByRef Index As Integer)

    Private UpdateUI As New UpdateUIDelegate(AddressOf UpdateUIProc)
    Private UpdatePlaylistEntry As New UpdatePlaylistEntryDelegate(AddressOf UpdatePlaylistEntryProc)

#Region "Form Events"
    Private Sub AMPlayer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Create decoder
        Decoder = New DecoderManager
        Decoder.Init(Me.Handle, DecoderManager.OutputPlugins.DirectSound)

        ' Reload output values
        VolumePanControl1.Volume = Decoder.Volume
        VolumePanControl1.Pan = Decoder.Pan

        ' Create playlist
        Playlist.AddColumn("Artist / Title", 405)
        Playlist.AddColumn("Duration", 100)

        ' Add effects to menù
        AddEffectsToMenu()

        ' Create Visualizations
        clsVis = New clsVisualization(Me.PicVisualization)
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        ' Free resources
        Decoder.Dispose()
        Decoder = Nothing

        clsVis.Dispose()
        clsVis = Nothing
    End Sub

    Private Sub VisualizationTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VisualizationTimer.Tick
        ' Use less CPU when minimized
        If Me.WindowState = FormWindowState.Minimized Then Exit Sub

        dblSamples = Decoder.GetFrequenciesMagnitude(FastFourierTrasform.SampleSize.FFT_Size_512)

        ' Draw spectrum
        clsVis.DrawAmplitudes(dblSamples)
        dblSamples = Nothing
    End Sub

    Private Sub TrackbarTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackbarTimer.Tick
        UpdatePositionAndTimeLabel(Decoder.Position)
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Close()
    End Sub

    Private Sub VolumeControlButton_Click(sender As Object, e As MouseEventArgs) Handles VolumeControlButton.MouseDown

        ' Show volume and pan control
        If VolumePanControl1.Visible = False Then
            Dim MousePosition As New Point

            ' Open near to mouse click
            MousePosition.X = e.Location.X
            MousePosition.Y = e.Location.Y + 50

            VolumePanControl1.ShowControl(MousePosition)

        Else
            VolumePanControl1.Visible = False
        End If

    End Sub

    Private Sub OpenCDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenCDToolStripMenuItem.Click
        Dim frmCd As New frmOpenCD()

        ' Check if the user decided to open drive
        If frmCd.ShowDialog() = DialogResult.OK Then
            AddCdToPlaylist(frmCd.chrSelectedDrive)
        End If

        frmCd.Dispose()
    End Sub

    Private Sub AddFilesToPlaylistToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddFilesToPlaylistToolStripMenuItem.Click
        AddMultipleFileToPlaylist()
    End Sub

    Private Sub VolumePanControl1_VolumeChanged(value As Integer) Handles VolumePanControl1.VolumeChanged
        ' Update pan value
        Decoder.Volume = value
    End Sub

    Private Sub VolumePanControl1_PanChanged(value As Integer) Handles VolumePanControl1.PanChanged
        'Update volume value
        Decoder.Pan = value
    End Sub

    Private Sub VolumePanControl1_OutputDialog() Handles VolumePanControl1.OutputDialog
        If Decoder Is Nothing Then Exit Sub

        ' Show output configuration dialog
        Decoder.CurrentOutput.ConfigDialog()
    End Sub

    Private Sub Playlist_ItemDoubleClick(index As Integer) Handles Playlist.ItemDoubleClick

        If Decoder.OpenFile(PlaylistList(index).FileLocation) = True Then
            PlayHelper()
            nCurrentPlayIndex = index

            ' Thread Safe Invocation
            Invoke(UpdatePlaylistEntry, Decoder.CurrentFileStreamInfo, index)
        End If
    End Sub

    Private Sub Playlist_ItemKeyDown(index As Integer, key As KeyEventArgs) Handles Playlist.ItemKeyDown
        If key.KeyCode = Keys.Enter Then
            If Decoder.OpenFile(PlaylistList(index).FileLocation) = True Then
                PlayHelper()
                nCurrentPlayIndex = index


                ' Thread Safe Invocation
                Invoke(UpdatePlaylistEntry, Decoder.CurrentFileStreamInfo, index)
            End If
        End If
    End Sub

    Private Sub PlayStripButton_Click(sender As Object, e As EventArgs) Handles PlayStripButton.Click
        PlayHelper()
    End Sub

    Private Sub PauseStripButton_Click(sender As Object, e As EventArgs) Handles PauseStripButton.Click
        PauseHelper()
    End Sub
    Private Sub StopStripButton_Click(sender As Object, e As EventArgs) Handles StopStripButton.Click
        StopHelper()
    End Sub

    Private Sub PlayToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlayToolStripMenuItem.Click
        PlayHelper()
    End Sub

    Private Sub PauseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PauseToolStripMenuItem.Click
        PauseHelper()
    End Sub

    Private Sub StopToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StopToolStripMenuItem.Click
        StopHelper()
    End Sub

    Private Sub ClearAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearAllToolStripMenuItem.Click
        DeleteSelectedItems()
    End Sub

    Private Sub ClearSelectedItemsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearSelectedItemsToolStripMenuItem.Click
        DeleteAllItems()
    End Sub

    Private Sub EffectsToolStrip_Click(sender As Object, e As EventArgs)
        Dim ToolStripEffect As ToolStripMenuItem = TryCast(sender, ToolStripMenuItem)
        Dim strNumber As String
        Dim Index As Integer

        ' Remove the word "Effect"
        strNumber = ToolStripEffect.Name.Remove(0, 6)

        ' Convert to index
        If Integer.TryParse(strNumber, Index) = True Then
            If Index < Decoder.EffectsCount Then
                Decoder.Effects(Index).ConfigurationDialog()
            End If
        End If

    End Sub

    Private Sub EqualizerToolStrip_Click(sender As Object, e As EventArgs) Handles EqualizerToolStrip.Click
        If Decoder Is Nothing Then Exit Sub

        ' Remeber the first effect is equalizer (index = 0)
        Decoder.Effects(0).ConfigurationDialog()
    End Sub

    Private Sub ForwardStripButton_Click(sender As Object, e As EventArgs) Handles ForwardStripButton.Click
        NextItemHelper()
    End Sub

    Private Sub RewindStripButton_Click(sender As Object, e As EventArgs) Handles RewindStripButton.Click
        PreviousItemHelper()
    End Sub

    Private Sub NextToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NextToolStripMenuItem.Click
        NextItemHelper()
    End Sub

    Private Sub PreviouToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PreviouToolStripMenuItem.Click
        PreviousItemHelper()
    End Sub

    Private Sub ViewFileTagsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewFileTagsToolStripMenuItem.Click
        OpenTagsDialog()
    End Sub

    Private Sub TranscodeFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TranscodeFilesToolStripMenuItem.Click
        OpenTranscodeDialog()
    End Sub

    Private Sub AddFolderToPlaylistToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddFolderToPlaylistToolStripMenuItem.Click
        AddFolderToPlaylist()
    End Sub

    Private Sub OpenFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenFileToolStripMenuItem.Click
        OpenSingleFile()
    End Sub

    Private Sub GetFullDetailsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetFullDetailsToolStripMenuItem.Click
        If Decoder IsNot Nothing Then

            ' If status is playing, ask to user if stop playback
            ' to get full details
            If Decoder.Status <> Status.STOPPED Then
                Dim result As DialogResult

                result =
                MessageBox.Show("To get full details, playback must stop. Continue?",
                                Name,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button1)

                ' If no, exit from function
                If result = DialogResult.Yes Then
                    GetFullDetailsOnPlaylist()
                End If
            Else
                GetFullDetailsOnPlaylist()
            End If
        End If
    End Sub

    Private Sub PositionTrackbar_SeekDone(sender As Object, e As Winamp.Components.WinampTrackBarSeekEventArgs) Handles PositionTrackbar.SeekDone
        If Decoder IsNot Nothing Then
            If Decoder.Status = Status.PLAYING Then
                Decoder.Position = e.Value
            End If
        End If
    End Sub

    Private Sub PositionTrackbar_ValueChanged(sender As Object, e As Winamp.Components.WinampTrackBarValueChangedEventArgs) Handles PositionTrackbar.ValueChanged
        Select Case e.ChangeSource
            Case Winamp.Components.WinampTrackBar.WinampTrackBarValueChangeSource.TrackClick
                If Decoder IsNot Nothing Then
                    If Decoder.Status = Status.PLAYING Then
                        Decoder.Position = e.Value
                    End If
                End If
        End Select
    End Sub


#End Region

#Region "Decoder Events"

    Private Sub Decoder_End_Of_Stream() Handles Decoder.End_Of_Stream
        NextItemHelper()
    End Sub

    Private Sub Decoder_File_Opened(ByVal info As StreamInformations) Handles Decoder.File_Opened
        Invoke(UpdateUI, info)
    End Sub

    Private Sub Decoder_Status_Charged(ByVal status As Status) Handles Decoder.Status_Charged
        Select Case status
            Case Status.PLAYING
                VisualizationTimer.Enabled = True
                TrackbarTimer.Enabled = True
            Case Status.STOPPED
                VisualizationTimer.Enabled = True
                TrackbarTimer.Enabled = False

                Invoke(UpdateUI, Decoder.CurrentFileStreamInfo)

            Case Status.PAUSING
                VisualizationTimer.Enabled = False
                TrackbarTimer.Enabled = False
        End Select
    End Sub

#End Region

#Region "Form Functions"

    Private Sub DeleteSelectedItems()
        ' Remove all items
        PlaylistList.Clear()
        Playlist.RemoveAllRows()

        ' Draw playlist and update scrollbars
        Playlist.UpdateRowGraphics()
        Playlist.UpdateScrollbars()

        ' Reset play index
        nCurrentPlayIndex = INVALID_INDEX
    End Sub

    Private Sub DeleteAllItems()
        Dim selectedItems As List(Of Integer)

        ' Get selected items
        selectedItems = Playlist.SelectedItems()

        ' If no elements are selected exit
        If selectedItems.Count = 0 Then Exit Sub

        For i As Integer = 0 To selectedItems.Count - 1
            Playlist.RemoveRow(selectedItems(i))
            PlaylistList.RemoveAt(selectedItems(i))
        Next

        ' Draw playlist and update scrollbars
        Playlist.UpdateRowGraphics()
        Playlist.UpdateScrollbars()

        ' Update Play Index
        If selectedItems.Count <> 0 Then
            nCurrentPlayIndex = 0
        Else
            nCurrentPlayIndex = INVALID_INDEX
        End If
    End Sub
    Private Sub UpdateUIProc(ByRef Info As StreamInformations)
        ' Update trackbar max value
        If PositionTrackbar.Maximum <> Decoder.Duration Then
            PositionTrackbar.Maximum = Decoder.Duration
        End If

        If Info IsNot Nothing Then
            Me.Text = "AMPlayer - " & Info.FileName

            StatusLabel.Text = Info.Samplerate & "Hz - " &
                                   Info.Channels & " channels - " &
                                   Info.BitsPerSample & " bits"
        End If

        If Decoder.Status = Status.STOPPED Then
            UpdatePositionAndTimeLabel(0)
        End If


    End Sub

    Private Sub UpdatePositionAndTimeLabel(ByVal Value As Long)
        If Value < 0 Then Exit Sub

        PositionTrackbar.Value = Value

        With TimeSpan.FromSeconds(Value)
            TimeStripLabel.Text = Format(.TotalHours, "0") &
                                  ":" & Fix(.TotalMinutes).ToString &
                                  ":" & Format(.Seconds, "00")
        End With
    End Sub

    ' Safe open file dialog using STA thread
    Public Sub AddMultipleFileToPlaylist()
        Dim SafeDialog As New SafeFileDialog
        Dim Results() As String

        Results = SafeDialog.OpenMultipleFiles("Supported Files|" & Decoder.GetSupportedExtension)

        If Results IsNot Nothing Then

            For i As Integer = 0 To Results.Length - 1
                AddFileToPlaylist(Results(i))
            Next

            ' Update playlist
            UpdatePlaylist()
        End If
    End Sub

    Private Sub AddFolderToPlaylist()
        Dim SafeDialog As New SafeFileDialog
        Dim Result As String


        ' Open folder selection dialog (STA thread)
        Result = SafeDialog.SelectFolder()

        ' Check if result is valid
        If (Result <> "") And (Decoder IsNot Nothing) Then
            Dim Files() As String
            Dim SupportedExtensions As String = Decoder.GetSupportedExtension

            ' Get all files in the selected directory
            Files = IO.Directory.GetFiles(Result, "*.*")

            ' Filter for supported extensions
            For i As Integer = 0 To Files.Length - 1
                If SupportedExtensions.Contains(IO.Path.GetExtension(Files(i).ToLower)) Then
                    AddFileToPlaylist(Files(i))
                End If
            Next

            UpdatePlaylist()
        End If
    End Sub

    Private Sub UpdatePlaylist()
        ' Update playlist
        Playlist.UpdateRowGraphics()
        Playlist.UpdateScrollbars()
    End Sub

    Private Sub AddFileToPlaylist(ByVal FilePath As String)
        ' Add file to list of Playlist
        Dim Info As StreamInformations = New StreamInformations

        Info.FillBasicFileInfo(FilePath)
        Info.DurationInMs = 0

        PlaylistList.Add(Info)

        ' Add files to Playlist
        Playlist.AddRow(IO.Path.GetFileNameWithoutExtension(FilePath),
                        "0:00")

    End Sub

    Private Sub AddCdToPlaylist(ByVal drive As Char)
        Dim cdrom As New CDDrive
        Dim nDuration As Long
        Dim strDuration As String
        Dim Info As StreamInformations

        ' Open cd
        If cdrom.Open(drive) = True Then
            If cdrom.Refresh() = True Then
                For i As Integer = 1 To cdrom.GetNumAudioTracks

                    ' Add in the list in a format C: - 1.cda
                    Info = New StreamInformations
                    Info.FileLocation = drive & "-" & i.ToString & ".cda"

                    ' Calculate duration of each track( 44100kz at 16bits 2 channels)
                    nDuration = cdrom.GetTrackLength(i) * 2352 \ 176400
                    Info.DurationInMs = nDuration * 1000

                    With TimeSpan.FromSeconds(nDuration)
                        strDuration = Fix(.Minutes) & ":" & Format(.Seconds, "00")
                    End With

                    PlaylistList.Add(Info)
                    Playlist.AddRow("CD Track: " & i.ToString, strDuration)
                Next
            End If

            ' Free resources
            cdrom.Close()
            cdrom = Nothing

            ' Update playlist
            UpdatePlaylist()
        End If
    End Sub

    Public Sub UpdatePlaylistEntryProc(ByRef info As StreamInformations, ByRef Index As Integer)
        Dim StrDuration As String

        ' Check if we have a valid idex
        If Index = INVALID_INDEX Then Exit Sub

        ' Update current item
        PlaylistList(Index) = info

        ' Calculate informations
        With TimeSpan.FromMilliseconds(info.DurationInMs)
            If .Hours <> 0 Then
                StrDuration = Fix(.Hours) & ":" & Fix(.Minutes) & ":" & Format(.Seconds, "00")
            Else
                StrDuration = Fix(.Minutes) & ":" & Format(.Seconds, "00")
            End If

        End With

        ' Update playlist
        If info.Artist <> "" And info.Title <> "" Then
            Playlist.EditRow(Index, info.Artist & " - " & info.Title, StrDuration)
        Else
            Playlist.EditRow(Index, info.FileName, StrDuration)
        End If

        ' Highlight playing Index
        UpdatePlaylistHighlight()
    End Sub

    Private Sub PlayHelper()
        If Decoder Is Nothing Then Exit Sub

        If Decoder.Status <> Status.PLAYING Then
            Decoder.Status = Status.PLAYING
        End If
    End Sub

    Private Sub PauseHelper()
        If Decoder Is Nothing Then Exit Sub

        If Decoder.Status <> Status.PAUSING Then
            Decoder.Status = Status.PAUSING
        End If
    End Sub

    Private Sub StopHelper()
        If Decoder Is Nothing Then Exit Sub

        If Decoder.Status <> Status.STOPPED Then
            Decoder.Status = Status.STOPPED
        End If
    End Sub

    Private Sub NextItemHelper()

        If nCurrentPlayIndex <> INVALID_INDEX Then
            If nCurrentPlayIndex < PlaylistList.Count - 1 Then

                nCurrentPlayIndex += 1

                If Decoder.OpenFile(PlaylistList(nCurrentPlayIndex).FileLocation) = True Then
                    PlayHelper()
                End If
            Else
                StopHelper()
                nCurrentPlayIndex = INVALID_INDEX
                VisualizationTimer.Enabled = True
                TrackbarTimer.Enabled = False
            End If

            ' Thread Safe Invocation
            Invoke(UpdatePlaylistEntry, Decoder.CurrentFileStreamInfo, nCurrentPlayIndex)
            Invoke(UpdateUI, Decoder.CurrentFileStreamInfo)
        End If
    End Sub

    Private Sub PreviousItemHelper()

        If nCurrentPlayIndex <> INVALID_INDEX Then
            If nCurrentPlayIndex > 0 Then
                nCurrentPlayIndex -= 1

                If Decoder.OpenFile(PlaylistList(nCurrentPlayIndex).FileLocation) = True Then
                    PlayHelper()
                End If

            Else
                StopHelper()
                nCurrentPlayIndex = INVALID_INDEX
                VisualizationTimer.Enabled = True
                TrackbarTimer.Enabled = False
            End If

            ' Thread Safe Invocation
            Invoke(UpdatePlaylistEntry, Decoder.CurrentFileStreamInfo, nCurrentPlayIndex)
            Invoke(UpdateUI, Decoder.CurrentFileStreamInfo)
        End If

    End Sub

    Private Sub AddEffectsToMenu()
        If Decoder IsNot Nothing Then
            For i As Integer = 0 To Decoder.EffectsCount - 1
                Dim ToolStripEffect As New ToolStripMenuItem

                ' Name of the effect
                ToolStripEffect.Text = Decoder.Effects(i).Name

                ' Add the word "Effect" and i value
                ToolStripEffect.Name = "Effect" & i.ToString

                ' Add to Menù and add event handler
                EffectsToolStripMenuItem.DropDownItems.Add(ToolStripEffect)
                AddHandler ToolStripEffect.Click, AddressOf EffectsToolStrip_Click
            Next

        End If
    End Sub

    Private Sub OpenTagsDialog()
        Dim Info As StreamInformations
        Dim TagsDialog As New frmTagDialog

        If Decoder Is Nothing Then Exit Sub

        ' Try to read current stream info
        Info = Decoder.CurrentFileStreamInfo

        If Info IsNot Nothing Then
            Dim InfoList As List(Of String)

            ' Convert stream info to a list
            InfoList = Info.Informations2List()

            ' Clear list
            TagsDialog.ListBoxTags.Items.Clear()

            For i As Integer = 0 To InfoList.Count - 1
                TagsDialog.ListBoxTags.Items.Add(InfoList(i))
            Next

            TagsDialog.ShowDialog()
        End If
    End Sub

    Private Sub OpenTranscodeDialog()
        Dim frmTransc As New frmTranscode

        If Decoder IsNot Nothing Then
            If Decoder.Status <> Status.STOPPED Then
                StopHelper()
            End If
        End If

        frmTransc.ShowDialog()
        frmTransc.Dispose()
    End Sub

    Public Sub OpenSingleFile()
        Dim SafeDialog As New SafeFileDialog
        Dim Result As String

        ' Open single file and try to play
        Result = SafeDialog.OpenSingleFile("Supported Files |" & Decoder.GetSupportedExtension)

        If Result IsNot Nothing Then
            ' Close current streaming
            If Decoder.Status <> Status.STOPPED Then
                Decoder.Status = Status.STOPPED
                Decoder.Close()
            End If

            ' Open and Play
            If Decoder.OpenFile(Result) = True Then
                PlayHelper()

                ' Update playlist
                nCurrentPlayIndex = INVALID_INDEX
                UpdatePlaylistHighlight()
            End If

        End If
    End Sub

    Private Sub GetFullDetailsOnPlaylist()
        Dim info As StreamInformations
        Dim ArtistTitle As String
        Dim strDuration As String

        For i As Integer = 0 To PlaylistList.Count - 1
            info = Decoder.FastStreamInformation(PlaylistList(i).FileLocation)

            ' Update current item
            PlaylistList(i) = info

            ' Check if informations are valid
            If info IsNot Nothing Then

                ' Convert to duration in H:M:S
                With TimeSpan.FromMilliseconds(info.DurationInMs)
                    If .Hours <> 0 Then
                        strDuration = Fix(.Hours) & ":" & Fix(.Minutes) & ":" & Format(.Seconds, "00")
                    Else
                        strDuration = Fix(.Minutes) & ":" & Format(.Seconds, "00")
                    End If

                End With

                ' Fill with TAG if avaiable
                If (info.Title <> "") And (info.Artist <> "") Then
                    ArtistTitle = info.Artist & " - " & info.Title
                    Playlist.EditRow(i, ArtistTitle, strDuration)
                Else
                    Playlist.EditRow(i, info.FileName, strDuration)
                End If
            End If
        Next

        ' Redraw playlist
        UpdatePlaylist()
    End Sub

    Private Sub UpdatePlaylistHighlight()
        If nCurrentPlayIndex <> INVALID_INDEX Then
            Playlist.ClearHighlight()
            Playlist.HighlightItem(nCurrentPlayIndex)
            Playlist.UpdateRowGraphics()
        Else
            Playlist.ClearHighlight()
            Playlist.UpdateRowGraphics()
        End If
    End Sub

    Private Sub OpenPlaylistFile()
        Dim OpenDialog As New SafeFileDialog
        Dim PlsManager As New PlaylistManager

        Dim FilePath As String
        Dim Info As StreamInformations

        Dim ArtistTitle As String
        Dim strDuration As String

        FilePath = OpenDialog.OpenSingleFile("Supported Playlist|" & PlsManager.GetPlaylistReaderExtensions)

        ' If no file are selected, exit
        If FilePath Is Nothing Then Exit Sub

        If PlsManager.Open(FilePath, PlaylistManager.PlaylistMode.Read) = True Then

            For i As Integer = 0 To PlsManager.ReadCount - 1


                ' Read Playlist informations
                Info = PlsManager.ReadIndex(i)

                ' Add file to list of Playlist
                PlaylistList.Add(Info)

                ' Check if duration is valid, oterwise use file size
                If Info.DurationInMs <> 0 Then
                    With TimeSpan.FromMilliseconds(Info.DurationInMs)
                        If .Hours <> 0 Then
                            strDuration = Fix(.Hours) & ":" & Fix(.Minutes) & ":" & Format(.Seconds, "00")
                        Else
                            strDuration = Fix(.Minutes) & ":" & Format(.Seconds, "00")
                        End If

                    End With
                Else
                    strDuration = "0:00"
                End If

                ' Fill with TAG if avaiable otherwise use file name
                If (Info.Title <> "") And (Info.Artist <> "") Then
                    ArtistTitle = Info.Artist & " - " & Info.Title
                Else
                    ArtistTitle = Info.FileName
                End If


                ' Add file to Playlist
                Playlist.AddRow(ArtistTitle,
                                strDuration)
            Next

            PlsManager.Close()
            PlsManager.Dispose()

            ' Redraw Playlist
            UpdatePlaylist()
        End If

    End Sub

#End Region
End Class
