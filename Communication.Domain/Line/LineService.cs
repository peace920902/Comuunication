using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Communication.Domain.Bots;
using Line.Messaging.Webhooks;
using Newtonsoft.Json;

namespace Communication.Domain.Line
{
    public class LineService : BaseThirdPartyService<dynamic, LineRequestObject, string, LineVerifyObject, string, string>
    {
        public LineService()
        {
        }

        public override async Task OnMessageReceivedAsync(LineRequestObject requestObject)
        {
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