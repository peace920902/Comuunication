﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Communication.Domain.Infrastructure;
using Communication.Domain.Shared;
using Communication.Domain.Shared.Messages;
using Communication.Domain.Users;
using Microsoft.Extensions.Logging;

namespace Communication.Domain
{
    public class MessageHandler : IMessageHandler
    {
        private readonly Dictionary<ChatType, Func<IEnumerable<Message>, Task>> _sendFuncDict;
        private readonly ILogger<MessageHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IGuidFactory _guidFactory;


        public MessageHandler(ILogger<MessageHandler> logger, IUserRepository userRepository, IGuidFactory guidFactory)
        {
            _sendFuncDict = new Dictionary<ChatType, Func<IEnumerable<Message>, Task>>();
            _logger = logger;
            _userRepository = userRepository;
            _guidFactory = guidFactory;
        }

        public void RegisterSendMessageFunc(ChatType chat, Func<IEnumerable<Message>, Task> sendMessageFunc)
        {
            if (!_sendFuncDict.ContainsKey(chat)) _sendFuncDict.Add(chat, sendMessageFunc);
            else _sendFuncDict[chat] = sendMessageFunc;
        }


        public async Task OnMessageReceivedAsync(IEnumerable<Message> messages)
        {
            var sendDict = new Dictionary<ChatType, List<Message>>();
            var tasks = new List<Task>();
            foreach (var message in messages)
            {
                switch (message.MessageType)
                {
                    case MessageType.Join:
                        CreateUser(tasks, message);
                        continue;
                    case MessageType.Text:
                        await SaveMessage(message, tasks, sendDict);
                        continue;
                }
            }

            foreach (var (chatType, sendMessages) in sendDict)
            {
                if (!_sendFuncDict.ContainsKey(chatType))
                {
                    _logger.Log(LogLevel.Error, $"ChatType: {chatType} is not registered");
                    continue;
                }
                tasks.Add(_sendFuncDict[chatType].Invoke(sendMessages));
            }

            await Task.WhenAll(tasks);
        }

        private async Task SaveMessage(Message message, ICollection<Task> tasks, IDictionary<ChatType, List<Message>> sendDict)
        {
            var user = await _userRepository.GetUserByThirdPartyUserId(message.User.ThirdPartyId);
            if (user == null)
            {
                CreateUser(tasks, message);
            }

            message.User = user;
            var destination = message.Destination;
            if (sendDict.ContainsKey(destination))
                sendDict[destination].Add(message);
            else
                sendDict.Add(destination, new List<Message> {message});
        }

        private void CreateUser(ICollection<Task> tasks, Message message)
        {
            tasks.Add(_userRepository.CreateAsync(
                new User
                {
                    Id = _guidFactory.CreateId(),
                    Name = message.User.Name,
                    ThirdPartyId = message.User.ThirdPartyId,
                    ChatType = message.User.ChatType
                }));
        }

        public async Task SendMessageAsync(IEnumerable<Message> messages)
        {
            throw new NotImplementedException();
        }
    }
}