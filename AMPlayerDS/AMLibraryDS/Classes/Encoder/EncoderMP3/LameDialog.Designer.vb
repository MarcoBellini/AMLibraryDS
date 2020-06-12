<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LameDialog
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
        Me.ButtonOK = New System.Windows.Forms.Button()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.TargetGroupbox = New System.Windows.Forms.GroupBox()
        Me.EncoderQualityGroupbox = New System.Windows.Forms.GroupBox()
        Me.BitrateRadioButton = New System.Windows.Forms.RadioButton()
        Me.QualityRadioButton = New System.Windows.Forms.RadioButton()
        Me.BitrateGroupbox = New System.Windows.Forms.GroupBox()
        Me.ForceCBR = New System.Windows.Forms.CheckBox()
        Me.BitrateTrackbar = New System.Windows.Forms.TrackBar()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.BitrateLabel = New System.Windows.Forms.Label()
        Me.EncoderQualityCombobox = New System.Windows.Forms.ComboBox()
        Me.QualityGroupBox = New System.Windows.Forms.GroupBox()
        Me.QualityLabel = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.QualityTrackbar = New System.Windows.Forms.TrackBar()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.VBRQualityCombobox = New System.Windows.Forms.ComboBox()
        Me.LameVersionLabel = New System.Windows.Forms.Label()
        Me.TargetGroupbox.SuspendLayout()
        Me.EncoderQualityGroupbox.SuspendLayout()
        Me.BitrateGroupbox.SuspendLayout()
        CType(Me.BitrateTrackbar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.QualityGroupBox.SuspendLayout()
        CType(Me.QualityTrackbar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ButtonOK
        '
        Me.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.ButtonOK.Location = New System.Drawing.Point(346, 321)
        Me.ButtonOK.Name = "ButtonOK"
        Me.ButtonOK.Size = New System.Drawing.Size(75, 23)
        Me.ButtonOK.TabIndex = 0
        Me.ButtonOK.Text = "OK"
        Me.ButtonOK.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonCancel.Location = New System.Drawing.Point(265, 321)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButtonCancel.TabIndex = 1
        Me.ButtonCancel.Text = "Cancel"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'TargetGroupbox
        '
        Me.TargetGroupbox.Controls.Add(Me.QualityRadioButton)
        Me.TargetGroupbox.Controls.Add(Me.BitrateRadioButton)
        Me.TargetGroupbox.Location = New System.Drawing.Point(23, 12)
        Me.TargetGroupbox.Name = "TargetGroupbox"
        Me.TargetGroupbox.Size = New System.Drawing.Size(174, 87)
        Me.TargetGroupbox.TabIndex = 2
        Me.TargetGroupbox.TabStop = False
        Me.TargetGroupbox.Text = "Target"
        '
        'EncoderQualityGroupbox
        '
        Me.EncoderQualityGroupbox.Controls.Add(Me.EncoderQualityCombobox)
        Me.EncoderQualityGroupbox.Location = New System.Drawing.Point(227, 12)
        Me.EncoderQualityGroupbox.Name = "EncoderQualityGroupbox"
        Me.EncoderQualityGroupbox.Size = New System.Drawing.Size(194, 87)
        Me.EncoderQualityGroupbox.TabIndex = 3
        Me.EncoderQualityGroupbox.TabStop = False
        Me.EncoderQualityGroupbox.Text = "Encoding Quality"
        '
        'BitrateRadioButton
        '
        Me.BitrateRadioButton.AutoSize = True
        Me.BitrateRadioButton.Checked = True
        Me.BitrateRadioButton.Location = New System.Drawing.Point(18, 33)
        Me.BitrateRadioButton.Name = "BitrateRadioButton"
        Me.BitrateRadioButton.Size = New System.Drawing.Size(55, 17)
        Me.BitrateRadioButton.TabIndex = 4
        Me.BitrateRadioButton.TabStop = True
        Me.BitrateRadioButton.Text = "Bitrate"
        Me.BitrateRadioButton.UseVisualStyleBackColor = True
        '
        'QualityRadioButton
        '
        Me.QualityRadioButton.AutoSize = True
        Me.QualityRadioButton.Location = New System.Drawing.Point(100, 33)
        Me.QualityRadioButton.Name = "QualityRadioButton"
        Me.QualityRadioButton.Size = New System.Drawing.Size(57, 17)
        Me.QualityRadioButton.TabIndex = 5
        Me.QualityRadioButton.Text = "Quality"
        Me.QualityRadioButton.UseVisualStyleBackColor = True
        '
        'BitrateGroupbox
        '
        Me.BitrateGroupbox.Controls.Add(Me.BitrateLabel)
        Me.BitrateGroupbox.Controls.Add(Me.Label1)
        Me.BitrateGroupbox.Controls.Add(Me.BitrateTrackbar)
        Me.BitrateGroupbox.Controls.Add(Me.ForceCBR)
        Me.BitrateGroupbox.Location = New System.Drawing.Point(23, 105)
        Me.BitrateGroupbox.Name = "BitrateGroupbox"
        Me.BitrateGroupbox.Size = New System.Drawing.Size(398, 101)
        Me.BitrateGroupbox.TabIndex = 6
        Me.BitrateGroupbox.TabStop = False
        Me.BitrateGroupbox.Text = "Bitrate"
        '
        'ForceCBR
        '
        Me.ForceCBR.AutoSize = True
        Me.ForceCBR.Location = New System.Drawing.Point(7, 70)
        Me.ForceCBR.Name = "ForceCBR"
        Me.ForceCBR.Size = New System.Drawing.Size(167, 17)
        Me.ForceCBR.TabIndex = 12
        Me.ForceCBR.Text = "Force LAME to costant bitrate"
        Me.ForceCBR.UseVisualStyleBackColor = True
        '
        'BitrateTrackbar
        '
        Me.BitrateTrackbar.LargeChange = 8
        Me.BitrateTrackbar.Location = New System.Drawing.Point(6, 19)
        Me.BitrateTrackbar.Maximum = 320
        Me.BitrateTrackbar.Minimum = 8
        Me.BitrateTrackbar.Name = "BitrateTrackbar"
        Me.BitrateTrackbar.Size = New System.Drawing.Size(386, 45)
        Me.BitrateTrackbar.SmallChange = 8
        Me.BitrateTrackbar.TabIndex = 13
        Me.BitrateTrackbar.TickFrequency = 8
        Me.BitrateTrackbar.Value = 192
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(268, 70)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(40, 13)
        Me.Label1.TabIndex = 14
        Me.Label1.Text = "Bitrate:"
        '
        'BitrateLabel
        '
        Me.BitrateLabel.AutoSize = True
        Me.BitrateLabel.Location = New System.Drawing.Point(314, 70)
        Me.BitrateLabel.Name = "BitrateLabel"
        Me.BitrateLabel.Size = New System.Drawing.Size(51, 13)
        Me.BitrateLabel.TabIndex = 15
        Me.BitrateLabel.Text = "192 kbps"
        '
        'EncoderQualityCombobox
        '
        Me.EncoderQualityCombobox.FormattingEnabled = True
        Me.EncoderQualityCombobox.Items.AddRange(New Object() {"Fast", "Standard", "High"})
        Me.EncoderQualityCombobox.Location = New System.Drawing.Point(20, 33)
        Me.EncoderQualityCombobox.Name = "EncoderQualityCombobox"
        Me.EncoderQualityCombobox.Size = New System.Drawing.Size(156, 21)
        Me.EncoderQualityCombobox.TabIndex = 0
        '
        'QualityGroupBox
        '
        Me.QualityGroupBox.Controls.Add(Me.VBRQualityCombobox)
        Me.QualityGroupBox.Controls.Add(Me.Label5)
        Me.QualityGroupBox.Controls.Add(Me.QualityLabel)
        Me.QualityGroupBox.Controls.Add(Me.Label4)
        Me.QualityGroupBox.Controls.Add(Me.QualityTrackbar)
        Me.QualityGroupBox.Location = New System.Drawing.Point(23, 212)
        Me.QualityGroupBox.Name = "QualityGroupBox"
        Me.QualityGroupBox.Size = New System.Drawing.Size(398, 101)
        Me.QualityGroupBox.TabIndex = 16
        Me.QualityGroupBox.TabStop = False
        Me.QualityGroupBox.Text = "Quality"
        '
        'QualityLabel
        '
        Me.QualityLabel.AutoSize = True
        Me.QualityLabel.Location = New System.Drawing.Point(314, 70)
        Me.QualityLabel.Name = "QualityLabel"
        Me.QualityLabel.Size = New System.Drawing.Size(30, 13)
        Me.QualityLabel.TabIndex = 15
        Me.QualityLabel.Text = "50 %"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(268, 70)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(42, 13)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "Quality:"
        '
        'QualityTrackbar
        '
        Me.QualityTrackbar.LargeChange = 10
        Me.QualityTrackbar.Location = New System.Drawing.Point(6, 19)
        Me.QualityTrackbar.Maximum = 100
        Me.QualityTrackbar.Minimum = 10
        Me.QualityTrackbar.Name = "QualityTrackbar"
        Me.QualityTrackbar.Size = New System.Drawing.Size(386, 45)
        Me.QualityTrackbar.SmallChange = 10
        Me.QualityTrackbar.TabIndex = 13
        Me.QualityTrackbar.TickFrequency = 10
        Me.QualityTrackbar.Value = 50
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(15, 71)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(109, 13)
        Me.Label5.TabIndex = 16
        Me.Label5.Text = "Variable bitrate mode:"
        '
        'VBRQualityCombobox
        '
        Me.VBRQualityCombobox.FormattingEnabled = True
        Me.VBRQualityCombobox.Items.AddRange(New Object() {"Standard", "Fast"})
        Me.VBRQualityCombobox.Location = New System.Drawing.Point(130, 67)
        Me.VBRQualityCombobox.Name = "VBRQualityCombobox"
        Me.VBRQualityCombobox.Size = New System.Drawing.Size(120, 21)
        Me.VBRQualityCombobox.TabIndex = 1
        '
        'LameVersionLabel
        '
        Me.LameVersionLabel.AutoSize = True
        Me.LameVersionLabel.Location = New System.Drawing.Point(27, 326)
        Me.LameVersionLabel.Name = "LameVersionLabel"
        Me.LameVersionLabel.Size = New System.Drawing.Size(57, 13)
        Me.LameVersionLabel.TabIndex = 17
        Me.LameVersionLabel.Text = "Lame 3.00"
        '
        'LameDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(446, 356)
        Me.Controls.Add(Me.LameVersionLabel)
        Me.Controls.Add(Me.QualityGroupBox)
        Me.Controls.Add(Me.BitrateGroupbox)
        Me.Controls.Add(Me.EncoderQualityGroupbox)
        Me.Controls.Add(Me.TargetGroupbox)
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ButtonOK)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "LameDialog"
        Me.ShowIcon = False
        Me.Text = "Lame Configuration Dialog"
        Me.TopMost = True
        Me.TargetGroupbox.ResumeLayout(False)
        Me.TargetGroupbox.PerformLayout()
        Me.EncoderQualityGroupbox.ResumeLayout(False)
        Me.BitrateGroupbox.ResumeLayout(False)
        Me.BitrateGroupbox.PerformLayout()
        CType(Me.BitrateTrackbar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.QualityGroupBox.ResumeLayout(False)
        Me.QualityGroupBox.PerformLayout()
        CType(Me.QualityTrackbar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ButtonOK As Button
    Friend WithEvents ButtonCancel As Button
    Friend WithEvents TargetGroupbox As GroupBox
    Friend WithEvents QualityRadioButton As RadioButton
    Friend WithEvents BitrateRadioButton As RadioButton
    Friend WithEvents EncoderQualityGroupbox As GroupBox
    Friend WithEvents EncoderQualityCombobox As ComboBox
    Friend WithEvents BitrateGroupbox As GroupBox
    Friend WithEvents BitrateLabel As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents BitrateTrackbar As TrackBar
    Friend WithEvents ForceCBR As CheckBox
    Friend WithEvents QualityGroupBox As GroupBox
    Friend WithEvents VBRQualityCombobox As ComboBox
    Friend WithEvents Label5 As Label
    Friend WithEvents QualityLabel As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents QualityTrackbar As TrackBar
    Friend WithEvents LameVersionLabel As Label
End Class
