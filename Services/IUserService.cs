using EmotionalIntelligence.Entities;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EmotionalIntelligence.Services
{
    public interface IUserService
    {
        Task<AppUser> GetOrCreate(Update update);
    }
}
