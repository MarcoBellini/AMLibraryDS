Option Strict On

Imports System.IO
Imports System.Runtime.InteropServices

' Informations to compile opusfile dll at 64 bit 
' refered to 17/04/20 
' (1) Download libogg at: https://xiph.org/downloads/
' (2) Download opus at: https://www.opus-codec.org/downloads/
' (3) Download OpenSSL 1.0.2s: https://www.openssl.org/source/old/1.0.2/
'     -> NB. opusfile 0.11 compile only with this version
'        otherwise for OpenSSL 1.1.1f apply this pach:
'        https://github.com/xiph/opusfile/issues/13
' (4) Download opusfile at: https://github.com/xiph/opusfile
' (5) Extract Opusfile in C:\libOpusFile
' (6) Extract OpenSSL in C:\openssl_x64
' (7) Extract Opus in C:\libOpusFile\opus
' (8) Extract LibOgg in C:\libOpusFile\ogg
' (9) Open libogg.sln in Visual studio 2019 and compile for RELEASE_DLL x64
' (10) Open opus.sln in Visual studio 2019 and compile for RELEASE_DLL x64
' (11) to compile OpenSSL download and install http://nasm.sourceforge.net/ 
'      and http://strawberryperl.com/ for 64bit
' (11) Compile OpenSSL in x64, so open Visual Studio Command Prompt
'      and type those commands: (tutorial -> https://web.archive.org/web/20161123004257/http://developer.covenanteyes.com/building-openssl-for-visual-studio//)
'           cd C:\openssl_x64
'           perl Configure VC-WIN64A --prefix=C:\Build-OpenSSL-VC-64
'           ms\do_win64a
'           nmake -f ms\ntdll.mak
'           nmake -f ms\ntdll.mak install
'      Your outputs will be in C:\Build-OpenSSL-VC
' (12) Copy lib,ssl,include,bin folders to C:\libOpusFile\openssl
' (13) Open opusfile.sln in Visual studio 2019 and select RELEASE and x64
'      - then go in Project->Properties
'      - Change from static .lib to dynamic lib .dll
'      - update VC++ Directory->Include Dyrectory and add include directory
'        of ogg, opus and openssl. Add something like this
'           C:\LibOpusFile\opus\include
'           C:\LibOpusFile\ogg\include
'           C:\LibOpusFile\openssl\include
'           C:\LibOpusFile\openssl\ssl
'           C:\LibOpusFile\include
'      - update linker->input->additional dependecies, then add:
'           C:\LibOpusFile\ogg\win32\VS2015\x64\ReleaseDLL\libogg.lib
'           C:\LibOpusFile\opus\win32\VS2015\x64\ReleaseDLL\opus.lib
'           C:\LibOpusFile\openssl\lib\libeay32.lib
'           C:\LibOpusFile\openssl\lib\ssleay32.lib
'           Ws2_32.lib
'           Crypt32.lib
' (14) Open "opusfile.h" and add this code:
'       #ifdef OPUSFILE_EXPORTS
'           #define OPUSFILE_API __declspec(dllexport)
'       #endif
'      and add at the beginnig of each function line "OPUSFILE_API"
'      es. OPUSFILE_API typedef opus_int64 (*op_tell_func)(void *_stream);
' (15) Add OPUSFILE_EXPORTS to project -> properties -> C/C++ -> Preprocessor -> PreprocessorDefinitions
' (16) Compile only opusfile
' (17) Remember opusfile.dll depends on ogg.dll, opus.dll, ssleay32.dll and libeay32.dll

' TODO: Use CircleBuffer to perform read(?)
Public Class StreamOpus
    Implements ISoundDecoder

#Region "Native Declarations"

#If PLATFORM = "x86" Then
    ' TODO: Correct this reference to a valid library
    Private Const OpusDll As String = "x86\opusfile.dll"
    Private Const OpusConv As CallingConvention = CallingConvention.Cdecl
#Else
    Private Const OpusDll As String = "x64\opusfile.dll"
    Private Const OpusConv As CallingConvention = CallingConvention.Cdecl
#End If


    ' Main documentation on opus codec site:
    ' https://www.opus-codec.org/docs/opusfile_api-0.7/index.html


    ' OpusFile decode always at 48Khz and with op_read_stereo
    ' output always 2 channels at 16 bps
    Private OPUS_FILE_SAMPLERATE As Integer = 48000
    Private OPUS_FILE_CHANNELS As Short = 2
    Private OPUS_FILE_BITSPERSAMPLE As Short = 16
    Private OPUS_FILE_BLOCKALIGN As Short = 4
    Private OPUS_FILE_AVGBYTESPERSEC As Integer = 192000

    Private OPUS_NOERROR As Integer = 0

    ''' <summary>
    ''' The metadata from an Ogg Opus stream.
    ''' 
    ''' This Structure holds the In-stream metadata corresponding To the 
    ''' comment' header packet of an Ogg Opus stream. The comment header is 
    ''' meant to be used much like someone jotting a quick note on the label
    ''' of a CD. It should be a short, to the point text note that can be 
    ''' more than a couple words, but not more than a short paragraph.
    '''
    '''The metadata Is stored As a series Of (tag, value) pairs, In 
    '''length-encoded String vectors, Using the same format As Vorbis 
    '''(without the final "framing bit"), Theora, And Speex, except For 
    '''the packet header. The first occurrence Of the '=' character
    '''delimits the tag and value. A particular tag may occur more than once,
    '''and order is significant. The character set encoding for the strings
    '''is always UTF-8, but the tag names are limited to ASCII, and treated 
    '''as case-insensitive. See the Vorbis comment header specification for
    '''details.
    '''
    '''In filling in this structure, libopusfile will null-terminate the
    '''user_comments strings for safety. However, the bitstream format
    '''itself treats them as 8-bit clean vectors, possibly containing
    '''NUL characters, so the comment_lengths array should be treated 
    '''as their authoritative length.
    '''
    '''This Structure is binary And source-compatible With a vorbis_comment, 
    '''And pointers To it may be freely cast To vorbis_comment pointers, 
    '''And vice versa. It Is provided As a separate type To avoid introducing
    '''a compile-time dependency On the libvorbis headers.
    '''
    ''' More at opus documentation:
    ''' https://www.opus-codec.org/docs/opusfile_api-0.7/structOpusTags.html#ad53d571bd8b23691089242e4e161358a
    ''' 
    ''' </summary>
    Public Structure OpusTags
        Public user_comments As IntPtr   ' The array of comment string vectors.
        Public comment_lengths As IntPtr ' An array of the corresponding length of each vector, in bytes
        Public comments As Int32         ' The total number of comment streams
        Public vendor As IntPtr          ' This identifies the software used To encode the stream
    End Structure

    ''' <summary>
    ''' Open a stream from the given file path.
    ''' </summary>
    ''' <param name="file">Path to the file</param>
    ''' <param name="err"> Var to store error code, 0 = success</param>
    ''' <returns>
    ''' Return an IntPtr to the address of memory of the 
    ''' initialized OggOpusFile structure, NULL on error
    ''' </returns>
    <DllImport(OpusDll, CallingConvention:=OpusConv)>
    Shared Function op_open_file(ByVal file As String,
                                 ByRef err As Integer) As IntPtr
    End Function

    ''' <summary>
    ''' Release all memory used by an OggOpusFile.
    ''' </summary>
    ''' <param name="handle">
    ''' IntPtr containt address of OggOpusFile
    ''' Normally is the IntPtr returned by op_open_file function
    ''' </param>
    ''' <returns>0 on Success</returns>
    <DllImport(OpusDll, CallingConvention:=OpusConv)>
    Shared Function op_free(ByVal handle As IntPtr) As IntPtr
    End Function

    ''' <summary>
    ''' Returns whether or not the data source being read is seekable.
    ''' </summary>
    ''' <param name="handle">IntPtr containt address of OggOpusFile</param>
    ''' <returns>A non-zero value if seekable, and 0 if unseekable.</returns>
    <DllImport(OpusDll, CallingConvention:=OpusConv)>
    Shared Function op_seekable(ByVal handle As IntPtr) As Int32
    End Function

    ''' <summary>
    ''' Get the total PCM length (number of samples at 48 kHz) of the stream,
    ''' or of an individual link in a (possibly-chained) Ogg Opus stream.
    ''' </summary>
    ''' <param name="handle">IntPtr contains address of OggOpusFile</param>
    ''' <param name="index"> 
    ''' The index of the link whose PCM length should be 
    ''' computed. Use a negative number to get the PCM length of the entire
    ''' stream
    ''' </param>
    ''' <returns>The PCM length of the entire stream or PCM length of link</returns>
    <DllImport(OpusDll, CallingConvention:=OpusConv)>
    Shared Function op_pcm_total(ByVal handle As IntPtr,
                                 ByVal index As Int32) As Int64
    End Function

    ''' <summary>
    ''' Obtain the PCM offset of the next sample to be read.
    ''' </summary>
    ''' <param name="handle">IntPtr contains address of OggOpusFile</param>
    ''' <returns> The PCM offset Of the Next sample To be read </returns>
    <DllImport(OpusDll, CallingConvention:=OpusConv)>
    Shared Function op_pcm_tell(ByVal handle As IntPtr) As Int64
    End Function

    ''' <summary>
    ''' Seek to the specified PCM offset, such that decoding will begin at 
    ''' exactly the requested position.
    ''' </summary>
    ''' <param name="handle">IntPtr contains address of OggOpusFile</param>
    ''' <param name="offset">The PCM offset to seek to. This is in samples at 48 kHz relative to the start of the stream.</param>
    ''' <returns> 0 on success, or a negative value on error. </returns>
    <DllImport(OpusDll, CallingConvention:=OpusConv)>
    Shared Function op_pcm_seek(ByVal handle As IntPtr,
                                ByVal offset As Int64) As Int64
    End Function

    ''' <summary>
    ''' Reads more samples from the stream and downmixes to stereo, if necessary.
    ''' </summary>
    ''' <param name="handle">IntPtr contains address of OggOpusFile</param>
    ''' <param name="pcm_buffer">
    ''' A buffer in which to store the output PCM samples, as signed 
    ''' native-endian 16-bit values at 48 kHz with a nominal range 
    ''' of [-32768,32767). The left and right channels are interleaved 
    ''' in the buffer. This must have room for at least _buf_size values
    ''' </param>
    ''' <param name="len">
    ''' The number of values that can be stored in _pcm. It is recommended 
    ''' that this be large enough for at least 120 ms of data at 48 kHz per 
    ''' channel (11520 values total). Smaller buffers will simply return less
    ''' data, possibly consuming more memory to buffer the data internally.
    ''' If less than _buf_size values are returned, libopusfile makes no 
    ''' guarantee that the remaining data in _pcm will be unmodified.
    ''' </param>
    ''' <returns>
    ''' The number of samples read per channel on success, or a 
    ''' negative value on failure. The number of samples returned may be 
    ''' 0 if the buffer was too small to store even a single sample for
    ''' both channels, or if end-of-file was reached. The list of possible 
    ''' failure codes follows. Most of them can only be returned by
    ''' unseekable, chained streams that encounter a new link.
    ''' </returns>
    <DllImport(OpusDll, CallingConvention:=OpusConv)>
    Shared Function op_read_stereo(ByVal handle As IntPtr,
                                   ByVal pcm_buffer As IntPtr,
                                   ByVal len As Int32) As Int32
    End Function

    ''' <summary>
    ''' Get the comment header information for the given link in a (possibly
    ''' chained) Ogg Opus stream
    ''' 
    ''' This function may be called on partially-opened streams, but it will always
    ''' Return the tags from the Opus stream In the first link.
    ''' </summary>
    ''' <param name="handle">
    ''' OggOpusFile from which to retrieve the comment header
    ''' information
    '''</param>
    ''' <param name="li">
    ''' The index of the link whose comment header information should be
    ''' retrieved.
    ''' 
    ''' Use a negative number to get the comment header information of
    ''' the current link.
    ''' </param>
    ''' <returns>
    ''' The contents of the comment header for the given link, or NULL
    ''' if this is an unseekable stream that encountered an invalid link
    ''' Pointer to structure [OpusTags] is returned
    ''' </returns>
    <DllImport(OpusDll, CallingConvention:=OpusConv)>
    Shared Function op_tags(ByVal handle As IntPtr,
                            ByVal li As Int32) As IntPtr
    End Function

    ''' <summary>
    ''' Initializes an #OpusTags structure.
    ''' 
    ''' This should be called on a freshly allocated #OpusTags structure before
    ''' attempting to use it.
    ''' </summary>
    ''' <param name="OpusTagStructPtr">The #OpusTags structure to initialize</param>
    <DllImport(OpusDll, CallingConvention:=OpusConv)>
    Shared Sub opus_tags_init(ByVal OpusTagStructPtr As IntPtr)
    End Sub

    ''' <summary>
    ''' This should be called on an #OpusTags structure after it is no longer
    ''' needed.
    ''' </summary>
    ''' <param name="OpusTagStructPtr">The #OpusTags structure to clear</param>
    <DllImport(OpusDll, CallingConvention:=OpusConv)>
    Shared Sub opus_tags_clear(ByVal OpusTagStructPtr As IntPtr)
    End Sub

    ''' <summary>
    ''' Look up a comment value by its tag.
    ''' </summary>
    ''' <param name="OpusTagStructPtr">An initialized #OpusTags structure.</param>
    ''' <param name="tag">The tag to look up.</param>
    ''' <param name="count">The instance of the tag, use 0 to read first occurence</param>
    ''' <returns>The pointer of the queried tag's value</returns>
    <DllImport(OpusDll, CallingConvention:=OpusConv)>
    Shared Function opus_tags_query(ByVal OpusTagStructPtr As IntPtr,
                                    ByVal tag As String,
                                    ByVal count As Int32) As IntPtr
    End Function



#End Region

    ' Buffer step in bytes for fill buffer stream
    Private Const BUFFER_STEP As Integer = 1024

    Private pOpusFileHandle As IntPtr = IntPtr.Zero
    Private bIsOpen As Boolean = False
    Private bIsEndOfStream As Boolean = False

    Private nDuration As Long = 0
    Private nPosition As Long = 0

    Private StreamInfo As StreamInformations

    Private OverflowBuffer() As Byte
    Private OverflowDataLen As Integer = 0

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
                ".opus"
            }

            Return Supported
        End Get
    End Property

    Public ReadOnly Property Name As String Implements ISoundDecoder.Name
        Get
            Return "OPUS file Decoder 0.1a"
        End Get
    End Property

    Public ReadOnly Property EndOfStream As Boolean Implements ISoundDecoder.EndOfStream
        Get
            Return bIsEndOfStream
        End Get
    End Property

    Public Sub Close() Implements ISoundDecoder.Close
        If bIsOpen = True Then
            ' Free resource of opus file
            op_free(pOpusFileHandle)
            pOpusFileHandle = IntPtr.Zero

            ' Free other resources
            StreamInfo = Nothing

            bIsEndOfStream = False

            nDuration = 0
            nPosition = 0
            OverflowDataLen = 0

            bIsOpen = False
        End If
    End Sub

    Public Function Open(Path As String) As Boolean Implements ISoundDecoder.Open
        Dim bResult As Boolean = False
        Dim nErrorCode As Integer

        ' If stream is open, then close
        If bIsOpen = True Then
            Me.Close()
        End If

        ' Check if file exist
        If My.Computer.FileSystem.FileExists(Path) = True Then

            pOpusFileHandle = op_open_file(Path, nErrorCode)

            ' Check if stream is open correctly
            If (nErrorCode = OPUS_NOERROR) And (pOpusFileHandle <> IntPtr.Zero) Then

                'Create new instace
                StreamInfo = New StreamInformations()

                ' Fill with basic file informations
                StreamInfo.FillBasicFileInfo(Path)

                ' Fill stream informations
                StreamInfo.Samplerate = OPUS_FILE_SAMPLERATE
                StreamInfo.Channels = OPUS_FILE_CHANNELS
                StreamInfo.BitsPerSample = OPUS_FILE_BITSPERSAMPLE
                StreamInfo.BlockAlign = OPUS_FILE_BLOCKALIGN
                StreamInfo.AvgBytesPerSec = OPUS_FILE_AVGBYTESPERSEC

                ' Find Tags 
                FindID3Tags()

                ' Find Duration in bytes
                nDuration = op_pcm_total(pOpusFileHandle, -1) * StreamInfo.BlockAlign
                nPosition = 0

                StreamInfo.DurationInMs = nDuration / StreamInfo.AvgBytesPerSec * 1000

                ' Open succesful
                bIsOpen = True
                bResult = True
            End If
        End If

        Return bResult
    End Function

    ' Find tags
    Private Sub FindID3Tags()
        Dim ID3Ptr As IntPtr = IntPtr.Zero
        Dim TAGPointer As IntPtr = IntPtr.Zero

        ' Check there is a valid handle
        If pOpusFileHandle <> IntPtr.Zero Then
            ID3Ptr = op_tags(pOpusFileHandle, -1)

            ' Check there is a valid handle
            If ID3Ptr <> IntPtr.Zero Then

                ' Artist
                TAGPointer = opus_tags_query(ID3Ptr, "artist", 0)
                If TAGPointer <> IntPtr.Zero Then
                    StreamInfo.Artist = Marshal.PtrToStringAnsi(TAGPointer)
                End If

                ' Title
                TAGPointer = opus_tags_query(ID3Ptr, "title", 0)
                If TAGPointer <> IntPtr.Zero Then
                    StreamInfo.Title = Marshal.PtrToStringAnsi(TAGPointer)
                End If

                ' Album
                TAGPointer = opus_tags_query(ID3Ptr, "album", 0)
                If TAGPointer <> IntPtr.Zero Then

                    StreamInfo.Album = Marshal.PtrToStringAnsi(TAGPointer)
                End If

                ' Genre
                TAGPointer = opus_tags_query(ID3Ptr, "genre", 0)
                If TAGPointer <> IntPtr.Zero Then
                    StreamInfo.Genre = Marshal.PtrToStringAnsi(TAGPointer)
                End If

                ' Comment
                TAGPointer = opus_tags_query(ID3Ptr, "comment", 0)
                If TAGPointer <> IntPtr.Zero Then
                    StreamInfo.Comment = Marshal.PtrToStringAnsi(TAGPointer)
                End If

                ' Year
                TAGPointer = opus_tags_query(ID3Ptr, "date", 0)
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
        If op_pcm_seek(pOpusFileHandle, nUpdatePosition) = OPUS_NOERROR Then
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
            ' NB: Opusfile don't give a costant buffer, so FillBuffer
            ' try to fill buffer for "count" bytes always
            nCurrentBytesReaded = FillBuffer(Buffer, Count)

            ' Update position
            nPosition = (nPosition + nCurrentBytesReaded)

            ' If there is data raise event
            If nCurrentBytesReaded <> 0 Then
                RaiseEvent DataReaded_Event(Buffer)
            End If
        End If

        ' Return current bytes readed
        Return nCurrentBytesReaded
    End Function

    ' Try to fill buffer for "count" bytes
    Private Function FillBuffer(ByRef buffer() As Byte, ByVal count As Int32) As Int32
        Dim result As Int32 = 0

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
            'Read data in byte
            result = ReadData(ByteBuffer, BUFFER_STEP)

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

    ' Opus read 16 bit samples, so convert from Short to Byte array
    Private Function ReadData(ByRef buffer() As Byte, ByVal count As Int32) As Int32
        Dim gc As GCHandle
        Dim shortBuffer() As Short
        Dim nDataReaded As Integer = 0

        ' To read "count" bytes = 8 bits, must read "count \ 2" short = 16bits
        ReDim shortBuffer(count \ 2 - 1)
        count = count \ 2

        ' Alloc memory for short buffer
        gc = GCHandle.Alloc(shortBuffer, GCHandleType.Pinned)

        ' Read PCM samples, the function return DataReaded \ Channels
        nDataReaded = op_read_stereo(pOpusFileHandle, gc.AddrOfPinnedObject(), count)

        ' Align Data Readed to 2 channels
        nDataReaded = nDataReaded * OPUS_FILE_CHANNELS

        ' Free resources
        gc.Free()

        ' Convert short = 16 bits into 2 bytes = 8 bits
        For i As Integer = 0 To nDataReaded - 1
            buffer(i * 2 + 0) = short_to_byte(shortBuffer(i))(0)
            buffer(i * 2 + 1) = short_to_byte(shortBuffer(i))(1)
        Next

        ' Data readed count how many Short Samples are readed,
        ' so bytes are  2 bytes for 1 short (2:1)
        nDataReaded = nDataReaded * 2

        ' Return data readed
        Return nDataReaded
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
        If bIsOpen = True Then
            Return StreamInfo
        Else
            Return Nothing
        End If
    End Function
End Class
