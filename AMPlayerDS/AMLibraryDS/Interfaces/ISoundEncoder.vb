
Public Interface ISoundEncoder
    ''' <summary>
    ''' Seek in a new write position in the stream
    ''' </summary>
    ''' <param name="position">New position in bytes</param>
    ''' <param name="mode">Mode to seek</param>
    ''' <returns>New position in the stream</returns>
    Function Seek(ByVal position As Long, ByVal mode As IO.SeekOrigin) As Long

    ''' <summary>
    ''' Write PCM data in a byte buffer
    ''' </summary>
    ''' <param name="Buffer">Valid buffer</param>
    ''' <param name="offset">Seek operation from current position</param>
    ''' <param name="Count">Number of bytes to write</param>
    ''' <returns></returns>
    Function Write(ByRef Buffer() As Byte, ByVal offset As Integer, ByVal Count As Integer) As Integer

    ''' <summary>
    ''' Create new file
    ''' </summary>
    ''' <param name="path">Path of file</param>
    ''' <param name="info">Valid Stream Informations class</param>
    ''' <returns>True on success</returns>
    Function Create(ByVal path As String, ByRef info As StreamInformations) As Boolean

    ''' <summary>
    ''' Close current file stream
    ''' </summary>
    Sub Close()

    ''' <summary>
    ''' Name of encoder
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property Name() As String

    ''' <summary>
    ''' Get the List(of string) of supported extensions
    ''' </summary>
    ''' <returns>Supported extensions</returns>
    ReadOnly Property Extension() As String

    ''' <summary>
    ''' Show configuration Dialog
    ''' </summary>
    Sub ShowConfiguration()

End Interface
