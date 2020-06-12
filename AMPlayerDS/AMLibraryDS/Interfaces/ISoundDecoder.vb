Public Interface ISoundDecoder

    ''' <summary>
    ''' Open new local o remote file stream
    ''' </summary>
    ''' <param name="path">Location of file</param>
    ''' <returns>Return true if succeded</returns>
    Function Open(ByVal Path As String) As Boolean

    ''' <summary>
    ''' Close current stream if is open
    ''' </summary>
    Sub Close()

    ''' <summary>
    ''' Seek to new position
    ''' </summary>
    ''' <param name="offset">New position in opened streamin bytes</param>
    ''' <param name="mode">Mode: BEGIN, CURRENT, END</param>
    ''' <returns>Current new position in bytes</returns>
    Function Seek(ByVal Offset As Long, ByVal Mode As IO.SeekOrigin) As Long

    ''' <summary>
    ''' Read PCM data from opened stream
    ''' </summary>
    ''' <param name="Buffer">Byte array where store PCM data</param>
    ''' <param name="Offset">Offset in byte from current position index</param>
    ''' <param name="Count">Number of bytes to read (the same size of buffer array)</param>
    ''' <returns>Number of bytes readed</returns>
    Function Read(ByRef Buffer() As Byte, ByVal Offset As Int32, ByVal Count As Int32) As Int32

    ''' <summary>
    ''' Get the current position in stream (in bytes)
    ''' </summary>
    ''' <returns>Bytes offset</returns>
    ReadOnly Property Position() As Long

    ''' <summary>
    ''' Get the current duration in stream (in bytes)
    ''' </summary>
    ''' <returns>total bytes</returns>
    ReadOnly Property Duration() As Long

    ''' <summary>
    ''' Get the List(of string) of supported extensions
    ''' </summary>
    ''' <returns>Supported extensions</returns>
    ReadOnly Property Extensions() As List(Of String)

    ''' <summary>
    ''' Get the name of this decoder
    ''' </summary>
    ''' <returns>String contnent the name</returns>
    ReadOnly Property Name() As String

    ''' <summary>
    ''' Check if the stream is at the end
    ''' </summary>
    ''' <returns>True if end of stream is reached, oterwise false</returns>
    ReadOnly Property EndOfStream() As Boolean

    ''' <summary>
    '''  Get in a fast way the stream information of a file
    ''' </summary>
    ''' <param name="Path">Location of the file</param>
    ''' <param name="info">Instance of stream info</param>
    ''' <returns>true if succeded otherwise false</returns>
    Function FastStreamInformations(ByVal Path As String, ByRef info As StreamInformations) As Boolean

    ''' <summary>
    ''' Get current stream information
    ''' </summary>
    ''' <returns>Valid stream info class handle</returns>
    Function OpenedStreamInformations() As StreamInformations

    ''' <summary>
    ''' Notified when an operation of Read is succeded
    ''' </summary>
    ''' <param name="Buffer">Buffer readed</param>
    Event DataReaded_Event(ByRef Buffer() As Byte)

    ''' <summary>
    ''' Notified when the stream reach the end of file
    ''' </summary>
    Event EndOfStream_Event()

    ' TODO: Add ShowConfiguraton Sub ??
End Interface
