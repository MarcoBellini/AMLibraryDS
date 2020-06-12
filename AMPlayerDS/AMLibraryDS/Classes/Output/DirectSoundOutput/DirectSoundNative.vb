' Thanks to Naudio to a lot of code here
'https://github.com/naudio/NAudio/blob/master/NAudio/Wave/WaveOutputs/DirectSoundOut.cs

Imports System.Runtime.InteropServices


Public Class DirectSoundNative
    Implements System.IDisposable

#Region "DirectSound Declaration"
    <StructLayout(LayoutKind.Sequential, Pack:=2)>
    Friend Class BufferDescription
        Public dwSize As Integer
        <MarshalAs(UnmanagedType.U4)> Public dwFlags As DirectSoundBufferCaps
        Public dwBufferBytes As UInteger
        Public dwReserved As Integer
        Public lpwfxFormat As IntPtr
        Public guidAlgo As Guid
    End Class

    <StructLayout(LayoutKind.Sequential, Pack:=2)>
    Friend Class BufferCaps
        Public dwSize As Integer
        Public dwFlags As Integer
        Public dwBufferBytes As Integer
        Public dwUnlockTransferRate As Integer
        Public dwPlayCpuOverhead As Integer
    End Class

    <StructLayoutAttribute(LayoutKind.Sequential, Pack:=2)>
    Friend Structure WaveFormat
        Public wFormatTag As Int16
        Public nChannels As Int16
        Public nSamplesPerSec As Int32
        Public nAvgBytesPerSec As Int32
        Public nBlockAlign As Int16
        Public wBitsPerSample As Int16
        Public cbSize As Int16
    End Structure

    Friend Enum DirectSoundCooperativeLevel As UInt32
        DSSCL_NORMAL = &H1
        DSSCL_PRIORITY = &H2
        DSSCL_EXCLUSIVE = &H3
        DSSCL_WRITEPRIMARY = &H4
    End Enum

    <FlagsAttribute>
    Friend Enum DirectSoundPlayFlags As UInt32
        DSBPLAY_LOOPING = &H1
        DSBPLAY_LOCHARDWARE = &H2
        DSBPLAY_LOCSOFTWARE = &H4
        DSBPLAY_TERMINATEBY_TIME = &H8
        DSBPLAY_TERMINATEBY_DISTANCE = &H10
        DSBPLAY_TERMINATEBY_PRIORITY = &H20
    End Enum

    Friend Enum DirectSoundBufferLockFlag As UInt32
        None = 0
        FromWriteCursor = &H1
        EntireBuffer = &H2
    End Enum

    <FlagsAttribute>
    Friend Enum DirectSoundBufferStatus As UInt32
        DSBSTATUS_PLAYING = &H1
        DSBSTATUS_BUFFERLOST = &H2
        DSBSTATUS_LOOPING = &H4
        DSBSTATUS_LOCHARDWARE = &H8
        DSBSTATUS_LOCSOFTWARE = &H10
        DSBSTATUS_TERMINATED = &H20
    End Enum

    <FlagsAttribute>
    Friend Enum DirectSoundBufferCaps As UInt32
        DSBCAPS_PRIMARYBUFFER = &H1
        DSBCAPS_STATIC = &H2
        DSBCAPS_LOCHARDWARE = &H4
        DSBCAPS_LOCSOFTWARE = &H8
        DSBCAPS_CTRL3D = &H10
        DSBCAPS_CTRLFREQUENCY = &H20
        DSBCAPS_CTRLPAN = &H40
        DSBCAPS_CTRLVOLUME = &H80
        DSBCAPS_CTRLPOSITIONNOTIFY = &H100
        DSBCAPS_CTRLFX = &H200
        DSBCAPS_STICKYFOCUS = &H4000
        DSBCAPS_GLOBALFOCUS = &H8000
        DSBCAPS_GETCURRENTPOSITION2 = &H10000
        DSBCAPS_MUTE3DATMAXDISTANCE = &H20000
        DSBCAPS_LOCDEFER = &H40000
    End Enum

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure DirectSoundBufferPositionNotify
        Public dwOffset As UInt32
        Public hEventNotify As IntPtr
    End Structure

    'System.Security.SuppressUnmanagedCodeSecurity
    <ComVisible(True), ComImport, Guid("279AFA83-4981-11CE-A521-0020AF0BE560"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IDirectSound

        <PreserveSig()>
        Function CreateSoundBuffer(
         <[In]()> ByVal desc As BufferDescription,
         <Out(), MarshalAs(UnmanagedType.[Interface])> ByRef dsDSoundBuffer As Object,
         <Out()> ByVal pUnkOuter As IntPtr) As Int32

        <PreserveSig()>
        Function GetCaps(<Out()> ByVal caps As IntPtr) As Int32

        <PreserveSig()>
        Function DuplicateSoundBuffer(
        <[In](), MarshalAs(UnmanagedType.[Interface])> ByVal bufferOriginal As IDirectSoundBuffer,
        <Out(), MarshalAs(UnmanagedType.[Interface])> ByVal bufferDuplicate As IDirectSoundBuffer) As Int32

        <PreserveSig()>
        Function SetCooperativeLevel(<[In]()> ByVal HWND As IntPtr,
        <[In](), MarshalAs(UnmanagedType.U4)> ByVal dwLevel As DirectSoundCooperativeLevel) As Int32

        <PreserveSig()>
        Function Compact() As Int32

        <PreserveSig()>
        Function GetSpeakerConfig(<Out()> ByVal pdwSpeakerConfig As IntPtr) As Int32

        <PreserveSig()>
        Function SetSpeakerConfig(<[In]()> ByVal pdwSpeakerConfig As UInt32) As Int32

        <PreserveSig()>
        Function Initialize(
        <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guid As Guid) As Int32
    End Interface

    'DEFINE_GUID(IID_IDirectSound8, 0xC50A7E93, 0xF395, 0x4834, 0x9E, 0xF6, 0x7F, 0xA9, 0x9D, 0xE5, 0x09, 0x66);
    <ComVisible(True), ComImport, Guid("C50A7E93-F395-4834-9EF6-7FA99DE50966"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IDirectSound8

        <PreserveSig()>
        Function CreateSoundBuffer(
         <[In]()> ByVal desc As BufferDescription,
         <Out(), MarshalAs(UnmanagedType.[Interface])> ByRef dsDSoundBuffer As Object,
         <Out()> ByVal pUnkOuter As IntPtr) As Int32

        <PreserveSig()>
        Function GetCaps(<Out()> ByVal caps As IntPtr) As Int32

        <PreserveSig()>
        Function DuplicateSoundBuffer(
        <[In](), MarshalAs(UnmanagedType.[Interface])> ByVal bufferOriginal As IDirectSoundBuffer8,
        <Out(), MarshalAs(UnmanagedType.[Interface])> ByVal bufferDuplicate As IDirectSoundBuffer8) As Int32

        <PreserveSig()>
        Function SetCooperativeLevel(<[In]()> ByVal HWND As IntPtr,
        <[In](), MarshalAs(UnmanagedType.U4)> ByVal dwLevel As DirectSoundCooperativeLevel) As Int32

        <PreserveSig()>
        Function Compact() As Int32

        <PreserveSig()>
        Function GetSpeakerConfig(<Out()> ByVal pdwSpeakerConfig As IntPtr) As Int32

        <PreserveSig()>
        Function SetSpeakerConfig(<[In]()> ByVal pdwSpeakerConfig As UInt32) As Int32

        <PreserveSig()>
        Function Initialize(
        <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal guid As Guid) As Int32

        '// IDirectSound8 methods
        ' STDMETHOD(VerifyCertification)  (THIS_ _Out_ LPDWORD pdwCertified) PURE;
        Function VerifyCertification(<Out()> ByRef pdwCertified As IntPtr) As Int32

    End Interface

    'DEFINE_GUID(IID_IDirectSoundBuffer, 0x279AFA85, 0x4981, 0x11CE, 0xA5, 0x21, 0x00, 0x20, 0xAF, 0x0B, 0xE5, 0x60);
    <ComVisible(True), ComImport, Guid("279AFA85-4981-11CE-A521-0020AF0BE560"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IDirectSoundBuffer
        'void GetCaps([MarshalAs(UnmanagedType.LPStruct)] BufferCaps pBufferCaps);
        <PreserveSig()>
        Function GetCaps(<Out(), MarshalAs(UnmanagedType.LPStruct)> ByVal pBufferCaps As BufferCaps) As Integer

        'void GetCurrentPosition([Out] out uint currentPlayCursor, [Out] out uint currentWriteCursor);
        <PreserveSig()>
        Function GetCurrentPosition(<Out()> ByRef currentPlayCursor As UInt32,
                                    <Out()> ByRef currentWriteCursor As UInt32) As Integer

        'void GetFormat();
        'HRESULT GetFormat( LPWAVEFORMATEX lpwfxFormat, DWORD dwSizeAllocated, LPDWORD lpdwSizeWritten );
        <PreserveSig()>
        Function GetFormat(<Out(), MarshalAs(UnmanagedType.LPStruct)> ByVal lpwfxFormat As WaveFormat,
                           <Out()> ByVal dwSizeAllocated As UInt32,
                           <Out()> ByRef lpdwSizeWritten As UInt32) As Integer

        '[return: MarshalAs(UnmanagedType.I4)]
        ' int GetVolume();
        <PreserveSig()>
        Function GetVolume(<Out()> ByRef lplVolume As Int32) As Integer

        ' void GetPan([Out] out uint pan);
        <PreserveSig()>
        Function GetPan(<Out()> ByRef lplPan As Int32) As Int32

        '[return: MarshalAs(UnmanagedType.I4)]
        'int GetFrequency();
        <PreserveSig()>
        Function GetFrequency(<Out()> ByRef lpdwFrequency As UInt32) As Integer

        '[return MarshalAs(UnmanagedType.U4)]
        'DirectSoundBufferStatus GetStatus();
        <PreserveSig()>
        Function GetStatus(<Out(), MarshalAs(UnmanagedType.U4)> ByRef plpdwStatus As UInt32) As Integer

        'void Initialize([In, MarshalAs(UnmanagedType.Interface)] IDirectSound directSound, [In] BufferDescription desc);
        <PreserveSig()>
        Function Initialize(<[In](), MarshalAs(UnmanagedType.Interface)> ByVal directSound As IDirectSound,
                            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal desc As BufferDescription) As Int32

        'void Lock(int dwOffset, uint dwBytes, [Out] out IntPtr audioPtr1, [Out] out int audioBytes1, [Out] out IntPtr audioPtr2, [Out] out int audioBytes2, [MarshalAs(UnmanagedType.U4)] DirectSoundBufferLockFlag dwFlags);
        <PreserveSig()>
        Function Lock(<[In]()> ByVal dwOffset As UInt32,
                 <[In]()> ByVal dwBytes As UInt32,
                 <Out()> ByRef audioPtr1 As IntPtr,
                 <Out()> ByRef audioBytes1 As UInt32,
                 <Out()> ByRef audioPtr2 As IntPtr,
                 <Out()> ByRef audioBytes2 As UInt32,
                 <[In](), MarshalAs(UnmanagedType.U4)> ByVal dwFlags As DirectSoundBufferLockFlag) As Int32

        ' void Play(uint dwReserved1, uint dwPriority, [In, MarshalAs(UnmanagedType.U4)] DirectSoundPlayFlags dwFlags);
        <PreserveSig()>
        Function Play(<[In]()> ByVal dwReserved1 As UInt32,
                 <[In]()> ByVal dwPriority As UInt32,
                 <[In](), MarshalAs(UnmanagedType.U4)> ByVal dwFlags As DirectSoundPlayFlags) As Int32

        'void SetCurrentPosition(uint dwNewPosition);
        <PreserveSig()>
        Function SetCurrentPosition(<[In]()> ByVal dwNewPosition As UInt32) As Int32

        'void SetFormat([In] WaveFormat pcfxFormat);
        <PreserveSig()>
        Sub SetFormat(<[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal pcfxFormat As WaveFormat)

        'void SetVolume(int volume);
        <PreserveSig()>
        Function SetVolume(<[In]()> ByVal volume As Int32) As Int32

        'void SetPan(uint pan);
        <PreserveSig()>
        Function SetPan(<[In]()> ByVal pan As Int32) As Int32

        ' void SetFrequency(uint frequency);
        <PreserveSig()>
        Function SetFrequency(<[In]()> ByVal frequency As UInt32) As Int32

        'void Stop();
        <PreserveSig()>
        Function [Stop]() As Int32

        'void Lock(int dwOffset, uint dwBytes, [Out] out IntPtr audioPtr1, [Out] out int audioBytes1, [Out] out IntPtr audioPtr2, [Out] out int audioBytes2, [MarshalAs(UnmanagedType.U4)] DirectSoundBufferLockFlag dwFlags);
        <PreserveSig()>
        Function Unlock(<[In]()> ByVal pvAudioPtr1 As IntPtr,
                 <[In]()> ByVal dwAudioBytes1 As UInt32,
                 <[In]()> ByVal pvAudioPtr2 As IntPtr,
                 <[In]()> ByVal dwAudioBytes2 As UInt32) As Int32

        'void Restore();
        <PreserveSig()>
        Function Restore() As Int32
    End Interface

    <ComVisible(True), ComImport, Guid("6825A449-7524-4D82-920F-50E36AB3AB1E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IDirectSoundBuffer8
        'void GetCaps([MarshalAs(UnmanagedType.LPStruct)] BufferCaps pBufferCaps);
        <PreserveSig()>
        Function GetCaps(<Out(), MarshalAs(UnmanagedType.LPStruct)> ByVal pBufferCaps As BufferCaps) As Integer

        'void GetCurrentPosition([Out] out uint currentPlayCursor, [Out] out uint currentWriteCursor);
        <PreserveSig()>
        Function GetCurrentPosition(<Out()> ByRef currentPlayCursor As UInt32,
                                    <Out()> ByRef currentWriteCursor As UInt32) As Integer

        'void GetFormat();
        'HRESULT GetFormat( LPWAVEFORMATEX lpwfxFormat, DWORD dwSizeAllocated, LPDWORD lpdwSizeWritten );
        <PreserveSig()>
        Function GetFormat(<Out(), MarshalAs(UnmanagedType.LPStruct)> ByVal lpwfxFormat As WaveFormat,
                           <Out()> ByVal dwSizeAllocated As UInt32,
                           <Out()> ByRef lpdwSizeWritten As UInt32) As Integer

        '[return: MarshalAs(UnmanagedType.I4)]
        ' int GetVolume();
        <PreserveSig()>
        Function GetVolume(<Out()> ByRef lplVolume As Int32) As Integer

        ' void GetPan([Out] out uint pan);
        <PreserveSig()>
        Function GetPan(<Out()> ByRef lplPan As Int32) As Int32

        '[return: MarshalAs(UnmanagedType.I4)]
        'int GetFrequency();
        <PreserveSig()>
        Function GetFrequency(<Out()> ByRef lpdwFrequency As UInt32) As Integer

        '[return MarshalAs(UnmanagedType.U4)]
        'DirectSoundBufferStatus GetStatus();
        <PreserveSig()>
        Function GetStatus(<Out(), MarshalAs(UnmanagedType.U4)> ByRef plpdwStatus As UInt32) As Integer

        'void Initialize([In, MarshalAs(UnmanagedType.Interface)] IDirectSound directSound, [In] BufferDescription desc);
        <PreserveSig()>
        Function Initialize(<[In](), MarshalAs(UnmanagedType.Interface)> ByVal directSound As IDirectSound,
                            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal desc As BufferDescription) As Int32

        'void Lock(int dwOffset, uint dwBytes, [Out] out IntPtr audioPtr1, [Out] out int audioBytes1, [Out] out IntPtr audioPtr2, [Out] out int audioBytes2, [MarshalAs(UnmanagedType.U4)] DirectSoundBufferLockFlag dwFlags);
        <PreserveSig()>
        Function Lock(<[In]()> ByVal dwOffset As UInt32,
                 <[In]()> ByVal dwBytes As UInt32,
                 <Out()> ByRef audioPtr1 As IntPtr,
                 <Out()> ByRef audioBytes1 As UInt32,
                 <Out()> ByRef audioPtr2 As IntPtr,
                 <Out()> ByRef audioBytes2 As UInt32,
                 <[In](), MarshalAs(UnmanagedType.U4)> ByVal dwFlags As DirectSoundBufferLockFlag) As Int32

        ' void Play(uint dwReserved1, uint dwPriority, [In, MarshalAs(UnmanagedType.U4)] DirectSoundPlayFlags dwFlags);
        <PreserveSig()>
        Function Play(<[In]()> ByVal dwReserved1 As UInt32,
                 <[In]()> ByVal dwPriority As UInt32,
                 <[In](), MarshalAs(UnmanagedType.U4)> ByVal dwFlags As DirectSoundPlayFlags) As Int32

        'void SetCurrentPosition(uint dwNewPosition);
        <PreserveSig()>
        Function SetCurrentPosition(<[In]()> ByVal dwNewPosition As UInt32) As Int32

        'void SetFormat([In] WaveFormat pcfxFormat);
        <PreserveSig()>
        Sub SetFormat(<[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal pcfxFormat As WaveFormat)

        'void SetVolume(int volume);
        <PreserveSig()>
        Function SetVolume(<[In]()> ByVal volume As Int32) As Int32

        'void SetPan(uint pan);
        <PreserveSig()>
        Function SetPan(<[In]()> ByVal pan As Int32) As Int32

        ' void SetFrequency(uint frequency);
        <PreserveSig()>
        Function SetFrequency(<[In]()> ByVal frequency As UInt32) As Int32

        'void Stop();
        <PreserveSig()>
        Function [Stop]() As Int32

        'void Lock(int dwOffset, uint dwBytes, [Out] out IntPtr audioPtr1, [Out] out int audioBytes1, [Out] out IntPtr audioPtr2, [Out] out int audioBytes2, [MarshalAs(UnmanagedType.U4)] DirectSoundBufferLockFlag dwFlags);
        <PreserveSig()>
        Function Unlock(<[In]()> ByVal pvAudioPtr1 As IntPtr,
                 <[In]()> ByVal dwAudioBytes1 As UInt32,
                 <[In]()> ByVal pvAudioPtr2 As IntPtr,
                 <[In]()> ByVal dwAudioBytes2 As UInt32) As Int32

        'void Restore();
        <PreserveSig()>
        Function Restore() As Int32


        ' // IDirectSoundBuffer8 methods
        'STDMETHOD(SetFX)                (THIS_ DWORD dwEffectsCount, _In_reads_opt_(dwEffectsCount) LPDSEFFECTDESC pDSFXDesc, _Out_writes_opt_(dwEffectsCount) LPDWORD pdwResultCodes) PURE;
        <PreserveSig()>
        Function SetFX(ByVal dwEffectsCount As UInt32,
                       <[In]()> ByVal pDSFXDesc As IntPtr,
                       <Out()> ByRef pdwResultCodes As UInt32) As Int32

        'STDMETHOD(AcquireResources)     (THIS_ DWORD dwFlags, DWORD dwEffectsCount, _Out_writes_(dwEffectsCount) LPDWORD pdwResultCodes) PURE;
        <PreserveSig()>
        Function AcquireResources(ByVal dwFlags As UInt32,
                                  ByVal dwEffectsCount As UInt32,
                                  <Out()> ByRef pdwResultCodes As UInt32) As Int32


        'STDMETHOD(GetObjectInPath)      (THIS_ _In_ REFGUID rguidObject, DWORD dwIndex, _In_ REFGUID rguidInterface, _Outptr_ LPVOID *ppObject) PURE;
        <PreserveSig()>
        Function GetObjectInPath(<[In]()> ByVal rguidObject As Guid,
                                ByVal dwIndex As UInt32,
                                <[In]()> ByVal rguidInterface As Guid,
                                 ByRef ppObject As IntPtr) As Int32
    End Interface


    <ComImport, Guid("b0210783-89cd-11d0-af08-00a0c925cd16"), System.Security.SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IDirectSoundNotify
        Sub SetNotificationPositions(ByVal dwPositionNotifies As UInt32,
                                    <[In], MarshalAs(UnmanagedType.LPArray)> ByVal pcPositionNotifies() As DirectSoundBufferPositionNotify)
    End Interface


    <DllImport("dsound.dll", EntryPoint:="DirectSoundCreate", SetLastError:=True, CharSet:=CharSet.Unicode, ExactSpelling:=True, CallingConvention:=CallingConvention.Winapi)>
    Private Shared Sub DirectSoundCreate(ByRef GUID As Guid,
    <[Out](), MarshalAs(UnmanagedType.[Interface])> ByRef directSound As IDirectSound, ByVal pUnkOuter As IntPtr)

    End Sub

    <DllImport("dsound.dll", EntryPoint:="DirectSoundCreate8", SetLastError:=True, CharSet:=CharSet.Unicode, ExactSpelling:=True, CallingConvention:=CallingConvention.Winapi)>
    Private Shared Sub DirectSoundCreate8(ByRef GUID As Guid,
    <[Out](), MarshalAs(UnmanagedType.[Interface])> ByRef directSound As IDirectSound8, ByVal pUnkOuter As IntPtr)

    End Sub


    Public Shared ReadOnly DSDEVID_DefaultPlayback As Guid = New Guid("DEF00000-9C6D-47ED-AAF1-4DDA8F2B5C03")
    Public Shared ReadOnly DSDEVID_DefaultCapture As Guid = New Guid("DEF00001-9C6D-47ED-AAF1-4DDA8F2B5C03")
    Public Shared ReadOnly DSDEVID_DefaultVoicePlayback As Guid = New Guid("DEF00002-9C6D-47ED-AAF1-4DDA8F2B5C03")
    Public Shared ReadOnly DSDEVID_DefaultVoiceCapture As Guid = New Guid("DEF00003-9C6D-47ED-AAF1-4DDA8F2B5C03")

    Delegate Function DSEnumCallback(ByVal lpGuid As IntPtr, ByVal lpcstrDescription As IntPtr, ByVal lpcstrModule As IntPtr, ByVal lpContext As IntPtr) As Boolean

    <DllImport("dsound.dll", EntryPoint:="DirectSoundEnumerateA", SetLastError:=True, CharSet:=CharSet.Unicode, ExactSpelling:=True, CallingConvention:=CallingConvention.Winapi)>
    Private Shared Sub DirectSoundEnumerate(ByVal lpDSEnumCallback As DSEnumCallback, ByVal lpContext As IntPtr)
    End Sub

    <DllImport("user32.dll")>
    Private Shared Function GetDesktopWindow() As IntPtr
    End Function
#End Region
#Region "IDisposable Support"
    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then

            If SecondaryBuffer IsNot Nothing Then
                Marshal.ReleaseComObject(SecondaryBuffer)
                SecondaryBuffer = Nothing
                objSecSoundBuffer = Nothing
            End If

            If PrimaryBuffer IsNot Nothing Then
                Marshal.ReleaseComObject(PrimaryBuffer)
                PrimaryBuffer = Nothing
            End If

            If Device IsNot Nothing Then
                Marshal.ReleaseComObject(Device)
                Device = Nothing
            End If

            sWaveFormatDs = Nothing

            If DeviceLst IsNot Nothing Then
                DeviceLst.Clear()
                DeviceLst = Nothing
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
#Region "Native Wrapper"

    Public Structure DirectSoundDeviceInfo
        Public DeviceGuid As Guid
        Public DeviceName As String
    End Structure

    Private Device As IDirectSound8

    Private PrimaryBuffer As IDirectSoundBuffer = Nothing
    Private SecondaryBuffer As IDirectSoundBuffer8 = Nothing

    Private sWaveFormatDs As New WaveFormat
    Private objSecSoundBuffer As Object = Nothing

    Private DeviceLst As List(Of DirectSoundDeviceInfo)

    ''' <summary>
    '''  Create Device (IDirectSound8 struct) + Set cooperative level to Priority
    ''' </summary>
    ''' <param name="pHandle">Handle of main window</param>
    ''' <param name="deviceGuid">Guid of device to init</param>
    ''' <returns>Return true if IDirectSound8 is created correctly</returns>
    Public Function CreateDevice(ByVal pHandle As IntPtr, ByVal deviceGuid As Guid) As Boolean
        Dim bResult As Boolean = False

        ' Init default device
        Device = Nothing

        'If guid is empty use Default device
        If deviceGuid = Guid.Empty Then
            deviceGuid = DSDEVID_DefaultPlayback
        End If

        ' Create IDirectSound8 interface
        DirectSoundCreate8(deviceGuid, Device, IntPtr.Zero)

#If DEBUG Then
        If Device Is Nothing Then
            DebugPrintLine("DirectSoundNative", "Failed to open device")
        End If
#End If

        If Device IsNot Nothing Then
            'Set cooperative level
            If pHandle <> IntPtr.Zero Then
                Device.SetCooperativeLevel(pHandle, DirectSoundCooperativeLevel.DSSCL_PRIORITY)
            Else
                Device.SetCooperativeLevel(GetDesktopWindow(), DirectSoundCooperativeLevel.DSSCL_PRIORITY)
            End If

            bResult = True
#If DEBUG Then
            DebugPrintLine("DirectSoundNative", "Init DirectSound Successful")
#End If
        End If

        Return bResult
    End Function

    ''' <summary>
    ''' Close Device(IDirectSound8) 
    ''' </summary>
    ''' <returns>Return true</returns>
    Public Function CloseDevice() As Boolean
        Dim bResult As Boolean = True

        If Device IsNot Nothing Then
            Marshal.ReleaseComObject(Device)
            Device = Nothing
        End If


#If DEBUG Then
        DebugPrintLine("DirectSoundNative", "DeInit DirectSound Successful")
#End If

        Return bResult
    End Function

    ''' <summary>
    ''' Open Primary buffer and Secondary Buffer
    ''' </summary>
    ''' <param name="sFormat">Valid WaveFormat structure</param>
    ''' <param name="nByteBufferLen">Len in byte of the buffer</param>
    ''' <returns>True if succeded</returns>
    Public Function CreateBuffers(ByVal sFormat As WaveFormatEx, ByVal nByteBufferLen As Integer) As Boolean
        'Primary Buffer
        Dim objSoundBuffer As Object = Nothing
        Dim sBufferDescr As New BufferDescription

        'SecondaryBuffer
        Dim sSecBufferDescr As New BufferDescription
        Dim ptrWaveFormat As GCHandle

        Dim bResult As Boolean = False

        ' Close Buffers if are open
        Me.CloseBuffers()

        ' Set new wave format (based on decoder values)
        With sWaveFormatDs
            .nSamplesPerSec = sFormat.Samplerate
            .nChannels = sFormat.Channels
            .wBitsPerSample = sFormat.BitsPerSample
            .nBlockAlign = sFormat.BlockAlign
            .nAvgBytesPerSec = sFormat.AvgBytesPerSec
            .wFormatTag = 1 'PCM
            .cbSize = 0
        End With


        'Primary Buffer creation
        With sBufferDescr
            .dwSize = Marshal.SizeOf(GetType(BufferDescription))
            .dwFlags = DirectSoundBufferCaps.DSBCAPS_PRIMARYBUFFER
            .dwBufferBytes = 0
            .dwReserved = 0
            .guidAlgo = Guid.Empty
            .lpwfxFormat = IntPtr.Zero
        End With

        ' Create primary buffer and play loop
        Device.CreateSoundBuffer(sBufferDescr, objSoundBuffer, IntPtr.Zero)
        PrimaryBuffer = TryCast(objSoundBuffer, IDirectSoundBuffer)

        PrimaryBuffer.Play(0, 0, DirectSoundPlayFlags.DSBPLAY_LOOPING)


        'Secondary Buffer Creation
        ptrWaveFormat = GCHandle.Alloc(sWaveFormatDs, GCHandleType.Pinned)

        With sSecBufferDescr
            .dwSize = Marshal.SizeOf(GetType(BufferDescription))
            .dwFlags = (DirectSoundBufferCaps.DSBCAPS_GETCURRENTPOSITION2 _
                     Or DirectSoundBufferCaps.DSBCAPS_CTRLPOSITIONNOTIFY _
                     Or DirectSoundBufferCaps.DSBCAPS_GLOBALFOCUS _
                     Or DirectSoundBufferCaps.DSBCAPS_CTRLVOLUME _
                     Or DirectSoundBufferCaps.DSBCAPS_STICKYFOCUS _
                     Or DirectSoundBufferCaps.DSBCAPS_CTRLPAN)
            .dwBufferBytes = CUInt(nByteBufferLen)
            .dwReserved = 0
            .guidAlgo = Guid.Empty
            .lpwfxFormat = ptrWaveFormat.AddrOfPinnedObject()
        End With

        ' Create secondary buffer
        Device.CreateSoundBuffer(sSecBufferDescr, objSecSoundBuffer, IntPtr.Zero)
        SecondaryBuffer = TryCast(objSecSoundBuffer, IDirectSoundBuffer8)

        ptrWaveFormat.Free()

        'Get effective SecondaryBuffer size
        Dim sBufferCaps As New BufferCaps()

        sBufferCaps.dwSize = Marshal.SizeOf(GetType(BufferCaps))
        SecondaryBuffer.GetCaps(sBufferCaps)

        If nByteBufferLen <> sBufferCaps.dwBufferBytes Then

#If DEBUG Then
            DebugPrintLine("DirectSoundNative", "nByteBufferLen size has been modificated with buffer caps size")
#End If

            nByteBufferLen = sBufferCaps.dwBufferBytes
        End If

#If DEBUG Then
        DebugPrintLine("DirectSoundNative", "Directsound correctly opened")
#End If

        bResult = True
    End Function

    ''' <summary>
    ''' Close Primary and Secondary Buffer
    ''' </summary>
    ''' <returns></returns>
    Public Function CloseBuffers() As Boolean
        ' Close secondary buffer
        If SecondaryBuffer IsNot Nothing Then
            SecondaryBuffer.Stop()
            Marshal.ReleaseComObject(SecondaryBuffer)
            SecondaryBuffer = Nothing
            objSecSoundBuffer = Nothing
        End If

        ' Close primary buffer
        If PrimaryBuffer IsNot Nothing Then
            PrimaryBuffer.Stop()
            Marshal.ReleaseComObject(PrimaryBuffer)
            PrimaryBuffer = Nothing
        End If

#If DEBUG Then
        DebugPrintLine("DirectSoundNative", "Directsound correctly closed")
#End If

        Return True
    End Function

    ''' <summary>
    ''' Create position notification in Secondary Buffer
    ''' 
    ''' To create Event use: 
    ''' New EventWaitHandle(False, EventResetMode.AutoReset)
    ''' 
    ''' To wait event in wait thread use:
    ''' WaitHandle.WaitAny(WaitHandles, 200, False)
    ''' 
    ''' Utility:
    ''' UInt32.MaxValue Offset called only on Stop() function
    ''' </summary>
    ''' <param name="pEvent1">DangerousGetHandle() of first Event</param>
    ''' <param name="nOffset1">Position in Byte in Secondary Buffer</param>
    ''' <param name="pEvent2">DangerousGetHandle() of second Event</param>
    ''' <param name="nOffset2">Position in Byte in Secondary Buffer</param>
    ''' <param name="pEvent3">DangerousGetHandle() of third Event</param>
    ''' <param name="nOffset3">Position in Byte in Secondary Buffer</param>
    ''' <returns></returns>
    Public Function Set3Notification(ByVal pEvent1 As IntPtr,
                                     ByVal nOffset1 As UInt32,
                                     ByVal pEvent2 As IntPtr,
                                     ByVal nOffset2 As UInt32,
                                     ByVal pEvent3 As IntPtr,
                                     ByVal nOffset3 As UInt32) As Boolean

        Dim bResult As Boolean = False

        If objSecSoundBuffer IsNot Nothing Then
            ' QueryInterface 
            Dim iNotify As IDirectSoundNotify = TryCast(objSecSoundBuffer, IDirectSoundNotify)
            Dim iBufferNotifies(3) As DirectSoundBufferPositionNotify

            'Set buffer notifications
            iBufferNotifies(0) = New DirectSoundBufferPositionNotify()
            iBufferNotifies(0).dwOffset = nOffset1 '0
            iBufferNotifies(0).hEventNotify = pEvent1

            iBufferNotifies(1) = New DirectSoundBufferPositionNotify()
            iBufferNotifies(1).dwOffset = nOffset2 'CUInt(nByteBufferLen \ 2)
            iBufferNotifies(1).hEventNotify = pEvent2

            iBufferNotifies(2) = New DirectSoundBufferPositionNotify()
            iBufferNotifies(2).dwOffset = nOffset3 ' UInt32.MaxValue ' = 0xFFFFFFFF
            iBufferNotifies(2).hEventNotify = pEvent3

            iNotify.SetNotificationPositions(3, iBufferNotifies)

            bResult = True
        End If

#If DEBUG Then
        If bResult = True Then
            DebugPrintLine("DirectSoundNative", "DirectSound 3 Notification Set")
        End If
#End If
        Return bResult
    End Function

    ''' <summary>
    ''' Check if Secondary buffer is lost
    ''' </summary>
    ''' <returns>True if lost</returns>
    Public Function IsBufferLost() As Boolean
        Dim result As Boolean = False
        Dim pStatus As UInt32

        If SecondaryBuffer IsNot Nothing Then
            SecondaryBuffer.GetStatus(pStatus)

            If pStatus = DirectSoundBufferStatus.DSBSTATUS_BUFFERLOST Then
                result = True
            End If

        End If

#If DEBUG Then
        If result = True Then
            DebugPrintLine("DirectSoundNative", "DirectSound Buffer is Lost")
        End If
#End If

        Return result
    End Function

    ''' <summary>
    ''' Check if buffer is lost with IsBufferLost() function
    ''' if true restore buffer
    ''' </summary>
    Public Sub RestoreBuffer()
        If SecondaryBuffer IsNot Nothing Then
#If DEBUG Then
            DebugPrintLine("DirectSoundNative", "Buffer Restore")
#End If

            SecondaryBuffer.Restore()
        End If
    End Sub

    ''' <summary>
    ''' Write PCM data in Secondary Buffer
    ''' </summary>
    ''' <param name="data">Byte array contnet PCM data</param>
    ''' <param name="dataLen">data array Lenght</param>
    ''' <param name="writeIndex">Offset where start to write in secondary buffer</param>
    ''' <returns></returns>
    Public Function WriteBuffer(ByRef data() As Byte, ByVal dataLen As Int32, ByVal writeIndex As Int32) As Boolean
        Dim pAudioData1, pAudioData2 As IntPtr
        Dim nAudioLen1, nAudioLen2 As UInt32
        Dim returnValue As Integer = 0

        If SecondaryBuffer IsNot Nothing Then

            returnValue = SecondaryBuffer.Lock(CUInt(writeIndex),
                                               CUInt(dataLen),
                                               pAudioData1,
                                               nAudioLen1,
                                               pAudioData2,
                                               nAudioLen2,
                                               DirectSoundBufferLockFlag.None)

            ' Check buffer lost and restore
            If returnValue = DirectSoundBufferStatus.DSBSTATUS_BUFFERLOST Then
#If DEBUG Then
                DebugPrintLine("DirectSoundNative", " DirectSound Buffer is Lost")
#End If

                ' Restore
                SecondaryBuffer.Restore()

                ' Try to write again
                SecondaryBuffer.Lock(CUInt(writeIndex),
                                     CUInt(dataLen),
                                     pAudioData1,
                                     nAudioLen1,
                                     pAudioData2,
                                     nAudioLen2,
                                     DirectSoundBufferLockFlag.None)
            End If

            ' Copy data to unmanaged memory (see MSDN documentation)
            If (pAudioData2 <> IntPtr.Zero) And (nAudioLen2 <> 0) Then
                Marshal.Copy(data, 0, pAudioData1, CInt(nAudioLen1))
                Marshal.Copy(data, CInt(nAudioLen1), pAudioData2, CInt(nAudioLen2))
            Else
                If (pAudioData1 <> IntPtr.Zero) And (nAudioLen1 <> 0) Then
                    Marshal.Copy(data, 0, pAudioData1, CInt(nAudioLen1))
                End If

            End If

#If DEBUG Then
            DebugPrintLine("DirectSoundNative", "Write() Buffer")
            DebugPrintLine("DirectSoundNative", "nAudioLen1: " + nAudioLen1.ToString())
            DebugPrintLine("DirectSoundNative", "nAudioLen2:   " + nAudioLen2.ToString())
            DebugPrintLine("DirectSoundNative", "dataLen: " + dataLen.ToString())
#End If

            'Unlock buffer after write
            SecondaryBuffer.Unlock(pAudioData1,
                                   nAudioLen1,
                                   pAudioData2,
                                   nAudioLen2)

        End If

        Return True
    End Function

    ''' <summary>
    ''' Play in Secondary Buffer
    ''' </summary>
    Public Sub PlayLoop()
        If SecondaryBuffer IsNot Nothing Then
            SecondaryBuffer.Play(0, 0, DirectSoundPlayFlags.DSBPLAY_LOOPING)
        End If

#If DEBUG Then
        DebugPrintLine("DirectSoundNative", "PlayLoop()")
#End If

    End Sub

    ''' <summary>
    ''' Stopin secodary Buffer
    ''' </summary>
    Public Sub StopLoop()
        If SecondaryBuffer IsNot Nothing Then
            SecondaryBuffer.Stop()

#If DEBUG Then
            DebugPrintLine("DirectSoundNative", "StopLoop()")
#End If
        End If
    End Sub

    ''' <summary>
    ''' Get write position in Secondary Buffer
    ''' </summary>
    ''' <returns>Write position Index</returns>
    Public ReadOnly Property WritePositionIndex As Int32
        Get
            If SecondaryBuffer IsNot Nothing Then
                Dim nPlayCursor, nWriteCursor As UInt32
                SecondaryBuffer.GetCurrentPosition(nPlayCursor, nWriteCursor)

#If DEBUG Then
                DebugPrintLine("DirectSoundNative", "WritePositionIndex: " + nWriteCursor.ToString())
#End If

                Return CInt(nWriteCursor)
            End If
        End Get
    End Property

    ''' <summary>
    ''' Get or Set Play position Index in Secondary Buffer
    ''' </summary>
    ''' <returns>Play position Index</returns>
    Public Property PlayPositionIndex As Int32
        Get
            If SecondaryBuffer IsNot Nothing Then
                Dim nPlayCursor, nWriteCursor As UInt32
                SecondaryBuffer.GetCurrentPosition(nPlayCursor, nWriteCursor)

                '#If DEBUG Then
                '                DebugPrintLine("DirectSoundNative", "Get PlayPositionIndex: " + nPlayCursor.ToString())
                '#End If

                Return CInt(nPlayCursor)
            End If
        End Get
        Set(ByVal value As Integer)
            If SecondaryBuffer IsNot Nothing Then
                SecondaryBuffer.SetCurrentPosition(CUInt(value))
#If DEBUG Then
                DebugPrintLine("DirectSoundNative", "Set PlayPositionIndex: " + value.ToString())
#End If

            End If
        End Set
    End Property

    ''' <summary>
    ''' Set Volume attenuation in DB
    ''' 0 = Max vol (No attentuation)
    ''' -10000 = Min vol (max attentuation)
    ''' </summary>
    ''' <returns>Volume (0 to -10000)</returns>
    Public Property Volume As Int32
        Get
            If SecondaryBuffer IsNot Nothing Then
                Dim nDBVolume As Integer
                SecondaryBuffer.GetVolume(nDBVolume)

#If DEBUG Then
                DebugPrintLine("DirectSoundNative", "Get Volume " + nDBVolume.ToString())
#End If
                Return nDBVolume
            End If
        End Get
        Set(ByVal value As Integer)
            If SecondaryBuffer IsNot Nothing Then
                SecondaryBuffer.SetVolume(value)
#If DEBUG Then
                DebugPrintLine("DirectSoundNative", "Set Volume " + value.ToString())
#End If
            End If
        End Set
    End Property

    ''' <summary>
    ''' Set Pan 
    ''' -10000 (Max attentuation for Right channel)
    ''' + 10000 (Max attentuation for Left Channel)
    ''' </summary>
    ''' <returns>Pan (-10000 to +10000)</returns>
    Public Property Pan As Int32
        Get
            If SecondaryBuffer IsNot Nothing Then
                Dim nDBPan As Integer
                SecondaryBuffer.GetPan(nDBPan)

#If DEBUG Then
                DebugPrintLine("DirectSoundNative", "Get Pan " + nDBPan.ToString())
#End If
                Return nDBPan
            End If
        End Get
        Set(ByVal value As Integer)
            If SecondaryBuffer IsNot Nothing Then
                SecondaryBuffer.SetPan(value)
#If DEBUG Then
                DebugPrintLine("DirectSoundNative", "Set Pan " + value.ToString())
#End If
            End If
        End Set
    End Property

    ''' <summary>
    ''' Get list of devices in the system as List(Of DirectSoundDeviceInfo)
    ''' </summary>
    ''' <returns>List(Of DirectSoundDeviceInfo)</returns>
    Public ReadOnly Property DeviceList As List(Of DirectSoundDeviceInfo)
        Get
            DeviceLst = New List(Of DirectSoundDeviceInfo)
            DirectSoundEnumerate(New DSEnumCallback(AddressOf DeviceEnumCallback), IntPtr.Zero)
            Return DeviceLst
        End Get
    End Property

    ' Delegate function for device enums
    Private Function DeviceEnumCallback(ByVal lpGuid As IntPtr, ByVal lpcstrDescription As IntPtr, ByVal lpcstrModule As IntPtr, ByVal lpContext As IntPtr) As Boolean
        Dim device As New DirectSoundDeviceInfo

        If lpGuid = IntPtr.Zero Then
            device.DeviceGuid = Guid.Empty
        Else
            Dim guidBytes(15) As Byte
            Marshal.Copy(lpGuid, guidBytes, 0, 15)
            device.DeviceGuid = New Guid(guidBytes)
        End If

        device.DeviceName = Marshal.PtrToStringAnsi(lpcstrDescription)
        DeviceLst.Add(device)

        Return True
    End Function

#End Region
End Class
