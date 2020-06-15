' Follow the Wikipedia specs: https://en.wikipedia.org/wiki/M3U
Public Class WriterM3U
    Implements IPlaylistWriter

    Private disposedValue As Boolean

    Public ReadOnly Property Extension As String Implements IPlaylistWriter.Extension
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public ReadOnly Property Count As Long Implements IPlaylistWriter.Count
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public Sub Close() Implements IPlaylistWriter.Close
        Throw New NotImplementedException()
    End Sub

    Public Function Create(path As String) As Boolean Implements IPlaylistWriter.Create
        Throw New NotImplementedException()
    End Function

    Public Function WriteInformations(Index As Long, ByRef Info As StreamInformations) As Boolean Implements IPlaylistWriter.WriteInformations
        Throw New NotImplementedException()
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: eliminare lo stato gestito (oggetti gestiti)
            End If

            ' TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire l'override del finalizzatore
            ' TODO: impostare campi di grandi dimensioni su Null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: eseguire l'override del finalizzatore solo se 'Dispose(disposing As Boolean)' contiene codice per liberare risorse non gestite
    ' Protected Overrides Sub Finalize()
    '     ' Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(disposing As Boolean)'
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(disposing As Boolean)'
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
