using EmotionalIntelligence.Entities;
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
    public class GetQuestionCommand : BaseCommand
    {
        private readonly IUserService _userService;
        private readonly TelegramBotClient _botClient;
        private readonly DataContext _context;

        public GetQuestionCommand(IUserService userService, TelegramBot telegramBot, DataContext context)
        {
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            _context = context;
            InFlow = true;
        }

        public override bool InFlow { get; set; }
        public override string Name => CommandNames.GetQuestionCommand;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);

            var userTest = _context.UserTests.Where(x =>x.UserId == user.Id && x.IsFinished == false).OrderByDescending(x => x.CreatedAt).FirstOrDefault();

            var answer = _context.UserTestAnswers.Join(_context.Questions, p => p.QuestionId, c => c.Id, (p, c) => new { UserTestId = p.UserTestId, Order = c.Order, QualifierID = c.Id})
                .Where(x => x.UserTestId == userTest.Id).OrderByDescending(x => x.Order).FirstOrDefault();

            Question question = new Question();

            if(answer == null)
            {
                question = _context.Questions.Where(x => x.TestId == userTest.TestId).OrderBy(x => x.Order).FirstOrDefault();
            }
            else
            {
                question = _context.Questions.Where(x => x.TestId == userTest.TestId && x.Order == (answer.Order + 1)).OrderBy(x => x.Order).FirstOrDefault();
            }

            if(question == null)
            {
                InFlow = false;
            }
            else
            {
                var answers = _context.Answers.Where(x => x.TestId == userTest.TestId).OrderByDescending(x => x.Value).ToList();

                List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>(answers.Count());

                foreach (var item in answers)
                {
                    buttons.Add(new List<InlineKeyboardButton>(1)
                {
                    InlineKeyboardButton.WithCallbackData(item.Description, $"{userTest.Id}_{question.Id}_{item.Id}")
                });
                };

                var inlineKeyboard = new InlineKeyboardMarkup(buttons);

                await _botClient.SendTextMessageAsync(user.ChatId, question.Description,
                    ParseMode.Markdown, replyMarkup: inlineKeyboard);
            }
        }
    }
}