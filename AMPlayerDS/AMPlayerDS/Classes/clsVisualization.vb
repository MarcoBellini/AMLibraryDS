Public Class clsVisualization
    Implements IDisposable

    ' Private Const CUSTOM_SCALE_FACTOR As Integer = 15
    Private Const NUMBER_OF_BARS As Integer = 20
    Private Const DECREASING_BAR_VALUE As Double = 1.5#
    Private Const DECREASING_PEAK_VALUE As Double = 3.0#
    Private Const WAIT_DECREASING_PEAK_VALUE As Double = 3.0#

    Private Structure VisualizationBar
        Public nIndex As Int32
        Public dOldValue As Double
        Public dPeakOldValue As Double
        Public dPeakTimeValue As Double
    End Structure


    ' At 44100hz samplerate remebrer: 1= 85 hz and 256=22050
    ' Human range value is from 30hz to 11000hz
    Private frequency() As Integer = {1, 5, 7, 9, 12, 15, 18, 21, 25, 28, 33, 38, 41, 43, 46, 50, 55, 60, 85, 95}

    ' Multiply Each band to Loud high frequecies. This allow to 
    ' draw a well shaped spectrums
    'Private gain() As Single = {0.6, 0.8, 1, 1.2, 1.4, 2, 2.3, 2.6, 3, 3.5, 4, 4.5, 5, 5.5, 6, 7, 7, 7, 7, 8}

    Private bars(NUMBER_OF_BARS - 1) As VisualizationBar

    Private pPictueBox As PictureBox = Nothing
    Private bit As Bitmap
    Private gc As Graphics
    'Private render As Graphics
    Private WhitePen As Brush

    Public Sub New(ByRef value As PictureBox)
        pPictueBox = value

        'Init bars
        For i As Integer = 0 To NUMBER_OF_BARS - 1
            bars(i).nIndex = frequency(i)
            bars(i).dOldValue = 0
            bars(i).dPeakOldValue = 0
        Next

        'Create graphics
        bit = New Bitmap(pPictueBox.Width, pPictueBox.Height)
        gc = Graphics.FromImage(bit)
        'render = pPictueBox.CreateGraphics
        WhitePen = New SolidBrush(Color.DarkOliveGreen) ' TODO: Dynamic color

        'Set rendering
        gc.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
        'render.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed

        'Clear with black background
        gc.Clear(Color.Transparent)
    End Sub

    Public Sub DrawAmplitudes(ByRef data() As Double)
        Dim rect As Rectangle
        Dim peak As Rectangle
        Dim value As Double
        Dim prevIndex As Integer = 0

        ' Clear background
        gc.Clear(Color.Transparent)

        'Draw bars
        For i As Integer = 0 To NUMBER_OF_BARS - 1

            ' calculate averange of frequency and return abs value
            value = AvgFrequency(data, prevIndex, bars(i).nIndex) 
            prevIndex = bars(i).nIndex

            ' Multiply by a custom factor to scale input to picturebox height
            value = value / 35 * pPictueBox.Height

            ' Clip values to fit in the picture box
            If value > pPictueBox.Height Then
                value = pPictueBox.Height
            End If

            If value < 0 Then
                value = 0
            End If

            ' if value > of stored value update value, otherwise decrease value
            If value > bars(i).dOldValue Then
                bars(i).dOldValue = value
            Else
                If bars(i).dOldValue > 0 Then
                    bars(i).dOldValue -= DECREASING_BAR_VALUE
                End If

            End If

            ' if value > of stored value update value, otherwise
            ' wait WAIT_DECREASING_PEAK_VALUE value then decrease
            ' peak value
            If value > bars(i).dPeakOldValue Then
                bars(i).dPeakOldValue = value
                bars(i).dPeakTimeValue = 0.0#
            Else
                If bars(i).dPeakTimeValue >= WAIT_DECREASING_PEAK_VALUE Then
                    If bars(i).dPeakOldValue > 0 Then
                        bars(i).dPeakOldValue -= DECREASING_PEAK_VALUE
                    End If

                Else
                    bars(i).dPeakTimeValue += 0.1#
                End If
            End If

            'Peak coord
            peak.X = 4 + i * 5
            peak.Width = 3
            peak.Y = pPictueBox.Height - CInt(bars(i).dPeakOldValue)
            peak.Height = 1

            ' TODO: dynamic color
            gc.DrawLine(Pens.White, peak.X, peak.Y, peak.X + peak.Width - 1, peak.Y)

            ' Bars
            rect.X = 2 + i * 5
            rect.Width = 3
            rect.Y = pPictueBox.Height - CInt(bars(i).dOldValue)
            rect.Height = CInt(bars(i).dOldValue)

            gc.FillRectangle(WhitePen, rect)

        Next

        ' draw double buffer image
        'render.DrawImage(bit, New Point(0, 0))
        pPictueBox.Image = bit
    End Sub

    ''' <summary>
    ''' Calculate the averange value of frenquences
    ''' </summary>
    ''' <param name="buffer">Data to process</param>
    ''' <param name="nStartIndex">Start Frequency Index</param>
    ''' <param name="nEndIndex">End Frequency Index</param>
    ''' <returns></returns>
    Private Function AvgFrequency(ByRef buffer() As Double, ByVal nStartIndex As Integer, ByVal nEndIndex As Integer) As Double
        Dim result As Double = 0

        ' Sum values
        For i As Integer = nStartIndex To nEndIndex - 1
            result = result + buffer(i)
        Next

        ' Calculate Avg
        result = result / (nEndIndex - nStartIndex)

        Return result
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then

                'Free resource
                If gc IsNot Nothing Then
                    gc.Dispose()
                    gc = Nothing
                End If

                'If render IsNot Nothing Then
                '    render.Dispose()
                '    render = Nothing
                'End If
            End If
        End If

        disposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub
#End Region
End Class
