Imports System.Runtime.InteropServices

Public Class TaskbarButtons
    Inherits NativeWindow
    Implements IDisposable

#Region "Windows API"
    Private Const WM_COMMAND As Integer = &H111
    Private Const THBN_CLICKED As Integer = &H1800

    Private ReadOnly CLSID_TASKBARLIST As New Guid("56FDF344-FD6D-11d0-958A-006097C9A090")


    <Flags()>
    Private Enum THUMBBUTTONMASK
        THB_BITMAP = &H1      ' The iBitmap member contains valid information.
        THB_ICON = &H2        ' The hIcon member contains valid information.
        THB_TOOLTIP = &H4     ' The szTip member contains valid information.
        THB_FLAGS = &H8       ' The dwFlags member contains valid information.
    End Enum

    <Flags>
    Private Enum THUMBBUTTONFLAGS
        THBF_ENABLED = &H0          ' The button is active and available to the user.
        THBF_DISABLED = &H1         ' The button is disabled. It is present, but has a visual state that indicates that it will not respond to user action.
        THBF_DISMISSONCLICK = &H2   ' When the button is clicked, the taskbar button's flyout closes immediately.
        THBF_NOBACKGROUND = &H4     ' Do not draw a button border, use only the image.
        THBF_HIDDEN = &H8           ' The button is not shown to the user
        THBF_NONINTERACTIVE = &H10  ' The button is enabled but not interactive; no pressed button state is drawn
    End Enum

    Private Enum TBPFLAG
        TBPF_NOPROGRESS = 0
        TBPF_INDETERMINATE = &H1
        TBPF_NORMAL = &H2
        TBPF_ERROR = &H4
        TBPF_PAUSED = &H8
    End Enum

    Private Enum STPFLAG
        STPF_NONE = &H0
        STPF_USEAPPTHUMBNAILALWAYS = &H1
        STPF_USEAPPTHUMBNAILWHENACTIVE = &H2
        STPF_USEAPPPEEKALWAYS = &H4
        STPF_USEAPPPEEKWHENACTIVE = &H8
    End Enum


    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Private Structure THUMBBUTTON
        <MarshalAs(UnmanagedType.U4)>
        Public dwMask As THUMBBUTTONMASK ' A combination of THUMBBUTTONMASK values that specify which members of this structure contain valid data
        Public iId As UInt32             ' The application-defined identifier of the button, unique within the toolbar.
        Public iBitmap As UInt32         ' The zero-based index of the button image within the image list set through ITaskbarList3::ThumbBarSetImageList.
        Public hIcon As IntPtr           ' The handle of an icon to use as the button image.
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)>
        Public szTip As String           ' A wide character array that contains the text of the button's tooltip, displayed when the mouse pointer hovers over the button.
        <MarshalAs(UnmanagedType.U4)>
        Public dwFlags As THUMBBUTTONFLAGS ' A combination of THUMBBUTTONMASK values that specify which members of this structure contain valid data
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure NativeRect
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure


    <ComImportAttribute()>
    <GuidAttribute("c43dc798-95d1-4bea-9030-bb99e2983a1a")>
    <InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)>
    Private Interface ITaskbarList4
        <PreserveSig>
        Sub HrInit()
        <PreserveSig>
        Sub AddTab(ByVal hwnd As IntPtr)
        <PreserveSig>
        Sub DeleteTab(ByVal hwnd As IntPtr)
        <PreserveSig>
        Sub ActivateTab(ByVal hwnd As IntPtr)
        <PreserveSig>
        Sub SetActiveAlt(ByVal hwnd As IntPtr)
        <PreserveSig>
        Sub MarkFullscreenWindow(ByVal hwnd As IntPtr,
        <MarshalAs(UnmanagedType.Bool)> ByVal fFullscreen As Boolean)
        <PreserveSig>
        Sub SetProgressValue(ByVal hwnd As IntPtr, ByVal ullCompleted As UInt64, ByVal ullTotal As UInt64)
        <PreserveSig>
        Sub SetProgressState(ByVal hwnd As IntPtr, ByVal tbpFlags As THUMBBUTTONFLAGS)
        <PreserveSig>
        Sub RegisterTab(ByVal hwndTab As IntPtr, ByVal hwndMDI As IntPtr)
        <PreserveSig>
        Sub UnregisterTab(ByVal hwndTab As IntPtr)
        <PreserveSig>
        Sub SetTabOrder(ByVal hwndTab As IntPtr, ByVal hwndInsertBefore As IntPtr)
        <PreserveSig>
        Sub SetTabActive(ByVal hwndTab As IntPtr, ByVal hwndMDI As IntPtr, ByVal dwReserved As UInteger)
        <PreserveSig>
        Function ThumbBarAddButtons(ByVal hwnd As IntPtr, ByVal cButtons As UInteger,
        <MarshalAs(UnmanagedType.LPArray)> ByVal pButtons() As THUMBBUTTON) As Integer
        <PreserveSig>
        Function ThumbBarUpdateButtons(ByVal hwnd As IntPtr, ByVal cButtons As UInteger,
        <MarshalAs(UnmanagedType.LPArray)> ByVal pButtons() As THUMBBUTTON) As Integer
        <PreserveSig>
        Sub ThumbBarSetImageList(ByVal hwnd As IntPtr, ByVal himl As IntPtr)
        <PreserveSig>
        Sub SetOverlayIcon(ByVal hwnd As IntPtr, ByVal hIcon As IntPtr,
        <MarshalAs(UnmanagedType.LPWStr)> ByVal pszDescription As String)
        <PreserveSig>
        Sub SetThumbnailTooltip(ByVal hwnd As IntPtr,
        <MarshalAs(UnmanagedType.LPWStr)> ByVal pszTip As String)
        <PreserveSig>
        Sub SetThumbnailClip(ByVal hwnd As IntPtr, ByRef prcClip As NativeRect)
        Sub SetTabProperties(ByVal hwndTab As IntPtr, ByVal stpFlags As STPFLAG)
    End Interface

    Private Function HIWORD(ByRef value As UInteger) As UInteger
        Return value >> 16
    End Function

    Private Function LOWORD(ByRef value As UInteger) As UInteger
        Return value And &HFFFF
    End Function


#End Region

    ''' <summary>
    ''' Used to address the Click of the button to a custom Function
    ''' </summary>
    Public Delegate Sub ButtonClick()


    ' Private vars
    Private ButtonsList As New List(Of ButtonEntry)
    Private ThubButtonList As New List(Of THUMBBUTTON)

    Private ButtonIDCounter As UInteger
    Private ITaskbar As ITaskbarList4 = Nothing

    Private disposedValue As Boolean

    ' Used internally to manage Delegates
    Private Structure ButtonEntry
        Public ButtonID As UInteger
        Public ButtonText As String
        Public ButtonImage As Icon
        Public ButtonDelegate As ButtonClick
    End Structure

    ''' <summary>
    ''' Add the Handle of the main window
    ''' </summary>
    ''' <param name="hWnd">Handle</param>
    Public Sub New(ByVal hWnd As IntPtr)
        AssignHandle(hWnd)
        ButtonIDCounter = 100
    End Sub

    ''' <summary>
    ''' Add new Thumbnail button
    ''' </summary>
    ''' <param name="ButtonText"></param>
    ''' <param name="ButtonImage"></param>
    ''' <param name="ButtonDelegate"></param>
    Public Sub AddButton(ByRef ButtonText As String, ByRef ButtonImage As Icon, ByRef ButtonDelegate As ButtonClick)
        Dim CurrentButtonEntry As New ButtonEntry
        Dim CurrentButtonThumb As New THUMBBUTTON

        ' Fill the Managed structure with data
        With CurrentButtonEntry
            .ButtonID = ButtonIDCounter
            .ButtonText = ButtonText
            .ButtonImage = ButtonImage
            .ButtonDelegate = ButtonDelegate
        End With

        ' Fill unmanaged structure with tooltip, icon and ID
        With CurrentButtonThumb
            .dwFlags = THUMBBUTTONFLAGS.THBF_ENABLED
            .dwMask = THUMBBUTTONMASK.THB_ICON Or
                     THUMBBUTTONMASK.THB_FLAGS Or
                     THUMBBUTTONMASK.THB_TOOLTIP
            .iBitmap = 0
            .hIcon = ButtonImage.Handle
            .szTip = ButtonText
            .iId = ButtonIDCounter
        End With

        ' Update lists
        ButtonsList.Add(CurrentButtonEntry)
        ThubButtonList.Add(CurrentButtonThumb)

        ' Increment ID Counter
        ButtonIDCounter += 1
    End Sub

    ''' <summary>
    ''' Call after 
    ''' </summary>
    Public Sub UpdateTaskbar()
        Dim result As Integer

        ' Try to create new instance of ITaskbarList4 interface
        If ITaskbar Is Nothing Then
            ITaskbar = CType(Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_TASKBARLIST)), ITaskbarList4)
            ITaskbar.HrInit()
        End If

        ' Add buttons
        result = ITaskbar.ThumbBarAddButtons(Handle, ThubButtonList.Count, ThubButtonList.ToArray)

        ' Check errors
        If result <> 0 Then
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error())
        End If
    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)
        Select Case m.Msg
            Case WM_COMMAND
                If HIWORD(m.WParam) = THBN_CLICKED Then

                    ' Scan all buttons and call the proper delegate
                    For i As Integer = 0 To ButtonsList.Count - 1
                        If ButtonsList(i).ButtonID = LOWORD(m.WParam) Then
                            ButtonsList(i).ButtonDelegate()
                        End If
                    Next

                End If
        End Select

        MyBase.WndProc(m)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then

            ' Release resource
            If ITaskbar IsNot Nothing Then
                Marshal.ReleaseComObject(ITaskbar)
                ITaskbar = Nothing
            End If

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
