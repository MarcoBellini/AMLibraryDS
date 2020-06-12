<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class AMPlayer
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla nell'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AMPlayer))
        Me.VisualizationTimer = New System.Windows.Forms.Timer(Me.components)
        Me.TrackbarTimer = New System.Windows.Forms.Timer(Me.components)
        Me.BottomStrip = New System.Windows.Forms.StatusStrip()
        Me.StatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.TimeStripLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenCDToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.AddFilesToPlaylistToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddFolderToPlaylistToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.PreferenciesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PlaylistToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearSelectedItemsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripSeparator()
        Me.SavePlaylistToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenPlaylistToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem8 = New System.Windows.Forms.ToolStripSeparator()
        Me.GetFullDetailsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PlaybackToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PlayToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PauseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StopToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.NextToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PreviouToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem6 = New System.Windows.Forms.ToolStripSeparator()
        Me.ViewFileTagsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TranscodeFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RipCDToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem7 = New System.Windows.Forms.ToolStripSeparator()
        Me.EffectsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TopToolstrip = New System.Windows.Forms.ToolStrip()
        Me.PlayStripButton = New System.Windows.Forms.ToolStripButton()
        Me.PauseStripButton = New System.Windows.Forms.ToolStripButton()
        Me.StopStripButton = New System.Windows.Forms.ToolStripButton()
        Me.RewindStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ForwardStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.EqualizerToolStrip = New System.Windows.Forms.ToolStripButton()
        Me.VolumeControlButton = New System.Windows.Forms.ToolStripButton()
        Me.PositionTrackbar = New System.Windows.Forms.TrackBar()
        Me.PicVisualization = New System.Windows.Forms.PictureBox()
        Me.VolumePanControl1 = New AMPlayerDS.VolumePanControl()
        Me.Playlist = New AMPlayerDS.CustomListViewControl()
        Me.BottomStrip.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.TopToolstrip.SuspendLayout()
        CType(Me.PositionTrackbar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PicVisualization, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'VisualizationTimer
        '
        Me.VisualizationTimer.Interval = 25
        '
        'TrackbarTimer
        '
        Me.TrackbarTimer.Interval = 1000
        '
        'BottomStrip
        '
        Me.BottomStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabel, Me.ToolStripStatusLabel1, Me.TimeStripLabel})
        Me.BottomStrip.Location = New System.Drawing.Point(0, 374)
        Me.BottomStrip.Name = "BottomStrip"
        Me.BottomStrip.Size = New System.Drawing.Size(524, 22)
        Me.BottomStrip.TabIndex = 27
        Me.BottomStrip.Text = "BottomStrip"
        '
        'StatusLabel
        '
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(93, 17)
        Me.StatusLabel.Text = "AMPlayer Ready"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusLabel1.Text = "|"
        '
        'TimeStripLabel
        '
        Me.TimeStripLabel.Name = "TimeStripLabel"
        Me.TimeStripLabel.Size = New System.Drawing.Size(34, 17)
        Me.TimeStripLabel.Text = "00:00"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.PlaylistToolStripMenuItem, Me.PlaybackToolStripMenuItem, Me.ToolsToolStripMenuItem, Me.SettingsToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(524, 24)
        Me.MenuStrip1.TabIndex = 28
        Me.MenuStrip1.Text = "MenuStrip"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenFileToolStripMenuItem, Me.OpenCDToolStripMenuItem, Me.ToolStripMenuItem1, Me.AddFilesToPlaylistToolStripMenuItem, Me.AddFolderToPlaylistToolStripMenuItem, Me.ToolStripMenuItem2, Me.PreferenciesToolStripMenuItem, Me.ToolStripMenuItem4, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'OpenFileToolStripMenuItem
        '
        Me.OpenFileToolStripMenuItem.Name = "OpenFileToolStripMenuItem"
        Me.OpenFileToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.OpenFileToolStripMenuItem.Text = "Open File"
        '
        'OpenCDToolStripMenuItem
        '
        Me.OpenCDToolStripMenuItem.Name = "OpenCDToolStripMenuItem"
        Me.OpenCDToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.OpenCDToolStripMenuItem.Text = "Open CD"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(183, 6)
        '
        'AddFilesToPlaylistToolStripMenuItem
        '
        Me.AddFilesToPlaylistToolStripMenuItem.Name = "AddFilesToPlaylistToolStripMenuItem"
        Me.AddFilesToPlaylistToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.AddFilesToPlaylistToolStripMenuItem.Text = "Add Files to Playlist"
        '
        'AddFolderToPlaylistToolStripMenuItem
        '
        Me.AddFolderToPlaylistToolStripMenuItem.Name = "AddFolderToPlaylistToolStripMenuItem"
        Me.AddFolderToPlaylistToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.AddFolderToPlaylistToolStripMenuItem.Text = "Add Folder to Playlist"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(183, 6)
        '
        'PreferenciesToolStripMenuItem
        '
        Me.PreferenciesToolStripMenuItem.Name = "PreferenciesToolStripMenuItem"
        Me.PreferenciesToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.PreferenciesToolStripMenuItem.Text = "Preferencies"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(183, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'PlaylistToolStripMenuItem
        '
        Me.PlaylistToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ClearSelectedItemsToolStripMenuItem, Me.ClearAllToolStripMenuItem, Me.ToolStripMenuItem5, Me.SavePlaylistToolStripMenuItem, Me.OpenPlaylistToolStripMenuItem, Me.ToolStripMenuItem8, Me.GetFullDetailsToolStripMenuItem})
        Me.PlaylistToolStripMenuItem.Name = "PlaylistToolStripMenuItem"
        Me.PlaylistToolStripMenuItem.Size = New System.Drawing.Size(56, 20)
        Me.PlaylistToolStripMenuItem.Text = "Playlist"
        '
        'ClearSelectedItemsToolStripMenuItem
        '
        Me.ClearSelectedItemsToolStripMenuItem.Name = "ClearSelectedItemsToolStripMenuItem"
        Me.ClearSelectedItemsToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ClearSelectedItemsToolStripMenuItem.Text = "Clear Selected Items"
        '
        'ClearAllToolStripMenuItem
        '
        Me.ClearAllToolStripMenuItem.Name = "ClearAllToolStripMenuItem"
        Me.ClearAllToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ClearAllToolStripMenuItem.Text = "Clear All"
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        Me.ToolStripMenuItem5.Size = New System.Drawing.Size(177, 6)
        '
        'SavePlaylistToolStripMenuItem
        '
        Me.SavePlaylistToolStripMenuItem.Name = "SavePlaylistToolStripMenuItem"
        Me.SavePlaylistToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SavePlaylistToolStripMenuItem.Text = "Save Playlist..."
        '
        'OpenPlaylistToolStripMenuItem
        '
        Me.OpenPlaylistToolStripMenuItem.Name = "OpenPlaylistToolStripMenuItem"
        Me.OpenPlaylistToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.OpenPlaylistToolStripMenuItem.Text = "Open Playlist..."
        '
        'ToolStripMenuItem8
        '
        Me.ToolStripMenuItem8.Name = "ToolStripMenuItem8"
        Me.ToolStripMenuItem8.Size = New System.Drawing.Size(177, 6)
        '
        'GetFullDetailsToolStripMenuItem
        '
        Me.GetFullDetailsToolStripMenuItem.Name = "GetFullDetailsToolStripMenuItem"
        Me.GetFullDetailsToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.GetFullDetailsToolStripMenuItem.Text = "Get Full Details"
        '
        'PlaybackToolStripMenuItem
        '
        Me.PlaybackToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PlayToolStripMenuItem, Me.PauseToolStripMenuItem, Me.StopToolStripMenuItem, Me.ToolStripMenuItem3, Me.NextToolStripMenuItem, Me.PreviouToolStripMenuItem, Me.ToolStripMenuItem6, Me.ViewFileTagsToolStripMenuItem})
        Me.PlaybackToolStripMenuItem.Name = "PlaybackToolStripMenuItem"
        Me.PlaybackToolStripMenuItem.Size = New System.Drawing.Size(66, 20)
        Me.PlaybackToolStripMenuItem.Text = "Playback"
        '
        'PlayToolStripMenuItem
        '
        Me.PlayToolStripMenuItem.Name = "PlayToolStripMenuItem"
        Me.PlayToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.PlayToolStripMenuItem.Text = "Play"
        '
        'PauseToolStripMenuItem
        '
        Me.PauseToolStripMenuItem.Name = "PauseToolStripMenuItem"
        Me.PauseToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.PauseToolStripMenuItem.Text = "Pause"
        '
        'StopToolStripMenuItem
        '
        Me.StopToolStripMenuItem.Name = "StopToolStripMenuItem"
        Me.StopToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.StopToolStripMenuItem.Text = "Stop"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(143, 6)
        '
        'NextToolStripMenuItem
        '
        Me.NextToolStripMenuItem.Name = "NextToolStripMenuItem"
        Me.NextToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.NextToolStripMenuItem.Text = "Next"
        '
        'PreviouToolStripMenuItem
        '
        Me.PreviouToolStripMenuItem.Name = "PreviouToolStripMenuItem"
        Me.PreviouToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.PreviouToolStripMenuItem.Text = "Previous"
        '
        'ToolStripMenuItem6
        '
        Me.ToolStripMenuItem6.Name = "ToolStripMenuItem6"
        Me.ToolStripMenuItem6.Size = New System.Drawing.Size(143, 6)
        '
        'ViewFileTagsToolStripMenuItem
        '
        Me.ViewFileTagsToolStripMenuItem.Name = "ViewFileTagsToolStripMenuItem"
        Me.ViewFileTagsToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.ViewFileTagsToolStripMenuItem.Text = "View File Tags"
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TranscodeFilesToolStripMenuItem, Me.RipCDToolStripMenuItem, Me.ToolStripMenuItem7, Me.EffectsToolStripMenuItem})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(46, 20)
        Me.ToolsToolStripMenuItem.Text = "Tools"
        '
        'TranscodeFilesToolStripMenuItem
        '
        Me.TranscodeFilesToolStripMenuItem.Name = "TranscodeFilesToolStripMenuItem"
        Me.TranscodeFilesToolStripMenuItem.Size = New System.Drawing.Size(162, 22)
        Me.TranscodeFilesToolStripMenuItem.Text = "Transcode Files..."
        '
        'RipCDToolStripMenuItem
        '
        Me.RipCDToolStripMenuItem.Name = "RipCDToolStripMenuItem"
        Me.RipCDToolStripMenuItem.Size = New System.Drawing.Size(162, 22)
        Me.RipCDToolStripMenuItem.Text = "Rip CD"
        '
        'ToolStripMenuItem7
        '
        Me.ToolStripMenuItem7.Name = "ToolStripMenuItem7"
        Me.ToolStripMenuItem7.Size = New System.Drawing.Size(159, 6)
        '
        'EffectsToolStripMenuItem
        '
        Me.EffectsToolStripMenuItem.Name = "EffectsToolStripMenuItem"
        Me.EffectsToolStripMenuItem.Size = New System.Drawing.Size(162, 22)
        Me.EffectsToolStripMenuItem.Text = "Effects"
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem})
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(24, 20)
        Me.SettingsToolStripMenuItem.Text = "?"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'TopToolstrip
        '
        Me.TopToolstrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PlayStripButton, Me.PauseStripButton, Me.StopStripButton, Me.RewindStripButton, Me.ForwardStripButton, Me.ToolStripSeparator1, Me.EqualizerToolStrip, Me.VolumeControlButton})
        Me.TopToolstrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.TopToolstrip.Location = New System.Drawing.Point(0, 24)
        Me.TopToolstrip.Name = "TopToolstrip"
        Me.TopToolstrip.Size = New System.Drawing.Size(524, 25)
        Me.TopToolstrip.TabIndex = 29
        Me.TopToolstrip.Text = "ToolStrip1"
        '
        'PlayStripButton
        '
        Me.PlayStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.PlayStripButton.Image = CType(resources.GetObject("PlayStripButton.Image"), System.Drawing.Image)
        Me.PlayStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.PlayStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.PlayStripButton.Name = "PlayStripButton"
        Me.PlayStripButton.Size = New System.Drawing.Size(23, 22)
        Me.PlayStripButton.Text = "Play"
        '
        'PauseStripButton
        '
        Me.PauseStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.PauseStripButton.Image = CType(resources.GetObject("PauseStripButton.Image"), System.Drawing.Image)
        Me.PauseStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.PauseStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.PauseStripButton.Name = "PauseStripButton"
        Me.PauseStripButton.Size = New System.Drawing.Size(23, 22)
        Me.PauseStripButton.Text = "Pause"
        '
        'StopStripButton
        '
        Me.StopStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.StopStripButton.Image = CType(resources.GetObject("StopStripButton.Image"), System.Drawing.Image)
        Me.StopStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.StopStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.StopStripButton.Name = "StopStripButton"
        Me.StopStripButton.Size = New System.Drawing.Size(23, 22)
        Me.StopStripButton.Text = "Stop"
        '
        'RewindStripButton
        '
        Me.RewindStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.RewindStripButton.Image = CType(resources.GetObject("RewindStripButton.Image"), System.Drawing.Image)
        Me.RewindStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.RewindStripButton.Name = "RewindStripButton"
        Me.RewindStripButton.Size = New System.Drawing.Size(23, 22)
        Me.RewindStripButton.Text = "Rewind"
        '
        'ForwardStripButton
        '
        Me.ForwardStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ForwardStripButton.Image = CType(resources.GetObject("ForwardStripButton.Image"), System.Drawing.Image)
        Me.ForwardStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ForwardStripButton.Name = "ForwardStripButton"
        Me.ForwardStripButton.Size = New System.Drawing.Size(23, 22)
        Me.ForwardStripButton.Text = "ToolStripButton2"
        Me.ForwardStripButton.ToolTipText = "Forward"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'EqualizerToolStrip
        '
        Me.EqualizerToolStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.EqualizerToolStrip.Image = CType(resources.GetObject("EqualizerToolStrip.Image"), System.Drawing.Image)
        Me.EqualizerToolStrip.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.EqualizerToolStrip.Name = "EqualizerToolStrip"
        Me.EqualizerToolStrip.Size = New System.Drawing.Size(23, 22)
        Me.EqualizerToolStrip.Text = "Equalizer"
        '
        'VolumeControlButton
        '
        Me.VolumeControlButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.VolumeControlButton.Image = CType(resources.GetObject("VolumeControlButton.Image"), System.Drawing.Image)
        Me.VolumeControlButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.VolumeControlButton.Name = "VolumeControlButton"
        Me.VolumeControlButton.Size = New System.Drawing.Size(23, 22)
        Me.VolumeControlButton.Text = "Volume and Pan"
        '
        'PositionTrackbar
        '
        Me.PositionTrackbar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PositionTrackbar.LargeChange = 10
        Me.PositionTrackbar.Location = New System.Drawing.Point(0, 328)
        Me.PositionTrackbar.Name = "PositionTrackbar"
        Me.PositionTrackbar.Size = New System.Drawing.Size(524, 45)
        Me.PositionTrackbar.SmallChange = 10
        Me.PositionTrackbar.TabIndex = 30
        Me.PositionTrackbar.TickFrequency = 0
        Me.PositionTrackbar.TickStyle = System.Windows.Forms.TickStyle.Both
        '
        'PicVisualization
        '
        Me.PicVisualization.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PicVisualization.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.PicVisualization.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicVisualization.Location = New System.Drawing.Point(409, 5)
        Me.PicVisualization.Name = "PicVisualization"
        Me.PicVisualization.Size = New System.Drawing.Size(104, 41)
        Me.PicVisualization.TabIndex = 32
        Me.PicVisualization.TabStop = False
        '
        'VolumePanControl1
        '
        Me.VolumePanControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.VolumePanControl1.Location = New System.Drawing.Point(12, 92)
        Me.VolumePanControl1.Name = "VolumePanControl1"
        Me.VolumePanControl1.Pan = 0
        Me.VolumePanControl1.Size = New System.Drawing.Size(254, 199)
        Me.VolumePanControl1.TabIndex = 31
        Me.VolumePanControl1.Visible = False
        Me.VolumePanControl1.Volume = 100
        '
        'Playlist
        '
        Me.Playlist.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Playlist.AutoScroll = True
        Me.Playlist.BackColor = System.Drawing.SystemColors.Control
        Me.Playlist.BackgroundColorProperty = System.Drawing.Color.DarkSeaGreen
        Me.Playlist.HighlightItemColorProperty = System.Drawing.Color.NavajoWhite
        Me.Playlist.ItemColorProperty = System.Drawing.Color.DarkSeaGreen
        Me.Playlist.ItemFontProperty = New System.Drawing.Font("Arial Narrow", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Playlist.ItemTextColorProperty = System.Drawing.Color.Black
        Me.Playlist.Location = New System.Drawing.Point(1, 52)
        Me.Playlist.Name = "Playlist"
        Me.Playlist.PairItemColorProperty = System.Drawing.Color.LightGreen
        Me.Playlist.RowHeightProperty = 20
        Me.Playlist.SelectedItemColorProperty = System.Drawing.Color.Aquamarine
        Me.Playlist.Size = New System.Drawing.Size(521, 270)
        Me.Playlist.TabIndex = 26
        '
        'AMPlayer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(524, 396)
        Me.Controls.Add(Me.PicVisualization)
        Me.Controls.Add(Me.VolumePanControl1)
        Me.Controls.Add(Me.PositionTrackbar)
        Me.Controls.Add(Me.TopToolstrip)
        Me.Controls.Add(Me.BottomStrip)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.Playlist)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MaximizeBox = False
        Me.MinimumSize = New System.Drawing.Size(540, 435)
        Me.Name = "AMPlayer"
        Me.Text = "AMPlayer"
        Me.BottomStrip.ResumeLayout(False)
        Me.BottomStrip.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.TopToolstrip.ResumeLayout(False)
        Me.TopToolstrip.PerformLayout()
        CType(Me.PositionTrackbar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PicVisualization, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents VisualizationTimer As System.Windows.Forms.Timer
    Friend WithEvents TrackbarTimer As System.Windows.Forms.Timer
    Friend WithEvents Playlist As CustomListViewControl
    Friend WithEvents BottomStrip As StatusStrip
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenCDToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents AddFilesToPlaylistToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddFolderToPlaylistToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents PreferenciesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PlaylistToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearSelectedItemsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PlaybackToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PlayToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PauseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StopToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As ToolStripSeparator
    Friend WithEvents NextToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PreviouToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SettingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TopToolstrip As ToolStrip
    Friend WithEvents PlayStripButton As ToolStripButton
    Friend WithEvents PauseStripButton As ToolStripButton
    Friend WithEvents StopStripButton As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents PositionTrackbar As TrackBar
    Friend WithEvents StatusLabel As ToolStripStatusLabel
    Friend WithEvents VolumeControlButton As ToolStripButton
    Friend WithEvents VolumePanControl1 As VolumePanControl
    Friend WithEvents PicVisualization As PictureBox
    Friend WithEvents RewindStripButton As ToolStripButton
    Friend WithEvents ForwardStripButton As ToolStripButton
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents TimeStripLabel As ToolStripStatusLabel
    Friend WithEvents EqualizerToolStrip As ToolStripButton
    Friend WithEvents ToolStripMenuItem5 As ToolStripSeparator
    Friend WithEvents SavePlaylistToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenPlaylistToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem6 As ToolStripSeparator
    Friend WithEvents ViewFileTagsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TranscodeFilesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RipCDToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem7 As ToolStripSeparator
    Friend WithEvents EffectsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem8 As ToolStripSeparator
    Friend WithEvents GetFullDetailsToolStripMenuItem As ToolStripMenuItem
End Class
