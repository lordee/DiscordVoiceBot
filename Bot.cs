using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

using discordvoicebot.Handlers;
using discordvoicebot.Logging;

namespace discordvoicebot
{
    public class Bot 
    {
        private IConfiguration _configuration;

        public Bot(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Run() {
            var clientConfig = new DiscordSocketConfig {
                MessageCacheSize = 1024
            };
            var client = new DiscordSocketClient(clientConfig);

            using (var services =
                new ServiceCollection()
                    .AddSingleton<DiscordSocketClient>(client)
                    .AddSingleton<CommandService>()
                    .AddSingleton<DiscordCommandHandler>()
                    .AddSingleton<ILogger, ConsoleLogger>()
                    .AddSingleton(_configuration)
                    .BuildServiceProvider()) {

                await services.GetRequiredService<DiscordCommandHandler>().InitializeAsync();

                await client.LoginAsync(TokenType.Bot, _configuration["Tokens:voicebot"]);
                await client.StartAsync();
                await Task.Delay(TimeSpan.FromMilliseconds(-1));
            }
        }
    }
}