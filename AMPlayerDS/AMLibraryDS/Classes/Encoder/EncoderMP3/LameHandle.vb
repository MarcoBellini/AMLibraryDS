Imports Microsoft.Win32.SafeHandles
Public Class LameHandle
    Inherits SafeHandleZeroOrMinusOneIsInvalid

    Public Sub New()
        MyBase.New(True)
    End Sub

    Public Sub New(ByVal device As IntPtr)
        MyBase.New(True)
        handle = device
    End Sub

    Protected Overrides Function ReleaseHandle() As Boolean
        If handle <> IntPtr.Zero Then

#If DEBUG Then
            DebugPrintLine("LameHandle", "Handle correctly Closed")
#End If

            Return Lame.lame_close(handle) = 0  ' Free lame resources
        End If

        Return False
    End Function
End Class
