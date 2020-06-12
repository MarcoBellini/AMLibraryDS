Public Module Commons

    Public Enum Status
        PLAYING = 0
        PAUSING = 1
        STOPPED = 2
    End Enum

    Public Structure WaveFormatEx
        Public Channels As Short
        Public BitsPerSample As Short
        Public Samplerate As Integer
        Public BlockAlign As Short
        Public AvgBytesPerSec As Integer
    End Structure

End Module
