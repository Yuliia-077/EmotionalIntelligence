using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EmotionalIntelligence.Services
{
    public interface ICommandExecutor
    {
        Task Execute(Update update);
    }
}
