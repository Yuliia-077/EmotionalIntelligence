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
    public class GetTestInfoCommand: BaseCommand
    {
        private readonly IUserService _userService;
        private readonly TelegramBotClient _botClient;
        private readonly DataContext _context;

        public GetTestInfoCommand(IUserService userService, TelegramBot telegramBot, DataContext context)
        {
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            _context = context;
            InFlow = false;
        }

        public override bool InFlow { get; set; }
        public override string Name => CommandNames.GetTestInfoCommand;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);

            var testId = int.Parse(update.CallbackQuery?.Data?.Replace("get_test_", "") ?? "0");

            var test = _context.Tests.FirstOrDefault(x => x.Id == testId);

            var message = new StringBuilder();
            message.AppendLine(test.Name);
            message.AppendLine();
            message.AppendLine(test.Description);

            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>(1);

            buttons.Add(new List<InlineKeyboardButton>(1)
            {
                InlineKeyboardButton.WithCallbackData("Почати", $"start_test_{test.Id}")
            });

            var inlineKeyboard = new InlineKeyboardMarkup(buttons);

            await _botClient.SendTextMessageAsync(user.ChatId, message.ToString(),
                ParseMode.Markdown, replyMarkup: inlineKeyboard);
        }
    }
}
