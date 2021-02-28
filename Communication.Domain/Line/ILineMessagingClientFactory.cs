using Line.Messaging;

namespace Communication.Domain.Line
{
    public interface ILineMessagingClientFactory
    {
        public ILineMessagingClient Create(string channelAccessToken, string url = LineDefine.LineMessagingApiUri);
    }
}