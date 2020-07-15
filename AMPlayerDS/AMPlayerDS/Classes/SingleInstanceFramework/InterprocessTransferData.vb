Imports System.Runtime.InteropServices

Public Enum InterprocessActions
    AddToPlaylistAndPlayItem = 0
    AddToPlaylist = 1
    AddFolderToPlaylist = 2
    Transcode = 3
End Enum

<StructLayout(LayoutKind.Sequential)>
Public Structure InterprocessTransferData
    <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=255)>
    Public Path As String
    Public Action As InterprocessActions
End Structure
