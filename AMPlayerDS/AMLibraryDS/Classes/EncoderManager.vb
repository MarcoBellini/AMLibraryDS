Public Class EncoderManager
    Implements IDisposable

    Private Const INPUT_BUFFER_SIZE As Integer = 4096

    Private Decoder As DecoderManager
    Private Resampler As SamplerateConverter

    Private InputInformations As StreamInformations
    Private OutputInformations As StreamInformations

    Private CurrentEncoder As ISoundEncoder
    Private CurrentDecoder As ISoundDecoder

    Private EncodersList As New List(Of ISoundEncoder)

    Private bIsOpen As Boolean = False
    Private bChargeSamplerate As Boolean = False

    Public Event WritePass(ByVal percent As Single)

    Public Sub New()

        ' Add encoders
        AddEncoder(New EncoderWAV)
        AddEncoder(New EncoderMP3)

        ' Add decoders
        Decoder = New DecoderManager()
        Decoder.AddDecoder(New StreamWAV)
        Decoder.AddDecoder(New StreamMP3)
        Decoder.AddDecoder(New StreamOGG)
        Decoder.AddDecoder(New StreamOpus)
        Decoder.AddDecoder(New StreamMediaFondation)
        Decoder.AddDecoder(New StreamCDA)


#If DEBUG Then
        DebugPrintLine("EncoderManager", "Create new Encoder Manager")
#End If

    End Sub

    Public Function AddEncoder(ByVal encoder As ISoundEncoder) As Boolean
        EncodersList.Add(encoder)
        Return True
    End Function

    Public Function RemoveDecoder(ByVal encoder As ISoundEncoder) As Boolean

        For i As Integer = 0 To EncodersList.Count - 1

            If String.Equals(EncodersList(i).Name, encoder.Name) Then
                EncodersList.RemoveAt(i)

                Return True
            End If

        Next

        Return False
    End Function

    Private Function GetNeededEncoder(ByVal path As String) As Boolean

        ' Scan all extension in all installed encoders
        For i As Integer = 0 To EncodersList.Count - 1

            If String.Equals(IO.Path.GetExtension(path).ToLower,
                                 EncodersList(i).Extension.ToLower) = True Then

                'We Found Desidered Encoder
                CurrentEncoder = EncodersList(i)

                Return True
            End If


        Next

        Return False
    End Function

    Public ReadOnly Property GetOutputSupportedExtension() As String
        Get
            Dim strBuilder As New Text.StringBuilder

            For i As Integer = 0 To EncodersList.Count - 1

                If i < EncodersList.Count - 1 Then
                    strBuilder.Append("*" & EncodersList(i).Extension & ";")
                Else
                    strBuilder.Append("*" & EncodersList(i).Extension & "")
                End If

            Next

            Return strBuilder.ToString
        End Get
    End Property

    Public ReadOnly Property GetInputSupportedExtension() As String
        Get
            Return Decoder.GetSupportedExtension()
        End Get
    End Property

    Public ReadOnly Property EncodersNameList As List(Of String)
        Get
            Dim EncodersListLocal As New List(Of String)

            For i As Integer = 0 To EncodersList.Count - 1
                EncodersListLocal.Add(EncodersList(i).Name)
            Next

            Return EncodersListLocal
        End Get
    End Property

    Public Function GetExtensionByEncoderName(ByVal EncoderName As String) As String
        Dim extension As String = ""

        For i As Integer = 0 To EncodersList.Count - 1

            If String.Equals(EncoderName, EncodersList(i).Name) = True Then
                extension = EncodersList(i).Extension
            End If
        Next

        Return extension
    End Function

    Public Function GetEncoderByEncoderName(ByVal EncoderName As String) As ISoundEncoder
        Dim encoder As ISoundEncoder = Nothing

        For i As Integer = 0 To EncodersList.Count - 1

            If String.Equals(EncoderName, EncodersList(i).Name) = True Then
                encoder = EncodersList(i)
            End If
        Next

        Return encoder
    End Function

    Public Function OpenTranscode(ByVal InputFile As String,
                                  ByVal OutputFile As String,
                                  ByRef OutInformations As StreamInformations) As Boolean

        Dim bResult As Boolean = False

        ' Check if there is an open session
        If bIsOpen = True Then
            CloseTranscode()
        End If

        ' Create resources
        Resampler = New SamplerateConverter()

        ' Check if input file is supported
        If Decoder.GetNeededDecoder(InputFile) Then

            ' Retive ref of correct decoder
            CurrentDecoder = Decoder.CurrentDecoder

            ' Try to open file
            If CurrentDecoder.Open(InputFile) = True Then

                ' Retive input file informations
                InputInformations = CurrentDecoder.OpenedStreamInformations()

                ' Get correct encoder
                If GetNeededEncoder(OutputFile) = True Then

                    ' Copy ref of output format
                    If OutInformations Is Nothing Then
                        ' if Output informations are nothing use Input stream info instead
                        ' to encode the file
                        OutputInformations = InputInformations
                    Else
                        ' If Output informations are not nothing provide resample
                        ' and copy ID3 tags
                        OutputInformations = OutInformations
                        InputInformations.CopyID3To(OutputInformations)
                    End If


                    ' Check if format is correct
                    If (InputInformations.IsBadFormat = False) And (OutputInformations.IsBadFormat = False) Then

                        ' Create file
                        If CurrentEncoder.Create(OutputFile, OutputInformations) = True Then



                            ' Check samplerate (only if stream info are different)
                            If InputInformations.Samplerate <> OutputInformations.Samplerate Then
                                bChargeSamplerate = True

                                ' Create new resampler session at medium quality
                                Resampler.NewConversion(InputInformations.Samplerate,
                                                        OutputInformations.Samplerate,
                                                        LibSamplerate.ConverterType.SRC_SINC_MEDIUM_QUALITY,
                                                        InputInformations.Channels,
                                                        OutputInformations.Channels)
                            End If


                            ' Correctly opened
                            bIsOpen = True
                            bResult = True

#If DEBUG Then
                            DebugPrintLine("EncoderManager", "OpenTranscode successful")
#End If

                        Else
                            ' Close Decoder
                            If CurrentDecoder IsNot Nothing Then
                                CurrentDecoder.Close()
                                CurrentDecoder = Nothing
                            End If

                            ' Close decoder manager
                            Decoder.Dispose()
                            Decoder = Nothing

                            ' Close lib samplerate
                            Resampler.Dispose()
                            Resampler = Nothing
                        End If

                    Else
                        MsgBox("Input or Output has bad wave format")
                    End If

                Else
                    MsgBox("Cannot encode file")
                End If

            Else
                MsgBox("Cannot open input file")
            End If

        Else
            MsgBox("Unable to decode the file")
        End If

        Return bResult
    End Function

    Public Sub CloseTranscode()

        'Check if open
        If bIsOpen = True Then
            ' Close encoder
            If CurrentEncoder IsNot Nothing Then
                CurrentEncoder.Close()
                CurrentEncoder = Nothing
            End If


            ' Close Decoder
            If CurrentDecoder IsNot Nothing Then
                CurrentDecoder.Close()
                CurrentDecoder = Nothing
            End If


            ' Close lib samplerate
            Resampler.Dispose()
            Resampler = Nothing

            ' Reset vars
            bIsOpen = False
            bChargeSamplerate = False

            InputInformations = Nothing
            OutputInformations = Nothing

#If DEBUG Then
            DebugPrintLine("EncoderManager", "CloseTranscode successful")
#End If
        End If


    End Sub

    Public Function TrancodeFileUntilEof() As Boolean
        Dim bResult As Boolean = False
        Dim bEndOfFile As Boolean = False

        Dim InputBuffer(INPUT_BUFFER_SIZE - 1) As Byte
        Dim OutputBuffer(INPUT_BUFFER_SIZE - 1) As Byte

        Dim nReadedBytes, nBytesToWrite As Integer


        If bIsOpen = True Then
            Do

                ' (1) Read PCM data from decoder
                If CurrentDecoder.Position + INPUT_BUFFER_SIZE < CurrentDecoder.Duration Then
                    nReadedBytes = CurrentDecoder.Read(InputBuffer, 0, INPUT_BUFFER_SIZE)
                Else
                    ' Reched the end of file
                    nReadedBytes = CurrentDecoder.Read(InputBuffer, 0, CurrentDecoder.Duration - CurrentDecoder.Position)
                    bEndOfFile = True
                End If


                ' (2) Perform samplerate conversion
                If bChargeSamplerate = True Then

                    Resampler.Convert_16_bits(InputBuffer,
                                              OutputBuffer,
                                              nReadedBytes,
                                              nBytesToWrite,
                                              bEndOfFile)

                Else
                    ' If no resample requied, just copy to output buffer
                    Array.Copy(InputBuffer, OutputBuffer, nReadedBytes)
                    nBytesToWrite = nReadedBytes
                End If

                ' (3) Write data to output file
                CurrentEncoder.Write(OutputBuffer, 0, nBytesToWrite)

                ' (4) Notify new write step
                RaiseEvent WritePass(CurrentDecoder.Position / CurrentDecoder.Duration * 100)

            Loop While Not bEndOfFile

            bResult = True
        End If

#If DEBUG Then
        DebugPrintLine("EncoderManager", "TrancodeFileUntilEof Terminated")
#End If

        Return bResult
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then

                ' Free resources of decoders
                For i As Integer = 0 To EncodersList.Count - 1
                    EncodersList(i).Close()
                Next

                EncodersList.Clear()
                EncodersList = Nothing

                If Decoder IsNot Nothing Then
                    Decoder.Dispose()
                    Decoder = Nothing
                End If


#If DEBUG Then
                DebugPrintLine("EncoderManager", "Encoder Manager Disposed")
#End If

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
