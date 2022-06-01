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
    public class FinishOrStopCommand : BaseCommand
    {
        private readonly IUserService _userService;
        private readonly TelegramBotClient _botClient;
        private readonly DataContext _context;

        public FinishOrStopCommand(IUserService userService, TelegramBot telegramBot, DataContext context)
        {
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            _context = context;
            InFlow = false;
        }

        public override bool InFlow { get; set; }
        public override string Name => CommandNames.FinishOrStopCommand;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);

            var userTest = _context.UserTests.Where(x => x.IsFinished == false).OrderByDescending(x => x.CreatedAt).FirstOrDefault();

            var higherQuestion = _context.UserTestAnswers.Where(x => x.UserTestId == userTest.Id).OrderByDescending(x => x.Question.Order).FirstOrDefault();
            var question = _context.Questions.Where(x => x.TestId == userTest.TestId).OrderByDescending(x => x.Order).FirstOrDefault();
            if(higherQuestion?.QuestionId == question?.Id)
            {
                userTest.IsFinished = true;
                await _context.SaveChangesAsync();

                var result = from uta in _context.UserTestAnswers
                             join a in _context.Answers on uta.AnswerId equals a.Id
                             join sq in _context.ScaleQuestions on uta.QuestionId equals sq.QuestionId
                             join s in _context.SvaleForResults on sq.ScaleForResultId equals s.Id
                             where uta.UserTestId == userTest.Id
                             && s.TestId == userTest.TestId
                             select new { Name = s.Name, Description = s.Description, Value = a.Value };

                var results = result.GroupBy(x => new { x.Name, x.Description }).Select(g => new
                {
                    g.Key.Name,
                    g.Key.Description,
                    Sum = g.Sum(x => x.Value)
                }).ToList();

                var message = new StringBuilder();
                foreach(var r in results)
                {
                    string status;
                    if (r.Sum >= 14)
                        status = "високий";
                    else if (r.Sum <= 7)
                        status = "низькій";
                    else
                        status = "середній";

                    message.AppendLine(r.Name);
                    message.AppendLine();
                    message.AppendLine(r.Description);
                    message.AppendLine();
                    message.AppendLine($"Результат = {r.Sum}, а саме {status}");
                    message.AppendLine("\n");
                }

                await _botClient.SendTextMessageAsync(user.ChatId, message.ToString(),
                    ParseMode.Markdown);
            }
        }
    }
}
