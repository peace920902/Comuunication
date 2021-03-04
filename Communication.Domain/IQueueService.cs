using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communication.Domain
{
    public interface IQueueService<T>
    {
        Task<IEnumerable<T>> GetMessages();
        Task SendMessages(IEnumerable<T> messages);
    }
}