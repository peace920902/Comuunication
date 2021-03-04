using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Communication.Domain;
using Communication.Domain.Shared;
using Communication.Domain.Shared.Messages;
using Communication.Domain.Users;
using Microsoft.Extensions.Logging;

namespace Communication.Application.Chat
{
    public class ChatAppService : IChatAppService
    {
        private readonly ILogger<ChatAppService> _logger;
        private readonly IMessageHandler _messageHandler;
        private readonly IUserRepository _userRepository;
        private Func<MessageViewModel, Task> _sendMessageFunc;

        public ChatAppService(ILogger<ChatAppService> logger,IMessageHandler messageHandler, IUserRepository userRepository)
        {
            _logger = logger;
            _messageHandler = messageHandler;
            _userRepository = userRepository;
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
            if (_sendMessageFunc != null) await _sendMessageFunc.Invoke(messageViewModel);
        }

        public void RegisterSendMessageFunc(Func<MessageViewModel, Task> sendMessageFunc)
        {
            _sendMessageFunc = sendMessageFunc;
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

        public MessageViewModel ParseToMessageViewModel(IEnumerable<Message> messages)
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