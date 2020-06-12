''' <summary>
'''  Basic Fading effect for DirectSound output
'''  
'''  Now provide only fade in effect manipulating audio buffer
''' 
''' </summary>
Public Class DirectSoundFadeEffect
    Implements IDisposable

    Private sWaveFormat As WaveFormatEx = Nothing
    Private nCurrentByteFaded As Integer = 0
    Private nCurrentByteToFade As Integer = 0
    Private dCurrentAmp As Double = 1.0#
    Private bIsFadeIn As Boolean = False
    Private disposedValue As Boolean
    Private BytesContainer() As Byte

    ''' <summary>
    '''  Construct of class (same as init)
    ''' </summary>
    ''' <param name="sWfx">Valid WaveFormatEx structure</param>
    Public Sub New(ByVal sWfx As WaveFormatEx)
        Init(sWfx)
    End Sub

    ''' <summary>
    '''  Init state of class
    ''' </summary>
    ''' <param name="sWfx">Valid WaveFormatEx structure</param>
    Public Sub Init(ByVal sWfx As WaveFormatEx)
        sWaveFormat = sWfx
    End Sub

    ''' <summary>
    ''' Call this function before writing function in DirectSoundOutput
    ''' </summary>
    ''' <param name="ms">Lenght of fading in Milliseconds</param>
    Public Sub FadeIn(ByVal ms As Integer)
        nCurrentByteFaded = 0
        nCurrentByteToFade = ms * sWaveFormat.AvgBytesPerSec \ 1000
        dCurrentAmp = 0.1#
        bIsFadeIn = True
    End Sub

    ''' <summary>
    '''  Call this function after Decoded data and Before write in DirectSound
    ''' </summary>
    ''' <param name="buffer">PCM Raw Data</param>
    ''' <param name="nLen">Lenght of Buffer</param>
    Public Sub FadeBuffer(ByRef buffer() As Byte, ByVal nLen As Integer)
        Dim index As Integer = 0
        Dim Left, Right As Double

        If bIsFadeIn = True Then
            If sWaveFormat.Channels = 2 Then ' fade only 2 channels audio

                While (index < nLen) And (nCurrentByteFaded < nCurrentByteToFade)

                    If sWaveFormat.AvgBytesPerSec = 8 Then
                        ' 1 byte for left channel + 1 byte for right channel (normalize)
                        Left = signed_byte_to_float(buffer(index + 0))
                        Right = signed_byte_to_float(buffer(index + 1))

                        Left = Left * dCurrentAmp
                        Right = Right * dCurrentAmp

                        buffer(index + 0) = float_to_signed_byte(Left)
                        buffer(index + 1) = float_to_signed_byte(Right)

                        dCurrentAmp = nCurrentByteFaded / nCurrentByteToFade
                    Else
                        ' 2 byte for left channel + 2 byte for right channel(convert+normalize)
                        Left = short_to_float(byte_to_short(buffer(index + 0), buffer(index + 1)))
                        Right = short_to_float(byte_to_short(buffer(index + 2), buffer(index + 3)))

                        Left = Left * dCurrentAmp
                        Right = Right * dCurrentAmp

                        'Left
                        BytesContainer = short_to_byte(float_to_short(Left))
                        buffer(index + 0) = BytesContainer(0)
                        buffer(index + 1) = BytesContainer(1)

                        'Right
                        BytesContainer = short_to_byte(float_to_short(Right))
                        buffer(index + 2) = BytesContainer(0)
                        buffer(index + 3) = BytesContainer(1)

                        dCurrentAmp = nCurrentByteFaded / nCurrentByteToFade
                    End If

                    index = index + sWaveFormat.BlockAlign
                    nCurrentByteFaded = nCurrentByteFaded + 1
                End While
            End If

            ' End Fade
            If nCurrentByteFaded = nCurrentByteToFade Then
                bIsFadeIn = False
            End If
        End If


    End Sub

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            bIsFadeIn = False
            sWaveFormat = Nothing
        End If
        disposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub

End Class
