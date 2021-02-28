using System.Threading.Tasks;
using Communication.Domain.Bots;

namespace Communication.Domain.Line
{
    public class LineBotFactory : ILineBotFactory
    {
        private readonly ILineMessagingClientFactory _lineMessagingClientFactory;
        private readonly IGuidFactory _guidFactory;

        public LineBotFactory(ILineMessagingClientFactory lineMessagingClientFactory, IGuidFactory guidFactory)
        {
            _lineMessagingClientFactory = lineMessagingClientFactory;
            _guidFactory = guidFactory;
        }

        public LineBot Create(BotInfo botInfo)
        {
            var lineBot = new LineBot(_lineMessagingClientFactory.Create(botInfo.BotSecret.Token));
            lineBot.BotInfo.BotSecret = new BotSecret {SecretKey = botInfo.BotSecret.SecretKey, Token = botInfo.BotSecret.Token};
            lineBot.BotInfo.Name = botInfo.Name;
            lineBot.BotInfo.ThirdPartyId = botInfo.ThirdPartyId;
            lineBot.BotInfo.Id = _guidFactory.CreateId();
            return lineBot;
        }
    }
}