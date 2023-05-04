using System.Diagnostics;
using Fitness_bot.Enums;
using Fitness_bot.Model.DAL;
using Fitness_bot.Model.Domain;
using Fitness_bot.View;
using Telegram.Bot.Types;

namespace Fitness_bot.Model.BLL;

public class TelegramBotLogic
{
    private readonly UnitOfWork _unitOfWork;
    private readonly MessageSender _sender;
    public ClientLogic Client { get; }
    public TrainerLogic Trainer { get; }

    public TelegramBotLogic(MessageSender sender)
    {
        _unitOfWork = new UnitOfWork();
        _sender = sender;
        Client = new ClientLogic(sender, _unitOfWork);
        Trainer = new TrainerLogic(sender, _unitOfWork);
    }

    public void RejectNewUser(Message message)
    {
        _sender.SendRejectClientMes(message.Chat);
    }

    public void UserIdentification(Message message)
    {
        Debug.Assert(message.Chat.Username != null, "message.Chat.Username != null");
        IdentificationStatus status = WhoIsIt(message);

        switch (status)
        {
            case IdentificationStatus.UnregisteredUser:
                _sender.SendQuestion(message.Chat);
                break;

            case IdentificationStatus.FullRegisteredClient:
                _sender.SendMenuMessage(message.Chat, MenuButtons.ClientMenu());
                break;

            case IdentificationStatus.PartRegisteredClient:
                _sender.SendFormStart(message.Chat);
                Client.Statuses.Add(message.Chat.Id, FormStatus.Name);
                Client client = _unitOfWork.Clients
                                    .GetAll()
                                    .FirstOrDefault(cl => cl.Identifier == message.Chat.Username) ??
                                throw new InvalidOperationException();
                client.Id = message.Chat.Id;
                Client.Clients.Add(message.Chat.Id, client);
                _sender.SendInputMessage(message.Chat, "имя");
                break;

            case IdentificationStatus.RegisteredTrainer:
                _sender.SendMenuMessage(message.Chat, MenuButtons.TrainerMenu());
                break;

            case IdentificationStatus.ImpossibleToDetermine:
                _sender.SendImpossibleToDetermineMes(message.Chat);
                break;
        }
    }

    private IdentificationStatus WhoIsIt(Message message)
    {
        if (message.Chat.Username == null) return IdentificationStatus.ImpossibleToDetermine;

        Trainer? trainer = _unitOfWork.Trainers
            .GetAll()
            .FirstOrDefault(t => t.Id == message.Chat.Id);

        if (trainer != null) return IdentificationStatus.RegisteredTrainer;

        Client? client = _unitOfWork.Clients
            .GetAll()
            .FirstOrDefault(cl => cl.Identifier == message.Chat.Username);

        if (client != null)
        {
            if (client.FinishedForm()) return IdentificationStatus.FullRegisteredClient;
            return IdentificationStatus.PartRegisteredClient;
        }

        return IdentificationStatus.UnregisteredUser;
    }
}