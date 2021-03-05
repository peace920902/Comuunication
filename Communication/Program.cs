using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Communication.Domain.Bots;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Communication
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .MinimumLevel.Information()
                    .CreateLogger();
                Log.Logger.Information("Host Start");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception e)
            {
                Log.Logger.Fatal(e.ToString());
                return -1;
            }
            finally
            {
                Log.Logger.Information("Host down");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(service=>
                {
                    service.AddHostedService<InitialService>();
                    service.AddHostedService<HubClient>();
                });
    }
}
