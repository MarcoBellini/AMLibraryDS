

Imports System.Runtime.InteropServices

Public Class SamplerateConverter
    Implements IDisposable

    ''' <summary>
    ''' Enum of supported samplerates
    ''' </summary>
    Public Enum Samplerate As Integer
        Samplerate_8000 = 8000
        Samplerate_11025 = 11025
        Samplerate_12000 = 12000
        Samplerate_16000 = 16000
        Samplerate_22050 = 22050
        Samplerate_32000 = 32000
        Samplerate_44100 = 44100
        Samplerate_48000 = 48000
    End Enum

    ''' <summary>
    ''' Enum of supported channels
    ''' </summary>
    Public Enum Channels
        Mono = 1
        Stereo = 2
    End Enum

    Private Handle As SRCState
    Private ConversionRatio As Double
    Private InChannels, OutChannels As Channels
    Private Data As New LibSamplerate.SRC_DATA

    Public Function NewConversion(ByVal Input As Samplerate,
                                  ByVal Output As Samplerate,
                                  ByVal Quality As LibSamplerate.ConverterType,
                                  ByVal InputChannels As Channels,
                                  ByVal OutputChannels As Channels) As Boolean

        Dim error_code As Integer

        ' New conversion ratio
        ConversionRatio = Output / Input

        ' Check if the conversion ratio is valid
        If LibSamplerate.src_is_valid_ratio(ConversionRatio) Then

            ' Try to open new instance of libSamplerate
            Handle = LibSamplerate.src_new(Quality, OutputChannels, error_code)

            ' Check if handle is valid and there aren't errors
            If (Handle.IsInvalid = False) And (error_code = 0) Then

                ' Setup conversion channels
                InChannels = InputChannels
                OutChannels = OutputChannels

                ' Success
                Return True
            End If
        End If

        ' Fail
        Return False
    End Function

    Public Function Convert_16_bits(ByRef InBuffer() As Byte,
                                    ByRef OutBuffer() As Byte,
                                    ByVal InBufferLen As Integer,
                                    ByRef OutBufferLen As Integer,
                                    ByVal EndOfStream As Boolean) As Boolean

        Dim InBufferFloat() As Single
        Dim OutBufferFloat() As Single
        Dim InBufferFloatSize, OutBufferFloatSize, ErrorCode As Integer

        Dim InBufferHandle, OutBufferHandle As GCHandle

        ' Check if Libsamplerate is open
        If (Handle.IsInvalid) Then
            Return False
        End If


        ' Calculate size of input buffer
        InBufferFloatSize = InBufferLen \ 2
        ReDim InBufferFloat(InBufferFloatSize - 1)

        ' Convert from byte to float
        PCMUtility.byte_to_float_array_16bits(InBuffer, InBufferFloat, InBufferLen)

        ' Convert channels
        If InChannels <> OutChannels Then
            If OutChannels = Channels.Mono Then
                StereoToMono(InBufferFloat, InBufferFloatSize)
            Else
                MonoToStereo(InBufferFloat, InBufferFloatSize)
            End If
        End If

        ' Calculate size of output buffer
        OutBufferFloatSize = Math.Floor(InBufferFloatSize * ConversionRatio) + Marshal.SizeOf(Of Short)
        ReDim OutBufferFloat(OutBufferFloatSize - 1)

        ' Alloc unmanaged memory
        InBufferHandle = GCHandle.Alloc(InBufferFloat, GCHandleType.Pinned)
        OutBufferHandle = GCHandle.Alloc(OutBufferFloat, GCHandleType.Pinned)

        ' Fill LibSamplerate Data structure
        With Data
            .data_in = InBufferHandle.AddrOfPinnedObject
            .data_out = OutBufferHandle.AddrOfPinnedObject
            .end_of_input = IIf(EndOfStream, 1, 0)
            .src_ratio = ConversionRatio
            .input_frames = InBufferFloatSize \ OutChannels
            .output_frames = OutBufferFloatSize \ OutChannels
        End With

        ' Process data
        ErrorCode = LibSamplerate.src_process(Handle, Data)

        ' Free resources
        InBufferHandle.Free()
        OutBufferHandle.Free()

        ' Show error on Debug Console
        If ErrorCode <> 0 Then
#If DEBUG Then
           DebugPrintLine("SamplerateConverter", LibSamplerate.src_pointer_to_string(
                          LibSamplerate.src_strerror(ErrorCode)))
#End If

            Return False
        End If


        ' Resize out buffer and OutBufferLen
        OutBufferFloatSize = Data.output_frames_gen * OutChannels
        OutBufferLen = OutBufferFloatSize * 2
        ReDim OutBuffer(OutBufferLen - 1)

        ' Fill out buffer with valid data
        PCMUtility.float_to_byte_array_16bits(OutBufferFloat, OutBuffer, OutBufferFloatSize)

        Return True
    End Function

    Private Sub StereoToMono(ByRef buffer() As Single, ByRef size As Integer)
        Dim Processed(size \ 2 - 1) As Single

        For i As Integer = 0 To size \ 2 - 1
            Processed(i) = (buffer(i * 2 + 0) + buffer(i * 2 + 1)) / 2.0F
        Next

        ' NB: Edit ByRef size variable
        size \= 2

        ReDim buffer(size - 1)

        Array.Copy(Processed, buffer, size)
    End Sub
    Private Sub MonoToStereo(ByRef buffer() As Single, ByRef size As Integer)
        Dim Processed(size * 2 - 1) As Single

        For i As Integer = 0 To size - 1
            Processed(i * 2 + 0) = buffer(i)
            Processed(i * 2 + 1) = buffer(i)
        Next

        ' NB: Edit ByRef size variable
        size *= 2

        ReDim buffer(size - 1)

        Array.Copy(Processed, buffer, size)
    End Sub

    Public Sub CloseConversion()
        ConversionRatio = 0

        ' Free resources
        If Handle IsNot Nothing Then
            Handle.Close()
            Handle = Nothing
        End If


    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            ConversionRatio = 0

            ' Free resources
            If Handle IsNot Nothing Then
                Handle.Close()
                Handle = Nothing
            End If

        End If
        disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
        MyBase.Finalize()
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
