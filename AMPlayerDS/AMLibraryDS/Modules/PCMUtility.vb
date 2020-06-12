
Module PCMUtility
    ' TODO: mono to stereo (8 bit)
    Private Function HIBYTE(ByRef param As Short) As Byte
        Return param >> 8 And &HFF
    End Function

    Private Function LOBYTE(ByRef param As Short) As Byte
        Return param And &HFF
    End Function

    'Converts a 8 bit sample to a 16 bit one
    Public Function byte_to_short_cast(ByVal b As Byte) As Short
        Return (b + CShort(b) << 8) - 32768
    End Function

    ' Converts a 16 bit sample to a 8 bit one (data loss !!)
    Public Function short_to_byte_cast(ByVal s As Short) As Byte
        Return HIBYTE(s) ^ &H80
    End Function

    Public Function byte_to_short(ByRef b0 As Byte, ByRef b1 As Byte) As Short
        Return b0 Or CShort(b1) << 8
    End Function

    Public Function short_to_byte(ByRef s0 As Short) As Byte()
        Return {LOBYTE(s0), HIBYTE(s0)}
    End Function

    Public Function short_to_float(ByRef s0 As Short) As Double
        Return s0 / Short.MaxValue
    End Function

    Public Function float_to_short(ByRef f0 As Double) As Short
        Dim result As Double

        result = f0 * Short.MaxValue

        ' Round for a less data lost 
        ' NB: This instruction require a lot of CPU, now commented 01/06/20
        ' result = Math.Round(result, 0, MidpointRounding.AwayFromZero)

        'Normalize value (2^15 numbers)
        If result > Short.MaxValue Then
            result = Short.MaxValue
        ElseIf result < Short.MinValue Then
            result = Short.MinValue
        End If

        Return CShort(result)
    End Function

    Public Function float_to_byte(ByRef f0 As Double) As Byte()
        Return short_to_byte(float_to_short(f0))
    End Function

    Public Function byte_to_float(ByRef b0 As Byte, ByRef b1 As Byte) As Double
        Return short_to_float(byte_to_short(b0, b1))
    End Function

    'Two's complement: https://en.wikipedia.org/wiki/Two%27s_complement
    Public Function signed_byte_to_float(ByRef b0 As Byte) As Double
        Dim result As Double

        If b0 < 128 Then
            result = b0 / 128.0# ' 0 - 127
        Else
            result = (b0 - 256.0#) / 128.0# ' 128 - 255
        End If

        Return result
    End Function

    'Two's complement: https://en.wikipedia.org/wiki/Two%27s_complement
    Public Function float_to_signed_byte(ByRef f0 As Double) As Byte
        Dim result As Byte

        If f0 < 0 Then
            ' Normalize value
            If f0 < -1.0# Then
                f0 = -1.0#
            End If

            result = CByte((f0 * 128.0#) + 256.0#)

        Else
            ' Normalize value
            If f0 > 1.0# Then
                f0 = 1.0#
            End If

            result = CByte(f0 * 128.0#)

        End If

    End Function

    Public Function short_mono_to_stereo(ByRef s0 As Short) As Short()
        Return {s0, s0}
    End Function

    Public Function short_stereo_to_mono(ByRef s0 As Short, ByRef s1 As Short) As Short
        Return (s0 + s1) \ 2
    End Function

    Public Sub byte_to_short_array(ByRef b0() As Byte, ByRef s0() As Short, ByVal count As Integer)
        For i As Integer = 0 To (count \ 2) - 1
            s0(i) = byte_to_short(b0(i * 2 + 0), b0(i * 2 + 1))
        Next
    End Sub

    Public Sub short_to_byte_array(ByRef s0() As Short, ByRef b0() As Byte, ByVal count As Integer)
        Dim arr() As Byte

        For i As Integer = 0 To count - 1
            arr = short_to_byte(s0(i))
            b0(i * 2 + 0) = arr(0)
            b0(i * 2 + 1) = arr(1)
        Next
    End Sub

    Public Sub byte_to_float_array_16bits(ByRef b0() As Byte, ByRef f0() As Double, ByVal count As Integer)

        For i As Integer = 0 To (count \ 2) - 1
            f0(i) = byte_to_float(b0(i * 2 + 0), b0(i * 2 + 1))
        Next

    End Sub

    Public Sub byte_to_float_array_16bits(ByRef b0() As Byte, ByRef f0() As Single, ByVal count As Integer)

        For i As Integer = 0 To (count \ 2) - 1
            f0(i) = CSng(byte_to_float(b0(i * 2 + 0), b0(i * 2 + 1)))
        Next

    End Sub

    Public Sub float_to_byte_array_16bits(ByRef f0() As Double, ByRef s0() As Byte, ByVal count As Integer)
        Dim arr() As Byte

        For i As Integer = 0 To count - 1
            arr = float_to_byte(f0(i))

            s0(i * 2 + 0) = arr(0)
            s0(i * 2 + 1) = arr(1)
        Next
    End Sub


    Public Sub float_to_byte_array_16bits(ByRef f0() As Single, ByRef s0() As Byte, ByVal count As Integer)
        Dim arr() As Byte

        For i As Integer = 0 To count - 1
            arr = float_to_byte(CSng(f0(i)))

            s0(i * 2 + 0) = arr(0)
            s0(i * 2 + 1) = arr(1)
        Next

    End Sub



End Module
