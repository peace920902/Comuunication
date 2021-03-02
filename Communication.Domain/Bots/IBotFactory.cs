namespace Communication.Domain.Bots
{
    public interface IBotFactory<out TBot, TVerify, TReceivedMessage, TSendMessage> where TBot : IBotService<TVerify>,IMessageService<TReceivedMessage, TSendMessage>
    {
        public TBot Create(BotInfo botInfo);
    }
}