Public Class WebSocket
    Public Property name As String
    Public Property current_socket As Fleck.WebSocketConnection = Nothing

    Public Sub New(socket As Fleck.WebSocketConnection, Optional name As String = Nothing)
        Me.current_socket = socket
        If Not name Is Nothing Then
            name = GetRandomString(20)
        End If
    End Sub
End Class
