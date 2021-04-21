using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Communication.Domain.Bots;
using Communication.Domain.Line;

namespace Communication.Domain
{
    public abstract class BaseThirdPartyService<TParsedMessage, TReceivedMessage, TSendMessage, TVerifyObject, TBotReceivedMessage, TBotSendMessage, TBot, TBotManger> : IMessageService<TReceivedMessage, TSendMessage>
        where TBot : BaseBot<TVerifyObject, TBotReceivedMessage, TBotSendMessage>
        where TBotManger : IBotManager<TBot, TVerifyObject, TBotReceivedMessage, TBotSendMessage>
    {
        protected readonly TBotManger BotManager;

        protected abstract Task<IEnumerable<Message>> ParseMessages(TParsedMessage message);
        public abstract Task OnMessageReceivedAsync(TReceivedMessage message);
        public abstract Task SendMessageAsync(IEnumerable<TSendMessage> messages);

        protected BaseThirdPartyService(TBotManger botManager)
        {
            BotManager = botManager;
        }
    }
}