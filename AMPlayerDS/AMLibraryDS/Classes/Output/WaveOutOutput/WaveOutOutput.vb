Imports System.Runtime.InteropServices
Imports Microsoft.Win32.SafeHandles
Imports System.Text

Public Class WaveOutOutput
    Implements ISoundOutput

#Region "SafeWaveOutHandle"
    Public Class SafeWaveOutHandle
        Inherits SafeHandleZeroOrMinusOneIsInvalid

        Public Sub New()
            MyBase.New(True)
        End Sub

        Public Sub New(ByVal device As IntPtr)
            MyBase.New(True)
            handle = device
        End Sub

        Protected Overrides Function ReleaseHandle() As Boolean
#If DEBUG Then
            DebugPrintLine("SafeWaveOutHandle", "Handle correctrly Closed")
#End If
            Return waveOutClose(handle) = 0
        End Function
    End Class
#End Region

#Region "Waveout declaration"

    <StructLayout(LayoutKind.Sequential)>
    Public Structure WaveHdr
        Public lpData As IntPtr 'pointer To locked data buffer
        Public dwBufferLength As Int32 'length Of data buffer
        Public dwBytesRecorded As Int32 'used For input only
        Public dwUser As IntPtr 'For client's use
        Public dwFlags As Int32 'assorted flags (see defines)
        Public dwLoops As Int32 'Loop control counter
        Public lpNext As IntPtr ' PWaveHdr, reserved For driver
        Public reserved As Int32 'reserved For driver
    End Structure

    Public Enum WaveFormats As Short
        Unknown = 0
        PCM = 1
        Adpcm = 2
        Float = 3
        alaw = 6
        mulaw = 7
    End Enum

    <StructLayoutAttribute(LayoutKind.Sequential)>
    Public Structure WaveFormat
        Public wFormatTag As Int16
        Public nChannels As Int16
        Public nSamplesPerSec As Int32
        Public nAvgBytesPerSec As Int32
        Public nBlockAlign As Int16
        Public wBitsPerSample As Int16
        Public cbSize As Int16
    End Structure

    <StructLayout(LayoutKind.Explicit)>
    Public Structure MMTIME
        <FieldOffset(0)> Public wType As TimeType
        <FieldOffset(4)> Public ms As UInteger
        <FieldOffset(4)> Public sample As UInteger
        <FieldOffset(4)> Public cb As UInteger
        <FieldOffset(4)> Public ticks As UInteger
        <FieldOffset(4)> Public smtpeHour As Byte
        <FieldOffset(5)> Public smpteMin As Byte
        <FieldOffset(6)> Public smpteSec As Byte
        <FieldOffset(7)> Public smpteFrame As Byte
        <FieldOffset(8)> Public smpteFps As Byte
        <FieldOffset(9)> Public smpteDummy As Byte
        <FieldOffset(10)> Public smptePad0 As Byte
        <FieldOffset(11)> Public smptePad1 As Byte
        <FieldOffset(4)> Public midiSongPtrPos As UInteger
    End Structure

    Public Enum TimeType As UInteger
        TIME_MS = &H1
        TIME_SAMPLES = &H2
        TIME_BYTES = &H4
        TIME_SMPTE = &H8
        TIME_MIDI = &H10
        TIME_TICKS = &H20
    End Enum

    <DllImport("winmm.dll")>
    Public Shared Function waveOutOpen(ByRef hWaveOut As IntPtr, ByVal uDeviceID As Int32, ByRef lpFormat As WaveFormat, ByVal dwCallback As WaveOutProc, ByVal dwInstance As IntPtr, ByVal dwFlags As Int32) As Int32
    End Function
    <DllImport("winmm.dll")>
    Public Shared Function waveOutReset(ByVal hWaveOut As SafeWaveOutHandle) As Int32
    End Function
    <DllImport("winmm.dll")>
    Public Shared Function waveOutRestart(ByVal hWaveOut As SafeWaveOutHandle) As Int32
    End Function
    <DllImport("winmm.dll")>
    Public Shared Function waveOutPause(ByVal hWaveOut As SafeWaveOutHandle) As Int32
    End Function
    <DllImport("winmm.dll")>
    Public Shared Function waveOutPrepareHeader(ByVal hWaveOut As SafeWaveOutHandle, ByVal lpWaveOutHdr As IntPtr, ByVal uSize As Int32) As Int32
    End Function
    <DllImport("winmm.dll")>
    Public Shared Function waveOutUnprepareHeader(ByVal hWaveOut As SafeWaveOutHandle, ByVal lpWaveOutHdr As IntPtr, ByVal uSize As Int32) As Int32
    End Function
    <DllImport("winmm.dll")>
    Public Shared Function waveOutWrite(ByVal hWaveOut As SafeWaveOutHandle, ByVal lpWaveOutHdr As IntPtr, ByVal uSize As Int32) As Int32
    End Function
    <DllImport("winmm.dll")>
    Public Shared Function waveOutClose(ByVal hWaveOut As IntPtr) As Int32
    End Function
    <DllImport("winmm.dll")>
    Public Shared Function waveOutSetVolume(ByVal hWaveOut As SafeWaveOutHandle, ByVal dwVolume As UInt32) As Int32
    End Function
    <DllImport("winmm.dll")>
    Public Shared Function waveOutGetVolume(ByVal hWaveOut As SafeWaveOutHandle, ByRef dwVolume As UInt32) As Int32
    End Function
    <DllImport("winmm.dll")>
    Public Shared Function waveOutGetNumDevs() As Int32
    End Function
    <DllImport("winmm.dll")>
    Public Shared Function waveOutGetPosition(ByVal hWaveOut As SafeWaveOutHandle, ByRef pmmt As MMTIME, ByVal cbmmt As Integer) As Int32
    End Function

    Public Delegate Sub WaveOutProc(ByVal dev As IntPtr, ByVal uMsg As Integer, ByVal dwUser As Integer, ByVal dwParam1 As IntPtr, ByVal dwParam2 As Integer)

    Public Const CALLBACK_FUNCTION As Integer = &H30000
    Public Const WAVE_FORMAT_DIRECT As Integer = &H8
    Public Const CALLBACK_NULL As Integer = &H0
    Public Const BUFFER_DONE As Integer = &H3BD

#End Region

    ' Const
    Private Const BUFFER_SIZE As Integer = 2000 'ms
    Private Const BUFFER_COUNT As Integer = 8 ' remeber BUFFER_SIZE / BUFFER_COUNT

    ' Property Values
    Private ptrAMPlayerHandle As IntPtr = Nothing
    Private ISndDecoder As ISoundDecoder = Nothing
    Private sCurrentStatus As Status = Status.STOPPED

    Private pHandle As SafeWaveOutHandle
    Private TempHandle As IntPtr = IntPtr.Zero

    ' Others
    Private woDone As WaveOutProc = New WaveOutProc(AddressOf WaveOutDone)
    Private iBufferSizeinByte As Integer = 0

    ' Spectrum Object
    Private SpectrumBuffer() As Byte
    Private WriteSpectrumBufferIndex As Integer = 0
    Private ReadSpectrumBufferIndex As Integer = 0
    Private nWrittenBytes As UInteger = 0

    ' Queue of WaveOut pointer
    Private lstWaveOutPtr As Queue(Of IntPtr)

    ' End of stream
    Public Event EndOfStream() Implements ISoundOutput.EndOfStream
    Private bEndOfSteam As Boolean = False

    '  SyncLock objects
    Private lockObject As New Object
    Private spectrumObject As New Object

    ' Write Thread
    Private NotifyEndOfBuffer As Threading.EventWaitHandle
    Private bAllowThread As Boolean = True
    Private WriteThread As Threading.Thread

    ' Callback of waveOutOpen(important to flush old buffer and 
    ' write the new one)
    Private Sub WaveOutDone(ByVal dev As IntPtr, ByVal uMsg As Integer, ByVal dwUser As Integer, ByVal dwParam1 As IntPtr, ByVal dwParam2 As Integer)
        If uMsg = BUFFER_DONE Then
            ' Notify End Block of data
            NotifyEndOfBuffer.Set()
        End If
    End Sub

    ''' <summary>
    ''' Name of this output(plugin)
    ''' </summary>
    ''' <returns>String content the name</returns>
    Public ReadOnly Property Name As String Implements ISoundOutput.Name
        Get
            Return "WaveOut Output 0.2"
        End Get
    End Property

    ''' <summary>
    ''' The main window Handle, important to open The output
    ''' </summary>
    ''' <returns></returns>
    Public Property AMPlayerHandle As IntPtr Implements ISoundOutput.AMPlayerHandle
        Get
            Return ptrAMPlayerHandle
        End Get
        Set(value As IntPtr)
            ptrAMPlayerHandle = value
        End Set
    End Property

    ''' <summary>
    ''' Set current status (PLAYING, PAUSING, STOPPED)
    ''' </summary>
    ''' <returns>PLAYING, PAUSING, STOPPED</returns>
    Public Property Status As Status Implements ISoundOutput.Status
        Get
            Return sCurrentStatus
        End Get
        Set(value As Status)
            Select Case value

                Case Status.PAUSING
                    If sCurrentStatus = Status.PLAYING Then
                        ' pause current stream
                        waveOutPause(pHandle)
                        sCurrentStatus = Status.PAUSING

#If DEBUG Then
                        DebugPrintLine(Me.Name, "PAUSING")
#End If
                    End If


                Case Status.PLAYING
                    If sCurrentStatus <> Status.PLAYING Then

                        If sCurrentStatus = Status.PAUSING Then
                            ' If in pause simply restart
                            waveOutRestart(pHandle)
                        Else
                            ' If stopped, write newdata in buffer
                            ' to start playing
                            WriteBuffer(iBufferSizeinByte)
                        End If

                        sCurrentStatus = Status.PLAYING

#If DEBUG Then
                        DebugPrintLine(Me.Name, "PLAYING")
#End If

                    End If


                Case Status.STOPPED
                    If sCurrentStatus <> Status.STOPPED Then

                        ' Check if current stream is in pause
                        ' then restart to before stoptoavoid deadblock
                        If sCurrentStatus = Status.PAUSING Then
                            waveOutRestart(pHandle)
                            sCurrentStatus = Status.PLAYING
                        End If

                        ' Close all buffer and seek to the begin of file
                        FlushAllBuffer()
                        ISndDecoder.Seek(0, IO.SeekOrigin.Begin)

                        ' Stop
                        sCurrentStatus = Status.STOPPED

#If DEBUG Then
                        DebugPrintLine(Me.Name, "STOPPED")
#End If
                    End If
            End Select

        End Set
    End Property

    ''' <summary>
    ''' Set decoder for read the stream (assign this after init and before open)
    ''' </summary>
    ''' <returns></returns>
    Public Property ISoundDecoder As ISoundDecoder Implements ISoundOutput.ISoundDecoder
        Get
            Return ISndDecoder
        End Get
        Set(value As ISoundDecoder)
            ISndDecoder = value
        End Set
    End Property

    ''' <summary>
    ''' Value from 0 (mute) 100 (max)
    ''' </summary>
    ''' <returns>0 to 100</returns>
    Public Property Volume As Integer Implements ISoundOutput.Volume
        Get
            Dim LeftVol As Single
            Dim RightVol As Single
            Dim Left As UInt32
            Dim Right As UInt32
            Dim vol As UInt32

            If pHandle Is Nothing Then Return 100

            ' Return value
            waveOutGetVolume(pHandle, vol)

            ' Convert from 32 bit to 16bit per channel
            Left = CUShort(Volume And &HFFFF)
            Right = CUShort(Volume >> 16)

            ' Convert from uint to signed int
            LeftVol = CSng((Left / UInt16.MaxValue) * 100.0F)
            RightVol = CSng((Right / UInt16.MaxValue) * 100.0F)

            ' Return the averange of two channels
            Return CInt(LeftVol + RightVol) \ 2
        End Get
        Set(value As Integer)
            Dim LeftVol As Single
            Dim RightVol As Single
            Dim Left As UInteger
            Dim Right As UInteger
            Dim vol As UInteger

            ' Normalize between 0.0 and +1.0
            LeftVol = Math.Min(Math.Max(value, 0), 100) / 100.0F
            RightVol = Math.Min(Math.Max(value, 0), 100) / 100.0F

            ' Convert in uint value(16 bit)
            Left = CUInt(UInt16.MaxValue * LeftVol)
            Right = CUInt(UInt16.MaxValue * RightVol)

            ' combine channels in a 32 bit variable
            vol = Left Or (Right << 16)

            ' Set new volume
            If pHandle IsNot Nothing Then
                waveOutSetVolume(pHandle, vol)

#If DEBUG Then
                DebugPrintLine(Me.Name, "Set volume " + vol.ToString)
#End If

            End If
        End Set
    End Property

    ''' <summary>
    ''' Set current Pan:
    ''' -100: right is muted
    ''' +100: left is muted
    ''' </summary>
    ''' <returns>Return -100 o +100</returns>
    Public Property Pan As Integer Implements ISoundOutput.Pan
        Get
            Dim LeftVol As Single
            Dim RightVol As Single
            Dim Left As UInt32
            Dim Right As UInt32
            Dim vol As UInt32

            ' Get value
            If pHandle Is Nothing Then Return 0


            ' Read value
            waveOutGetVolume(pHandle, vol)

            'Split from 32bit variable in two 16 bit var
            Left = CUShort(Volume And &HFFFF)
            Right = CUShort(Volume >> 16)

            ' Convert to unsigned
            LeftVol = CSng((Left / UInt16.MaxValue) * 100.0F)
            RightVol = CSng((Right / UInt16.MaxValue) * 100.0F)

            ' Convert to signed (from -100 to +100)
            Return CInt(RightVol - LeftVol)
        End Get
        Set(value As Integer)
            Dim normValue As Integer
            Dim LeftVol As Single
            Dim RightVol As Single
            Dim Left As UInt32
            Dim Right As UInt32
            Dim vol As UInt32

            ' Normalize between -1.0 and +1.0
            normValue = Math.Min(Math.Max(value, -100), 100)

            ' Mute left or right channel and fade the other
            If normValue <= 0 Then
                LeftVol = 1.0F
                RightVol = 1.0F + normValue / 100.0F
            Else
                LeftVol = 1.0F - normValue / 100.0F
                RightVol = 1.0F
            End If

            ' Cast to uint value 16bit
            Left = CUInt(UInt16.MaxValue * LeftVol)
            Right = CUInt(UInt16.MaxValue * RightVol)

            ' Combine in a 32bit var
            vol = Left Or (Right << 16)

#If DEBUG Then
            DebugPrintLine(Me.Name, "Set pan " + vol.ToString)
#End If

            ' Set pan
            If pHandle IsNot Nothing Then
                waveOutSetVolume(pHandle, vol)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Configuration Dialog of waveformat output
    ''' </summary>
    Public Sub ConfigDialog() Implements ISoundOutput.ConfigDialog
        Dim StrBlt As New StringBuilder

        StrBlt.AppendLine("<<" + Name + ">>")
        StrBlt.AppendLine(" ")
        StrBlt.AppendLine("BUFFER LEN: " + BUFFER_SIZE.ToString)
        StrBlt.AppendLine("BUFFER PARTS: " + BUFFER_COUNT.ToString)
        StrBlt.AppendLine("Current allocated parts: " + lstWaveOutPtr.Count.ToString)
        StrBlt.AppendLine("Current volume: " + Volume.ToString)
        StrBlt.AppendLine("Basic plugin, no configuration avaiabile")

        MsgBox(StrBlt.ToString)
    End Sub

    ''' <summary>
    ''' About dialog of waveformat output
    ''' </summary>
    Public Sub AboutDialog() Implements ISoundOutput.AboutDialog
        Dim StrBlt As New StringBuilder

        StrBlt.AppendLine("<<" + Name + ">>")
        StrBlt.AppendLine(" ")
        StrBlt.AppendLine("Output plugin based on WaveOut API")

        MsgBox(StrBlt.ToString)
    End Sub

    ''' <summary>
    ''' Refill buffer with fresh data, use after seek in file
    ''' </summary>
    Public Sub ReFill(ByVal state As RefillPoint) Implements ISoundOutput.ReFill
        If (Not pHandle.IsInvalid) And (sCurrentStatus = Status.PLAYING) Then
            If state = RefillPoint.Start Then
                ' Clear buffer sent to waveout
                FlushAllBuffer()
            Else
                ' Write new PCM
                WriteBuffer(iBufferSizeinByte)
            End If



#If DEBUG Then
            DebugPrintLine(Me.Name, "Refill buffer: " + iBufferSizeinByte.ToString)
#End If
        End If
    End Sub

    ''' <summary>
    ''' Init write thread and queue of buffer
    ''' </summary>
    Public Sub Init() Implements ISoundOutput.Init
        ' Reset boolean
        bEndOfSteam = False
        bAllowThread = True

        ' Create New Queue and New WaitHandle
        lstWaveOutPtr = New Queue(Of IntPtr)
        NotifyEndOfBuffer = New Threading.EventWaitHandle(False, Threading.EventResetMode.AutoReset)

#If DEBUG Then
        DebugPrintLine(Me.Name, "Init succesful")
#End If

    End Sub

    ''' <summary>
    ''' Close write thread and opened stream
    ''' </summary>
    Public Sub DeInit() Implements ISoundOutput.DeInit

        ' Close Open session
        If pHandle IsNot Nothing Then
            Close()
        End If

        'Release Event
        NotifyEndOfBuffer.Close()
        NotifyEndOfBuffer = Nothing

#If DEBUG Then
        DebugPrintLine(Me.Name, "DeInit succesful")
#End If
    End Sub

    ''' <summary>
    ''' Close opened Stream
    ''' </summary>
    Public Sub Close() Implements ISoundOutput.Close

        'Close all resource
        If pHandle IsNot Nothing Then
            Status = Status.STOPPED

            ' Close Device
            pHandle.Dispose()
            pHandle = Nothing
            TempHandle = IntPtr.Zero

            'Reset spectrum var
            WriteSpectrumBufferIndex = 0
            ReadSpectrumBufferIndex = 0
            Array.Clear(SpectrumBuffer, 0, SpectrumBuffer.Length)
            nWrittenBytes = 0

            ' Close Write Thread
            bAllowThread = False

            ' If thread still alive then close it
            If WriteThread.IsAlive Then
                If bEndOfSteam = False Then
                    WriteThread.Join()
                End If
            End If


            bEndOfSteam = False

            ' Close decoder and queue
            ISndDecoder = Nothing
            lstWaveOutPtr.Clear()
        End If

#If DEBUG Then
        DebugPrintLine(Me.Name, "Closed succesful")
#End If

    End Sub

    ' Fill buffer with played PCM data
    Public Function GetPlayedData(ByRef Buffer() As Byte, Count As Integer) As Boolean Implements ISoundOutput.GetPlayedData
        Dim bResult As Boolean = False

        SyncLock spectrumObject

            If sCurrentStatus = Status.PLAYING Then

                If nWrittenBytes >= iBufferSizeinByte Then

                    If pHandle IsNot Nothing Then
                        Dim mmTimeStr As MMTIME
                        Dim BufferPlayed, PositionInBuffer As Double

                        mmTimeStr.wType = TimeType.TIME_BYTES
                        waveOutGetPosition(pHandle, mmTimeStr, Marshal.SizeOf(mmTimeStr))

                        ' Get number of buffer are played 
                        BufferPlayed = mmTimeStr.cb / iBufferSizeinByte

                        ' Store a value from 0.0# to 1.0#
                        PositionInBuffer = BufferPlayed - Math.Floor(BufferPlayed)

                        ' Played index is PositionInBuffer * iBufferSizeinByte
                        ReadSpectrumBufferIndex = CInt(Math.Round(PositionInBuffer * iBufferSizeinByte, 0, MidpointRounding.AwayFromZero))

                        ' Return buffer from read position (circular buffer)
                        If ReadSpectrumBufferIndex + Count <= iBufferSizeinByte Then
                            Array.Copy(SpectrumBuffer, ReadSpectrumBufferIndex, Buffer, 0, Count)
                            ReadSpectrumBufferIndex = ReadSpectrumBufferIndex + Count
                        Else
                            Dim iLenA As Integer = iBufferSizeinByte - ReadSpectrumBufferIndex
                            Dim iLenB As Integer = Count - iLenA

                            Array.Copy(SpectrumBuffer, ReadSpectrumBufferIndex, Buffer, 0, iLenA)
                            Array.Copy(SpectrumBuffer, 0, Buffer, iLenA, iLenB)
                        End If

                        mmTimeStr = Nothing
                        bResult = True
                    End If
                End If

            End If

        End SyncLock

        Return bResult
    End Function

    ''' <summary>
    ''' Open new stream
    ''' </summary>
    ''' <param name="format">Valid Wave Format returned by decoder</param>
    ''' <returns></returns>
    Public Function Open(format As WaveFormatEx) As Boolean Implements ISoundOutput.Open

        Dim sWaveFormat As WaveFormat = New WaveFormat

        ' Setup waveformat
        With sWaveFormat
            .nAvgBytesPerSec = format.AvgBytesPerSec
            .nBlockAlign = format.BlockAlign
            .nChannels = format.Channels
            .nSamplesPerSec = format.Samplerate
            .wBitsPerSample = format.BitsPerSample
            .wFormatTag = WaveFormats.PCM
            .cbSize = 0
        End With

        'Check if system has audio device
        If waveOutGetNumDevs() = 0 Then
            Return False
        End If

        ' Create New wave format object(-1 = WAVE_MAPPER)
        waveOutOpen(TempHandle,
                    -1,
                    sWaveFormat,
                    woDone,
                    IntPtr.Zero,
                    CALLBACK_FUNCTION Or WAVE_FORMAT_DIRECT)

        ' Create new safe handle
        pHandle = New SafeWaveOutHandle(TempHandle)

        If pHandle.IsInvalid = False Then
            ' convert size from ms to byte
            iBufferSizeinByte = BUFFER_SIZE * format.AvgBytesPerSec \ 1000

            ' Multiple of byte
            iBufferSizeinByte = iBufferSizeinByte - (iBufferSizeinByte Mod 8)

            ' Reset spectrum values
            ReadSpectrumBufferIndex = 0
            WriteSpectrumBufferIndex = 0
            nWrittenBytes = 0
            ReDim SpectrumBuffer(iBufferSizeinByte - 1)

            ' Reset bool
            bEndOfSteam = False
            bAllowThread = True

            ' Create write thread
            WriteThread = New Threading.Thread(AddressOf WriteThreadProc)
            WriteThread.Name = "Write Thread"
            WriteThread.IsBackground = True
            WriteThread.Start()

#If DEBUG Then
            DebugPrintLine(Me.Name, "Opened succesful")
#End If



            Return True
        Else
            pHandle = Nothing
            Return False
        End If


    End Function

    ' Write decoded PCM data in buffer
    ' This function mantain BUFFER_COUNT buffers in memory.
    ' the first time is called, it fills the BUFFER_LEN bytes lenght with BUFFER_COUNT buffers
    ' and after manaint BUFFER_COUNT buffers every time is called
    Private Sub WriteBuffer(ByVal iLength As Integer)
        Dim ByteBuffer(iLength \ BUFFER_COUNT - 1) As Byte
        Dim pBuffer As IntPtr
        Dim sWaveHdr As WaveHdr
        Dim pWaveHdr As IntPtr

        Dim iPartSize As Integer = 0
        Dim iMod As Integer = 0
        Dim iNrOfRemainBuffer As Integer = 0
        Dim SizeofWaveHdr As Integer = Marshal.SizeOf(GetType(WaveHdr))

        SyncLock lockObject

            ' Calculate size of parts
            iPartSize = iLength \ BUFFER_COUNT
            iMod = iLength Mod BUFFER_COUNT
            iNrOfRemainBuffer = BUFFER_COUNT - lstWaveOutPtr.Count

#If DEBUG Then
            DebugPrintLine(Me.Name, "Writing " + iNrOfRemainBuffer.ToString + " Buffer(s)")
#End If

            For i As Integer = 0 To iNrOfRemainBuffer - 1

                ' If there is data add at end of buffer
                If i = (iNrOfRemainBuffer - 1) Then
                    iPartSize = iPartSize + iMod
                End If

                ' check the end of file before fill buffer
                If ISndDecoder.Duration - (ISndDecoder.Position + iPartSize) <= 0 Then
                    iPartSize = CInt(ISndDecoder.Duration - ISndDecoder.Position)
                    i = iNrOfRemainBuffer 'exit for
                    bEndOfSteam = True
                End If

                ' Check if buffer need to resize
                If ByteBuffer.Length <> iPartSize Then
                    ReDim ByteBuffer(iPartSize - 1)
                End If

                'Read from decoder. The decoder always return the bytes readed
                iPartSize = ISndDecoder.Read(ByteBuffer, 0, iPartSize)

                ' Check there is data, oterwise End of Stream?
                If iPartSize > 0 Then

                    ' Copy byte to unmanaged memory
                    pBuffer = Marshal.AllocHGlobal(iPartSize)
                    Marshal.Copy(ByteBuffer, 0, pBuffer, iPartSize)

                    ' Fill wave hdr struct
                    sWaveHdr = New WaveHdr()
                    sWaveHdr.lpData = pBuffer
                    sWaveHdr.dwBufferLength = iPartSize
                    sWaveHdr.dwUser = IntPtr.Zero
                    sWaveHdr.dwFlags = 0
                    sWaveHdr.dwLoops = 0

                    ' Alloc unmanaged memory for struct
                    pWaveHdr = New IntPtr()
                    pWaveHdr = Marshal.AllocHGlobal(SizeofWaveHdr)
                    Marshal.StructureToPtr(sWaveHdr, pWaveHdr, False)

                    ' Prepare data for playing
                    waveOutPrepareHeader(pHandle, pWaveHdr, SizeofWaveHdr)
                    waveOutWrite(pHandle, pWaveHdr, SizeofWaveHdr)

                    ' Add data to spectrum buffer
                    WriteSpectrumBuffer(ByteBuffer, iPartSize)

                    ' Enqueue the address of pWaveHdr (important 
                    ' in write thread )
                    lstWaveOutPtr.Enqueue(pWaveHdr)
                Else
                    ' Notify End Of Stream
                    bEndOfSteam = True

                    ' Exit
                    Exit For
                End If
            Next

        End SyncLock
    End Sub

    Private Sub WriteSpectrumBuffer(ByRef bBuffer() As Byte, ByVal iLen As Integer)

        SyncLock spectrumObject
            ' Fill spectrum buffer and update write index (circular buffer)
            If WriteSpectrumBufferIndex + iLen <= iBufferSizeinByte Then
                Array.Copy(bBuffer, 0, SpectrumBuffer, WriteSpectrumBufferIndex, iLen)
                WriteSpectrumBufferIndex = WriteSpectrumBufferIndex + iLen
            Else
                Dim iLenA As Integer = iBufferSizeinByte - WriteSpectrumBufferIndex
                Dim iLenB As Integer = iLen - iLenA

                Array.Copy(bBuffer, 0, SpectrumBuffer, WriteSpectrumBufferIndex, iLenA)
                Array.Copy(bBuffer, iLenA, SpectrumBuffer, 0, iLenB)
                WriteSpectrumBufferIndex = iLenB
            End If

            'Increment written bytes in buffer
            nWrittenBytes += CUInt(iLen)
        End SyncLock
    End Sub

    ' Close all buffer in the stream and reset device
    ' userful when Refill is called or stop is called
    ' to stop playing and free resources
    Private Sub FlushAllBuffer()
        If pHandle IsNot Nothing Then

            SyncLock lockObject

                If lstWaveOutPtr.Count > 0 Then
                    Do ' Close all Header written in buffer
                        Dim strWavehdr As WaveHdr
                        Dim pWaveHdr As IntPtr

                        pWaveHdr = lstWaveOutPtr.Dequeue

                        strWavehdr = CType(Marshal.PtrToStructure(pWaveHdr, GetType(WaveHdr)), WaveHdr)

                        waveOutUnprepareHeader(pHandle, pWaveHdr, Marshal.SizeOf(GetType(WaveHdr)))

                        Marshal.FreeHGlobal(strWavehdr.lpData)
                        Marshal.FreeHGlobal(pWaveHdr)

                        ' Release resources
                        strWavehdr = Nothing
                        pWaveHdr = Nothing
                    Loop While lstWaveOutPtr.Count > 0

                    ' Reset only if playing(avoid deadblock)
                    If Status = Status.PLAYING Then
                        waveOutReset(pHandle)
                        nWrittenBytes = 0
                        ReadSpectrumBufferIndex = 0
                        WriteSpectrumBufferIndex = 0
                    End If
                End If

#If DEBUG Then
                DebugPrintLine(Me.Name, "Flush all buffer done")
#End If

            End SyncLock
        End If
    End Sub

    ' This thread wait the event called in waveoutproc:
    ' when NotifyEndOfBuffer is set it means a block of buffer
    ' is done. So free memory for this block and write new one
    ' with fill buffer function
    Private Sub WriteThreadProc()

        Dim WaitEvent() As Threading.WaitHandle = {NotifyEndOfBuffer}
        Dim result As Integer

        While (bAllowThread = True)

            ' Wait for event signal
            result = Threading.WaitHandle.WaitAny(WaitEvent, 25, False)

            ' If NotifyEndOfBuffer 
            If result <> Threading.WaitHandle.WaitTimeout Then

                ' If list is not empty delete Header and free memory
                If lstWaveOutPtr.Count > 0 Then
                    Dim strWavehdr As WaveHdr
                    Dim pWaveHdr As IntPtr

                    pWaveHdr = lstWaveOutPtr.Dequeue()

                    strWavehdr = CType(Marshal.PtrToStructure(pWaveHdr, GetType(WaveHdr)), WaveHdr)

                    waveOutUnprepareHeader(pHandle, pWaveHdr, Marshal.SizeOf(GetType(WaveHdr)))

                    Marshal.FreeHGlobal(strWavehdr.lpData)
                    Marshal.FreeHGlobal(pWaveHdr)

                    strWavehdr = Nothing
                    pWaveHdr = Nothing

                    ' Write new part of buffer only if playing 
                    ' and end of stream is not reached
                    If (Status = Status.PLAYING) And (bEndOfSteam = False) Then
                        WriteBuffer(iBufferSizeinByte)
                    End If

                    ' Check if end of stream
                    If (lstWaveOutPtr.Count = 0) And (bEndOfSteam = True) Then
                        Status = Status.STOPPED

                        ' Notify End of stream
                        RaiseEvent EndOfStream()
                        bEndOfSteam = False

#If DEBUG Then
                        DebugPrintLine(Me.Name, "End of stream event raised")
#End If
                    End If
                End If

            End If
        End While
    End Sub
End Class
