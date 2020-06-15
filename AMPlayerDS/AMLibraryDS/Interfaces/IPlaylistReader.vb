Public Interface IPlaylistReader
    Inherits IDisposable

    Function Open(ByVal path As String) As Boolean
    Sub Close()

    ReadOnly Property Extension As String
    ReadOnly Property Count As Long

    Function ReadInformations(ByVal Index As Long) As StreamInformations
End Interface
