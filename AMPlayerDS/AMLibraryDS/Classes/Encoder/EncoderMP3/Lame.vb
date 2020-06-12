Imports System.Runtime.InteropServices

Public Class Lame
    ' See LameAPI.txt for documentation
    ' Ported lame.h - https://sourceforge.net/p/lame/svn/6430/tree/trunk/lame/include/lame.h

    ' Select Dll based on compile settings
#If PLATFORM = "x86" Then
    ' Dll name in app folder
    Private Const LAME_DLL As String = "x86\libmp3lame.dll"
#ElseIf PLATFORM = "x64" Then
    ' Dll name in app folder
    Private Const LAME_DLL As String = "x64\libmp3lame.dll"
#End If

    'Calling convention of LAME
    Private Const LAME_CNV As CallingConvention = CallingConvention.Cdecl

    Public Const LAME_MAX_BITRATE As Integer = 320
    Public Const LAME_MIN_BITRATE As Integer = 80
    Public Const LAME_MEAN_BITRATE As Integer = 192

    Public Enum vbr_mode
        vbr_off = 0
        vbr_mt                   ' /* obsolete, same As vbr_mtrh */
        vbr_rh
        vbr_abr
        vbr_mtrh
        vbr_max_indicator        '/* Don't use this! It's used for sanity checks.       */
        vbr_default = vbr_mtrh   '/* change this To change the Default VBR mode Of LAME */
    End Enum

    Public Enum MPEG_mode
        STEREO = 0
        JOINT_STEREO
        DUAL_CHANNEL             '/* LAME doesn't supports this! */
        MONO
        NOT_SET
        MAX_INDICATOR            '/* Don't use this! It's used for sanity checks. */
    End Enum

    Public Enum Padding_type
        PAD_NO = 0
        PAD_ALL
        PAD_ADJUST
        PAD_MAX_INDICATOR       ' /* Don't use this! It's used for sanity checks. */
    End Enum

    Public Enum preset_mode
        ' /*values from 8 to 320 should be reserved for abr bitrates*/
        ' /*for abr I'd suggest to directly use the targeted bitrate as a value*/
        ABR_8 = 8
        ABR_320 = 320

        V9 = 410 '/*Vx to match Lame And VBR_xx to match FhG*/
        VBR_10 = 410
        V8 = 420
        VBR_20 = 420
        V7 = 430
        VBR_30 = 430
        V6 = 440
        VBR_40 = 440
        V5 = 450
        VBR_50 = 450
        V4 = 460
        VBR_60 = 460
        V3 = 470
        VBR_70 = 470
        V2 = 480
        VBR_80 = 480
        V1 = 490
        VBR_90 = 490
        V0 = 500
        VBR_100 = 500


        '/*still there for compatibility*/
        R3MIX = 1000
        STANDARD = 1001
        EXTREME = 1002
        INSANE = 1003
        STANDARD_FAST = 1004
        EXTREME_FAST = 1005
        MEDIUM = 1006
        MEDIUM_FAST = 1007
    End Enum

    Public Enum asm_optimizations
        MMX = 1
        AMD_3DNOW = 2
        SSE = 3
    End Enum

    Public Enum Psy_model
        PSY_GPSYCHO = 1
        PSY_NSPSYTUNE = 2
    End Enum

    Public Enum buffer_constraint
        MDB_DEFAULT = 0
        MDB_STRICT_ISO = 1
        MDB_MAXIMUM = 2
    End Enum

    Public Enum Lame_Quality
        BEST = 0
        NEARBEST = 2
        MEDIUM = 5
        LOW = 7
        WORST = 9
    End Enum

    Public Enum lame_errorcodes
        LAME_OKAY = 0
        LAME_NOERROR = 0
        LAME_GENERICERROR = -1
        LAME_NOMEM = -10
        LAME_BADBITRATE = -11
        LAME_BADSAMPFREQ = -12
        LAME_INTERNALERROR = -13
        FRONTEND_READERROR = -80
        FRONTEND_WRITEERROR = -81
        FRONTEND_FILETOOLARGE = -82
    End Enum

    ''' <summary>
    ''' 
    '''  REQUIRED
    '''  initialize the encoder.  sets default for all encoder parameters,
    '''  returns NULL if some malloc()'s failed
    '''  otherwise returns pointer to structure needed for all future
    '''  API calls.
    '''
    ''' </summary>
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_init() As LameHandle
    End Function

    ''' <summary>     
    ''' 
    ''' REQUIRED
    ''' final call to free all remaining buffers
    ''' 
    ''' </summary>
    ''' <param name="handle">Pointer to lame_global_flags structure</param>
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_close(ByVal handle As IntPtr) As Integer
    End Function



    '/* input sample rate in Hz.  default = 44100hz */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_in_samplerate(ByVal handle As LameHandle,
                                                  ByVal samplerate As Integer) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_in_samplerate(ByVal handle As LameHandle) As Integer
    End Function



    '/* number of channels in input stream. default=2  */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_num_channels(ByVal handle As LameHandle,
                                                  ByVal channels As Integer) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_num_channels(ByVal handle As LameHandle) As Integer
    End Function


    '1 = write a Xing VBR header frame.
    '    Default = 1
    ' this variable must have been added by a Hungarian notation Windows programmer :-)
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_bWriteVbrTag(ByVal handle As LameHandle,
                                                 ByVal value As Integer) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_bWriteVbrTag(ByVal handle As LameHandle) As Integer
    End Function



    '  /*
    '  output sample rate In Hz.  Default = 0, which means LAME picks best value
    '  based on the amount of compression.  MPEG only allows:
    '  MPEG1    32, 44.1,   48khz
    '  MPEG2    16, 22.05,  24
    '  MPEG2.5   8, 11.025, 12
    '  (Not used by decoding routines)
    '  */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_out_samplerate(ByVal handle As LameHandle,
                                                  ByVal samplerate As Integer) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_out_samplerate(ByVal handle As LameHandle) As Integer
    End Function

    '  /*
    '  internal algorithm selection.  True quality Is determined by the bitrate
    '  but this variable will effect quality by selecting expensive Or cheap algorithms.
    '  quality=0..9.  0=best (very slow).  9=worst.
    '  recommended:  2     near-best quality, Not too slow
    '                5     good quality, fast
    '                7     ok quality, really fast
    '   */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_quality(ByVal handle As LameHandle,
                                            ByVal samplerate As Lame_Quality) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_quality(ByVal handle As LameHandle) As Lame_Quality
    End Function


    ' set one of brate compression ratio.
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_brate(ByVal handle As LameHandle,
                                            ByVal brate As Integer) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_brate(ByVal handle As LameHandle) As Integer
    End Function



    '/* mark as copyright.  default=0 */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_copyright(ByVal handle As LameHandle,
                                              ByVal value As Integer) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_copyright(ByVal handle As LameHandle) As Integer
    End Function

    '/* mark as original.  default=1 */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_original(ByVal handle As LameHandle,
                                              ByVal value As Integer) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_original(ByVal handle As LameHandle) As Integer
    End Function


    '/* error_protection.  Use 2 bytes from each frame for CRC checksum. default=0 */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_error_protection(ByVal handle As LameHandle,
                                                     ByVal value As Integer) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_error_protection(ByVal handle As LameHandle) As Integer
    End Function



    '/********************************************************************
    '* VBR control
    '**********************************************************************/

    ' /* Types of VBR.  default = vbr_off = CBR */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_VBR(ByVal handle As LameHandle,
                                        ByVal value As vbr_mode) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_VBR(ByVal handle As LameHandle) As vbr_mode
    End Function


    '/* VBR quality level.  0=highest  9=lowest  */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_VBR_q(ByVal handle As LameHandle,
                                        ByVal value As Integer) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_VBR_q(ByVal handle As LameHandle) As Integer
    End Function

    '/* Ignored except for VBR=vbr_abr (ABR mode) */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_VBR_mean_bitrate_kbps(ByVal handle As LameHandle,
                                                          ByVal value As Integer) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_VBR_mean_bitrate_kbps(ByVal handle As LameHandle) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_VBR_min_bitrate_kbps(ByVal handle As LameHandle,
                                                         ByVal value As Integer) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_VBR_min_bitrate_kbps(ByVal handle As LameHandle) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_set_VBR_max_bitrate_kbps(ByVal handle As LameHandle,
                                                         ByVal value As Integer) As Integer
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_get_VBR_max_bitrate_kbps(ByVal handle As LameHandle) As Integer
    End Function





    ''' <summary>
    ''' 
    ''' REQUIRED:
    ''' sets more internal configuration based on data provided above.
    ''' returns -1 if something failed.
    ''' 
    ''' </summary>
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_init_params(ByVal handle As LameHandle) As Integer
    End Function


    '/*
    '* OPTIONAL:
    '* get the version number, in a string. of the form
    '* "3.63 (beta)" Or just "3.63".
    '*/
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function get_lame_version() As IntPtr
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function get_lame_short_version() As IntPtr
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function get_lame_very_short_version() As IntPtr
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function get_psy_version() As IntPtr
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function get_lame_url() As IntPtr
    End Function

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function get_lame_os_bitness() As IntPtr
    End Function


    '/*
    ' * input pcm data, output (maybe) mp3 frames.
    ' * This routine handles all buffering, resampling And filtering for you.
    ' *
    ' * return code     number of bytes output in mp3buf. Can be 0
    ' *                 -1:  mp3buf was too small
    ' *                 -2:  malloc() problem
    ' *                 -3:  lame_init_params() Not called
    ' *                 -4:  psycho acoustic problems
    ' *
    ' * The required mp3buf_size can be computed from num_samples,
    ' * samplerate And encoding rate, but here Is a worst case estimate:
    ' *
    ' * mp3buf_size in bytes = 1.25*num_samples + 7200
    ' *
    ' * I think a tighter bound could be:  (mt, March 2000)
    ' * MPEG1:
    ' *    num_samples*(bitrate/8)/samplerate + 4*1152*(bitrate/8)/samplerate + 512
    ' * MPEG2:
    ' *    num_samples*(bitrate/8)/samplerate + 4*576*(bitrate/8)/samplerate + 256
    ' *
    ' * but test first if you use that!
    ' *
    ' * set mp3buf_size = 0 And LAME will Not check if mp3buf_size Is
    ' * large enough.
    ' *
    ' * NOTE:
    ' * if gfp->num_channels=2, but gfp->mode = 3 (mono), the L & R channels
    ' * will be averaged into the L channel before encoding only the L channel
    ' * This will overwrite the data in buffer_l[] And buffer_r[].
    ' *
    ' */

    ''' <summary>
    ''' Encode PCM Stream
    ''' </summary>
    ''' <param name="handle">global context handle </param>
    ''' <param name="LeftShortBuffer">PCM data For left channel </param>
    ''' <param name="RightShortBuffer">PCM data For right channel</param>
    ''' <param name="nsamples">number Of samples per channel</param>
    ''' <param name="mp3buf">pointer to encoded MP3 stream</param>
    ''' <param name="mp3buf_size">number Of valid octets In this stream</param>
    ''' <returns></returns>
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_encode_buffer(ByVal handle As LameHandle,
                                              ByVal LeftShortBuffer As IntPtr,
                                              ByVal RightShortBuffer As IntPtr,
                                              ByVal nsamples As Integer,
                                              ByVal mp3buf As IntPtr,
                                              ByVal mp3buf_size As Integer) As Integer

    End Function

    ''' <summary>
    ''' <see cref="lame_encode_buffer"/>
    ''' </summary>
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_encode_buffer_interleaved(
                                            ByVal handle As LameHandle,
                                            ByVal LeftRightShortBuffer As IntPtr,
                                            ByVal nsamples As Integer,
                                            ByVal mp3buf As IntPtr,
                                            ByVal mp3buf_size As Integer) As Integer

    End Function

    '/*
    '* REQUIRED:
    '* lame_encode_flush will flush the intenal PCM buffers, padding with
    '* 0's to make sure the final frame is complete, and then flush
    '* the internal MP3 buffers, And thus may return a
    '* final few mp3 frames.  'mp3buf' should be at least 7200 bytes long
    '* to hold all possible emitted data.
    '*
    '* will also write id3v1 tags (if any) into the bitstream
    '*
    '* return code = number of bytes output to mp3buf. Can be 0
    '*/

    ''' <summary>
    ''' Close Intenal Stream
    ''' </summary>
    ''' <param name="handle">global context handle</param>
    ''' <param name="mp3buf">pointer to encoded MP3 stream</param>
    ''' <param name="size">number Of valid octets In this stream</param>
    ''' <returns></returns>
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Function lame_encode_flush(ByVal handle As LameHandle,
                                             ByVal mp3buf As IntPtr,
                                             ByVal size As Integer) As Integer

    End Function

    ' /*********************************************************************
    ' *
    ' * id3tag stuff (TODO: check if pass handle ByRef)
    ' *
    ' *********************************************************************/
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Sub id3tag_init(ByVal handle As LameHandle)
    End Sub

    ' /* force addition of version 2 tag */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Sub id3tag_add_v2(ByVal handle As LameHandle)
    End Sub


    ' /* add only a version 1 tag */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Sub id3tag_v1_only(ByVal handle As LameHandle)
    End Sub

    ' /* add only a version 2 tag */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Sub id3tag_v2_only(ByVal handle As LameHandle)
    End Sub

    ' /* pad version 1 tag with spaces instead of nulls */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Sub id3tag_space_v1(ByVal handle As LameHandle)
    End Sub


    ' /* pad version 2 tag with extra 128 bytes */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Sub id3tag_pad_v2(ByVal handle As LameHandle)
    End Sub

    ' /* pad version 2 tag with extra n bytes */
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Sub id3tag_set_pad(ByVal handle As LameHandle,
                                     ByVal size As Integer)
    End Sub

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Sub id3tag_set_title(ByVal handle As LameHandle,
                                       ByVal title As String)
    End Sub

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Sub id3tag_set_artist(ByVal handle As LameHandle,
                                        ByVal artist As String)
    End Sub

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Sub id3tag_set_album(ByVal handle As LameHandle,
                                       ByVal album As String)
    End Sub

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Sub id3tag_set_year(ByVal handle As LameHandle,
                                      ByVal year As String)
    End Sub

    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Sub id3tag_set_comment(ByVal handle As LameHandle,
                                         ByVal comment As String)
    End Sub

    ' return non-zero result if genre name or number is invalid
    ' result 0: OK
    ' result -1: genre number out of range
    ' result -2: no valid ID3v1 genre name, mapped to ID3v1 'Other'
    '            but taken as-is for ID3v2 genre tag
    <DllImport(LAME_DLL, CallingConvention:=LAME_CNV)>
    Public Shared Sub id3tag_set_genre(ByVal handle As LameHandle,
                                       ByVal genre As String)
    End Sub



End Class
