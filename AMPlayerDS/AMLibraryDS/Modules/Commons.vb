Public Module Commons

    Public ReadOnly AMPlayerChannel As String = "AMPlayerChannel01"

#If PLATFORM = "x86" Then
    ' Dll name in app folder
    Public ReadOnly CompiledPlatform As String = "x86"
#ElseIf PLATFORM = "x64" Then
    ' Dll name in app folder
    Public ReadOnly CompiledPlatform As String = "x64"
#End If


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
        Dim Result As New Text.StringBuilder

        With Time
            If .Hours <> 0 Then
                Result.Append(Fix(.Hours) & ":" & Fix(.Minutes) & ":" & Format(.Seconds, "00"))
            Else
                Result.Append(Fix(.Minutes) & ":" & Format(.Seconds, "00"))
            End If

        End With

        Return Result.ToString
    End Function

End Module
