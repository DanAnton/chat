using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Chat.Api.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Controllers
{
    [Route("/api/chat")]
    public class MessagesController : Controller
    {
        private readonly NotificationsMessageHandler _notificationsMessageHandler;

        public MessagesController(NotificationsMessageHandler notificationsMessageHandler)
            => _notificationsMessageHandler = notificationsMessageHandler;

        [HttpGet("getAllConnected")]
        public List<string> GetAllConnected([FromQuery] string username) => _notificationsMessageHandler.GetAllConnected(username);

        [HttpGet]
        public async Task SendMessage([FromQuery] string message, [FromQuery] string username)
        {
            var newMessage = $"{username} said: {message}";
            await _notificationsMessageHandler.SendMessageToAllAsync(newMessage);
        }

        [HttpGet("receiver")]
        public async Task SendMessageToOne([FromQuery] string message,
                                           [FromQuery] string username,
                                           [FromQuery] string receiver)
        {
            var newMessage = $"{username} said to {receiver}: {message}";
            await _notificationsMessageHandler.SendMessageAsync(username, newMessage);
            await _notificationsMessageHandler.SendMessageAsync(receiver, newMessage);
        }
    }
}