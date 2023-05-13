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
        _sender.SendInputMessage(message.Chat, "вес \\(в кг\\)");
    }

    public void InputWeight(Message message)
    {
        if (int.TryParse(message.Text, out int weight) && weight is > 0 and < 200)
        {
            if (Clients.ContainsKey(message.Chat.Id))
                Clients[message.Chat.Id].Weight = weight;
            if (Statuses.ContainsKey(message.Chat.Id))
                Statuses[message.Chat.Id] = ClientActionStatus.AddHeight;
            _sender.SendInputMessage(message.Chat, "рост \\(в см\\)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "вес");
    }

    public void InputHeight(Message message)
    {
        if (int.TryParse(message.Text, out int height) && height is > 0 and < 250)
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
        _sender.SendInputMessage(message.Chat, "обхват груди \\(в см\\)");
    }

    public void InputBust(Message message)
    {
        if (int.TryParse(message.Text, out int bust) && bust is > 0 and < 200)
        {
            if (Clients.ContainsKey(message.Chat.Id))
                Clients[message.Chat.Id].Bust = bust;
            if (Statuses.ContainsKey(message.Chat.Id))
                Statuses[message.Chat.Id] = ClientActionStatus.AddWaist;
            _sender.SendInputMessage(message.Chat, "обхват талии \\(в см\\)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват груди");
    }

    public void InputWaist(Message message)
    {
        if (int.TryParse(message.Text, out int waist) && waist is > 0 and < 200)
        {
            if (Clients.ContainsKey(message.Chat.Id))
                Clients[message.Chat.Id].Waist = waist;
            if (Statuses.ContainsKey(message.Chat.Id))
                Statuses[message.Chat.Id] = ClientActionStatus.AddStomach;
            _sender.SendInputMessage(message.Chat, "обхват живота \\(в см\\)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват талии");
    }

    public void InputStomach(Message message)
    {
        if (int.TryParse(message.Text, out int stomach) && stomach is > 0 and < 200)
        {
            if (Clients.ContainsKey(message.Chat.Id))
                Clients[message.Chat.Id].Stomach = stomach;
            if (Statuses.ContainsKey(message.Chat.Id))
                Statuses[message.Chat.Id] = ClientActionStatus.AddHips;
            _sender.SendInputMessage(message.Chat, "обхват бёдер \\(в см\\)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват живота");
    }

    public void InputHips(Message message)
    {
        if (int.TryParse(message.Text, out int hips) && hips is > 0 and < 200)
        {
            if (Clients.ContainsKey(message.Chat.Id))
                Clients[message.Chat.Id].Hips = hips;
            if (Statuses.ContainsKey(message.Chat.Id))
                Statuses[message.Chat.Id] = ClientActionStatus.AddLegs;
            _sender.SendInputMessage(message.Chat, "обхват ноги \\(в см\\)");
        }
        else
            _sender.SendFailureMessage(message.Chat, "обхват бёдер");
    }

    public void InputLegs(Message message)
    {
        if (int.TryParse(message.Text, out int legs) && legs is > 0 and < 200)
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
            .Where(t => t.ClientUsername == message.Chat.Username &&
                        DateTime.Parse(t.Identifier.Split('+')[0]) >= now &&
                        DateTime.Parse(t.Identifier.Split('+')[0]) <= now.AddDays(7) && client.TrainerId == t.TrainerId)
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

        String text = timetable.Length == 0
            ? "Тренировок на ближайшие 7 дней не запланировано :)"
            : timetable.ToString();

        _sender.SendTextMessage(message.Chat, text);
        _sender.SendMenuMessage(message.Chat, MenuButtons.ClientMenu());
    }

    public void StartRecordTraining(Message message)
    {
        DateTime now = DateTime.Now;

        var trainings = _unitOfWork.Trainings
            .GetAll()
            .Where(t => t.ClientUsername == "окно" &&
                        DateTime.Parse(t.Identifier.Split('+')[0]) >= now &&
                        DateTime.Parse(t.Identifier.Split('+')[0]) <= now.AddDays(7))
            .ToList();

        if (trainings.Count == 0)
        {
            _sender.SendTextMessage(message.Chat, "Окон на ближайщую неделю нет :(");
            _sender.SendMenuMessage(message.Chat, MenuButtons.ClientMenu());
            return;
        }

        _sender.SendChooseMenuMessage(message.Chat, MenuButtons.GetButtonsFromListOfTrainings(trainings, "record"),
            "слот из списка доступных");
    }

    public void FinishRecordTraining(Message message, string identifier)
    {
        Training? training = _unitOfWork.Trainings
            .GetAll()
            .FirstOrDefault(t => t.Identifier == identifier);

        if (training != null)
        {
            training.ClientUsername = message.Chat.Username;
            _unitOfWork.SaveChanges();
            _sender.SendAddTrainingMes(message);
            Chat trainerChat = new Chat { Id = training.TrainerId };
            _sender.SendTextMessage(trainerChat,
                $"Клиент {message.Chat.Username} записался на тренировку \n{training}");
            _sender.SendMenuMessage(message.Chat, MenuButtons.ClientMenu());
            return;
        }

        _sender.SendTextMessage(message.Chat, "Не удалось записаться на тренировку");
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
            .OrderBy(t => DateTime.Parse(t.Identifier.Split('+')[0]))
            .FirstOrDefault(t => DateTime.Parse(t.Identifier.Split('+')[0]) - DateTime.Now >= TimeSpan.FromHours(3));

        if (training != null)
        {
            _unitOfWork.Trainings.Delete(training);
            _unitOfWork.SaveChanges();

            Chat trainerChat = new Chat { Id = training.TrainerId };
            _sender.SendTextMessage(trainerChat, $"Клиент {message.Chat.Username} отменил тренировку \n{training}");
            _sender.SendDeleteTrainingMes(message.Chat);
            _sender.SendMenuMessage(message.Chat, MenuButtons.ClientMenu());
            return;
        }

        _sender.SendTextMessage(message.Chat, "Невозможно отменить тренировку, напишите тренеру лично");
        _sender.SendMenuMessage(message.Chat, MenuButtons.ClientMenu());
    }

    public void StartEditForm(Message message)
    {
        var client = _unitOfWork.Clients
            .GetAll()
            .FirstOrDefault(cl => cl.Identifier == message.Chat.Username);

        if (client == null) return;

        _sender.SendChooseMenuMessage(message.Chat, MenuButtons.GetButtonsForClientForm(client),
            "параметр, который хотите редактировать");
    }

    public void FinishEditForm(Message message)
    {
        if (message.Text == null) return;

        Client? client = _unitOfWork.Clients.GetAll().FirstOrDefault(c => c.Identifier == message.Chat.Username);

        if (client == null) return;

        if (!Statuses.ContainsKey(message.Chat.Id)) return;

        switch (Statuses[message.Chat.Id])
        {
            case ClientActionStatus.EditGoal:
                client.Goal = message.Text;
                _unitOfWork.SaveChanges();
                _sender.SendTextMessage(message.Chat, "Данные успешно обновлены 👌🏼");
                break;

            case ClientActionStatus.EditWeight:
                if (int.TryParse(message.Text, out int w))
                {
                    client.Weight = w;
                    _unitOfWork.SaveChanges();
                    _sender.SendTextMessage(message.Chat, "Данные успешно обновлены 👌🏼");
                }
                else
                    _sender.SendFailureMessage(message.Chat, "вес");

                break;

            case ClientActionStatus.EditHeight:
                if (int.TryParse(message.Text, out int h))
                {
                    client.Height = h;
                    _unitOfWork.SaveChanges();
                    _sender.SendTextMessage(message.Chat, "Данные успешно обновлены 👌🏼");
                }
                else
                    _sender.SendFailureMessage(message.Chat, "рост");

                break;

            case ClientActionStatus.EditBust:
                if (int.TryParse(message.Text, out int b))
                {
                    client.Bust = b;
                    _unitOfWork.SaveChanges();
                    _sender.SendTextMessage(message.Chat, "Данные успешно обновлены 👌🏼");
                }
                else
                    _sender.SendFailureMessage(message.Chat, "обхват груди");

                break;

            case ClientActionStatus.EditWaist:
                if (int.TryParse(message.Text, out int waist))
                {
                    client.Waist = waist;
                    _unitOfWork.SaveChanges();
                    _sender.SendTextMessage(message.Chat, "Данные успешно обновлены 👌🏼");
                }
                else
                    _sender.SendFailureMessage(message.Chat, "обхват талии");

                break;

            case ClientActionStatus.EditStomach:
                if (int.TryParse(message.Text, out int s))
                {
                    client.Stomach = s;
                    _unitOfWork.SaveChanges();
                    _sender.SendTextMessage(message.Chat, "Данные успешно обновлены 👌🏼");
                }
                else
                    _sender.SendFailureMessage(message.Chat, "обхват живота");

                break;

            case ClientActionStatus.EditHips:
                if (int.TryParse(message.Text, out int hips))
                {
                    client.Hips = hips;
                    _unitOfWork.SaveChanges();
                    _sender.SendTextMessage(message.Chat, "Данные успешно обновлены 👌🏼");
                }
                else
                    _sender.SendFailureMessage(message.Chat, "обхват бёдер");

                break;

            case ClientActionStatus.EditLegs:
                if (int.TryParse(message.Text, out int legs))
                {
                    client.Legs = legs;
                    _unitOfWork.SaveChanges();
                    _sender.SendTextMessage(message.Chat, "Данные успешно обновлены 👌🏼");
                }
                else
                    _sender.SendFailureMessage(message.Chat, "обхват ноги");

                break;
        }
        
        _sender.SendMenuMessage(message.Chat, MenuButtons.ClientMenu());

        Statuses.Remove(message.Chat.Id);
    }

    public void EditForm(Message message, string par)
    {
        switch (par)
        {
            case "height":
                Statuses.Add(message.Chat.Id, ClientActionStatus.EditHeight);
                _sender.SendInputMessage(message.Chat, "новый рост");
                break;

            case "weight":
                Statuses.Add(message.Chat.Id, ClientActionStatus.EditWeight);
                _sender.SendInputMessage(message.Chat, "новый вес");
                break;

            case "bust":
                Statuses.Add(message.Chat.Id, ClientActionStatus.EditBust);
                _sender.SendInputMessage(message.Chat, "новый обхва груди");
                break;

            case "waist":
                Statuses.Add(message.Chat.Id, ClientActionStatus.EditWaist);
                _sender.SendInputMessage(message.Chat, "новый обхва талии");
                break;

            case "stomach":
                Statuses.Add(message.Chat.Id, ClientActionStatus.EditStomach);
                _sender.SendInputMessage(message.Chat, "новый обхват живота");
                break;

            case "hips":
                Statuses.Add(message.Chat.Id, ClientActionStatus.EditHips);
                _sender.SendInputMessage(message.Chat, "новый обхват бёдер");
                break;

            case "legs":
                Statuses.Add(message.Chat.Id, ClientActionStatus.EditLegs);
                _sender.SendInputMessage(message.Chat, "новый обхват ноги");
                break;

            case "goal":
                Statuses.Add(message.Chat.Id, ClientActionStatus.EditGoal);
                _sender.SendInputMessage(message.Chat, "новую цель");
                break;
        }
    }
    
    public void Menu(Message message)
    {
        _sender.SendMenuMessage(message.Chat, MenuButtons.ClientMenu());
    }

    public void RegisterNewClient(Message message)
    {
        _sender.SendFormStart(message.Chat);
        Statuses.Add(message.Chat.Id, ClientActionStatus.AddName);
        Client client = _unitOfWork.Clients
                            .GetAll()
                            .FirstOrDefault(cl => cl.Identifier == message.Chat.Username) ??
                        throw new InvalidOperationException();
        client.Id = message.Chat.Id;
        Clients.Add(message.Chat.Id, client);
    }

    public void SendFirstQuestion(Message message)
    {
        _sender.SendInputMessage(message.Chat, "имя");
    }
}