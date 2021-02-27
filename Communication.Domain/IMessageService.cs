using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communication.Domain
{
    public interface IMessageService<in TReceivedMessage, in TSendMessage>
    {
        public Task OnMessageReceivedAsync(TReceivedMessage message);
        public Task SendMessageAsync(IEnumerable<TSendMessage> messages);
    }
}