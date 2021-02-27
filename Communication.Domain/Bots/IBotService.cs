using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communication.Domain.Bots
{
    public interface IBotService<in TVerify, in TReceivedMessage, in TSendMessage> : IMessageService<TReceivedMessage,TSendMessage>
    {
        bool VerifyMessage(TVerify input);
    }
}