using System.Collections.Generic;
using Communication.Domain.Shared.Messages;

namespace Communication.Application
{
    public record MessageViewModel
    {
        public IEnumerable<MessageDto> Messages { get; init; }
        public long SendTime { get; init; }
    }
}