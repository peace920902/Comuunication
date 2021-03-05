using System.Threading.Tasks;

namespace Communication.Domain.Users
{
    public interface IUserRepository: IRepository<string, User>
    {
        Task<User> GetUserByThirdPartyUserId(string thirdPartyUserId);
    }
}