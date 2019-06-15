using Chat.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Chat.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app, 
                                                              PathString path,
                                                              WebSocketHandler handler)
            => app.Map(path, a => a.UseMiddleware<WebSocketManagerMiddleware>(handler));
    }
}
