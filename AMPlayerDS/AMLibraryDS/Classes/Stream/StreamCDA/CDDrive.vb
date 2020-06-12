Imports System.Runtime.InteropServices
Imports Microsoft.Win32.SafeHandles

' CDDrive class is used to access CD Rom by DeviceIoControl
' Nb. This class works only from Windows 2000 and not work correctly
' in previous versions of windows (es. Win98)

' Many thanks at thoose articles on www.codeproject.com
' This class is a union of those articles
' https://www.codeproject.com/Articles/15725/Tutorial-on-reading-Audio-CDs
' https://www.codeproject.com/Articles/5458/C-Sharp-Ripper

' So to thanks authors support them rating articles 5 stars

' Tested with DirectSound Output and WaveOut Output

' TODO: Add MetaBrainz CD Database function or FreeDB
Public Class CDDrive
    Implements IDisposable

#Region "SafeCDHandle"
    Public Class SafeCDHandle
        Inherits SafeHandleZeroOrMinusOneIsInvalid

        Public Sub New()
            MyBase.New(True)
        End Sub

        Protected Overrides Function ReleaseHandle() As Boolean
#If DEBUG Then
            DebugPrintLine("SafeCDHandle", "Handle correctly Closed")
#End If
            Return CloseHandle(handle) = 0
        End Function
    End Class

#End Region

#Region "Native Imports"


    ' DesiredAccess values
    Private Const GENERIC_READ As Int32 = &H80000000
    Private Const GENERIC_WRITE As Int32 = &H40000000
    Private Const GENERIC_EXECUTE As Int32 = &H20000000
    Private Const GENERIC_ALL As Int32 = &H10000000

    ' Share constants
    Private Const FILE_SHARE_READ As Int32 = &H1
    Private Const FILE_SHARE_WRITE As Int32 = &H2
    Private Const FILE_SHARE_DELETE As Int32 = &H4

    ' CreationDisposition constants
    Private Const CREATE_NEW As Int32 = 1
    Private Const CREATE_ALWAYS As Int32 = 2
    Private Const OPEN_EXISTING As Int32 = 3
    Private Const OPEN_ALWAYS As Int32 = 4
    Private Const TRUNCATE_EXISTING As Int32 = 5

    ' DeviceIoControl constants
    Private Const IOCTL_CDROM_READ_TOC As Int32 = &H24000
    Private Const IOCTL_STORAGE_CHECK_VERIFY As Int32 = &H2D4800
    Private Const IOCTL_CDROM_RAW_READ As Int32 = &H2403E
    Private Const IOCTL_STORAGE_MEDIA_REMOVAL As Int32 = &H2D4804
    Private Const IOCTL_STORAGE_EJECT_MEDIA As Int32 = &H2D4808
    Private Const IOCTL_STORAGE_LOAD_MEDIA As Int32 = &H2D480C

    ' CD ROM constants
    Private Const CDROM_SECTOR_SIZE As Integer = 2352
    Private Const CDROM_DATA_TO_SEC As Integer = 2048
    Private Const ENHANCED_CD_OFFSET As Integer = 11400

    Private Const MAXIMUM_NUMBER_TRACKS As Integer = 100

    ''' <summary>
    ''' List of drives returned by <see cref="GetDriveType"/>
    ''' function
    ''' </summary>
    Public Enum DriveTypes As Integer
        DRIVE_UNKNOWN = 0
        DRIVE_NO_ROOT_DIR
        DRIVE_REMOVABLE
        DRIVE_FIXED
        DRIVE_REMOTE
        DRIVE_CDROM
        DRIVE_RAMDISK
    End Enum

    ''' <summary>
    ''' Determines whether a disk drive is a removable, fixed, CD-ROM, RAM disk, or network drive.
    ''' </summary>
    ''' <param name="drive">The root directory for the drive.</param>
    ''' <returns>Return the drive type</returns>
    <DllImport("kernel32.dll")>
    Public Shared Function GetDriveType(ByVal drive As String) As DriveTypes
    End Function

    ''' <summary>
    ''' Win32 CreateFile function, look for complete information at Platform SDK
    ''' </summary>
    ''' <param name="FileName">In order to read CD data FileName must be "\\.\\D:" where D Is the CDROM drive letter</param>
    ''' <param name="DesiredAccess">Must be GENERIC_READ for CDROMs others access flags are Not important in this case</param>
    ''' <param name="ShareMode">O means exlusive access, FILE_SHARE_READ allow open the CDROM</param>
    ''' <param name="lpSecurityAttributes">See Platform SDK documentation for details. NULL pointer could be enough</param>
    ''' <param name="CreationDisposition">Must be OPEN_EXISTING for CDROM drives</param>
    ''' <param name="dwFlagsAndAttributes">0 in fine for this case</param>
    ''' <param name="hTemplateFile">NULL handle in this case</param>
    ''' <returns>INVALID_HANDLE_VALUE on error Or the handle to file if success</returns>
    <DllImport("Kernel32.dll", EntryPoint:="CreateFileA", CharSet:=CharSet.Ansi)>
    Public Shared Function CreateFile(<MarshalAs(UnmanagedType.LPStr)> ByVal FileName As String,
                                      ByVal DesiredAccess As Int32,
                                      ByVal ShareMode As Int32,
                                      ByVal lpSecurityAttributes As IntPtr,
                                      ByVal CreationDisposition As Int32,
                                      ByVal dwFlagsAndAttributes As Int32,
                                      ByVal hTemplateFile As IntPtr) As SafeCDHandle

    End Function

    ''' <summary>
    ''' The CloseHandle function closes an open object handle.
    ''' </summary>
    ''' <param name="hObject">Handle to an open object.</param>
    ''' <returns>If the function succeeds, the return value Is nonzero. If the function fails, the return value Is zero. To get extended error information, call GetLastError.</returns>
    <DllImport("kernel32.dll", SetLastError:=True)>
    Public Shared Function CloseHandle(ByVal hObject As IntPtr) As Integer
    End Function


    ''' <summary>
    ''' Most general form of DeviceIoControl Win32 function
    ''' </summary>
    ''' <param name="hDevice">Handle of device opened with CreateFile</param>
    ''' <param name="IoControlCode">Code of DeviceIoControl operation</param>
    ''' <param name="lpInBuffer">Pointer to a buffer that contains the data required to perform the operation.</param>
    ''' <param name="InBufferSize">Size of the buffer pointed to by lpInBuffer, in bytes.</param>
    ''' <param name="lpOutBuffer">Pointer to a buffer that receives the operation's output data.</param>
    ''' <param name="nOutBufferSize">Size of the buffer pointed to by lpOutBuffer, in bytes.</param>
    ''' <param name="lpBytesReturned">Receives the size, in bytes, of the data stored into the buffer pointed to by lpOutBuffer. </param>
    ''' <param name="lpOverlapped">Pointer to an OVERLAPPED structure. Discarded for this case</param>
    ''' <returns>If the function succeeds, the return value Is nonzero. If the function fails, the return value Is zero.</returns>
    <DllImport("kernel32.dll", SetLastError:=True)>
    Public Shared Function DeviceIoControl(ByVal hDevice As SafeCDHandle,
                                    ByVal IoControlCode As Integer,
                                    ByVal lpInBuffer As IntPtr,
                                    ByVal InBufferSize As Integer,
                                    ByVal lpOutBuffer As IntPtr,
                                    ByVal nOutBufferSize As Integer,
                                    ByRef lpBytesReturned As Integer,
                                    ByVal lpOverlapped As IntPtr) As Integer

    End Function

    ''' <summary>
    ''' The TRACK_DATA structure is used in conjunction with CDROM_TOC
    ''' See https://docs.microsoft.com/it-ch/windows-hardware/drivers/ddi/ntddcdrm/ns-ntddcdrm-_track_data
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)>
    Public Structure TRACK_DATA
        Public Reserved As Byte
        Private BitMapped As Byte ' -> Attention: private field!
        Public Property Control As Byte
            Get
                Return CByte(BitMapped And CByte(&HF))
            End Get
            Set(value As Byte)
                BitMapped = CByte((BitMapped And CByte(&HF0)) Or (value And CByte(&HF)))
            End Set
        End Property

        Public Property Adr As Byte
            Get
                Return CByte((BitMapped And CByte(&HF)) >> 4)
            End Get
            Set(value As Byte)
                BitMapped = CByte((BitMapped And CByte(&HF0)) Or (value << 4))
            End Set
        End Property
        Public TrackNumber As Byte
        Public Reserved1 As Byte

        ''' <summary>
        ''' Don't use array to avoid array creation
        ''' </summary>
        Public Address_0 As Byte
        Public Address_1 As Byte
        Public Address_2 As Byte
        Public Address_3 As Byte
    End Structure

    ''' <summary>
    ''' Used to parse array of track data in CDROM_TOC
    ''' ported from Idael Cardoso article on codeproject
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)>
    Public Class TrackDataList
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=MAXIMUM_NUMBER_TRACKS * 8)>
        Private Data() As Byte

        Public ReadOnly Property Item(ByVal Index As Integer) As TRACK_DATA
            Get

                If (Index < 0) Or (Index >= MAXIMUM_NUMBER_TRACKS) Then
                    Throw New IndexOutOfRangeException()
                End If

                Dim res As TRACK_DATA
                Dim handle As GCHandle = GCHandle.Alloc(Data, GCHandleType.Pinned)

                Try
                    Dim buffer As IntPtr = handle.AddrOfPinnedObject()
                    buffer = CType((buffer + (Index * Marshal.SizeOf(GetType(TRACK_DATA)))), IntPtr)
                    res = CType(Marshal.PtrToStructure(buffer, GetType(TRACK_DATA)), TRACK_DATA)
                Finally
                    handle.Free()
                End Try

                Return res
            End Get
        End Property

        Public Sub New()
            Data = New Byte(MAXIMUM_NUMBER_TRACKS * Marshal.SizeOf(GetType(TRACK_DATA)) - 1) {}
            Array.Clear(Data, 0, Data.Length)
        End Sub
    End Class

    ''' <summary>
    ''' Device control IRPs with a control code of IOCTL_CDROM_READ_TOC_EX 
    ''' and a format of CDROM_READ_TOC_EX_FORMAT_TOC 
    ''' return their output data in this structure followed by a 
    ''' series of TRACK_DATA structures.
    ''' 
    ''' See https://docs.microsoft.com/en-us/windows-hardware/drivers/ddi/ntddcdrm/ns-ntddcdrm-_cdrom_toc
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)>
    Public Class CDROM_TOC
        Public Length As UShort
        Public FirstTrack As Byte = 0
        Public LastTrack As Byte = 0
        Public TrackData As TrackDataList

        Public Sub New()
            TrackData = New TrackDataList()
            Length = CUShort(Marshal.SizeOf(Me))
        End Sub
    End Class

    <StructLayout(LayoutKind.Sequential)>
    Public Class PREVENT_MEDIA_REMOVAL
        Public PreventMediaRemoval As Byte = 0
    End Class

    ''' <summary>
    ''' The TRACK_MODE_TYPE enumeration type is used in conjunction 
    ''' with the IOCTL_CDROM_RAW_READ request and the RAW_READ_INFO 
    ''' structure to read data from a CD-ROM in raw mode.
    ''' </summary>
    Public Enum TRACK_MODE_TYPE
        YellowMode2 = 0
        XAForm2
        CDDA
    End Enum

    <StructLayout(LayoutKind.Sequential)>
    Public Class RAW_READ_INFO
        Public DiskOffset As Long = 0
        Public SectorCount As UInteger = 0
        Public TrackMode As TRACK_MODE_TYPE = TRACK_MODE_TYPE.CDDA
    End Class


    ''' <summary>
    ''' Overload version of DeviceIOControl to read the TOC (Table of contents)
    ''' </summary>
    ''' <param name="hDevice">Handle of device opened with CreateFile, </param>
    ''' <param name="IoControlCode">Must be IOCTL_CDROM_READ_TOC for this overload version</param>
    ''' <param name="InBuffer">Must be <code>IntPtr.Zero</code> for this overload version </param>
    ''' <param name="InBufferSize">Must be 0 for this overload version</param>
    ''' <param name="OutTOC">TOC object that receive the CDROM TOC</param>
    ''' <param name="OutBufferSize">Must be <code>(UInt32)Marshal.SizeOf(CDROM_TOC)</code> for this overload version</param>
    ''' <param name="BytesReturned">Receives the size, in bytes, of the data stored into OutTOC</param>
    ''' <param name="Overlapped">Pointer to an OVERLAPPED structure. Discarded for this case</param>
    ''' <returns>If the function succeeds, the return value Is nonzero. If the function fails, the return value Is zero.</returns>
    <DllImport("Kernel32.dll", SetLastError:=True)>
    Public Shared Function DeviceIoControl(ByVal hDevice As SafeCDHandle,
                                           ByVal IoControlCode As Integer,
                                           ByVal InBuffer As IntPtr,
                                           ByVal InBufferSize As Integer,
                                     <Out> ByVal OutTOC As CDROM_TOC,
                                           ByVal OutBufferSize As Integer,
                                           ByRef BytesReturned As Integer,
                                           ByVal Overlapped As IntPtr) As Integer

    End Function

    ''' <summary>
    ''' Overload version of DeviceIOControl to lock/unlock the CD
    ''' </summary>
    ''' <param name="hDevice">Handle of device opened with CreateFile</param>
    ''' <param name="IoControlCode">Must be IOCTL_STORAGE_MEDIA_REMOVAL for this overload version</param>
    ''' <param name="InMediaRemoval">Set the lock/unlock state</param>
    ''' <param name="InBufferSize">Must be <code>(UInt32)Marshal.SizeOf(PREVENT_MEDIA_REMOVAL)</code> for this overload version</param>
    ''' <param name="OutBuffer">Must be <code>IntPtr.Zero</code> for this overload version </param>
    ''' <param name="OutBufferSize">Must be 0 for this overload version</param>
    ''' <param name="BytesReturned">A "dummy" varible in this case</param>
    ''' <param name="Overlapped">Pointer to an OVERLAPPED structure. Discarded for this case</param>
    ''' <returns>If the function succeeds, the return value Is nonzero. If the function fails, the return value Is zero.</returns>
    <DllImport("Kernel32.dll", SetLastError:=True)>
    Public Shared Function DeviceIoControl(ByVal hDevice As SafeCDHandle,
                                           ByVal IoControlCode As Integer,
                                    <[In]> ByVal InMediaRemoval As PREVENT_MEDIA_REMOVAL,
                                           ByVal InBufferSize As Integer,
                                           ByVal OutBuffer As IntPtr,
                                           ByVal OutBufferSize As Integer,
                                           ByRef BytesReturned As Integer,
                                           ByVal Overlapped As IntPtr) As Integer

    End Function


    ''' <summary>
    ''' Overload version of DeviceIOControl to read digital data
    ''' </summary>
    ''' <param name="hDevice">Handle of device opened with CreateFile</param>
    ''' <param name="IoControlCode">Must be IOCTL_CDROM_RAW_READ for this overload version</param>
    ''' <param name="rri">RAW_READ_INFO structure</param>
    ''' <param name="InBufferSize">Size of RAW_READ_INFO structure</param>
    ''' <param name="OutBuffer">Buffer that will receive the data to be read</param>
    ''' <param name="OutBufferSize">Size of the buffer</param>
    ''' <param name="BytesReturned">Receives the size, in bytes, of the data stored into OutBuffer</param>
    ''' <param name="Overlapped">Pointer to an OVERLAPPED structure. Discarded for this case</param>
    ''' <returns>If the function succeeds, the return value Is nonzero. If the function fails, the return value Is zero.</returns>
    <DllImport("Kernel32.dll", SetLastError:=True)>
    Public Shared Function DeviceIoControl(ByVal hDevice As SafeCDHandle,
                                           ByVal IoControlCode As Integer,
                                    <[In]> ByVal rri As RAW_READ_INFO,
                                           ByVal InBufferSize As Integer,
                               <[In], Out> ByVal OutBuffer As Byte(),
                                           ByVal OutBufferSize As Integer,
                                           ByRef BytesReturned As Integer,
                                           ByVal Overlapped As IntPtr) As Integer

    End Function
#End Region

    Private cdHandle As SafeCDHandle
    Private TocValid As Boolean = False
    Private Toc As New CDROM_TOC
    Private bIsOpen As Boolean = False

    ''' <summary>
    ''' Open CD-ROM drive
    ''' 
    ''' Example: Open ("D")
    ''' </summary>
    ''' <param name="drive">Letter of the drive</param>
    ''' <returns>True on success</returns>
    Public Function Open(ByVal drive As Char) As Boolean
        Dim bResult As Boolean = False

        ' Check if device is open
        If bIsOpen = True Then
            Me.Close()
        End If

        ' Check if current drive is a CD-ROM
        If GetDriveType(drive + ":\\") = DriveTypes.DRIVE_CDROM Then

            Dim path As String = "\\.\" & drive & ":"
            ' Try to open CD drive
            cdHandle = CreateFile(path,
                                  GENERIC_READ,
                                  FILE_SHARE_READ,
                                  IntPtr.Zero,
                                  OPEN_EXISTING,
                                  0,
                                  IntPtr.Zero)

            ' Check if the function returned a valid device handle
            If cdHandle.IsInvalid = False Then
                bResult = True
                bIsOpen = True
            Else
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error())
            End If

        End If

        Return bResult
    End Function

    ' Close drive
    Public Sub Close()
        If bIsOpen = True Then

            ' Close current device
            cdHandle.Dispose()
            cdHandle = Nothing

            ' Reset
            TocValid = False
            bIsOpen = False
        End If
    End Sub

    ''' <summary>
    '''  Check if Device is open
    ''' </summary>
    ''' <returns>True or false</returns>
    Public ReadOnly Property IsOpen As Boolean
        Get
            Return bIsOpen
        End Get
    End Property

    ''' <summary>
    ''' Lock: Prevent eject CD while playing
    ''' </summary>
    ''' <returns></returns>
    Public Function LockCD() As Boolean
        Dim bResult As Boolean = False

        If IsOpen = True Then
            Dim pmr As New PREVENT_MEDIA_REMOVAL
            Dim Dummy As Integer = 0

            pmr.PreventMediaRemoval = 1 ' 1 = lock, 0 = unlock


            bResult = DeviceIoControl(cdHandle,
                                      IOCTL_STORAGE_MEDIA_REMOVAL,
                                      pmr,
                                      Marshal.SizeOf(pmr),
                                      IntPtr.Zero,
                                      0,
                                      Dummy,
                                      IntPtr.Zero) <> 0

        End If

        Return bResult
    End Function

    ''' <summary>
    ''' UnLock: restore eject CD
    ''' </summary>
    ''' <returns></returns>
    Public Function UnlockCD() As Boolean
        Dim bResult As Boolean = False

        If IsOpen = True Then
            Dim pmr As New PREVENT_MEDIA_REMOVAL
            Dim Dummy As Integer = 0

            pmr.PreventMediaRemoval = 0 ' 1 = lock, 0 = unlock


            bResult = DeviceIoControl(cdHandle,
                                      IOCTL_STORAGE_MEDIA_REMOVAL,
                                      pmr,
                                      Marshal.SizeOf(pmr),
                                      IntPtr.Zero,
                                      0,
                                      Dummy,
                                      IntPtr.Zero) <> 0
        End If

        Return bResult
    End Function

    ''' <summary>
    ''' Check if there is CD in the drive
    ''' </summary>
    ''' <returns>True on success</returns>
    Public Function IsCdReady() As Boolean
        Dim bResult As Boolean = False

        If IsOpen = True Then
            Dim Dummy As Integer = 0

            bResult = DeviceIoControl(cdHandle,
                                      IOCTL_STORAGE_CHECK_VERIFY,
                                      IntPtr.Zero,
                                      0,
                                      IntPtr.Zero,
                                      0,
                                      Dummy,
                                      IntPtr.Zero) <> 0
        End If

        TocValid = bResult

        Return bResult
    End Function

    ''' <summary>
    ''' Refresh CD drive data
    ''' </summary>
    ''' <returns></returns>
    Public Function Refresh() As Boolean
        Dim bResult As Boolean = False

        If IsOpen = True Then
            If IsCdReady() = True Then
                If ReadTOC() = True Then
                    bResult = True
                End If
            End If
        End If

        TocValid = bResult

        Return bResult
    End Function

    ''' <summary>
    ''' Read table of contnent from CD
    ''' </summary>
    ''' <returns></returns>
    Private Function ReadTOC() As Boolean
        Dim bResult As Boolean = False

        If IsOpen = True Then
            Dim Dummy As Integer = 0

            ' Try to read table of contnent
            bResult = DeviceIoControl(cdHandle,
                                      IOCTL_CDROM_READ_TOC,
                                      IntPtr.Zero,
                                      0,
                                      Toc,
                                      Marshal.SizeOf(Toc),
                                      Dummy,
                                      IntPtr.Zero) <> 0
        End If

        TocValid = bResult

        Return bResult
    End Function

    ''' <summary>
    ''' Return the number of all track on CD
    ''' Nb. Return -1 on error
    ''' </summary>
    Public Function GetNumTracks() As Integer
        If TocValid = True Then
            Return Toc.LastTrack - Toc.FirstTrack + 1
        Else
            Return -1
        End If
    End Function

    ''' <summary>
    ''' Return the number of Audio track on CD
    ''' Nb. Return -1 on error
    ''' </summary>
    Public Function GetNumAudioTracks() As Integer
        If TocValid Then
            Dim nTracks As Integer = 0

            ' Check if track control = 0 (2 channel audio) 
            ' or control = 1 (2 channel audio with a Emphasis of high freq, here not processed)
            For i As Integer = Toc.FirstTrack - 1 To Toc.LastTrack - 1
                If (Toc.TrackData.Item(i).Control = 0) Or
                    (Toc.TrackData.Item(i).Control = 2) Then
                    nTracks += 1
                End If
            Next

            Return nTracks
        Else
            Return -1
        End If
    End Function

    ''' <summary>
    '''  Return a Char Array with avaiable CD device
    '''  innstalled in the computer
    '''  
    ''' If empty there is not any drive to open
    ''' </summary>
    Public Function GetCDDriveLetters() As Char()
        Dim res As String = ""
        Dim c As Char = vbNullChar

        ' Scan all chars from ascii A to Z
        For i As Integer = 0 To 25

            c = Chr(i + 65)

            ' If a drive exist and is a CD then add to array
            If GetDriveType(c & ":") = DriveTypes.DRIVE_CDROM Then
                res += c
            End If
        Next

        ' Return drive array
        Return res.ToCharArray()
    End Function

    ''' <summary>
    ''' Convert Minute sec
    ''' </summary>
    Public Function Msb2Lsb(ByVal a0 As Byte,
                            ByVal a1 As Byte,
                            ByVal a2 As Byte,
                            ByVal a3 As Byte) As Long
        Return ((a1 * 75 * 60) + (a2 * 75) + a3) - 150

    End Function

    ''' <summary>
    ''' Return the first sector address of the track in LSB
    ''' </summary>
    ''' <param name="nTrack">Track number</param>
    Public Function GetTrackStartingAddress(ByVal nTrack As Integer) As Long
        If TocValid = True Then

            ' Align to track 0
            nTrack = nTrack - 1

            If (nTrack >= Toc.FirstTrack) And (nTrack <= Toc.LastTrack) Then
                Dim result As Long

                ' Convert to LSB
                result = Msb2Lsb(Toc.TrackData.Item(nTrack).Address_0,
                                 Toc.TrackData.Item(nTrack).Address_1,
                                 Toc.TrackData.Item(nTrack).Address_2,
                                 Toc.TrackData.Item(nTrack).Address_3)

                Return result
            End If
        Else
            Return -1
        End If
    End Function

    ''' <summary>
    ''' Return the track length in LSB
    ''' </summary>
    ''' <param name="nTrack"></param>
    ''' <returns></returns>
    Public Function GetTrackLength(ByVal nTrack As Integer) As Long
        If TocValid = True Then
            If (nTrack >= Toc.FirstTrack) And (nTrack <= Toc.LastTrack) Then
                Dim result As Long

                result = GetTrackStartingAddress(nTrack + 1) - GetTrackStartingAddress(nTrack)

                ' Check if last track is data. Should be an Enhanced CD
                ' ref. https://en.wikipedia.org/wiki/Enhanced_CD
                If (nTrack + 1) = Toc.LastTrack Then
                    If (Toc.TrackData.Item(nTrack + 1).Control <> 0) And
                       (Toc.TrackData.Item(nTrack + 1).Control <> 2) Then
                        result = result - ENHANCED_CD_OFFSET
                    End If
                End If

#If DEBUG Then
                DebugPrintLine("CDDrive", "Track: " & nTrack.ToString & " Length: " & result.ToString)
#End If

                Return result
            End If
        Else
            Return -1
        End If
    End Function

    ''' <summary>
    ''' Read "SectorsToRead" sectors starting at "StartSectorLsb"
    ''' and fill the Buffer Array.
    ''' 
    ''' Size of buffer = DROM_SECTOR_TO_READ * CDROM_SECTOR_SIZE - 1
    ''' 
    ''' And the param "SectorsToRead" must be at least 20 sectors
    ''' 
    ''' </summary>
    ''' <param name="Buffer">Buffer to fill</param>
    ''' <param name="StartSectorLsb">Start sector address in LSB</param>
    ''' <param name="SectorsToRead">Number of sectors to read (Max: 20) </param>
    ''' <returns></returns>
    Public Function ReadSectorsRaw(ByRef Buffer() As Byte,
                                   ByVal StartSectorLsb As Long,
                                   ByVal SectorsToRead As Long) As Boolean

        Dim sReadInfo As New RAW_READ_INFO
        Dim nBytesRead As Integer = 0
        Dim nResult As Integer = 0
        Dim bResult As Boolean = False

        If TocValid = True Then

            ' Fill structure
            With sReadInfo
                .TrackMode = TRACK_MODE_TYPE.CDDA
                .SectorCount = CUInt(SectorsToRead)
                .DiskOffset = StartSectorLsb * CDROM_DATA_TO_SEC
            End With

            ' Try to read sectors
            nResult = DeviceIoControl(cdHandle,
                            IOCTL_CDROM_RAW_READ,
                            sReadInfo,
                            Marshal.SizeOf(sReadInfo),
                            Buffer,
                            SectorsToRead * CDROM_SECTOR_SIZE,
                            nBytesRead, IntPtr.Zero)

            If nResult <> 0 Then
                bResult = True
            End If

        End If

        Return bResult
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Per rilevare chiamate ridondanti

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: eliminare lo stato gestito (oggetti gestiti).
            End If

            ' TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire sotto l'override di Finalize().
            ' TODO: impostare campi di grandi dimensioni su Null.
            Close()

        End If
        disposedValue = True
    End Sub

    ' TODO: eseguire l'override di Finalize() solo se Dispose(disposing As Boolean) include il codice per liberare risorse non gestite.
    'Protected Overrides Sub Finalize()
    '    ' Non modificare questo codice. Inserire sopra il codice di pulizia in Dispose(disposing As Boolean).
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Questo codice viene aggiunto da Visual Basic per implementare in modo corretto il criterio Disposable.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Non modificare questo codice. Inserire sopra il codice di pulizia in Dispose(disposing As Boolean).
        Dispose(True)
        ' TODO: rimuovere il commento dalla riga seguente se è stato eseguito l'override di Finalize().
        'GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

