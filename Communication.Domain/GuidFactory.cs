using System;

namespace Communication.Domain
{
    public class GuidFactory:IGuidFactory
    {
        public Guid Create()
        {
            return Guid.NewGuid();
        }

        public string CreateId()
        {
            return Create().ToString().Replace("-", "").Substring(0, 10);
        }
    }
}