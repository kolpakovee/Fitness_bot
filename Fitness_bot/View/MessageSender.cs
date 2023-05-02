using Telegram.Bot;
using Telegram.Bot.Types;

namespace Fitness_bot.View;

public static class MessageSender
{
    public static async void SendTrainerMenu(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Выберите пункт меню:",
            replyMarkup: MenuButtons.TrainerMenu(),
            cancellationToken: cancellationToken);
    }

    public static async void SendQuestion(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Здравствуйте! Вы тренер и хотите пользоваться телеграмм-ботом Leo?",
            cancellationToken: cancellationToken,
            replyMarkup: MenuButtons.YesOrNoButtons());
    }

    public static async void SendFormStart(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Здравствуйте! Перед тем как начать пользоваться ботом необходимо заполнить небольшую анкету.",
            cancellationToken: cancellationToken);
    }

    public static async void SendInputMessage(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken, string text)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            $"Введите {text}:",
            cancellationToken: cancellationToken);
    }

    public static async void SendFailureMessage(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken, string text)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            $"Не удалось ввести {text}, попробуйте снова:",
            cancellationToken: cancellationToken);
    }

    public static async void SendFormFinish(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat,
            "Вы успешно прошли анкету, теперь вам открыт основной функционал!",
            replyMarkup: MenuButtons.ClientMenu(),
            cancellationToken: cancellationToken);
    }
}