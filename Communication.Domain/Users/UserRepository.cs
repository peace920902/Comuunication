namespace Communication.Domain.Users
{
    public class UserRepository : FileRepository<string, User>, IUserRepository
    {
        public UserRepository()
        {
            FileName = "User.json";
        }
    }
}