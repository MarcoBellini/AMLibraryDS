Imports System.Threading
Public Class SafeFileDialog

    Private Class ThreadData
        Public Multiselect As Boolean
        Public Filter As String
        Public Type As DialogType
        Public Result() As String
    End Class

    Private Enum DialogType
        SelectFolderDialog
        OpenFileDialog
        SaveFileDialog
    End Enum

    Private FileOpenDialog As OpenFileDialog
    Private FileSaveDialog As SaveFileDialog
    Private FolderSelectDialog As FolderBrowserDialog

    Private DialogThread As Thread


    Public Function OpenSingleFile(ByVal Filter As String) As String
        Dim Data As New ThreadData
        Dim result As String = Nothing

        ' Create new thread
        DialogThread = New Thread(AddressOf DialogThreadProc)
        DialogThread.IsBackground = True
        DialogThread.Name = "File/Folder Dialog Thread"
        DialogThread.SetApartmentState(ApartmentState.STA)

        ' Set data values
        Data.Filter = Filter
        Data.Multiselect = False
        Data.Type = DialogType.OpenFileDialog

        ' Start thread (Open Dialog in STA thread)
        DialogThread.Start(Data)

        ' Wait until dialog is closed
        DialogThread.Join()

        ' Check result
        If (Data.Result IsNot Nothing) Then
            If Data.Result.Length > 0 Then
                result = Data.Result(0)
            End If
        End If

        ' Free resources
        Data = Nothing
        DialogThread = Nothing

        Return result
    End Function

    Public Function OpenMultipleFiles(ByVal Filter As String) As String()
        Dim Data As New ThreadData
        Dim result() As String = Nothing

        ' Create new thread
        DialogThread = New Thread(AddressOf DialogThreadProc)
        DialogThread.IsBackground = True
        DialogThread.Name = "File/Folder Dialog Thread"
        DialogThread.SetApartmentState(ApartmentState.STA)

        ' Set data values
        Data.Filter = Filter
        Data.Multiselect = True
        Data.Type = DialogType.OpenFileDialog

        ' Start thread (Open Dialog in STA thread)
        DialogThread.Start(Data)

        ' Wait until dialog is closed
        DialogThread.Join()

        ' Check result
        If (Data.Result IsNot Nothing) Then
            If Data.Result.Length > 0 Then
                result = Data.Result
            End If
        End If

        ' Free resources
        Data = Nothing
        DialogThread = Nothing

        Return result
    End Function

    Public Function SaveSingleFile(ByVal Filter As String) As String
        Dim Data As New ThreadData
        Dim result As String = ""

        ' Create new thread
        DialogThread = New Thread(AddressOf DialogThreadProc)
        DialogThread.IsBackground = True
        DialogThread.Name = "File/Folder Dialog Thread"
        DialogThread.SetApartmentState(ApartmentState.STA)

        ' Set data values
        Data.Filter = Filter
        Data.Multiselect = False
        Data.Type = DialogType.SaveFileDialog

        ' Start thread (Open Dialog in STA thread)
        DialogThread.Start(Data)

        ' Wait until dialog is closed
        DialogThread.Join()

        ' Check result
        If (Data.Result IsNot Nothing) Then
            If Data.Result.Length > 0 Then
                result = Data.Result(0)
            End If
        End If

        ' Free resources
        Data = Nothing
        DialogThread = Nothing

        Return result
    End Function

    Public Function SelectFolder() As String
        Dim Data As New ThreadData
        Dim result As String = ""

        ' Create new thread
        DialogThread = New Thread(AddressOf DialogThreadProc)
        DialogThread.IsBackground = True
        DialogThread.Name = "File/Folder Dialog Thread"
        DialogThread.SetApartmentState(ApartmentState.STA)

        ' Set data values
        Data.Filter = ""
        Data.Multiselect = False
        Data.Type = DialogType.SelectFolderDialog

        ' Start thread (Open Dialog in STA thread)
        DialogThread.Start(Data)

        ' Wait until dialog is closed
        DialogThread.Join()

        ' Check result
        If (Data.Result IsNot Nothing) Then
            If Data.Result.Length > 0 Then
                result = Data.Result(0)
            End If
        End If

        ' Free resources
        Data = Nothing
        DialogThread = Nothing

        Return result
    End Function

    Private Sub DialogThreadProc(ByVal type As Object)
        Dim CurrentData As ThreadData = DirectCast(type, ThreadData)

        ' Switch to correct Dialog
        Select Case CurrentData.Type
            Case DialogType.OpenFileDialog
                FileOpenDialog = New OpenFileDialog
                FileOpenDialog.Multiselect = CurrentData.Multiselect
                FileOpenDialog.Filter = CurrentData.Filter

                If FileOpenDialog.ShowDialog() = DialogResult.OK Then
                    CurrentData.Result = FileOpenDialog.FileNames
                End If

                FileOpenDialog = Nothing
            Case DialogType.SelectFolderDialog
                FolderSelectDialog = New FolderBrowserDialog

                If FolderSelectDialog.ShowDialog() = DialogResult.OK Then
                    ReDim CurrentData.Result(1)
                    CurrentData.Result(0) = FolderSelectDialog.SelectedPath
                End If

                FolderSelectDialog = Nothing
            Case DialogType.SaveFileDialog
                FileSaveDialog = New SaveFileDialog
                FileSaveDialog.Filter = CurrentData.Filter

                If FileSaveDialog.ShowDialog() = DialogResult.OK Then
                    ReDim CurrentData.Result(1)
                    CurrentData.Result(0) = FileSaveDialog.FileName
                End If

                FileSaveDialog = Nothing
        End Select

    End Sub

End Class
