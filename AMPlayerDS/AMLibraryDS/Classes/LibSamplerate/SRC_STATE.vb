Imports System.Runtime.InteropServices
Imports Microsoft.Win32.SafeHandles

''' <summary>
''' Pointer to Samplerate handle
''' </summary>
Public Class SRC_STATE
    Inherits SafeHandleZeroOrMinusOneIsInvalid

    Public Sub New()
        MyBase.New(True)
    End Sub

    Public Overrides ReadOnly Property IsInvalid As Boolean
        Get
            If handle = IntPtr.Zero Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Protected Overrides Function ReleaseHandle() As Boolean
        LibSamplerate.src_delete(handle)
        Return True
    End Function
End Class