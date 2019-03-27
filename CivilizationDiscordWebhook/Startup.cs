using Discord.Webhook;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Http;

namespace CivilizationDiscordWebhook
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }
        public GameOptions GameOptions { get; private set; } = new GameOptions();
        public DiscordWebhookOptions WebhookOptions { get; private set; } = new DiscordWebhookOptions();

        public Startup(IConfiguration config)
        {
            Configuration = config;
            config.GetSection("Game").Bind(GameOptions);
            config.GetSection("DiscordWebhook").Bind(WebhookOptions);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GameOptions>(Configuration.GetSection("Game"));
            services.Configure<DiscordWebhookOptions>(Configuration.GetSection("DiscordWebook"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                string data = await new StreamReader(context.Request.Body).ReadToEndAsync().ConfigureAwait(false);
                CivilizationData civData = JsonConvert.DeserializeObject<CivilizationData>(data);
                if (!civData.GameName.Contains(GameOptions.Filter))
                    return;

                if (!GameOptions.UserMap.TryGetValue(civData.PlayerName, out string userId))
                    userId = civData.PlayerName;

                var webhook = new DiscordWebhookClient(WebhookOptions.Id, WebhookOptions.Token);
                await webhook.SendMessageAsync($"{civData.GameName}, Turn #{civData.TurnNumber}: <@{userId}>, it's your turn!");
            });
        }
    }
}
