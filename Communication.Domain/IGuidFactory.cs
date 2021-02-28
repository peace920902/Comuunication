using System;

namespace Communication.Domain
{
    public interface IGuidFactory
    {
        Guid Create();
        string CreateId();
    }
}