﻿using System;
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
            var task = _lineService.OnMessageReceivedAsync(
                new LineRequestObject
                {
                    AuthToken = Request.Headers.TryGetValue("X-Line-Signature", out var authToken) ? authToken.ToString() : null,
                    Content = content
                });
            return Ok();
        }
    }
}