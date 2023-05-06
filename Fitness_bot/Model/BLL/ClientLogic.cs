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
        if (Clients.ContainsKey(message.Chat.Id))
            Clients[message.Chat.Id].Name = message.Text;
        if (Statuses.ContainsKey(message.Chat.Id))
            Statuses[message.Chat.Id] = ClientActionStatus.AddSurname;
        _sender.SendInputMessage(message.Chat, "фамилию");
    }

    public void InputSurname(Message message)
    {
        if (Clients.ContainsKey(message.Chat.Id))
            Clients[message.Chat.Id].Surname = message.Text;
        if (Statuses.ContainsKey(message.Chat.Id))
            Statuses[message.Chat.Id] = ClientActionStatus.AddDateOfBirth;
        _sender.SendInputMessage(message.Chat, "дату рождения");
    }

    public void InputDateOfBirth(Message message)
    {
        if (Clients.ContainsKey(message.Chat.Id))
            Clients[message.Chat.Id].DateOfBirth = message.Text;
        if (Statuses.ContainsKey(message.Chat.Id))
            Statuses[message.Chat.Id] = ClientActionStatus.AddGoal;
        _sender.SendInputMessage(message.Chat, "цель тренировок");
    }

    public void InputGoal(Message message)
    {
        if (Clients.ContainsKey(message.Chat.Id))
            Clients[message.Chat.Id].Goal = message.Text;
        if (Statuses.ContainsKey(message.Chat.Id))
            Statuses[message.Chat.Id] = ClientActionStatus.AddWeight;
        _sender.SendInputMessage(message.Chat, "вес (в кг)");
    }

    public void InputWeight(Message message)
    {
        if (int.TryParse(message.Text, out int weight))
        {
            if (Clients.ContainsKey(message.Chat.Id))
                Clients[message.Chat.Id].Weight = weight;
            if (Statuses.ContainsKey(message.Chat.Id))
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
            if (Clients.ContainsKey(message.Chat.Id))
                Clients[message.Chat.Id].Height = height;
            if (Statuses.ContainsKey(message.Chat.Id))
                Statuses[message.Chat.Id] = ClientActionStatus.AddContraindications;
            _sender.SendInputMessage(message.Chat, "противопоказания");
        }
        else
            _sender.SendFailureMessage(message.Chat, "рост");
    }

    public void InputContraindications(Message message)
    {
        if (Clients.ContainsKey(message.Chat.Id))
            Clients[message.Chat.Id].Contraindications = message.Text;
        if (Statuses.ContainsKey(message.Chat.Id))
            Statuses[message.Chat.Id] = ClientActionStatus.AddExp;
        _sender.SendExpQuestion(message.Chat);
    }

    public void InputExp(Message message)
    {
        if (Clients.ContainsKey(message.Chat.Id))
            Clients[message.Chat.Id].HaveExp = message.Text;
        if (Statuses.ContainsKey(message.Chat.Id))
            Statuses[message.Chat.Id] = ClientActionStatus.AddBust;
        _sender.SendInputMessage(message.Chat, "обхват груди (в см)");
    }

    public void InputBust(Message message)
    {
        if (int.TryParse(message.Text, out int bust))
        {
            if (Clients.ContainsKey(message.Chat.Id))
                Clients[message.Chat.Id].Bust = bust;
            if (Statuses.ContainsKey(message.Chat.Id))
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
            if (Clients.ContainsKey(message.Chat.Id))
                Clients[message.Chat.Id].Waist = waist;
            if (Statuses.ContainsKey(message.Chat.Id))
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
            if (Clients.ContainsKey(message.Chat.Id))
                Clients[message.Chat.Id].Stomach = stomach;
            if (Statuses.ContainsKey(message.Chat.Id))
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
            if (Clients.ContainsKey(message.Chat.Id))
                Clients[message.Chat.Id].Hips = hips;
            if (Statuses.ContainsKey(message.Chat.Id))
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
            if (Clients.ContainsKey(message.Chat.Id))
                Clients[message.Chat.Id].Legs = legs;

            _unitOfWork.Clients.Update(Clients[message.Chat.Id]);
            _unitOfWork.SaveChanges();

            // Очищаем из памяти, чтобы не засорять
            Clients.Remove(message.Chat.Id);
            if (Statuses.ContainsKey(message.Chat.Id))
                Statuses.Remove(message.Chat.Id);

            _sender.SendFormFinish(message.Chat);
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват ноги");
    }

    public void Timetable(Message message)
    {
        DateTime now = DateTime.Now;

        Client client = _unitOfWork.Clients.GetAll().FirstOrDefault(c => c.Identifier == message.Chat.Username)!;

        List<Training> trainingsIn7Days = _unitOfWork.Trainings
            .GetAll()
            .Where(t => t.ClientUsername == message.Chat.Username && DateTime.Parse(t.Identifier) >= now &&
                        DateTime.Parse(t.Identifier) <= now.AddDays(7) && client.TrainerId == t.TrainerId)
            .OrderBy(t => DateTime.Parse(t.Identifier))
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
        if (!Statuses.ContainsKey(message.Chat.Id))
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
            if (Statuses.ContainsKey(message.Chat.Id))
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

            if (Statuses.ContainsKey(message.Chat.Id))
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

    public void StartEditForm(Message message)
    {
        var client = _unitOfWork.Clients
            .GetAll()
            .FirstOrDefault(cl => cl.Identifier == message.Chat.Username);

        _sender.SendTextMessage(message.Chat, client!.ToString());

        if (!Statuses.ContainsKey(message.Chat.Id))
        {
            Statuses.Add(message.Chat.Id, ClientActionStatus.EditForm);
            _sender.SendEditFormMes(message.Chat);
        }
        else
            _sender.SendTextMessage(message.Chat, "Невозможно редактировать форму!");
    }

    public void FinishEditForm(Message message)
    {
        if (message.Text == null) return;

        Client? client = _unitOfWork.Clients.GetAll().FirstOrDefault(c => c.Identifier == message.Chat.Username);

        if (client == null) return;

        var arr = message.Text.Split(' ');
        StringBuilder otherStr = new StringBuilder();
        for (int i = 1; i < arr.Length; i++)
            otherStr.Append(arr[i]).Append(' ');

        switch (arr[0])
        {
            case "1":
                client.DateOfBirth = otherStr.ToString();
                _unitOfWork.SaveChanges();
                break;

            case "2":
                client.Goal = otherStr.ToString();
                _unitOfWork.SaveChanges();
                break;

            case "3":
                if (int.TryParse(otherStr.ToString(), out int w))
                {
                    client.Weight = w;
                    _unitOfWork.SaveChanges();
                }
                else
                    _sender.SendFailureMessage(message.Chat, "вес");

                break;

            case "4":
                if (int.TryParse(otherStr.ToString(), out int h))
                {
                    client.Height = h;
                    _unitOfWork.SaveChanges();
                }
                else
                    _sender.SendFailureMessage(message.Chat, "рост");

                break;

            case "5":
                client.Contraindications = otherStr.ToString();
                _unitOfWork.SaveChanges();
                break;

            case "6":
                client.HaveExp = otherStr.ToString();
                _unitOfWork.SaveChanges();
                break;

            case "7":
                if (int.TryParse(otherStr.ToString(), out int b))
                {
                    client.Bust = b;
                    _unitOfWork.SaveChanges();
                }
                else
                    _sender.SendFailureMessage(message.Chat, "обхват груди");

                break;

            case "8":
                if (int.TryParse(otherStr.ToString(), out int waist))
                {
                    client.Waist = waist;
                    _unitOfWork.SaveChanges();
                }
                else
                    _sender.SendFailureMessage(message.Chat, "обхват талии");

                break;

            case "9":
                if (int.TryParse(otherStr.ToString(), out int s))
                {
                    client.Stomach = s;
                    _unitOfWork.SaveChanges();
                }
                else
                    _sender.SendFailureMessage(message.Chat, "обхват живота");

                break;

            case "10":
                if (int.TryParse(otherStr.ToString(), out int hips))
                {
                    client.Hips = hips;
                    _unitOfWork.SaveChanges();
                }
                else
                    _sender.SendFailureMessage(message.Chat, "обхват бёдер");

                break;

            case "11":
                if (int.TryParse(otherStr.ToString(), out int legs))
                {
                    client.Legs = legs;
                    _unitOfWork.SaveChanges();
                }
                else
                    _sender.SendFailureMessage(message.Chat, "обхват ноги");

                break;

            default:
                _sender.SendTextMessage(message.Chat, "Некорректный ввод!");
                return;
        }

        _sender.SendTextMessage(message.Chat, "Данные успешно обновлены");
        if (Statuses.ContainsKey(message.Chat.Id))
            Statuses.Remove(message.Chat.Id);
    }
}