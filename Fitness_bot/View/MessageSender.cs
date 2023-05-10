using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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

    public async void SendChooseMenuMessage(Chat chat, InlineKeyboardMarkup keyboardMarkup, string text)
    {
        await _botClient.SendTextMessageAsync(chat,
            $"Выберите {text}:",
            replyMarkup: keyboardMarkup,
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    public async void SendQuestion(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Здравствуйте! Вы тренер и хотите пользоваться телеграмм-ботом Leo?",
            cancellationToken: _cancellationToken,
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: MenuButtons.YesOrNoButtons());
    }

    public async void SendFormStart(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Здравствуйте! Перед тем как начать пользоваться ботом необходимо заполнить небольшую анкету.",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    public async void SendInputMessage(Chat chat, string text)
    {
        await _botClient.SendTextMessageAsync(chat,
            $"Введите {text}:",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }
 
    public async void SendFailureMessage(Chat chat, string text)
    {
        await _botClient.SendTextMessageAsync(chat,
            $"Не удалось ввести {text}, попробуйте снова:",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    public async void SendFormFinish(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Вы успешно прошли анкету, теперь вам открыт основной функционал!",
            replyMarkup: MenuButtons.ClientMenu(),
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }
    
    public async void SendExpQuestion(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat, 
            "Есть ли у вас опыт в спорте?",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    public async void SendAddClientMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Клиент успешно добавлен в базу данных. Теперь ему необходимо заполнить анкету.",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }
    
    public async void SendDeleteClientMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Клиент успешно удалён из базы данных.",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    public async void SendAddTrainingMes(Message message)
    {
        await _botClient.SendTextMessageAsync(message.Chat,
            message.Text!.ToLower() == "окно" ? "✅ Окно для тренировок *успешно добавлено*" : "✅ Тренировка *успешно добавлена*",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    public async void SendDeleteTrainingMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "😢 Очень жаль, что тренировка *успешно отменена*",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    public async void SendTextMessage(Chat chat, string text)
    {
        await _botClient.SendTextMessageAsync(chat,
            text,
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }
    
    public async void SendEmptyTimetableMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Тренировок на ближайшие 7 дней *не запланировано* 🙃",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    public async void SendAddOrDeleteClientMes(Chat chat, string text)
    {
        await _botClient.SendTextMessageAsync(chat,
            $"Чтобы {text} клиента, введите его имя пользователя в Telegram:",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    public async void SendTrainerInstructionMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Инстукция для тренера",
            replyMarkup: MenuButtons.TrainerMenu(),
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    public async void SendClientInstructionMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Инструкция для клиента",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    public async void SendRejectClientMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Извините, тренер пока не добавил вас в список своих клиентов",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    public async void SendImpossibleToDetermineMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Извините, но я не могу определить ваше имя пользователя в Telegram, чтобы идентифицировать :(",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }
}