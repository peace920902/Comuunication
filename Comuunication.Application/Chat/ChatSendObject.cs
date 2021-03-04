using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Communication.Domain;

namespace Communication.Application.Chat
{
    public class ChatSendObject 
    {
        public Func<IEnumerable<Message>,Task> SendFunc { get; set; }
        public IEnumerable<Message> Messages { get; set; }
    }
}