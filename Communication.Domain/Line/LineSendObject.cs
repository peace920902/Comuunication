using Communication.Domain.Shared.Messages;

namespace Communication.Domain.Line
{
    public class LineSendObject
    {
        public string UserId { get; set; }
        public string Content { get; set; }
        public string ReplyType { get; set; }
        public MessageType ContentType { get; set; }
    }
}