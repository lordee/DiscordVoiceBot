using System.Threading.Tasks;

namespace discordvoicebot.Handlers
{
    public interface IDiscordCommandHandler
    {
        Task InitializeAsync();
    }
}