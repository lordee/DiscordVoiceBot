using System.Threading.Tasks;
using Discord;

namespace discordvoicebot.Logging
{
    public interface ILogger
    {
        void Log(string message);
        Task Log(LogMessage logMessage);
    }
}
