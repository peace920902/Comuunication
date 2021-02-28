using System.Collections.Concurrent;
using System.Threading;
using Line.Messaging;

namespace Communication.Domain.Line
{
    public class LineMessagingClientFactory: ILineMessagingClientFactory
    {
        private readonly ConcurrentDictionary<string, LineMessagingClient> _lineMessagingClients;
        public const int MaxCreateError = 5;

        public LineMessagingClientFactory()
        {
            _lineMessagingClients = new ConcurrentDictionary<string, LineMessagingClient>();
        }
        public ILineMessagingClient Create(string channelAccessToken, string url = LineDefine.LineMessagingApiUri)
        {
            if (_lineMessagingClients.ContainsKey(channelAccessToken))
                return _lineMessagingClients[channelAccessToken];
            var lineMessagingClient = new LineMessagingClient(channelAccessToken, url);
            var errorTimes = 0;
            while (!_lineMessagingClients.TryAdd(channelAccessToken, lineMessagingClient))
            {
                if (errorTimes >= MaxCreateError)
                {
                    //todo log
                    return lineMessagingClient;
                }
                Thread.Sleep(500);
                errorTimes++;
            }
            return _lineMessagingClients[channelAccessToken];
        }
    }
}