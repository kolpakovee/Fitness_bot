using Fitness_bot.Enums;
using Fitness_bot.Model.BL;
using Fitness_bot.Model.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Fitness_bot.Presenter;

public class TelegramBotPresenter
{
    public static readonly Dictionary<long, FormStatus> Statuses = new();
    public static readonly Dictionary<long, Client> Users = new();
    public static readonly Dictionary<long, ActionStatus> TrainersActions = new();
    public static readonly Dictionary<long, Training> Trainings = new();
    
    private readonly TelegramBotModel _model;

    public TelegramBotPresenter(TelegramBotModel telegramBotModel)
    {
        _model = telegramBotModel;
    }

    public Task HandleUpdate(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.CallbackQuery)
            HandleCallbackQuery(botClient, update, cancellationToken);

        // Если получили сообщение
        if (update.Type == UpdateType.Message)
            HandleMessage(botClient, update, cancellationToken);
        
        return Task.CompletedTask;
    }

    private void HandleMessage(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var message = update.Message;

        // Проверили на null, чтобы дальше не было проблем
        if (message == null || message.Text == null) return;

        long id = message.Chat.Id;

        // Проверяем, вводит ли ожидаемые данные клиент
        if (Statuses.ContainsKey(id))
        {
            HandleClientAnswers(botClient, message, cancellationToken);
            return;
        }
        
        // Проверяем, вводит ли ожидаемые данные тренер
        if (TrainersActions.ContainsKey(id))
        {
            HandleTrainerAnswers(botClient, message, cancellationToken);
            return;
        }
        
        _model.HandleMessageDateBase(botClient, message, cancellationToken);
    }

    // В случае получния ошибки
    public Task HandleError(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        return Task.CompletedTask;
    }

    // Опрос для пользователя
    private void HandleClientAnswers(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        switch (Statuses[message.Chat.Id])
        {
            case FormStatus.Name:
                _model.InputName(botClient, message, cancellationToken);
                break;

            case FormStatus.Surname:
                _model.InputSurname(botClient, message, cancellationToken);
                break;

            case FormStatus.DateOfBirth:
                _model.InputDateOfBirth(botClient, message, cancellationToken);
                break;

            case FormStatus.Goal:
                _model.InputGoal(botClient, message, cancellationToken);
                break;

            case FormStatus.Weight:
                _model.InputWeight(botClient, message, cancellationToken);
                break;

            case FormStatus.Height:
                _model.InputHeight(botClient, message, cancellationToken);
                break;

            case FormStatus.Contraindications:
                _model.InputContraindications(botClient, message, cancellationToken);
                break;

            case FormStatus.HaveExp:
                _model.InputExp(botClient, message, cancellationToken);
                break;

            case FormStatus.Bust:
                _model.InputBust(botClient, message, cancellationToken);
                break;

            case FormStatus.Waist:
                _model.InputWaist(botClient, message, cancellationToken);
                break;

            case FormStatus.Stomach:
                _model.InputStomach(botClient, message, cancellationToken);
                break;

            case FormStatus.Hips:
                _model.InputHips(botClient, message, cancellationToken);
                break;

            case FormStatus.Legs:
                _model.InputLegs(botClient, message, cancellationToken);
                break;
        }
    }

    // Обработчик ответов тренера
    private void HandleTrainerAnswers(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        switch (TrainersActions[message.Chat.Id])
        {
            case ActionStatus.AddClientUsername:
                _model.AddClientUsername(botClient, message, cancellationToken);
                break;

            case ActionStatus.DeleteClientByUsername:
                _model.DeleteClientByUsername(botClient, message, cancellationToken);
                break;

            case ActionStatus.AddTrainingDate:
                _model.AddTrainingDate(botClient, message, cancellationToken);
                break;

            case ActionStatus.AddTrainingLocation:
                _model.AddTrainingLocation(botClient, message, cancellationToken);
                break;

            case ActionStatus.AddClientForTraining:
                _model.AddClientForTraining(botClient, message, cancellationToken);
                break;

            case ActionStatus.DeleteTrainingByTime:
                _model.DeleteTrainingByTime(botClient, message, cancellationToken);
                break;
        }
    }

    // Обработчик нажатых кнопок
    private void HandleCallbackQuery(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var queryMessage = update.CallbackQuery?.Message;

        if (queryMessage == null)
            return;

        switch (update.CallbackQuery?.Data)
        {
            case "t_timetable":
                _model.TrainerTimetable(botClient, queryMessage, cancellationToken);
                break;

            case "clients":
                _model.TrainerClients(botClient, queryMessage, cancellationToken);
                break;

            case "add_training":
                _model.AddTraining(botClient, queryMessage, cancellationToken);
                break;

            case "cancel_training":
                _model.CancelTraining(botClient, queryMessage, cancellationToken);
                break;

            case "week_timetable":
                _model.WeekTrainerTimetable(botClient, queryMessage, cancellationToken);
                break;

            case "add_client":
                _model.AddClient(botClient, queryMessage, cancellationToken);
                break;

            case "delete_client":
                _model.DeleteClient(botClient, queryMessage, cancellationToken);
                break;

            case "check_base":
                _model.CheckBase(botClient, queryMessage, cancellationToken);
                break;

            case "cl_timetable":
                break;

            case "cl_form":
                break;

            case "i_am_trainer": // пришёл новый тренер
                _model.TrainerRegistration(botClient, queryMessage, cancellationToken);
                break;

            case "i_am_client":
                _model.RejectNewUser(botClient, queryMessage, cancellationToken);
                break;
        }
    }
}