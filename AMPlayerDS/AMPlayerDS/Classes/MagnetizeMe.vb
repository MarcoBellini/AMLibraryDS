Imports System.Runtime.InteropServices
Public Class MagnetizeMe
    Inherits NativeWindow
    Implements IDisposable

    ' Used to control the distance to dock the
    ' window to the screen edge
    Private Const nMagnetDelta As Integer = 20

    ' Vars
    Private disposedValue As Boolean

#Region "Windows API"

    Private Const WM_WINDOWPOSCHANGING As Integer = &H46
    Private Const DWMWA_EXTENDED_FRAME_BOUNDS As Integer = 9

    <StructLayout(LayoutKind.Sequential)>
    Private Structure NativeRect
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure WINDOWPOS
        Public hwnd As IntPtr
        Public hwndInsertAfter As IntPtr
        Public x As Integer
        Public y As Integer
        Public cx As Integer
        Public cy As Integer
        Public flags As Integer
    End Structure

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetWindowRect(ByVal Hwnd As IntPtr,
                                          ByRef lpRect As NativeRect) As <MarshalAs(UnmanagedType.Bool)> Boolean

    End Function

    <DllImport("dwmapi.dll", SetLastError:=True)>
    Private Shared Function DwmGetWindowAttribute(ByVal Hwnd As IntPtr,
                                                  ByVal dwAttribute As Integer,
                                                  ByRef lpRect As NativeRect,
                                                  ByVal cbAttribute As Integer) As Integer

    End Function
#End Region

    Public Sub New(ByRef Frm As Form)
        AssignHandle(Frm.Handle)

#If DEBUG Then
        DebugPrintLine("MagnetizeMe", Frm.Name & " is now magnetic")
#End If
    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)

        Select Case m.Msg
            Case WM_WINDOWPOSCHANGING
                CheckWindowBound(m.LParam)
        End Select

        MyBase.WndProc(m)
    End Sub

    Private Function GetShadowMargin(ByVal Edge As Integer) As Integer
        Dim PositionA, PositionB As NativeRect
        Dim nShadowMargin As Integer

        '+------------------------------------------------------------------------------+
        '|                    |   PlatformID    |   Major version   |   Minor version   |
        '+------------------------------------------------------------------------------+
        '| Windows 95         |  Win32Windows   |         4         |          0        |
        '| Windows 98         |  Win32Windows   |         4         |         10        |
        '| Windows Me         |  Win32Windows   |         4         |         90        |
        '| Windows NT 4.0     |  Win32NT        |         4         |          0        |
        '| Windows 2000       |  Win32NT        |         5         |          0        |
        '| Windows XP         |  Win32NT        |         5         |          1        |
        '| Windows 2003       |  Win32NT        |         5         |          2        |
        '| Windows Vista      |  Win32NT        |         6         |          0        |
        '| Windows 2008       |  Win32NT        |         6         |          0        |
        '| Windows 7          |  Win32NT        |         6         |          1        |
        '| Windows 2008 R2    |  Win32NT        |         6         |          1        |
        '| Windows 8          |  Win32NT        |         6         |          2        |
        '| Windows 8.1        |  Win32NT        |         6         |          3        |
        '+------------------------------------------------------------------------------+
        '| Windows 10         |  Win32NT        |        10         |          0        |
        '+------------------------------------------------------------------------------+


        If Environment.OSVersion.Version.Major > 5 Then

            If GetWindowRect(Handle, PositionA) = True Then

                If DwmGetWindowAttribute(Handle,
                         DWMWA_EXTENDED_FRAME_BOUNDS,
                         PositionB,
                         Marshal.SizeOf(PositionB)) = 0 Then


                    Select Case Edge
                        Case 0 ' Left
                            nShadowMargin = PositionB.Left - PositionA.Left
                        Case 1 ' Top
                            nShadowMargin = PositionB.Top - PositionA.Top
                        Case 2 ' Right
                            nShadowMargin = PositionA.Right - PositionB.Right
                        Case 3 ' Bottom
                            nShadowMargin = PositionA.Bottom - PositionB.Bottom
                    End Select

                End If
            End If
        Else
            nShadowMargin = 0
        End If

        Return nShadowMargin
    End Function

    Private Function GetWorkingArea(ByRef CurrentPosition As WINDOWPOS) As NativeRect
        Dim NativeWorkingArea As NativeRect
        Dim CurrentWorkingArea As Rectangle

        ' Get Current screen working area
        CurrentWorkingArea = Screen.GetWorkingArea(New Point(CurrentPosition.x, CurrentPosition.y))

        ' Convert to Native RECT
        NativeWorkingArea.Left = CurrentWorkingArea.Left
        NativeWorkingArea.Top = CurrentWorkingArea.Top
        NativeWorkingArea.Right = CurrentWorkingArea.Right
        NativeWorkingArea.Bottom = CurrentWorkingArea.Bottom

        ' Return Native RECT
        Return NativeWorkingArea
    End Function

    Private Sub CheckWindowBound(ByVal lParam As IntPtr)
        Dim WindowPosition As WINDOWPOS
        Dim WorkingArea As NativeRect

        ' Get new window position
        WindowPosition = Marshal.PtrToStructure(Of WINDOWPOS)(lParam)

        ' Check if need to process window move
        If WindowPosition.x = 0 And WindowPosition.y = 0 Then Exit Sub

        ' Get Current screen working area(based on current window position)
        WorkingArea = GetWorkingArea(WindowPosition)

        ' Check rectangle edges
        If WindowPosition.x >= (WorkingArea.Left - nMagnetDelta) And
            WindowPosition.x <= nMagnetDelta Then
            WindowPosition.x = WorkingArea.Left - GetShadowMargin(0)
        End If

        If WindowPosition.y >= (WorkingArea.Top - nMagnetDelta) And
            WindowPosition.y <= nMagnetDelta Then
            WindowPosition.y = WorkingArea.Top - GetShadowMargin(1)
        End If

        If (WindowPosition.y + WindowPosition.cy) <= (WorkingArea.Bottom + nMagnetDelta) And
            (WorkingArea.Bottom - (WindowPosition.y + WindowPosition.cy)) <= nMagnetDelta Then
            WindowPosition.y = WorkingArea.Bottom - WindowPosition.cy + GetShadowMargin(2)
        End If

        If (WindowPosition.x + WindowPosition.cx) <= (WorkingArea.Right + nMagnetDelta) And
            (WorkingArea.Right - (WindowPosition.x + WindowPosition.cx)) <= nMagnetDelta Then
            WindowPosition.x = WorkingArea.Right - WindowPosition.cx + GetShadowMargin(3)
        End If

        ' Retun edited structure
        Marshal.StructureToPtr(WindowPosition, lParam, True)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            ' Free resources
            DestroyHandle()
            disposedValue = True
        End If
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(disposing:=False)
        MyBase.Finalize()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
