Imports System.Runtime.InteropServices

' LibSamplerate - Erik de Castro Lopo
' http://www.mega-nerd.com/SRC/index.html
' This class is a VB.NET port of samplerate.h 
Public Class LibSamplerate

#If PLATFORM = "x86" Then
    Private Const LibSamplerateDll As String = "x86\libsamplerate.dll"
#ElseIf PLATFORM = "x64" Then
    Private Const LibSamplerateDll As String = "x64\libsamplerate.dll"
#End If

    ''' <summary>
    ''' SRC_DATA is used to pass data to src_simple() and src_process(). 
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)>
    Public Structure SRC_DATA
        ''' <summary>
        ''' The data_in pointer is used to pass audio data into the converter 
        ''' while the data_out pointer supplies the converter with an array to 
        ''' hold the converter's output. For a converter which has been configured
        ''' for mulitchannel operation, these pointers need to point to a single array 
        ''' of interleaved data.
        ''' </summary>
        Public data_in As IntPtr

        Public data_out As IntPtr

        ''' <summary>
        ''' The input_frames and output_frames fields supply the converter with the lengths 
        ''' of the arrays (in frames) pointed to by the data_in and data_out pointers respectively.
        ''' For monophinc data, these values would indicate the length of the arrays while for 
        ''' multi channel data these values would be equal to the the length of the array divided
        ''' by the number of channels.
        ''' </summary>
        Public input_frames As Integer
        Public output_frames As Integer

        ''' <summary>
        ''' The input_frames_used and output_frames_gen fields are set by the converter to inform 
        ''' the caller of the number of frames consumed from the data_in array and the number of 
        ''' frames generated in the data_out array respectively. These values are for the current 
        ''' call to src_process only.
        ''' </summary>
        Public input_frames_used As Integer
        Public output_frames_gen As Integer

        ''' <summary>
        ''' The end_of_input field is only used when the sample rate converter is used by
        ''' calling the src_process function. In this case it should be set to zero if more 
        ''' buffers are to be passed to the converter and 1 if the current buffer is the last.
        ''' </summary>
        Public end_of_input As Short

        ''' <summary>
        ''' Finally, the src_ratio field specifies the conversion ratio defined as the 
        ''' input sample rate divided by the output sample rate. For a connected set of buffers,
        ''' this value can be varies on each call to src_process resulting in a time varying
        ''' sample rate conversion process. For time varying sample rate conversions, the ratio 
        ''' will be linearly interpolated between the src_ratio value of the previous call 
        ''' to src_process and the value for the current call.
        ''' </summary>
        Public src_ratio As Double
    End Structure

    ''' <summary>
    ''' The details of these converters are as follows:
    '''
    ''' SRC_SINC_BEST_QUALITY - This Is a bandlimited interpolator derived from the mathematical 
    ''' sinc function And this Is the highest quality sinc based converter, providing a worst case 
    ''' Signal-to-Noise Ratio (SNR) of 97 decibels (dB) at a bandwidth of 97%. All three SRC_SINC_* 
    ''' converters are based on the techniques of Julius O. Smith although this code was developed 
    ''' independantly.
    ''' 
    ''' SRC_SINC_MEDIUM_QUALITY - This Is another bandlimited interpolator much Like the previous one.
    ''' It has an SNR of 97dB And a bandwidth of 90%. The speed of the conversion Is much faster than 
    ''' the previous one.
    ''' 
    ''' SRC_SINC_FASTEST - This Is the fastest bandlimited interpolator And has an SNR of 97dB And a
    ''' bandwidth of 80%.
    ''' 
    ''' SRC_ZERO_ORDER_HOLD - A Zero Order Hold converter (interpolated value Is equal to the last value)
    ''' The quality Is poor but the conversion speed Is blindlingly fast.
    ''' 
    ''' SRC_LINEAR - A linear converter. Again the quality Is poor, but the conversion speed
    ''' Is blindingly fast.
    ''' </summary>
    Public Enum ConverterType
        SRC_SINC_BEST_QUALITY = 0
        SRC_SINC_MEDIUM_QUALITY = 1
        SRC_SINC_FASTEST = 2
        SRC_ZERO_ORDER_HOLD = 3
        SRC_LINEAR = 4
    End Enum

    ''' <summary>
    ''' User supplied callback function type for use with src_callback_new()
    ''' And src_callback_read(). First parameter Is the same pointer that was
    ''' passed into src_callback_new(). Second parameter Is pointer To a
    ''' pointer. The user supplied callback function must modify *data to
    ''' point to the start of the user supplied float array. The user supplied
    ''' function must Return the number Of frames that **data points To.
    ''' </summary>
    Public Delegate Function src_callback_t(ByVal cb_data As IntPtr, ByVal data As IntPtr) As Long


    ''' <summary>
    ''' Standard initialisation function:
    ''' Return an anonymous pointer To the
    ''' internal state of the converter. Choose a converter from the enums below.
    ''' Error returned in *error
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_new(ByVal converter_type As ConverterType,
                                   ByVal channels As Integer,
                                   ByRef error_value As Integer) As SRCState

    End Function

    ''' <summary>
    '''  Clone a handle : return an anonymous pointer to a new converter
    ''' containing the same internal state as orig. Error returned in *error.
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_clone(ByVal origin As SRCState,
                                     ByRef error_value As Integer) As SRCState

    End Function

    ''' <summary>
    ''' Initilisation for callback based API : return an anonymous pointer to the
    ''' internal state of the converter. Choose a converter from the enums below.
    ''' The cb_data pointer can point to any data or be set to NULL. Whatever the
    ''' value, when processing, user supplied function "func" gets called with
    ''' cb_data as first parameter.
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_callback_new(ByVal func As src_callback_t,
                                            ByVal converter_type As ConverterType,
                                            ByVal channels As Integer,
                                            ByRef error_value As Integer,
                                            ByVal cb_data As IntPtr) As SRCState

    End Function

    ''' <summary>
    ''' Cleanup all internal allocations.
    ''' Always returns NULL.
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Sub src_delete(ByVal state As IntPtr)
    End Sub

    ''' <summary>
    ''' Standard processing function.
    ''' Returns non zero on error.
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_process(ByVal state As SRCState,
                                       ByRef data As SRC_DATA) As Integer
    End Function


    ''' <summary>
    ''' Callback based processing function. Read up to frames worth of data from
    ''' the converter int *data and return frames read or -1 on error.
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_callback_read(ByVal state As SRCState,
                                             ByVal src_ratio As Double,
                                             ByVal frames As Long,
                                             ByVal data As IntPtr) As Integer
    End Function

    ''' <summary>
    ''' Simple interface for performing a single conversion from input buffer to
    ''' output buffer at a fixed conversion ratio.
    ''' Simple interface does not require initialisation as it can only operate on
    ''' a single buffer worth of audio.
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_simple(ByRef data As SRC_DATA,
                                      ByVal converter_type As ConverterType,
                                      ByVal channels As Integer) As Integer
    End Function

    ''' <summary>
    ''' This library contains a number of different sample rate converters,
    ''' numbered 0 through N.
    '''
    ''' Return a string giving either a name Or a more full description of each
    ''' sample rate converter Or NULL if no sample rate converter exists for
    ''' the given value. The converters are sequentially numbered from 0 to N.
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_get_name(ByVal converter_type As ConverterType) As IntPtr
    End Function

    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_get_description(ByVal converter_type As ConverterType) As IntPtr
    End Function

    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_get_version() As IntPtr
    End Function

    ''' <summary>
    ''' Set a new SRC ratio. This allows step responses
    ''' in the conversion ratio.
    ''' Returns non zero on error.
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_set_ratio(ByVal state As SRCState,
                                         ByVal new_ratio As Double) As Integer

    End Function

    ''' <summary>
    ''' Get the current channel count.
    ''' Returns negative on error, positive channel count otherwise
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_get_channels(ByVal state As SRCState) As Integer

    End Function

    ''' <summary>
    ''' Reset the internal SRC state.
    ''' Does not modify the quality settings.
    ''' Does not free any memory allocations.
    ''' Returns non zero on error.
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_reset(ByVal state As SRCState) As Integer

    End Function

    ''' <summary>
    ''' Return TRUE if ratio is a valid conversion ratio, FALSE otherwise.
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_is_valid_ratio(ByVal ratio As Double) As <MarshalAs(UnmanagedType.Bool)> Boolean

    End Function

    ''' <summary>
    ''' Return an error number.
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_error(ByVal state As SRCState) As Integer
    End Function

    ''' <summary>
    ''' Convert the error number into a string.
    ''' </summary>
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function src_strerror(ByVal error_value As Integer) As IntPtr
    End Function



    'Extra helper functions for converting from short to float And
    'back again.
    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Sub src_short_to_float_array(<MarshalAs(UnmanagedType.LPArray)> ByVal inBuffer() As Short,
                                               <MarshalAs(UnmanagedType.LPArray)> ByVal outBuffer() As Single,
                                               ByVal len As Integer)
    End Sub

    <DllImport(LibSamplerateDll, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Sub src_float_to_short_array(<MarshalAs(UnmanagedType.LPArray)> ByVal inBuffer() As Single,
                                               <MarshalAs(UnmanagedType.LPArray)> ByVal outBuffer() As Short,
                                               ByVal len As Integer)
    End Sub

    ''' <summary>
    ''' Convert from LibSamplerate pointer to a VB.NET string
    ''' </summary>
    ''' <param name="pointer">Pointer from Libsamplerate function</param>
    ''' <returns>String with data</returns>
    Public Shared Function src_pointer_to_string(ByVal pointer As IntPtr) As String
        Return Marshal.PtrToStringAnsi(pointer)
    End Function

End Class
