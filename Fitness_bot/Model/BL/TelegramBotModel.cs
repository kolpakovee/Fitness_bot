using System.Text;
using Fitness_bot.Enums;
using Fitness_bot.Model.DAL;
using Fitness_bot.Model.DAL.Interfaces;
using Fitness_bot.Model.Domain;
using Fitness_bot.Presenter;
using Fitness_bot.View;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Fitness_bot.Model.BL;

public class TelegramBotModel
{
    private readonly IClientRepository _clientRepository;
    private readonly ITrainerRepository _trainerRepository;
    private readonly ITrainingRepository _trainingRepository;
    private readonly MessageSender _sender;

    public TelegramBotModel(MessageSender sender, TelegramBotContext context)
    {
        _clientRepository = new ClientRepository(context);
        _trainerRepository = new TrainerRepository(context);
        _trainingRepository = new TrainingRepository(context);
        _sender = sender;
    }

    public void InputName(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Users[message.Chat.Id].Name = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Surname;
        _sender.SendInputMessage(botClient, message, cancellationToken, "фамилию");
    }

    public void InputSurname(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Users[message.Chat.Id].Surname = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.DateOfBirth;
        _sender.SendInputMessage(botClient, message, cancellationToken, "дату рождения");
    }

    public void InputDateOfBirth(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Users[message.Chat.Id].DateOfBirth = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Goal;
        _sender.SendInputMessage(botClient, message, cancellationToken, "цель тренировок");
    }

    public void InputGoal(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Users[message.Chat.Id].Goal = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Weight;
        _sender.SendInputMessage(botClient, message, cancellationToken, "вес (в кг)");
    }

    public void InputWeight(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int weight))
        {
            TelegramBotPresenter.Users[message.Chat.Id].Weight = weight;
            TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Height;
            _sender.SendInputMessage(botClient, message, cancellationToken, "рост (в см)");
        }
        else
            _sender.SendFailureMessage(botClient, message, cancellationToken, "вес");
    }

    public void InputHeight(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int height))
        {
            TelegramBotPresenter.Users[message.Chat.Id].Height = height;
            TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Contraindications;
            _sender.SendInputMessage(botClient, message, cancellationToken, "противопоказания");
        }
        else
            _sender.SendFailureMessage(botClient, message, cancellationToken, "рост");
    }

    public void InputContraindications(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Users[message.Chat.Id].Contraindications = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.HaveExp;
        _sender.SendExpQuestion(botClient, message, cancellationToken);
    }

    public void InputExp(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Users[message.Chat.Id].HaveExp = message.Text;
        TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Bust;
        _sender.SendInputMessage(botClient, message, cancellationToken, "обхват груди (в см)");
    }

    public void InputBust(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int bust))
        {
            TelegramBotPresenter.Users[message.Chat.Id].Bust = bust;
            TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Waist;
            _sender.SendInputMessage(botClient, message, cancellationToken, "обхват талии (в см)");
        }
        else
            _sender.SendFailureMessage(botClient, message, cancellationToken, "обхват груди");
    }

    public void InputWaist(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int waist))
        {
            TelegramBotPresenter.Users[message.Chat.Id].Waist = waist;
            TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Stomach;
            _sender.SendInputMessage(botClient, message, cancellationToken, "обхват живота (в см)");
        }
        else
            _sender.SendFailureMessage(botClient, message, cancellationToken, "обхват талии");
    }

    public void InputStomach(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int stomach))
        {
            TelegramBotPresenter.Users[message.Chat.Id].Stomach = stomach;
            TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Hips;
            _sender.SendInputMessage(botClient, message, cancellationToken, "обхват бёдер (в см)");
        }
        else
            _sender.SendFailureMessage(botClient, message, cancellationToken, "обхват живота");
    }

    public void InputHips(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int hips))
        {
            TelegramBotPresenter.Users[message.Chat.Id].Hips = hips;
            TelegramBotPresenter.Statuses[message.Chat.Id] = FormStatus.Legs;
            _sender.SendInputMessage(botClient, message, cancellationToken, "обхват ноги (в см)");
        }
        else
            _sender.SendFailureMessage(botClient, message, cancellationToken, "обхват бёдер");
    }

    public void InputLegs(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Text, out int legs))
        {
            TelegramBotPresenter.Users[message.Chat.Id].Legs = legs;

            _clientRepository.UpdateClient(TelegramBotPresenter.Users[message.Chat.Id]);

            // Очищаем из памяти, чтобы не засорять
            TelegramBotPresenter.Users.Remove(message.Chat.Id);
            TelegramBotPresenter.Statuses.Remove(message.Chat.Id);

            _sender.SendFormFinish(botClient, message, cancellationToken);
        }
        else
            _sender.SendFailureMessage(botClient, message, cancellationToken, "обхват ноги");
    }

    public void AddClientUsername(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (message.Text == null) return;

        Client client = new Client(message.Text, message.Chat.Id);

        _clientRepository.AddClient(client);

        _sender.SendAddClientMes(botClient, message, cancellationToken);

        TelegramBotPresenter.TrainersActions.Remove(message.Chat.Id);
    }

    public void DeleteClientByUsername(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (message.Text == null) return;

        _clientRepository.DeleteClientByUsername(message.Text);

        _sender.SendDeleteClientMes(botClient, message, cancellationToken);

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

            _sender.SendInputMessage(botClient, message, cancellationToken, "адрес места проведения тренировки");
        }
        else
            _sender.SendFailureMessage(botClient, message, cancellationToken, "дату");
    }

    public void AddTrainingLocation(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Trainings[message.Chat.Id].Location = message.Text;
        TelegramBotPresenter.TrainersActions[message.Chat.Id] = ActionStatus.AddClientForTraining;

        _sender.SendAddWindowMes(botClient, message, cancellationToken);
    }

    public void AddClientForTraining(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        TelegramBotPresenter.Trainings[message.Chat.Id].ClientUsername = message.Text;

        _trainingRepository.AddTraining(TelegramBotPresenter.Trainings[message.Chat.Id]);

        TelegramBotPresenter.Trainings.Remove(message.Chat.Id);
        TelegramBotPresenter.TrainersActions.Remove(message.Chat.Id);

        _sender.SendAddTrainingMes(botClient, message, cancellationToken);
    }

    public void DeleteTrainingByTime(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (DateTime.TryParseExact(message.Text, "dd.MM.yyyy HH:mm", null,
                System.Globalization.DateTimeStyles.None, out DateTime dateTime))
        {
            _trainingRepository.DeleteTrainingByDateTime(dateTime);
            TelegramBotPresenter.TrainersActions.Remove(message.Chat.Id);

            _sender.SendDeleteTrainingMes(botClient, message, cancellationToken);
        }
        else
            _sender.SendFailureMessage(botClient, message, cancellationToken, "дату");
    }

    public void TrainerTimetable(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendTrainerMenu(botClient, message, cancellationToken, MenuButtons.TrainerTimetableMenu());
    }

    public void TrainerClients(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendTrainerMenu(botClient, message, cancellationToken, MenuButtons.TrainerClientsMenu());
    }

    public void AddTraining(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendAddOrDeleteTrainingMes(botClient, message, cancellationToken, "добавить");
        TelegramBotPresenter.TrainersActions.Add(message.Chat.Id, ActionStatus.AddTrainingDate);
    }

    public void CancelTraining(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendAddOrDeleteTrainingMes(botClient, message, cancellationToken, "удалить");
        TelegramBotPresenter.TrainersActions.Add(message.Chat.Id, ActionStatus.DeleteTrainingByTime);
    }

    public void WeekTrainerTimetable(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        List<Training> trainings = _trainingRepository.GetTrainingsByTrainerId(message.Chat.Id);
        DateTime now = DateTime.Now;

        List<Training> trainingsIn7Days =
            trainings.Where(t => (t.Time >= now) && (t.Time <= now.AddDays(7)) && (t.ClientUsername != "окно"))
                .ToList();

        var groupedTrainings = trainingsIn7Days.GroupBy(t => t.Time.DayOfWeek);

        StringBuilder timetable = new StringBuilder();

        foreach (var group in groupedTrainings)
        {
            timetable.Append(group.Key).Append('\n');

            foreach (var t in group)
                timetable.Append(t).Append("\n\n");
        }

        String text = timetable.Length == 0 ? "Тренировок на ближайшие 7 дней нет :)" : timetable.ToString();
        
        _sender.SendTextMessage(botClient, message, cancellationToken, text);
    }

    public void AddClient(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendAddOrDeleteClientMes(botClient, message, cancellationToken, "добавить");
        TelegramBotPresenter.TrainersActions.Add(message.Chat.Id, ActionStatus.AddClientUsername);
    }

    public void DeleteClient(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendAddOrDeleteClientMes(botClient, message, cancellationToken, "удалить");
        TelegramBotPresenter.TrainersActions.Add(message.Chat.Id, ActionStatus.DeleteClientByUsername);
    }

    public void CheckBase(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        List<Client> users = _clientRepository.GetAllClientsByTrainerId(message.Chat.Id);

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < users.Count; i++)
            sb.Append(i + 1).Append(") ").Append(users[i]).Append("\n\n");

        String text = sb.Length == 0 ? "Пока клиентов в базе нет :)" : sb.ToString();

        _sender.SendTextMessage(botClient, message, cancellationToken, text);
    }

    public void TrainerRegistration(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (message.Chat.Username == null) return;

        _trainerRepository.AddTrainer(new Trainer(message.Chat.Id, message.Chat.Username));
        _sender.SendTrainerInstructionMes(botClient, message, cancellationToken);
    }

    public void RejectNewUser(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _sender.SendRejectClientMes(botClient, message, cancellationToken);
    }

    public void HandleMessageDateBase(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        long id = message.Chat.Id;

        // Проверяем, тренер ли это
        Trainer? trainer = _trainerRepository.GetTrainerById(id);

        if (trainer != null) // пришёл зарегистрированный тренер
        {
            _sender.SendTrainerMenu(botClient, message, cancellationToken, MenuButtons.TrainerMenu());
            return;
        }

        if (message.Chat.Username != null)
        {
            Client? client = _clientRepository.GetClientByUsername(message.Chat.Username);

            // Если в БД нет такого пользователя
            if (client == null)
            {
                _sender.SendQuestion(botClient, message, cancellationToken);
                return;
            }

            // Если в БД есть такой пользователь
            if (!client.FinishedForm())
            {
                _sender.SendFormStart(botClient, message, cancellationToken);

                TelegramBotPresenter.Statuses.Add(message.Chat.Id, FormStatus.Name);
                client.Id = message.Chat.Id;
                TelegramBotPresenter.Users.Add(message.Chat.Id, client);

                _sender.SendInputMessage(botClient, message, cancellationToken, "имя");
            }
        }
    }
}