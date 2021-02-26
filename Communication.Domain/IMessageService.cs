using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communication.Domain
{
    public interface IMessageService<out TMessage>
    {
        Task ReceiveMessages(Func<TMessage, Task> handleMessageFunc);
    }
}