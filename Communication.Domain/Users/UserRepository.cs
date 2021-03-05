using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Communication.Domain.Users
{
    public class UserRepository : FileRepository<string, User>, IUserRepository
    {
        public UserRepository()
        {
            FileName = "User.json";
        }

        public async Task<User> GetUserByThirdPartyUserId(string thirdPartyUserId)
        {
            var data = await GetDataAsync<List<User>>();
            return data?.FirstOrDefault(x => x.ThirdPartyId.Equals(thirdPartyUserId));
        }
    }
}