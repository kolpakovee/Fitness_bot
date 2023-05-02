using Fitness_bot.Enums;
using Fitness_bot.Model.DAL;
using Fitness_bot.Presenter;
using Fitness_bot.View;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Fitness_bot.Model.BL;

public class TelegramBotModel
{
    private ClientRepository _clientRepository;
    private TrainerRepository _trainerRepository;
    private TrainingRepository _trainingRepository;

    public TelegramBotModel(ClientRepository clientRepository, TrainerRepository trainerRepository, TrainingRepository trainingRepository)
    {
        _clientRepository = clientRepository;
        _trainerRepository = trainerRepository;
        _trainingRepository = trainingRepository;
    }

    public static void InputName(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Users[message.Chat.Id].Name = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Surname;
        MessageSender.SendInputMessage(botClient, message, cancellationToken, "фамилию");
    }
    
    public static void InputSurname(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Users[message.Chat.Id].Surname = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.DateOfBirth;
        MessageSender.SendInputMessage(botClient, message, cancellationToken, "дату рождения");
    }

    public static void InputDateOfBirth(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Users[message.Chat.Id].DateOfBirth = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Goal;
        MessageSender.SendInputMessage(botClient, message, cancellationToken, "цель тренировок");
    }

    public static void InputGoal(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Users[message.Chat.Id].Goal = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Weight;
        MessageSender.SendInputMessage(botClient, message, cancellationToken, "вес (в кг)");
    }
}