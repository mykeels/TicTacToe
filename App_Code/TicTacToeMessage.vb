Imports Newtonsoft.Json.Linq


Public Class TicTacToeMessage
    Class Message
        Property message As Object
    End Class
    Class RequestMessage
        Inherits Message
        Enum MessageType
            CreateGame
            JoinGame
            LeaveGame
            NewGame
            MakePlay
        End Enum
        Property message_type As MessageType
    End Class
    Class ResponseMessage
        Inherits Message
        Enum MessageType
            CreateGame
            JoinGame
            LeaveGame
            NewGame
            MakePlay
        End Enum
        Property message_type As MessageType
    End Class

    Public Shared Function GetResponseMessage(type As ResponseMessage.MessageType, Optional msg As String = Nothing)
        Dim r As New ResponseMessage()
        r.message_type = type
        If Not msg Is Nothing Then
            r.message = msg
        End If
        Return r
    End Function

    Private Shared game_size As Integer = 0
    Public Shared Sub HandleMessage(message As String, socket As Fleck.WebSocketConnection)
        Dim m As RequestMessage = Newtonsoft.Json.JsonConvert.DeserializeObject(message).ToObject(Of TicTacToeMessage.RequestMessage)()
        Dim relay As Relay = Relay.GetRelay(relays, socket)
        Select Case m.message_type
            Case RequestMessage.MessageType.CreateGame

                Dim bool = relay.AddSocket(socket)
                Dim msg = New TicTacToeMessage.ResponseMessage()
                If relay.IsFull() Then
                    msg.message = game_size
                    msg.message_type = TicTacToeMessage.ResponseMessage.MessageType.JoinGame
                Else
                    game_size = m.message
                    msg.message = bool
                    msg.message_type = TicTacToeMessage.ResponseMessage.MessageType.CreateGame
                End If
                socket.Send(msg.ToJson())
            Case RequestMessage.MessageType.JoinGame
                Dim bool = relay.AddSocket(socket)
                Dim msg = New TicTacToeMessage.ResponseMessage()
                msg.message_type = TicTacToeMessage.ResponseMessage.MessageType.JoinGame
                msg.message = game_size
                socket.Send(msg.ToJson())
            Case RequestMessage.MessageType.LeaveGame
                relay.RemoveSocket(socket)
            Case RequestMessage.MessageType.MakePlay
                relay.AddMessage(GetResponseMessage(ResponseMessage.MessageType.MakePlay, message), socket)
            Case RequestMessage.MessageType.NewGame
                relay.AddMessage(GetResponseMessage(ResponseMessage.MessageType.NewGame, message), socket)
            Case Else
        End Select
    End Sub
End Class