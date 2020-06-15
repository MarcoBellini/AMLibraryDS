Public Interface IPlaylistWriter
    Inherits IDisposable

    Function Create(ByVal path As String) As Boolean
    Sub Close()

    ReadOnly Property Extension As String
    ReadOnly Property Count As Long

    Function WriteInformations(ByVal Index As Long, ByRef Info As StreamInformations) As Boolean

End Interface
