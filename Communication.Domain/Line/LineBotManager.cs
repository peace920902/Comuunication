using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private void Initialize()
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

        public LineBot GetBotByThirdPartyBotId(string thirdPartyBotId)
        {
            return _bots.Values.FirstOrDefault(x => x.BotInfo.ThirdPartyId == thirdPartyBotId);
        }

        public async Task SetThirdPartyId(string botId, string thirdPartyId)
        {
            if (!_bots.ContainsKey(botId)) return;
            var botInfo = _bots[botId].BotInfo;
            botInfo.ThirdPartyId = thirdPartyId;
            await _botRepository.UpdateAsync(botInfo.Id, botInfo);
        }

        public async Task<LineBot> ChangeBotSecret(string botId, BotSecret botSecret)
        {
            if (!_bots.ContainsKey(botId)) return null;
            var botInfo = _bots[botId].BotInfo;
            botInfo.BotSecret = botSecret;
            var lineBot = _lineBotFactory.Create(botInfo);
            _bots[botId] = lineBot;
            await _botRepository.UpdateAsync(lineBot.BotInfo.Id, lineBot.BotInfo);
            return lineBot;
        }

        public async Task<LineBot> CreateBot(BotInfo botInfo)
        {
            var lineBot = _lineBotFactory.Create(botInfo);
            await _botRepository.CreateAsync(lineBot.BotInfo);
            _bots.TryAdd(lineBot.BotInfo.Id, lineBot);
            return lineBot;
        }

        public IEnumerable<LineBot> GetNoneThirdPartyIdBots()
        {
            return _bots.Values.Where(x => string.IsNullOrEmpty(x.BotInfo.ThirdPartyId));
        }
    }
}