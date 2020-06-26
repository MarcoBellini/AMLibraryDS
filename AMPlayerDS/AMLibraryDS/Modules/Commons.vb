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

    Public Function FormatTime(ByRef Time As TimeSpan) As String
        Dim Result As String

        With Time
            If .Hours <> 0 Then
                Result = Fix(.Hours) & ":" & Fix(.Minutes) & ":" & Format(.Seconds, "00")
            Else
                Result = Fix(.Minutes) & ":" & Format(.Seconds, "00")
            End If

        End With

        Return Result
    End Function

End Module
