using System;
using System.Text.Json;
using System.Threading.Tasks;
using Communication.Application.Chat;
using Microsoft.AspNetCore.SignalR;

namespace Communication.Application.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatAppService _chatAppService;
        public ChatHub(IChatAppService chatAppService)
        {
            _chatAppService = chatAppService;
        }

        
        [HubMethodName("SendMessages")]
        public async Task ReceiveMessage(MessageViewModel message)
        {
            await _chatAppService.OnMessageReceivedAsync(message);
        }
    }
}