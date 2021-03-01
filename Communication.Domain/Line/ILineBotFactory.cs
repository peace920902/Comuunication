using Communication.Domain.Bots;

namespace Communication.Domain.Line
{
    public interface ILineBotFactory : IBotFactory<LineBot, LineVerifyObject, string, LineSendObject>
    {
    }
}