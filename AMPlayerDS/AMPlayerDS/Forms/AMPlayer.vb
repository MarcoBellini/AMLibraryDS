' Winamp style trackbar source code:
' https://www.codeproject.com/Articles/997101/Custom-Winamp-Style-TrackBar-Slider
Public Class AMPlayer
    Private Const INVALID_INDEX As Integer = -1

    Private WithEvents Decoder As DecoderManager
    Private WithEvents MsgReciver As InterprocessReciver

    Private clsVis As clsVisualization
    Private clsMagnet As MagnetizeMe

    Public PlaylistList As New List(Of StreamInformations)

    Private nCurrentPlayIndex As Integer = INVALID_INDEX
    Private dblSamples() As Double

    Private Delegate Sub UpdateUIDelegate(ByRef Info As StreamInformations)
    Private Delegate Sub UpdatePlaylistEntryDelegate(ByRef info As StreamInformations, ByRef Index As Integer)

    Private UpdateUI As New UpdateUIDelegate(AddressOf UpdateUIProc)
    Private UpdatePlaylistEntry As New UpdatePlaylistEntryDelegate(AddressOf UpdatePlaylistEntryProc)

    Private bLoadingParamsPending As Boolean

#Region "Constructor"

    Public Sub New(ByRef Params() As String)

        ' La chiamata è richiesta dalla finestra di progettazione.
        InitializeComponent()

        ' Init var
        bLoadingParamsPending = False

        'If there are params, process...
        If Params.Length > 0 Then

            ' Add files to playlist
            For i As Integer = 0 To Params.Length - 1
                AddFileToPlaylist(Params(i))
            Next

            ' Notify there are pending params to process
            bLoadingParamsPending = True
        End If

    End Sub

#End Region

#Region "Form Events"
    Private Sub AMPlayer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Create decoder
        Decoder = New DecoderManager

        ' Try to load saved output setting. If fail use DirectSound
        If [Enum].IsDefined(GetType(DecoderManager.OutputPlugins), My.Settings.AMPlayer_OutputPlugin) = True Then
            Decoder.Init(Me.Handle, My.Settings.AMPlayer_OutputPlugin)
        Else
            Decoder.Init(Me.Handle, DecoderManager.OutputPlugins.DirectSound)
        End If


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

        ' Create Message reciver
        MsgReciver = New InterprocessReciver(Me.Handle, AMPlayerChannel)

        ' Magnetize main form
        clsMagnet = New MagnetizeMe(Me)

        ' Check if there are some pending params to process
        If bLoadingParamsPending = True Then

            ' Open and Play first item of playlist
            If Decoder.OpenFile(PlaylistList(0).FileLocation) = True Then
                PlayHelper()

                ' Update playlist
                nCurrentPlayIndex = 0

                ' Thread Safe Invocation
                Invoke(UpdatePlaylistEntry, Decoder.CurrentFileStreamInfo, 0)
            End If
        End If
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

            ' Reload values
            VolumePanControl1.Volume = Decoder.Volume
            VolumePanControl1.Pan = Decoder.Pan

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
        DeleteAllItems()
    End Sub

    Private Sub ClearSelectedItemsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearSelectedItemsToolStripMenuItem.Click

        DeleteSelectedItems()
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

    Private Sub OpenPlaylistToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenPlaylistToolStripMenuItem.Click
        OpenPlaylistFile()
    End Sub

    Private Sub SavePlaylistToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SavePlaylistToolStripMenuItem.Click
        SavePlaylistFile()
    End Sub

    Private Sub PlayContextToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlayContextToolStripMenuItem.Click
        PlaySelectedItem()
    End Sub

    Private Sub AddFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddFilesToolStripMenuItem.Click
        AddMultipleFileToPlaylist()
    End Sub

    Private Sub AddFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddFolderToolStripMenuItem.Click
        AddFolderToPlaylist()
    End Sub

    Private Sub DeleteSelectedToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteSelectedToolStripMenuItem.Click
        DeleteSelectedItems()
    End Sub

    Private Sub PreferenciesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PreferenciesToolStripMenuItem.Click
        Dim FrmPref As New frmPreferences

        ' Show Configuration form
        FrmPref.Show(Me)
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
        Invoke(UpdateUI, Decoder.CurrentFileStreamInfo)
    End Sub

#End Region

#Region "Form Functions"

    Private Sub DeleteAllItems()
        ' Remove all items
        PlaylistList.Clear()
        Playlist.RemoveAllRows()

        ' Draw playlist and update scrollbars
        Playlist.UpdateRowGraphics()
        Playlist.UpdateScrollbars()

        ' Reset play index
        nCurrentPlayIndex = INVALID_INDEX
    End Sub

    Private Sub DeleteSelectedItems()
        Dim selectedItems As List(Of Integer)

        ' Get selected items
        selectedItems = Playlist.SelectedItems()

        ' If no elements are selected exit
        If selectedItems.Count = 0 Then Exit Sub

        ' If all items are selected delete all and exit sub
        If selectedItems.Count = PlaylistList.Count Then
            DeleteAllItems()

            Exit Sub
        End If

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

            ' Fix "Display Error in the trackbar at position 0 when playback end" issue
            If Decoder.Duration > 0 Then
                PositionTrackbar.Maximum = Decoder.Duration
            End If

        End If

        If Info IsNot Nothing Then
            Me.Text = "AMPlayer - " & Info.FileName

            StatusLabel.Text = Info.Samplerate & "Hz - " &
                                   Info.Channels & " channels - " &
                                   Info.BitsPerSample & " bits"
        End If


        ' Update menus, Trackbar / Label and Timers
        Select Case Decoder.Status
            Case Status.PLAYING
                StopToolStripMenuItem.Checked = False
                PauseToolStripMenuItem.Checked = False
                PlayToolStripMenuItem.Checked = True

                VisualizationTimer.Enabled = True
                TrackbarTimer.Enabled = True
            Case Status.PAUSING
                StopToolStripMenuItem.Checked = False
                PauseToolStripMenuItem.Checked = True
                PlayToolStripMenuItem.Checked = False

                VisualizationTimer.Enabled = False
                TrackbarTimer.Enabled = False
            Case Status.STOPPED
                StopToolStripMenuItem.Checked = True
                PauseToolStripMenuItem.Checked = False
                PlayToolStripMenuItem.Checked = False

                UpdatePositionAndTimeLabel(0)

                VisualizationTimer.Enabled = True
                TrackbarTimer.Enabled = False
        End Select

    End Sub

    Private Sub UpdatePositionAndTimeLabel(ByVal Value As Long)
        If Value < 0 Then Exit Sub

        PositionTrackbar.Value = Value
        TimeStripLabel.Text = FormatTime(TimeSpan.FromSeconds(Value))
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
                        FormatTime(TimeSpan.FromMilliseconds(0)))

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

                    ' Format time value
                    strDuration = FormatTime(TimeSpan.FromMilliseconds(Info.DurationInMs))

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
        StrDuration = FormatTime(TimeSpan.FromMilliseconds(info.DurationInMs))

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
                strDuration = FormatTime(TimeSpan.FromMilliseconds(info.DurationInMs))

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
        If FilePath = "" Then Exit Sub

        If PlsManager.Open(FilePath, PlaylistManager.PlaylistMode.Read) = True Then

            For i As Integer = 0 To PlsManager.ReadCount - 1


                ' Read Playlist informations
                Info = PlsManager.ReadIndex(i)

                ' Add file to list of Playlist
                PlaylistList.Add(Info)

                ' Check if duration is valid, oterwise use file size
                strDuration = FormatTime(TimeSpan.FromMilliseconds(Info.DurationInMs))

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

    Private Sub SavePlaylistFile()
        Dim OpenDialog As New SafeFileDialog
        Dim PlsManager As New PlaylistManager

        Dim FilePath As String

        FilePath = OpenDialog.SaveSingleFile("Supported Playlist|" & PlsManager.GetPlaylistWriterExtensions)

        ' If no file are selected, exit
        If FilePath = "" Then Exit Sub

        If PlsManager.Open(FilePath, PlaylistManager.PlaylistMode.Write) = True Then

            For i As Integer = 0 To PlaylistList.Count - 1

                ' Write playlist line
                If PlsManager.WriteLine(PlaylistList(i)) = False Then
                    ' On error close
                    MsgBox("Error writing playlist")
                    PlsManager.Close()
                    PlsManager.Dispose()
                    Exit Sub
                End If
            Next

            ' Free resources
            PlsManager.Close()
            PlsManager.Dispose()
        End If

    End Sub

    Private Sub PlaySelectedItem()
        Dim Indexes As List(Of Integer)

        If Decoder IsNot Nothing Then
            ' Read selected items
            Indexes = Playlist.SelectedItems()

            ' Check at least one file is selected
            If Indexes.Count = 0 Then Exit Sub

            ' Open and Play first selected file
            If Decoder.OpenFile(PlaylistList(Indexes(0)).FileLocation) = True Then
                PlayHelper()

                ' Update playlist
                nCurrentPlayIndex = Indexes(0)

                ' Thread Safe Invocation
                Invoke(UpdatePlaylistEntry, Decoder.CurrentFileStreamInfo, nCurrentPlayIndex)
                Invoke(UpdateUI, Decoder.CurrentFileStreamInfo)
            End If

        End If
    End Sub

#End Region

#Region "Other Instances message reciver"

    Private Sub MsgReciver_RecivedDataEvent(ByRef data As InterprocessTransferData) Handles MsgReciver.RecivedDataEvent

        Select Case data.Action
            Case InterprocessActions.AddToPlaylist
                AddFileToPlaylist(data.Path)
                UpdatePlaylist()
            Case InterprocessActions.AddToPlaylistAndPlayItem
                AddFileToPlaylist(data.Path)

                ' Close current streaming
                If Decoder.Status <> Status.STOPPED Then
                    Decoder.Status = Status.STOPPED
                    Decoder.Close()
                End If

                ' Open and Play
                If Decoder.OpenFile(data.Path) = True Then
                    PlayHelper()

                    ' Update playlist
                    nCurrentPlayIndex = PlaylistList.Count - 1

                    ' Thread Safe Invocation
                    Invoke(UpdatePlaylistEntry, Decoder.CurrentFileStreamInfo, nCurrentPlayIndex)
                End If


        End Select

#If DEBUG Then
        DebugPrintLine("AMPlayer", "Message recived: " & data.Path)
#End If
    End Sub




#End Region
End Class
