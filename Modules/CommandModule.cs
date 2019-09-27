using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace discordvoicebot.Modules
{
    public class CommandModule : ModuleBase<SocketCommandContext> {

        [Command("test")]
        [Summary("Test stuff")]
        public async Task Test()
        {
            await Context.Channel.SendMessageAsync("test success");
        }
    }
}