using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Fitness_bot.View;

public class MessageSender
{
    public async void SendTrainerMenu(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken, InlineKeyboardMarkup keyboardMarkup)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Выберите пункт меню:",
            replyMarkup: keyboardMarkup,
            cancellationToken: cancellationToken);
    }

    public async void SendQuestion(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Здравствуйте! Вы тренер и хотите пользоваться телеграмм-ботом Leo?",
            cancellationToken: cancellationToken,
            replyMarkup: MenuButtons.YesOrNoButtons());
    }

    public async void SendFormStart(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Здравствуйте! Перед тем как начать пользоваться ботом необходимо заполнить небольшую анкету.",
            cancellationToken: cancellationToken);
    }

    public async void SendInputMessage(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken, string text)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            $"Введите {text}:",
            cancellationToken: cancellationToken);
    }

    public async void SendFailureMessage(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken, string text)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            $"Не удалось ввести {text}, попробуйте снова:",
            cancellationToken: cancellationToken);
    }

    public async void SendFormFinish(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Вы успешно прошли анкету, теперь вам открыт основной функционал!",
            replyMarkup: MenuButtons.ClientMenu(),
            cancellationToken: cancellationToken);
    }
    
    public async void SendExpQuestion(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat, "Есть ли у вас опыт в спорте?",
            cancellationToken: cancellationToken);
    }

    public async void SendAddClientMes(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Клиент успешно добавлен в базу данных. Теперь ему необходимо заполнить анкету.",
            cancellationToken: cancellationToken);
    }
    
    public async void SendDeleteClientMes(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Клиент успешно удалён из базы данных.",
            cancellationToken: cancellationToken);
    }

    public async void SendAddWindowMes(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Если вы хотите добавить окно для клиентов, то введите 'окно', иначе введите имя пользователя клиента в Telegram:",
            cancellationToken: cancellationToken);
    }

    public async void SendAddTrainingMes(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            message.Text == "окно" ? "Окно для тренировок успешно добавлено." : "Тренировка успешно добавлена.",
            cancellationToken: cancellationToken);
    }

    public async void SendDeleteTrainingMes(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Тренировка успешно удалена из базы данных.",
            cancellationToken: cancellationToken);
    }

    public async void SendAddOrDeleteTrainingMes(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken, string text)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            $"Чтобы {text} тренировку, введите время проведения в формате dd.MM.yyyy HH:mm:",
            cancellationToken: cancellationToken);
    }

    public async void SendTextMessage(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken, string text)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            text,
            cancellationToken: cancellationToken);
    }

    public async void SendAddOrDeleteClientMes(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken, string text)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            $"Чтобы {text} клиента, введите его имя пользователя в Telegram:",
            cancellationToken: cancellationToken);
    }

    public async void SendTrainerInstructionMes(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Инструкция для тренера (написать)",
            replyMarkup: MenuButtons.TrainerMenu(),
            cancellationToken: cancellationToken);
    }

    public async void SendRejectClientMes(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Извините, тренер пока не добавил вас в список своих клиентов",
            cancellationToken: cancellationToken);
    }
}