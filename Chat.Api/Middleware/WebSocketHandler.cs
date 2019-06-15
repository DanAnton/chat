using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Api.Middleware
{
    public abstract class WebSocketHandler
    {
        protected WebSocketConnectionManager WebSocketConnectionManager { get; set; }

        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager) 
            => WebSocketConnectionManager = webSocketConnectionManager;

        public virtual async Task OnConnected(WebSocket socket, string username) 
            => WebSocketConnectionManager.AddSocket(socket,username);

        public virtual async Task OnDisconnected(WebSocket socket)
            => await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if(socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message),
                                                                  0, 
                                                                  message.Length),
                                   WebSocketMessageType.Text,
                                   true,
                                   CancellationToken.None);          
        }

        public async Task SendMessageAsync(string socketId, string message) 
            => await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), message);

        public async Task SendMessageToAllAsync(string message)
        {
            foreach(var pair in WebSocketConnectionManager.GetAll())
            {
                if(pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }

        public List<string> GetAllConnected(string currentUser) 
            => WebSocketConnectionManager.GetAll()
                                         .Where(u => u.Value.State == WebSocketState.Open && !u.Key.Equals(currentUser))
                                         .Select(u => u.Key)
                                         .ToList();

        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}