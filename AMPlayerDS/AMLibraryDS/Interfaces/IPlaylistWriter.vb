Public Interface IPlaylistWriter
    Inherits IDisposable

    Function Create(ByVal path As String) As Boolean
    Sub Close()

    ReadOnly Property Extension As String

    Function WriteInformations(ByRef Info As StreamInformations) As Boolean

End Interface
