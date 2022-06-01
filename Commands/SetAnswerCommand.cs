using EmotionalIntelligence.Entities;
using EmotionalIntelligence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EmotionalIntelligence.Commands
{
    public class SetAnswerCommand: BaseCommand
    {
        private readonly IUserService _userService;
        private readonly TelegramBotClient _botClient;
        private readonly DataContext _context;

        public SetAnswerCommand(IUserService userService, TelegramBot telegramBot, DataContext context)
        {
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            _context = context;
            InFlow = true;
        }

        public override bool InFlow { get; set; }
        public override string Name => CommandNames.SetAnswerCommand;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);

            var items = update.CallbackQuery?.Data?.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            if(items!= null)
            {
                var userAns = new UserTestAnswer
                {
                    UserTestId = int.Parse(items[0]),
                    QuestionId = int.Parse(items[1]),
                    AnswerId = int.Parse(items[2]),
                };

                await _context.UserTestAnswers.AddAsync(userAns);
                await _context.SaveChangesAsync();
            }
        }
    }
}
