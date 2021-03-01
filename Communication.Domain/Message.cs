using Communication.Domain.Shared.Messages;
using Communication.Domain.Users;

namespace Communication.Domain
{
    public class Message
    {
        public MessageType MessageType { get; set; }
        public User User { get; set; } 
        public string BotId { get; set; }
        public string Content { get; set; }
    }
}