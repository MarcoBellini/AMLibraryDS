Option Strict On

Imports System.IO
Imports System.Runtime.InteropServices

Public Class StreamMediaFondation
    Implements ISoundDecoder

#Region "Native Declarations"

    ' MediaFoundation is an audio subsystem introduced in Vista, and allowed in Win8 store apps.
    ' It is an evolution of the old DirectShow. In this sample we stream data from a url into
    ' raw PCM, and then write that PCM data into a WAV file.
    ' It is largely based on Microsoft's C++ MFSourceReader sample http://msdn.microsoft.com/en-us/library/windows/desktop/dd757929(v=vs.85).aspx
    ' and borrowed a few pinvoke prototypes from Tamir Khason http://www.codeproject.com/Articles/239378/Video-encoder-and-metadata-reading-by-using-Window


    ' Media Fondations const
    Public Const MF_SOURCE_READER_MEDIASOURCE As Integer = &HFFFFFFFF
    Public Const MF_SOURCE_READER_ALL_STREAMS As Integer = &HFFFFFFFE
    Public Const MF_SOURCE_READER_FIRST_AUDIO_STREAM As Integer = &HFFFFFFFD
    Public Const MF_SDK_VERSION As Integer = &H2
    Public Const MF_API_VERSION As Integer = &H70
    Public Const MF_VERSION As Integer = (MF_SDK_VERSION << 16) Or MF_API_VERSION
    Public Const S_OK As Integer = 0

    ' Media Fondations guids
    Public ReadOnly MF_MT_MAJOR_TYPE As New Guid("48eba18e-f8c9-4687-bf11-0a74c9f96a8f")
    Public ReadOnly MF_PD_DURATION As New Guid(&H6C990D33, &HBB8E, &H477A, &H85, &H98, &HD, &H5D, &H96, &HFC, &HD8, &H8A)
    Public ReadOnly MF_MT_SUBTYPE As New Guid("f7e34c9a-42e8-4714-b74b-cb29d72c35e5")
    Public ReadOnly MF_MT_AUDIO_BLOCK_ALIGNMENT As New Guid("322de230-9eeb-43bd-ab7a-ff412251541d")
    Public ReadOnly MF_MT_AUDIO_AVG_BYTES_PER_SECOND As New Guid("1aab75c8-cfef-451c-ab95-ac034b8e1731")
    Public ReadOnly MF_MT_AUDIO_NUM_CHANNELS As New Guid("37e48bf5-645e-4c5b-89de-ada9e29b696a")
    Public ReadOnly MF_MT_AUDIO_SAMPLES_PER_SECOND As New Guid("5faeeae7-0290-4c31-9e8a-c534f68d9dba")
    Public ReadOnly MF_MT_AUDIO_BITS_PER_SAMPLE As New Guid("f2deb57f-40fa-4764-aa33-ed4f2d1ff669")
    Public ReadOnly MFMediaType_Audio As New Guid("73647561-0000-0010-8000-00AA00389B71")
    Public ReadOnly MFAudioFormat_PCM As New Guid("00000001-0000-0010-8000-00AA00389B71")

    Public ReadOnly MF_PROPERTY_HANDLER_SERVICE As New Guid(&HA3FACE02, &H32B8, &H41DD, &H90, &HE7, &H5F, &HEF, &H7C, &H89, &H91, &HB5)
    Public ReadOnly MF_PROPERTY_STORE_GUID As New Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")
    ''' <summary>
    ''' Implemented by the Microsoft Media Foundation source reader object.
    ''' 
    ''' REMARKS:
    ''' To create the source reader, call one of the following functions:
    ''' - MFCreateSourceReaderFromByteStream
    ''' - MFCreateSourceReaderFromMediaSource
    ''' - MFCreateSourceReaderFromURL
    ''' 
    ''' Ref. https://docs.microsoft.com/en-us/windows/win32/api/mfreadwrite/nn-mfreadwrite-imfsourcereader
    ''' </summary>
    <ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("70ae66f2-c809-4e4f-8915-bdcb406b7993")>
    Public Interface IMFSourceReader

        ''' <summary>
        ''' Queries whether a stream is selected.
        ''' </summary>
        Sub GetStreamSelection(<[In]> ByVal dwStreamIndex As Integer,
                               <Out, MarshalAs(UnmanagedType.Bool)> ByRef pSelected As Boolean)


        ''' <summary>
        ''' Selects or deselects one or more streams.
        ''' </summary>
        Sub SetStreamSelection(<[In]> ByVal dwStreamIndex As Integer,
                               <[In], MarshalAs(UnmanagedType.Bool)> ByVal pSelected As Boolean)


        ''' <summary>
        ''' Gets a format that is supported natively by the media source.
        ''' </summary>
        Sub GetNativeMediaType(<[In]> ByVal dwStreamIndex As Integer,
                               <[In]> ByVal dwMediaTypeIndex As Integer,
                               <Out> ByRef ppMediaType As IntPtr)

        ''' <summary>
        ''' Gets the current media type for a stream.
        ''' </summary>
        Sub GetCurrentMediaType(<[In]> ByVal dwStreamIndex As Integer,
                                <Out> ByRef ppMediaType As IMFMediaType)

        ''' <summary>
        ''' Sets the media type for a stream.
        ''' </summary>
        Sub SetCurrentMediaType(<[In]> ByVal dwStreamIndex As Integer,
                                ByVal pdwReserved As IntPtr,
                                <[In]> ByVal pMediaType As IMFMediaType)

        ''' <summary>
        ''' Seeks to a new position in the media source
        ''' </summary>
        Sub SetCurrentPosition(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidTimeFormat As Guid,
                               <[In]> ByVal varPosition As IntPtr)

        ''' <summary>
        ''' Reads the next sample from the media source.
        ''' </summary>
        Sub ReadSample(<[In]> ByVal dwStreamIndex As Integer,
                       <[In]> ByVal dwControlFlags As Integer,
                       <Out> ByRef pdwActualStreamIndex As Integer,
                       <Out> ByRef pdwStreamFlags As Integer,
                       <Out> ByRef pllTimestamp As UInt64,
                       <Out> ByRef ppSample As IMFSample)

        ''' <summary>
        ''' Flushes one or more streams.
        ''' </summary>
        Sub Flush(<[In]> ByVal dwStreamIndex As Integer)

        ''' <summary>
        ''' Queries the underlying media source or decoder for an interface
        ''' </summary>
        Sub GetServiceForStream(<[In]> ByVal dwStreamIndex As Integer,
                                <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidService As Guid,
                                <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal riid As Guid,
                                <Out> ByRef ppvObject As IntPtr)

        ''' <summary>
        ''' Gets an attribute from the underlying media source.
        ''' </summary>
        Sub GetPresentationAttribute(<[In]> ByVal dwStreamIndex As Integer,
                                     <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidAttribute As Guid,
                                     <Out> ByVal pvarAttribute As IntPtr)
    End Interface

    ''' <summary>
    ''' Provides a generic way to store key/value pairs on an object.
    ''' The keys are GUIDs, and the values can be any of the following 
    ''' data types: UINT32, UINT64, double, GUID, wide-character string,
    ''' byte array, or IUnknown pointer. The standard implementation of 
    ''' this interface holds a thread lock while values are added, deleted, 
    ''' or retrieved.
    ''' 
    '''For a list of predefined attribute GUIDs, see Media Foundation Attributes.
    '''Each attribute GUID has an expected data type. The various "set" methods
    '''in IMFAttributes do Not validate the type against the attribute GUID. 
    '''It Is the application's responsibility to set the correct type for the
    '''attribute.
    '''
    ''' To create an empty attribute store, call MFCreateAttributes.
    ''' 
    ''' Ref. https://docs.microsoft.com/en-us/windows/win32/api/mfobjects/nn-mfobjects-imfattributes
    ''' </summary>
    <ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("2CD2D921-C447-44A7-A13C-4ADABFC247E3")>
    Public Interface IMFAttributes
        ''' <summary>
        ''' Retrieves the value associated with a key.
        ''' </summary>
        Sub GetItem(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                    ByVal pValue As IntPtr)

        ''' <summary>
        ''' 	Retrieves the data type of the value associated with a key.
        ''' </summary>
        Sub GetItemType(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                        ByRef pType As Integer)

        ''' <summary>
        ''' Queries whether a stored attribute value equals to a specified PROPVARIANT.
        ''' </summary>
        Sub CompareItem(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                        ByVal Value As IntPtr,
                        <MarshalAs(UnmanagedType.Bool)> ByRef pbResult As Boolean)

        ''' <summary>
        ''' Compares the attributes on this object with the attributes on another object.
        ''' </summary>
        Sub Compare(<MarshalAs(UnmanagedType.Interface)> ByVal pTheirs As IMFAttributes,
                    ByVal MatchType As Integer,
                    <MarshalAs(UnmanagedType.Bool)> ByRef pbResult As Boolean)

        ''' <summary>
        ''' Retrieves a UINT32 value associated with a key
        ''' </summary>
        Sub GetUINT32(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                      ByRef punValue As Integer)

        ''' <summary>
        ''' Retrieves a UINT64 value associated with a key
        ''' </summary>
        Sub GetUINT64(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                      ByRef punValue As Long)

        ''' <summary>
        ''' Retrieves a double value associated with a key
        ''' </summary>
        Sub GetDouble(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                      ByRef pfValue As Double)

        ''' <summary>
        ''' Retrieves a GUID value associated with a key.
        ''' </summary>
        Sub GetGUID(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                    ByRef pguidValue As Guid)

        ''' <summary>
        ''' Retrieves the length of a string value associated with a key.
        ''' </summary>
        Sub GetStringLength(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                            ByRef pcchLength As Integer)

        ''' <summary>
        ''' Retrieves a wide-character string associated with a key.
        ''' </summary>
        Sub GetString(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                      <Out, MarshalAs(UnmanagedType.LPWStr)> ByVal pwszValue As Text.StringBuilder,
                      ByVal cchBufSize As Integer,
                      ByRef pcchLength As Integer)

        ''' <summary>
        ''' Gets a wide-character string associated with a key. This method allocates the memory for the string
        ''' </summary>
        Sub GetAllocatedString(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                               <MarshalAs(UnmanagedType.LPWStr)> ByRef ppwszValue As String,
                               ByRef pcchLength As Integer)

        ''' <summary>
        ''' Retrieves the length of a byte array associated with a key.
        ''' </summary>
        Sub GetBlobSize(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                        ByRef pcbBlobSize As Integer)

        ''' <summary>
        ''' Retrieves a byte array associated with a key. This method copies the array into a caller-allocated buffer
        ''' </summary>
        Sub GetBlob(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                    <Out, MarshalAs(UnmanagedType.LPArray)> ByVal pBuf As Byte(),
                    ByVal cbBufSize As Integer,
                    ByRef pcbBlobSize As Integer)

        ''' <summary>
        ''' Retrieves a byte array associated with a key. This method allocates the memory for the array
        ''' </summary>
        Sub GetAllocatedBlob(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                             ByRef ip As IntPtr,
                             ByRef pcbSize As Integer)

        ''' <summary>
        ''' Retrieves an interface pointer associated with a key.
        ''' </summary>
        Sub GetUnknown(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                       <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal riid As Guid,
                       <MarshalAs(UnmanagedType.IUnknown)> ByRef ppv As Object)

        ''' <summary>
        ''' Adds an attribute value with a specified key.
        ''' </summary>
        Sub SetItem(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                    ByVal Value As IntPtr)

        ''' <summary>
        ''' Removes a key/value pair from the object's attribute list.
        ''' </summary>
        Sub DeleteItem(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid)

        ''' <summary>
        ''' Removes all key/value pairs from the object's attribute list.
        ''' </summary>
        Sub DeleteAllItems()

        ''' <summary>
        ''' Associates a UINT32 value with a key
        ''' </summary>
        Sub SetUINT32(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                      ByVal unValue As Integer)

        ''' <summary>
        ''' Associates a UINT64 value with a key
        ''' </summary>
        Sub SetUINT64(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                      ByVal unValue As Long)

        ''' <summary>
        ''' Associates a double value with a key.
        ''' </summary>
        Sub SetDouble(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                      ByVal fValue As Double)

        ''' <summary>
        ''' Associates a GUID value with a key
        ''' </summary>
        Sub SetGUID(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                    <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidValue As Guid)

        ''' <summary>
        ''' Associates a wide-character string with a key.
        ''' </summary>
        Sub SetString(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                      <[In], MarshalAs(UnmanagedType.LPWStr)> ByVal wszValue As String)

        ''' <summary>
        ''' Associates a byte array with a key.
        ''' </summary>
        Sub SetBlob(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                    <[In], MarshalAs(UnmanagedType.LPArray, SizeParamIndex:=2)> ByVal pBuf As Byte(),
                    ByVal cbBufSize As Integer)

        ''' <summary>
        ''' Associates an IUnknown pointer with a key.
        ''' </summary>
        Sub SetUnknown(<MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                       <[In], MarshalAs(UnmanagedType.IUnknown)> ByVal pUnknown As Object)

        ''' <summary>
        ''' Locks the attribute store so that no other thread can access it.
        ''' </summary>
        Sub LockStore()

        ''' <summary>
        ''' Unlocks the attribute store after a call to the IMFAttributes::LockStore method. 
        ''' While the object is unlocked, multiple threads can access the object's attributes.
        ''' </summary>
        Sub UnlockStore()

        ''' <summary>
        ''' Retrieves the number of attributes that are set on this objec
        ''' </summary>
        Sub GetCount(ByRef pcItems As Integer)

        ''' <summary>
        ''' Retrieves an attribute at the specified index
        ''' </summary>
        Sub GetItemByIndex(ByVal unIndex As Integer,
                           ByRef pguidKey As Guid,
                           ByVal pValue As IntPtr)

        ''' <summary>
        ''' Copies all of the attributes from this object into another attribute store.
        ''' </summary>
        Sub CopyAllItems(<[In], MarshalAs(UnmanagedType.Interface)> ByVal pDest As IMFAttributes)
    End Interface

    ''' <summary>
    ''' Represents a description of a media format
    ''' 
    ''' To create a new media type, call MFCreateMediaType.
    ''' 
    ''' All of the information in a media type Is stored as attributes. 
    ''' To clone a media type, call IMFAttributesCopyAllItems.
    ''' 
    ''' Ref. https://docs.microsoft.com/en-us/windows/win32/api/mfobjects/nn-mfobjects-imfmediatype
    ''' </summary>
    <ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("44AE0FA8-EA31-4109-8D2E-4CAE4997C555")>
    Public Interface IMFMediaType
        Inherits IMFAttributes

        Overloads Sub GetItem(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                              ByVal pValue As IntPtr)

        Overloads Sub GetItemType(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                  ByRef pType As Integer)

        Overloads Sub CompareItem(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                  ByVal Value As IntPtr,
                                  <MarshalAs(UnmanagedType.Bool)> ByRef pbResult As Boolean)

        Overloads Sub Compare(<MarshalAs(UnmanagedType.Interface)> ByVal pTheirs As IMFAttributes,
                              ByVal MatchType As Integer,
                              <MarshalAs(UnmanagedType.Bool)> ByRef pbResult As Boolean)

        Overloads Sub GetUINT32(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                ByRef punValue As Integer)

        Overloads Sub GetUINT64(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                ByRef punValue As Long)

        Overloads Sub GetDouble(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                ByRef pfValue As Double)

        Overloads Sub GetGUID(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                              ByRef pguidValue As Guid)

        Overloads Sub GetStringLength(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                      ByRef pcchLength As Integer)

        Overloads Sub GetString(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                <Out, MarshalAs(UnmanagedType.LPWStr)> ByVal pwszValue As Text.StringBuilder,
                                ByVal cchBufSize As Integer,
                                ByRef pcchLength As Integer)

        Overloads Sub GetAllocatedString(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                         <MarshalAs(UnmanagedType.LPWStr)> ByRef ppwszValue As String,
                                         ByRef pcchLength As Integer)

        Overloads Sub GetBlobSize(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                  ByRef pcbBlobSize As Integer)

        Overloads Sub GetBlob(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                              <Out, MarshalAs(UnmanagedType.LPArray)> ByVal pBuf As Byte(),
                              ByVal cbBufSize As Integer,
                              ByRef pcbBlobSize As Integer)

        Overloads Sub GetAllocatedBlob(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                       ByRef ip As IntPtr,
                                       ByRef pcbSize As Integer)

        Overloads Sub GetUnknown(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                 <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal riid As Guid,
                                 <MarshalAs(UnmanagedType.IUnknown)> ByRef ppv As Object)

        Overloads Sub SetItem(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                              ByVal Value As IntPtr)

        Overloads Sub DeleteItem(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid)

        Overloads Sub DeleteAllItems()

        Overloads Sub SetUINT32(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                ByVal unValue As Integer)

        Overloads Sub SetUINT64(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                ByVal unValue As Long)

        Overloads Sub SetDouble(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                ByVal fValue As Double)

        Overloads Sub SetGUID(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                              <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidValue As Guid)

        Overloads Sub SetString(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                <[In], MarshalAs(UnmanagedType.LPWStr)> ByVal wszValue As String)

        Overloads Sub SetBlob(<[In], MarshalAs(UnmanagedType.LPStruct)> guidKey As Guid, <[In], MarshalAs(UnmanagedType.LPArray, SizeParamIndex:=2)> pBuf As Byte(), cbBufSize As Integer)

        Overloads Sub SetUnknown(<MarshalAs(UnmanagedType.LPStruct)> guidKey As Guid, <[In], MarshalAs(UnmanagedType.IUnknown)> pUnknown As Object)

        Overloads Sub LockStore()

        Overloads Sub UnlockStore()

        Overloads Sub GetCount(ByRef pcItems As Integer)

        Overloads Sub GetItemByIndex(unIndex As Integer, ByRef pguidKey As Guid, pValue As IntPtr)

        Overloads Sub CopyAllItems(<[In], MarshalAs(UnmanagedType.Interface)> pDest As IMFAttributes)


        ''' <summary>
        ''' Gets the major type of the format.
        ''' </summary>
        Sub GetMajorType(ByRef pguidMajorType As Guid)

        ''' <summary>
        ''' Queries whether the media type is a temporally compressed format.
        ''' </summary>
        Sub IsCompressedFormat(<MarshalAs(UnmanagedType.Bool)> ByRef pfCompressed As Boolean)

        ''' <summary>
        ''' Compares two media types and determines whether they are identical.
        ''' If they are not identical, the method indicates how the two formats 
        ''' differ.
        ''' </summary>
        <PreserveSig>
        Function IsEqual(<[In], MarshalAs(UnmanagedType.Interface)> ByVal pIMediaType As IMFMediaType,
                         ByRef pdwFlags As Integer) As Integer

        ''' <summary>
        ''' Retrieves an alternative representation of the media type. Currently only the 
        ''' DirectShow AM_MEDIA_TYPE structure is supported
        ''' </summary>
        Sub GetRepresentation(<[In], MarshalAs(UnmanagedType.Struct)> ByVal guidRepresentation As Guid,
                              ByRef ppvRepresentation As IntPtr)

        ''' <summary>
        ''' Frees memory that was allocated by the IMFMediaType::GetRepresentation method.
        ''' </summary>
        Sub FreeRepresentation(<[In], MarshalAs(UnmanagedType.Struct)> ByVal guidRepresentation As Guid,
                               <[In]> ByVal pvRepresentation As IntPtr)
    End Interface

    ''' <summary>
    ''' Represents a media sample, which is a container object for media data. 
    ''' For video, a sample typically contains one video frame. 
    ''' For audio data, a sample typically contains multiple audio samples,
    ''' rather than a single sample of audio.
    ''' 
    ''' A media sample contains zero or more buffers. Each buffer manages a block
    ''' of memory, and is represented by the IMFMediaBuffer interface. 
    ''' A sample can have multiple buffers. The buffers are kept in an ordered
    ''' list and accessed by index value. It is also valid to have an empty s
    ''' ample with no buffers.
    ''' 
    ''' To create a new media sample, call MFCreateSample.
    ''' 
    ''' Ref. https://docs.microsoft.com/en-us/windows/win32/api/mfobjects/nn-mfobjects-imfsample
    ''' </summary>
    <ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("c40a00f2-b93a-4d80-ae8c-5a1c634f58e4")>
    Public Interface IMFSample
        Inherits IMFAttributes

        Overloads Sub GetItem(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                              ByVal pValue As IntPtr)

        Overloads Sub GetItemType(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                  ByRef pType As Integer)

        Overloads Sub CompareItem(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                  ByVal Value As IntPtr,
                                  <MarshalAs(UnmanagedType.Bool)> ByRef pbResult As Boolean)

        Overloads Sub Compare(<MarshalAs(UnmanagedType.Interface)> ByVal pTheirs As IMFAttributes,
                              ByVal MatchType As Integer,
                              <MarshalAs(UnmanagedType.Bool)> ByRef pbResult As Boolean)

        Overloads Sub GetUINT32(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                ByRef punValue As Integer)

        Overloads Sub GetUINT64(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                ByRef punValue As Long)

        Overloads Sub GetDouble(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                ByRef pfValue As Double)

        Overloads Sub GetGUID(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                              ByRef pguidValue As Guid)

        Overloads Sub GetStringLength(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                      ByRef pcchLength As Integer)

        Overloads Sub GetString(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                <Out, MarshalAs(UnmanagedType.LPWStr)> ByVal pwszValue As Text.StringBuilder,
                                ByVal cchBufSize As Integer,
                                ByRef pcchLength As Integer)

        Overloads Sub GetAllocatedString(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                         <MarshalAs(UnmanagedType.LPWStr)> ByRef ppwszValue As String,
                                         ByRef pcchLength As Integer)

        Overloads Sub GetBlobSize(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                  ByRef pcbBlobSize As Integer)

        Overloads Sub GetBlob(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                              <Out, MarshalAs(UnmanagedType.LPArray)> ByVal pBuf As Byte(),
                              ByVal cbBufSize As Integer, ByRef pcbBlobSize As Integer)

        Overloads Sub GetAllocatedBlob(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                       ByRef ip As IntPtr,
                                       ByRef pcbSize As Integer)

        Overloads Sub GetUnknown(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                 <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal riid As Guid,
                                 <MarshalAs(UnmanagedType.IUnknown)> ByRef ppv As Object)

        Overloads Sub SetItem(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                              ByVal Value As IntPtr)

        Overloads Sub DeleteItem(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid)

        Overloads Sub DeleteAllItems()

        Overloads Sub SetUINT32(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                ByVal unValue As Integer)

        Overloads Sub SetUINT64(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                ByVal unValue As Long)

        Overloads Sub SetDouble(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                ByVal fValue As Double)

        Overloads Sub SetGUID(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                              <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidValue As Guid)

        Overloads Sub SetString(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                <[In], MarshalAs(UnmanagedType.LPWStr)> ByVal wszValue As String)

        Overloads Sub SetBlob(<[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                              <[In], MarshalAs(UnmanagedType.LPArray, SizeParamIndex:=2)> ByVal pBuf As Byte(),
                              ByVal cbBufSize As Integer)

        Overloads Sub SetUnknown(<MarshalAs(UnmanagedType.LPStruct)> ByVal guidKey As Guid,
                                 <[In], MarshalAs(UnmanagedType.IUnknown)> ByVal pUnknown As Object)

        Overloads Sub LockStore()

        Overloads Sub UnlockStore()

        Overloads Sub GetCount(ByRef pcItems As Integer)

        Overloads Sub GetItemByIndex(ByVal unIndex As Integer,
                                     ByRef pguidKey As Guid,
                                     ByVal pValue As IntPtr)

        Overloads Sub CopyAllItems(<[In], MarshalAs(UnmanagedType.Interface)> ByVal pDest As IMFAttributes)


        ''' <summary>
        ''' Retrieves flags associated with the sample.Currently no flags are defined.
        ''' </summary>
        Sub GetSampleFlags(ByRef pdwSampleFlags As Integer)

        ''' <summary>
        ''' Sets flags associated with the sample.Currently no flags are defined.
        ''' </summary>
        Sub SetSampleFlags(ByVal dwSampleFlags As Integer)

        ''' <summary>
        ''' Retrieves the presentation time of the sample.
        ''' </summary>
        Sub GetSampleTime(ByRef phnsSampletime As Long)

        ''' <summary>
        ''' Sets the presentation time of the sample.
        ''' </summary>
        Sub SetSampleTime(ByVal hnsSampleTime As Long)

        ''' <summary>
        ''' Retrieves the duration of the sample.
        ''' </summary>
        Sub GetSampleDuration(ByRef phnsSampleDuration As Long)

        ''' <summary>
        ''' Sets the duration of the sample.
        ''' </summary>
        Sub SetSampleDuration(ByVal hnsSampleDuration As Long)

        ''' <summary>
        ''' Retrieves the number of buffers in the sample.
        ''' </summary>
        Sub GetBufferCount(ByRef pdwBufferCount As Integer)

        ''' <summary>
        ''' Gets a buffer from the sample, by index.
        ''' </summary>
        Sub GetBufferByIndex(ByVal dwIndex As Integer,
                             ByRef ppBuffer As IMFMediaBuffer)

        ''' <summary>
        ''' Converts a sample with multiple buffers into a sample with a single buffer.
        ''' </summary>
        Sub ConvertToContiguousBuffer(ByRef ppBuffer As IMFMediaBuffer)

        ''' <summary>
        ''' Adds a buffer to the end of the list of buffers in the sample
        ''' </summary>
        Sub AddBuffer(ByVal pBuffer As IMFMediaBuffer)

        ''' <summary>
        ''' 	Removes a buffer at a specified index from the sample.
        ''' </summary>
        Sub RemoveBuferByindex(ByVal dwIndex As Integer)

        ''' <summary>
        ''' Removes all of the buffers from the sample.
        ''' </summary>
        Sub RemoveAllBuffers()

        ''' <summary>
        ''' Retrieves the total length of the valid data in all of the buffers
        ''' in the sample. The length is calculated as the sum of the values 
        ''' retrieved by the IMFMediaBuffer::GetCurrentLength method.
        ''' </summary>
        Sub GetTotalLength(ByRef pcbTotalLength As Integer)

        ''' <summary>
        ''' Copies the sample data to a buffer. This method concatenates the
        ''' valid data from all of the buffers of the sample, in order
        ''' </summary>
        Sub CopyToByffer(ByVal pBuffer As IMFMediaBuffer)
    End Interface

    ''' <summary>
    ''' Represents a block of memory that contains media data. 
    ''' Use this interface to access the data in the buffer.
    ''' 
    ''' To get a buffer from a media sample, call one of the following
    ''' IMFSample methods:
    '''  - IMFSample::GetBufferByIndex
    '''  - IMFSample::ConvertToContiguousBuffer
    '''  
    ''' To create a new buffer object, use one of the following functions.
    ''' 
    ''' - MFCreateMemoryBuffer: Creates a buffer and allocates system memory.
    ''' - MFCreateMediaBufferWrapper: Creates a media buffer that wraps an existing media buffer.
    ''' - MFCreateDXSurfaceBuffer: Creates a buffer that manages a DirectX surface
    ''' - MFCreateAlignedMemoryBuffer: Creates a buffer and allocates system memory with a specified alignment.
    ''' 
    ''' Ref. https://docs.microsoft.com/en-us/windows/win32/api/mfobjects/nn-mfobjects-imfmediabuffer
    ''' </summary>
    <ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("045FA593-8799-42b8-BC8D-8968C6453507")>
    Public Interface IMFMediaBuffer

        ''' <summary>
        ''' 	Gives the caller access to the memory in the buffer, for reading or writing.
        ''' </summary>
        Sub Lock(ByRef ppbBuffer As IntPtr,
                 ByRef pcbMaxLength As Integer,
                 ByRef pcbCurrentLength As Integer)

        ''' <summary>
        ''' Unlocks a buffer that was previously locked. Call this method once 
        ''' for every call to IMFMediaBuffer::Lock.
        ''' </summary>
        Sub Unlock()

        ''' <summary>
        ''' Retrieves the length of the valid data in the buffer.
        ''' </summary>
        Sub GetCurrentLength(ByRef pcbCurrentLength As Integer)

        ''' <summary>
        ''' Sets the length of the valid data in the buffer.
        ''' </summary>
        Sub SetCurrentLength(ByVal cbCurrentLength As Integer)

        ''' <summary>
        ''' Retrieves the allocated size of the buffer.
        ''' </summary>
        Sub GetMaxLength(ByRef pcbMaxLength As Integer)
    End Interface

    ''' <summary>
    ''' Represents a byte stream from some data source, which might be a 
    ''' local file, a network file, or some other source. The IMFByteStream
    ''' interface supports the typical stream operations, such as reading, 
    ''' writing, and seeking.
    ''' 
    ''' The following functions return IMFByteStream pointers for local files:
    '''
    ''' - MFBeginCreateFile
    ''' - MFCreateFile
    ''' - MFCreateTempFile
    ''' 
    '''A byte stream for a media souce can be opened with read access. A byte stream for an archive media sink should be opened with both read And write access. (Read access may be required, because the archive sink might need to read portions of the file as it writes.)
    '''Some implementations Of this Interface also expose one Or more Of the following interfaces:
    '''
    ''' - IMFAttributes
    ''' - IMFByteStreamBuffering
    ''' - IMFByteStreamCacheControl
    ''' - IMFGetService
    ''' - IMFMediaEventGenerator
    ''' 
    ''' Ref. https://docs.microsoft.com/en-us/windows/win32/api/mfobjects/nn-mfobjects-imfbytestream
    ''' </summary>
    <ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("ad4c1b00-4bf7-422f-9175-756693d9130d")>
    Public Interface IMFByteStream

        ''' <summary>
        ''' Retrieves the characteristics of the byte stream.
        ''' </summary>
        Sub GetCapabilities(ByRef pdwCapabiities As Integer)

        ''' <summary>
        ''' Retrieves the length of the stream.
        ''' </summary>
        Sub GetLength(ByRef pqwLength As Long)

        ''' <summary>
        ''' Sets the length of the stream.
        ''' </summary>
        Sub SetLength(ByVal qwLength As Long)

        ''' <summary>
        ''' Retrieves the current read or write position in the stream.
        ''' </summary>
        Sub GetCurrentPosition(ByRef pqwPosition As Long)


        ''' <summary>
        ''' Sets the current read or write position.
        ''' </summary>
        Sub SetCurrentPosition(ByVal qwPosition As Long)

        ''' <summary>
        ''' Queries whether the current position has reached the end of the stream.
        ''' </summary>
        Sub IsEndOfStream(<MarshalAs(UnmanagedType.Bool)> ByRef pfEndOfStream As Boolean)

        ''' <summary>
        ''' Reads data from the stream.
        ''' </summary>
        Sub Read(ByVal pb As IntPtr,
                 ByVal cb As Integer,
                 ByRef pcbRead As Integer)

        ''' <summary>
        ''' Begins an asynchronous read operation from the stream.
        ''' </summary>
        Sub BeginRead(ByVal pb As IntPtr, cb As Integer,
                      ByVal pCallback As IntPtr,
                      ByVal punkState As IntPtr)

        ''' <summary>
        ''' Completes an asynchronous read operation.
        ''' </summary>
        Sub EndRead(ByVal pResult As IntPtr,
                    ByRef pcbRead As Integer)

        ''' <summary>
        ''' Writes data to the stream.
        ''' </summary>
        Sub Write(ByVal pb As IntPtr,
                  ByVal cb As Integer,
                  ByRef pcbWritten As Integer)


        ''' <summary>
        ''' 	Begins an asynchronous write operation to the stream.
        ''' </summary>
        Sub BeginWrite(ByVal pb As IntPtr,
                       ByVal cb As Integer,
                       ByVal pCallback As IntPtr,
                       ByVal punkState As IntPtr)

        ''' <summary>
        ''' Completes an asynchronous write operation.
        ''' </summary>
        Sub EndWrite(ByVal pResult As IntPtr,
                     ByRef pcbWritten As Integer)

        ''' <summary>
        ''' Moves the current position in the stream by a specified offset.
        ''' </summary>
        Sub Seek(ByVal SeekOrigin As Integer,
                 ByVal llSeekOffset As Long,
                 ByVal dwSeekFlags As Integer,
                 ByRef pqwCurrentPosition As Long)

        ''' <summary>
        ''' Clears any internal buffers used by the stream. 
        ''' If you are writing to the stream, the buffered data
        ''' is written to the underlying file or device
        ''' </summary>
        Sub Flush()

        ''' <summary>
        ''' Closes the stream and releases any resources associated with 
        ''' the stream, such as sockets or file handles. 
        ''' This method also cancels any pending asynchronous I/O requests.
        ''' </summary>
        Sub Close()
    End Interface

    ''' <summary>
    ''' Easy PROPVARIANT structure specialized for Long values only
    ''' NB. VT_I8 = 20 - Used to read duration of stream
    ''' </summary>
    <StructLayout(LayoutKind.Explicit)>
    Public Structure PROPVARIANT_LONG
        <FieldOffset(0)>
        Public vt As Short

        <FieldOffset(2)>
        Public wReserved1 As Short

        <FieldOffset(4)>
        Public wReserved2 As Short

        <FieldOffset(6)>
        Public wReserved3 As Short

        <FieldOffset(8)>
        Public cVal As Long
    End Structure


    ''' <summary>
    ''' Initializes Microsoft Media Foundation.
    ''' 
    ''' An application must call this function before using Media Foundation. 
    ''' Before your application quits, call MFShutdown once for every previous 
    ''' call to MFStartup.
    ''' 
    ''' Do not call this function from work queue threads.
    ''' </summary>
    ''' <param name="Version">Version number. Use the value MF_VERSION, defined in mfapi.h.</param>
    ''' <param name="dwFlags">use MFSTARTUP_FULL = 0</param>
    <DllImport("mfplat.dll", ExactSpelling:=True, PreserveSig:=False)>
    Shared Function MFStartup(ByVal Version As Integer,
                         Optional dwFlags As Integer = 0) As Integer
    End Function

    ''' <summary>
    ''' Shuts down the Microsoft Media Foundation platform.
    ''' Call this function once for every call to MFStartup.
    ''' Do not call this function from work queue threads.
    ''' </summary>
    <DllImport("mfplat.dll", ExactSpelling:=True, PreserveSig:=False)>
    Shared Function MFShutdown() As Integer
    End Function

    ''' <summary>
    ''' Creates an empty media type.
    ''' </summary>
    ''' <param name="ppMFType">
    ''' Receives a pointer to the IMFMediaType interface. 
    ''' The caller must release the interface.
    ''' </param>
    <DllImport("mfplat.dll", ExactSpelling:=True, PreserveSig:=False)>
    Shared Function MFCreateMediaType(ByRef ppMFType As IMFMediaType) As Integer
    End Function

    ''' <summary>
    ''' Converts a Media Foundation audio media type to a WAVEFORMATEX structure.
    ''' </summary>
    ''' <param name="pMFType">Pointer to the IMFMediaType interface of the media type.</param>
    ''' <param name="ppWF">Receives a pointer to the WAVEFORMATEX structure. The caller must release the memory allocated for the structure by calling CoTaskMemFree.</param>
    ''' <param name="pcbSize">Receives the size of the WAVEFORMATEX structure.</param>
    ''' <param name="Flags">Contains a flag from the MFWaveFormatExConvertFlags enumeration.</param>
    <DllImport("mfplat.dll", ExactSpelling:=True, PreserveSig:=False)>
    Shared Sub MFCreateWaveFormatExFromMFMediaType(ByVal pMFType As IMFMediaType,
                                                   ByRef ppWF As IntPtr,
                                                   ByRef pcbSize As Integer,
                                                   Optional Flags As Integer = 0)
    End Sub

    ''' <summary>
    ''' Creates the source reader from a URL.
    ''' </summary>
    ''' <param name="pwszURL">The URL of a media file to open.</param>
    ''' <param name="pAttributes">
    ''' Pointer to the IMFAttributes interface. 
    ''' You can use this parameter to configure the source reader.
    ''' For more information, see Source Reader Attributes. 
    ''' This parameter can be NULL.</param>
    ''' <param name="ppSourceReader">eceives a pointer to the IMFSourceReader interface. The caller must release the interface.</param>
    <DllImport("mfreadwrite.dll", ExactSpelling:=True, PreserveSig:=False)>
    Shared Function MFCreateSourceReaderFromURL(<MarshalAs(UnmanagedType.LPWStr)> ByVal pwszURL As String,
                                           ByVal pAttributes As IntPtr,
                                           ByRef ppSourceReader As IMFSourceReader) As Integer
    End Function

    ''' <summary>
    ''' Creates the source reader from a byte stream.
    ''' </summary>
    ''' <param name="pByteStream">A pointer to the IMFByteStream interface of a
    ''' byte stream. This byte stream will provide the source data for
    ''' the source reader.</param>
    ''' <param name="pAttributes">Pointer to the IMFAttributes interface.
    ''' You can use this parameter to configure the source reader. 
    ''' For more information, see Source Reader Attributes. T
    ''' his parameter can be NULL.</param>
    ''' <param name="ppSourceReader">Receives a pointer to the IMFSourceReader interface. The caller must release the interface.</param>
    <DllImport("mfreadwrite.dll", ExactSpelling:=True, PreserveSig:=False)>
    Shared Function MFCreateSourceReaderFromByteStream(ByVal pByteStream As IMFByteStream,
                                                  ByVal pAttributes As IntPtr,
                                                  ByRef ppSourceReader As IMFSourceReader) As Integer
    End Function

    ''' <summary>
    ''' Creates a Microsoft Media Foundation byte stream that wraps an IRandomAccessStream object
    ''' </summary>
    ''' <param name="punkStream">A pointer to the IRandomAccessStream interface.</param>
    ''' <param name="ppByteStream">Receives a pointer to the IMFByteStream interface. The caller must release the interface.</param>
    <DllImport("mfplat.dll", ExactSpelling:=True, PreserveSig:=False)>
    Shared Function MFCreateMFByteStreamOnStreamEx(<MarshalAs(UnmanagedType.IUnknown)> ByVal punkStream As Object,
                                              ByRef ppByteStream As IMFByteStream) As Integer
    End Function

    ''' <summary>
    ''' Queries an object for a specified service interface.
    ''' </summary>
    ''' <param name="punkObject">A pointer to the IUnknown interface of the object to query.</param>
    ''' <param name="guidService">The service identifier (SID) of the service. For a list of service identifiers, see Service Interfaces.</param>
    ''' <param name="riid">The interface identifier (IID) of the interface being requested.</param>
    ''' <param name="ppvObject">Receives the interface pointer. The caller must release the interface.</param>
    <DllImport("mf.dll", ExactSpelling:=True, PreserveSig:=False)>
    Shared Sub MFGetService(<[In], MarshalAs(UnmanagedType.Interface)> ByVal punkObject As Object,
                            <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guidService As Guid,
                            <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal riid As Guid,
                            <Out, MarshalAs(UnmanagedType.Interface)> ByRef ppvObject As Object)

    End Sub


#End Region

    Private BUFFER_MAX_SIZE As Integer = 65536

    Private nPosition As Long = 0
    Private nDuration As Long = 0

    Private bIsOpen As Boolean = False
    Private bIsEndOfStream As Boolean = False

    Private StreamInfo As StreamInformations
    Private CircBuffer As CircleBuffer

    Private pSourceReader As IMFSourceReader
    Private pMediaType As IMFMediaType

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
                ".aac",
                ".wma"}
            '".mp3"


            Return Supported
        End Get
    End Property

    Public ReadOnly Property Name As String Implements ISoundDecoder.Name
        Get
            Return ".NET Media Fondation Decoder 0.1a"
        End Get
    End Property

    Public ReadOnly Property EndOfStream As Boolean Implements ISoundDecoder.EndOfStream
        Get
            Return bIsEndOfStream
        End Get
    End Property

    Public Sub Close() Implements ISoundDecoder.Close
        ' Chek if stream is open,then close
        If bIsOpen = True Then

            ' Free resource
            If pSourceReader IsNot Nothing Then
                Marshal.ReleaseComObject(pSourceReader)
                pSourceReader = Nothing
            End If

            ' Close media fondation
            MFShutdown()

            ' Close Circle buffer
            CircBuffer.Dispose()
            CircBuffer = Nothing

            ' Reset other vars
            bIsEndOfStream = False
            bIsOpen = False
        End If
    End Sub

    Public Function Open(Path As String) As Boolean Implements ISoundDecoder.Open
        Dim bResult As Boolean = False
        Dim ptrPropVar As IntPtr = IntPtr.Zero
        Dim propVar As PROPVARIANT_LONG
        Dim nValue As Integer = 0

        ' Close opened stream
        If bIsOpen = True Then
            Me.Close()
        End If

        ' Check if file exist
        If My.Computer.FileSystem.FileExists(Path) = True Then

            ' Open new media fondation com object
            If MFStartup(MF_VERSION) = S_OK Then

                ' Try to open file
                If MFCreateSourceReaderFromURL(Path, Nothing, pSourceReader) = S_OK Then

                    ' Stream only first stream in the file
                    pSourceReader.SetStreamSelection(MF_SOURCE_READER_ALL_STREAMS, False)
                    pSourceReader.SetStreamSelection(MF_SOURCE_READER_FIRST_AUDIO_STREAM, True)

                    ' Try to Create MediaType interface
                    If MFCreateMediaType(pMediaType) = S_OK Then

                        ' We want only audio in PCM format
                        pMediaType.SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio)
                        pMediaType.SetGUID(MF_MT_SUBTYPE, MFAudioFormat_PCM)

                        ' Set media type
                        pSourceReader.SetCurrentMediaType(MF_SOURCE_READER_FIRST_AUDIO_STREAM,
                                                          Nothing,
                                                          pMediaType)

                        ' Relase resources
                        Marshal.ReleaseComObject(pMediaType)
                        pMediaType = Nothing

                        ' Read current stream informations
                        pSourceReader.GetCurrentMediaType(MF_SOURCE_READER_FIRST_AUDIO_STREAM, pMediaType)

                        ' Fill Stream Informations
                        StreamInfo = New StreamInformations()

                        ' Fill file basic informations
                        StreamInfo.FillBasicFileInfo(Path)

                        ' Samplerate
                        pMediaType.GetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, StreamInfo.Samplerate)

                        ' Channels
                        pMediaType.GetUINT32(MF_MT_AUDIO_NUM_CHANNELS, nValue)
                        StreamInfo.Channels = CShort(nValue)

                        ' Bits per sample
                        pMediaType.GetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, nValue)
                        StreamInfo.BitsPerSample = CShort(nValue)

                        ' Block Align
                        pMediaType.GetUINT32(MF_MT_AUDIO_BLOCK_ALIGNMENT, nValue)
                        StreamInfo.BlockAlign = CShort(nValue)

                        ' Averange bytes per second
                        pMediaType.GetUINT32(MF_MT_AUDIO_AVG_BYTES_PER_SECOND, StreamInfo.AvgBytesPerSec)

                        ' Alloc memory to contain the structure
                        ptrPropVar = Marshal.AllocHGlobal(Marshal.SizeOf(GetType(PROPVARIANT_LONG)))

                        ' Get the PROPVARIANT_LONG structure
                        ' NB. MF_PD_DURATION return Duration in nanoseconds
                        pSourceReader.GetPresentationAttribute(MF_SOURCE_READER_MEDIASOURCE, MF_PD_DURATION, ptrPropVar)

                        ' Convert pointer to structure
                        propVar = CType(Marshal.PtrToStructure(ptrPropVar, GetType(PROPVARIANT_LONG)), PROPVARIANT_LONG)

                        ' Set duration(convert to byte size) and reset position
                        nDuration = (propVar.cVal * StreamInfo.AvgBytesPerSec) \ 10000000L
                        nPosition = 0

                        ' Fill the duration in milliseconds
                        StreamInfo.DurationInMs = nDuration / StreamInfo.AvgBytesPerSec * 1000

                        ' Free resource
                        Marshal.FreeHGlobal(ptrPropVar)

                        ' No Metadata (Not yet implemented)
                        StreamInfo.Title = StreamInfo.FileName

                        ' Ensure to select first stream
                        pSourceReader.SetStreamSelection(MF_SOURCE_READER_FIRST_AUDIO_STREAM, True)

                        ' Release resources
                        Marshal.ReleaseComObject(pMediaType)
                        pMediaType = Nothing

                        ' Create new circle buffer
                        CircBuffer = New CircleBuffer(BUFFER_MAX_SIZE)

                        'Open successful
                        bIsOpen = True
                        bResult = True
                    End If
                End If
            End If
        End If
        Return bResult
    End Function


    Public Function Seek(Offset As Long, Mode As SeekOrigin) As Long Implements ISoundDecoder.Seek
        If bIsOpen = True Then
            Dim ptrPropVar As IntPtr = IntPtr.Zero
            Dim propVar As PROPVARIANT_LONG

            ' Align to current origin mode
            Select Case Mode
                Case SeekOrigin.Begin
                    Offset = 0 + Offset
                Case SeekOrigin.Current
                    Offset = nPosition + Offset
                Case SeekOrigin.End
                    Offset = nDuration - Offset
            End Select

            ' Update position
            nPosition = Offset

            ' Fill prop variant structure
            propVar.vt = 20 ' VT_I8 = Long
            propVar.cVal = (Offset \ StreamInfo.AvgBytesPerSec) * 10000000L

            ' Alloc memory to store Prop Variant structure
            ptrPropVar = Marshal.AllocHGlobal(Marshal.SizeOf(GetType(PROPVARIANT_LONG)))

            ' Copy structure to memory
            Marshal.StructureToPtr(propVar, ptrPropVar, False)

            ' Call new position function
            pSourceReader.SetCurrentPosition(Guid.Empty, ptrPropVar)

            ' Free resources
            Marshal.FreeHGlobal(ptrPropVar)

            ' Reset circle buffer
            CircBuffer.ClearMemory()
        End If
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
            ' NB: Media foundation don't give a costant buffer, so FillBuffer
            ' try to fill buffer for "count" bytes always
            nCurrentBytesReaded = FillBuffer(Buffer, Count)

            ' Update position
            nPosition = nPosition + nCurrentBytesReaded

            ' If there is data raise event
            If nCurrentBytesReaded > 0 Then
                RaiseEvent DataReaded_Event(Buffer)
            End If
        End If

        ' Return current bytes readed
        Return nCurrentBytesReaded
    End Function


    ' TODO: Test this function better, Try to fill buffer for "count" bytes
    Private Function FillBuffer(ByRef buffer() As Byte, ByVal count As Int32) As Int32
        Dim nWrittenData As Integer = 0

        ' Resize circle buffer based on output request
        If CircBuffer.BufferSize < (count * 2) Then
            CircBuffer.ResizeMemory(count * 2)
        End If

        ' Fill circle buffer except if end of file found
        While (CircBuffer.ValidReadData < count) And (bIsEndOfStream = False)
            MediaFoundationReadSample()
        End While

        ' Check if we reached endo of stream
        If bIsEndOfStream = False Then

            ' Copy requied count to buffer
            CircBuffer.ReadData(buffer, count)
            nWrittenData = count
        Else
            ' Check if buffer has valid data
            If CircBuffer.ValidReadData > 0 Then

                ' Check if data is more than requed or not
                If CircBuffer.ValidReadData >= count Then
                    'Copy requied count to buffer
                    CircBuffer.ReadData(buffer, count)
                    nWrittenData = count
                Else
                    ' Copy remaining count to buffer
                    Dim nValidData As Integer = CircBuffer.ValidReadData
                    CircBuffer.ReadData(buffer, nValidData)
                    nWrittenData = nValidData

                    ' Notify we reach the end of stream
                    RaiseEvent EndOfStream_Event()
                End If
            Else
                ' Notify we reach the end of stream
                RaiseEvent EndOfStream_Event()
            End If

        End If

        ' Return the number of bytes written
        Return nWrittenData
    End Function

    Private Sub MediaFoundationReadSample()
        Dim tempBuffer() As Byte
        Dim pSample As IMFSample = Nothing
        Dim pBuffer As IMFMediaBuffer = Nothing
        Dim pAudioData As IntPtr
        Dim cbBuffer As Integer = 0
        Dim dwFlags As Integer = 0

        ' Read next sample in streaming
        pSourceReader.ReadSample(MF_SOURCE_READER_FIRST_AUDIO_STREAM, 0, 0, dwFlags, 0, pSample)

        ' Check if end of stream o not
        If (dwFlags = S_OK) And (pSample IsNot Nothing) Then

            ' Convet to IMFMediaBuffer
            pSample.ConvertToContiguousBuffer(pBuffer)

            ' Convert to byte buffer
            pBuffer.Lock(pAudioData, Nothing, cbBuffer)

            ' Copy read PCM data to buffer
            ReDim tempBuffer(cbBuffer - 1)
            Marshal.Copy(pAudioData, tempBuffer, 0, cbBuffer)

            ' Check if Circle buffer is well sized, otherwise resize
            If CircBuffer.BufferSize < (cbBuffer * 2) Then
                CircBuffer.ResizeMemory(cbBuffer * 2)
            End If

            ' Write data in the circle buffer
            CircBuffer.WriteData(tempBuffer, cbBuffer)

            ' Unlock buffer
            pBuffer.Unlock()

            ' Clear resources
            Marshal.ReleaseComObject(pBuffer)
            Marshal.ReleaseComObject(pSample)

            pSample = Nothing
            pBuffer = Nothing
        Else
            ' Notify we reached end of stream
            bIsEndOfStream = True
        End If
    End Sub

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
