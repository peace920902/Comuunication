namespace Communication.Domain.Bots
{
    public class BotRepository : FileRepository<string, BotInfo>, IBotRepository
    {
        public BotRepository()
        {
            FileName = "Bot.json";
        }
    }
}