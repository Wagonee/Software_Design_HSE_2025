using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace OrdersService;

public class SocketManager
{
    private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public void AddSocket(string userId, WebSocket socket)
    {
        _sockets.AddOrUpdate(userId, socket, (_, oldSocket) =>
        {
            try
            {
                oldSocket.Dispose();
            }
            catch
            {
                // ignored
            }

            return socket;
        });

        _locks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));
    }

    public async Task<bool> SendMessage(string userId, string message)
    {
        if (_sockets.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
        {
            var sem = _locks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));
            await sem.WaitAsync();
            try
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            finally
            {
                sem.Release();
            }
        }

        return false;
    }
}