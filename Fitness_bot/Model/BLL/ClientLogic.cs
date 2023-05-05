using System.Text;
using Fitness_bot.Enums;
using Fitness_bot.Model.DAL;
using Fitness_bot.Model.Domain;
using Fitness_bot.View;
using Telegram.Bot.Types;

namespace Fitness_bot.Model.BLL;

public class ClientLogic
{
    private readonly UnitOfWork _unitOfWork;
    private readonly MessageSender _sender;

    public Dictionary<long, ClientActionStatus> Statuses { get; }
    public Dictionary<long, Client> Clients { get; }

    public ClientLogic(MessageSender sender, UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _sender = sender;
        Statuses = new Dictionary<long, ClientActionStatus>();
        Clients = new Dictionary<long, Client>();
    }

    public void InputName(Message message)
    {
        Clients[message.Chat.Id].Name = message.Text;
        Statuses[message.Chat.Id] = ClientActionStatus.AddSurname;
        _sender.SendInputMessage(message.Chat, "фамилию");
    }

    public void InputSurname(Message message)
    {
        Clients[message.Chat.Id].Surname = message.Text;
        Statuses[message.Chat.Id] = ClientActionStatus.AddDateOfBirth;
        _sender.SendInputMessage(message.Chat, "дату рождения");
    }

    public void InputDateOfBirth(Message message)
    {
        Clients[message.Chat.Id].DateOfBirth = message.Text;
        Statuses[message.Chat.Id] = ClientActionStatus.AddGoal;
        _sender.SendInputMessage(message.Chat, "цель тренировок");
    }

    public void InputGoal(Message message)
    {
        Clients[message.Chat.Id].Goal = message.Text;
        Statuses[message.Chat.Id] = ClientActionStatus.AddWeight;
        _sender.SendInputMessage(message.Chat, "вес (в кг)");
    }

    public void InputWeight(Message message)
    {
        if (int.TryParse(message.Text, out int weight))
        {
            Clients[message.Chat.Id].Weight = weight;
            Statuses[message.Chat.Id] = ClientActionStatus.AddHeight;
            _sender.SendInputMessage(message.Chat, "рост (в см)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "вес");
    }

    public void InputHeight(Message message)
    {
        if (int.TryParse(message.Text, out int height))
        {
            Clients[message.Chat.Id].Height = height;
            Statuses[message.Chat.Id] = ClientActionStatus.AddContraindications;
            _sender.SendInputMessage(message.Chat, "противопоказания");
        }
        else
            _sender.SendFailureMessage(message.Chat, "рост");
    }

    public void InputContraindications(Message message)
    {
        Clients[message.Chat.Id].Contraindications = message.Text;
        Statuses[message.Chat.Id] = ClientActionStatus.AddExp;
        _sender.SendExpQuestion(message.Chat);
    }

    public void InputExp(Message message)
    {
        Clients[message.Chat.Id].HaveExp = message.Text;
        Statuses[message.Chat.Id] = ClientActionStatus.AddBust;
        _sender.SendInputMessage(message.Chat, "обхват груди (в см)");
    }

    public void InputBust(Message message)
    {
        if (int.TryParse(message.Text, out int bust))
        {
            Clients[message.Chat.Id].Bust = bust;
            Statuses[message.Chat.Id] = ClientActionStatus.AddWaist;
            _sender.SendInputMessage(message.Chat, "обхват талии (в см)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват груди");
    }

    public void InputWaist(Message message)
    {
        if (int.TryParse(message.Text, out int waist))
        {
            Clients[message.Chat.Id].Waist = waist;
            Statuses[message.Chat.Id] = ClientActionStatus.AddStomach;
            _sender.SendInputMessage(message.Chat, "обхват живота (в см)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват талии");
    }

    public void InputStomach(Message message)
    {
        if (int.TryParse(message.Text, out int stomach))
        {
            Clients[message.Chat.Id].Stomach = stomach;
            Statuses[message.Chat.Id] = ClientActionStatus.AddHips;
            _sender.SendInputMessage(message.Chat, "обхват бёдер (в см)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват живота (в см)");
    }

    public void InputHips(Message message)
    {
        if (int.TryParse(message.Text, out int hips))
        {
            Clients[message.Chat.Id].Hips = hips;
            Statuses[message.Chat.Id] = ClientActionStatus.AddLegs;
            _sender.SendInputMessage(message.Chat, "обхват ноги (в см)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват бёдер");
    }

    public void InputLegs(Message message)
    {
        if (int.TryParse(message.Text, out int legs))
        {
            Clients[message.Chat.Id].Legs = legs;

            _unitOfWork.Clients.Update(Clients[message.Chat.Id]);
            _unitOfWork.SaveChanges();

            // Очищаем из памяти, чтобы не засорять
            Clients.Remove(message.Chat.Id);
            Statuses.Remove(message.Chat.Id);

            _sender.SendFormFinish(message.Chat);
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват ноги");
    }

    public void Timetable(Message message)
    {
        DateTime now = DateTime.Now;

        List<Training> trainingsIn7Days = _unitOfWork.Trainings
            .GetAll()
            .Where(t => t.ClientUsername == message.Chat.Username && DateTime.Parse(t.Identifier) >= now &&
                        DateTime.Parse(t.Identifier) <= now.AddDays(7))
            .ToList();

        var groupedTrainings = trainingsIn7Days.GroupBy(t => DateTime.Parse(t.Identifier).DayOfWeek);

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

    public void StartRecordTraining(Message message)
    {
        Statuses.Add(message.Chat.Id, ClientActionStatus.AddTraining);

        DateTime now = DateTime.Now;

        var trainings = _unitOfWork.Trainings
            .GetAll()
            .Where(t => t.ClientUsername == "окно" &&
                        DateTime.Parse(t.Identifier) >= now &&
                        DateTime.Parse(t.Identifier) <= now.AddDays(7))
            .ToList();

        StringBuilder timetable = new StringBuilder();

        var groupedTrainings = trainings
            .GroupBy(t => DateTime.Parse(t.Identifier).DayOfWeek);

        foreach (var group in groupedTrainings)
        {
            timetable.Append(group.Key).Append('\n');

            foreach (var t in group)
                timetable.Append(t).Append("\n\n");
        }

        if (timetable.Length == 0)
        {
            _sender.SendTextMessage(message.Chat, "Окон на ближайщую неделю нет :(");
            Statuses.Remove(message.Chat.Id);
            return;
        }

        _sender.SendTextMessage(message.Chat, timetable.ToString());
        _sender.SendTextMessage(message.Chat,
            "Выберите окно из доступных (введите время проведения в формате dd.MM.yyyy HH:mm)");
    }

    public void FinishRecordTraining(Message message)
    {
        if (DateTime.TryParseExact(message.Text, "dd.MM.yyyy HH:mm", null,
                System.Globalization.DateTimeStyles.None, out DateTime dt))
        {
            Training? training = _unitOfWork.Trainings
                .GetAll()
                .FirstOrDefault(t => t.Identifier == dt.ToString("dd.MM.yyyy HH:mm"));

            Statuses.Remove(message.Chat.Id);

            if (training != null)
            {
                training.ClientUsername = message.Chat.Username;
                _unitOfWork.SaveChanges();
                _sender.SendAddTrainingMes(message);
                Chat trainerChat = new Chat { Id = training.TrainerId };
                _sender.SendTextMessage(trainerChat,
                    $"Клиент {message.Chat.Username} записался на тренировку \n{training}");
                return;
            }

            _sender.SendTextMessage(message.Chat, "Не удалось записаться на тренировку");
        }
        else
            _sender.SendFailureMessage(message.Chat, "дату");
    }

    public void Trainings(Message message)
    {
        _sender.SendMenuMessage(message.Chat, MenuButtons.ClientTrainingMenu());
    }

    public void CancelTraining(Message message)
    {
        Training? training = _unitOfWork.Trainings
            .GetAll()
            .Where(t => t.ClientUsername == message.Chat.Username)
            .FirstOrDefault(t => DateTime.Parse(t.Identifier) - DateTime.Now >= TimeSpan.FromHours(3));

        if (training != null)
        {
            _unitOfWork.Trainings.Delete(training);
            _unitOfWork.SaveChanges();

            Chat trainerChat = new Chat { Id = training.TrainerId };
            _sender.SendTextMessage(trainerChat, $"Клиент {message.Chat.Username} отменил тренировку \n{training}");
            _sender.SendTextMessage(message.Chat, "Тренировка успешно отменена.");
            return;
        }

        _sender.SendTextMessage(message.Chat, "Невозможно отменить тренировку, напишите тренеру лично");
    }
}