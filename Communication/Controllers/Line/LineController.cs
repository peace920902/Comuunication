using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Communication.Controllers.Line
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineController : Controller
    {
        public LineController()
        {
            
        }
        
        [HttpPost]
        public async Task ReceiveMessage()
        {
            
        }
    }
}