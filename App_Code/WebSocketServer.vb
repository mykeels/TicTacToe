Imports System.Threading
Imports System.Threading.Thread
Imports Fleck
Public Class WebSocketServer
    Private socket_count As Integer = 0
    Private currentsockets As New List(Of Fleck.WebSocketConnection)
    Public Sub New(Optional index As Integer = 0)
        socket_count = index
        CreateSocket()
    End Sub

    Public Sub CreateSocket()
        FleckLog.Level = LogLevel.Info
        Dim server = New Fleck.WebSocketServer("ws://198.37.116.19:23222") 'ws://127.0.0.1:23222
        server.Start(Sub(socket As Fleck.WebSocketConnection)
                         socket.OnOpen = Sub() Socket_OnOpen(socket)
                         socket.OnClose = Sub() Socket_OnClose(socket)
                         socket.OnMessage = Sub(message As String)
                                                Console.WriteLine(message)
                                                TicTacToeMessage.HandleMessage(message, socket)
                                            End Sub
                     End Sub)
    End Sub

    Public Sub Socket_OnOpen(ByVal socket As Fleck.WebSocketConnection)
        socket_count += 1
        Console.WriteLine("Socket Connection " & socket_count & " Opened")
        'socket.Send("Man, You Suck!")
        'currentsockets.Add(socket)
    End Sub

    Public Sub Socket_OnClose(ByVal socket As Fleck.WebSocketConnection)
        'currentsockets.Remove(socket)
        'socket.Send("Man, You Suck!")
        Dim r As Relay = Relay.GetRelay(relays, socket)
        Dim rm = New TicTacToeMessage.ResponseMessage()
        rm.message_type = TicTacToeMessage.ResponseMessage.MessageType.LeaveGame
        rm.message = "Connection Terminated"
        r.AddMessage(rm, socket)
        relays.Remove(r)
        Console.WriteLine("Socket Connection " & socket_count & " Closed")
    End Sub

    Public Sub Socket_OnMessage(ByVal message As String)
        Console.WriteLine("Socket " & socket_count & ": " & message)
        'socket.Send("Man, You Suck!")
        '
    End Sub

End Class