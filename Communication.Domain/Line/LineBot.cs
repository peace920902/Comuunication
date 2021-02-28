using System.Collections.Generic;
using System.Threading.Tasks;
using Communication.Domain.Bots;
using Line.Messaging;

namespace Communication.Domain.Line
{
    public class LineBot: BaseBot<LineVerifyObject, string, string>
    {
        private readonly ILineMessagingClient _lineMessagingClient;
        public override bool VerifyMessage(LineVerifyObject input)
        {
            return true;
        }

        public override async Task OnMessageReceivedAsync(string message)
        {
            return;
        }

        public override async Task SendMessageAsync(IEnumerable<string> messages)
        {
            var tasks = new List<Task>();
            foreach (var message in messages)
            {
                tasks.Add(_lineMessagingClient.PushMessageAsync(message, message));
            }
            await Task.WhenAll(tasks);
        }

        public LineBot(ILineMessagingClient lineMessagingClient)
        {
            _lineMessagingClient = lineMessagingClient;
        }
    }
}