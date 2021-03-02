using System.Collections.Generic;
using System.Threading.Tasks;
using Communication.Domain.Bots;
using Line.Messaging;

namespace Communication.Domain.Line
{
    public class LineBot: BaseBot<LineVerifyObject, string, LineSendObject>, ILineBotService
    {
        private readonly ILineMessagingClient _lineMessagingClient;
        public LineBot(ILineMessagingClient lineMessagingClient)
        {
            _lineMessagingClient = lineMessagingClient;
        }
         
        public override bool VerifyMessage(LineVerifyObject input)
        {
            return true;
        }

        public override async Task OnMessageReceivedAsync(string message)
        {
            return;
        }

        public override async Task SendMessageAsync(IEnumerable<LineSendObject> messages)
        {
            var tasks = new List<Task>();
            foreach (var message in messages)
            {
                tasks.Add(_lineMessagingClient.PushMessageAsync(message.UserId, message.Content));
                //tasks.Add(_lineMessagingClient.PushMessageAsync(message.UserId, message.Content));
            }
            await Task.WhenAll(tasks);
        }

        public async Task<UserProfile> GetUserProfileAsync(string userId)
        {
            return await _lineMessagingClient.GetUserProfileAsync(userId);
        }
    }
}