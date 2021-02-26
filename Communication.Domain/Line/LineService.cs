using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Communication.Domain.Bots;
using Line.Messaging.Webhooks;
using Newtonsoft.Json;

namespace Communication.Domain.Line
{
    public class LineService
    {
        private ConcurrentDictionary<string, LineBot> _bots;
        public LineService()
        {
            _bots = new ConcurrentDictionary<string, LineBot>();
        }
        
        public async Task OnMessageReceivedAsync(LineRequestObject requestObject)
        {
            var botId = (string)requestObject.Content.destination;
            if (string.IsNullOrEmpty(botId))
            {
                //todo log
                return;
            }

            if (!_bots.ContainsKey(botId))
            {
                //todo
                //get no botId bot from bot manger;
                //verify message
                //if one of bot is true
                //set botId and add new bot to _bots
            }
            else
            {
                if (!_bots[botId].VerifyMessage(new LineVerifyObject {AuthToken = requestObject.AuthToken, Content = requestObject.ToString()}))
                {
                    //todo log
                    return;
                }
            }
        }

        public async Task SendMessageAsync()
        {

        }

        private async Task<IEnumerable<Message>> ParseMessage(dynamic content)
        {
            var events = JsonConvert.DeserializeObject(content);
            IEnumerable<WebhookEvent> webHookEvent = WebhookEventParser.ParseEvents(events);
            return default;
        }
    }
}