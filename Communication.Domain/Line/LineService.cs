using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Communication.Domain.Shared;
using Communication.Domain.Users;
using Line.Messaging;
using Line.Messaging.Webhooks;
using Newtonsoft.Json;
using MessageType = Communication.Domain.Shared.Messages.MessageType;

namespace Communication.Domain.Line
{
    public class LineService : BaseThirdPartyService<LineParseObject, LineRequestObject, string, LineVerifyObject, string, LineSendObject, LineBot, ILineBotManager>, ILineService
    {
        public LineService(ILineBotManager botManager) : base(botManager)
        {
        }

        public override async Task OnMessageReceivedAsync(LineRequestObject requestObject)
        {
            var content = JsonConvert.DeserializeObject<dynamic>(requestObject.Content.ToString());
            var botId = (string)content.destination;
            await ParseMessages(new LineParseObject { BotId = botId, Content = content.ToString() });
            if (string.IsNullOrEmpty(botId))
            {
                //todo log
                return;
            }

            var lineBot = BotManager.GetBot(botId);
            if (lineBot == null) 
            {
                //todo
                //get no botId bot from bot manger;
                //verify message
                //if one of bot is true
                //set botId and add new bot to _bots
            }
            else
            {
                if (lineBot.VerifyMessage(new LineVerifyObject { AuthToken = requestObject.AuthToken, Content = requestObject.ToString() }))
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

        protected override async Task<IEnumerable<Message>> ParseMessages(LineParseObject lineParseObject)
        {
            IEnumerable<WebhookEvent> webHookEvents = WebhookEventParser.Parse(lineParseObject.Content);
            var messages = new List<Message>();
            foreach (var webHookEvent in webHookEvents)
            {
                if (webHookEvent is MessageEvent messageEvent)
                {
                    messages.Add(ParseMessage(lineParseObject.BotId, messageEvent));
                }
            }
            return default;
        }

        private Message ParseMessage(string botId, MessageEvent messageEvent)
        {
            var message = new Message()
            {
                MessageType = (MessageType)messageEvent.Message.Type,
                BotId = botId,
                User = new User { ThirdPartyId = messageEvent.Source.UserId, ThirdPartyType = ThirdPartyType.Line }
            };

            message.Content = messageEvent.Message switch
            {
                TextEventMessage textEvent => textEvent.Text,
                MediaEventMessage mediaEvent => mediaEvent.ContentProvider.PreviewImageUrl,
                _ => message.Content
            };
            return message;
        }

        private Message ParseMessage(string botId, JoinEvent joinEvent)
        {
            //todo let bot to get user info
            var userInfo = new UserProfile();

            return new Message
            {
                MessageType = MessageType.Join,
                BotId = botId,
                User = new User { Name = userInfo.DisplayName, ThirdPartyId = userInfo.UserId, ThirdPartyType = ThirdPartyType.Line }
            };
        }

       
    }
}