Imports System.IO


Public Class StreamCDA
    Implements ISoundDecoder

    Private Const CDROM_SECTOR_SIZE As Integer = 2352 'bytes
    Private Const CDROM_SECTOR_TO_READ As Integer = 10 ' number of sectors

    ' CD WaveFormat
    Private Const CDA_FILE_SAMPLERATE As Integer = 44100
    Private Const CDA_FILE_CHANNELS As Short = 2
    Private Const CDA_FILE_BITSPERSAMPLE As Short = 16
    Private Const CDA_FILE_BLOCKALIGN As Short = 4
    Private Const CDA_FILE_AVGBYTESPERSEC As Integer = 176400

    Private bIsEndOfStream As Boolean = False
    Private bIsOpen As Boolean = False

    Private StreamInfo As StreamInformations
    Private CircleBuffer As CircleBuffer
    Private Cdrom As CdDrive

    Private nPosition As Long = 0
    Private nDuration As Long = 0

    Private nPositionLSB As Long = 0
    Private nDurationLSB As Long = 0

    Private nTrackOffset As Long = 0

    Public Event DataReaded_Event(ByRef Buffer() As Byte) Implements ISoundDecoder.DataReaded_Event
    Public Event EndOfStream_Event() Implements ISoundDecoder.EndOfStream_Event


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

    Public ReadOnly Property Extensions As List(Of String) Implements ISoundDecoder.Extensions
        Get
            Dim Supported As New List(Of String) From {
                ".cda"
            }

            Return Supported
        End Get
    End Property

    Public ReadOnly Property Name As String Implements ISoundDecoder.Name
        Get
            Return ".NET CD ROM Reader 0.1a"
        End Get
    End Property

    Public ReadOnly Property EndOfStream As Boolean Implements ISoundDecoder.EndOfStream
        Get
            Return bIsEndOfStream
        End Get
    End Property


    Public Sub Close() Implements ISoundDecoder.Close
        If bIsOpen = True Then

            ' Free resources
            StreamInfo.Dispose()
            StreamInfo = Nothing

            Cdrom.UnlockCD()
            Cdrom.Dispose()
            Cdrom = Nothing

            CircleBuffer.Dispose()
            CircleBuffer = Nothing

            ' Reset vars
            nPosition = 0
            nDuration = 0
            nPositionLSB = 0
            nDurationLSB = 0
            nTrackOffset = 0

            bIsEndOfStream = False
            bIsOpen = False

#If DEBUG Then
            DebugPrintLine("StreamCDA", "Closed Successful")
#End If
        End If
    End Sub


    ''' <summary>
    ''' Open a stream of a track in the CD-ROM
    ''' 
    ''' This function must recive path like
    ''' "D-1.cda" ... "D-2.cda" ... etc
    ''' </summary>
    ''' <param name="Path">Path to open</param>
    ''' <returns></returns>
    Public Function Open(Path As String) As Boolean Implements ISoundDecoder.Open
        Dim bResult As Boolean = False
        Dim strTrackNumber As String
        Dim chrDrive As Char
        Dim nTrackNumber As Integer

        ' Check if there is an open session
        If bIsOpen = True Then
            Close()
        End If

        ' Read drive to open
        chrDrive = CChar(Path.Remove(1, Path.Length - 1))

        ' Convert string to integer
        strTrackNumber = Path.Remove(Path.IndexOf("."c), 4)
        strTrackNumber = strTrackNumber.Remove(0, Path.IndexOf("-"c) + 1)

        If Integer.TryParse(strTrackNumber, nTrackNumber) = True Then
            Cdrom = New CdDrive()

            ' Try to open drive
            If Cdrom.Open(chrDrive) Then

                ' Check if cd is in the cd drive
                If Cdrom.IsCdReady = True Then

                    ' Read table of contnent
                    If Cdrom.Refresh = True Then

                        ' Check if track is in the range of cd tracks
                        If (nTrackNumber > 0) And (nTrackNumber <= Cdrom.GetNumAudioTracks()) Then


                            ' Track parameters in sectors
                            nTrackOffset = Cdrom.GetTrackStartingAddress(nTrackNumber)
                            nDurationLSB = Cdrom.GetTrackLength(nTrackNumber)
                            nPositionLSB = 0

                            ' Stream info in bytes
                            nDuration = nDurationLSB * CDROM_SECTOR_SIZE
                            nPosition = 0

                            StreamInfo = New StreamInformations()

                            ' Basic file info
                            StreamInfo.FileExtension = "CDA"
                            StreamInfo.FileName = strTrackNumber
                            StreamInfo.FileFolder = chrDrive
                            StreamInfo.FileLocation = ""
                            StreamInfo.FileSize = nDurationLSB * CDROM_SECTOR_SIZE

                            ' Waveformat
                            StreamInfo.Samplerate = CDA_FILE_SAMPLERATE
                            StreamInfo.BitsPerSample = CDA_FILE_BITSPERSAMPLE
                            StreamInfo.Channels = CDA_FILE_CHANNELS
                            StreamInfo.BlockAlign = CDA_FILE_BLOCKALIGN
                            StreamInfo.AvgBytesPerSec = CDA_FILE_AVGBYTESPERSEC

                            StreamInfo.DurationInMs = StreamInfo.FileSize / StreamInfo.AvgBytesPerSec * 1000
                            StreamInfo.Title = "CD Track: " & strTrackNumber

                            ' Init circle buffer (2 second length)
                            CircleBuffer = New CircleBuffer(StreamInfo.AvgBytesPerSec * 2)

                            ' Prevent eject CD during playing
                            Cdrom.LockCD()

                            ' Open successful
                            bResult = True
                            bIsOpen = True

#If DEBUG Then
                            DebugPrintLine("StreamCDA", "Open Successful Drive: " & chrDrive & " Track: " & strTrackNumber)
#End If
                        End If

                    End If
                End If
            End If
        End If

        Return bResult
    End Function

    Public Function Seek(Offset As Long, Mode As SeekOrigin) As Long Implements ISoundDecoder.Seek
        Dim OffsetLSB As Long

        If bIsOpen = True Then

            ' Convert offset from byte to sector unit
            Offset = Offset - Offset Mod CDROM_SECTOR_SIZE
            OffsetLSB = Offset \ CDROM_SECTOR_SIZE

            ' Update position 
            Select Case Mode
                Case SeekOrigin.Begin
                    nPositionLSB = 0 + OffsetLSB
                    nPosition = 0 + Offset
                Case SeekOrigin.Current
                    nPositionLSB = nPositionLSB + OffsetLSB
                    nPosition = nPosition + Offset
                Case SeekOrigin.End
                    nPositionLSB = nDurationLSB - OffsetLSB
                    nPosition = nDuration - Offset
            End Select

            ' Reset circle buffer
            CircleBuffer.ClearMemory()

            ' Check if is at end of stream
            If nPositionLSB = nDurationLSB Then
                bIsEndOfStream = True
            Else
                bIsEndOfStream = False
            End If

            Return nPosition
        End If
    End Function

    Public Function Read(ByRef Buffer() As Byte, Offset As Integer, Count As Integer) As Integer Implements ISoundDecoder.Read
        Dim nWrittenData As Integer = 0

        If bIsOpen = True Then

            ' If offset <> 0 change position
            If Offset <> 0 Then
                Seek(Offset, SeekOrigin.Current)
            End If

            ' Fill buffer until end of stream
            While (CircleBuffer.ValidReadData < Count) And (bIsEndOfStream = False)
                FillBuffer()
            End While

            ' Check if end of stream is reached
            If bIsEndOfStream = False Then
                ' If not, the buffer is big enough
                CircleBuffer.ReadData(Buffer, Count)
                nWrittenData = nWrittenData + Count
            Else
                ' Else check how much data we can read
                If CircleBuffer.ValidReadData > 0 Then

                    If CircleBuffer.ValidReadData >= Count Then
                        'Copy requied count to buffer
                        CircleBuffer.ReadData(Buffer, Count)
                        nWrittenData = nWrittenData + Count
                    Else
                        ' Copy remaining count to buffer
                        Dim nValidData As Integer = CircleBuffer.ValidReadData
                        CircleBuffer.ReadData(Buffer, nValidData)
                        nWrittenData = nWrittenData + nValidData

                        ' Notify we reach the end of stream
                        RaiseEvent EndOfStream_Event()
                    End If
                Else
                    ' Notify we reach the end of stream
                    RaiseEvent EndOfStream_Event()
                End If
            End If

        End If

        ' Update position in bytes
        nPosition = nPosition + nWrittenData

        ' Notify new data readed
        If nWrittenData > 0 Then
            RaiseEvent DataReaded_Event(Buffer)
        End If

        ' Return written bytes
        Return nWrittenData
    End Function


    Private Function FillBuffer() As Long
        Dim buffer(CDROM_SECTOR_TO_READ * CDROM_SECTOR_SIZE - 1) As Byte

        ' Check if stream is open
        If bIsOpen = True Then

            ' Try to read raw sectors
            If Cdrom.ReadSectorsRaw(buffer,
                                    nPositionLSB + nTrackOffset,
                                    CDROM_SECTOR_TO_READ) = True Then

                ' Add data in the circle buffer and update position in sectors unit
                CircleBuffer.WriteData(buffer, CDROM_SECTOR_TO_READ * CDROM_SECTOR_SIZE)
                nPositionLSB = nPositionLSB + CDROM_SECTOR_TO_READ

                ' Check if we reached the end of stream
                If nPositionLSB >= nDurationLSB Then
                    bIsEndOfStream = True
                End If

            Else
                ' If the function fail we probabily 
                ' reached the end of stream
                bIsEndOfStream = True

#If DEBUG Then
                DebugPrintLine("StreamCDA", "DeviceIoControl Return False, End Of Stream")
#End If
            End If
        End If
    End Function

    Public Function FastStreamInformations(Path As String, ByRef info As StreamInformations) As Boolean Implements ISoundDecoder.FastStreamInformations
        ' Return fast stream info, only if stream is not open
        If bIsOpen = False Then
            Open(Path)
            info = StreamInfo
            Close()
        End If
    End Function

    Public Function OpenedStreamInformations() As StreamInformations Implements ISoundDecoder.OpenedStreamInformations
        If bIsOpen = True Then
            Return StreamInfo
        Else
            Return Nothing
        End If
    End Function
End Class
