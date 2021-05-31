Option Strict On

Imports System.Runtime.InteropServices
Imports System.IO

' TODO: Use CircleBuffer to perform read(?)
Public Class StreamOGG
    Implements ISoundDecoder

#Region "Native declarations"


    ' Select Dll based on compile settings
#If PLATFORM = "x86" Then
    Private Const VORBIS_DLL As String = "x86\libvorbis.dll"
    Private Const VF_DLL As String = "x86\libvorbisfile.dll"
#ElseIf PLATFORM = "x64" Then
    Private Const VORBIS_DLL As String = "x64\libvorbis.dll"
    Private Const VF_DLL As String = "x64\libvorbisfile.dll"
#End If

    <StructLayout(LayoutKind.Sequential)>
    Public Class vorbis_info
        Public version As Integer
        Public Channels As Integer
        Public rate As Integer
        Public bitrate_upper As Integer
        Public bitrate_nominal As Integer
        Public bitrate_lower As Integer
        Public bitrate_window As Integer
        Public codec_setup As Integer
    End Class

    Public Enum byte_packing
        little_endian = 0
        big_endian = 1
    End Enum

    Public Enum word_size
        eight_bit = 1
        sixteen_bit = 2
    End Enum

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_comment(
        ByVal ovf_struct As IntPtr,
        ByVal link As Integer
    ) As IntPtr

    End Function

    <DllImport(VORBIS_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function vorbis_comment_query(
         ByVal vorbiscomment As IntPtr,
         <MarshalAs(UnmanagedType.LPStr)> ByVal TagName As String,
         ByVal count As Integer
    ) As IntPtr

    End Function

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_time_seek(
        ByVal ovf_struct As IntPtr,
        ByVal seconds As Double
    ) As Integer

    End Function

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_time_tell(
        ByVal ovf_struct As IntPtr
    ) As Integer

    End Function

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_pcm_seek(
        ByVal ovf_struct As IntPtr,
        ByVal seconds As Long
    ) As Integer

    End Function

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_raw_seek(
        ByVal ovf_struct As IntPtr,
        ByVal seconds As Double
    ) As Integer

    End Function

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_read_float(
        ByVal ovf_struct As IntPtr,
        ByRef pcmChannels As IntPtr,
        ByVal samples As Integer,
        ByRef bitstream As Integer
    ) As Integer

    End Function

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_read(
        ByVal ovf_struct As IntPtr,
        <MarshalAs(UnmanagedType.LPArray)> ByVal buffer() As Byte,
        ByVal length As Integer,
        ByVal bigendianp As byte_packing,
        ByVal wordsize As word_size,
        ByVal sgned As Integer,
        ByRef bitstream As Integer
    ) As Integer

    End Function

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_pcm_total(
        ByVal ovf_struct As IntPtr,
        ByVal i As Integer
    ) As Long

    End Function

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_time_total(
        ByVal ovf_struct As IntPtr,
        ByVal i As Integer
    ) As Double

    End Function

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_raw_total(
        ByVal ovf_struct As IntPtr,
        ByVal i As Integer
    ) As Long

    End Function

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_info(
        ByVal ovf_struct As IntPtr,
        ByVal link As Integer
    ) As IntPtr

    End Function

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_open(
        ByVal hFile As IntPtr,
        ByVal ovf_struct As IntPtr,
        ByVal initial As Integer,
        ByVal ibytes As Integer
    ) As Integer

    End Function

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_fopen(
        ByVal path As String,
        ByVal vf As IntPtr
    ) As Integer

    End Function

    <DllImport(VF_DLL, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function ov_clear(
        ByVal ovf_struct As IntPtr
    ) As Integer

    End Function

#End Region


    ' Buffer step in bytes for fill buffer stream
    Private Const BUFFER_STEP As Integer = 1024

    Private bIsOpen As Boolean = False
    Private bIsEndOfStream As Boolean = False

    Private nDuration As Long = 0
    Private nPosition As Long = 0

    Private OverflowBuffer() As Byte
    Private OverflowDataLen As Integer = 0

    Private OggVorbisHandle As IntPtr = IntPtr.Zero
    Private StreamInfo As StreamInformations

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
                ".ogg"
            }

            Return Supported
        End Get
    End Property

    Public ReadOnly Property Name As String Implements ISoundDecoder.Name
        Get
            Return "Ogg Vorbis Decoder 0.1a"
        End Get
    End Property

    Public ReadOnly Property EndOfStream As Boolean Implements ISoundDecoder.EndOfStream
        Get
            Return bIsEndOfStream
        End Get
    End Property

    Public Sub Close() Implements ISoundDecoder.Close
        If bIsOpen = True Then

            ' 0 for success
            ov_clear(OggVorbisHandle)

            ' Free resource (OggVorbis_File structure)
            Marshal.FreeHGlobal(OggVorbisHandle)

            ' Free other resources
            OggVorbisHandle = IntPtr.Zero

            nDuration = 0
            nPosition = 0
            OverflowDataLen = 0

            StreamInfo = Nothing
            bIsEndOfStream = False
            bIsOpen = False
        End If
    End Sub

    Public Function Open(Path As String) As Boolean Implements ISoundDecoder.Open
        Dim bResult As Boolean = False
        Dim strVorbInfo As New vorbis_info
        Dim ptrVorbInfo As IntPtr = IntPtr.Zero

        ' If stream is open, then close
        If bIsOpen = True Then
            Me.Close()
        End If

        ' Check if file exist
        If My.Computer.FileSystem.FileExists(Path) = True Then

            ' Alloc memory for OggVorbis_File structure
            ' this module don't need the real structure filled
            ' So only alloc the size of structure
            ' SizeOf(OggVorbis_File) = 840 or 348 in hex
            OggVorbisHandle = Marshal.AllocHGlobal(&H348)

            ' Try to open ogg file, if 0 = No errors
            If ov_fopen(Path, OggVorbisHandle) = 0 Then

                ' Try to read file informations
                ptrVorbInfo = ov_info(OggVorbisHandle, -1)

                ' If we have a valid pointer, convert to structure
                If ptrVorbInfo <> IntPtr.Zero Then
                    Marshal.PtrToStructure(ptrVorbInfo, strVorbInfo)

                    ' New instance of Stream Informations
                    StreamInfo = New StreamInformations()

                    ' Fill basic file informations
                    StreamInfo.FillBasicFileInfo(Path)

                    ' Fill streaminformations
                    StreamInfo.Samplerate = strVorbInfo.rate
                    StreamInfo.Channels = CShort(strVorbInfo.Channels)
                    StreamInfo.BitsPerSample = 16 'tipically 16 bit per sample
                    StreamInfo.BlockAlign = CShort(StreamInfo.Channels * StreamInfo.BitsPerSample / 8)
                    StreamInfo.AvgBytesPerSec = StreamInfo.Samplerate * StreamInfo.BlockAlign

                    ' Try to find ID3 tags
                    FindID3Tags()

                    ' Fill duration and position
                    nDuration = ov_pcm_total(OggVorbisHandle, -1) * StreamInfo.BlockAlign
                    nPosition = 0

                    StreamInfo.DurationInMs = nDuration / StreamInfo.AvgBytesPerSec * 1000

                    ' Open successful
                    bIsOpen = True
                    bResult = True
                End If
            End If
        End If

        Return bResult
    End Function

    Private Sub FindID3Tags()
        Dim ID3Ptr As IntPtr = IntPtr.Zero
        Dim TAGPointer As IntPtr = IntPtr.Zero

        ' Check if handle is valid
        If OggVorbisHandle <> IntPtr.Zero Then
            ID3Ptr = ov_comment(OggVorbisHandle, -1)

            ' If ID3Ptr has a valid value, find tags
            If ID3Ptr <> IntPtr.Zero Then

                ' Artist
                TAGPointer = vorbis_comment_query(ID3Ptr, "artist", 0)
                If TAGPointer <> IntPtr.Zero Then
                    StreamInfo.Artist = Marshal.PtrToStringAnsi(TAGPointer)
                End If

                ' Title
                TAGPointer = vorbis_comment_query(ID3Ptr, "title", 0)
                If TAGPointer <> IntPtr.Zero Then
                    StreamInfo.Title = Marshal.PtrToStringAnsi(TAGPointer)
                End If

                ' Album
                TAGPointer = vorbis_comment_query(ID3Ptr, "album", 0)
                If TAGPointer <> IntPtr.Zero Then

                    StreamInfo.Album = Marshal.PtrToStringAnsi(TAGPointer)
                End If

                ' Genre
                TAGPointer = vorbis_comment_query(ID3Ptr, "genre", 0)
                If TAGPointer <> IntPtr.Zero Then
                    StreamInfo.Genre = Marshal.PtrToStringAnsi(TAGPointer)
                End If

                ' Comment
                TAGPointer = vorbis_comment_query(ID3Ptr, "comment", 0)
                If TAGPointer <> IntPtr.Zero Then
                    StreamInfo.Comment = Marshal.PtrToStringAnsi(TAGPointer)
                End If

                ' Year
                TAGPointer = vorbis_comment_query(ID3Ptr, "date", 0)
                If TAGPointer <> IntPtr.Zero Then
                    StreamInfo.Year = Marshal.PtrToStringAnsi(TAGPointer)
                End If

            End If
        End If
    End Sub

    Public Function Seek(Offset As Long, Mode As SeekOrigin) As Long Implements ISoundDecoder.Seek
        Dim nUpdatePosition As Long = 0

        ' Align offset
        Offset = Offset \ StreamInfo.BlockAlign

        ' Calc new position
        Select Case Mode
            Case IO.SeekOrigin.Begin
                nUpdatePosition = 0 + Offset
            Case IO.SeekOrigin.Current
                nUpdatePosition = (nPosition \ StreamInfo.BlockAlign) + Offset
            Case IO.SeekOrigin.End
                nUpdatePosition = (nDuration \ StreamInfo.BlockAlign) - Offset
        End Select

        ' Check if the stream is at end
        If nUpdatePosition < (nDuration \ StreamInfo.BlockAlign) Then
            bIsEndOfStream = False
        Else
            bIsEndOfStream = True
        End If

        ' Try to seek to new position, 0 = success
        If ov_pcm_seek(OggVorbisHandle, nUpdatePosition) = 0 Then
            'Reset Overflow buffer
            OverflowDataLen = 0

            ' Align new position value
            nPosition = nUpdatePosition * StreamInfo.BlockAlign
        End If

        ' Return new position
        Return nPosition
    End Function

    Public Function Read(ByRef Buffer() As Byte, Offset As Integer, Count As Integer) As Integer Implements ISoundDecoder.Read
        Dim nCurrentBytesReaded As Integer = 0

        ' Check if stream is open
        If bIsOpen = True Then

            ' If offset <> 0 change position
            If Offset <> 0 Then
                Seek(Offset, SeekOrigin.Current)
            End If

            ' Read Bytes
            ' NB: Vorbis don't give a costant buffer, so FillBuffer
            ' try to fill buffer for "count" bytes always
            nCurrentBytesReaded = FillBuffer(Buffer, Count)

            ' Update position
            nPosition = (nPosition + nCurrentBytesReaded)

            ' If there is data raise event
            If nCurrentBytesReaded > 0 Then
                RaiseEvent DataReaded_Event(Buffer)
            End If
        End If

        ' Return current bytes readed
        Return nCurrentBytesReaded
    End Function

    ' Try to fill buffer for "count" bytes
    Private Function FillBuffer(ByRef buffer() As Byte, ByVal count As Int32) As Int32
        Dim result As Int32 = 0
        Dim nCurrentSection As Integer

        Dim ByteBuffer(BUFFER_STEP - 1) As Byte
        Dim WrittenData As Int32 = 0

        Dim MemStrm As New IO.MemoryStream

        ' If there is any data remaining from previous reading
        ' add to the vuffer
        If OverflowDataLen > 0 Then
            ' Write to memory stream
            MemStrm.Write(OverflowBuffer, 0, OverflowDataLen)

            ' Update written data counter
            WrittenData += OverflowDataLen

            ' Reset overflow buffer value
            OverflowDataLen = 0
        End If

        ' Try to fill buffer BUFFER_STEP bytes per times
        Do
            result = ov_read(OggVorbisHandle, ByteBuffer, BUFFER_STEP, byte_packing.little_endian, word_size.sixteen_bit, CInt(True), nCurrentSection)

            ' Check if end of stream
            If result = 0 Then

                ' Signal end of stream
                bIsEndOfStream = True

                'Raise event 
                RaiseEvent EndOfStream_Event()
            ElseIf result > 0 Then
                ' Add bytes to stream
                MemStrm.Write(ByteBuffer, 0, result)

                ' Update written data counter
                WrittenData += result
            End If

        Loop While (WrittenData < count) And (bIsEndOfStream = False)

        ' Return to start and copy temp data to buffer
        MemStrm.Seek(0, IO.SeekOrigin.Begin)
        MemStrm.Read(buffer, 0, count)

        ' Save overflow in a buffer and write into the next buffer
        If WrittenData > count Then

            ' calculate overflow len
            OverflowDataLen = WrittenData - count

            ' Resize byte buffer
            ReDim OverflowBuffer(OverflowDataLen - 1)

            ' Seek to correct position into temp stream
            MemStrm.Seek(count, IO.SeekOrigin.Begin)

            ' Fill OverflowBuffer with data
            MemStrm.Read(OverflowBuffer, 0, OverflowDataLen)

#If DEBUG Then
            DebugPrintLine(Me.Name, "Buffer Overflow: " + OverflowDataLen.ToString)
#End If
        End If

        ' Free resource
        MemStrm.Close()
        MemStrm.Dispose()

        ' Return written data
        Return WrittenData - OverflowDataLen
    End Function

    Public Function FastStreamInformations(Path As String, ByRef info As StreamInformations) As Boolean Implements ISoundDecoder.FastStreamInformations
        Dim bResult As Boolean = False

        ' Check there are no stream opened
        If bIsOpen = False Then
            Open(Path)
            info = StreamInfo
            Close()
            bResult = True
        End If

        Return bResult
    End Function

    Public Function OpenedStreamInformations() As StreamInformations Implements ISoundDecoder.OpenedStreamInformations
        ' Return streamInfo only if there is an open stream
        If bIsOpen = True Then
            Return StreamInfo
        Else
            Return Nothing
        End If
    End Function
End Class
