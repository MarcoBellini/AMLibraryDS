Imports System.Threading
Public Class SafeFileDialog

    Friend Class ThreadData
        Public Multiselect As Boolean
        Public Filter As String
        Public Type As DialogType
        Public Result() As String
    End Class

    Public Enum DialogType
        FolderDialog
        FileDialog
    End Enum

    Private FileDialog As OpenFileDialog
    Private FolderDialog As FolderBrowserDialog

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
        Data.Type = DialogType.FileDialog

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
        Data.Type = DialogType.FileDialog

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
        Data.Type = DialogType.FolderDialog

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
            Case DialogType.FileDialog
                FileDialog = New OpenFileDialog
                FileDialog.Multiselect = CurrentData.Multiselect
                FileDialog.Filter = CurrentData.Filter

                If FileDialog.ShowDialog() = DialogResult.OK Then
                    CurrentData.Result = FileDialog.FileNames
                End If

                FileDialog = Nothing
            Case DialogType.FolderDialog
                FolderDialog = New FolderBrowserDialog

                If FolderDialog.ShowDialog() = DialogResult.OK Then
                    ReDim CurrentData.Result(1)
                    CurrentData.Result(0) = FolderDialog.SelectedPath
                End If

                FolderDialog = Nothing
        End Select

    End Sub





End Class
