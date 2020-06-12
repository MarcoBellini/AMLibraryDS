Imports System.Runtime.InteropServices
Imports System.Threading

' Main class for Directsound Output (NB: Work only with MTAThread)
' DirectSoundNative: this class is a wrapper between managed and unmanaged
' DirectSoundFadeEffect: this class provide a basic FadeIn effect
' DirectSoundConfiguration: this dialog load and save user preferences for output

Public Class DirectSoundOutput
    Implements ISoundOutput ' to use in AMPlayer

    Private Const BUFFER_SIZE As Int32 = 2000 'ms
    Private Const PRE_BUFFER As Int32 = 250 'ms

    Private Const VOLUME_MAX As Int32 = 100
    Private Const VOLUME_MIN As Int32 = 0

    Public Event EndOfStream() Implements ISoundOutput.EndOfStream

    Private bDirectSoundInit As Boolean = False
    Private bDirectSoundIsOpen As Boolean = False
    Private bEndOfStream As Boolean = False

    Private pHandle As IntPtr = IntPtr.Zero

    Private nByteBufferLen As Int32 = 0
    Private nBytePreBufferLen As Int32 = 0

    Private sWaveFormat As WaveFormatEx

    Private StartframeEvent As EventWaitHandle
    Private MiddleframeEvent As EventWaitHandle
    Private EndframeEvent As EventWaitHandle
    Private RefillWriteEvent As EventWaitHandle

    Private iDecoder As ISoundDecoder = Nothing
    Private eCurrentStatus As Status = Status.STOPPED

    Private NotifyThread As Thread
    Private bAllowThread As Boolean = False
    Private dSoundNative As DirectSoundNative

    Private oLockObject As New Object
    Private SpectrumBuffer() As Byte

    Private clsFadeEffect As DirectSoundFadeEffect

    ''' <summary>
    ''' Open About Dialog
    ''' </summary>
    Public Sub AboutDialog() Implements ISoundOutput.AboutDialog
        MsgBox("Output Plugin based on DirectSound API, open configuration to see more...")
    End Sub

    ''' <summary>
    '''  Open Configuration Dialog
    ''' </summary>
    Public Sub ConfigDialog() Implements ISoundOutput.ConfigDialog
        If dSoundNative IsNot Nothing Then
            Dim fConfigDialog As New DirectSoundConfiguration

            ' Create and open configuration Dialog
            fConfigDialog.LstDevice = dSoundNative.DeviceList
            fConfigDialog.ShowDialog()
            fConfigDialog.Dispose()
        End If

    End Sub

    ''' <summary>
    ''' Set the main window handle
    ''' </summary>
    ''' <returns></returns>
    Public Property AMPlayerHandle() As System.IntPtr Implements ISoundOutput.AMPlayerHandle
        Get
            Return pHandle
        End Get
        Set(ByVal value As System.IntPtr)
            If value <> IntPtr.Zero Then
                pHandle = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' Get current playing buffer
    ''' </summary>
    ''' <param name="Buffer">Byte array to fill with PCM data</param>
    ''' <param name="Count"> Sizeof buffer</param>
    ''' <returns></returns>
    Public Function GetPlayedData(ByRef Buffer() As Byte, ByVal Count As Integer) As Boolean Implements ISoundOutput.GetPlayedData
        Dim nPlayPosition As Integer

        ' Get played sample only if current status is PLAYING
        If (bDirectSoundIsOpen = True) And (eCurrentStatus = Status.PLAYING) Then

            ' Read the current Play Index
            nPlayPosition = dSoundNative.PlayPositionIndex

            ' Copy data from Spectrum Circular buffer, to Linear Buffer
            If nPlayPosition + Count <= nByteBufferLen Then
                Array.Copy(SpectrumBuffer, nPlayPosition, Buffer, 0, Count)
            Else
                Dim nLenA As Integer = nByteBufferLen - nPlayPosition
                Dim nLenB As Integer = Count - nLenA

                Array.Copy(SpectrumBuffer, nPlayPosition, Buffer, 0, nLenA)
                Array.Copy(SpectrumBuffer, 0, Buffer, nLenA, nLenB)
            End If
        End If

        ' Return
        Return True
    End Function

    ''' <summary>
    ''' Set current decoder to read input file(assign this after init and before open)
    ''' </summary>
    ''' <returns></returns>
    Public Property ISoundDecoder() As ISoundDecoder Implements ISoundOutput.ISoundDecoder
        Get
            Return iDecoder
        End Get
        Set(ByVal value As ISoundDecoder)
            If value IsNot Nothing Then
                iDecoder = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' Name of the decoder
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Name() As String Implements ISoundOutput.Name
        Get
            Return "DirectSound Output 0.3b"
        End Get
    End Property

    ''' <summary>
    ''' Get or set playback volume:
    ''' 0 = min vol
    ''' 100 = max volume
    ''' </summary>
    ''' <returns></returns>
    Public Property Volume() As Integer Implements ISoundOutput.Volume
        Get
            Return LogToLinearVol(dSoundNative.Volume)
        End Get
        Set(ByVal value As Integer)
            dSoundNative.Volume = LinearToLogVol(value)
        End Set
    End Property

    ''' <summary>
    ''' Set current pan:
    ''' -100: right muted
    ''' +100 : left muted
    ''' </summary>
    ''' <returns></returns>
    Public Property Pan() As Integer Implements ISoundOutput.Pan
        Get
            Return dSoundNative.Pan \ 100
        End Get
        Set(ByVal value As Integer)
            ' -100 to + 100
            If (value > -101) And (value < 101) Then
                dSoundNative.Pan = value * 100
            End If
        End Set
    End Property

    ''' <summary>
    ''' Call when position change in the Decoder:
    ''' Refill the buffer with new data
    ''' </summary>
    Public Sub ReFill(ByVal state As RefillPoint) Implements ISoundOutput.ReFill
        If (eCurrentStatus = Status.PLAYING) And (bDirectSoundIsOpen = True) Then

            If state = RefillPoint.Start Then
                BeginRefill()
            Else
                ' Process refill from write thread
                RefillWriteEvent.Set()
            End If

        End If
    End Sub

    ''' <summary>
    ''' Inizializate Output
    ''' </summary>
    Public Sub Init() Implements ISoundOutput.Init
        If bDirectSoundInit = False Then
            dSoundNative = New DirectSoundNative

            ' Use device in the settings
            dSoundNative.CreateDevice(pHandle, My.Settings.DirectSound_DeviceGuid)

            ' Create Notification Event(used to notify the Play index
            ' in the buffer reached an offset)
            StartframeEvent = New EventWaitHandle(False, EventResetMode.AutoReset)
            MiddleframeEvent = New EventWaitHandle(False, EventResetMode.AutoReset)
            EndframeEvent = New EventWaitHandle(False, EventResetMode.AutoReset)
            RefillWriteEvent = New EventWaitHandle(False, EventResetMode.AutoReset)

            ' Init successful
            bDirectSoundInit = True
        End If
    End Sub

    ''' <summary>
    ''' Close Output
    ''' </summary>
    Public Sub DeInit() Implements ISoundOutput.DeInit
        If bDirectSoundInit = True Then

            ' If still open, close streaming
            If bDirectSoundIsOpen = True Then
                Me.Close()
            End If

            ' Close device
            dSoundNative.CloseDevice()
            dSoundNative.Dispose()
            dSoundNative = Nothing

            ' Close events
            StartframeEvent.Close()
            MiddleframeEvent.Close()
            EndframeEvent.Close()
            RefillWriteEvent.Close()

            StartframeEvent = Nothing
            MiddleframeEvent = Nothing
            EndframeEvent = Nothing
            RefillWriteEvent = Nothing

            'Reset variabiles
            pHandle = IntPtr.Zero
            dSoundNative = Nothing
            bDirectSoundInit = False
        End If
    End Sub

    ''' <summary>
    ''' Open output with current file wave format
    ''' </summary>
    ''' <param name="format">Valid waveformat returned by decoder</param>
    ''' <returns></returns>
    Public Function Open(ByVal format As Commons.WaveFormatEx) As Boolean Implements ISoundOutput.Open
        If (bDirectSoundInit = True) And (format.Samplerate <> 0) Then

            ' Save waveformt
            sWaveFormat = format

            ' Create new class for fade in effect
            clsFadeEffect = New DirectSoundFadeEffect(sWaveFormat)

            ' Load BUFFER size from saved settings
            If (My.Settings.DirectSound_BufferLen >= 800) And (My.Settings.DirectSound_BufferLen <= 4000) Then
                nByteBufferLen = MsToBytes(My.Settings.DirectSound_BufferLen)
            Else
                nByteBufferLen = MsToBytes(BUFFER_SIZE)
            End If

            ' Multiple of a byte
            nByteBufferLen = nByteBufferLen - nByteBufferLen Mod 8

            ' Load PRE BUFFER size from saved settings
            If (My.Settings.DirectSound_PreBufferLen >= 100) And (My.Settings.DirectSound_PreBufferLen <= 400) Then
                nBytePreBufferLen = MsToBytes(My.Settings.DirectSound_PreBufferLen)
            Else
                nBytePreBufferLen = MsToBytes(PRE_BUFFER)
            End If

            ' Multiple of a byte
            nBytePreBufferLen = nBytePreBufferLen - nBytePreBufferLen Mod 8

            ' Set the buffer buffer for FFT data
            ReDim SpectrumBuffer(nByteBufferLen - 1)
            Array.Clear(SpectrumBuffer, 0, SpectrumBuffer.Length)

            ' Create Primary + Secondary buffer and set notifications
            dSoundNative.CreateBuffers(sWaveFormat, nByteBufferLen)
            dSoundNative.Set3Notification(StartframeEvent.SafeWaitHandle.DangerousGetHandle(),
                                       CUInt(nByteBufferLen \ 2), ' Middle
                                       MiddleframeEvent.SafeWaitHandle.DangerousGetHandle(),
                                       CUInt(nByteBufferLen - 1), ' End buffer
                                       EndframeEvent.SafeWaitHandle.DangerousGetHandle(),
                                       UInt32.MaxValue)

            'Notify check in play buffer Thread
            bAllowThread = True
            bEndOfStream = False

            ' Thread to wait notification event in the buffer
            NotifyThread = New Thread(AddressOf WaitNotificationThread)
            NotifyThread.IsBackground = True
            NotifyThread.Priority = ThreadPriority.Normal
            NotifyThread.Name = "DirectSound Write Buffer Thread"
            NotifyThread.Start()

            ' Directsound is correctly opened
            bDirectSoundIsOpen = True
        End If

        Return bDirectSoundIsOpen
    End Function

    Public Sub Close() Implements ISoundOutput.Close
        If bDirectSoundIsOpen = True Then

            ' Stop playback
            If eCurrentStatus <> Status.STOPPED Then
                SetStatus(Status.STOPPED)
            End If

            ' Close notification thread
            bAllowThread = False

            ' Check if thread is still alive
            If NotifyThread.IsAlive Then

                ' Check the call to close is not invoked
                ' during raise event in WaitThreadProc
                ' Otherwise the app crash
                If bEndOfStream = False Then

                    ' Wait gently the closing of thread
                    NotifyThread.Join()
                End If
            End If

            ' Close buffer
            dSoundNative.CloseBuffers()
            clsFadeEffect.Dispose()

            iDecoder = Nothing
            bDirectSoundIsOpen = False
        End If
    End Sub

    ' Wait any Notification event (separate thread)
    Private Sub WaitNotificationThread()
        Dim WaitHandles() As WaitHandle
        Dim indexHandle As Integer = WaitHandle.WaitTimeout

        ' Array of notifications
        WaitHandles = {StartframeEvent, MiddleframeEvent, EndframeEvent, RefillWriteEvent}

        Do
            ' Wait DirectSound buffer notifications
            indexHandle = WaitHandle.WaitAny(WaitHandles, 50, False)


            ' If a notification is reached, write the buffer
            If (indexHandle <> WaitHandle.WaitTimeout) Then
                Select Case indexHandle
                    Case 0 'StartframeEvent

                        ' Check if end of stream
                        If bEndOfStream = True Then
                            SetStatus(Status.STOPPED)
                            RaiseEvent EndOfStream()
                        Else
#If DEBUG Then
                            DebugPrintLine(Me.Name, "Write Thread: Fill first buffer")
#End If

                            ' Fill buffer first part of buffer
                            FillBuffer(nByteBufferLen \ 2, 0)
                        End If

                    Case 1 'MiddleframeEvent

                        ' Check if end of stream
                        If bEndOfStream = True Then
                            SetStatus(Status.STOPPED)
                            RaiseEvent EndOfStream()
                        Else
#If DEBUG Then
                            DebugPrintLine(Me.Name, "Write Thread: Fill second buffer")
#End If
                            ' Fill second buffer first part of buffer
                            FillBuffer(nByteBufferLen \ 2, nByteBufferLen \ 2)
                        End If

                    Case 2 'EndframeEvent
                        'Called on Stop() in IDirectSoundBuffer8

                    Case 3 'PreBufferWriteEvent
                        ProcessRefill()
                End Select
            End If

        Loop While bAllowThread = True

#If DEBUG Then
        DebugPrintLine(Me.Name, "WaitNotificationThread Closed")
#End If
    End Sub

    ' Fill buffer with valid PCM data,Thread safe
    Private Sub FillBuffer(ByVal nWriteBytes As Integer, ByVal nBufferOffset As Integer)
        SyncLock oLockObject
            If bDirectSoundIsOpen = True And bEndOfStream = False Then
                Dim nDataReaded As Integer = 0
                Dim DataBuffer(nWriteBytes - 1) As Byte

                ' Find if stream reach the End of File
                If iDecoder.Duration - iDecoder.Position < nWriteBytes Then
                    Dim nRemaingBytes As Integer

                    nRemaingBytes = CInt(iDecoder.Duration - iDecoder.Position)
                    nDataReaded = iDecoder.Read(DataBuffer, 0, nRemaingBytes)

                    If nDataReaded > 0 Then
                        'TODO: Check if need to fill buffer with data + silence
                        dSoundNative.WriteBuffer(DataBuffer, nDataReaded, nBufferOffset)
                        Array.Copy(DataBuffer, 0, SpectrumBuffer, nBufferOffset, nDataReaded)

                        ' Notify end of stream, so waitHandleThread signal end of stream
                        ' and reset current output state
                        bEndOfStream = True
#If DEBUG Then
                        DebugPrintLine(Me.Name, "DirectSound FillBuffer Found End of Stream")
#End If
                    Else
                        ' If data = 0 end of stream reached
                        bEndOfStream = True
#If DEBUG Then
                        DebugPrintLine(Me.Name, "DirectSound FillBuffer Found End of Stream")
#End If
                    End If

                Else

                    ' Read data
                    nDataReaded = iDecoder.Read(DataBuffer, 0, nWriteBytes)

                    If nDataReaded > 0 Then
                        'Apply Fading
                        If My.Settings.DirectSound_EnableFading = True Then
                            clsFadeEffect.FadeBuffer(DataBuffer, nDataReaded)
                        End If

                        'Write buffer with decoded PCM data
                        dSoundNative.WriteBuffer(DataBuffer, nDataReaded, nBufferOffset)

                        ' Write the same data in the spectrum buffer, for visualization
                        Array.Copy(DataBuffer, 0, SpectrumBuffer, nBufferOffset, nDataReaded)
                    Else

                        ' If data = 0 end of stream reached??
                        bEndOfStream = True
#If DEBUG Then
                        DebugPrintLine(Me.Name, "DirectSound FillBuffer Found End of Stream")
#End If
                    End If


                End If

            End If

        End SyncLock
    End Sub

    Private Sub BeginRefill()

        ' Stop Playing
        dSoundNative.StopLoop()

        ' Reset playing position
        dSoundNative.PlayPositionIndex = 0

        ' Apply fading effect
        If clsFadeEffect IsNot Nothing Then
            If My.Settings.DirectSound_EnableFading = True Then
                clsFadeEffect.FadeIn(300)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Rewrite buffer after seek (function Refill)
    ''' </summary>
    Private Sub ProcessRefill()

        ' Fill buffer with pre buffer bytes
        FillBuffer(nBytePreBufferLen, 0)

        ' Start Playing
        dSoundNative.PlayLoop()

        ' Fill buffer with remanig data
        FillBuffer(nByteBufferLen - nBytePreBufferLen, nBytePreBufferLen)


#If DEBUG Then
        DebugPrintLine(Me.Name, "DirectSound Refill Done")
#End If
    End Sub


    ''' <summary>
    ''' Get or set current status:
    ''' PLAYING,PAUSING or STOPPED
    ''' </summary>
    ''' <returns>PLAYING,PAUSING or STOPPED</returns>
    Public Property Status() As Commons.Status Implements ISoundOutput.Status
        Get
            Return eCurrentStatus
        End Get
        Set(ByVal value As Commons.Status)
            SetStatus(value)
        End Set
    End Property

    ' Helper to manage status change
    Private Sub SetStatus(ByVal eStatus As Status)
        Select Case eStatus
            Case Status.PLAYING
#If DEBUG Then
                DebugPrintLine(Me.Name, "DirectSound change status: PLAYING")
#End If
                ' Checkif current playing is topped or in pause
                If eCurrentStatus = Status.STOPPED Then
                    ' Tell to fade class to start fade in
                    If clsFadeEffect IsNot Nothing Then
                        If My.Settings.DirectSound_EnableFading = True Then
                            clsFadeEffect.FadeIn(400)
                        End If
                    End If

                    ' Reset position  
                    dSoundNative.PlayPositionIndex = 0

                    ' Fill buffer with pre buffer bytes
                    FillBuffer(nBytePreBufferLen, 0)

                    ' Play
                    dSoundNative.PlayLoop()

                    ' Fill buffer with remanig data
                    FillBuffer(nByteBufferLen - nBytePreBufferLen, nBytePreBufferLen)

                    ' Update status
                    eCurrentStatus = eStatus
                ElseIf eCurrentStatus = Status.PAUSING Then
                    ' Play from pause
                    dSoundNative.PlayLoop()

                    ' Update status
                    eCurrentStatus = eStatus
                End If

            Case Status.PAUSING
#If DEBUG Then
                DebugPrintLine(Me.Name, "DirectSound change status: PAUSING")
#End If
                ' Pause only if playing
                If eCurrentStatus = Status.PLAYING Then
                    dSoundNative.StopLoop()

                    ' Update status
                    eCurrentStatus = eStatus
                End If

            Case Status.STOPPED
#If DEBUG Then
                DebugPrintLine(Me.Name, "DirectSound change status: STOPPED")
#End If

                ' Stop and reset position
                If eCurrentStatus = Status.PLAYING Then
                    dSoundNative.StopLoop()
                End If

                dSoundNative.PlayPositionIndex = 0

                ' Seek to start of the file
                iDecoder.Seek(0, IO.SeekOrigin.Begin)

                ' Update status
                eCurrentStatus = eStatus
        End Select

    End Sub

    ' Helper function
    ' convert from bytes to milliseconds
    Private Function BytesToMs(ByVal bytes As Int32) As Int32
        Return bytes * 1000 \ sWaveFormat.AvgBytesPerSec
    End Function

    ' Convert from milliseconds to byte
    Private Function MsToBytes(ByVal ms As Int32) As Int32
        Dim bytes As Int32 = ms * sWaveFormat.AvgBytesPerSec \ 1000

        Return bytes - (bytes Mod sWaveFormat.BlockAlign)
    End Function

    ' use this functions to transform Logaritmic scale of directsound volume
    Private Function LinearToLogVol(ByVal vol As Integer) As Integer
        Dim result As Integer

        If vol <= VOLUME_MIN Then
            result = -10000
        ElseIf vol >= VOLUME_MAX Then
            result = 0
        Else
            result = CInt(-4000 * Math.Log10(VOLUME_MAX / vol))
        End If

        Return result
    End Function

    ' use this functions to transform Logaritmic scale of directsound volume
    Private Function LogToLinearVol(ByRef vol As Integer) As Integer
        Dim result As Integer

        result = CInt(VOLUME_MAX / Math.Pow(10, vol / (-4000)))

        Return result
    End Function

End Class
