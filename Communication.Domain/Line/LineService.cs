using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Communication.Domain.Shared;
using Communication.Domain.ThirdPartyService;
using Communication.Domain.Users;
using Line.Messaging.Webhooks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MessageType = Communication.Domain.Shared.Messages.MessageType;

namespace Communication.Domain.Line
{
    public class LineService : BaseThirdPartyService<LineParseObject, LineRequestObject, Message, LineVerifyObject, string, LineSendObject, LineBot, ILineBotManager>, ILineService
    {
        private readonly IMessageHandler _messageHandler;
        private readonly ILogger<LineService> _logger;

        public LineService(ILineBotManager botManager, IMessageHandler messageHandler, ILogger<LineService> logger) : base(botManager)
        {
            _messageHandler = messageHandler;
            _logger = logger;
            _messageHandler.RegisterSendMessageFunc(ChatType.Line, SendMessageAsync);
        }

        public override async Task OnMessageReceivedAsync(LineRequestObject requestObject)
        {
            var content = JsonConvert.DeserializeObject<dynamic>(requestObject.Content.ToString());

            var thirdPartyBotId = (string) content.destination;
            if (string.IsNullOrEmpty(thirdPartyBotId))
            {
                _logger.Log(LogLevel.Error, "Cannot get thirdPartyBotId");
                return;
            }

            var bot = BotManager.GetBotByThirdPartyBotId(thirdPartyBotId);
            var lineVerifyObject = new LineVerifyObject {AuthToken = requestObject.AuthToken, Content = requestObject.Content.ToString()};
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

            var messages = await ParseMessages(new LineParseObject {BotId = bot.BotInfo.Id, Content = content.ToString()});
            await _messageHandler.OnMessageReceivedAsync(messages);
        }

        public override async Task SendMessageAsync(IEnumerable<Message> messages)
        {
            var tasks = new List<Task>();
            var sendDict = new Dictionary<LineBot, List<LineSendObject>>();
            foreach (var messageGroup in messages.GroupBy(x => x.BotId))
            {
                var lineBot = BotManager.GetBot(messageGroup.Key);
                if (lineBot == null)
                {
                    _logger.Log(LogLevel.Error, $"Cannot find bot: Id: {messageGroup.Key}");
                    continue;
                }

                if (!sendDict.ContainsKey(lineBot)) sendDict.Add(lineBot, messageGroup.Select(ParseToLineSendObject).ToList());
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
                    case FollowEvent followEvent:
                        var message = await ParseToMessage(lineParseObject.BotId, followEvent);
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
                MessageType = (MessageType) messageEvent.Message.Type,
                BotId = botId,
                User = new User {ThirdPartyId = messageEvent.Source.UserId, ChatType = ChatType.Line},
                TimeStamp = messageEvent.Timestamp,
                Destination = ChatType.ChatInterface
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

        private async Task<Message> ParseToMessage(string botId, FollowEvent followEvent)
        {
            var lineBot = BotManager.GetBot(botId);
            var userInfo = await lineBot.GetUserProfileAsync(followEvent.Source.UserId);
            if (userInfo == null) return null;

            return new Message
            {
                MessageType = MessageType.Join,
                BotId = botId,
                User = new User
                {
                    Name = userInfo.DisplayName,
                    ThirdPartyId = userInfo.UserId,
                    ChatType = ChatType.Line
                },
                TimeStamp = followEvent.Timestamp,
                Destination = ChatType.ChatInterface
            };
        }

        private static LineSendObject ParseToLineSendObject(Message message)
        {
            return new()
            {
                UserId = message.User.ThirdPartyId,
                Content = message.Content,
                ContentType = message.MessageType
            };
        }
    }
}