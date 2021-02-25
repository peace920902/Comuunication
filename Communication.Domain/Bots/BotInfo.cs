namespace Communication.Domain.Bots
{
    public class BotInfo: Entity<string>
    {
        public string Name { get; set; }
        public BotSecret BotSecret { get; set; }
        
    }
}