using System.Threading.Tasks;

namespace Communication.Domain.Bots
{
    public interface IBotManager<TBot, TVerify, TReceivedMessage, TSendMessage> where TBot : BaseBot<TVerify, TReceivedMessage, TSendMessage>
    {
        TBot GetBot(string botId);
        Task SetThirdPartyId(string botId, string thirdPartyId);
        Task<TBot> ChangeBotSecret(string botId, BotSecret botSecret);
        Task<TBot> CreateBot(BotInfo botInfo);
    }
}