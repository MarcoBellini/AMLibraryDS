<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DSPPhaser
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
        Me.TrackBar_Stages = New System.Windows.Forms.TrackBar()
        Me.TrackBar_DryWet = New System.Windows.Forms.TrackBar()
        Me.TrackBar_LFOFrequency = New System.Windows.Forms.TrackBar()
        Me.TrackBar_LFOStartPhase = New System.Windows.Forms.TrackBar()
        Me.TrackBar_Depth = New System.Windows.Forms.TrackBar()
        Me.TrackBar_Feedback = New System.Windows.Forms.TrackBar()
        Me.Label_Stages = New System.Windows.Forms.Label()
        Me.Label_DryWet = New System.Windows.Forms.Label()
        Me.Label_LFOFrequency = New System.Windows.Forms.Label()
        Me.Label_LFOStartPhase = New System.Windows.Forms.Label()
        Me.Label_Depth = New System.Windows.Forms.Label()
        Me.Label_Feedback = New System.Windows.Forms.Label()
        Me.Button_close = New System.Windows.Forms.Button()
        Me.bEnableCkeckBox = New System.Windows.Forms.CheckBox()
        CType(Me.TrackBar_Stages, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TrackBar_DryWet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TrackBar_LFOFrequency, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TrackBar_LFOStartPhase, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TrackBar_Depth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TrackBar_Feedback, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TrackBar_Stages
        '
        Me.TrackBar_Stages.Location = New System.Drawing.Point(12, 11)
        Me.TrackBar_Stages.Maximum = 23
        Me.TrackBar_Stages.Name = "TrackBar_Stages"
        Me.TrackBar_Stages.Size = New System.Drawing.Size(211, 45)
        Me.TrackBar_Stages.TabIndex = 0
        Me.TrackBar_Stages.TickStyle = System.Windows.Forms.TickStyle.None
        Me.TrackBar_Stages.Value = 2
        '
        'TrackBar_DryWet
        '
        Me.TrackBar_DryWet.Location = New System.Drawing.Point(12, 62)
        Me.TrackBar_DryWet.Maximum = 255
        Me.TrackBar_DryWet.Name = "TrackBar_DryWet"
        Me.TrackBar_DryWet.Size = New System.Drawing.Size(211, 45)
        Me.TrackBar_DryWet.TabIndex = 0
        Me.TrackBar_DryWet.TickStyle = System.Windows.Forms.TickStyle.None
        Me.TrackBar_DryWet.Value = 128
        '
        'TrackBar_LFOFrequency
        '
        Me.TrackBar_LFOFrequency.Location = New System.Drawing.Point(12, 113)
        Me.TrackBar_LFOFrequency.Maximum = 20
        Me.TrackBar_LFOFrequency.Minimum = 1
        Me.TrackBar_LFOFrequency.Name = "TrackBar_LFOFrequency"
        Me.TrackBar_LFOFrequency.Size = New System.Drawing.Size(211, 45)
        Me.TrackBar_LFOFrequency.TabIndex = 0
        Me.TrackBar_LFOFrequency.TickStyle = System.Windows.Forms.TickStyle.None
        Me.TrackBar_LFOFrequency.Value = 1
        '
        'TrackBar_LFOStartPhase
        '
        Me.TrackBar_LFOStartPhase.Location = New System.Drawing.Point(12, 164)
        Me.TrackBar_LFOStartPhase.Maximum = 360
        Me.TrackBar_LFOStartPhase.Name = "TrackBar_LFOStartPhase"
        Me.TrackBar_LFOStartPhase.Size = New System.Drawing.Size(211, 45)
        Me.TrackBar_LFOStartPhase.TabIndex = 0
        Me.TrackBar_LFOStartPhase.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'TrackBar_Depth
        '
        Me.TrackBar_Depth.Location = New System.Drawing.Point(12, 215)
        Me.TrackBar_Depth.Maximum = 255
        Me.TrackBar_Depth.Name = "TrackBar_Depth"
        Me.TrackBar_Depth.Size = New System.Drawing.Size(211, 45)
        Me.TrackBar_Depth.TabIndex = 0
        Me.TrackBar_Depth.TickStyle = System.Windows.Forms.TickStyle.None
        Me.TrackBar_Depth.Value = 100
        '
        'TrackBar_Feedback
        '
        Me.TrackBar_Feedback.Location = New System.Drawing.Point(12, 266)
        Me.TrackBar_Feedback.Maximum = 100
        Me.TrackBar_Feedback.Minimum = -100
        Me.TrackBar_Feedback.Name = "TrackBar_Feedback"
        Me.TrackBar_Feedback.Size = New System.Drawing.Size(211, 45)
        Me.TrackBar_Feedback.TabIndex = 0
        Me.TrackBar_Feedback.TickStyle = System.Windows.Forms.TickStyle.None
        Me.TrackBar_Feedback.Value = 20
        '
        'Label_Stages
        '
        Me.Label_Stages.AutoSize = True
        Me.Label_Stages.Location = New System.Drawing.Point(243, 22)
        Me.Label_Stages.Name = "Label_Stages"
        Me.Label_Stages.Size = New System.Drawing.Size(52, 13)
        Me.Label_Stages.TabIndex = 1
        Me.Label_Stages.Text = "Stages: 2"
        '
        'Label_DryWet
        '
        Me.Label_DryWet.AutoSize = True
        Me.Label_DryWet.Location = New System.Drawing.Point(243, 72)
        Me.Label_DryWet.Name = "Label_DryWet"
        Me.Label_DryWet.Size = New System.Drawing.Size(75, 13)
        Me.Label_DryWet.TabIndex = 1
        Me.Label_DryWet.Text = "Dry/Wet:  128"
        '
        'Label_LFOFrequency
        '
        Me.Label_LFOFrequency.AutoSize = True
        Me.Label_LFOFrequency.Location = New System.Drawing.Point(243, 122)
        Me.Label_LFOFrequency.Name = "Label_LFOFrequency"
        Me.Label_LFOFrequency.Size = New System.Drawing.Size(111, 13)
        Me.Label_LFOFrequency.TabIndex = 1
        Me.Label_LFOFrequency.Text = "LFO Frequency:  1 Hz"
        '
        'Label_LFOStartPhase
        '
        Me.Label_LFOStartPhase.AutoSize = True
        Me.Label_LFOStartPhase.Location = New System.Drawing.Point(243, 173)
        Me.Label_LFOStartPhase.Name = "Label_LFOStartPhase"
        Me.Label_LFOStartPhase.Size = New System.Drawing.Size(100, 13)
        Me.Label_LFOStartPhase.TabIndex = 1
        Me.Label_LFOStartPhase.Text = "LFO Start Phase:  0"
        '
        'Label_Depth
        '
        Me.Label_Depth.AutoSize = True
        Me.Label_Depth.Location = New System.Drawing.Point(243, 225)
        Me.Label_Depth.Name = "Label_Depth"
        Me.Label_Depth.Size = New System.Drawing.Size(63, 13)
        Me.Label_Depth.TabIndex = 1
        Me.Label_Depth.Text = "Depth:  100"
        '
        'Label_Feedback
        '
        Me.Label_Feedback.AutoSize = True
        Me.Label_Feedback.Location = New System.Drawing.Point(243, 277)
        Me.Label_Feedback.Name = "Label_Feedback"
        Me.Label_Feedback.Size = New System.Drawing.Size(84, 13)
        Me.Label_Feedback.TabIndex = 1
        Me.Label_Feedback.Text = "Feedback:  20%"
        '
        'Button_close
        '
        Me.Button_close.Location = New System.Drawing.Point(323, 294)
        Me.Button_close.Name = "Button_close"
        Me.Button_close.Size = New System.Drawing.Size(68, 25)
        Me.Button_close.TabIndex = 2
        Me.Button_close.Text = "Close"
        Me.Button_close.UseVisualStyleBackColor = True
        '
        'bEnableCkeckBox
        '
        Me.bEnableCkeckBox.AutoSize = True
        Me.bEnableCkeckBox.Location = New System.Drawing.Point(12, 294)
        Me.bEnableCkeckBox.Name = "bEnableCkeckBox"
        Me.bEnableCkeckBox.Size = New System.Drawing.Size(59, 17)
        Me.bEnableCkeckBox.TabIndex = 3
        Me.bEnableCkeckBox.Text = "Enable"
        Me.bEnableCkeckBox.UseVisualStyleBackColor = True
        '
        'DSPPhaser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(403, 323)
        Me.Controls.Add(Me.bEnableCkeckBox)
        Me.Controls.Add(Me.Button_close)
        Me.Controls.Add(Me.Label_Feedback)
        Me.Controls.Add(Me.Label_Depth)
        Me.Controls.Add(Me.Label_LFOStartPhase)
        Me.Controls.Add(Me.Label_LFOFrequency)
        Me.Controls.Add(Me.Label_DryWet)
        Me.Controls.Add(Me.Label_Stages)
        Me.Controls.Add(Me.TrackBar_Feedback)
        Me.Controls.Add(Me.TrackBar_Depth)
        Me.Controls.Add(Me.TrackBar_LFOStartPhase)
        Me.Controls.Add(Me.TrackBar_LFOFrequency)
        Me.Controls.Add(Me.TrackBar_DryWet)
        Me.Controls.Add(Me.TrackBar_Stages)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "DSPPhaser"
        Me.Text = "DSPPhaser Configuration"
        CType(Me.TrackBar_Stages, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TrackBar_DryWet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TrackBar_LFOFrequency, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TrackBar_LFOStartPhase, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TrackBar_Depth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TrackBar_Feedback, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents TrackBar_Stages As TrackBar
    Friend WithEvents TrackBar_DryWet As TrackBar
    Friend WithEvents TrackBar_LFOFrequency As TrackBar
    Friend WithEvents TrackBar_LFOStartPhase As TrackBar
    Friend WithEvents TrackBar_Depth As TrackBar
    Friend WithEvents TrackBar_Feedback As TrackBar
    Friend WithEvents Label_Stages As Label
    Friend WithEvents Label_DryWet As Label
    Friend WithEvents Label_LFOFrequency As Label
    Friend WithEvents Label_LFOStartPhase As Label
    Friend WithEvents Label_Depth As Label
    Friend WithEvents Label_Feedback As Label
    Friend WithEvents Button_close As Button
    Friend WithEvents bEnableCkeckBox As CheckBox
End Class
