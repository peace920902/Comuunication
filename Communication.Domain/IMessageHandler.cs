using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Communication.Domain.Shared;

namespace Communication.Domain
{
    public interface IMessageHandler : IMessageService<IEnumerable<Message>, Message>
    { 
        void RegisterSendMessageFunc(ChatType chat, Func<IEnumerable<Message>, Task> sendMessageFunc);
        
    }
}