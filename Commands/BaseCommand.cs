using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EmotionalIntelligence.Commands
{
    public abstract class BaseCommand
    {
        public abstract string Name { get; }
        public abstract bool InFlow { get; set; }
        public abstract Task ExecuteAsync(Update update);
    }
}
