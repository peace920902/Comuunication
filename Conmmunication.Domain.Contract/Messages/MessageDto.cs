namespace Communication.Domain.Shared.Messages
{
    public record MessageDto
    {
        public ChatType ChatType { get; init; }
        public string UserId { get; init; }
        public string UserName { get; init; }
        public string BotId { get; init; }
        public MessageType MessageType { get; init; }
        public string Content { get; init; }
    }
}