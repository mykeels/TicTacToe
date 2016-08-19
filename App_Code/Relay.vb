Public Class Relay
    Private name As String = "myrelay"
    Private maxsize As Integer = 2
    Private sockets As New List(Of WebSocket)
    Private messages As New List(Of Message)
    Public Sub New(size As Integer, Optional name As String = "myrelay")
        Me.name = name
        Me.maxsize = size
    End Sub

    Public Function Count() As Integer
        Return sockets.Count
    End Function

    Public Function IsFull() As Boolean
        If Count() < maxsize Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Function ContainsSocket(socket As Fleck.WebSocketConnection) As Boolean
        For index = 0 To sockets.Count - 1
            Dim s As Fleck.WebSocketConnection = sockets(index).current_socket
            If s.ConnectionInfo.Id.Equals(socket.ConnectionInfo.Id) Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function AddSocket(socket As Fleck.WebSocketConnection)
        If (sockets.Count() + 1) <= maxsize Then
            Dim socketname As String = GetRandomString(16)
            sockets.Add(New WebSocket(socket, socketname))
            socket.OnMessage = Sub(message As String)
                                   AddMessage(message, socket.ConnectionInfo.Id)
                               End Sub
            Dim msg As Object = TicTacToeMessage.GetResponseMessage(TicTacToeMessage.ResponseMessage.MessageType.JoinGame)
            AddMessage(Newtonsoft.Json.JsonConvert.SerializeObject(msg), socket.ConnectionInfo.Id)
            Return True
        Else
            Return False
        End If
    End Function

    Public Function RemoveSocket(socket As Fleck.WebSocketConnection)
        Dim leavingsocket As WebSocket = (From pp In sockets Where pp.current_socket.Equals(socket)).FirstOrDefault()
        Dim msg = New TicTacToeMessage.ResponseMessage()
        msg.message_type = TicTacToeMessage.ResponseMessage.MessageType.LeaveGame
        msg.message = "leaving"
        If Not leavingsocket Is Nothing Then
            AddMessage(msg.ToJson(), socket.ConnectionInfo.Id)
            sockets.Remove(leavingsocket)
        End If
        Return True
    End Function

    Private Sub AddMessage(message As String, sender As Guid)
        Dim m As New Message()
        m.dates = DateTime.Now
        m.id = messages.Count() + 1
        m.sender = sender.ToString()
        For Each socket In (From pp In sockets Where pp.current_socket.ConnectionInfo.Id <> sender)
            socket.current_socket.Send(message)
        Next
    End Sub

    Public Sub AddMessage(message As TicTacToeMessage.ResponseMessage, socket As Fleck.WebSocketConnection)
        Dim leavingsocket As WebSocket = (From pp In sockets Where pp.current_socket.ConnectionInfo.Id.Equals(socket.ConnectionInfo.Id)).FirstOrDefault()
        For Each s In (From pp In sockets Where Not pp.current_socket.ConnectionInfo.Id.Equals(socket.ConnectionInfo.Id))
            s.current_socket.Send(message.ToJson())
        Next
    End Sub

    Public Shared Function GetRelay(relays As List(Of Relay), socket As Fleck.WebSocketConnection) As Relay
        Dim relay As Relay = (From pp In relays Where pp.ContainsSocket(socket)).FirstOrDefault()
        If relay Is Nothing Then
            relay = (From pp In relays Where Not pp.IsFull()).FirstOrDefault()
            If relay Is Nothing Then
                relays.Add(New Relay(2))
                relay = relays.Last()
            End If
        End If
        Return relay
    End Function
End Class
