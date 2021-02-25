namespace Communication.Domain.Bots
{
    public record BotSecret
    {
        public string Token { get; init; }
        public string SecretKey { get; init; }
    }
}