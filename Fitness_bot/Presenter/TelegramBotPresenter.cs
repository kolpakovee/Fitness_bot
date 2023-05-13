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
        {
            HandleCallbackQuery(update);
            return Task.CompletedTask;
        }

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

            case ClientActionStatus.EditBust:
            case ClientActionStatus.EditGoal:
            case ClientActionStatus.EditHips:
            case ClientActionStatus.EditLegs:
            case ClientActionStatus.EditStomach:
            case ClientActionStatus.EditWaist:
            case ClientActionStatus.EditWeight:
            case ClientActionStatus.EditHeight:
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

            case TrainerActionStatus.AddTrainingLocation:
                _logic.Trainer.AddTrainingLocation(message);
                break;
        }
    }

    // Обработчик нажатых кнопок
    private void HandleCallbackQuery(Update update)
    {
        var queryMessage = update.CallbackQuery?.Message;

        if (queryMessage == null)
            return;
        
        if (update.CallbackQuery?.Data != "ignore")
        {
            try
            {
                _logic.DeleteMessage(queryMessage);
            }
            catch
            {
                Console.WriteLine("Не удалось удалить сообщение");
            }
        }

        switch (update.CallbackQuery?.Data)
        {
            case "t_timetable":
                _logic.Trainer.Timetable(queryMessage);
                break;

            case "clients":
                _logic.Trainer.TrainerClients(queryMessage);
                break;

            case "add_training":
                _logic.Trainer.AddTraining(queryMessage, DateTime.Now);
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
            
            case "ready":
                _logic.Client.SendFirstQuestion(queryMessage);
                break;

            case var less when less?.Split('*')[0] == "<":
                DateTime.TryParseExact(less.Split('*')[1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime d1);
                _logic.Trainer.AddTraining(queryMessage, d1.AddMonths(-1));
                break;

            case var more when more?.Split('*')[0] == ">":
                DateTime.TryParseExact(more.Split('*')[1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime d2);
                _logic.Trainer.AddTraining(queryMessage, d2.AddMonths(1));
                break;

            case var cancel when cancel?.Split('*')[0] == "cancel":
                if (cancel.Split('*')[1] == "cl")
                    _logic.Client.Menu(queryMessage);
                else
                    _logic.Trainer.Menu(queryMessage);
                break;

            case var str when str?.Split('*')[0] == "delete":
                _logic.Trainer.DeleteTrainingByTime(queryMessage, str.Split('*')[1]);
                break;

            case var s when DateTime.TryParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime date):
                _logic.Trainer.AddTrainingDateWithoutTime(queryMessage, date);
                break;

            case var s when DateTime.TryParseExact(s, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime time):
                _logic.Trainer.AddTrainingTime(queryMessage, time);
                break;

            case var q when q?.Split('*')[0] == "view":
                _logic.Trainer.CheckClientById(queryMessage, q.Split('*')[1]);
                break;

            case var client when client?.Split('*')[0] == "remove_client":
                _logic.Trainer.DeleteClientByUsername(queryMessage, client.Split('*')[1]);
                break;

            case var c when c?.Split('*')[0] == "add_for_training":
                _logic.Trainer.AddClientForTraining(queryMessage, c.Split('*')[1]);
                break;

            case var command when command?.Split('*')[0] == "record":
                _logic.Client.FinishRecordTraining(queryMessage, command.Split('*')[1]);
                break;

            case var par when par?.Split('*')[0] == "edit":
                _logic.Client.EditForm(queryMessage, par.Split('*')[1]);
                break;
        }
    }
}