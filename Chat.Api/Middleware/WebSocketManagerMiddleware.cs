using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Chat.Api.Middleware
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketHandler _webSocketHandler;

        public WebSocketManagerMiddleware(RequestDelegate next,
                                          WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;
            var username = context.Request.QueryString.Value.Split("=")[1];
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await _webSocketHandler.OnConnected(socket,username);

            await Receive(socket, async (result, buffer) =>
                                  {
                                      switch (result.MessageType) {
                                          case WebSocketMessageType.Text:
                                              await _webSocketHandler.ReceiveAsync(socket, result, buffer);
                                              break;
                                          case WebSocketMessageType.Close:
                                              await _webSocketHandler.OnDisconnected(socket);
                                              break;
                                      }
                                  });

            await _next.Invoke(context);
        }

        private static async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open) {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer),
                                                       CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}