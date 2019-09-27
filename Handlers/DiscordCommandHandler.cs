using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using discordvoicebot.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace discordvoicebot.Handlers
{
    public class DiscordCommandHandler : IDiscordCommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly CommandService _commands;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public DiscordCommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider services, ILogger logger, IConfiguration config)
        {
            _client = client;
            _services = services;
            _commands = commandService;
            _logger = logger;
            _config = config;
        }

        public async Task InitializeAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;
            
            var argPos = 0;
            if (msg.HasStringPrefix(_config["CmdPrefix"], ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, msg);
                await TryRunAsBotCommand(context, argPos).ConfigureAwait(false);
            }
        }

        private async Task TryRunAsBotCommand(SocketCommandContext context, int argPos)
        {
            var result = await _commands.ExecuteAsync(context, argPos, _services);

            if(!result.IsSuccess)
            {
                _logger.Log($"Command execution failed. Reason: {result.ErrorReason}.");
            }
        }
    }
}
