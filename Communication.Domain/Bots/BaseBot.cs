using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communication.Domain.Bots
{
    public abstract class BaseBot<TVerify,  TSendMessage, TMessage> : IBotService< TVerify,  TSendMessage,  TMessage>
    {
        protected BotInfo BotInfo;

        public abstract Task ReceiveMessages(Func<TMessage, Task> handleMessageFunc);

        public abstract bool VerifyMessage(TVerify input);

        public abstract Task<bool> SendMessage(IEnumerable<TSendMessage> messages);
    }
}