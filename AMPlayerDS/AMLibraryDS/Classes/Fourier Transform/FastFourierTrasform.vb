Public Class FastFourierTrasform

    Private Const LOG2 As Double = 0.69314718055994518
    Private Const PI2 As Double = 6.2831853071795862

    Public Enum SampleSize
        FFT_Size_512 = 512
        FFT_Size_1024 = 1024
        FFT_Size_2048 = 2048
        FFT_Size_4096 = 4096
        FFT_Size_8192 = 8192
    End Enum

    Public Enum Window
        FFT_Wnd_None
        FFT_Wnd_Hann
        FFT_Wnd_Hamming
        FFT_Wnd_Blackman
        FFT_Wnd_Welch
        FFT_Wnd_Rectangular
        FFT_Wnd_Lanczos
        FFT_Wnd_Triangular
    End Enum

    Private nSampleSize As Integer
    Private nSampleSize2 As Integer
    Private CosSinTable() As Complex
    Private BitRevTable() As Integer
    Private WindowTable() As Double

    Public Sub New()
        nSampleSize = 0
        nSampleSize2 = 0
    End Sub

    Public Sub New(FFTSize As SampleSize)
        Init(FFTSize)
    End Sub

    Public Sub New(FFTSize As SampleSize, ByVal FFTWindow As Window)
        Init(FFTSize, FFTWindow)
    End Sub

    Private Function NumberOfBitsNeeded(ByVal PowerOfTwo As Integer) As Integer
        Return Math.Round(Math.Log(PowerOfTwo) / LOG2)
    End Function

    Private Function IsPowerOfTwo(ByVal nOutSamples As Integer) As Boolean

        If (nOutSamples < 2) Then Return False

        Return CBool(nOutSamples And (nOutSamples - 1)) = False
    End Function

    Private Function ReverseBits(ByVal Index As Integer, ByVal NumBits As Integer) As Integer

        Dim i, rev As Integer

        rev = 0

        For i = 0 To NumBits - 1
            rev = (rev + rev) Or (Index And 1)
            Index = Index \ 2
        Next

        Return rev
    End Function

    Public Function Init(ByVal FFTSize As SampleSize, Optional ByVal FFTWindow As Window = Window.FFT_Wnd_Hann) As Boolean
        Dim DeltaAngle As Double
        Dim nNumBits As Integer

        If IsPowerOfTwo(FFTSize) Then
            nSampleSize = FFTSize
            nSampleSize2 = FFTSize \ 2

            ReDim CosSinTable(nSampleSize2)
            ReDim BitRevTable(nSampleSize - 1)
            ReDim WindowTable(nSampleSize - 1)

            nNumBits = NumberOfBitsNeeded(nSampleSize)

            ' Init Cos Sin Table
            For i As Integer = 0 To nSampleSize2
                DeltaAngle = PI2 * i / nSampleSize
                CosSinTable(i) = New Complex()
                CosSinTable(i).re = Math.Cos(DeltaAngle)
                CosSinTable(i).im = -Math.Sin(DeltaAngle)
            Next

            ' Init Bit Reverse table
            For i As Integer = 0 To nSampleSize - 1
                BitRevTable(i) = ReverseBits(i, nNumBits)
            Next

            ' Init FFT Window
            If FFTWindow <> Window.FFT_Wnd_None Then
                WindowTable = FFTCreateWindow(FFTWindow, FFTSize)
            End If

            Return True
        End If

        Return False
    End Function

    Public Sub FFT(ByRef Samples() As Complex, Optional InverseTrasform As Boolean = False)
        Dim i, j, k, n, NumBlocks, BlockSize, BlockEnd, QIndex As Integer
        Dim T As New Complex

        ' Reverse bits
        For i = 0 To nSampleSize - 1
            j = BitRevTable(i)

            If j > i Then
                T = Samples(j)
                Samples(j) = Samples(i)
                Samples(i) = T
            End If
        Next

        BlockEnd = 1
        BlockSize = 2

        ' Perform FFT
        Do While BlockSize <= nSampleSize

            NumBlocks = nSampleSize \ BlockSize
            i = 0

            Do While i < nSampleSize

                QIndex = 0

                For n = 0 To BlockEnd - 1
                    j = i + n
                    k = j + BlockEnd

                    If InverseTrasform = False Then
                        T = CosSinTable(QIndex) * Samples(k)
                        Samples(k) = Samples(j) - T
                        Samples(j) = Samples(j) + T
                    Else
                        T = CosSinTable(nSampleSize2 - QIndex) * Samples(k)
                        Samples(k) = Samples(j) + T
                        Samples(j) = Samples(j) - T
                    End If

                    QIndex += NumBlocks
                Next

                i += BlockSize

            Loop

            BlockEnd = BlockSize
            BlockSize += BlockSize
        Loop

        If InverseTrasform = True Then
            For k = 0 To nSampleSize - 1
                Samples(k).re = Samples(k).re / nSampleSize
                Samples(k).im = Samples(k).im / nSampleSize
            Next k
        End If
    End Sub

    Public Sub FFTApplyWindow(ByRef Samples() As Double)
        For i As Integer = 0 To nSampleSize - 1
            Samples(i) *= WindowTable(i)
        Next
    End Sub

    Public Function FFTGetMagnitude(ByRef Sample As Complex) As Double
        Return Math.Sqrt(Sample.re * Sample.re + Sample.im * Sample.im)
    End Function

    Public Function FFTGetMagnitudeDB(ByRef Sample As Complex) As Double
        Return 20 * Math.Log10(FFTGetMagnitude(Sample))
    End Function

    Private Function FFTCreateWindow(ByVal wnd As Window, ByVal intSamples As SampleSize) As Double()

        Dim dblWnd(intSamples - 1) As Double
        Dim i As Integer

        Select Case wnd

            Case Window.FFT_Wnd_Hann
                ' von-Hann Window
                For i = 0 To intSamples - 1
                    dblWnd(i) = 0.5 * (1 - Math.Cos((2 * Math.PI * i) / (intSamples - 1)))
                Next

            Case Window.FFT_Wnd_Hamming
                ' Hamming Window
                For i = 0 To intSamples - 1
                    dblWnd(i) = 0.54 - 0.46 * Math.Cos((2 * Math.PI * i) / (intSamples - 1))
                Next

            Case Window.FFT_Wnd_Blackman
                ' Blackman Window
                For i = 0 To intSamples - 1
                    dblWnd(i) = 0.42 - 0.5 * Math.Cos((2 * Math.PI * i) / (intSamples - 1)) + 0.8 * Math.Cos((4 * Math.PI * i) / (intSamples - 1))
                Next

            Case Window.FFT_Wnd_Welch
                ' Welch Window
                For i = 0 To intSamples - 1
                    dblWnd(i) = 1 - ((2 * i - (intSamples - 1)) / (intSamples - 1)) ^ 2
                Next

            Case Window.FFT_Wnd_Rectangular
                For i = 0 To intSamples - 1
                    dblWnd(i) = 1
                Next

            Case Window.FFT_Wnd_Lanczos
                For i = 0 To intSamples - 1
                    dblWnd(i) = Math.Sin(Math.PI * (2 * i / intSamples - 1))
                Next

            Case Window.FFT_Wnd_Triangular
                For i = 0 To intSamples - 1
                    dblWnd(i) = (2 / intSamples) * ((intSamples / 2) - intSamples - intSamples - 1 / 2)
                Next
        End Select

        Return dblWnd

    End Function

End Class
