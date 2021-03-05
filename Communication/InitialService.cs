using System;
using System.Threading;
using System.Threading.Tasks;
using Communication.Application.Chat;
using Communication.Domain;
using Communication.Domain.Line;
using Microsoft.Extensions.Hosting;

namespace Communication
{
    public class InitialService: BackgroundService
    {
        private readonly ILineService _lineService;
        private readonly IMessageHandler _messageHandler;
        private readonly IChatAppService _chatAppService;

        public InitialService(ILineService lineService, IMessageHandler messageHandler, IChatAppService chatAppService)
        {
            _lineService = lineService;
            _messageHandler = messageHandler;
            _chatAppService = chatAppService;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Initialized");
        }
    }
}