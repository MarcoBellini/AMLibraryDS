<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class CustomListViewControl
    Inherits System.Windows.Forms.UserControl

    'UserControl esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.VScrollBar = New System.Windows.Forms.VScrollBar()
        Me.PictureBox_List = New System.Windows.Forms.PictureBox()
        Me.PictureBox_Header = New System.Windows.Forms.PictureBox()
        CType(Me.PictureBox_List, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox_Header, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'VScrollBar
        '
        Me.VScrollBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.VScrollBar.LargeChange = 1
        Me.VScrollBar.Location = New System.Drawing.Point(345, 0)
        Me.VScrollBar.Name = "VScrollBar"
        Me.VScrollBar.Size = New System.Drawing.Size(17, 267)
        Me.VScrollBar.TabIndex = 7
        '
        'PictureBox_List
        '
        Me.PictureBox_List.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBox_List.BackColor = System.Drawing.SystemColors.Control
        Me.PictureBox_List.Location = New System.Drawing.Point(0, 23)
        Me.PictureBox_List.Name = "PictureBox_List"
        Me.PictureBox_List.Size = New System.Drawing.Size(346, 244)
        Me.PictureBox_List.TabIndex = 5
        Me.PictureBox_List.TabStop = False
        '
        'PictureBox_Header
        '
        Me.PictureBox_Header.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBox_Header.BackColor = System.Drawing.SystemColors.Control
        Me.PictureBox_Header.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox_Header.Name = "PictureBox_Header"
        Me.PictureBox_Header.Size = New System.Drawing.Size(346, 24)
        Me.PictureBox_Header.TabIndex = 4
        Me.PictureBox_Header.TabStop = False
        '
        'CustomListViewControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.Controls.Add(Me.VScrollBar)
        Me.Controls.Add(Me.PictureBox_List)
        Me.Controls.Add(Me.PictureBox_Header)
        Me.Name = "CustomListViewControl"
        Me.Size = New System.Drawing.Size(362, 267)
        CType(Me.PictureBox_List, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox_Header, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents VScrollBar As VScrollBar
    Friend WithEvents PictureBox_List As PictureBox
    Friend WithEvents PictureBox_Header As PictureBox
End Class
