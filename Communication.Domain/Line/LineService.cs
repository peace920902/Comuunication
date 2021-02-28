using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Line.Messaging.Webhooks;
using Newtonsoft.Json;

namespace Communication.Domain.Line
{
    public class LineService : BaseThirdPartyService<dynamic, LineRequestObject, string, LineVerifyObject, string, string>, ILineService
    {
        public LineService()
        {
        }

        public override async Task OnMessageReceivedAsync(LineRequestObject requestObject)
        {
            await Task.CompletedTask;
            var botId = (string)requestObject.Content.destination;
             if (string.IsNullOrEmpty(botId))
             {
                 //todo log
                 return;
             }
            
             if (!BotServices.ContainsKey(botId))
             {
                 //todo
                 //get no botId bot from bot manger;
                 //verify message
                 //if one of bot is true
                 //set botId and add new bot to _bots
             }
             else
             {
                 if (!BotServices[botId].VerifyMessage(new LineVerifyObject { AuthToken = requestObject.AuthToken, Content = requestObject.ToString() }))
                 {
                     //todo log
                     return;
                 }
             }
        }

        public override async Task SendMessageAsync(IEnumerable<string> messages)
        {
            throw new NotImplementedException();
        }

        protected override async Task<IEnumerable<Message>> ParseMessages(dynamic content)
        {
            var events = JsonConvert.DeserializeObject(content);
            IEnumerable<WebhookEvent> webHookEvent = WebhookEventParser.ParseEvents(events);
            return default;
        }
    }
}