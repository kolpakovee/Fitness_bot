using System.Text;
using Fitness_bot.Enums;
using Fitness_bot.Model.DAL;
using Fitness_bot.Model.Domain;
using Fitness_bot.View;
using Telegram.Bot.Types;

namespace Fitness_bot.Model.BLL;

public class TrainerLogic
{
    private readonly UnitOfWork _unitOfWork;
    private readonly MessageSender _sender;
    public Dictionary<long, TrainerActionStatus> Statuses { get; }
    private readonly Dictionary<long, Training> _trainings;

    public TrainerLogic(MessageSender sender, UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _sender = sender;
        Statuses = new Dictionary<long, TrainerActionStatus>();
        _trainings = new Dictionary<long, Training>();
    }

    public void AddClientByUsername(Message message)
    {
        if (message.Text == null) return;

        Client client = new Client(message.Text, message.Chat.Id);

        if (!_unitOfWork.Clients.GetAll().Contains(client) && !client.FinishedForm())
        {
            _unitOfWork.Clients.Add(client);
            _unitOfWork.SaveChanges();
            _sender.SendAddClientMes(message.Chat);
        }

        if (Statuses.ContainsKey(message.Chat.Id))
            Statuses.Remove(message.Chat.Id);
        
        _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerMenu());
    }

    public void DeleteClientByUsername(Message message, string identifier)
    {
        if (message.Text == null) return;

        Client? client = _unitOfWork.Clients
            .GetAll()
            .FirstOrDefault(c => c.Identifier == identifier);

        if (client == null) return;

        _unitOfWork.Clients.Delete(client);
        _unitOfWork.SaveChanges();

        _sender.SendDeleteClientMes(message.Chat);

        if (Statuses.ContainsKey(message.Chat.Id))
            Statuses.Remove(message.Chat.Id);
        
        _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerMenu());
    }

    public void AddTrainingDateWithoutTime(Message message, DateTime dt)
    {
        if (dt < DateTime.Now)
        {
            _sender.SendTextMessage(message.Chat, "🌝Невозможность добавить тренировку в прошлое...");
            Menu(message);
            return;
        }
        Training training = new Training(message.Chat.Id)
        {
            Time = dt
        };

        if (!_trainings.ContainsKey(message.Chat.Id))
            _trainings.Add(message.Chat.Id, training);

        _sender.SendChooseMenuMessage(message.Chat, MenuButtons.GetTimeIntervals(), "*время* тренировки");
    }

    public void AddTrainingTime(Message message, DateTime dt)
    {
        if (_trainings.ContainsKey(message.Chat.Id))
        {
            DateTime oldTime = _trainings[message.Chat.Id].Time;
            DateTime newTime = new DateTime(oldTime.Year, oldTime.Month, oldTime.Day, dt.Hour, dt.Minute, 0, 0);
            _trainings[message.Chat.Id].Time = newTime;

            if (!Statuses.ContainsKey(message.Chat.Id))
                Statuses.Add(message.Chat.Id, TrainerActionStatus.AddTrainingLocation);

            _sender.SendInputMessage(message.Chat, "*место* тренировки");
        }
    }

    public void AddTrainingLocation(Message message)
    {
        if (_trainings.ContainsKey(message.Chat.Id))
            _trainings[message.Chat.Id].Location = message.Text;

        List<Client> clients = _unitOfWork.Clients
            .GetAll()
            .Where(cl => cl.TrainerId == message.Chat.Id)
            .ToList();
        
        _sender.SendChooseMenuMessage(message.Chat, MenuButtons.GetButtonsFromListOfClients(clients, "add_for_training"), "*клиента* или *«окно»*, чтобы у клиентов появился слот для записи");

        Statuses.Remove(message.Chat.Id);
    }

    public void AddClientForTraining(Message message, string identifier)
    {
        if (_trainings.ContainsKey(message.Chat.Id))
            _trainings[message.Chat.Id].ClientUsername = identifier;
        else
            return;

        _trainings[message.Chat.Id].Identifier =
            $"{_trainings[message.Chat.Id].Time.ToString("dd.MM.yyyy HH:mm")}+{_trainings[message.Chat.Id].Location}+{message.Chat.Id}";
        _unitOfWork.Trainings.Add(_trainings[message.Chat.Id]);
        _unitOfWork.SaveChanges();

        var client = _unitOfWork.Clients
            .GetAll()
            .FirstOrDefault(cl => cl.Identifier == identifier);

        if (client != null)
        {
            Chat chat = new Chat {Id = client.Id};
            _sender.SendTextMessage(chat, $"💪Ваш тренер добавил новую тренировку {_trainings[message.Chat.Id]}");
        }

        _trainings.Remove(message.Chat.Id);

        _sender.SendAddTrainingMes(message);
        
        _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerMenu());
    }

    public void DeleteTrainingByTime(Message message, String identifier)
    {
        Training? training = _unitOfWork.Trainings
            .GetAll()
            .FirstOrDefault(t => t.Identifier == identifier);

        if (Statuses.ContainsKey(message.Chat.Id))
            Statuses.Remove(message.Chat.Id);

        if (training != null)
        {
            _unitOfWork.Trainings.Delete(training);
            _unitOfWork.SaveChanges();
            _sender.SendDeleteTrainingMes(message.Chat);

            Client? client = _unitOfWork.Clients
                .GetAll()
                .FirstOrDefault(c => c.Identifier == training.ClientUsername);

            if (client != null && client.FinishedForm())
            {
                Chat clientChat = new Chat { Id = client.Id };
                _sender.SendTextMessage(clientChat, $"Ваш тренер {message.Chat.Username} отменил тренировку");
            }

            _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerMenu());
            return;
        }

        _sender.SendTextMessage(message.Chat, "Не удалось отменить тренировку");
    }

    public void Timetable(Message message)
    {
        _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerTimetableMenu());
    }

    public void TrainerClients(Message message)
    {
        _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerClientsMenu());
    }

    public void AddTraining(Message message, DateTime dateTime)
    {
         _sender.SendChooseMenuMessage(message.Chat, MenuButtons.GetCalendarButtons(dateTime), "*дату* проведения тренировки");
    }

    public void CancelTraining(Message message)
    {
        DateTime now = DateTime.Now;

        List<Training> trainings = _unitOfWork.Trainings.GetAll()
            .Where(t => DateTime.Parse(t.Identifier.Split('+')[0]) >= now &&
                        (DateTime.Parse(t.Identifier.Split('+')[0]) <= now.AddDays(7)) &&
                        t.TrainerId == message.Chat.Id)
            .OrderBy(t => DateTime.Parse(t.Identifier.Split('+')[0]))
            .ToList();

        _sender.SendChooseMenuMessage(message.Chat, MenuButtons.GetButtonsFromListOfTrainings(trainings, "delete"),
            "тренировку, которую хотите *отменить*");
        
    }

    public void WeekTrainerTimetable(Message message)
    {
        var trainings = _unitOfWork.Trainings
            .GetAll()
            .Where(t => t.TrainerId == message.Chat.Id);

        DateTime now = DateTime.Now;

        List<Training> trainingsIn7Days = trainings
            .Where(t => DateTime.Parse(t.Identifier.Split('+')[0]) >= now &&
                        (DateTime.Parse(t.Identifier.Split('+')[0]) <= now.AddDays(7)) && (t.ClientUsername != "окно"))
            .OrderBy(t => DateTime.Parse(t.Identifier.Split('+')[0]))
            .ToList();

        var groupedTrainings = trainingsIn7Days.GroupBy(t => DateTime.Parse(t.Identifier.Split('+')[0]).DayOfWeek);

        StringBuilder timetable = new StringBuilder();

        foreach (var group in groupedTrainings)
        {
            timetable.Append(group.Key).Append('\n');

            foreach (var t in group)
                timetable.Append(t).Append("\n\n");
        }

        if (timetable.Length == 0)
            _sender.SendEmptyTimetableMes(message.Chat);
        else
            _sender.SendTextMessage(message.Chat, timetable.ToString());
        _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerMenu());
    }

    public void TrainerRegistration(Message message)
    {
        if (message.Chat.Username == null) return;

        _unitOfWork.Trainers.Add(new Trainer(message.Chat.Id, message.Chat.Username));
        _unitOfWork.SaveChanges();
        
        Menu(message);
    }

    public void CheckBase(Message message)
    {
        var clients = _unitOfWork.Clients
            .GetAll()
            .Where(cl => cl.TrainerId == message.Chat.Id)
            .ToList();
        
        _sender.SendChooseMenuMessage(message.Chat, MenuButtons.GetButtonsFromListOfClients(clients, "view"),
            "клиента для просмотра его анкеты");
    }

    public void CheckClientById(Message message, string identifier)
    {
        Client? client = _unitOfWork.Clients
            .GetAll()
            .FirstOrDefault(c => c.Identifier == identifier);

        if (client == null) return;

        _sender.SendTextMessage(message.Chat, client.ToString());
        _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerMenu());
    }

    public void AddClient(Message message)
    {
        _sender.SendAddOrDeleteClientMes(message.Chat, "добавить");
        if (!Statuses.ContainsKey(message.Chat.Id))
            Statuses.Add(message.Chat.Id, TrainerActionStatus.AddClientUsername);
    }

    public void DeleteClient(Message message)
    {
        var clients = _unitOfWork.Clients
            .GetAll()
            .Where(cl => cl.TrainerId == message.Chat.Id)
            .ToList();
        
        _sender.SendChooseMenuMessage(message.Chat,
            MenuButtons.GetButtonsFromListOfClients(clients, "remove_client"),
            "клиента, которого хотите удалить");
    }

    public void Menu(Message message)
    {
        _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerMenu());
    }
}