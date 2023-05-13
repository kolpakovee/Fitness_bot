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
        var a = await _botClient.SendTextMessageAsync(chat,
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
            replyMarkup: MenuButtons.YesOrNoButtons());
    }

    public async void SendFormStart(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Хеллоу 🤪 Перед тем как начать пользоваться 🤖 необходимо заполнить анкету",
            replyMarkup: MenuButtons.ReadyButton(),
            cancellationToken: _cancellationToken);
    }

    public async void SendInputMessage(Chat chat, string text)
    {
        await _botClient.SendTextMessageAsync(chat,
            $"Введите {text}:",
            ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }
 
    public async void SendFailureMessage(Chat chat, string text)
    {
        await _botClient.SendTextMessageAsync(chat,
            $"Не удалось ввести {text} 😬",
            cancellationToken: _cancellationToken);
    }

    public async void SendFormFinish(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Анкета пройдена 🎉 Let’s go",
            replyMarkup: MenuButtons.ClientMenu(),
            cancellationToken: _cancellationToken);
    }
    
    public async void SendExpQuestion(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat, 
            "Есть ли у вас опыт в спорте? 🏋🏼",
            cancellationToken: _cancellationToken);
    }

    public async void SendAddClientMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Клиент добавлен 🥳 Теперь ему необходимо *заполнить анкету*",
            ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }
    
    public async void SendDeleteClientMes(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat,
            "Клиент был удален 🥺💔",
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
            $"Чтобы {text} клиента, введите его имя пользователя в Telegram *\\(без @\\)*:",
            parseMode: ParseMode.MarkdownV2,
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

    public async void DeleteMessageAsync(Message message)
    {
        await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId,  _cancellationToken);
    }
}