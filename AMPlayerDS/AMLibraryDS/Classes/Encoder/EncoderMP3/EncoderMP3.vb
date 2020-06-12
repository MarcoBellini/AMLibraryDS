Imports System.IO
Imports System.Runtime.InteropServices

' https://lame.sourceforge.io/index.php

Public Class EncoderMP3
    Implements ISoundEncoder

    Private Const LAME_MIN_BUFFER_LEN As Integer = 7200

    Private bIsOpen As Boolean = False
    Private IOStream As FileStream
    Private Writer As BinaryWriter
    Private Handle As LameHandle

    Public ReadOnly Property Name As String Implements ISoundEncoder.Name
        Get
            Return "LAME MP3 Encoder .NET"
        End Get
    End Property

    Public ReadOnly Property Extension As String Implements ISoundEncoder.Extension
        Get
            Return ".mp3"
        End Get
    End Property

    Public Sub Close() Implements ISoundEncoder.Close
        If bIsOpen = True Then
            Dim ByteBuffer(LAME_MIN_BUFFER_LEN - 1) As Byte
            Dim ByteBufferHandle As GCHandle

            Dim nWrittenBytes As Integer

            ' Check if there is data after lame flush
            ' if data > 0 write to file and then close
            ByteBufferHandle = GCHandle.Alloc(ByteBuffer, GCHandleType.Pinned)

            nWrittenBytes =
            Lame.lame_encode_flush(Handle,
                                   ByteBufferHandle.AddrOfPinnedObject,
                                   LAME_MIN_BUFFER_LEN)

            ByteBufferHandle.Free()

            If nWrittenBytes > 0 Then
                Writer.Write(ByteBuffer, 0, nWrittenBytes)
            End If

            ' Free resources
            Writer.Close()
            Writer.Dispose()
            Writer = Nothing

            IOStream.Close()
            IOStream.Dispose()
            IOStream = Nothing

            bIsOpen = False
        End If
    End Sub

    Public Sub ShowConfiguration() Implements ISoundEncoder.ShowConfiguration
        Dim frmLame As New LameDialog

        ' If OK overwrite default encoding settings
        If frmLame.ShowDialog() = DialogResult.OK Then
            ' Demanded to Form
        End If

        ' Free resource
        frmLame.Dispose()
    End Sub

    Public Function Seek(position As Long, mode As SeekOrigin) As Long Implements ISoundEncoder.Seek
        If bIsOpen = True Then
            ' Maybe not userful ??
            Writer.Seek(position, mode)
        End If
    End Function

    Public Function Write(ByRef Buffer() As Byte, offset As Integer, Count As Integer) As Integer Implements ISoundEncoder.Write
        If bIsOpen = True Then
            Dim ShortIntervaledArray(Count \ 2 - 1) As Short
            Dim ShortBufferHandle As GCHandle

            Dim ByteBuffer() As Byte
            Dim ByteBufferHandle As GCHandle

            Dim nWrittenBytes As Integer
            Dim nBufferSize As Integer

            ' Prepare memory for input PCM 
            byte_to_short_array(Buffer, ShortIntervaledArray, Count)
            ShortBufferHandle = GCHandle.Alloc(ShortIntervaledArray, GCHandleType.Pinned)

            ' Prepare memory for MP3 output buffer
            nBufferSize = 1.25 * Count \ 4 + 7200
            ReDim ByteBuffer(nBufferSize - 1)
            ByteBufferHandle = GCHandle.Alloc(ByteBuffer, GCHandleType.Pinned)


            ' Encode PCM -> MP3
            nWrittenBytes =
            Lame.lame_encode_buffer_interleaved(Handle,
                                                ShortBufferHandle.AddrOfPinnedObject(),
                                                Count \ 4,
                                                ByteBufferHandle.AddrOfPinnedObject(),
                                                nBufferSize)

            ShortBufferHandle.Free()
            ByteBufferHandle.Free()

            ' Write MP3 data into the file
            If nWrittenBytes > 0 Then
                Writer.Write(ByteBuffer, 0, nWrittenBytes)
            End If

        End If
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

        'Init Lame encoder
        Handle = Lame.lame_init()

        If Handle.IsInvalid Then
            Return False ' -> Exit function
        End If

        ' Set Lame parameters
        Lame.lame_set_in_samplerate(Handle, info.Samplerate)
        Lame.lame_set_out_samplerate(Handle, info.Samplerate)

        Lame.lame_set_num_channels(Handle, info.Channels)
        Lame.lame_set_brate(Handle, My.Settings.LAME_Bitrate)

        Select Case My.Settings.LAME_EncoderQualityIndex
            Case 0 ' Fast
                Lame.lame_set_quality(Handle, Lame.Lame_Quality.LOW)
            Case 1 ' Standard
                Lame.lame_set_quality(Handle, Lame.Lame_Quality.MEDIUM)
            Case 2 ' High quality
                Lame.lame_set_quality(Handle, Lame.Lame_Quality.NEARBEST)
        End Select

        ' Set lame as VBR, ABR or CBR
        If My.Settings.LAME_Target = 0 Then
            If My.Settings.LAME_ForceCBR Then
                ' Costant Bitrate
                Lame.lame_set_VBR(Handle, Lame.vbr_mode.vbr_off)
            Else
                Lame.lame_set_VBR(Handle, Lame.vbr_mode.vbr_abr)
            End If
        Else
            ' VBR is ON
            Lame.lame_set_VBR(Handle, Lame.vbr_mode.vbr_default)

            ' Setup VBR settings ( TODO: Test this settings)
            Lame.lame_set_VBR_max_bitrate_kbps(Handle, Lame.LAME_MAX_BITRATE * My.Settings.LAME_VBRQuality \ 100)
            Lame.lame_set_VBR_min_bitrate_kbps(Handle, Lame.LAME_MIN_BITRATE * My.Settings.LAME_VBRQuality \ 100)
            Lame.lame_set_VBR_mean_bitrate_kbps(Handle, Lame.LAME_MEAN_BITRATE * My.Settings.LAME_VBRQuality \ 100)


            ' Set VBR quality based on settings
            If My.Settings.LAME_VBRModeIndex = 0 Then
                ' Standard
                Lame.lame_set_VBR_q(Handle, 2)
            Else
                ' Fast
                Lame.lame_set_VBR_q(Handle, 6)
            End If

            ' Force Write VBR tag
            Lame.lame_set_bWriteVbrTag(Handle, 1)

        End If

        ' Set ID3 tags
        Lame.id3tag_init(Handle)
        Lame.id3tag_set_artist(Handle, info.Artist)
        Lame.id3tag_set_title(Handle, info.Title)
        Lame.id3tag_set_album(Handle, info.Album)
        Lame.id3tag_set_year(Handle, info.Year)
        Lame.id3tag_set_genre(Handle, info.Genre)
        Lame.id3tag_set_comment(Handle, info.Comment)


        ' Init params
        If Lame.lame_init_params(Handle) <> Lame.lame_errorcodes.LAME_NOERROR Then
            Handle.Dispose()
            Handle = Nothing
            Return False ' -> Exit function
        End If

        ' We are ready to write PCM data
        bIsOpen = True

        ' Success
        Return True

    End Function
End Class
