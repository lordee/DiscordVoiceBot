using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace discordvoicebot.Modules
{
    public class CommandModule : ModuleBase<SocketCommandContext> {

        private readonly AudioService _service;

        public CommandModule(AudioService service)
        {
            _service = service;
        }

        [Command("test")]
        [Summary("Test stuff")]
        public async Task Test()
        {
            await Context.Channel.SendMessageAsync("test success");
        }

        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd()
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCmd()
        {
            await _service.LeaveAudio(Context.Guild);
        }
        
        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCmd([Remainder] string song)
        {
            await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
        }

        /*[Command("join", RunMode = RunMode.Async)]
        [Summary("Join voice chan")]
        public async Task Join(IVoiceChannel channel = null)
        {
            // Get the audio channel
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            var audioClient = await channel.ConnectAsync();
        }*/
    }
}