using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Api.Middleware
{
    public class WebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public WebSocket GetSocketById(string id)
            => _sockets.FirstOrDefault(p => p.Key == id).Value;

        public ConcurrentDictionary<string, WebSocket> GetAll()
            => _sockets;

        public string GetId(WebSocket socket)
            => _sockets.FirstOrDefault(p => p.Value == socket).Key;

        public List<string> GetAllKeys(string currentUser) => _sockets.Keys.Where(k => !k.Equals(currentUser)).ToList();

        public void AddSocket(WebSocket socket,string username)
            => _sockets.TryAdd(Uri.UnescapeDataString(username)?? CreateConnectionId(), socket);

        public async Task RemoveSocket(string id)
        {
            _sockets.TryRemove(id, out var socket);

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                    "Closed by the WebSocketManager",
                                    CancellationToken.None);
        }

        private static string CreateConnectionId()
            => Guid.NewGuid().ToString();
    }
}