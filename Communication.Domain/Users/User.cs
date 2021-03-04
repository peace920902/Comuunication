using Communication.Domain.Shared;

namespace Communication.Domain.Users
{
    public class User: Entity<string>
    {
        public string Name { get; set; }
        public ChatType ChatType { get; set; }
        public string ThirdPartyId { get; set; }
    }
}