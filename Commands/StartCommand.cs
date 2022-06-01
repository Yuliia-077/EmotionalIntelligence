using EmotionalIntelligence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EmotionalIntelligence.Commands
{
    public class StartCommand : BaseCommand
    {
        private readonly IUserService _userService;
        private readonly TelegramBotClient _botClient;
        private readonly DataContext _context;

        public StartCommand(IUserService userService, TelegramBot telegramBot, DataContext context)
        {
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            _context = context;
            InFlow = false;
        }

        public override bool InFlow { get; set; }
        public override string Name => CommandNames.StartCommand;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);

            var tests = _context.Tests.OrderBy(x => x.Name).ToList();

            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>(tests.Count());

            foreach (var test in tests)
            {
                buttons.Add(new List<InlineKeyboardButton>(1)
                {
                    InlineKeyboardButton.WithCallbackData(test.Name, $"get_test_{test.Id}")
                });
            };

            var inlineKeyboard = new InlineKeyboardMarkup(buttons);

            await _botClient.SendTextMessageAsync(user.ChatId, "Привіт! Тут можна пройти тест для визначення рівня емоційного інтелекту! ",
                ParseMode.Markdown, replyMarkup: inlineKeyboard);
        }
    }
}