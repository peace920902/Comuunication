using System;

namespace Communication.Domain.Infrastructure
{
    public class GuidFactory:IGuidFactory
    {
        public Guid Create()
        {
            return Guid.NewGuid();
        }

        public string CreateId()
        {
            return Create().ToString().Replace("-", "")[..10];
        }
    }
}