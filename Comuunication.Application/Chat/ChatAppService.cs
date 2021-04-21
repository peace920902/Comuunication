using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Communication.Application.Hubs;
using Communication.Domain;
using Communication.Domain.Shared;
using Communication.Domain.Shared.Messages;
using Communication.Domain.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Communication.Application.Chat
{
    public class ChatAppService : IChatAppService
    {
        private readonly ILogger<ChatAppService> _logger;
        private readonly IMessageHandler _messageHandler;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub> _hubContext;


        public ChatAppService(ILogger<ChatAppService> logger,IMessageHandler messageHandler, IUserRepository userRepository, IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _messageHandler = messageHandler;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _messageHandler.RegisterSendMessageFunc(ChatType.ChatInterface, SendMessageAsync);
        }

        public async Task OnMessageReceivedAsync(MessageViewModel messageViewModel)
        {
            var messages = await ParseToMessages(messageViewModel);
            await _messageHandler.OnMessageReceivedAsync(messages);
        }

        public async Task SendMessageAsync(IEnumerable<Message> messages)
        {
            var messageViewModel = ParseToMessageViewModel(messages);
            await _hubContext.Clients.All.SendAsync("GetMessages", messageViewModel);
        }

        private async Task<IEnumerable<Message>> ParseToMessages(MessageViewModel messageViewModel)
        {
            var messages = new List<Message>();
            foreach (var message in messageViewModel.Messages)
            {
                var user = await _userRepository.FindAsync(message.UserId);
                if (user == null)
                {
                    _logger.Log(LogLevel.Error, $"Cannot find user. userId : {message.UserId}, Message: {JsonSerializer.Serialize(message)}");
                    continue;
                }
                messages.Add(new Message
                {
                    BotId = message.BotId,
                    Destination = message.ChatType,
                    MessageType = message.MessageType,
                    TimeStamp = messageViewModel.SendTime,
                    Content = message.Content,
                    User = user
                });
            }
            return messages;
        }

        private MessageViewModel ParseToMessageViewModel(IEnumerable<Message> messages)
        {
            return new ()
            {
                Messages = messages.Select(x => new MessageDto
                {
                    BotId = x.BotId,
                    Content = x.Content,
                    ChatType = x.User.ChatType,
                    MessageType = x.MessageType,
                    UserId = x.User.Id,
                    UserName = x.User.Name
                }),
                SendTime = DateTimeOffset.Now.ToUnixTimeSeconds()
            };
        }
    }
}