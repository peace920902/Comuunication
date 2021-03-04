using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Communication.Domain;

namespace Communication.Application.Chat
{
    public interface IChatAppService : IMessageService<MessageViewModel, Message>
    {
        void RegisterSendMessageFunc(Func<MessageViewModel, Task> sendMessageFunc);
    }
}