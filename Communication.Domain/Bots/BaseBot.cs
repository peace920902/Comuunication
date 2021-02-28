using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communication.Domain.Bots
{
    public abstract class BaseBot<TVerify, TReceivedMessage, TSendMessage > : IBotService< TVerify, TReceivedMessage, TSendMessage>
    {
        public BotInfo BotInfo { get; }
        public abstract bool VerifyMessage(TVerify input);
        public abstract Task OnMessageReceivedAsync(TReceivedMessage message);
        public abstract Task SendMessageAsync(IEnumerable<TSendMessage> messages);

        protected BaseBot()
        {
            BotInfo = new BotInfo();
        }
    }
}