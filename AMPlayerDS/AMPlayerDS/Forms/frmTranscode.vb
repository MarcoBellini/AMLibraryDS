Public Class frmTranscode

    Private Class ThreadData
        Public OutputFolder As String
        Public OutputChannels As Short
        Public OutputSamplerate As Integer
        Public OutputFormat As String
        Public EnableResample As Boolean
        Public Files() As String
    End Class

    Private OutputFolder As String = ""
    Private bTranscodeInProgress As Boolean = False
    Private Encoder As EncoderManager

    Private Sub CloseButton_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
        Close()
    End Sub

    Private Sub frmTranscode_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        ' Free Resources
        Encoder.Dispose()
        Encoder = Nothing
    End Sub
    Private Sub frmTranscode_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim ListOfEncoders As List(Of String)

        Encoder = New EncoderManager

        ListOfEncoders = Encoder.EncodersNameList

        ' Add supported encoders to the combobox
        For i As Integer = 0 To ListOfEncoders.Count - 1
            OutputFormatCombobox.Items.Add(ListOfEncoders(i))
        Next

        ' Add supported Samplerate
        For Each i As Integer In [Enum].GetValues(GetType(SamplerateConverter.Samplerate))
            OutputSamplerateCombobox.Items.Add(i)
        Next
    End Sub

    Private Sub ConfigureOutputButton_Click(sender As Object, e As EventArgs) Handles ConfigureOutputButton.Click
        ' Configure encoder settings
        If OutputFormatCombobox.SelectedItem <> "" Then
            Dim RequestedEncoder As ISoundEncoder

            RequestedEncoder = Encoder.GetEncoderByEncoderName(OutputFormatCombobox.SelectedItem)

            If RequestedEncoder IsNot Nothing Then
                RequestedEncoder.ShowConfiguration()
            End If
        Else
            MsgBox("Select an output to configure")
        End If
    End Sub

    Private Sub AddButton_Click(sender As Object, e As EventArgs) Handles AddButton.Click
        Dim SafeDialog As New SafeFileDialog
        Dim result() As String

        ' Open file dialog in safe STA thread
        result = SafeDialog.OpenMultipleFiles("Supported Files|" & Encoder.GetInputSupportedExtension)

        ' If result is valid, add files to listbox
        If result IsNot Nothing Then
            For i As Integer = 0 To result.Length - 1
                ListboxFiles.Items.Add(result(i))
            Next

        End If
    End Sub

    Private Sub RemoveButton_Click(sender As Object, e As EventArgs) Handles RemoveButton.Click
        ' Remove selected items in the listbox
        For i As Integer = 0 To ListboxFiles.SelectedItems.Count - 1
            ListboxFiles.Items.RemoveAt(ListboxFiles.SelectedIndex)
        Next
    End Sub

    Private Sub SelectFolderButton_Click(sender As Object, e As EventArgs) Handles SelectFolderButton.Click
        Dim SafeDialog As New SafeFileDialog
        Dim Result As String

        ' Open folder dialog in STA thread
        Result = SafeDialog.SelectFolder()

        If Result <> "" Then
            ' Update path
            OutputFolder = Result
            FolderPathLabel.Text = "\..." & Result.Substring(Result.LastIndexOf("\"c))
        End If
    End Sub

    Private Sub ProcessButton_Click(sender As Object, e As EventArgs) Handles ProcessButton.Click
        If bTranscodeInProgress = False Then
            StartTranscode()
        Else
            AbortTranscode()
        End If
    End Sub

    Public Sub EnableUI()
        ' Enable all UI Elements
        ListboxFiles.Enabled = True
        AddButton.Enabled = True
        RemoveButton.Enabled = True
        EnableResampleCheckbox.Enabled = True
        StereoRadioButton.Enabled = True
        MonoRadioButton.Enabled = True
        OutputSamplerateCombobox.Enabled = True
        SelectFolderButton.Enabled = True
        OutputFormatCombobox.Enabled = True
        ConfigureOutputButton.Enabled = True
        CloseButton.Enabled = True
        ProcessButton.Text = "Process"
    End Sub

    Public Sub DisableUI()
        ' Disable all UI Elements
        ListboxFiles.Enabled = False
        AddButton.Enabled = False
        RemoveButton.Enabled = False
        EnableResampleCheckbox.Enabled = False
        StereoRadioButton.Enabled = False
        MonoRadioButton.Enabled = False
        OutputSamplerateCombobox.Enabled = False
        SelectFolderButton.Enabled = False
        OutputFormatCombobox.Enabled = False
        ConfigureOutputButton.Enabled = False
        CloseButton.Enabled = False
        ProcessButton.Text = "Abort"
    End Sub

    Public Sub StartTranscode()
        Dim Data As New ThreadData

        ' Check if Samplerate combobox has valid value
        If EnableResampleCheckbox.Checked = True Then

            If OutputSamplerateCombobox.SelectedItem = Nothing Then
                MsgBox("Select valid Output Samplerate")
                Exit Sub
            End If
        End If

        ' Check output folder 
        If OutputFolder = "" Then
            MsgBox("Select valid Output Folder")
            Exit Sub
        End If

        ' Check Output format
        If OutputFormatCombobox.SelectedItem = Nothing Then
            MsgBox("Select valid Output Format")
            Exit Sub
        End If

        ' Check if there are files in the listbox
        If ListboxFiles.Items.Count = 0 Then
            MsgBox("No file in the Listbox, add file before start transcode")
            Exit Sub
        End If

        ' Start transcode process
        bTranscodeInProgress = True
        DisableUI()

        ' Fill data for thread
        Data.EnableResample = EnableResampleCheckbox.Checked
        Data.OutputChannels = IIf(StereoRadioButton.Checked, 2, 1)
        Data.OutputFolder = OutputFolder
        Data.OutputFormat = OutputFormatCombobox.SelectedItem
        Data.OutputSamplerate = OutputSamplerateCombobox.SelectedItem

        ' Resize array
        ReDim Data.Files(ListboxFiles.Items.Count - 1)

        ' Add files
        For i As Integer = 0 To ListboxFiles.Items.Count - 1
            Data.Files(i) = ListboxFiles.Items(i)
        Next

        ' Run Anync thread
        TranscodeThread.RunWorkerAsync(Data)
    End Sub

    Public Sub AbortTranscode()
        ' Abort thread async
        TranscodeThread.CancelAsync()
    End Sub

    Private Sub TranscodeThread_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles TranscodeThread.DoWork
        Dim Data As ThreadData = DirectCast(e.Argument, ThreadData)
        Dim OutputExtension, OutputFilePath As String
        Dim Index As Integer
        Dim StreamInfo As StreamInformations = Nothing

        ' Get proper file extension from the selected encoder
        OutputExtension = Encoder.GetExtensionByEncoderName(Data.OutputFormat)
        Index = 0

        ' Report Initialization of encoding
        TranscodeThread.ReportProgress(0)

        Do
            ' Calculate output file path
            OutputFilePath = Data.OutputFolder &
                             IO.Path.DirectorySeparatorChar &
                             IO.Path.GetFileNameWithoutExtension(Data.Files(Index)) &
                             OutputExtension


            ' If Resaple checkbox is enable, resample
            ' Otherwise use current file waveformat
            If Data.EnableResample Then
                StreamInfo = New StreamInformations
                StreamInfo.Samplerate = Data.OutputSamplerate
                StreamInfo.Channels = Data.OutputChannels
                StreamInfo.BitsPerSample = 16
                StreamInfo.BlockAlign = CShort(StreamInfo.Channels * StreamInfo.BitsPerSample / 8)
                StreamInfo.AvgBytesPerSec = (StreamInfo.BlockAlign * StreamInfo.Samplerate)
            End If

            ' Try to create new file and transcode
            If Encoder.OpenTranscode(Data.Files(Index), OutputFilePath, StreamInfo) Then
                Encoder.TrancodeFileUntilEof()
                Encoder.CloseTranscode()
            End If

            ' Report progress 
            Index += 1
            TranscodeThread.ReportProgress((Index / Data.Files.Length) * 100)

            ' Free resources
            If StreamInfo IsNot Nothing Then
                StreamInfo = Nothing
            End If
        Loop While (TranscodeThread.CancellationPending = False) _
                   And (Index < Data.Files.Length) ' Loop while end, or abort 
    End Sub

    Private Sub TranscodeThread_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles TranscodeThread.ProgressChanged
        ' Update Progressbar
        TranscodeProgressBar.Value = e.ProgressPercentage
        ProgressLabel.Text = "Encoding " & e.ProgressPercentage & " %"

        'Force Form Update
        'Refresh()
        'Application.DoEvents()
    End Sub

    Private Sub TranscodeThread_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles TranscodeThread.RunWorkerCompleted
        ' On finish, enable UI
        EnableUI()
        bTranscodeInProgress = False
    End Sub

    Private Sub RipCDButton_Click(sender As Object, e As EventArgs) Handles RipCDButton.Click
        Dim frmCd As New frmOpenCD()

        ' Check if the user decided to open drive
        If frmCd.ShowDialog() = DialogResult.OK Then
            AddCdToList(frmCd.chrSelectedDrive)
        End If

        frmCd.Dispose()
    End Sub

    Private Sub AddCdToList(ByVal drive As Char)
        Dim cdrom As New CDDrive

        ' Open cd
        If cdrom.Open(drive) = True Then
            If cdrom.Refresh() = True Then
                For i As Integer = 1 To cdrom.GetNumAudioTracks

                    ' Add in the list in a format C: - 1.cda
                    ListboxFiles.Items.Add(drive & "-" & i.ToString & ".cda")
                Next
            End If

            ' Free resources
            cdrom.Close()
            cdrom = Nothing
        End If
    End Sub
End Class