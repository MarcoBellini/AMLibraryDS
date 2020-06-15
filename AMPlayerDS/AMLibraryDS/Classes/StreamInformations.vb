' Class to store stream informations (for Decoders and Encoders)
Public Class StreamInformations
    Implements IDisposable

    ' Empty string
    Private Const EMPTY_STRING As String = ""

    ' Basic file informations
    Public FileLocation As String = EMPTY_STRING
    Public FileSize As Long = 0
    Public FileName As String = EMPTY_STRING
    Public FileFolder As String = EMPTY_STRING
    Public FileExtension As String = EMPTY_STRING

    ' Stream informations
    Public Channels As Short = 0
    Public BitsPerSample As Short = 0
    Public Samplerate As Integer = 0
    Public BlockAlign As Short = 0
    Public AvgBytesPerSec As Integer = 0
    Public DurationInMs As Double = 0

    ' Tags informations
    Public Artist As String = EMPTY_STRING
    Public Title As String = EMPTY_STRING
    Public Album As String = EMPTY_STRING
    Public Year As String = EMPTY_STRING
    Public Comment As String = EMPTY_STRING
    Public Genre As String = EMPTY_STRING

    'ID3 Genre list https://en.wikipedia.org/wiki/ID3
    Public Enum GenreList
        Blues = 0
        ClassicRock = 1
        Country = 2
        Dance = 3
        Disco = 4
        Funk = 5
        Grunge = 6
        HipHop = 7
        Jazz = 8
        Metal = 9
        NewAge = 10
        Oldies = 11
        Other = 12
        Pop = 13
        RhythmAndBlues = 14
        Rap = 15
        Reggage = 16
        Rock = 17
        Techno = 18
        Industrial = 19
        Alternative = 20
        Ska = 21
        DeathMetal = 22
        Pranks = 23
        SoundTrack = 24
        EuroTechno = 25
        Ambient = 26
        TripHop = 27
        Vocal = 28
        JazzAndFunk = 29
        Fusion = 30
        Trance = 31
        Classical = 32
        Instrumental = 33
        Acid = 34
        House = 35
        Game = 36
        SoundClip = 37
        Noise = 39
        AlternativeRock = 40
        Bass = 41
        Soul = 42
        Puck = 43
        Space = 44
        Meditative = 45
        InstrumentalPop = 46
        IstrumentalRock = 47
        Ethnic = 48
        Gothic = 49
        Darkwave = 50
        TechnoIndustrial = 51
        Electronic = 52
        PopFolk = 53
        Eurodance = 54
        Dream = 55
        SouthernRock = 56
        Comedy = 57
        Clut = 58
        Gangsta = 59
        Top40 = 60
        ChristianRap = 61
        PopFunk = 62
        Jungle = 63
        NativeUS = 64
        Cabaret = 65
        NewWave = 66
        Psychedelic = 67
        Rave = 68
        ShowTunes = 69
        Trailer = 70
        LoFi = 71
        Tibal = 72
        AcidPunk = 73
        AcidJazz = 74
        Polka = 75
        Retro = 76
        Musical = 77
        RockNRoll = 78
        HardRock = 79
    End Enum

    ' Array used to convert from enum number to string
    Private GenresString() As String =
    {"Blues", "Classic Rock", "Country",
    "Dance", "Disco", "Funk",
    "Grunge", "Hip-Hop", "Jazz",
    "Metal", "New Age", "Oldies",
    "Other", "Pop", "Rhythm and Blues",
    "Rap", "Reggage", "Rock",
    "Techno", "Industrial", "Alternative",
    "Ska", "Death Metal", "Pranks",
    "SoundTrack", "Euro-Techno", "Ambient",
    "Trip-Hop", "Vocal", "Jazz & Funk",
    "Fusion", "Trance", "Classical",
    "Instrumental", "Acid", "House",
    "Game", "Sound clip", "Gospel",
    "Noise", "Alternative Rock", "Bass",
    "Soul", "Punk", "Space",
    "Meditative", "Instrumental Pop", "Instrumental Rock",
    "Ethnic", "Gothic", "Darkwave",
    "Techno Industrial", "Electronic", "Pop Folk",
    "Eurodance", "Dream", "Southern Rock",
    "Comedy", "Cult", "Gangsta",
    "Top 40", "Christian Rap", "Pop/Funk",
    "Jungle", "Native US", "Cabaret",
    "New Wave", "Psychedelic", "Rave",
    "Show tunes", "Trailer", "Lo-Fi",
    "Tribal", "Acid Punk", "Acid Jazz",
    "Polka", "Retro", "Musical",
    "Rock ’n’ Roll", "Hard Rock"}

    ''' <summary>
    ''' Convert from enum value, to string value
    ''' </summary>
    ''' <param name="value">Value from GenreList</param>
    ''' <returns>A String with the genre selected in value list</returns>
    Public Function GenreEnumToString(ByVal value As GenreList) As String
        Dim result As String = "Unknown"

        ' Check if index is in the array lenght
        If value <= GenresString.Length Then
            result = GenresString(value)
        End If

        Return result
    End Function

    ''' <summary>
    ''' Convert from integer value, to string value
    ''' </summary>
    ''' <param name="value">Value from genre index</param>
    ''' <returns>A String with the genre selected in value list</returns>
    Public Function GenreIndexToString(ByVal value As Integer) As String
        Dim result As String = "Unknown"

        ' Check if index is in the array lenght
        If value <= GenresString.Length Then
            result = GenresString(value)
        End If

        Return result
    End Function

    ''' <summary>
    ''' Fill Basic file informations (Location,size, extension...)
    ''' </summary>
    ''' <param name="path"></param>
    Public Sub FillBasicFileInfo(ByVal path As String)
        Dim info As New IO.FileInfo(path)

        If info.Exists Then
            FileLocation = path
            FileFolder = info.DirectoryName
            FileName = IO.Path.GetFileNameWithoutExtension(path)
            FileSize = info.Length
            FileExtension = info.Extension
        End If

    End Sub

    ''' <summary>
    ''' Convert all informations(File Info, Stream Info and ID3) to List (of String)
    ''' </summary>
    ''' <returns>List of informations</returns>
    Public Function Informations2List() As List(Of String)
        Dim Info As New List(Of String)

        Info.Add("<<< BASIC INFORMATIONS >>>")
        Info.Add("File location: " + FileLocation)
        Info.Add("File Folder: " + FileFolder)
        Info.Add("File Name: " + FileName)
        Info.Add("File Extension: " + FileExtension)
        Info.Add(" ")

        Info.Add("<<< STREAM INFORMATIONS >>>")
        Info.Add("Samplerate: " + Samplerate.ToString)
        Info.Add("Channels: " + Channels.ToString)
        Info.Add("Bit per Sample: " + BitsPerSample.ToString)

        With TimeSpan.FromMilliseconds(DurationInMs)
            Info.Add("Duration: " + Format(.TotalHours, "0") + ":" + Format(.TotalMinutes, "00") + ":" + Format(.TotalSeconds, "00"))
        End With
        Info.Add(" ")

        Info.Add("<<< FILE TAGS >>>")
        Info.Add("Artist: " + Artist)
        Info.Add("Title: " + Title)
        Info.Add("Album: " + Album)
        Info.Add("Year: " + Year)
        Info.Add("Genre: " + Genre)
        Info.Add("Comment: " + Comment)

        Return Info
    End Function

    ''' <summary>
    ''' Convert Stream informations to WaveFormatEx structure
    ''' </summary>
    ''' <returns></returns>
    Public Function Stream2WaveFormat() As WaveFormatEx
        Dim sWaveFormat As WaveFormatEx

        With sWaveFormat
            .Samplerate = Samplerate
            .Channels = Channels
            .BlockAlign = BlockAlign
            .AvgBytesPerSec = AvgBytesPerSec
            .BitsPerSample = BitsPerSample
        End With

        Return sWaveFormat
    End Function

    ''' <summary>
    ''' Check if current wave format is supported by AMPlayer
    ''' </summary>
    ''' <returns></returns>
    Public Function IsBadFormat() As Boolean
        If (Channels < 1) Or (Channels > 2) Then
            Return True
        End If

        If (Samplerate < 8000) Or
            (Samplerate > 48000) Then
            Return True
        End If

        If (BitsPerSample < 8) Or (BitsPerSample > 16) Then
            Return True
        End If

        If BlockAlign <> CShort(Channels * BitsPerSample / 8) Then
            Return True
        End If

        If AvgBytesPerSec <> (BlockAlign * Samplerate) Then
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' Copy ID3 tags to another stream info class
    ''' </summary>
    ''' <param name="info">Class to copy</param>
    Public Sub CopyID3To(ByRef info As StreamInformations)
        With info
            .Title = Title
            .Artist = Artist
            .Album = Album
            .Comment = Comment
            .Genre = Genre
            .Year = Year
        End With
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean


    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' Nothing to do,maybe in the future...
            End If


        End If
        disposedValue = True
    End Sub


    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub
#End Region
End Class
