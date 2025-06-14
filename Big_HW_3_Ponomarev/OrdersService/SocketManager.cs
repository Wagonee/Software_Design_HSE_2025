using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace OrdersService;

public class SocketManager
{
    private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    public void AddSocket(string userId, WebSocket socket)
    {
        _sockets.TryAdd(userId, socket);
    }

    public async Task SendMessage(string userId, string message)
    {
        if (_sockets.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}