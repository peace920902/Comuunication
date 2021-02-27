using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communication.Domain.Bots
{
    public abstract class BaseBot<TVerify, TReceivedMessage, TSendMessage > : IBotService< TVerify, TReceivedMessage, TSendMessage>
    {
        protected BotInfo BotInfo;

        public abstract bool VerifyMessage(TVerify input);
        public abstract Task OnMessageReceivedAsync(TReceivedMessage message);
        public abstract Task SendMessageAsync(IEnumerable<TSendMessage> messages);

    }
}