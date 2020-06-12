Imports System.IO

' Thanks to http://soundfile.sapp.org/doc/WaveFormat/ for
' format explain

' This class writes only Microsoft PCM Wave File

Public Class EncoderWAV
    Implements ISoundEncoder

    ' 16 = PCM
    Private Const Subchunk1Size As Integer = 16

    ' 1 = PCM
    Private Const AudioFormat As Short = 1

    ' ChunkID - 0x52494646
    Private Shared ReadOnly RIFF_BYTES() As Byte = {&H52, &H49, &H46, &H46}

    ' Format  - 0x57415645
    Private Shared ReadOnly WAVE_BYTES() As Byte = {&H57, &H41, &H56, &H45}

    ' Subchunk1ID - 0x666d7420 
    Private Shared ReadOnly FMT0_BYTES() As Byte = {&H66, &H6D, &H74, &H20}

    ' Subchunk2ID - 0x64617461
    Private Shared ReadOnly DATA_BYTES() As Byte = {&H64, &H61, &H74, &H61}


    ' Global Variables
    Private bIsOpen As Boolean = False
    Private nWrittenBytes As Integer = 0

    Private IOStream As FileStream
    Private Writer As BinaryWriter

    Public ReadOnly Property Name As String Implements ISoundEncoder.Name
        Get
            Return ".NET Wave Encoder 0.1a"
        End Get
    End Property

    Public ReadOnly Property Extensions As String Implements ISoundEncoder.Extension
        Get
            Return ".wav"
        End Get
    End Property

    Public Sub Close() Implements ISoundEncoder.Close
        If bIsOpen = True Then

            ' --- Finalize Header ---

            ' ChunkSize        
            Writer.Seek(4, SeekOrigin.Begin)
            Writer.Write(36 + nWrittenBytes)

            ' Subchunk2Size
            Writer.Seek(40, SeekOrigin.Begin)
            Writer.Write(nWrittenBytes)

            ' Free resources
            Writer.Flush()
            Writer.Dispose()
            IOStream.Close()
            IOStream.Dispose()
            Writer = Nothing
            IOStream = Nothing

            bIsOpen = False
            nWrittenBytes = 0
        End If

    End Sub

    Public Sub ShowConfiguration() Implements ISoundEncoder.ShowConfiguration
        MsgBox("Nothing to configure")
    End Sub

    Public Function Seek(offset As Long, mode As SeekOrigin) As Long Implements ISoundEncoder.Seek
        If bIsOpen = True Then
            Writer.BaseStream.Seek(offset, SeekOrigin.Begin)
        End If
    End Function

    Public Function Write(ByRef Buffer() As Byte, offset As Integer, Count As Integer) As Integer Implements ISoundEncoder.Write
        If bIsOpen = True Then

            ' Seek to new position
            If offset <> 0 Then
                Writer.BaseStream.Seek(offset, SeekOrigin.Current)
            End If

            ' Write PCM data
            Writer.Write(Buffer, 0, Count)
            nWrittenBytes += Count
        End If

        ' Return written bytes
        Return Count
    End Function

    Public Function Create(path As String, ByRef info As StreamInformations) As Boolean Implements ISoundEncoder.Create

        ' Check if stream is already open
        If bIsOpen = True Then
            Return False ' -> Exit function
        End If

        'Check if file exist
        If My.Computer.FileSystem.FileExists(path) Then

            Dim result As DialogResult

            result =
            MessageBox.Show("The " & path & " file already exists. Do you want to overwrite the file?",
                            Name,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1)

            ' If no, exit from function
            If result = DialogResult.No Then
                Return False ' -> Exit function
            End If

        End If

        ' Try to Create new file stream
        Try
            IOStream = New FileStream(path, FileMode.Create)
            Writer = New BinaryWriter(IOStream)
        Catch ex As Exception
            MessageBox.Show(ex.Message,
                            Name,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)

            Return False ' -> Exit function
        End Try

        ' Write ChickID = Riff string
        Writer.Write(RIFF_BYTES)

        ' Write ChuckSize (Here an arbitrary value)
        Writer.Write(2048)

        ' Format - Write Format = Wave String
        Writer.Write(WAVE_BYTES)

        ' Subchunk1ID - Write Fmt = fmt string
        Writer.Write(FMT0_BYTES)

        ' Subchunk1Size - Write 16 value = PCM
        Writer.Write(Subchunk1Size)

        ' Audio Format - Write 1 value = PCM
        Writer.Write(AudioFormat)

        ' Write Number of channels
        Writer.Write(info.Channels)

        ' Write samplerate
        Writer.Write(info.Samplerate)

        ' Write Byterate
        Writer.Write(info.AvgBytesPerSec)

        ' Write BlockAlign
        Writer.Write(info.BlockAlign)

        ' Write Bits Per sample
        Writer.Write(info.BitsPerSample)

        ' Write Subchunk2ID = Write data = Data string
        Writer.Write(DATA_BYTES)

        ' Write Subchunk2Size  (Here an arbitrary value)  
        Writer.Write(2048)

        ' We are ready to write PCM data
        bIsOpen = True

        ' Success
        Return True
    End Function
End Class
