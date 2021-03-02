using System;
using System.IO;
using System.Threading.Tasks;
using Communication.Domain.Line;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Communication.Controllers.Line
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineController : Controller
    {
        private readonly ILineService _lineService;

        public LineController(ILineService lineService)
        {
            _lineService = lineService;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveMessage(dynamic content)
        {
            var token = Request.Headers.TryGetValue(LineDefine.LineAuthorizeHeader, out var authToken) ? authToken.ToString() : null;
            await _lineService.OnMessageReceivedAsync(
                new LineRequestObject
                {
                    AuthToken = token,
                    Content = content
                });
                      
            return Ok();
        }
    }
}