Public Interface ISoundOutput

    ' Name of the plugin
    ReadOnly Property Name() As String

    ' Dialogs
    Sub ConfigDialog()
    Sub AboutDialog()

    ' Use to set the main window handle
    Property AMPlayerHandle() As IntPtr

    ' Get buffer in playing
    Function GetPlayedData(ByRef Buffer() As Byte, ByVal Count As Int32) As Boolean

    ' Refill buffer with new data(es. after seek operation)
    Sub ReFill(ByVal state As RefillPoint)

    ' Use to initialize or deinitialize output
    Sub Init()
    Sub DeInit()

    ' Open output at a wave format
    Function Open(ByVal format As WaveFormatEx) As Boolean
    Sub Close()

    ' Use to change playing status (PLAYING, PAUSING,STOPPED)
    Property Status() As Status

    ' Use to set decoder to output (output control main 
    ' streaming)
    Property ISoundDecoder() As ISoundDecoder

    ' Option for the output
    Property Volume() As Int32
    Property Pan() As Int32

    ' On this event the end of stream is reached
    Event EndOfStream()

End Interface

Public Enum RefillPoint
    Start = 0
    Finish = 1
End Enum