Imports System.Runtime.InteropServices
Public Class InterprocessWinapi

    Public Const WM_COPYDATA As Integer = &H4A
    Public Const WM_DESTROY As Integer = &H2

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Public Shared Function GetProp(ByVal hWnd As IntPtr, ByVal lpString As String) As IntPtr
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Public Shared Function SetProp(ByVal hWnd As IntPtr, ByVal lpString As String, ByVal hData As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Public Shared Function RemoveProp(ByVal hWnd As IntPtr, ByVal lpString As String) As IntPtr
    End Function

    Public Delegate Function EnumWindowsDelegate(ByVal hWnd As IntPtr, ByVal lParam As String) As Boolean

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Public Shared Function EnumWindows(ByVal lpEnumFunc As EnumWindowsDelegate, <MarshalAs(UnmanagedType.LPStr)> ByVal lParam As String) As Boolean

    End Function

    <StructLayout(LayoutKind.Sequential)>
    Public Structure COPYDATASTRUCT
        Public dwData As IntPtr
        Public cbData As Integer
        Public lpData As IntPtr
    End Structure

End Class
