using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Communication.Application;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Communication
{
    public class HubClient: IHostedService
    {
        private readonly IHostEnvironment _environment;
        private readonly ILogger<HubClient> _logger;
        private HubConnection _connection;

        public HubClient(IHostEnvironment environment,ILogger<HubClient> logger)
        {
            _environment = environment;
            _logger = logger;
            _connection = new HubConnectionBuilder().WithUrl($"http://127.0.0.1:5000/chat").Build();
            _connection.On<MessageViewModel>("GetMessages", handleMsg);
            //_connection.On<string>("GetMessages", handleMsg);
        }

        // private Task handleMsg(string message)
        // {
        //     Console.WriteLine(message);
        //     return Task.CompletedTask;
        // }
        private async Task handleMsg(MessageViewModel message)
        {
            Console.WriteLine(JsonSerializer.Serialize(message));
            await _connection.SendAsync("SendMessages", message);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    await _connection.StartAsync(cancellationToken);
                    
                    break;
                }
                catch
                {
                    await Task.Delay(1000);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _connection.DisposeAsync();
        }
    }
}