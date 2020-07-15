''' <summary>
''' This is the Entry Point of AMPlayer program
'''  
''' This allow to Run application in MTAThread instead of
''' STAThread select by VB.NET by Default
''' 
''' In DEBUG mode show console application to easy debug app
''' 
''' In RELEASE mode the console window is hidden
''' </summary>
Module ApplicationEntryPoint

    ' We need to run application in multithreaded apartment
    ' oterwise the MediaFoundation and DirectSound doesn't 
    ' workcorrecty with multi threaded COM calls
    <MTAThread> ' NB. This app works only in MTAThread, don't change!
    Public Sub Main(ByVal params() As String)
        If FoundOpenInstance(params) = False Then
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)



#If DEBUG Then
            DebugPrintLine("ApplicationStarter", "AMPlayer DS: Running in DEBUG mode")
#End If

            ' The application point remain here and wait form
            ' closing
            Application.Run(New AMPlayer(params))

            ' Save settings
            My.Settings.Save()
        Else

#If DEBUG Then
            DebugPrintLine("ApplicationStarter", "AMPlayer DS: Found already open instance")
#End If
        End If


    End Sub

    Private Function FoundOpenInstance(ByRef params() As String) As Boolean
        Dim Sender As New InterprocessSender
        Dim Handle As IntPtr
        Dim Data As New InterprocessTransferData

        ' Check if there is an open instance
        Handle = Sender.FindWindowByChannel(AMPlayerChannel)

        If Handle <> IntPtr.Zero Then

            If params.Length > 0 Then
                For i As Integer = 0 To params.Length - 1

                    ' Send data only if param is a valid file
                    If My.Computer.FileSystem.FileExists(params(i)) = True Then

                        ' If is the first path add and play, otherwise add to playlist
                        If i = 0 Then
                            Data.Action = InterprocessActions.AddToPlaylistAndPlayItem
                        Else
                            Data.Action = InterprocessActions.AddToPlaylist
                        End If


                        ' Path
                        Data.Path = params(i)

                        ' Send data to the opened instance
                        Sender.SendData(Handle, Data)
                    End If


                Next
            End If


            Return True
        End If


        Return False
    End Function


#If DEBUG Then

    ' Print debug informations
    Public Sub DebugPrintLine(ByVal context As String, ByVal text As String)
        Debug.WriteLine(context)
        Debug.WriteLine(vbTab + text)
    End Sub

#End If


End Module
