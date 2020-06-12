Option Strict On

Imports System.Runtime.InteropServices
Imports System.IO

Public Class StreamMP3
    Implements ISoundDecoder

#Region "Native declarations"

    ' Select Dll based on compile settings
#If PLATFORM = "x86" Then
    ' Dll name in app folder
    Private Const MPG123_DLL As String = "x86\libmpg123.dll"
#ElseIf PLATFORM = "x64" Then
    ' Dll name in app folder
    Private Const MPG123_DLL As String = "x64\libmpg123.dll"
#End If

    'Calling convention of MPG123
    Private Const CLLCNV As CallingConvention = CallingConvention.Cdecl

    ''' <summary>
    ''' Enumeration of the parameters types that it is possible to set/get.
    ''' </summary>
    Public Enum mpg123_parms
        MPG123_VERBOSE = 0      '/**< Set verbosity value For enabling messages To stderr, >= 0 makes sense (Integer) */
        MPG123_FLAGS            '/**< Set all flags, p.ex val = MPG123_GAPLESS|MPG123_MONO_MIX (Integer) */
        MPG123_ADD_FLAGS        '/**< add some flags (Integer) */
        MPG123_FORCE_RATE       '/**< When value > 0, force output rate To that value (Integer) */
        MPG123_DOWN_SAMPLE      '/**< 0=native rate, 1=half rate, 2=quarter rate (Integer) */
        MPG123_RVA              '/**< one Of the RVA choices above (Integer) */
        MPG123_DOWNSPEED        '/**< play a frame N times (Integer) */
        MPG123_UPSPEED          '/**< play every Nth frame (Integer) */
        MPG123_START_FRAME      '/**< start With this frame (skip frames before that, Integer) */ 
        MPG123_DECODE_FRAMES    '/**< decode only this number Of frames (Integer) */
        MPG123_ICY_INTERVAL     '/**< stream contains ICY metadata With this interval (Integer) */
        MPG123_OUTSCALE         '/**< the scale For output samples (amplitude - Integer Or float according To mpg123 output format, normally Integer) */
        MPG123_TIMEOUT          '/**< timeout For reading from a stream (Not supported On win32, Integer) */
        MPG123_REMOVE_FLAGS     '/**< remove some flags (inverse Of MPG123_ADD_FLAGS, Integer) */
        MPG123_RESYNC_LIMIT     '/**< Try resync On frame parsing For that many bytes Or until End Of stream (<0 ... Integer). This can enlarge the limit For skipping junk On beginning, too (but Not reduce it).  */
        MPG123_INDEX_SIZE       '/**< Set the frame index size (If supported). Values <0 mean that the index Is allowed To grow dynamically In these steps (In positive direction, Of course) -- Use this When you really want a full index With every individual frame. */
        MPG123_PREFRAMES        '/**< Decode/ignore that many frames in advance for layer 3. This Is needed to fill bit reservoir after seeking, for example (but also at least one frame in advance Is needed to have all "normal" data for layer 3). Give a positive integer value, please.*/
        MPG123_FEEDPOOL         '/**< For feeder mode, keep that many buffers in a pool to avoid frequent malloc/free. The pool Is allocated on mpg123_open_feed(). If you change this parameter afterwards, you can trigger growth And shrinkage during decoding. The default value could change any time. If you care about this, then set it. (integer) */
        MPG123_FEEDBUFFER       '/**< Minimal size of one internal feeder buffer, again, the default value Is subject to change. (integer) */
    End Enum


    ''' <summary>
    ''' Flag bits for MPG123_FLAGS, use the usual binary or to combine.
    ''' </summary>
    Public Enum mpg123_param_flags
        MPG123_FORCE_MONO = &H7     '*<0111 Force some mono mode: This is a test bitmask for seeing if any mono forcing is active. */
        MPG123_MONO_LEFT = &H1      '*<0001 Force playback of left channel only.  */
        MPG123_MONO_RIGHT = &H2     '*<0010 Force playback of right channel only. */
        MPG123_MONO_MIX = &H4       '*<0100 Force playback of mixed mono.         */
        MPG123_FORCE_STEREO = &H8   '*<1000 Force stereo output.                  */
        MPG123_FORCE_8BIT = &H10    '*<00010000 Force 8bit formats.                   */
        MPG123_QUIET = &H20         '*<00100000 Suppress any printouts (overrules verbose).                    */
        MPG123_GAPLESS = &H40       '*<01000000 Enable gapless decoding (default on if libmpg123 has support). */
        MPG123_NO_RESYNC = &H80     '*<10000000 Disable resync stream after error.                             */
        MPG123_SEEKBUFFER = &H100   '*<000100000000 Enable small buffer on non-seekable streams to allow some peek-ahead (for better MPEG sync). */
        MPG123_FUZZY = &H200        '*<001000000000 Enable fuzzy seeks (guessing byte offsets or using approximate seek points from Xing TOC) */
        MPG123_FORCE_FLOAT = &H400   '*<010000000000 Force floating point output (32 or 64 bits depends on mpg123 internal precision). */
        MPG123_PLAIN_ID3TEXT = &H800 '*<100000000000 Do not translate ID3 text data to UTF-8. ID3 strings will contain the raw text data  with the first byte containing the ID3 encoding code. */
        MPG123_IGNORE_STREAMLENGTH = &H1000 '*<1000000000000 Ignore any stream length information contained in the stream  which can be contained in a 'TLEN' frame of an ID3v2 tag or a Xing tag */
        MPG123_SKIP_ID3V2 = &H2000          '*<10 0000 0000 0000 Do not parse ID3v2 tags  just skip them. */
        MPG123_IGNORE_INFOFRAME = &H4000    '*<100 0000 0000 0000 Do not parse the LAME/Xing info frame  treat it as normal MPEG data. */
        MPG123_AUTO_RESAMPLE = &H8000       '*<1000 0000 0000 0000 Allow automatic internal resampling of any kind (default on if supported). Especially when going lowlevel with replacing output buffer  you might want to unset this flag. Setting MPG123_DOWNSAMPLE or MPG123_FORCE_RATE will override this. */
        MPG123_PICTURE = &H10000            '*<17th bit: Enable storage of pictures from tags (ID3v2 APIC). */
        MPG123_NO_PEEK_END = &H20000        '*<18th bit: Do not seek to the end of
        '*  the stream in order to probe
        '*  the stream length and search for the id3v1 field. This also means
        '*  the file size is unknown unless set using mpg123_set_filesize() and
        '*  the stream is assumed as non-seekable unless overridden.
        '*/
        MPG123_FORCE_SEEKABLE = &H40000     '*< 19th bit: Force the stream to be seekable. */
    End Enum

    ''' <summary>
    ''' Choices for MPG123_RVA
    ''' </summary>
    Public Enum mpg123_param_rva
        MPG123_RVA_OFF = 0                  '/**< RVA disabled (default).   */
        MPG123_RVA_MIX = 1                  '/**< Use mix/track/radio gain. */
        MPG123_RVA_ALBUM = 2                '/**< Use album/audiophile gain */
        MPG123_RVA_MAX = MPG123_RVA_ALBUM   '/**< The maximum RVA code, may increase in future. */
    End Enum

    ''' <summary>
    '''  Feature set available for query with mpg123_feature.
    ''' </summary>
    Public Enum mpg123_feature_set
        MPG123_FEATURE_ABI_UTF8OPEN = 0      '*< mpg123 expects path names to be given in UTF-8 encoding instead of plain native. */
        MPG123_FEATURE_OUTPUT_8BIT           '*< 8bit output   */
        MPG123_FEATURE_OUTPUT_16BIT          '*< 16bit output  */
        MPG123_FEATURE_OUTPUT_32BIT          '*< 32bit output  */
        MPG123_FEATURE_INDEX                 '*< support for building a frame index for accurate seeking */
        MPG123_FEATURE_PARSE_ID3V2           '*< id3v2 parsing */
        MPG123_FEATURE_DECODE_LAYER1         '*< mpeg layer-1 decoder enabled */
        MPG123_FEATURE_DECODE_LAYER2         '*< mpeg layer-2 decoder enabled */
        MPG123_FEATURE_DECODE_LAYER3         '*< mpeg layer-3 decoder enabled */
        MPG123_FEATURE_DECODE_ACCURATE       '*< accurate decoder rounding    */
        MPG123_FEATURE_DECODE_DOWNSAMPLE     '*< downsample (sample omit)     */
        MPG123_FEATURE_DECODE_NTOM           '*< flexible rate decoding       */
        MPG123_FEATURE_PARSE_ICY             '*< ICY support                  */
        MPG123_FEATURE_TIMEOUT_READ          '*< Reader with timeout (network). */
        MPG123_FEATURE_EQUALIZER             '*< tunable equalizer */
    End Enum

    ''' <summary>
    ''' Enumeration of the message and error codes and returned by libmpg123 functions.
    ''' </summary>
    Public Enum mpg123_errors
        MPG123_DONE = -12        '*< Message: Track ended. Stop decoding. */
        MPG123_NEW_FORMAT = -11  '*< Message: Output format will be different on next call. Note that some libmpg123 versions between 1.4.3 and 1.8.0 insist on you calling mpg123_getformat() after getting this message code. Newer verisons behave like advertised: You have the chance to call mpg123_getformat(), but you can also just continue decoding and get your data. */
        MPG123_NEED_MORE = -10   '*< Message: For feed reader: "Feed me more!" (call mpg123_feed() or mpg123_decode() with some new input data). */
        MPG123_ERR = -1          '*< Generic Error */
        MPG123_OK = 0            '*< Success */
        MPG123_BAD_OUTFORMAT     '*< Unable to set up output format! */
        MPG123_BAD_CHANNEL       '*< Invalid channel number specified. */
        MPG123_BAD_RATE          '*< Invalid sample rate specified.  */
        MPG123_ERR_16TO8TABLE    '*< Unable to allocate memory for 16 to 8 converter table! */
        MPG123_BAD_PARAM         '*< Bad parameter id! */
        MPG123_BAD_BUFFER        '*< Bad buffer given -- invalid pointer or too small size. */
        MPG123_OUT_OF_MEM        '*< Out of memory -- some malloc() failed. */
        MPG123_NOT_INITIALIZED   '*< You didn't initialize the library! */
        MPG123_BAD_DECODER       '*< Invalid decoder choice. */
        MPG123_BAD_HANDLE        '*< Invalid mpg123 handle. */
        MPG123_NO_BUFFERS        '*< Unable to initialize frame buffers (out of memory?). */
        MPG123_BAD_RVA           '*< Invalid RVA mode. */
        MPG123_NO_GAPLESS        '*< This build doesn't support gapless decoding. */
        MPG123_NO_SPACE          '*< Not enough buffer space. */
        MPG123_BAD_TYPES         '*< Incompatible numeric data types. */
        MPG123_BAD_BAND          '*< Bad equalizer band. */
        MPG123_ERR_NULL          '*< Null pointer given where valid storage address needed. */
        MPG123_ERR_READER        '*< Error reading the stream. */
        MPG123_NO_SEEK_FROM_END  '*< Cannot seek from end (end is not known). */
        MPG123_BAD_WHENCE        '*< Invalid 'whence' for seek function.*/
        MPG123_NO_TIMEOUT        '*< Build does not support stream timeouts. */
        MPG123_BAD_FILE          '*< File access error. */
        MPG123_NO_SEEK           '*< Seek not supported by stream. */
        MPG123_NO_READER         '*< No stream opened. */
        MPG123_BAD_PARS          '*< Bad parameter handle. */
        MPG123_BAD_INDEX_PAR     '*< Bad parameters to mpg123_index() and mpg123_set_index() */
        MPG123_OUT_OF_SYNC       '*< Lost track in bytestream and did not try to resync. */
        MPG123_RESYNC_FAIL       '*< Resync failed to find valid MPEG data. */
        MPG123_NO_8BIT           '*< No 8bit encoding possible. */
        MPG123_BAD_ALIGN         '*< Stack aligmnent error */
        MPG123_NULL_BUFFER       '*< NULL input buffer with non-zero size... */
        MPG123_NO_RELSEEK        '*< Relative seek not possible (screwed up file offset) */
        MPG123_NULL_POINTER      '*< You gave a null pointer somewhere where you shouldn't have. */
        MPG123_BAD_KEY           '*< Bad key value given. */
        MPG123_NO_INDEX          '*< No frame index in this build. */
        MPG123_INDEX_FAIL        '*< Something with frame index went wrong. */
        MPG123_BAD_DECODER_SETUP '*< Something prevents a proper decoder setup */
        MPG123_MISSING_FEATURE   '*< This feature has not been built into libmpg123. */
        MPG123_BAD_VALUE         '*< A bad value has been given  somewhere. */
        MPG123_LSEEK_FAILED      '*< Low-level seek failed. */
        MPG123_BAD_CUSTOM_IO     '*< Custom I/O not prepared. */
        MPG123_LFS_OVERFLOW      '*< Offset value overflow during translation of large file API calls -- your client program cannot handle that large file. */
        MPG123_INT_OVERFLOW      '*< Some integer overflow. */
    End Enum

    Public Enum mpg123_enc_enum As Short
        MPG123_ENC_16 = &H40
        MPG123_ENC_SIGNED = &H80
        MPG123_ENC_8 = &HF
        MPG123_ENC_SIGNED_16 = (MPG123_ENC_16 Or MPG123_ENC_SIGNED Or &H10)
        MPG123_ENC_UNSIGNED_16 = (MPG123_ENC_16 Or &H20)
        MPG123_ENC_UNSIGNED_8 = &H1
        MPG123_ENC_SIGNED_8 = (MPG123_ENC_SIGNED Or &H2)
        MPG123_ENC_ULAW_8 = &H4
        MPG123_ENC_ALAW_8 = &H8
        MPG123_ENC_ANY = (MPG123_ENC_SIGNED_16 Or MPG123_ENC_UNSIGNED_16 Or MPG123_ENC_UNSIGNED_8 Or MPG123_ENC_SIGNED_8 Or MPG123_ENC_ULAW_8 Or MPG123_ENC_ALAW_8)
    End Enum

    ''' <summary>
    ''' They can be combined into one number (3) to indicate mono and stereo.
    ''' </summary>
    Public Enum mpg123_channelcount
        MPG123_MONO = 1     '*< mono */
        MPG123_STEREO = 2   '*< Stereo */
    End Enum

    Public Enum mpg123_channels
        MPG123_LEFT = &H1
        MPG123_RIGHT = &H2
        MPG123_LR = &H3
    End Enum

    ''' <summary>
    '''  Enumeration of the mode types of Variable Bitrate 
    ''' </summary>
    Public Enum mpg123_vbr
        MPG123_CBR = 0  ' Constant Bitrate Mode (default)
        MPG123_VBR      ' Variable Bitrate Mode
        MPG123_ABR      ' Average Bitrate Mode
    End Enum

    ''' <summary>
    ''' Enumeration of the MPEG Versions
    ''' </summary>
    Public Enum mpg123_version
        MPG123_1_0 = 0  '< MPEG Version 1.0
        MPG123_2_0      '< MPEG Version 2.0
        MPG123_2_5      '< MPEG Version 2.5
    End Enum

    ''' <summary>
    ''' Enumeration of the MPEG Audio mode.
    ''' Only the mono mode has 1 channel, the others have 2 channels
    ''' </summary>
    Public Enum mpg123_mode
        MPG123_M_STEREO = 0 ' < Standard Stereo.
        MPG123_M_JOINT      ' < Joint Stereo.
        MPG123_M_DUAL       ' < Dual Channel.
        MPG123_M_MONO       ' < Single Channel.
    End Enum

    ''' <summary>
    '''  Enumeration of the MPEG Audio flag bits
    ''' </summary>
    Public Enum mpg123_flags
        MPG123_CRC = &H1        ' < The bitstream is error protected using 16-bit CRC.
        MPG123_COPYRIGHT = &H2  ' < The bitstream is copyrighted.
        MPG123_PRIVATE = &H4    ' < The private bit has been set.
        MPG123_ORIGINAL = &H8   ' < The bitstream is an original, not a copy.
    End Enum

    ''' <summary>
    ''' Set the seek origin
    ''' </summary>
    Public Enum SEEK_ORIGIN
        SEEK_SET = 0
        SEEK_CUR
        SEEK_END
    End Enum

    ''' <summary>
    ''' Data structure for storing information about a frame of MPEG Audio
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)>
    Public Structure mpg123_frameinfo
        Public version As mpg123_version    ' < The MPEG version (1.0/2.0/2.5). */
        Public layer As Int32               ' < The MPEG Audio Layer (MP1/MP2/MP3).
        Public rate As Long                 ' < The sampling rate in Hz.
        Public mode As mpg123_mode          ' < The audio mode (Mono, Stereo, Joint-stero, Dual Channel). 
        Public mode_ext As Int32            ' < The mode extension bit flag.
        Public framesize As Int32           ' < The size of the frame (in bytes, including header).
        Public flags As mpg123_flags        ' < MPEG Audio flag bits. Just now I realize that it should be declared as int, not enum. It's a bitwise combination of the enum values
        Public emphasis As Int32            ' < The emphasis type. 
        Public bitrate As Int32             ' < Bitrate of the frame (kbps).
        Public abr_rate As Int32            ' < The target average bitrate.
        Public vbr As mpg123_vbr            ' < The VBR mode. 
    End Structure

    ''' <summary>
    ''' Data structure for storing strings in a safer way than a standard C-String
    ''' Can also hold a number of null-terminated strings.
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)>
    Public Structure mpg123_string
        ''' <summary>
        ''' pointer to the string data
        ''' </summary>
        Public p As String

        ''' <summary>
        ''' raw number of bytes allocated
        ''' </summary>
        Public size As UInt32

        ''' <summary>
        ''' number of used bytes (including closing zero byte)
        ''' </summary>
        Public fill As UInt32
    End Structure

    ''' <summary>
    ''' The mpg123 text encodings. This contains encodings we encounter in ID3 tags or ICY meta info.
    ''' </summary>
    Public Enum mpg123_text_encoding
        mpg123_text_unknown = 0 ' < Unkown encoding... mpg123_id3_encoding can return that on invalid codes. */
        mpg123_text_utf8 = 1    ' < UTF-8 */
        mpg123_text_latin1 = 2  ' < ISO-8859-1. Note that sometimes latin1 in ID3 is abused for totally different encodings. */
        mpg123_text_icy = 3     ' < ICY metadata encoding  usually CP-1252 but we take it as UTF-8 if it qualifies as such. */
        mpg123_text_cp1252 = 4  ' < Really CP-1252 without any guessing. */
        mpg123_text_utf16 = 5   ' < Some UTF-16 encoding. The last of a set of leading BOMs (byte order mark) rules.
        '*   When there is no BOM  big endian ordering is used. Note that UCS-2 qualifies as UTF-8 when
        ' *   you don't mess with the reserved code points. If you want to decode little endian data
        '*   without BOM you need to prepend 0xff 0xfe yourself. */
        mpg123_text_utf16bom = 6    ' < Just an alias for UTF-16  ID3v2 has this as distinct code. */
        mpg123_text_utf16be = 7     ' < Another alias for UTF16 from ID3v2. Note  that  because of the mess that is reality 
        '*   BOMs are used if encountered. There really is not much distinction between the UTF16 types for mpg123
        ' *   One exception: Since this is seen in ID3v2 tags  leading null bytes are skipped for all other UTF16
        ' *   types (we expect a BOM before real data there)  not so for utf16be!*/
        mpg123_text_max = 7 '        < Placeholder for the maximum encoding value. */
    End Enum

    ''' <summary>
    ''' The encoding byte values from ID3v2.
    ''' </summary>
    Public Enum mpg123_id3_enc
        mpg123_id3_latin1 = 0 '   < Note: This sometimes can mean anything in practice... */
        mpg123_id3_utf16bom = 1 ' < UTF16, UCS-2 ... it's all the same for practical purposes. */
        mpg123_id3_utf16be = 2 '  < Big-endian UTF-16, BOM see note for mpg123_text_utf16be. */
        mpg123_id3_utf8 = 3 '     < Our lovely overly ASCII-compatible 8 byte encoding for the world. */
        mpg123_id3_enc_max = 3 '  < Placeholder to check valid range of encoding byte. */
    End Enum

    ''' <summary>
    '''  Sub data structure for ID3v2, for storing various text fields (including comments).
    '''  This is for ID3v2 COMM, TXXX and all the other text fields.
    '''  Only COMM and TXXX have a description, only COMM and USLT have a language.
    '''  You should consult the ID3v2 specification for the use of the various text fields ("frames" in ID3v2 documentation, I use "fields" here to separate from MPEG frames). 
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)>
    Public Structure mpg123_text
        ''' <summary>
        ''' Three-letter language code (not terminated).
        ''' </summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=3)>
        Public lang As String

        ''' <summary>
        ''' The ID3v2 text field id, like TALB, TPE2, ... (4 characters, no string termination).
        ''' </summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)>
        Public id As String

        ''' <summary>
        ''' Empty for the generic comment
        ''' </summary>
        Public description As mpg123_string

        ''' <summary>
        ''' Text of comment
        ''' </summary>
        Public text As mpg123_string
    End Structure

    ''' <summary>
    ''' The picture type values from ID3v2.
    ''' </summary>
    Public Enum mpg123_id3_pic_type
        mpg123_id3_pic_other = 0 ' < see ID3v2 docs */
        mpg123_id3_pic_icon = 1 ' < see ID3v2 docs */
        mpg123_id3_pic_other_icon = 2 ' < see ID3v2 docs */
        mpg123_id3_pic_front_cover = 3 ' < see ID3v2 docs */
        mpg123_id3_pic_back_cover = 4 ' < see ID3v2 docs */
        mpg123_id3_pic_leaflet = 5 ' < see ID3v2 docs */
        mpg123_id3_pic_media = 6 ' < see ID3v2 docs */
        mpg123_id3_pic_lead = 7 ' < see ID3v2 docs */
        mpg123_id3_pic_artist = 8 ' < see ID3v2 docs */
        mpg123_id3_pic_conductor = 9 ' < see ID3v2 docs */
        mpg123_id3_pic_orchestra = 10 ' < see ID3v2 docs */
        mpg123_id3_pic_composer = 11 ' < see ID3v2 docs */
        mpg123_id3_pic_lyricist = 12 ' < see ID3v2 docs */
        mpg123_id3_pic_location = 13 ' < see ID3v2 docs */
        mpg123_id3_pic_recording = 14 ' < see ID3v2 docs */
        mpg123_id3_pic_performance = 15 ' < see ID3v2 docs */
        mpg123_id3_pic_video = 16 ' < see ID3v2 docs */
        mpg123_id3_pic_fish = 17 ' < see ID3v2 docs */
        mpg123_id3_pic_illustration = 18 ' < see ID3v2 docs */
        mpg123_id3_pic_artist_logo = 19 ' < see ID3v2 docs */
        mpg123_id3_pic_publisher_logo = 20 ' < see ID3v2 docs */
    End Enum

    ''' <summary>
    ''' Sub data structure for ID3v2, for storing picture data including comment.
    ''' This is for the ID3v2 APIC field. You should consult the ID3v2 specification
    ''' for the use of the APIC field ("frames" in ID3v2 documentation, I use "fields"
    ''' here to separate from MPEG frames).
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)>
    Public Structure mpg123_picture
        Public type As mpg123_id3_pic_type  ' < mpg123_id3_pic_type value
        Public description As mpg123_string ' < description string
        Public mime_type As mpg123_string   ' < MIME type
        Public size As UInt32               ' < size in bytes
        Public data As IntPtr               ' < pointer to the image data
    End Structure

    ''' <summary>
    ''' Data structure for storing IDV3v2 tags.
    ''' This structure is not a direct binary mapping with the file contents.
    ''' The ID3v2 text frames are allowed to contain multiple strings.
    ''' So check for null bytes until you reach the mpg123_string fill.
    ''' All text is encoded in UTF-8. */
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)>
    Public Structure mpg123_id3v2
        ''' <summary>
        ''' 3 Or 4 for ID3v2.3 Or ID3v2.4. 
        ''' </summary>
        Public version As Byte

        ''' <summary>
        ''' Title string (pointer into text_list) [mpg123_string]
        ''' </summary>
        Public title As IntPtr

        ''' <summary>
        ''' Artist string (pointer into text_list).
        ''' </summary>
        Public artist As IntPtr

        ''' <summary>
        ''' Album string (pointer into text_list) [mpg123_string]
        ''' </summary>
        Public album As IntPtr

        ''' <summary>
        ''' The year as a string (pointer into text_list) [mpg123_string]
        ''' </summary>
        Public year As IntPtr

        ''' <summary>
        ''' Genre String (pointer into text_list). 
        ''' The genre string(s) may very well need postprocessing, esp. 
        ''' for ID3v2.3. [mpg123_string]
        ''' </summary>
        Public genre As IntPtr

        ''' <summary>
        ''' Pointer to last encountered comment text with empty description [mpg123_string]
        ''' </summary>
        Public comment As IntPtr

        ''' <summary>
        ''' Array of comments.
        ''' </summary>
        Public comment_list As IntPtr

        ''' <summary>
        ''' Number Of comments
        ''' </summary>
        Public comments As UInt32

        ''' <summary>
        ''' Array of ID3v2 text fields (including USLT)
        ''' </summary>
        Public text As IntPtr

        ''' <summary>
        ''' Numer Of text fields.
        ''' </summary>
        Public texts As UInt32

        ''' <summary>
        ''' The array of extra (TXXX) fields
        ''' </summary>
        Public extra As IntPtr

        ''' <summary>
        ''' Number Of extra text (TXXX) fields
        ''' </summary>
        Public extras As UInt32

        ''' <summary>
        ''' Array of ID3v2 pictures fields (APIC).
        ''' </summary>
        Public picture As IntPtr

        ''' <summary>
        ''' Number Of picture (APIC) fields.
        ''' </summary>
        Public pictures As UInt32
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure mpg123_id3v1
        ''' <summary>
        ''' Always the String "TAG", the classic intro.
        ''' </summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=3)>
        Public tag As String

        ''' <summary>
        ''' Title string..
        ''' </summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=30)>
        Public title As String

        ''' <summary>
        ''' Artist string..
        ''' </summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=30)>
        Public artist As String

        ''' <summary>
        ''' Album string..
        ''' </summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=30)>
        Public album As String

        ''' <summary>
        ''' Year string..
        ''' </summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)>
        Public year As String

        ''' <summary>
        ''' Comment string..
        ''' </summary>
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=30)>
        Public comment As String

        ''' <summary>
        ''' Genre index
        ''' </summary>
        Public genre As Byte
    End Structure

    ''' <summary>
    ''' Point v1 and v2 to existing data structures wich may change on any next read/decode function call.
    ''' v1 and/or v2 can be set to NULL when there is no corresponding data.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <param name="v1">ID3v1 structure ref</param>
    ''' <param name="v2">ID3v2 structure ref</param>
    ''' <returns>MPG123_OK on success</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_id3(
                                     ByVal mh As IntPtr,
                                     ByRef v1 As IntPtr,
                                     ByRef v2 As IntPtr) As Integer
    End Function

    ''' <summary>
    ''' Function to initialise the mpg123 library. 
    ''' This function is not thread-safe. Call it exactly once per process, 
    ''' before any other (possibly threaded) work with the library.
    ''' </summary>
    ''' <returns>MPG123_OK if successful, otherwise an error number</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_init() As Integer
    End Function

    ''' <summary>
    ''' Function to close down the mpg123 library. 
    ''' This function is not thread-safe. Call it 
    ''' exactly once per process, before any other (possibly threaded) 
    ''' work with the library. 
    ''' </summary>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Sub mpg123_exit()
    End Sub

    ''' <summary>
    ''' Open and prepare to decode the specified file by filesystem path.
    ''' This does Not open HTTP urls; libmpg123 contains no networking code.
    ''' If you want to decode internet streams, use mpg123_open_fd() Or mpg123_open_feed().
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <param name="path">filesystem path</param>
    ''' <returns>MPG123_OK on success</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_open(
                           ByVal mh As IntPtr,
                           ByVal path As String) As Integer

    End Function

    ''' <summary>
    ''' Use an already opened file descriptor as the bitstream input
    ''' mpg123_close() will _not_ close the file descriptor.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <param name="fd">file descriptor</param>
    ''' <returns>MPG123_OK on success</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_open_fd(
                           ByVal mh As IntPtr,
                           ByVal fd As Integer) As Integer

    End Function

    ''' <summary>
    ''' Open a new bitstream and prepare for direct feeding
    ''' This works together with mpg123_decode(); you are responsible 
    ''' for reading and feeding the input bitstream.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <returns>MPG123_OK on success</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_open_feed(
                            ByVal mh As IntPtr
                            ) As Integer

    End Function

    ''' <summary>
    ''' Closes the source, if libmpg123 opened it
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <returns>MPG123_OK on success</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_close(
                           ByVal mh As IntPtr) As Integer
    End Function

    ''' <summary>
    ''' Create a handle with optional choice of decoder (named by 
    ''' a string, see mpg123_decoders() or mpg123_supported_decoders())
    ''' and optional retrieval of an error code to feed to mpg123_plain_strerror().
    ''' Optional means: Any of or both the parameters may be NULL.
    ''' </summary>
    ''' <param name="Decoder">optional choice of decoder variant (NULL for default)</param>
    ''' <param name="errors">optional address to store error codes</param>
    ''' <returns>Non-NULL pointer to fresh handle when successful.</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_new(
                           ByVal Decoder As String,
                           ByRef errors As Int32) As IntPtr

    End Function

    ''' <summary>
    ''' Delete handle, mh is either a valid mpg123 handle or NULL.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Sub mpg123_delete(
                       ByVal mh As IntPtr)

    End Sub

    ''' <summary>
    ''' Get the current output format written to the addresses given.
    ''' If the stream Is freshly loaded, this will try to parse enough
    ''' of it to give you the format to come. This clears the flag that
    ''' would otherwise make the first decoding call return
    '''  MPG123_NEW_FORMAT.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <param name="rate">sampling rate return address</param>
    ''' <param name="channels">channel count return address</param>
    ''' <param name="encoding">encoding return address</param>
    ''' <returns>MPG123_OK on success</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_getformat(
                           ByVal mh As IntPtr,
                           ByRef rate As Int32,
                           ByRef channels As Int16,
                           ByRef encoding As Int16
                           ) As Integer
    End Function

    ''' <summary>
    ''' Make a full parsing scan of each frame in the file. ID3 tags are found. An
    ''' accurate length value is stored. Seek index will be filled. A seek back to
    ''' current position is performed. At all, this function refuses work when
    ''' stream is not seekable.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <returns>MPG123_OK on success</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_scan(
                           ByVal mh As IntPtr) As Integer
    End Function

    ''' <summary>
    ''' Return, if possible, the full (expected) length of current track in PCM samples.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <returns>length >= 0 or MPG123_ERR if there is no length guess possible.</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_length(
                           ByVal mh As IntPtr) As Long
    End Function

    ''' <summary>
    ''' Returns the current position in samples.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <returns>sample offset or MPG123_ERR (null handle)</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_tell(
                            ByVal mh As IntPtr) As Long

    End Function

    ''' <summary>
    ''' Returns the current byte offset in the input stream.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <returns>byte offset or MPG123_ERR (null handle)</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_tell_stream(
                            ByVal mh As IntPtr) As Long

    End Function

    ''' <summary>
    ''' Seek to a desired sample offset.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <param name="sampleoff">offset in PCM samples</param>
    ''' <param name="whence">one of SEEK_SET, SEEK_CUR or SEEK_END</param>
    ''' <returns>The resulting offset >= 0 or error/message code</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_seek(
                            ByVal mh As IntPtr,
                            ByVal sampleoff As Long,
                            ByVal whence As SEEK_ORIGIN) As Long

    End Function

    ''' <summary>
    ''' Seek to a desired sample offset.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <param name="sampleoff">offset in PCM samples</param>
    ''' <param name="whence">one of SEEK_SET, SEEK_CUR or SEEK_END</param>
    ''' <returns>The resulting offset >= 0 or error/message code</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_seek_frame(
                            ByVal mh As IntPtr,
                            ByVal sampleoff As Long,
                            ByVal whence As SEEK_ORIGIN) As Integer

    End Function

    ''' <summary>
    ''' Read from stream and decode up to outmemsize bytes.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <param name="outmemory">outmemory address of output buffer to write to</param>
    ''' <param name="outmemsize">maximum number of bytes to write</param>
    ''' <param name="done">address to store the number of actually decoded bytes to</param>
    ''' <returns>MPG123_OK or error/message code</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_read(
                            ByVal mh As IntPtr,
                            ByVal outmemory As IntPtr,
                            ByVal outmemsize As UInt32,
                            ByRef done As UInt32) As Integer

    End Function

    ''' <summary>
    ''' Feed data for a stream that has been opened with mpg123_open_feed()
    ''' It's give and take: You provide the bytestream, mpg123 gives you the decoded samples.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <param name="inBuffer">pointer to input buffer</param>
    ''' <param name="size">number of input bytes</param>
    ''' <returns>MPG123_OK or error/message code</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_feed(ByVal mh As IntPtr,
                                        ByVal inBuffer As IntPtr,
                                        ByVal size As UInt32
                                        ) As Integer

    End Function

    ''' <summary>
    ''' Decode MPEG Audio from inmemory to outmemory.
    ''' This is very close to a drop-in replacement for old mpglib.
    ''' When you give zero-sized output buffer the input will be parsed until
    ''' decoded data is available. This enables you to get MPG123_NEW_FORMAT (and query it) 
    ''' without taking decoded data.
    ''' Think of this function being the union of mpg123_read() and mpg123_feed() (which it actually is, sort of;-).
    ''' You can actually always decide if you want those specialized functions in separate steps or one call this one here.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <param name="inmemory">pointer to input buffer</param>
    ''' <param name="inmemsize">number of input bytes</param>
    ''' <param name="outmemory">pointer to output buffer</param>
    ''' <param name="outmemsize">maximum number of output bytes</param>
    ''' <param name="done">address to store the number of actually decoded bytes to</param>
    ''' <returns>error/message code (watch out especially for MPG123_NEED_MORE)</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_decode(ByVal mh As IntPtr,
                                         ByVal inmemory As IntPtr,
                                         ByVal inmemsize As UInt32,
                                         ByVal outmemory As IntPtr,
                                         ByVal outmemsize As UInt32,
                                         ByRef done As UInt32) As Integer

    End Function

    ''' <summary>
    ''' Get frame information about the MPEG audio bitstream and store 
    ''' it in a mpg123_frameinfo structure
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <param name="mi">address of existing frameinfo structure to write to</param>
    ''' <returns>MPG123_OK on success</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_info(
    ByVal mh As IntPtr,
    <MarshalAs(UnmanagedType.Struct)> ByVal mi As mpg123_frameinfo) As Int32

    End Function

    ''' <summary>
    ''' Set a specific parameter, for a specific mpg123_handle, using a parameter 
    ''' type key chosen from the mpg123_parms enumeration, to the specified value.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <param name="type">parameter choice</param>
    ''' <param name="value">integer value</param>
    ''' <param name="fvalue">floating point value</param>
    ''' <returns>MPG123_OK on success</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_param(ByVal mh As IntPtr,
                                        ByVal type As mpg123_parms,
                                        ByVal value As Long,
                                        ByVal fvalue As Double) As Integer

    End Function


    ''' <summary>
    ''' Get a specific parameter, For a specific mpg123_handle. 
    ''' See the mpg123_parms enumeration for a list of available parameters.
    ''' </summary>
    ''' <param name="mh">mpg123 handle</param>
    ''' <param name="type">parameter choice</param>
    ''' <param name="value">integer value</param>
    ''' <param name="fvalue">floating point value</param>
    ''' <returns>MPG123_OK on success</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_getparam(ByVal mh As IntPtr,
                                        ByVal type As mpg123_parms,
                                        ByRef value As Long,
                                        ByRef fvalue As Double) As Integer

    End Function

    ''' <summary>
    ''' Query libmpg123 features
    ''' </summary>
    ''' <param name="key">feature selection</param>
    ''' <returns>1 for success, 0 for unimplemented functions</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_feature(ByVal key As mpg123_feature_set) As Integer
    End Function

    ''' <summary>
    ''' Look up error strings given integer code.
    ''' </summary>
    ''' <param name="errcode">integer error code</param>
    ''' <returns>string describing what that error error code means</returns>
    <DllImport(MPG123_DLL, CallingConvention:=CLLCNV)>
    Public Shared Function mpg123_plain_strerror(ByVal errcode As mpg123_errors) As String
    End Function


#End Region

    Private mpg123_handle As IntPtr = IntPtr.Zero

    Private bClassOpen As Boolean = False
    Private bIsEndOfStream As Boolean = False

    Private nPosition As Long = 0
    Private nDuration As Long = 0

    Private StreamInfo As StreamInformations

    Public Event DataReaded_Event(ByRef Buffer() As Byte) Implements ISoundDecoder.DataReaded_Event
    Public Event EndOfStream_Event() Implements ISoundDecoder.EndOfStream_Event

    ''' <summary>
    ''' Get the current duration in stream (in bytes)
    ''' </summary>
    ''' <returns>total bytes</returns>
    Public ReadOnly Property Duration() As Long Implements ISoundDecoder.Duration
        Get
            Return nDuration
        End Get
    End Property

    ''' <summary>
    ''' Get the current position in stream (in bytes)
    ''' </summary>
    ''' <returns>Bytes offset</returns>
    Public ReadOnly Property Position() As Long Implements ISoundDecoder.Position
        Get
            Return nPosition
        End Get
    End Property

    ''' <summary>
    ''' Check if the stream is at the end
    ''' </summary>
    ''' <returns>True if end of stream is reached, oterwise false</returns>
    Public ReadOnly Property EndOfStream() As Boolean Implements ISoundDecoder.EndOfStream
        Get
            Return bIsEndOfStream
        End Get
    End Property

    ''' <summary>
    ''' Get the List(of string) of supporte extensions
    ''' </summary>
    ''' <returns>Supported extensions</returns>
    Public ReadOnly Property Extensions() As System.Collections.Generic.List(Of String) Implements ISoundDecoder.Extensions
        Get
            Dim Supported As New List(Of String) From {
                ".mp1",
                ".mp2",
                ".mp3"
            }

            Return Supported
        End Get
    End Property

    ''' <summary>
    ''' Get the name of this decoder
    ''' </summary>
    ''' <returns>String contnent the name</returns>
    Public ReadOnly Property Name() As String Implements ISoundDecoder.Name
        Get
            Return "MPEG Audio Decoder 0.1b"
        End Get
    End Property

    ''' <summary>
    ''' Open new local file stream
    ''' </summary>
    ''' <param name="path">Location of file</param>
    ''' <returns>Return true if succeded</returns>
    Public Function Open(ByVal path As String) As Boolean Implements ISoundDecoder.Open
        Dim bResult As Boolean = False

        ' If there is an opened istance, close it
        If bClassOpen = True Then
            Me.Close()
        End If

        ' Check if file exist
        If My.Computer.FileSystem.FileExists(path) Then

            ' Init mpg123 dll
            If mpg123_init() = mpg123_errors.MPG123_OK Then

                ' Create new instance of mpg123 dll
                mpg123_handle = mpg123_new(Nothing, Nothing)

                ' If succeed the handle is not 0
                If mpg123_handle <> IntPtr.Zero Then

                    ' Try to open file, if succeded dll return MPG123_OK
                    If mpg123_open(mpg123_handle, path) = mpg123_errors.MPG123_OK Then

                        'Seek file to find ID3 and lenght (return OK only if stream is seekable)
                        If mpg123_scan(mpg123_handle) = mpg123_errors.MPG123_OK Then

                            ' Find stream info and ID3 tags
                            If FindStreamInformations(path) = True Then
                                bResult = True
                                bClassOpen = True
                            End If
                        End If
                    End If
                End If
            End If
        End If

        ' Return result
        Return bResult
    End Function

    ' Find wave format and ID3 tags
    Private Function FindStreamInformations(ByVal path As String) As Boolean
        ' Store the result
        Dim bResult As Boolean = False

        ' For stream info
        Dim samplerate As Int32
        Dim channels, encoding, BitsPerSample As Int16

        ' For ID3 tags
        Dim id3_v1 As New mpg123_id3v1
        Dim id3_v1_pointer As IntPtr
        Dim id3_v2 As New mpg123_id3v2
        Dim id3_v2_pointer As IntPtr

        ' Get WaveFormat for current stream
        If mpg123_getformat(mpg123_handle, samplerate, channels, encoding) = mpg123_errors.MPG123_OK Then

            ' Select BitsPerSample
            Select Case encoding
                Case mpg123_enc_enum.MPG123_ENC_UNSIGNED_16
                    BitsPerSample = 16
                Case mpg123_enc_enum.MPG123_ENC_16
                    BitsPerSample = 16
                Case mpg123_enc_enum.MPG123_ENC_SIGNED_16
                    BitsPerSample = 16
                Case mpg123_enc_enum.MPG123_ENC_8
                    BitsPerSample = 8
                Case mpg123_enc_enum.MPG123_ENC_ALAW_8
                    BitsPerSample = 8
                Case mpg123_enc_enum.MPG123_ENC_SIGNED_8
                    BitsPerSample = 8
                Case mpg123_enc_enum.MPG123_ENC_ULAW_8
                    BitsPerSample = 8
                Case mpg123_enc_enum.MPG123_ENC_UNSIGNED_8
                    BitsPerSample = 8
            End Select

            ' Create new instance of StreamInfo class
            StreamInfo = New StreamInformations

            ' Fill Stream info
            With StreamInfo
                .Samplerate = samplerate
                .Channels = channels
                .BitsPerSample = BitsPerSample
                .BlockAlign = CShort(channels * BitsPerSample / 8)
                .AvgBytesPerSec = .Samplerate * .BlockAlign
            End With

            ' Get lenght of stream and reset position
            nDuration = mpg123_length(mpg123_handle) * StreamInfo.BlockAlign
            nPosition = 0

            ' Fill the duration in milliseconds
            StreamInfo.DurationInMs = nDuration / StreamInfo.AvgBytesPerSec * 1000

            ' Use helper to fille basic file info
            StreamInfo.FillBasicFileInfo(path)

            'Set the result as true
            bResult = True

            ' Find ID3 tags
            If mpg123_id3(mpg123_handle, id3_v1_pointer, id3_v2_pointer) = mpg123_errors.MPG123_OK Then


                ' Convert pointer 2 to structure
                If id3_v2_pointer <> IntPtr.Zero Then
                    id3_v2 = CType(Marshal.PtrToStructure(id3_v2_pointer, GetType(mpg123_id3v2)), mpg123_id3v2)

                    StreamInfo.Artist = mpg123_string_to_vb_string(id3_v2.artist)
                    StreamInfo.Title = mpg123_string_to_vb_string(id3_v2.title)
                    StreamInfo.Album = mpg123_string_to_vb_string(id3_v2.album)
                    StreamInfo.Comment = mpg123_string_to_vb_string(id3_v2.comment)
                    StreamInfo.Year = mpg123_string_to_vb_string(id3_v2.year)
                    StreamInfo.Genre = mpg123_string_to_vb_string(id3_v2.genre)

                    ' Check if tags are empty and if true use filename
                    If (StreamInfo.Artist = "") And (StreamInfo.Title = "") Then
                        StreamInfo.Title = StreamInfo.FileName
                    End If
                Else
                    ' Convert pointer 1 to structure
                    If id3_v1_pointer <> IntPtr.Zero Then
                        ' Clip some values because add End of line char
                        'id3_v1 = CType(Marshal.PtrToStructure(id3_v1_pointer, GetType(mpg123_id3v1)), mpg123_id3v1)

                        ' So, Copy manually every value from memory (0 to 128)
                        id3_v1.tag = Marshal.PtrToStringAnsi(id3_v1_pointer + 0, 3)
                        id3_v1.title = Marshal.PtrToStringAnsi(id3_v1_pointer + 3, 30)
                        id3_v1.artist = Marshal.PtrToStringAnsi(id3_v1_pointer + 33, 30)
                        id3_v1.album = Marshal.PtrToStringAnsi(id3_v1_pointer + 63, 30)
                        id3_v1.year = Marshal.PtrToStringAnsi(id3_v1_pointer + 93, 4)
                        id3_v1.comment = Marshal.PtrToStringAnsi(id3_v1_pointer + 97, 30)
                        id3_v1.genre = Marshal.ReadByte(id3_v1_pointer, 127)

                        ' Copy to stream info
                        StreamInfo.Title = id3_v1.title
                        StreamInfo.Artist = id3_v1.artist
                        StreamInfo.Album = id3_v1.album
                        StreamInfo.Year = id3_v1.year
                        StreamInfo.Comment = id3_v1.comment
                        StreamInfo.Genre = StreamInfo.GenreIndexToString(id3_v1.genre)

                    Else
                        ' if no tag is present, use file name
                        StreamInfo.Title = StreamInfo.FileName
                    End If
                End If

            End If

        End If

        ' Return true if stream is open
        Return bResult
    End Function

    ' Convert from mpg123_string structure to String
    Private Function mpg123_string_to_vb_string(ByVal ptr As IntPtr) As String
        Dim mpg123_str As mpg123_string
        Dim result As String = ""

        ' Check if is a valid pointer
        If ptr <> IntPtr.Zero Then

            ' Convert pointer to structure
            mpg123_str = CType(Marshal.PtrToStructure(ptr, GetType(mpg123_string)), mpg123_string)

            ' If is not empty convert to string
            If mpg123_str.size <> 0 Then
                result = mpg123_str.p
            End If

        End If

        ' Converted string
        Return result
    End Function


    ''' <summary>
    ''' Close current stream if is open
    ''' </summary>
    Public Sub Close() Implements ISoundDecoder.Close
        ' Check if stream is open
        If bClassOpen = True Then

            ' Try to close mpg123 and reset vars
            If mpg123_close(mpg123_handle) = mpg123_errors.MPG123_OK Then
                mpg123_delete(mpg123_handle)
                mpg123_exit()

                mpg123_handle = IntPtr.Zero

                bClassOpen = False
                bIsEndOfStream = False

                StreamInfo.Dispose()
                StreamInfo = Nothing

                nPosition = 0
                nDuration = 0
            End If
        End If
    End Sub


    ''' <summary>
    ''' Read PCM data from opened stream
    ''' </summary>
    ''' <param name="Buffer">Byte array where store PCM data</param>
    ''' <param name="offset">Offset in byte from current position index</param>
    ''' <param name="Count">Number of bytes to read (the same size of buffer array)</param>
    ''' <returns>Number of bytes readed</returns>
    Public Function Read(ByRef Buffer() As Byte, ByVal offset As Integer, ByVal Count As Integer) As Integer Implements ISoundDecoder.Read
        Dim GCBuffer As GCHandle
        Dim DataReaded As UInt32 = 0

        ' Check if stream is open
        If bClassOpen = True Then
            ' Alloc an unmanaged buffer
            GCBuffer = GCHandle.Alloc(Buffer, GCHandleType.Pinned)

            ' If an offset is <> 0 seek to correct position
            If offset <> 0 Then
                Me.Seek(CLng(offset), SeekOrigin.Current)
            End If

            ' try to read PCM data from stream
            If mpg123_read(mpg123_handle,
               GCBuffer.AddrOfPinnedObject(),
               CUInt(Count),
               DataReaded) = mpg123_errors.MPG123_OK Then

                ' Tell new data has been readed
                RaiseEvent DataReaded_Event(Buffer)

                ' Calculate current position in the stream
                nPosition = mpg123_tell(mpg123_handle) * StreamInfo.BlockAlign

                ' Check if the stream is at the end
                If nDuration - nPosition = 0 Then
                    bIsEndOfStream = True

                    ' Notify end of stream
                    RaiseEvent EndOfStream_Event()
                End If

            End If

            ' Free resources
            GCBuffer.Free()
        End If

        ' Return number of bytes readed
        Return CInt(DataReaded)
    End Function

    ''' <summary>
    ''' Seek to new position
    ''' </summary>
    ''' <param name="position">New position in opened streamin bytes</param>
    ''' <param name="mode">Mode: BEGIN, CURRENT, END</param>
    ''' <returns>Current new position in bytes</returns>
    Public Function Seek(ByVal position As Long, ByVal mode As System.IO.SeekOrigin) As Long Implements ISoundDecoder.Seek
        Dim offset As Long = 0

        'Check if steam is open
        If bClassOpen = True Then

            ' Convert byte in PCM samples
            position = position \ StreamInfo.BlockAlign

            ' Seek the steam
            Select Case mode
                Case IO.SeekOrigin.Begin
                    offset = mpg123_seek(mpg123_handle, position, SEEK_ORIGIN.SEEK_SET)
                Case IO.SeekOrigin.Current
                    offset = mpg123_seek(mpg123_handle, position, SEEK_ORIGIN.SEEK_CUR)
                Case IO.SeekOrigin.End
                    offset = mpg123_seek(mpg123_handle, position, SEEK_ORIGIN.SEEK_END)
            End Select

            ' Convert offset from PCM to bytes
            offset = offset * StreamInfo.BlockAlign

            ' Check if seek to end of stream
            If offset < nDuration Then
                bIsEndOfStream = False
            Else
                bIsEndOfStream = True
                RaiseEvent EndOfStream_Event()
            End If
        End If

        ' Return current new position in bytes
        Return offset
    End Function

    ''' <summary>
    '''  Get in a fast way the stream information of a file
    ''' </summary>
    ''' <param name="Path">Location of the file</param>
    ''' <param name="info">Instance of stream info</param>
    ''' <returns>true if succeded otherwise false</returns>
    Public Function FastStreamInformations(Path As String, ByRef info As StreamInformations) As Boolean Implements ISoundDecoder.FastStreamInformations
        Dim result As Boolean = False

        ' Use this function only if current stream is close
        If bClassOpen = False Then

            'Try to open file
            If Me.Open(Path) Then

                ' Copy stream info
                info = OpenedStreamInformations()

                ' Close current stream
                Me.Close()

                ' Succeded
                result = True
            End If
        End If

        Return result
    End Function

    ''' <summary>
    ''' Get current stream information
    ''' - File informations
    ''' - Wave format informations
    ''' - ID3v2 or ID3v1 tags where found
    ''' </summary>
    ''' <returns>Valid stream info class handle</returns>
    Public Function OpenedStreamInformations() As StreamInformations Implements ISoundDecoder.OpenedStreamInformations
        'Return stream info only if current stream is open
        If bClassOpen = True Then
            Return StreamInfo
        Else
            Return Nothing
        End If
    End Function

End Class
