using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using discordvoicebot.Logging;

public class AudioService
{
    private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
    private readonly ILogger _logger;

    public AudioService(ILogger logger)
    {
        _logger = logger;
    }

    public async Task JoinAudio(IGuild guild, IVoiceChannel target)
    {
        IAudioClient client;
        if (ConnectedChannels.TryGetValue(guild.Id, out client))
        {
            return;
        }
        if (target.Guild.Id != guild.Id)
        {
            return;
        }

        var audioClient = await target.ConnectAsync();

        if (ConnectedChannels.TryAdd(guild.Id, audioClient))
        {
            _logger.Log($"Connected to voice on {guild.Name}.");
        }
    }

    public async Task LeaveAudio(IGuild guild)
    {
        IAudioClient client;
        if (ConnectedChannels.TryRemove(guild.Id, out client))
        {
            await client.StopAsync();
            _logger.Log($"Disconnected from voice on {guild.Name}.");
        }
    }
    
    public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
    {
        // Your task: Get a full path to the file if the value of 'path' is only a filename.
        path = Directory.GetCurrentDirectory() + @"/" + path;
        if (!File.Exists(path))
        {
            await channel.SendMessageAsync("File does not exist: " + path);
            return;
        }
        IAudioClient client;
        if (ConnectedChannels.TryGetValue(guild.Id, out client))
        {
            _logger.Log($"Starting playback of {path} in {guild.Name}");
            using (var ffmpeg = CreateProcess(path))
            using (var stream = client.CreatePCMStream(AudioApplication.Music))
            {
                try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                finally { await stream.FlushAsync(); }
            }
        }
    }

    private Process CreateProcess(string path)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true
        });
    }
}