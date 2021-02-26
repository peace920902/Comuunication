using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communication.Domain.Bots
{
    public interface IBotService<in TVerify, in TSendMessage, out TMessage> : IMessageService<TMessage>
    {
        bool VerifyMessage(TVerify input);
        Task<bool> SendMessage(IEnumerable<TSendMessage> messages);
    }
}