Imports System.IO

' Follow the Wikipedia specs: https://en.wikipedia.org/wiki/M3U
Public Class WriterM3U
    Implements IPlaylistWriter

    Private EXTENDED_M3U_TAG As String = "#EXTM3U"
    Private EXTENDED_M3U_INFO_TAG As String = "#EXTINF:{0},{1}"

    Private disposedValue As Boolean
    Private bIsOpen As Boolean = False

    Private TextStream As StreamWriter

    Public ReadOnly Property Extension As String Implements IPlaylistWriter.Extension
        Get
            Return ".m3u"
        End Get
    End Property

    Public Sub Close() Implements IPlaylistWriter.Close
        If bIsOpen = True Then

            ' Close file and free resources
            TextStream.Close()
            TextStream.Dispose()
            TextStream = Nothing
            bIsOpen = False
        End If
    End Sub

    Public Function Create(path As String) As Boolean Implements IPlaylistWriter.Create

        ' Check if there is an open stream
        If bIsOpen = True Then
            Close()
        End If

        Try
            ' Try to open new write stream (Overwrite existing file)
            TextStream = New StreamWriter(File.Create(path, IO.FileAccess.Write), Text.Encoding.Default)

            ' Check for a valid object
            If TextStream Is Nothing Then Return False

            TextStream.WriteLine(EXTENDED_M3U_TAG)

            bIsOpen = True
            Return True
        Catch ex As Exception
            MsgBox("Cannot write M3U Playlist: " & ex.Message)
            Return False
        End Try

    End Function

    Public Function WriteInformations(ByRef Info As StreamInformations) As Boolean Implements IPlaylistWriter.WriteInformations
        Dim DurationString, ArtistTitleString As String

        If bIsOpen = False Then Return False

        ' Converto from milliseconds to seconds
        If Info.DurationInMs <> 0 Then
            DurationString = (Info.DurationInMs \ 1000).ToString
        Else
            ' Estimade duration of mp3 based on file size
            DurationString = 0
        End If

        ' If there are valid ID3 informations use it, otherwise use file name
        If (Info.Artist <> "") And (Info.Title <> "") Then
            ArtistTitleString = Info.Artist & " - " & Info.Title
        Else
            ArtistTitleString = Info.FileName
        End If

        ' Write Extended info + File Path
        TextStream.WriteLine(String.Format(EXTENDED_M3U_INFO_TAG, DurationString, ArtistTitleString))
        TextStream.WriteLine(Info.FileLocation)

        Return True
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then

                ' Check if there is an open stream
                If bIsOpen = True Then
                    Close()
                End If

            End If
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
