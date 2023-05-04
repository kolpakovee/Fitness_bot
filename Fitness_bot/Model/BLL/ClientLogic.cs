using Fitness_bot.Enums;
using Fitness_bot.Model.DAL;
using Fitness_bot.Model.Domain;
using Fitness_bot.Presenter;
using Fitness_bot.View;
using Telegram.Bot.Types;

namespace Fitness_bot.Model.BLL;

public class ClientLogic
{
    private readonly UnitOfWork _unitOfWork;
    private readonly MessageSender _sender;

    public Dictionary<long, FormStatus> Statuses { get; }
    public Dictionary<long, Client> Clients { get; }
    
    public ClientLogic(MessageSender sender, UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _sender = sender;
        Statuses = new Dictionary<long, FormStatus>();
        Clients = new Dictionary<long, Client>();
    }
    
    public void InputName(Message message)
    {
        Clients[message.Chat.Id].Name = message.Text;
        Statuses[message.Chat.Id] = FormStatus.Surname;
        _sender.SendInputMessage(message.Chat, "фамилию");
    }

    public void InputSurname(Message message)
    {
        Clients[message.Chat.Id].Surname = message.Text;
        Statuses[message.Chat.Id] = FormStatus.DateOfBirth;
        _sender.SendInputMessage(message.Chat, "дату рождения");
    }

    public void InputDateOfBirth(Message message)
    {
        Clients[message.Chat.Id].DateOfBirth = message.Text;
        Statuses[message.Chat.Id] = FormStatus.Goal;
        _sender.SendInputMessage(message.Chat, "цель тренировок");
    }

    public void InputGoal(Message message)
    {
        Clients[message.Chat.Id].Goal = message.Text;
        Statuses[message.Chat.Id] = FormStatus.Weight;
        _sender.SendInputMessage(message.Chat, "вес (в кг)");
    }

    public void InputWeight(Message message)
    {
        if (int.TryParse(message.Text, out int weight))
        {
            Clients[message.Chat.Id].Weight = weight;
            Statuses[message.Chat.Id] = FormStatus.Height;
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
            Statuses[message.Chat.Id] = FormStatus.Contraindications;
            _sender.SendInputMessage(message.Chat, "противопоказания");
        }
        else
            _sender.SendFailureMessage(message.Chat, "рост");
    }

    public void InputContraindications(Message message)
    {
        Clients[message.Chat.Id].Contraindications = message.Text;
        Statuses[message.Chat.Id] = FormStatus.HaveExp;
        _sender.SendExpQuestion(message.Chat);
    }

    public void InputExp(Message message)
    {
        Clients[message.Chat.Id].HaveExp = message.Text;
        Statuses[message.Chat.Id] = FormStatus.Bust;
        _sender.SendInputMessage(message.Chat, "обхват груди (в см)");
    }

    public void InputBust(Message message)
    {
        if (int.TryParse(message.Text, out int bust))
        {
            Clients[message.Chat.Id].Bust = bust;
            Statuses[message.Chat.Id] = FormStatus.Waist;
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
            Statuses[message.Chat.Id] = FormStatus.Stomach;
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
            Statuses[message.Chat.Id] = FormStatus.Hips;
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
            Statuses[message.Chat.Id] = FormStatus.Legs;
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
}