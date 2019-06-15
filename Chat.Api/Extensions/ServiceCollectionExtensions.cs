using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Chat.Api.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WebSocketConnectionManager>();

            var exportedTypes = Assembly.GetEntryAssembly()?.ExportedTypes;
            if (exportedTypes == null) 
                return services;
            foreach (var type in exportedTypes) {
                if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler)) {
                    services.AddSingleton(type);
                }
            }

            return services;
        }
    }
}
