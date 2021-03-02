using System.Threading.Tasks;
using Communication.Domain.Bots;
using Line.Messaging;

namespace Communication.Domain.Line
{
    public interface ILineBotService:IBotService<LineVerifyObject>, IMessageService<string,LineSendObject>
    {
        Task<UserProfile> GetUserProfileAsync(string userId);
    }
}