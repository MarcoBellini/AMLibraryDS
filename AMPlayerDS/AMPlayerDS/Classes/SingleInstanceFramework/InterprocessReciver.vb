Imports System.Runtime.InteropServices

Public Class InterprocessReciver
    Inherits NativeWindow
    Implements IDisposable

    Private CurrentChannel As String
    Private disposedValue As Boolean

    Public Event RecivedDataEvent(ByRef data As InterprocessTransferData)

    Public Sub New()
        CurrentChannel = ""

#If DEBUG Then
        DebugPrintLine("SingleInstanceFramework", "Create new class (1)")
#End If
    End Sub

    Public Sub New(ByVal handle As IntPtr)

        CurrentChannel = ""

        ' Try to associate new window handle
        If AssociateWindow(handle) = False Then
            Throw New Exception("Cannot Initializate subclassing")
        End If

#If DEBUG Then
        DebugPrintLine("SingleInstanceFramework", "Create new class (2)")
#End If
    End Sub

    Public Sub New(ByVal handle As IntPtr, ByVal Channel As String)

        ' Try to associate new window handle
        ' and assign a channel
        If AssociateWindow(handle) = False Then
            Throw New Exception("Cannot Initializate subclassing")
        End If

        If AddChannel(Channel) = False Then
            Throw New Exception("Cannot Add Prop")
        End If

#If DEBUG Then
        DebugPrintLine("SingleInstanceFramework", "Create new class (3)")
#End If
    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)

        Select Case m.Msg
            Case InterprocessWinapi.WM_COPYDATA
                RecivedData(m.LParam)
            Case InterprocessWinapi.WM_DESTROY
                RemoveChannel()
        End Select

        MyBase.WndProc(m)
    End Sub


    Public Function AssociateWindow(ByVal handle As IntPtr) As Boolean
        If handle = IntPtr.Zero Then Return False

        ' Start subclassing
        AssignHandle(handle)

#If DEBUG Then
        DebugPrintLine("SingleInstanceFramework", "Handle assigned: " & handle.ToString)
#End If

        Return True
    End Function

    Public Function AddChannel(ByVal Channel As String) As Boolean
        If Handle = IntPtr.Zero Then Return False
        If Channel = "" Then Return False

        CurrentChannel = Channel

        Return InterprocessWinapi.SetProp(Handle, Channel, Handle)
    End Function

    Public Function HasChannel(ByVal Channel As String) As Boolean
        If Handle = IntPtr.Zero Then Return False
        If Channel = "" Then Return False

        Return InterprocessWinapi.GetProp(Handle, Channel) <> IntPtr.Zero
    End Function
    Public Function RemoveChannel() As Boolean
        If Handle = IntPtr.Zero Then Return False

        Return InterprocessWinapi.RemoveProp(Handle, CurrentChannel) <> IntPtr.Zero
    End Function

    Private Sub RecivedData(ByVal data As IntPtr)
        Dim CopyDataStruct As InterprocessWinapi.COPYDATASTRUCT
        Dim TransferData As InterprocessTransferData

        ' Convert recived data to structures
        CopyDataStruct = Marshal.PtrToStructure(Of InterprocessWinapi.COPYDATASTRUCT)(data)
        TransferData = Marshal.PtrToStructure(Of InterprocessTransferData)(CopyDataStruct.lpData)

        RaiseEvent RecivedDataEvent(TransferData)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            MyBase.DestroyHandle()

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
