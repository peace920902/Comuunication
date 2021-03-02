using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communication.Domain.Bots
{
    public interface IBotService<in TVerify>
    {
        bool VerifyMessage(TVerify input);
    }
}