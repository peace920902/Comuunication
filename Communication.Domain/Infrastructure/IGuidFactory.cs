using System;

namespace Communication.Domain.Infrastructure
{
    public interface IGuidFactory
    {
        Guid Create();
        string CreateId();
    }
}