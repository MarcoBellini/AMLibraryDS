Imports System.Runtime.InteropServices
Public Class InterprocessSender

    Private FoundWindowHandle As IntPtr

    Public Sub New()
        FoundWindowHandle = IntPtr.Zero
    End Sub

    Public Function FindWindowByChannel(ByVal Channel As String) As IntPtr
        Dim EnumProc As New InterprocessWinapi.EnumWindowsDelegate(AddressOf EnumWindowsProc)

        ' Enum all visible windows
        InterprocessWinapi.EnumWindows(EnumProc, Channel)

        Return FoundWindowHandle
    End Function

    Private Function EnumWindowsProc(ByVal Handle As IntPtr, ByVal lParam As String) As Boolean

        ' Find the Channel in all window
        If InterprocessWinapi.GetProp(Handle, lParam) <> IntPtr.Zero Then
            FoundWindowHandle = Handle

            ' Stop enum
            Return False
        End If

        Return True
    End Function

    Public Function SendData(ByVal Handle As IntPtr, ByRef src As InterprocessTransferData) As Boolean
        Dim CopyStruct As New InterprocessWinapi.COPYDATASTRUCT
        Dim CopyStructPtr As IntPtr

        If Handle = IntPtr.Zero Then Return False

        With CopyStruct
            .dwData = IntPtr.Zero
            .cbData = Marshal.SizeOf(src)
            .lpData = Marshal.AllocHGlobal(.cbData)
        End With

        ' Copy InterprocessTransferData structure to unmanaged memory
        Marshal.StructureToPtr(Of InterprocessTransferData)(src, CopyStruct.lpData, False)

        ' ' Copy InterprocessWinapi.COPYDATASTRUCT structure to unmanaged memory
        CopyStructPtr = Marshal.AllocHGlobal(Marshal.SizeOf(CopyStruct))
        Marshal.StructureToPtr(Of InterprocessWinapi.COPYDATASTRUCT)(CopyStruct, CopyStructPtr, False)

        ' Send message to main instance
        InterprocessWinapi.SendMessage(Handle,
                                       InterprocessWinapi.WM_COPYDATA,
                                       IntPtr.Zero,
                                       CopyStructPtr)

        ' Free resources
        Marshal.FreeHGlobal(CopyStruct.lpData)
        Marshal.FreeHGlobal(CopyStructPtr)

        CopyStruct = Nothing
        Return True
    End Function
End Class
