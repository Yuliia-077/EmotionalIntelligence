using EmotionalIntelligence.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EmotionalIntelligence.Services
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly List<BaseCommand> _commands;
        private BaseCommand _lastCommand;

        public CommandExecutor(IServiceProvider serviceProvider)
        {
            _commands = serviceProvider.GetServices<BaseCommand>().ToList();
        }

        public async Task Execute(Update update)
        {
            if (update?.Message?.Chat == null && update?.CallbackQuery == null)
                return;

            if (update.Type == UpdateType.Message)
            {
                switch (update.Message?.Text)
                {
                    case "Завершити або зупинити":
                        await ExecuteCommand(CommandNames.FinishOrStopCommand, update);
                        return;
                }
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                if (update.CallbackQuery.Data.Contains("get_test"))
                {
                    await ExecuteCommand(CommandNames.GetTestInfoCommand, update);
                    return;
                }
                if (update.CallbackQuery.Data.Contains("start_test"))
                {
                    await ExecuteCommand(CommandNames.StartTestCommand, update);
                    return;
                }
            }

            if (update.Message != null && update.Message.Text.Contains(CommandNames.StartCommand))
            {
                await ExecuteCommand(CommandNames.StartCommand, update);
                return;
            }

            switch (_lastCommand?.Name)
            {
                    case CommandNames.StartTestCommand:
                    {
                        if(_lastCommand.InFlow)
                        {
                            await ExecuteCommand(CommandNames.GetQuestionCommand, update);
                        }
                        break;
                    }
                    case CommandNames.GetQuestionCommand:
                    {
                        if (_lastCommand.InFlow)
                        {
                            await ExecuteCommand(CommandNames.SetAnswerCommand, update);
                        }
                        else
                        {
                            await ExecuteCommand(CommandNames.FinishOrStopCommand, update);
                        }
                        break;
                    }
                    case CommandNames.SetAnswerCommand:
                    {
                        if (_lastCommand.InFlow)
                        {
                            await ExecuteCommand(CommandNames.GetQuestionCommand, update);
                        }
                        break;
                    }
                    case CommandNames.FinishOrStopCommand:
                    {
                        await ExecuteCommand(CommandNames.StartCommand, update);
                        break;
                    }
                case null:
                    {
                        await ExecuteCommand(CommandNames.StartCommand, update);
                        break;
                    }
            }
        }

        private async Task ExecuteCommand(string commandName, Update update)
        {
            _lastCommand = _commands.First(x => x.Name == commandName);
            await _lastCommand.ExecuteAsync(update);
        }
    }
}
