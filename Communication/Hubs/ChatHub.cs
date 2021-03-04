using System.Collections.Generic;
using System.Threading.Tasks;
using Communication.Application;
using Communication.Application.Chat;
using Communication.Domain;
using Microsoft.AspNetCore.SignalR;

namespace Communication.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatAppService _chatAppService;

        public ChatHub(IChatAppService chatAppService)
        {
            _chatAppService = chatAppService;
            _chatAppService.RegisterSendMessageFunc(SendMessageAsync);
        }

        public async Task ReceiveMessage(MessageViewModel message)
        {
            await _chatAppService.OnMessageReceivedAsync(message);
        }

        public async Task SendMessageAsync(MessageViewModel messages)
        {
            await Clients.All.SendAsync("GetMessages", messages);
        }
    }
}