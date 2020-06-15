Imports System.IO
Imports System.Text.RegularExpressions


' Follows Wikipedia M3U specs: https://en.wikipedia.org/wiki/M3U
' This class opens M3U and Extended M3U (only EXTINF tag)
' i'm not sure this way is correct, but it's here to test
'
' Tested with Winamp and Foobar m3u Playlist exports
Public Class ReaderM3U
    Implements IPlaylistReader

    Private Const DEFAULT_DRIVE As String = "C:"

    ' Find if extended m3u ^[#]\bEXTM3U\b
    Private Const EXTENDED_M3U_PATTERN As String = "^[#]\bEXTM3U\b"

    ' Find if line start without # or space (path of the file)
    Private Const START_WO_HASH_SPACE As String = "^[^#\s]\b[^\n]+\b" '"^[^#\s][ A-Za-z0-9._-]+"

    ' Find if line start with #EXTINF
    Private Const START_W_EXTINF As String = "^[#]\bEXTINF\b"

    ' Find integer number after EXTINF:
    Private Const START_W_EXTINF_DURATION As String = "(?<=EXTINF:)\d+"

    'Find a string after the duration(integer) in EXTINF (?<=\d,).+
    Private Const AFTER_DURATION_STRING As String = "(?<=\d,)\b[^\n]+\b"

    ' Vars
    Private disposedValue As Boolean
    Private bIsExtendedM3U As Boolean = False
    Private bIsOpen As Boolean = False

    Private TextStream As StreamReader
    Private PlaylistFile As String

    ' Use regular expressions
    Private PathRegexMatches As MatchCollection
    Private DurationRegexMatches As MatchCollection
    Private ArtistTitleRegexMatches As MatchCollection

    Public ReadOnly Property Extesion As String Implements IPlaylistReader.Extension
        Get
            Return ".m3u"
        End Get
    End Property

    Public ReadOnly Property Count As Long Implements IPlaylistReader.Count
        Get
            If bIsOpen Then
                Return PathRegexMatches.Count
            Else
                Return 0
            End If
        End Get
    End Property

    Public Sub Close() Implements IPlaylistReader.Close
        If bIsOpen Then

            ' Free all resources
            If TextStream IsNot Nothing Then
                TextStream.Close()
                TextStream.Dispose()
                TextStream = Nothing
            End If

            If PathRegexMatches IsNot Nothing Then
                PathRegexMatches = Nothing
            End If

            If DurationRegexMatches IsNot Nothing Then
                DurationRegexMatches = Nothing
            End If

            If ArtistTitleRegexMatches IsNot Nothing Then
                ArtistTitleRegexMatches = Nothing
            End If

            If PlaylistFile IsNot Nothing Then
                PlaylistFile = Nothing
            End If

            bIsExtendedM3U = False
            bIsOpen = False
        End If
    End Sub

    Public Function Open(path As String) As Boolean Implements IPlaylistReader.Open

        ' Check if the session is already open
        If bIsOpen = True Then Return False

        ' Check if file exist
        If My.Computer.FileSystem.FileExists(path) = False Then Return False

        ' Try to open new stream
        TextStream = New StreamReader(path, System.Text.Encoding.UTF8)

        ' Check if TextStream is valid
        If TextStream Is Nothing Then Return False

        ' Read the entire file to a string stream
        PlaylistFile = TextStream.ReadToEnd()

        ' Find Paths
        PathRegexMatches = Regex.Matches(PlaylistFile, START_WO_HASH_SPACE, RegexOptions.Multiline)

        ' Use regext to find EXTM3U tag
        If Regex.IsMatch(PlaylistFile, EXTENDED_M3U_PATTERN, RegexOptions.Multiline) = True Then

            ' Find Duration (in milliseconds) and "Artist - Title" string
            DurationRegexMatches = Regex.Matches(PlaylistFile, START_W_EXTINF_DURATION, RegexOptions.Multiline)
            ArtistTitleRegexMatches = Regex.Matches(PlaylistFile, AFTER_DURATION_STRING, RegexOptions.Multiline)

            ' Check if found a valid Extended M3U (Duration negative value not supported)
            If (PathRegexMatches.Count <> DurationRegexMatches.Count) Or
                (PathRegexMatches.Count <> ArtistTitleRegexMatches.Count) Then
                bIsExtendedM3U = False
            Else
                bIsExtendedM3U = True
            End If

        Else
            bIsExtendedM3U = False
        End If

        ' Success
        bIsOpen = True
        Return True
    End Function

    Public Function ReadInformations(Index As Long) As StreamInformations Implements IPlaylistReader.ReadInformations
        Dim info As StreamInformations = Nothing
        Dim nSplitIndex, nStringLen As Integer

        If bIsOpen Then

            ' Check if Index is in a valid range
            If (Index >= 0) And (Index <= Count) Then
                info = New StreamInformations()

                ' Check if path is absolute or relative
                If PathRegexMatches(Index).Value.StartsWith("\"c) Then

                    ' Set "C:" as default drive
                    info.FillBasicFileInfo(DEFAULT_DRIVE & PathRegexMatches(Index).Value)
                Else

                    ' Absolute path (nothing to do)
                    info.FillBasicFileInfo(PathRegexMatches(Index).Value)
                End If


                ' Fill Extended values
                If bIsExtendedM3U = True Then

                    ' Convert duration from seconds to milliseconds
                    info.DurationInMs = CDbl(DurationRegexMatches(Index).Value) * 1000


                    ' Split Artist, title to the last "-" char
                    If ArtistTitleRegexMatches(Index).Value.Contains("-") = True Then
                        nSplitIndex = ArtistTitleRegexMatches(Index).Value.LastIndexOf("-")
                        nStringLen = ArtistTitleRegexMatches(Index).Value.Length

                        info.Artist = ArtistTitleRegexMatches(Index).Value.Substring(
                                                        0,
                                                        nSplitIndex - 1)

                        info.Title = ArtistTitleRegexMatches(Index).Value.Substring(
                                                             nSplitIndex + 1,
                                                             nStringLen - nSplitIndex - 1)
                    End If


                End If

            End If
        End If

        Return info
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' Close only if bIsOpen=true
                Close()
            End If
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
