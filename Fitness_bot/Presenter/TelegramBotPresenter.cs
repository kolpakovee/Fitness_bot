using System.Globalization;
using Fitness_bot.Enums;
using Fitness_bot.Model.BLL;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Fitness_bot.Presenter;

public class TelegramBotPresenter
{
    private readonly TelegramBotLogic _logic;

    public TelegramBotPresenter(TelegramBotLogic telegramBotLogic)
    {
        _logic = telegramBotLogic;
    }

    public Task HandleUpdate(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.CallbackQuery)
            HandleCallbackQuery(update);

        // Если получили сообщение
        if (update.Type == UpdateType.Message)
            HandleMessage(update);

        return Task.CompletedTask;
    }

    private void HandleMessage(Update update)
    {
        var message = update.Message;

        // Проверили на null, чтобы дальше не было проблем
        if (message == null || message.Text == null) return;

        long id = message.Chat.Id;

        // Проверяем, вводит ли ожидаемые данные клиент
        if (_logic.Client.Statuses.ContainsKey(id))
        {
            HandleClientAnswers(message);
            return;
        }

        // Проверяем, вводит ли ожидаемые данные тренер
        if (_logic.Trainer.Statuses.ContainsKey(id))
        {
            HandleTrainerAnswers(message);
            return;
        }

        _logic.UserIdentification(message);
    }

    // В случае получния ошибки
    public Task HandleError(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        return Task.CompletedTask;
    }

    // Опрос для пользователя
    private void HandleClientAnswers(Message message)
    {
        switch (_logic.Client.Statuses[message.Chat.Id])
        {
            case ClientActionStatus.AddName:
                _logic.Client.InputName(message);
                break;

            case ClientActionStatus.AddSurname:
                _logic.Client.InputSurname(message);
                break;

            case ClientActionStatus.AddDateOfBirth:
                _logic.Client.InputDateOfBirth(message);
                break;

            case ClientActionStatus.AddGoal:
                _logic.Client.InputGoal(message);
                break;

            case ClientActionStatus.AddWeight:
                _logic.Client.InputWeight(message);
                break;

            case ClientActionStatus.AddHeight:
                _logic.Client.InputHeight(message);
                break;

            case ClientActionStatus.AddContraindications:
                _logic.Client.InputContraindications(message);
                break;

            case ClientActionStatus.AddExp:
                _logic.Client.InputExp(message);
                break;

            case ClientActionStatus.AddBust:
                _logic.Client.InputBust(message);
                break;

            case ClientActionStatus.AddWaist:
                _logic.Client.InputWaist(message);
                break;

            case ClientActionStatus.AddStomach:
                _logic.Client.InputStomach(message);
                break;

            case ClientActionStatus.AddHips:
                _logic.Client.InputHips(message);
                break;

            case ClientActionStatus.AddLegs:
                _logic.Client.InputLegs(message);
                break;

            case ClientActionStatus.AddTraining:
                _logic.Client.FinishRecordTraining(message);
                break;

            case ClientActionStatus.EditForm:
                _logic.Client.FinishEditForm(message);
                break;
        }
    }

    // Обработчик ответов тренера
    private void HandleTrainerAnswers(Message message)
    {
        switch (_logic.Trainer.Statuses[message.Chat.Id])
        {
            case TrainerActionStatus.AddClientUsername:
                _logic.Trainer.AddClientByUsername(message);
                break;

            case TrainerActionStatus.DeleteClientByUsername:
                _logic.Trainer.DeleteClientByUsername(message);
                break;

            case TrainerActionStatus.AddTrainingDate:
                break;

            case TrainerActionStatus.AddTrainingTime:
                break;

            case TrainerActionStatus.AddTrainingLocation:
                _logic.Trainer.AddTrainingLocation(message);
                break;

            case TrainerActionStatus.AddClientForTraining:
                _logic.Trainer.AddClientForTraining(message);
                break;

            case TrainerActionStatus.DeleteTrainingByTime:
                break;
        }
    }

    // Обработчик нажатых кнопок
    private void HandleCallbackQuery(Update update)
    {
        var queryMessage = update.CallbackQuery?.Message;

        if (queryMessage == null)
            return;

        switch (update.CallbackQuery?.Data)
        {
            case var str when str?.Split('*')[0] == "delete":
                if (!_logic.Trainer.Statuses.ContainsKey(queryMessage.Chat.Id)) return;
                if (_logic.Trainer.Statuses[queryMessage.Chat.Id] == TrainerActionStatus.DeleteTrainingByTime)
                    _logic.Trainer.DeleteTrainingByTime(queryMessage, str.Split('*')[1]);
                break;

            case var s when DateTime.TryParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime date):
                if (!_logic.Trainer.Statuses.ContainsKey(queryMessage.Chat.Id)) return;
                if (_logic.Trainer.Statuses[queryMessage.Chat.Id] == TrainerActionStatus.AddTrainingDate)
                    _logic.Trainer.AddTrainingDateWithoutTime(queryMessage, date);
                break;

            case var s when DateTime.TryParseExact(s, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime time):
                if (!_logic.Trainer.Statuses.ContainsKey(queryMessage.Chat.Id)) return;
                if (_logic.Trainer.Statuses[queryMessage.Chat.Id] == TrainerActionStatus.AddTrainingTime)
                    _logic.Trainer.AddTrainingTime(queryMessage, time);
                break;
            
            case "t_timetable":
                _logic.Trainer.Timetable(queryMessage);
                break;

            case "clients":
                _logic.Trainer.TrainerClients(queryMessage);
                break;

            case "add_training":
                _logic.Trainer.AddTraining(queryMessage);
                break;

            case "cancel_training":
                _logic.Trainer.CancelTraining(queryMessage);
                break;

            case "week_timetable":
                _logic.Trainer.WeekTrainerTimetable(queryMessage);
                break;

            case "add_client":
                _logic.Trainer.AddClient(queryMessage);
                break;

            case "delete_client":
                _logic.Trainer.DeleteClient(queryMessage);
                break;

            case "check_base":
                _logic.Trainer.CheckBase(queryMessage);
                break;

            case "cl_timetable":
                _logic.Client.Timetable(queryMessage);
                break;

            case "cl_trainings":
                _logic.Client.Trainings(queryMessage);
                break;

            case "cl_record":
                _logic.Client.StartRecordTraining(queryMessage);
                break;

            case "cl_cancel":
                _logic.Client.CancelTraining(queryMessage);
                break;

            case "cl_form":
                _logic.Client.StartEditForm(queryMessage);
                break;

            case "i_am_trainer":
                _logic.Trainer.TrainerRegistration(queryMessage);
                break;

            case "i_am_client":
                _logic.RejectNewUser(queryMessage);
                break;
        }
    }
}