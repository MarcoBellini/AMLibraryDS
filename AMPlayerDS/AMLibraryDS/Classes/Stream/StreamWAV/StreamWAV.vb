Option Explicit On

' Thanks to http://soundfile.sapp.org/doc/WaveFormat/ for
' format explain

' This class Opens only Microsoft PCM Wave File

Imports System.IO

Public Class StreamWAV
    Implements ISoundDecoder

    Private IOStream As IO.FileStream
    Private Reader As IO.BinaryReader
    Private StreamInfo As StreamInformations

    Private bIsOpen As Boolean = False
    Private bIsEndOfStream As Boolean = False

    Private nHeaderOffset As Long = 0
    Private nPosition As Long = 0
    Private nDuration As Long = 0

    Public Event DataReaded_Event(ByRef Buffer() As Byte) Implements ISoundDecoder.DataReaded_Event
    Public Event EndOfStream_Event() Implements ISoundDecoder.EndOfStream_Event

    ''' <summary>
    ''' Get the current position in stream (in bytes)
    ''' </summary>
    ''' <returns>Bytes offset</returns>
    Public ReadOnly Property Position As Long Implements ISoundDecoder.Position
        Get
            Return nPosition
        End Get
    End Property

    Public ReadOnly Property Duration As Long Implements ISoundDecoder.Duration
        Get
            Return nDuration
        End Get
    End Property

    ''' <summary>
    ''' Get the List(of string) of supporte extensions
    ''' </summary>
    ''' <returns>Supported extensions</returns>
    Public ReadOnly Property Extensions As List(Of String) Implements ISoundDecoder.Extensions
        Get
            Dim Supported As New List(Of String) From {
                ".wav"
            }

            Return Supported
        End Get
    End Property

    ''' <summary>
    ''' Get the name of this decoder
    ''' </summary>
    ''' <returns>String contnent the name</returns>
    Public ReadOnly Property Name As String Implements ISoundDecoder.Name
        Get
            Return ".NET Wave Reader 0.1a"
        End Get
    End Property


    ''' <summary>
    ''' Check if the stream is at the end
    ''' </summary>
    ''' <returns>True if end of stream is reached, oterwise false</returns>
    Public ReadOnly Property EndOfStream As Boolean Implements ISoundDecoder.EndOfStream
        Get
            Return bIsEndOfStream
        End Get
    End Property

    ''' <summary>
    ''' Close current stream if is open
    ''' </summary>
    Public Sub Close() Implements ISoundDecoder.Close
        If bIsOpen = True Then
            ' Close streams
            StreamInfo = Nothing
            Reader.Dispose()
            IOStream.Dispose()

            ' Reset vars
            nHeaderOffset = 0
            nPosition = 0
            nDuration = 0

            bIsEndOfStream = False
            bIsOpen = False
        End If
    End Sub

    ''' <summary>
    ''' Open new local file stream
    ''' Decode only Microsoft WAV file
    ''' </summary>
    ''' <param name="path">Location of file</param>
    ''' <returns>Return true if succeded</returns>
    Public Function Open(Path As String) As Boolean Implements ISoundDecoder.Open
        Dim bresult As Boolean = False
        Dim buffer(3) As Byte
        Dim AudioFormat As Short
        Dim ChunkSize, Subchunk1Size, Subchunk2Size As Integer

        ' Check if stream is already open
        If bIsOpen = True Then
            Me.Close()
        End If

        'Check if file exist
        If My.Computer.FileSystem.FileExists(Path) = True Then

            ' Create new instances of main classes
            IOStream = New FileStream(Path, FileMode.Open)
            Reader = New BinaryReader(IOStream)
            StreamInfo = New StreamInformations()

            ' Start reading to 0
            Reader.BaseStream.Seek(0, SeekOrigin.Begin)

            ' (1) Read ChunkID
            '     -> Offset: 0 , Size: 4 byte
            Reader.Read(buffer, 0, 4)

            ' Check if contains Ascii code 0x52494646 - "R" "I" "F" "F" 
            ' NB: big-endian form
            If (buffer(0) = &H52) And (buffer(1) = &H49) And
               (buffer(2) = &H46) And (buffer(3) = &H46) Then

                ' (2) Read ChunkSize 
                '     -> Offset 4, Size: 4
                ChunkSize = Reader.ReadInt32()
                'ChunkSize= 36 + SubChunk2Size, or more precisely:
                '4 + (8 + SubChunk1Size) + (8 + SubChunk2Size)
                'This Is the size of the rest of the chunk 
                'following this number.  This Is the size of the 
                'entire File in bytes minus 8 bytes for the
                'two fields Not included in this count
                'ChunkID And ChunkSize (little endian)

                ' (3) Read Format 
                '     -> Offset 8, Size: 4
                Reader.Read(buffer, 0, 4)

                'Check if contains Ascii code 0x57415645 "W" "A" "V" "E"
                ' NB: big-endian form
                If (buffer(0) = &H57) And (buffer(1) = &H41) And
                   (buffer(2) = &H56) And (buffer(3) = &H45) Then

                    ' (4) Subchunk1ID
                    '     -> Offset 12, Size: 4
                    Reader.Read(buffer, 0, 4)

                    'Check if contains Ascii code  0x666d7420  "f""m""t"
                    ' NB: big-endian form
                    If (buffer(0) = &H66) And (buffer(1) = &H6D) And
                       (buffer(2) = &H74) And (buffer(3) = &H20) Then

                        ' 16 for PCM. This is the size of the
                        ' rest of the Subchunk which follows this number
                        Subchunk1Size = Reader.ReadInt32()

                        ' Check if the format is PCM 
                        If Subchunk1Size = 16 Then

                            ' PCM=1 if PCM without compression
                            AudioFormat = Reader.ReadInt16()

                            ' Check again if the format is PCM 
                            If AudioFormat = 1 Then
                                ' Read channel 16 bit
                                StreamInfo.Channels = Reader.ReadInt16()

                                ' Read Samplerate 32 bit
                                StreamInfo.Samplerate = Reader.ReadInt32()

                                ' Read ByteRate 32 bit
                                StreamInfo.AvgBytesPerSec = Reader.ReadInt32()

                                ' Read block align 16 bit
                                StreamInfo.BlockAlign = Reader.ReadInt16()

                                ' Read Bits per sample 16 bit
                                StreamInfo.BitsPerSample = Reader.ReadInt16()

                                ' (5) Subchunk2ID
                                '     -> Offset 36, Size: 4
                                Reader.Read(buffer, 0, 4)

                                'Check if contains Ascii code 0x64617461   "D""A""T""A"
                                ' NB: big-endian form
                                If (buffer(0) = &H64) And (buffer(1) = &H61) And
                                   (buffer(2) = &H74) And (buffer(3) = &H61) Then

                                    ' Data size 
                                    Subchunk2Size = Reader.ReadInt32()

                                    ' Current header size
                                    ' 44 byte is the offset from the origin where
                                    ' start to read PCM byte samples
                                    nHeaderOffset = Reader.BaseStream.Position()

                                    ' Align variables
                                    nDuration = Reader.BaseStream.Length - nHeaderOffset
                                    nPosition = 0

                                    ' Fill the duration in milliseconds
                                    StreamInfo.DurationInMs = nDuration / StreamInfo.AvgBytesPerSec * 1000

                                    ' Fill basic file info and ID3 tag
                                    StreamInfo.FillBasicFileInfo(Path)

                                    ' No metadata 
                                    StreamInfo.Title = StreamInfo.FileName


                                    'File correctly opened
                                    bresult = True
                                    bIsOpen = True
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            ' Try to close on fail
            If bresult = False Then
                Me.Close()
            End If
        End If

        Return bresult
    End Function

    ''' <summary>
    ''' Seek to new position
    ''' </summary>
    ''' <param name="offset">New position in opened streamin bytes</param>
    ''' <param name="mode">Mode: BEGIN, CURRENT, END</param>
    ''' <returns>Current new position in bytes</returns>
    Public Function Seek(Offset As Long, Mode As SeekOrigin) As Long Implements ISoundDecoder.Seek
        If bIsOpen = True Then

            ' Check if offset is less than Duration
            If nPosition + Offset <= nDuration Then
                ' Align to nHeaderOffset and seek to new position
                nPosition = Reader.BaseStream.Seek(Offset + nHeaderOffset, Mode)

                ' Check if new position is at end of stream
                If nPosition = nDuration Then
                    bIsEndOfStream = True
                Else
                    bIsEndOfStream = False
                End If

                ' ReAlign with Header offset
                nPosition = nPosition - nHeaderOffset
            End If

        End If
    End Function

    ''' <summary>
    ''' Read PCM data from opened stream
    ''' </summary>
    ''' <param name="Buffer">Byte array where store PCM data</param>
    ''' <param name="offset">Offset in byte from current position index</param>
    ''' <param name="Length">Number of bytes to read (the same size of buffer array)</param>
    ''' <returns>Number of bytes readed</returns>
    Public Function Read(ByRef Buffer() As Byte, offset As Integer, Length As Integer) As Integer Implements ISoundDecoder.Read
        Dim nByteReaded As Long

        ' Read data only if stream is open
        If bIsOpen = True Then

            ' If there is an offset from current posiiton, seek
            If offset <> 0 Then
                Me.Seek(offset, SeekOrigin.Current)
            End If

            ' Read and check end of stream
            If Position + Length <= Duration Then
                nByteReaded = Reader.Read(Buffer, 0, Length)
            Else
                nByteReaded = Reader.Read(Buffer, 0, CInt(Duration - Position))
                bIsEndOfStream = True
                RaiseEvent EndOfStream_Event()
            End If

            ' Notify new data readed
            RaiseEvent DataReaded_Event(Buffer)

            ' Increment current position
            nPosition = nPosition + nByteReaded

        End If

        Return nByteReaded
    End Function

    ''' <summary>
    '''  Get in a fast way the stream information of a file
    ''' </summary>
    ''' <param name="Path">Location of the file</param>
    ''' <param name="info">Instance of stream info</param>
    ''' <returns>true if succeded otherwise false</returns>
    Public Function FastStreamInformations(Path As String, ByRef info As StreamInformations) As Boolean Implements ISoundDecoder.FastStreamInformations
        If bIsOpen = False Then
            Open(Path)
            info = StreamInfo
            Close()
        End If
    End Function

    ''' <summary>
    ''' Get current stream information
    ''' </summary>
    ''' <returns>Valid stream info class handle</returns>
    Public Function OpenedStreamInformations() As StreamInformations Implements ISoundDecoder.OpenedStreamInformations
        If bIsOpen = True Then
            Return StreamInfo
        Else
            Return Nothing
        End If
    End Function

End Class






