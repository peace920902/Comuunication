using System.Collections.Generic;
using System.Threading.Tasks;
using Communication.Domain.Bots;

namespace Communication.Domain.Line
{
    public interface ILineBotManager: IBotManager<LineBot, LineVerifyObject, string, LineSendObject>
    {
        IEnumerable<LineBot> GetNoneThirdPartyIdBots();
        LineBot GetBotByThirdPartyBotId(string thirdPartyBotId);
    }
}