Imports System.Runtime.InteropServices
Namespace DirectShow
    <StructLayout(LayoutKind.Sequential), ComVisible(False)> _
       Public Class VideoInfoHeader
        Public rcSource As Rect
        Public rcTarget As Rect
        Public dwBitrate As Int32
        Public dwBitErrorRate As Int32
        Public AvgTimePerFrame As Int64
        Public bmiHeader As BitmapInfoHeader
    End Class

    <StructLayout(LayoutKind.Sequential), ComVisible(False)> _
    Public Class BitmapInfoHeader
        Public biSize As Int32
        Public biWidth As Int32
        Public biHeight As Int32
        Public biPlanes As Int16
        Public biBitCount As Int16
        Public biCompression As Int32
        Public biSizeImage As Int32
        Public biXPelsPerMeter As Int32
        Public biYPelsPerMeter As Int32
        Public biClrUsed As Int32
        Public biClrImportant As Int32
    End Class

    <StructLayout(LayoutKind.Sequential), ComVisible(True)> _
    Public Class Rect
        Public Left As Int32
        Public Top As Int32
        Public Right As Int32
        Public Bottom As Int32
    End Class

    <StructLayout(LayoutKind.Sequential), ComVisible(False)> _
    Public Class WaveFormatEx
        Public wFormatTag As Int16
        Public nChannels As Int16
        Public nSamplesPerSec As Int32
        Public nAvgBytesPerSec As Int32
        Public nBlockAlign As Int16
        Public wBitsPerSample As Int16
        Public cbSize As Int16
    End Class

    <StructLayout(LayoutKind.Sequential), ComVisible(False)> _
    Public Class AMMediaType
        Public majorType As Guid
        Public subType As Guid
        <MarshalAs(UnmanagedType.Bool)> Public fixedSizeSamples As Boolean
        <MarshalAs(UnmanagedType.Bool)> Public temporalCompression As Boolean
        Public sampleSize As Integer
        Public formatType As Guid
        Public unkPtr As IntPtr
        Public formatSize As Integer
        Public formatPtr As IntPtr
    End Class

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode), ComVisible(False)> _
    Public Class FilterInfo
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)> Public achName As String
        <MarshalAs(UnmanagedType.IUnknown)> Public pUnk As Object
    End Class

    Public Enum EventCode
        NONE
        COMPLETE = &H1
        USERABORT = &H2
        ERRORABORT = &H3
        TIME = &H4
        REPAINT = &H5
        STREAM_ERROR_STOPPED = &H6
        STREAM_ERROR_STILLPLAYING = &H7
        ERROR_STILLPLAYING = &H8
        PALETTE_CHANGED = &H9
        VIDEO_SIZE_CHANGED = &HA
        QUALITY_CHANGE = &HB
        SHUTTING_DOWN = &HC
        CLOCK_CHANGED = &HD
        PAUSED = &HE
        OPENING_FILE = &H10
        BUFFERING_DATA = &H11
        FULLSCREEN_LOST = &H12
        ACTIVATE = &H13
        NEED_RESTART = &H14
        WINDOW_DESTROYED = &H15
        DISPLAY_CHANGED = &H16
        STARVATION = &H17
        OLE_EVENT = &H18
        NOTIFY_WINDOW = &H19
    End Enum

    <Flags(), ComVisible(False)> _
    Public Enum SeekingCapabilities
        CanSeekAbsolute = &H1
        CanSeekForwards = &H2
        CanSeekBackwards = &H4
        CanGetCurrentPos = &H8
        CanGetStopPos = &H10
        CanGetDuration = &H20
        CanPlayBackwards = &H40
        CanDoSegments = &H80
        Source = &H100
    End Enum

    <Flags(), ComVisible(False)> _
    Public Enum SeekingFlags
        NoPositioning = &H0         '	// No change
        AbsolutePositioning = &H1  '	// Position is supplied and is absolute
        RelativePositioning = &H2  '	// Position is supplied and is relative
        IncrementalPositioning = &H3  '	// (Stop) position relative to current, useful for seeking when paused (use +1)
        PositioningBitsMask = &H3  '	// Useful mask
        SeekToKeyFrame = &H4        '	// Just seek to key frame (performance gain)
        ReturnTime = &H8            '	// Plug the media time equivalents back into the supplied LONGLONGs
        Segment = &H10              '	// At end just do EC_ENDOFSEGMENT, don't do EndOfStream
        NoFlush = &H20              '	// Don't flush
    End Enum

    <ComVisible(False)> _
    Public Enum PinDirection
        PINDIR_INPUT
        PINDIR_OUTPUT
    End Enum

    <ComVisible(True), ComImport(), _
    Guid("0000000f-0000-0000-C000-000000000046"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IMoniker

        ' Spezialversion

        <PreserveSig()> _
        Function GetClassID( _
            <Out()> ByRef pClassID As Guid _
        ) As Integer

        <PreserveSig()> _
        Function IsDirty( _
        ) As Integer

        <PreserveSig()> _
        Function Load( _
            <[In]()> ByVal pStm As ComTypes.IStream _
        ) As Integer

        <PreserveSig()> _
        Function Save( _
            <[In]()> ByVal pStm As ComTypes.IStream, _
            <[In](), MarshalAs(UnmanagedType.Bool)> ByVal fClearDirty As Boolean _
        ) As Integer

        <PreserveSig()> _
        Function GetSizeMax( _
            <Out()> ByRef pcbSize As Long _
        ) As Integer

        <PreserveSig()> _
        Function BindToObject( _
            <[In]()> ByVal pbc As ComTypes.IBindCtx, _
            <[In]()> ByVal pmkToLeft As IMoniker, _
            <[In]()> ByRef riidResult As Guid, _
            <Out()> ByRef ppvResult As IBaseFilter _
        ) As Integer

        <PreserveSig()> _
        Function BindToStorage( _
            <[In]()> ByVal pbc As ComTypes.IBindCtx, _
            <[In]()> ByVal pmkToLeft As IMoniker, _
            <[In]()> ByRef riidResult As Guid, _
            <Out()> ByRef ppvResult As IPropertyBag _
        ) As Integer

        <PreserveSig()> _
        Function Reduce( _
            <[In]()> ByVal pbc As ComTypes.IBindCtx, _
            <[In]()> ByVal dwReduceHowFar As Integer, _
            <[In](), Out()> ByRef ppmkToLeft As IMoniker, _
            <Out()> ByRef ppmkReduced As IMoniker _
        ) As Integer

        <PreserveSig()> _
        Function ComposeWith( _
            <[In]()> ByVal pmkRight As IMoniker, _
            <[In](), MarshalAs(UnmanagedType.Bool)> ByVal fOnlyIfNotGeneric As Boolean, _
            <Out()> ByRef ppmkComposite As IMoniker _
        ) As Integer

        <PreserveSig()> _
        Function EnumMoniker( _
            <[In](), MarshalAs(UnmanagedType.Bool)> ByVal fForward As Boolean, _
            <[Out]()> ByRef ppenumMoniker As IEnumMoniker _
        ) As Integer

        <PreserveSig()> _
        Function IsEqual( _
            <[In]()> ByVal pmkOtherMoniker As IMoniker _
        ) As Integer

        <PreserveSig()> _
        Function Hash( _
            <[Out]()> ByRef pdwHash As Integer _
        ) As Integer

        <PreserveSig()> _
        Function IsRunning( _
            <[In]()> ByVal pbc As ComTypes.IBindCtx, _
            <[In]()> ByVal pmkToLeft As IMoniker, _
            <[In]()> ByVal pmkNewlyRunning As IMoniker _
        ) As Integer

        <PreserveSig()> _
        Function GetTimeOfLastChange( _
            <[In]()> ByVal pbc As ComTypes.IBindCtx, _
            <[In]()> ByVal pmkToLeft As IMoniker, _
            <[Out]()> ByVal pFileTime As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function Inverse( _
            <[Out]()> ByRef ppmk As IMoniker _
        ) As Integer

        <PreserveSig()> _
        Function CommonPrefixWith( _
            <[In]()> ByVal pmkOther As IMoniker, _
            <[Out]()> ByRef ppmkPrefix As IMoniker _
        ) As Integer

        <PreserveSig()> _
        Function RelativePathTo( _
            <[In]()> ByVal pmkOther As IMoniker, _
            <[Out]()> ByRef ppmkRelPath As IMoniker _
        ) As Integer

        <PreserveSig()> _
        Function GetDisplayName( _
            <[In]()> ByVal pbc As ComTypes.IBindCtx, _
            <[In]()> ByVal pmkToLeft As IMoniker, _
            <[Out](), MarshalAs(UnmanagedType.BStr)> ByRef ppszDisplayName As String _
        ) As Integer

        <PreserveSig()> _
        Function ParseDisplayName( _
            <[In]()> ByVal pbc As ComTypes.IBindCtx, _
            <[In]()> ByVal pmkToLeft As IMoniker, _
            <[Out](), MarshalAs(UnmanagedType.BStr)> ByRef ppszDisplayName As String, _
            <[Out]()> ByVal pchEaten As Integer, _
            <[Out]()> ByRef ppmkOut As IMoniker _
        ) As Integer

        <PreserveSig()> _
        Function IsSystemMoniker( _
            <[Out]()> ByRef pdwMksys As Integer _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("00000102-0000-0000-C000-000000000046"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IEnumMoniker

        <PreserveSig()> _
        Function NextItem( _
            <[In]()> ByVal celt As Integer, _
            <Out()> ByRef IMoniker As IMoniker, _
            <Out()> ByRef pceltFetched As Integer _
        ) As Integer

        <PreserveSig()> _
        Function Skip( _
            ByVal celt As Integer _
        ) As Integer

        <PreserveSig()> _
        Function Reset( _
        ) As Integer

        <PreserveSig()> _
        Function Clone( _
            <Out()> ByRef ppenum As IEnumMoniker _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("55272A00-42CB-11CE-8135-00AA004BB851"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IPropertyBag

        <PreserveSig()> _
        Function Read( _
            <[In](), MarshalAs(UnmanagedType.BStr)> ByVal pszPropName As String, _
            <Out(), [In](), MarshalAs(UnmanagedType.LPArray)> ByVal pVar() As Byte, _
            <[In](), MarshalAs(UnmanagedType.IUnknown)> ByVal pErrLog As Object _
        ) As Integer

        <PreserveSig()> _
        Function Write( _
            <[In](), MarshalAs(UnmanagedType.BStr)> ByVal pszPropName As String, _
            <[In](), MarshalAs(UnmanagedType.AsAny)> ByVal pVar As VariantWrapper _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("29840822-5B84-11d0-BD3B-00A0C911CE86"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface ICreateDevEnum

        <PreserveSig()> _
        Function CreateClassEnumerator( _
            <[In](), MarshalAs(UnmanagedType.LPArray)> ByVal clsidDeviceClass() As Byte, _
            <Out()> ByRef ppEnum As DirectShow.IEnumMoniker, _
            <[In]()> ByVal dwFlags As Integer _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a86891-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IPin

        <PreserveSig()> _
        Function Connect( _
            <[In]()> ByVal pRecievePin As IPin, _
            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal ptm As AMMediaType _
        ) As Integer

        <PreserveSig()> _
        Function RecieveConnection( _
            <[In]()> ByVal pRecievePin As IPin, _
            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal ptm As AMMediaType _
        ) As Integer

        <PreserveSig()> _
        Function Disconnect( _
        ) As Integer

        <PreserveSig()> _
        Function ConnectedTo( _
            <Out()> ByRef ppPin As IPin _
        ) As Integer

        <PreserveSig()> _
        Function ConnectionMediaType( _
            <Out(), MarshalAs(UnmanagedType.LPStruct)> ByVal pmt As AMMediaType _
        ) As Integer

        <PreserveSig()> _
        Function QueryPinInfo( _
            ByVal pInfo As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function QueryDirection( _
            <Out()> ByRef pPinDir As PinDirection _
        ) As Integer

        <PreserveSig()> _
        Function QueryId( _
            <Out(), MarshalAs(UnmanagedType.LPWStr)> ByVal Id As String _
        ) As Integer

        <PreserveSig()> _
        Function QueryAccept( _
            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal ptm As AMMediaType _
        ) As Integer

        <PreserveSig()> _
        Function EnumMediaTypes( _
            ByRef ppEnum As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function QueryInternalConnections( _
            <Out()> ByRef apPin As IntPtr, _
            <[In](), Out()> ByRef nPin As Integer _
        ) As Integer

        <PreserveSig()> _
        Function EndOfStream( _
        ) As Integer

        <PreserveSig()> _
        Function BeginFlush( _
        ) As Integer

        <PreserveSig()> _
        Function EndFlush( _
        ) As Integer

        <PreserveSig()> _
        Function NewSegment( _
            ByVal tStart As Long, _
            ByVal tStop As Long, _
            ByVal dRate As Double _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a8689f-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IFilterGraph

        <PreserveSig()> _
        Function AddFilter( _
            <[In]()> ByVal pFilter As IBaseFilter, _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal pName As String _
        ) As Integer

        <PreserveSig()> _
        Function RemoveFilter( _
            <[In]()> ByVal pFilter As IBaseFilter _
        ) As Integer

        <PreserveSig()> _
        Function EnumFilters( _
            <Out()> ByRef ppEnum As IEnumFilters _
        ) As Integer

        <PreserveSig()> _
        Function FindFilterByName( _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal pName As String, _
            <Out()> ByRef ppFilter As IBaseFilter _
        ) As Integer

        <PreserveSig()> _
        Function ConnectDirect( _
            <[In]()> ByVal ppinOut As IPin, _
            <[In]()> ByVal ppinIn As IPin, _
            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal pmt As AMMediaType _
        ) As Integer

        <PreserveSig()> _
        Function Reconnect( _
            <[In]()> ByVal ppIn As IPin _
        ) As Integer

        <PreserveSig()> _
        Function Disconnect( _
            <[In]()> ByVal ppIn As IPin _
        ) As Integer

        <PreserveSig()> _
        Function SetDefaultSyncSource( _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("0000010c-0000-0000-C000-000000000046"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IPersist

        <PreserveSig()> _
        Function GetClassID( _
            <Out()> ByRef pClassID As Guid _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a86899-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IMediaFilter

        <PreserveSig()> _
        Function GetClassID( _
            <Out()> ByRef pClassID As Guid _
        ) As Integer

        <PreserveSig()> _
        Function StopFilter( _
        ) As Integer

        <PreserveSig()> _
        Function Pause( _
        ) As Integer

        <PreserveSig()> _
        Function Run( _
            ByVal tStart As Long _
        ) As Integer

        <PreserveSig()> _
        Function GetState( _
            ByVal dwMilliSecsTimeout As Integer, _
            <Out()> ByRef filtstate As Integer _
        ) As Integer

        <PreserveSig()> _
        Function SetSyncSource( _
            <[In]()> ByVal pClock As IReferenceClock _
        ) As Integer

        <PreserveSig()> _
        Function GetSyncSource( _
            <Out()> ByRef pClock As IReferenceClock _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a86895-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IBaseFilter

        <PreserveSig()> _
         Function GetClassID( _
            <Out()> ByRef pClassID As Guid _
        ) As Integer

        <PreserveSig()> _
        Function StopFilter( _
        ) As Integer

        <PreserveSig()> _
        Function Pause( _
        ) As Integer

        <PreserveSig()> _
        Function Run( _
            ByVal tStart As Long _
        ) As Integer

        <PreserveSig()> _
        Function GetState( _
            ByVal dwMilliSecsTimeout As Integer, _
            <Out()> ByRef filtstate As Integer _
        ) As Integer

        <PreserveSig()> _
        Function SetSyncSource( _
            <[In]()> ByVal pClock As IReferenceClock _
        ) As Integer

        <PreserveSig()> _
        Function GetSyncSource( _
            <Out()> ByRef pClock As IReferenceClock _
        ) As Integer

        <PreserveSig()> _
        Function EnumPins( _
            <Out()> ByRef ByRefppEnum As IEnumPins _
        ) As Integer

        <PreserveSig()> _
        Function FindPin( _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal Id As String, _
            <Out()> ByRef ppPin As IPin _
        ) As Integer

        <PreserveSig()> _
        Function QueryInterface( _
            <Out()> ByRef pInfo As FilterInfo _
        ) As Integer

        <PreserveSig()> _
        Function JoinFilterGraph( _
            <[In]()> ByVal pGraph As IFilterGraph, _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal pName As String _
        ) As Integer

        <PreserveSig()> _
        Function QueryVendorInfo( _
            <Out(), MarshalAs(UnmanagedType.LPWStr)> ByRef pVendorInfo As String _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("36b73880-c2c8-11cf-8b46-00805f6cef60"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IMediaSeeking

        <PreserveSig()> _
        Function GetCapabilities( _
            <Out()> ByRef pCapabilities As SeekingCapabilities _
        ) As Integer

        <PreserveSig()> _
        Function CheckCapabilities( _
            <[In](), Out()> ByRef pCapabilities As SeekingCapabilities _
        ) As Integer

        <PreserveSig()> _
        Function IsFormatSupported( _
            <[In]()> ByVal pFormat As Guid _
        ) As Integer

        <PreserveSig()> _
        Function QueryPreferredFormat( _
            <[Out]()> ByVal pFormat As Guid _
        ) As Integer

        <PreserveSig()> _
        Function GetTimeFormat( _
            <[Out]()> ByVal pFormat As Guid _
        ) As Integer

        <PreserveSig()> _
        Function IsUsingTimeFormat( _
            <[In]()> ByVal pFormat As Guid _
        ) As Integer

        <PreserveSig()> _
        Function SetTimeFormat( _
            <[In]()> ByVal pFormat As Guid _
        ) As Integer

        <PreserveSig()> _
        Function GetDuration( _
            <Out()> ByRef pDuration As Long _
        ) As Integer

        <PreserveSig()> _
        Function GetStopPosition( _
            <Out()> ByRef pStop As Long _
        ) As Integer

        <PreserveSig()> _
        Function GetCurrentPosition( _
            <Out()> ByRef pCurrent As Long _
        ) As Integer

        <PreserveSig()> _
        Function ConvertTimeFormat( _
            <Out()> ByRef pTarget As Long, _
            <[In]()> ByVal pTargetFormat As Guid, _
            ByVal Source As Long, _
            <[In]()> ByVal pSourceFormat As Guid _
        ) As Integer

        <PreserveSig()> _
        Function SetPositions( _
            <[In](), Out(), MarshalAs(UnmanagedType.LPStruct)> ByRef pCurrent As Long, _
            ByVal dwCurrentFlags As SeekingFlags, _
            <[In](), Out(), MarshalAs(UnmanagedType.LPStruct)> ByRef pStop As Long, _
            ByVal dwStopFlags As SeekingFlags _
        ) As Integer

        <PreserveSig()> _
        Function GetPositions( _
            <Out()> ByRef pCurrent As Long, _
            <Out()> ByRef pStop As Long _
        ) As Integer

        <PreserveSig()> _
        Function GetAvailable( _
            <Out()> ByRef pEarliest As Long, _
            <Out()> ByRef pLatest As Long _
        ) As Integer

        <PreserveSig()> _
        Function SetRate( _
            ByVal dRate As Double _
        ) As Integer

        <PreserveSig()> _
        Function GetRate( _
            <Out()> ByRef pdRate As Double _
        ) As Integer

        <PreserveSig()> _
        Function GetPreroll( _
            <Out()> ByRef pllPreroll As Long _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a86897-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IReferenceClock

        <PreserveSig()> _
        Function GetTime( _
            <Out()> ByRef pTime As Long _
        ) As Integer

        <PreserveSig()> _
        Function AdviseTime( _
            ByVal baseTime As Long, _
            ByVal streamTime As Long, _
            ByVal hEvent As IntPtr, _
            <Out()> ByRef pdwAdviseCookie As Integer _
        ) As Integer

        <PreserveSig()> _
        Function AdvisePeriodic( _
            ByVal startTime As Long, _
            ByVal periodTime As Long, _
            ByVal hSemaphore As IntPtr, _
            <Out()> ByRef pdwAdviseCookie As Integer _
        ) As Integer

        <PreserveSig()> _
        Function Unadvise( _
            ByVal dwAdviseCookie As Integer _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a86893-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IEnumFilters

        <PreserveSig()> _
        Function NextItem( _
            <[In]()> ByVal cFilters As Integer, _
            <Out(), MarshalAs(UnmanagedType.LPArray, SizeParamIndex:=2)> ByVal ppFilter() As IBaseFilter, _
            <Out()> ByRef pcFetched As Integer _
        ) As Integer

        <PreserveSig()> _
        Function Skip( _
            <[In]()> ByVal cFilters As Integer _
        ) As Integer

        <PreserveSig()> _
        Function Reset( _
        ) As Integer

        <PreserveSig()> _
        Function Clone( _
            <[Out]()> ByRef ppEnum As IEnumFilters _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a86892-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IEnumPins

        <PreserveSig()> _
        Function NextItem( _
            <[In]()> ByVal cPins As Integer, _
            <Out(), MarshalAs(UnmanagedType.LPArray, SizeParamIndex:=2)> ByVal ppPins() As IPin, _
            <Out()> ByRef pcFetched As Integer _
        ) As Integer

        <PreserveSig()> _
        Function Skip( _
            <[In]()> ByVal cPins As Integer _
        ) As Integer

        <PreserveSig()> _
        Function Reset( _
        ) As Integer

        <PreserveSig()> _
        Function Clone( _
            <Out()> ByRef ppEnum As IEnumPins _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a8689a-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IMediaSample

        <PreserveSig()> _
        Function GetPointer( _
            <Out()> ByRef ppBuffer As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function GetSize( _
        ) As Integer

        <PreserveSig()> _
        Function GetTime( _
            <Out()> ByRef pTimeStart As Long, _
            <Out()> ByRef pTimeEnd As Long _
        ) As Integer

        <PreserveSig()> _
        Function SetTime( _
            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal pTimeStart As Long, _
            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal pTimeEnd As Long _
        ) As Integer

        <PreserveSig()> _
        Function IsSyncPoint( _
        ) As Integer

        <PreserveSig()> _
        Function SetSyncPoint( _
            <[In](), MarshalAs(UnmanagedType.Bool)> ByVal bIsSyncPoint As Boolean _
        ) As Integer

        <PreserveSig()> _
        Function IsPreroll( _
        ) As Integer

        <PreserveSig()> _
        Function SetPreroll( _
            <[In](), MarshalAs(UnmanagedType.Bool)> ByVal bIsPreroll As Boolean _
        ) As Integer

        <PreserveSig()> _
        Function GetActualDataLength( _
        ) As Integer

        <PreserveSig()> _
        Function SetActualDataLength( _
            ByVal len As Integer _
        ) As Integer

        <PreserveSig()> _
        Function GetMediaType( _
            <Out(), MarshalAs(UnmanagedType.LPStruct)> ByRef ppMediaType As AMMediaType _
        ) As Integer

        <PreserveSig()> _
        Function SetMediaType( _
            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByRef pMediaType As AMMediaType _
        ) As Integer

        <PreserveSig()> _
        Function IsDiscontinuity( _
        ) As Integer

        <PreserveSig()> _
         Function SetDiscontinuity( _
            <[In](), MarshalAs(UnmanagedType.Bool)> ByVal bDiscontinuity As Boolean _
        ) As Integer

        <PreserveSig()> _
        Function GetMediaTime( _
            <Out()> ByRef pTimeStart As Long, _
            <Out()> ByRef pTimeEnd As Long _
        ) As Integer

        <PreserveSig()> _
        Function SetMediaTime( _
            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal pTimeStart As Long, _
            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal pTimeEnd As Long _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a868b1-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsDual)> _
    Public Interface IMediaControl

        <PreserveSig()> _
        Function Run( _
        ) As Integer

        <PreserveSig()> _
        Function Pause( _
        ) As Integer

        <PreserveSig()> _
        Function StopMedia( _
        ) As Integer

        <PreserveSig()> _
        Function GetState( _
            ByVal msTimeout As Integer, _
            <Out()> ByRef pfs As Integer _
        ) As Integer

        <PreserveSig()> _
        Function RenderFile( _
            ByVal strFilename As String _
        ) As Integer

        <PreserveSig()> _
        Function AddSourceFilter( _
            <[In]()> ByVal strFilename As String, _
            <Out(), MarshalAs(UnmanagedType.IDispatch)> ByRef ppUnk As Object _
        ) As Integer

        <PreserveSig()> _
        Function get_FilterCollection( _
            <Out(), MarshalAs(UnmanagedType.IDispatch)> ByRef ppUnk As Object _
        ) As Integer

        <PreserveSig()> _
        Function get_RegFilterCollection( _
            <Out(), MarshalAs(UnmanagedType.IDispatch)> ByRef ppUnk As Object _
        ) As Integer

        <PreserveSig()> _
        Function StopWhenReady( _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a868b6-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsDual)> _
    Public Interface IMediaEvent

        <PreserveSig()> _
        Function GetEventHandle( _
            <Out()> ByVal hEvent As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function GetEvent( _
            ByRef lEventCode As EventCode, _
            ByRef lParam1 As Integer, _
            ByRef lParam2 As Integer, _
            ByVal msTimeout As Integer _
        ) As Integer

        <PreserveSig()> _
        Function WaitForCompletion( _
            ByVal msTimeout As Integer, _
            ByRef pEvCode As Integer _
        ) As Integer

        <PreserveSig()> _
        Function CancelDefaultHandling( _
            ByVal lEvCode As Integer _
        ) As Integer

        <PreserveSig()> _
        Function RestoreDefaultHandling( _
            ByVal lEvCode As Integer _
        ) As Integer

        <PreserveSig()> _
        Function FreeEventParams( _
            ByVal lEvCode As EventCode, _
            ByVal lParam1 As Integer, _
            ByVal lParam2 As Integer _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a868c0-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsDual)> _
    Public Interface IMediaEventEx

        <PreserveSig()> _
        Function GetEventHandle( _
            <Out()> ByVal hEvent As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function GetEvent( _
            <Out()> ByRef lEventCode As EventCode, _
            <Out()> ByRef lParam1 As Integer, _
            <Out()> ByRef lParam2 As Integer, _
            ByVal msTimeout As Integer _
        ) As Integer

        <PreserveSig()> _
        Function WaitForCompletion( _
            ByVal msTimeout As Integer, _
            <Out()> ByRef pEvCode As Integer _
        ) As Integer

        <PreserveSig()> _
        Function CancelDefaultHandling( _
            ByVal lEvCode As Integer _
        ) As Integer

        <PreserveSig()> _
        Function RestoreDefaultHandling( _
            ByVal lEvCode As Integer _
        ) As Integer

        <PreserveSig()> _
        Function FreeEventParams( _
            ByVal lEvCode As EventCode, _
            ByVal lParam1 As Integer, _
            ByVal lParam2 As Integer _
        ) As Integer

        <PreserveSig()> _
        Function SetNotifyWindow( _
            ByVal hwnd As IntPtr, _
            ByVal lMsg As Integer, _
            ByVal lInstanceData As Integer _
        ) As Integer

        <PreserveSig()> _
        Function SetNotifyFlags( _
            ByVal lNoNotifyFlags As Integer _
        ) As Integer

        <PreserveSig()> _
        Function GetNotifyFlags( _
            <Out()> ByRef lplNoNotifyFlags As Integer _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("329bb360-f6ea-11d1-9038-00a0c9697298"), _
    InterfaceType(ComInterfaceType.InterfaceIsDual)> _
    Public Interface IBasicVideo2

        <PreserveSig()> _
        Function AvgTimePerFrame( _
            <Out()> ByRef pAvgTimePerFrame As Double _
        ) As Integer

        <PreserveSig()> _
        Function BitRate( _
            <Out()> ByRef pBitRate As Integer _
        ) As Integer

        <PreserveSig()> _
        Function BitErrorRate( _
            <Out()> ByRef pBitRate As Integer _
        ) As Integer

        <PreserveSig()> _
        Function VideoWidth( _
            <Out()> ByRef pVideoWidth As Integer _
        ) As Integer

        <PreserveSig()> _
        Function VideoHeight( _
            <Out()> ByRef pVideoHeight As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_SourceLeft( _
            ByVal SourceLeft As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_SourceLeft( _
            <Out()> ByRef pSourceLeft As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_SourceWidth( _
            ByVal SourceWidth As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_SourceWidth( _
            <Out()> ByRef pSourceWidth As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_SourceTop( _
            ByVal SourceTop As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_SourceTop( _
            <Out()> ByRef pSourceTop As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_SourceHeight( _
            ByVal SourceHeight As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_SourceHeight( _
            <Out()> ByRef pSourceHeight As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_DestinationLeft( _
            ByVal DestinationLeft As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_DestinationLeft( _
            <Out()> ByRef pDestinationLeft As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_DestinationWidth( _
            ByVal DestinationWidth As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_DestinationWidth( _
            <Out()> ByRef pDestinationWidth As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_DestinationTop( _
            ByVal DestinationTop As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_DestinationTop( _
            <Out()> ByRef pDestinationTop As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_DestinationHeight( _
            ByVal DestinationHeight As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_DestinationHeight( _
            <Out()> ByRef pDestinationHeight As Integer _
        ) As Integer

        <PreserveSig()> _
        Function SetSourcePosition( _
            ByVal left As Integer, _
            ByVal top As Integer, _
            ByVal width As Integer, _
            ByVal height As Integer _
        ) As Integer

        <PreserveSig()> _
        Function GetSourcePosition( _
            <Out()> ByRef left As Integer, _
            <Out()> ByRef top As Integer, _
            <Out()> ByRef width As Integer, _
            <Out()> ByRef height As Integer _
        ) As Integer

        <PreserveSig()> _
        Function SetDefaultSourcePosition( _
        ) As Integer

        <PreserveSig()> _
        Function SetDestinationPosition( _
            ByVal left As Integer, _
            ByVal top As Integer, _
            ByVal width As Integer, _
            ByVal height As Integer _
        ) As Integer

        <PreserveSig()> _
        Function GetDestinationPosition( _
            <Out()> ByRef left As Integer, _
            <Out()> ByRef top As Integer, _
            <Out()> ByRef width As Integer, _
            <Out()> ByRef height As Integer _
        ) As Integer

        <PreserveSig()> _
        Function SetDefaultDestinationPosition( _
        ) As Integer

        <PreserveSig()> _
        Function GetVideoSize( _
            <Out()> ByRef pWidth As Integer, _
            <Out()> ByRef pHeight As Integer _
        ) As Integer

        <PreserveSig()> _
        Function GetVideoPaletteEntries( _
            ByVal StartIndex As Integer, _
            ByVal Entries As Integer, _
            <Out()> ByRef pRetrieved As Integer, _
            ByVal pPalette As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function GetCurrentImage( _
            ByVal pBufferSize As Integer, _
            ByVal pDIBImage As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function IsUsingDefaultSource( _
        ) As Integer

        <PreserveSig()> _
        Function IsUsingDefaultDestination( _
        ) As Integer

        <PreserveSig()> _
        Function GetPreferredAspectRatio( _
            <Out()> ByRef plAspectX As Integer, _
            <Out()> ByRef plAspectY As Integer _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a868b4-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsDual)> _
    Public Interface IVideoWindow

        <PreserveSig()> _
        Function put_Caption( _
            ByVal caption As String _
        ) As Integer

        <PreserveSig()> _
        Function get_Caption( _
            <Out()> ByRef caption As String _
        ) As Integer

        <PreserveSig()> _
        Function put_WindowStyle( _
            ByVal windowStyle As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_WindowStyle( _
            <Out()> ByRef windowStyle As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_WindowStyleEx( _
            ByVal windowStyleEx As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_WindowStyleEx( _
            <Out()> ByRef windowStyleEx As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_AutoShow( _
            ByVal autoShow As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_AutoShow( _
            <Out()> ByRef autoShow As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_WindowState( _
            ByVal windowState As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_WindowState( _
            <Out()> ByRef windowState As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_BackgroundPalette( _
            ByVal backgroundPalette As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_BackgroundPalette( _
            <Out()> ByRef backgroundPalette As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_Visible( _
            ByVal visible As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_Visible( _
            <Out()> ByRef visible As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_Left( _
            ByVal left As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_Left( _
            <Out()> ByRef left As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_Width( _
            ByVal width As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_Width( _
            <Out()> ByRef width As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_Top( _
            ByVal top As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_Top( _
            <Out()> ByRef top As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_Height( _
            ByVal height As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_Height( _
            <Out()> ByRef height As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_Owner( _
            ByVal owner As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function get_Owner( _
            <Out()> ByRef owner As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function put_MessageDrain( _
            ByVal drain As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function get_MessageDrain( _
            <Out()> ByRef drain As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function get_BorderColor( _
            <Out()> ByRef color As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_BorderColor( _
            ByVal color As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_FullScreenMode( _
            <Out()> ByRef fullScreenMode As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_FullScreenMode( _
            ByVal fullScreenMode As Integer _
        ) As Integer

        <PreserveSig()> _
        Function SetWindowForeground( _
            ByVal focus As Integer _
        ) As Integer

        <PreserveSig()> _
        Function NotifyOwnerMessage( _
            ByVal hwnd As IntPtr, _
            ByVal msg As Integer, _
            ByVal wParam As IntPtr, _
            ByVal lParam As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function SetWindowPosition( _
            ByVal left As Integer, _
            ByVal top As Integer, _
            ByVal width As Integer, _
            ByVal height As Integer _
        ) As Integer

        <PreserveSig()> _
        Function GetWindowPosition( _
            <Out()> ByRef left As Integer, _
            <Out()> ByRef top As Integer, _
            <Out()> ByRef width As Integer, _
            <Out()> ByRef height As Integer _
        ) As Integer

        <PreserveSig()> _
        Function GetMinIdealImageSize( _
            <Out()> ByRef width As Integer, _
            <Out()> ByRef height As Integer _
        ) As Integer

        <PreserveSig()> _
        Function GetMaxIdealImageSize( _
            <Out()> ByRef width As Integer, _
            <Out()> ByRef height As Integer _
        ) As Integer

        <PreserveSig()> _
        Function GetRestorePosition( _
            <Out()> ByRef left As Integer, _
            <Out()> ByRef top As Integer, _
            <Out()> ByRef width As Integer, _
            <Out()> ByRef height As Integer _
        ) As Integer

        <PreserveSig()> _
        Function HideCursor( _
            ByVal ihideCursor As Integer _
        ) As Integer

        <PreserveSig()> _
        Function IsCursorHidden( _
            <Out()> ByRef hideCursor As Integer _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a868b2-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsDual)> _
    Public Interface IMediaPosition

        <PreserveSig()> _
        Function get_Duration( _
            <Out()> ByRef pLength As Double _
        ) As Integer

        <PreserveSig()> _
        Function put_CurrentPosition( _
            ByVal llTime As Double _
        ) As Integer

        <PreserveSig()> _
        Function get_CurrentPosition( _
            <Out()> ByRef pllTime As Double _
        ) As Integer

        <PreserveSig()> _
        Function get_StopTime( _
            <Out()> ByRef pllTime As Double _
        ) As Integer

        <PreserveSig()> _
        Function put_StopTime( _
            ByVal llTime As Double _
        ) As Integer

        <PreserveSig()> _
        Function get_PrerollTime( _
            <Out()> ByRef pllTime As Double _
        ) As Integer

        <PreserveSig()> _
        Function put_PrerollTime( _
            ByVal llTime As Double _
        ) As Integer

        <PreserveSig()> _
        Function put_Rate( _
            ByVal dRate As Double _
        ) As Integer

        <PreserveSig()> _
        Function get_Rate( _
            <Out()> ByRef pdRate As Double _
        ) As Integer

        <PreserveSig()> _
        Function CanSeekForward( _
            <Out()> ByRef pCanSeekForward As Integer _
        ) As Integer

        <PreserveSig()> _
        Function CanSeekBackward( _
            <Out()> ByRef pCanSeekBackward As Integer _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a868b3-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsDual)> _
    Public Interface IBasicAudio

        <PreserveSig()> _
        Function put_Volume( _
            ByVal lVolume As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_Volume( _
            <Out()> ByRef plVolume As Integer _
        ) As Integer

        <PreserveSig()> _
        Function put_Balance( _
            ByVal lBalance As Integer _
        ) As Integer

        <PreserveSig()> _
        Function get_Balance( _
            <Out()> ByRef plBalance As Integer _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a868b9-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsDual)> _
    Public Interface IAMCollection

        <PreserveSig()> _
        Function get_Count( _
            <Out()> ByRef plCount As Integer _
        ) As Integer

        <PreserveSig()> _
        Function Item( _
            ByVal lItem As Integer, _
            <Out(), MarshalAs(UnmanagedType.IUnknown)> ByRef ppUnk As Object _
        ) As Integer

        <PreserveSig()> _
        Function get_NewEnum( _
            <Out(), MarshalAs(UnmanagedType.IUnknown)> ByRef ppUnk As Object _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("6B652FFF-11FE-4fce-92AD-0266B5D7C78F"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface ISampleGrabber

        <PreserveSig()> _
        Function SetOneShot( _
            <[In](), MarshalAs(UnmanagedType.Bool)> ByVal OneShot As Boolean _
        ) As Integer

        <PreserveSig()> _
        Function SetMediaType( _
            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal pmt As AMMediaType _
        ) As Integer

        <PreserveSig()> _
        Function GetConnectedMediaType( _
            <Out(), MarshalAs(UnmanagedType.LPStruct)> ByVal pmt As AMMediaType _
        ) As Integer

        <PreserveSig()> _
        Function SetBufferSamples( _
            <[In](), MarshalAs(UnmanagedType.Bool)> ByVal BufferThem As Boolean _
        ) As Integer

        <PreserveSig()> _
        Function GetCurrentBuffer( _
            ByVal pBufferSize As Integer, _
            ByVal pBuffer As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function GetCurrentSample( _
            ByVal ppSample As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function SetCallback( _
            ByVal pCallback As ISampleGrabberCB, _
            ByVal WhichMethodToCallback As Integer _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("0579154A-2B53-4994-B0D0-E773148EFF85"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface ISampleGrabberCB

        <PreserveSig()> _
        Function SampleCB( _
            ByVal SampleTime As Double, _
            ByVal pSample As IMediaSample _
        ) As Integer

        <PreserveSig()> _
        Function BufferCB( _
            ByVal SampleTime As Double, _
            ByVal pBuffer As IntPtr, _
            ByVal BufferLen As Integer _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("93E5A4E0-2D50-11d2-ABFA-00A0C9C6E38D"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface ICaptureGraphBuilder2

        <PreserveSig()> _
        Function SetFiltergraph( _
            <[In]()> ByVal pfg As IGraphBuilder _
        ) As Integer

        <PreserveSig()> _
        Function GetFiltergraph( _
            <Out()> ByRef ppfg As IGraphBuilder _
        ) As Integer

        <PreserveSig()> _
        Function SetOutputFileName( _
            <[In]()> ByVal pType As Guid, _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal lpstrFile As String, _
            <Out()> ByRef ppbf As IBaseFilter, _
            <Out()> ByRef ppSink As IFileSinkFilter _
        ) As Integer

        <PreserveSig()> _
        Function FindInterface( _
            <[In]()> ByVal pCategory As Guid, _
            <[In]()> ByVal pType As Guid, _
            <[In]()> ByVal pbf As IBaseFilter, _
            <[In]()> ByVal riid As Guid, _
            <Out(), MarshalAs(UnmanagedType.IUnknown)> ByRef ppint As Object _
        ) As Integer

        <PreserveSig()> _
        Function RenderStream( _
            <[In]()> ByVal pCategory As Guid, _
            <[In]()> ByVal pType As Guid, _
            <[In](), MarshalAs(UnmanagedType.IUnknown)> ByVal pSource As Object, _
            <[In]()> ByVal pfCompressor As IBaseFilter, _
            <[In]()> ByVal pfRenderer As IBaseFilter _
        ) As Integer

        <PreserveSig()> _
        Function ControlStream( _
            <[In]()> ByVal pCategory As Guid, _
            <[In]()> ByVal pType As Guid, _
            <[In]()> ByVal pFilter As IBaseFilter, _
            <[In]()> ByVal pstart As IntPtr, _
            <[In]()> ByVal pstop As IntPtr, _
            <[In]()> ByVal wStartCookie As Short, _
            <[In]()> ByVal wStopCookie As Short _
        ) As Integer

        <PreserveSig()> _
        Function AllocCapFile( _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal lpstrFile As String, _
            <[In]()> ByVal dwlSize As Long _
        ) As Integer

        <PreserveSig()> _
        Function CopyCaptureFile( _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal lpwstrOld As String, _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal lpwstrNew As String, _
            <[In]()> ByVal fAllowEscAbort As Integer, _
            <[In]()> ByVal pFilter As IAMCopyCaptureFileProgress _
        ) As Integer

        <PreserveSig()> _
        Function FindPin( _
            <[In]()> ByVal pSource As Object, _
            <[In]()> ByVal pindir As Integer, _
            <[In]()> ByVal pCategory As Guid, _
            <[In]()> ByVal pType As Guid, _
            <[In](), MarshalAs(UnmanagedType.Bool)> ByVal fUnconnected As Boolean, _
            <[In]()> ByVal num As Integer, _
            <Out()> ByRef ppPin As IPin _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("56a868a9-0ad4-11ce-b03a-0020af0ba770"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IGraphBuilder

        <PreserveSig()> _
        Function AddFilter( _
            <[In]()> ByVal pFilter As IBaseFilter, _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal pName As String _
        ) As Integer

        <PreserveSig()> _
        Function RemoveFilter( _
            <[In]()> ByVal pFilter As IBaseFilter _
        ) As Integer

        <PreserveSig()> _
        Function EnumFilters( _
            <Out()> ByRef ppEnum As IEnumFilters _
        ) As Integer

        <PreserveSig()> _
        Function FindFilterByName( _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal pName As String, _
            <Out()> ByRef ppFilter As IBaseFilter _
        ) As Integer

        <PreserveSig()> _
        Function ConnectDirect( _
            <[In]()> ByVal ppinOut As IPin, _
            <[In]()> ByVal ppinIn As IPin, _
            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal pmt As AMMediaType _
        ) As Integer

        <PreserveSig()> _
        Function Reconnect( _
            <[In]()> ByVal ppIn As IPin _
        ) As Integer

        <PreserveSig()> _
        Function Disconnect( _
            <[In]()> ByVal ppIn As IPin _
        ) As Integer

        <PreserveSig()> _
        Function SetDefaultSyncSource( _
        ) As Integer

        <PreserveSig()> _
        Function Connect( _
            <[In]()> ByVal ppinOut As IPin, _
            <[In]()> ByVal ppinIn As IPin _
        ) As Integer

        <PreserveSig()> _
        Function Render( _
            <[In]()> ByVal ppinOut As IPin _
        ) As Integer

        <PreserveSig()> _
        Function RenderFile( _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal lpcwstrFile As String, _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal lpcwstrPlayList As String _
        ) As Integer

        <PreserveSig()> _
        Function AddSourceFilter( _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal lpcwstrFileName As String, _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal lpcwstrFilterName As String, _
            <Out()> ByRef ppFilter As IBaseFilter _
        ) As Integer

        <PreserveSig()> _
        Function SetLogFile( _
            ByVal hFile As IntPtr _
        ) As Integer

        <PreserveSig()> _
        Function Abort( _
        ) As Integer

        <PreserveSig()> _
        Function ShouldOperationContinue( _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("a2104830-7c70-11cf-8bce-00aa00a3f1a6"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IFileSinkFilter

        <PreserveSig()> _
        Function SetFileName( _
            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal pszFileName As String, _
            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal pmt As AMMediaType _
        ) As Integer

        <PreserveSig()> _
        Function GetCurFile( _
            <Out(), MarshalAs(UnmanagedType.LPWStr)> ByRef pszFileName As String, _
            <Out(), MarshalAs(UnmanagedType.LPStruct)> ByVal pmt As AMMediaType _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("670d1d20-a068-11d0-b3f0-00aa003761c5"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IAMCopyCaptureFileProgress

        <PreserveSig()> _
        Function Progress( _
            ByVal iProgress As Integer _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("e46a9787-2b71-444d-a4b5-1fab7b708d6a"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IVideoFrameStep

        <PreserveSig()> _
        Function StepF( _
            ByVal dwFrames As Integer, _
            <[In](), MarshalAs(UnmanagedType.IUnknown)> ByVal pStepObject As Object _
        ) As Integer

        <PreserveSig()> _
        Function CanStep( _
            ByVal bMultiple As Integer, _
            <[In](), MarshalAs(UnmanagedType.IUnknown)> ByVal pStepObject As Object _
        ) As Integer

        <PreserveSig()> _
        Function CancelStep( _
        ) As Integer

    End Interface

    <ComVisible(True), ComImport(), _
    Guid("C6E13340-30AC-11d0-A18C-00A0C9118956"), _
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Public Interface IAMStreamConfig

        <PreserveSig()> _
        Function SetFormat( _
            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal pmt As AMMediaType _
        ) As Integer

        <PreserveSig()> _
        Function GetFormat( _
            <Out(), MarshalAs(UnmanagedType.LPStruct)> ByVal pmt As AMMediaType _
        ) As Integer

        <PreserveSig()> _
        Function GetNumberOfCapabilities( _
            <Out()> ByRef piCount As Integer, _
            <Out()> ByRef piSize As Integer _
        ) As Integer

        <PreserveSig()> _
        Function GetStreamCaps( _
            ByVal iIndex As Integer, _
            <Out(), MarshalAs(UnmanagedType.LPStruct)> ByVal ppmt As AMMediaType, _
            ByVal pSCC As IntPtr _
        ) As Integer

    End Interface
End Namespace
