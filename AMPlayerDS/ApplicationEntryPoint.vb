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
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)


#If DEBUG Then
        DebugPrintLine("ApplicationStarter", "AMPlayer DS: Running in DEBUG mode")
#End If

        ' The application point remain here and wait form
        ' closing
        Application.Run(New AMPlayer())

        ' Save settings
        My.Settings.Save()

    End Sub


#If DEBUG Then

    ' Print debug informations
    Public Sub DebugPrintLine(ByVal context As String, ByVal text As String)
        Debug.WriteLine(context)
        Debug.WriteLine(vbTab + text)
    End Sub

#End If


End Module
