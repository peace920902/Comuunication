using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Communication.Application;
using Communication.Application.Chat;
using Communication.Domain;
using Communication.Domain.Bots;
using Communication.Domain.Line;
using Communication.Domain.Users;
using Communication.Hubs;

namespace Communication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSignalR();
            services.AddSingleton<IMessageHandler, MessageHandler>();
            services.AddSingleton<ILineService, LineService>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<ILineMessagingClientFactory, LineMessagingClientFactory>();
            services.AddSingleton<ILineBotFactory, LineBotFactory>();
            services.AddSingleton<IGuidFactory, GuidFactory>();
            services.AddSingleton<IBotRepository, BotRepository>();
            services.AddSingleton<ILineBotManager, LineBotManager>();
            services.AddSingleton<IChatAppService, ChatAppService>();
            //services.AddAutoMapper(typeof(AppMapperProfile));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Communication", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Communication v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapControllers();
            });
        }
    }
}
