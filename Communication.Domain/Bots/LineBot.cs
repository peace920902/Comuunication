using System.Collections.Generic;
using System.Threading.Tasks;
using Communication.Domain.Line;

namespace Communication.Domain.Bots
{
    public class LineBot: BaseBot<LineVerifyObject, string, dynamic>
    {
        public override bool VerifyMessage(LineVerifyObject input)
        {
            return true;
        }

        public override Task OnMessageReceivedAsync(string message)
        {
            throw new System.NotImplementedException();
        }

        public override Task SendMessageAsync(IEnumerable<dynamic> messages)
        {
            throw new System.NotImplementedException();
        }

    }
}