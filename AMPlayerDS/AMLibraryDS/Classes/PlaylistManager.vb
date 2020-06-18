Public Class PlaylistManager
    Implements IDisposable

    Public Enum PlaylistMode
        Read
        Write
    End Enum

    Private disposedValue As Boolean
    Private bIsOpen As Boolean = False
    Private PlaylistReaderList As New List(Of IPlaylistReader)
    Private PlaylistWriterList As New List(Of IPlaylistWriter)
    Private Input As IPlaylistReader
    Private Output As IPlaylistWriter
    Private CurrentMode As PlaylistMode

    Public Sub New()

        ' Add readers
        PlaylistReaderList.Add(New ReaderM3U)

        ' Add writers
        PlaylistWriterList.Add(New WriterM3U)
    End Sub

    Public ReadOnly Property GetPlaylistReaderExtensions() As String
        Get
            Dim Extensions As New Text.StringBuilder
            For i As Integer = 0 To PlaylistReaderList.Count - 1

                If (i + 1) < PlaylistReaderList.Count Then
                    Extensions.Append("*" & PlaylistReaderList(i).Extension & ";")
                Else
                    Extensions.Append("*" & PlaylistReaderList(i).Extension)
                End If

            Next

            Return Extensions.ToString
        End Get
    End Property
    Public ReadOnly Property GetPlaylistWriterExtensions() As String
        Get
            Dim Extensions As New Text.StringBuilder
            For i As Integer = 0 To PlaylistWriterList.Count - 1

                If (i + 1) < PlaylistWriterList.Count Then
                    Extensions.Append("*" & PlaylistWriterList(i).Extension & ";")
                Else
                    Extensions.Append("*" & PlaylistWriterList(i).Extension)
                End If

            Next

            Return Extensions.ToString
        End Get
    End Property

    Public Function GetNeededReader(ByVal path As String) As Boolean

        ' Scan all extension in all installed decoders
        For i As Integer = 0 To PlaylistReaderList.Count - 1


            If String.Equals(IO.Path.GetExtension(path).ToLower,
                             PlaylistReaderList(i).Extension.ToLower) = True Then

                'We Found Desidered Decoder
                Input = PlaylistReaderList(i)

                Return True
            End If


        Next

        Return False
    End Function

    Public Function GetNeededWriter(ByVal path As String) As Boolean

        ' Scan all extension in all installed decoders
        For i As Integer = 0 To PlaylistWriterList.Count - 1


            If String.Equals(IO.Path.GetExtension(path).ToLower,
                             PlaylistWriterList(i).Extension.ToLower) = True Then

                'We Found Desidered Decoder
                Output = PlaylistWriterList(i)

                Return True
            End If


        Next

        Return False
    End Function

    Public Function Open(ByVal Path As String, ByVal Mode As PlaylistMode) As Boolean
        Dim bResult As Boolean = False

        ' Check if a file is already open
        If bIsOpen = True Then
            Close()
        End If

        ' Select read mode or write mode
        Select Case Mode
            Case PlaylistMode.Read

                ' Check if there is a valid reader
                If GetNeededReader(Path) = True Then
                    If Input.Open(Path) = True Then
                        CurrentMode = PlaylistMode.Read
                        bResult = True
                        bIsOpen = True
                    Else
                        MsgBox("Cannot open this playlist file")
                    End If
                Else
                    MsgBox("Playlist format not supported.")
                End If



            Case PlaylistMode.Write

                ' Check if there is a valid reader
                If GetNeededWriter(Path) = True Then
                    If Output.Create(Path) = True Then
                        CurrentMode = PlaylistMode.Write
                        bResult = True
                        bIsOpen = True
                    Else
                        MsgBox("Cannot open this playlist file")
                    End If
                Else
                    MsgBox("Playlist format not supported.")
                End If

        End Select

        Return bResult
    End Function

    Public Sub Close()
        If bIsOpen = True Then
            Select Case CurrentMode
                Case PlaylistMode.Read
                    Input.Close()
                    Input = Nothing
                    bIsOpen = False
                Case PlaylistMode.Write
                    Output.Close()
                    Output = Nothing
                    bIsOpen = False
            End Select
        End If
    End Sub

    Public ReadOnly Property ReadCount() As Long
        Get
            If CurrentMode = PlaylistMode.Read Then
                If bIsOpen = True Then
                    Return Input.Count
                End If
            End If

            Return 0
        End Get
    End Property

    Public Function ReadIndex(ByVal Index As Integer) As StreamInformations
        If CurrentMode = PlaylistMode.Read Then
            If bIsOpen = True Then
                Return Input.ReadInformations(Index)
            End If
        End If

        Return Nothing
    End Function

    Public Function WriteLine(ByRef Info As StreamInformations) As Boolean
        If CurrentMode = PlaylistMode.Write Then
            If bIsOpen = True Then
                Return Output.WriteInformations(Info)
            End If
        End If

        Return False
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then

                ' Free resources
                If bIsOpen = True Then
                    Close()
                End If

                If PlaylistWriterList IsNot Nothing Then
                    PlaylistReaderList.Clear()
                    PlaylistReaderList = Nothing
                End If


                If PlaylistWriterList IsNot Nothing Then
                    PlaylistWriterList.Clear()
                    PlaylistWriterList = Nothing
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
