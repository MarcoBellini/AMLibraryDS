<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DirectSoundConfiguration
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.button_close = New System.Windows.Forms.Button()
        Me.DeviceListControl = New System.Windows.Forms.ComboBox()
        Me.BufferTrackBar = New System.Windows.Forms.TrackBar()
        Me.PreBufferTrackBar = New System.Windows.Forms.TrackBar()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.BufferSizeLabel = New System.Windows.Forms.Label()
        Me.PreBufferSizeLabel = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.CheckBoxFading = New System.Windows.Forms.CheckBox()
        Me.Label6 = New System.Windows.Forms.Label()
        CType(Me.BufferTrackBar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PreBufferTrackBar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(16, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(196, 25)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "DirectSound Plugin"
        '
        'button_close
        '
        Me.button_close.Location = New System.Drawing.Point(326, 196)
        Me.button_close.Name = "button_close"
        Me.button_close.Size = New System.Drawing.Size(75, 23)
        Me.button_close.TabIndex = 4
        Me.button_close.Text = "OK"
        Me.button_close.UseVisualStyleBackColor = True
        '
        'DeviceListControl
        '
        Me.DeviceListControl.FormattingEnabled = True
        Me.DeviceListControl.Location = New System.Drawing.Point(134, 51)
        Me.DeviceListControl.Name = "DeviceListControl"
        Me.DeviceListControl.Size = New System.Drawing.Size(242, 21)
        Me.DeviceListControl.TabIndex = 5
        '
        'BufferTrackBar
        '
        Me.BufferTrackBar.LargeChange = 50
        Me.BufferTrackBar.Location = New System.Drawing.Point(134, 78)
        Me.BufferTrackBar.Maximum = 4000
        Me.BufferTrackBar.Minimum = 800
        Me.BufferTrackBar.Name = "BufferTrackBar"
        Me.BufferTrackBar.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.BufferTrackBar.Size = New System.Drawing.Size(242, 45)
        Me.BufferTrackBar.SmallChange = 10
        Me.BufferTrackBar.TabIndex = 6
        Me.BufferTrackBar.TickFrequency = 200
        Me.BufferTrackBar.Value = 2000
        '
        'PreBufferTrackBar
        '
        Me.PreBufferTrackBar.LargeChange = 50
        Me.PreBufferTrackBar.Location = New System.Drawing.Point(134, 129)
        Me.PreBufferTrackBar.Maximum = 400
        Me.PreBufferTrackBar.Minimum = 100
        Me.PreBufferTrackBar.Name = "PreBufferTrackBar"
        Me.PreBufferTrackBar.Size = New System.Drawing.Size(242, 45)
        Me.PreBufferTrackBar.SmallChange = 10
        Me.PreBufferTrackBar.TabIndex = 6
        Me.PreBufferTrackBar.TickFrequency = 25
        Me.PreBufferTrackBar.Value = 200
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(36, 54)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(77, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Output device:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(36, 91)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(59, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Buffer size:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(36, 129)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(75, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "PreBuffer size:"
        '
        'BufferSizeLabel
        '
        Me.BufferSizeLabel.AutoSize = True
        Me.BufferSizeLabel.Location = New System.Drawing.Point(36, 104)
        Me.BufferSizeLabel.Name = "BufferSizeLabel"
        Me.BufferSizeLabel.Size = New System.Drawing.Size(31, 13)
        Me.BufferSizeLabel.TabIndex = 10
        Me.BufferSizeLabel.Text = "2000"
        '
        'PreBufferSizeLabel
        '
        Me.PreBufferSizeLabel.AutoSize = True
        Me.PreBufferSizeLabel.Location = New System.Drawing.Point(36, 142)
        Me.PreBufferSizeLabel.Name = "PreBufferSizeLabel"
        Me.PreBufferSizeLabel.Size = New System.Drawing.Size(25, 13)
        Me.PreBufferSizeLabel.TabIndex = 11
        Me.PreBufferSizeLabel.Text = "200"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(36, 172)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(78, 13)
        Me.Label5.TabIndex = 12
        Me.Label5.Text = "Enable Fading:"
        '
        'CheckBoxFading
        '
        Me.CheckBoxFading.AutoSize = True
        Me.CheckBoxFading.Location = New System.Drawing.Point(134, 172)
        Me.CheckBoxFading.Name = "CheckBoxFading"
        Me.CheckBoxFading.Size = New System.Drawing.Size(59, 17)
        Me.CheckBoxFading.TabIndex = 13
        Me.CheckBoxFading.Text = "Enable"
        Me.CheckBoxFading.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.ForeColor = System.Drawing.SystemColors.GrayText
        Me.Label6.Location = New System.Drawing.Point(36, 206)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(197, 13)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "Nb: to apply this setting restart AMPlayer"
        '
        'DirectSoundConfiguration
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(426, 227)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.CheckBoxFading)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.PreBufferSizeLabel)
        Me.Controls.Add(Me.BufferSizeLabel)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.PreBufferTrackBar)
        Me.Controls.Add(Me.BufferTrackBar)
        Me.Controls.Add(Me.DeviceListControl)
        Me.Controls.Add(Me.button_close)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DirectSoundConfiguration"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "DirectSound Plugin Settings Dialog"
        CType(Me.BufferTrackBar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PreBufferTrackBar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents button_close As System.Windows.Forms.Button
    Friend WithEvents DeviceListControl As ComboBox
    Friend WithEvents BufferTrackBar As TrackBar
    Friend WithEvents PreBufferTrackBar As TrackBar
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents BufferSizeLabel As Label
    Friend WithEvents PreBufferSizeLabel As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents CheckBoxFading As CheckBox
    Friend WithEvents Label6 As Label
End Class
