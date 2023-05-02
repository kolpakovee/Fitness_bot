using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Fitness_bot.TelegramBot;

public static class TelegramBotExstention
{
    public static ITelegramBotClient Start(
        this ITelegramBotClient botClient,
        CancellationToken cancellationToken,
        Func<ITelegramBotClient,Update,CancellationToken, Task> updateHandler,
        Func<ITelegramBotClient,Exception,CancellationToken,Task>  pollingErrorHandler
    )
    {
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
        };

        botClient.StartReceiving(
            updateHandler: updateHandler,
            pollingErrorHandler: pollingErrorHandler,
            receiverOptions: receiverOptions,
            cancellationToken: cancellationToken
        );

        return botClient;
    }
}