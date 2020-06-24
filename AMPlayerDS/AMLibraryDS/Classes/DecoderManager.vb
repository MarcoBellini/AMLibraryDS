
Imports System.Text

' TODO: implement fast stream info + stream info
Public Class DecoderManager
    Implements IDisposable

    Private ptrHandle As IntPtr = IntPtr.Zero

    Private clsFFT As New FastFourierTrasform
    Private bFftInit As Boolean = False

    Private WithEvents Input As ISoundDecoder = Nothing
    Private WithEvents Output As ISoundOutput = Nothing

    Private DecoderList As New List(Of ISoundDecoder)
    Private SoundEffectList As New List(Of ISoundEffect)

    Private bIsInit As Boolean = False
    Private bIsOpen As Boolean = False

    Private sWaveFormat As WaveFormatEx

    Public Event End_Of_Stream()
    Public Event File_Opened(ByVal info As StreamInformations)
    Public Event Status_Charged(ByVal status As Status)

    ' TODO: Add fast stream information
    ' Function FastStreamInfo(path) as streaminformations

    Public Function FastStreamInformation(ByVal path As String) As StreamInformations
        Dim info As StreamInformations = Nothing

        If bIsInit = True Then

            ' Close Stream if is open
            If bIsOpen = True Then
                Close()
            End If

            ' Check for a decoder for "path"
            If GetNeededDecoder(path) = True Then

                ' Try to get Stream informations
                Input.FastStreamInformations(path, info)
            End If
        End If

        Return info
    End Function

    Public Enum OutputPlugins
        WaveOut = 0
        DirectSound
    End Enum

    Private Sub Input_NeedData(ByRef Buffer() As Byte) Handles Input.DataReaded_Event
        If Not bIsOpen Then Exit Sub ' Prevent processing on not streaming data

        Dim Samples As Integer
        Dim BytesContainer() As Byte
        Dim LeftChannel(), RightChannel() As Double

        ' Get the number of samples in the byte buffer
        Samples = Buffer.Length \ sWaveFormat.BlockAlign

        ReDim LeftChannel(Samples - 1)
        ReDim RightChannel(Samples - 1)

        ' Apply effects only to 2 channels sample
        If sWaveFormat.Channels = 2 Then
            Select Case sWaveFormat.BitsPerSample
                Case 8
                    ' (1) Convert to double samples
                    For i As Integer = 0 To Samples - 1
                        LeftChannel(i) = signed_byte_to_float(Buffer(i * 2 + 0))
                        RightChannel(i) = signed_byte_to_float(Buffer(i * 2 + 1))
                    Next

                    ' (2) Apply effects
                    For i As Integer = 0 To SoundEffectList.Count - 1
                        SoundEffectList(i).ProcessSamples(LeftChannel, RightChannel)
                    Next

                    ' (3) Convert to bytes
                    For i As Integer = 0 To Samples - 1
                        Buffer(i * 2 + 0) = float_to_signed_byte(LeftChannel(i))
                        Buffer(i * 2 + 1) = float_to_signed_byte(RightChannel(i))
                    Next
                Case 16
                    ' (1) Convert to double samples
                    For i As Integer = 0 To Samples - 1
                        LeftChannel(i) = byte_to_float(Buffer(i * 4 + 0), Buffer(i * 4 + 1))
                        RightChannel(i) = byte_to_float(Buffer(i * 4 + 2), Buffer(i * 4 + 3))
                    Next

                    ' (2) Apply effects
                    For i As Integer = 0 To SoundEffectList.Count - 1
                        SoundEffectList(i).ProcessSamples(LeftChannel, RightChannel)
                    Next

                    ' (3) Convert to bytes
                    For i As Integer = 0 To Samples - 1
                        BytesContainer = float_to_byte(LeftChannel(i))
                        Buffer(i * 4 + 0) = BytesContainer(0)
                        Buffer(i * 4 + 1) = BytesContainer(1)

                        BytesContainer = float_to_byte(RightChannel(i))
                        Buffer(i * 4 + 2) = BytesContainer(0)
                        Buffer(i * 4 + 3) = BytesContainer(1)
                    Next


            End Select
        End If
    End Sub

    Public Function GetNeededDecoder(ByVal path As String) As Boolean

        ' Scan all extension in all installed decoders
        For i As Integer = 0 To DecoderList.Count - 1
            For j As Integer = 0 To DecoderList(i).Extensions.Count - 1

                If String.Equals(IO.Path.GetExtension(path).ToLower,
                                 DecoderList(i).Extensions(j).ToLower) = True Then

                    'We Found Desidered Decoder
                    Input = DecoderList(i)

                    Return True
                End If

            Next
        Next

        Return False
    End Function

    Public Function OpenFile(ByVal path As String) As Boolean
        Dim bResult As Boolean = False

        ' Check if class is initializated
        If bIsInit = True Then

            If bIsOpen = True Then
                Close()
            End If

            If GetNeededDecoder(path) = True Then
                If Input.Open(path) = True Then
                    Dim stream As StreamInformations

                    stream = Input.OpenedStreamInformations
                    sWaveFormat = stream.Stream2WaveFormat()

                    ' verify the wave format
                    If Not stream.IsBadFormat Then

                        ' Update Input
                        Output.ISoundDecoder = Input
                        Output.Open(sWaveFormat)

                        UpdateEffectWFX()

                        bIsOpen = True
                        bResult = True

                        RaiseEvent File_Opened(stream)
                    Else
                        Input.Close()
                        Input = Nothing
                        MsgBox("Bad Wave Format. Unable to open the file.")
                    End If
                Else
                    MsgBox("Unable to Open the file.")
                End If
            Else
                MsgBox("Unable to decode the file")
            End If
        Else
            MsgBox("Class not initializated, Please call Init()")
        End If

        Return bResult
    End Function

    Public Sub Close()
        If bIsOpen Then

            ' Close Output (Still alive, only close play resources)
            Output.Close()
            Output.ISoundDecoder = Nothing

            ' Close Input
            Input.Close()
            Input = Nothing

            bIsOpen = False
        End If
    End Sub

    Public Function Init(ByVal handle As IntPtr, Optional ByVal OutputPlugin As OutputPlugins = OutputPlugins.DirectSound) As Boolean

        ' Check if current thread mode is MTA
        If Threading.Thread.CurrentThread.GetApartmentState = Threading.ApartmentState.STA Then
#If DEBUG Then
            DebugPrintLine("Decoder Manager", "Cannot Init, Use MTA Thread Instead")
#End If

            Return False
        End If

        If bIsInit = True Then
            DeInit()
        End If

        If handle <> IntPtr.Zero Then
            ptrHandle = handle

            ' Create new Output
            Select Case OutputPlugin
                Case OutputPlugins.WaveOut
                    Output = New WaveOutOutput
                Case OutputPlugins.DirectSound
                    Output = New DirectSoundOutput
            End Select

            ' Init output
            Output.AMPlayerHandle = ptrHandle
            Output.Init()

            ' Add Decoders
            AddDecoder(New StreamWAV)
            AddDecoder(New StreamMP3)
            AddDecoder(New StreamOGG)
            AddDecoder(New StreamOpus)
            AddDecoder(New StreamMediaFondation)
            AddDecoder(New StreamCDA)

            ' Add equalizer (always the first effect)
            AddEffect(New DSPEqualizer)
            AddEffect(New DSPPhaser)

            ' Init successful
            bIsInit = True

            Return True
        End If

        Return False
    End Function

    Public Sub DeInit()
        If bIsInit = True Then
            Close()

            Output.AMPlayerHandle = IntPtr.Zero
            Output.DeInit()
            Output = Nothing

            ' Free resources of decoders
            For i As Integer = 0 To DecoderList.Count - 1
                DecoderList(i).Close()
            Next

            DecoderList.Clear()
            DecoderList = Nothing

            ' Free resources of effects
            For i As Integer = 0 To SoundEffectList.Count - 1
                SoundEffectList(i).DeInit()
            Next

            SoundEffectList.Clear()
            SoundEffectList = Nothing

            ptrHandle = IntPtr.Zero
            bIsInit = False
        End If
    End Sub


    Public Function GetFrequenciesMagnitude(ByVal intFFTSize As FastFourierTrasform.SampleSize) As Double()
        ' TODO: Move to global variables?
        Dim complexSamples(intFFTSize - 1) As Complex
        Dim InSamples(intFFTSize - 1) As Double
        Dim OutSamples(intFFTSize - 1) As Double

        ' Init Class
        If bFftInit = False Then
            clsFFT.Init(intFFTSize, FastFourierTrasform.Window.FFT_Wnd_Hann)
            bFftInit = True
        End If

        ' Get value only if Playing
        If Status = Status.PLAYING Then

            ' Get samples
            If GetOutputSamples(InSamples, intFFTSize * sWaveFormat.BlockAlign) And (bIsOpen = True) Then

                ' Apply Hann Window to samples
                clsFFT.FFTApplyWindow(InSamples)

                ' Compute Inverse FFT
                For i As Integer = 0 To intFFTSize - 1
                    complexSamples(i) = New Complex()
                    complexSamples(i).re = InSamples(i)
                    complexSamples(i).im = 0
                Next

                clsFFT.FFT(complexSamples, True)

                ' Get magnitude of frequencies
                For i As Integer = 0 To intFFTSize - 1
                    OutSamples(i) = clsFFT.FFTGetMagnitude(complexSamples(i))
                Next

            Else
                Array.Clear(OutSamples, 0, OutSamples.Length)
            End If

        Else
            Array.Clear(OutSamples, 0, OutSamples.Length)
        End If

        complexSamples = Nothing
        InSamples = Nothing

        Return OutSamples
    End Function

    Public Function GetOutputSamples(ByRef buffer() As Double, ByVal Count As Integer) As Boolean
        Dim Samples As Integer = Count \ sWaveFormat.BlockAlign
        Dim bBuffer(Count - 1) As Byte
        Dim LeftChannel, RightChannel As Double

        ' Get played data from output
        Output.GetPlayedData(bBuffer, Count)


        ' Convert samples to floating point
        If sWaveFormat.BitsPerSample = 8 Then
            If sWaveFormat.Channels = 1 Then
                For i As Integer = 0 To Samples - 1
                    LeftChannel = signed_byte_to_float(bBuffer(i))
                    buffer(i) = LeftChannel
                Next
            Else
                For i As Integer = 0 To Samples - 1

                    LeftChannel = signed_byte_to_float(bBuffer(i * 2 + 0))
                    RightChannel = signed_byte_to_float(bBuffer(i * 2 + 1))

                    buffer(i) = (LeftChannel + RightChannel) / 2.0#
                Next
            End If

        Else
            If sWaveFormat.Channels = 1 Then
                For i As Integer = 0 To Samples - 1
                    LeftChannel = byte_to_float(bBuffer(i * 2 + 0), bBuffer(i * 2 + 1))
                    buffer(i) = LeftChannel
                Next
            Else
                For i As Integer = 0 To Samples - 1

                    LeftChannel = byte_to_float(bBuffer(i * 4 + 0), bBuffer(i * 4 + 1))
                    RightChannel = byte_to_float(bBuffer(i * 4 + 2), bBuffer(i * 4 + 3))

                    buffer(i) = (LeftChannel + RightChannel) / 2.0#
                Next
            End If

        End If

        Return True
    End Function

    Public ReadOnly Property CurrentFileStreamInfo() As StreamInformations
        Get
            If bIsOpen = True Then
                Return CurrentDecoder.OpenedStreamInformations()
            End If

            Return Nothing
        End Get
    End Property

    Public Property Status() As Status
        Get
            Return Output.Status
        End Get
        Set(ByVal value As Status)
            If bIsOpen = True Then
                Output.Status = value
                RaiseEvent Status_Charged(value)
            End If
        End Set
    End Property

    Public ReadOnly Property Duration() As Long
        Get
            If Not bIsOpen Then Return 0

            Return BytesToMs(Input.Duration)
        End Get
    End Property

    Public Property Volume() As Long
        Get
            Return Output.Volume
        End Get
        Set(ByVal value As Long)
            Output.Volume = value
        End Set
    End Property

    Public Property Pan() As Integer
        Get
            Return Output.Pan
        End Get
        Set(ByVal value As Integer)
            Output.Pan = value
        End Set
    End Property

    Public Property Position() As Long
        Get
            If Not bIsOpen Then Return 0
            If Status = Commons.Status.STOPPED Then Return 0

            Return BytesToMs(Input.Position)
        End Get
        Set(ByVal value As Long)

            If bIsOpen Then
                Output.ReFill(RefillPoint.Start)
                Input.Seek(MsToBytes(value), IO.SeekOrigin.Begin)
                Output.ReFill(RefillPoint.Finish)
            End If
        End Set
    End Property

    Public ReadOnly Property CurrentDecoder() As ISoundDecoder
        Get
            Return Input
        End Get
    End Property

    Public ReadOnly Property Effects(ByVal index As Integer) As ISoundEffect
        Get
            Return SoundEffectList(index)
        End Get
    End Property

    Public ReadOnly Property EffectsCount As Integer
        Get
            Return SoundEffectList.Count
        End Get
    End Property

    Public ReadOnly Property CurrentOutput() As ISoundOutput
        Get
            If Not bIsInit Then Return Nothing

            Return Output
        End Get
    End Property


    Public Function AddDecoder(ByVal decoder As ISoundDecoder) As Boolean
        DecoderList.Add(decoder)
        Return True
    End Function

    Public Function RemoveDecoder(ByVal decoder As ISoundDecoder) As Boolean

        For i As Integer = 0 To DecoderList.Count - 1

            If String.Equals(DecoderList(i).Name, decoder.Name) Then
                DecoderList.RemoveAt(i)

                Return True
            End If

        Next

        Return False
    End Function

    Public Function AddEffect(ByVal effect As ISoundEffect) As Boolean
        Try
            effect.Init()
            SoundEffectList.Add(effect)
            Return True
        Catch ex As Exception
#If DEBUG Then
            DebugPrintLine("DecoderManager", ex.Message)
#End If

            Return False
        End Try
    End Function

    Public Function RemoveEffect(ByVal effect As ISoundEffect) As Boolean
        For i As Integer = 0 To SoundEffectList.Count - 1

            If String.Equals(effect.Name, SoundEffectList(i).Name) Then
                SoundEffectList.RemoveAt(i)

                Return True
            End If

        Next

        Return False
    End Function

    Public ReadOnly Property GetSupportedExtension() As String
        Get
            Dim strBuilder As New StringBuilder
            For i As Integer = 0 To DecoderList.Count - 1
                For j As Integer = 0 To DecoderList(i).Extensions.Count - 1

                    If (i = DecoderList.Count - 1) And (j = DecoderList(i).Extensions.Count - 1) Then
                        strBuilder.Append("*" & DecoderList(i).Extensions(j) & "")
                    Else
                        strBuilder.Append("*" & DecoderList(i).Extensions(j) & ";")
                    End If

                Next
            Next

            Return strBuilder.ToString
        End Get
    End Property

    Public ReadOnly Property EndOfStream() As Boolean
        Get
            Return Input.EndOfStream
        End Get
    End Property

    Private Sub UpdateEffectWFX()
        If SoundEffectList Is Nothing Then Exit Sub

        For i As Integer = 0 To SoundEffectList.Count - 1
            SoundEffectList(i).UpdateWaveFormat(sWaveFormat)
        Next
    End Sub

    Private Sub Output_EndOfStream() Handles Output.EndOfStream
        Close()
        RaiseEvent End_Of_Stream()
    End Sub

    Private Function BytesToMs(ByVal bytes As Long) As Long
        Return bytes / sWaveFormat.AvgBytesPerSec
    End Function


    Private Function MsToBytes(ByVal ms As Long) As Long
        Dim bytes As Long = ms * sWaveFormat.AvgBytesPerSec

        Return bytes - (bytes Mod sWaveFormat.BlockAlign)
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then

            End If

            DeInit()

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
