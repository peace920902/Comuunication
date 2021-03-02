using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Communication.Domain.Shared;
using Communication.Domain.Users;
using Line.Messaging;
using Line.Messaging.Webhooks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MessageType = Communication.Domain.Shared.Messages.MessageType;

namespace Communication.Domain.Line
{
    public class LineService : BaseThirdPartyService<LineParseObject, LineRequestObject, Message, LineVerifyObject, string, LineSendObject, LineBot, ILineBotManager>, ILineService
    {
        private readonly ILogger<LineService> _logger;

        public LineService(ILineBotManager botManager, ILogger<LineService> logger) : base(botManager)
        {
            _logger = logger;
        }

        public override async Task OnMessageReceivedAsync(LineRequestObject requestObject)
        {
            var content = JsonConvert.DeserializeObject<dynamic>(requestObject.Content.ToString());
            
            var thirdPartyBotId = (string)content.destination;
            if (string.IsNullOrEmpty(thirdPartyBotId))
            {
                _logger.Log(LogLevel.Error, "Cannot get thirdPartyBotId");
                return;
            }

            var bot = BotManager.GetBotByThirdPartyBotId(thirdPartyBotId);
            var lineVerifyObject = new LineVerifyObject { AuthToken = requestObject.AuthToken, Content = requestObject.Content.ToString() };
            if (bot == null)
            {
                var noneThirdPartyIdBots = BotManager.GetNoneThirdPartyIdBots();
                var flag = false;
                
                foreach (var noneThirdPartyIdBot in noneThirdPartyIdBots)
                {
                    if (!noneThirdPartyIdBot.VerifyMessage(lineVerifyObject)) continue;
                    await BotManager.SetThirdPartyId(noneThirdPartyIdBot.BotInfo.Id, thirdPartyBotId);
                    flag = true;
                    break;
                }
                if (flag)
                {
                    var lineBot = BotManager.GetBotByThirdPartyBotId(thirdPartyBotId);
                    if (lineBot == null)
                    {
                        _logger.Log(LogLevel.Error, $"LineBot should not be null. ThirdPartyBotId: {thirdPartyBotId}");
                        return;
                    }
                    bot = lineBot;
                }
                else
                {
                    _logger.Log(LogLevel.Error, $"No bot can verify message. LineVerifyObject: {JsonConvert.SerializeObject(lineVerifyObject)}");
                    return;
                }
            }
            else
            {
                if (!bot.VerifyMessage(lineVerifyObject))
                {
                    _logger.Log(LogLevel.Error, $"Bot: {JsonConvert.SerializeObject(bot.BotInfo)} cannot verify message. LineVerifyObject: {JsonConvert.SerializeObject(lineVerifyObject)}");
                    return;
                }
            }
            var messages = await ParseMessages(new LineParseObject { BotId = bot.BotInfo.Id, Content = content.ToString() });
            //todo send to signalR
        }

        public override async Task SendMessageAsync(IEnumerable<Message> messages)
        {
            var tasks = new List<Task>();
            var sendDict = new Dictionary<LineBot, List<LineSendObject>>();
            foreach (var messageGroup in messages.GroupBy(x=>x.BotId))
            {
                var lineBot = BotManager.GetBot(messageGroup.Key);
                if (lineBot == null)
                {
                    _logger.Log(LogLevel.Error,$"Cannot find bot: Id: {messageGroup.Key}");
                    continue;
                }
                if(!sendDict.ContainsKey(lineBot)) sendDict.Add(lineBot, messageGroup.Select(ParseToLineSendObject).ToList());
                else
                {
                    sendDict[lineBot] ??= new List<LineSendObject>();
                    sendDict[lineBot].AddRange(messageGroup.Select(ParseToLineSendObject));
                }
            }

            foreach (var (bot, sendObjects) in sendDict)
            {
                tasks.Add(bot.SendMessageAsync(sendObjects));
            }

            await Task.WhenAll(tasks);
        }

        protected override async Task<IEnumerable<Message>> ParseMessages(LineParseObject lineParseObject)
        {
            var webHookEvents = WebhookEventParser.Parse(lineParseObject.Content);
            var messages = new List<Message>();
            foreach (var webHookEvent in webHookEvents)
            {
                switch (webHookEvent)
                {
                    case MessageEvent messageEvent:
                        messages.Add(ParseToMessage(lineParseObject.BotId, messageEvent));
                        break;
                    case JoinEvent joinEvent:
                        var message = await ParseToMessage(lineParseObject.BotId, joinEvent);
                        if (message != null) messages.Add(message);
                        break;
                }
            }
            return messages;
        }

        private Message ParseToMessage(string botId, MessageEvent messageEvent)
        {
            var message = new Message
            {
                MessageType = (MessageType)messageEvent.Message.Type,
                BotId = botId,
                User = new User { ThirdPartyId = messageEvent.Source.UserId, ThirdPartyType = ThirdPartyType.Line }
            };

            message.Content = messageEvent.Message switch
            {
                TextEventMessage textEvent => textEvent.Text,
                MediaEventMessage mediaEvent => mediaEvent.ContentProvider.PreviewImageUrl,
                StickerEventMessage stickerEvent => stickerEvent.PackageId,
                _ => message.Content
            };
            return message;
        }

        private async Task<Message> ParseToMessage(string botId, JoinEvent joinEvent)
        {
            var lineBot = BotManager.GetBot(botId);
            var userInfo = await lineBot.GetUserProfileAsync(joinEvent.Source.UserId);
            if (userInfo == null) return null;

            return new Message
            {
                MessageType = MessageType.Join,
                BotId = botId,
                User = new User
                {
                    Name = userInfo.DisplayName,
                    ThirdPartyId = userInfo.UserId,
                    ThirdPartyType = ThirdPartyType.Line
                }
            };
        }

        private LineSendObject ParseToLineSendObject(Message message)
        {
            return new ()
            {
                UserId = message.User.ThirdPartyId,
                Content = message.Content,
                ContentType = message.MessageType
            };
        }
    }
}