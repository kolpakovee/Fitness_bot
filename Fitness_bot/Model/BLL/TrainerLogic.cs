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
    public Dictionary<long, ActionStatus> Statuses { get; }
    private readonly Dictionary<long, Training> _trainings;

    public TrainerLogic(MessageSender sender, UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _sender = sender;
        Statuses = new Dictionary<long, ActionStatus>();
        _trainings = new Dictionary<long, Training>();
    }
    
    public void AddClientByUsername(Message message)
    {
        if (message.Text == null) return;

        Client client = new Client(message.Text, message.Chat.Id);

        _unitOfWork.Clients.Add(client);
        _unitOfWork.SaveChanges();

        _sender.SendAddClientMes(message.Chat);

        Statuses.Remove(message.Chat.Id);
    }

    public void DeleteClientByUsername(Message message)
    {
        if (message.Text == null) return;

        _unitOfWork.Clients.Delete(new Client(message.Text));
        _unitOfWork.SaveChanges();

        _sender.SendDeleteClientMes(message.Chat);

        Statuses.Remove(message.Chat.Id);
    }
    
    public void AddTrainingDate(Message message)
    {
        if (DateTime.TryParseExact(message.Text, "dd.MM.yyyy HH:mm", null,
                System.Globalization.DateTimeStyles.None, out DateTime dt))
        {
            Training training = new Training(dt, message.Chat.Id);
            _trainings.Add(message.Chat.Id, training);
            Statuses[message.Chat.Id] = ActionStatus.AddTrainingLocation;

            _sender.SendInputMessage(message.Chat, "адрес места проведения тренировки");
        }
        else
            _sender.SendFailureMessage(message.Chat, "дату");
    }

    public void AddTrainingLocation(Message message)
    {
        _trainings[message.Chat.Id].Location = message.Text;
        Statuses[message.Chat.Id] = ActionStatus.AddClientForTraining;

        _sender.SendAddWindowMes(message.Chat);
    }
    
    public void AddClientForTraining(Message message)
    {
        _trainings[message.Chat.Id].ClientUsername = message.Text;

        _unitOfWork.Trainings.Add(_trainings[message.Chat.Id]);
        _unitOfWork.SaveChanges();

        _trainings.Remove(message.Chat.Id);
        Statuses.Remove(message.Chat.Id);

        _sender.SendAddTrainingMes(message);
    }

    public void DeleteTrainingByTime(Message message)
    {
        if (DateTime.TryParseExact(message.Text, "dd.MM.yyyy HH:mm", null,
                System.Globalization.DateTimeStyles.None, out DateTime time))
        {
            _unitOfWork.Trainings.Delete(new Training(time));
            _unitOfWork.SaveChanges();
            
            Statuses.Remove(message.Chat.Id);

            _sender.SendDeleteTrainingMes(message.Chat);
        }
        else
            _sender.SendFailureMessage(message.Chat, "дату");
    }

    public void TrainerTimetable(Message message)
    {
        _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerTimetableMenu());
    }

    public void TrainerClients(Message message)
    {
        _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerClientsMenu());
    }
    
    public void AddTraining(Message message)
    {
        _sender.SendAddOrDeleteTrainingMes(message.Chat, "добавить");
        Statuses.Add(message.Chat.Id, ActionStatus.AddTrainingDate);
    }

    public void CancelTraining(Message message)
    {
        _sender.SendAddOrDeleteTrainingMes(message.Chat, "удалить");
        Statuses.Add(message.Chat.Id, ActionStatus.DeleteTrainingByTime);
    }

    public void WeekTrainerTimetable(Message message)
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
    
    public void TrainerRegistration(Message message)
    {
        if (message.Chat.Username == null) return;

        _unitOfWork.Trainers.Add(new Trainer(message.Chat.Id, message.Chat.Username));
        _unitOfWork.SaveChanges();
        
        _sender.SendTrainerInstructionMes(message.Chat);
    }
    
    public void CheckBase(Message message)
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
    
    public void AddClient(Message message)
    {
        _sender.SendAddOrDeleteClientMes(message.Chat, "добавить");
        Statuses.Add(message.Chat.Id, ActionStatus.AddClientUsername);
    }

    public void DeleteClient(Message message)
    {
        _sender.SendAddOrDeleteClientMes(message.Chat, "удалить");
        Statuses.Add(message.Chat.Id, ActionStatus.DeleteClientByUsername);
    }
}