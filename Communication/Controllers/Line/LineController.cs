using System;
using System.IO;
using System.Threading.Tasks;
using Communication.Domain.Line;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        public async Task ReceiveMessage(dynamic content)
        {
            var header = Request.Headers.TryGetValue("X-Line-Signature", out var authToken);
            Console.WriteLine(authToken);
        }
    }
}