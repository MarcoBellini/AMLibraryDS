<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class VolumePanControl
    Inherits System.Windows.Forms.UserControl

    'UserControl esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
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
        Me.VolumeLabel = New System.Windows.Forms.Label()
        Me.PanLabel = New System.Windows.Forms.Label()
        Me.TrackBarVolume = New System.Windows.Forms.TrackBar()
        Me.TrackBarPan = New System.Windows.Forms.TrackBar()
        Me.CloseButton = New System.Windows.Forms.Button()
        Me.OutputSettingsButton = New System.Windows.Forms.Button()
        CType(Me.TrackBarVolume, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TrackBarPan, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'VolumeLabel
        '
        Me.VolumeLabel.AutoSize = True
        Me.VolumeLabel.Location = New System.Drawing.Point(19, 22)
        Me.VolumeLabel.Name = "VolumeLabel"
        Me.VolumeLabel.Size = New System.Drawing.Size(45, 13)
        Me.VolumeLabel.TabIndex = 0
        Me.VolumeLabel.Text = "Volume:"
        '
        'PanLabel
        '
        Me.PanLabel.AutoSize = True
        Me.PanLabel.Location = New System.Drawing.Point(19, 86)
        Me.PanLabel.Name = "PanLabel"
        Me.PanLabel.Size = New System.Drawing.Size(29, 13)
        Me.PanLabel.TabIndex = 1
        Me.PanLabel.Text = "Pan:"
        '
        'TrackBarVolume
        '
        Me.TrackBarVolume.Location = New System.Drawing.Point(22, 38)
        Me.TrackBarVolume.Maximum = 100
        Me.TrackBarVolume.Name = "TrackBarVolume"
        Me.TrackBarVolume.Size = New System.Drawing.Size(213, 45)
        Me.TrackBarVolume.TabIndex = 2
        Me.TrackBarVolume.TickFrequency = 10
        Me.TrackBarVolume.Value = 100
        '
        'TrackBarPan
        '
        Me.TrackBarPan.Location = New System.Drawing.Point(22, 112)
        Me.TrackBarPan.Maximum = 100
        Me.TrackBarPan.Minimum = -100
        Me.TrackBarPan.Name = "TrackBarPan"
        Me.TrackBarPan.Size = New System.Drawing.Size(213, 45)
        Me.TrackBarPan.TabIndex = 3
        Me.TrackBarPan.TickFrequency = 20
        '
        'CloseButton
        '
        Me.CloseButton.Location = New System.Drawing.Point(225, 3)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(24, 21)
        Me.CloseButton.TabIndex = 4
        Me.CloseButton.Text = "X"
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'OutputSettingsButton
        '
        Me.OutputSettingsButton.Location = New System.Drawing.Point(62, 156)
        Me.OutputSettingsButton.Name = "OutputSettingsButton"
        Me.OutputSettingsButton.Size = New System.Drawing.Size(120, 30)
        Me.OutputSettingsButton.TabIndex = 5
        Me.OutputSettingsButton.Text = "Output Configuration"
        Me.OutputSettingsButton.UseVisualStyleBackColor = True
        '
        'VolumePanControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.OutputSettingsButton)
        Me.Controls.Add(Me.CloseButton)
        Me.Controls.Add(Me.TrackBarPan)
        Me.Controls.Add(Me.TrackBarVolume)
        Me.Controls.Add(Me.PanLabel)
        Me.Controls.Add(Me.VolumeLabel)
        Me.Name = "VolumePanControl"
        Me.Size = New System.Drawing.Size(252, 189)
        CType(Me.TrackBarVolume, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TrackBarPan, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents VolumeLabel As Label
    Friend WithEvents PanLabel As Label
    Friend WithEvents TrackBarVolume As TrackBar
    Friend WithEvents TrackBarPan As TrackBar
    Friend WithEvents CloseButton As Button
    Friend WithEvents OutputSettingsButton As Button
End Class
