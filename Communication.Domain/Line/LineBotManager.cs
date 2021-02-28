using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Communication.Domain.Bots;

namespace Communication.Domain.Line
{
    public class LineBotManager: ILineBotManager
    {
        private readonly ILineBotFactory _lineBotFactory;
        private readonly IBotRepository _botRepository;

        private readonly ConcurrentDictionary<string, LineBot> _bots;
        
        public LineBotManager(ILineBotFactory lineBotFactory, IBotRepository botRepository)
        {
            _lineBotFactory = lineBotFactory;
            _botRepository = botRepository;
            _bots = new ConcurrentDictionary<string, LineBot>();
            Initialize();
        }

        public void Initialize()
        {
            var botInfos = _botRepository.GetAll().ToList();
            foreach (var lineBot in botInfos.Select(botInfo => _lineBotFactory.Create(botInfo)).Where(lineBot => lineBot != null))
            {
                _bots.TryAdd(lineBot.BotInfo.Id, lineBot);
            }
        }

        public LineBot GetBot(string botId)
        {
            return _bots.ContainsKey(botId) ? _bots[botId] : null;
        }

        public void SetThirdPartyId(string botId, string thirdPartyId)
        {
            if (!_bots.ContainsKey(botId)) return;
            _bots[botId].BotInfo.ThirdPartyId = thirdPartyId;
        }

        public void ChangeBotSecret(string botId, BotSecret botSecret)
        {
            if (!_bots.ContainsKey(botId)) return;
            var botInfo = _bots[botId].BotInfo;
            botInfo.BotSecret = botSecret;
            var lineBot = _lineBotFactory.Create(botInfo);
            _bots[botId] = lineBot;
        }
    }
}