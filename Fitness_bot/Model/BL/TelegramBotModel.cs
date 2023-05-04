using System.Text;
using Fitness_bot.Enums;
using Fitness_bot.Model.DAL;
using Fitness_bot.Model.Domain;
using Fitness_bot.Presenter;
using Fitness_bot.View;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Fitness_bot.Model.BL;

public class TelegramBotModel
{
    private readonly UnitOfWork _unitOfWork;
    private readonly MessageSender _sender;

    public TelegramBotModel(MessageSender sender)
    {
        _unitOfWork = new UnitOfWork();
        _sender = sender;
    }

    public void InputName(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Clients[message.Chat.Id].Name = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Surname;
        _sender.SendInputMessage(message.Chat, "фамилию");
    }

    public void InputSurname(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Clients[message.Chat.Id].Surname = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.DateOfBirth;
        _sender.SendInputMessage(message.Chat, "дату рождения");
    }

    public void InputDateOfBirth(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Clients[message.Chat.Id].DateOfBirth = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Goal;
        _sender.SendInputMessage(message.Chat, "цель тренировок");
    }

    public void InputGoal(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Clients[message.Chat.Id].Goal = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Weight;
        _sender.SendInputMessage(message.Chat, "вес (в кг)");
    }

    public void InputWeight(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int weight))
        {
            TelegramBotPresenter.Clients[message.Chat.Id].Weight = weight;
            TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Height;
            _sender.SendInputMessage(message.Chat, "рост (в см)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "вес");
    }

    public void InputHeight(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int height))
        {
            TelegramBotPresenter.Clients[message.Chat.Id].Height = height;
            TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Contraindications;
            _sender.SendInputMessage(message.Chat, "противопоказания");
        }
        else
            _sender.SendFailureMessage(message.Chat, "рост");
    }

    public void InputContraindications(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Clients[message.Chat.Id].Contraindications = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.HaveExp;
        _sender.SendExpQuestion(message.Chat);
    }

    public void InputExp(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Clients[message.Chat.Id].HaveExp = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Bust;
        _sender.SendInputMessage(message.Chat, "обхват груди (в см)");
    }

    public void InputBust(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int bust))
        {
            TelegramBotPresenter.Clients[message.Chat.Id].Bust = bust;
            TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Waist;
            _sender.SendInputMessage(message.Chat, "обхват талии (в см)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват груди");
    }

    public void InputWaist(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int waist))
        {
            TelegramBotPresenter.Clients[message.Chat.Id].Waist = waist;
            TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Stomach;
            _sender.SendInputMessage(message.Chat, "обхват живота (в см)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват талии");
    }

    public void InputStomach(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int stomach))
        {
            TelegramBotPresenter.Clients[message.Chat.Id].Stomach = stomach;
            TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Hips;
            _sender.SendInputMessage(message.Chat, "обхват бёдер (в см)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват живота (в см)");
    }

    public void InputHips(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int hips))
        {
            TelegramBotPresenter.Clients[message.Chat.Id].Hips = hips;
            TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Legs;
            _sender.SendInputMessage(message.Chat, "обхват ноги (в см)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват бёдер");
    }

    public void InputLegs(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int legs))
        {
            TelegramBotPresenter.Clients[message.Chat.Id].Legs = legs;

            _unitOfWork.Clients.Update(TelegramBotPresenter.Clients[message.Chat.Id]);
            _unitOfWork.SaveChanges();

            // Очищаем из памяти, чтобы не засорять
            TelegramBotPresenter.Clients.Remove(message.Chat.Id);
            TelegramBotPresenter.Statuses.Remove(message.Chat.Id);

            _sender.SendFormFinish(message.Chat);
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват ноги");
    }

    public void AddClientUsername(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (message.Text == null) return;

        Client client = new Client(message.Text, message.Chat.Id);

        _unitOfWork.Clients.Add(client);
        _unitOfWork.SaveChanges();

        _sender.SendAddClientMes(message.Chat);

        TelegramBotPresenter.TrainersActions.Remove(message.Chat.Id);
    }

    public void DeleteClientByUsername(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (message.Text == null) return;

        _unitOfWork.Clients.Delete(new Client(message.Text));
        _unitOfWork.SaveChanges();

        _sender.SendDeleteClientMes(message.Chat);

        TelegramBotPresenter.TrainersActions.Remove(message.Chat.Id);
    }

    public void AddTrainingDate(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (DateTime.TryParseExact(message.Text, "dd.MM.yyyy HH:mm", null,
                System.Globalization.DateTimeStyles.None, out DateTime dt))
        {
            Training training = new Training(dt, message.Chat.Id);
            TelegramBotPresenter.Trainings.Add(message.Chat.Id, training);
            TelegramBotPresenter.TrainersActions[message.Chat.Id] = ActionStatus.AddTrainingLocation;

            _sender.SendInputMessage(message.Chat, "адрес места проведения тренировки");
        }
        else
            _sender.SendFailureMessage(message.Chat, "дату");
    }

    public void AddTrainingLocation(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Trainings[message.Chat.Id].Location = message.Text;
        TelegramBotPresenter.TrainersActions[message.Chat.Id] = ActionStatus.AddClientForTraining;

        _sender.SendAddWindowMes(message.Chat);
    }

    public void AddClientForTraining(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Trainings[message.Chat.Id].ClientUsername = message.Text;

        _unitOfWork.Trainings.Add(TelegramBotPresenter.Trainings[message.Chat.Id]);
        _unitOfWork.SaveChanges();

        TelegramBotPresenter.Trainings.Remove(message.Chat.Id);
        TelegramBotPresenter.TrainersActions.Remove(message.Chat.Id);

        _sender.SendAddTrainingMes(message);
    }

    public void DeleteTrainingByTime(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (DateTime.TryParseExact(message.Text, "dd.MM.yyyy HH:mm", null,
                System.Globalization.DateTimeStyles.None, out DateTime time))
        {
            _unitOfWork.Trainings.Delete(new Training(time));
            _unitOfWork.SaveChanges();
            
            TelegramBotPresenter.TrainersActions.Remove(message.Chat.Id);

            _sender.SendDeleteTrainingMes(message.Chat);
        }
        else
            _sender.SendFailureMessage(message.Chat, "дату");
    }

    public void TrainerTimetable(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerTimetableMenu());
    }

    public void TrainerClients(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerClientsMenu());
    }

    public void AddTraining(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendAddOrDeleteTrainingMes(message.Chat, "добавить");
        TelegramBotPresenter.TrainersActions.Add(message.Chat.Id, ActionStatus.AddTrainingDate);
    }

    public void CancelTraining(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendAddOrDeleteTrainingMes(message.Chat, "удалить");
        TelegramBotPresenter.TrainersActions.Add(message.Chat.Id, ActionStatus.DeleteTrainingByTime);
    }

    public void WeekTrainerTimetable(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        var trainings = _unitOfWork.Trainings
            .GetAll()
            .Where(t => t.TrainerId == message.Chat.Id);

        DateTime now = DateTime.Now;

        List<Training> trainingsIn7Days = trainings
            .Where(t => (t.Time >= now) && (t.Time <= now.AddDays(7)) && (t.ClientUsername != "окно"))
            .ToList();

        var groupedTrainings = trainingsIn7Days.GroupBy(t => t.Time.DayOfWeek);

        StringBuilder timetable = new StringBuilder();

        foreach (var group in groupedTrainings)
        {
            timetable.Append(group.Key).Append('\n');

            foreach (var t in group)
                timetable.Append(t).Append("\n\n");
        }

        String text = timetable.Length == 0
            ? "Тренировок на ближайшие 7 дней не запланировано :)"
            : timetable.ToString();

        _sender.SendTextMessage(message.Chat, text);
    }

    public void AddClient(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendAddOrDeleteClientMes(message.Chat, "добавить");
        TelegramBotPresenter.TrainersActions.Add(message.Chat.Id, ActionStatus.AddClientUsername);
    }

    public void DeleteClient(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendAddOrDeleteClientMes(message.Chat, "удалить");
        TelegramBotPresenter.TrainersActions.Add(message.Chat.Id, ActionStatus.DeleteClientByUsername);
    }

    public void CheckBase(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        var clients = _unitOfWork.Clients
            .GetAll()
            .Where(cl => cl.TrainerId == message.Chat.Id)
            .ToList();

        StringBuilder sb = new StringBuilder();

        foreach (var client in clients)
            sb.Append(client).Append("\n\n");

        String text = sb.Length == 0 ? "Пока клиентов в базе нет :)" : sb.ToString();

        _sender.SendTextMessage(message.Chat, text);
    }

    public void TrainerRegistration(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (message.Chat.Username == null) return;

        _unitOfWork.Trainers.Add(new Trainer(message.Chat.Id, message.Chat.Username));
        _unitOfWork.SaveChanges();
        
        _sender.SendTrainerInstructionMes(message.Chat);
    }

    public void RejectNewUser(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendRejectClientMes(message.Chat);
    }

    public void HandleMessageDateBase(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        // Проверяем, тренер ли это
        Trainer? trainer = _unitOfWork.Trainers
            .GetAll()
            .FirstOrDefault(t => t.Id == message.Chat.Id);

        if (trainer != null) // пришёл зарегистрированный тренер
        {
            _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerMenu());
            return;
        }

        if (message.Chat.Username != null)
        {
            Client? client = _unitOfWork.Clients
                .GetAll()
                .FirstOrDefault(cl => cl.Identifier == message.Chat.Username);
            
            // Если в БД нет такого пользователя
            if (client == null)
            {
                _sender.SendQuestion(message.Chat);
                return;
            }

            // Если в БД есть такой пользователь и он не прошёл форму
            if (!client.FinishedForm())
            {
                _sender.SendFormStart(message.Chat);

                TelegramBotPresenter.Statuses.Add(message.Chat.Id, FormStatus.Name);
                client.Id = message.Chat.Id;
                TelegramBotPresenter.Clients.Add(message.Chat.Id, client);

                _sender.SendInputMessage(message.Chat, "имя");
                return;
            }

            _sender.SendMenuMessage(message.Chat, MenuButtons.ClientMenu());
        }
    }
}