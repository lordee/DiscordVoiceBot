using System.Threading.Tasks;

namespace discordvoicebot.Handlers
{
    public interface ICommandHandler
    {
        Task InitializeAsync();
    }
}