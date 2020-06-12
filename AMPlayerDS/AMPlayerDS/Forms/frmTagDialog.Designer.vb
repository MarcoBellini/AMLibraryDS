<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTagDialog
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
        Me.ListBoxTags = New System.Windows.Forms.ListBox()
        Me.SuspendLayout()
        '
        'ListBoxTags
        '
        Me.ListBoxTags.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.ListBoxTags.Font = New System.Drawing.Font("Arial Narrow", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListBoxTags.FormattingEnabled = True
        Me.ListBoxTags.HorizontalScrollbar = True
        Me.ListBoxTags.ItemHeight = 15
        Me.ListBoxTags.Location = New System.Drawing.Point(12, 12)
        Me.ListBoxTags.Name = "ListBoxTags"
        Me.ListBoxTags.Size = New System.Drawing.Size(473, 244)
        Me.ListBoxTags.TabIndex = 0
        '
        'frmTagDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(497, 262)
        Me.Controls.Add(Me.ListBoxTags)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmTagDialog"
        Me.ShowIcon = False
        Me.Text = "Current Track tags"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ListBoxTags As ListBox
End Class
