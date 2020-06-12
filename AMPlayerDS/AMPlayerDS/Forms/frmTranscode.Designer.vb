<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTranscode
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla mediante l'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ListboxFiles = New System.Windows.Forms.ListBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.OutputSamplerateCombobox = New System.Windows.Forms.ComboBox()
        Me.MonoRadioButton = New System.Windows.Forms.RadioButton()
        Me.StereoRadioButton = New System.Windows.Forms.RadioButton()
        Me.EnableResampleCheckbox = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.ConfigureOutputButton = New System.Windows.Forms.Button()
        Me.OutputFormatCombobox = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.SelectFolderButton = New System.Windows.Forms.Button()
        Me.FolderPathLabel = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TranscodeProgressBar = New System.Windows.Forms.ProgressBar()
        Me.AddButton = New System.Windows.Forms.Button()
        Me.RemoveButton = New System.Windows.Forms.Button()
        Me.ProcessButton = New System.Windows.Forms.Button()
        Me.CloseButton = New System.Windows.Forms.Button()
        Me.TranscodeThread = New System.ComponentModel.BackgroundWorker()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.ProgressLabel = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'ListboxFiles
        '
        Me.ListboxFiles.FormattingEnabled = True
        Me.ListboxFiles.HorizontalScrollbar = True
        Me.ListboxFiles.Location = New System.Drawing.Point(12, 12)
        Me.ListboxFiles.Name = "ListboxFiles"
        Me.ListboxFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple
        Me.ListboxFiles.Size = New System.Drawing.Size(226, 329)
        Me.ListboxFiles.TabIndex = 0
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.OutputSamplerateCombobox)
        Me.GroupBox1.Controls.Add(Me.MonoRadioButton)
        Me.GroupBox1.Controls.Add(Me.StereoRadioButton)
        Me.GroupBox1.Controls.Add(Me.EnableResampleCheckbox)
        Me.GroupBox1.Location = New System.Drawing.Point(260, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(259, 115)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Resample"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(14, 64)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(96, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Output samplerate:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(128, 21)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(54, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Channels:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(16, 21)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(88, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Enable resample:"
        '
        'OutputSamplerateCombobox
        '
        Me.OutputSamplerateCombobox.FormattingEnabled = True
        Me.OutputSamplerateCombobox.Location = New System.Drawing.Point(17, 80)
        Me.OutputSamplerateCombobox.Name = "OutputSamplerateCombobox"
        Me.OutputSamplerateCombobox.Size = New System.Drawing.Size(228, 21)
        Me.OutputSamplerateCombobox.TabIndex = 3
        '
        'MonoRadioButton
        '
        Me.MonoRadioButton.AutoSize = True
        Me.MonoRadioButton.Location = New System.Drawing.Point(193, 37)
        Me.MonoRadioButton.Name = "MonoRadioButton"
        Me.MonoRadioButton.Size = New System.Drawing.Size(52, 17)
        Me.MonoRadioButton.TabIndex = 2
        Me.MonoRadioButton.Text = "Mono"
        Me.MonoRadioButton.UseVisualStyleBackColor = True
        '
        'StereoRadioButton
        '
        Me.StereoRadioButton.AutoSize = True
        Me.StereoRadioButton.Checked = True
        Me.StereoRadioButton.Location = New System.Drawing.Point(131, 37)
        Me.StereoRadioButton.Name = "StereoRadioButton"
        Me.StereoRadioButton.Size = New System.Drawing.Size(56, 17)
        Me.StereoRadioButton.TabIndex = 1
        Me.StereoRadioButton.TabStop = True
        Me.StereoRadioButton.Text = "Stereo"
        Me.StereoRadioButton.UseVisualStyleBackColor = True
        '
        'EnableResampleCheckbox
        '
        Me.EnableResampleCheckbox.AutoSize = True
        Me.EnableResampleCheckbox.Location = New System.Drawing.Point(17, 38)
        Me.EnableResampleCheckbox.Name = "EnableResampleCheckbox"
        Me.EnableResampleCheckbox.Size = New System.Drawing.Size(108, 17)
        Me.EnableResampleCheckbox.TabIndex = 0
        Me.EnableResampleCheckbox.Text = "Resample Output"
        Me.EnableResampleCheckbox.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.ConfigureOutputButton)
        Me.GroupBox2.Controls.Add(Me.OutputFormatCombobox)
        Me.GroupBox2.Controls.Add(Me.Label6)
        Me.GroupBox2.Controls.Add(Me.SelectFolderButton)
        Me.GroupBox2.Controls.Add(Me.FolderPathLabel)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Location = New System.Drawing.Point(260, 133)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(259, 152)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Output"
        '
        'ConfigureOutputButton
        '
        Me.ConfigureOutputButton.Location = New System.Drawing.Point(131, 123)
        Me.ConfigureOutputButton.Name = "ConfigureOutputButton"
        Me.ConfigureOutputButton.Size = New System.Drawing.Size(114, 22)
        Me.ConfigureOutputButton.TabIndex = 6
        Me.ConfigureOutputButton.Text = "Configure Output..."
        Me.ConfigureOutputButton.UseVisualStyleBackColor = True
        '
        'OutputFormatCombobox
        '
        Me.OutputFormatCombobox.FormattingEnabled = True
        Me.OutputFormatCombobox.Location = New System.Drawing.Point(17, 96)
        Me.OutputFormatCombobox.Name = "OutputFormatCombobox"
        Me.OutputFormatCombobox.Size = New System.Drawing.Size(228, 21)
        Me.OutputFormatCombobox.TabIndex = 7
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(16, 80)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(74, 13)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = "Output format:"
        '
        'SelectFolderButton
        '
        Me.SelectFolderButton.Location = New System.Drawing.Point(17, 39)
        Me.SelectFolderButton.Name = "SelectFolderButton"
        Me.SelectFolderButton.Size = New System.Drawing.Size(100, 23)
        Me.SelectFolderButton.TabIndex = 9
        Me.SelectFolderButton.Text = "Select Folder..."
        Me.SelectFolderButton.UseVisualStyleBackColor = True
        '
        'FolderPathLabel
        '
        Me.FolderPathLabel.AutoSize = True
        Me.FolderPathLabel.Location = New System.Drawing.Point(128, 44)
        Me.FolderPathLabel.Name = "FolderPathLabel"
        Me.FolderPathLabel.Size = New System.Drawing.Size(113, 13)
        Me.FolderPathLabel.TabIndex = 8
        Me.FolderPathLabel.Text = "Select Output Folder..."
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(14, 23)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(95, 13)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Destination Folder:"
        '
        'TranscodeProgressBar
        '
        Me.TranscodeProgressBar.Location = New System.Drawing.Point(103, 24)
        Me.TranscodeProgressBar.Name = "TranscodeProgressBar"
        Me.TranscodeProgressBar.Size = New System.Drawing.Size(142, 12)
        Me.TranscodeProgressBar.TabIndex = 11
        '
        'AddButton
        '
        Me.AddButton.Location = New System.Drawing.Point(39, 354)
        Me.AddButton.Name = "AddButton"
        Me.AddButton.Size = New System.Drawing.Size(75, 23)
        Me.AddButton.TabIndex = 3
        Me.AddButton.Text = "Add"
        Me.AddButton.UseVisualStyleBackColor = True
        '
        'RemoveButton
        '
        Me.RemoveButton.Location = New System.Drawing.Point(120, 354)
        Me.RemoveButton.Name = "RemoveButton"
        Me.RemoveButton.Size = New System.Drawing.Size(75, 23)
        Me.RemoveButton.TabIndex = 4
        Me.RemoveButton.Text = "Remove"
        Me.RemoveButton.UseVisualStyleBackColor = True
        '
        'ProcessButton
        '
        Me.ProcessButton.Location = New System.Drawing.Point(444, 354)
        Me.ProcessButton.Name = "ProcessButton"
        Me.ProcessButton.Size = New System.Drawing.Size(75, 23)
        Me.ProcessButton.TabIndex = 5
        Me.ProcessButton.Text = "Process"
        Me.ProcessButton.UseVisualStyleBackColor = True
        '
        'CloseButton
        '
        Me.CloseButton.Location = New System.Drawing.Point(363, 354)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(75, 23)
        Me.CloseButton.TabIndex = 6
        Me.CloseButton.Text = "Close"
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'TranscodeThread
        '
        Me.TranscodeThread.WorkerReportsProgress = True
        Me.TranscodeThread.WorkerSupportsCancellation = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.ProgressLabel)
        Me.GroupBox3.Controls.Add(Me.TranscodeProgressBar)
        Me.GroupBox3.Location = New System.Drawing.Point(260, 291)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(259, 50)
        Me.GroupBox3.TabIndex = 12
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Progress"
        '
        'ProgressLabel
        '
        Me.ProgressLabel.AutoSize = True
        Me.ProgressLabel.Location = New System.Drawing.Point(14, 23)
        Me.ProgressLabel.Name = "ProgressLabel"
        Me.ProgressLabel.Size = New System.Drawing.Size(86, 13)
        Me.ProgressLabel.TabIndex = 12
        Me.ProgressLabel.Text = "Start Encoding..."
        '
        'frmTranscode
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(538, 385)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.CloseButton)
        Me.Controls.Add(Me.RemoveButton)
        Me.Controls.Add(Me.AddButton)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.ProcessButton)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.ListboxFiles)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmTranscode"
        Me.Text = "Transcode file"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ListboxFiles As ListBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents ProcessButton As Button
    Friend WithEvents AddButton As Button
    Friend WithEvents RemoveButton As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents OutputSamplerateCombobox As ComboBox
    Friend WithEvents MonoRadioButton As RadioButton
    Friend WithEvents StereoRadioButton As RadioButton
    Friend WithEvents EnableResampleCheckbox As CheckBox
    Friend WithEvents ConfigureOutputButton As Button
    Friend WithEvents OutputFormatCombobox As ComboBox
    Friend WithEvents Label6 As Label
    Friend WithEvents SelectFolderButton As Button
    Friend WithEvents FolderPathLabel As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents CloseButton As Button
    Friend WithEvents TranscodeThread As System.ComponentModel.BackgroundWorker
    Friend WithEvents TranscodeProgressBar As ProgressBar
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents ProgressLabel As Label
End Class
