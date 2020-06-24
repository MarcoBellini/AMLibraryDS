Imports System.Runtime.InteropServices

' Very basic Customized listview 
' In design mode you can edit color,size and font

' Version: 0.03
' Developed by Marco Bellini

' 0.02
' Added multiple row selection
' Removed multithreaded drawing
' Fixed Header resize clip value

' 0.03
' Improved Column resizing
' Fixed some row text clipping
' Fixed problem With incorrect text cutting On some lines

Public Class CustomListViewControl

    ' Rows data collection
    Private Class Row
        Public Text As List(Of String)
        Public IsSelected As Boolean
        Public IsHighlight As Boolean
    End Class

    ' Columns data collection
    Private Class Column
        Public Text As String
        Public Width As Integer
        Public IsMouseHover As Boolean
        Public IsMouseDown As Boolean
    End Class

    'Const
    Private Const RESIZE_AREA_MAGIN As Integer = 15 'px
    Private Const MINUMUM_COLUMN_WIDTH As Integer = 120 'px
    Private Const ALLOW_MULTIPLE_SELECTIONS As Boolean = True

    ' Property
    Private BackgroundColor As Color = Color.LightSlateGray
    Private ItemColor As Color = Color.SlateGray
    Private PairItemColor As Color = Color.LightSlateGray
    Private ItemTextColor As Color = Color.WhiteSmoke
    Private SelectedItemColor As Color = Color.CadetBlue
    Private HighlightItemColor As Color = Color.RoyalBlue
    Private ItemFont As Font = SystemFonts.DefaultFont
    Private RowHeight As Int32 = 15

    ' Memory of data
    Private RowsArray As New List(Of Row)
    Private ColumnsArray As New List(Of Column)

    ' Graphics function
    Private HeaderImage As Bitmap
    Private HeaderGraph As Graphics
    Private ListImage As Bitmap
    Private ListGraph As Graphics
    Private ItemRectangleBrush As SolidBrush
    Private PairItemRectangleBrush As SolidBrush
    Private SelectedItemRectangleBrush As SolidBrush
    Private ItemTextColorBrush As SolidBrush
    Private HighlightColorBrush As SolidBrush
    Private ThemeHandle As IntPtr = IntPtr.Zero
    Private PictureboxHdc As IntPtr = IntPtr.Zero

    ' Booleans
    Private bCanDraw As Boolean = False
    Private bMouseDownOnHeader As Boolean = False
    Private bMouseDownOnList As Boolean = False
    Private bHeaderResizeMode As Boolean = False

    ' Used for resizing
    Private nResizeColumnIndex As Integer = 0
    Private nResizeColumnStartWidth As Integer = 0

    'Events
    Public Event ItemDoubleClick(ByVal index As Int32)
    Public Event ItemKeyDown(ByVal index As Int32, ByVal key As KeyEventArgs)

#Region "Control Properties"

    ''' <summary>
    ''' Set Background color of listview
    ''' </summary>
    ''' <returns>Color</returns>
    Public Property BackgroundColorProperty As Color
        Get
            Return BackgroundColor
        End Get
        Set(value As Color)
            BackgroundColor = value
            PaintData()
        End Set
    End Property

    ''' <summary>
    ''' Set / Get The item color
    ''' </summary>
    ''' <returns>Color</returns>
    Public Property ItemColorProperty As Color
        Get
            Return ItemColor
        End Get
        Set(value As Color)
            ItemColor = value
            PaintData()
        End Set
    End Property

    ''' <summary>
    ''' Set / Get The item color of pair item
    ''' </summary>
    ''' <returns>Color</returns>
    Public Property PairItemColorProperty As Color
        Get
            Return PairItemColor
        End Get
        Set(value As Color)
            PairItemColor = value
            PaintData()
        End Set
    End Property

    ''' <summary>
    ''' Set / Get The main text color of item
    ''' </summary>
    ''' <returns>Color</returns>
    Public Property ItemTextColorProperty As Color
        Get
            Return ItemTextColor
        End Get
        Set(value As Color)
            ItemTextColor = value
            PaintData()
        End Set
    End Property

    ''' <summary>
    ''' Set / Get The selected item color
    ''' </summary>
    ''' <returns>Color</returns>
    Public Property SelectedItemColorProperty As Color
        Get
            Return SelectedItemColor
        End Get
        Set(value As Color)
            SelectedItemColor = value
            PaintData()
        End Set
    End Property

    ''' <summary>
    ''' Get or Set the color of Highlight item
    ''' </summary>
    ''' <returns>Color</returns>
    Public Property HighlightItemColorProperty As Color
        Get
            Return HighlightItemColor
        End Get
        Set(value As Color)
            HighlightItemColor = value
            PaintData()
        End Set
    End Property

    ''' <summary>
    ''' Set / Get the text font of every row
    ''' </summary>
    ''' <returns>A Font object</returns>
    Public Property ItemFontProperty As Font
        Get
            Return ItemFont
        End Get
        Set(value As Font)
            ItemFont = value
            PaintData()
        End Set
    End Property

    ''' <summary>
    ''' Set the height of each row
    ''' </summary>
    ''' <value>0 , 30</value>
    ''' <returns>An Integer of the height of each row</returns>
    Public Property RowHeightProperty As Integer
        Get
            Return RowHeight
        End Get
        Set(value As Integer)
            RowHeight = value
            PaintData()
        End Set
    End Property



#End Region

#Region "Picturebox + Control Events"
    ' Detect Mouse Click on list picture box
    Private Sub PictureBox_List_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox_List.MouseDown
        Dim rect As Rectangle
        Dim mouse As POINT
        Dim nEndIndex, nStartIndex, nRowYCoord As Int32
        Dim bFoundElement As Boolean = False

        ' Allow only selection with left button
        If e.Button <> MouseButtons.Left Then Exit Sub

        'Get mouse position and correct Y coordinate
        mouse = GetMousePosition()
        mouse.Y = mouse.Y - PictureBox_Header.Height

        ' Get visible rows
        nStartIndex = StartRowIndex()
        nEndIndex = nStartIndex + VisibleRows()
        nRowYCoord = 0

        ' Clear all state
        For i As Integer = 0 To RowsArray.Count - 1
            RowsArray(i).IsSelected = False
        Next

        ' In visible rows check where mouse is down
        For i As Integer = nStartIndex To nEndIndex - 1
            With rect
                .X = 0
                .Width = PictureBox_List.Width
                .Y = nRowYCoord
                .Height = RowHeightProperty
            End With

            If (mouse.Y > rect.Top) And (mouse.Y < rect.Bottom) Then

                ' Only left button allowed
                If e.Button = MouseButtons.Left Then
                    RowsArray(i).IsSelected = True
                End If

            End If

            nRowYCoord = nRowYCoord + RowHeightProperty
        Next

        ' Update graphics
        PaintData()

        ' Used in mouse move
        bMouseDownOnList = True
    End Sub

    ' Allow multiple selections only if ALLOW_MULTIPLE_SELECTIONS is set true
    Private Sub PictureBox_List_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox_List.MouseMove
        If bMouseDownOnList = True Then
            Dim rect As Rectangle
            Dim mouse As POINT
            Dim nEndIndex, nStartIndex, nRowYCoord As Int32
            Dim bFoundElement As Boolean = False

            'Get mouse position and correct Y coordinate
            mouse = GetMousePosition()
            mouse.Y = mouse.Y - PictureBox_Header.Height

            ' Get visible rows
            nStartIndex = StartRowIndex()
            nEndIndex = nStartIndex + VisibleRows()
            nRowYCoord = 0

            ' Check if multiple selections are allowed
            If Not ALLOW_MULTIPLE_SELECTIONS Then

                ' Clear all state
                For i As Integer = 0 To RowsArray.Count - 1
                    RowsArray(i).IsSelected = False
                Next

            End If

            ' In visible rows check where mouse is down
            For i As Integer = nStartIndex To nEndIndex - 1
                With rect
                    .X = 0
                    .Width = PictureBox_List.Width
                    .Y = nRowYCoord
                    .Height = RowHeightProperty
                End With

                If (mouse.Y > rect.Top) And (mouse.Y < rect.Bottom) Then

                    ' Only left button allowed
                    If e.Button = MouseButtons.Left Then
                        RowsArray(i).IsSelected = True
                    End If

                End If

                nRowYCoord = nRowYCoord + RowHeightProperty
            Next

            ' Update graphics
            PaintData()
        End If
    End Sub

    Private Sub PictureBox_List_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox_List.MouseUp
        ' Reset
        bMouseDownOnList = False
    End Sub

    ' Detect Mouse Double Click on list picture box
    Private Sub PictureBox_List_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles PictureBox_List.MouseDoubleClick
        Dim nSelectedIndex As Int32 = 0

        If RowsArray.Count = 0 Then Exit Sub

        ' Not optimized, but work 
        For i As Integer = 0 To RowsArray.Count - 1
            If RowsArray(i).IsSelected = True Then
                nSelectedIndex = i
            End If
        Next

        RaiseEvent ItemDoubleClick(nSelectedIndex)
    End Sub

    ' Detect Key down on list picture box
    Private Sub PictureBox_List_KeyDown(sender As Object, e As KeyEventArgs) Handles PictureBox_List.KeyDown, Me.KeyDown
        Dim nSelectedIndex As Int32 = 0

        If RowsArray.Count = 0 Then Exit Sub

        ' Not optimized, but low work
        For i As Integer = 0 To RowsArray.Count - 1
            If RowsArray(i).IsSelected = True Then
                nSelectedIndex = i
            End If
        Next

        RaiseEvent ItemKeyDown(nSelectedIndex, e)
    End Sub

    ' Detect Wheel on list picture box
    Private Sub PictureBox_List_MouseWheel(sender As Object, e As MouseEventArgs) Handles PictureBox_List.MouseWheel
        Dim nValue As Int32

        If e.Delta > 0 Then
            nValue = VScrollBar.Value - 1
        Else
            nValue = VScrollBar.Value + 1
        End If

        nValue = Math.Min(nValue, VScrollBar.Maximum)
        nValue = Math.Max(nValue, VScrollBar.Minimum)

        If VScrollBar.Value <> nValue Then
            VScrollBar.Value = nValue
            PaintData()
        End If


    End Sub

    ' Detect Mouse Hover header picture box
    Private Sub PictureBox_Header_MouseMove(sender As Object, e As EventArgs) Handles PictureBox_Header.MouseMove
        Dim rect As Rectangle
        Dim mouse As POINT
        Dim nCoordX As Int32
        Dim ShowResizing As Boolean = False

        mouse = GetMousePosition()
        nCoordX = 0

        If bHeaderResizeMode = True Then

            ColumnsArray(nResizeColumnIndex).Width = mouse.X - nResizeColumnStartWidth

            ' Clip low values
            If ColumnsArray(nResizeColumnIndex).Width < MINUMUM_COLUMN_WIDTH Then
                ColumnsArray(nResizeColumnIndex).Width = MINUMUM_COLUMN_WIDTH
            End If

            ShowResizing = True
        Else

            For i As Integer = 0 To ColumnsArray.Count - 1

                With rect
                    rect.X = nCoordX
                    rect.Y = 0
                    rect.Width = ColumnsArray(i).Width
                    rect.Height = PictureBox_Header.Height
                End With



                ' Check if mouse is on resize area
                If (mouse.X > rect.Right - RESIZE_AREA_MAGIN) And
                    (mouse.X < rect.Right + RESIZE_AREA_MAGIN) And
                    (mouse.Y > rect.Top) And
                    (mouse.Y < rect.Bottom) Then

                    ShowResizing = True
                Else

                    ' Find the colum where mouse is on
                    If (mouse.X > rect.Left) And (mouse.Y > rect.Top) And (mouse.X < rect.Right) And (mouse.Y < rect.Bottom) Then
                        ColumnsArray(i).IsMouseHover = True
                    Else
                        ColumnsArray(i).IsMouseHover = False
                    End If
                End If


                nCoordX = nCoordX + ColumnsArray(i).Width
            Next
        End If

        If ShowResizing = True Then
            Me.Cursor = Cursors.VSplit
        Else
            Me.Cursor = Cursors.Default
        End If

        ' Update row width
        PaintData()
    End Sub

    '  Detect mouse leave on header
    Private Sub PictureBox_Header_MouseLeave(sender As Object, e As EventArgs) Handles PictureBox_Header.MouseLeave
        ' Reset mouse Hover
        For i As Integer = 0 To ColumnsArray.Count - 1
            ColumnsArray(i).IsMouseHover = False
        Next

        'Reset cursor
        If Me.Cursor = Cursors.VSplit Then
            Me.Cursor = Cursors.Default
        End If

        ' Update graphics
        PaintData()
    End Sub

    ' Detect Mouse Down header picture box
    Private Sub PictureBox_Header_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox_Header.MouseDown
        Dim rect As Rectangle
        Dim mouse As POINT
        Dim nCoordX As Int32

        mouse = GetMousePosition()
        nCoordX = 0

        For i As Integer = 0 To ColumnsArray.Count - 1

            With rect
                rect.X = nCoordX
                rect.Y = 0
                rect.Width = ColumnsArray(i).Width
                rect.Height = PictureBox_Header.Height
            End With

            ' Check if mouse is on resize area
            If (mouse.X > rect.Right - RESIZE_AREA_MAGIN) And
                (mouse.X < rect.Right + RESIZE_AREA_MAGIN) And
                (mouse.Y > rect.Top) And
                (mouse.Y < rect.Bottom) Then

                ' Resize header 
                bHeaderResizeMode = True
                nResizeColumnIndex = i
                nResizeColumnStartWidth = nCoordX
                Me.Cursor = Cursors.VSplit
            End If

            ' Find which column has mouse with button pressed
            If (mouse.X > rect.Left) And (mouse.Y > rect.Top) And (mouse.X < rect.Right) And (mouse.Y < rect.Bottom) Then
                ColumnsArray(i).IsMouseDown = True
            Else
                ColumnsArray(i).IsMouseDown = False
            End If

            nCoordX = nCoordX + ColumnsArray(i).Width
        Next

        bMouseDownOnHeader = True

        ' Update graphics
        PaintData()
    End Sub

    ' Detect Mouse Up on header picture box
    Private Sub PictureBox_Header_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox_Header.MouseUp
        ' Reset
        For i As Integer = 0 To ColumnsArray.Count - 1
            ColumnsArray(i).IsMouseDown = False
        Next

        'Reset cursor
        If Me.Cursor = Cursors.VSplit Then
            Me.Cursor = Cursors.Default
        End If

        bHeaderResizeMode = False
        bMouseDownOnHeader = False

        ' Update graphics
        PaintData()
    End Sub

    ' Redraw on paint event
    Private Sub CustomListViewControl_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        ' Update graphics
        PaintData()
    End Sub

    ' Create graphics resources
    Private Sub CustomListViewControl_Load(sender As Object, e As EventArgs) Handles Me.Load
        CreateNewGraphics()
    End Sub

    ' Close resources
    Protected Overrides Sub Finalize()
        CloseGraphicsResources()

        MyBase.Finalize()
    End Sub

    Private Sub CustomListViewControl_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged

        '  Resize bitmap where drawing
        If bCanDraw Then
            CloseGraphicsResources()
            CreateNewGraphics()
        Else
            CreateNewGraphics()
        End If

        UpdateScrollbars()
        PaintData()
    End Sub

    Private Sub VScrollBar_Scroll(sender As Object, e As ScrollEventArgs) Handles VScrollBar.Scroll
        ' Update graphics
        PaintData()
    End Sub

#End Region

#Region "Rendering Functions"

    ' Create new graphics resources
    Private Sub CreateNewGraphics()

        ' Resolve bug when click "show desktop" on Application Bar
        If PictureBox_Header.Width = 0 Then Exit Sub

        'Create graphics for Header
        HeaderImage = New Bitmap(PictureBox_Header.Width, PictureBox_Header.Height)
        HeaderGraph = Graphics.FromImage(HeaderImage)
        HeaderGraph.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        'Create graphics for Row List
        ListImage = New Bitmap(PictureBox_List.Width, PictureBox_List.Height)
        ListGraph = Graphics.FromImage(ListImage)
        ListGraph.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        ' Create colors for drawing
        ItemRectangleBrush = New SolidBrush(ItemColorProperty)
        PairItemRectangleBrush = New SolidBrush(PairItemColorProperty)
        SelectedItemRectangleBrush = New SolidBrush(SelectedItemColorProperty)
        ItemTextColorBrush = New SolidBrush(ItemTextColorProperty)
        HighlightColorBrush = New SolidBrush(HighlightItemColorProperty)

        ' Init Native theme color support
        ThemeHandle = InitTheme(UX_HEADER)

        bCanDraw = True
    End Sub

    ' Free resources created with CreateNewGraphics function
    Private Sub CloseGraphicsResources()
        bCanDraw = False

        ' Close native theme
        CloseTheme(ThemeHandle)
        ThemeHandle = IntPtr.Zero

        ' Free resource
        ItemRectangleBrush.Dispose()
        PairItemRectangleBrush.Dispose()
        SelectedItemRectangleBrush.Dispose()
        ItemTextColorBrush.Dispose()
        HighlightColorBrush.Dispose()

        HeaderGraph.Dispose()
        HeaderImage.Dispose()

        ListImage.Dispose()
        ListGraph.Dispose()
    End Sub

    Private Sub PaintData()
        ' Edit this variable one thread per time
        If bCanDraw = True Then
            DrawColumns()
            DrawRows()
        End If

    End Sub

    ' Draw header
    Private Sub DrawColumns()
        Dim nColumnXCoord As Int32 = 0
        Dim Rect As Rectangle
        Dim eState As State = State.NORMAL

        'Clear screen
        HeaderGraph.Clear(SystemColors.ControlLight)

        ' Get HDC of graphics
        PictureboxHdc = HeaderGraph.GetHdc()

        ' Draw all columns
        For i As Int32 = 0 To ColumnsArray.Count - 1

            With Rect
                .X = nColumnXCoord
                .Y = 0
                .Height = PictureBox_Header.Height
                .Width = ColumnsArray(i).Width
            End With

            ' Find the current state of column "tab"
            If ColumnsArray(i).IsMouseDown = True Then
                eState = State.PRESSED
            ElseIf ColumnsArray(i).IsMouseHover = True Then
                eState = State.HOT
            Else
                eState = State.NORMAL
            End If

            ' Draw background
            DrawThemeBackgroundHelper(ThemeHandle,
                                      PictureboxHdc,
                                      Rect,
                                      eState)
            ' Draw text
            DrawThemeTextHelper(ThemeHandle,
                                PictureboxHdc,
                                ColumnsArray(i).Text,
                                Rect,
                                eState)

            ' Increment X position
            nColumnXCoord += ColumnsArray(i).Width
        Next

        ' Show image
        Try
            PictureBox_Header.Image = HeaderImage
        Catch ex As Exception
            ' Nothing to do
#If DEBUG Then
            DebugPrintLine("CustomListView", "Exception: " + ex.Message)
#End If
        End Try


        'Relase HDC
        HeaderGraph.ReleaseHdc()

    End Sub

    ' Return the number of visible columns
    Private Function VisibleRows() As Int32
        Dim nRows As Int32

        nRows = PictureBox_List.Height \ RowHeightProperty
        nRows = Math.Min(nRows, RowsArray.Count)

        Return nRows
    End Function

    ' Return the index of Vertical scrollbar
    Private Function StartRowIndex() As Int32
        Dim index As Int32 = 0

        If VScrollBar.Value <> 0 Then
            index = VScrollBar.Value
            index = Math.Min(index, RowsArray.Count)
        End If

        Return index
    End Function

    ' Clip string with width > column width
    Private Function PrepareString(ByVal source As String, ByVal nColumRef As Int32) As String
        Dim sProcessedStr As String = ""
        Dim nColumnWidth, nStringWidth As Int32

        If nColumRef <= ColumnsArray.Count Then

            ' Find basic size
            nColumnWidth = ColumnsArray(nColumRef).Width
            nStringWidth = TextRenderer.MeasureText(source, ItemFontProperty).Width

            ' Remove char to fit the size of text
            While nStringWidth >= nColumnWidth
                source = source.Remove(source.Length - 1, 1)
                nStringWidth = TextRenderer.MeasureText(source, ItemFontProperty).Width
            End While

            ' return processed string
            sProcessedStr = source

        End If

        Return sProcessedStr
    End Function

    ' Draw list of row
    Private Sub DrawRows()
        Dim rect As Rectangle
        Dim nRowYPoint, nRowWidth As Int32
        Dim nStartIndex, nEndIndex As Int32
        Dim sPoint As Drawing.Point
        Dim nTextXCoord, nColumnBorderX As Int32

        ' Clear with background color
        ListGraph.Clear(BackgroundColorProperty)

        ' Find how to start and end in the list
        nStartIndex = StartRowIndex()
        nEndIndex = nStartIndex + VisibleRows()
        nRowYPoint = 0

        ' Calculate total width of header
        For i As Int32 = 0 To ColumnsArray.Count - 1
            nRowWidth = nRowWidth + ColumnsArray(i).Width
        Next

        'Draw Each row
        For i As Integer = nStartIndex To nEndIndex - 1

            With rect
                .X = 0
                .Y = nRowYPoint
                .Width = nRowWidth
                .Height = RowHeightProperty
            End With

            ' Fill with correct brush
            If RowsArray(i).IsSelected = True Then
                ListGraph.FillRectangle(SelectedItemRectangleBrush, rect)

                ' Draw rectangle around the selected item
                ListGraph.DrawRectangle(Pens.DarkGray, rect)
            Else
                If RowsArray(i).IsHighlight = True Then
                    ListGraph.FillRectangle(HighlightColorBrush, rect)

                    ' Draw rectangle around the selected item
                    ListGraph.DrawRectangle(Pens.DarkGray, rect)
                Else
                    ' Check if use color 1 or color 2
                    If i Mod 2 = 0 Then
                        ListGraph.FillRectangle(PairItemRectangleBrush, rect)
                    Else
                        ListGraph.FillRectangle(ItemRectangleBrush, rect)
                    End If
                End If

            End If


            ' Draw text in the right column
            nTextXCoord = 0
            For j As Int32 = 0 To ColumnsArray.Count - 1
                With sPoint
                    .X = nTextXCoord
                    .Y = nRowYPoint
                End With


                ListGraph.DrawString(PrepareString(RowsArray(i).Text(j), j),
                                     ItemFontProperty,
                                     ItemTextColorBrush,
                                     sPoint)

                nTextXCoord = nTextXCoord + ColumnsArray(j).Width
            Next


            ' Increment Y value to draw Line
            nRowYPoint = nRowYPoint + RowHeightProperty
        Next

        'Draw Column Lines
        nColumnBorderX = 0
        For i As Int32 = 0 To ColumnsArray.Count - 1
            nColumnBorderX = nColumnBorderX + ColumnsArray(i).Width

            ListGraph.DrawLine(SystemPens.ActiveBorder,
                               nColumnBorderX,
                               0,
                               nColumnBorderX,
                               PictureBox_List.Height)


        Next

        ' Assign new image to picturebox
        PictureBox_List.Image = ListImage
    End Sub




#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Add a column
    ''' </summary>
    ''' <param name="caption">Caption of the column</param>
    ''' <param name="width">Width of the column</param>
    Public Sub AddColumn(ByVal caption As String,
                         ByVal width As Int32)


        Dim tempColumn As New Column

        With tempColumn
            .Text = caption
            .Width = width
            .IsMouseDown = False
            .IsMouseHover = False
        End With

        ColumnsArray.Add(tempColumn)

    End Sub

    ''' <summary>
    ''' Edit a column
    ''' </summary>
    ''' <param name="index">Index of the column to edit</param>
    ''' <param name="caption">New caption</param>
    ''' <param name="width">New width</param>
    Public Sub EditColumn(ByVal index As Int32,
                         ByVal caption As String,
                         ByVal width As Int32)


        If (index <= ColumnsArray.Count) And (index >= 0) Then
            ColumnsArray(index).Text = caption
            ColumnsArray(index).Width = width
        End If

    End Sub

    ''' <summary>
    ''' Add single row. Remeber to add the same parameter as
    ''' the number of columns
    ''' </summary>
    ''' <param name="TextPerColumn"> Array referred to a column</param>
    Public Sub AddRow(ParamArray ByVal TextPerColumn() As String)

        Dim tempRow As New Row

        tempRow.IsSelected = False
        tempRow.IsHighlight = False
        tempRow.Text = New List(Of String)

        For i As Int32 = 0 To TextPerColumn.Length - 1
            tempRow.Text.Add(TextPerColumn(i))
        Next

        RowsArray.Add(tempRow)

    End Sub

    ''' <summary>
    ''' Edit an existing row
    ''' </summary>
    ''' <param name="index">Index of the row</param>
    ''' <param name="TextPerColumn">New param of row</param>
    Public Sub EditRow(ByVal index As Int32,
                      ParamArray ByVal TextPerColumn() As String)


        If (index <= RowsArray.Count) And (index >= 0) Then
            RowsArray(index).Text.Clear()

            For i As Int32 = 0 To TextPerColumn.Length - 1
                RowsArray(index).Text.Add(TextPerColumn(i))
            Next

        End If

    End Sub

    ''' <summary>
    ''' Remove a row
    ''' </summary>
    ''' <param name="index">Index of row to delete</param>
    Public Sub RemoveRow(ByVal index As Int32)

        If (index <= ColumnsArray.Count) And (index >= 0) Then
            RowsArray.RemoveAt(index)
        End If

    End Sub

    ''' <summary>
    ''' Clear the List, all items will be removed
    ''' </summary>
    Public Sub RemoveAllRows()

        If RowsArray IsNot Nothing Then
            RowsArray.Clear()
        End If

    End Sub

    ''' <summary>
    ''' Update scrollbar after adding, editing o rmeoving row
    ''' </summary>
    Public Sub UpdateScrollbars()
        'TODO: Implement Horizonal scrollbar

        ' Update vertical scrollbar
        If RowsArray.Count > 0 Then
            VScrollBar.Enabled = True
            VScrollBar.Value = 0
            VScrollBar.Minimum = 0
            VScrollBar.Maximum = RowsArray.Count - VisibleRows()
        Else
            VScrollBar.Enabled = False
            VScrollBar.Value = 0
            VScrollBar.Minimum = 0
            VScrollBar.Maximum = 1
        End If
    End Sub

    ''' <summary>
    '''  Call this function when a row is added, edited or removed.
    '''  This method is userful when there are a lot of row to add.
    ''' </summary>
    Public Sub UpdateRowGraphics()
        PaintData()
    End Sub

    ''' <summary>
    ''' Get current selected item
    ''' </summary>
    ''' <returns></returns>
    Public Function SelectedItems() As List(Of Integer)
        Dim ListOfSelected As New List(Of Integer)

        ' Not optimized, but low work
        For i As Integer = 0 To RowsArray.Count - 1
            If RowsArray(i).IsSelected = True Then
                ListOfSelected.Add(i)
            End If
        Next

        Return ListOfSelected
    End Function

    ''' <summary>
    ''' Highlight an Index
    ''' </summary>
    ''' <param name="index"></param>
    ''' <returns>True or False</returns>
    Public Function HighlightItem(ByVal index As Integer) As Boolean

        Dim bResult As Boolean = False

        If (index >= 0) And (index < RowsArray.Count) Then
            RowsArray(index).IsHighlight = True
            bResult = True
        End If

        Return bResult
    End Function

    ''' <summary>
    ''' Clear all Highlight flag
    ''' </summary>
    Public Sub ClearHighlight()
        For i As Integer = 0 To RowsArray.Count - 1
            RowsArray(i).IsHighlight = False
        Next
    End Sub

#End Region

#Region "Native methods"
    Private Const UXTHEME_DLL As String = "uxtheme.dll"

    Private Const UX_BUTTON As String = "Button"
    Private Const UX_TABLE As String = "Tab"
    Private Const UX_HEADER As String = "Header"

    ' States
    Private Const TS_NORMAL As Int32 = 1
    Private Const TS_HOT As Int32 = 2
    Private Const TS_PRESSED As Int32 = 3
    Private Const TS_DISABLED As Int32 = 4
    Private Const TS_CHECKED As Int32 = 5

    'DrawTextFlags
    Private Const DT_TOP As Int32 = &H0
    Private Const DT_LEFT As Int32 = &H0
    Private Const DT_CENTER As Int32 = &H1
    Private Const DT_RIGHT As Int32 = &H2
    Private Const DT_VCENTER As Int32 = &H4
    Private Const DT_BOTTOM As Int32 = &H8
    Private Const DT_WORDBREAK As Int32 = &H10
    Private Const DT_SINGLELINE As Int32 = &H20
    Private Const DT_EXPANDTABS As Int32 = &H40
    Private Const DT_TABSTOP As Int32 = &H80
    Private Const DT_NOCLIP As Int32 = &H100
    Private Const DT_EXTERNALLEADING As Int32 = &H200
    Private Const DT_CALCRECT As Int32 = &H400
    Private Const DT_NOPREFIX As Int32 = &H800
    Private Const DT_INTERNAL As Int32 = &H1000
    Private Const DT_EDITCONTROL As Int32 = &H2000
    Private Const DT_PATH_ELLIPSIS As Int32 = &H4000
    Private Const DT_END_ELLIPSIS As Int32 = &H8000
    Private Const DT_MODIFYSTRING As Int32 = &H10000
    Private Const DT_RTLREADING As Int32 = &H20000
    Private Const DT_WORD_ELLIPSIS As Int32 = &H40000
    Private Const DT_NOFULLWIDTHCHARBREAK As Int32 = &H80000
    Private Const DT_HIDEPREFIX As Int32 = &H100000
    Private Const DT_PREFIXONLY As Int32 = &H200000

    ' UxTheme DrawText Additional Flag
    Private Const DTT_GRAYED As Int32 = &H1


    Friend Enum UsxThemeTABParts
        TABP_TABITEM = 1
        TABP_TABITEMLEFTEDGE = 2
        TABP_TABITEMRIGHTEDGE = 3
        TABP_TABITEMBOTHEDGE = 4
        TABP_TOPTABITEM = 5
        TABP_TOPTABITEMLEFTEDGE = 6
        TABP_TOPTABITEMRIGHTEDGE = 7
        TABP_TOPTABITEMBOTHEDGE = 8
        TABP_PANE = 9
        TABP_BODY = 10
    End Enum

    <DllImport(UXTHEME_DLL, CallingConvention:=CallingConvention.Winapi)>
    Private Shared Function OpenThemeData(
    <[In]()>
    ByVal hwnd As IntPtr,
    <MarshalAs(UnmanagedType.LPWStr), [In]()>
    ByVal pszClassList As String) _
    As IntPtr

    End Function

    <DllImport(UXTHEME_DLL, CallingConvention:=CallingConvention.Winapi)>
    Private Shared Function CloseThemeData(
    <[In]()>
    ByVal hTheme As IntPtr) _
    As Int32

    End Function

    <DllImport(UXTHEME_DLL, CallingConvention:=CallingConvention.Winapi)>
    Private Shared Function IsThemeActive() _
    As Boolean

    End Function

    <DllImport(UXTHEME_DLL, CallingConvention:=CallingConvention.Winapi)>
    Private Shared Function DrawThemeText(
     <[In]()> ByVal hTheme As IntPtr,
     <[In]()> ByVal hdc As IntPtr,
     <[In]()> ByVal iPartId As Int32,
     <[In]()> ByVal iStateId As Int32,
     <MarshalAs(UnmanagedType.LPWStr), [In]()> ByVal pszText As String,
     <[In]()> ByVal iCharCount As Int32,
     <[In]()> ByVal dwTextFlags As Int32,
     <[In]()> ByVal dwTextFlags2 As Int32,
     <[In]()> ByRef pRect As RECT) _
    As Int32

    End Function

    <DllImport(UXTHEME_DLL, CallingConvention:=CallingConvention.Winapi)>
    Private Shared Function DrawThemeBackground(
    <[In]()> ByVal hTheme As IntPtr,
    <[In]()> ByVal hdc As IntPtr,
    <[In]()> ByVal iPartId As Int32,
    <[In]()> ByVal iStateId As Int32,
    <[In]()> ByRef pRect As RECT,
    <[In]()> ByRef pClipRect As RECT) _
    As Int32

    End Function

    <DllImport("gdi32")>
    Private Shared Function SelectObject(
    ByVal hDC As IntPtr,
    ByVal hObject As IntPtr) As IntPtr

    End Function

    <DllImport("gdi32")>
    Private Shared Function DeleteObject(
    ByVal hObject As IntPtr) As IntPtr

    End Function

    <DllImport("user32")>
    Private Shared Function GetCursorPos(
    ByRef lpPoint As POINT) As Boolean

    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetCursorPos(ByVal X As Integer, ByVal Y As Integer) As Boolean
    End Function

    <DllImport("user32")>
    Private Shared Function GetWindowRect(
    ByVal hwnd As IntPtr,
    ByRef lpRect As RECT) As Int32

    End Function

    <StructLayout(LayoutKind.Sequential)>
    Private Structure POINT
        Public X As Integer
        Public Y As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure RECT
        Public left As Int32
        Public top As Int32
        Public right As Int32
        Public bottom As Int32
    End Structure

    Private Enum State
        NORMAL = 1
        HOT
        PRESSED
        DISABLED 'TODO:funtion not yet implemented!
    End Enum

    Private Function InitTheme(ByVal part As String) As IntPtr
        Dim htheme As IntPtr = IntPtr.Zero

        htheme = OpenThemeData(IntPtr.Zero, part)

        Return htheme
    End Function

    Private Sub CloseTheme(ByVal htheme As IntPtr)
        CloseThemeData(htheme)
    End Sub

    Private Sub DrawThemeBackgroundHelper(ByRef htheme As IntPtr, ByRef hdc As IntPtr, ByVal rect As Rectangle, ByVal state As State)
        Dim sRect As New RECT

        With sRect
            .bottom = rect.Bottom
            .right = rect.Right
            .left = rect.Left
            .top = rect.Top
        End With

        DrawThemeBackground(htheme, hdc, 1, state, sRect, sRect)
    End Sub

    ' TODO: Disabled not implemented
    Private Sub DrawThemeTextHelper(ByRef htheme As IntPtr, ByRef hdc As IntPtr, ByVal text As String, ByVal rect As Rectangle, ByVal state As State)

        Dim sRect As New RECT
        Dim pFont As IntPtr

        pFont = SelectObject(hdc, Me.ItemFont.ToHfont)

        With sRect
            .bottom = rect.Bottom
            .right = rect.Right
            .left = rect.Left
            .top = rect.Top
        End With

        DrawThemeText(htheme, hdc, 1, 1, text, -1, DT_CENTER Or DT_VCENTER Or DT_SINGLELINE, 0, sRect)

        DeleteObject(pFont)
        pFont = IntPtr.Zero

    End Sub

    ' Return mouse position relative to CustomListView Control
    Private Function GetMousePosition() As POINT
        Dim MousePoint As New POINT
        Dim WindowRect As New RECT

        If GetCursorPos(MousePoint) = True Then
            With MousePoint
                If GetWindowRect(Me.Handle, WindowRect) <> 0 Then
                    .X = .X - WindowRect.left
                    .Y = .Y - WindowRect.top
                End If
            End With
        End If

        Return MousePoint
    End Function
#End Region
End Class
