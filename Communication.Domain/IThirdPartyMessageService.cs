using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Communication.Domain.Bots;
using Communication.Domain.Line;

namespace Communication.Domain
{
    public abstract class BaseThirdPartyService<TParsedMessage, TReceivedMessage, TSendMessage, TVerifyObject, TBotReceivedMessage, TBotSendMessage> : IMessageService<TReceivedMessage, TSendMessage> 
    {
        protected ConcurrentDictionary<string, IBotService<TVerifyObject, TBotReceivedMessage, TBotSendMessage>> BotServices;
        
        protected abstract Task<IEnumerable<Message>> ParseMessages(TParsedMessage message);
        public abstract Task OnMessageReceivedAsync(TReceivedMessage message);
        public abstract Task SendMessageAsync(IEnumerable<TSendMessage> messages);

        protected BaseThirdPartyService()
        {
            BotServices = new ConcurrentDictionary<string, IBotService<TVerifyObject, TBotReceivedMessage, TBotSendMessage>>();
        }
    }
}