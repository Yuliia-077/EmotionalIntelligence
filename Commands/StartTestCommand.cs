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
    public class StartTestCommand: BaseCommand
    {
        private readonly IUserService _userService;
        private readonly TelegramBotClient _botClient;
        private readonly DataContext _context;

        public StartTestCommand(IUserService userService, TelegramBot telegramBot, DataContext context)
        {
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            _context = context;
            InFlow = true;
        }

        public override bool InFlow { get; set; }
        public override string Name => CommandNames.StartTestCommand;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);

            var testId = int.Parse(update.CallbackQuery?.Data?.Replace("start_test_", "") ?? "0");

            var userTestExist = _context.UserTests.Any(x => x.TestId == testId && x.UserId == user.Id && x.IsFinished == false);
            var message = new StringBuilder();
            if (!userTestExist)
            {
                var userTest = new UserTest
                {
                    UserId = user.Id,
                    TestId = testId,
                    IsFinished = false
                };

                await _context.UserTests.AddAsync(userTest);
                await _context.SaveChangesAsync();
            }
            else
            {
                message.AppendLine("У вас є незавершений тест, ми його продовжимо.");
            }

            var btn = new KeyboardButton("Завершити або зупинити");
            var btn1 = new KeyboardButton("Продовжити");

            var list = new List<KeyboardButton>();
            list.Add(btn);
            list.Add(btn1);

            var inlineKeyboard = new ReplyKeyboardMarkup(list);
            message.AppendLine("Якщо захочете перервати тест натисніть \"Завершити або зупинити\".");
            await _botClient.SendTextMessageAsync(user.ChatId, message.ToString(),
                ParseMode.Markdown, replyMarkup: inlineKeyboard);
        }
    }
}
