using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Fitness_bot.View;

public class MessageSender
{
    private readonly ITelegramBotClient _botClient;
    private readonly CancellationToken _cancellationToken;

    public MessageSender(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        _botClient = botClient;
        _cancellationToken = cancellationToken;
    }

    public async void SendMenuMessage(Chat chat, InlineKeyboardMarkup keyboardMarkup)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Выберите пункт меню:",
            replyMarkup: keyboardMarkup,
            cancellationToken: _cancellationToken);
    }

    public async void SendQuestion(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Здравствуйте! Вы тренер и хотите пользоваться телеграмм-ботом Leo?",
            cancellationToken: _cancellationToken,
            replyMarkup: MenuButtons.YesOrNoButtons());
    }

    public async void SendFormStart(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Здравствуйте! Перед тем как начать пользоваться ботом необходимо заполнить небольшую анкету.",
            cancellationToken: _cancellationToken);
    }

    public async void SendInputMessage(Chat chat, string text)
    {
        await _botClient.SendTextMessageAsync(chat,
            $"Введите {text}:",
            cancellationToken: _cancellationToken);
    }

    public async void SendFailureMessage(Chat chat, string text)
    {
        await _botClient.SendTextMessageAsync(chat,
            $"Не удалось ввести {text}, попробуйте снова:",
            cancellationToken: _cancellationToken);
    }

    public async void SendFormFinish(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Вы успешно прошли анкету, теперь вам открыт основной функционал!",
            replyMarkup: MenuButtons.ClientMenu(),
            cancellationToken: _cancellationToken);
    }
    
    public async void SendExpQuestion(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat, 
            "Есть ли у вас опыт в спорте?",
            cancellationToken: _cancellationToken);
    }

    public async void SendAddClientMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Клиент успешно добавлен в базу данных. Теперь ему необходимо заполнить анкету.",
            cancellationToken: _cancellationToken);
    }
    
    public async void SendDeleteClientMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Клиент успешно удалён из базы данных.",
            cancellationToken: _cancellationToken);
    }

    public async void SendAddWindowMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Если вы хотите добавить окно для клиентов, то введите 'окно', иначе введите имя пользователя клиента в Telegram:",
            cancellationToken: _cancellationToken);
    }

    public async void SendAddTrainingMes(Message message)
    {
        await _botClient.SendTextMessageAsync(message.Chat,
            message.Text == "окно" ? "Окно для тренировок успешно добавлено." : "Тренировка успешно добавлена.",
            cancellationToken: _cancellationToken);
    }

    public async void SendDeleteTrainingMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Тренировка успешно удалена из базы данных.",
            cancellationToken: _cancellationToken);
    }

    public async void SendAddOrDeleteTrainingMes(Chat chat, string text)
    {
        await _botClient.SendTextMessageAsync(chat,
            $"Чтобы {text} тренировку, введите время проведения в формате dd.MM.yyyy HH:mm:",
            cancellationToken: _cancellationToken);
    }

    public async void SendTextMessage(Chat chat, string text)
    {
        await _botClient.SendTextMessageAsync(chat,
            text,
            cancellationToken: _cancellationToken);
    }

    public async void SendAddOrDeleteClientMes(Chat chat, string text)
    {
        await _botClient.SendTextMessageAsync(chat,
            $"Чтобы {text} клиента, введите его имя пользователя в Telegram:",
            cancellationToken: _cancellationToken);
    }

    public async void SendTrainerInstructionMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Инструкция для тренера (написать)",
            replyMarkup: MenuButtons.TrainerMenu(),
            cancellationToken: _cancellationToken);
    }

    public async void SendRejectClientMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Извините, тренер пока не добавил вас в список своих клиентов",
            cancellationToken: _cancellationToken);
    }

    public async void SendImpossibleToDetermineMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Извините, но я не могу определить ваше имя пользователя в Telegram, чтобы идентифицировать :(",
            cancellationToken: _cancellationToken);
    }
}